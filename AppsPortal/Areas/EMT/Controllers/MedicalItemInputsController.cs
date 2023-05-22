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
    public class MedicalItemInputsController : EMTBaseController
    {
        #region Medical Item Inputs

        public ActionResult Index()
        {
            return View();
        }

        [Route("EMT/MedicalItemInputs/")]
        public ActionResult MedicalItemInputsIndex()
        {
            if (!CMS.HasAction(Permissions.MedicalItemInput.Access, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/EMT/Views/MedicalItemInputs/Index.cshtml");
        }

        [Route("EMT/MedicalItemInputsDataTable/")]
        public JsonResult MedicalItemInputsDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<MedicalItemInputsDataTableModel, bool>> Predicate = p => true;
            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<MedicalItemInputsDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.MedicalPharmacy.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();

            var All = (from a in DbEMT.dataMedicalItemInput.AsExpandable()
                       join b in DbEMT.codeOrganizationsInstances on a.ProcuredByOrganizationInstanceGUID equals b.OrganizationInstanceGUID
                       join c in DbEMT.codeOrganizations on b.OrganizationGUID equals c.OrganizationGUID
                       join d in DbEMT.codeMedicalPharmacyLanguage.Where(x => x.Active && x.LanguageID == LAN).Where(x => AuthorizedList.Contains(x.MedicalPharmacyGUID.ToString()))
                       on a.MedicalPharmacyGUID equals d.MedicalPharmacyGUID
                       select new MedicalItemInputsDataTableModel
                       {
                           MedicalItemInputGUID = a.MedicalItemInputGUID,
                           ConfirmedReceived = a.ConfirmedReceived,
                           DeliveryDate = a.DeliveryDate,
                           MedicalPharmacyGUID = a.MedicalPharmacyGUID.ToString(),
                           MedicalPharmacyDescription = d.MedicalPharmacyDescription,
                           OrganizationInstanceDescription = c.OrganizationShortName,
                           ProcuredByOrganizationInstanceGUID = a.ProcuredByOrganizationInstanceGUID.ToString(),
                           ItemsConfirmedCount = a.dataMedicalItemInputDetail.Where(x => x.Confirmed).Count(),
                           Active = a.Active,
                           dataMedicalItemInputRowVersion = a.dataMedicalItemInputRowVersion
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<MedicalItemInputsDataTableModel> Result = Mapper.Map<List<MedicalItemInputsDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("EMT/MedicalWarehouseDataTable/")]
        public JsonResult MedicalPharmacysDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<MedicalPharmacysDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<MedicalPharmacysDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.MedicalPharmacy.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();

            var All = (from a in DbEMT.codeMedicalPharmacy.AsExpandable().Where(x => x.MainWarehouse && x.Active).Where(x => AuthorizedList.Contains(x.MedicalPharmacyGUID.ToString()))
                       join b in DbEMT.codeMedicalPharmacyLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.MedicalPharmacyGUID equals b.MedicalPharmacyGUID into LJ1
                       from R1 in LJ1.DefaultIfEmpty()
                       join c in DbEMT.codeOrganizationsInstancesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.OrganizationInstanceGUID equals c.OrganizationInstanceGUID into LJ2
                       from R2 in LJ2.DefaultIfEmpty()
                       select new MedicalPharmacysDataTableModel
                       {

                           MedicalPharmacyGUID = a.MedicalPharmacyGUID,
                           MedicalPharmacyDescription = R1.MedicalPharmacyDescription,
                           OrganizationInstanceDescription = R2.OrganizationInstanceDescription,
                           Active = a.Active,
                           codeMedicalPharmacyRowVersion = a.codeMedicalPharmacyRowVersion
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<MedicalPharmacysDataTableModel> Result = Mapper.Map<List<MedicalPharmacysDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("EMT/MedicalItemInputs/View/{PK}")]
        public ActionResult MedicalItemInputView(Guid PK)
        {
            if (!CMS.HasAction(Permissions.MedicalItemInput.Access, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var pharmacy = DbEMT.codeMedicalPharmacyLanguage.Where(x => x.LanguageID == LAN && x.Active && x.MedicalPharmacyGUID == PK).FirstOrDefault();
            ViewBag.PharmacyName = pharmacy!=null? pharmacy.MedicalPharmacyDescription:"";
            return View("~/Areas/EMT/Views/MedicalItemInputs/MedicalItemInputDetailsDataTable.cshtml", new MasterRecordStatus() { ParentGUID = PK });
        }

        public ActionResult MedicalItemInputDetailsViewDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/EMT/Views/MedicalItemInputs/MedicalItemInputDetailsDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<MedicalItemInputDetailsDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<MedicalItemInputDetailsDataTableModel>(DataTable.Filters);
            }
            bool CostViewAuthorization = DbCMS.userServiceHistory.Where(x => x.UserGUID == UserGUID).FirstOrDefault().EmailAddress.ToUpper().EndsWith("UNHCR.ORG");

            var All = (from a in DbEMT.dataMedicalItemInputDetail.AsNoTracking().AsExpandable().Where(x => x.dataMedicalItemInput.MedicalPharmacyGUID == PK && x.Active)
                       join b in DbEMT.codeMedicalItem on a.MedicalItemGUID equals b.MedicalItemGUID
                       select new MedicalItemInputDetailsDataTableModel
                       {
                           MedicalItemInputDetailGUID = a.MedicalItemInputDetailGUID,
                           QuantityByPackingTransfer = a.QuantityByPackingTransfer != null ? a.QuantityByPackingTransfer.Value : 0,
                           BatchNumber = a.BatchNumber,
                           ExpirationDate = a.ExpirationDate,
                           PriceOfSmallestUnit =CostViewAuthorization? a.PriceOfSmallestUnit:0,
                           PriceOfPackingUnit = CostViewAuthorization? a.PriceOfPackingUnit:0,
                           QuantityByPackingUnit = a.QuantityByPackingUnit != 0 ? a.QuantityByPackingUnit : a.QuantityByPackingTransfer.Value,
                           QuantityBySmallestUnit = a.QuantityBySmallestUnit,
                           RemainingItems = a.RemainingItems,
                           MedicalItemGUID = a.MedicalItemGUID.ToString(),
                           BrandName = b.BrandName + ", " + a.codeMedicalItem.DoseQuantity,
                           MedicalGenericNameDescription = b.codeMedicalGenericName.codeMedicalGenericNameLanguage.Where(x => x.LanguageID == LAN && x.Active).FirstOrDefault().MedicalGenericNameDescription.Replace("+", " "),
                           MedicalGenericNameGUID = b.codeMedicalGenericName.MedicalGenericNameGUID.ToString(),
                           ManufacturingDate = a.ManufacturingDate,
                           MedicalItemInputGUID = a.MedicalItemInputGUID,
                           DeliveryDate = a.dataMedicalItemInput.DeliveryDate,
                           Active = a.Active,
                           dataMedicalItemInputDetailRowVersion = a.dataMedicalItemInputDetailRowVersion
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<MedicalItemInputDetailsDataTableModel> Result = Mapper.Map<List<MedicalItemInputDetailsDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("EMT/MedicalItemInputs/Create/")]
        public ActionResult MedicalItemInputCreate()
        {
            if (!CMS.HasAction(Permissions.MedicalItemInput.Create, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/EMT/Views/MedicalItemInputs/MedicalItemInput.cshtml", new MedicalItemInputUpdateModel());
        }

        [Route("EMT/MedicalItemInputs/Update/{PK}")]
        public ActionResult MedicalItemInputUpdate(Guid PK)
        {
            //if (!CMS.HasAction(Permissions.MedicalItemInput.Access, Apps.EMT))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            var Audit = DbEMT.spAuditHistory(LAN, PK).OrderBy(x=>x.ExecutionTime).FirstOrDefault();
            if (Audit == null)
            {
                Audit = DbEMT.spAuditHistoryOld("EN", PK).OrderBy(x => x.ExecutionTime).Select(x =>
                            new spAuditHistory_Result()
                            {
                                ExecutedBy = x.ExecutedBy,
                                ExecutionTime = x.ExecutionTime
                            }).FirstOrDefault();
            }
            var model = (from a in DbEMT.dataMedicalItemInput.WherePK(PK)
                         select new MedicalItemInputUpdateModel
                         {
                             MedicalItemInputGUID = a.MedicalItemInputGUID,
                             ConfirmedReceived = a.ConfirmedReceived,
                             DeliveryDate = a.DeliveryDate,
                             MedicalPharmacyGUID = a.MedicalPharmacyGUID,
                             ProcuredByOrganizationInstanceGUID = a.ProcuredByOrganizationInstanceGUID,
                             ProvidedByOrganizationInstanceGUID = a.ProvidedByOrganizationInstanceGUID,
                             Active = a.Active,
                             dataMedicalItemInputRowVersion = a.dataMedicalItemInputRowVersion,
                             CreatedBy= Audit.ExecutedBy,
                             CreatedOn=Audit.ExecutionTime,
                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("MedicalItemInput", "MedicalItemInputs", new { Area = "EMT" }));

            return View("~/Areas/EMT/Views/MedicalItemInputs/MedicalItemInput.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalItemInputCreate(MedicalItemInputUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.MedicalItemInput.Create, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveMedicalItemInput(model)) return PartialView("~/Areas/EMT/Views/MedicalItemInputs/_MedicalItemInputForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            dataMedicalItemInput MedicalItemInput = Mapper.Map(model, new dataMedicalItemInput());
            MedicalItemInput.MedicalItemInputGUID = EntityPK;
            DbEMT.Create(MedicalItemInput, Permissions.MedicalItemInput.CreateGuid, ExecutionTime, DbCMS);


            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.MedicalItemInputDetailsDataTable, ControllerContext, "MedicalItemInputDetailsContainer"));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.MedicalItemInput.Create, Apps.EMT, new UrlHelper(Request.RequestContext).Action("Create", "MedicalItemInputs", new { Area = "EMT" })), Container = "MedicalItemInputFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.MedicalItemInput.Update, Apps.EMT), Container = "MedicalItemInputFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.MedicalItemInput.Delete, Apps.EMT), Container = "MedicalItemInputFormControls" });

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleCreateMessage(DbEMT.PrimaryKeyControl(MedicalItemInput), DbEMT.RowVersionControls(new List<dataMedicalItemInput> { MedicalItemInput }), Partials, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalItemInputUpdate(MedicalItemInputUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.MedicalItemInput.Update, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveMedicalItemInput(model)) return PartialView("~/Areas/EMT/Views/MedicalItemInputs/_MedicalItemInputForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;
            Guid OriginalPharmacyGUID = DbEMT.dataMedicalItemInput.Where(x => x.MedicalItemInputGUID == model.MedicalItemInputGUID).FirstOrDefault().MedicalPharmacyGUID.Value;

            dataMedicalItemInput MedicalItemInput = Mapper.Map(model, new dataMedicalItemInput());
            DbEMT.Update(MedicalItemInput, Permissions.MedicalItemInput.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                //in case change the pharmacy
                if (model.MedicalPharmacyGUID != OriginalPharmacyGUID)
                {
                    updateItemQuantityThresholdQuantity();
                }
                return Json(DbEMT.SingleUpdateMessage(null, null, DbEMT.RowVersionControls(new List<dataMedicalItemInput> { MedicalItemInput })));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyMedicalItemInput(model.MedicalItemInputGUID);
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        private void updateItemQuantityThresholdQuantity()
        {
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
                    else
                    {
                        dataItemQuantityThreshold quantityThreshold = new dataItemQuantityThreshold();
                        quantityThreshold.Active = true;
                        quantityThreshold.ItemQuantityThresholdGUID = Guid.NewGuid();
                        quantityThreshold.MedicalItemGUID = item.MedicalItemGUID;
                        quantityThreshold.MedicalPharmacyGUID = ph.MedicalPharmacyGUID;
                        quantityThreshold.QuantityThreshold = 0;
                        quantityThreshold.QuantityTotalRemainingItems = DispatchItemPharmacyQuantity;
                        DbEMT.dataItemQuantityThreshold.Add(quantityThreshold);
                    }
                }
            }
            DbEMT.SaveChanges();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalItemInputDelete(dataMedicalItemInput model)
        {
            if (!CMS.HasAction(Permissions.MedicalItemInput.Delete, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            List<dataMedicalItemInput> DeletedMedicalItemInput = DeleteMedicalItemInputs(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.MedicalItemInput.Restore, Apps.EMT), Container = "MedicalItemInputFormControls" });

            try
            {
                int CommitedRows = DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleDeleteMessage(CommitedRows, DeletedMedicalItemInput.FirstOrDefault(), "MedicalItemInputDetailsContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyMedicalItemInput(model.MedicalItemInputGUID);
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }

        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalItemInputRestore(dataMedicalItemInput model)
        {
            if (!CMS.HasAction(Permissions.MedicalItemInput.Restore, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveMedicalItemInput(model))
            {
                return Json(DbEMT.RecordExists());
            }

            List<dataMedicalItemInput> RestoredMedicalItemInputs = RestoreMedicalItemInputs(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.MedicalItemInput.Create, Apps.EMT, new UrlHelper(Request.RequestContext).Action("MedicalItemInputCreate", "Configuration", new { Area = "EMT" })), Container = "MedicalItemInputFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.MedicalItemInput.Update, Apps.EMT), Container = "MedicalItemInputFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.MedicalItemInput.Delete, Apps.EMT), Container = "MedicalItemInputFormControls" });

            try
            {
                int CommitedRows = DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleRestoreMessage(CommitedRows, RestoredMedicalItemInputs, DbEMT.PrimaryKeyControl(RestoredMedicalItemInputs.FirstOrDefault()), Url.Action(DataTableNames.MedicalItemInputDetailsDataTable, Portal.GetControllerName(ControllerContext)), "MedicalItemInputDetailsContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyMedicalItemInput(model.MedicalItemInputGUID);
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult MedicalItemInputsDataTableDelete(List<dataMedicalItemInput> models)
        {
            if (!CMS.HasAction(Permissions.MedicalItemInput.Delete, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataMedicalItemInput> DeletedMedicalItemInputs = DeleteMedicalItemInputs(models);

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.PartialDeleteMessage(DeletedMedicalItemInputs, models, DataTableNames.MedicalItemInputsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult MedicalItemInputsDataTableRestore(List<dataMedicalItemInput> models)
        {
            if (!CMS.HasAction(Permissions.MedicalItemInput.Restore, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataMedicalItemInput> RestoredMedicalItemInputs = RestoreMedicalItemInputs(models);

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.PartialRestoreMessage(RestoredMedicalItemInputs, models, DataTableNames.MedicalItemInputsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        private List<dataMedicalItemInput> DeleteMedicalItemInputs(List<dataMedicalItemInput> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<dataMedicalItemInput> DeletedMedicalItemInputs = new List<dataMedicalItemInput>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbEMT.QueryBuilder(models, Permissions.MedicalItemInput.DeleteGuid, SubmitTypes.Delete, baseQuery);

            var Records = DbEMT.Database.SqlQuery<dataMedicalItemInput>(query).ToList();
            foreach (var record in Records)
            {
                if (!record.ConfirmedReceived)
                {
                    DeletedMedicalItemInputs.Add(DbEMT.Delete(record, ExecutionTime, Permissions.MedicalItemInput.DeleteGuid, DbCMS));
                }
            }

            var MedicalItemInputDetails = DeletedMedicalItemInputs.SelectMany(a => a.dataMedicalItemInputDetail).Where(l => l.Active).ToList();
            foreach (var MedicalItemInputDetail in MedicalItemInputDetails)
            {
                DbEMT.Delete(MedicalItemInputDetail, ExecutionTime, Permissions.MedicalItemInput.DeleteGuid, DbCMS);

                RemoveItemQuantity(MedicalItemInputDetail.MedicalItemInputDetailGUID);
            }
            return DeletedMedicalItemInputs;
        }

        private List<dataMedicalItemInput> RestoreMedicalItemInputs(List<dataMedicalItemInput> models)
        {

            DateTime RestoringTime = DateTime.Now;

            List<dataMedicalItemInput> RestoredMedicalItemInputs = new List<dataMedicalItemInput>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbEMT.QueryBuilder(models, Permissions.MedicalItemInput.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Records = DbEMT.Database.SqlQuery<dataMedicalItemInput>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveMedicalItemInput(record))
                {
                    RestoredMedicalItemInputs.Add(DbEMT.Restore(record, Permissions.MedicalItemInput.DeleteGuid, Permissions.MedicalItemInput.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            var MedicalItemInputDetails = RestoredMedicalItemInputs.SelectMany(x => x.dataMedicalItemInputDetail.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var MedicalItemInputDetail in MedicalItemInputDetails)
            {
                DbEMT.Restore(MedicalItemInputDetail, Permissions.MedicalItemInput.DeleteGuid, Permissions.MedicalItemInput.RestoreGuid, RestoringTime, DbCMS);

                AddItemQuantity(MedicalItemInputDetail.MedicalItemInputDetailGUID);
            }

            return RestoredMedicalItemInputs;
        }

        private JsonResult ConcurrencyMedicalItemInput(Guid PK)
        {
            MedicalItemInputUpdateModel dbModel = new MedicalItemInputUpdateModel();

            var MedicalItemInput = DbEMT.dataMedicalItemInput.Where(x => x.MedicalItemInputGUID == PK).FirstOrDefault();
            var dbMedicalItemInput = DbEMT.Entry(MedicalItemInput).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbMedicalItemInput, dbModel);

            var MedicalItemInputDetail = DbEMT.dataMedicalItemInputDetail.Where(x => x.MedicalItemInputGUID == PK).Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.dataMedicalItemInput.DeletedOn)).FirstOrDefault();
            var dbMedicalItemInputDetail = DbEMT.Entry(MedicalItemInputDetail).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbMedicalItemInputDetail, dbModel);

            if (MedicalItemInput.dataMedicalItemInputRowVersion.SequenceEqual(dbModel.dataMedicalItemInputRowVersion))
            {
                return Json(DbEMT.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbEMT, dbModel, "MedicalItemInputDetailsContainer"));
        }

        private bool ActiveMedicalItemInput(Object model)
        {
            return false;
        }

        #endregion

        #region  Medical Item Input Details

        //[Route("EMT/MedicalItemInputDetailsDataTable/{PK}")]
        public ActionResult MedicalItemInputDetailsDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/EMT/Views/MedicalItemInputs/_MedicalItemInputDetailsDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<MedicalItemInputDetailsDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<MedicalItemInputDetailsDataTableModel>(DataTable.Filters);
            }

            var Result = (from a in DbEMT.dataMedicalItemInputDetail.AsNoTracking().AsExpandable().Where(x => x.MedicalItemInputGUID == PK)
                          join b in DbEMT.codeMedicalItem on a.MedicalItemGUID equals b.MedicalItemGUID
                          select new MedicalItemInputDetailsDataTableModel
                          {
                              MedicalItemInputDetailGUID = a.MedicalItemInputDetailGUID,
                              QuantityByPackingTransfer = a.QuantityByPackingTransfer != null ? a.QuantityByPackingTransfer.Value : 0,
                              BatchNumber = a.BatchNumber,
                              ExpirationDate = a.ExpirationDate,
                              PriceOfSmallestUnit = a.PriceOfSmallestUnit,
                              PriceOfPackingUnit = a.PriceOfPackingUnit,
                              QuantityByPackingUnit = a.QuantityByPackingUnit != 0 ? a.QuantityByPackingUnit : a.QuantityByPackingTransfer.Value,
                              QuantityBySmallestUnit = a.QuantityBySmallestUnit,
                              RemainingItems = a.RemainingItems,
                              MedicalItemGUID = a.MedicalItemGUID.ToString(),
                              BrandName = b.BrandName + ", " + a.codeMedicalItem.DoseQuantity,
                              MedicalGenericNameDescription = b.codeMedicalGenericName.codeMedicalGenericNameLanguage.Where(x => x.LanguageID == LAN && x.Active).FirstOrDefault().MedicalGenericNameDescription.Replace("+", " "),
                              ManufacturingDate = a.ManufacturingDate,
                              MedicalItemInputGUID = a.MedicalItemInputGUID,
                              Active = a.Active,
                              dataMedicalItemInputDetailRowVersion = a.dataMedicalItemInputDetailRowVersion
                          }).Where(Predicate);

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        private bool ActiveMedicalItemInput(MedicalItemTransferUpdateModel model)
        {
            bool valid = false;
            int index = 0;
            foreach (var item in model.MedicalItemInputDetailsDataTableModel)
            {
                if (item.QuantityByPackingUnit > item.RemainingItems)
                {
                    ModelState.AddModelError("MedicalItemInputDetailsDataTableModel[" + index + "].QuantityByPackingUnit", "Quantity of Packing Unit :" + item.QuantityByPackingUnit + ", Grater Than Remaining Item" + item.RemainingItems);
                    valid = true;
                }
                index++;
            }

            return valid;
        }

        public ActionResult MedicalItemInputDetailCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.MedicalItemInput.Create, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/EMT/Views/MedicalItemInputs/_MedicalItemInputDetailsUpdateModal.cshtml",
                new dataMedicalItemInputDetail { MedicalItemInputGUID = FK });
        }

        public ActionResult MedicalItemInputDetailUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.MedicalItemInput.Access, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/EMT/Views/MedicalItemInputs/_MedicalItemInputDetailsUpdateModal.cshtml", DbEMT.dataMedicalItemInputDetail.Find(PK));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalItemInputDetailCreate(dataMedicalItemInputDetail model)
        {
            if (!CMS.HasAction(Permissions.MedicalItemInput.Create, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveMedicalItemInputDetail(model)) return PartialView("~/Areas/EMT/Views/MedicalItemInputs/_MedicalItemInputDetailsUpdateModal.cshtml", model);

            var Item = DbEMT.codeMedicalItem.Where(x => x.MedicalItemGUID == model.MedicalItemGUID).FirstOrDefault();
            DateTime ExecutionTime = DateTime.Now;
            model.MedicalItemInputDetailGUID = Guid.NewGuid();

            model.PriceOfPackingUnit = model.PriceOfSmallestUnit * Item.PackingUnit.Value;
            model.QuantityByPackingUnit = Convert.ToInt32(model.QuantityBySmallestUnit / Item.PackingUnit.Value);
            model.RemainingItems = model.QuantityByPackingUnit;
            DbEMT.Create(model, Permissions.MedicalItemInput.CreateGuid, ExecutionTime, DbCMS);


            Item.RemainingItemsQuantity = Item.RemainingItemsQuantity + model.QuantityByPackingUnit;
            Item.TotalDispatchedItems = Item.TotalDispatchedItems + model.QuantityByPackingUnit;
            var medicalItemInput = DbEMT.dataMedicalItemInput.Where(x => x.MedicalItemInputGUID == model.MedicalItemInputGUID).FirstOrDefault();
            dataItemQuantityThreshold itemQuantityThreshold = DbEMT.dataItemQuantityThreshold.Where(x => x.MedicalItemGUID == model.MedicalItemGUID && x.MedicalPharmacyGUID == medicalItemInput.MedicalPharmacyGUID && x.Active).FirstOrDefault();
            if (itemQuantityThreshold == null)
            {
                itemQuantityThreshold = new dataItemQuantityThreshold();
                itemQuantityThreshold.ItemQuantityThresholdGUID = Guid.NewGuid();
                itemQuantityThreshold.MedicalItemGUID = model.MedicalItemGUID;
                itemQuantityThreshold.QuantityThreshold = 0;
                itemQuantityThreshold.QuantityTotalRemainingItems = model.QuantityByPackingUnit;
                itemQuantityThreshold.MedicalPharmacyGUID = medicalItemInput.MedicalPharmacyGUID.Value;
                DbEMT.Create(itemQuantityThreshold, Permissions.MedicalItem.CreateGuid, ExecutionTime, DbCMS);
            }
            else
            {
                itemQuantityThreshold.QuantityTotalRemainingItems = itemQuantityThreshold.QuantityTotalRemainingItems + model.QuantityByPackingUnit;
            }

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleUpdateMessage(DataTableNames.MedicalItemInputDetailsDataTable, DbEMT.PrimaryKeyControl(model), DbEMT.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalItemInputDetailUpdate(dataMedicalItemInputDetail model)
        {
            if (!CMS.HasAction(Permissions.MedicalItemInput.Update, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveMedicalItemInputDetail(model)) return PartialView("~/Areas/EMT/Views/MedicalItemInputs/_MedicalItemInputDetailsUpdateModal.cshtml", model);
            if (!model.Confirmed)
            {
                RemoveItemQuantity(model.MedicalItemInputDetailGUID);
                DateTime ExecutionTime = DateTime.Now;

                var Item = DbEMT.codeMedicalItem.Where(x => x.MedicalItemGUID == model.MedicalItemGUID).FirstOrDefault();
                // model.PriceOfPackingUnit = model.PriceOfSmallestUnit * Item.PackingUnit.Value;
                // model.QuantityBySmallestUnit =(int)( model.QuantityByPackingUnit * Item.PackingUnit.Value);

                var itemInputDetails = DbEMT.dataMedicalItemInputDetail.Where(x => x.MedicalItemInputDetailGUID == model.MedicalItemInputDetailGUID && x.Active).ToList().FirstOrDefault();
                //in case the PackingUnit changed
                if (Convert.ToInt32(itemInputDetails.QuantityBySmallestUnit / itemInputDetails.QuantityByPackingUnit) != Item.PackingUnit.Value)
                {
                    double changeCal = (itemInputDetails.QuantityBySmallestUnit / itemInputDetails.QuantityByPackingUnit) / Item.PackingUnit.Value;
                    model.RemainingItems = (int)((itemInputDetails.RemainingItems * changeCal) +
                        (model.QuantityByPackingUnit - itemInputDetails.QuantityByPackingUnit * changeCal));
                }
                //in case the quaintity changed and  PackingUnit still the same 
                if ((itemInputDetails.QuantityBySmallestUnit / itemInputDetails.QuantityByPackingUnit) == Item.PackingUnit.Value &&
                    model.QuantityByPackingUnit != itemInputDetails.QuantityByPackingUnit)
                {
                    model.RemainingItems = itemInputDetails.RemainingItems + (model.QuantityByPackingUnit - itemInputDetails.QuantityByPackingUnit);
                }

                DbEMT.Update(model, Permissions.MedicalItemInput.UpdateGuid, ExecutionTime, DbCMS);

                AddItemQuantity(model.MedicalItemInputDetailGUID);
                var MedicalItemInputSupplyDetail = DbEMT.dataMedicalItemInputSupplyDetail.Where(x => x.MedicalItemInputSupplyDetailGUID == model.MedicalItemInputSupplyDetailGUID).FirstOrDefault();
                bool update = true;
                //Check if the remaining items enough to update it by new Quantity By Packing Unit
                //Jawad Updates 16 jan 2023
                if (MedicalItemInputSupplyDetail != null) { if (MedicalItemInputSupplyDetail.RemainingItems < 0) { update = false; } }
                try
                {
                    if (update)
                    {
                        DbEMT.SaveChanges();
                        DbCMS.SaveChanges();
                        return Json(DbEMT.SingleUpdateMessage(DataTableNames.MedicalItemInputDetailsDataTable,
                            DbEMT.PrimaryKeyControl(model),
                            DbEMT.RowVersionControls(Portal.SingleToList(model))));
                    }
                    else { return Json(DbEMT.ErrorMessage("Not Enough Remaining Items")); }
                }
                catch (DbUpdateConcurrencyException)
                {
                    return ConcrrencyMedicalItemInputDetail(model.MedicalItemInputDetailGUID);
                }
                catch (Exception ex)
                {
                    return Json(DbEMT.ErrorMessage(ex.Message));
                }
            }
            if (Session[SessionKeys.EmailAddress].ToString().ToLower().EndsWith("unhcr.org"))
            {
                //RemoveItemQuantity(model.MedicalItemInputDetailGUID);
                DateTime ExecutionTime = DateTime.Now;

                var Item = DbEMT.codeMedicalItem.Where(x => x.MedicalItemGUID == model.MedicalItemGUID).FirstOrDefault();

                DbEMT.Update(model, Permissions.MedicalItemInput.UpdateGuid, ExecutionTime, DbCMS);

                //AddItemQuantity(model.MedicalItemInputDetailGUID);
                var MedicalItemInputSupplyDetail = DbEMT.dataMedicalItemInputSupplyDetail.Where(x => x.MedicalItemInputSupplyDetailGUID == model.MedicalItemInputSupplyDetailGUID).FirstOrDefault();
                bool update = true;
                //Check if the remaining items enough to update it by new Quantity By Packing Unit
                //Jawad Updates 16 jan 2023
                //if (MedicalItemInputSupplyDetail != null) { if (MedicalItemInputSupplyDetail.RemainingItems < 0) { update = false; } }
                try
                {
                    if (update)
                    {
                        DbEMT.SaveChanges();
                        DbCMS.SaveChanges();
                        return Json(DbEMT.SingleUpdateMessage(DataTableNames.MedicalItemInputDetailsDataTable,
                            DbEMT.PrimaryKeyControl(model),
                            DbEMT.RowVersionControls(Portal.SingleToList(model))));
                    }
                    else { return Json(DbEMT.ErrorMessage("Not Enough Remaining Items")); }
                }
                catch (DbUpdateConcurrencyException)
                {
                    return ConcrrencyMedicalItemInputDetail(model.MedicalItemInputDetailGUID);
                }
                catch (Exception ex)
                {
                    return Json(DbEMT.ErrorMessage(ex.Message));
                }
            }
            else
            {
                return Json(DbEMT.ErrorMessage("Item Confirmed Received!"));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalItemInputDetailDelete(dataMedicalItemInputDetail model)
        {
            if (!CMS.HasAction(Permissions.MedicalItemInput.Delete, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataMedicalItemInputDetail> DeletedLanguages = DeleteMedicalItemInputDetails(new List<dataMedicalItemInputDetail> { model });
            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleDeleteMessage(DeletedLanguages, DataTableNames.MedicalItemInputDetailsDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyMedicalItemInputDetail(model.MedicalItemInputDetailGUID);
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }


        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalItemInputDetailRestore(dataMedicalItemInputDetail model)
        {
            if (!CMS.HasAction(Permissions.MedicalItemInput.Restore, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveMedicalItemInputDetail(model))
            {
                return Json(DbEMT.RecordExists());
            }

            List<dataMedicalItemInputDetail> RestoredLanguages = RestoreMedicalItemInputDetails(Portal.SingleToList(model));

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleRestoreMessage(RestoredLanguages, DataTableNames.MedicalItemInputDetailsDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyMedicalItemInputDetail(model.MedicalItemInputDetailGUID);
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult MedicalItemInputDetailsDataTableDelete(List<dataMedicalItemInputDetail> models)
        {
            if (!CMS.HasAction(Permissions.MedicalItemInput.Delete, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            List<dataMedicalItemInputDetail> DeletedLanguages = DeleteMedicalItemInputDetails(models);

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.MedicalItemInputDetailsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }


        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult MedicalItemInputDetailsDataTableRestore(List<dataMedicalItemInputDetail> models)
        {
            if (!CMS.HasAction(Permissions.MedicalItemInput.Restore, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataMedicalItemInputDetail> RestoredLanguages = RestoreMedicalItemInputDetails(models);

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.MedicalItemInputDetailsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        private List<dataMedicalItemInputDetail> DeleteMedicalItemInputDetails(List<dataMedicalItemInputDetail> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<dataMedicalItemInputDetail> DeletedMedicalItemInputDetails = new List<dataMedicalItemInputDetail>();

            string query = DbEMT.QueryBuilder(models, Permissions.MedicalItemInput.DeleteGuid, SubmitTypes.Delete, "");

            var MedicalItemInputDetails = DbEMT.Database.SqlQuery<dataMedicalItemInputDetail>(query).ToList();

            //var medicalItemInput = DbEMT.dataMedicalItemInput.Where(x => x.MedicalItemInputGUID == models.FirstOrDefault().MedicalItemInputGUID).FirstOrDefault();
            foreach (var MedicalItemInputDetail in MedicalItemInputDetails)
            {
                if (!MedicalItemInputDetail.Confirmed)
                {
                    DeletedMedicalItemInputDetails.Add(DbEMT.Delete(MedicalItemInputDetail, ExecutionTime, Permissions.MedicalItemInput.DeleteGuid, DbCMS));
                    RemoveItemQuantity(MedicalItemInputDetail.MedicalItemInputDetailGUID);
                }
            }

            return DeletedMedicalItemInputDetails;
        }

        private List<dataMedicalItemInputDetail> RestoreMedicalItemInputDetails(List<dataMedicalItemInputDetail> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<dataMedicalItemInputDetail> RestoredLanguages = new List<dataMedicalItemInputDetail>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbEMT.QueryBuilder(models, Permissions.MedicalItemInput.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var MedicalItemInputDetails = DbEMT.Database.SqlQuery<dataMedicalItemInputDetail>(query).ToList();

            //var medicalItemInput = DbEMT.dataMedicalItemInput.Where(x => x.MedicalItemInputGUID == models.FirstOrDefault().MedicalItemInputGUID).FirstOrDefault();
            foreach (var MedicalItemInputDetail in MedicalItemInputDetails)
            {
                if (!ActiveMedicalItemInputDetail(MedicalItemInputDetail))
                {
                    RestoredLanguages.Add(DbEMT.Restore(MedicalItemInputDetail, Permissions.MedicalItemInput.DeleteGuid, Permissions.MedicalItemInput.RestoreGuid, RestoringTime, DbCMS));
                    AddItemQuantity(MedicalItemInputDetail.MedicalItemInputDetailGUID);
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyMedicalItemInputDetail(Guid PK)
        {
            dataMedicalItemInputDetail dbModel = new dataMedicalItemInputDetail();

            var Language = DbEMT.dataMedicalItemInputDetail.Where(l => l.MedicalItemInputDetailGUID == PK).FirstOrDefault();
            var dbLanguage = DbEMT.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.dataMedicalItemInputDetailRowVersion.SequenceEqual(dbModel.dataMedicalItemInputDetailRowVersion))
            {
                return Json(DbEMT.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbEMT, dbModel, "MedicalItemInputDetailsContainer"));
        }

        private bool ActiveMedicalItemInputDetail(dataMedicalItemInputDetail model)
        {
            var Item = DbEMT.codeMedicalItem.Where(x => x.MedicalItemGUID == model.MedicalItemGUID).FirstOrDefault();

            int ItemInputDetail = DbEMT.dataMedicalItemInputDetail
                                  .Where(x => x.MedicalItemInputDetailGUID != model.MedicalItemInputDetailGUID &&
                                  x.MedicalItemInputGUID == model.MedicalItemInputGUID &&
                                              x.MedicalItemGUID == model.MedicalItemGUID &&
                                              x.BatchNumber == model.BatchNumber &&
                                              x.PriceOfSmallestUnit == model.PriceOfSmallestUnit &&
                                              x.Active).Count();
            if (ItemInputDetail > 0)
            {
                ModelState.AddModelError("MedicalItemInputDetailGUID", "Item already exists"); //From resource ?????? Amer  
            }
            if (Item.PackingUnit == 0)
            {
                ModelState.AddModelError("QuantityByPackingUnit", "Packing Unit = 0 Quantity Can't be calculated");
                ModelState.AddModelError("PriceOfPackingUnit", "Packing Unit = 0 Price Can't be calculated");
                ItemInputDetail++;
            }
            //dataMedicalItemInput itemInput = DbEMT.dataMedicalItemInput.Where(x => x.MedicalItemInputGUID == model.MedicalItemInputGUID).FirstOrDefault();
            //double period = (model.ExpirationDate.Value - model.ManufacturingDate.Value).TotalDays;
            //double periodRemaining = (model.ExpirationDate.Value - itemInput.DeliveryDate.Value).TotalDays;
            //double cal1 = (periodRemaining / period);
            //double cal2 = (2.0 / 3.0);
            //if (cal1 <= cal2)
            //{
            //    ModelState.AddModelError("ExpirationDate", "The Remaining Expire of Item Should Be  >=  2/3 of Expire Date");
            //    ItemInputDetail++;
            //}
            return (ItemInputDetail > 0);
        }

        public void AddItemQuantity(Guid MedicalItemInputDetailGUID)
        {
            var model = DbEMT.dataMedicalItemInputDetail.Where(x => x.MedicalItemInputDetailGUID == MedicalItemInputDetailGUID).FirstOrDefault();
            DateTime ExecutionTime = DateTime.Now;
            codeMedicalItem item = DbEMT.codeMedicalItem.Where(x => x.MedicalItemGUID == model.MedicalItemGUID).FirstOrDefault();
            item.RemainingItemsQuantity = item.RemainingItemsQuantity + model.QuantityByPackingUnit;
            item.TotalDispatchedItems = item.TotalDispatchedItems + model.QuantityByPackingUnit;

            var medicalItemInput = DbEMT.dataMedicalItemInput.Where(x => x.MedicalItemInputGUID == model.MedicalItemInputGUID).FirstOrDefault();
            dataItemQuantityThreshold itemQuantityThreshold = DbEMT.dataItemQuantityThreshold.Where(x => x.MedicalItemGUID == model.MedicalItemGUID && x.MedicalPharmacyGUID == medicalItemInput.MedicalPharmacyGUID && x.Active).FirstOrDefault();
            itemQuantityThreshold.QuantityTotalRemainingItems = itemQuantityThreshold.QuantityTotalRemainingItems + model.QuantityByPackingUnit;

            //In case transfer from other  Item Input Detail
            if (model.MedicalItemInputDetailTransferGUID != null)
            {
                var modelTransfer = DbEMT.dataMedicalItemInputDetail.Where(x => x.MedicalItemInputDetailGUID == model.MedicalItemInputDetailTransferGUID).FirstOrDefault();
                modelTransfer.RemainingItems = modelTransfer.RemainingItems - model.RemainingItems;
            }
            if (model.MedicalItemTransferDetailTransferGUID != null)
            {
                var modelTransfer = DbEMT.dataMedicalItemTransferDetail.Where(x => x.MedicalItemTransferDetailGUID == model.MedicalItemTransferDetailTransferGUID).FirstOrDefault();
                modelTransfer.RemainingItems = modelTransfer.RemainingItems - model.RemainingItems.Value;
            }
            else
            {
                //update Medical Item Input Supply Remaining Items
                model.dataMedicalItemInputSupplyDetail.RemainingItems = model.dataMedicalItemInputSupplyDetail.RemainingItems - model.RemainingItems;
            }
        }

        public void RemoveItemQuantity(Guid MedicalItemInputDetailGUID)
        {
            var model = DbEMT.dataMedicalItemInputDetail.Where(x => x.MedicalItemInputDetailGUID == MedicalItemInputDetailGUID).FirstOrDefault();
            //remove the QuantityByPackingUnit from  RemainingItemsQuantity
            codeMedicalItem item = DbEMT.codeMedicalItem.Where(x => x.MedicalItemGUID == model.MedicalItemGUID).FirstOrDefault();
            item.RemainingItemsQuantity = item.RemainingItemsQuantity - model.QuantityByPackingUnit;
            item.TotalDispatchedItems = item.TotalDispatchedItems - model.QuantityByPackingUnit;

            dataItemQuantityThreshold itemQuantityThreshold = DbEMT.dataItemQuantityThreshold.Where(x => x.MedicalItemGUID == model.MedicalItemGUID && x.MedicalPharmacyGUID == model.dataMedicalItemInput.MedicalPharmacyGUID && x.Active).FirstOrDefault();
            itemQuantityThreshold.QuantityTotalRemainingItems = itemQuantityThreshold.QuantityTotalRemainingItems - model.QuantityByPackingUnit;

            //In case transfer from other  Item Input Detail
            if (model.MedicalItemInputDetailTransferGUID != null)
            {
                var modelTransfer = DbEMT.dataMedicalItemInputDetail.Where(x => x.MedicalItemInputDetailGUID == model.MedicalItemInputDetailTransferGUID).FirstOrDefault();
                modelTransfer.RemainingItems = modelTransfer.RemainingItems + model.RemainingItems;
            }
            if (model.MedicalItemTransferDetailTransferGUID != null)
            {
                var modelTransfer = DbEMT.dataMedicalItemTransferDetail.Where(x => x.MedicalItemTransferDetailGUID == model.MedicalItemTransferDetailTransferGUID).FirstOrDefault();
                modelTransfer.RemainingItems = modelTransfer.RemainingItems + model.RemainingItems.Value;
            }
            else
            {
                //update Medical Item Input Supply Remaining Items
                model.dataMedicalItemInputSupplyDetail.RemainingItems = model.dataMedicalItemInputSupplyDetail.RemainingItems + model.RemainingItems;
            }
        }

        #endregion

        #region Transfer
        public ActionResult MedicalItemInputOverViewTransferCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.MedicalItemInput.Create, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var Pharmacy = DbEMT.codeMedicalPharmacy.Where(x => x.MedicalPharmacyGUID == FK).FirstOrDefault();
            Guid UNHCR = Guid.Parse("E156C022-EC72-4A5A-BE09-163BD85C68EF");
            var model = new MedicalItemTransferUpdateModel()
            {
                MedicalPharmacyGUID = Pharmacy.MedicalPharmacyGUID,
                OrganizationInstanceGUID = Pharmacy.OrganizationInstanceGUID,
                ProvidedByOrganizationInstanceGUID = UNHCR,
                MedicalItemInputDetailsDataTableModel = (from a in DbEMT.dataMedicalItemInputDetail.Where(x => x.dataMedicalItemInput.MedicalPharmacyGUID == FK && x.Confirmed && x.Active && x.RemainingItems > 0)
                                                         join b in DbEMT.codeMedicalItem on a.MedicalItemGUID equals b.MedicalItemGUID
                                                         orderby b.BrandName
                                                         select
            new MedicalItemInputDetailsDataTableModel
            {
                BrandName = b.BrandName,
                MedicalItemInputDetailGUID = a.MedicalItemInputDetailGUID,
                BatchNumber = a.BatchNumber,
                QuantityByPackingUnit = 0,
                RemainingItems = a.RemainingItems,
                MedicalItemGUID = a.MedicalItemGUID.ToString(),
                ExpirationDate = a.ExpirationDate
            }
                ).ToList()
            };
            return PartialView("~/Areas/EMT/Views/MedicalItemInputs/_MedicalItemInputTransferUpdateModal.cshtml", model);

        }
        public ActionResult MedicalItemInputTransferCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.MedicalItemInput.Create, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var Pharmacy = DbEMT.dataMedicalItemInputDetail.Where(x => x.MedicalItemInputGUID == FK).FirstOrDefault().dataMedicalItemInput.codeMedicalPharmacy;
            var MedicalItemInput = DbEMT.dataMedicalItemInput.Where(x => x.MedicalItemInputGUID == FK).FirstOrDefault();
            var model = new MedicalItemTransferUpdateModel()
            {
                MedicalPharmacyGUID = Pharmacy.MedicalPharmacyGUID,
                OrganizationInstanceGUID = Pharmacy.OrganizationInstanceGUID,
                ProvidedByOrganizationInstanceGUID = MedicalItemInput.ProvidedByOrganizationInstanceGUID.Value,
                MedicalItemInputDetailsDataTableModel = (from a in DbEMT.dataMedicalItemInputDetail.Where(x => x.MedicalItemInputGUID == FK && x.Confirmed && x.Active && x.RemainingItems != 0)
                                                         join b in DbEMT.codeMedicalItem on a.MedicalItemGUID equals b.MedicalItemGUID
                                                         orderby b.BrandName
                                                         select
            new MedicalItemInputDetailsDataTableModel
            {
                BrandName = b.BrandName,
                MedicalItemInputDetailGUID = a.MedicalItemInputDetailGUID,
                BatchNumber = a.BatchNumber,
                QuantityByPackingUnit = 0,
                RemainingItems = a.RemainingItems,
                MedicalItemGUID = a.MedicalItemGUID.ToString(),
                ExpirationDate=a.ExpirationDate
            }
                ).ToList()
            };
            return PartialView("~/Areas/EMT/Views/MedicalItemInputs/_MedicalItemInputTransferUpdateModal.cshtml", model);

        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalItemInputTransferCreate(MedicalItemTransferUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.MedicalItemInput.Create, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveMedicalItemInput(model)) return PartialView("~/Areas/EMT/Views/MedicalItemInputs/_MedicalItemInputTransferUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            PrimaryKeyControl primaryKeyControl = new PrimaryKeyControl();
            List<RowVersionControl> RowVersionControl = new List<RowVersionControl>();
            //Transfer from warehouse to pharmacy
            var MedicalItemTransferPharmacy = new dataMedicalItemTransfer();
            if (!DbEMT.codeMedicalPharmacy.Where(x => x.MedicalPharmacyGUID == model.MedicalPharmacyGUID).FirstOrDefault().MainWarehouse)
            {
                MedicalItemTransferPharmacy = TransferToPharmacy(model, ExecutionTime);
            }
            //Transfer from warehouse to warehouse
            var MedicalItemTransferWarehouse = new dataMedicalItemInput();
            if (DbEMT.codeMedicalPharmacy.Where(x => x.MedicalPharmacyGUID == model.MedicalPharmacyGUID).FirstOrDefault().MainWarehouse)
            {
                MedicalItemTransferWarehouse = TransferToWarehouse(model, ExecutionTime);
            }

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleCreateMessage(new PrimaryKeyControl(), null, null, "", null));
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        private dataMedicalItemInput TransferToWarehouse(MedicalItemTransferUpdateModel model, DateTime ExecutionTime)
        {
            //dataMedicalItemInput MedicalItemInput = new dataMedicalItemInput(); //DbEMT.dataMedicalItemInput.Where(x => x.ProcuredByOrganizationInstanceGUID == model.OrganizationInstanceGUID && x.DeliveryDate == model.DeliveryDate && x.MedicalPharmacyGUID == model.MedicalPharmacyGUID && x.Active).FirstOrDefault();
            Guid EntityPK = Guid.NewGuid();

                dataMedicalItemInput MedicalItemInput = Mapper.Map(model, new dataMedicalItemInput());
                MedicalItemInput.MedicalItemInputGUID = EntityPK;
                MedicalItemInput.ProvidedByOrganizationInstanceGUID = model.ProvidedByOrganizationInstanceGUID;
                MedicalItemInput.ProcuredByOrganizationInstanceGUID = DbEMT.codeMedicalPharmacy.Where(x => x.MedicalPharmacyGUID == model.MedicalPharmacyGUID).FirstOrDefault().OrganizationInstanceGUID;
                DbEMT.Create(MedicalItemInput, Permissions.MedicalItemInput.CreateGuid, ExecutionTime, DbCMS);

            foreach (var item in model.MedicalItemInputDetailsDataTableModel)
            {
                if (item.QuantityByPackingUnit != 0)
                {
                    var ItemPacket = DbEMT.codeMedicalItem.Where(x => x.MedicalItemGUID.ToString() == item.MedicalItemGUID).FirstOrDefault();
                    dataMedicalItemInputDetail Source = DbEMT.dataMedicalItemInputDetail.AsNoTracking().Where(x => x.MedicalItemInputDetailGUID == item.MedicalItemInputDetailGUID).FirstOrDefault();
                    Source.RemainingItems = Source.RemainingItems - item.QuantityByPackingUnit;
                    DbEMT.Update(Source, Permissions.MedicalItemInput.UpdateGuid, ExecutionTime, DbCMS);
                    dataMedicalItemInputDetail Destination = DbEMT.dataMedicalItemInputDetail.AsNoTracking().Where(x => x.MedicalItemGUID.ToString() == item.MedicalItemGUID && x.BatchNumber == item.BatchNumber && x.PriceOfSmallestUnit == x.PriceOfSmallestUnit && x.Active && x.MedicalItemInputGUID == EntityPK).FirstOrDefault();
                    int quantity = Source.QuantityByPackingUnit != 0 ? Source.QuantityByPackingUnit : Source.QuantityByPackingTransfer.Value;

                    if (Destination == null)
                    {
                        Destination = new dataMedicalItemInputDetail();
                        Destination.MedicalItemInputDetailGUID = Guid.NewGuid();
                        Destination.Active = true;
                        Destination.BatchNumber = Source.BatchNumber;
                        Destination.PriceOfPackingUnit = Source.PriceOfPackingUnit;
                        Destination.PriceOfSmallestUnit = Source.PriceOfSmallestUnit;
                        Destination.Comments = Source.Comments;
                        Destination.Confirmed = false;
                        Destination.ConfirmedBy = null;
                        Destination.ConfirmedDate = null;
                        Destination.ExpirationDate = Source.ExpirationDate;
                        Destination.ManufacturingDate = Source.ManufacturingDate;
                        Destination.MedicalItemGUID = Source.MedicalItemGUID;
                        Destination.MedicalItemInputSupplyDetailGUID = Source.MedicalItemInputSupplyDetailGUID;
                        Destination.MedicalItemInputDetailTransferGUID = Source.MedicalItemInputDetailGUID;
                        Destination.QuantityByPackingTransfer = item.QuantityByPackingUnit;
                        Destination.RemainingItems = item.QuantityByPackingUnit;

                        Destination.QuantityBySmallestUnit = Convert.ToInt32(item.QuantityByPackingUnit * (Source.QuantityBySmallestUnit / quantity));

                        Destination.MedicalItemInputGUID = EntityPK;
                        DbEMT.Create(Destination, Permissions.MedicalItemInput.CreateGuid, ExecutionTime, DbCMS);
                    }
                    else
                    {
                        Destination.QuantityBySmallestUnit = Destination.QuantityBySmallestUnit + Convert.ToInt32(item.QuantityByPackingUnit * (Source.QuantityBySmallestUnit / quantity));
                        Destination.QuantityByPackingUnit = Destination.QuantityByPackingUnit + item.QuantityByPackingUnit;
                        Destination.RemainingItems = Destination.RemainingItems + item.QuantityByPackingUnit;
                    }

                    //Check If the Item Quantity Threshold exist
                    dataItemQuantityThreshold itemQuantityThreshold = DbEMT.dataItemQuantityThreshold.Where(x => x.MedicalItemGUID.ToString() == item.MedicalItemGUID && x.MedicalPharmacyGUID == MedicalItemInput.MedicalPharmacyGUID && x.Active).FirstOrDefault();
                    if (itemQuantityThreshold == null)
                    {
                        itemQuantityThreshold = new dataItemQuantityThreshold();
                        itemQuantityThreshold.ItemQuantityThresholdGUID = Guid.NewGuid();
                        itemQuantityThreshold.MedicalItemGUID = Guid.Parse(item.MedicalItemGUID);
                        itemQuantityThreshold.QuantityThreshold = 0;
                        itemQuantityThreshold.QuantityTotalRemainingItems = item.QuantityByPackingUnit;
                        itemQuantityThreshold.MedicalPharmacyGUID = MedicalItemInput.MedicalPharmacyGUID.Value;
                        DbEMT.Create(itemQuantityThreshold, Permissions.MedicalItem.CreateGuid, ExecutionTime, DbCMS);
                        DbEMT.SaveChanges();
                    }
                    else
                    {
                        itemQuantityThreshold.QuantityTotalRemainingItems = itemQuantityThreshold.QuantityTotalRemainingItems + item.QuantityByPackingUnit;
                    }
                    //update Item (Quantity Threshold and Input Detail) which we transfer the items <from> it.
                    dataItemQuantityThreshold itemQuantityThresholdFrom = DbEMT.dataItemQuantityThreshold.Where(x => x.MedicalItemGUID.ToString() == item.MedicalItemGUID && x.MedicalPharmacyGUID == Source.dataMedicalItemInput.MedicalPharmacyGUID && x.Active).FirstOrDefault();
                    itemQuantityThresholdFrom.QuantityTotalRemainingItems = itemQuantityThresholdFrom.QuantityTotalRemainingItems - item.QuantityByPackingUnit;


                }
            }

            return MedicalItemInput;
        }

        private dataMedicalItemTransfer TransferToPharmacy(MedicalItemTransferUpdateModel model, DateTime ExecutionTime)
        {
            //dataMedicalItemTransfer MedicalItemTransfer = new dataMedicalItemTransfer();//DbEMT.dataMedicalItemTransfer.Where(x => x.MedicalPharmacyGUID == model.MedicalPharmacyGUID && x.DeliveryDate == model.DeliveryDate && x.Active).FirstOrDefault();
            Guid EntityPK = Guid.NewGuid();

            dataMedicalItemTransfer MedicalItemTransfer = Mapper.Map(model, new dataMedicalItemTransfer());
            MedicalItemTransfer.MedicalItemTransferGUID = EntityPK;
            DbEMT.Create(MedicalItemTransfer, Permissions.MedicalItemInput.CreateGuid, ExecutionTime, DbCMS);

            foreach (var item in model.MedicalItemInputDetailsDataTableModel)
            {
                if (item.QuantityByPackingUnit > 0)
                {
                    var Source = DbEMT.dataMedicalItemInputDetail.Where(x => x.MedicalItemInputDetailGUID == item.MedicalItemInputDetailGUID).FirstOrDefault();
                    Source.RemainingItems = Source.RemainingItems - item.QuantityByPackingUnit;

                    dataMedicalItemTransferDetail Destination = DbEMT.dataMedicalItemTransferDetail.Where(x => x.MedicalItemGUID.ToString() == item.MedicalItemGUID && x.MedicalItemTransferGUID == EntityPK && x.Active).FirstOrDefault();
                    if (Destination == null)
                    {
                        Destination = new dataMedicalItemTransferDetail();
                        Mapper.Map(Source, Destination);
                        Destination.MedicalItemTransferDetailTransferGUID = null;
                        Destination.QuantityByPackingTransfer = 0;
                        Destination.QuantityByPackingUnit = item.QuantityByPackingUnit;
                        Destination.RemainingItems = item.QuantityByPackingUnit;
                        Destination.Confirmed = null;
                        Destination.ConfirmedBy = null;
                        Destination.ConfirmedOn = null;
                        Destination.MedicalItemTransferGUID = EntityPK;

                        DbEMT.Create(Destination, Permissions.MedicalItemInput.CreateGuid, ExecutionTime, DbCMS);
                    }
                    else
                    {
                        //Destination.QuantityByPackingTransfer = 0;
                        Destination.QuantityByPackingUnit = Destination.QuantityByPackingUnit + item.QuantityByPackingUnit;
                        Destination.RemainingItems = Destination.RemainingItems + item.QuantityByPackingUnit;
                    }

                    //Check If the Item Quantity Threshold exist
                    dataItemQuantityThreshold itemQuantityThreshold = DbEMT.dataItemQuantityThreshold.Where(x => x.MedicalItemGUID.ToString() == item.MedicalItemGUID && x.MedicalPharmacyGUID == model.MedicalPharmacyGUID && x.Active).FirstOrDefault();
                    if (itemQuantityThreshold == null)
                    {
                        itemQuantityThreshold = new dataItemQuantityThreshold();
                        itemQuantityThreshold.ItemQuantityThresholdGUID = Guid.NewGuid();
                        itemQuantityThreshold.MedicalItemGUID = Guid.Parse(item.MedicalItemGUID);
                        itemQuantityThreshold.QuantityThreshold = 0;
                        itemQuantityThreshold.QuantityTotalRemainingItems = item.QuantityByPackingUnit;
                        itemQuantityThreshold.MedicalPharmacyGUID = model.MedicalPharmacyGUID;

                        DbEMT.Create(itemQuantityThreshold, Permissions.MedicalItem.CreateGuid, ExecutionTime, DbCMS);
                        DbEMT.SaveChanges();
                    }

                    else
                    {
                        itemQuantityThreshold.QuantityTotalRemainingItems = itemQuantityThreshold.QuantityTotalRemainingItems + item.QuantityByPackingUnit;
                    }
                    //update Item (Quantity Threshold and Input Detail) which we transfer the items <from> it.
                    dataItemQuantityThreshold itemQuantityThresholdFrom = DbEMT.dataItemQuantityThreshold.Where(x => x.MedicalItemGUID.ToString() == item.MedicalItemGUID && x.MedicalPharmacyGUID == Source.dataMedicalItemInput.MedicalPharmacyGUID && x.Active).FirstOrDefault();
                    itemQuantityThresholdFrom.QuantityTotalRemainingItems = itemQuantityThresholdFrom.QuantityTotalRemainingItems - item.QuantityByPackingUnit;
                }
            }
            return MedicalItemTransfer;
        }
        #endregion

        #region Confirm 
        [Route("EMT/MedicalItemInputs/Confirm/{PK}")]
        public ActionResult MedicalItemInputConfirmCreate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.MedicalItemInput.Confirm, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var model = new MedicalItemInputUpdateModel()
            {
                MedicalItemInputGUID = PK,
                MedicalItemInputDetailsDataTableModel = (from a in DbEMT.dataMedicalItemInputDetail.Where(x => x.MedicalItemInputGUID == PK && x.Active)
                                                         join b in DbEMT.codeMedicalItem on a.MedicalItemGUID equals b.MedicalItemGUID
                                                         orderby b.BrandName
                                                         select
            new MedicalItemInputDetailsDataTableModel
            {
                BrandName = b.BrandName,
                QuantityByPackingTransfer = a.QuantityByPackingTransfer == null ? 0 : a.QuantityByPackingTransfer.Value,
                MedicalItemInputDetailGUID = a.MedicalItemInputDetailGUID,
                BatchNumber = a.BatchNumber,
                QuantityByPackingUnit = a.QuantityByPackingUnit,
                RemainingItems = a.RemainingItems,
                MedicalItemGUID = a.MedicalItemGUID.ToString(),
                Comments = a.Comments,
                Confirmed = a.Confirmed,
                ConfirmedBy = a.ConfirmedBy,
                ConfirmedDate = a.ConfirmedDate,
                ExpirationDate = a.ExpirationDate,
                ManufacturingDate = a.ManufacturingDate,
                MedicalItemInputGUID = a.MedicalItemInputGUID,
                PriceOfPackingUnit = a.PriceOfPackingUnit,
                PriceOfSmallestUnit = a.PriceOfSmallestUnit,
                QuantityBySmallestUnit = a.QuantityBySmallestUnit
            }
                ).ToList()
            };
            return View("~/Areas/EMT/Views/MedicalItemInputs/_MedicalItemInputDetailConfirm.cshtml", model);

        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalItemInputConfirmCreate(MedicalItemInputUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.MedicalItemInput.Create, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            //if (!ModelState.IsValid || ActiveMedicalItemInput(model)) return PartialView("~/Areas/EMT/Views/MedicalItemInputs/_MedicalItemInputDetailConfirm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;
            dataMedicalItemInput MedicalItemInput = DbEMT.dataMedicalItemInput.Where(x => x.MedicalItemInputGUID == model.MedicalItemInputGUID).FirstOrDefault();

            foreach (var item in model.MedicalItemInputDetailsDataTableModel)
            {
                if (item.MedicalItemInputDetailGUID != Guid.Empty)
                {
                    var Source = DbEMT.dataMedicalItemInputDetail.Where(x => x.MedicalItemInputDetailGUID == item.MedicalItemInputDetailGUID).FirstOrDefault();

                    Source.Confirmed = item.Confirmed;
                    Source.ConfirmedBy = Source.ConfirmedBy == null && item.Confirmed ? CMS.GetFullName(UserGUID, "EN") : Source.ConfirmedBy;
                    Source.ConfirmedDate = Source.ConfirmedDate == null && item.Confirmed ? ExecutionTime : Source.ConfirmedDate;
                    MedicalItemInput.ConfirmedReceived = item.Confirmed;

                    Source.Comments = item.Comments;
                    DbEMT.Update(Source, Permissions.MedicalItemInput.UpdateGuid, ExecutionTime, DbCMS);
                }
            }
            DbEMT.Update(MedicalItemInput, Permissions.MedicalItemInput.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleCreateMessage(DbEMT.PrimaryKeyControl(MedicalItemInput), DbEMT.RowVersionControls(new List<dataMedicalItemInput> { MedicalItemInput }), null, "" +
                    " setTimeout(function () { window.location.href = '/EMT/MedicalItemInputs/Confirm/" + model.MedicalItemInputGUID + "';}, 2000); ", null));
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }
        #endregion

        #region Transfer History
        public JsonResult LoadingWarehouseTransferSource(Guid PK)
        {
            List<TransferDetailsDataTableModel> transfers = new List<TransferDetailsDataTableModel>();
            var Record = DbEMT.dataMedicalItemInputDetail.Where(x => x.MedicalItemInputDetailGUID == PK).FirstOrDefault();
            //Pharmacy Source
               var Result = (from a in DbEMT.dataMedicalItemTransferDetail.Where(x => x.MedicalItemTransferDetailGUID == Record.MedicalItemTransferDetailTransferGUID && x.Active && x.MedicalItemTransferDetailTransferGUID == null)
                          join b in DbEMT.codeMedicalItem on a.MedicalItemGUID equals b.MedicalItemGUID
                          join c in DbEMT.codeMedicalPharmacyLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.dataMedicalItemTransfer.MedicalPharmacyGUID equals c.MedicalPharmacyGUID
                          join d in DbEMT.dataMedicalItemInputDetail.Where(x => x.MedicalItemInputDetailGUID == PK) on a.MedicalItemTransferDetailGUID equals d.MedicalItemTransferDetailTransferGUID

                          select new TransferDetailsDataTableModel
                          {
                              QuantityByPackingUnit = a.QuantityByPackingUnit,
                              QuantityByPackingTransferUnit = d.QuantityByPackingTransfer != null ? d.QuantityByPackingTransfer.Value : 0,
                              RemainingItems = a.RemainingItems,
                              Pharmacy = c.MedicalPharmacyDescription,
                              DeliveryDate = d.dataMedicalItemInput.DeliveryDate.Value
                          }).ToList();
            //Warehouse Source
              var Result1 = (from a in DbEMT.dataMedicalItemInputDetail.AsNoTracking().AsExpandable().Where(x =>  x.Active)
                           join b in DbEMT.codeMedicalItem on a.MedicalItemGUID equals b.MedicalItemGUID
                           join c in DbEMT.codeMedicalPharmacyLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.dataMedicalItemInput.MedicalPharmacyGUID equals c.MedicalPharmacyGUID
                           join d in DbEMT.dataMedicalItemInputDetail.Where(x => x.MedicalItemInputDetailGUID == PK) on a.MedicalItemInputDetailGUID equals d.MedicalItemInputDetailTransferGUID

                            select new TransferDetailsDataTableModel
                            {
                               QuantityByPackingUnit = a.QuantityByPackingUnit,
                               QuantityByPackingTransferUnit = d.QuantityByPackingTransfer != null ? d.QuantityByPackingTransfer.Value : 0,
                               RemainingItems = a.RemainingItems.Value,
                               Pharmacy = c.MedicalPharmacyDescription,
                               DeliveryDate = d.dataMedicalItemInput.DeliveryDate.Value
                            }).ToList();

            var Result2 = (from a in DbEMT.dataMedicalItemInputSupplyDetail.AsNoTracking().AsExpandable().Where(x => x.MedicalItemInputSupplyDetailGUID == Record.MedicalItemInputSupplyDetailGUID && x.Active)
                           join b in DbEMT.codeMedicalItem on a.MedicalItemGUID equals b.MedicalItemGUID
                           join c in DbEMT.dataMedicalItemInputDetail.Where(x => x.MedicalItemInputDetailGUID == PK) on a.MedicalItemInputSupplyDetailGUID equals c.MedicalItemInputSupplyDetailGUID
                           select new TransferDetailsDataTableModel
                           {
                               QuantityByPackingUnit = a.QuantityByPackingUnit,
                               QuantityByPackingTransferUnit =  c.QuantityByPackingUnit ,
                               RemainingItems = a.RemainingItems != null ? a.RemainingItems.Value : 0,
                               Pharmacy = "UNHCR Supply",
                               DeliveryDate = c.dataMedicalItemInput.DeliveryDate.Value
                           }).ToList();
            transfers.AddRange(Result);
            transfers.AddRange(Result1);
            transfers.AddRange(Result2);
            var all = transfers.OrderBy(x => x.DeliveryDate).ToList();
            return Json(new { transfers = all }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult LoadingWarehouseTransfer(Guid PK)
        {
            List<TransferDetailsDataTableModel> transfers = new List<TransferDetailsDataTableModel>();
            var Result = (from a in DbEMT.dataMedicalItemTransferDetail.Where(x => x.MedicalItemInputDetailGUID == PK && x.Active  && x.MedicalItemTransferDetailTransferGUID ==null)
                          join b in DbEMT.codeMedicalItem on a.MedicalItemGUID equals b.MedicalItemGUID
                          join c in DbEMT.codeMedicalPharmacyLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.dataMedicalItemTransfer.MedicalPharmacyGUID equals c.MedicalPharmacyGUID
                          select new TransferDetailsDataTableModel
                          {
                              QuantityByPackingUnit = a.QuantityByPackingUnit,
                              QuantityByPackingTransferUnit = a.QuantityByPackingTransfer,
                              RemainingItems = a.RemainingItems,
                              Pharmacy = c.MedicalPharmacyDescription,
                              DeliveryDate = a.dataMedicalItemTransfer.DeliveryDate
                          }).ToList();

            var Result1 = (from a in DbEMT.dataMedicalItemInputDetail.AsNoTracking().AsExpandable().Where(x => x .MedicalItemInputDetailTransferGUID == PK && x.Active)
                           join b in DbEMT.codeMedicalItem on a.MedicalItemGUID equals b.MedicalItemGUID
                           join c in DbEMT.codeMedicalPharmacyLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.dataMedicalItemInput.MedicalPharmacyGUID equals c.MedicalPharmacyGUID

                           select new TransferDetailsDataTableModel
                           {
                               QuantityByPackingUnit = 0,
                               QuantityByPackingTransferUnit = a.QuantityByPackingTransfer != null ? a.QuantityByPackingTransfer.Value : 0,
                               RemainingItems = a.RemainingItems.Value,
                               Pharmacy = c.MedicalPharmacyDescription,
                               DeliveryDate = a.dataMedicalItemInput.DeliveryDate.Value
                           }).ToList();

            var Result2 = (from a in DbEMT.dataMedicalDiscrepancyDetail.AsNoTracking().AsExpandable().Where(x => x.ReferenceItemGUID == PK && x.Active)
                           join b in DbEMT.codeMedicalItem on a.MedicalItemGUID equals b.MedicalItemGUID

                           select new TransferDetailsDataTableModel
                           {
                               QuantityByPackingUnit =(int) a.DiscrepancyQuantity,
                               QuantityByPackingTransferUnit = 0,
                               RemainingItems = 0,
                               Pharmacy = LAN=="EN"?"Inventory ":" جرد المستوع" +a.dataMedicalDiscrepancy.DiscrepancyDate,
                               DeliveryDate = a.dataMedicalDiscrepancy.DiscrepancyDate
                           }).ToList();
            transfers.AddRange(Result);
            transfers.AddRange(Result1);
            transfers.AddRange(Result2);
            var all = transfers.OrderBy(x => x.DeliveryDate).ToList();
            return Json(new { transfers = all }, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}