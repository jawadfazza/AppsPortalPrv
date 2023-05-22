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
    public class ReleaseModelsController : WMSBaseController
    {
        // GET: WMS/ReleaseModels
        #region Release 

        [Route("WMS/ReleaseModels/")]
        public ActionResult ReleaseModelIndex()
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Access, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            return View("~/Areas/WMS/Views/ReleaseModels/Index.cshtml");
        }

        [Route("WMS/WarehouseItemReleasesDataTable/")]
        public JsonResult ReleaseModelsDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);
            Expression<Func<ReleaseModelsDataTableModel, bool>> Predicate = p => true;
            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<ReleaseModelsDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.WarehouseItemsRelease.CreateGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();

            var All = (

                from a in DbWMS.dataItemOutput.AsNoTracking().AsExpandable().Where(x=>x.Active)
                join b in DbWMS.codeTablesValues.Where(x=>x.Active) on a.RequesterGUID equals b.ValueGUID 
                join c in DbWMS.codeTablesValuesLanguages.Where(x=>x.Active && x.LanguageID==LAN ) on b.ValueGUID equals c.ValueGUID into LJ1
                from R1 in LJ1.DefaultIfEmpty()
                //join c in DbWMS.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID==LAN) on a.PartyGUID equals c.UserGUID into LJ2
                //from R2 in LJ2.DefaultIfEmpty()
                //join c in DbWMS.codeWarehouse.Where(x => x.Active) on a.PartyGUID equals c.WarehouseGUID
                //join d in DbWMS.codeWarehouseLanguage.Where(x => x.Active && x.LanguageID == LAN) on c.WarehouseGUID equals d.WarehouseGUID into LJ3
                //from R3 in LJ3.DefaultIfEmpty()
                select new ReleaseModelsDataTableModel
                {

                    ItemOutputGUID = a.ItemOutputGUID,
                    Active = a.Active,
                    OutputDate = a.OutputDate,
                    OutputNumber = a.OutputNumber,

                    RequsterType = R1.ValueDescription,
                    //BillSourceName = a.PartyTypeGUID==PartyTypes.Staff? ,
                    //RequsterName = (a.PartyTypeGUID == PartyTypes.Warehouse ? R3.WarehouseDescription : R2.FirstName + " " + R2.Surname),
                    //ItemDescription = 
                    dataItemOutputRowVersion = a.dataItemOutputRowVersion
                }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<ReleaseModelsDataTableModel> Result = Mapper.Map<List<ReleaseModelsDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("WMS/ReleaseModels/Create/")]
        public ActionResult ReleaseModelCreate()
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            return View("~/Areas/WMS/Views/ReleaseModels/ReleaseModel.cshtml", new ReleaseModelUpdateModel());
        }

        [Route("WMS/ReleaseModels/Update/{PK}")]
        public ActionResult ReleaseModelUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            var model = (from a in DbWMS.dataItemOutput.WherePK(PK)
                             //join b in DbWMS.dataItemOutput.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.dataItemOutput.DeletedOn) && x.LanguageID == LAN)
                             //on a.ItemOutputGUID equals b.ItemOutputGUID into LJ1
                             //from R1 in LJ1.DefaultIfEmpty()
                         select new ReleaseModelUpdateModel
                         {
                             ItemOutputGUID = a.ItemOutputGUID,
                             Active = a.Active,
                             OutputDate = a.OutputDate,
                             OutputNumber = a.OutputNumber,
                             RequesterGUID = (Guid)a.RequesterGUID,
                             RequesterNameGUID = a.RequesterNameGUID,
                             CreatedByGUID =(Guid) a.CreatedByGUID,
                             CreatedDate = a.CreatedDate,

                             //ItemDescription = 
                             dataItemOutputRowVersion = a.dataItemOutputRowVersion
                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("ReleaseModels", "ReleaseModels", new { Area = "WMS" }));

            return View("~/Areas/WMS/Views/ReleaseModels/ReleaseModel.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ReleaseModelCreate(ReleaseModelUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (!ModelState.IsValid || ActiveReleaseModel(model)) return PartialView("~/Areas/WMS/Views/ReleaseModels/_ReleaseModelForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            dataItemOutput ReleaseModel = Mapper.Map(model, new dataItemOutput());
            ReleaseModel.ItemOutputGUID = EntityPK;
            ReleaseModel.CreatedByGUID = UserGUID;
            ReleaseModel.CreatedDate = ExecutionTime;
            ReleaseModel.OutputNumber = DbWMS.dataItemOutput.Select(x => x.OutputNumber).Max()+1??1;
            DbWMS.Create(ReleaseModel, Permissions.WarehouseItemsRelease.CreateGuid, ExecutionTime, DbCMS);

            //dataItemOutputLanguage Language = Mapper.Map(model, new dataItemOutputLanguage());
            //Language.ItemOutputGUID = EntityPK;

            //DbWMS.Create(Language, Permissions.WarehouseItemsRelease.CreateGuid, ExecutionTime, DbCMS);

            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.WarehouseItemReleaseDeatilsDataTable, ControllerContext, "ReleaseModelDetailsContainer"));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.WarehouseItemsRelease.Create, Apps.WMS, new UrlHelper(Request.RequestContext).Action("Create", "ReleaseModels", new { Area = "WMS" })), Container = "ReleaseModelFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.WarehouseItemsRelease.Update, Apps.WMS), Container = "ReleaseModelFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.WarehouseItemsRelease.Delete, Apps.WMS), Container = "ReleaseModelFormControls" });

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleCreateMessage(DbWMS.PrimaryKeyControl(ReleaseModel), null, Partials, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ReleaseModelUpdate(ReleaseModelUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Update, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (!ModelState.IsValid || ActiveReleaseModel(model)) return PartialView("~/Areas/WMS/Views/ReleaseModels/_ReleaseModelForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            dataItemOutput ReleaseModel = Mapper.Map(model, new dataItemOutput());
            DbWMS.Update(ReleaseModel, Permissions.WarehouseItemsRelease.UpdateGuid, ExecutionTime, DbCMS);
            var myOutputDetailsGUIDs = DbWMS.dataItemOutputDetail
                .Where(x => x.ItemOutputGUID == ReleaseModel.ItemOutputGUID).Select(x=>x.ItemInputDetailGUID).ToList();
            var myInputDetails = DbWMS.dataItemInputDetail
                .Where(x => myOutputDetailsGUIDs.Contains(x.ItemInputDetailGUID)).ToList();
            foreach (var item in myInputDetails)
            {
                item.LastCustdianGUID =(Guid) ReleaseModel.RequesterGUID;
                item.LastCustdianNameGUID = ReleaseModel.RequesterNameGUID;

            }

            DbWMS.UpdateBulk(myInputDetails, Permissions.WarehouseItemsRelease.UpdateGuid, ExecutionTime, DbCMS); 
            //var Language = DbWMS.dataItemOutput.Where(l => l.item == model.ItemOutputGUID && l.LanguageID == LAN && l.Active).FirstOrDefault();

            //if (Language == null)
            //{
            //    Language = Mapper.Map(model, Language);
            //    Language.ItemOutputGUID = ReleaseModel.ItemOutputGUID;
            //    DbWMS.Create(Language, Permissions.WarehouseItemsRelease.CreateGuid, ExecutionTime, DbCMS);
            //}
            //else if (Language.ModelDescription != model.ModelDescription)
            //{
            //    Language = Mapper.Map(model, Language);
            //    DbWMS.Update(Language, Permissions.WarehouseItemsRelease.UpdateGuid, ExecutionTime, DbCMS);
            //}

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleUpdateMessage(null, null, DbWMS.RowVersionControls(Portal.SingleToList(ReleaseModel))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyReleaseModel(model.ItemOutputGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ReleaseModelDelete(dataItemOutput model)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Delete, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            List<dataItemOutput> DeletedReleaseModel = DeleteReleaseModel(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.WarehouseItemsRelease.Restore, Apps.WMS), Container = "ReleaseModelFormControls" });

            try
            {
                int CommitedRows = DbWMS.SaveChanges();
                DbWMS.SaveChanges();
                return Json(DbWMS.SingleDeleteMessage(CommitedRows, DeletedReleaseModel.FirstOrDefault(), "ReleaseModelFormControls", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyReleaseModel(model.ItemOutputGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ReleaseModelRestore(dataItemOutput model)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Restore, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (ActiveReleaseModel(model))
            {
                return Json(DbWMS.RecordExists());
            }

            List<dataItemOutput> RestoredReleaseModel = RestoreReleaseModel(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.WarehouseItemsRelease.Create, Apps.WMS, new UrlHelper(Request.RequestContext).Action("ReleaseModelCreate", "ReleaseModels", new { Area = "WMS" })), Container = "ReleaseModelFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.WarehouseItemsRelease.Update, Apps.WMS), Container = "ReleaseModelFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.WarehouseItemsRelease.Delete, Apps.WMS), Container = "ReleaseModelFormControls" });

            try
            {
                int CommitedRows = DbWMS.SaveChanges();
                DbWMS.SaveChanges();
                return Json(DbWMS.SingleRestoreMessage(CommitedRows, RestoredReleaseModel, DbWMS.PrimaryKeyControl(RestoredReleaseModel.FirstOrDefault()), null, null, UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyReleaseModel(model.ItemOutputGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ReleaseModelDataTableDelete(List<dataItemOutput> models)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Delete, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            List<dataItemOutput> DeletedReleaseModel = DeleteReleaseModel(models);

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.PartialDeleteMessage(DeletedReleaseModel, models, DataTableNames.WarehouseItemReleasesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ReleaseModelDataTableRestore(List<dataItemOutput> models)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Restore, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            List<dataItemOutput> RestoredReleaseModel = RestoreReleaseModel(models);

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.PartialRestoreMessage(RestoredReleaseModel, models, DataTableNames.WarehouseItemReleasesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        private List<dataItemOutput> DeleteReleaseModel(List<dataItemOutput> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<dataItemOutput> DeletedReleaseModel = new List<dataItemOutput>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "SELECT ItemOutputGUID,CONVERT(varchar(50), ItemOutputGUID) as C2 ,dataItemOutputRowVersion FROM code.dataItemOutput where ItemOutputGUID in (" + string.Join(",", models.Select(x => "'" + x.ItemOutputGUID + "'").ToArray()) + ")";

            string query = DbWMS.QueryBuilder(models, Permissions.WarehouseItemsRelease.DeleteGuid, SubmitTypes.Delete, baseQuery);

            var Records = DbWMS.Database.SqlQuery<dataItemOutput>(query).ToList();
            foreach (var record in Records)
            {
                DeletedReleaseModel.Add(DbWMS.Delete(record, ExecutionTime, Permissions.WarehouseItemsRelease.DeleteGuid, DbCMS));
            }

            //var Languages = DeletedReleaseModel.SelectMany(a => a.dataItemOutputLanguage).Where(l => l.Active).ToList();
            //foreach (var language in Languages)
            //{
            //    DbWMS.Delete(language, ExecutionTime, Permissions.WarehouseItemsRelease.DeleteGuid, DbCMS);
            //}
            return DeletedReleaseModel;
        }

        private List<dataItemOutput> RestoreReleaseModel(List<dataItemOutput> models)
        {

            DateTime RestoringTime = DateTime.Now;

            List<dataItemOutput> RestoredReleaseModel = new List<dataItemOutput>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "SELECT ItemOutputGUID,CONVERT(varchar(50), ItemOutputGUID) as C2 ,dataItemOutputRowVersion FROM code.dataItemOutput where ItemOutputGUID in (" + string.Join(",", models.Select(x => "'" + x.ItemOutputGUID + "'").ToArray()) + ")";

            string query = DbWMS.QueryBuilder(models, Permissions.WarehouseItemsRelease.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Records = DbWMS.Database.SqlQuery<dataItemOutput>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveReleaseModel(record))
                {
                    RestoredReleaseModel.Add(DbWMS.Restore(record, Permissions.WarehouseItemsRelease.DeleteGuid, Permissions.WarehouseItemsRelease.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            //var Languages = RestoredReleaseModel.SelectMany(x => x.dataItemOutputLanguage.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            //foreach (var language in Languages)
            //{
            //    DbWMS.Restore(language, Permissions.WarehouseItemsRelease.DeleteGuid, Permissions.WarehouseItemsRelease.RestoreGuid, RestoringTime, DbCMS);
            //}

            return RestoredReleaseModel;
        }

        private JsonResult ConcurrencyReleaseModel(Guid PK)
        {
            ReleaseModelUpdateModel dbModel = new ReleaseModelUpdateModel();

            var ReleaseModel = DbWMS.dataItemOutput.Where(x => x.ItemOutputGUID == PK).FirstOrDefault();
            var dbReleaseModel = DbWMS.Entry(ReleaseModel).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbReleaseModel, dbModel);

            //var Language = DbWMS.dataItemOutputLanguage.Where(x => x.ItemOutputGUID == PK).Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.dataItemOutput.DeletedOn) && x.LanguageID == LAN).FirstOrDefault();
            //var dbLanguage = DbWMS.Entry(Language).GetDatabaseValues().ToObject();
            //dbModel = Mapper.Map(dbLanguage, dbModel);

            //if (ReleaseModel.dataItemOutputRowVersion.SequenceEqual(dbModel.dataItemOutputRowVersion) && Language.dataItemOutputLanguageRowVersion.SequenceEqual(dbModel.dataItemOutputLanguageRowVersion))
            //{
            //    return Json(DbWMS.PermissionError());
            //}

            return Json(JsonMessages.ConcurrencyError(DbWMS, dbModel, "ReleaseModelDetailsContainer"));
        }

        private bool ActiveReleaseModel(Object model)
        {
            dataItemOutput ReleaseModel = Mapper.Map(model, new dataItemOutput());
            int ModelDescription = DbWMS.dataItemOutput
                                    .Where(x => x.OutputNumber == ReleaseModel.OutputNumber &&
                                                x.WarehouseGUID==ReleaseModel.WarehouseGUID&&
                                                x.ItemOutputGUID != ReleaseModel.ItemOutputGUID &&


                                                x.Active).Count();
            if (ModelDescription > 0)
            {
                ModelState.AddModelError("ModelDescription", "Output Number is already exists");
            }
            return (ModelDescription > 0);
        }

        #endregion

        #region Release Details

        //[Route("WMS/WarehouseItemReleaseDetailsDataTable/{PK}")]
        
        
        public ActionResult WarehouseItemReleaseDeatilsDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/WMS/Views/ReleaseModelDetails/_ReleaseModelDetailsDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<ReleaseModelDetailUpdateModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<ReleaseModelDetailUpdateModel>(DataTable.Filters);
            }

            var Result = (from a in DbWMS.dataItemOutputDetail.AsNoTracking().AsExpandable().Where(x => x.ItemOutputGUID == PK)

                          select new ReleaseModelDetailUpdateModel
                          {
                              ItemOutputDetailGUID= a.ItemOutputDetailGUID,
                              ItemOutputGUID = a.ItemOutputGUID,
                              ModelDescription = a.dataItemInputDetail.codeItemModelWarehouse.codeWarehouseItemModel.codeWarehouseItemModelLanguage.Where(x => x.Active && x.LanguageID == LAN).FirstOrDefault().ModelDescription,
                              BarcodeNumber = a.dataItemInputDetail.dataItemInputDeterminant.FirstOrDefault(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.Barcode).DeterminantValue.ToString(),
                              SerialNumber = a.dataItemInputDetail.dataItemInputDeterminant.FirstOrDefault(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.SerialNumber).DeterminantValue.ToString(),
                              IME1 = a.dataItemInputDetail.dataItemInputDeterminant.FirstOrDefault(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.IME1).DeterminantValue.ToString(),
                              RequestedQunatity = a.RequestedQunatity,
                              ExpectedStartDate = a.ExpectedStartDate,
                              ExpectedReturenedDate = a.ExpectedReturenedDate,
                              ActualReturenedDate = a.ActualReturenedDate,
                              Comments = a.Comments,
                              dataItemOutputDetailRowVersion = a.dataItemOutputDetailRowVersion,
                              CreatedByGUID = (Guid)a.CreatedByGUID,
                              CreatedDate = a.CreatedDate,
                         

                          });


            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReleaseModelDetailCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            return PartialView("~/Areas/WMS/Views/ReleaseModelDetails/_ReleaseModelDetailUpdateModal.cshtml",
                new ReleaseModelDetailUpdateModel { ItemOutputGUID = FK, ItemOutputDetailGUID=Guid.Empty });
        }

        public ActionResult ReleaseModelDetailUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }

            
            ReleaseModelDetailUpdateModel model = Mapper.Map(DbWMS.dataItemOutputDetail.Find(PK), new ReleaseModelDetailUpdateModel());
            return PartialView("~/Areas/WMS/Views/ReleaseModelDetails/_ReleaseModelDetailUpdateModal.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ReleaseModelDetailCreate(ReleaseModelDetailUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            dataItemOutputDetail ReleaseModelDetail = Mapper.Map(model, new dataItemOutputDetail());
            if ( ActiveReleaseModelDetail(ReleaseModelDetail)) return PartialView("~/Areas/WMS/Views/ReleaseModelDetails/_ReleaseModelDetailUpdateModal.cshtml", model);
            //!ModelState.IsValid ||
            DateTime ExecutionTime = DateTime.Now;
            ReleaseModelDetail.ItemOutputDetailGUID=Guid.NewGuid();
            if (ReleaseModelDetail.RequestedQunatity == null)
                ReleaseModelDetail.RequestedQunatity = 1;
            ReleaseModelDetail.CreatedByGUID = UserGUID;
            ReleaseModelDetail.CreatedDate = ExecutionTime;

            

            DbWMS.Create(ReleaseModelDetail, Permissions.WarehouseItemsRelease.CreateGuid, ExecutionTime, DbCMS);

            dataItemInputDetail InputDetail = DbWMS.dataItemInputDetail.Find(ReleaseModelDetail.ItemInputDetailGUID);
            dataItemOutput itemOutput = DbWMS.dataItemOutput.Find(ReleaseModelDetail.ItemOutputGUID);
            InputDetail.IsAvaliable = false;
            InputDetail.LastCustdianGUID =(Guid) itemOutput.RequesterGUID;
            InputDetail.LastCustdianNameGUID = itemOutput.RequesterNameGUID;
            
            DbWMS.Update(InputDetail, Permissions.WarehouseItemsRelease.UpdateGuid, ExecutionTime, DbCMS);

            List<dataItemOutputDetailFlow> flows = DbWMS.dataItemOutputDetailFlow
                .Where(x => x.dataItemOutputDetail.dataItemInputDetail.ItemInputDetailGUID == ReleaseModelDetail.ItemInputDetailGUID).ToList();

            foreach (var item in flows)
            {
                item.IsLastAction = false;
            
            }
            DbWMS.UpdateBulk(flows, Permissions.WarehouseItemsEntry.UpdateGuid, ExecutionTime, DbCMS);

            dataItemOutputDetailFlow outputDetailFlow = new dataItemOutputDetailFlow();

            outputDetailFlow.ItemOutputDetailFlowGUID = Guid.NewGuid();
            outputDetailFlow.ItemOutputDetailGUID = ReleaseModelDetail.ItemOutputDetailGUID;
            outputDetailFlow.FlowTypeGUID= WarehouseRequestFlowType.PendingConfirmed;
            outputDetailFlow.ItemStatuGUID = WarehouseItemStatus.Functionting;
            outputDetailFlow.CreatedDate=DateTime.Now;
            outputDetailFlow.IsLastAction = true;
            outputDetailFlow.IsLastMove = true;
            outputDetailFlow.CreatedByGUID = UserGUID;
            outputDetailFlow.Active = true;
            

            DbWMS.Create(outputDetailFlow, Permissions.WarehouseItemsEntry.CreateGuid, ExecutionTime, DbCMS);


            
            if (itemOutput.RequesterGUID == WarehouseRequestSourceTypes.Warehouse)
            {
                var CurrentWarehouseGUID = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).Select(x => x.WarehouseGUID)
                    .FirstOrDefault();
                List<dataItemTransfer> allPriviousTransfers = DbWMS.dataItemTransfer
                    .Where(x => x.ItemInputDetailGUID == InputDetail.ItemInputDetailGUID).ToList();
                foreach (var item in allPriviousTransfers)
                {
                    item.IsLastTransfer = false;
                }
                DbWMS.UpdateBulk(allPriviousTransfers, Permissions.WarehouseItemsEntry.UpdateGuid, ExecutionTime, DbCMS);

                dataItemTransfer TransferModel = new dataItemTransfer();
                TransferModel.ItemTransferGUID = Guid.NewGuid();
                TransferModel.ItemInputDetailGUID = ReleaseModelDetail.ItemInputDetailGUID;
                TransferModel.TransferDate = ExecutionTime;
                TransferModel.SourceGUID = CurrentWarehouseGUID;
                TransferModel.DestionationGUID = itemOutput.RequesterNameGUID;
                TransferModel.TransferedByGUID = UserGUID;
                TransferModel.IsLastTransfer = true;

                DbWMS.Create(TransferModel, Permissions.WarehouseItemsEntry.CreateGuid, ExecutionTime, DbCMS);

            }

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleUpdateMessage(DataTableNames.WarehouseItemReleaseDeatilsDataTable, DbWMS.PrimaryKeyControl(ReleaseModelDetail), DbWMS.RowVersionControls(Portal.SingleToList(ReleaseModelDetail))));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ReleaseModelDetailUpdate(dataItemOutputDetail model)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Update, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (!ModelState.IsValid || ActiveReleaseModelDetail(model)) return PartialView("~/Areas/WMS/Views/ReleaseModelDetails/_ReleaseModelDetailUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbWMS.Update(model, Permissions.WarehouseItemsRelease.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleUpdateMessage(DataTableNames.WarehouseItemReleasesDataTable,
                    DbWMS.PrimaryKeyControl(model),
                    DbWMS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyReleaseModelDetail(model.ItemOutputDetailGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ReleaseModelDetailDelete(ReleaseModelDetailUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Delete, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            dataItemOutputDetail ReleaseModelDetail = Mapper.Map(model, new dataItemOutputDetail());
            List<dataItemOutputDetail> DeletedLanguages = DeleteReleaseModelDetails(new List<dataItemOutputDetail> { ReleaseModelDetail });

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleDeleteMessage(DeletedLanguages, DataTableNames.WarehouseItemReleasesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyReleaseModelDetail(ReleaseModelDetail.ItemOutputDetailGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ReleaseModelDetailRestore(dataItemOutputDetail model)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Restore, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (ActiveReleaseModelDetail(model))
            {
                return Json(DbWMS.RecordExists());
            }

            List<dataItemOutputDetail> RestoredLanguages = RestoreReleaseModelDetails(Portal.SingleToList(model));

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleRestoreMessage(RestoredLanguages, DataTableNames.WarehouseItemReleasesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyReleaseModelDetail(model.ItemOutputDetailGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult WarehouseItemReleaseDeatilsDataTableDelete(List<dataItemOutputDetail> models)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Delete, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            List<dataItemOutputDetail> ReleaseModelDetails = Mapper.Map(models, new List<dataItemOutputDetail>());
            List<dataItemOutputDetail> DeletedLanguages = DeleteReleaseModelDetails(ReleaseModelDetails);

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.PartialDeleteMessage(DeletedLanguages, ReleaseModelDetails, DataTableNames.WarehouseItemReleasesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult WarehouseItemReleaseDeatilsDataTableRestore(List<dataItemOutputDetail> models)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Restore, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            List<dataItemOutputDetail> RestoredLanguages = RestoreReleaseModelDetails(models);

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.WarehouseItemReleasesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        private List<dataItemOutputDetail> DeleteReleaseModelDetails(List<dataItemOutputDetail> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<dataItemOutputDetail> DeletedReleaseModelDetails = new List<dataItemOutputDetail>();

            string query = DbWMS.QueryBuilder(models, Permissions.WarehouseItemsRelease.DeleteGuid, SubmitTypes.Delete, "");

            var languages = DbWMS.Database.SqlQuery<dataItemOutputDetail>(query).ToList();

            foreach (var language in languages)
            {
                DeletedReleaseModelDetails.Add(DbWMS.Delete(language, ExecutionTime, Permissions.WarehouseItemsRelease.DeleteGuid, DbCMS));
            }

            return DeletedReleaseModelDetails;
        }

        private List<dataItemOutputDetail> RestoreReleaseModelDetails(List<dataItemOutputDetail> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<dataItemOutputDetail> RestoredLanguages = new List<dataItemOutputDetail>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbWMS.QueryBuilder(models, Permissions.WarehouseItemsRelease.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Languages = DbWMS.Database.SqlQuery<dataItemOutputDetail>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveReleaseModelDetail(language))
                {
                    RestoredLanguages.Add(DbWMS.Restore(language, Permissions.WarehouseItemsRelease.DeleteGuid, Permissions.WarehouseItemsRelease.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyReleaseModelDetail(Guid PK)
        {
            dataItemOutputDetail dbModel = new dataItemOutputDetail();

            var Language = DbWMS.dataItemOutputDetail.Where(l => l.ItemOutputDetailGUID == PK).FirstOrDefault();
            var dbLanguage = DbWMS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.dataItemOutputDetailRowVersion.SequenceEqual(dbModel.dataItemOutputDetailRowVersion))
            {
                return Json(DbWMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbWMS, dbModel, "ReleaseModelDetailsContainer"));
        }

        private bool ActiveReleaseModelDetail(object model)
        {
            dataItemOutputDetail ReleaseModelDetail = Mapper.Map(model, new dataItemOutputDetail());
            int LanguageID = DbWMS.dataItemOutputDetail
                                  .Where(x =>
                                            
                                              x.ItemOutputDetailGUID == ReleaseModelDetail.ItemOutputDetailGUID &&
                                              x.Active == true).Count();
            if (LanguageID > 0)
            {
                ModelState.AddModelError("LanguageID", "Model Already exists");
            }

            return (LanguageID > 0);
        }

        #endregion Language

        #region Flow Details

        //public ActionResult ReleaseModelDetailFlowCreate(Guid FK)
        //{
        //    if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Create, Apps.WMS))
        //    {
        //        return Json(DbWMS.PermissionError());
        //    }
        //    return PartialView("~/Areas/WMS/Views/ReleaseModelFlow/_ReleaseModelFlowUpdateModal.cshtml",
        //        new dataItemOutputDetailFlow { ItemOutputDetailGUID = FK });
        //}

        public ActionResult ReleaseModelFlowUpdate(Guid? PK)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            return PartialView("~/Areas/WMS/Views/ReleaseModelFlows/_ReleaseModelFlowUpdateModal.cshtml",
                   new ItemOutputDetailFlowModel { ItemOutputDetailGUID = PK});
          


        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ReleaseModelFlowCreate(ItemOutputDetailFlowModel model)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            dataItemOutputDetailFlow myModel = Mapper.Map(model, new dataItemOutputDetailFlow());
            // if (!ModelState.IsValid || ActiveReleaseModelFlow(model)) return PartialView("~/Areas/WMS/Views/ReleaseModelFlows/_ReleaseModelFlowUpdateModal.cshtml", model);
            DateTime ExecutionTime = DateTime.Now;
            List<dataItemOutputDetailFlow> flows = DbWMS.dataItemOutputDetailFlow
                .Where(x => x.ItemOutputDetailGUID == myModel.ItemOutputDetailGUID).ToList();

            foreach (var item in flows)
            {
                item.IsLastAction = false;
                item.IsLastMove = false;
            }
            DbWMS.UpdateBulk(flows, Permissions.WarehouseItemsEntry.UpdateGuid, ExecutionTime, DbCMS);

            myModel.ItemOutputDetailFlowGUID = Guid.NewGuid();
            myModel.FlowTypeGUID = WarehouseRequestFlowType.Returned;

            myModel.CreatedDate = myModel.CreatedDate;
            myModel.IsLastAction = true;
            myModel.IsLastMove = true;
            myModel.CreatedByGUID = UserGUID;
            myModel.Active = true;
            DbWMS.Create(myModel, Permissions.WarehouseItemsRelease.CreateGuid, ExecutionTime, DbCMS);

            dataItemOutputDetail outputDetail = DbWMS.dataItemOutputDetail.Find(myModel.ItemOutputDetailGUID);
            outputDetail.ActualReturenedDate = myModel.CreatedDate;

            DbWMS.Update(outputDetail, Permissions.WarehouseItemsRelease.UpdateGuid, ExecutionTime, DbCMS);
            
                var CurrentWarehouseGUID = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).Select(x => x.WarehouseGUID)
                    .FirstOrDefault();
                dataItemInputDetail input = DbWMS.dataItemInputDetail.Find(outputDetail.ItemInputDetailGUID);
            if (model.ItemStatuGUID == WarehouseItemStatus.Functionting)
            {
                input.IsAvaliable = true;
            }
            input.LastCustdianGUID = WarehouseRequestSourceTypes.Warehouse;
                input.LastCustdianNameGUID = CurrentWarehouseGUID;
                DbWMS.Update(input, Permissions.WarehouseItemsRelease.UpdateGuid, ExecutionTime, DbCMS);
            
          try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleUpdateMessage(DataTableNames.WarehosueModelMovementsDataTable, DbWMS.PrimaryKeyControl(myModel), DbWMS.RowVersionControls(Portal.SingleToList(myModel))));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }
        private bool ActiveReleaseModelFlow(dataItemOutputDetailFlow model)
        {

            if (model.ItemStatuGUID == null || model.ItemStatuGUID == Guid.Empty)
            {
                ModelState.AddModelError("LanguageID", "Kindly select item status");
                return (false);
            }
            //int LanguageID = DbWMS.dataItemOutputDetailFlow
            //    .Where(x =>

            //        x.ItemOutputDetailGUID == model.ItemOutputDetailGUID &&
            //        x.Active == true).Count();
            //if (LanguageID > 0)
            //{
            //    ModelState.AddModelError("LanguageID", "Model Already exists");
            //}

            return (true);
        }

        #endregion

        #region Release Single Item

        public ActionResult ReleaseSingleItemCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }

            var dataItemInputDetail = DbWMS.dataItemInputDetail.Find(FK);
            if (dataItemInputDetail.IsAvaliable == false)
            {

                ModelState.AddModelError("LanguageID", "Model is not avaiable for releasing ");
                return PartialView("~/Areas/WMS/Views/ReleaseModelDetails/_ReleaseModelDetailUpdateModal.cshtml");
            }

            ReleaseSingleItemUpdateModalUpdateModel model = new ReleaseSingleItemUpdateModalUpdateModel
            {
                ItemInputDetailGUID = FK,
                ItemOutputDetailGUID = Guid.Empty,
            };

            return PartialView("~/Areas/WMS/Views/ReleaseModelDetails/_ReleaseSingleItemUpdateModal.cshtml",
                model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ReleaseSingleItemCreate(ReleaseSingleItemUpdateModalUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }

            //if (ActiveReleaseModelDetail(ReleaseModelDetail)) return PartialView("~/Areas/WMS/Views/ReleaseModelDetails/_ReleaseModelDetailUpdateModal.cshtml", model);
          

            Guid EntityPK = Guid.NewGuid();
            DateTime ExecutionTime = DateTime.Now;
            dataItemOutput ReleaseModel = Mapper.Map(model, new dataItemOutput());
            ReleaseModel.ItemOutputGUID = EntityPK;
            ReleaseModel.CreatedByGUID = UserGUID;
            ReleaseModel.CreatedDate = ExecutionTime;
            ReleaseModel.OutputNumber = DbWMS.dataItemOutput.Select(x => x.OutputNumber).Max() + 1 ?? 1;
            DbWMS.Create(ReleaseModel, Permissions.WarehouseItemsRelease.CreateGuid, ExecutionTime, DbCMS);
            dataItemOutputDetail ReleaseModelDetail = Mapper.Map(model, new dataItemOutputDetail());

  
            ReleaseModelDetail.ItemOutputDetailGUID = Guid.NewGuid();
            if (ReleaseModelDetail.RequestedQunatity == null)
                ReleaseModelDetail.RequestedQunatity = 1;
            ReleaseModelDetail.CreatedByGUID = UserGUID;
            ReleaseModelDetail.ItemOutputGUID = ReleaseModel.ItemOutputGUID;
            ReleaseModelDetail.CreatedDate = ExecutionTime;
            DbWMS.Create(ReleaseModelDetail, Permissions.WarehouseItemsRelease.CreateGuid, ExecutionTime, DbCMS);
      

            dataItemInputDetail InputDetail = DbWMS.dataItemInputDetail.Find(ReleaseModelDetail.ItemInputDetailGUID);
            //dataItemOutput itemOutput = DbWMS.dataItemOutput.Find(ReleaseModelDetail.ItemOutputGUID);
            InputDetail.IsAvaliable = false;
            InputDetail.LastCustdianGUID = (Guid)ReleaseModel.RequesterGUID;
            InputDetail.LastCustdianNameGUID = ReleaseModel.RequesterNameGUID;

            DbWMS.Update(InputDetail, Permissions.WarehouseItemsRelease.UpdateGuid, ExecutionTime, DbCMS);

            List<dataItemOutputDetailFlow> flows = DbWMS.dataItemOutputDetailFlow
                .Where(x => x.dataItemOutputDetail.dataItemInputDetail.ItemInputDetailGUID == ReleaseModelDetail.ItemInputDetailGUID).ToList();

            foreach (var item in flows)
            {
                item.IsLastAction = false;

            }
            DbWMS.UpdateBulk(flows, Permissions.WarehouseItemsEntry.UpdateGuid, ExecutionTime, DbCMS);

            dataItemOutputDetailFlow outputDetailFlow = new dataItemOutputDetailFlow();

            outputDetailFlow.ItemOutputDetailFlowGUID = Guid.NewGuid();
            outputDetailFlow.ItemOutputDetailGUID = ReleaseModelDetail.ItemOutputDetailGUID;
            outputDetailFlow.FlowTypeGUID = WarehouseRequestFlowType.PendingConfirmed;
            outputDetailFlow.ItemStatuGUID = WarehouseItemStatus.Functionting;
            outputDetailFlow.CreatedDate = DateTime.Now;
            outputDetailFlow.IsLastAction = true;
            outputDetailFlow.IsLastMove = true;
            outputDetailFlow.CreatedByGUID = UserGUID;
            outputDetailFlow.Active = true;


            DbWMS.Create(outputDetailFlow, Permissions.WarehouseItemsEntry.CreateGuid, ExecutionTime, DbCMS);



            if (ReleaseModel.RequesterGUID == WarehouseRequestSourceTypes.Warehouse)
            {
                var CurrentWarehouseGUID = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).Select(x => x.WarehouseGUID)
                    .FirstOrDefault();
                List<dataItemTransfer> allPriviousTransfers = DbWMS.dataItemTransfer
                    .Where(x => x.ItemInputDetailGUID == InputDetail.ItemInputDetailGUID).ToList();
                foreach (var item in allPriviousTransfers)
                {
                    item.IsLastTransfer = false;
                }
                DbWMS.UpdateBulk(allPriviousTransfers, Permissions.WarehouseItemsEntry.UpdateGuid, ExecutionTime, DbCMS);

                dataItemTransfer TransferModel = new dataItemTransfer();
                TransferModel.ItemTransferGUID = Guid.NewGuid();
                TransferModel.ItemInputDetailGUID = ReleaseModelDetail.ItemInputDetailGUID;
                TransferModel.TransferDate = ExecutionTime;
                TransferModel.SourceGUID = CurrentWarehouseGUID;
                TransferModel.DestionationGUID = ReleaseModel.RequesterNameGUID;
                TransferModel.TransferedByGUID = UserGUID;
                TransferModel.IsLastTransfer = true;

                DbWMS.Create(TransferModel, Permissions.WarehouseItemsEntry.CreateGuid, ExecutionTime, DbCMS);

            }

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleUpdateMessage(DataTableNames.WarehosueModelMovementsDataTable, DbWMS.PrimaryKeyControl(ReleaseModelDetail), DbWMS.RowVersionControls(Portal.SingleToList(ReleaseModelDetail))));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

      
        #endregion
    }
}