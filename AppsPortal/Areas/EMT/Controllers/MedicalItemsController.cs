using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.ViewModels;
using AutoMapper;
using EMT_DAL.Model;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.EMT.Controllers
{
    public class MedicalItemsController : EMTBaseController
    {
        #region Medical Items

        public ActionResult Index()
        {
            return View();
        }

        [Route("EMT/MedicalItems/")]
        public ActionResult MedicalItemsIndex()
        {
            if (!CMS.HasAction(Permissions.MedicalItem.Access, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
         
            return View("~/Areas/EMT/Views/MedicalItems/Index.cshtml");
        }

        [Route("EMT/MedicalItemsDataTable/")]
        public JsonResult MedicalItemsDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<MedicalItemsDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<MedicalItemsDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.MedicalItem.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();

            var spResult = DbEMT.spMedicalItem(LAN);
            var All = (from a in DbEMT.spMedicalItem(LAN).AsQueryable()
                       select
                       new MedicalItemsDataTableModel
                       {
                           MedicalItemGUID = a.MedicalItemGUID.ToString(),
                           BrandName = a.BrandName,
                           
                           DoseQuantity = a.DoseQuantity,
                           RemainingItemsQuantity = a.RemainingItemsQuantity,
                           Barcode = a.Barcode,
                           LicenseNoDate = a.LicenseNoDate,
                           //MedicalDoseUnitGUID = a.MedicalDoseUnitGUID.ToString(),
                           MedicalDoseUnitDescription = a.Dose,
                           MedicalGenericNameGUID = a.MedicalGenericNameGUID.ToString(),
                           MedicalGenericNameDescription = a.MedicalGenericNameDescription.Replace("+"," "),
                           MedicalManufacturerGUID = a.MedicalManufacturerGUID.ToString(),
                           MedicalManufacturerDescription = a.MedicalManufacturerDescription,
                           PackingUnit = a.PackingUnit,
                           TotalDispatchedItems = a.TotalDispatchedItems,
                           MedicalPackingUnitGUID = a.MedicalPackingUnitGUID.ToString(),
                           MedicalPackingUnitDescription = a.MedicalPackingUnit,
                           MedicalPharmacologicalFormGUID = a.MedicalPharmacologicalFormGUID.ToString(),
                           MedicalPharmacologicalFormDescription = a.MedicalPharmacologicalForm,
                           MedicalRouteAdministrationGUID = a.MedicalRouteAdministrationGUID.ToString(),
                           MedicalRouteAdministrationDescription = a.MedicalRouteAdministration,
                           MedicalTreatmentGUID = a.MedicalTreatmentGUID.ToString(),
                           MedicalTreatmentDescription = a.MedicalTreatment,
                           SourceGUID = a.SourceGUID.ToString(),
                           Active = a.Active,
                           codeMedicalItemRowVersion = a.codeMedicalItemRowVersion
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<MedicalItemsDataTableModel> Result = Mapper.Map<List<MedicalItemsDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(DbEMT.codeMedicalItem.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("EMT/MedicalItems/Create/")]
        public ActionResult MedicalItemCreate()
        {
            if (!CMS.HasAction(Permissions.MedicalItem.Create, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/EMT/Views/MedicalItems/MedicalItem.cshtml", new MedicalItemUpdateModel());
        }

        [Route("EMT/MedicalItems/Update/{PK}")]
        public ActionResult MedicalItemUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.MedicalItem.Access, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var Audit = DbCMS.spAuditHistory(LAN, PK).OrderBy(x => x.ExecutionTime).FirstOrDefault();
            var model = (from a in DbEMT.codeMedicalItem.WherePK(PK)
                         select new MedicalItemUpdateModel
                         {
                             MedicalItemGUID = a.MedicalItemGUID,
                             Barcode = a.Barcode,
                             RemainingItemsQuantity = a.RemainingItemsQuantity,
                             BrandName = a.BrandName,
                             DoseQuantity = a.DoseQuantity,
                             LicenseNoDate = a.LicenseNoDate,
                             MedicalDoseUnitGUID = a.MedicalDoseUnitGUID,
                             MedicalGenericNameGUID = a.MedicalGenericNameGUID,
                             MedicalManufacturerGUID = a.MedicalManufacturerGUID,
                             MedicalPackingUnitGUID = a.MedicalPackingUnitGUID,
                             MedicalPharmacologicalFormGUID = a.MedicalPharmacologicalFormGUID,
                             MedicalRouteAdministrationGUID = a.MedicalRouteAdministrationGUID,
                             MedicalTreatmentGUID = a.MedicalTreatmentGUID,
                             SourceGUID = a.SourceGUID,
                             TotalDispatchedItems=a.TotalDispatchedItems,
                             PackingUnit=a.PackingUnit,
                             Active = a.Active,
                             codeMedicalItemRowVersion = a.codeMedicalItemRowVersion,
                             CreatedBy = Audit.ExecutedBy,
                             CreatedOn = Audit.ExecutionTime,
                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("MedicalItem", "MedicalItems", new { Area = "EMT" }));

            return View("~/Areas/EMT/Views/MedicalItems/MedicalItem.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalItemCreate(MedicalItemUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.MedicalItem.Create, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveMedicalItem(model)) return PartialView("~/Areas/EMT/Views/MedicalItems/_MedicalItemForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            codeMedicalItem MedicalItem = Mapper.Map(model, new codeMedicalItem());
            MedicalItem.MedicalItemGUID = EntityPK;
            MedicalItem.TotalDispatchedItems = 0;
            DbEMT.Create(MedicalItem, Permissions.MedicalItem.CreateGuid, ExecutionTime, DbCMS);

            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.ItemQuantityThresholdsDataTable, ControllerContext, "ItemQuantityThresholdsContainer"));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.MedicalItem.Create, Apps.EMT, new UrlHelper(Request.RequestContext).Action("Create", "MedicalItems", new { Area = "EMT" })), Container = "MedicalItemFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.MedicalItem.Update, Apps.EMT), Container = "MedicalItemFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.MedicalItem.Delete, Apps.EMT), Container = "MedicalItemFormControls" });

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleCreateMessage(DbEMT.PrimaryKeyControl(MedicalItem), DbEMT.RowVersionControls(new List<codeMedicalItem>() { MedicalItem }), Partials, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalItemUpdate(MedicalItemUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.MedicalItem.Update, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveMedicalItem(model)) return PartialView("~/Areas/EMT/Views/MedicalItems/_MedicalItemForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            codeMedicalItem MedicalItem = Mapper.Map(model, new codeMedicalItem());
            DbEMT.Update(MedicalItem, Permissions.MedicalItem.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleUpdateMessage(null, null, DbEMT.RowVersionControls(new List<codeMedicalItem> { MedicalItem })));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyMedicalItem(model.MedicalItemGUID);
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalItemDelete(codeMedicalItem model)
        {
            if (!CMS.HasAction(Permissions.MedicalItem.Delete, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            List<codeMedicalItem> DeletedMedicalItem = DeleteMedicalItems(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.MedicalItem.Restore, Apps.EMT), Container = "MedicalItemFormControls" });

            try
            {
                int CommitedRows = DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleDeleteMessage(CommitedRows, DeletedMedicalItem.FirstOrDefault(), "ItemQuantityThresholdsContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyMedicalItem(model.MedicalItemGUID);
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalItemRestore(codeMedicalItem model)
        {
            if (!CMS.HasAction(Permissions.MedicalItem.Restore, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveMedicalItem(model))
            {
                return Json(DbEMT.RecordExists());
            }

            List<codeMedicalItem> RestoredMedicalItems = RestoreMedicalItems(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.MedicalItem.Create, Apps.EMT, new UrlHelper(Request.RequestContext).Action("MedicalItemCreate", "Configuration", new { Area = "EMT" })), Container = "MedicalItemFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.MedicalItem.Update, Apps.EMT), Container = "MedicalItemFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.MedicalItem.Delete, Apps.EMT), Container = "MedicalItemFormControls" });

            try
            {
                int CommitedRows = DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleRestoreMessage(CommitedRows, RestoredMedicalItems, DbEMT.PrimaryKeyControl(RestoredMedicalItems.FirstOrDefault()), Url.Action(DataTableNames.ItemQuantityThresholdsDataTable, Portal.GetControllerName(ControllerContext)), "ItemQuantityThresholdsContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyMedicalItem(model.MedicalItemGUID);
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult MedicalItemsDataTableDelete(List<codeMedicalItem> models)
        {
            if (!CMS.HasAction(Permissions.MedicalItem.Delete, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeMedicalItem> DeletedMedicalItems = DeleteMedicalItems(models);

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.PartialDeleteMessage(DeletedMedicalItems, models, DataTableNames.MedicalItemsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult MedicalItemsDataTableRestore(List<codeMedicalItem> models)
        {
            if (!CMS.HasAction(Permissions.MedicalItem.Restore, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeMedicalItem> RestoredMedicalItems = RestoreMedicalItems(models);

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.PartialRestoreMessage(RestoredMedicalItems, models, DataTableNames.MedicalItemsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        private List<codeMedicalItem> DeleteMedicalItems(List<codeMedicalItem> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<codeMedicalItem> DeletedMedicalItems = new List<codeMedicalItem>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbEMT.QueryBuilder(models, Permissions.MedicalItem.DeleteGuid, SubmitTypes.Delete, baseQuery);

            var Records = DbEMT.Database.SqlQuery<codeMedicalItem>(query).ToList();
            
            foreach (var record in Records)
            {
                if (record.RemainingItemsQuantity == 0)
                {
                    DeletedMedicalItems.Add(DbEMT.Delete(record, ExecutionTime, Permissions.MedicalItem.DeleteGuid, DbCMS));
                }
            }

            var ItemQuantityThresholds = DeletedMedicalItems.SelectMany(a => a.dataItemQuantityThreshold).Where(l => l.Active).ToList();
            foreach (var ItemQuantityThreshold in ItemQuantityThresholds)
            {
                DbEMT.Delete(ItemQuantityThreshold, ExecutionTime, Permissions.MedicalItem.DeleteGuid, DbCMS);
            }
            return DeletedMedicalItems;
        }

        private List<codeMedicalItem> RestoreMedicalItems(List<codeMedicalItem> models)
        {

            DateTime RestoringTime = DateTime.Now;

            List<codeMedicalItem> RestoredMedicalItems = new List<codeMedicalItem>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbEMT.QueryBuilder(models, Permissions.MedicalItem.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Records = DbEMT.Database.SqlQuery<codeMedicalItem>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveMedicalItem(record))
                {
                    RestoredMedicalItems.Add(DbEMT.Restore(record, Permissions.MedicalItem.DeleteGuid, Permissions.MedicalItem.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            var ItemQuantityThresholds = RestoredMedicalItems.SelectMany(x => x.dataItemQuantityThreshold.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var ItemQuantityThreshold in ItemQuantityThresholds)
            {
                DbEMT.Restore(ItemQuantityThreshold, Permissions.MedicalItem.DeleteGuid, Permissions.MedicalItem.RestoreGuid, RestoringTime, DbCMS);
            }

            return RestoredMedicalItems;
        }

        private JsonResult ConcurrencyMedicalItem(Guid PK)
        {
            MedicalItemUpdateModel dbModel = new MedicalItemUpdateModel();

            var MedicalItem = DbEMT.codeMedicalItem.Where(x => x.MedicalItemGUID == PK).FirstOrDefault();
            var dbMedicalItem = DbEMT.Entry(MedicalItem).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbMedicalItem, dbModel);

            var ItemQuantityThreshold = DbEMT.dataItemQuantityThreshold.Where(x => x.MedicalItemGUID == PK).Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeMedicalItem.DeletedOn)).FirstOrDefault();
            var dbItemQuantityThreshold = DbEMT.Entry(ItemQuantityThreshold).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbItemQuantityThreshold, dbModel);

            if (MedicalItem.codeMedicalItemRowVersion.SequenceEqual(dbModel.codeMedicalItemRowVersion))
            {
                return Json(DbEMT.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbEMT, dbModel, "ItemQuantityThresholdsContainer"));
        }

        private bool ActiveMedicalItem(Object model)
        {
            codeMedicalItem MedicalItem = Mapper.Map(model, new codeMedicalItem());
            int BrandName = DbEMT.codeMedicalItem 
                                    .Where(x => x.BrandName == MedicalItem.BrandName &&
                                                x.MedicalItemGUID != MedicalItem.MedicalItemGUID &&
                                                x.Active).Count();
            if (BrandName > 0)
            {
                ModelState.AddModelError("BrandName", "Brand Name is already exists");
            }
            return (BrandName > 0);
        }

        #endregion

        #region Item Quantity Threshold

        //[Route("EMT/ItemQuantityThresholdsDataTable/{PK}")]
        public ActionResult ItemQuantityThresholdsDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/EMT/Views/MedicalItems/_ItemQuantityThresholdsDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<ItemQuantityThresholdDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<ItemQuantityThresholdDataTableModel>(DataTable.Filters);
            }

            var Result = (from a in   DbEMT.dataItemQuantityThreshold.AsNoTracking().AsExpandable().Where(x =>   x.MedicalItemGUID == PK)
                          join b in DbEMT.codeMedicalPharmacyLanguage.Where(y => y.LanguageID == LAN && y.Active) on a.MedicalPharmacyGUID equals b.MedicalPharmacyGUID
                          join c in DbEMT.codeOrganizationsInstances on b.codeMedicalPharmacy.codeOrganizationsInstances.OrganizationInstanceGUID equals c.OrganizationInstanceGUID
                          join d in DbEMT.codeOrganizations on c.OrganizationGUID equals d.OrganizationGUID
                          select new ItemQuantityThresholdDataTableModel
                          {
                              ItemQuantityThresholdGUID = a.ItemQuantityThresholdGUID,
                              QuantityThreshold = a.QuantityThreshold,
                              MedicalItemGUID = a.MedicalItemGUID,
                              MedicalPharmacyGUID = a.MedicalPharmacyGUID,
                              MedicalPharmacyDescription = b.MedicalPharmacyDescription + " - " + d.OrganizationShortName,
                              QuantityTotalRemainingItems = a.QuantityTotalRemainingItems,
                              Active = a.Active,
                              dataItemQuantityThresholdRowVersion = a.dataItemQuantityThresholdRowVersion
                          }).Where(Predicate);

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ItemQuantityThresholdCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.MedicalItem.Create, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/EMT/Views/MedicalItems/_ItemQuantityThresholdUpdateModal.cshtml",
                new dataItemQuantityThreshold { MedicalItemGUID = FK });
        }

        public ActionResult ItemQuantityThresholdUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.MedicalItem.Access, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/EMT/Views/MedicalItems/_ItemQuantityThresholdUpdateModal.cshtml", DbEMT.dataItemQuantityThreshold.Find(PK));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ItemQuantityThresholdCreate(dataItemQuantityThreshold model)
        {
            if (!CMS.HasAction(Permissions.MedicalItem.Create, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveItemQuantityThreshold(model)) return PartialView("~/Areas/EMT/Views/MedicalItems/_ItemQuantityThresholdUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbEMT.Create(model, Permissions.MedicalItem.CreateGuid, ExecutionTime, DbCMS);

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleUpdateMessage(DataTableNames.ItemQuantityThresholdsDataTable, DbEMT.PrimaryKeyControl(model), DbEMT.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ItemQuantityThresholdUpdate(dataItemQuantityThreshold model)
        {
            if (!CMS.HasAction(Permissions.MedicalItem.Update, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveItemQuantityThreshold(model)) return PartialView("~/Areas/EMT/Views/MedicalItems/_ItemQuantityThresholdUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbEMT.Update(model, Permissions.MedicalItem.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleUpdateMessage(DataTableNames.ItemQuantityThresholdsDataTable,
                    DbEMT.PrimaryKeyControl(model),
                    DbEMT.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyItemQuantityThreshold(model.ItemQuantityThresholdGUID);
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ItemQuantityThresholdDelete(dataItemQuantityThreshold model)
        {
            if (!CMS.HasAction(Permissions.MedicalItem.Delete, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataItemQuantityThreshold> DeletedLanguages = DeleteItemQuantityThresholds(new List<dataItemQuantityThreshold> { model });

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleDeleteMessage(DeletedLanguages, DataTableNames.ItemQuantityThresholdsDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyItemQuantityThreshold(model.ItemQuantityThresholdGUID);
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ItemQuantityThresholdRestore(dataItemQuantityThreshold model)
        {
            if (!CMS.HasAction(Permissions.MedicalItem.Restore, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveItemQuantityThreshold(model))
            {
                return Json(DbEMT.RecordExists());
            }

            List<dataItemQuantityThreshold> RestoredLanguages = RestoreItemQuantityThresholds(Portal.SingleToList(model));

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleRestoreMessage(RestoredLanguages, DataTableNames.ItemQuantityThresholdsDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyItemQuantityThreshold(model.ItemQuantityThresholdGUID);
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ItemQuantityThresholdsDataTableDelete(List<dataItemQuantityThreshold> models)
        {
            if (!CMS.HasAction(Permissions.MedicalItem.Delete, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataItemQuantityThreshold> DeletedLanguages = DeleteItemQuantityThresholds(models);

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.ItemQuantityThresholdsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ItemQuantityThresholdsDataTableRestore(List<dataItemQuantityThreshold> models)
        {
            if (!CMS.HasAction(Permissions.MedicalItem.Restore, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataItemQuantityThreshold> RestoredLanguages = RestoreItemQuantityThresholds(models);

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.ItemQuantityThresholdsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        private List<dataItemQuantityThreshold> DeleteItemQuantityThresholds(List<dataItemQuantityThreshold> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<dataItemQuantityThreshold> DeletedItemQuantityThresholds = new List<dataItemQuantityThreshold>();

            string query = DbEMT.QueryBuilder(models, Permissions.MedicalItem.DeleteGuid, SubmitTypes.Delete, "");

            var QuantityThresholds = DbEMT.Database.SqlQuery<dataItemQuantityThreshold>(query).ToList();

            foreach (var QuantityThreshold in QuantityThresholds)
            {
                if (QuantityThreshold.QuantityTotalRemainingItems == 0)
                {
                    DeletedItemQuantityThresholds.Add(DbEMT.Delete(QuantityThreshold, ExecutionTime, Permissions.MedicalItem.DeleteGuid, DbCMS));
                }
            }

            return DeletedItemQuantityThresholds;
        }

        private List<dataItemQuantityThreshold> RestoreItemQuantityThresholds(List<dataItemQuantityThreshold> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<dataItemQuantityThreshold> RestoredLanguages = new List<dataItemQuantityThreshold>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbEMT.QueryBuilder(models, Permissions.MedicalItem.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Languages = DbEMT.Database.SqlQuery<dataItemQuantityThreshold>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveItemQuantityThreshold(language))
                {
                    RestoredLanguages.Add(DbEMT.Restore(language, Permissions.MedicalItem.DeleteGuid, Permissions.MedicalItem.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyItemQuantityThreshold(Guid PK)
        {
            dataItemQuantityThreshold dbModel = new dataItemQuantityThreshold();

            var Language = DbEMT.dataItemQuantityThreshold.Where(l => l.ItemQuantityThresholdGUID == PK).FirstOrDefault();
            var dbLanguage = DbEMT.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.dataItemQuantityThresholdRowVersion.SequenceEqual(dbModel.dataItemQuantityThresholdRowVersion))
            {
                return Json(DbEMT.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbEMT, dbModel, "LanguagesContainer"));
        }

        private bool ActiveItemQuantityThreshold(dataItemQuantityThreshold model)
        {
            int LanguageID = DbEMT.dataItemQuantityThreshold
                                  .Where(x => x.MedicalPharmacyGUID == model.MedicalPharmacyGUID &&
                                              x.MedicalItemGUID == model.MedicalItemGUID &&
                                              x.ItemQuantityThresholdGUID != model.ItemQuantityThresholdGUID &&
                                              x.Active).Count();
            if (LanguageID > 0)
            {
                ModelState.AddModelError("LanguageID", "selected already exists"); //From resource ?????? Amer  
            }

            return (LanguageID > 0);
        }

        #endregion

        [HttpPost]
        public ActionResult MedicalItemsBalance()
        {
            if (!CMS.HasAction(Permissions.MedicalItem.Create, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var ItemQuantityThreshold = DbEMT.dataItemQuantityThreshold.Where(x => x.Active).ToList();
            foreach (var ph in DbEMT.codeMedicalPharmacy.Where(x => x.MainWarehouse && x.Active).ToList())
            {
                foreach (var item in DbEMT.spMedicalItem(LAN))
                {
                    int? DispatchItemPharmacyQuantity = DbEMT.dataMedicalItemInputDetail.Where(x => x.MedicalItemGUID == item.MedicalItemGUID && x.dataMedicalItemInput.MedicalPharmacyGUID == ph.MedicalPharmacyGUID && x.Active).ToList().Sum(x => x.RemainingItems);
                    var QuantityThreshold = ItemQuantityThreshold.Where(x => x.MedicalItemGUID == item.MedicalItemGUID && x.MedicalPharmacyGUID == ph.MedicalPharmacyGUID).FirstOrDefault();
                    if (QuantityThreshold != null)
                    {
                        QuantityThreshold.QuantityTotalRemainingItems = DispatchItemPharmacyQuantity;
                    }
                }
            }
            foreach (var ph in DbEMT.codeMedicalPharmacy.Where(x => !x.MainWarehouse && x.Active).ToList())
            {
                foreach (var item in DbEMT.spMedicalItem(LAN))
                {
                    int? MedicalItemTransferDetail = (int?)DbEMT.dataMedicalItemTransferDetail.Where(x => x.MedicalItemGUID == item.MedicalItemGUID && x.dataMedicalItemTransfer.MedicalPharmacyGUID == ph.MedicalPharmacyGUID && x.Active).ToList().Sum(x => x.RemainingItems);
                    var QuantityThreshold = ItemQuantityThreshold.Where(x => x.MedicalItemGUID == item.MedicalItemGUID && x.MedicalPharmacyGUID == ph.MedicalPharmacyGUID).FirstOrDefault();
                    if (QuantityThreshold != null)
                    {
                        if (MedicalItemTransferDetail > 0)
                        {
                            QuantityThreshold.QuantityTotalRemainingItems = MedicalItemTransferDetail;
                        }
                    }
                }
            }

            foreach (var item in DbEMT.codeMedicalItem.Where(x => x.Active).ToList())
            {
                var ItemQuantity = ItemQuantityThreshold.Where(x => x.MedicalItemGUID == item.MedicalItemGUID).ToList().Sum(x => x.QuantityTotalRemainingItems);
                item.RemainingItemsQuantity = ItemQuantity.Value;

            }

            foreach (var item in DbEMT.codeMedicalItem.Where(x => x.Active).ToList())
            {
                int? DispatchItemPharmacyQuantity = DbEMT.dataMedicalItemInputDetail.Where(x => x.MedicalItemGUID == item.MedicalItemGUID &&  x.Active).ToList().Sum(x => x.QuantityByPackingUnit);

                item.TotalDispatchedItems = DispatchItemPharmacyQuantity.Value;
            }

            try
            {
                DbEMT.SaveChanges();
                return Json(DbEMT.SingleUpdateMessage(DataTableNames.MedicalItemsDataTable, null, null));
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }
    }
}