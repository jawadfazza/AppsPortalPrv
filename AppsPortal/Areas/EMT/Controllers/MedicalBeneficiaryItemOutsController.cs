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
    public class MedicalBeneficiaryItemOutsController : EMTBaseController
    {
        #region Medical Beneficiary Item Out

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult DistributeBulkItems(List<MedicalItemsDataTableModel> items,Guid MedicalPharmacyGUID)
        {
            var model = new MedicalBeneficiaryItemOutUpdateModel()
            {
                DeliveryDate=DateTime.Now,
                Cost=0,
                DeliveryStatus= CMS.HasAction(Permissions.MedicalBeneficiaryItemOut.Confirm, Apps.EMT)?true:false,
                MedicalPharmacyGUID=MedicalPharmacyGUID,
                medicalBeneficiaryItemOutDetails = (from a in items.Where(x=>x.Quantity>0)
                                                    join b in DbEMT.dataMedicalItemTransferDetail.Where(x=>x.Active ) on a.FK equals b.MedicalItemTransferDetailGUID
                                                    select
                                                      new MedicalBeneficiaryItemOutDetailsDataTableModel
                                                      {
                                                          BrandName = a.BrandName,
                                                          QuantityByPackingUnit = a.Quantity>b.RemainingItems? b.RemainingItems: a.Quantity,
                                                          RemainingItems = b.RemainingItems,
                                                          MedicalItemGUID = a.MedicalItemGUID,
                                                          MedicalItemTransferDetailGUID=b.MedicalItemTransferDetailGUID
                                                      }).ToList()
            };
             return View("~/Areas/EMT/Views/MedicalBeneficiaryItemOuts/_MedicalBeneficiaryItemOutTransferUpdateModal.cshtml", model);
        }

        public ActionResult DistributeBulkItemsCreate(MedicalBeneficiaryItemOutUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.MedicalBeneficiaryItemOut.Create, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveMedicalBeneficiaryItemOut(model)) return PartialView("~/Areas/EMT/Views/MedicalBeneficiaryItemOuts/_MedicalBeneficiaryItemOutTransferUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            dataMedicalBeneficiaryItemOut dataMedicalBeneficiaryItem = Mapper.Map(model, new dataMedicalBeneficiaryItemOut());
            dataMedicalBeneficiaryItem.MedicalBeneficiaryItemOutGUID = EntityPK;
            dataMedicalBeneficiaryItem.DiagnosisGUID = string.Join(",", model.DiagnosisGUID);
            

            DbEMT.Create(dataMedicalBeneficiaryItem, Permissions.MedicalBeneficiaryItemOut.CreateGuid, ExecutionTime, DbCMS);

            foreach (var item in model.medicalBeneficiaryItemOutDetails)
            {
                if (item.QuantityByPackingUnit > 0)
                {
                    dataMedicalBeneficiaryItemOutDetail Destination = new dataMedicalBeneficiaryItemOutDetail();
                    //Check If the Item Quantity Threshold exist
                    dataItemQuantityThreshold itemQuantityThreshold = DbEMT.dataItemQuantityThreshold.Where(x => x.MedicalItemGUID.ToString() == item.MedicalItemGUID && x.MedicalPharmacyGUID == dataMedicalBeneficiaryItem.MedicalPharmacyGUID && x.Active).FirstOrDefault();
                    itemQuantityThreshold.QuantityTotalRemainingItems -= item.QuantityByPackingUnit;

                    var Source = DbEMT.dataMedicalItemTransferDetail.Where(x => x.MedicalItemTransferDetailGUID==item.MedicalItemTransferDetailGUID ).FirstOrDefault();
                    Source.dataMedicalItemTransfer.ConfirmedReceived = true;
                    Source.RemainingItems -= item.QuantityByPackingUnit;

                    Mapper.Map(Source, Destination);
                    Destination.QuantityByPackingUnit = item.QuantityByPackingUnit;
                    Destination.MedicalBeneficiaryItemOutGUID = EntityPK;
                    DbEMT.Create(Destination, Permissions.MedicalBeneficiaryItemOut.CreateGuid, ExecutionTime, DbCMS);
                    
                    //Update code Medical Item Remaining Items Quantity
                    var MedicalItem = DbEMT.codeMedicalItem.Where(x => x.MedicalItemGUID.ToString() == item.MedicalItemGUID).FirstOrDefault();
                    MedicalItem.RemainingItemsQuantity -=  item.QuantityByPackingUnit;

                }
            }
            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.MedicalAvailableItemsDataTable, ControllerContext,null));
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.MedicalBeneficiaryItemOutsDataTable, ControllerContext, null));

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                CalculateCost(dataMedicalBeneficiaryItem.MedicalBeneficiaryItemOutGUID);
              //  return Json(DbEMT.SingleCreateMessage(DbEMT.PrimaryKeyControl(dataMedicalBeneficiaryItem), DbEMT.RowVersionControls(new List<dataMedicalBeneficiaryItemOut> { dataMedicalBeneficiaryItem }), null, "", null));
                return Json(DbEMT.SingleUpdateMessage(DataTableNames.MedicalBeneficiaryItemOutsDataTable, DbEMT.PrimaryKeyControl(dataMedicalBeneficiaryItem), DbEMT.RowVersionControls(Portal.SingleToList(dataMedicalBeneficiaryItem)), "ClearQuantity();"));

            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [Route("EMT/MedicalBeneficiaryItemOuts/")]
        public ActionResult MedicalBeneficiaryItemOutsIndex()
        {
            if (!CMS.HasAction(Permissions.MedicalBeneficiaryItemOut.Access, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }          
            return View("~/Areas/EMT/Views/MedicalBeneficiaryItemOuts/Index.cshtml",new List<MedicalItemsDataTableModel>());
        }

        public ActionResult MedicalBeneficiaryItemOutsIndex(Guid? MedicalPharmacyGUID)
        {
            if (!CMS.HasAction(Permissions.MedicalBeneficiaryItemOut.Access, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            var spResult = DbEMT.spMedicalItem("EN").ToList();
            var spResult1 = (from b in DbEMT.dataMedicalItemTransferDetail.Where(x => x.RemainingItems > 0 && x.Active && x.Confirmed.Value)
                             join c in DbEMT.codeMedicalPharmacyLanguage.Where(x => x.Active && x.LanguageID == LAN && !x.codeMedicalPharmacy.MainWarehouse && x.MedicalPharmacyGUID == MedicalPharmacyGUID) on b.dataMedicalItemTransfer.MedicalPharmacyGUID equals c.MedicalPharmacyGUID
                             join d in DbEMT.dataMedicalItemInputDetail on b.MedicalItemInputDetailGUID equals d.MedicalItemInputDetailGUID
                             join e in DbEMT.dataMedicalItemInput on d.MedicalItemInputGUID equals e.MedicalItemInputGUID
                             join f in DbEMT.codeOrganizationsInstances on e.ProvidedByOrganizationInstanceGUID equals f.OrganizationInstanceGUID
                             join j in DbEMT.codeOrganizations on f.OrganizationGUID equals j.OrganizationGUID
                             select new
                             {
                                 b.MedicalItemGUID,
                                 b.MedicalItemTransferDetailGUID,
                                 c.MedicalPharmacyGUID,
                                 b.RemainingItems,
                                 b.dataMedicalItemTransfer.DeliveryDate,
                                 b.dataMedicalItemInputDetail.ExpirationDate,
                                 b.dataMedicalItemInputDetail.BatchNumber,
                                 j.OrganizationShortName
                             }).ToList();
            var All = (from a in spResult.AsQueryable()
                       join b in spResult1 on a.MedicalItemGUID equals b.MedicalItemGUID
                       select
                       new MedicalItemsDataTableModel
                       {
                           MedicalItemGUID = a.MedicalItemGUID.ToString(),
                           FK = b.MedicalItemTransferDetailGUID,
                           MedicalPharmacyGUID=b.MedicalPharmacyGUID.ToString(),
                           BrandName = a.BrandName,
                           Barcode = a.Barcode,
                           DoseQuantity = a.DoseQuantity,
                           RemainingItemsQuantity = b.RemainingItems,
                           LicenseNoDate = a.LicenseNoDate,
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
                           codeMedicalItemRowVersion = a.codeMedicalItemRowVersion,
                           Quantity = 0,
                           DeliveryDate=b.DeliveryDate,
                           ExpirationDate=b.ExpirationDate.Value,
                           BatchNumber=b.BatchNumber,
                           ProvidedBy= b.OrganizationShortName

                       }).OrderBy(x=>x.BrandName).ThenByDescending(x=>x.ExpirationDate).ThenBy(x => x.DeliveryDate).ToList();
            List<MedicalItemsDataTableModel> finalList = new List<MedicalItemsDataTableModel>();
            foreach(var row in All)
            {
                var found = All.Where(x => x.MedicalItemGUID == row.MedicalItemGUID && x.ExpirationDate >= row.ExpirationDate).FirstOrDefault();
                if (found != null)
                {
                    finalList.Remove(found);
                    finalList.Add(row);
                }
                else
                {
                    finalList.Add(row);
                }
            }
            return View("~/Areas/EMT/Views/MedicalBeneficiaryItemOuts/Index.cshtml", finalList);
        }

        [Route("EMT/MedicalBeneficiaryItemOutsDataTable/")]
        public JsonResult MedicalBeneficiaryItemOutsDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<MedicalBeneficiaryItemOutsDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<MedicalBeneficiaryItemOutsDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.MedicalPharmacy.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();
            bool CostViewAuthorization = DbCMS.userServiceHistory.Where(x => x.UserGUID == UserGUID).FirstOrDefault().EmailAddress.ToUpper().EndsWith("UNHCR.ORG");
            var All = (from a in DbEMT.dataMedicalBeneficiaryItemOut.AsExpandable()
                       join d in DbEMT.codeMedicalPharmacyLanguage.Where(x => x.Active && x.LanguageID ==LAN).Where(x => AuthorizedList.Contains(x.MedicalPharmacyGUID.ToString())) on a.MedicalPharmacyGUID equals d.MedicalPharmacyGUID
                       join b in DbEMT.codeOrganizationsInstances on d.codeMedicalPharmacy.OrganizationInstanceGUID equals b.OrganizationInstanceGUID
                       join c in DbEMT.codeOrganizations on b.OrganizationGUID equals c.OrganizationGUID
                       join e in DbEMT.dataMedicalBeneficiary on a.MedicalBeneficiaryGUID equals e.MedicalBeneficiaryGUID
                       select new MedicalBeneficiaryItemOutsDataTableModel
                       {
                           MedicalBeneficiaryItemOutGUID = a.MedicalBeneficiaryItemOutGUID,
                           Cost= CostViewAuthorization?Math.Round(a.Cost.Value, 2):0,
                           DiagnosisGUID =a.DiagnosisGUID.ToString(),
                           MedicalBeneficiaryGUID=a.MedicalBeneficiaryGUID,
                           DiseaseTypeGUID=a.DiseaseTypeGUID.ToString(),
                           UNHCRNumber= e.IDNumber == null ? e.UNHCRNumber : e.IDNumber,
                           FullName=e.RefugeeFullName,
                           DeliveryDate =a.DeliveryDate,
                           MedicalPharmacyGUID=a.MedicalPharmacyGUID.ToString(),
                           MedicalPharmacyDescription=d.MedicalPharmacyDescription,
                           DeliveryStatus=a.DeliveryStatus,
                           Active = a.Active,
                           dataMedicalBeneficiaryItemOutRowVersion = a.dataMedicalBeneficiaryItemOutRowVersion
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<MedicalBeneficiaryItemOutsDataTableModel> Result = Mapper.Map<List<MedicalBeneficiaryItemOutsDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("EMT/MedicalAvailableItemsDataTable/")]
        public JsonResult MedicalAvailableItemsDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<MedicalItemsDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<MedicalItemsDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.MedicalPharmacy.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();

            var spResult = DbEMT.spMedicalItem(LAN).ToList();
            var spResult1 = (from b in DbEMT.dataItemQuantityThreshold.Where(x=>x.QuantityTotalRemainingItems>0 && x.Active)
                             join c in DbEMT.codeMedicalPharmacyLanguage.Where(x => x.Active && x.LanguageID == LAN).Where(x => AuthorizedList.Contains(x.MedicalPharmacyGUID.ToString())) on b.MedicalPharmacyGUID equals c.MedicalPharmacyGUID select b).ToList();
            var All = (from a in spResult.AsQueryable()
                       join b in spResult1 on a.MedicalItemGUID equals b.MedicalItemGUID
                       select
                       new MedicalItemsDataTableModel
                       {
                           MedicalItemGUID = a.MedicalItemGUID.ToString(),
                           FK=b.ItemQuantityThresholdGUID,
                           BrandName = a.BrandName.ToLower(),
                           Barcode=a.Barcode,
                           DoseQuantity = a.DoseQuantity,
                           RemainingItemsQuantity = b.QuantityTotalRemainingItems.Value,
                           LicenseNoDate = a.LicenseNoDate,
                           MedicalDoseUnitDescription = a.Dose,
                           MedicalGenericNameGUID = a.MedicalGenericNameGUID.ToString(),
                           MedicalGenericNameDescription = a.MedicalGenericNameDescription,
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

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("EMT/MedicalBeneficiaryItemOuts/Create/")]
        public ActionResult MedicalBeneficiaryItemOutCreate()
        {
            if (!CMS.HasAction(Permissions.MedicalBeneficiaryItemOut.Create, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/EMT/Views/MedicalBeneficiaryItemOuts/MedicalBeneficiaryItemOut.cshtml", new MedicalBeneficiaryItemOutUpdateModel());
        }

        [Route("EMT/MedicalBeneficiaryItemOuts/Update/{PK}")]
        public ActionResult MedicalBeneficiaryItemOutUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.MedicalBeneficiaryItemOut.Access, Apps.EMT))
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
            bool CostViewAuthorization = DbCMS.userServiceHistory.Where(x => x.UserGUID == UserGUID).FirstOrDefault().EmailAddress.ToUpper().EndsWith("UNHCR.ORG");
            CalculateCost(PK);
            var model = (from a in DbEMT.dataMedicalBeneficiaryItemOut.WherePK(PK).AsEnumerable()
                         select new MedicalBeneficiaryItemOutUpdateModel
                         {
                             MedicalBeneficiaryItemOutGUID = a.MedicalBeneficiaryItemOutGUID,
                             Cost =CostViewAuthorization? (a.Cost != null? Math.Round( a.Cost.Value,2):0):0,
                             DiagnosisGUID= a.DiagnosisGUID.Split(','),
                             DiagnosisClientGUID= a.DiagnosisGUID,
                             DiseaseTypeGUID =a.DiseaseTypeGUID,
                             MedicalBeneficiaryGUID=a.MedicalBeneficiaryGUID,
                             DeliveryDate=a.DeliveryDate,
                             MedicalPharmacyGUID=a.MedicalPharmacyGUID,
                             DeliveryStatus=a.DeliveryStatus,
                             Active = a.Active,
                             dataMedicalBeneficiaryItemOutRowVersion = a.dataMedicalBeneficiaryItemOutRowVersion,
                             CreatedBy = Audit.ExecutedBy,
                             CreatedOn = Audit.ExecutionTime,
                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("MedicalBeneficiaryItemOut", "MedicalBeneficiaryItemOuts", new { Area = "EMT" }));
           

            return View("~/Areas/EMT/Views/MedicalBeneficiaryItemOuts/MedicalBeneficiaryItemOut.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalBeneficiaryItemOutCreate(MedicalBeneficiaryItemOutUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.MedicalBeneficiaryItemOut.Create, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid /*|| ActiveMedicalBeneficiaryItemOut(model)*/) return PartialView("~/Areas/EMT/Views/MedicalBeneficiaryItemOuts/_MedicalBeneficiaryItemOutForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();
            model.Cost = 0;
            dataMedicalBeneficiaryItemOut MedicalBeneficiaryItemOut = Mapper.Map(model, new dataMedicalBeneficiaryItemOut());
            MedicalBeneficiaryItemOut.MedicalBeneficiaryItemOutGUID = EntityPK;
            MedicalBeneficiaryItemOut.DiagnosisGUID = string.Join(",", model.DiagnosisGUID);
            DbEMT.Create(MedicalBeneficiaryItemOut, Permissions.MedicalBeneficiaryItemOut.CreateGuid, ExecutionTime, DbCMS);


            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.MedicalBeneficiaryItemOutDetailsDataTable, ControllerContext, "MedicalBeneficiaryItemOutDetailsContainer"));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.MedicalBeneficiaryItemOut.Create, Apps.EMT, new UrlHelper(Request.RequestContext).Action("Create", "MedicalBeneficiaryItemOuts", new { Area = "EMT" })), Container = "MedicalBeneficiaryItemOutFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.MedicalBeneficiaryItemOut.Update, Apps.EMT), Container = "MedicalBeneficiaryItemOutFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.MedicalBeneficiaryItemOut.Delete, Apps.EMT), Container = "MedicalBeneficiaryItemOutFormControls" });

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleCreateMessage(DbEMT.PrimaryKeyControl(MedicalBeneficiaryItemOut), DbEMT.RowVersionControls(new List<dataMedicalBeneficiaryItemOut> { MedicalBeneficiaryItemOut }), Partials, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalBeneficiaryItemOutUpdate(MedicalBeneficiaryItemOutUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.MedicalBeneficiaryItemOut.Update, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid /*|| ActiveMedicalBeneficiaryItemOut(model)*/) return PartialView("~/Areas/EMT/Views/MedicalBeneficiaryItemOuts/_MedicalBeneficiaryItemOutForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            dataMedicalBeneficiaryItemOut MedicalBeneficiaryItemOut = Mapper.Map(model, new dataMedicalBeneficiaryItemOut());
            MedicalBeneficiaryItemOut.DiagnosisGUID = string.Join(",", model.DiagnosisGUID);
            DbEMT.Update(MedicalBeneficiaryItemOut, Permissions.MedicalBeneficiaryItemOut.UpdateGuid, ExecutionTime, DbCMS);



            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleUpdateMessage(null, null, DbEMT.RowVersionControls(new List<dataMedicalBeneficiaryItemOut> { MedicalBeneficiaryItemOut })));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyMedicalBeneficiaryItemOut(model.MedicalBeneficiaryItemOutGUID);
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalBeneficiaryItemOutDelete(dataMedicalBeneficiaryItemOut model)
        {
            if (!CMS.HasAction(Permissions.MedicalBeneficiaryItemOut.Delete, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            List<dataMedicalBeneficiaryItemOut> DeletedMedicalBeneficiaryItemOut = DeleteMedicalBeneficiaryItemOuts(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.MedicalBeneficiaryItemOut.Restore, Apps.EMT), Container = "MedicalBeneficiaryItemOutFormControls" });

            try
            {
                int CommitedRows = DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleDeleteMessage(CommitedRows, DeletedMedicalBeneficiaryItemOut.FirstOrDefault(), "MedicalBeneficiaryItemOutDetailsContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyMedicalBeneficiaryItemOut(model.MedicalBeneficiaryItemOutGUID);
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }

        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalBeneficiaryItemOutRestore(dataMedicalBeneficiaryItemOut model)
        {
            if (!CMS.HasAction(Permissions.MedicalBeneficiaryItemOut.Restore, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
           
                return Json(DbEMT.RecordExists());
            

            List<dataMedicalBeneficiaryItemOut> RestoredMedicalBeneficiaryItemOuts = RestoreMedicalBeneficiaryItemOuts(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.MedicalBeneficiaryItemOut.Create, Apps.EMT, new UrlHelper(Request.RequestContext).Action("MedicalBeneficiaryItemOutCreate", "Configuration", new { Area = "EMT" })), Container = "MedicalBeneficiaryItemOutFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.MedicalBeneficiaryItemOut.Update, Apps.EMT), Container = "MedicalBeneficiaryItemOutFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.MedicalBeneficiaryItemOut.Delete, Apps.EMT), Container = "MedicalBeneficiaryItemOutFormControls" });

            try
            {
                int CommitedRows = DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleRestoreMessage(CommitedRows, RestoredMedicalBeneficiaryItemOuts, DbEMT.PrimaryKeyControl(RestoredMedicalBeneficiaryItemOuts.FirstOrDefault()), Url.Action(DataTableNames.MedicalBeneficiaryItemOutDetailsDataTable, Portal.GetControllerName(ControllerContext)), "MedicalBeneficiaryItemOutDetailsContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyMedicalBeneficiaryItemOut(model.MedicalBeneficiaryItemOutGUID);
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult MedicalBeneficiaryItemOutsDataTableDelete(List<dataMedicalBeneficiaryItemOut> models)
        {
            if (!CMS.HasAction(Permissions.MedicalBeneficiaryItemOut.Delete, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataMedicalBeneficiaryItemOut> DeletedMedicalBeneficiaryItemOuts = DeleteMedicalBeneficiaryItemOuts(models);

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.PartialDeleteMessage(DeletedMedicalBeneficiaryItemOuts, models, DataTableNames.MedicalBeneficiaryItemOutsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult MedicalBeneficiaryItemOutsDataTableRestore(List<dataMedicalBeneficiaryItemOut> models)
        {
            if (!CMS.HasAction(Permissions.MedicalBeneficiaryItemOut.Restore, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataMedicalBeneficiaryItemOut> RestoredMedicalBeneficiaryItemOuts = RestoreMedicalBeneficiaryItemOuts(models);

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.PartialRestoreMessage(RestoredMedicalBeneficiaryItemOuts, models, DataTableNames.MedicalBeneficiaryItemOutsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        private List<dataMedicalBeneficiaryItemOut> DeleteMedicalBeneficiaryItemOuts(List<dataMedicalBeneficiaryItemOut> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<dataMedicalBeneficiaryItemOut> DeletedMedicalBeneficiaryItemOuts = new List<dataMedicalBeneficiaryItemOut>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbEMT.QueryBuilder(models, Permissions.MedicalBeneficiaryItemOut.DeleteGuid, SubmitTypes.Delete, baseQuery);

            var Records = DbEMT.Database.SqlQuery<dataMedicalBeneficiaryItemOut>(query).ToList();
            foreach (var record in Records)
            {
                    DeletedMedicalBeneficiaryItemOuts.Add(DbEMT.Delete(record, ExecutionTime, Permissions.MedicalBeneficiaryItemOut.DeleteGuid, DbCMS));
            }

            var MedicalBeneficiaryItemOutDetails = DeletedMedicalBeneficiaryItemOuts.SelectMany(a => a.dataMedicalBeneficiaryItemOutDetail).Where(l => l.Active).ToList();
            foreach (var MedicalBeneficiaryItemOutDetail in MedicalBeneficiaryItemOutDetails)
            {
                DbEMT.Delete(MedicalBeneficiaryItemOutDetail, ExecutionTime, Permissions.MedicalBeneficiaryItemOut.DeleteGuid, DbCMS);

                AddItemQuantity(MedicalBeneficiaryItemOutDetail.MedicalBeneficiaryItemOutDetailGUID);
            }
            return DeletedMedicalBeneficiaryItemOuts;
        }

        private List<dataMedicalBeneficiaryItemOut> RestoreMedicalBeneficiaryItemOuts(List<dataMedicalBeneficiaryItemOut> models)
        {

            DateTime RestoringTime = DateTime.Now;

            List<dataMedicalBeneficiaryItemOut> RestoredMedicalBeneficiaryItemOuts = new List<dataMedicalBeneficiaryItemOut>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbEMT.QueryBuilder(models, Permissions.MedicalBeneficiaryItemOut.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Records = DbEMT.Database.SqlQuery<dataMedicalBeneficiaryItemOut>(query).ToList();
            foreach (var record in Records)
            {
              
                    RestoredMedicalBeneficiaryItemOuts.Add(DbEMT.Restore(record, Permissions.MedicalBeneficiaryItemOut.DeleteGuid, Permissions.MedicalBeneficiaryItemOut.RestoreGuid, RestoringTime, DbCMS));
                
            }

            var MedicalBeneficiaryItemOutDetails = RestoredMedicalBeneficiaryItemOuts.SelectMany(x => x.dataMedicalBeneficiaryItemOutDetail.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var MedicalBeneficiaryItemOutDetail in MedicalBeneficiaryItemOutDetails)
            {
                DbEMT.Restore(MedicalBeneficiaryItemOutDetail, Permissions.MedicalBeneficiaryItemOut.DeleteGuid, Permissions.MedicalBeneficiaryItemOut.RestoreGuid, RestoringTime, DbCMS);

                RemoveItemQuantity(MedicalBeneficiaryItemOutDetail.MedicalBeneficiaryItemOutDetailGUID);
            }

            return RestoredMedicalBeneficiaryItemOuts;
        }

        private JsonResult ConcurrencyMedicalBeneficiaryItemOut(Guid PK)
        {
            MedicalBeneficiaryItemOutUpdateModel dbModel = new MedicalBeneficiaryItemOutUpdateModel();

            var MedicalBeneficiaryItemOut = DbEMT.dataMedicalBeneficiaryItemOut.Where(x => x.MedicalBeneficiaryItemOutGUID == PK).FirstOrDefault();
            var dbMedicalBeneficiaryItemOut = DbEMT.Entry(MedicalBeneficiaryItemOut).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbMedicalBeneficiaryItemOut, dbModel);

            var MedicalBeneficiaryItemOutDetail = DbEMT.dataMedicalBeneficiaryItemOutDetail.Where(x => x.MedicalBeneficiaryItemOutGUID == PK).Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.dataMedicalBeneficiaryItemOut.DeletedOn)).FirstOrDefault();
            var dbMedicalBeneficiaryItemOutDetail = DbEMT.Entry(MedicalBeneficiaryItemOutDetail).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbMedicalBeneficiaryItemOutDetail, dbModel);

            if (MedicalBeneficiaryItemOut.dataMedicalBeneficiaryItemOutRowVersion.SequenceEqual(dbModel.dataMedicalBeneficiaryItemOutRowVersion))
            {
                return Json(DbEMT.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbEMT, dbModel, "MedicalBeneficiaryItemOutDetailsContainer"));
        }

        private bool ActiveMedicalItemTransfer(MedicalItemTransferUpdateModel model)
        {
            bool valid = false;
            int index = 0;
            foreach (var item in model.MedicalItemInputDetailsDataTableModel)
            {
                if (item.QuantityByPackingUnit > item.RemainingItems)
                {
                    ModelState.AddModelError("MedicalItemInputDetailsDataTableModel[" + index + "].QuantityByPackingUnit", "Quantity of Packing Unit :" + item.QuantityByPackingUnit + ", Grater Than Remaining Item " + item.RemainingItems);
                    valid = true;
                }
                index++;
            }

            return valid;
        }
        private bool ActiveMedicalBeneficiaryItemOut(MedicalBeneficiaryItemOutUpdateModel model)
        {
            bool error = false;
            Guid UNHCR = Guid.Parse("E156C022-EC72-4A5A-BE09-163BD85C68EF");
            var beneficiary = DbEMT.dataMedicalBeneficiary.Where(x => x.MedicalBeneficiaryGUID == model.MedicalBeneficiaryGUID).FirstOrDefault();
            var restriction = DbEMT.dataMedicalDistributionRestriction.Where(x => x.Active && x.ProvideByOrganizationInstanceGUID == UNHCR && x.BeneficiaryTypeGUID.Contains(beneficiary.BeneficiaryTypeGUID.ToString())).FirstOrDefault();
            if (restriction != null)
            {
                DateTime Expiration = DateTime.Now.AddMonths(restriction.MedicinesExpiration.Value);
                var medicineGUIDs = model.medicalBeneficiaryItemOutDetails.Select(x => x.MedicalItemGUID).ToList();
                var MedicalItemTransferDetailGUIDs = model.medicalBeneficiaryItemOutDetails.Select(x => x.MedicalItemTransferDetailGUID).ToList();
                var MedicinesExcludedByClassification = restriction.MedicinesExcludedByClassification.Split(',').ToList();
                var MedicinesExcludedByBrand = restriction.MedicinesExcludedByBrand.Split(',').ToList();
                var MedicinesExpiration = (from a in DbEMT.dataMedicalItemTransferDetail
                                           join b in DbEMT.dataMedicalItemInputDetail on a.MedicalItemInputDetailGUID equals b.MedicalItemInputDetailGUID
                                           where  b.ExpirationDate.Value<= Expiration
                                           && MedicalItemTransferDetailGUIDs.Contains(a.MedicalItemTransferDetailGUID)
                                           select a.MedicalItemGUID).ToList();
                var medicines = DbEMT.codeMedicalItem.Where(x => medicineGUIDs.Contains(x.MedicalItemGUID.ToString())).ToList();


                int PrescriptionNumberPerMonth = DbEMT.dataMedicalBeneficiaryItemOut.Where(x => x.MedicalBeneficiaryGUID == model.MedicalBeneficiaryGUID && x.DeliveryDate.Month == model.DeliveryDate.Value.Month).Count();

                //Check Prescription Number Per Month
                if (PrescriptionNumberPerMonth > restriction.PrescriptionNumberPerMonth)
                {
                    ModelState.AddModelError("MedicalBeneficiaryGUID", "Exceed Prescription Count Per Month Max:" + restriction.PrescriptionNumberPerMonth);
                    error = true;
                }
                //check Medicine Number Per Prescription
                int MedicineNumberPerPrescription = medicines.Where(x => !MedicinesExcludedByClassification.Contains(x.MedicalTreatmentGUID.ToString()) && !MedicinesExcludedByBrand.Contains(x.MedicalItemGUID.ToString()) && !MedicinesExpiration.Contains(x.MedicalItemGUID)).Count();

                if (MedicineNumberPerPrescription > restriction.MedicineNumberPerPrescription)
                {
                    ModelState.AddModelError("MedicalBeneficiaryGUID", "Exceed Medicine Count Per Prescription Max:" + restriction.MedicineNumberPerPrescription);
                    error = true;
                }
                //Medicines Quantity 
                foreach (var item in model.medicalBeneficiaryItemOutDetails)
                {
                    if (item.QuantityByPackingUnit > restriction.MedicinesQuantity)
                    {
                        ModelState.AddModelError("MedicalItemGUID", "Exceed Medicines Quantity Max:" + restriction.MedicinesQuantity + " Per Item");
                        error = true;
                    }
                }
            }
            int index = 0;
            foreach (var item in model.medicalBeneficiaryItemOutDetails)
            {
                if (item.QuantityByPackingUnit > item.RemainingItems)
                {
                    ModelState.AddModelError("medicalBeneficiaryItemOutDetails[" + index + "].QuantityByPackingUnit", "Quantity of Packing Unit :" + item.QuantityByPackingUnit + ", Grater Than Remaining Item" + item.RemainingItems);
                    error = true;
                }
                index++;
            }


            return error;
        }

        #endregion

        #region  Medical Beneficiary Item Out Details

        //[Route("EMT/MedicalBeneficiaryItemOutDetailsDataTable/{PK}")]
        public ActionResult MedicalBeneficiaryItemOutDetailsDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/EMT/Views/MedicalBeneficiaryItemOuts/_MedicalBeneficiaryItemOutDetailsDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<MedicalBeneficiaryItemOutDetailsDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<MedicalBeneficiaryItemOutDetailsDataTableModel>(DataTable.Filters);
            }
            bool CostViewAuthorization = DbCMS.userServiceHistory.Where(x => x.UserGUID == UserGUID).FirstOrDefault().EmailAddress.ToUpper().EndsWith("UNHCR.ORG");

            var Result = (from a in DbEMT.dataMedicalBeneficiaryItemOutDetail.AsNoTracking().AsExpandable().Where(x => x.MedicalBeneficiaryItemOutGUID == PK)
                          join b in DbEMT.codeMedicalItem on a.MedicalItemGUID equals b.MedicalItemGUID
                          join c in DbEMT.dataMedicalItemTransferDetail on a.MedicalItemTransferDetailGUID equals c.MedicalItemTransferDetailGUID
                          join e in DbEMT.dataMedicalItemInputDetail on c.MedicalItemInputDetailGUID equals e.MedicalItemInputDetailGUID
                          select new MedicalBeneficiaryItemOutDetailsDataTableModel
                          {
                              MedicalBeneficiaryItemOutDetailGUID = a.MedicalBeneficiaryItemOutDetailGUID,
                              MedicalItemTransferDetailGUID = a.MedicalItemTransferDetailGUID,
                              MedicalItemGUID = a.MedicalItemGUID.ToString(),
                              BrandName = b.BrandName,
                              MedicalBeneficiaryItemOutGUID = a.MedicalBeneficiaryItemOutGUID,
                              QuantityByPackingUnit = a.QuantityByPackingUnit,
                              Cost =CostViewAuthorization? Math.Round( a.QuantityByPackingUnit * e.PriceOfPackingUnit,1):0,
                              PriceOfPackingUnit = CostViewAuthorization? Math.Round( e.PriceOfPackingUnit,1):0,
                              Active = a.Active,
                              dataMedicalBeneficiaryItemOutDetailRowVersion = a.dataMedicalBeneficiaryItemOutDetailRowVersion
                          }).Where(Predicate);

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult MedicalBeneficiaryItemOutDetailCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.MedicalBeneficiaryItemOut.Create, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var MedicalPharmacy = (from a in DbEMT.dataMedicalBeneficiaryItemOut.Where(x => x.MedicalBeneficiaryItemOutGUID == FK)
                                   join b in DbEMT.codeMedicalPharmacy on a.MedicalPharmacyGUID equals b.MedicalPharmacyGUID
                                   select b).FirstOrDefault();
            return PartialView("~/Areas/EMT/Views/MedicalBeneficiaryItemOuts/_MedicalBeneficiaryItemOutDetailsUpdateModal.cshtml",
                new MedicalBeneficiaryItemOutDetailsUpdateModel { MedicalBeneficiaryItemOutGUID = FK, MedicalPharmacyGUID = MedicalPharmacy.MedicalPharmacyGUID });
        }

        public ActionResult MedicalBeneficiaryItemOutDetailUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.MedicalBeneficiaryItemOut.Access, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var medicalBeneficiaryItemOutDetail = (from a in DbEMT.dataMedicalBeneficiaryItemOut
                                                   join b in DbEMT.codeMedicalPharmacy on a.MedicalPharmacyGUID equals b.MedicalPharmacyGUID
                                                   join c in DbEMT.dataMedicalBeneficiaryItemOutDetail.Where(x => x.MedicalBeneficiaryItemOutDetailGUID == PK) on a.MedicalBeneficiaryItemOutGUID equals c.MedicalBeneficiaryItemOutGUID
                                                   select new
                                              MedicalBeneficiaryItemOutDetailsUpdateModel
                                                   {
                                                       MedicalBeneficiaryItemOutDetailGUID = c.MedicalBeneficiaryItemOutDetailGUID,
                                                       MedicalBeneficiaryItemOutGUID = a.MedicalBeneficiaryItemOutGUID,
                                                       MedicalPharmacyGUID = b.MedicalPharmacyGUID,
                                                       MedicalItemGUID = c.MedicalItemGUID,
                                                       MedicalItemTransferDetailGUID = c.MedicalItemTransferDetailGUID,
                                                       QuantityByPackingUnit = c.QuantityByPackingUnit,
                                                       Active = c.Active,
                                                       dataMedicalBeneficiaryItemOutDetailRowVersion = c.dataMedicalBeneficiaryItemOutDetailRowVersion
                                                   }
                                           ).FirstOrDefault();
            return PartialView("~/Areas/EMT/Views/MedicalBeneficiaryItemOuts/_MedicalBeneficiaryItemOutDetailsUpdateModal.cshtml",
                medicalBeneficiaryItemOutDetail);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalBeneficiaryItemOutDetailCreate(MedicalBeneficiaryItemOutDetailsUpdateModel model1)
        {
            if (!CMS.HasAction(Permissions.MedicalBeneficiaryItemOut.Create, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            
            if (!ModelState.IsValid || ActiveMedicalBeneficiaryItemOutDetail(model1)) return PartialView("~/Areas/EMT/Views/MedicalBeneficiaryItemOuts/_MedicalBeneficiaryItemOutDetailsUpdateModal.cshtml", model1);
            dataMedicalBeneficiaryItemOutDetail model = Mapper.Map(model1, new dataMedicalBeneficiaryItemOutDetail());
            DateTime ExecutionTime = DateTime.Now;
            model.MedicalBeneficiaryItemOutDetailGUID = Guid.NewGuid();

            DbEMT.Create(model, Permissions.MedicalBeneficiaryItemOut.CreateGuid, ExecutionTime, DbCMS);

            //update Item (Quantity Threshold and Input Detail) which we transfer the items <from> it.
            dataMedicalItemTransferDetail itemImputDetail = DbEMT.dataMedicalItemTransferDetail.Where(x => x.MedicalItemTransferDetailGUID == model.MedicalItemTransferDetailGUID).FirstOrDefault();
            itemImputDetail.RemainingItems = itemImputDetail.RemainingItems - model.QuantityByPackingUnit;

            dataItemQuantityThreshold itemQuantityThresholdFrom = DbEMT.dataItemQuantityThreshold.Where(x => x.MedicalItemGUID == model.MedicalItemGUID && x.MedicalPharmacyGUID == itemImputDetail.dataMedicalItemTransfer.MedicalPharmacyGUID && x.Active).FirstOrDefault();
            itemQuantityThresholdFrom.QuantityTotalRemainingItems = itemQuantityThresholdFrom.QuantityTotalRemainingItems - model.QuantityByPackingUnit;
            itemQuantityThresholdFrom.codeMedicalItem.RemainingItemsQuantity = itemQuantityThresholdFrom.codeMedicalItem.RemainingItemsQuantity - model.QuantityByPackingUnit;

            
            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                CalculateCost(model.MedicalBeneficiaryItemOutGUID);
                return Json(DbEMT.SingleUpdateMessage(DataTableNames.MedicalBeneficiaryItemOutDetailsDataTable, DbEMT.PrimaryKeyControl(model), DbEMT.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        private void CalculateCost(Guid? medicalBeneficiaryItemOutGUID)
        {
            var Details = DbEMT.dataMedicalBeneficiaryItemOutDetail.Where(x => x.MedicalBeneficiaryItemOutGUID == medicalBeneficiaryItemOutGUID && x.Active).ToList();
            double cost = 0;
            foreach(var Detail in Details)
            {
                dataMedicalItemInputDetail medicalItemInputDetail= DbEMT.dataMedicalItemTransferDetail.Where(x => x.MedicalItemTransferDetailGUID == Detail.MedicalItemTransferDetailGUID).FirstOrDefault().dataMedicalItemInputDetail;
                cost = cost + (Detail.QuantityByPackingUnit * medicalItemInputDetail.PriceOfPackingUnit);
                
            }
            DbEMT.dataMedicalBeneficiaryItemOut.Where(x => x.MedicalBeneficiaryItemOutGUID == medicalBeneficiaryItemOutGUID && x.Active).FirstOrDefault().Cost = cost;
            DbEMT.SaveChanges();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalBeneficiaryItemOutDetailUpdate(MedicalBeneficiaryItemOutDetailsUpdateModel model1)
        {
            if (!CMS.HasAction(Permissions.MedicalBeneficiaryItemOut.Update, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            if (!ModelState.IsValid || ActiveMedicalBeneficiaryItemOutDetail(model1)) return PartialView("~/Areas/EMT/Views/MedicalBeneficiaryItemOuts/_MedicalBeneficiaryItemOutDetailsUpdateModal.cshtml", model1);
            dataMedicalBeneficiaryItemOutDetail model = Mapper.Map(model1, new dataMedicalBeneficiaryItemOutDetail());

            AddItemQuantity(model.MedicalBeneficiaryItemOutDetailGUID);
            DateTime ExecutionTime = DateTime.Now;
            DbEMT.Update(model, Permissions.MedicalBeneficiaryItemOut.UpdateGuid, ExecutionTime, DbCMS);
            RemoveItemQuantity(model.MedicalBeneficiaryItemOutDetailGUID);
            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                CalculateCost(model.MedicalBeneficiaryItemOutGUID);
                return Json(DbEMT.SingleUpdateMessage(DataTableNames.MedicalBeneficiaryItemOutDetailsDataTable,
                    DbEMT.PrimaryKeyControl(model),
                    DbEMT.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyMedicalBeneficiaryItemOutDetail(model.MedicalBeneficiaryItemOutDetailGUID);
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalBeneficiaryItemOutDetailDelete(dataMedicalBeneficiaryItemOutDetail model)
        {
            if (!CMS.HasAction(Permissions.MedicalBeneficiaryItemOut.Delete, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            List<dataMedicalBeneficiaryItemOutDetail> DeletedLanguages = DeleteMedicalBeneficiaryItemOutDetails(new List<dataMedicalBeneficiaryItemOutDetail> { model });

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                CalculateCost(DeletedLanguages.FirstOrDefault().MedicalBeneficiaryItemOutGUID);
                return Json(DbEMT.SingleDeleteMessage(DeletedLanguages, DataTableNames.MedicalBeneficiaryItemOutDetailsDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyMedicalBeneficiaryItemOutDetail(model.MedicalBeneficiaryItemOutDetailGUID);
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }

        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalBeneficiaryItemOutDetailRestore(MedicalBeneficiaryItemOutDetailsUpdateModel model1)
        {
            if (!CMS.HasAction(Permissions.MedicalBeneficiaryItemOut.Restore, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            dataMedicalBeneficiaryItemOutDetail model = Mapper.Map(model1, new dataMedicalBeneficiaryItemOutDetail());
            if (ActiveMedicalBeneficiaryItemOutDetail(model1))
            {
                return Json(DbEMT.RecordExists());
            }

            List<dataMedicalBeneficiaryItemOutDetail> RestoredLanguages = RestoreMedicalBeneficiaryItemOutDetails(Portal.SingleToList(model));

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                CalculateCost(RestoredLanguages.FirstOrDefault().MedicalBeneficiaryItemOutGUID);
                return Json(DbEMT.SingleRestoreMessage(RestoredLanguages, DataTableNames.MedicalBeneficiaryItemOutDetailsDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyMedicalBeneficiaryItemOutDetail(model.MedicalBeneficiaryItemOutDetailGUID);
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult MedicalBeneficiaryItemOutDetailsDataTableDelete(List<dataMedicalBeneficiaryItemOutDetail> models)
        {
            if (!CMS.HasAction(Permissions.MedicalBeneficiaryItemOut.Delete, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            Guid MedicalBeneficiaryItemOutDetailGUID = models.FirstOrDefault().MedicalBeneficiaryItemOutDetailGUID;
            var MedicalBeneficiaryItemOut = DbEMT.dataMedicalBeneficiaryItemOut.Where(x => x.dataMedicalBeneficiaryItemOutDetail.FirstOrDefault().MedicalBeneficiaryItemOutDetailGUID == MedicalBeneficiaryItemOutDetailGUID).FirstOrDefault();
            List<dataMedicalBeneficiaryItemOutDetail> DeletedLanguages = DeleteMedicalBeneficiaryItemOutDetails(models);

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                CalculateCost(DeletedLanguages.FirstOrDefault().MedicalBeneficiaryItemOutGUID);
                return Json(DbEMT.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.MedicalBeneficiaryItemOutDetailsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }

        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult MedicalBeneficiaryItemOutDetailsDataTableRestore(List<dataMedicalBeneficiaryItemOutDetail> models)
        {
            if (!CMS.HasAction(Permissions.MedicalBeneficiaryItemOut.Restore, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataMedicalBeneficiaryItemOutDetail> RestoredLanguages = RestoreMedicalBeneficiaryItemOutDetails(models);

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                CalculateCost(RestoredLanguages.FirstOrDefault().MedicalBeneficiaryItemOutGUID);
                return Json(DbEMT.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.MedicalBeneficiaryItemOutDetailsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        private List<dataMedicalBeneficiaryItemOutDetail> DeleteMedicalBeneficiaryItemOutDetails(List<dataMedicalBeneficiaryItemOutDetail> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<dataMedicalBeneficiaryItemOutDetail> DeletedMedicalBeneficiaryItemOutDetails = new List<dataMedicalBeneficiaryItemOutDetail>();

            string query = DbEMT.QueryBuilder(models, Permissions.MedicalBeneficiaryItemOut.DeleteGuid, SubmitTypes.Delete, "");

            var MedicalBeneficiaryItemOutDetails = DbEMT.Database.SqlQuery<dataMedicalBeneficiaryItemOutDetail>(query).ToList();

            //var MedicalBeneficiaryItemOut = DbEMT.dataMedicalBeneficiaryItemOut.Where(x => x.MedicalBeneficiaryItemOutGUID == models.FirstOrDefault().MedicalBeneficiaryItemOutGUID).FirstOrDefault();
            foreach (var MedicalBeneficiaryItemOutDetail in MedicalBeneficiaryItemOutDetails)
            {
                DeletedMedicalBeneficiaryItemOutDetails.Add(DbEMT.Delete(MedicalBeneficiaryItemOutDetail, ExecutionTime, Permissions.MedicalBeneficiaryItemOut.DeleteGuid, DbCMS));

                AddItemQuantity(MedicalBeneficiaryItemOutDetail.MedicalBeneficiaryItemOutDetailGUID);
            }
            return DeletedMedicalBeneficiaryItemOutDetails;
        }

        private List<dataMedicalBeneficiaryItemOutDetail> RestoreMedicalBeneficiaryItemOutDetails(List<dataMedicalBeneficiaryItemOutDetail> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<dataMedicalBeneficiaryItemOutDetail> RestoredLanguages = new List<dataMedicalBeneficiaryItemOutDetail>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbEMT.QueryBuilder(models, Permissions.MedicalBeneficiaryItemOut.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var MedicalBeneficiaryItemOutDetails = DbEMT.Database.SqlQuery<dataMedicalBeneficiaryItemOutDetail>(query).ToList();

            //var MedicalBeneficiaryItemOut = DbEMT.dataMedicalBeneficiaryItemOut.Where(x => x.MedicalBeneficiaryItemOutGUID == models.FirstOrDefault().MedicalBeneficiaryItemOutGUID).FirstOrDefault();
            foreach (var MedicalBeneficiaryItemOutDetail in MedicalBeneficiaryItemOutDetails)
            {
                MedicalBeneficiaryItemOutDetailsUpdateModel model = Mapper.Map(MedicalBeneficiaryItemOutDetail, new MedicalBeneficiaryItemOutDetailsUpdateModel());
                if (!ActiveMedicalBeneficiaryItemOutDetail(model))
                {
                    RestoredLanguages.Add(DbEMT.Restore(MedicalBeneficiaryItemOutDetail, Permissions.MedicalBeneficiaryItemOut.DeleteGuid, Permissions.MedicalBeneficiaryItemOut.RestoreGuid, RestoringTime, DbCMS));
                    RemoveItemQuantity(MedicalBeneficiaryItemOutDetail.MedicalBeneficiaryItemOutDetailGUID);
                }
            }
            

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyMedicalBeneficiaryItemOutDetail(Guid PK)
        {
            dataMedicalBeneficiaryItemOutDetail dbModel = new dataMedicalBeneficiaryItemOutDetail();

            var Language = DbEMT.dataMedicalBeneficiaryItemOutDetail.Where(l => l.MedicalBeneficiaryItemOutDetailGUID == PK).FirstOrDefault();
            var dbLanguage = DbEMT.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.dataMedicalBeneficiaryItemOutDetailRowVersion.SequenceEqual(dbModel.dataMedicalBeneficiaryItemOutDetailRowVersion))
            {
                return Json(DbEMT.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbEMT, dbModel, "MedicalBeneficiaryItemOutDetailsContainer"));
        }

        private bool ActiveMedicalBeneficiaryItemOutDetail(MedicalBeneficiaryItemOutDetailsUpdateModel model)
        {
            int BeneficiaryItemOutDetail = DbEMT.dataMedicalBeneficiaryItemOutDetail
                                  .Where(x =>  x.MedicalBeneficiaryItemOutDetailGUID != model.MedicalBeneficiaryItemOutDetailGUID &&
                                              x.MedicalBeneficiaryItemOutGUID == model.MedicalBeneficiaryItemOutGUID &&
                                              x.MedicalItemGUID == model.MedicalItemGUID &&
                                              x.Active).Count();
            if (BeneficiaryItemOutDetail > 0)
            {
                ModelState.AddModelError("MedicalBeneficiaryItemOutDetailGUID", "Item already exists"); //From resource ?????? Amer  
            }
            var MedicalItemTransferDetail = DbEMT.dataMedicalItemTransferDetail.Where(x => x.MedicalItemTransferDetailGUID == model.MedicalItemTransferDetailGUID).FirstOrDefault();
            if (model.QuantityByPackingUnit > MedicalItemTransferDetail.RemainingItems)
            {
                ModelState.AddModelError("QuantityByPackingUnit", "Quantity of Batch Number Remaining not Enough"); //From resource ?????? Amer  
                BeneficiaryItemOutDetail = BeneficiaryItemOutDetail + 1;
            }
            return (BeneficiaryItemOutDetail > 0);
        }

        public void AddItemQuantity(Guid MedicalItemTransferDetailGUID)
        {
            //UPDATE Medical Beneficiary Item Out Detail Quantity 
            var MedicalBeneficiaryItemOutDetail = DbEMT.dataMedicalBeneficiaryItemOutDetail.Where(x => x.MedicalBeneficiaryItemOutDetailGUID == MedicalItemTransferDetailGUID).FirstOrDefault();
            //ADD the QuantityByPackingUnit from  RemainingItemsQuantity
            dataItemQuantityThreshold itemQuantityThresholdRemove = DbEMT.dataItemQuantityThreshold.Where(x => x.MedicalItemGUID == MedicalBeneficiaryItemOutDetail.MedicalItemGUID && x.MedicalPharmacyGUID == MedicalBeneficiaryItemOutDetail.dataMedicalBeneficiaryItemOut.MedicalPharmacyGUID && x.Active).FirstOrDefault();
            itemQuantityThresholdRemove.QuantityTotalRemainingItems = itemQuantityThresholdRemove.QuantityTotalRemainingItems + MedicalBeneficiaryItemOutDetail.QuantityByPackingUnit;
            itemQuantityThresholdRemove.codeMedicalItem.RemainingItemsQuantity = itemQuantityThresholdRemove.codeMedicalItem.RemainingItemsQuantity + MedicalBeneficiaryItemOutDetail.QuantityByPackingUnit;

            dataMedicalItemTransferDetail medicalItemTransferDetail = DbEMT.dataMedicalItemTransferDetail.Where(x => x.MedicalItemTransferDetailGUID == MedicalBeneficiaryItemOutDetail.MedicalItemTransferDetailGUID).FirstOrDefault();
            medicalItemTransferDetail.RemainingItems = medicalItemTransferDetail.RemainingItems + MedicalBeneficiaryItemOutDetail.QuantityByPackingUnit;

        }

        public void RemoveItemQuantity(Guid MedicalItemTransferDetailGUID)
        {
            //UPDATE Medical Beneficiary Item Out Detail Quantity 
            var MedicalBeneficiaryItemOutDetail = DbEMT.dataMedicalBeneficiaryItemOutDetail.Where(x => x.MedicalBeneficiaryItemOutDetailGUID == MedicalItemTransferDetailGUID).FirstOrDefault();

            //ADD the QuantityByPackingUnit from  RemainingItemsQuantity
            dataItemQuantityThreshold itemQuantityThresholdRemove = DbEMT.dataItemQuantityThreshold.Where(x => x.MedicalItemGUID == MedicalBeneficiaryItemOutDetail.MedicalItemGUID && x.MedicalPharmacyGUID == MedicalBeneficiaryItemOutDetail.dataMedicalBeneficiaryItemOut.MedicalPharmacyGUID && x.Active).FirstOrDefault();
            itemQuantityThresholdRemove.QuantityTotalRemainingItems = itemQuantityThresholdRemove.QuantityTotalRemainingItems - MedicalBeneficiaryItemOutDetail.QuantityByPackingUnit;
            itemQuantityThresholdRemove.codeMedicalItem.RemainingItemsQuantity = itemQuantityThresholdRemove.codeMedicalItem.RemainingItemsQuantity - MedicalBeneficiaryItemOutDetail.QuantityByPackingUnit;

            dataMedicalItemTransferDetail medicalItemTransferDetail = DbEMT.dataMedicalItemTransferDetail.Where(x => x.MedicalItemTransferDetailGUID == MedicalBeneficiaryItemOutDetail.MedicalItemTransferDetailGUID).FirstOrDefault();
            medicalItemTransferDetail.RemainingItems = medicalItemTransferDetail.RemainingItems - MedicalBeneficiaryItemOutDetail.QuantityByPackingUnit;

        }

        #endregion
    }
}