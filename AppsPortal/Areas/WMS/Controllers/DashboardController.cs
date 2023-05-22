using AppsPortal.BaseControllers;
using AppsPortal.Library;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WMS_DAL.ViewModels;

namespace AppsPortal.Areas.WMS.Controllers
{
    public class DashboardController : WMSBaseController
    {
        public ActionResult Home()
        {
            if (!CMS.HasAction(Permissions.WarehouseDashboard.Access, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            return View();
        }
        // GET: WMS/Dashboard
        public ActionResult Index()
        {
            if (!CMS.HasAction(Permissions.WarehouseDashboard.Access, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            DashboardItemVM model = new DashboardItemVM();
            model.TotalDelayItems = DbWMS.dataItemInputDetail.Where(x => x.LastFlowTypeGUID == WarehouseRequestFlowType.PendingConfirmed
                                                   && x.Active).Select(x => x.ItemInputDetailGUID).Count();
            DateTime exectionTime = DateTime.Now;
            model.TotalPendingConfirmationdItems = DbWMS.dataItemOutputDetail
                 .Where(x => x.ExpectedReturenedDate < exectionTime &&
                             x.dataItemInputDetail.LastFlowTypeGUID == WarehouseRequestFlowType.Confirmed)
                 .Select(x => x.ItemOutputDetailGUID).Count();

            //model.TotalDelayItems = 12;
            model.TotalDepreciatedItems = 0;
            model.TotalExpiredItems = 0;

            Guid ICTGuid = Guid.Parse("9317FBDA-E360-45CC-A064-11E6A21C1E17");

            var itemInputs = DbWMS.dataItemInputDetail.Where(x =>
                x.codeItemModelWarehouse.codeWarehouseItemModel.codeWarehouseItem.codeWarehouseItemClassification.WarehouseTypeGUID ==
                ICTGuid
                && x.Active
                && x.TransferToCountryGUID == null
                ).ToList();

            //var _profile = DbWMS.userProfiles.Where(x => x.UserProfileGUID == UserProfileGUID).FirstOrDefault();
            //var _locations = DbWMS.codeWarehouseLocation.Where(x => x.Active && x.OrgnanizationInstanceGUID == _profile.OrganizationInstanceGUID).Select(x => x.WarehouseLocationGUID).ToList();
            var currentWarehouseGUID = DbCMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).Select(x => x.WarehouseGUID)
                     .FirstOrDefault();
            var _currwarehouse = DbCMS.codeWarehouse.Where(x => (x.WarehouseGUID == currentWarehouseGUID && x.ParentGUID == null) || (x.ParentGUID == currentWarehouseGUID)).FirstOrDefault();

            var _itemsmodelsGUids = DbWMS.codeItemModelWarehouse.Where(x => x.WarehouseGUID == _currwarehouse.WarehouseGUID).Select(x => x.codeWarehouseItemModel.codeWarehouseItem.WarehouseItemGUID).Distinct().ToList();

            var items = DbWMS.codeWarehouseItemLanguage.Where(x => x.codeWarehouseItem.codeWarehouseItemClassification.WarehouseTypeGUID ==
                                                                 ICTGuid
                                                                 && _itemsmodelsGUids.Contains((Guid)x.WarehouseItemGUID)).ToList();
            model.items = items.Select(
                x => new ItemsVM()
                {
                    ItemName = x.WarehouseItemDescription,
                    WarehouseItemGUID = x.WarehouseItemGUID,
                    models = x.codeWarehouseItem.codeWarehouseItemModel.Where(f => f.Active).Select(d => new ModelVM()
                    {
                        ModelName = d.codeWarehouseItemModelLanguage.Select(f => f.ModelDescription).FirstOrDefault(),
                        WarehouseItemModelGUID = d.WarehouseItemModelGUID,
                        TotalAvaiable = itemInputs.Where(f => f.codeItemModelWarehouse.WarehouseItemModelGUID == d.WarehouseItemModelGUID
                                                            && f.IsAvaliable == true).Count(),
                        TotalItems = itemInputs.Where(f => f.codeItemModelWarehouse.WarehouseItemModelGUID == d.WarehouseItemModelGUID
                                                              ).Count()

                    }).ToList()
                }).ToList();

            return View(model);
        }


        public JsonResult InitDashboardData()
        {
            DashboardItemVM model = new DashboardItemVM();

            DateTime exectionTime = DateTime.Now;


            Guid ICTGuid = Guid.Parse("9317FBDA-E360-45CC-A064-11E6A21C1E17");



            var CurrentWarehouseGUID = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).Select(x => x.WarehouseGUID)
                .FirstOrDefault();
            var warehouseAuthGUIDs = DbWMS.codeWarehouse
                .Where(x => x.WarehouseGUID == CurrentWarehouseGUID || x.ParentGUID == CurrentWarehouseGUID)
                .Select(x => x.WarehouseGUID).ToList();


            var itemInputs = DbWMS.dataItemInputDetail.Where(x =>
                x.codeItemModelWarehouse.codeWarehouseItemModel.codeWarehouseItem.codeWarehouseItemClassification.WarehouseTypeGUID ==
                ICTGuid
                && x.Active
                && warehouseAuthGUIDs.Contains((Guid)x.WarehouseOwnerGUID)
                ).ToList();
            var currentWarehouseGUID = DbCMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).Select(x => x.WarehouseGUID)
                   .FirstOrDefault();
            var _currwarehouse = DbCMS.codeWarehouse.Where(x => (x.WarehouseGUID == currentWarehouseGUID && x.ParentGUID == null) || (x.ParentGUID == currentWarehouseGUID)).FirstOrDefault();

            var _itemsmodels = DbWMS.codeItemModelWarehouse.Where(x => x.WarehouseGUID == _currwarehouse.WarehouseGUID).Distinct().ToList();
            var _itemModelGUIDs = _itemsmodels.Select(x => x.WarehouseItemModelGUID).Distinct().ToList();
            var _itemsGUids = _itemsmodels.Select(x => x.codeWarehouseItemModel.codeWarehouseItem.WarehouseItemGUID).ToList();




            var items = DbWMS.codeWarehouseItemLanguage.Where(x => x.codeWarehouseItem.codeWarehouseItemClassification.WarehouseTypeGUID ==
                                                                   ICTGuid
                                                                   && x.codeWarehouseItem.IsDeterminanted != false
                                                                   && _itemsGUids.Contains((Guid)x.WarehouseItemGUID)).ToList();
            model.items = items.Select(
                x => new ItemsVM()
                {
                    ItemName = x.WarehouseItemDescription,
                    WarehouseItemGUID = x.WarehouseItemGUID,
                    TotalAvaiable = itemInputs.Where(f => f.codeItemModelWarehouse.codeWarehouseItemModel.WarehouseItemGUID == x.WarehouseItemGUID
                                                          && f.IsAvaliable == true
                                                           ).Count(),
                    TotalItems = itemInputs.Where(f => f.codeItemModelWarehouse.codeWarehouseItemModel.WarehouseItemGUID == x.WarehouseItemGUID
                    ).Count(),
                    models = x.codeWarehouseItem.codeWarehouseItemModel.Where(f => f.Active && _itemModelGUIDs.Contains((Guid)f.WarehouseItemModelGUID)).Select(d => new ModelVM()
                    {
                        ModelName = d.codeWarehouseItemModelLanguage.Select(f => f.ModelDescription).FirstOrDefault(),
                        WarehouseItemModelGUID = d.WarehouseItemModelGUID,
                        BrandName = d.codeBrand.codeBrandLanguage.Where(f => f.LanguageID == LAN).Select(f => f.BrandDescription).FirstOrDefault(),
                        BrandGUID = d.BrandGUID,
                        TotalAvaiable = itemInputs.Where(f => f.codeItemModelWarehouse.WarehouseItemModelGUID == d.WarehouseItemModelGUID
                                                              && f.IsAvaliable == true).Count(),
                        TotalItems = itemInputs.Where(f => f.codeItemModelWarehouse.WarehouseItemModelGUID == d.WarehouseItemModelGUID
                        ).Count()

                    }).ToList()
                }).Where(x => x.models.Select(f => f.TotalItems).Any()).ToList();

            Guid itemStatus = Guid.Parse("0C44822F-A898-476D-B291-CAF1B055AAC6");
            var brands = DbWMS.codeBrandLanguage.Where(x => x.Active && x.LanguageID == LAN).Select(x => new { BrandName = x.BrandDescription, BrandGUID = x.BrandGUID }).ToList();
            var warehouseItems = DbWMS.codeWarehouseItemLanguage.Where(x => x.Active && x.LanguageID == LAN).Select(x => new { WarehouseItemDescription = x.WarehouseItemDescription, WarehouseItemGUID = x.WarehouseItemGUID }).ToList();
            var warehouseItemClassification = DbWMS.codeWarehouseItemClassificationLanguage.Where(x => x.Active && x.LanguageID == LAN).Select(x => new { WarehouseItemClassificationDescription = x.WarehouseItemClassificationDescription, WarehouseItemClassificationGUID = x.WarehouseItemClassificationGUID }).ToList();
            var ItemServiceStatus = DbWMS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN && x.codeTablesValues.TableGUID == LookupTables.ItemServiceStatus).Select(x => new { ValueDescription = x.ValueDescription, ValueGUID = x.ValueGUID }).ToList();



            var warehouses = DbWMS.codeWarehouseLanguage.Where(x => x.Active && x.LanguageID == LAN
            && (x.WarehouseGUID == _currwarehouse.WarehouseGUID || x.codeWarehouse.ParentGUID == _currwarehouse.WarehouseGUID || x.WarehouseGUID == _currwarehouse.ParentGUID)

            ).Select(x => new { WarehouseDescription = x.WarehouseDescription, WarehouseGUID = x.WarehouseGUID }).ToList();
            var _profile = DbWMS.userProfiles.Where(x => x.UserProfileGUID == UserProfileGUID).FirstOrDefault();

            var WarehouseLocations = DbWMS.codeWarehouseLocationLanguage.Where(x => x.Active && x.LanguageID == LAN && x.codeWarehouseLocation.OrgnanizationInstanceGUID == _profile.OrganizationInstanceGUID).Select(x => new { WarehouseLocationDescription = x.WarehouseLocationDescription, WarehouseLocationGUID = x.WarehouseLocationGUID }).ToList();
            var ItemMovementStatus = DbWMS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN && x.codeTablesValues.TableGUID == itemStatus).Select(x => new { ValueDescription = x.ValueDescription, ValueGUID = x.ValueGUID }).ToList();
            //List<ItemsVM> myitems = model.items;
            return Json(new { ItemMovementStatus = ItemMovementStatus, warehouseItemClassification = warehouseItemClassification, ItemServiceStatus = ItemServiceStatus, WarehouseLocations = WarehouseLocations, warehouses = warehouses, warehouseItems = warehouseItems, itemSummary = model, items = items.Select(x => new { x.WarehouseItemDescription, x.WarehouseItemGUID }).ToList(), brands = brands }, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetSearchModels(DashboardVMFilter myModel)
        {
            DashboardItemVM model = new DashboardItemVM();



            DateTime exectionTime = DateTime.Now;


            Guid ICTGuid = Guid.Parse("9317FBDA-E360-45CC-A064-11E6A21C1E17");
            var CurrentWarehouseGUID = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).Select(x => x.WarehouseGUID)
                 .FirstOrDefault();
            var warehouseAuthGUIDs = DbWMS.codeWarehouse
                .Where(x => x.WarehouseGUID == CurrentWarehouseGUID || x.ParentGUID == CurrentWarehouseGUID)
                .Select(x => x.WarehouseGUID).ToList();

            var itemInputs = DbWMS.dataItemInputDetail.Where(x =>
                x.codeItemModelWarehouse.codeWarehouseItemModel.codeWarehouseItem.codeWarehouseItemClassification.WarehouseTypeGUID == ICTGuid
                && warehouseAuthGUIDs.Contains((Guid)x.WarehouseOwnerGUID)
                && x.Active
                ).ToList();


            if (myModel.WarehouseLocationGUIDs != null && myModel.WarehouseLocationGUIDs.Count > 0)
            {
                itemInputs = itemInputs.Where(x => x.LastWarehouseLocationGUID != null).ToList();

                itemInputs = itemInputs.Where(x => myModel.WarehouseLocationGUIDs.Contains((Guid)x.LastWarehouseLocationGUID)).ToList();
            }
            //var items = DbWMS.codeWarehouseItemLanguage.Where(x =>
            //                ((myModel.itemGuids != null && myModel.itemGuids.Count > 0)? myModel.itemGuids.Contains((Guid)x.WarehouseItemGUID):true) &&
            //                ((myModel.brandGuids != null && myModel.brandGuids.Count > 0) ? myModel.brandGuids.Intersect((x.codeWarehouseItem.codeWarehouseItemModel.Select(f=>f.BrandGUID).AsQueryable())) : true) &&
            //                x.codeWarehouseItem.codeWarehouseItemClassification.WarehouseTypeGUID ==
            //                                                       ICTGuid).ToList();
            var currentWarehouseGUID = DbCMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).Select(x => x.WarehouseGUID)
                    .FirstOrDefault();
            var _currwarehouse = DbCMS.codeWarehouse.Where(x => (x.WarehouseGUID == currentWarehouseGUID && x.ParentGUID == null) || (x.ParentGUID == currentWarehouseGUID)).FirstOrDefault();

