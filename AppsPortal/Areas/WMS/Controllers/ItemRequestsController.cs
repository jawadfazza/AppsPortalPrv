using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls.WebParts;
using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;


using AppsPortal.ViewModels;
using AutoMapper;
using LinqKit;
using Microsoft.Ajax.Utilities;
using WMS_DAL.Model;
using WMS_DAL.ViewModels;

namespace AppsPortal.Areas.WMS.Controllers
{
    public class ItemRequestsController : WMSBaseController
    {

        #region Item Requests

        [Route("WMS/ItemRequests/")]
        public ActionResult ItemRequestIndex()
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            return View("~/Areas/WMS/Views/ItemRequests/Index.cshtml");
        }

        [Route("WMS/WarehouseItemRequestsDataTable/")]
        public JsonResult WarehouseItemRequestsDataTable(DataTableRecievedOptions options)
        {

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<ItemRequestDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<ItemRequestDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.WarehouseItemsEntry.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();
            var All = (

                from a in DbWMS.dataItemRequest.AsNoTracking().AsExpandable().Where(x=>x.Active)
                join b in DbWMS.codeTablesValues.Where(x => x.Active) on a.RequesterGUID equals b.ValueGUID
                join c in DbWMS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on b.ValueGUID equals c.ValueGUID into LJ1
                from R1 in LJ1.DefaultIfEmpty()

                join d in DbWMS.codeTablesValues.Where(x => x.Active) on a.RequestTypeGUID equals d.ValueGUID
                join e in DbWMS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on d.ValueGUID equals e.ValueGUID into LJ2
                from R2 in LJ2.DefaultIfEmpty()

                join f in DbWMS.codeTablesValues.Where(x => x.Active) on a.LastFlowStatusGUID equals f.ValueGUID
                join g in DbWMS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on f.ValueGUID equals g.ValueGUID into LJ3
                from R3 in LJ3.DefaultIfEmpty()
                join h in DbWMS.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID==LAN) on a.CreatedByGUID equals h.UserGUID into LJ4
               
                from R4 in LJ4.DefaultIfEmpty()

                select new ItemRequestDataTableModel
                {
                    ItemRequestGUID = a.ItemRequestGUID,
                    Active = a.Active,
                    Requester=R1.ValueDescription,
                    RequesterName="",
                    RequestType=R2.ValueDescription,
                    RequestStartDate=a.RequestStartDate,
                    RequestEndDate = a.RequestEndDate,
                    RequestStatus = R3.ValueDescription,
                    Comments=a.Comments,
                    CreatedBy=R4.FirstName+" "+R4.Surname,
                    CreatedDate=a.CreatedDate,
                    


                    //ModelDescription = R1.ModelDescription,
                    //ItemDescription = R2.WarehouseItemDescription,
                    //BrandDescription = R3.BrandDescription,
                    dataItemRequestRowVersion = a.dataItemRequestRowVersion
                }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<ItemRequestDataTableModel> Result = Mapper.Map<List<ItemRequestDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("WMS/ItemRequests/Create/")]
        public ActionResult ItemRequestCreate()
        {
            //if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Create, Apps.WMS))
            //{
            //    return Json(DbWMS.PermissionError());
            //}
            return View("~/Areas/WMS/Views/ItemRequests/ItemRequest.cshtml", new ItemRequestUpdateModel());
        }

