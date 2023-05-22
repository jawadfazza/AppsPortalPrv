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
    public class MedicalDiscrepancysController : EMTBaseController
    {

        #region Medical Beneficiary Item Out

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult DistributeBulkItems(List<MedicalItemsDataTableModel> items, Guid MedicalPharmacyGUID)
        {
            bool IsMainWarehouse = DbEMT.codeMedicalPharmacy.Where(x => x.MedicalPharmacyGUID == MedicalPharmacyGUID).FirstOrDefault().MainWarehouse;
            if (IsMainWarehouse)
            {
                var model = new MedicalDiscrepancyUpdateModel()
                {
                    DiscrepancyDate = DateTime.Now,
                    MedicalPharmacyGUID = MedicalPharmacyGUID,
                    ConfirmedBy = CMS.GetFullName(UserGUID, LAN),
                    ConfirmedOn = DateTime.Now,
                    
                    MedicalDiscrepancyDetails = (from a in items.Where(x=>x.Active)
                                                 join b in DbEMT.dataMedicalItemInputDetail.Where(x => x.Active) on a.FK equals b.MedicalItemInputDetailGUID
                                                 select
                                                   new MedicalDiscrepancyDetailsDataTableModel
                                                   {
                                                       BrandName = a.BrandName,
                                                       DiscrepancyQuantity = (int)(a.Quantity - b.RemainingItems),
                                                       OriginalQuantity = (int)b.RemainingItems,
                                                       RemainingQuaintity = (int) b.RemainingItems + (int)(a.Quantity - b.RemainingItems),
                                                       Comment=a.Comment,
                                                       Active = true,
                                                       MedicalItemGUID = a.MedicalItemGUID,
                                                       ReferenceItemGUID = b.MedicalItemInputDetailGUID
                                                   }).ToList()

                };
                return View("~/Areas/EMT/Views/MedicalDiscrepancys/_MedicalDiscrepancyTransferUpdateModal.cshtml", model);
            }
            else
            {
                var model = new MedicalDiscrepancyUpdateModel()
                {
                    DiscrepancyDate = DateTime.Now,
                    MedicalPharmacyGUID = MedicalPharmacyGUID,
                    ConfirmedBy = CMS.GetFullName(UserGUID, LAN),
                    ConfirmedOn = DateTime.Now,

                    MedicalDiscrepancyDetails = (from a in items.Where(x => x.Active)
                                                 join b in DbEMT.dataMedicalItemTransferDetail.Where(x => x.Active) on a.FK equals b.MedicalItemTransferDetailGUID
                                                 select
                                                   new MedicalDiscrepancyDetailsDataTableModel
                                                   {
                                                       
                                                       BrandName=a.BrandName,
                                                       DiscrepancyQuantity = (a.Quantity - b.RemainingItems),
                                                       OriginalQuantity = b.RemainingItems,
                                                       RemainingQuaintity = b.RemainingItems +(a.Quantity - b.RemainingItems),
                                                       Comment = a.Comment,
                                                       Active = true,
                                                       MedicalItemGUID = a.MedicalItemGUID,
                                                       ReferenceItemGUID = b.MedicalItemTransferDetailGUID
                                                   }).ToList()
                };
                return View("~/Areas/EMT/Views/MedicalDiscrepancys/_MedicalDiscrepancyTransferUpdateModal.cshtml", model);
            }
        }

        public ActionResult DistributeBulkItemsCreate(MedicalDiscrepancyUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.MedicalItemInput.Create, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            model.ConfirmedBy = CMS.GetFullName(UserGUID, LAN);
            model.ConfirmedOn = DateTime.Now;
            if (!ModelState.IsValid ) return PartialView("~/Areas/EMT/Views/MedicalDiscrepancys/_MedicalDiscrepancyTransferUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            dataMedicalDiscrepancy dataMedicalBeneficiaryItem = Mapper.Map(model, new dataMedicalDiscrepancy());
            dataMedicalBeneficiaryItem.MedicalDiscrepancyGUID = EntityPK;


            DbEMT.Create(dataMedicalBeneficiaryItem, Permissions.MedicalItemInput.CreateGuid, ExecutionTime, DbCMS);

            foreach (var item in model.MedicalDiscrepancyDetails)
            {
                dataMedicalDiscrepancyDetail Destination = new dataMedicalDiscrepancyDetail();
                //Check If the Item Quantity Threshold exist
                dataItemQuantityThreshold itemQuantityThreshold = DbEMT.dataItemQuantityThreshold.Where(x => x.MedicalItemGUID.ToString() == item.MedicalItemGUID && x.MedicalPharmacyGUID == dataMedicalBeneficiaryItem.MedicalPharmacyGUID && x.Active).FirstOrDefault();
                itemQuantityThreshold.QuantityTotalRemainingItems = itemQuantityThreshold.QuantityTotalRemainingItems + item.DiscrepancyQuantity;

              
                bool IsMainWarehouse = DbEMT.codeMedicalPharmacy.Where(x => x.MedicalPharmacyGUID == model.MedicalPharmacyGUID).FirstOrDefault().MainWarehouse;
                //in case  warehouse add the +-Discrepancy value to  Item Input Detail
                if (IsMainWarehouse)
                {
                    var Source = DbEMT.dataMedicalItemInputDetail.Where(x => x.MedicalItemInputDetailGUID == item.ReferenceItemGUID && x.dataMedicalItemInput.MedicalPharmacyGUID == dataMedicalBeneficiaryItem.MedicalPharmacyGUID && x.Active && x.RemainingItems > 0).OrderBy(x => x.dataMedicalItemInput.DeliveryDate).FirstOrDefault();
                    Source.dataMedicalItemInput.ConfirmedReceived = true;
                    Source.RemainingItems = (int)item.RemainingQuaintity;
                    Mapper.Map(Source, Destination);
                }//in case  pharmacy add the +-Discrepancy value to  Item Transfer Detail
                    else
                    {
                        var Source = DbEMT.dataMedicalItemTransferDetail.Where(x => x.MedicalItemTransferDetailGUID == item.ReferenceItemGUID && x.dataMedicalItemTransfer.MedicalPharmacyGUID == dataMedicalBeneficiaryItem.MedicalPharmacyGUID && x.Active && x.RemainingItems > 0).OrderBy(x => x.dataMedicalItemTransfer.DeliveryDate).FirstOrDefault();
                        Source.dataMedicalItemTransfer.ConfirmedReceived = true;
                        Source.RemainingItems = item.RemainingQuaintity;
                        Mapper.Map(Source, Destination);
                    }


                Destination.DiscrepancyQuantity = item.DiscrepancyQuantity;
                Destination.OriginalQuantity = item.OriginalQuantity;
                Destination.RemainingItems = item.RemainingQuaintity;
                Destination.ReferenceItemGUID = item.ReferenceItemGUID;
                Destination.Comment = item.Comment;
                Destination.MedicalDiscrepancyGUID = EntityPK;
                DbEMT.Create(Destination, Permissions.MedicalItemInput.CreateGuid, ExecutionTime, DbCMS);

                //Update code Medical Item Remaining Items Quantity
                var MedicalItem = DbEMT.codeMedicalItem.Where(x => x.MedicalItemGUID.ToString() == item.MedicalItemGUID).FirstOrDefault();
                MedicalItem.RemainingItemsQuantity = item.DiscrepancyQuantity;
            }
            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.MedicalAvailableItemsDataTable, ControllerContext, null));
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.MedicalDiscrepancysDataTable, ControllerContext, null));

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                //  return Json(DbEMT.SingleCreateMessage(DbEMT.PrimaryKeyControl(dataMedicalBeneficiaryItem), DbEMT.RowVersionControls(new List<dataMedicalDiscrepancy> { dataMedicalBeneficiaryItem }), null, "", null));
                return Json(DbEMT.SingleUpdateMessage(DataTableNames.MedicalDiscrepancysDataTable, DbEMT.PrimaryKeyControl(dataMedicalBeneficiaryItem), DbEMT.RowVersionControls(Portal.SingleToList(dataMedicalBeneficiaryItem)), "ClearQuantity();"));

            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [Route("EMT/MedicalDiscrepancys/")]
        public ActionResult MedicalDiscrepancysIndex()
        {
            if (!CMS.HasAction(Permissions.MedicalDiscrepancy.Access, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/EMT/Views/MedicalDiscrepancys/Index.cshtml", new List<MedicalItemsDataTableModel>());
        }

        public ActionResult MedicalDiscrepancysIndex(Guid? MedicalPharmacyGUID)
        {
            if (!CMS.HasAction(Permissions.MedicalDiscrepancy.Access, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            var Pharmacy = DbEMT.codeMedicalPharmacy.Where(x => x.MedicalPharmacyGUID == MedicalPharmacyGUID).FirstOrDefault();
            if (Pharmacy != null)
            {
                bool IsMainWarehouse = Pharmacy.MainWarehouse;
                var spResult = DbEMT.spMedicalItem(LAN).ToList();
                //in case  warehouse add the +-Discrepancy value to  Item Input Detail
                if (IsMainWarehouse)
                {
                    var spResult1 = (from b in DbEMT.dataMedicalItemInputDetail.Where(x => x.RemainingItems > 0 && x.Active && x.Confirmed)
                                     join c in DbEMT.codeMedicalPharmacyLanguage.Where(x => x.Active && x.LanguageID == LAN && x.codeMedicalPharmacy.MainWarehouse && x.MedicalPharmacyGUID == MedicalPharmacyGUID) on b.dataMedicalItemInput.MedicalPharmacyGUID equals c.MedicalPharmacyGUID
                                     join e in DbEMT.dataMedicalItemInput on b.MedicalItemInputGUID equals e.MedicalItemInputGUID
                                     join f in DbEMT.codeOrganizationsInstances on e.ProvidedByOrganizationInstanceGUID equals f.OrganizationInstanceGUID
                                     join j in DbEMT.codeOrganizations on f.OrganizationGUID equals j.OrganizationGUID
                                     select new
                                     {
                                         b.MedicalItemGUID,
                                         b.MedicalItemInputDetailGUID,
                                         c.MedicalPharmacyGUID,
                                         b.RemainingItems,
                                         b.dataMedicalItemInput.DeliveryDate,
                                         b.ExpirationDate,
                                         b.BatchNumber,
                                         j.OrganizationShortName
                                     }).ToList();
                    var All = (from a in spResult.AsQueryable()
                               join b in spResult1 on a.MedicalItemGUID equals b.MedicalItemGUID
                               orderby a.BrandName, b.ExpirationDate
                               select
                               new MedicalItemsDataTableModel
                               {
                                   
                                   MedicalItemGUID = a.MedicalItemGUID.ToString(),
                                   FK = b.MedicalItemInputDetailGUID,
                                   MedicalPharmacyGUID = b.MedicalPharmacyGUID.ToString(),
                                   BrandName = a.BrandName,
                                   Barcode = a.Barcode,
                                   DoseQuantity = a.DoseQuantity,
                                   RemainingItemsQuantity = (double)b.RemainingItems,
                                   LicenseNoDate = a.LicenseNoDate,
                                   MedicalDoseUnitDescription = a.Dose,
                                   MedicalGenericNameGUID = a.MedicalGenericNameGUID.ToString(),
                                   MedicalGenericNameDescription = a.MedicalGenericNameDescription.Replace("+", " "),
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
                                   Quantity = (double)b.RemainingItems,
                                   DeliveryDate = b.DeliveryDate.Value,
                                   ExpirationDate = b.ExpirationDate.Value,
                                   BatchNumber = b.BatchNumber,
                                   ProvidedBy = b.OrganizationShortName

                               }).ToList();
                    return View("~/Areas/EMT/Views/MedicalDiscrepancys/Index.cshtml", All);
                }
                else
                {
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
                               orderby a.BrandName, b.ExpirationDate
                               select
                               new MedicalItemsDataTableModel
                               {
                                   MedicalItemGUID = a.MedicalItemGUID.ToString(),
                                   FK = b.MedicalItemTransferDetailGUID,
                                   MedicalPharmacyGUID = b.MedicalPharmacyGUID.ToString(),
                                   BrandName = a.BrandName,
                                   Barcode = a.Barcode,
                                   DoseQuantity = a.DoseQuantity,
                                   RemainingItemsQuantity = b.RemainingItems,
                                   LicenseNoDate = a.LicenseNoDate,
                                   MedicalDoseUnitDescription = a.Dose,
                                   MedicalGenericNameGUID = a.MedicalGenericNameGUID.ToString(),
                                   MedicalGenericNameDescription = a.MedicalGenericNameDescription.Replace("+", " "),
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
                                   Quantity = (double)b.RemainingItems,
                                   DeliveryDate = b.DeliveryDate,
                                   ExpirationDate = b.ExpirationDate.Value,
                                   BatchNumber = b.BatchNumber,
                                   ProvidedBy = b.OrganizationShortName

                               }).ToList();
                    return View("~/Areas/EMT/Views/MedicalDiscrepancys/Index.cshtml", All);
                }

            }
            return View("~/Areas/EMT/Views/MedicalDiscrepancys/Index.cshtml", new List<MedicalItemsDataTableModel>());
        }

        [Route("EMT/MedicalDiscrepancysDataTable/")]
        public JsonResult MedicalDiscrepancysDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<MedicalDiscrepancysDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<MedicalDiscrepancysDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.MedicalPharmacy.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();

            var All = (from a in DbEMT.dataMedicalDiscrepancy.AsExpandable()
                       join d in DbEMT.codeMedicalPharmacyLanguage.Where(x => x.Active && x.LanguageID == LAN).Where(x => AuthorizedList.Contains(x.MedicalPharmacyGUID.ToString())) on a.MedicalPharmacyGUID equals d.MedicalPharmacyGUID
                       join b in DbEMT.codeOrganizationsInstances on d.codeMedicalPharmacy.OrganizationInstanceGUID equals b.OrganizationInstanceGUID
                       join c in DbEMT.codeOrganizations on b.OrganizationGUID equals c.OrganizationGUID
                      
                       select new MedicalDiscrepancysDataTableModel
                       {
                           MedicalDiscrepancyGUID = a.MedicalDiscrepancyGUID,
                           MedicalPharmacyGUID = a.MedicalPharmacyGUID,
                           MedicalPharmacyDescription=d.MedicalPharmacyDescription,
                           DiscrepancyDate=a.DiscrepancyDate,
                           DiscrepancyType=a.DiscrepancyType==1? "Periodic inventory": "Annual inventory",
                          
                           Active = a.Active,
                           dataMedicalDiscrepancyRowVersion = a.dataMedicalDiscrepancyRowVersion
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<MedicalDiscrepancysDataTableModel> Result = Mapper.Map<List<MedicalDiscrepancysDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

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
            var spResult1 = (from b in DbEMT.dataItemQuantityThreshold.Where(x => x.QuantityTotalRemainingItems > 0 && x.Active)
                             join c in DbEMT.codeMedicalPharmacyLanguage.Where(x => x.Active && x.LanguageID == LAN).Where(x => AuthorizedList.Contains(x.MedicalPharmacyGUID.ToString())) on b.MedicalPharmacyGUID equals c.MedicalPharmacyGUID
                             select b).ToList();
            var All = (from a in spResult.AsQueryable()
                       join b in spResult1 on a.MedicalItemGUID equals b.MedicalItemGUID
                       select
                       new MedicalItemsDataTableModel
                       {
                           MedicalItemGUID = a.MedicalItemGUID.ToString(),
                           FK = b.ItemQuantityThresholdGUID,
                           BrandName = a.BrandName.ToLower(),
                           Barcode = a.Barcode,
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

        [Route("EMT/MedicalDiscrepancys/Create/")]
        public ActionResult MedicalDiscrepancyCreate()
        {
            if (!CMS.HasAction(Permissions.MedicalDiscrepancy.Create, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/EMT/Views/MedicalDiscrepancys/MedicalDiscrepancy.cshtml", new MedicalDiscrepancyUpdateModel());
        }

        [Route("EMT/MedicalDiscrepancys/Update/{PK}")]
        public ActionResult MedicalDiscrepancyUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.MedicalDiscrepancy.Access, Apps.EMT))
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
            var model = (from a in DbEMT.dataMedicalDiscrepancy.WherePK(PK).AsEnumerable()
                         select new MedicalDiscrepancyUpdateModel
                         {
                             MedicalDiscrepancyGUID = a.MedicalDiscrepancyGUID,
                           
                             MedicalPharmacyGUID = a.MedicalPharmacyGUID,
                             ConfirmedBy=a.ConfirmedBy,
                             ConfirmedOn=a.ConfirmedOn.Value,
                             DiscrepancyDate=a.DiscrepancyDate,
                             DiscrepancyType=a.DiscrepancyType,
                             Active = a.Active,
                             dataMedicalDiscrepancyRowVersion = a.dataMedicalDiscrepancyRowVersion,
                             CreatedBy = Audit.ExecutedBy,
                             CreatedOn = Audit.ExecutionTime,
                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("MedicalDiscrepancy", "MedicalDiscrepancys", new { Area = "EMT" }));

            return View("~/Areas/EMT/Views/MedicalDiscrepancys/MedicalDiscrepancy.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalDiscrepancyCreate(MedicalDiscrepancyUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.MedicalDiscrepancy.Create, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid /*|| ActiveMedicalDiscrepancy(model)*/) return PartialView("~/Areas/EMT/Views/MedicalDiscrepancys/_MedicalDiscrepancyForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();
            dataMedicalDiscrepancy MedicalDiscrepancy = Mapper.Map(model, new dataMedicalDiscrepancy());
            MedicalDiscrepancy.MedicalDiscrepancyGUID = EntityPK;
            DbEMT.Create(MedicalDiscrepancy, Permissions.MedicalDiscrepancy.CreateGuid, ExecutionTime, DbCMS);


            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.MedicalDiscrepancyDetailsDataTable, ControllerContext, "MedicalDiscrepancyDetailsContainer"));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.MedicalDiscrepancy.Create, Apps.EMT, new UrlHelper(Request.RequestContext).Action("Create", "MedicalDiscrepancys", new { Area = "EMT" })), Container = "MedicalDiscrepancyFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.MedicalDiscrepancy.Update, Apps.EMT), Container = "MedicalDiscrepancyFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.MedicalDiscrepancy.Delete, Apps.EMT), Container = "MedicalDiscrepancyFormControls" });

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleCreateMessage(DbEMT.PrimaryKeyControl(MedicalDiscrepancy), DbEMT.RowVersionControls(new List<dataMedicalDiscrepancy> { MedicalDiscrepancy }), Partials, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalDiscrepancyUpdate(MedicalDiscrepancyUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.MedicalDiscrepancy.Update, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
          
            if (!ModelState.IsValid ) return PartialView("~/Areas/EMT/Views/MedicalDiscrepancys/_MedicalDiscrepancyForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            dataMedicalDiscrepancy MedicalDiscrepancy = Mapper.Map(model, new dataMedicalDiscrepancy());
            DbEMT.Update(MedicalDiscrepancy, Permissions.MedicalDiscrepancy.UpdateGuid, ExecutionTime, DbCMS);



            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleUpdateMessage(null, null, DbEMT.RowVersionControls(new List<dataMedicalDiscrepancy> { MedicalDiscrepancy })));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyMedicalDiscrepancy(model.MedicalDiscrepancyGUID);
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalDiscrepancyDelete(dataMedicalDiscrepancy model)
        {
            if (!CMS.HasAction(Permissions.MedicalDiscrepancy.Delete, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            List<dataMedicalDiscrepancy> DeletedMedicalDiscrepancy = DeleteMedicalDiscrepancys(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.MedicalDiscrepancy.Restore, Apps.EMT), Container = "MedicalDiscrepancyFormControls" });

            try
            {
                int CommitedRows = DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleDeleteMessage(CommitedRows, DeletedMedicalDiscrepancy.FirstOrDefault(), "MedicalDiscrepancyDetailsContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyMedicalDiscrepancy(model.MedicalDiscrepancyGUID);
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }

        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalDiscrepancyRestore(dataMedicalDiscrepancy model)
        {
            if (!CMS.HasAction(Permissions.MedicalDiscrepancy.Restore, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
                return Json(DbEMT.RecordExists());

            List<dataMedicalDiscrepancy> RestoredMedicalDiscrepancys = RestoreMedicalDiscrepancys(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.MedicalDiscrepancy.Create, Apps.EMT, new UrlHelper(Request.RequestContext).Action("MedicalDiscrepancyCreate", "Configuration", new { Area = "EMT" })), Container = "MedicalDiscrepancyFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.MedicalDiscrepancy.Update, Apps.EMT), Container = "MedicalDiscrepancyFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.MedicalDiscrepancy.Delete, Apps.EMT), Container = "MedicalDiscrepancyFormControls" });

            try
            {
                int CommitedRows = DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleRestoreMessage(CommitedRows, RestoredMedicalDiscrepancys, DbEMT.PrimaryKeyControl(RestoredMedicalDiscrepancys.FirstOrDefault()), Url.Action(DataTableNames.MedicalDiscrepancyDetailsDataTable, Portal.GetControllerName(ControllerContext)), "MedicalDiscrepancyDetailsContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyMedicalDiscrepancy(model.MedicalDiscrepancyGUID);
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult MedicalDiscrepancysDataTableDelete(List<dataMedicalDiscrepancy> models)
        {
            if (!CMS.HasAction(Permissions.MedicalDiscrepancy.Delete, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataMedicalDiscrepancy> DeletedMedicalDiscrepancys = DeleteMedicalDiscrepancys(models);

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.PartialDeleteMessage(DeletedMedicalDiscrepancys, models, DataTableNames.MedicalDiscrepancysDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult MedicalDiscrepancysDataTableRestore(List<dataMedicalDiscrepancy> models)
        {
            if (!CMS.HasAction(Permissions.MedicalDiscrepancy.Restore, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataMedicalDiscrepancy> RestoredMedicalDiscrepancys = RestoreMedicalDiscrepancys(models);

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.PartialRestoreMessage(RestoredMedicalDiscrepancys, models, DataTableNames.MedicalDiscrepancysDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        private List<dataMedicalDiscrepancy> DeleteMedicalDiscrepancys(List<dataMedicalDiscrepancy> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<dataMedicalDiscrepancy> DeletedMedicalDiscrepancys = new List<dataMedicalDiscrepancy>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbEMT.QueryBuilder(models, Permissions.MedicalDiscrepancy.DeleteGuid, SubmitTypes.Delete, baseQuery);

            var Records = DbEMT.Database.SqlQuery<dataMedicalDiscrepancy>(query).ToList();
            foreach (var record in Records)
            {
                DeletedMedicalDiscrepancys.Add(DbEMT.Delete(record, ExecutionTime, Permissions.MedicalDiscrepancy.DeleteGuid, DbCMS));
            }

            var MedicalDiscrepancyDetails = DeletedMedicalDiscrepancys.SelectMany(a => a.dataMedicalDiscrepancyDetail).Where(l => l.Active).ToList();
            foreach (var MedicalDiscrepancyDetail in MedicalDiscrepancyDetails)
            {
                DbEMT.Delete(MedicalDiscrepancyDetail, ExecutionTime, Permissions.MedicalDiscrepancy.DeleteGuid, DbCMS);

                AddItemQuantity(MedicalDiscrepancyDetail.MedicalDiscrepancyDetailGUID);
            }
            return DeletedMedicalDiscrepancys;
        }

        private List<dataMedicalDiscrepancy> RestoreMedicalDiscrepancys(List<dataMedicalDiscrepancy> models)
        {

            DateTime RestoringTime = DateTime.Now;

            List<dataMedicalDiscrepancy> RestoredMedicalDiscrepancys = new List<dataMedicalDiscrepancy>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbEMT.QueryBuilder(models, Permissions.MedicalDiscrepancy.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Records = DbEMT.Database.SqlQuery<dataMedicalDiscrepancy>(query).ToList();
            foreach (var record in Records)
            {
                    RestoredMedicalDiscrepancys.Add(DbEMT.Restore(record, Permissions.MedicalDiscrepancy.DeleteGuid, Permissions.MedicalDiscrepancy.RestoreGuid, RestoringTime, DbCMS));
                
            }

            var MedicalDiscrepancyDetails = RestoredMedicalDiscrepancys.SelectMany(x => x.dataMedicalDiscrepancyDetail.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var MedicalDiscrepancyDetail in MedicalDiscrepancyDetails)
            {
                DbEMT.Restore(MedicalDiscrepancyDetail, Permissions.MedicalDiscrepancy.DeleteGuid, Permissions.MedicalDiscrepancy.RestoreGuid, RestoringTime, DbCMS);

                RemoveItemQuantity(MedicalDiscrepancyDetail.MedicalDiscrepancyDetailGUID);
            }

            return RestoredMedicalDiscrepancys;
        }

        private JsonResult ConcurrencyMedicalDiscrepancy(Guid PK)
        {
            MedicalDiscrepancyUpdateModel dbModel = new MedicalDiscrepancyUpdateModel();

            var MedicalDiscrepancy = DbEMT.dataMedicalDiscrepancy.Where(x => x.MedicalDiscrepancyGUID == PK).FirstOrDefault();
            var dbMedicalDiscrepancy = DbEMT.Entry(MedicalDiscrepancy).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbMedicalDiscrepancy, dbModel);

            var MedicalDiscrepancyDetail = DbEMT.dataMedicalDiscrepancyDetail.Where(x => x.MedicalDiscrepancyGUID == PK).Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.dataMedicalDiscrepancy.DeletedOn)).FirstOrDefault();
            var dbMedicalDiscrepancyDetail = DbEMT.Entry(MedicalDiscrepancyDetail).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbMedicalDiscrepancyDetail, dbModel);

            if (MedicalDiscrepancy.dataMedicalDiscrepancyRowVersion.SequenceEqual(dbModel.dataMedicalDiscrepancyRowVersion))
            {
                return Json(DbEMT.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbEMT, dbModel, "MedicalDiscrepancyDetailsContainer"));
        }

        

        #endregion

        #region  Medical Beneficiary Item Out Details

        //[Route("EMT/MedicalDiscrepancyDetailsDataTable/{PK}")]
        public ActionResult MedicalDiscrepancyDetailsDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/EMT/Views/MedicalDiscrepancys/_MedicalDiscrepancyDetailsDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<MedicalDiscrepancyDetailsDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<MedicalDiscrepancyDetailsDataTableModel>(DataTable.Filters);
            }

            var Result = (from a in DbEMT.dataMedicalDiscrepancyDetail.AsNoTracking().AsExpandable().Where(x => x.MedicalDiscrepancyGUID == PK)
                          join b in DbEMT.codeMedicalItem on a.MedicalItemGUID equals b.MedicalItemGUID
                          select new MedicalDiscrepancyDetailsDataTableModel
                          {
                              MedicalDiscrepancyDetailGUID = a.MedicalDiscrepancyDetailGUID,
                              MedicalItemGUID = a.MedicalItemGUID.ToString(),
                              BrandName = b.BrandName,
                              MedicalDiscrepancyGUID = a.MedicalDiscrepancyGUID,
                              DiscrepancyQuantity = a.DiscrepancyQuantity,
                              OriginalQuantity=a.OriginalQuantity.Value,
                              RemainingQuaintity=a.RemainingItems.Value,
                              Comment=a.Comment,
                              Active = a.Active,
                              dataMedicalDiscrepancyDetailRowVersion = a.dataMedicalDiscrepancyDetailRowVersion,
                              
                          }).Where(Predicate);

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult MedicalDiscrepancyDetailCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.MedicalDiscrepancy.Create, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var MedicalPharmacy = (from a in DbEMT.dataMedicalDiscrepancy.Where(x => x.MedicalDiscrepancyGUID == FK)
                                   join b in DbEMT.codeMedicalPharmacy on a.MedicalPharmacyGUID equals b.MedicalPharmacyGUID
                                   select b).FirstOrDefault();
            return PartialView("~/Areas/EMT/Views/MedicalDiscrepancys/_MedicalDiscrepancyDetailsUpdateModal.cshtml",
                new MedicalDiscrepancyDetailsUpdateModel { MedicalDiscrepancyGUID = FK, MedicalPharmacyGUID = MedicalPharmacy.MedicalPharmacyGUID });
        }

        public ActionResult MedicalDiscrepancyDetailUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.MedicalDiscrepancy.Access, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var MedicalDiscrepancyDetail = (from a in DbEMT.dataMedicalDiscrepancy
                                                   join b in DbEMT.codeMedicalPharmacy on a.MedicalPharmacyGUID equals b.MedicalPharmacyGUID
                                                   join c in DbEMT.dataMedicalDiscrepancyDetail.Where(x => x.MedicalDiscrepancyDetailGUID == PK) on a.MedicalDiscrepancyGUID equals c.MedicalDiscrepancyGUID
                                                   select new
                                                    MedicalDiscrepancyDetailsUpdateModel
                                                   {
                                                       MedicalDiscrepancyDetailGUID = c.MedicalDiscrepancyDetailGUID,
                                                       MedicalDiscrepancyGUID = a.MedicalDiscrepancyGUID,
                                                       MedicalPharmacyGUID = b.MedicalPharmacyGUID,
                                                       MedicalItemGUID = c.MedicalItemGUID,
                                                       DiscrepancyQuantity = c.DiscrepancyQuantity,
                                                       RemainingItems=c.RemainingItems,
                                                       OriginalQuantity=c.OriginalQuantity,
                                                       ReferenceItemGUID=c.ReferenceItemGUID,
                                                       Comment=c.Comment,
                                                       Active = c.Active,
                                                       dataMedicalDiscrepancyDetailRowVersion = c.dataMedicalDiscrepancyDetailRowVersion
                                                   }
                                           ).FirstOrDefault();
            return PartialView("~/Areas/EMT/Views/MedicalDiscrepancys/_MedicalDiscrepancyDetailsUpdateModal.cshtml",
                MedicalDiscrepancyDetail);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalDiscrepancyDetailCreate(MedicalDiscrepancyDetailsUpdateModel model1)
        {
            if (!CMS.HasAction(Permissions.MedicalDiscrepancy.Create, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            if (!ModelState.IsValid || ActiveMedicalDiscrepancyDetail(model1)) return PartialView("~/Areas/EMT/Views/MedicalDiscrepancys/_MedicalDiscrepancyDetailsUpdateModal.cshtml", model1);
            dataMedicalDiscrepancyDetail model = Mapper.Map(model1, new dataMedicalDiscrepancyDetail());
            DateTime ExecutionTime = DateTime.Now;
            model.MedicalDiscrepancyDetailGUID = Guid.NewGuid();

            DbEMT.Create(model, Permissions.MedicalDiscrepancy.CreateGuid, ExecutionTime, DbCMS);
            //Jawad Update Later
            //update Item (Quantity Threshold and Input Detail) which we transfer the items <from> it.
            //dataMedicalItemTransferDetail itemImputDetail = DbEMT.dataMedicalItemTransferDetail.Where(x => x.MedicalItemTransferDetailGUID == model.MedicalItemTransferDetailGUID).FirstOrDefault();
            //itemImputDetail.RemainingItems = itemImputDetail.RemainingItems - model.QuantityByPackingUnit;

            //dataItemQuantityThreshold itemQuantityThresholdFrom = DbEMT.dataItemQuantityThreshold.Where(x => x.MedicalItemGUID == model.MedicalItemGUID && x.MedicalPharmacyGUID == itemImputDetail.dataMedicalItemTransfer.MedicalPharmacyGUID && x.Active).FirstOrDefault();
            //itemQuantityThresholdFrom.QuantityTotalRemainingItems = itemQuantityThresholdFrom.QuantityTotalRemainingItems + model.QuantityByPackingUnit;
            //itemQuantityThresholdFrom.codeMedicalItem.RemainingItemsQuantity = itemQuantityThresholdFrom.codeMedicalItem.RemainingItemsQuantity + model.QuantityByPackingUnit;


            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleUpdateMessage(DataTableNames.MedicalDiscrepancyDetailsDataTable, DbEMT.PrimaryKeyControl(model), DbEMT.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }



        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalDiscrepancyDetailUpdate(MedicalDiscrepancyDetailsUpdateModel model1)
        {
            if (!CMS.HasAction(Permissions.MedicalDiscrepancy.Update, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            if (!ModelState.IsValid /*|| ActiveMedicalDiscrepancyDetail(model1)*/) return PartialView("~/Areas/EMT/Views/MedicalDiscrepancys/_MedicalDiscrepancyDetailsUpdateModal.cshtml", model1);
            dataMedicalDiscrepancyDetail model = Mapper.Map(model1, new dataMedicalDiscrepancyDetail());

            AddItemQuantity(model.MedicalDiscrepancyDetailGUID);
            DateTime ExecutionTime = DateTime.Now;
            DbEMT.Update(model, Permissions.MedicalDiscrepancy.UpdateGuid, ExecutionTime, DbCMS);
            RemoveItemQuantity(model.MedicalDiscrepancyDetailGUID);
            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();

                return Json(DbEMT.SingleUpdateMessage(DataTableNames.MedicalDiscrepancyDetailsDataTable,
                    DbEMT.PrimaryKeyControl(model),
                    DbEMT.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyMedicalDiscrepancyDetail(model.MedicalDiscrepancyDetailGUID);
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalDiscrepancyDetailDelete(dataMedicalDiscrepancyDetail model)
        {
            if (!CMS.HasAction(Permissions.MedicalDiscrepancy.Delete, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            List<dataMedicalDiscrepancyDetail> DeletedLanguages = DeleteMedicalDiscrepancyDetails(new List<dataMedicalDiscrepancyDetail> { model });

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleDeleteMessage(DeletedLanguages, DataTableNames.MedicalDiscrepancyDetailsDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyMedicalDiscrepancyDetail(model.MedicalDiscrepancyDetailGUID);
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }

        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalDiscrepancyDetailRestore(MedicalDiscrepancyDetailsUpdateModel model1)
        {
            if (!CMS.HasAction(Permissions.MedicalDiscrepancy.Restore, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            dataMedicalDiscrepancyDetail model = Mapper.Map(model1, new dataMedicalDiscrepancyDetail());
            if (ActiveMedicalDiscrepancyDetail(model1))
            {
                return Json(DbEMT.RecordExists());
            }

            List<dataMedicalDiscrepancyDetail> RestoredLanguages = RestoreMedicalDiscrepancyDetails(Portal.SingleToList(model));

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleRestoreMessage(RestoredLanguages, DataTableNames.MedicalDiscrepancyDetailsDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyMedicalDiscrepancyDetail(model.MedicalDiscrepancyDetailGUID);
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult MedicalDiscrepancyDetailsDataTableDelete(List<dataMedicalDiscrepancyDetail> models)
        {
            if (!CMS.HasAction(Permissions.MedicalDiscrepancy.Delete, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            Guid MedicalDiscrepancyDetailGUID = models.FirstOrDefault().MedicalDiscrepancyDetailGUID;
            var MedicalDiscrepancy = DbEMT.dataMedicalDiscrepancy.Where(x => x.dataMedicalDiscrepancyDetail.FirstOrDefault().MedicalDiscrepancyDetailGUID == MedicalDiscrepancyDetailGUID).FirstOrDefault();
            List<dataMedicalDiscrepancyDetail> DeletedLanguages = DeleteMedicalDiscrepancyDetails(models);

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.MedicalDiscrepancyDetailsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }

        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult MedicalDiscrepancyDetailsDataTableRestore(List<dataMedicalDiscrepancyDetail> models)
        {
            if (!CMS.HasAction(Permissions.MedicalDiscrepancy.Restore, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataMedicalDiscrepancyDetail> RestoredLanguages = RestoreMedicalDiscrepancyDetails(models);

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.MedicalDiscrepancyDetailsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        private List<dataMedicalDiscrepancyDetail> DeleteMedicalDiscrepancyDetails(List<dataMedicalDiscrepancyDetail> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<dataMedicalDiscrepancyDetail> DeletedMedicalDiscrepancyDetails = new List<dataMedicalDiscrepancyDetail>();

            string query = DbEMT.QueryBuilder(models, Permissions.MedicalDiscrepancy.DeleteGuid, SubmitTypes.Delete, "");

            var MedicalDiscrepancyDetails = DbEMT.Database.SqlQuery<dataMedicalDiscrepancyDetail>(query).ToList();

            //var MedicalDiscrepancy = DbEMT.dataMedicalDiscrepancy.Where(x => x.MedicalDiscrepancyGUID == models.FirstOrDefault().MedicalDiscrepancyGUID).FirstOrDefault();
            foreach (var MedicalDiscrepancyDetail in MedicalDiscrepancyDetails)
            {
                DeletedMedicalDiscrepancyDetails.Add(DbEMT.Delete(MedicalDiscrepancyDetail, ExecutionTime, Permissions.MedicalDiscrepancy.DeleteGuid, DbCMS));

                AddItemQuantity(MedicalDiscrepancyDetail.MedicalDiscrepancyDetailGUID);
            }
            return DeletedMedicalDiscrepancyDetails;
        }

        private List<dataMedicalDiscrepancyDetail> RestoreMedicalDiscrepancyDetails(List<dataMedicalDiscrepancyDetail> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<dataMedicalDiscrepancyDetail> RestoredLanguages = new List<dataMedicalDiscrepancyDetail>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbEMT.QueryBuilder(models, Permissions.MedicalDiscrepancy.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var MedicalDiscrepancyDetails = DbEMT.Database.SqlQuery<dataMedicalDiscrepancyDetail>(query).ToList();

            //var MedicalDiscrepancy = DbEMT.dataMedicalDiscrepancy.Where(x => x.MedicalDiscrepancyGUID == models.FirstOrDefault().MedicalDiscrepancyGUID).FirstOrDefault();
            foreach (var MedicalDiscrepancyDetail in MedicalDiscrepancyDetails)
            {
                MedicalDiscrepancyDetailsUpdateModel model = Mapper.Map(MedicalDiscrepancyDetail, new MedicalDiscrepancyDetailsUpdateModel());
                if (!ActiveMedicalDiscrepancyDetail(model))
                {
                    RestoredLanguages.Add(DbEMT.Restore(MedicalDiscrepancyDetail, Permissions.MedicalDiscrepancy.DeleteGuid, Permissions.MedicalDiscrepancy.RestoreGuid, RestoringTime, DbCMS));
                    RemoveItemQuantity(MedicalDiscrepancyDetail.MedicalDiscrepancyDetailGUID);
                }
            }


            return RestoredLanguages;
        }

        private JsonResult ConcrrencyMedicalDiscrepancyDetail(Guid PK)
        {
            dataMedicalDiscrepancyDetail dbModel = new dataMedicalDiscrepancyDetail();

            var Language = DbEMT.dataMedicalDiscrepancyDetail.Where(l => l.MedicalDiscrepancyDetailGUID == PK).FirstOrDefault();
            var dbLanguage = DbEMT.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.dataMedicalDiscrepancyDetailRowVersion.SequenceEqual(dbModel.dataMedicalDiscrepancyDetailRowVersion))
            {
                return Json(DbEMT.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbEMT, dbModel, "MedicalDiscrepancyDetailsContainer"));
        }

        private bool ActiveMedicalDiscrepancyDetail(MedicalDiscrepancyDetailsUpdateModel model)
        {
            int BeneficiaryItemOutDetail = DbEMT.dataMedicalDiscrepancyDetail
                                  .Where(x => x.MedicalDiscrepancyDetailGUID != model.MedicalDiscrepancyDetailGUID &&
                                              x.MedicalDiscrepancyGUID == model.MedicalDiscrepancyGUID &&
                                              x.MedicalItemGUID == model.MedicalItemGUID &&
                                              x.Active).Count();
            if (BeneficiaryItemOutDetail > 0)
            {
                ModelState.AddModelError("MedicalDiscrepancyDetailGUID", "Item already exists"); //From resource ?????? Amer  
            }
            //Jawad Update Later
            //var MedicalItemTransferDetail = DbEMT.dataMedicalItemTransferDetail.Where(x => x.MedicalItemTransferDetailGUID == model.MedicalItemTransferDetailGUID).FirstOrDefault();
            //if (model.QuantityByPackingUnit > MedicalItemTransferDetail.RemainingItems)
            //{
            //    ModelState.AddModelError("QuantityByPackingUnit", "Quantity of Batch Number Remaining not Enough"); //From resource ?????? Amer  
            //    BeneficiaryItemOutDetail = BeneficiaryItemOutDetail + 1;
            //}
            return (BeneficiaryItemOutDetail > 0);
        }

        public void AddItemQuantity(Guid MedicalItemTransferDetailGUID)
        {
            //UPDATE Medical Beneficiary Item Out Detail Quantity 
            var MedicalDiscrepancyDetail = DbEMT.dataMedicalDiscrepancyDetail.Where(x => x.MedicalDiscrepancyDetailGUID == MedicalItemTransferDetailGUID).FirstOrDefault();
            //ADD the QuantityByPackingUnit from  RemainingItemsQuantity
            dataItemQuantityThreshold itemQuantityThresholdRemove = DbEMT.dataItemQuantityThreshold.Where(x => x.MedicalItemGUID == MedicalDiscrepancyDetail.MedicalItemGUID && x.MedicalPharmacyGUID == MedicalDiscrepancyDetail.dataMedicalDiscrepancy.MedicalPharmacyGUID && x.Active).FirstOrDefault();
            itemQuantityThresholdRemove.QuantityTotalRemainingItems = itemQuantityThresholdRemove.QuantityTotalRemainingItems - MedicalDiscrepancyDetail.DiscrepancyQuantity;
            itemQuantityThresholdRemove.codeMedicalItem.RemainingItemsQuantity = itemQuantityThresholdRemove.codeMedicalItem.RemainingItemsQuantity - MedicalDiscrepancyDetail.DiscrepancyQuantity;
            var Pharmacy = DbEMT.codeMedicalPharmacy.Where(x => x.MedicalPharmacyGUID == MedicalDiscrepancyDetail.dataMedicalDiscrepancy.MedicalPharmacyGUID).FirstOrDefault();
            if (Pharmacy != null)
            {
                bool IsMainWarehouse = Pharmacy.MainWarehouse;
                var spResult = DbEMT.spMedicalItem(LAN).ToList();
                //in case  warehouse add the +-Discrepancy value to  Item Input Detail
                if (IsMainWarehouse)
                {
                    dataMedicalItemInputDetail medicalItemTransferDetail = DbEMT.dataMedicalItemInputDetail.Where(x => x.MedicalItemInputDetailGUID == MedicalDiscrepancyDetail.ReferenceItemGUID).FirstOrDefault();
                    medicalItemTransferDetail.RemainingItems = medicalItemTransferDetail.RemainingItems - (int)MedicalDiscrepancyDetail.DiscrepancyQuantity;
                }
                else
                {
                    dataMedicalItemTransferDetail medicalItemTransferDetail = DbEMT.dataMedicalItemTransferDetail.Where(x => x.MedicalItemTransferDetailGUID == MedicalDiscrepancyDetail.ReferenceItemGUID).FirstOrDefault();
                    medicalItemTransferDetail.RemainingItems = medicalItemTransferDetail.RemainingItems - MedicalDiscrepancyDetail.DiscrepancyQuantity;
                }
            }


        }

        public void RemoveItemQuantity(Guid MedicalItemTransferDetailGUID)
        {
            //UPDATE Medical Beneficiary Item Out Detail Quantity 
            var MedicalDiscrepancyDetail = DbEMT.dataMedicalDiscrepancyDetail.Where(x => x.MedicalDiscrepancyDetailGUID == MedicalItemTransferDetailGUID).FirstOrDefault();
            var MedicalDiscrepancy = DbEMT.dataMedicalDiscrepancy.Where(x => x.MedicalDiscrepancyGUID == MedicalDiscrepancyDetail.MedicalDiscrepancyGUID).FirstOrDefault();

            //ADD the QuantityByPackingUnit from  RemainingItemsQuantity
            dataItemQuantityThreshold itemQuantityThresholdRemove = DbEMT.dataItemQuantityThreshold.Where(x => x.MedicalItemGUID == MedicalDiscrepancyDetail.MedicalItemGUID && x.MedicalPharmacyGUID == MedicalDiscrepancy.MedicalPharmacyGUID && x.Active).FirstOrDefault();
            itemQuantityThresholdRemove.QuantityTotalRemainingItems = itemQuantityThresholdRemove.QuantityTotalRemainingItems + MedicalDiscrepancyDetail.DiscrepancyQuantity;
            itemQuantityThresholdRemove.codeMedicalItem.RemainingItemsQuantity = itemQuantityThresholdRemove.codeMedicalItem.RemainingItemsQuantity + MedicalDiscrepancyDetail.DiscrepancyQuantity;

            var Pharmacy = DbEMT.codeMedicalPharmacy.Where(x => x.MedicalPharmacyGUID == MedicalDiscrepancy.MedicalPharmacyGUID).FirstOrDefault();

            if (Pharmacy != null)
            {
                bool IsMainWarehouse = Pharmacy.MainWarehouse;
                var spResult = DbEMT.spMedicalItem(LAN).ToList();
                //in case  warehouse add the +-Discrepancy value to  Item Input Detail
                if (IsMainWarehouse)
                {
                    dataMedicalItemInputDetail medicalItemTransferDetail = DbEMT.dataMedicalItemInputDetail.Where(x => x.MedicalItemInputDetailGUID == MedicalDiscrepancyDetail.ReferenceItemGUID).FirstOrDefault();
                    medicalItemTransferDetail.RemainingItems = medicalItemTransferDetail.RemainingItems + (int)MedicalDiscrepancyDetail.DiscrepancyQuantity;
                }
                else
                {
                    dataMedicalItemTransferDetail medicalItemTransferDetail = DbEMT.dataMedicalItemTransferDetail.Where(x => x.MedicalItemTransferDetailGUID == MedicalDiscrepancyDetail.ReferenceItemGUID).FirstOrDefault();
                    medicalItemTransferDetail.RemainingItems = medicalItemTransferDetail.RemainingItems + MedicalDiscrepancyDetail.DiscrepancyQuantity;
                }
            }

        }

        [Route("EMT/MedicalDiscrepancy/Upload/{PK}")]
        public ActionResult Upload(Guid PK)
        {
            return PartialView("~/Areas/EMT/Views/MedicalDiscrepancys/_FileUpload.cshtml", PK);
        }




        #endregion
    }
}