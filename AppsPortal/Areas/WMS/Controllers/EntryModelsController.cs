using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.ViewModels;
using AutoMapper;
using LinqKit;
using WMS_DAL.Model;
using WMS_DAL.ViewModels;

namespace AppsPortal.Areas.WMS.Controllers
{
    public class EntryModelsController : WMSBaseController
    {
        // GET: WMS/EntryModels
        #region Entry Models
        [Route("WMS/EntryModels/")]
        public ActionResult EntryModelIndex()
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            return View("~/Areas/WMS/Views/EntryModels/Index.cshtml");
        }

        [Route("WMS/WarehouseItemEntriesDataTable/")]
        public JsonResult EntryModelsDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);
            Expression<Func<EntryModelsDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<EntryModelsDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.WarehouseItemsEntry.CreateGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();
            //Fix
            var All = (

                from a in DbWMS.dataItemInput.AsExpandable()
                join b in DbWMS.codeTablesValues.Where(x => x.Active) on a.SourceGUID equals b.ValueGUID
                join c in DbWMS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on b.ValueGUID equals c.ValueGUID into LJ1
                from R1 in LJ1.DefaultIfEmpty()
                select new EntryModelsDataTableModel
                    {
                        ItemInputGUID = a.ItemInputGUID,
                        Active = a.Active,
                        InputDate = a.InputDate,
                        BillNumber = a.BillNumber,
                        SequenceNumber = a.SequenceNumber,
                         BillSourceType = R1.ValueDescription,
                        dataItemInputRowVersion = a.dataItemInputRowVersion
                    }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<EntryModelsDataTableModel> Result = Mapper.Map<List<EntryModelsDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("WMS/EntryModels/Create/")]
        public ActionResult EntryModelCreate()
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            return View("~/Areas/WMS/Views/EntryModels/EntryModel.cshtml", new EntryModelUpdateModel());
        }

        [Route("WMS/EntryModels/Update/{PK}")]
        public ActionResult EntryModelUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            var model = (from a in DbWMS.dataItemInput.WherePK(PK)
                             //join b in DbWMS.dataItemInput.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.dataItemInput.DeletedOn) && x.LanguageID == LAN)
                             //on a.ItemInputGUID equals b.ItemInputGUID into LJ1
                             //from R1 in LJ1.DefaultIfEmpty()
                         select new EntryModelUpdateModel
                         {
                             ItemInputGUID = a.ItemInputGUID,
                             Active = a.Active,
                             InputDate = a.InputDate,
                             BillNumber = a.BillNumber,
                             SequenceNumber = a.SequenceNumber,
                             SourceNameGUID = (Guid) a.SourceNameGUID,
                             SourceGUID = (Guid) a.SourceGUID,
                             dataItemInputRowVersion = a.dataItemInputRowVersion
                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("EntryModels", "EntryModels", new { Area = "WMS" }));

            return View("~/Areas/WMS/Views/EntryModels/EntryModel.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult EntryModelCreate(EntryModelUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            DateTime ExecutionTime = DateTime.Now;
            model.CreatedDate = ExecutionTime;
            if (!ModelState.IsValid || ActiveEntryModel(model)) return PartialView("~/Areas/WMS/Views/EntryModels/_EntryModelForm.cshtml", model);

            Guid EntityPK = Guid.NewGuid();

            dataItemInput EntryModel = Mapper.Map(model, new dataItemInput());
            EntryModel.ItemInputGUID = EntityPK;
            EntryModel.CreatedByGUID = UserGUID;
            //EntryModel.CreatedDate = ExecutionTime;

            DbWMS.Create(EntryModel, Permissions.WarehouseItemsEntry.CreateGuid, ExecutionTime, DbCMS);

            //dataItemInputLanguage Language = Mapper.Map(model, new dataItemInputLanguage());
            //Language.ItemInputGUID = EntityPK;

            //DbWMS.Create(Language, Permissions.WarehouseItemsEntry.CreateGuid, ExecutionTime, DbCMS);

            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.WarehouseItemEntryDetailsDataTable, ControllerContext, "ModelDetailsContainer"));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.WarehouseItemsEntry.Create, Apps.WMS, new UrlHelper(Request.RequestContext).Action("Create", "EntryModels", new { Area = "WMS" })), Container = "EntryModelFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.WarehouseItemsEntry.Update, Apps.WMS), Container = "EntryModelFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.WarehouseItemsEntry.Delete, Apps.WMS), Container = "EntryModelFormControls" });

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleCreateMessage(DbWMS.PrimaryKeyControl(EntryModel), null, Partials, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult EntryModelUpdate(EntryModelUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Update, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (!ModelState.IsValid || ActiveEntryModel(model)) return PartialView("~/Areas/WMS/Views/EntryModels/_EntryModelForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            dataItemInput EntryModel = Mapper.Map(model, new dataItemInput());
            DbWMS.Update(EntryModel, Permissions.WarehouseItemsEntry.UpdateGuid, ExecutionTime, DbCMS);

            //var Language = DbWMS.dataItemInput.Where(l => l.item == model.ItemInputGUID && l.LanguageID == LAN && l.Active).FirstOrDefault();

            //if (Language == null)
            //{
            //    Language = Mapper.Map(model, Language);
            //    Language.ItemInputGUID = EntryModel.ItemInputGUID;
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
                DbWMS.SaveChanges();
                return Json(DbWMS.SingleUpdateMessage(null, null, DbWMS.RowVersionControls(Portal.SingleToList(EntryModel))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyEntryModel(model.ItemInputGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost]
        public ActionResult WarehouseItemEntriesDataTableDelete(EntryModelUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Delete, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            dataItemInput EntryModel = Mapper.Map(model, new dataItemInput());
            List<dataItemInput> DeletedEntryModel = DeleteEntryModel(Portal.SingleToList(EntryModel));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.WarehouseItemsEntry.Restore, Apps.WMS), Container = "EntryModelFormControls" });

            try
            {
                int CommitedRows = DbWMS.SaveChanges();
                DbWMS.SaveChanges();
                return Json(DbWMS.SingleDeleteMessage(CommitedRows, DeletedEntryModel.FirstOrDefault(), "EntryModelDetailsContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyEntryModel(model.ItemInputGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult EntryModelRestore(dataItemInput model)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Restore, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (ActiveEntryModel(model))
            {
                return Json(DbWMS.RecordExists());
            }

            List<dataItemInput> RestoredEntryModel = RestoreEntryModel(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.WarehouseItemsEntry.Create, Apps.WMS, new UrlHelper(Request.RequestContext).Action("EntryModelCreate", "EntryModels", new { Area = "WMS" })), Container = "EntryModelFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.WarehouseItemsEntry.Update, Apps.WMS), Container = "EntryModelFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.WarehouseItemsEntry.Delete, Apps.WMS), Container = "EntryModelFormControls" });

            try
            {
                int CommitedRows = DbWMS.SaveChanges();
                DbWMS.SaveChanges();
                return Json(DbWMS.SingleRestoreMessage(CommitedRows, RestoredEntryModel, DbWMS.PrimaryKeyControl(RestoredEntryModel.FirstOrDefault()), null, null, UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyEntryModel(model.ItemInputGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult EntryModelDataTableDelete(List<dataItemInput> models)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Delete, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            List<dataItemInput> DeletedEntryModel = DeleteEntryModel(models);

            try
            {
                DbWMS.SaveChanges();
                DbWMS.SaveChanges();
                return Json(DbWMS.PartialDeleteMessage(DeletedEntryModel, models, DataTableNames.WarehouseItemEntriesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult EntryModelDataTableRestore(List<dataItemInput> models)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Restore, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            List<dataItemInput> RestoredEntryModel = RestoreEntryModel(models);

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.PartialRestoreMessage(RestoredEntryModel, models, DataTableNames.WarehouseItemEntriesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        private List<dataItemInput> DeleteEntryModel(List<dataItemInput> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<dataItemInput> DeletedEntryModel = new List<dataItemInput>();
            //Fix
            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbWMS.QueryBuilder(models, Permissions.WarehouseItemsEntry.DeleteGuid, SubmitTypes.Delete, "");

            var Records = DbWMS.Database.SqlQuery<dataItemInput>(query).ToList();
            foreach (var record in Records)
            {
                DeletedEntryModel.Add(DbWMS.Delete(record, ExecutionTime, Permissions.WarehouseItemsEntry.DeleteGuid, DbCMS));
            }

            //var Languages = DeletedEntryModel.SelectMany(a => a.dataItemInputLanguage).Where(l => l.Active).ToList();
            //foreach (var language in Languages)
            //{
            //    DbWMS.Delete(language, ExecutionTime, Permissions.WarehouseItemsEntry.DeleteGuid, DbCMS);
            //}
            return DeletedEntryModel;
        }

        private List<dataItemInput> RestoreEntryModel(List<dataItemInput> models)
        {

            DateTime RestoringTime = DateTime.Now;

            List<dataItemInput> RestoredEntryModel = new List<dataItemInput>();
            //Fix
            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbWMS.QueryBuilder(models, Permissions.WarehouseItemsEntry.DeleteGuid, SubmitTypes.Restore, "");

            var Records = DbWMS.Database.SqlQuery<dataItemInput>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveEntryModel(record))
                {
                    RestoredEntryModel.Add(DbWMS.Restore(record, Permissions.WarehouseItemsEntry.DeleteGuid, Permissions.WarehouseItemsEntry.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            //var Languages = RestoredEntryModel.SelectMany(x => x.dataItemInputLanguage.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            //foreach (var language in Languages)
            //{
            //    DbWMS.Restore(language, Permissions.WarehouseItemsEntry.DeleteGuid, Permissions.WarehouseItemsEntry.RestoreGuid, RestoringTime, DbCMS);
            //}

            return RestoredEntryModel;
        }

        private JsonResult ConcurrencyEntryModel(Guid PK)
        {
            EntryModelUpdateModel dbModel = new EntryModelUpdateModel();

            var EntryModel = DbWMS.dataItemInput.Where(x => x.ItemInputGUID == PK).FirstOrDefault();
            var dbEntryModel = DbWMS.Entry(EntryModel).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbEntryModel, dbModel);

            //var Language = DbWMS.dataItemInputLanguage.Where(x => x.ItemInputGUID == PK).Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.dataItemInput.DeletedOn) && x.LanguageID == LAN).FirstOrDefault();
            //var dbLanguage = DbWMS.Entry(Language).GetDatabaseValues().ToObject();
            //dbModel = Mapper.Map(dbLanguage, dbModel);

            if (EntryModel.dataItemInputRowVersion.SequenceEqual(dbModel.dataItemInputRowVersion))
            {
                return Json(DbWMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbWMS, dbModel, "EntryModelDetailsContainer"));
        }

        private bool ActiveEntryModel(Object model)
        {
            dataItemInput EntryModel = Mapper.Map(model, new dataItemInput());
            int ModelDescription = DbWMS.dataItemInput
                                    .Where(x => x.BillNumber == EntryModel.BillNumber &&
                                                x.ItemInputGUID!=EntryModel.ItemInputGUID &&
                                                x.Active).Count();
            if (ModelDescription > 0)
            {
                ModelState.AddModelError("ModelDescription", "PO Number is already exists");
            }
            return (ModelDescription > 0);
        }

        #endregion

        #region Entry Details

        //[Route("WMS/WarehouseItemEntryDetailsDataTable/{PK}")]
        public ActionResult WarehouseItemEntryDetailsDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/WMS/Views/EntryModelDetails/_EntryModelDetailsDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<dataItemInputDetail, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<dataItemInputDetail>(DataTable.Filters);
            }

            var Result = (from a in DbWMS.dataItemInputDetail.AsNoTracking().AsExpandable().Where(x => x.ItemInputGUID == PK ).Where(Predicate)
                          join b in DbWMS.codeItemModelWarehouse.Where(x=>x.Active) on a.ItemModelWarehouseGUID equals b.ItemModelWarehouseGUID
                          join c in DbWMS.codeWarehouseItemModel.Where(x=>x.Active) on b.WarehouseItemModelGUID equals c.WarehouseItemModelGUID
                          join d in DbWMS.codeWarehouseItemModelLanguage.Where(x=>x.Active && x.LanguageID == LAN) on c.WarehouseItemModelGUID equals d.WarehouseItemModelGUID
                          select new
                          {
                              ItemInputDetailGUID = a.ItemInputDetailGUID,
                              ModelDescription = d.ModelDescription,
                              BarcodeNumber=a.dataItemInputDeterminant.Where(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.Barcode).Select(x=>x.DeterminantValue).FirstOrDefault(),
                              SerialNumber = a.dataItemInputDeterminant.Where(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.SerialNumber).Select(x=>x.DeterminantValue).FirstOrDefault(),
                              IME1 = a.dataItemInputDeterminant.Where(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.IME1).Select(x=>x.DeterminantValue).FirstOrDefault(),
                              Qunatity = a.Qunatity,
                              a.dataItemInputDetailRowVersion,
                              a.Active
                          });


            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult WarehouseItemEntryDetailCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.ItemModel.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            return PartialView("~/Areas/WMS/Views/EntryModelDetails/_EntryModelDetailUpdateModal.cshtml",
                new ItemInputDetailModel { ItemInputGUID = FK });
        }

        public ActionResult WarehouseItemEntryDetailUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.ItemModel.Access, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
 
            var myDetail= DbWMS.dataItemInputDetail.Find(PK);


            ItemInputDetailModel EntryModel = Mapper.Map(myDetail, new ItemInputDetailModel());
            var determinantModel= DbWMS.dataItemInputDeterminant.Where(x => x.ItemInputDetailGUID == PK).ToList();
            var Determinants = DbWMS.codeTablesValuesLanguages.Where(x =>
                x.codeTablesValues.TableGUID == LookupTables.ModelDeterminants
                && x.LanguageID==LAN
                && x.Active).ToList();
            foreach (var item in determinantModel)
            {
                ModelDeterminantVM currentDet = new ModelDeterminantVM
                {
                    DeterminantValue = item.DeterminantValue,
                    WarehouseItemModelDeterminantGUID =(Guid) item.WarehouseItemModelDeterminantGUID,
                    DeterminantName= Determinants.Where(x=>x.ValueGUID==item.codeWarehouseItemModelDeterminant.DeterminantGUID).Select(x=>x.ValueDescription).FirstOrDefault(),
                };
                EntryModel.ModelDeterminantVM.Add(currentDet);


            }


            return PartialView("~/Areas/WMS/Views/EntryModelDetails/_EntryModelDetailUpdateModal.cshtml", EntryModel);
        }

        [HttpPost]
        public ActionResult WarehouseItemEntryDetailCreate(ItemInputDetailModel model)
        {
            if (!CMS.HasAction(Permissions.ItemModel.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            dataItemInputDetail EntryModel = Mapper.Map(model, new dataItemInputDetail());
            if (!ModelState.IsValid && ActiveEntryModelDetail(EntryModel) ) return PartialView("~/Areas/WMS/Views/EntryModelDetails/_EntryModelDetailUpdateModal.cshtml", model);
            if(ActiveCheckDeterminantEntryModelDetail(model)) return PartialView("~/Areas/WMS/Views/EntryModelDetails/_EntryModelDetailUpdateModal.cshtml", model);
            var CurrentWarehouseGUID = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).Select(x => x.WarehouseGUID)
                .FirstOrDefault();
            List<dataItemInputDeterminant> allModelDeterminants = new List<dataItemInputDeterminant>();
            DateTime ExecutionTime = DateTime.Now;
            Guid EntityPK = Guid.NewGuid();
            if (EntryModel.Qunatity == 0)
                EntryModel.Qunatity = 1;
            EntryModel.ItemInputDetailGUID = EntityPK;
            EntryModel.ItemStatusGUID = WarehouseItemStatus.Functionting;
            EntryModel.IsAvaliable = true;
            EntryModel.CreatedByGUID = UserGUID;
            EntryModel.CreatedDate = ExecutionTime;
            EntryModel.LastCustdianGUID = WarehouseRequestSourceTypes.Warehouse;
            EntryModel.LastCustdianNameGUID = CurrentWarehouseGUID;
            EntryModel.WarehouseOwnerGUID = CurrentWarehouseGUID;
            DbWMS.Create(EntryModel, Permissions.WarehouseItemsEntry.CreateGuid, ExecutionTime, DbCMS);

            foreach (var myDet in model.ModelDeterminantVM)
            {
                allModelDeterminants.Add(new dataItemInputDeterminant
                {
                    ItemInputDeterminantGUID = Guid.NewGuid(),
                    ItemInputDetailGUID = EntryModel.ItemInputDetailGUID,
                    WarehouseItemModelDeterminantGUID = myDet.WarehouseItemModelDeterminantGUID,
                    DeterminantValue = myDet.DeterminantValue,
                    CreatedByGUID=UserGUID,
                    CreatedDate = ExecutionTime,
                    Active = true,
                    
                });
            }
            //Fix Try This
            //model.ModelDeterminantVM.ForEach(myDet =>
            //     allModelDeterminants.Add(new dataItemInputDeterminant
            //     {
            //         ItemInputDeterminantGUID = Guid.NewGuid(),
            //         ItemInputDetailGUID = EntryModel.ItemInputDetailGUID,
            //         WarehouseItemModelDeterminantGUID = myDet.WarehouseItemModelDeterminantGUID,
            //         DeterminantValue = myDet.DeterminantValue,
            //         CreatedByGUID = UserGUID,
            //         CreatedDate = ExecutionTime,
            //         Active = true,
            //     })
            //    );
            DbWMS.CreateBulk(allModelDeterminants, Permissions.WarehouseItemsEntry.CreateGuid, DateTime.Now, DbCMS);

            dataItemTransfer TransferModel = new dataItemTransfer();
            TransferModel.ItemTransferGUID = Guid.NewGuid();
            TransferModel.ItemInputDetailGUID = EntryModel.ItemInputDetailGUID;
            TransferModel.TransferDate = ExecutionTime;
            
            TransferModel.SourceGUID = CurrentWarehouseGUID;
            TransferModel.DestionationGUID = CurrentWarehouseGUID;
            TransferModel.TransferedByGUID = UserGUID;
            TransferModel.IsLastTransfer = true;
            DbWMS.Create(TransferModel, Permissions.WarehouseItemsEntry.CreateGuid, ExecutionTime, DbCMS);
            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleUpdateMessage(DataTableNames.WarehouseItemEntryDetailsDataTable, DbWMS.PrimaryKeyControl(EntryModel), DbWMS.RowVersionControls(Portal.SingleToList(EntryModel))));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult WarehouseItemEntryDetailUpdate(ItemInputDetailModel model)
        {
            if (!CMS.HasAction(Permissions.ItemModel.Update, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (!ModelState.IsValid ) return PartialView("~/Areas/WMS/Views/ItemModels/_ModelsDeterminantModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;
            dataItemInputDetail EntryModel = Mapper.Map(model, new dataItemInputDetail());
            List<dataItemInputDeterminant> allModelDeterminants = new List<dataItemInputDeterminant>();
           var detmintantToChange= DbWMS.dataItemInputDeterminant.Where(x => x.ItemInputDetailGUID == EntryModel.ItemInputDetailGUID).ToList();

            DbWMS.Update(EntryModel, Permissions.WarehouseItemsEntry.UpdateGuid, ExecutionTime, DbCMS);
            
            foreach (var myDet in detmintantToChange)
            {
                allModelDeterminants.Add(new dataItemInputDeterminant
                {
                    ItemInputDeterminantGUID = myDet.ItemInputDeterminantGUID,
                    ItemInputDetailGUID = myDet.ItemInputDetailGUID,
                    WarehouseItemModelDeterminantGUID = myDet.WarehouseItemModelDeterminantGUID,
                    DeterminantValue = model.ModelDeterminantVM.Where(x=>x.WarehouseItemModelDeterminantGUID==myDet.WarehouseItemModelDeterminantGUID).Select(x=>x.DeterminantValue).FirstOrDefault(),
                    Active = true,
                    CreatedDate = myDet.CreatedDate,
                    CreatedByGUID = myDet.CreatedByGUID
                });
            }
            DbWMS.UpdateBulk(allModelDeterminants, Permissions.WarehouseItemsEntry.UpdateGuid, ExecutionTime, DbCMS);
         
            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleUpdateMessage(DataTableNames.WarehouseItemEntryDetailsDataTable,
                    DbWMS.PrimaryKeyControl(EntryModel),
                    DbWMS.RowVersionControls(Portal.SingleToList(EntryModel))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyEntryModelDetail((Guid)model.ItemInputDetailGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult WarehouseItemEntryDetailDelete(dataItemInputDetail model)
        {
            if (!CMS.HasAction(Permissions.ItemModel.Delete, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            List<dataItemInputDetail> DeletedLanguages = DeleteEntryModelDetails(new List<dataItemInputDetail> { model });

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleDeleteMessage(DeletedLanguages, DataTableNames.WarehouseItemEntryDetailsDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyEntryModelDetail(model.ItemInputDetailGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult WarehouseItemEntryDetailRestore(dataItemInputDetail model)
        {
            if (!CMS.HasAction(Permissions.ItemModel.Restore, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (ActiveEntryModelDetail(model))
            {
                return Json(DbWMS.RecordExists());
            }

            List<dataItemInputDetail> RestoredLanguages = RestoreEntryModelDetails(Portal.SingleToList(model));

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleRestoreMessage(RestoredLanguages, DataTableNames.WarehouseItemEntryDetailsDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyEntryModelDetail(model.ItemInputDetailGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult WarehouseItemEntryDetailsDataTableDelete(List<dataItemInputDetail> models)
        {
            if (!CMS.HasAction(Permissions.ItemModel.Delete, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            List<dataItemInputDetail> DeletedLanguages = DeleteEntryModelDetails(models);

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.WarehouseItemEntryDetailsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult WarehouseItemEntryDetailsDataTableRestore(List<dataItemInputDetail> models)
        {
            if (!CMS.HasAction(Permissions.ItemModel.Restore, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            List<dataItemInputDetail> RestoredLanguages = RestoreEntryModelDetails(models);

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.WarehouseItemEntryDetailsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        private List<dataItemInputDetail> DeleteEntryModelDetails(List<dataItemInputDetail> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<dataItemInputDetail> DeletedEntryModelDetails = new List<dataItemInputDetail>();

            string query = DbWMS.QueryBuilder(models, Permissions.ItemModel.DeleteGuid, SubmitTypes.Delete, "");

            var languages = DbWMS.Database.SqlQuery<dataItemInputDetail>(query).ToList();

            foreach (var language in languages)
            {
                DeletedEntryModelDetails.Add(DbWMS.Delete(language, ExecutionTime, Permissions.ItemModel.DeleteGuid, DbCMS));
            }

            return DeletedEntryModelDetails;
        }

        private List<dataItemInputDetail> RestoreEntryModelDetails(List<dataItemInputDetail> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<dataItemInputDetail> RestoredLanguages = new List<dataItemInputDetail>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbWMS.QueryBuilder(models, Permissions.ItemModel.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Languages = DbWMS.Database.SqlQuery<dataItemInputDetail>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveEntryModelDetail(language))
                {
                    RestoredLanguages.Add(DbWMS.Restore(language, Permissions.ItemModel.DeleteGuid, Permissions.ItemModel.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyEntryModelDetail(Guid PK)
        {
            dataItemInputDetail dbModel = new dataItemInputDetail();

            var Language = DbWMS.dataItemInputDetail.Where(l => l.ItemInputDetailGUID == PK).FirstOrDefault();
            var dbLanguage = DbWMS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.dataItemInputDetailRowVersion.SequenceEqual(dbModel.dataItemInputDetailRowVersion))
            {
                return Json(DbWMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbWMS, dbModel, "EntryModelDetailsContainer"));
        }

        private bool ActiveEntryModelDetail(object model)
        {
            dataItemInputDetail EntryDetailModel = Mapper.Map(model, new dataItemInputDetail());
            


            int LanguageID = DbWMS.dataItemInputDetail.Count(x =>
               x.ItemInputDetailGUID== EntryDetailModel.ItemInputDetailGUID &&
               x.ItemInputGUID==EntryDetailModel.ItemInputGUID && 
                x.Active);
            if (LanguageID > 0)
            {
                ModelState.AddModelError("LanguageID", "Model Already exists");
            }

            return (LanguageID > 0);
        }

        private bool ActiveCheckDeterminantEntryModelDetail(ItemInputDetailModel model)
        {
            int LanguageID = DbWMS.dataItemInputDetail.ToList().Count(x =>
                (x.dataItemInputDeterminant
                     .Where(d => model.ModelDeterminantVM.Select(f => f.DeterminantValue).Contains(d.DeterminantValue)).Count() > 0

                ) &&
                x.ItemInputDetailGUID != model.ItemInputDetailGUID &&

                x.Active);
            if (LanguageID > 0)
            {
                ModelState.AddModelError("LanguageID", "Model Already exists");
            }

            return (LanguageID > 0);
        }


        #endregion Language

        #region Lists
        public ActionResult GetModelDeterminats(Guid ItemModelWarehouseGUID)
      {
          ItemInputDetailModel model = new ItemInputDetailModel();

         var myDet = (from a in DbWMS.codeItemModelWarehouse.Where(x => x.Active && x.ItemModelWarehouseGUID == ItemModelWarehouseGUID)
                             join b in DbWMS.codeWarehouseItemModel.Where(x => x.Active) on a.WarehouseItemModelGUID equals b.WarehouseItemModelGUID
                             join c in DbWMS.codeWarehouseItemModelDeterminant.Where(x =>  x.Active) on b.WarehouseItemModelGUID equals c.WarehouseItemModelGUID
                             join d in DbWMS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID==LAN) on c.DeterminantGUID equals d.ValueGUID
                          
                             select new ModelDeterminantVM  
                             {
                                 WarehouseItemModelDeterminantGUID = (Guid)c.WarehouseItemModelDeterminantGUID,
                                 DeterminantName = d.ValueDescription
                         
                             }).ToList();
          model.ModelDeterminantVM = myDet;



            return PartialView("~/Areas/WMS/Views/EntryModelDetails/_EntryModelModelDeterminant.cshtml", model);
        }
        #endregion


    }
}