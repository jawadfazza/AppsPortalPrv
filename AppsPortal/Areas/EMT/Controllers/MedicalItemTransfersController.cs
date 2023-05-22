using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.ViewModels;
using AutoMapper;
using EMT_DAL.Model;
using iTextSharp.text.pdf.qrcode;
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
    public class MedicalItemTransfersController : EMTBaseController
    {
        #region Medical Item Transfer

        public ActionResult Index()
        {
            return View();
        }

        [Route("EMT/MedicalItemTransfers/")]
        public ActionResult MedicalItemTransfersIndex()
        {
            if (!CMS.HasAction(Permissions.MedicalItemTransfer.Access, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/EMT/Views/MedicalItemTransfers/Index.cshtml");
        }

        [Route("EMT/MedicalItemTransfersDataTable/")]
        public JsonResult MedicalItemTransfersDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<MedicalItemTransfersDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<MedicalItemTransfersDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.MedicalPharmacy.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();

            var All = (from a in DbEMT.dataMedicalItemTransfer.AsExpandable()
                       join d in DbEMT.codeMedicalPharmacyLanguage.Where(x => x.Active && x.LanguageID == LAN).Where(x => AuthorizedList.Contains(x.MedicalPharmacyGUID.ToString())) on a.MedicalPharmacyGUID equals d.MedicalPharmacyGUID
                       join b in DbEMT.codeOrganizationsInstances on d.codeMedicalPharmacy.OrganizationInstanceGUID equals b.OrganizationInstanceGUID
                       join c in DbEMT.codeOrganizations on b.OrganizationGUID equals c.OrganizationGUID
                       select new MedicalItemTransfersDataTableModel
                       {
                           MedicalItemTransferGUID = a.MedicalItemTransferGUID,
                           ConfirmedReceived = a.ConfirmedReceived,
                           DeliveryDate = a.DeliveryDate,
                           MedicalPharmacyGUID = a.MedicalPharmacyGUID.ToString(),
                           MedicalPharmacyDescription = d.MedicalPharmacyDescription,
                           OrganizationInstanceDescription = c.OrganizationShortName,
                           OrganizationInstanceGUID = b.OrganizationInstanceGUID.ToString(),
                           ItemsConfirmedCount = a.dataMedicalItemTransferDetail.Where(x => x.Confirmed.Value).Count(),
                           Active = a.Active,
                           dataMedicalItemTransferRowVersion = a.dataMedicalItemTransferRowVersion
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<MedicalItemTransfersDataTableModel> Result = Mapper.Map<List<MedicalItemTransfersDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("EMT/MedicalPharmacyDataTable/")]
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

            var All = (from a in DbEMT.codeMedicalPharmacy.AsExpandable().Where(x => !x.MainWarehouse && x.Active).Where(x => AuthorizedList.Contains(x.MedicalPharmacyGUID.ToString()))
                       join b in DbEMT.codeMedicalPharmacyLanguage.Where(x => x.Active  && x.LanguageID == LAN) on a.MedicalPharmacyGUID equals b.MedicalPharmacyGUID into LJ1
                       from R1 in LJ1.DefaultIfEmpty()
                       join c in DbEMT.codeOrganizationsInstancesLanguages.Where(x => x.Active  && x.LanguageID == LAN) on a.OrganizationInstanceGUID equals c.OrganizationInstanceGUID into LJ2
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

        [Route("EMT/MedicalItemTransfers/View/{PK}")]
        public ActionResult MedicalItemTransferView(Guid PK)
        {
            if (!CMS.HasAction(Permissions.MedicalItemInput.Access, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/EMT/Views/MedicalItemTransfers/MedicalItemTransferDetailsDataTable.cshtml", new MasterRecordStatus() { ParentGUID = PK });
        }

        public ActionResult MedicalItemTransferDetailsViewDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/EMT/Views/MedicalItemTransfers/MedicalItemTransferDetailsDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<MedicalItemInputDetailsDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<MedicalItemInputDetailsDataTableModel>(DataTable.Filters);
            }

            var All = (from a in DbEMT.dataMedicalItemTransferDetail.AsNoTracking().AsExpandable().Where(x => x.dataMedicalItemTransfer.MedicalPharmacyGUID == PK && x.Active)
                       join b in DbEMT.codeMedicalItem on a.MedicalItemGUID equals b.MedicalItemGUID
                       select new MedicalItemInputDetailsDataTableModel
                       {
                           MedicalItemTransferDetailGUID = a.MedicalItemTransferDetailGUID,
                           QuantityByPackingTransfer = a.QuantityByPackingTransfer != null ? a.QuantityByPackingTransfer.Value : 0,
                           BatchNumber = a.dataMedicalItemInputDetail.BatchNumber,
                           ExpirationDate = a.dataMedicalItemInputDetail.ExpirationDate,
                           PriceOfSmallestUnit = a.dataMedicalItemInputDetail.PriceOfSmallestUnit,
                           PriceOfPackingUnit = a.dataMedicalItemInputDetail.PriceOfPackingUnit,
                           QuantityByPackingUnit = a.QuantityByPackingUnit != 0 ? a.QuantityByPackingUnit : a.QuantityByPackingTransfer.Value,
                           QuantityBySmallestUnit = a.dataMedicalItemInputDetail.QuantityBySmallestUnit,
                           RemainingItems = a.RemainingItems,
                           MedicalItemGUID = a.MedicalItemGUID.ToString(),
                           BrandName = b.BrandName + ", " + a.dataMedicalItemInputDetail.codeMedicalItem.DoseQuantity,
                           MedicalGenericNameDescription = b.codeMedicalGenericName.codeMedicalGenericNameLanguage.Where(x => x.LanguageID == LAN && x.Active).FirstOrDefault().MedicalGenericNameDescription.Replace("+", " "),
                           MedicalGenericNameGUID = b.codeMedicalGenericName.MedicalGenericNameGUID.ToString(),
                           ManufacturingDate = a.dataMedicalItemInputDetail.ManufacturingDate,
                           MedicalItemInputGUID = a.dataMedicalItemInputDetail.MedicalItemInputGUID,
                           DeliveryDate = a.dataMedicalItemTransfer.DeliveryDate,
                           Active = a.Active,
                           dataMedicalItemTransferDetailRowVersion = a.dataMedicalItemTransferDetailRowVersion
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<MedicalItemInputDetailsDataTableModel> Result = Mapper.Map<List<MedicalItemInputDetailsDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadingPharmacyTransfer(Guid PK)
        {
            List<TransferDetailsDataTableModel> transfers = new List<TransferDetailsDataTableModel>();
            var Result = (from a in DbEMT.dataMedicalItemTransferDetail.Where(x => x.MedicalItemTransferDetailTransferGUID == PK && x.Active)
                          join b in DbEMT.codeMedicalItem on a.MedicalItemGUID equals b.MedicalItemGUID
                          join c in DbEMT.codeMedicalPharmacyLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.dataMedicalItemTransfer.MedicalPharmacyGUID equals c.MedicalPharmacyGUID
                          select new TransferDetailsDataTableModel
                          {
                              QuantityByPackingUnit = a.QuantityByPackingUnit,
                              QuantityByPackingTransferUnit = a.QuantityByPackingUnit == 0 ? a.QuantityByPackingTransfer.Value : a.QuantityByPackingUnit,
                              RemainingItems = a.RemainingItems,
                              Pharmacy = c.MedicalPharmacyDescription,
                              DeliveryDate = a.dataMedicalItemTransfer.DeliveryDate
                          }).ToList();

            var Result1 = (from a in DbEMT.dataMedicalItemInputDetail.AsNoTracking().AsExpandable().Where(x => x.MedicalItemTransferDetailTransferGUID == PK && x.Active)
                           join b in DbEMT.codeMedicalItem on a.MedicalItemGUID equals b.MedicalItemGUID
                           join c in DbEMT.codeMedicalPharmacyLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.dataMedicalItemInput.MedicalPharmacyGUID equals c.MedicalPharmacyGUID

                           select new TransferDetailsDataTableModel
                           {
                               QuantityByPackingUnit = a.QuantityByPackingUnit,
                               QuantityByPackingTransferUnit = a.QuantityByPackingTransfer != null ? a.QuantityByPackingTransfer.Value : 0,
                               RemainingItems = a.RemainingItems.Value,
                               Pharmacy = c.MedicalPharmacyDescription,
                               DeliveryDate = a.dataMedicalItemInput.DeliveryDate.Value
                           }).ToList();
            var Result2 = (from a in DbEMT.dataMedicalDiscrepancyDetail.AsNoTracking().AsExpandable().Where(x => x.ReferenceItemGUID == PK && x.Active)
                           join b in DbEMT.codeMedicalItem on a.MedicalItemGUID equals b.MedicalItemGUID

                           select new TransferDetailsDataTableModel
                           {
                               QuantityByPackingUnit = 0,
                               QuantityByPackingTransferUnit = (int)a.DiscrepancyQuantity,
                               RemainingItems = 0,
                               Pharmacy = "Inventory" ,
                               DeliveryDate = a.dataMedicalDiscrepancy.DiscrepancyDate
                           }).ToList();
            transfers.AddRange(Result);
            transfers.AddRange(Result1);
            transfers.AddRange(Result2);
            var all = transfers.OrderBy(x => x.DeliveryDate).ToList();
            return Json(new { transfers = all }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadingBeneficiaryDispensing(Guid PK)
        {
            List<TransferDetailsDataTableModel> transfers = new List<TransferDetailsDataTableModel>();
            var Result = (from a in DbEMT.dataMedicalBeneficiaryItemOutDetail.Where(x => x.MedicalItemTransferDetailGUID == PK && x.Active)
                          join b in DbEMT.codeMedicalItem on a.MedicalItemGUID equals b.MedicalItemGUID
                          join c in DbEMT.dataMedicalBeneficiary.Where(x => x.Active) on a.dataMedicalBeneficiaryItemOut.MedicalBeneficiaryGUID equals c.MedicalBeneficiaryGUID
                          select new TransferDetailsDataTableModel
                          {
                              QuantityByPackingUnit = (int)a.QuantityByPackingUnit,
                              Text = (c.UNHCRNumber != null ? c.UNHCRNumber : c.IDNumber) + " - " + c.RefugeeFullName,
                              DeliveryDate = a.dataMedicalBeneficiaryItemOut.DeliveryDate
                          }).ToList();


            transfers.AddRange(Result);
            var all = transfers.OrderBy(x => x.DeliveryDate).ToList();
            return Json(new { transfers = all }, JsonRequestBehavior.AllowGet);
        }

        [Route("EMT/MedicalItemTransfers/Create/")]
        public ActionResult MedicalItemTransferCreate()
        {
            if (!CMS.HasAction(Permissions.MedicalItemTransfer.Create, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/EMT/Views/MedicalItemTransfers/MedicalItemTransfer.cshtml", new MedicalItemTransferUpdateModel());
        }

        [Route("EMT/MedicalItemTransfers/Update/{PK}")]
        public ActionResult MedicalItemTransferUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.MedicalItemTransfer.Access, Apps.EMT))
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
            var model = (from a in DbEMT.dataMedicalItemTransfer.WherePK(PK)
                         select new MedicalItemTransferUpdateModel
                         {
                             MedicalItemTransferGUID = a.MedicalItemTransferGUID,
                             ConfirmedReceived = a.ConfirmedReceived,
                             DeliveryDate = a.DeliveryDate,
                             MedicalPharmacyGUID = a.MedicalPharmacyGUID,
                             Active = a.Active,
                             dataMedicalItemTransferRowVersion = a.dataMedicalItemTransferRowVersion,
                             CreatedBy = Audit.ExecutedBy,
                             CreatedOn = Audit.ExecutionTime,
                             
                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("MedicalItemTransfer", "MedicalItemTransfers", new { Area = "EMT" }));

            return View("~/Areas/EMT/Views/MedicalItemTransfers/MedicalItemTransfer.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalItemTransferCreate(MedicalItemTransferUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.MedicalItemTransfer.Create, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveMedicalItemTransfer(model)) return PartialView("~/Areas/EMT/Views/MedicalItemTransfers/_MedicalItemTransferForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            dataMedicalItemTransfer MedicalItemTransfer = Mapper.Map(model, new dataMedicalItemTransfer());
            MedicalItemTransfer.MedicalItemTransferGUID = EntityPK;
            DbEMT.Create(MedicalItemTransfer, Permissions.MedicalItemTransfer.CreateGuid, ExecutionTime, DbCMS);


            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.MedicalItemTransferDetailsDataTable, ControllerContext, "MedicalItemTransferDetailsContainer"));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.MedicalItemTransfer.Create, Apps.EMT, new UrlHelper(Request.RequestContext).Action("Create", "MedicalItemTransfers", new { Area = "EMT" })), Container = "MedicalItemTransferFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.MedicalItemTransfer.Update, Apps.EMT), Container = "MedicalItemTransferFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.MedicalItemTransfer.Delete, Apps.EMT), Container = "MedicalItemTransferFormControls" });

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleCreateMessage(DbEMT.PrimaryKeyControl(MedicalItemTransfer), DbEMT.RowVersionControls(new List<dataMedicalItemTransfer> { MedicalItemTransfer }), Partials, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalItemTransferUpdate(MedicalItemTransferUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.MedicalItemTransfer.Update, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveMedicalItemTransfer(model)) return PartialView("~/Areas/EMT/Views/MedicalItemTransfers/_MedicalItemTransferForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            dataMedicalItemTransfer MedicalItemTransfer = Mapper.Map(model, new dataMedicalItemTransfer());
            DbEMT.Update(MedicalItemTransfer, Permissions.MedicalItemTransfer.UpdateGuid, ExecutionTime, DbCMS);



            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleUpdateMessage(null, null, DbEMT.RowVersionControls(new List<dataMedicalItemTransfer> { MedicalItemTransfer })));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyMedicalItemTransfer(model.MedicalItemTransferGUID);
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalItemTransferDelete(dataMedicalItemTransfer model)
        {
            if (!CMS.HasAction(Permissions.MedicalItemTransfer.Delete, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            List<dataMedicalItemTransfer> DeletedMedicalItemTransfer = DeleteMedicalItemTransfers(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.MedicalItemTransfer.Restore, Apps.EMT), Container = "MedicalItemTransferFormControls" });

            try
            {
                int CommitedRows = DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleDeleteMessage(CommitedRows, DeletedMedicalItemTransfer.FirstOrDefault(), "MedicalItemTransferDetailsContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyMedicalItemTransfer(model.MedicalItemTransferGUID);
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }

        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalItemTransferRestore(dataMedicalItemTransfer model)
        {
            if (!CMS.HasAction(Permissions.MedicalItemTransfer.Restore, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveMedicalItemTransfer(model))
            {
                return Json(DbEMT.RecordExists());
            }

            List<dataMedicalItemTransfer> RestoredMedicalItemTransfers = RestoreMedicalItemTransfers(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.MedicalItemTransfer.Create, Apps.EMT, new UrlHelper(Request.RequestContext).Action("MedicalItemTransferCreate", "Configuration", new { Area = "EMT" })), Container = "MedicalItemTransferFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.MedicalItemTransfer.Update, Apps.EMT), Container = "MedicalItemTransferFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.MedicalItemTransfer.Delete, Apps.EMT), Container = "MedicalItemTransferFormControls" });

            try
            {
                int CommitedRows = DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleRestoreMessage(CommitedRows, RestoredMedicalItemTransfers, DbEMT.PrimaryKeyControl(RestoredMedicalItemTransfers.FirstOrDefault()), Url.Action(DataTableNames.MedicalItemTransferDetailsDataTable, Portal.GetControllerName(ControllerContext)), "MedicalItemTransferDetailsContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyMedicalItemTransfer(model.MedicalItemTransferGUID);
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult MedicalItemTransfersDataTableDelete(List<dataMedicalItemTransfer> models)
        {
            if (!CMS.HasAction(Permissions.MedicalItemTransfer.Delete, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataMedicalItemTransfer> DeletedMedicalItemTransfers = DeleteMedicalItemTransfers(models);

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.PartialDeleteMessage(DeletedMedicalItemTransfers, models, DataTableNames.MedicalItemTransfersDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult MedicalItemTransfersDataTableRestore(List<dataMedicalItemTransfer> models)
        {
            if (!CMS.HasAction(Permissions.MedicalItemTransfer.Restore, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataMedicalItemTransfer> RestoredMedicalItemTransfers = RestoreMedicalItemTransfers(models);

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.PartialRestoreMessage(RestoredMedicalItemTransfers, models, DataTableNames.MedicalItemTransfersDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        private List<dataMedicalItemTransfer> DeleteMedicalItemTransfers(List<dataMedicalItemTransfer> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<dataMedicalItemTransfer> DeletedMedicalItemTransfers = new List<dataMedicalItemTransfer>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbEMT.QueryBuilder(models, Permissions.MedicalItemTransfer.DeleteGuid, SubmitTypes.Delete, baseQuery);

            var Records = DbEMT.Database.SqlQuery<dataMedicalItemTransfer>(query).ToList();
            foreach (var record in Records)
            {
                if (!record.ConfirmedReceived)
                {
                    DeletedMedicalItemTransfers.Add(DbEMT.Delete(record, ExecutionTime, Permissions.MedicalItemTransfer.DeleteGuid, DbCMS));
                }
            }

            var MedicalItemTransferDetails = DeletedMedicalItemTransfers.SelectMany(a => a.dataMedicalItemTransferDetail).Where(l => l.Active).ToList();
            foreach (var MedicalItemTransferDetail in MedicalItemTransferDetails)
            {
                DbEMT.Delete(MedicalItemTransferDetail, ExecutionTime, Permissions.MedicalItemTransfer.DeleteGuid, DbCMS);
                RemoveItemQuantity(MedicalItemTransferDetail.MedicalItemTransferDetailGUID);
            }
            return DeletedMedicalItemTransfers;
        }

        private List<dataMedicalItemTransfer> RestoreMedicalItemTransfers(List<dataMedicalItemTransfer> models)
        {

            DateTime RestoringTime = DateTime.Now;

            List<dataMedicalItemTransfer> RestoredMedicalItemTransfers = new List<dataMedicalItemTransfer>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbEMT.QueryBuilder(models, Permissions.MedicalItemTransfer.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Records = DbEMT.Database.SqlQuery<dataMedicalItemTransfer>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveMedicalItemTransfer(record))
                {
                    RestoredMedicalItemTransfers.Add(DbEMT.Restore(record, Permissions.MedicalItemTransfer.DeleteGuid, Permissions.MedicalItemTransfer.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            var MedicalItemTransferDetails = RestoredMedicalItemTransfers.SelectMany(x => x.dataMedicalItemTransferDetail.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var MedicalItemTransferDetail in MedicalItemTransferDetails)
            {
                DbEMT.Restore(MedicalItemTransferDetail, Permissions.MedicalItemTransfer.DeleteGuid, Permissions.MedicalItemTransfer.RestoreGuid, RestoringTime, DbCMS);
                AddItemQuantity(MedicalItemTransferDetail.MedicalItemTransferDetailGUID);
            }

            return RestoredMedicalItemTransfers;
        }

        private JsonResult ConcurrencyMedicalItemTransfer(Guid PK)
        {
            MedicalItemTransferUpdateModel dbModel = new MedicalItemTransferUpdateModel();

            var MedicalItemTransfer = DbEMT.dataMedicalItemTransfer.Where(x => x.MedicalItemTransferGUID == PK).FirstOrDefault();
            var dbMedicalItemTransfer = DbEMT.Entry(MedicalItemTransfer).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbMedicalItemTransfer, dbModel);

            var MedicalItemTransferDetail = DbEMT.dataMedicalItemTransferDetail.Where(x => x.MedicalItemTransferGUID == PK).Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.dataMedicalItemTransfer.DeletedOn)).FirstOrDefault();
            var dbMedicalItemTransferDetail = DbEMT.Entry(MedicalItemTransferDetail).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbMedicalItemTransferDetail, dbModel);

            if (MedicalItemTransfer.dataMedicalItemTransferRowVersion.SequenceEqual(dbModel.dataMedicalItemTransferRowVersion))
            {
                return Json(DbEMT.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbEMT, dbModel, "MedicalItemTransferDetailsContainer"));
        }

        private bool ActiveMedicalItemTransfer(Object model)
        {
            return false;
        }

        #endregion

        #region  Medical Item Transfer Details

        //[Route("EMT/MedicalItemTransferDetailsDataTable/{PK}")]
        public ActionResult MedicalItemTransferDetailsDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/EMT/Views/MedicalItemTransfers/_MedicalItemTransferDetailsDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<MedicalItemTransferDetailsDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<MedicalItemTransferDetailsDataTableModel>(DataTable.Filters);
            }

            var Result = (from a in DbEMT.dataMedicalItemTransferDetail.AsNoTracking().AsExpandable().Where(x => x.MedicalItemTransferGUID == PK)
                          join b in DbEMT.codeMedicalItem on a.MedicalItemGUID equals b.MedicalItemGUID
                          select new MedicalItemTransferDetailsDataTableModel
                          {
                              MedicalItemTransferDetailGUID = a.MedicalItemTransferDetailGUID,
                              MedicalItemInputDetailGUID = a.MedicalItemInputDetailGUID,
                              MedicalItemGUID = a.MedicalItemGUID,
                              BrandName = b.BrandName,
                              MedicalItemTransferGUID = a.MedicalItemTransferGUID,
                              QuantityByPackingUnit = a.QuantityByPackingUnit !=0?a.QuantityByPackingUnit : a.QuantityByPackingTransfer.Value,
                              QuantityByPackingTransfer = a.QuantityByPackingTransfer != null ? a.QuantityByPackingTransfer.Value : 0,
                              MedicalGenericNameDescription = b.codeMedicalGenericName.codeMedicalGenericNameLanguage.Where(x => x.LanguageID == LAN && x.Active).FirstOrDefault().MedicalGenericNameDescription.Replace("+", " "),
                              RemainingItems = a.RemainingItems,
                              Active = a.Active,
                              dataMedicalItemTransferDetailRowVersion = a.dataMedicalItemTransferDetailRowVersion
                          }).Where(Predicate);

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult MedicalItemTransferDetailCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.MedicalItemTransfer.Create, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var MedicalPharmacy = (from a in DbEMT.dataMedicalItemTransfer.Where(x => x.MedicalItemTransferGUID == FK)
                                   join b in DbEMT.codeMedicalPharmacy on a.MedicalPharmacyGUID equals b.MedicalPharmacyGUID
                                   select b).FirstOrDefault();
            return PartialView("~/Areas/EMT/Views/MedicalItemTransfers/_MedicalItemTransferDetailsUpdateModal.cshtml",
                new MedicalItemTransferDetailUpdateModel { MedicalItemTransferGUID = FK, OrganizationInstanceGUID = MedicalPharmacy.OrganizationInstanceGUID });
        }

        public ActionResult MedicalItemTransferDetailUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.MedicalItemTransfer.Access, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var medicalItemTransferDetails = (from a in DbEMT.dataMedicalItemTransfer
                                              join b in DbEMT.codeMedicalPharmacy on a.MedicalPharmacyGUID equals b.MedicalPharmacyGUID
                                              join c in DbEMT.dataMedicalItemTransferDetail.Where(x => x.MedicalItemTransferDetailGUID == PK) on a.MedicalItemTransferGUID equals c.MedicalItemTransferGUID
                                              select new
                                              MedicalItemTransferDetailUpdateModel
                                              {
                                                  MedicalItemTransferGUID = a.MedicalItemTransferGUID,
                                                  OrganizationInstanceGUID = b.OrganizationInstanceGUID,
                                                  MedicalItemGUID = c.MedicalItemGUID,
                                                  MedicalItemInputDetailGUID = c.MedicalItemInputDetailGUID,
                                                  MedicalItemTransferDetailGUID = c.MedicalItemTransferDetailGUID,
                                                  QuantityByPackingUnit = c.QuantityByPackingUnit,
                                                  QuantityByPackingTransfer=c.QuantityByPackingTransfer!=null? c.QuantityByPackingTransfer:0,
                                                  RemainingItems = c.RemainingItems,
                                                  Active = c.Active,
                                                  dataMedicalItemTransferDetailRowVersion = c.dataMedicalItemTransferDetailRowVersion
                                              }
                                             ).FirstOrDefault();
            return PartialView("~/Areas/EMT/Views/MedicalItemTransfers/_MedicalItemTransferDetailsUpdateModal.cshtml", medicalItemTransferDetails);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalItemTransferDetailCreate(MedicalItemTransferDetailUpdateModel model1)
        {
            if (!CMS.HasAction(Permissions.MedicalItemTransfer.Create, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            dataMedicalItemTransferDetail model = Mapper.Map(model1, new dataMedicalItemTransferDetail());
            if (!ModelState.IsValid || ActiveMedicalItemTransferDetail(model)) return PartialView("~/Areas/EMT/Views/MedicalItemTransfers/_MedicalItemTransferDetailsUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;
            model.MedicalItemTransferDetailGUID = Guid.NewGuid();
            model.RemainingItems = model.QuantityByPackingUnit;
            DbEMT.Create(model, Permissions.MedicalItemTransfer.CreateGuid, ExecutionTime, DbCMS);

            //update Item (Quantity Threshold and Input Detail) which we transfer the items <from> it.
            dataMedicalItemInputDetail itemImputDetail = DbEMT.dataMedicalItemInputDetail.Where(x => x.MedicalItemInputDetailGUID == model.MedicalItemInputDetailGUID).FirstOrDefault();
            itemImputDetail.RemainingItems = itemImputDetail.RemainingItems - model.QuantityByPackingUnit;

            dataItemQuantityThreshold itemQuantityThresholdFrom = DbEMT.dataItemQuantityThreshold.Where(x => x.MedicalItemGUID == model.MedicalItemGUID && x.MedicalPharmacyGUID == itemImputDetail.dataMedicalItemInput.MedicalPharmacyGUID && x.Active).FirstOrDefault();
            itemQuantityThresholdFrom.QuantityTotalRemainingItems = itemQuantityThresholdFrom.QuantityTotalRemainingItems - model.QuantityByPackingUnit;

            //Create or update Item Quantity Threshold which we transfer the items <to> it .
            var MedicalItemTransfer = DbEMT.dataMedicalItemTransfer.Where(x => x.MedicalItemTransferGUID == model.MedicalItemTransferGUID).FirstOrDefault();
            dataItemQuantityThreshold itemQuantityThresholdTo = DbEMT.dataItemQuantityThreshold.Where(x => x.MedicalItemGUID == model.MedicalItemGUID && x.MedicalPharmacyGUID == MedicalItemTransfer.MedicalPharmacyGUID && x.Active).FirstOrDefault();
            if (itemQuantityThresholdTo == null)
            {
                itemQuantityThresholdTo = new dataItemQuantityThreshold();
                itemQuantityThresholdTo.ItemQuantityThresholdGUID = Guid.NewGuid();
                itemQuantityThresholdTo.MedicalItemGUID = model.MedicalItemGUID;
                itemQuantityThresholdTo.QuantityThreshold = 0;
                itemQuantityThresholdTo.QuantityTotalRemainingItems = model.QuantityByPackingUnit;
                itemQuantityThresholdTo.MedicalPharmacyGUID = MedicalItemTransfer.MedicalPharmacyGUID;
                DbEMT.Create(itemQuantityThresholdTo, Permissions.MedicalItem.CreateGuid, ExecutionTime, DbCMS);
            }
            else
            {
                itemQuantityThresholdTo.QuantityTotalRemainingItems = itemQuantityThresholdTo.QuantityTotalRemainingItems + model.QuantityByPackingUnit;
            }


            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleUpdateMessage(DataTableNames.MedicalItemTransferDetailsDataTable, DbEMT.PrimaryKeyControl(model), DbEMT.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalItemTransferDetailUpdate(dataMedicalItemTransferDetail model1)
        {
            if (!CMS.HasAction(Permissions.MedicalItemTransfer.Update, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            
            dataMedicalItemTransferDetail model = Mapper.Map(model1, new dataMedicalItemTransferDetail());
            DateTime ExecutionTime = DateTime.Now;
            if (Session[SessionKeys.EmailAddress].ToString().ToLower().EndsWith("unhcr.org"))
            {
                DbEMT.Update(model, Permissions.MedicalItemTransfer.UpdateGuid, ExecutionTime, DbCMS);
            }
            else
            {
                if (!ModelState.IsValid || ActiveMedicalItemTransferDetail(model)) return PartialView("~/Areas/EMT/Views/MedicalItemTransfers/_MedicalItemTransferDetailsUpdateModal.cshtml", model);
                RemoveItemQuantity(model1.MedicalItemTransferDetailGUID);
                var DispensedQuintitySum = DbEMT.dataMedicalBeneficiaryItemOutDetail.Where(x => x.MedicalItemTransferDetailGUID == model.MedicalItemTransferDetailGUID).FirstOrDefault() != null ? DbEMT.dataMedicalBeneficiaryItemOutDetail.Where(x => x.MedicalItemTransferDetailGUID == model.MedicalItemTransferDetailGUID).Select(x => x.QuantityByPackingUnit).Sum() : 0;
                model.RemainingItems = model.QuantityByPackingUnit - DispensedQuintitySum;
                DbEMT.Update(model, Permissions.MedicalItemTransfer.UpdateGuid, ExecutionTime, DbCMS);
                AddItemQuantity(model.MedicalItemTransferDetailGUID);
            }
            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleUpdateMessage(DataTableNames.MedicalItemTransferDetailsDataTable,
                    DbEMT.PrimaryKeyControl(model),
                    DbEMT.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyMedicalItemTransferDetail(model.MedicalItemTransferDetailGUID);
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalItemTransferDetailDelete(dataMedicalItemTransferDetail model)
        {
            if (!CMS.HasAction(Permissions.MedicalItemTransfer.Delete, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            var MedicalItemTransfer = DbEMT.dataMedicalItemTransfer.Where(x => x.MedicalItemTransferGUID == model.MedicalItemTransferGUID).FirstOrDefault();
            if (!MedicalItemTransfer.ConfirmedReceived)
            {
                List<dataMedicalItemTransferDetail> DeletedLanguages = DeleteMedicalItemTransferDetails(new List<dataMedicalItemTransferDetail> { model });

                try
                {
                    DbEMT.SaveChanges();
                    DbCMS.SaveChanges();
                    return Json(DbEMT.SingleDeleteMessage(DeletedLanguages, DataTableNames.MedicalItemTransferDetailsDataTable));
                }
                catch (DbUpdateConcurrencyException)
                {
                    return ConcrrencyMedicalItemTransferDetail(model.MedicalItemTransferDetailGUID);
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
        public ActionResult MedicalItemTransferDetailRestore(dataMedicalItemTransferDetail model)
        {
            if (!CMS.HasAction(Permissions.MedicalItemTransfer.Restore, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveMedicalItemTransferDetail(model))
            {
                return Json(DbEMT.RecordExists());
            }

            List<dataMedicalItemTransferDetail> RestoredLanguages = RestoreMedicalItemTransferDetails(Portal.SingleToList(model));

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleRestoreMessage(RestoredLanguages, DataTableNames.MedicalItemTransferDetailsDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyMedicalItemTransferDetail(model.MedicalItemTransferDetailGUID);
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult MedicalItemTransferDetailsDataTableDelete(List<dataMedicalItemTransferDetail> models)
        {
            if (!CMS.HasAction(Permissions.MedicalItemTransfer.Delete, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            Guid MedicalItemTransferDetailGUID = models.FirstOrDefault().MedicalItemTransferDetailGUID;
            var MedicalItemTransfer = DbEMT.dataMedicalItemTransferDetail.AsNoTracking().Where(x => x.MedicalItemTransferDetailGUID == MedicalItemTransferDetailGUID).FirstOrDefault();
            if (MedicalItemTransfer.Confirmed == null) { MedicalItemTransfer.Confirmed = false; }
            if (!MedicalItemTransfer.Confirmed.Value)
            {
                List<dataMedicalItemTransferDetail> DeletedLanguages = DeleteMedicalItemTransferDetails(models);

                try
                {
                    DbEMT.SaveChanges();
                    DbCMS.SaveChanges();
                    return Json(DbEMT.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.MedicalItemTransferDetailsDataTable));
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
        public JsonResult MedicalItemTransferDetailsDataTableRestore(List<dataMedicalItemTransferDetail> models)
        {
            if (!CMS.HasAction(Permissions.MedicalItemTransfer.Restore, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataMedicalItemTransferDetail> RestoredLanguages = RestoreMedicalItemTransferDetails(models);

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.MedicalItemTransferDetailsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        private List<dataMedicalItemTransferDetail> DeleteMedicalItemTransferDetails(List<dataMedicalItemTransferDetail> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<dataMedicalItemTransferDetail> DeletedMedicalItemTransferDetails = new List<dataMedicalItemTransferDetail>();

            string query = DbEMT.QueryBuilder(models, Permissions.MedicalItemTransfer.DeleteGuid, SubmitTypes.Delete, "");

            var MedicalItemTransferDetails = DbEMT.Database.SqlQuery<dataMedicalItemTransferDetail>(query).ToList();

            //var MedicalItemTransfer = DbEMT.dataMedicalItemTransfer.Where(x => x.MedicalItemTransferGUID == models.FirstOrDefault().MedicalItemTransferGUID).FirstOrDefault();
            foreach (var MedicalItemTransferDetail in MedicalItemTransferDetails)
            {
                DeletedMedicalItemTransferDetails.Add(DbEMT.Delete(MedicalItemTransferDetail, ExecutionTime, Permissions.MedicalItemTransfer.DeleteGuid, DbCMS));

                RemoveItemQuantity(MedicalItemTransferDetail.MedicalItemTransferDetailGUID);
            }

            return DeletedMedicalItemTransferDetails;
        }

        private List<dataMedicalItemTransferDetail> RestoreMedicalItemTransferDetails(List<dataMedicalItemTransferDetail> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<dataMedicalItemTransferDetail> RestoredLanguages = new List<dataMedicalItemTransferDetail>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbEMT.QueryBuilder(models, Permissions.MedicalItemTransfer.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var MedicalItemTransferDetails = DbEMT.Database.SqlQuery<dataMedicalItemTransferDetail>(query).ToList();

            //var MedicalItemTransfer = DbEMT.dataMedicalItemTransfer.Where(x => x.MedicalItemTransferGUID == models.FirstOrDefault().MedicalItemTransferGUID).FirstOrDefault();
            foreach (var MedicalItemTransferDetail in MedicalItemTransferDetails)
            {
                if (!ActiveMedicalItemTransferDetail(MedicalItemTransferDetail))
                {
                    RestoredLanguages.Add(DbEMT.Restore(MedicalItemTransferDetail, Permissions.MedicalItemTransfer.DeleteGuid, Permissions.MedicalItemTransfer.RestoreGuid, RestoringTime, DbCMS));
                    AddItemQuantity(MedicalItemTransferDetail.MedicalItemTransferDetailGUID);
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyMedicalItemTransferDetail(Guid PK)
        {
            dataMedicalItemTransferDetail dbModel = new dataMedicalItemTransferDetail();

            var Language = DbEMT.dataMedicalItemTransferDetail.Where(l => l.MedicalItemTransferDetailGUID == PK).FirstOrDefault();
            var dbLanguage = DbEMT.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.dataMedicalItemTransferDetailRowVersion.SequenceEqual(dbModel.dataMedicalItemTransferDetailRowVersion))
            {
                return Json(DbEMT.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbEMT, dbModel, "MedicalItemTransferDetailsContainer"));
        }

        private bool ActiveMedicalItemTransferDetail(dataMedicalItemTransferDetail model)
        {
            int ItemTransferDetail = DbEMT.dataMedicalItemTransferDetail
                                  .Where(x => x.MedicalItemTransferDetailGUID != model.MedicalItemTransferDetailGUID &&
                                              x.MedicalItemTransferGUID == model.MedicalItemTransferGUID &&
                                              x.MedicalItemGUID == model.MedicalItemGUID &&
                                              x.MedicalItemInputDetailGUID == model.MedicalItemInputDetailGUID &&
                                              x.Active).Count();
            if (DbEMT.dataMedicalBeneficiaryItemOutDetail.Where(x => x.MedicalItemTransferDetailGUID == model.MedicalItemTransferDetailGUID).FirstOrDefault() != null)
            {
                double? DispensedQuintitySum = DbEMT.dataMedicalBeneficiaryItemOutDetail.Where(x => x.MedicalItemTransferDetailGUID == model.MedicalItemTransferDetailGUID).Select(x => x.QuantityByPackingUnit).Sum();
                if (model.QuantityByPackingUnit < DispensedQuintitySum)
                {
                    ItemTransferDetail = ItemTransferDetail + 1;
                    ModelState.AddModelError("MedicalItemTransferDetailGUID", "The Quantity Of Dispensed Medicine Greater Than Updated Quantity");
                }
            }
            if (ItemTransferDetail > 0)
            {
                ModelState.AddModelError("MedicalItemTransferDetailGUID", "Item already exists");
            }
            var MedicalItemTransferDetail = DbEMT.dataMedicalItemInputDetail.Where(x => x.MedicalItemInputDetailGUID == model.MedicalItemInputDetailGUID).FirstOrDefault();
            if (model.QuantityByPackingUnit > MedicalItemTransferDetail.RemainingItems)
            {
                ModelState.AddModelError("QuantityByPackingUnit", "Quantity of Batch Number Remaining not Enough"); //From resource ?????? Amer  
                ItemTransferDetail = ItemTransferDetail + 1;
            }

            return (ItemTransferDetail > 0);
        }

        public void AddItemQuantity(Guid MedicalItemTransferDetailGUID)
        {
            var MedicalItemTransferDetail = DbEMT.dataMedicalItemTransferDetail.Where(x => x.MedicalItemTransferDetailGUID == MedicalItemTransferDetailGUID).FirstOrDefault();
            //add the QuantityByPackingUnit from  RemainingItemsQuantity
            dataItemQuantityThreshold itemQuantityThresholdRemove = DbEMT.dataItemQuantityThreshold.Where(x => x.MedicalItemGUID == MedicalItemTransferDetail.MedicalItemGUID && x.MedicalPharmacyGUID == MedicalItemTransferDetail.dataMedicalItemTransfer.MedicalPharmacyGUID && x.Active).FirstOrDefault();
            itemQuantityThresholdRemove.QuantityTotalRemainingItems = itemQuantityThresholdRemove.QuantityTotalRemainingItems + MedicalItemTransferDetail.QuantityByPackingUnit;
            //remove the QuantityByPackingUnit from  RemainingItemsQuantity
            dataItemQuantityThreshold itemQuantityThresholdAdd = DbEMT.dataItemQuantityThreshold.Where(x => x.MedicalItemGUID == MedicalItemTransferDetail.MedicalItemGUID && x.MedicalPharmacyGUID == MedicalItemTransferDetail.dataMedicalItemInputDetail.dataMedicalItemInput.MedicalPharmacyGUID && x.Active).FirstOrDefault();
            itemQuantityThresholdAdd.QuantityTotalRemainingItems = itemQuantityThresholdAdd.QuantityTotalRemainingItems - MedicalItemTransferDetail.QuantityByPackingUnit;

            if (MedicalItemTransferDetail.MedicalItemTransferDetailTransferGUID != null)
            {
                var medicalItemTransferDetail = DbEMT.dataMedicalItemTransferDetail.Where(x => x.MedicalItemTransferDetailGUID == MedicalItemTransferDetail.MedicalItemTransferDetailTransferGUID.Value).FirstOrDefault();
                medicalItemTransferDetail.RemainingItems = medicalItemTransferDetail.RemainingItems - (int)MedicalItemTransferDetail.RemainingItems;

            }
            else
            {
                dataMedicalItemInputDetail medicalItemInputDetail = DbEMT.dataMedicalItemInputDetail.Where(x => x.MedicalItemInputDetailGUID == MedicalItemTransferDetail.MedicalItemInputDetailGUID).FirstOrDefault();
                medicalItemInputDetail.RemainingItems = medicalItemInputDetail.RemainingItems - (int)MedicalItemTransferDetail.RemainingItems;

            }


        }

        public void RemoveItemQuantity(Guid MedicalItemTransferDetailGUID)
        {
            var MedicalItemTransferDetail = DbEMT.dataMedicalItemTransferDetail.Where(x => x.MedicalItemTransferDetailGUID == MedicalItemTransferDetailGUID).FirstOrDefault();
            //remove the QuantityByPackingUnit from  RemainingItemsQuantity
            dataItemQuantityThreshold itemQuantityThresholdRemove = DbEMT.dataItemQuantityThreshold.Where(x => x.MedicalItemGUID == MedicalItemTransferDetail.MedicalItemGUID && x.MedicalPharmacyGUID == MedicalItemTransferDetail.dataMedicalItemTransfer.MedicalPharmacyGUID && x.Active).FirstOrDefault();
            itemQuantityThresholdRemove.QuantityTotalRemainingItems = itemQuantityThresholdRemove.QuantityTotalRemainingItems - MedicalItemTransferDetail.QuantityByPackingUnit;
            //add the QuantityByPackingUnit from  RemainingItemsQuantity
            dataItemQuantityThreshold itemQuantityThresholdAdd = DbEMT.dataItemQuantityThreshold.Where(x => x.MedicalItemGUID == MedicalItemTransferDetail.MedicalItemGUID && x.MedicalPharmacyGUID == MedicalItemTransferDetail.dataMedicalItemInputDetail.dataMedicalItemInput.MedicalPharmacyGUID && x.Active).FirstOrDefault();
            itemQuantityThresholdAdd.QuantityTotalRemainingItems = itemQuantityThresholdAdd.QuantityTotalRemainingItems + MedicalItemTransferDetail.QuantityByPackingUnit;

            if (MedicalItemTransferDetail.MedicalItemTransferDetailTransferGUID != null)
            {
                var medicalItemTransferDetail = DbEMT.dataMedicalItemTransferDetail.Where(x => x.MedicalItemTransferDetailGUID == MedicalItemTransferDetail.MedicalItemTransferDetailTransferGUID.Value).FirstOrDefault();
                medicalItemTransferDetail.RemainingItems = medicalItemTransferDetail.RemainingItems + (int)MedicalItemTransferDetail.RemainingItems;
            }
            else
            {
                dataMedicalItemInputDetail medicalItemInputDetail = DbEMT.dataMedicalItemInputDetail.Where(x => x.MedicalItemInputDetailGUID == MedicalItemTransferDetail.MedicalItemInputDetailGUID).FirstOrDefault();
                medicalItemInputDetail.RemainingItems = medicalItemInputDetail.RemainingItems + (int)MedicalItemTransferDetail.RemainingItems;

            }

        }

        #endregion

        #region Transfer 
        private bool ActiveMedicalItemTransfer(MedicalItemTransferUpdateModel model)
        {
            bool valid = false;
            if (model.MedicalItemInputDetailsDataTableModel != null)
            {
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

                
            }
            return valid;
        }
        public ActionResult MedicalItemInputOverViewTransferCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.MedicalItemTransfer.Create, Apps.EMT))
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
                MedicalItemInputDetailsDataTableModel = (from a in DbEMT.dataMedicalItemTransferDetail.Where(x => x.dataMedicalItemTransfer.MedicalPharmacyGUID == FK && x.Active && x.RemainingItems > 0)

                                                         join b in DbEMT.codeMedicalItem on a.MedicalItemGUID equals b.MedicalItemGUID
                                                         join c in DbEMT.dataMedicalItemInputDetail on a.MedicalItemInputDetailGUID equals c.MedicalItemInputDetailGUID
                                                         orderby b.BrandName
                                                         select
            new MedicalItemInputDetailsDataTableModel
            {
                BrandName = b.BrandName,
                MedicalItemTransferDetailGUID = a.MedicalItemTransferDetailGUID,
                MedicalItemInputDetailGUID = a.MedicalItemInputDetailGUID.Value,
                BatchNumber = c.BatchNumber,
                QuantityByPackingUnit = 0,
                RemainingItems = a.RemainingItems,
                ExpirationDate=c.ExpirationDate,
                MedicalItemGUID = c.MedicalItemGUID.ToString(),
            }
                ).ToList()
            };
            return PartialView("~/Areas/EMT/Views/MedicalItemTransfers/_MedicalItemInputTransferUpdateModal.cshtml", model);

        }

        public ActionResult MedicalItemInputTransferCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.MedicalItemTransfer.Create, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var Pharmacy = DbEMT.dataMedicalItemTransferDetail.Where(x => x.MedicalItemTransferGUID == FK).FirstOrDefault().dataMedicalItemInputDetail.dataMedicalItemInput.codeMedicalPharmacy;
            var MedicalItemTransfer = DbEMT.dataMedicalItemTransfer.Where(x => x.MedicalItemTransferGUID == FK).FirstOrDefault();
            var model = new MedicalItemTransferUpdateModel()
            {
                MedicalPharmacyGUID = MedicalItemTransfer.MedicalPharmacyGUID,
                OrganizationInstanceGUID = Pharmacy.OrganizationInstanceGUID,
                ProvidedByOrganizationInstanceGUID = MedicalItemTransfer.dataMedicalItemTransferDetail.FirstOrDefault().dataMedicalItemInputDetail.dataMedicalItemInput.ProvidedByOrganizationInstanceGUID.Value,
                MedicalItemInputDetailsDataTableModel = (from a in DbEMT.dataMedicalItemTransferDetail.Where(x => x.MedicalItemTransferGUID.Value == FK)

                                                         join b in DbEMT.codeMedicalItem on a.MedicalItemGUID equals b.MedicalItemGUID
                                                         join c in DbEMT.dataMedicalItemInputDetail on a.MedicalItemInputDetailGUID equals c.MedicalItemInputDetailGUID
                                                         orderby b.BrandName
                                                         select
            new MedicalItemInputDetailsDataTableModel
            {
                BrandName = b.BrandName,
                MedicalItemTransferDetailGUID = a.MedicalItemTransferDetailGUID,
                MedicalItemInputDetailGUID = a.MedicalItemInputDetailGUID.Value,
                BatchNumber = c.BatchNumber,
                ExpirationDate=c.ExpirationDate,
                QuantityByPackingUnit = 0,
                RemainingItems = a.RemainingItems,
                MedicalItemGUID = c.MedicalItemGUID.ToString(),
            }
                ).ToList()
            };
            return PartialView("~/Areas/EMT/Views/MedicalItemTransfers/_MedicalItemInputTransferUpdateModal.cshtml", model);

        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalItemInputTransferCreate(MedicalItemTransferUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.MedicalItemTransfer.Create, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveMedicalItemTransfer(model)) return PartialView("~/Areas/EMT/Views/MedicalItemTransfers/_MedicalItemInputTransferUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            PrimaryKeyControl primaryKeyControl = new PrimaryKeyControl();
            List<RowVersionControl> RowVersionControl = new List<RowVersionControl>();
            //transfer from warehouse to pharmacy
            var MedicalItemTransferPharmacy = new dataMedicalItemTransfer();
            if (!DbEMT.codeMedicalPharmacy.Where(x => x.MedicalPharmacyGUID == model.MedicalPharmacyGUID).FirstOrDefault().MainWarehouse)
            {
                MedicalItemTransferPharmacy = TransferToPharmacy(model, ExecutionTime);
            }
            //transfer from warehouse to warehouse
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
            //dataMedicalItemInput MedicalItemInput = new dataMedicalItemInput();//DbEMT.dataMedicalItemInput.Where(x => x.ProcuredByOrganizationInstanceGUID == model.OrganizationInstanceGUID && x.DeliveryDate == model.DeliveryDate && x.MedicalPharmacyGUID == model.MedicalPharmacyGUID && x.Active).FirstOrDefault();
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
                    dataMedicalItemTransferDetail Source = DbEMT.dataMedicalItemTransferDetail.Where(x => x.MedicalItemTransferDetailGUID == item.MedicalItemTransferDetailGUID).FirstOrDefault();
                    Source.RemainingItems = Source.RemainingItems - item.QuantityByPackingUnit;
                    DbEMT.Update(Source, Permissions.MedicalItemInput.UpdateGuid, ExecutionTime, DbCMS);
                    dataMedicalItemInputDetail Destination = DbEMT.dataMedicalItemInputDetail.AsNoTracking().Where(x => x.MedicalItemGUID.ToString() == item.MedicalItemGUID && x.BatchNumber == item.BatchNumber && x.PriceOfSmallestUnit == x.PriceOfSmallestUnit && x.Active && x.MedicalItemInputGUID == EntityPK).FirstOrDefault();
                    int quantity = Source.dataMedicalItemInputDetail.QuantityByPackingUnit != 0 ? Source.dataMedicalItemInputDetail.QuantityByPackingUnit : Source.dataMedicalItemInputDetail.QuantityByPackingTransfer.Value;

                    if (Destination == null)
                    {
                        Destination = new dataMedicalItemInputDetail();
                        Destination.MedicalItemInputDetailGUID = Guid.NewGuid();
                        Destination.Active = true;
                        Destination.BatchNumber = Source.dataMedicalItemInputDetail.BatchNumber;
                        Destination.PriceOfPackingUnit = Source.dataMedicalItemInputDetail.PriceOfPackingUnit;
                        Destination.PriceOfSmallestUnit = Source.dataMedicalItemInputDetail.PriceOfSmallestUnit;
                        Destination.Comments = Source.Comments;
                        Destination.Confirmed = false;
                        Destination.ConfirmedBy = null;
                        Destination.ConfirmedDate = null;
                        Destination.ExpirationDate = Source.dataMedicalItemInputDetail.ExpirationDate;
                        Destination.ManufacturingDate = Source.dataMedicalItemInputDetail.ManufacturingDate;
                        Destination.MedicalItemGUID = Source.dataMedicalItemInputDetail.MedicalItemGUID;
                        Destination.MedicalItemInputSupplyDetailGUID = Source.dataMedicalItemInputDetail.MedicalItemInputSupplyDetailGUID;
                        Destination.MedicalItemTransferDetailTransferGUID = Source.MedicalItemTransferDetailGUID;
                        Destination.QuantityByPackingTransfer = item.QuantityByPackingUnit;
                        Destination.RemainingItems = item.QuantityByPackingUnit;
                        Destination.QuantityBySmallestUnit = Convert.ToInt32(item.QuantityByPackingUnit * (Source.dataMedicalItemInputDetail.QuantityBySmallestUnit / quantity));
                        Destination.MedicalItemInputDetailTransferGUID = null;

                        Destination.MedicalItemInputGUID = EntityPK;
                        DbEMT.Create(Destination, Permissions.MedicalItemTransfer.CreateGuid, ExecutionTime, DbCMS);
                    }
                    else
                    {
                        Destination.QuantityBySmallestUnit = Destination.QuantityBySmallestUnit + Convert.ToInt32(item.QuantityByPackingUnit * (Source.dataMedicalItemInputDetail.QuantityBySmallestUnit / quantity));
                        Destination.QuantityByPackingUnit = Destination.QuantityByPackingUnit + item.QuantityByPackingUnit;
                        Destination.RemainingItems = Destination.RemainingItems + item.QuantityByPackingUnit;
                        Destination.MedicalItemInputDetailTransferGUID = null;
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
                    dataItemQuantityThreshold itemQuantityThresholdFrom = DbEMT.dataItemQuantityThreshold.Where(x => x.MedicalItemGUID.ToString() == item.MedicalItemGUID && x.MedicalPharmacyGUID == Source.dataMedicalItemInputDetail.dataMedicalItemInput.MedicalPharmacyGUID && x.Active).FirstOrDefault();
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
                    var Source = DbEMT.dataMedicalItemTransferDetail.Where(x => x.MedicalItemTransferDetailGUID == item.MedicalItemTransferDetailGUID).FirstOrDefault();
                    Source.RemainingItems = Source.RemainingItems - item.QuantityByPackingUnit;

                    dataMedicalItemTransferDetail Destination = DbEMT.dataMedicalItemTransferDetail.Where(x => x.MedicalItemGUID.ToString() == item.MedicalItemGUID && x.MedicalItemTransferGUID == EntityPK && x.Active).FirstOrDefault();
                    if (Destination == null)
                    {
                        Destination = new dataMedicalItemTransferDetail();
                        Destination.MedicalItemTransferDetailGUID = Guid.NewGuid();
                        Destination.MedicalItemTransferDetailTransferGUID = Source.MedicalItemTransferDetailGUID;
                        Destination.MedicalItemInputDetailGUID = Source.MedicalItemInputDetailGUID;
                        Destination.MedicalItemGUID = Source.MedicalItemGUID;

                        Destination.QuantityByPackingTransfer = item.QuantityByPackingUnit;
                        Destination.RemainingItems = item.QuantityByPackingUnit;
                        Destination.Confirmed = null;
                        Destination.ConfirmedBy = null;
                        Destination.ConfirmedOn = null;
                        Destination.MedicalItemTransferGUID = EntityPK;

                        DbEMT.Create(Destination, Permissions.MedicalItemTransfer.CreateGuid, ExecutionTime, DbCMS);
                    }
                    else
                    {
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
                    dataItemQuantityThreshold itemQuantityThresholdFrom = DbEMT.dataItemQuantityThreshold.Where(x => x.MedicalItemGUID.ToString() == item.MedicalItemGUID && x.MedicalPharmacyGUID == Source.dataMedicalItemInputDetail.dataMedicalItemInput.MedicalPharmacyGUID && x.Active).FirstOrDefault();
                    itemQuantityThresholdFrom.QuantityTotalRemainingItems = itemQuantityThresholdFrom.QuantityTotalRemainingItems - item.QuantityByPackingUnit;
                }
            }
            return MedicalItemTransfer;
        }
        #endregion

        #region Confirm Transfer

        [Route("EMT/MedicalItemTransfers/Confirm/{PK}")]
        public ActionResult MedicalItemTransferConfirmCreate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.MedicalItemTransfer.Create, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var model = new MedicalItemTransferUpdateModel()
            {
                MedicalItemTransferGUID = PK,
                MedicalItemInputDetailsDataTableModel = (from a in DbEMT.dataMedicalItemTransferDetail.Where(x => x.MedicalItemTransferGUID == PK && x.Active)

                                                         join b in DbEMT.codeMedicalItem on a.MedicalItemGUID equals b.MedicalItemGUID
                                                         join c in DbEMT.dataMedicalItemInputDetail.Where(x => x.Active) on a.MedicalItemInputDetailGUID equals c.MedicalItemInputDetailGUID
                                                         orderby b.BrandName

                                                         select
            new MedicalItemInputDetailsDataTableModel
            {
                BrandName = b.BrandName,
                MedicalItemTransferDetailGUID = a.MedicalItemTransferDetailGUID,
                BatchNumber = c.BatchNumber,
                QuantityByPackingUnit = a.QuantityByPackingUnit,
                RemainingItems = a.RemainingItems,
                MedicalItemGUID = b.MedicalItemGUID.ToString(),
                Comments = c.Comments,
                Confirmed = a.Confirmed == null ? false : a.Confirmed.Value,
                ConfirmedBy = a.ConfirmedBy,
                ConfirmedDate = a.ConfirmedOn,
                ExpirationDate = c.ExpirationDate,
                ManufacturingDate = c.ManufacturingDate,
                MedicalItemInputGUID = c.MedicalItemInputGUID,
                QuantityByPackingTransfer = a.QuantityByPackingTransfer == null ? 0 : a.QuantityByPackingTransfer.Value,
                PriceOfPackingUnit = c.PriceOfPackingUnit,
                PriceOfSmallestUnit = c.PriceOfSmallestUnit,
                QuantityBySmallestUnit = c.QuantityBySmallestUnit
            }
                ).ToList()
            };
            return View("~/Areas/EMT/Views/MedicalItemTransfers/_MedicalItemTransferDetailConfirm.cshtml", model);

        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalItemTransferConfirmCreate(MedicalItemTransferUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.MedicalItemTransfer.Create, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            DateTime ExecutionTime = DateTime.Now;
            dataMedicalItemTransfer medicalItemTransfer = DbEMT.dataMedicalItemTransfer.Where(x => x.MedicalItemTransferGUID == model.MedicalItemTransferGUID).FirstOrDefault();

            foreach (var item in model.MedicalItemInputDetailsDataTableModel)
            {
                if (item.MedicalItemTransferDetailGUID != Guid.Empty)
                {
                    var Source = DbEMT.dataMedicalItemTransferDetail.Where(x => x.MedicalItemTransferDetailGUID == item.MedicalItemTransferDetailGUID).FirstOrDefault();

                    Source.Confirmed = item.Confirmed;
                    Source.ConfirmedBy = Source.ConfirmedBy == null && item.Confirmed ? CMS.GetFullName(UserGUID, "EN") : Source.ConfirmedBy;
                    Source.ConfirmedOn = Source.ConfirmedOn == null && item.Confirmed ? ExecutionTime : Source.ConfirmedOn;
                    medicalItemTransfer.ConfirmedReceived = item.Confirmed;

                    Source.Comments = item.Comments;
                    DbEMT.Update(Source, Permissions.MedicalItemInput.UpdateGuid, ExecutionTime, DbCMS);
                }
            }
            DbEMT.Update(medicalItemTransfer, Permissions.MedicalItemInput.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleCreateMessage(DbEMT.PrimaryKeyControl(medicalItemTransfer), DbEMT.RowVersionControls(new List<dataMedicalItemTransfer> { medicalItemTransfer }), null, "" +
                    " setTimeout(function () { window.location.href = '/EMT/MedicalItemTransfers/Confirm/" + model.MedicalItemTransferGUID + "';}, 2000); ", null));
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }
        #endregion
    }
}