            var _itemsmodels = DbWMS.codeItemModelWarehouse.Where(x => x.WarehouseGUID == _currwarehouse.WarehouseGUID).Distinct().ToList();
            var _itemModelGUIDs = _itemsmodels.Select(x => x.WarehouseItemModelGUID).Distinct().ToList();
            var _itemwarehouGUID = _itemsmodels.Select(x => x.ItemModelWarehouseGUID).Distinct().ToList();
            var _itemsGUids = _itemsmodels.Select(x => x.codeWarehouseItemModel.codeWarehouseItem.WarehouseItemGUID).ToList();

            var items = DbWMS.dataItemInputDetail.Where(x => x.Active && x.codeItemModelWarehouse.codeWarehouseItemModel.codeWarehouseItem.IsDeterminanted != false
            && _itemwarehouGUID.Contains((Guid)x.ItemModelWarehouseGUID)).AsQueryable();



            var models = items.AsEnumerable().Select
                (f =>
                f.codeItemModelWarehouse.codeWarehouseItemModel.codeWarehouseItem.codeWarehouseItemLanguage.Where(a => a.LanguageID == LAN).FirstOrDefault()

                ).Distinct().ToList();



            if (myModel.brandGuids != null && myModel.brandGuids.Count > 0)
            {
                items = items.Where(x => myModel.brandGuids.Contains((Guid)x.codeItemModelWarehouse.codeWarehouseItemModel.BrandGUID));
                models = items.Where(x => myModel.brandGuids.Contains((Guid)x.codeItemModelWarehouse.codeWarehouseItemModel.BrandGUID)).Select
                (f =>
                f.codeItemModelWarehouse.codeWarehouseItemModel.codeWarehouseItem.codeWarehouseItemLanguage.Where(a => a.LanguageID == LAN).FirstOrDefault()

                ).Distinct().ToList();
            }
            if (myModel.itemGuids != null && myModel.itemGuids.Count > 0)
            {
                items = items.Where(x => myModel.itemGuids.Contains(
                    (Guid)x.codeItemModelWarehouse.codeWarehouseItemModel.WarehouseItemGUID));
                models = items.Where(x => myModel.brandGuids.Contains((Guid)x.codeItemModelWarehouse.codeWarehouseItemModel.BrandGUID)).Select
                         (f =>
                         f.codeItemModelWarehouse.codeWarehouseItemModel.codeWarehouseItem.codeWarehouseItemLanguage.Where(a => a.LanguageID == LAN).FirstOrDefault()

         ).Distinct().ToList();
            }
            if (myModel.WarehouseLocationGUIDs != null && myModel.WarehouseLocationGUIDs.Count > 0)
            {
                items = items.Where(x => myModel.WarehouseLocationGUIDs.Contains((Guid)x.LastWarehouseLocationGUID));
            }
            if (myModel.itemClassificationGuids != null && myModel.itemClassificationGuids.Count > 0)
            {
                items = items.Where(x => myModel.itemClassificationGuids.Contains((Guid)x.codeItemModelWarehouse.codeWarehouseItemModel.codeWarehouseItem.WarehouseItemClassificationGUID));
            }
            if (myModel.ItemServiceGUIDs != null && myModel.ItemServiceGUIDs.Count > 0)
            {
                items = items.Where(x => myModel.ItemServiceGUIDs.Contains((Guid)x.ItemServiceStatusGUID));
            }
            if (myModel.DeliveryStatusGUIDs != null && myModel.DeliveryStatusGUIDs.Count > 0)
            {
                items = items.Where(x => myModel.DeliveryStatusGUIDs.Contains((Guid)x.LastFlowTypeGUID));
            }
            var selectedItemWarehouseGUID = items.Select(x => x.codeItemModelWarehouse.codeWarehouseItemModel.WarehouseItemGUID).Distinct().ToList();
            var selectedItemModelWarehouseGUID = items.Select(x => x.ItemModelWarehouseGUID).Distinct().ToList();
            var _iteminput = items.Select(x => x.ItemInputDetailGUID).Distinct().ToList();
            model.items = models.Where(x => selectedItemWarehouseGUID.Contains((Guid)x.WarehouseItemGUID)).ToList().Select(
                x => new ItemsVM()
                {
                    ItemName = x.codeWarehouseItem.codeWarehouseItemLanguage.FirstOrDefault(f => f.Active && f.LanguageID == LAN).WarehouseItemDescription,

                    WarehouseItemGUID = x.WarehouseItemGUID,
                    TotalAvaiable = itemInputs.Where(f => f.codeItemModelWarehouse.codeWarehouseItemModel.WarehouseItemGUID == x.WarehouseItemGUID
                                                          && f.IsAvaliable == true).Count(),
                    TotalItems = itemInputs.Where(f => f.codeItemModelWarehouse.codeWarehouseItemModel.WarehouseItemGUID == x.WarehouseItemGUID
                    ).Count(),
                    models = x.codeWarehouseItem.codeWarehouseItemModel.Where(f => f.Active &&
                    selectedItemModelWarehouseGUID.Contains((Guid)f.codeItemModelWarehouse.FirstOrDefault().ItemModelWarehouseGUID)).Select(d => new ModelVM()
                    {
                        ModelName = d.codeWarehouseItemModelLanguage.Select(f => f.ModelDescription).FirstOrDefault(),
                        WarehouseItemModelGUID = d.WarehouseItemModelGUID,
                        BrandName = d.codeBrand.codeBrandLanguage.Where(f => f.LanguageID == LAN).Select(f => f.BrandDescription).FirstOrDefault(),
                        BrandGUID = d.BrandGUID,
                        TotalAvaiable = itemInputs.Where(f => f.codeItemModelWarehouse.WarehouseItemModelGUID == d.WarehouseItemModelGUID
                                                              && f.IsAvaliable == true).Count(),
                        TotalItems = itemInputs.Where(f => f.codeItemModelWarehouse.WarehouseItemModelGUID == d.WarehouseItemModelGUID
                       ).Count()

                    }).ToList()
                }).Where(x => x.models.Select(f => f.TotalItems).Any()).ToList();


