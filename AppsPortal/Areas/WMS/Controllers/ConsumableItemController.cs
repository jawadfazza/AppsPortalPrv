using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.Models;
using AppsPortal.ViewModels;
using AutoMapper;
using LinqKit;
using RES_Repo.Globalization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using WMS_DAL.Model;
using WMS_DAL.ViewModels;

namespace AppsPortal.Areas.WMS.Controllers
{
    public class ConsumableItemController : WMSBaseController
    {
        #region Main Page
        // GET: WMS/ConsumableItem
        [Route("WMS/ConsumableItemOverview/")]
        public ActionResult ConsumableOverviewItemsIndex()
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Access, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            var CurrentWarehouseGUIDs = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).Select(x => x.WarehouseGUID)
              .ToList();
            ViewBag.TotalPendingVerification = DbWMS.dataItemOutputDetailFlow.Where(x => x.FlowTypeGUID == WarehouseRequestFlowType.PendingConfirmed
              && x.dataItemOutputDetail.dataItemInputDetail.codeItemModelWarehouse.codeWarehouseItemModel.codeWarehouseItem.IsDeterminanted == false
              && CurrentWarehouseGUIDs.Contains(x.dataItemOutputDetail.dataItemOutput.RequesterNameGUID)).Select(x => x.ItemOutputDetailGUID).Count();
            return View("~/Areas/WMS/Views/ConsumableItem/Index.cshtml");
        }

        [Route("WMS/WarehouseConsumableItemOverviewDataTable/")]
        public JsonResult WarehouseConsumableItemOverviewDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<ConsumableOverviewItemsDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<ConsumableOverviewItemsDataTableModel>(DataTable.Filters);
            }
            var CurrentWarehouseGUID = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).Select(x => x.WarehouseGUID)
                .ToList();
            var warehouseAuthGUIDs = DbWMS.codeWarehouse
                .Where(x => CurrentWarehouseGUID.Contains(x.WarehouseGUID) || CurrentWarehouseGUID.Contains(x.ParentGUID))
                .Select(x => x.WarehouseGUID).ToList();
            //Access is authorized by Access Action

            var All = (from a in DbWMS.v_ItemDefinationPerWarhouse.AsExpandable().Where(x => warehouseAuthGUIDs.Contains((Guid)x.WarehouseGUID) && x.IsDeterminanted == false)
                       select new ConsumableOverviewItemsDataTableModel
                       {

                           ItemModelWarehouseGUID = a.ItemModelWarehouseGUID,

                           Active = a.Active,
                           WarehouseDescription = a.WarehouseDescription,
                           ModelDescription = a.ModelDescription,
                           WarehouseItemDescription = a.WarehouseItemDescription,
                           WarehouseItemClassificationDescription = a.WarehouseItemClassificationDescription,
                           WarehouseItemClassificationGUID = a.WarehouseItemClassificationGUID.ToString(),
                           WarehouseItemGUID = a.WarehouseItemGuid.ToString(),
                           WarehouseItemModelGUID = a.WarehouseItemModelGUID.ToString(),
                           WarehouseGUID = a.WarehouseGUID.ToString(),
                           codeItemModelWarehouseRowVersion = a.codeItemModelWarehouseRowVersion,
                           TotalAvaiable = a.TotalAvaiable,
                           TotalEntry = a.TotalEntry,
                           TotalRelease = a.TotalRelease
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<ConsumableOverviewItemsDataTableModel> Result = Mapper.Map<List<ConsumableOverviewItemsDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Pending confirmation
        //[Route("WMS/PendingConfirmationIndex/")]
        public ActionResult PendingConfirmationIndex()
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Access, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }

            return View("~/Areas/WMS/Views/ConsumableItem/PendingConfirmationIndex.cshtml");
        }

        [Route("WMS/WarehouseConsumablePendingConfirmationoDataTable/")]
        public JsonResult WarehouseConsumablePendingConfirmationoDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<ConsumableItemPendingConfirmationDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<ConsumableItemPendingConfirmationDataTableModel>(DataTable.Filters);
            }
            var CurrentWarehouseGUID = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).Select(x => x.WarehouseGUID)
                .ToList();
            var warehouseAuthGUIDs = DbWMS.codeWarehouse
                .Where(x => CurrentWarehouseGUID.Contains(x.WarehouseGUID) || CurrentWarehouseGUID.Contains(x.ParentGUID))
                .Select(x => x.WarehouseGUID).ToList();


            //Access is authorized by Access Action

            var All = (from a in DbWMS.v_trackItemOutputFlow.AsExpandable().Where(x => warehouseAuthGUIDs.Contains((Guid)x.OutputWarehouseGUID) && x.FlowTypeGUID == WarehouseRequestFlowType.PendingConfirmed && x.IsDeterminanted == false
                       && x.IsLastAction == true)

                       select new ConsumableItemPendingConfirmationDataTableModel
                       {
                           ItemOutputDetailGUID = a.ItemOutputDetailGUID,
                           WarehouseItemGUID = a.WarehouseItemGuid.ToString(),
                           WarehouseItemModelGUID = a.WarehouseItemModelGUID.ToString(),
                           RequesterGUID = a.RequesterGUID.ToString(),
                           RequesterNameGUID = a.RequesterNameGUID.ToString(),
                           ItemOutputDetailFlowGUID = a.ItemOutputDetailFlowGUID,
                           dataItemOutputDetailFlowRowVersion = a.dataItemOutputDetailFlowRowVersion,

                           Active = a.Active,
                           Custodian = a.OutputCustodian,
                           OutputCustodianName = a.OutputCustodianName,
                           ModelDescription = a.ModelDescription,
                           RequestedQunatity = a.RequestedQunatity,
                           //WarehouseDescription = a.WarehouseDescription,
                           //ModelDescription = c.ModelDescription,
                           //WarehouseItemDescription = a.WarehouseItemDescription,
                           //WarehouseItemClassificationDescription = a.WarehouseItemClassificationDescription,
                           //WarehouseItemClassificationGUID = a.WarehouseItemClassificationGUID.ToString(),
                           //WarehouseItemGUID = a.WarehouseItemGuid.ToString(),
                           //WarehouseItemModelGUID = a.WarehouseItemModelGUID.ToString(),
                           //WarehouseGUID = a.WarehouseGUID.ToString(),
                           //codeItemModelWarehouseRowVersion = a.codeItemModelWarehouseRowVersion,
                           //TotalAvaiable = a.TotalAvaiable,
                           //TotalEntry = a.TotalEntry,
                           //TotalRelease = a.TotalRelease
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<ConsumableItemPendingConfirmationDataTableModel> Result = Mapper.Map<List<ConsumableItemPendingConfirmationDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Entry Consumables
        [Route("WMS/ConsumableItem/CreateEntry/")]
        public ActionResult ConsumableItemEntryCreate()
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            return View("~/Areas/WMS/Views/ConsumableItem/ConsumableItemEntry.cshtml", new EntryModelUpdateModel());
        }



        [Route("WMS/ConsumableItem/ConsumableItemEntryCreate/")]
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ConsumableItemEntryCreate(EntryModelUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (!ModelState.IsValid) return PartialView("~/Areas/WMS/Views/ConsumableItem/ConsumableItemEntry.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            dataItemInput ItemInput = Mapper.Map(model, new dataItemInput());
            var myWarehouse = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
            ItemInput.ItemInputGUID = EntityPK;
            ItemInput.CreatedDate = ExecutionTime;
            ItemInput.CreatedByGUID = UserGUID;
            ItemInput.SourceGUID = model.SourceGUID;
            ItemInput.WarehouseSiteGUID = myWarehouse.WarehouseGUID;

            DbWMS.Create(ItemInput, Permissions.WarehouseItemsEntry.CreateGuid, ExecutionTime, DbCMS);



            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.WarehouseConsumableItemEntryDetailDataTable, "ConsumableItem", "ItemEntryDetailContainer"));



            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.WarehouseItemsEntry.Create, Apps.WMS, new UrlHelper(Request.RequestContext).Action("Create", "ConsumableItem", new { Area = "WMS" })), Container = "ItemModelFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.WarehouseItemsEntry.Update, Apps.WMS), Container = "ItemModelFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.WarehouseItemsEntry.Delete, Apps.WMS), Container = "ItemModelFormControls" });
            dataItemInputDetail detail = new dataItemInputDetail { ItemInputGUID = ItemInput.ItemInputGUID, dataItemInputDetailRowVersion = ItemInput.dataItemInputRowVersion };
            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleCreateMessage(DbWMS.PrimaryKeyControl(ItemInput), DbWMS.RowVersionControls(ItemInput, detail), Partials, "", UIButtons));

                //return Json(DbWMS.SingleCreateMessage(DbWMS.PrimaryKeyControl(ItemInput), DbWMS.RowVersionControls(ItemInput, entryMoedelDetail), Partials, "", UIButtons));


            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }
        [Route("WMS/ConsumableItem/Update/{PK}")]
        public ActionResult ConsumableItemEntryUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Update, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            var model = (from a in DbWMS.dataItemInput.WherePK(PK)
                             //join b in DbWMS.dataItemInputLanguage.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.dataItemInput.DeletedOn) && x.LanguageID == LAN)
                             //on a.WarehouseItemClassificationGUID equals b.WarehouseItemClassificationGUID into LJ1
                             //from R1 in LJ1.DefaultIfEmpty()
                         select new EntryModelsDataTableModel
                         {
                             ItemInputGUID = a.ItemInputGUID,
                             SourceGUID = a.SourceGUID,
                             BillNumber = a.BillNumber,
                             Active = a.Active,
                             dataItemInputRowVersion = a.dataItemInputRowVersion,

                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("CreateEntry", "ConsumableItem", new { Area = "WMS" }));

            return View("~/Areas/WMS/Views/ConsumableItem/ConsumableItemEntry.cshtml", model);
        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ConsumableItemEntryUpdate(EntryModelUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Update, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (!ModelState.IsValid) return PartialView("~/Areas/WMS/Views/ConsumableItem/ConsumableItemEntry.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            dataItemInput WarehouseItemsEntry = Mapper.Map(model, new dataItemInput());
            DbWMS.Update(WarehouseItemsEntry, Permissions.WarehouseItemsEntry.UpdateGuid, ExecutionTime, DbCMS);
            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleUpdateMessage(DataTableNames.WarehouseConsumableItemOverviewDataTable, DbWMS.PrimaryKeyControl(WarehouseItemsEntry), DbWMS.RowVersionControls(Portal.SingleToList(WarehouseItemsEntry))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return Json(DbWMS.SingleUpdateMessage(DataTableNames.WarehouseConsumableItemOverviewDataTable, DbWMS.PrimaryKeyControl(WarehouseItemsEntry), DbWMS.RowVersionControls(Portal.SingleToList(WarehouseItemsEntry))));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }




        #endregion

        #region Entry Detail
        //public ActionResult ConsumableItemEntryDetailIndex()
        //{

        //    if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Access, Apps.WMS))
        //    {
        //        return Json(DbWMS.PermissionError());
        //    }
        //    return View("~/Areas/WMS/Views/ConsumableItemEntryDetail/_ConsumableItemEntryDetailDataTable.cshtml");
        //}
        public ActionResult WarehouseConsumableItemEntryDetailDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/WMS/Views/ConsumableItemEntryDetail/_ConsumableItemEntryDetailDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<ConsumableItemInputDetailDataTable, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<ConsumableItemInputDetailDataTable>(DataTable.Filters);
            }
            var All = (from a in DbWMS.dataItemInputDetail.AsExpandable().Where(x => x.ItemInputGUID == PK)
                       join b in DbWMS.codeItemModelWarehouse on a.ItemModelWarehouseGUID equals b.ItemModelWarehouseGUID
                       join c in DbWMS.codeWarehouseItemModelLanguage.Where(x => x.LanguageID == LAN) on b.WarehouseItemModelGUID equals c.WarehouseItemModelGUID into LJ1
                       from R1 in LJ1.DefaultIfEmpty()
                       select new ConsumableItemInputDetailDataTable
                       {
                           ItemInputDetailGUID = a.ItemInputDetailGUID,
                           ItemInputGUID = a.ItemInputGUID,
                           Qunatity = a.Qunatity,
                           LastWarehouseLocationGUID = a.LastWarehouseLocationGUID,
                           dataItemInputDetailRowVersion = a.dataItemInputDetailRowVersion,
                           ModelDescription = R1.ModelDescription,
                           ItemModelWarehouseGUID = a.ItemModelWarehouseGUID,
                           Active = a.Active,

                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<ConsumableItemInputDetailDataTable> Result = Mapper.Map<List<ConsumableItemInputDetailDataTable>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }





        public ActionResult ConsumableItemEntryDetailCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            return PartialView("~/Areas/WMS/Views/ConsumableItemEntryDetail/_ConsumableItemEntryDetailForm.cshtml",
                new ConsumableItemDetailModel { ItemInputGUID = FK });
        }



        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ConsumableItemEntryDetailCreate(ConsumableItemDetailModel model)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (!ModelState.IsValid) return PartialView("~/Areas/WMS/Views/ConsumableItemEntryDetail/_ConsumableItemEntryDetailForm.cshtml", model);
            if (model.Qunatity <= 0 || model.ItemModelWarehouseGUID == null)
            {
                return PartialView("~/Areas/WMS/Views/ConsumableItemEntryDetail/_ConsumableItemEntryDetailForm.cshtml", model);
            }
            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            dataItemInputDetail ItemInputdetail = Mapper.Map(model, new dataItemInputDetail());
            var myWarehouse = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
            ItemInputdetail.ItemInputDetailGUID = EntityPK;
            ItemInputdetail.ItemInputGUID = (Guid)model.ItemInputGUID;
            ItemInputdetail.CreatedDate = ExecutionTime;
            ItemInputdetail.CreatedByGUID = UserGUID;
            ItemInputdetail.ItemStatusGUID = WarehouseItemStatus.Functionting;
            ItemInputdetail.WarehouseOwnerGUID = myWarehouse.WarehouseGUID;

            DbWMS.Create(ItemInputdetail, Permissions.WarehouseItemsEntry.CreateGuid, ExecutionTime, DbCMS);
            var myModel = DbWMS.codeItemModelWarehouse.Where(x => x.ItemModelWarehouseGUID == model.ItemModelWarehouseGUID).FirstOrDefault();
            int avai = myModel.TotalAvaiable ?? 0;
            int Entry = myModel.TotalEntry ?? 0;
            myModel.TotalAvaiable = avai + model.Qunatity;
            myModel.TotalEntry = Entry + model.Qunatity;
            DbWMS.Update(myModel, Permissions.WarehouseItemsEntry.UpdateGuid, ExecutionTime, DbCMS);

            dataItemTransfer TransferModel = new dataItemTransfer();
            TransferModel.ItemTransferGUID = Guid.NewGuid();
            TransferModel.ItemInputDetailGUID = ItemInputdetail.ItemInputDetailGUID;
            TransferModel.TransferDate = ExecutionTime;
            TransferModel.SourceGUID = myWarehouse.WarehouseGUID;
            TransferModel.DestionationGUID = myWarehouse.WarehouseGUID;

            TransferModel.TransferedByGUID = UserGUID;
            TransferModel.IsLastTransfer = true;
            DbWMS.Create(TransferModel, Permissions.WarehouseItemsEntry.CreateGuid, ExecutionTime, DbCMS);


            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleUpdateMessage(DataTableNames.WarehouseConsumableItemEntryDetailDataTable, DbWMS.PrimaryKeyControl(ItemInputdetail), DbWMS.RowVersionControls(Portal.SingleToList(ItemInputdetail))));

            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        public ActionResult ConsumableItemEntryDetailUpdate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            ConsumableItemDetailModel model = DbWMS.dataItemInputDetail.Where(x => x.ItemInputDetailGUID == FK).Select(x => new ConsumableItemDetailModel
            {
                ItemInputDetailGUID = x.ItemInputDetailGUID,
                ItemInputGUID = x.ItemInputGUID,
                ItemModelWarehouseGUID = x.ItemModelWarehouseGUID,
                Qunatity = x.Qunatity,
                Active = x.Active,



            }).FirstOrDefault();

            return PartialView("~/Areas/WMS/Views/ConsumableItemEntryDetail/_ConsumableItemEntryDetailForm.cshtml", model);
        }



        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ConsumableItemEntryDetailUpdate(ConsumableItemDetailModel model)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (!ModelState.IsValid) return PartialView("~/Areas/WMS/Views/ConsumableItemEntryDetail/_ConsumableItemEntryDetailForm.cshtml", model);
            if (model.Qunatity <= 0)
            {
                return PartialView("~/Areas/WMS/Views/ConsumableItemEntryDetail/_ConsumableItemEntryDetailForm.cshtml", model);
            }
            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();
            dataItemInputDetail ItemInputdetail = DbWMS.dataItemInputDetail.Where(x => x.ItemInputDetailGUID == model.ItemInputDetailGUID).FirstOrDefault();
            int currQuantity = ItemInputdetail.Qunatity;
            ItemInputdetail.Qunatity = model.Qunatity;



            DbWMS.Update(ItemInputdetail, Permissions.WarehouseItemsEntry.UpdateGuid, ExecutionTime, DbCMS);
            var myModel = DbWMS.codeItemModelWarehouse.Where(x => x.ItemModelWarehouseGUID == model.ItemModelWarehouseGUID).FirstOrDefault();
            var dataItemInputDetail = DbWMS.dataItemInputDetail.Where(x => x.ItemModelWarehouseGUID == model.ItemModelWarehouseGUID).ToList();
            int det = dataItemInputDetail.Where(x => x.ItemInputDetailGUID != model.ItemInputDetailGUID).Select(x => x.Qunatity).Sum();
            int avai = myModel.TotalAvaiable ?? 0;
            int CurrAvai = avai - currQuantity;
            int Entry = myModel.TotalEntry ?? 0;
            myModel.TotalAvaiable = CurrAvai + model.Qunatity;
            myModel.TotalEntry = det + model.Qunatity;
            DbWMS.Update(myModel, Permissions.WarehouseItemsEntry.UpdateGuid, ExecutionTime, DbCMS);



            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleUpdateMessage(DataTableNames.WarehouseConsumableItemEntryDetailDataTable, DbWMS.PrimaryKeyControl(ItemInputdetail), DbWMS.RowVersionControls(Portal.SingleToList(ItemInputdetail))));

            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }
        #endregion

        #region Relase Consumables
        [Route("WMS/ConsumableItem/CreateRelease/")]
        public ActionResult ConsumableItemReleaseCreate()
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            return View("~/Areas/WMS/Views/ConsumableItemRelease/ConsumableItemRelease.cshtml", new ReleaseModelUpdateModel());
        }



        [Route("WMS/ConsumableItem/ConsumableItemReleaseCreate/")]
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ConsumableItemReleaseCreate(ReleaseModelUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (!ModelState.IsValid) return PartialView("~/Areas/WMS/Views/ConsumableItemRelease/ConsumableItemRelease.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            dataItemOutput myModel = Mapper.Map(model, new dataItemOutput());
            int outputNumber = DbWMS.dataItemOutput.Select(x => x.OutputNumber).Max() + 1 ?? 1;
            var myWarehouse = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
            myModel.ItemOutputGUID = EntityPK;
            myModel.CreatedDate = ExecutionTime;
            myModel.CreatedByGUID = UserGUID;
            myModel.RequesterGUID = model.RequesterGUID;
            myModel.RequesterNameGUID = model.RequesterNameGUID;
            myModel.WarehouseGUID = myWarehouse.WarehouseGUID;
            myModel.OutputNumber = outputNumber;

            DbWMS.Create(myModel, Permissions.WarehouseItemsRelease.CreateGuid, ExecutionTime, DbCMS);



            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.WarehouseConsumableItemReleaseDetailDataTable, "ConsumableItem", "WarehouseConsumableReleaseDetailContainer"));



            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.WarehouseItemsEntry.Create, Apps.WMS, new UrlHelper(Request.RequestContext).Action("ConsumableItemReleaseCreate", "ConsumableItem", new { Area = "WMS" })), Container = "releaseForm" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.WarehouseItemsEntry.Update, Apps.WMS), Container = "releaseForm" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.WarehouseItemsEntry.Delete, Apps.WMS), Container = "releaseForm" });
            dataItemOutputDetail detail = new dataItemOutputDetail { ItemOutputGUID = myModel.ItemOutputGUID, dataItemOutputDetailRowVersion = myModel.dataItemOutputRowVersion };
            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleCreateMessage(DbWMS.PrimaryKeyControl(myModel), DbWMS.RowVersionControls(myModel, detail), Partials, "", UIButtons));
                //return Json(DbWMS.SingleCreateMessage(DbWMS.PrimaryKeyControl(ItemInput), DbWMS.RowVersionControls(ItemInput, entryMoedelDetail), Partials, "", UIButtons));


            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }
        [Route("WMS/ConsumableItemRelease/Update/{PK}")]
        public ActionResult ConsumableItemReleaseUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Update, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            var model = (from a in DbWMS.dataItemOutput.WherePK(PK)
                             //join b in DbWMS.dataItemInputLanguage.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.dataItemInput.DeletedOn) && x.LanguageID == LAN)
                             //on a.WarehouseItemClassificationGUID equals b.WarehouseItemClassificationGUID into LJ1
                             //from R1 in LJ1.DefaultIfEmpty()
                         select new ReleaseModelUpdateModel
                         {
                             ItemOutputGUID = a.ItemOutputGUID,
                             WarehouseGUID = a.WarehouseGUID,
                             OutputNumber = a.OutputNumber,
                             Active = a.Active,
                             RequesterGUID = a.RequesterGUID,
                             RequesterNameGUID = a.RequesterNameGUID,
                             dataItemOutputRowVersion = a.dataItemOutputRowVersion,

                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("CreateEntry", "ConsumableItem", new { Area = "WMS" }));
            return View("~/Areas/WMS/Views/ConsumableItemRelease/ConsumableItemRelease.cshtml", model);

        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ConsumableItemReleaseUpdate(ReleaseModelUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Update, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (!ModelState.IsValid) return View("~/Areas/WMS/Views/ConsumableItemRelease/ConsumableItemRelease.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            dataItemOutput WarehouseItemsEntry = Mapper.Map(model, new dataItemOutput());
            DbWMS.Update(WarehouseItemsEntry, Permissions.WarehouseItemsRelease.UpdateGuid, ExecutionTime, DbCMS);




            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleUpdateMessage(DataTableNames.WarehouseConsumableItemOverviewDataTable, DbWMS.PrimaryKeyControl(WarehouseItemsEntry), DbWMS.RowVersionControls(Portal.SingleToList(WarehouseItemsEntry))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return Json(DbWMS.SingleUpdateMessage(DataTableNames.WarehouseConsumableItemOverviewDataTable, DbWMS.PrimaryKeyControl(WarehouseItemsEntry), DbWMS.RowVersionControls(Portal.SingleToList(WarehouseItemsEntry))));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }




        #endregion

        #region Release Detail
        //public ActionResult ConsumableItemEntryDetailIndex()
        //{

        //    if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Access, Apps.WMS))
        //    {
        //        return Json(DbWMS.PermissionError());
        //    }
        //    return View("~/Areas/WMS/Views/ConsumableItemEntryDetail/_ConsumableItemEntryDetailDataTable.cshtml");
        //}


        public ActionResult WarehouseConsumableItemReleaseDetailDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/WMS/Views/ConsumableItemReleaseDetail/_ConsumableItemReleaseDetailDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<ConsumableItemReleaseDetailDataTable, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<ConsumableItemReleaseDetailDataTable>(DataTable.Filters);
            }


            //Access is authorized by Access Action

            var All = (from a in DbWMS.dataItemOutputDetail.AsExpandable().Where(x => x.ItemOutputGUID == PK)
                       join b in DbWMS.dataItemInputDetail on a.ItemInputDetailGUID equals b.ItemInputDetailGUID
                       join d in DbWMS.codeItemModelWarehouse on b.ItemModelWarehouseGUID equals d.ItemModelWarehouseGUID
                       join c in DbWMS.codeWarehouseItemModelLanguage.Where(x => x.LanguageID == LAN) on d.WarehouseItemModelGUID equals c.WarehouseItemModelGUID into LJ1
                       from R1 in LJ1.DefaultIfEmpty()
                       join e in DbWMS.codeWarehouseLocationLanguage on a.WarehouseLocationGUID equals e.WarehouseLocationGUID into LJ2
                       from R2 in LJ2.DefaultIfEmpty()
                       select new ConsumableItemReleaseDetailDataTable
                       {
                           ItemOutputDetailGUID = a.ItemOutputDetailGUID,
                           ItemOutputGUID = a.ItemOutputGUID,
                           ModelDescription = R1.ModelDescription,
                           WarehouseLocationDescription = R2.WarehouseLocationDescription,
                           RequestedQunatity = a.RequestedQunatity,
                           WarehouseLocationGUID = a.WarehouseLocationGUID,
                           ItemInputDetailGUID = a.ItemInputDetailGUID,

                           ItemModelWarehouseGUID = b.ItemModelWarehouseGUID.ToString(),
                           dataItemOutputDetailRowVersion = a.dataItemOutputDetailRowVersion,
                           Active = a.Active,

                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<ConsumableItemReleaseDetailDataTable> Result = Mapper.Map<List<ConsumableItemReleaseDetailDataTable>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }


        public ActionResult ConsumableItemReleaseDetailCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            return PartialView("~/Areas/WMS/Views/ConsumableItemReleaseDetail/_ConsumableItemReleaseDetailForm.cshtml", new ConsumableReleaseModelDetailUpdateModel { ItemOutputGUID = FK });

        }




        public ActionResult ConsumableItemReleaseDetailUpdate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            dataItemOutputDetail detail = DbWMS.dataItemOutputDetail.Where(x => x.ItemOutputDetailGUID == FK).FirstOrDefault();
            ConsumableReleaseModelDetailUpdateModel myModel = Mapper.Map(detail, new ConsumableReleaseModelDetailUpdateModel());

            myModel.ItemModelWarehouseGUID = detail.dataItemInputDetail.ItemModelWarehouseGUID;
            return PartialView("~/Areas/WMS/Views/ConsumableItemReleaseDetail/_ConsumableItemReleaseDetailForm.cshtml", myModel);

        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ConsumableItemReleaseDetailUpdate(ConsumableReleaseModelDetailUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (!ModelState.IsValid) return PartialView("~/Areas/WMS/Views/ConsumableItemReleaseDetail/_ConsumableItemReleaseDetailForm.cshtml", model);
            var inputDetail = DbWMS.dataItemOutputDetail.Where(x => x.ItemOutputDetailGUID == model.ItemOutputDetailGUID).Select(x => x.ItemInputDetailGUID).FirstOrDefault();
            var modelWarehouse = DbWMS.dataItemInputDetail.Where(x => x.ItemInputDetailGUID == inputDetail).Select(x => x.ItemModelWarehouseGUID).FirstOrDefault();
            var codeModel = DbWMS.codeItemModelWarehouse.Where(x => x.ItemModelWarehouseGUID == modelWarehouse).FirstOrDefault();
            if (model.WarehouseLocationGUID == null || (model.RequestedQunatity == null || model.RequestedQunatity == 0 || model.RequestedQunatity > codeModel.TotalAvaiable))
            {
                return PartialView("~/Areas/WMS/Views/ConsumableItemReleaseDetail/_ConsumableItemReleaseDetailForm.cshtml", model);
            }
            DateTime ExecutionTime = DateTime.Now;
            var itemoutput = DbWMS.dataItemOutput.Where(x => x.ItemOutputGUID == model.ItemOutputGUID).FirstOrDefault();
            var detail = DbWMS.dataItemOutputDetail.Where(x => x.ItemOutputDetailGUID == model.ItemOutputDetailGUID).FirstOrDefault();
            detail.WarehouseLocationGUID = model.WarehouseLocationGUID;
            detail.RequestedQunatity = model.RequestedQunatity;
            detail.ExpectedStartDate = model.ExpectedStartDate;
            detail.Comments = model.Comments;

            //dataItemOutputDetail detail = Mapper.Map(model, new dataItemOutputDetail());
            var myWarehouse = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).FirstOrDefault();


            if (model.RequestedQunatity == null)
                detail.RequestedQunatity = 1;


            var myWarhouseModel = DbWMS.codeItemModelWarehouse.Where(x => x.ItemModelWarehouseGUID == modelWarehouse).FirstOrDefault();
            var currentRelease = myWarhouseModel.TotalRelease ?? 0;
            var totalInput = DbWMS.dataItemInputDetail.Where(x => x.ItemModelWarehouseGUID == model.ItemModelWarehouseGUID && x.Active).Select(x => x.Qunatity).Sum();
            var totalRelease = DbWMS.dataItemOutputDetail.Where(x => x.dataItemInputDetail.ItemModelWarehouseGUID == model.ItemModelWarehouseGUID && x.Active).Select(x => x.RequestedQunatity).Sum();
            var totalReleaseExcpet = DbWMS.dataItemOutputDetail.Where(x => x.dataItemInputDetail.ItemModelWarehouseGUID == model.ItemModelWarehouseGUID && x.Active
            && x.ItemOutputDetailGUID != model.ItemOutputDetailGUID).Select(x => x.RequestedQunatity).Sum();
            myWarhouseModel.TotalAvaiable = (totalInput - totalReleaseExcpet) - model.RequestedQunatity;
            myWarhouseModel.TotalRelease = totalReleaseExcpet + model.RequestedQunatity;

            var output = DbWMS.dataItemOutput.Where(x => x.ItemOutputGUID == model.ItemOutputGUID).FirstOrDefault();

            int? myOrder = DbWMS.dataItemOutputDetailFlow
                            .Where(x => x.dataItemOutputDetail.ItemOutputGUID == model.ItemOutputGUID)
                            .Select(x => x.OrderId).Max() + 1;
            if (myOrder == null)
                myOrder = 1;

            DbWMS.Update(detail, Permissions.WarehouseItemsRelease.CreateGuid, ExecutionTime, DbCMS);


            DbWMS.Update(myWarhouseModel, Permissions.WarehouseItemsRelease.UpdateGuid, ExecutionTime, DbCMS);
            //ConsumableReleaseModelDetailUpdateModel myVM = Mapper.Map(detail, new ConsumableReleaseModelDetailUpdateModel());
            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                if (itemoutput.RequesterGUID == WarehouseConsumableRequestSourceTypes.Warehouse)
                    SendConfirmationItemEmail(itemoutput);
                return Json(DbWMS.SingleUpdateMessage(DataTableNames.WarehouseConsumableItemReleaseDetailDataTable, DbWMS.PrimaryKeyControl(detail), DbWMS.RowVersionControls(Portal.SingleToList(detail))));

            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ConsumableItemReleaseDetailCreate(ConsumableReleaseModelDetailUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (!ModelState.IsValid) return PartialView("~/Areas/WMS/Views/ConsumableItemReleaseDetail/_ConsumableItemReleaseDetailForm.cshtml", model);
            var codeModel = DbWMS.codeItemModelWarehouse.Where(x => x.ItemModelWarehouseGUID == model.ItemModelWarehouseGUID).FirstOrDefault();
            if (model.WarehouseLocationGUID == null || model.ItemModelWarehouseGUID == null || (model.RequestedQunatity == null || model.RequestedQunatity == 0 || model.RequestedQunatity > codeModel.TotalAvaiable))
            {
                return PartialView("~/Areas/WMS/Views/ConsumableItemReleaseDetail/_ConsumableItemReleaseDetailForm.cshtml", model);
            }
            DateTime ExecutionTime = DateTime.Now;
            var itemoutput = DbWMS.dataItemOutput.Where(x => x.ItemOutputGUID == model.ItemOutputGUID).FirstOrDefault();
            Guid EntityPK = Guid.NewGuid();
            var inputDetail = DbWMS.dataItemInputDetail.Where(x => x.ItemModelWarehouseGUID == model.ItemModelWarehouseGUID).FirstOrDefault();

            dataItemOutputDetail detail = Mapper.Map(model, new dataItemOutputDetail());
            var myWarehouse = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
            detail.ItemOutputDetailGUID = EntityPK;
            detail.CreatedDate = ExecutionTime;
            detail.CreatedByGUID = UserGUID;
            detail.ItemInputDetailGUID = inputDetail.ItemInputDetailGUID;
            if (model.RequestedQunatity == null)
                detail.RequestedQunatity = 1;



            var myWarhouseModel = DbWMS.codeItemModelWarehouse.Where(x => x.ItemModelWarehouseGUID == model.ItemModelWarehouseGUID).FirstOrDefault();
            var currentRelease = myWarhouseModel.TotalRelease ?? 0;
            myWarhouseModel.TotalAvaiable = myWarhouseModel.TotalAvaiable - model.RequestedQunatity;
            myWarhouseModel.TotalRelease = currentRelease + model.RequestedQunatity;

            var output = DbWMS.dataItemOutput.Where(x => x.ItemOutputGUID == model.ItemOutputGUID).FirstOrDefault();

            int? myOrder = DbWMS.dataItemOutputDetailFlow
                            .Where(x => x.dataItemOutputDetail.ItemOutputGUID == model.ItemOutputGUID)
                            .Select(x => x.OrderId).Max() + 1;
            if (myOrder == null)
                myOrder = 1;
            dataItemOutputDetailFlow myoutputDetailFlow = new dataItemOutputDetailFlow();
            myoutputDetailFlow.ItemOutputDetailFlowGUID = Guid.NewGuid();
            myoutputDetailFlow.ItemOutputDetailGUID = EntityPK;
            if (output.RequesterGUID == WarehouseConsumableRequestSourceTypes.Warehouse)
            {
                myoutputDetailFlow.FlowTypeGUID = WarehouseRequestFlowType.PendingConfirmed;
                //myWarhouseModel.FlowTypeGUID = WarehouseRequestFlowType.PendingConfirmed;
            }
            else
            {
                myoutputDetailFlow.FlowTypeGUID = WarehouseRequestFlowType.Confirmed;
                //myWarhouseModel.FlowTypeGUID = WarehouseRequestFlowType.Confirmed;

            }
            myoutputDetailFlow.ItemStatuGUID = WarehouseItemStatus.Functionting;
            myoutputDetailFlow.CreatedDate = DateTime.Now;
            myoutputDetailFlow.IsLastAction = true;
            myoutputDetailFlow.IsLastMove = true;
            myoutputDetailFlow.CreatedByGUID = UserGUID;
            myoutputDetailFlow.Active = true;
            myoutputDetailFlow.OrderId = myOrder;
            DbWMS.Create(detail, Permissions.WarehouseItemsRelease.CreateGuid, ExecutionTime, DbCMS);
            DbWMS.Create(myoutputDetailFlow, Permissions.WarehouseItemsRelease.CreateGuid, ExecutionTime, DbCMS);

            DbWMS.Update(myWarhouseModel, Permissions.WarehouseItemsRelease.UpdateGuid, ExecutionTime, DbCMS);
            //ConsumableReleaseModelDetailUpdateModel myVM = Mapper.Map(detail, new ConsumableReleaseModelDetailUpdateModel());
            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                if (itemoutput.RequesterGUID == WarehouseConsumableRequestSourceTypes.Warehouse)
                    SendConfirmationItemEmail(itemoutput);
                return Json(DbWMS.SingleUpdateMessage(DataTableNames.WarehouseConsumableItemReleaseDetailDataTable, DbWMS.PrimaryKeyControl(detail), DbWMS.RowVersionControls(Portal.SingleToList(detail))));

            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }
        #endregion
        #region confirm
        public ActionResult ConfirmReceivingConsumableBulkModelEmail(Guid id)
        {
            //ss

            var myFlow = DbWMS.dataItemOutputDetailFlow.Where(x => x.dataItemOutputDetail.ItemOutputGUID == id
                                                                   && x.Active
                                                                   && x.IsLastAction == true
                                                                   && x.FlowTypeGUID ==
                                                                   WarehouseRequestFlowType.PendingConfirmed).ToList();
            if (myFlow == null)
            {

                ReleaseSingleItemUpdateModalUpdateModel mymodel = new ReleaseSingleItemUpdateModalUpdateModel();
                return View("~/Areas/WMS/Views/ModelConfirmation/ConfirmReceivingConsumableBulkModelsEmail.cshtml", mymodel);

            }

            else if (myFlow.Select(x => x.dataItemOutputDetail.dataItemOutput.RequesterGUID).FirstOrDefault() == WarehouseConsumableRequestSourceTypes.Warehouse)
            {
                var requesterNameGUID = myFlow.Select(f => f.dataItemOutputDetail.dataItemOutput.RequesterNameGUID).FirstOrDefault();
                var focalPointGUID = DbWMS.codeWarehouseFocalPoint.Where(x =>
                      x.WarehouseGUID == requesterNameGUID
                      && x.UserGUID == UserGUID).FirstOrDefault();
                if (focalPointGUID == null)
                {
                    return Json(DbWMS.PermissionError());
                }
            }
            if (myFlow != null && myFlow.Count > 0)
            {
                var myFlowGuis = myFlow.Select(x => x.ItemOutputDetailGUID).ToList();

                var myDetail = DbWMS.dataItemOutputDetail.Where(x => x.ItemOutputGUID == id
                                                                     && myFlowGuis.Contains(x.ItemOutputDetailGUID)).ToList();


                List<ReleaseSingleItemUpdateModalUpdateModel> model = new List<ReleaseSingleItemUpdateModalUpdateModel>();
                foreach (var item in myDetail)
                {
                    ReleaseSingleItemUpdateModalUpdateModel mymodel = new ReleaseSingleItemUpdateModalUpdateModel();
                    mymodel.ItemInputDetailGUID = item.ItemInputDetailGUID;
                    mymodel.ItemOutputGUID = id;
                    mymodel.ItemOutputDetailGUID = item.ItemOutputDetailGUID;
                    mymodel.ExpectedStartDate = item.ExpectedStartDate;
                    mymodel.RequestedQunatity = item.RequestedQunatity;



                    mymodel.IssuedBy = item.userAccounts.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).Select(x => x.FirstName).FirstOrDefault() + " " + item.userAccounts.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).Select(x => x.Surname).FirstOrDefault();
                    mymodel.WarehouseItemDescription = item.dataItemInputDetail.codeItemModelWarehouse.codeWarehouseItemModel
                        .codeWarehouseItem.codeWarehouseItemLanguage.Where(x => x.LanguageID == LAN).Select(x => x.WarehouseItemDescription)
                        .FirstOrDefault();
                    mymodel.BrandDescription = item.dataItemInputDetail.codeItemModelWarehouse.codeWarehouseItemModel
                        .codeBrand.codeBrandLanguage.Where(x => x.LanguageID == LAN).Select(x => x.BrandDescription)
                        .FirstOrDefault();

                    //zz
                    mymodel.WarehouseItemDescription = item.dataItemInputDetail.codeItemModelWarehouse.codeWarehouseItemModel
                        .codeWarehouseItem.codeWarehouseItemLanguage.Where(x => x.LanguageID == LAN).Select(x => x.WarehouseItemDescription)
                        .FirstOrDefault();
                    mymodel.BrandDescription = item.dataItemInputDetail.codeItemModelWarehouse.codeWarehouseItemModel
                        .codeBrand.codeBrandLanguage.Where(x => x.LanguageID == LAN).Select(x => x.BrandDescription)
                        .FirstOrDefault();
                    mymodel.ModelDescription = item.dataItemInputDetail.codeItemModelWarehouse.codeWarehouseItemModel
                        .codeWarehouseItemModelLanguage.Where(x => x.LanguageID == LAN).Select(x => x.ModelDescription)
                        .FirstOrDefault();

                    mymodel.Validation = true;
                    model.Add(mymodel);


                }


                ViewBag.ItemOutputGUID = id;



                return View("~/Areas/WMS/Views/ModelConfirmation/ConfirmReceivingConsumableBulkModelsEmail.cshtml", model);
            }

            return View("~/Areas/WMS/Views/ModelConfirmation/ConfirmReceivingConsumableBulkModelsEmail.cshtml", new List<ReleaseSingleItemUpdateModalUpdateModel>());

        }





        [HttpPost]
        public ActionResult ConfirmReceivingConsumableBulkModelEmail(List<Guid> guids)
        {
            //if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Create, Apps.WMS))
            //{
            //    return Json(DbWMS.PermissionError());
            //}
            //dataItemOutputDetailFlow myModel = Mapper.Map(model, new dataItemOutputDetailFlow());
            // if (!ModelState.IsValid || ActiveDamagedModelFlow(model)) return PartialView("~/Areas/WMS/Views/ReleaseModelFlows/_ReleaseModelFlowUpdateModal.cshtml", model);
            DateTime ExecutionTime = DateTime.Now;
            var model = DbWMS.dataItemOutputDetail.Where(x => guids.Contains(x.ItemOutputDetailGUID)).ToList();
            var itemInputGUID = model.Select(x => x.dataItemInputDetail.ItemInputGUID).FirstOrDefault();
            var itemInput = DbWMS.dataItemInput.Where(x => x.ItemInputGUID == itemInputGUID).FirstOrDefault();

            var requester = model.Select(x => x.dataItemOutput.RequesterGUID).FirstOrDefault();
            var requesterNameGUID = model.Select(x => x.dataItemOutput.RequesterNameGUID).FirstOrDefault();
            List<dataItemOutputDetailFlow> flowDetails = new List<dataItemOutputDetailFlow>();
            foreach (var item in model)
            {
                List<dataItemOutputDetailFlow> flows = DbWMS.dataItemOutputDetailFlow
               .Where(x => x.ItemOutputDetailGUID == item.ItemOutputDetailGUID).ToList();
                foreach (var flow in flows)
                {
                    flow.IsLastAction = false;
                    flow.IsLastMove = false;
                }

                DbWMS.SaveChanges();

                //List<dataItemInputDetail> ItemInputs = DbWMS.dataItemInputDetail
                //    .Where(x => x.ParentItemModelWarehouseGUID == myoutputDetail.ItemInputDetailGUID).ToList();

                int? order = DbWMS.dataItemOutputDetailFlow
                                 .Where(x => x.dataItemOutputDetail.ItemInputDetailGUID == item.ItemInputDetailGUID)
                                 .Select(x => x.OrderId).Max() + 1;
                if (order == null)
                    order = 1;
                //DbWMS.Up(flows, Permissions.WarehouseItemsEntry.UpdateGuid, ExecutionTime, DbCMS);
                dataItemOutputDetailFlow flowDetail = new dataItemOutputDetailFlow();
                flowDetail.ItemOutputDetailFlowGUID = Guid.NewGuid();
                flowDetail.FlowTypeGUID = WarehouseRequestFlowType.Confirmed;
                flowDetail.ItemOutputDetailGUID = item.ItemOutputDetailGUID;

                flowDetail.CreatedDate = ExecutionTime;
                flowDetail.ItemStatuGUID = WarehouseItemStatus.Functionting;
                flowDetail.IsLastAction = true;
                flowDetail.IsLastMove = true;
                flowDetail.CreatedByGUID = UserGUID;
                flowDetail.Active = true;
                flowDetail.OrderId = order;
                //flowDetails.Add(flowDetail);
                DbWMS.CreateNoAudit(flowDetail);
                var input = DbWMS.dataItemInputDetail.Find(item.ItemInputDetailGUID);
                input.LastFlowTypeGUID = WarehouseRequestFlowType.Confirmed;
                DbWMS.UpdateNoAudit(input);


            }

            if (requester == WarehouseConsumableRequestSourceTypes.Warehouse)
            {
                var itemModelGUIDs = model.Select(x => x.dataItemInputDetail.ItemModelWarehouseGUID).Distinct().ToList();

                dataItemInput myItemInput = new dataItemInput();

                myItemInput.ItemInputGUID = Guid.NewGuid();
                myItemInput.SourceGUID = itemInput.SourceGUID;
                myItemInput.WarehouseSiteGUID = requesterNameGUID;
                myItemInput.ItemMovementTypeGUID = itemInput.ItemMovementTypeGUID;
                myItemInput.BillNumber = itemInput.BillNumber;
                myItemInput.InputDate = ExecutionTime;
                myItemInput.CreatedByGUID = UserGUID;
                myItemInput.CreatedDate = ExecutionTime;
                DbWMS.Create(myItemInput, Permissions.WarehouseItemsEntry.CreateGuid, ExecutionTime, DbCMS);

                var myWarehouse = DbWMS.codeWarehouseFocalPoint.Where(x => x.WarehouseGUID == requesterNameGUID).FirstOrDefault();
                foreach (var item in itemModelGUIDs)
                {

                    Guid NewDetailEntityPK = Guid.NewGuid();
                    var myModel = DbWMS.codeItemModelWarehouse.Where(x => x.ItemModelWarehouseGUID == item).FirstOrDefault();
                    var myItemModelWarehouse = DbWMS.codeItemModelWarehouse.Where(x => x.WarehouseItemModelGUID == myModel.WarehouseItemModelGUID
                     && x.WarehouseGUID == requesterNameGUID).FirstOrDefault();

                    var reqQuantity = model.Where(x => x.dataItemInputDetail.ItemModelWarehouseGUID == item).Select(x => x.RequestedQunatity).Sum();
                    dataItemInputDetail myInputDetail = new dataItemInputDetail
                    {
                        ItemInputDetailGUID = NewDetailEntityPK,
                        ItemInputGUID = myItemInput.ItemInputGUID,
                        ItemModelWarehouseGUID = myItemModelWarehouse.ItemModelWarehouseGUID,
                        WarehouseOwnerGUID = requesterNameGUID,
                        ItemStatusGUID = WarehouseItemStatus.Functionting,
                        Qunatity = (int)reqQuantity,
                        IsDeterminanted = true,
                        LastWarehouseLocationGUID = myWarehouse.codeWarehouse.WarehouseLocationGUID,
                        CreatedByGUID = UserGUID,
                        CreatedDate = ExecutionTime,



                    };
                    DbWMS.Create(myInputDetail, Permissions.WarehouseItemsEntry.CreateGuid, ExecutionTime, DbCMS);




                    //var myModel = DbWMS.codeItemModelWarehouse.Where(x => x.ItemModelWarehouseGUID == item).FirstOrDefault();
                    int avai = myItemModelWarehouse.TotalAvaiable ?? 0;
                    int Entry = myItemModelWarehouse.TotalEntry ?? 0;
                    myItemModelWarehouse.TotalAvaiable = avai + reqQuantity;
                    myItemModelWarehouse.TotalEntry = Entry + reqQuantity;
                    DbWMS.Update(myItemModelWarehouse, Permissions.WarehouseItemsEntry.UpdateGuid, ExecutionTime, DbCMS);

                    dataItemTransfer TransferModel = new dataItemTransfer();
                    TransferModel.ItemTransferGUID = Guid.NewGuid();
                    TransferModel.ItemInputDetailGUID = NewDetailEntityPK;
                    TransferModel.TransferDate = ExecutionTime;
                    TransferModel.SourceGUID = myWarehouse.WarehouseGUID;
                    TransferModel.DestionationGUID = myWarehouse.WarehouseGUID;

                    TransferModel.TransferedByGUID = UserGUID;
                    TransferModel.IsLastTransfer = true;
                    DbWMS.Create(TransferModel, Permissions.WarehouseItemsEntry.CreateGuid, ExecutionTime, DbCMS);

                }
            }
            try
            {

                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);
                // return RedirectToAction("ConfirmReceivingConsumableBulkModelEmail", new { id = model.Select(x => x.ItemOutputDetailGUID).FirstOrDefault() });
                //return View("Login");

                //return View("ModelConfirmationComplate");
                //return Json(DbWMS.SingleUpdateMessage(DataTableNames.WarehouseModelReleaseMovementsDataTable, DbWMS.PrimaryKeyControl(myModel), DbWMS.RowVersionControls(Portal.SingleToList(myModel))));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }



        }
        #endregion
        #region Mail


        public void SendConfirmationItemEmail(dataItemOutput model)
        {

            string URL = AppSettingsKeys.Domain + "/WMS/ConsumableItem/ConfirmReceivingConsumableBulkModelEmail/?id=" + new Portal().GUIDToString(model.ItemOutputGUID);
            string Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ConfirmItemReceiving + "</a>";
            string Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";
            string standardURL = AppSettingsKeys.Domain + "/WMS/ModelMovements/GetICTSOPFile/";
            string LinkStandard = "<a href='" + standardURL + "' target='_blank'>" + "Syria ICT equipment SOP" + "</a>";
            List<userServiceHistory> users = new List<userServiceHistory>();
            if (model.RequesterGUID == WarehouseConsumableRequestSourceTypes.Warehouse)
            {
                var warehouseFocalPointsGUIDs = DbWMS.codeWarehouseFocalPoint.Where(x => x.WarehouseGUID == model.RequesterNameGUID
                && x.IsFocalPoint == true)
                      .Select(x => x.UserGUID).ToList();
                users = DbCMS.userServiceHistory.Where(x => warehouseFocalPointsGUIDs.Contains(x.UserGUID)).ToList();
            }


            var detail = DbWMS.dataItemOutputDetail.Where(x => x.ItemOutputGUID == model.ItemOutputGUID).ToList();
            var personal = DbWMS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN
                                                                         && x.UserGUID == model.CreatedByGUID).FirstOrDefault();
            string fullName = personal.FirstName + " " + personal.Surname;
            string table = "<br><table class='table table-bordered' style='border-collapse: collapse; width: 900px'><tr><th style='border: 1px solid  #333; vertical-align:middle'>Description (SC)</th><th style='border: 1px solid  #333; vertical-align: middle'>Brand</th><th style='border: 1px solid  #333; vertical-align: middle'>Item</th><th style='border: 1px solid  #333; vertical-align: middle'>Quantity </th><th style='border: 1px solid  #333; vertical-align: middle'>Issue Date</th><th style='border: 1px solid  #333; vertical-align: middle'>Issued By</th></tr>";
            foreach (var item in detail)
            {
                string createdate = "";
                if (item.CreatedDate != null)
                    createdate = item.CreatedDate.ToLongDateString();
                string modelName = item.dataItemInputDetail.codeItemModelWarehouse.codeWarehouseItemModel
              .codeWarehouseItemModelLanguage.Where(x => x.LanguageID == LAN).Select(x => x.ModelDescription)
              .FirstOrDefault();
                string itemName = item.dataItemInputDetail.codeItemModelWarehouse.codeWarehouseItemModel.codeWarehouseItem
                    .codeWarehouseItemLanguage.Where(x => x.LanguageID == LAN).Select(f => f.WarehouseItemDescription).FirstOrDefault();

                string brandName = item.dataItemInputDetail.codeItemModelWarehouse.codeWarehouseItemModel.codeBrand.codeBrandLanguage.Where(x => x.LanguageID == LAN)
                    .Select(f => f.BrandDescription).FirstOrDefault();

                table += "<tr><td style='border: 1px solid  #333; vertical-align: middle'>" + itemName + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + brandName + "</td><td style='border: 1px solid  #333; vertical-align: middle'> " + modelName + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + item.RequestedQunatity + "<td style='border: 1px solid  #333; vertical-align: middle'>" + item.CreatedDate + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + fullName + "</td></tr>";
            }
            table += "</table>";




            //dataItemOutputNotification notification = new dataItemOutputNotification();
            //notification.ItemOutputDetailGUID = model.ItemOutputDetailGUID;
            //notification.NotificationMessage = "Verification Item(1)";
            //notification.NotificationTypeGUID = WarehosueNotificationType.Notification;
            //SendNotification(notification);


            var IssuedUser = DbWMS.userPersonalDetailsLanguage.Where(x => x.UserGUID == UserGUID
                                                                          && x.LanguageID == LAN).FirstOrDefault();

            if (model.RequesterGUID == WarehouseConsumableRequestSourceTypes.Warehouse)
            {
                foreach (var user in users)
                {
                    string _message = resxEmails.warehosueCustodianItemConfirmation
                    .Replace("$FullName", user.userAccounts.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).Select(x => x.FirstName).FirstOrDefault() + " " + user.userAccounts.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).Select(x => x.Surname).FirstOrDefault())
                    .Replace("$VerifyLink", Anchor)
                    .Replace("$Items", table)
                    .Replace("$VerifyStandard", LinkStandard)
                        .Replace("$IssuedUser", "Sent by: " + IssuedUser.FirstName + " " + IssuedUser.Surname);
                    if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                    int isRec = 1;
                    if (user.EmailAddress == "maksoud@unhcr.org")
                        isRec = 0;
                    Send(user.EmailAddress, resxEmails.ItemModelVerificationSubject, _message, isRec);
                }

            }
        }

        public void Send(string recipients, string subject, string body, int? isRec)
        {
            //string copy_recipients = "syrdasti@unhcr.org";
            string copy_recipients = "";
            //if (isRec == 1)
            //{
            //    copy_recipients = "sahhar@unhcr.org;AYDI@unhcr.org;rampersa@unhcr.org;isac@unhcr.org;";
            //}

            string blind_copy_recipients = null;
            string body_format = "HTML";
            string importance = "Normal";
            string file_attachments = null;
            string _body = "<div style='font-family:Arial;'>" + body.Replace("\r\n", "<br/>") + "</div>";
            if (LAN == "AR") { _body = "<p align='right'>" + _body + "</p>"; }
            //DbCMS.SendEmailSTI("maksoud@unhcr.org", copy_recipients, blind_copy_recipients, subject, _body, body_format, importance, file_attachments);
            DbCMS.SendEmailSTI(recipients, copy_recipients, blind_copy_recipients, subject, _body, body_format, importance, file_attachments);
        }
        #endregion
        #region Track Item Movements
        public ActionResult WarehouseConsumableItemMovement(Guid PK)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Update, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }

            ItemInputDetailModel EntryModel = new ItemInputDetailModel();
            EntryModel.ItemModelWarehouseGUID = PK;
            EntryModel.Active = true;

            var myItem = DbWMS.codeItemModelWarehouse.Where(x => x.ItemModelWarehouseGUID == PK).FirstOrDefault();
            EntryModel.TotalAvaiable = myItem.TotalAvaiable;
            ViewBag.TotalAvaiable = myItem.TotalAvaiable;
            ViewBag.ItemName = myItem.codeWarehouseItemModel.codeWarehouseItemModelLanguage.FirstOrDefault(f => f.LanguageID == LAN).ModelDescription;
            return View("~/Areas/WMS/Views/ConsumableItem/ConsumableItemMovements.cshtml", EntryModel);
        }

        public ActionResult WarehouseConsumableItemEntryDetailInfoDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/WMS/Views/ConsumableItemEntryDetail/_ConsumableItemEntryDetailDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<ConsumableItemInputDetailDataTable, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<ConsumableItemInputDetailDataTable>(DataTable.Filters);
            }
            var All = (from a in DbWMS.dataItemInputDetail.AsExpandable().Where(x => x.ItemModelWarehouseGUID == PK)
                       join aa in DbWMS.dataItemInput on a.ItemInputGUID equals aa.ItemInputGUID
                       join b in DbWMS.codeItemModelWarehouse on a.ItemModelWarehouseGUID equals b.ItemModelWarehouseGUID
                       join c in DbWMS.codeWarehouseItemModelLanguage.Where(x => x.LanguageID == LAN) on b.WarehouseItemModelGUID equals c.WarehouseItemModelGUID into LJ1
                       from R1 in LJ1.DefaultIfEmpty()
                       join d in DbWMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN) on aa.SourceGUID equals d.ValueGUID into LJ2
                       from R2 in LJ2.DefaultIfEmpty()
                       select new ConsumableItemInputDetailDataTable
                       {
                           ItemInputDetailGUID = a.ItemInputDetailGUID,
                           ItemInputGUID = a.ItemInputGUID,
                           Qunatity = a.Qunatity,
                           BillNumber = aa.BillNumber,
                           LastWarehouseLocationGUID = a.LastWarehouseLocationGUID,
                           dataItemInputDetailRowVersion = a.dataItemInputDetailRowVersion,
                           ModelDescription = R1.ModelDescription,
                           ItemModelWarehouseGUID = a.ItemModelWarehouseGUID,
                           Active = a.Active,
                           SourceName = R2.ValueDescription,
                           CreatedDate = a.CreatedDate,

                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<ConsumableItemInputDetailDataTable> Result = Mapper.Map<List<ConsumableItemInputDetailDataTable>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult WarehouseConsumableItemReleaseDetailInfoDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/WMS/Views/ConsumableItemReleaseDetail/_ConsumableItemReleaseDetailDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<WarehouseConsumableItemReleaseDetailInfoIDataTable, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<WarehouseConsumableItemReleaseDetailInfoIDataTable>(DataTable.Filters);
            }


            //Access is authorized by Access Action

            var All = (

                from a in DbWMS.dataItemOutputDetail.AsNoTracking().AsExpandable().Where(x => x.Active && x.dataItemInputDetail.ItemModelWarehouseGUID == PK)
                join b in DbWMS.dataItemOutput.Where(x => x.Active) on a.ItemOutputGUID equals b.ItemOutputGUID into LJ1
                from R1 in LJ1.DefaultIfEmpty()
                join c in DbWMS.codeTablesValues.Where(x => x.Active) on R1.RequesterGUID equals c.ValueGUID
                join d in DbWMS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on c.ValueGUID equals d.ValueGUID into LJ2
                from R2 in LJ2.DefaultIfEmpty()
                join e in DbWMS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.ItemRequestTypeGUID equals e.ValueGUID into LJ3
                from R3 in LJ3.DefaultIfEmpty()
                join f in DbWMS.dataItemOutputDetailFlow.Where(x => x.Active && x.IsLastMove == true) on a.ItemOutputDetailGUID equals f.ItemOutputDetailGUID
                join g in DbWMS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on f.FlowTypeGUID equals g.ValueGUID into LJ4
                from R4 in LJ4.DefaultIfEmpty()

                join k in DbWMS.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on R1.RequesterNameGUID equals k.UserGUID into LJ7
                from R7 in LJ7.DefaultIfEmpty()



                join M in DbWMS.codeWarehouseLanguage.Where(x => x.Active && x.LanguageID == LAN) on R1.RequesterNameGUID equals M.WarehouseGUID into LJ8
                from R8 in LJ8.DefaultIfEmpty()

                join N in DbWMS.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.CreatedByGUID equals N.UserGUID into LJ9
                from R9 in LJ9.DefaultIfEmpty()

                join O in DbWMS.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.RetunedByGUID equals O.UserGUID into LJ10
                from R10 in LJ10.DefaultIfEmpty()


                join p in DbWMS.dataItemInputDetail.Where(x => x.Active) on a.ItemInputDetailGUID equals p.ItemInputDetailGUID into LJ11
                from R11 in LJ11.DefaultIfEmpty()

                join r in DbWMS.codeWarehouseRequesterTypeLanguage.Where(x => x.Active && x.LanguageID == LAN) on R1.RequesterNameGUID equals r.WarehouseRequesterTypeGUID into LJ12
                from R12 in LJ12.DefaultIfEmpty()

                join yy in DbWMS.codeWarehouseVehicleLanguage.Where(x => x.Active && x.LanguageID == LAN) on R1.RequesterNameGUID equals yy.WarehouseVehicleGUID into LJ13
                from R13 in LJ13.DefaultIfEmpty()

                join zz in DbWMS.dataItemInputDeterminant.Where(x => x.Active) on R1.RequesterNameGUID equals zz.ItemInputDeterminantGUID into LJ14
                from R14 in LJ14.DefaultIfEmpty()

                select new WarehouseConsumableItemReleaseDetailInfoIDataTable
                {
                    CustodianType = R2.ValueDescription,
                    Custodian = R1.RequesterGUID == WarehouseConsumableRequestSourceTypes.Printers ? R14.DeterminantValue : (R1.RequesterGUID == WarehouseConsumableRequestSourceTypes.Warehouse ? R8.WarehouseDescription : R1.RequesterGUID == WarehouseRequestSourceTypes.OtherRequester ? R12.WarehouseRequesterTypeDescription : (R1.RequesterGUID == WarehouseRequestSourceTypes.Vehicle ? R13.VehicleDescription : null)),
                    ItemOutputDetailGUID = a.ItemOutputDetailGUID,
                    ReleaseType = R3.ValueDescription,
                    Qunatity = a.RequestedQunatity,
                    ExpectedStartDate = a.ExpectedStartDate,
                    ExpectedReturenedDate = a.ExpectedReturenedDate,
                    ActualReturenedDate = a.ActualReturenedDate,
                    DeliveryStatus = R4.ValueDescription,
                    DeliveryActionDate = f.CreatedDate,
                    Active = a.Active,
                    Comments = a.Comments,
                    IssuedBy = R9.FirstName + " " + R9.Surname,
                    IssuedDate = a.CreatedDate,
                    ParentItemModelWarehouseGUID = R11.ParentItemModelWarehouseGUID,
                    //ReturnedBy = R10.FirstName + " " + R10.Surname,
                    //ReturnedDate = a.ReturnedDate,
                    ItemInputGUID = a.dataItemInputDetail.ItemInputGUID,
                    dataItemOutputDetailRowVersion = a.dataItemOutputDetailRowVersion

                }).OrderByDescending(x => x.IssuedDate).Where(Predicate);




            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<WarehouseConsumableItemReleaseDetailInfoIDataTable> Result = Mapper.Map<List<WarehouseConsumableItemReleaseDetailInfoIDataTable>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region Get Item peind

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult WarehouseConsumablePendingConfirmationoDataTableConfirmReceivingConumableBulkItems(List<v_trackItemOutputFlow> models)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }

            var guids = models.Select(f => f.ItemOutputDetailGUID);
            if (guids.Count() <= 0)
            {
                return Json(DbWMS.PermissionError());

            }

            DateTime ExecutionTime = DateTime.Now;
            var model = DbWMS.dataItemOutputDetail.Where(x => guids.Contains(x.ItemOutputDetailGUID)).ToList();
            var itemInputGUID = model.Select(x => x.dataItemInputDetail.ItemInputGUID).FirstOrDefault();
            var itemInput = DbWMS.dataItemInput.Where(x => x.ItemInputGUID == itemInputGUID).FirstOrDefault();

            var requester = model.Select(x => x.dataItemOutput.RequesterGUID).FirstOrDefault();
            var requesterNameGUID = model.Select(x => x.dataItemOutput.RequesterNameGUID).FirstOrDefault();
            List<dataItemOutputDetailFlow> flowDetails = new List<dataItemOutputDetailFlow>();
            foreach (var item in model)
            {
                List<dataItemOutputDetailFlow> flows = DbWMS.dataItemOutputDetailFlow
               .Where(x => x.ItemOutputDetailGUID == item.ItemOutputDetailGUID).ToList();
                foreach (var flow in flows)
                {
                    flow.IsLastAction = false;
                    flow.IsLastMove = false;
                }

                DbWMS.SaveChanges();

                //List<dataItemInputDetail> ItemInputs = DbWMS.dataItemInputDetail
                //    .Where(x => x.ParentItemModelWarehouseGUID == myoutputDetail.ItemInputDetailGUID).ToList();

                int? order = DbWMS.dataItemOutputDetailFlow
                                 .Where(x => x.dataItemOutputDetail.ItemInputDetailGUID == item.ItemInputDetailGUID)
                                 .Select(x => x.OrderId).Max() + 1;
                if (order == null)
                    order = 1;
                //DbWMS.Up(flows, Permissions.WarehouseItemsEntry.UpdateGuid, ExecutionTime, DbCMS);
                dataItemOutputDetailFlow flowDetail = new dataItemOutputDetailFlow();
                flowDetail.ItemOutputDetailFlowGUID = Guid.NewGuid();
                flowDetail.FlowTypeGUID = WarehouseRequestFlowType.Confirmed;
                flowDetail.ItemOutputDetailGUID = item.ItemOutputDetailGUID;

                flowDetail.CreatedDate = ExecutionTime;
                flowDetail.ItemStatuGUID = WarehouseItemStatus.Functionting;
                flowDetail.IsLastAction = true;
                flowDetail.IsLastMove = true;
                flowDetail.CreatedByGUID = UserGUID;
                flowDetail.Active = true;
                flowDetail.OrderId = order;
                //flowDetails.Add(flowDetail);
                DbWMS.CreateNoAudit(flowDetail);
                var input = DbWMS.dataItemInputDetail.Find(item.ItemInputDetailGUID);
                input.LastFlowTypeGUID = WarehouseRequestFlowType.Confirmed;
                DbWMS.UpdateNoAudit(input);


            }

            if (requester == WarehouseConsumableRequestSourceTypes.Warehouse)
            {
                var itemModelGUIDs = model.Select(x => x.dataItemInputDetail.ItemModelWarehouseGUID).Distinct().ToList();

                dataItemInput myItemInput = new dataItemInput();

                myItemInput.ItemInputGUID = Guid.NewGuid();
                myItemInput.SourceGUID = itemInput.SourceGUID;
                myItemInput.WarehouseSiteGUID = requesterNameGUID;
                myItemInput.ItemMovementTypeGUID = itemInput.ItemMovementTypeGUID;
                myItemInput.BillNumber = itemInput.BillNumber;
                myItemInput.InputDate = ExecutionTime;
                myItemInput.CreatedByGUID = UserGUID;
                myItemInput.CreatedDate = ExecutionTime;
                DbWMS.Create(myItemInput, Permissions.WarehouseItemsEntry.CreateGuid, ExecutionTime, DbCMS);

                var myWarehouse = DbWMS.codeWarehouseFocalPoint.Where(x => x.WarehouseGUID == requesterNameGUID).FirstOrDefault();
                foreach (var item in itemModelGUIDs)
                {

                    Guid NewDetailEntityPK = Guid.NewGuid();
                    var reqQuantity = model.Where(x => x.dataItemInputDetail.ItemModelWarehouseGUID == item).Select(x => x.RequestedQunatity).Sum();
                    var myModel = DbWMS.codeItemModelWarehouse.Where(x => x.ItemModelWarehouseGUID == item).FirstOrDefault();
                    var myItemModelWarehouse = DbWMS.codeItemModelWarehouse.Where(x => x.WarehouseItemModelGUID == myModel.WarehouseItemModelGUID
                     && x.WarehouseGUID == requesterNameGUID).FirstOrDefault();

                    dataItemInputDetail myInputDetail = new dataItemInputDetail
                    {
                        ItemInputDetailGUID = NewDetailEntityPK,
                        ItemInputGUID = myItemInput.ItemInputGUID,
                        ItemModelWarehouseGUID = myItemModelWarehouse.ItemModelWarehouseGUID,
                        WarehouseOwnerGUID = requesterNameGUID,
                        ItemStatusGUID = WarehouseItemStatus.Functionting,
                        Qunatity = (int)reqQuantity,
                        IsDeterminanted = true,
                        LastWarehouseLocationGUID = myWarehouse.codeWarehouse.WarehouseLocationGUID,
                        CreatedByGUID = UserGUID,
                        CreatedDate = ExecutionTime,



                    };
                    DbWMS.Create(myInputDetail, Permissions.WarehouseItemsEntry.CreateGuid, ExecutionTime, DbCMS);




                    //var myModel = DbWMS.codeItemModelWarehouse.Where(x => x.ItemModelWarehouseGUID == item).FirstOrDefault();
                    int avai = myItemModelWarehouse.TotalAvaiable ?? 0;
                    int Entry = myItemModelWarehouse.TotalEntry ?? 0;
                    myItemModelWarehouse.TotalAvaiable = avai + reqQuantity;
                    myItemModelWarehouse.TotalEntry = Entry + reqQuantity;
                    DbWMS.Update(myItemModelWarehouse, Permissions.WarehouseItemsEntry.UpdateGuid, ExecutionTime, DbCMS);

                    dataItemTransfer TransferModel = new dataItemTransfer();
                    TransferModel.ItemTransferGUID = Guid.NewGuid();
                    TransferModel.ItemInputDetailGUID = NewDetailEntityPK;
                    TransferModel.TransferDate = ExecutionTime;
                    TransferModel.SourceGUID = myWarehouse.WarehouseGUID;
                    TransferModel.DestionationGUID = myWarehouse.WarehouseGUID;

                    TransferModel.TransferedByGUID = UserGUID;
                    TransferModel.IsLastTransfer = true;
                    DbWMS.Create(TransferModel, Permissions.WarehouseItemsEntry.CreateGuid, ExecutionTime, DbCMS);

                }
            }
            try
            {
                var EntryModel = DbWMS.dataItemOutputDetail.Where(x => guids.Contains(x.ItemOutputDetailGUID)).FirstOrDefault();
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleUpdateMessage(DataTableNames.WarehouseConsumablePendingConfirmationoDataTable, DbWMS.PrimaryKeyControl(EntryModel), DbWMS.RowVersionControls(Portal.SingleToList(EntryModel))));
                return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);
                // return RedirectToAction("ConfirmReceivingConsumableBulkModelEmail", new { id = model.Select(x => x.ItemOutputDetailGUID).FirstOrDefault() });
                //return View("Login");

                //return View("ModelConfirmationComplate");
                //return Json(DbWMS.SingleUpdateMessage(DataTableNames.WarehouseModelReleaseMovementsDataTable, DbWMS.PrimaryKeyControl(myModel), DbWMS.RowVersionControls(Portal.SingleToList(myModel))));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }

        }

        #endregion
        #region Consumable Entry
        public ActionResult ConsumableSingleItemReleaseDetailCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }

            return PartialView("~/Areas/WMS/Views/ConsumableItemReleaseDetail/_ConsumableSingleItemReleaseDetailForm.cshtml", new ConsumableSingleReleaseModelDetailUpdateModel { ItemModelWarehouseGUID = FK });

        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ConsumableSingleItemReleaseDetailCreate(ConsumableSingleReleaseModelDetailUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (!ModelState.IsValid) return PartialView("~/Areas/WMS/Views/ConsumableItemReleaseDetail/_ConsumableItemReleaseDetailForm.cshtml", model);
            var codeModel = DbWMS.codeItemModelWarehouse.Where(x => x.ItemModelWarehouseGUID == model.ItemModelWarehouseGUID).FirstOrDefault();
            if (model.WarehouseLocationGUID == null || model.ItemModelWarehouseGUID == null || (model.RequestedQunatity == null || model.RequestedQunatity == 0 || model.RequestedQunatity > codeModel.TotalAvaiable))
            {
                return PartialView("~/Areas/WMS/Views/ConsumableItemReleaseDetail/_ConsumableItemReleaseDetailForm.cshtml", model);
            }
            DateTime ExecutionTime = DateTime.Now;

            Guid EntityOutPK = Guid.NewGuid();

            dataItemOutput myModel = Mapper.Map(model, new dataItemOutput());
            int outputNumber = DbWMS.dataItemOutput.Select(x => x.OutputNumber).Max() + 1 ?? 1;
            var myWarehouse = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
            myModel.ItemOutputGUID = EntityOutPK;
            myModel.CreatedDate = ExecutionTime;
            myModel.RequesterGUID = model.RequesterGUID;
            myModel.RequesterNameGUID = model.RequesterNameGUID;

            myModel.CreatedByGUID = UserGUID;
            myModel.WarehouseGUID = myWarehouse.WarehouseGUID;
            myModel.OutputNumber = outputNumber;

            DbWMS.Create(myModel, Permissions.WarehouseItemsRelease.CreateGuid, ExecutionTime, DbCMS);



            //var itemoutput = DbWMS.dataItemOutput.Where(x => x.ItemOutputGUID == model.ItemOutputGUID).FirstOrDefault();
            Guid EntityPK = Guid.NewGuid();
            var inputDetail = DbWMS.dataItemInputDetail.Where(x => x.ItemModelWarehouseGUID == model.ItemModelWarehouseGUID).FirstOrDefault();

            dataItemOutputDetail detail = Mapper.Map(model, new dataItemOutputDetail());

            detail.ItemOutputDetailGUID = EntityPK;
            detail.ItemOutputGUID = EntityOutPK;
            detail.CreatedDate = ExecutionTime;
            detail.CreatedByGUID = UserGUID;
            detail.ItemInputDetailGUID = inputDetail.ItemInputDetailGUID;
            if (model.RequestedQunatity == null)
                detail.RequestedQunatity = 1;



            var myWarhouseModel = DbWMS.codeItemModelWarehouse.Where(x =>
            x.ItemModelWarehouseGUID == model.ItemModelWarehouseGUID).FirstOrDefault();
            var currentRelease = myWarhouseModel.TotalRelease ?? 0;
            myWarhouseModel.TotalAvaiable = myWarhouseModel.TotalAvaiable - model.RequestedQunatity;
            myWarhouseModel.TotalRelease = currentRelease + model.RequestedQunatity;

            //var output = DbWMS.dataItemOutput.Where(x => x.ItemOutputGUID == model.ItemOutputGUID).FirstOrDefault();

            int? myOrder = DbWMS.dataItemOutputDetailFlow
                            .Where(x => x.dataItemOutputDetail.ItemOutputGUID == model.ItemOutputGUID)
                            .Select(x => x.OrderId).Max() + 1;
            if (myOrder == null)
                myOrder = 1;
            dataItemOutputDetailFlow myoutputDetailFlow = new dataItemOutputDetailFlow();
            myoutputDetailFlow.ItemOutputDetailFlowGUID = Guid.NewGuid();
            myoutputDetailFlow.ItemOutputDetailGUID = EntityPK;
            if (model.RequesterGUID == WarehouseConsumableRequestSourceTypes.Warehouse)
            {
                myoutputDetailFlow.FlowTypeGUID = WarehouseRequestFlowType.PendingConfirmed;
                //myWarhouseModel.FlowTypeGUID = WarehouseRequestFlowType.PendingConfirmed;
            }
            else
            {
                myoutputDetailFlow.FlowTypeGUID = WarehouseRequestFlowType.Confirmed;
                //myWarhouseModel.FlowTypeGUID = WarehouseRequestFlowType.Confirmed;

            }
            myoutputDetailFlow.ItemStatuGUID = WarehouseItemStatus.Functionting;
            myoutputDetailFlow.CreatedDate = DateTime.Now;
            myoutputDetailFlow.IsLastAction = true;
            myoutputDetailFlow.IsLastMove = true;
            myoutputDetailFlow.CreatedByGUID = UserGUID;
            myoutputDetailFlow.Active = true;
            myoutputDetailFlow.OrderId = myOrder;
            DbWMS.Create(detail, Permissions.WarehouseItemsRelease.CreateGuid, ExecutionTime, DbCMS);
            DbWMS.Create(myoutputDetailFlow, Permissions.WarehouseItemsRelease.CreateGuid, ExecutionTime, DbCMS);

            DbWMS.Update(myWarhouseModel, Permissions.WarehouseItemsRelease.UpdateGuid, ExecutionTime, DbCMS);
            //ConsumableReleaseModelDetailUpdateModel myVM = Mapper.Map(detail, new ConsumableReleaseModelDetailUpdateModel());
            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                if (model.RequesterGUID == WarehouseConsumableRequestSourceTypes.Warehouse)
                {
                    var itemoutput = DbWMS.dataItemOutput.Where(x => x.ItemOutputGUID == EntityOutPK).FirstOrDefault();
                    SendConfirmationItemEmail(itemoutput);
                }
                return Json(DbWMS.SingleUpdateMessage(DataTableNames.WarehouseConsumableItemReleaseDetailInfoDataTable, DbWMS.PrimaryKeyControl(detail), DbWMS.RowVersionControls(Portal.SingleToList(detail))));

            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        public ActionResult WarehouseConsumableSingleItemEntryCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }

            var dataItemInputDetail = DbWMS.dataItemInputDetail.Find(FK);

            EntryModelSingleConsumableUpdateModel model = new EntryModelSingleConsumableUpdateModel
            {

                ItemModelWarehouseGUID = FK

            };

            return PartialView("~/Areas/WMS/Views/ConsumableItemEntryDetail/_ConsumableSingleItemEntryDetailForm.cshtml",
                model);
        }



        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ConsumableSingleItemEntryDetailCreate(EntryModelSingleConsumableUpdateModel model)
        {

            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (!ModelState.IsValid) return PartialView("~/Areas/WMS/Views/ConsumableItemEntryDetail/_ConsumableItemEntryDetailForm.cshtml", model);
            if (model.Qunatity <= 0 || model.ItemModelWarehouseGUID == null)
            {
                return PartialView("~/Areas/WMS/Views/ConsumableItemEntryDetail/_ConsumableItemEntryDetailForm.cshtml", model);
            }
            DateTime ExecutionTime = DateTime.Now;


            Guid EntityPKInput = Guid.NewGuid();

            dataItemInput ItemInput = Mapper.Map(model, new dataItemInput());
            var myWarehouse = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
            ItemInput.ItemInputGUID = EntityPKInput;
            ItemInput.CreatedDate = ExecutionTime;
            ItemInput.CreatedByGUID = UserGUID;
            ItemInput.SourceGUID = model.SourceGUID;
            ItemInput.WarehouseSiteGUID = myWarehouse.WarehouseGUID;

            DbWMS.Create(ItemInput, Permissions.WarehouseItemsEntry.CreateGuid, ExecutionTime, DbCMS);
            Guid EntityPK = Guid.NewGuid();
            dataItemInputDetail ItemInputdetail = Mapper.Map(model, new dataItemInputDetail());

            ItemInputdetail.ItemInputDetailGUID = EntityPK;
            ItemInputdetail.CreatedDate = ExecutionTime;
            ItemInputdetail.ItemInputGUID = EntityPKInput;
            ItemInputdetail.CreatedByGUID = UserGUID;
            ItemInputdetail.ItemStatusGUID = WarehouseItemStatus.Functionting;
            ItemInputdetail.WarehouseOwnerGUID = myWarehouse.WarehouseGUID;

            DbWMS.Create(ItemInputdetail, Permissions.WarehouseItemsEntry.CreateGuid, ExecutionTime, DbCMS);
            var myModel = DbWMS.codeItemModelWarehouse.Where(x => x.ItemModelWarehouseGUID == model.ItemModelWarehouseGUID).FirstOrDefault();
            int avai = myModel.TotalAvaiable ?? 0;
            int Entry = myModel.TotalEntry ?? 0;
            myModel.TotalAvaiable = avai + model.Qunatity;
            myModel.TotalEntry = Entry + model.Qunatity;
            DbWMS.Update(myModel, Permissions.WarehouseItemsEntry.UpdateGuid, ExecutionTime, DbCMS);

            dataItemTransfer TransferModel = new dataItemTransfer();
            TransferModel.ItemTransferGUID = Guid.NewGuid();
            TransferModel.ItemInputDetailGUID = ItemInputdetail.ItemInputDetailGUID;
            TransferModel.TransferDate = ExecutionTime;
            TransferModel.SourceGUID = myWarehouse.WarehouseGUID;
            TransferModel.DestionationGUID = myWarehouse.WarehouseGUID;

            TransferModel.TransferedByGUID = UserGUID;
            TransferModel.IsLastTransfer = true;
            DbWMS.Create(TransferModel, Permissions.WarehouseItemsEntry.CreateGuid, ExecutionTime, DbCMS);


            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleUpdateMessage(DataTableNames.WarehouseConsumableItemEntryDetailInfoDataTable, DbWMS.PrimaryKeyControl(ItemInputdetail), DbWMS.RowVersionControls(Portal.SingleToList(ItemInputdetail))));

            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }
        #endregion
    }
}