       // [Route("WMS/ItemRequests/Update/{PK}")]
        public ActionResult ItemRequestUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            var model = (from a in DbWMS.dataItemRequest.WherePK(PK)
                         //join b in DbWMS.dataItemRequestLanguage.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.dataItemRequest.DeletedOn) && x.LanguageID == LAN)
                         //on a.ItemRequestGUID equals b.ItemRequestGUID into LJ1
                         //from R1 in LJ1.DefaultIfEmpty()
                         select new ItemRequestUpdateModel
                         {
                             ItemRequestGUID = a.ItemRequestGUID,
                             RequesterGUID = a.RequesterGUID,
                             RequesterNameGUID = (Guid)a.RequesterNameGUID,
                             RequestTypeGUID = a.RequestTypeGUID,
                             RequestStartDate= a.RequestStartDate,
                             RequestEndDate = a.RequestEndDate,
                             Comments = a.Comments,
                             //BrandGUID = a.BrandGUID,
                             //WarehouseItemGUID = a.WarehouseItemGUID,
                             //ModelDescription = R1.ModelDescription,
                             Active = a.Active,
                             dataItemRequestRowVersion = a.dataItemRequestRowVersion,
                             //dataItemRequestLanguageRowVersion = R1.dataItemRequestLanguageRowVersion
                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("ItemRequests", "ItemRequests", new { Area = "WMS" }));

            return View("~/Areas/WMS/Views/ItemRequests/ItemRequest.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ItemRequestCreate(ItemRequestUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            DateTime ExecutionTime = DateTime.Now;
            dataItemRequest ItemRequest = Mapper.Map(model, new dataItemRequest());
            ItemRequest.CreatedDate = ExecutionTime;
            if ( ActiveItemRequest(ItemRequest)) return PartialView("~/Areas/WMS/Views/ItemRequests/_ItemRequestForm.cshtml", model);

        

            Guid EntityPK = Guid.NewGuid();
        

          
            ItemRequest.ItemRequestGUID = EntityPK;
            ItemRequest.CreatedByGUID = UserGUID;
            ItemRequest.CreatedDate = ExecutionTime;
            ItemRequest.LastFlowStatusGUID = WarehouseRequestFlowType.Requested;
            DbWMS.Create(ItemRequest, Permissions.WarehouseItemsEntry.CreateGuid, ExecutionTime, DbCMS);

            dataItemRequestFlow itemRequestFlow = new dataItemRequestFlow();

            itemRequestFlow.ItemRequestFlowGUID = Guid.NewGuid();
            itemRequestFlow.ItemRequestGUID = ItemRequest.ItemRequestGUID;
            itemRequestFlow.FlowStatusGUID = WarehouseRequestFlowType.Requested;
          
            itemRequestFlow.UserGUID =UserGUID;
            itemRequestFlow.IsLastAction = true;
    
            itemRequestFlow.CreatedByGUID = UserGUID;
            itemRequestFlow.Active = true;
            itemRequestFlow.CreatedDate = ExecutionTime;


            DbWMS.Create(itemRequestFlow, Permissions.WarehouseItemsEntry.CreateGuid, ExecutionTime, DbCMS);

            //dataItemRequestLanguage Language = Mapper.Map(model, new dataItemRequestLanguage());
            //Language.ItemRequestGUID = EntityPK;

            //DbWMS.Create(Language, Permissions.WarehouseItemsEntry.CreateGuid, ExecutionTime, DbCMS);

            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.WarehouseItemRequestsDataTable, ControllerContext, "ItemRequestDetailsFormControls"));
            //Partials.Add(Portal.PartialView(EntityPK, DataTableNames.ItemRequestDeterminantsDataTable, ControllerContext, "ModelsDeterminantsFormControls"));
            //Partials.Add(Portal.PartialView(EntityPK, DataTableNames.ItemRequestWarehouseDataTable, ControllerContext, "ModelsWarehousesFormControls"));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.WarehouseItemsEntry.Create, Apps.WMS, new UrlHelper(Request.RequestContext).Action("Create", "ItemRequests", new { Area = "WMS" })), Container = "ItemRequestDetailFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.WarehouseItemsEntry.Update, Apps.WMS), Container = "ItemRequestDetailFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.WarehouseItemsEntry.Delete, Apps.WMS), Container = "ItemRequestDetailFormControls" });

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
               // return Json(DbWMS.SingleCreateMessage(DbWMS.PrimaryKeyControl(ItemRequest), DbWMS.RowVersionControls(ItemRequest, Language), Partials, "", UIButtons));

