using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.ViewModels;
using AutoMapper;
using LinqKit;
using PMD_DAL.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace AppsPortal.Areas.PMD.Controllers
{
    public class ItemsTransferController : PMDBaseController
    {
        #region ItemsTransfer

        [Route("PMD/ItemsTransfer/")]
        public ActionResult ItemsTransferIndex()
        {
            if (!CMS.HasAction(Permissions.PMDItemsTransfer.Access, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/PMD/Views/ItemsTransfer/Index.cshtml");
        }

        [Route("PMD/ItemsTransferDataTable/")]
        public JsonResult ItemsTransferDataTable(DataTableRecievedOptions options)
        {

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<ItemsTransferDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<ItemsTransferDataTableModel>(DataTable.Filters);
            }

            
            var All = (from a in DbPMD.dataPMDItemsTransfer.AsExpandable()
                       join b in DbPMD.codeOrganizationsInstancesLanguages.Where(x => x.Active && x.LanguageID==LAN ) on a.FromOrganizationInstanceGUID equals b.OrganizationInstanceGUID into LJ1
                       from R1 in LJ1.DefaultIfEmpty()
                       join c in DbPMD.codeOrganizationsInstancesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.ToOrganizationInstanceGUID equals c.OrganizationInstanceGUID into LJ2
                       from R2 in LJ2.DefaultIfEmpty()
                       join d in DbPMD.codeOchaLocationGovernorate on a.FromGovernorateGUID equals d.GovernorateGUID
                       join e in DbPMD.codeOchaLocationGovernorate on a.ToGovernorateGUID equals e.GovernorateGUID

                       select new ItemsTransferDataTableModel
                       {
                           ItemsTransferDate = a.ItemsTransferDate,
                           Fromadmin1Pcode = d.admin1Name_en,
                           //Fromadmin4Pcode = d.admin4Name_en,
                           FromOrganizationInstanceGUID = R1.OrganizationInstanceGUID.ToString(),
                           FromOrganizationInstanceDescription = R1.OrganizationInstanceDescription,
                           FromGovernorateGUID=d.admin1Pcode,

                           Toadmin1Pcode = e.admin1Name_en,
                           //Toadmin4Pcode = e.admin4Name_en,
                           ToOrganizationInstanceGUID = R2.OrganizationInstanceGUID.ToString(),
                           ToOrganizationInstanceDescription = R2.OrganizationInstanceDescription,
                           ToGovernorateGUID=e.admin1Pcode,
                           
                           ItemsTransferGUID = a.ItemsTransferGUID,
                           Active = a.Active,
                           dataPMDItemsTransferRowVersion = a.dataPMDItemsTransferRowVersion,
                       }).Where(Predicate).Distinct();

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);
            List<ItemsTransferDataTableModel> Result = Mapper.Map<List<ItemsTransferDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("PMD/ItemsTransfer/Create/")]
        public ActionResult ItemsTransferCreate()
        {
            if (!CMS.HasAction(Permissions.PMDItemsTransfer.Create, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/PMD/Views/ItemsTransfer/ItemsTransfer.cshtml", new ItemsTransferUpdateModel());
        }


        [Route("PMD/ItemsTransfer/Update/{PK}")]
        public ActionResult ItemsTransferUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.PMDItemsTransfer.Access, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            var model = (from x in DbPMD.dataPMDItemsTransfer.Where(x => x.ItemsTransferGUID == PK)
                         select new ItemsTransferUpdateModel
                         {
                             ItemsTransferDate = x.ItemsTransferDate,
                             Comments = x.Comments,
                             ItemsTransferGUID = x.ItemsTransferGUID,

                             Fromadmin1Pcode = x.Fromadmin1Pcode,
                             Fromadmin4Pcode = x.Fromadmin4Pcode,
                             FromOrganizationInstanceGUID = x.FromOrganizationInstanceGUID,
                             FromGovernorateGUID = x.FromGovernorateGUID,
                             FromPmdWarehouseGUID = x.FromPmdWarehouseGUID,

                             Toadmin1Pcode = x.Toadmin1Pcode,
                             Toadmin4Pcode = x.Toadmin4Pcode,
                             ToOrganizationInstanceGUID = x.ToOrganizationInstanceGUID,
                             ToGovernorateGUID = x.ToGovernorateGUID,
                             ToPmdWarehouseGUID=x.ToPmdWarehouseGUID,

                             
                             Active = x.Active,
                             dataPMDItemsTransferRowVersion = x.dataPMDItemsTransferRowVersion,
                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("ItemsTransfer", "", new { Area = "PMD" }));
            return View("~/Areas/PMD/Views/ItemsTransfer/ItemsTransfer.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ItemsTransferCreate(ItemsTransferUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.PMDItemsTransfer.Create, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            if (!ModelState.IsValid || ActiveItemsTransfer(model)) return PartialView("~/Areas/PMD/Views/ItemsTransfer/_ItemsTransferForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;
            Guid EntityPK = Guid.NewGuid();
            if (model.ItemsTransferGUID != Guid.Empty)
            {
                EntityPK = model.ItemsTransferGUID;
            }

            dataPMDItemsTransfer ItemsTransfer = Mapper.Map(model, new dataPMDItemsTransfer());
            ItemsTransfer.ItemsTransferGUID = EntityPK;
            ItemsTransfer.ToGovernorateGUID = (Guid)DbPMD.codeOchaLocationGovernorate.Where(x => x.admin1Pcode == model.Toadmin1Pcode).FirstOrDefault().GovernorateGUID;
            ItemsTransfer.FromGovernorateGUID = (Guid)DbPMD.codeOchaLocationGovernorate.Where(x => x.admin1Pcode == model.Fromadmin1Pcode).FirstOrDefault().GovernorateGUID;


            DbPMD.Create(ItemsTransfer, Permissions.PMDItemsTransfer.CreateGuid, ExecutionTime, DbCMS);


            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.ItemsTransferDetailDataTable, ControllerContext, "ItemsTransferDetailContainer"));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.PMDItemsTransfer.Create, Apps.PMD, new UrlHelper(Request.RequestContext).Action("ItemsTransfer/Create", "", new { Area = "PMD" })), Container = "ItemsTransferFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.PMDItemsTransfer.Update, Apps.PMD), Container = "ItemsTransferFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.PMDItemsTransfer.Delete, Apps.PMD), Container = "ItemsTransferFormControls" });

            try
            {
                DbPMD.SaveChanges();
                return Json(DbPMD.SingleCreateMessage(DbPMD.PrimaryKeyControl(ItemsTransfer), null, Partials, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbPMD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ItemsTransferUpdate(ItemsTransferUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.PMDItemsTransfer.Update, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            if (!ModelState.IsValid || ActiveItemsTransfer(model)) return PartialView("~/Areas/PMD/Views/ItemsTransfer/_ItemsTransferForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            dataPMDItemsTransfer ItemsTransfer = Mapper.Map(model, new dataPMDItemsTransfer());
            ItemsTransfer.ToGovernorateGUID = (Guid)DbPMD.codeOchaLocationGovernorate.Where(x => x.admin1Pcode == model.Toadmin1Pcode).FirstOrDefault().GovernorateGUID;
            ItemsTransfer.FromGovernorateGUID = (Guid)DbPMD.codeOchaLocationGovernorate.Where(x => x.admin1Pcode == model.Fromadmin1Pcode).FirstOrDefault().GovernorateGUID;

            DbPMD.Update(ItemsTransfer, Permissions.PMDItemsTransfer.UpdateGuid, ExecutionTime, DbCMS);

            var ItemsTransferDetail = DbPMD.dataPMDItemsTransferDetail.Where(x => x.ItemsTransferGUID == model.ItemsTransferGUID &&  x.Active).FirstOrDefault();

           
            try
            {
                DbPMD.SaveChanges();
                return Json(DbPMD.SingleUpdateMessage(null, null, DbPMD.RowVersionControls(ItemsTransfer, ItemsTransferDetail)));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyItemsTransfer(model.ItemsTransferGUID);
            }
            catch (Exception ex)
            {
                return Json(DbPMD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ItemsTransferDelete(dataPMDItemsTransfer model)
        {
            if (!CMS.HasAction(Permissions.PMDItemsTransfer.Delete, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            List<dataPMDItemsTransfer> DeletedItemsTransfer = DeleteItemsTransfer(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.PMDItemsTransfer.Restore, Apps.PMD), Container = "ItemsTransferFormControls" });

            try
            {
                int CommitedRows = DbPMD.SaveChanges();
                return Json(DbPMD.SingleDeleteMessage(CommitedRows, DeletedItemsTransfer.FirstOrDefault(), "ItemsTransferDetailContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyItemsTransfer(model.ItemsTransferGUID);
            }
            catch (Exception ex)
            {
                return Json(DbPMD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ItemsTransferRestore(dataPMDItemsTransfer model)
        {
            if (!CMS.HasAction(Permissions.PMDItemsTransfer.Restore, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            if (ActiveItemsTransfer(model))
            {
                return Json(DbPMD.RecordExists());
            }

            List<dataPMDItemsTransfer> RestoredItemsTransfer = RestoreItemsTransfer(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.PMDItemsTransfer.Create, Apps.PMD, new UrlHelper(Request.RequestContext).Action("ItemsTransfer/Create", "", new { Area = "PMD" })), Container = "ItemsTransferFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.PMDItemsTransfer.Update, Apps.PMD), Container = "ItemsTransferFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.PMDItemsTransfer.Restore, Apps.PMD), Container = "ItemsTransferFormControls" });

            try
            {
                int CommitedRows = DbPMD.SaveChanges();
                return Json(DbPMD.SingleRestoreMessage(CommitedRows, RestoredItemsTransfer, DbPMD.PrimaryKeyControl(RestoredItemsTransfer.FirstOrDefault()), Url.Action(DataTableNames.ItemsTransferDetailDataTable, Portal.GetControllerName(ControllerContext)), "ItemsTransferDetailContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyItemsTransfer(model.ItemsTransferGUID);
            }
            catch (Exception ex)
            {
                return Json(DbPMD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ItemsTransferDataTableDelete(List<dataPMDItemsTransfer> models)
        {
            if (!CMS.HasAction(Permissions.PMDItemsTransfer.Delete, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataPMDItemsTransfer> DeletedItemsTransfer = DeleteItemsTransfer(models);

            try
            {
                DbPMD.SaveChanges();
                return Json(DbPMD.PartialDeleteMessage(DeletedItemsTransfer, models, DataTableNames.ItemsTransferDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbPMD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ItemsTransferDataTableRestore(List<dataPMDItemsTransfer> models)
        {
            if (!CMS.HasAction(Permissions.PMDItemsTransfer.Restore, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataPMDItemsTransfer> RestoredItemsTransfer = RestoreItemsTransfer(models);

            try
            {
                DbPMD.SaveChanges();
                return Json(DbPMD.PartialRestoreMessage(RestoredItemsTransfer, models, DataTableNames.ItemsTransferDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbPMD.ErrorMessage(ex.Message));
            }
        }

        private List<dataPMDItemsTransfer> DeleteItemsTransfer(List<dataPMDItemsTransfer> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<dataPMDItemsTransfer> DeletedItemsTransfer = new List<dataPMDItemsTransfer>();

            //THIS IS FOR PERMISSION TEST//
            //DO NOT DELETE THIS CODE
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = (from a in DbPMD.dataPMDItemsTransfer
            //                    from f in DbPMD.dataPMDItemsTransferFactorForTest
            //                    where a.ItemsTransferGUID == f.ItemsTransferGUID
            //                    select new { a.ItemsTransferGUID, a.dataPMDItemsTransferRowVersion, f.OperationGUID, f.OrganizationGUID, f.CountryGUID }).AsQueryable().ToString();

            string query = DbPMD.QueryBuilder(models, Permissions.PMDItemsTransfer.DeleteGuid, SubmitTypes.Delete, "");

            var Records = DbPMD.Database.SqlQuery<dataPMDItemsTransfer>(query).ToList();
            foreach (var record in Records)
            {
                DeletedItemsTransfer.Add(DbPMD.Delete(record, ExecutionTime, Permissions.PMDItemsTransfer.DeleteGuid, DbCMS));
            }

            var ItemsTransferDetail = DeletedItemsTransfer.SelectMany(a => a.dataPMDItemsTransferDetail).Where(l => l.Active).ToList();
            foreach (var itemsTransferDetail in ItemsTransferDetail)
            {
                DbPMD.Delete(itemsTransferDetail, ExecutionTime, Permissions.PMDItemsTransfer.DeleteGuid, DbCMS);
            }
            return DeletedItemsTransfer;
        }

        private List<dataPMDItemsTransfer> RestoreItemsTransfer(List<dataPMDItemsTransfer> models)
        {
            Guid DeleteActionGUID = Permissions.PMDItemsTransfer.DeleteGuid;
            Guid RestoreActionGUID = Permissions.PMDItemsTransfer.RestoreGuid;
            DateTime RestoringTime = DateTime.Now;

            List<dataPMDItemsTransfer> RestoredItemsTransfer = new List<dataPMDItemsTransfer>();

            //THIS IS FOR PERMISSION TEST//
            //DO NOT DELETE THIS CODE
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = (from a in DbPMD.dataPMDItemsTransfer
            //                    from f in DbPMD.dataPMDItemsTransferFactorForTest
            //                    where a.ItemsTransferGUID == f.ItemsTransferGUID
            //                    select new
            //                    {
            //                        a.ItemsTransferGUID,
            //                        a.dataPMDItemsTransferRowVersion,
            //                        C2 = f.OperationGUID + "," + f.OrganizationGUID,
            //                    }).AsQueryable().ToString();//.Replace("[C2]", "[FactorsToken]");

            string query = DbPMD.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Restore, "");


            var Records = DbPMD.Database.SqlQuery<dataPMDItemsTransfer>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveItemsTransfer(record))
                {
                    RestoredItemsTransfer.Add(DbPMD.Restore(record, DeleteActionGUID, RestoreActionGUID, RestoringTime, DbCMS));
                }
            }

            var ItemsTransferDetail = RestoredItemsTransfer.SelectMany(x => x.dataPMDItemsTransferDetail.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var itemsTransferDetail in ItemsTransferDetail)
            {
               
                    DbPMD.Restore(itemsTransferDetail, DeleteActionGUID, RestoreActionGUID, RestoringTime, DbCMS);
                
            }

            return RestoredItemsTransfer;
        }

        private JsonResult ConcrrencyItemsTransfer(Guid PK)
        {
            ItemsTransferUpdateModel dbModel = new ItemsTransferUpdateModel();

            var ItemsTransfer = DbPMD.dataPMDItemsTransfer.Where(a => a.ItemsTransferGUID == PK).FirstOrDefault();
            var dbItemsTransfer = DbPMD.Entry(ItemsTransfer).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbItemsTransfer, dbModel);

            var ItemsTransferDetail = DbPMD.dataPMDItemsTransferDetail.Where(x => x.ItemsTransferGUID == PK).Where(p => (p.Active == true ? p.Active : p.DeletedOn == p.dataPMDItemsTransfer.DeletedOn) ).FirstOrDefault();
            var dbItemsTransferDetail = DbPMD.Entry(ItemsTransferDetail).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbItemsTransferDetail, dbModel);

            if (ItemsTransfer.dataPMDItemsTransferRowVersion.SequenceEqual(dbModel.dataPMDItemsTransferRowVersion) && ItemsTransferDetail.dataPMDItemsTransferDetailRowVersion.SequenceEqual(dbModel.dataPMDItemsTransferRowVersion))
            {
                return Json(DbPMD.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbPMD, dbModel, "ItemsTransferDetailContainer"));
        }

        private bool ActiveItemsTransfer(Object model)
        {
            dataPMDItemsTransfer ItemsTransfer = Mapper.Map(model, new dataPMDItemsTransfer());
            dataPMDItemsTransferDetail ItemsTransferDetail = Mapper.Map(model, new dataPMDItemsTransferDetail());

          
          
            return false;
        }

        public ActionResult ItemsTransferAuditHistory(Guid RecordGUID)
        {
            List<Guid> ChildrenGUIDs = new List<Guid>();

            DbPMD.dataPMDItemsTransferDetail.AsNoTracking().Where(x => x.ItemsTransferGUID == RecordGUID).ToList().ForEach(x => ChildrenGUIDs.Add(x.ItemsTransferDetailGUID));
            ChildrenGUIDs.Add(RecordGUID);

            List<Guid> ChildrenActions = new List<Guid>
            {
                Permissions.PMDItemsTransfer.UpdateGuid
            };
            return new AppsPortal.Controllers.AuditController().GetAuditHistoryGlobalizationVersion(RecordGUID, ChildrenActions, ChildrenGUIDs);
        }

        #endregion 

        #region ItemsTransfer ItemsTransferDetail
        public ActionResult ItemsTransferDetailDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/PMD/Views/ItemsTransfer/_ItemsTransferDetailDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<dataPMDItemsTransferDetail, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<dataPMDItemsTransferDetail>(DataTable.Filters);
            }

            var Result =(from a in DbPMD.dataPMDItemsTransferDetail.AsNoTracking().AsExpandable().Where(x => x.ItemsTransferGUID == PK).Where(Predicate)
                         join b in DbPMD.codePmd2023UnitOfAchievementLanguages.Where(x=>x.LanguageID==LAN && x.Active) on a.UnitOfAchievementGUID equals b.UnitOfAchievementGUID 
                              select new 
                              {
                                  a.Quantity,
                                  b.UnitOfAchievementDescription,
                                  a.ItemsTransferDetailGUID,
                                  a.dataPMDItemsTransferDetailRowVersion
                              });

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result.ToList()), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ItemsTransferDetailCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.PMDItemsTransfer.Create, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/PMD/Views/ItemsTransfer/_ItemsTransferDetailUpdateModal.cshtml",
                new dataPMDItemsTransferDetail { ItemsTransferGUID = FK });
        }

        public ActionResult ItemsTransferDetailUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.PMDItemsTransfer.Access, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/PMD/Views/ItemsTransfer/_ItemsTransferDetailUpdateModal.cshtml", DbPMD.dataPMDItemsTransferDetail.Find(PK));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ItemsTransferDetailCreate(dataPMDItemsTransferDetail model)
        {
            if (!CMS.HasAction(Permissions.PMDItemsTransfer.Create, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveItemsTransferDetail(model)) return PartialView("~/Areas/PMD/Views/ItemsTransfer/_ItemsTransferDetailUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbPMD.Create(model, Permissions.PMDItemsTransfer.CreateGuid, ExecutionTime,DbCMS);

            try
            {
                DbPMD.SaveChanges();
                return Json(DbPMD.SingleUpdateMessage(DataTableNames.ItemsTransferDetailDataTable,
                   DbPMD.PrimaryKeyControl(model),
                   DbPMD.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbPMD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ItemsTransferDetailUpdate(dataPMDItemsTransferDetail model)
        {
            if (!CMS.HasAction(Permissions.PMDItemsTransfer.Update, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveItemsTransferDetail(model)) return PartialView("~/Areas/PMD/Views/ItemsTransfer/_ItemsTransferDetailUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbPMD.Update(model, Permissions.PMDItemsTransfer.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbPMD.SaveChanges();
                return Json(DbPMD.SingleUpdateMessage(DataTableNames.ItemsTransferDetailDataTable,
                    DbPMD.PrimaryKeyControl(model),
                    DbPMD.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyItemsTransferDetail(model.ItemsTransferDetailGUID);
            }
            catch (Exception ex)
            {
                return Json(DbPMD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ItemsTransferDetailDelete(dataPMDItemsTransferDetail model)
        {
            if (!CMS.HasAction(Permissions.PMDItemsTransfer.Delete, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataPMDItemsTransferDetail> DeletedItemsTransferDetail = DeleteItemsTransferDetail(new List<dataPMDItemsTransferDetail> { model });

            try
            {
                DbPMD.SaveChanges();
                return Json(DbPMD.SingleDeleteMessage(DeletedItemsTransferDetail, DataTableNames.ItemsTransferDetailDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyItemsTransferDetail(model.ItemsTransferGUID);
            }
            catch (Exception ex)
            {
                return Json(DbPMD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ItemsTransferDetailRestore(dataPMDItemsTransferDetail model)
        {
            if (!CMS.HasAction(Permissions.PMDItemsTransfer.Restore, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveItemsTransferDetail(model))
            {
                return Json(DbPMD.RecordExists());
            }

            List<dataPMDItemsTransferDetail> RestoredItemsTransferDetail = RestoreItemsTransferDetail(Portal.SingleToList(model));

            try
            {
                DbPMD.SaveChanges();
                return Json(DbPMD.SingleRestoreMessage(RestoredItemsTransferDetail, DataTableNames.ItemsTransferDetailDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyItemsTransferDetail(model.ItemsTransferDetailGUID);
            }
            catch (Exception ex)
            {
                return Json(DbPMD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ItemsTransferDetailDataTableDelete(List<dataPMDItemsTransferDetail> models)
        {
            if (!CMS.HasAction(Permissions.PMDItemsTransfer.Delete, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataPMDItemsTransferDetail> DeletedItemsTransferDetail = DeleteItemsTransferDetail(models);

            try
            {
                DbPMD.SaveChanges();
                return Json(DbPMD.PartialDeleteMessage(DeletedItemsTransferDetail, models, DataTableNames.ItemsTransferDetailDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbPMD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ItemsTransferDetailDataTableRestore(List<dataPMDItemsTransferDetail> models)
        {

            if (!CMS.HasAction(Permissions.PMDItemsTransfer.Restore, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataPMDItemsTransferDetail> RestoredItemsTransferDetail = RestoreItemsTransferDetail(models);

            try
            {
                DbPMD.SaveChanges();
                return Json(DbPMD.PartialRestoreMessage(RestoredItemsTransferDetail, models, DataTableNames.ItemsTransferDetailDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbPMD.ErrorMessage(ex.Message));
            }
        }

        private List<dataPMDItemsTransferDetail> DeleteItemsTransferDetail(List<dataPMDItemsTransferDetail> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<dataPMDItemsTransferDetail> DeletedItemsTransferDetail = new List<dataPMDItemsTransferDetail>();

            string query = DbPMD.QueryBuilder(models, Permissions.PMDItemsTransfer.DeleteGuid, SubmitTypes.Delete, "");

            var ItemsTransferDetail = DbPMD.Database.SqlQuery<dataPMDItemsTransferDetail>(query).ToList();

            foreach (var itemsTransferDetail in ItemsTransferDetail)
            {
                DeletedItemsTransferDetail.Add(DbPMD.Delete(itemsTransferDetail, ExecutionTime, Permissions.PMDItemsTransfer.DeleteGuid, DbCMS));
            }

            return DeletedItemsTransferDetail;
        }

        private List<dataPMDItemsTransferDetail> RestoreItemsTransferDetail(List<dataPMDItemsTransferDetail> models)
        {
            Guid DeleteActionGUID = Permissions.PMDItemsTransfer.DeleteGuid;
            Guid RestoreActionGUID = Permissions.PMDItemsTransfer.RestoreGuid;
            DateTime RestoringTime = DateTime.Now;

            List<dataPMDItemsTransferDetail> RestoredItemsTransferDetail = new List<dataPMDItemsTransferDetail>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbPMD.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Restore, baseQuery);

            var ItemsTransferDetail = DbPMD.Database.SqlQuery<dataPMDItemsTransferDetail>(query).ToList();
            foreach (var itemsTransferDetail in ItemsTransferDetail)
            {
                if (!ActiveItemsTransferDetail(itemsTransferDetail))
                {
                    RestoredItemsTransferDetail.Add(DbPMD.Restore(itemsTransferDetail, DeleteActionGUID, RestoreActionGUID, RestoringTime, DbCMS));
                }
            }

            return RestoredItemsTransferDetail;
        }

        private JsonResult ConcrrencyItemsTransferDetail(Guid PK)
        {
            dataPMDItemsTransferDetail dbModel = new dataPMDItemsTransferDetail();

            var ItemsTransferDetail = DbPMD.dataPMDItemsTransferDetail.Where(x => x.ItemsTransferDetailGUID == PK).FirstOrDefault();
            var dbItemsTransferDetail = DbPMD.Entry(ItemsTransferDetail).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbItemsTransferDetail, dbModel);

            if (ItemsTransferDetail.dataPMDItemsTransferDetailRowVersion.SequenceEqual(dbModel.dataPMDItemsTransferDetailRowVersion))
            {
                return Json(DbPMD.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbPMD, dbModel, "ItemsTransferDetailContainer"));
        }

        private bool ActiveItemsTransferDetail(dataPMDItemsTransferDetail model)
        {
            int ItemsTransferDetailID = DbPMD.dataPMDItemsTransferDetail
                                  .Where(l => 
                                              l.ItemsTransferGUID == model.ItemsTransferGUID &&
                                              l.UnitOfAchievementGUID == model.UnitOfAchievementGUID &&
                                              l.Active).Count();
            if (ItemsTransferDetailID > 0)
            {
                ModelState.AddModelError("ItemsTransferDetailID", "Items already exists"); //From resource ?????? Amer  
            }

            return (ItemsTransferDetailID > 0);
        }

        #endregion

        #region Bulk Items ItemsTransfer
        public ActionResult ItemsTransferBlukItems(Guid FK)
        {
            if (!CMS.HasAction(Permissions.PMDItemsTransfer.Create, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var model = new ItemsTransferBlukItemUpdateModel()
            {
                ItemsTransferGUID = FK,
                ItemsTransferDetails = (from a in DbPMD.codePmd2023UnitOfAchievementLanguages
                                   join b in DbPMD.codePmd2023UnitOfAchievement on a.UnitOfAchievementGUID equals b.UnitOfAchievementGUID
                                   join c in DbPMD.dataPMDItemsTransferDetail.Where(x => x.ItemsTransferGUID == FK && x.Active) on b.UnitOfAchievementGUID equals c.UnitOfAchievementGUID into LJ1
                                   from R1 in LJ1.DefaultIfEmpty()
                                   where a.LanguageID == "EN" && b.UnitOfAchievementCategory == "CRI"
                                   select
                    new PMDItemsTransferItmesDetail
                    {
                        UnitOfAchievementGUID = a.UnitOfAchievementGUID,
                        ItemsTransferDetailGUID = R1.ItemsTransferDetailGUID,
                        UnitOfAchievementGroupingDescription = a.UnitOfAchievementGroupingDescription,
                        UnitOfAchievementDescription = a.UnitOfAchievementDescription,
                        UnitOfAchievementGuidance = a.UnitOfAchievementGuidance,
                        UnitOfAchievementOrder = b.UnitOfAchievementOrder,
                        MeasurementTotal = R1 != null ? R1.Quantity : 0
                    }
                ).Distinct().OrderBy(x => x.UnitOfAchievementOrder).ToList()
            };
            return PartialView("~/Areas/PMD/Views/ItemsTransfer/_ItemsTransferBulkDetailUpdateModal.cshtml", model);

        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ItemsTransferBlukItemsCreate(ItemsTransferBlukItemUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.PMDItemsTransfer.Create, Apps.PMD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid) return PartialView("~/Areas/PMD/Views/ItemsTransfer/_ItemsTransferBulkDetailUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;
            var ItemsTransfer = DbPMD.dataPMDItemsTransfer.Where(x => x.ItemsTransferGUID == model.ItemsTransferGUID).FirstOrDefault();
            foreach (var item in model.ItemsTransferDetails)
            {

                if (item.MeasurementTotal > 0)
                {
                    if (item.ItemsTransferDetailGUID == null)
                    {
                        dataPMDItemsTransferDetail Destination = new dataPMDItemsTransferDetail();
                        Destination.ItemsTransferDetailGUID = Guid.NewGuid();
                        Destination.Quantity = item.MeasurementTotal;
                        Destination.UnitOfAchievementGUID = item.UnitOfAchievementGUID;
                        Destination.Active = true;
                        Destination.ItemsTransferGUID = model.ItemsTransferGUID;

                        DbPMD.Create(Destination, Permissions.PMDItemsTransfer.CreateGuid, ExecutionTime, DbCMS);
                    }
                    else
                    {
                        dataPMDItemsTransferDetail Destination = DbPMD.dataPMDItemsTransferDetail.Where(x => x.ItemsTransferDetailGUID == item.ItemsTransferDetailGUID && x.Active).FirstOrDefault();
                        Destination.Quantity = item.MeasurementTotal;
                    }
                }
                else
                {
                    if (item.ItemsTransferDetailGUID != null)
                    {
                        dataPMDItemsTransferDetail Destination = DbPMD.dataPMDItemsTransferDetail.Where(x => x.ItemsTransferDetailGUID == item.ItemsTransferDetailGUID && x.Active).FirstOrDefault();
                        Destination.Quantity = 0;
                        Destination.Active = false;
                    }
                }
            }

            try
            {
                DbPMD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbPMD.SingleUpdateMessage(DataTableNames.ItemsTransferDetailDataTable,
                      DbPMD.PrimaryKeyControl(ItemsTransfer),
                      DbPMD.RowVersionControls(Portal.SingleToList(ItemsTransfer))));
            }
            catch (Exception ex)
            {
                return Json(DbPMD.ErrorMessage(ex.Message));
            }
        }

        #endregion
    }


}