            return Json(new { itemSummary = model }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSearchModels_test(DashboardVMFilter myModel)
        {
            DashboardItemVM model = new DashboardItemVM();



            DateTime exectionTime = DateTime.Now;


            Guid ICTGuid = Guid.Parse("9317FBDA-E360-45CC-A064-11E6A21C1E17");
            var CurrentWarehouseGUID = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).Select(x => x.WarehouseGUID)
                 .FirstOrDefault();
            var warehouseAuthGUIDs = DbWMS.codeWarehouse
                .Where(x => x.WarehouseGUID == CurrentWarehouseGUID || x.ParentGUID == CurrentWarehouseGUID)
                .Select(x => x.WarehouseGUID).ToList();

            var itemInputs = DbWMS.dataItemInputDetail.Where(x =>
                x.codeItemModelWarehouse.codeWarehouseItemModel.codeWarehouseItem.codeWarehouseItemClassification.WarehouseTypeGUID == ICTGuid
                && warehouseAuthGUIDs.Contains((Guid)x.WarehouseOwnerGUID)
                && x.Active
                ).ToList();


            if (myModel.WarehouseLocationGUIDs != null && myModel.WarehouseLocationGUIDs.Count > 0)
            {
                itemInputs = itemInputs.Where(x => x.LastWarehouseLocationGUID != null).ToList();

                itemInputs = itemInputs.Where(x => myModel.WarehouseLocationGUIDs.Contains((Guid)x.LastWarehouseLocationGUID)).ToList();
            }
            //var items = DbWMS.codeWarehouseItemLanguage.Where(x =>
            //                ((myModel.itemGuids != null && myModel.itemGuids.Count > 0)? myModel.itemGuids.Contains((Guid)x.WarehouseItemGUID):true) &&
            //                ((myModel.brandGuids != null && myModel.brandGuids.Count > 0) ? myModel.brandGuids.Intersect((x.codeWarehouseItem.codeWarehouseItemModel.Select(f=>f.BrandGUID).AsQueryable())) : true) &&
            //                x.codeWarehouseItem.codeWarehouseItemClassification.WarehouseTypeGUID ==
            //                                                       ICTGuid).ToList();
            var currentWarehouseGUID = DbCMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).Select(x => x.WarehouseGUID)
                    .FirstOrDefault();
            var _currwarehouse = DbCMS.codeWarehouse.Where(x => (x.WarehouseGUID == currentWarehouseGUID && x.ParentGUID == null) || (x.ParentGUID == currentWarehouseGUID)).FirstOrDefault();

            var _itemsmodels = DbWMS.codeItemModelWarehouse.Where(x => x.WarehouseGUID == _currwarehouse.WarehouseGUID).Distinct().ToList();
            var _itemModelGUIDs = _itemsmodels.Select(x => x.WarehouseItemModelGUID).Distinct().ToList();
            var _itemwarehouGUID = _itemsmodels.Select(x => x.ItemModelWarehouseGUID).Distinct().ToList();
            var _itemsGUids = _itemsmodels.Select(x => x.codeWarehouseItemModel.codeWarehouseItem.WarehouseItemGUID).ToList();

            var items = DbWMS.dataItemInputDetail.Where(x => x.Active && x.codeItemModelWarehouse.codeWarehouseItemModel.codeWarehouseItem.IsDeterminanted != false
            && _itemwarehouGUID.Contains((Guid)x.ItemModelWarehouseGUID)).AsQueryable();



            var models = items.AsEnumerable().Select
                (f =>
                f.codeItemModelWarehouse.codeWarehouseItemModel.codeWarehouseItem.codeWarehouseItemLanguage.Where(a => a.LanguageID == LAN).FirstOrDefault()

                ).Distinct().ToList();



            if (myModel.brandGuids != null && myModel.brandGuids.Count > 0)
            {
                items = items.Where(x => myModel.brandGuids.Contains((Guid)x.codeItemModelWarehouse.codeWarehouseItemModel.BrandGUID));
            }
            if (myModel.itemGuids != null && myModel.itemGuids.Count > 0)
            {
                items = items.Where(x => myModel.itemGuids.Contains((Guid)x.codeItemModelWarehouse.codeWarehouseItemModel.WarehouseItemGUID));
            }
            if (myModel.WarehouseLocationGUIDs != null && myModel.WarehouseLocationGUIDs.Count > 0)
            {
                items = items.Where(x => myModel.WarehouseLocationGUIDs.Contains((Guid)x.LastWarehouseLocationGUID));
            }
            if (myModel.itemClassificationGuids != null && myModel.itemClassificationGuids.Count > 0)
            {
                items = items.Where(x => myModel.itemClassificationGuids.Contains((Guid)x.codeItemModelWarehouse.codeWarehouseItemModel.codeWarehouseItem.WarehouseItemClassificationGUID));
            }
            if (myModel.ItemServiceGUIDs != null && myModel.ItemServiceGUIDs.Count > 0)
            {
                items = items.Where(x => myModel.ItemServiceGUIDs.Contains((Guid)x.ItemServiceStatusGUID));
            }
            if (myModel.DeliveryStatusGUIDs != null && myModel.DeliveryStatusGUIDs.Count > 0)
            {
                items = items.Where(x => myModel.DeliveryStatusGUIDs.Contains((Guid)x.LastFlowTypeGUID));
            }