                return Json(DbWMS.SingleCreateMessage(DbWMS.PrimaryKeyControl(ItemRequest), null, Partials, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ItemRequestUpdate(ItemRequestUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Update, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (!ModelState.IsValid || ActiveItemRequest(model)) return PartialView("~/Areas/WMS/Views/ItemRequests/_ItemRequestForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            dataItemRequest ItemRequest = Mapper.Map(model, new dataItemRequest());
            DbWMS.Update(ItemRequest, Permissions.WarehouseItemsEntry.UpdateGuid, ExecutionTime, DbCMS);

            //var Language = DbWMS.dataItemRequestLanguage.Where(l => l.ItemRequestGUID == model.ItemRequestGUID && l.LanguageID == LAN && l.Active).FirstOrDefault();

            //if (Language == null)
            //{
            //    Language = Mapper.Map(model, Language);
            //    Language.ItemRequestGUID = ItemRequest.ItemRequestGUID;
            //    DbWMS.Create(Language, Permissions.WarehouseItemsEntry.CreateGuid, ExecutionTime, DbCMS);
            //}
            //else if (Language.ModelDescription != model.ModelDescription)
            //{
            //    Language = Mapper.Map(model, Language);
            //    DbWMS.Update(Language, Permissions.WarehouseItemsEntry.UpdateGuid, ExecutionTime, DbCMS);
            //}

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleUpdateMessage(null, null, DbWMS.RowVersionControls(Portal.SingleToList(ItemRequest))));
              
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyItemRequest(model.ItemRequestGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ItemRequestDelete(dataItemRequest model)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Delete, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            List<dataItemRequest> DeletedItemRequest = DeleteItemRequest(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.WarehouseItemsEntry.Restore, Apps.WMS), Container = "ItemRequestFormControls" });

            try
            {
                int CommitedRows = DbWMS.SaveChanges();
                DbWMS.SaveChanges();
                return Json(DbWMS.SingleDeleteMessage(CommitedRows, DeletedItemRequest.FirstOrDefault(), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyItemRequest(model.ItemRequestGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ItemRequestRestore(dataItemRequest model)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Restore, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (ActiveItemRequest(model))
            {
                return Json(DbWMS.RecordExists());
            }

            List<dataItemRequest> RestoredItemRequest = RestoreItemRequest(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.WarehouseItemsEntry.Create, Apps.WMS, new UrlHelper(Request.RequestContext).Action("ItemRequestCreate", "Configuration", new { Area = "WMS" })), Container = "ItemRequestFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.WarehouseItemsEntry.Update, Apps.WMS), Container = "ItemRequestFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.WarehouseItemsEntry.Delete, Apps.WMS), Container = "ItemRequestFormControls" });

            try
            {
                int CommitedRows = DbWMS.SaveChanges();
                DbWMS.SaveChanges();
                return Json(DbWMS.SingleRestoreMessage(CommitedRows, RestoredItemRequest, DbWMS.PrimaryKeyControl(RestoredItemRequest.FirstOrDefault()), Url.Action(DataTableNames.WarehouseItemRequestsDataTable, Portal.GetControllerName(ControllerContext)), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyItemRequest(model.ItemRequestGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
      
        public JsonResult WarehouseItemRequestsDataTableDelete(List<dataItemRequest> models)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Delete, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            List<dataItemRequest> DeletedItemRequest = DeleteItemRequest(models);

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.PartialDeleteMessage(DeletedItemRequest, models, DataTableNames.WarehouseItemRequestsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ItemRequestDataTableRestore(List<dataItemRequest> models)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Restore, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            List<dataItemRequest> RestoredItemRequest = RestoreItemRequest(models);

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.PartialRestoreMessage(RestoredItemRequest, models, DataTableNames.WarehouseItemRequestsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        private List<dataItemRequest> DeleteItemRequest(List<dataItemRequest> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<dataItemRequest> DeletedItemRequest = new List<dataItemRequest>();

            //Select the table and all the factors from other tables into one query.
            string query = DbWMS.QueryBuilder(models, Permissions.WarehouseItemsEntry.DeleteGuid, SubmitTypes.Delete, "");
     

            var Records = DbWMS.Database.SqlQuery<dataItemRequest>(query).ToList();
            foreach (var record in Records)
            {
                DeletedItemRequest.Add(DbWMS.Delete(record, ExecutionTime, Permissions.WarehouseItemsEntry.DeleteGuid, DbCMS));
            }

            //var Languages = DeletedItemRequest.SelectMany(a => a.dataItemRequestLanguage).Where(l => l.Active).ToList();
            //foreach (var language in Languages)
            //{
            //    DbWMS.Delete(language, ExecutionTime, Permissions.WarehouseItemsEntry.DeleteGuid, DbCMS);
            //}
            return DeletedItemRequest;
        }

        private List<dataItemRequest> RestoreItemRequest(List<dataItemRequest> models)
        {

            DateTime RestoringTime = DateTime.Now;

            List<dataItemRequest> RestoredItemRequest = new List<dataItemRequest>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbWMS.QueryBuilder(models, Permissions.WarehouseItemsEntry.DeleteGuid, SubmitTypes.Restore, "");

            var Records = DbWMS.Database.SqlQuery<dataItemRequest>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveItemRequest(record))
                {
                    RestoredItemRequest.Add(DbWMS.Restore(record, Permissions.WarehouseItemsEntry.DeleteGuid, Permissions.WarehouseItemsEntry.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            //var Languages = RestoredItemRequest.SelectMany(x => x.dataItemRequestLanguage.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            //foreach (var language in Languages)
            //{
            //    DbWMS.Restore(language, Permissions.WarehouseItemsEntry.DeleteGuid, Permissions.WarehouseItemsEntry.RestoreGuid, RestoringTime, DbCMS);
            //}

            return RestoredItemRequest;
        }

        private JsonResult ConcurrencyItemRequest(Guid PK)
        {
            ItemRequestUpdateModel dbModel = new ItemRequestUpdateModel();

            var ItemRequest = DbWMS.dataItemRequest.Where(x => x.ItemRequestGUID == PK).FirstOrDefault();
            var dbItemRequest = DbWMS.Entry(ItemRequest).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbItemRequest, dbModel);

            //var Language = DbWMS.dataItemRequestLanguage.Where(x => x.ItemRequestGUID == PK).Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.dataItemRequest.DeletedOn) && x.LanguageID == LAN).FirstOrDefault();
            //var dbLanguage = DbWMS.Entry(Language).GetDatabaseValues().ToObject();
            //dbModel = Mapper.Map(dbLanguage, dbModel);

            //if (ItemRequest.dataItemRequestRowVersion.SequenceEqual(dbModel.dataItemRequestRowVersion) && Language.dataItemRequestLanguageRowVersion.SequenceEqual(dbModel.dataItemRequestLanguageRowVersion))
            //{
            //    return Json(DbWMS.PermissionError());
            //}

            return Json(JsonMessages.ConcurrencyError(DbWMS, dbModel, "LanguagesContainer"));
        }

        private bool ActiveItemRequest(Object model)
        {
            dataItemRequest ItemRequest = Mapper.Map(model, new dataItemRequest());
            int ModelDescription = DbWMS.dataItemRequest
                                    .Where(x => x.RequesterNameGUID == ItemRequest.RequesterNameGUID &&
                                                x.RequesterGUID == ItemRequest.RequesterGUID &&
                                                x.CreatedDate==x.CreatedDate &&
                                                x.RequestStartDate==x.RequestStartDate &&
                                                x.RequestEndDate == x.RequestEndDate &&
                                                x.Active).Count();
            if (ModelDescription > 0)
            {
                ModelState.AddModelError("ModelDescription", "Item Request is already exists");
            }
            return (ModelDescription > 0);
        }

        #endregion


        #region Request Details

       //[Route("WMS/WarehouseItemRequestDetailsDataTable/{PK}")]

        public ActionResult WarehouseItemRequestDetailsDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/WMS/Views/ItemRequestDetails/_ItemRequestDetailsDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<ItemRequestDetailDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<ItemRequestDetailDataTableModel>(DataTable.Filters);
            }


            var Result = DbWMS.dataItemRequestDetail.AsNoTracking().AsExpandable().Where(x => x.ItemRequestGUID == PK)
                .Select(x => new ItemRequestDetailDataTableModel
                {
                    ItemRequestDetailGUID = x.ItemRequestDetailGUID,
                    ItemRequestGUID = x.ItemRequestGUID,
                    RequestedItem= x.codeWarehouseItem.codeWarehouseItemLanguage.FirstOrDefault(c=>c.LanguageID==LAN ).WarehouseItemDescription,
                    QuantityOrdered=x.QuantityOrdered,
                    Comments = x.Comments,
                    dataItemRequestDetailRowVersion = x.dataItemRequestDetailRowVersion,
                    Active = x.Active
                }).Where(Predicate);

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

 

        public ActionResult WarehouseItemRequestDetailCreate(Guid FK)
        {
            //if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Create, Apps.WMS))
            //{
            //    return Json(DbWMS.PermissionError());
            //}
            return PartialView("~/Areas/WMS/Views/ItemRequestDetails/_ItemRequestDetailUpdateModal.cshtml",
                new ItemRequestDetailUpdateModel { ItemRequestGUID = FK, ItemRequestDetailGUID = Guid.Empty });
        }

        public ActionResult WarehouseItemRequestDetailUpdate(Guid PK)
        {
            //if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Access, Apps.WMS))
            //{
            //    return Json(DbWMS.PermissionError());
            //}


            ItemRequestDetailUpdateModel model = Mapper.Map(DbWMS.dataItemRequestDetail.Find(PK), new ItemRequestDetailUpdateModel());
            return PartialView("~/Areas/WMS/Views/ItemRequestDetails/_ItemRequestDetailUpdateModal.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult WarehouseItemRequestDetailCreate(ItemRequestDetailUpdateModel model)
        {
            //if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Create, Apps.WMS))
            //{
            //    return Json(DbWMS.PermissionError());
            //}
            dataItemRequestDetail ItemRequestDetail = Mapper.Map(model, new dataItemRequestDetail());
            if (ActiveItemRequestDetail(ItemRequestDetail)) return PartialView("~/Areas/WMS/Views/ItemRequestDetails/_ItemRequestDetailUpdateModal.cshtml", model);
            //!ModelState.IsValid ||
            DateTime ExecutionTime = DateTime.Now;
            ItemRequestDetail.ItemRequestDetailGUID = Guid.NewGuid();



            DbWMS.Create(ItemRequestDetail, Permissions.WarehouseItemsRelease.CreateGuid, ExecutionTime, DbCMS);



     



            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleUpdateMessage(DataTableNames.WarehouseItemRequestDetailsDataTable, DbWMS.PrimaryKeyControl(ItemRequestDetail), DbWMS.RowVersionControls(Portal.SingleToList(ItemRequestDetail))));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult WarehouseItemRequestDetailUpdate(dataItemRequestDetail model)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Update, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (!ModelState.IsValid || ActiveItemRequestDetail(model)) return PartialView("~/Areas/WMS/Views/ItemRequestDetails/_ItemRequestDetailUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbWMS.Update(model, Permissions.WarehouseItemsRelease.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleUpdateMessage(DataTableNames.WarehouseItemRequestDetailsDataTable,
                    DbWMS.PrimaryKeyControl(model),
                    DbWMS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyItemRequestDetail(model.ItemRequestDetailGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult WarehouseItemRequestDetailDelete(ItemRequestDetailUpdateModel model)
        {
            //if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Delete, Apps.WMS))
            //{
            //    return Json(DbWMS.PermissionError());
            //}
            dataItemRequestDetail ItemRequestDetail = Mapper.Map(model, new dataItemRequestDetail());
            List<dataItemRequestDetail> DeletedLanguages = DeleteItemRequestDetails(new List<dataItemRequestDetail> { ItemRequestDetail });

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleDeleteMessage(DeletedLanguages, DataTableNames.WarehouseItemRequestDetailsDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyItemRequestDetail(ItemRequestDetail.ItemRequestDetailGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult WarehouseItemRequestDetailRestore(dataItemRequestDetail model)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Restore, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (ActiveItemRequestDetail(model))
            {
                return Json(DbWMS.RecordExists());
            }

            List<dataItemRequestDetail> RestoredLanguages = RestoreItemRequestDetails(Portal.SingleToList(model));

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleRestoreMessage(RestoredLanguages, DataTableNames.WarehouseItemRequestDetailsDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyItemRequestDetail(model.ItemRequestDetailGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult WarehouseItemRequestDetailsDataTableDelete(List<dataItemRequestDetail> models)
        {
            //if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Delete, Apps.WMS))
            //{
            //    return Json(DbWMS.PermissionError());
            //}
            List<dataItemRequestDetail> ItemRequestDetails = Mapper.Map(models, new List<dataItemRequestDetail>());
            List<dataItemRequestDetail> DeletedLanguages = DeleteItemRequestDetails(ItemRequestDetails);

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.PartialDeleteMessage(DeletedLanguages, ItemRequestDetails, DataTableNames.WarehouseItemRequestDetailsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult WarehouseItemRequestDetailsDataTableRestore(List<dataItemRequestDetail> models)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Restore, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            List<dataItemRequestDetail> RestoredLanguages = RestoreItemRequestDetails(models);

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.WarehouseItemRequestDetailsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        private List<dataItemRequestDetail> DeleteItemRequestDetails(List<dataItemRequestDetail> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<dataItemRequestDetail> DeletedItemRequestDetails = new List<dataItemRequestDetail>();

            string query = DbWMS.QueryBuilder(models, Permissions.WarehouseItemsRelease.DeleteGuid, SubmitTypes.Delete, "");

            var languages = DbWMS.Database.SqlQuery<dataItemRequestDetail>(query).ToList();

            foreach (var language in languages)
            {
                DeletedItemRequestDetails.Add(DbWMS.Delete(language, ExecutionTime, Permissions.WarehouseItemsRelease.DeleteGuid, DbCMS));
            }

            return DeletedItemRequestDetails;
        }

        private List<dataItemRequestDetail> RestoreItemRequestDetails(List<dataItemRequestDetail> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<dataItemRequestDetail> RestoredLanguages = new List<dataItemRequestDetail>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbWMS.QueryBuilder(models, Permissions.WarehouseItemsRelease.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Languages = DbWMS.Database.SqlQuery<dataItemRequestDetail>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveItemRequestDetail(language))
                {
                    RestoredLanguages.Add(DbWMS.Restore(language, Permissions.WarehouseItemsRelease.DeleteGuid, Permissions.WarehouseItemsRelease.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyItemRequestDetail(Guid PK)
        {
            dataItemRequestDetail dbModel = new dataItemRequestDetail();

            var Language = DbWMS.dataItemRequestDetail.Where(l => l.ItemRequestDetailGUID == PK).FirstOrDefault();
            var dbLanguage = DbWMS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.dataItemRequestDetailRowVersion.SequenceEqual(dbModel.dataItemRequestDetailRowVersion))
            {
                return Json(DbWMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbWMS, dbModel, "ItemRequestDetailsContainer"));
        }

        private bool ActiveItemRequestDetail(object model)
        {
            dataItemRequestDetail ItemRequestDetail = Mapper.Map(model, new dataItemRequestDetail());
            int LanguageID = DbWMS.dataItemRequestDetail
                                  .Where(x =>

                                              x.ItemRequestDetailGUID == ItemRequestDetail.ItemRequestDetailGUID &&
                                              x.WarehouseItemGUID==ItemRequestDetail.WarehouseItemGUID&&
                                              x.QuantityOrdered==ItemRequestDetail.QuantityOrdered&&
                                              
                                              x.Active == true).Count();
            if (LanguageID > 0)
            {
                ModelState.AddModelError("LanguageID", "Model Already exists");
            }

            return (LanguageID > 0);
        }

        #endregion Language




    }
}