            model.items = models.Select(
                x => new ItemsVM()
                {
                    ItemName = x.codeWarehouseItem.codeWarehouseItemLanguage.FirstOrDefault(f => f.Active && f.LanguageID == LAN).WarehouseItemDescription,

                    WarehouseItemGUID = x.WarehouseItemGUID,
                    TotalAvaiable = itemInputs.Where(f => f.codeItemModelWarehouse.codeWarehouseItemModel.WarehouseItemGUID == x.WarehouseItemGUID
                                                          && f.IsAvaliable == true).Count(),
                    TotalItems = itemInputs.Where(f => f.codeItemModelWarehouse.codeWarehouseItemModel.WarehouseItemGUID == x.WarehouseItemGUID
                    ).Count(),
                    models = x.codeWarehouseItem.codeWarehouseItemModel.Where(f => f.Active).Select(d => new ModelVM()
                    {
                        ModelName = d.codeWarehouseItemModelLanguage.Select(f => f.ModelDescription).FirstOrDefault(),
                        WarehouseItemModelGUID = d.WarehouseItemModelGUID,
                        BrandName = d.codeBrand.codeBrandLanguage.Where(f => f.LanguageID == LAN).Select(f => f.BrandDescription).FirstOrDefault(),
                        BrandGUID = d.BrandGUID,
                        TotalAvaiable = itemInputs.Where(f => f.codeItemModelWarehouse.WarehouseItemModelGUID == d.WarehouseItemModelGUID
                                                              && f.IsAvaliable == true).Count(),
                        TotalItems = itemInputs.Where(f => f.codeItemModelWarehouse.WarehouseItemModelGUID == d.WarehouseItemModelGUID
                       ).Count()

                    }).ToList()
                }).Where(x => x.models.Select(f => f.TotalItems).Any()).ToList();


            return Json(new { itemSummary = model }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetItemModelInformation(Guid PK)
        {
            //if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Access, Apps.CMS))
            //{
            //  return Json(DbWMS.PermissionError());
            //}
            Guid ICTWarehouse = Guid.Parse("9317FBDA-E360-45CC-A064-11E6A21C1E17");
            Guid inServiceGuid = Guid.Parse("675DE853-151B-4C2F-93F4-DA1435EEE761");
            Guid DamagedGuid = Guid.Parse("675DE853-151B-4C2F-93F4-DA1434EEE766");
            Guid Lost = Guid.Parse("675DE853-151B-4C2F-93F4-DA1434EEE765");
            Guid newItem = Guid.Parse("A40EC252-622E-4FF1-9EF4-E328C7F3CEC9");
            Guid itemUsed = Guid.Parse("A40EC252-622E-4FF1-9EF4-E323C7A3CEC9");
            Guid GS = Guid.Parse("675DE853-151B-4C2F-93F4-DA1434EEE767");
            var _profile = DbWMS.userProfiles.Where(x => x.UserProfileGUID == UserProfileGUID).FirstOrDefault();
            var _locations = DbWMS.codeWarehouseLocation.Where(x => x.Active && x.OrgnanizationInstanceGUID == _profile.OrganizationInstanceGUID).Select(x => x.WarehouseLocationGUID).ToList();


            var warehouses = DbWMS.codeWarehouseLanguage.Where(x => x.codeWarehouse.WarehouseTypeGUID == ICTWarehouse && _locations.Contains((Guid)x.codeWarehouse.WarehouseLocationGUID)
                                                                    && x.LanguageID == LAN).ToList();
            List<ItemWarehouseInformationDetail> model = new List<ItemWarehouseInformationDetail>();

            var modelTransfers = DbWMS.dataItemTransfer.Where(x => x.Active && x.dataItemInputDetail.codeItemModelWarehouse.WarehouseItemModelGUID == PK
                                                                 && x.IsLastTransfer == true).ToList();
            foreach (var item in warehouses)
            {
                ItemWarehouseInformationDetail myDetail = new ItemWarehouseInformationDetail
                {
                    Warehousename = item.WarehouseDescription,
                    totalItems = modelTransfers.Where(x => x.DestionationGUID == item.WarehouseGUID).Select(x => x.ItemInputDetailGUID).Count(),
                    totalAvailable = modelTransfers.Where(x => x.DestionationGUID == item.WarehouseGUID && x.dataItemInputDetail.IsAvaliable == true).Select(x => x.ItemInputDetailGUID).Count(),
                    totalCustody = modelTransfers.Where(x => x.DestionationGUID == item.WarehouseGUID && x.dataItemInputDetail.IsAvaliable == false
                                                          && x.dataItemInputDetail.ItemStatusGUID == inServiceGuid).Select(x => x.ItemInputDetailGUID).Count(),
                    totalDamaged = modelTransfers.Where(x => x.DestionationGUID == item.WarehouseGUID && x.dataItemInputDetail.IsAvaliable == false
                                                                                                     && x.dataItemInputDetail.ItemStatusGUID == DamagedGuid).Select(x => x.ItemInputDetailGUID).Count(),
                    totalLost = modelTransfers.Where(x => x.DestionationGUID == item.WarehouseGUID && x.dataItemInputDetail.IsAvaliable == false
                                                                                                     && x.dataItemInputDetail.ItemStatusGUID == Lost).Select(x => x.ItemInputDetailGUID).Count(),
                    totalGS = modelTransfers.Where(x => x.DestionationGUID == item.WarehouseGUID && x.dataItemInputDetail.IsAvaliable == false
                                                                                                  && x.dataItemInputDetail.ItemStatusGUID == GS).Select(x => x.ItemInputDetailGUID).Count(),
                    totalAvailableNew = modelTransfers.Where(x => x.DestionationGUID == item.WarehouseGUID && x.dataItemInputDetail.IsAvaliable == true && x.dataItemInputDetail.ItemConditionGUID == newItem).Select(x => x.ItemInputDetailGUID).Count(),
                    totalAvailablUsed = modelTransfers.Where(x => x.DestionationGUID == item.WarehouseGUID && x.dataItemInputDetail.IsAvaliable == true && x.dataItemInputDetail.ItemConditionGUID == itemUsed).Select(x => x.ItemInputDetailGUID).Count(),


                };
                model.Add(myDetail);
            }

            ViewBag.ItemPhoto = PK + ".jpg";
            ViewBag.ModelName = DbWMS.codeWarehouseItemModelLanguage.Where(x => x.WarehouseItemModelGUID == PK)
                .Select(f => f.ModelDescription).FirstOrDefault();
            ViewBag.WarehouseItemModelGUID = PK;
            ViewBag.totalItems = modelTransfers.Where(x => x.Active && x.dataItemInputDetail.codeItemModelWarehouse.WarehouseItemModelGUID == PK
                                                                    && x.IsLastTransfer == true)
                .Select(x => x.ItemInputDetailGUID).Count();
            return PartialView("~/Areas/WMS/Views/Dashboard/_ItemModelInformation.cshtml", model);
        }

        #region Export 

        public ActionResult ExportModelItemInformation(Guid id)
        {
            var CurrentWarehouseGUID = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).Select(x => x.WarehouseGUID)
    .FirstOrDefault();
            var warehouseAuthGUIDs = DbWMS.codeWarehouse
                .Where(x => x.WarehouseGUID == CurrentWarehouseGUID || x.ParentGUID == CurrentWarehouseGUID)
                .Select(x => x.WarehouseGUID).ToList();
            var result = (
             from a in DbWMS.dataItemInputDetail.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.dataItemInput.DeletedOn) && x.codeItemModelWarehouse.codeWarehouseItemModel.WarehouseItemGUID == id
             && warehouseAuthGUIDs.Contains((Guid)x.WarehouseOwnerGUID))
             join b in DbWMS.dataItemInput.Where(x => (x.Active)) on a.ItemInputGUID equals b.ItemInputGUID
             join c in DbWMS.dataItemTransfer.Where(x => x.IsLastTransfer == true && x.Active) on a.ItemInputDetailGUID equals c.ItemInputDetailGUID into LJ1
             from R1 in LJ1.DefaultIfEmpty()
             join d in DbWMS.codeItemModelWarehouse.Where(x => x.Active) on a.ItemModelWarehouseGUID equals d.ItemModelWarehouseGUID into LJ2
             from R2 in LJ2.DefaultIfEmpty()
             join e in DbWMS.codeWarehouseItemModel.Where(x => x.Active) on R2.WarehouseItemModelGUID equals e.WarehouseItemModelGUID
             join f in DbWMS.codeWarehouseItemModelLanguage.Where(x => x.LanguageID == LAN && x.Active) on e.WarehouseItemModelGUID equals f.WarehouseItemModelGUID into LJ3
             from R3 in LJ3.DefaultIfEmpty()
             join g in DbWMS.codeWarehouseItem.Where(x => x.Active) on e.WarehouseItemGUID equals g.WarehouseItemGUID
             join h in DbWMS.codeWarehouseItemLanguage.Where(x => x.LanguageID == LAN && x.Active) on g.WarehouseItemGUID equals h.WarehouseItemGUID into LJ4
             from R4 in LJ4.DefaultIfEmpty()
             join gg in DbWMS.codeWarehouseItemClassification.Where(x => x.Active) on g.WarehouseItemClassificationGUID equals gg.WarehouseItemClassificationGUID
             join hh in DbWMS.codeWarehouseItemClassificationLanguage.Where(x => x.LanguageID == LAN && x.Active) on gg.WarehouseItemClassificationGUID equals hh.WarehouseItemClassificationGUID into LJ44
             from R44 in LJ44.DefaultIfEmpty()
             join k in DbWMS.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.LastCustdianNameGUID equals k.UserGUID into LJ5
             from R5 in LJ5.DefaultIfEmpty()
             join L in DbWMS.codeWarehouse.Where(x => x.Active) on a.LastCustdianNameGUID equals L.WarehouseGUID into LJ6
             from R6 in LJ6.DefaultIfEmpty()
             join M in DbWMS.codeWarehouseLanguage.Where(x => x.Active && x.LanguageID == LAN) on R6.WarehouseGUID equals M.WarehouseGUID into LJ7
             from R7 in LJ7.DefaultIfEmpty()
             join N in DbWMS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN && x.codeTablesValues.TableGUID == LookupTables.ItemStatus) on a.ItemStatusGUID equals N.ValueGUID into LJ8
             from R8 in LJ8.DefaultIfEmpty()
             join o in DbWMS.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.CreatedByGUID equals o.UserGUID into LJ9
             from R9 in LJ9.DefaultIfEmpty()
             join p in DbWMS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.LastFlowTypeGUID equals p.ValueGUID into LJ10
             from R10 in LJ10.DefaultIfEmpty()
             join s in DbWMS.codeLocationsLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.LastLocationGUID equals s.LocationGUID into LJ11
             from R11 in LJ11.DefaultIfEmpty()
             join y in DbWMS.codeWarehouseRequesterTypeLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.LastCustdianNameGUID equals y.WarehouseRequesterTypeGUID into LJ17
             from R17 in LJ17.DefaultIfEmpty()
             join yy in DbWMS.codeWarehouseVehicleLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.LastCustdianNameGUID equals yy.WarehouseVehicleGUID into LJ18
             from R18 in LJ18.DefaultIfEmpty()
             join zz in DbWMS.codeWarehouseLocationLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.LastWarehouseLocationGUID equals zz.WarehouseLocationGUID into LJ19
             from R19 in LJ19.DefaultIfEmpty()
             select new WarehouseModelEntryMovementDataTableModel
             {
                 ItemInputDetailGUID = a.ItemInputDetailGUID,
                 Active = a.Active,
                 ItemModelWarehouseGUID = a.ItemModelWarehouseGUID.ToString(),
                 ModelDescription = R3.ModelDescription,
                 ItemDescription = R4.WarehouseItemDescription,
                 WarehouseItemClassificationDescription = R44.WarehouseItemClassificationDescription,
                 WarehouseOwnerGUID = R1.codeWarehouse.WarehouseGUID.ToString(),
                 BarcodeNumber = a.dataItemInputDeterminant.Where(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.Barcode).Select(x => x.DeterminantValue).FirstOrDefault(),
                 SerialNumber = a.dataItemInputDeterminant.Where(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.SerialNumber).Select(x => x.DeterminantValue).FirstOrDefault(),
                 IME1 = a.dataItemInputDeterminant.Where(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.IME1).Select(x => x.DeterminantValue).FirstOrDefault(),
                 GSM = a.dataItemInputDeterminant.Where(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.GSM).Select(x => x.DeterminantValue).FirstOrDefault(),
                 WarehouseOwner = R1.codeWarehouse.codeWarehouseLanguage.FirstOrDefault(x => x.Active && x.LanguageID == LAN).WarehouseDescription,
                 Governorate = R11.LocationDescription,
                 Custodian = a.IsAvaliable == true
                     ? "Stock " + R1.codeWarehouse.codeWarehouseLanguage
                           .FirstOrDefault(x => x.Active && x.LanguageID == LAN).WarehouseDescription
                     : (a.LastCustdianGUID == WarehouseRequestSourceTypes.Warehouse
                         ? R7.WarehouseDescription
                         : (a.LastCustdianGUID == WarehouseRequestSourceTypes.Staff
                             ? R5.FirstName + " " + R5.Surname
                             : (a.LastCustdianGUID == WarehouseRequestSourceTypes.Vehicle ?
                                 R17.WarehouseRequesterTypeDescription :
                                 a.LastCustdianGUID == WarehouseRequestSourceTypes.OtherRequester ? R18
                                     .VehicleDescription : null))),
                 ModelStatus = R8.ValueDescription,
                 CreatedDate = a.CreatedDate,
                 WarehouseLocationDescription = R19.WarehouseLocationDescription,

                 CreatedBy = R9.FirstName + " " + R9.Surname,
                 Comments = a.Comments,
                 DeliveryStatus = R10.ValueDescription,
                 dataItemInputDetailRowVersion = a.dataItemInputDetailRowVersion
             }).ToList();
            if (result.Count > 0)
            {
                string sourceFile = Server.MapPath("~/Areas/WMS/Templates/ItemModelTrackCustdoy.xlsx");
                string DisFolder =
                    Server.MapPath("~/Areas/WMS/temp/ItemModelTrackCustdoy" + DateTime.Now.ToBinary() + ".xlsx");
                System.IO.File.Copy(sourceFile, DisFolder);
                using (ExcelPackage package = new ExcelPackage(new FileInfo(DisFolder)))
                {
                    var cx = package.Workbook.Worksheets.ToList();
                    ExcelWorksheet workSheet = package.Workbook.Worksheets[1];

                    DataTable dt = new DataTable();
                    dt.Columns.Add("Item Classification", typeof(string));
                    dt.Columns.Add("Item Sub Classification", typeof(string));
                    dt.Columns.Add("Model Name", typeof(string));
                    dt.Columns.Add("Warehouse Owner", typeof(string));
                    dt.Columns.Add("Custodian", typeof(string));
                    dt.Columns.Add("Barcode", typeof(string));
                    dt.Columns.Add("Serial Number", typeof(string));
                    dt.Columns.Add("IME1", typeof(string));
                    dt.Columns.Add("GSM", typeof(string));
                    dt.Columns.Add("Location", typeof(string));
                    dt.Columns.Add("Service Status", typeof(string));
                    dt.Columns.Add("DeliveryStatus", typeof(string));
                    dt.Columns.Add("Comments", typeof(string));

                    foreach (var item in result)
                    {

                        DataRow dr;
                        dr = dt.NewRow();
                        dr[0] = item.WarehouseItemClassificationDescription;
                        dr[1] = item.ItemDescription;
                        dr[2] = item.ModelDescription;
                        dr[3] = item.WarehouseOwner;
                        dr[4] = item.Custodian;
                        dr[5] = item.BarcodeNumber;
                        dr[6] = item.SerialNumber;
                        dr[7] = item.IME1;
                        dr[8] = item.GSM;
                        dr[9] = item.WarehouseLocationDescription;
                        dr[10] = item.ServiceItemStatus;
                        dr[11] = item.DeliveryStatus;
                        dr[12] = item.Comments;
                        dt.Rows.Add(dr);
                    }
                    workSheet.Cells["B5"].LoadFromDataTable(dt, true);
                    package.Save();
                }

                byte[] fileBytes = System.IO.File.ReadAllBytes(DisFolder);
                string fileName = DateTime.Now.ToString("yyMMdd") + "_Stock Overview  " + ".xlsx";

                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }

            bool success = false;
            return Json(new { success = success }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult ExportAllModelItemInformation()
        {
            var CurrentWarehouseGUID = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).Select(x => x.WarehouseGUID)
                .FirstOrDefault();
            var warehouseAuthGUIDs = DbWMS.codeWarehouse
                .Where(x => x.WarehouseGUID == CurrentWarehouseGUID || x.ParentGUID == CurrentWarehouseGUID)
                .Select(x => x.WarehouseGUID).ToList();
            var result = DbWMS.v_EntryMovementDataTable.Where(x => warehouseAuthGUIDs.Contains((Guid)x.WarehouseOwnerGUID)).ToList();
            if (result.Count > 0)
            {
                string sourceFile = Server.MapPath("~/Areas/WMS/Templates/ItemModelTrackCustdoy.xlsx");
                string DisFolder =
                    Server.MapPath("~/Areas/WMS/temp/ItemModelTrackCustdoy" + DateTime.Now.ToBinary() + ".xlsx");
                System.IO.File.Copy(sourceFile, DisFolder);
                using (ExcelPackage package = new ExcelPackage(new FileInfo(DisFolder)))
                {
                    var cx = package.Workbook.Worksheets.ToList();
                    ExcelWorksheet workSheet = package.Workbook.Worksheets[1];

                    DataTable dt = new DataTable();
                    dt.Columns.Add("Item Classification", typeof(string));
                    dt.Columns.Add("Item Name", typeof(string));
                    dt.Columns.Add("Model Name", typeof(string));
                    dt.Columns.Add("Warehouse Owner", typeof(string));
                    dt.Columns.Add("Custodian", typeof(string));
                    dt.Columns.Add("Barcode", typeof(string));
                    dt.Columns.Add("Serial Number", typeof(string));
                    dt.Columns.Add("IME1", typeof(string));
                    dt.Columns.Add("GSM", typeof(string));
                    dt.Columns.Add("Location", typeof(string));
                    dt.Columns.Add("Service Status", typeof(string));
                    dt.Columns.Add("DeliveryStatus", typeof(string));
                    dt.Columns.Add("Comments", typeof(string));

                    foreach (var item in result)
                    {

                        DataRow dr;
                        dr = dt.NewRow();
                        dr[0] = item.WarehouseItemClassificationDescription;
                        dr[1] = item.WarehouseItemDescription;
                        dr[2] = item.ModelDescription;
                        dr[3] = item.WarehosueOwnerName;
                        dr[4] = item.LastCustodianName;
                        dr[5] = item.BarcodeNumber;
                        dr[6] = item.SerialNumber;
                        dr[7] = item.IMEI1;
                        dr[8] = item.GSM;
                        dr[9] = item.LastLocation;
                        dr[10] = item.ServiceItemStatus;
                        dr[11] = item.LastFlow;
                        dr[12] = item.Comments;
                        dt.Rows.Add(dr);
                    }
                    workSheet.Cells["B5"].LoadFromDataTable(dt, true);
                    package.Save();
                }

                byte[] fileBytes = System.IO.File.ReadAllBytes(DisFolder);

                string fileName = DateTime.Now.ToString("yyMMdd") + "_List of item custodian distribution " + ".xlsx";
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }

            string success = "No Data Available for this items";
            return Json(new { success = success }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult ExportItemInformation(Guid id)
        {
            var CurrentWarehouseGUID = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).Select(x => x.WarehouseGUID)
           .FirstOrDefault();
            var warehouseAuthGUIDs = DbWMS.codeWarehouse
                .Where(x => x.WarehouseGUID == CurrentWarehouseGUID || x.ParentGUID == CurrentWarehouseGUID)
                .Select(x => x.WarehouseGUID).ToList();
            var result = DbWMS.v_EntryMovementDataTable.Where(x => x.WarehouseItemModelGUID == id && warehouseAuthGUIDs.Contains((Guid)x.WarehouseOwnerGUID)).ToList();
            if (result.Count > 0)
            {
                string sourceFile = Server.MapPath("~/Areas/WMS/Templates/ItemModelTrackCustdoy.xlsx");
                string DisFolder =
                    Server.MapPath("~/Areas/WMS/temp/ItemModelTrackCustdoy" + DateTime.Now.ToBinary() + ".xlsx");
                System.IO.File.Copy(sourceFile, DisFolder);
                using (ExcelPackage package = new ExcelPackage(new FileInfo(DisFolder)))
                {
                    var cx = package.Workbook.Worksheets.ToList();
                    ExcelWorksheet workSheet = package.Workbook.Worksheets[1];

                    DataTable dt = new DataTable();
                    dt.Columns.Add("Item Classification", typeof(string));
                    dt.Columns.Add("Item Name", typeof(string));
                    dt.Columns.Add("Model Name", typeof(string));
                    dt.Columns.Add("Warehouse Owner", typeof(string));
                    dt.Columns.Add("Custodian", typeof(string));
                    dt.Columns.Add("Barcode", typeof(string));
                    dt.Columns.Add("Serial Number", typeof(string));
                    dt.Columns.Add("IME1", typeof(string));
                    dt.Columns.Add("GSM", typeof(string));
                    dt.Columns.Add("Location", typeof(string));
                    dt.Columns.Add("Service Status", typeof(string));
                    dt.Columns.Add("DeliveryStatus", typeof(string));
                    dt.Columns.Add("Comments", typeof(string));

                    foreach (var item in result)
                    {

                        DataRow dr;
                        dr = dt.NewRow();
                        dr[0] = item.WarehouseItemClassificationDescription;
                        dr[1] = item.WarehouseItemDescription;
                        dr[2] = item.ModelDescription;
                        dr[3] = item.WarehosueOwnerName;
                        dr[4] = item.LastCustodianName;
                        dr[5] = item.BarcodeNumber;
                        dr[6] = item.SerialNumber;
                        dr[7] = item.IMEI1;
                        dr[8] = item.GSM;
                        dr[9] = item.LastLocation;
                        dr[10] = item.ServiceItemStatus;
                        dr[11] = item.LastFlow;
                        dr[12] = item.Comments;
                        dt.Rows.Add(dr);
                    }
                    workSheet.Cells["B5"].LoadFromDataTable(dt, true);
                    package.Save();
                }

                byte[] fileBytes = System.IO.File.ReadAllBytes(DisFolder);
                string fileName = DateTime.Now.ToString("yyMMdd") + "List of item custodian distribution " + ".xlsx";
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }

            string success = "No Data Available for this items";
            return Json(new { success = success }, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region Item Modals Information
        public ActionResult ExportPendingConfirmationModlesIntoExcel()
        {
            var CurrentWarehouseGUID = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).Select(x => x.WarehouseGUID)
      .FirstOrDefault();
            var warehouseAuthGUIDs = DbWMS.codeWarehouse
                .Where(x => x.WarehouseGUID == CurrentWarehouseGUID || x.ParentGUID == CurrentWarehouseGUID)
                .Select(x => x.WarehouseGUID).ToList();


            var result = (
              from a in DbWMS.dataItemInputDetail.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.dataItemInput.DeletedOn) && x.LastFlowTypeGUID == WarehouseRequestFlowType.PendingConfirmed
              && warehouseAuthGUIDs.Contains((Guid)x.WarehouseOwnerGUID))
              join b in DbWMS.dataItemInput.Where(x => (x.Active)) on a.ItemInputGUID equals b.ItemInputGUID
              join c in DbWMS.dataItemTransfer.Where(x => x.IsLastTransfer == true && x.Active) on a.ItemInputDetailGUID equals c.ItemInputDetailGUID into LJ1
              from R1 in LJ1.DefaultIfEmpty()
              join d in DbWMS.codeItemModelWarehouse.Where(x => x.Active) on a.ItemModelWarehouseGUID equals d.ItemModelWarehouseGUID into LJ2
              from R2 in LJ2.DefaultIfEmpty()
              join e in DbWMS.codeWarehouseItemModel.Where(x => x.Active) on R2.WarehouseItemModelGUID equals e.WarehouseItemModelGUID
              join f in DbWMS.codeWarehouseItemModelLanguage.Where(x => x.LanguageID == LAN && x.Active) on e.WarehouseItemModelGUID equals f.WarehouseItemModelGUID into LJ3
              from R3 in LJ3.DefaultIfEmpty()
              join g in DbWMS.codeWarehouseItem.Where(x => x.Active) on e.WarehouseItemGUID equals g.WarehouseItemGUID
              join h in DbWMS.codeWarehouseItemLanguage.Where(x => x.LanguageID == LAN && x.Active) on g.WarehouseItemGUID equals h.WarehouseItemGUID into LJ4
              from R4 in LJ4.DefaultIfEmpty()
              join k in DbWMS.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.LastCustdianNameGUID equals k.UserGUID into LJ5
              from R5 in LJ5.DefaultIfEmpty()
              join L in DbWMS.codeWarehouse.Where(x => x.Active) on a.LastCustdianNameGUID equals L.WarehouseGUID into LJ6
              from R6 in LJ6.DefaultIfEmpty()
              join M in DbWMS.codeWarehouseLanguage.Where(x => x.Active && x.LanguageID == LAN) on R6.WarehouseGUID equals M.WarehouseGUID into LJ7
              from R7 in LJ7.DefaultIfEmpty()
              join N in DbWMS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN && x.codeTablesValues.TableGUID == LookupTables.ItemStatus) on a.ItemStatusGUID equals N.ValueGUID into LJ8
              from R8 in LJ8.DefaultIfEmpty()
              join o in DbWMS.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.CreatedByGUID equals o.UserGUID into LJ9
              from R9 in LJ9.DefaultIfEmpty()
              join p in DbWMS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.LastFlowTypeGUID equals p.ValueGUID into LJ10
              from R10 in LJ10.DefaultIfEmpty()
              join s in DbWMS.codeLocationsLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.LastLocationGUID equals s.LocationGUID into LJ11
              from R11 in LJ11.DefaultIfEmpty()
              join y in DbWMS.codeWarehouseRequesterTypeLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.LastCustdianNameGUID equals y.WarehouseRequesterTypeGUID into LJ17
              from R17 in LJ17.DefaultIfEmpty()
              join yy in DbWMS.codeWarehouseVehicleLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.LastCustdianNameGUID equals yy.WarehouseVehicleGUID into LJ18
              from R18 in LJ18.DefaultIfEmpty()

              select new WarehouseModelEntryMovementDataTableModel
              {
                  ItemInputDetailGUID = a.ItemInputDetailGUID,
                  Active = a.Active,
                  ItemModelWarehouseGUID = a.ItemModelWarehouseGUID.ToString(),
                  ModelDescription = R3.ModelDescription,
                  ItemDescription = R4.WarehouseItemDescription,
                  WarehouseOwnerGUID = R1.codeWarehouse.WarehouseGUID.ToString(),
                  BarcodeNumber = a.dataItemInputDeterminant.Where(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.Barcode).Select(x => x.DeterminantValue).FirstOrDefault(),
                  SerialNumber = a.dataItemInputDeterminant.Where(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.SerialNumber).Select(x => x.DeterminantValue).FirstOrDefault(),
                  IME1 = a.dataItemInputDeterminant.Where(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.IME1).Select(x => x.DeterminantValue).FirstOrDefault(),
                  GSM = a.dataItemInputDeterminant.Where(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.GSM).Select(x => x.DeterminantValue).FirstOrDefault(),
                  WarehouseOwner = R1.codeWarehouse.codeWarehouseLanguage.FirstOrDefault(x => x.Active && x.LanguageID == LAN).WarehouseDescription,
                  Governorate = R11.LocationDescription,
                  Custodian = a.IsAvaliable == true
                      ? "Stock " + R1.codeWarehouse.codeWarehouseLanguage
                            .FirstOrDefault(x => x.Active && x.LanguageID == LAN).WarehouseDescription
                      : (a.LastCustdianGUID == WarehouseRequestSourceTypes.Warehouse
                          ? R7.WarehouseDescription
                          : (a.LastCustdianGUID == WarehouseRequestSourceTypes.Staff
                              ? R5.FirstName + " " + R5.Surname
                              : (a.LastCustdianGUID == WarehouseRequestSourceTypes.Vehicle ?
                                  R17.WarehouseRequesterTypeDescription :
                                  a.LastCustdianGUID == WarehouseRequestSourceTypes.OtherRequester ? R18
                                      .VehicleDescription : null))),
                  ModelStatus = R8.ValueDescription,
                  CreatedDate = a.CreatedDate,

                  CreatedBy = R9.FirstName + " " + R9.Surname,
                  Comments = a.Comments,
                  DeliveryStatus = R10.ValueDescription,
                  dataItemInputDetailRowVersion = a.dataItemInputDetailRowVersion
              }).ToList();
            if (result.Count > 0)
            {
                string sourceFile = Server.MapPath("~/Areas/WMS/Templates/ItemModelTrackCustdoy.xlsx");
                string DisFolder =
                    Server.MapPath("~/Areas/WMS/temp/ItemModelTrackCustdoy" + DateTime.Now.ToBinary() + ".xlsx");
                System.IO.File.Copy(sourceFile, DisFolder);
                using (ExcelPackage package = new ExcelPackage(new FileInfo(DisFolder)))
                {
                    var cx = package.Workbook.Worksheets.ToList();
                    ExcelWorksheet workSheet = package.Workbook.Worksheets[1];

                    DataTable dt = new DataTable();
                    dt.Columns.Add("Item Classification", typeof(string));
                    dt.Columns.Add("Item Sub Classification", typeof(string));
                    dt.Columns.Add("Model Name", typeof(string));
                    dt.Columns.Add("Warehouse Owner", typeof(string));
                    dt.Columns.Add("Custodian", typeof(string));
                    dt.Columns.Add("Barcode", typeof(string));
                    dt.Columns.Add("Serial Number", typeof(string));
                    dt.Columns.Add("IME1", typeof(string));
                    dt.Columns.Add("GSM", typeof(string));
                    dt.Columns.Add("Location", typeof(string));
                    dt.Columns.Add("Service Status", typeof(string));
                    dt.Columns.Add("DeliveryStatus", typeof(string));
                    dt.Columns.Add("Comments", typeof(string));

                    foreach (var item in result)
                    {

                        DataRow dr;
                        dr = dt.NewRow();
                        dr[0] = item.WarehouseItemClassificationDescription;
                        dr[1] = item.ItemDescription;
                        dr[2] = item.ModelDescription;
                        dr[3] = item.WarehouseOwner;
                        dr[4] = item.Custodian;
                        dr[5] = item.BarcodeNumber;
                        dr[6] = item.SerialNumber;
                        dr[7] = item.IME1;
                        dr[8] = item.GSM;
                        dr[9] = item.Governorate;
                        dr[10] = item.ServiceItemStatus;
                        dr[11] = item.DeliveryStatus;
                        dr[12] = item.Comments;
                        dt.Rows.Add(dr);
                    }
                    workSheet.Cells["B5"].LoadFromDataTable(dt, true);
                    package.Save();
                }

                byte[] fileBytes = System.IO.File.ReadAllBytes(DisFolder);
                string fileName = DateTime.Now.ToString("yyMMdd") + "List of item custodian distribution " + ".xlsx";
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }

            bool success = false;
            return Json(new { success = success }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportDelayConfirmationModlesIntoExcel()
        {
            var CurrentWarehouseGUID = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).Select(x => x.WarehouseGUID)
                .FirstOrDefault();
            var warehouseAuthGUIDs = DbWMS.codeWarehouse
                .Where(x => x.WarehouseGUID == CurrentWarehouseGUID || x.ParentGUID == CurrentWarehouseGUID)
                .Select(x => x.WarehouseGUID).ToList();

            DateTime mydate = DateTime.Now;
            var result = (
              from a in DbWMS.dataItemInputDetail.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.dataItemInput.DeletedOn) && x.LastFlowTypeGUID == WarehouseRequestFlowType.Confirmed
              && warehouseAuthGUIDs.Contains((Guid)x.WarehouseOwnerGUID)
                                                             )
              join aa in DbWMS.dataItemOutputDetail.Where(x => x.Active && x.ExpectedReturenedDate < mydate) on a.ItemInputDetailGUID equals aa.ItemInputDetailGUID
              join b in DbWMS.dataItemInput.Where(x => (x.Active)) on a.ItemInputGUID equals b.ItemInputGUID
              join c in DbWMS.dataItemTransfer.Where(x => x.IsLastTransfer == true && x.Active) on a.ItemInputDetailGUID equals c.ItemInputDetailGUID into LJ1
              from R1 in LJ1.DefaultIfEmpty()
              join d in DbWMS.codeItemModelWarehouse.Where(x => x.Active) on a.ItemModelWarehouseGUID equals d.ItemModelWarehouseGUID into LJ2
              from R2 in LJ2.DefaultIfEmpty()
              join e in DbWMS.codeWarehouseItemModel.Where(x => x.Active) on R2.WarehouseItemModelGUID equals e.WarehouseItemModelGUID
              join f in DbWMS.codeWarehouseItemModelLanguage.Where(x => x.LanguageID == LAN && x.Active) on e.WarehouseItemModelGUID equals f.WarehouseItemModelGUID into LJ3
              from R3 in LJ3.DefaultIfEmpty()
              join g in DbWMS.codeWarehouseItem.Where(x => x.Active) on e.WarehouseItemGUID equals g.WarehouseItemGUID
              join h in DbWMS.codeWarehouseItemLanguage.Where(x => x.LanguageID == LAN && x.Active) on g.WarehouseItemGUID equals h.WarehouseItemGUID into LJ4
              from R4 in LJ4.DefaultIfEmpty()
              join k in DbWMS.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.LastCustdianNameGUID equals k.UserGUID into LJ5
              from R5 in LJ5.DefaultIfEmpty()
              join L in DbWMS.codeWarehouse.Where(x => x.Active) on a.LastCustdianNameGUID equals L.WarehouseGUID into LJ6
              from R6 in LJ6.DefaultIfEmpty()
              join M in DbWMS.codeWarehouseLanguage.Where(x => x.Active && x.LanguageID == LAN) on R6.WarehouseGUID equals M.WarehouseGUID into LJ7
              from R7 in LJ7.DefaultIfEmpty()
              join N in DbWMS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN && x.codeTablesValues.TableGUID == LookupTables.ItemStatus) on a.ItemStatusGUID equals N.ValueGUID into LJ8
              from R8 in LJ8.DefaultIfEmpty()
              join o in DbWMS.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.CreatedByGUID equals o.UserGUID into LJ9
              from R9 in LJ9.DefaultIfEmpty()
              join p in DbWMS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.LastFlowTypeGUID equals p.ValueGUID into LJ10
              from R10 in LJ10.DefaultIfEmpty()
              join s in DbWMS.codeLocationsLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.LastLocationGUID equals s.LocationGUID into LJ11
              from R11 in LJ11.DefaultIfEmpty()
              join y in DbWMS.codeWarehouseRequesterTypeLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.LastCustdianNameGUID equals y.WarehouseRequesterTypeGUID into LJ17
              from R17 in LJ17.DefaultIfEmpty()
              join yy in DbWMS.codeWarehouseVehicleLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.LastCustdianNameGUID equals yy.WarehouseVehicleGUID into LJ18
              from R18 in LJ18.DefaultIfEmpty()

              select new WarehouseModelEntryMovementDataTableModel
              {
                  ItemInputDetailGUID = a.ItemInputDetailGUID,
                  Active = a.Active,
                  ItemModelWarehouseGUID = a.ItemModelWarehouseGUID.ToString(),
                  ModelDescription = R3.ModelDescription,
                  ItemDescription = R4.WarehouseItemDescription,
                  WarehouseOwnerGUID = R1.codeWarehouse.WarehouseGUID.ToString(),
                  BarcodeNumber = a.dataItemInputDeterminant.Where(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.Barcode).Select(x => x.DeterminantValue).FirstOrDefault(),
                  SerialNumber = a.dataItemInputDeterminant.Where(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.SerialNumber).Select(x => x.DeterminantValue).FirstOrDefault(),
                  IME1 = a.dataItemInputDeterminant.Where(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.IME1).Select(x => x.DeterminantValue).FirstOrDefault(),
                  GSM = a.dataItemInputDeterminant.Where(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.GSM).Select(x => x.DeterminantValue).FirstOrDefault(),
                  WarehouseOwner = R1.codeWarehouse.codeWarehouseLanguage.FirstOrDefault(x => x.Active && x.LanguageID == LAN).WarehouseDescription,
                  Governorate = R11.LocationDescription,
                  Custodian = a.IsAvaliable == true
                      ? "Stock " + R1.codeWarehouse.codeWarehouseLanguage
                            .FirstOrDefault(x => x.Active && x.LanguageID == LAN).WarehouseDescription
                      : (a.LastCustdianGUID == WarehouseRequestSourceTypes.Warehouse
                          ? R7.WarehouseDescription
                          : (a.LastCustdianGUID == WarehouseRequestSourceTypes.Staff
                              ? R5.FirstName + " " + R5.Surname
                              : (a.LastCustdianGUID == WarehouseRequestSourceTypes.Vehicle ?
                                  R17.WarehouseRequesterTypeDescription :
                                  a.LastCustdianGUID == WarehouseRequestSourceTypes.OtherRequester ? R18
                                      .VehicleDescription : null))),
                  ModelStatus = R8.ValueDescription,
                  CreatedDate = a.CreatedDate,

                  CreatedBy = R9.FirstName + " " + R9.Surname,
                  Comments = a.Comments,
                  DeliveryStatus = R10.ValueDescription,
                  dataItemInputDetailRowVersion = a.dataItemInputDetailRowVersion
              }).ToList();
            if (result.Count > 0)
            {
                string sourceFile = Server.MapPath("~/Areas/WMS/Templates/ItemModelTrackCustdoy.xlsx");
                string DisFolder =
                    Server.MapPath("~/Areas/WMS/temp/ItemModelTrackCustdoy" + DateTime.Now.ToBinary() + ".xlsx");
                System.IO.File.Copy(sourceFile, DisFolder);
                using (ExcelPackage package = new ExcelPackage(new FileInfo(DisFolder)))
                {
                    var cx = package.Workbook.Worksheets.ToList();
                    ExcelWorksheet workSheet = package.Workbook.Worksheets[1];

                    DataTable dt = new DataTable();
                    dt.Columns.Add("Item Classification", typeof(string));
                    dt.Columns.Add("Item Sub Classification", typeof(string));
                    dt.Columns.Add("Model Name", typeof(string));
                    dt.Columns.Add("Warehouse Owner", typeof(string));
                    dt.Columns.Add("Custodian", typeof(string));
                    dt.Columns.Add("Barcode", typeof(string));
                    dt.Columns.Add("Serial Number", typeof(string));
                    dt.Columns.Add("IME1", typeof(string));
                    dt.Columns.Add("GSM", typeof(string));
                    dt.Columns.Add("Location", typeof(string));
                    dt.Columns.Add("Service Status", typeof(string));
                    dt.Columns.Add("DeliveryStatus", typeof(string));
                    dt.Columns.Add("Comments", typeof(string));

                    foreach (var item in result)
                    {

                        DataRow dr;
                        dr = dt.NewRow();
                        dr[0] = item.WarehouseItemClassificationDescription;
                        dr[1] = item.ItemDescription;
                        dr[2] = item.ModelDescription;
                        dr[3] = item.WarehouseOwner;
                        dr[4] = item.Custodian;
                        dr[5] = item.BarcodeNumber;
                        dr[6] = item.SerialNumber;
                        dr[7] = item.IME1;
                        dr[8] = item.GSM;
                        dr[9] = item.Governorate;
                        dr[10] = item.ServiceItemStatus;
                        dr[11] = item.DeliveryStatus;
                        dr[12] = item.Comments;
                        dt.Rows.Add(dr);
                    }
                    workSheet.Cells["B5"].LoadFromDataTable(dt, true);
                    package.Save();
                }

                byte[] fileBytes = System.IO.File.ReadAllBytes(DisFolder);
                string fileName = DateTime.Now.ToString("yyMMdd") + "List of item custodian distribution " + ".xlsx";

                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }

            bool success = false;
            return Json(new { success = success }, JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region test
        public ActionResult Test()
        {
            return View();
        }
        #endregion

    }
}