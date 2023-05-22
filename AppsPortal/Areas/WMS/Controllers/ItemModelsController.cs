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
using AppsPortal.Library.MimeDetective;
using AppsPortal.ViewModels;
using AutoMapper;
using LinqKit;
using Microsoft.Ajax.Utilities;
using WMS_DAL.Model;
using WMS_DAL.ViewModels;

namespace AppsPortal.Areas.WMS.Controllers
{
    public class ItemModelsController : WMSBaseController
    {
        #region item org chart

        public ActionResult ItemOrgChart()
        {
            return View("~/Areas/WMS/Views/ItemClassifications/ItemOrgChart.cshtml");
        }

        public JsonResult GetItemOrgChart()
        {
            List<ItemorgChart> itemchart=new List<ItemorgChart>();
            Guid WarehouseTypeGUID = Guid.Parse("9317FBDA-E360-45CC-A064-11E6A21C1E17");
            var itemClassifications=DbWMS.codeWarehouseItemClassificationLanguage.Where(x => x.LanguageID == LAN && x.Active && x.codeWarehouseItemClassification.WarehouseTypeGUID== WarehouseTypeGUID).ToList();
            var items = DbWMS.codeWarehouseItemLanguage.Where(x => x.LanguageID == LAN && x.Active).ToList();
            var itemModels = DbWMS.codeWarehouseItemModelLanguage.Where(x => x.LanguageID == LAN && x.Active).ToList();
            int id = 0;
            int level1Id = 0;
            int level2Id = 0;

            foreach (var level1 in itemClassifications)
            {
                id = id + 1;
                level1Id = id;
               ItemorgChart itemClassification = new ItemorgChart
               {
                   id= level1Id,
                   parentID = null,
                   name=level1.WarehouseItemClassificationDescription,
                   title = "Level 1",
                   phone="",
                   

               };
                itemchart.Add(itemClassification);
                foreach (var level2 in items.Where(x=>x.codeWarehouseItem.codeWarehouseItemClassification.WarehouseItemClassificationGUID==level1.WarehouseItemClassificationGUID).ToList())
                {
                    id = id + 1;
                    level2Id = id;
                    ItemorgChart item = new ItemorgChart
                    {
                        id = id,
                        parentID = level1Id,
                        name = level2.WarehouseItemDescription,
                        title = "Level 2"

                    };
                    itemchart.Add(item);

                    foreach (var level3 in itemModels.Where(x =>
                        x.codeWarehouseItemModel.WarehouseItemGUID ==
                        level2.WarehouseItemGUID).ToList())
                    {
                        id = id + 1;
                        ItemorgChart model = new ItemorgChart
                        {
                            id = id,
                            parentID = level2Id,
                            name = level3.ModelDescription,
                            title = "Level 3"

                        };
                        itemchart.Add(model);
                    }

                }
            }
            return Json(itemchart, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Item Classification

        [Route("WMS/ItemClassifications/")]
        public ActionResult ItemClassificationIndex()
        {
            if (!CMS.HasAction(Permissions.ItemModel.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            return View("~/Areas/WMS/Views/ItemClassifications/Index.cshtml");
        }

        [Route("WMS/ItemClassificationsDataTable/")]
        public JsonResult ItemClassificationsDataTable(DataTableRecievedOptions options)
        {

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<ItemClassificaitonDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<ItemClassificaitonDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.ItemModel.CreateGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();
            //Fix
            var All = (

                from a in DbWMS.codeWarehouseItemClassification.Where(x => x.Active).AsExpandable()
                join b in DbWMS.codeWarehouseItemClassificationLanguage.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeWarehouseItemClassification.DeletedOn) && x.LanguageID == LAN) on a.WarehouseItemClassificationGUID equals b.WarehouseItemClassificationGUID into LJ1
                from R1 in LJ1.DefaultIfEmpty()
             
                select new ItemClassificaitonDataTableModel
                {
                    WarehouseItemClassificationGUID = a.WarehouseItemClassificationGUID,
                    Active = a.Active,
                    WarehouseItemClassificationDescription = R1.WarehouseItemClassificationDescription,

                    codeWarehouseItemClassificationRowVersion = a.codeWarehouseItemClassificationRowVersion
                }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<ItemClassificaitonDataTableModel> Result = Mapper.Map<List<ItemClassificaitonDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("WMS/ItemClassification/Create/")]
        public ActionResult ItemClassificationCreate()
        {
            if (!CMS.HasAction(Permissions.ItemModel.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            return View("~/Areas/WMS/Views/ItemClassifications/ItemClassification.cshtml", new ItemClassificationUpdateModel());
        }

        [Route("WMS/ItemClassification/Update/{PK}")]
        public ActionResult ItemClassificationUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.ItemModel.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            var model = (from a in DbWMS.codeWarehouseItemClassification.WherePK(PK)
                         join b in DbWMS.codeWarehouseItemClassificationLanguage.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeWarehouseItemClassification.DeletedOn) && x.LanguageID == LAN)
                         on a.WarehouseItemClassificationGUID equals b.WarehouseItemClassificationGUID into LJ1
                         from R1 in LJ1.DefaultIfEmpty()
                         select new ItemClassificationUpdateModel
                         {
                             WarehouseItemClassificationGUID = a.WarehouseItemClassificationGUID,
                             WarehouseItemClassificationDescription = R1.WarehouseItemClassificationDescription,
                         
                             Active = a.Active,
                             codeWarehouseItemClassificationRowVersion = a.codeWarehouseItemClassificationRowVersion,
                             codeWarehouseItemClassificationLanguageRowVersion = R1.codeWarehouseItemClassificationLanguageRowVersion
                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("ItemClassification", "ItemModels", new { Area = "WMS" }));

            return View("~/Areas/WMS/Views/ItemClassifications/ItemClassification.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ItemClassificationCreate(ItemClassificationUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.ItemModel.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (!ModelState.IsValid || ActiveItemClassification(model)) return PartialView("~/Areas/WMS/Views/ItemClassifications/_ItemClassificationForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            codeWarehouseItemClassification itemClassification = Mapper.Map(model, new codeWarehouseItemClassification());
            itemClassification.WarehouseItemClassificationGUID = EntityPK;
            itemClassification.WarehouseTypeGUID= Guid.Parse("9317FBDA-E360-45CC-A064-11E6A21C1E17");
            DbWMS.Create(itemClassification, Permissions.ItemModel.CreateGuid, ExecutionTime, DbCMS);

            codeWarehouseItemClassificationLanguage Language = Mapper.Map(model, new codeWarehouseItemClassificationLanguage());
            Language.WarehouseItemClassificationLanguageGUID = EntityPK;
            Language.WarehouseItemClassificationGUID = itemClassification.WarehouseItemClassificationGUID;

            DbWMS.Create(Language, Permissions.ItemModel.CreateGuid, ExecutionTime, DbCMS);



            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.ItemModelLanguagesDataTable, ControllerContext, "ItemModelLanguagesFormControls"));
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.ItemModelDeterminantsDataTable, ControllerContext, "ModelsDeterminantsFormControls"));
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.ItemModelWarehouseDataTable, ControllerContext, "ModelsWarehousesFormControls"));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.ItemModel.Create, Apps.WMS, new UrlHelper(Request.RequestContext).Action("Create", "ItemModels", new { Area = "WMS" })), Container = "ItemModelDetailFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.ItemModel.Update, Apps.WMS), Container = "ItemModelDetailFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.ItemModel.Delete, Apps.WMS), Container = "ItemModelDetailFormControls" });

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleCreateMessage(DbWMS.PrimaryKeyControl(itemClassification), DbWMS.RowVersionControls(itemClassification, Language), Partials, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ItemClassificationUpdate(ItemClassificationUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.ItemModel.Update, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (!ModelState.IsValid || ActiveItemClassification(model)) return PartialView("~/Areas/WMS/Views/ItemClassifications/_ItemClassificationForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            codeWarehouseItemClassification ItemModel = Mapper.Map(model, new codeWarehouseItemClassification());
            DbWMS.Update(ItemModel, Permissions.ItemModel.UpdateGuid, ExecutionTime, DbCMS);

            var Language = DbWMS.codeWarehouseItemClassificationLanguage.Where(l => l.WarehouseItemClassificationGUID == model.WarehouseItemClassificationGUID && l.LanguageID == LAN && l.Active).FirstOrDefault();

            if (Language == null)
            {
                Language = Mapper.Map(model, Language);
                Language.WarehouseItemClassificationGUID = ItemModel.WarehouseItemClassificationGUID;
                DbWMS.Create(Language, Permissions.ItemModel.CreateGuid, ExecutionTime, DbCMS);
            }
            else if (Language.WarehouseItemClassificationDescription != model.WarehouseItemClassificationDescription)
            {
                Language = Mapper.Map(model, Language);
                DbWMS.Update(Language, Permissions.ItemModel.UpdateGuid, ExecutionTime, DbCMS);
            }

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleUpdateMessage(null, null, DbWMS.RowVersionControls(ItemModel, Language)));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyItemClassification(model.WarehouseItemClassificationGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ItemClassificationDelete(codeWarehouseItemClassification model)
        {
            if (!CMS.HasAction(Permissions.ItemModel.Delete, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            List<codeWarehouseItemClassification> DeletedItemModel = DeleteItemClassification(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.ItemModel.Restore, Apps.WMS), Container = "ItemClassificationFormControls" });

            try
            {
                int CommitedRows = DbWMS.SaveChanges();
                DbWMS.SaveChanges();
                return Json(DbWMS.SingleDeleteMessage(CommitedRows, DeletedItemModel.FirstOrDefault(), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyItemClassification(model.WarehouseItemClassificationGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ItemClassificationRestore(codeWarehouseItemClassification model)
        {
            if (!CMS.HasAction(Permissions.ItemModel.Restore, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (ActiveItemClassification(model))
            {
                return Json(DbWMS.RecordExists());
            }

            List<codeWarehouseItemClassification> RestoredItemModel = RestoreItemClassifications(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.ItemModel.Create, Apps.WMS, new UrlHelper(Request.RequestContext).Action("ItemModelCreate", "Configuration", new { Area = "WMS" })), Container = "ItemModelFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.ItemModel.Update, Apps.WMS), Container = "ItemModelFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.ItemModel.Delete, Apps.WMS), Container = "ItemModelFormControls" });

            try
            {
                int CommitedRows = DbWMS.SaveChanges();
                DbWMS.SaveChanges();
                return Json(DbWMS.SingleRestoreMessage(CommitedRows, RestoredItemModel, DbWMS.PrimaryKeyControl(RestoredItemModel.FirstOrDefault()), Url.Action(DataTableNames.ItemClassificationsDataTable, Portal.GetControllerName(ControllerContext)), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyItemClassification(model.WarehouseItemClassificationGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ItemClassificationsDataTableDelete(List<codeWarehouseItemClassification> models)
        {
            if (!CMS.HasAction(Permissions.ItemModel.Delete, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            List<codeWarehouseItemClassification> DeletedItemClassificaiton = DeleteItemClassification(models);

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.PartialDeleteMessage(DeletedItemClassificaiton, models, DataTableNames.ItemClassificationsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ItemClassificationDataTableRestore(List<codeWarehouseItemClassification> models)
        {
            if (!CMS.HasAction(Permissions.ItemModel.Restore, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            List<codeWarehouseItemClassification> RestoredItemModel = DeleteItemClassification(models);

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.PartialRestoreMessage(RestoredItemModel, models, DataTableNames.ItemClassificationsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        private List<codeWarehouseItemClassification> DeleteItemClassification(List<codeWarehouseItemClassification> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<codeWarehouseItemClassification> DeletedItemModel = new List<codeWarehouseItemClassification>();
            //Fix
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = "SELECT WarehouseItemModelGUID,CONVERT(varchar(50), WarehouseItemModelGUID) as C2 ,codeWarehouseItemModelRowVersion FROM code.codeWarehouseItemModel where WarehouseItemModelGUID in (" + string.Join(",", models.Select(x => "'" + x.WarehouseItemModelGUID + "'").ToArray()) + ")";

            string query = DbWMS.QueryBuilder(models, Permissions.ItemModel.DeleteGuid, SubmitTypes.Delete, "");

            var Records = DbWMS.Database.SqlQuery<codeWarehouseItemClassification>(query).ToList();
            foreach (var record in Records)
            {
                DeletedItemModel.Add(DbWMS.Delete(record, ExecutionTime, Permissions.ItemModel.DeleteGuid, DbCMS));
            }

            var Languages = DeletedItemModel.SelectMany(a => a.codeWarehouseItemClassificationLanguage).Where(l => l.Active).ToList();
            foreach (var language in Languages)
            {
                DbWMS.Delete(language, ExecutionTime, Permissions.ItemModel.DeleteGuid, DbCMS);
            }
            return DeletedItemModel;
        }

        private List<codeWarehouseItemClassification> RestoreItemClassifications(List<codeWarehouseItemClassification> models)
        {

            DateTime RestoringTime = DateTime.Now;

            List<codeWarehouseItemClassification> RestoredItemClassification = new List<codeWarehouseItemClassification>();
            //Fix
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = "SELECT WarehouseItemModelGUID,CONVERT(varchar(50), WarehouseItemModelGUID) as C2 ,codeWarehouseItemModelRowVersion FROM code.codeWarehouseItemModel where WarehouseItemModelGUID in (" + string.Join(",", models.Select(x => "'" + x.WarehouseItemModelGUID + "'").ToArray()) + ")";

            string query = DbWMS.QueryBuilder(models, Permissions.ItemModel.DeleteGuid, SubmitTypes.Restore, "");

            var Records = DbWMS.Database.SqlQuery<codeWarehouseItemClassification>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveItemClassification(record))
                {
                    RestoredItemClassification.Add(DbWMS.Restore(record, Permissions.ItemModel.DeleteGuid, Permissions.ItemModel.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            var Languages = RestoredItemClassification.SelectMany(x => x.codeWarehouseItemClassificationLanguage.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var language in Languages)
            {
                DbWMS.Restore(language, Permissions.ItemModel.DeleteGuid, Permissions.ItemModel.RestoreGuid, RestoringTime, DbCMS);
            }

            return RestoredItemClassification;
        }

        private JsonResult ConcurrencyItemClassification(Guid PK)
        {
            ItemClassificationUpdateModel dbModel = new ItemClassificationUpdateModel();

            var ItemModel = DbWMS.codeWarehouseItemClassification.Where(x => x.WarehouseItemClassificationGUID == PK).FirstOrDefault();
            var dbItemModel = DbWMS.Entry(ItemModel).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbItemModel, dbModel);

            var Language = DbWMS.codeWarehouseItemClassificationLanguage.Where(x => x.WarehouseItemClassificationGUID == PK).Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeWarehouseItemClassification.DeletedOn) && x.LanguageID == LAN).FirstOrDefault();
            var dbLanguage = DbWMS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (ItemModel.codeWarehouseItemClassificationRowVersion.SequenceEqual(dbModel.codeWarehouseItemClassificationRowVersion) && Language.codeWarehouseItemClassificationLanguageRowVersion.SequenceEqual(dbModel.codeWarehouseItemClassificationLanguageRowVersion))
            {
                return Json(DbWMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbWMS, dbModel, "LanguagesContainer"));
        }

        private bool ActiveItemClassification(Object model)
        {
            codeWarehouseItemModelLanguage ItemModel = Mapper.Map(model, new codeWarehouseItemModelLanguage());
            int ModelDescription = DbWMS.codeWarehouseItemModelLanguage
                                    .Where(x => x.ModelDescription == ItemModel.ModelDescription &&
                                                x.LanguageID == LAN &&
                                                x.Active).Count();
            if (ModelDescription > 0)
            {
                ModelState.AddModelError("ModelDescription", "ItemModel is already exists");
            }
            return (ModelDescription > 0);
        }

        #endregion


        #region Items

        [Route("WMS/Items/")]
        public ActionResult ItemIndex()
        {
            if (!CMS.HasAction(Permissions.ItemModel.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            return View("~/Areas/WMS/Views/Items/Index.cshtml");
        }

        [Route("WMS/ItemsDataTable/")]
        public JsonResult ItemsDataTable(DataTableRecievedOptions options)
        {

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<ItemDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<ItemDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.ItemModel.CreateGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();
            //Fix
            var All = (

                from a in DbWMS.codeWarehouseItem.Where(x => x.Active).AsExpandable()
                join b in DbWMS.codeWarehouseItemLanguage.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeWarehouseItem.DeletedOn) && x.LanguageID == LAN) on a.WarehouseItemGUID equals b.WarehouseItemGUID into LJ1
                from R1 in LJ1.DefaultIfEmpty()
                join c in DbWMS.codeWarehouseItemClassificationLanguage.Where(x=>x.Active && x.LanguageID==LAN) on a.WarehouseItemClassificationGUID equals c.WarehouseItemClassificationGUID into LJ2
                from R2 in LJ2.DefaultIfEmpty( )
                

                select new ItemDataTableModel
                {
                    WarehouseItemGUID = a.WarehouseItemGUID,
                    Active = a.Active,
                    WarehouseItemDescription = R1.WarehouseItemDescription,
                    WarehouseItemClassificationDescription = R2.WarehouseItemClassificationDescription,
                    codeWarehouseItemRowVersion = a.codeWarehouseItemRowVersion
                }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<ItemDataTableModel> Result = Mapper.Map<List<ItemDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("WMS/Item/Create/")]
        public ActionResult ItemCreate()
        {
            if (!CMS.HasAction(Permissions.ItemModel.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            ItemUpdateModel model = new ItemUpdateModel();
            model.IsDeterminanted = true;
            return View("~/Areas/WMS/Views/Items/Item.cshtml", new ItemUpdateModel());
        }

        [Route("WMS/Item/Update/{PK}")]
        public ActionResult ItemUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.ItemModel.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            var model = (from a in DbWMS.codeWarehouseItem.WherePK(PK)
                         join b in DbWMS.codeWarehouseItemLanguage.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeWarehouseItem.DeletedOn) && x.LanguageID == LAN)
                         on a.WarehouseItemGUID equals b.WarehouseItemGUID into LJ1
                         from R1 in LJ1.DefaultIfEmpty()
                         select new ItemUpdateModel
                         {
                             WarehouseItemGUID = a.WarehouseItemGUID,
                             WarehouseItemDescription = R1.WarehouseItemDescription,
                             WarehouseItemClassificationGUID =(Guid) a.WarehouseItemClassificationGUID,
                             IsDeterminanted = (bool)a.IsDeterminanted,

                             Active = a.Active,
                             codeWarehouseItemRowVersion = a.codeWarehouseItemRowVersion,
                             codeWarehouseItemLanguageRowVersion = R1.codeWarehouseItemLanguageRowVersion
                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("Item", "ItemModels", new { Area = "WMS" }));

            return View("~/Areas/WMS/Views/Items/Item.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ItemCreate(ItemUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.ItemModel.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (!ModelState.IsValid || ActiveItem(model)) return PartialView("~/Areas/WMS/Views/Items/_ItemForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            codeWarehouseItem Item = Mapper.Map(model, new codeWarehouseItem());
            Item.WarehouseItemGUID = EntityPK;

            DbWMS.Create(Item, Permissions.ItemModel.CreateGuid, ExecutionTime, DbCMS);

            codeWarehouseItemLanguage Language = Mapper.Map(model, new codeWarehouseItemLanguage());
            Language.WarehouseItemLanguagGUID = EntityPK;
            Language.WarehouseItemGUID = Item.WarehouseItemGUID;

            DbWMS.Create(Language, Permissions.ItemModel.CreateGuid, ExecutionTime, DbCMS);



            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.ItemModelLanguagesDataTable, ControllerContext, "ItemModelLanguagesFormControls"));
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.ItemModelDeterminantsDataTable, ControllerContext, "ModelsDeterminantsFormControls"));
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.ItemModelWarehouseDataTable, ControllerContext, "ModelsWarehousesFormControls"));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.ItemModel.Create, Apps.WMS, new UrlHelper(Request.RequestContext).Action("Create", "ItemModels", new { Area = "WMS" })), Container = "ItemModelDetailFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.ItemModel.Update, Apps.WMS), Container = "ItemModelDetailFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.ItemModel.Delete, Apps.WMS), Container = "ItemModelDetailFormControls" });

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleCreateMessage(DbWMS.PrimaryKeyControl(Item), DbWMS.RowVersionControls(Item, Language), Partials, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ItemUpdate(ItemUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.ItemModel.Update, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (!ModelState.IsValid || ActiveItem(model)) return PartialView("~/Areas/WMS/Views/Items/_ItemForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            codeWarehouseItem ItemModel = Mapper.Map(model, new codeWarehouseItem());
            DbWMS.Update(ItemModel, Permissions.ItemModel.UpdateGuid, ExecutionTime, DbCMS);

            var Language = DbWMS.codeWarehouseItemLanguage.Where(l => l.WarehouseItemGUID == model.WarehouseItemGUID && l.LanguageID == LAN && l.Active).FirstOrDefault();

            if (Language == null)
            {
                Language = Mapper.Map(model, Language);
                Language.WarehouseItemGUID = ItemModel.WarehouseItemGUID;
                DbWMS.Create(Language, Permissions.ItemModel.CreateGuid, ExecutionTime, DbCMS);
            }
            else if (Language.WarehouseItemDescription != model.WarehouseItemDescription)
            {
                Language = Mapper.Map(model, Language);
                DbWMS.Update(Language, Permissions.ItemModel.UpdateGuid, ExecutionTime, DbCMS);
            }

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleUpdateMessage(null, null, DbWMS.RowVersionControls(ItemModel, Language)));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyItem(model.WarehouseItemGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ItemDelete(codeWarehouseItem model)
        {
            if (!CMS.HasAction(Permissions.ItemModel.Delete, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            List<codeWarehouseItem> DeletedItemModel = DeleteItem(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.ItemModel.Restore, Apps.WMS), Container = "ItemFormControls" });

            try
            {
                int CommitedRows = DbWMS.SaveChanges();
                DbWMS.SaveChanges();
                return Json(DbWMS.SingleDeleteMessage(CommitedRows, DeletedItemModel.FirstOrDefault(), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyItem(model.WarehouseItemGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ItemRestore(codeWarehouseItem model)
        {
            if (!CMS.HasAction(Permissions.ItemModel.Restore, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (ActiveItem(model))
            {
                return Json(DbWMS.RecordExists());
            }

            List<codeWarehouseItem> RestoredItemModel = RestoreItems(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.ItemModel.Create, Apps.WMS, new UrlHelper(Request.RequestContext).Action("ItemModelCreate", "Configuration", new { Area = "WMS" })), Container = "ItemModelFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.ItemModel.Update, Apps.WMS), Container = "ItemModelFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.ItemModel.Delete, Apps.WMS), Container = "ItemModelFormControls" });

            try
            {
                int CommitedRows = DbWMS.SaveChanges();
                DbWMS.SaveChanges();
                return Json(DbWMS.SingleRestoreMessage(CommitedRows, RestoredItemModel, DbWMS.PrimaryKeyControl(RestoredItemModel.FirstOrDefault()), Url.Action(DataTableNames.ItemsDataTable, Portal.GetControllerName(ControllerContext)), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyItem(model.WarehouseItemGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ItemsDataTableDelete(List<codeWarehouseItem> models)
        {
            if (!CMS.HasAction(Permissions.ItemModel.Delete, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            List<codeWarehouseItem> DeletedItem = DeleteItem(models);

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.PartialDeleteMessage(DeletedItem, models, DataTableNames.ItemsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ItemDataTableRestore(List<codeWarehouseItem> models)
        {
            if (!CMS.HasAction(Permissions.ItemModel.Restore, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            List<codeWarehouseItem> RestoredItemModel = DeleteItem(models);

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.PartialRestoreMessage(RestoredItemModel, models, DataTableNames.ItemsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        private List<codeWarehouseItem> DeleteItem(List<codeWarehouseItem> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<codeWarehouseItem> DeletedItemModel = new List<codeWarehouseItem>();
            //Fix
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = "SELECT WarehouseItemModelGUID,CONVERT(varchar(50), WarehouseItemModelGUID) as C2 ,codeWarehouseItemModelRowVersion FROM code.codeWarehouseItemModel where WarehouseItemModelGUID in (" + string.Join(",", models.Select(x => "'" + x.WarehouseItemModelGUID + "'").ToArray()) + ")";

            string query = DbWMS.QueryBuilder(models, Permissions.ItemModel.DeleteGuid, SubmitTypes.Delete, "");

            var Records = DbWMS.Database.SqlQuery<codeWarehouseItem>(query).ToList();
            foreach (var record in Records)
            {
                DeletedItemModel.Add(DbWMS.Delete(record, ExecutionTime, Permissions.ItemModel.DeleteGuid, DbCMS));
            }

            var Languages = DeletedItemModel.SelectMany(a => a.codeWarehouseItemLanguage).Where(l => l.Active).ToList();
            foreach (var language in Languages)
            {
                DbWMS.Delete(language, ExecutionTime, Permissions.ItemModel.DeleteGuid, DbCMS);
            }
            return DeletedItemModel;
        }

        private List<codeWarehouseItem> RestoreItems(List<codeWarehouseItem> models)
        {

            DateTime RestoringTime = DateTime.Now;

            List<codeWarehouseItem> RestoredItem = new List<codeWarehouseItem>();
            //Fix
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = "SELECT WarehouseItemModelGUID,CONVERT(varchar(50), WarehouseItemModelGUID) as C2 ,codeWarehouseItemModelRowVersion FROM code.codeWarehouseItemModel where WarehouseItemModelGUID in (" + string.Join(",", models.Select(x => "'" + x.WarehouseItemModelGUID + "'").ToArray()) + ")";

            string query = DbWMS.QueryBuilder(models, Permissions.ItemModel.DeleteGuid, SubmitTypes.Restore, "");

            var Records = DbWMS.Database.SqlQuery<codeWarehouseItem>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveItem(record))
                {
                    RestoredItem.Add(DbWMS.Restore(record, Permissions.ItemModel.DeleteGuid, Permissions.ItemModel.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            var Languages = RestoredItem.SelectMany(x => x.codeWarehouseItemLanguage.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var language in Languages)
            {
                DbWMS.Restore(language, Permissions.ItemModel.DeleteGuid, Permissions.ItemModel.RestoreGuid, RestoringTime, DbCMS);
            }

            return RestoredItem;
        }

        private JsonResult ConcurrencyItem(Guid PK)
        {
            ItemUpdateModel dbModel = new ItemUpdateModel();

            var ItemModel = DbWMS.codeWarehouseItem.Where(x => x.WarehouseItemGUID == PK).FirstOrDefault();
            var dbItemModel = DbWMS.Entry(ItemModel).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbItemModel, dbModel);

            var Language = DbWMS.codeWarehouseItemLanguage.Where(x => x.WarehouseItemGUID == PK).Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeWarehouseItem.DeletedOn) && x.LanguageID == LAN).FirstOrDefault();
            var dbLanguage = DbWMS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (ItemModel.codeWarehouseItemRowVersion.SequenceEqual(dbModel.codeWarehouseItemRowVersion) && Language.codeWarehouseItemLanguageRowVersion.SequenceEqual(dbModel.codeWarehouseItemLanguageRowVersion))
            {
                return Json(DbWMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbWMS, dbModel, "LanguagesContainer"));
        }

        private bool ActiveItem(Object model)
        {
            codeWarehouseItemModelLanguage ItemModel = Mapper.Map(model, new codeWarehouseItemModelLanguage());
            int ModelDescription = DbWMS.codeWarehouseItemModelLanguage
                                    .Where(x => x.ModelDescription == ItemModel.ModelDescription &&
                                                x.LanguageID == LAN &&
                                                x.Active).Count();
            if (ModelDescription > 0)
            {
                ModelState.AddModelError("ModelDescription", "Item Model is already exists");
            }
            return (ModelDescription > 0);
        }

        #endregion


        #region Item Models

        public ActionResult ItemModelHome()
        {
            if (!CMS.HasAction(Permissions.ItemModel.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            return View("~/Areas/WMS/Views/ItemModels/Index.cshtml");
        }


        [Route("WMS/ItemModels/")]
        public ActionResult ItemModelIndex()
        {
            if (!CMS.HasAction(Permissions.ItemModel.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            return View("~/Areas/WMS/Views/ItemModels/Index.cshtml");
        }

        [Route("WMS/ItemModelsDataTable/")]
        public JsonResult ItemModelsDataTable(DataTableRecievedOptions options)
        {
          
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<ItemModelDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<ItemModelDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.ItemModel.CreateGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();
            //Fix
            var All = (

                from a in DbWMS.codeWarehouseItemModel.Where(x=>x.Active).AsExpandable()
                join b in DbWMS.codeWarehouseItemModelLanguage.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeWarehouseItemModel.DeletedOn) && x.LanguageID == LAN) on a.WarehouseItemModelGUID equals b.WarehouseItemModelGUID into LJ1
                from R1 in LJ1.DefaultIfEmpty()
                join c in DbWMS.codeWarehouseItem on a.WarehouseItemGUID equals c.WarehouseItemGUID
                join d in DbWMS.codeWarehouseItemLanguage.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeWarehouseItem.DeletedOn) && x.LanguageID == LAN) on c.WarehouseItemGUID equals d.WarehouseItemGUID into LJ2
                from R2 in LJ2.DefaultIfEmpty()
            
                join d in DbWMS.codeBrand.Where(x =>  x.Active) on a.BrandGUID equals d.BrandGUID
                join e in DbWMS.codeBrandLanguage.Where(x=>x.Active && x.LanguageID==LAN) on d.BrandGUID equals e.BrandGUID into LJ3
                from R3 in LJ3.DefaultIfEmpty()
                join g in DbWMS.codeWarehouseItemClassification.Where(x => x.Active) on a.codeWarehouseItem.WarehouseItemClassificationGUID equals g.WarehouseItemClassificationGUID
                join h in DbWMS.codeWarehouseItemClassificationLanguage.Where(x => x.Active && x.LanguageID == LAN) on g.WarehouseItemClassificationGUID equals h.WarehouseItemClassificationGUID into LJ4
                from R4 in LJ4.DefaultIfEmpty()
                select new ItemModelDataTableModel
                {
                    WarehouseItemModelGUID = a.WarehouseItemModelGUID,
                    Active = a.Active,
                    ModelDescription = R1.ModelDescription,
                    WarehouseItemClassificationDescription = R4.WarehouseItemClassificationDescription,
                    WarehouseItemClassificationGUID = R4.WarehouseItemClassificationGUID.ToString(),
                    ItemDescription = R2.WarehouseItemDescription,
                    WarehouseItemGUID = a.WarehouseItemGUID.ToString(),
                    BrandGUID = a.BrandGUID.ToString(),
                    BrandDescription = R3.BrandDescription,
                    codeWarehouseItemModelRowVersion = a.codeWarehouseItemModelRowVersion
                }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<ItemModelDataTableModel> Result = Mapper.Map<List<ItemModelDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("WMS/ItemModels/Create/")]
        public ActionResult ItemModelCreate()
        {
            if (!CMS.HasAction(Permissions.ItemModel.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            ItemModelUpdateModel model = new ItemModelUpdateModel();
            model.WarehouseItemModelGUID = Guid.NewGuid();
            model.IsEdit = false;
            return View("~/Areas/WMS/Views/ItemModels/ItemModels.cshtml", model);
        }

        [Route("WMS/ItemModels/Update/{PK}")]
        public ActionResult ItemModelUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.ItemModel.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            var model = (from a in DbWMS.codeWarehouseItemModel.WherePK(PK)
                         join b in DbWMS.codeWarehouseItemModelLanguage.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeWarehouseItemModel.DeletedOn) && x.LanguageID == LAN)
                         on a.WarehouseItemModelGUID equals b.WarehouseItemModelGUID into LJ1
                         from R1 in LJ1.DefaultIfEmpty()
                         select new ItemModelUpdateModel
                         {
                             WarehouseItemModelGUID = a.WarehouseItemModelGUID,
                             BrandGUID = a.BrandGUID,
                             WarehouseItemGUID = a.WarehouseItemGUID,
                             ModelDescription = R1.ModelDescription,
                             WarehouseItemClassificationGUID = a.codeWarehouseItem.WarehouseItemClassificationGUID,
                             ItemModelRelationTypeGUID=a.ItemModelRelationTypeGUID,
                             ParentWarehouseItemModelGUID=a.ParentWarehouseItemModelGUID,
                             ItemImage= a.WarehouseItemModelGUID+".jpg",
                             IsEdit=true,

                             Active = a.Active,
                             codeWarehouseItemModelRowVersion = a.codeWarehouseItemModelRowVersion,
                             codeWarehouseItemModelLanguageRowVersion = R1.codeWarehouseItemModelLanguageRowVersion
                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("ItemModels", "ItemModels", new { Area = "WMS" }));

            return View("~/Areas/WMS/Views/ItemModels/ItemModels.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ItemModelCreate(ItemModelUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.ItemModel.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (!ModelState.IsValid || ActiveItemModel(model)) return PartialView("~/Areas/WMS/Views/ItemModels/_ItemModelForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            //Guid EntityPK = Guid.NewGuid();

            codeWarehouseItemModel ItemModel = Mapper.Map(model, new codeWarehouseItemModel());
            ItemModel.WarehouseItemModelGUID = model.WarehouseItemModelGUID;
            DbWMS.Create(ItemModel, Permissions.ItemModel.CreateGuid, ExecutionTime, DbCMS);

            codeWarehouseItemModelLanguage Language = Mapper.Map(model, new codeWarehouseItemModelLanguage());
            Language.WarehouseItemModelGUID = model.WarehouseItemModelGUID;

            DbWMS.Create(Language, Permissions.ItemModel.CreateGuid, ExecutionTime, DbCMS);


            //if (model.UploadModelImage != null)
            //{

            //    var filePath = Server.MapPath("~\\Uploads\\WMS\\ItemImages\\");
            //    string extension = System.IO.Path.GetExtension(model.UploadModelImage.FileName);
            //    string fullFileName = filePath + "\\" + EntityPK + extension;
            //    model.UploadModelImage.SaveAs(fullFileName);
            //}

            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(model.WarehouseItemModelGUID, DataTableNames.ItemModelLanguagesDataTable, ControllerContext, "ItemModelLanguagesFormControls"));
            Partials.Add(Portal.PartialView(model.WarehouseItemModelGUID, DataTableNames.ItemModelDeterminantsDataTable, ControllerContext, "ModelsDeterminantsFormControls"));
            Partials.Add(Portal.PartialView(model.WarehouseItemModelGUID, DataTableNames.ItemModelWarehouseDataTable, ControllerContext, "ModelsWarehousesFormControls"));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.ItemModel.Create, Apps.WMS, new UrlHelper(Request.RequestContext).Action("Create", "ItemModels", new { Area = "WMS" })), Container = "ItemModelDetailFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.ItemModel.Update, Apps.WMS), Container = "ItemModelDetailFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.ItemModel.Delete, Apps.WMS), Container = "ItemModelDetailFormControls" });

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleCreateMessage(DbWMS.PrimaryKeyControl(ItemModel), DbWMS.RowVersionControls(ItemModel, Language), Partials, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }
        public ActionResult ItemModelUploadImage(ItemModelUpdateModel model)
        {
            string error = "Error ";

            if (FileTypeValidator.IsImage(model.UploadModelImage.InputStream)
               )

            {




                if (model.UploadModelImage != null)
                {

                    var filePath = Server.MapPath("~\\Uploads\\WMS\\ItemImages\\");
                    string extension = System.IO.Path.GetExtension(model.UploadModelImage.FileName);
                    string fullFileName = filePath + "\\" + model.WarehouseItemModelGUID + extension;
                    model.UploadModelImage.SaveAs(fullFileName);
                }
                return RedirectToAction("ItemModelHome", "ItemModels", false);
            }
            return Json(DbWMS.SingleUpdateMessage(null, null, DbWMS.RowVersionControls(model, model)));

            //return Json(new {success = 1}, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult ItemModelUploadImage(ItemModelUpdateModel model)
        //{
        //    if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Create, Apps.WMS))
        //    {
        //        return Json(DbWMS.PermissionError());
        //    }

        //    if (model.UploadModelImage != null)
        //    {

        //        var filePath = Server.MapPath("~\\Uploads\\WMS\\ItemImages\\");
        //        string extension = System.IO.Path.GetExtension(model.UploadModelImage.FileName);
        //        string fullFileName = filePath + "\\" + model.WarehouseItemModelGUID + extension;
        //        model.UploadModelImage.SaveAs(fullFileName);
        //    }
        //    return RedirectToAction("ItemModelHome", "ItemModels", false);
        //    return Json(DbWMS.SingleUpdateMessage(null, null, DbWMS.RowVersionControls(model,model)));

        //    //return Json(new {success = 1}, JsonRequestBehavior.AllowGet);
        //}

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ItemModelUpdate(ItemModelUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.ItemModel.Update, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (!ModelState.IsValid ) return PartialView("~/Areas/WMS/Views/ItemModels/_ItemModelForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            codeWarehouseItemModel ItemModel = Mapper.Map(model, new codeWarehouseItemModel());
            DbWMS.Update(ItemModel, Permissions.ItemModel.UpdateGuid, ExecutionTime, DbCMS);

            var Language = DbWMS.codeWarehouseItemModelLanguage.Where(l => l.WarehouseItemModelGUID == model.WarehouseItemModelGUID && l.LanguageID == LAN && l.Active).FirstOrDefault();


            if (model.UploadModelImage != null)
            {

                var filePath = Server.MapPath("~\\Uploads\\WMS\\ItemImages\\");
                string extension = System.IO.Path.GetExtension(model.UploadModelImage.FileName);
                string fullFileName = filePath + "\\" + ItemModel.WarehouseItemModelGUID + extension;
                model.UploadModelImage.SaveAs(fullFileName);
            }

            if (Language == null)
            {
                Language = Mapper.Map(model, Language);
                Language.WarehouseItemModelGUID = ItemModel.WarehouseItemModelGUID;
                DbWMS.Create(Language, Permissions.ItemModel.CreateGuid, ExecutionTime, DbCMS);
            }
            else if (Language.ModelDescription != model.ModelDescription)
            {
                Language = Mapper.Map(model, Language);
                DbWMS.Update(Language, Permissions.ItemModel.UpdateGuid, ExecutionTime, DbCMS);
            }

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleUpdateMessage(null, null, DbWMS.RowVersionControls(ItemModel, Language)));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyItemModel(model.WarehouseItemModelGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ItemModelDelete(codeWarehouseItemModel model)
        {
            if (!CMS.HasAction(Permissions.ItemModel.Delete, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            List<codeWarehouseItemModel> DeletedItemModel = DeleteItemModel(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.ItemModel.Restore, Apps.WMS), Container = "ItemModelFormControls" });

            try
            {
                int CommitedRows = DbWMS.SaveChanges();
                DbWMS.SaveChanges();
                return Json(DbWMS.SingleDeleteMessage(CommitedRows, DeletedItemModel.FirstOrDefault(), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyItemModel(model.WarehouseItemModelGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ItemModelRestore(codeWarehouseItemModel model)
        {
            if (!CMS.HasAction(Permissions.ItemModel.Restore, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (ActiveItemModel(model))
            {
                return Json(DbWMS.RecordExists());
            }

            List<codeWarehouseItemModel> RestoredItemModel = RestoreItemModel(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.ItemModel.Create, Apps.WMS, new UrlHelper(Request.RequestContext).Action("ItemModelCreate", "Configuration", new { Area = "WMS" })), Container = "ItemModelFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.ItemModel.Update, Apps.WMS), Container = "ItemModelFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.ItemModel.Delete, Apps.WMS), Container = "ItemModelFormControls" });

            try
            {
                int CommitedRows = DbWMS.SaveChanges();
                DbWMS.SaveChanges();
                return Json(DbWMS.SingleRestoreMessage(CommitedRows, RestoredItemModel, DbWMS.PrimaryKeyControl(RestoredItemModel.FirstOrDefault()), Url.Action(DataTableNames.ItemModelLanguagesDataTable, Portal.GetControllerName(ControllerContext)), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyItemModel(model.WarehouseItemModelGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ItemModelsDataTableDelete(List<codeWarehouseItemModel> models)
        {
            if (!CMS.HasAction(Permissions.ItemModel.Delete, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            List<codeWarehouseItemModel> DeletedItemModel = DeleteItemModel(models);

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.PartialDeleteMessage(DeletedItemModel, models, DataTableNames.ItemModelsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ItemModelDataTableRestore(List<codeWarehouseItemModel> models)
        {
            if (!CMS.HasAction(Permissions.ItemModel.Restore, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            List<codeWarehouseItemModel> RestoredItemModel = RestoreItemModel(models);

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.PartialRestoreMessage(RestoredItemModel, models, DataTableNames.ItemModelsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        private List<codeWarehouseItemModel> DeleteItemModel(List<codeWarehouseItemModel> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<codeWarehouseItemModel> DeletedItemModel = new List<codeWarehouseItemModel>();
            //Fix
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = "SELECT WarehouseItemModelGUID,CONVERT(varchar(50), WarehouseItemModelGUID) as C2 ,codeWarehouseItemModelRowVersion FROM code.codeWarehouseItemModel where WarehouseItemModelGUID in (" + string.Join(",", models.Select(x => "'" + x.WarehouseItemModelGUID + "'").ToArray()) + ")";

            string query = DbWMS.QueryBuilder(models, Permissions.ItemModel.DeleteGuid, SubmitTypes.Delete, "");

            var Records = DbWMS.Database.SqlQuery<codeWarehouseItemModel>(query).ToList();
            foreach (var record in Records)
            {
                DeletedItemModel.Add(DbWMS.Delete(record, ExecutionTime, Permissions.ItemModel.DeleteGuid, DbCMS));
            }

            var Languages = DeletedItemModel.SelectMany(a => a.codeWarehouseItemModelLanguage).Where(l => l.Active).ToList();
            foreach (var language in Languages)
            {
                DbWMS.Delete(language, ExecutionTime, Permissions.ItemModel.DeleteGuid, DbCMS);
            }
            return DeletedItemModel;
        }

        private List<codeWarehouseItemModel> RestoreItemModel(List<codeWarehouseItemModel> models)
        {

            DateTime RestoringTime = DateTime.Now;

            List<codeWarehouseItemModel> RestoredItemModel = new List<codeWarehouseItemModel>();
            //Fix
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = "SELECT WarehouseItemModelGUID,CONVERT(varchar(50), WarehouseItemModelGUID) as C2 ,codeWarehouseItemModelRowVersion FROM code.codeWarehouseItemModel where WarehouseItemModelGUID in (" + string.Join(",", models.Select(x => "'" + x.WarehouseItemModelGUID + "'").ToArray()) + ")";

            string query = DbWMS.QueryBuilder(models, Permissions.ItemModel.DeleteGuid, SubmitTypes.Restore, "");

            var Records = DbWMS.Database.SqlQuery<codeWarehouseItemModel>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveItemModel(record))
                {
                    RestoredItemModel.Add(DbWMS.Restore(record, Permissions.ItemModel.DeleteGuid, Permissions.ItemModel.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            var Languages = RestoredItemModel.SelectMany(x => x.codeWarehouseItemModelLanguage.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var language in Languages)
            {
                DbWMS.Restore(language, Permissions.ItemModel.DeleteGuid, Permissions.ItemModel.RestoreGuid, RestoringTime, DbCMS);
            }

            return RestoredItemModel;
        }

        private JsonResult ConcurrencyItemModel(Guid PK)
        {
            ItemModelUpdateModel dbModel = new ItemModelUpdateModel();

            var ItemModel = DbWMS.codeWarehouseItemModel.Where(x => x.WarehouseItemModelGUID == PK).FirstOrDefault();
            var dbItemModel = DbWMS.Entry(ItemModel).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbItemModel, dbModel);

            var Language = DbWMS.codeWarehouseItemModelLanguage.Where(x => x.WarehouseItemModelGUID == PK).Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeWarehouseItemModel.DeletedOn) && x.LanguageID == LAN).FirstOrDefault();
            var dbLanguage = DbWMS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (ItemModel.codeWarehouseItemModelRowVersion.SequenceEqual(dbModel.codeWarehouseItemModelRowVersion) && Language.codeWarehouseItemModelLanguageRowVersion.SequenceEqual(dbModel.codeWarehouseItemModelLanguageRowVersion))
            {
                return Json(DbWMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbWMS, dbModel, "LanguagesContainer"));
        }

        private bool ActiveItemModel(Object model)
        {
            codeWarehouseItemModelLanguage ItemModel = Mapper.Map(model, new codeWarehouseItemModelLanguage());
            int ModelDescription = DbWMS.codeWarehouseItemModelLanguage
                                    .Where(x => x.ModelDescription == ItemModel.ModelDescription &&
                                                x.LanguageID == LAN&&
                                                x.Active).Count();
            if (ModelDescription > 0)
            {
                ModelState.AddModelError("ModelDescription", "ItemModel is already exists");
            }
            return (ModelDescription > 0);
        }

        #endregion

        #region Item Models Language

        //[Route("WMS/ItemModelLanguagesDataTable/{PK}")]
        public ActionResult ItemModelLanguagesDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/WMS/Views/ItemModels/_LanguagesDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<codeWarehouseItemModelLanguage, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<codeWarehouseItemModelLanguage>(DataTable.Filters);
            }

            var Result = DbWMS.codeWarehouseItemModelLanguage.AsNoTracking().AsExpandable().Where(x => x.LanguageID != LAN && x.WarehouseItemModelGUID == PK).Where(Predicate)
                              .Select(x => new
                              {
                                  x.WarehouseItemModelLanguageGUID,
                                  x.LanguageID,
                                  Title = x.ModelDescription,
                                  x.codeWarehouseItemModelLanguageRowVersion
                              });

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ItemModelLanguageCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.ItemModel.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            return PartialView("~/Areas/WMS/Views/ItemModels/_LanguageUpdateModal.cshtml",
                new codeWarehouseItemModelLanguage { WarehouseItemModelGUID = FK });
        }

        public ActionResult ItemModelLanguageUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.ItemModel.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            return PartialView("~/Areas/WMS/Views/ItemModels/_LanguageUpdateModal.cshtml", DbWMS.codeWarehouseItemModelLanguage.Find(PK));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ItemModelLanguageCreate(codeWarehouseItemModelLanguage model)
        {
            if (!CMS.HasAction(Permissions.ItemModel.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (!ModelState.IsValid || ActiveItemModelLanguage(model)) return PartialView("~/Areas/WMS/Views/ItemModels/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbWMS.Create(model, Permissions.ItemModel.CreateGuid, ExecutionTime, DbCMS);

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleUpdateMessage(DataTableNames.ItemModelLanguagesDataTable, DbWMS.PrimaryKeyControl(model), DbWMS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ItemModelLanguageUpdate(codeWarehouseItemModelLanguage model)
        {
            if (!CMS.HasAction(Permissions.ItemModel.Update, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (!ModelState.IsValid || ActiveItemModelLanguage(model)) return PartialView("~/Areas/WMS/Views/ItemModels/_LanguageUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbWMS.Update(model, Permissions.ItemModel.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleUpdateMessage(DataTableNames.ItemModelLanguagesDataTable,
                    DbWMS.PrimaryKeyControl(model),
                    DbWMS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyItemModelLanguage(model.WarehouseItemModelLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ItemModelLanguageDelete(codeWarehouseItemModelLanguage model)
        {
            if (!CMS.HasAction(Permissions.ItemModel.Delete, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            List<codeWarehouseItemModelLanguage> DeletedLanguages = DeleteItemModelLanguages(new List<codeWarehouseItemModelLanguage> { model });

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleDeleteMessage(DeletedLanguages, DataTableNames.ItemModelLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyItemModelLanguage(model.WarehouseItemModelLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ItemModelLanguageRestore(codeWarehouseItemModelLanguage model)
        {
            if (!CMS.HasAction(Permissions.ItemModel.Restore, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (ActiveItemModelLanguage(model))
            {
                return Json(DbWMS.RecordExists());
            }

            List<codeWarehouseItemModelLanguage> RestoredLanguages = RestoreItemModelLanguages(Portal.SingleToList(model));

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleRestoreMessage(RestoredLanguages, DataTableNames.ItemModelLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyItemModelLanguage(model.WarehouseItemModelLanguageGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ItemModelLanguagesDataTableDelete(List<codeWarehouseItemModelLanguage> models)
        {
            if (!CMS.HasAction(Permissions.ItemModel.Delete, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            List<codeWarehouseItemModelLanguage> DeletedLanguages = DeleteItemModelLanguages(models);

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.ItemModelLanguagesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ItemModelLanguagesDataTableRestore(List<codeWarehouseItemModelLanguage> models)
        {
            if (!CMS.HasAction(Permissions.ItemModel.Restore, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            List<codeWarehouseItemModelLanguage> RestoredLanguages = RestoreItemModelLanguages(models);

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.ItemModelLanguagesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        private List<codeWarehouseItemModelLanguage> DeleteItemModelLanguages(List<codeWarehouseItemModelLanguage> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<codeWarehouseItemModelLanguage> DeletedItemModelLanguages = new List<codeWarehouseItemModelLanguage>();

            string query = DbWMS.QueryBuilder(models, Permissions.ItemModel.DeleteGuid, SubmitTypes.Delete, "");

            var languages = DbWMS.Database.SqlQuery<codeWarehouseItemModelLanguage>(query).ToList();

            foreach (var language in languages)
            {
                DeletedItemModelLanguages.Add(DbWMS.Delete(language, ExecutionTime, Permissions.ItemModel.DeleteGuid, DbCMS));
            }

            return DeletedItemModelLanguages;
        }

        private List<codeWarehouseItemModelLanguage> RestoreItemModelLanguages(List<codeWarehouseItemModelLanguage> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<codeWarehouseItemModelLanguage> RestoredLanguages = new List<codeWarehouseItemModelLanguage>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbWMS.QueryBuilder(models, Permissions.ItemModel.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Languages = DbWMS.Database.SqlQuery<codeWarehouseItemModelLanguage>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveItemModelLanguage(language))
                {
                    RestoredLanguages.Add(DbWMS.Restore(language, Permissions.ItemModel.DeleteGuid, Permissions.ItemModel.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyItemModelLanguage(Guid PK)
        {
            codeWarehouseItemModelLanguage dbModel = new codeWarehouseItemModelLanguage();

            var Language = DbWMS.codeWarehouseItemModelLanguage.Where(l => l.WarehouseItemModelLanguageGUID == PK).FirstOrDefault();
            var dbLanguage = DbWMS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.codeWarehouseItemModelLanguageRowVersion.SequenceEqual(dbModel.codeWarehouseItemModelLanguageRowVersion))
            {
                return Json(DbWMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbWMS, dbModel, "LanguagesContainer"));
        }

        private bool ActiveItemModelLanguage(codeWarehouseItemModelLanguage model)
        {
            int LanguageID = DbWMS.codeWarehouseItemModelLanguage
                                  .Where(x => x.LanguageID == model.LanguageID &&
                                              x.ModelDescription==model.ModelDescription && 
                                              x.WarehouseItemModelGUID==model.WarehouseItemModelGUID && 
                                              x.Active).Count();
            if (LanguageID > 0)
            {
                //Fix
                ModelState.AddModelError("LanguageID", "Telecom Company Name in selected language already exists"); //From resource ?????? Amer  
            }

            return (LanguageID > 0);
        }

        #endregion Language

    

        #region Item Models Determinant

        //[Route("WMS/ItemModelDeterminantsDataTable/{PK}")]
        public ActionResult ItemModelDeterminantsDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/WMS/Views/ItemModels/_ModelsDeterminantDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<codeWarehouseItemModelDeterminant, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<codeWarehouseItemModelDeterminant>(DataTable.Filters);
            }

            var Result = (from a in DbWMS.codeWarehouseItemModelDeterminant.AsNoTracking().AsExpandable().Where(x => x.WarehouseItemModelGUID == PK).Where(Predicate)
                          join b in DbWMS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.DeterminantGUID equals b.ValueGUID
                          select new 
                          {
                              WarehouseItemModelGUID = a.WarehouseItemModelGUID,
                              WarehouseItemModelDeterminantGUID = a.WarehouseItemModelDeterminantGUID,
                              DeterminatDescription = b.ValueDescription,
                              a.codeWarehouseItemModelDeterminantRowVersion,
                              a.Active


                          });


             Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ItemModelDeterminantCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.ItemModel.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            return PartialView("~/Areas/WMS/Views/ItemModels/_ModelsDeterminantModal.cshtml",
                new codeWarehouseItemModelDeterminant { WarehouseItemModelGUID = FK });
        }

        public ActionResult ItemModelDeterminantUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.ItemModel.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            return PartialView("~/Areas/WMS/Views/ItemModels/_ModelsDeterminantModal.cshtml", DbWMS.codeWarehouseItemModelDeterminant.Find(PK));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ItemModelDeterminantCreate(codeWarehouseItemModelDeterminant model)
        {
            if (!CMS.HasAction(Permissions.ItemModel.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (!ModelState.IsValid || ActiveItemModelDeterminant(model)) return PartialView("~/Areas/WMS/Views/ItemModels/_ModelsDeterminantModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbWMS.Create(model, Permissions.ItemModel.CreateGuid, ExecutionTime, DbCMS);

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleUpdateMessage(DataTableNames.ItemModelDeterminantsDataTable, DbWMS.PrimaryKeyControl(model), DbWMS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ItemModelDeterminantUpdate(codeWarehouseItemModelDeterminant model)
        {
            if (!CMS.HasAction(Permissions.ItemModel.Update, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (!ModelState.IsValid || ActiveItemModelDeterminant(model)) return PartialView("~/Areas/WMS/Views/ItemModels/_ModelsDeterminantModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbWMS.Update(model, Permissions.ItemModel.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleUpdateMessage(DataTableNames.ItemModelDeterminantsDataTable,
                    DbWMS.PrimaryKeyControl(model),
                    DbWMS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyItemModelDeterminant(model.WarehouseItemModelDeterminantGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ItemModelDeterminantDelete(codeWarehouseItemModelDeterminant model)
        {
            if (!CMS.HasAction(Permissions.ItemModel.Delete, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            List<codeWarehouseItemModelDeterminant> DeletedLanguages = DeleteItemModelDeterminants(new List<codeWarehouseItemModelDeterminant>() { model });

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleDeleteMessage(DeletedLanguages, DataTableNames.ItemModelLanguagesDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyItemModelDeterminant(model.WarehouseItemModelDeterminantGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ItemModelDeterminantRestore(codeWarehouseItemModelDeterminant model)
        {
            if (!CMS.HasAction(Permissions.ItemModel.Restore, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (ActiveItemModelDeterminant(model))
            {
                return Json(DbWMS.RecordExists());
            }

            List<codeWarehouseItemModelDeterminant> RestoredLanguages = RestoreItemModelDeterminants(Portal.SingleToList(model));

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleRestoreMessage(RestoredLanguages, DataTableNames.ItemModelDeterminantsDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyItemModelDeterminant(model.WarehouseItemModelDeterminantGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ItemModelDeterminantsDataTableDelete(List<codeWarehouseItemModelDeterminant> models)
        {
            if (!CMS.HasAction(Permissions.ItemModel.Delete, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            List<codeWarehouseItemModelDeterminant> DeletedLanguages = DeleteItemModelDeterminants(models);

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.ItemModelDeterminantsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ItemModelDeterminantsDataTableRestore(List<codeWarehouseItemModelDeterminant> models)
        {
            if (!CMS.HasAction(Permissions.ItemModel.Restore, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            List<codeWarehouseItemModelDeterminant> RestoredLanguages = RestoreItemModelDeterminants(models);

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.ItemModelDeterminantsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        private List<codeWarehouseItemModelDeterminant> DeleteItemModelDeterminants(List<codeWarehouseItemModelDeterminant> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<codeWarehouseItemModelDeterminant> DeletedItemModelDeterminants = new List<codeWarehouseItemModelDeterminant>();

            string query = DbWMS.QueryBuilder(models, Permissions.ItemModel.DeleteGuid, SubmitTypes.Delete, "");

            var languages = DbWMS.Database.SqlQuery<codeWarehouseItemModelDeterminant>(query).ToList();

            foreach (var language in languages)
            {
                DeletedItemModelDeterminants.Add(DbWMS.Delete(language, ExecutionTime, Permissions.ItemModel.DeleteGuid, DbCMS));
            }

            return DeletedItemModelDeterminants;
        }

        private List<codeWarehouseItemModelDeterminant> RestoreItemModelDeterminants(List<codeWarehouseItemModelDeterminant> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<codeWarehouseItemModelDeterminant> RestoredLanguages = new List<codeWarehouseItemModelDeterminant>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbWMS.QueryBuilder(models, Permissions.ItemModel.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Languages = DbWMS.Database.SqlQuery<codeWarehouseItemModelDeterminant>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveItemModelDeterminant(language))
                {
                    RestoredLanguages.Add(DbWMS.Restore(language, Permissions.ItemModel.DeleteGuid, Permissions.ItemModel.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyItemModelDeterminant(Guid PK)
        {
            codeWarehouseItemModelDeterminant dbModel = new codeWarehouseItemModelDeterminant();

            var Language = DbWMS.codeWarehouseItemModelDeterminant.Where(l => l.WarehouseItemModelDeterminantGUID == PK).FirstOrDefault();
            var dbLanguage = DbWMS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.codeWarehouseItemModelDeterminantRowVersion.SequenceEqual(dbModel.codeWarehouseItemModelDeterminantRowVersion))
            {
                return Json(DbWMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbWMS, dbModel, "LanguagesContainer"));
        }

        private bool ActiveItemModelDeterminant(codeWarehouseItemModelDeterminant model)
        {
            int LanguageID = DbWMS.codeWarehouseItemModelDeterminant
                                  .Where(x =>
                                              x.WarehouseItemModelGUID == model.WarehouseItemModelGUID &&
                                              x.DeterminantGUID == model.DeterminantGUID &&
                                              x.Active).Count();
            if (LanguageID > 0)
            {
                ModelState.AddModelError("LanguageID", "Item Already exists");
            }

            return (LanguageID > 0);
        }

        #endregion Language


        #region Item Warehouses

        //[Route("WMS/ItemModelWarehouseDataTable/{PK}")]
        public ActionResult ItemModelWarehouseDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/WMS/Views/ItemModels/_WarehouseDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<codeItemModelWarehouse, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<codeItemModelWarehouse>(DataTable.Filters);
            }

            var Result = (from a in DbWMS.codeItemModelWarehouse.AsNoTracking().AsExpandable().Where(x => x.WarehouseItemModelGUID == PK).Where(Predicate)
                          join b in DbWMS.codeWarehouse.Where(x => x.Active) on a.WarehouseGUID equals b.WarehouseGUID
                          join c in DbWMS.codeWarehouseLanguage.Where(x => x.Active && x.LanguageID == LAN) on b.WarehouseGUID equals c.WarehouseGUID
                          select new
                          {
                              ItemModelWarehouseGUID = a.ItemModelWarehouseGUID,
                              WarehouseDescription = c.WarehouseDescription,
                              LowestAmountAllowed = a.LowestAmountAllowed,
                              ReOrderedLimit = a.ReOrderedLimit,
                              a.codeItemModelWarehouseRowVersion,
                              a.Active
                          });


            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult WarehouseItemKitChildrenDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/WMS/Views/ItemModels/_ItemKitChildrenDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<codeWarehouseItemModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<codeWarehouseItemModel>(DataTable.Filters);
            }

            var Result = (from a in DbWMS.codeWarehouseItemModel.AsNoTracking().AsExpandable().Where(x => x.ParentWarehouseItemModelGUID == PK).Where(Predicate)
                
                join b in DbWMS.codeWarehouseItemModelLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.WarehouseItemModelGUID equals b.WarehouseItemModelGUID
                  select new
                {
                      WarehouseItemModelGUID = a.WarehouseItemModelGUID,
                      ModelDescription = b.ModelDescription,
                   
                    a.Active
                });


            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ItemModelWarehouseCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.ItemModel.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            return PartialView("~/Areas/WMS/Views/ItemModels/_WarehouseModal.cshtml",
                new codeItemModelWarehouse { WarehouseItemModelGUID = FK });
        }

        public ActionResult ItemModelWarehouseUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.ItemModel.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            return PartialView("~/Areas/WMS/Views/ItemModels/_WarehouseModal.cshtml", DbWMS.codeItemModelWarehouse.Find(PK));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ItemModelWarehouseCreate(codeItemModelWarehouse model)
        {
            if (!CMS.HasAction(Permissions.ItemModel.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (!ModelState.IsValid || ActiveItemModelWarehouse(model)) return PartialView("~/Areas/WMS/Views/ItemModels/_WarehouseModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbWMS.Create(model, Permissions.ItemModel.CreateGuid, ExecutionTime, DbCMS);

            try
            {
                DbWMS.SaveChanges();
                DbWMS.SaveChanges();
                return Json(DbWMS.SingleUpdateMessage(DataTableNames.ItemModelWarehouseDataTable, DbWMS.PrimaryKeyControl(model), DbWMS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ItemModelWarehouseUpdate(codeItemModelWarehouse model)
        {
            if (!CMS.HasAction(Permissions.ItemModel.Update, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (!ModelState.IsValid || ActiveItemModelWarehouse(model)) return PartialView("~/Areas/WMS/Views/ItemModels/_WarehouseModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbWMS.Update(model, Permissions.ItemModel.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbWMS.SaveChanges();
                DbWMS.SaveChanges();
                return Json(DbWMS.SingleUpdateMessage(DataTableNames.ItemModelWarehouseDataTable,
                    DbWMS.PrimaryKeyControl(model),
                    DbWMS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyItemModelWarehouse(model.ItemModelWarehouseGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ItemModelWarehouseDelete(codeItemModelWarehouse model)
        {
            if (!CMS.HasAction(Permissions.ItemModel.Delete, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            List<codeItemModelWarehouse> DeletedLanguages = DeleteItemModelWarehousesDataTable(new List<codeItemModelWarehouse> { model });

            try
            {
                DbWMS.SaveChanges();
                DbWMS.SaveChanges();
                return Json(DbWMS.SingleDeleteMessage(DeletedLanguages, DataTableNames.ItemModelWarehouseDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyItemModelWarehouse(model.ItemModelWarehouseGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ItemModelWarehouseRestore(codeItemModelWarehouse model)
        {
            if (!CMS.HasAction(Permissions.ItemModel.Restore, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (ActiveItemModelWarehouse(model))
            {
                return Json(DbWMS.RecordExists());
            }

            List<codeItemModelWarehouse> RestoredLanguages = RestoreItemModelWarehousesDataTable(Portal.SingleToList(model));

            try
            {
                DbWMS.SaveChanges();
                DbWMS.SaveChanges();
                return Json(DbWMS.SingleRestoreMessage(RestoredLanguages, DataTableNames.ItemModelWarehouseDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyItemModelWarehouse(model.ItemModelWarehouseGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ItemModelWarehouseDataTableDelete(List<codeItemModelWarehouse> models)
        {
            if (!CMS.HasAction(Permissions.ItemModel.Delete, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            List<codeItemModelWarehouse> DeletedLanguages = DeleteItemModelWarehousesDataTable(models);

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.ItemModelWarehouseDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ItemModelWarehouseDataTableRestore(List<codeItemModelWarehouse> models)
        {
            if (!CMS.HasAction(Permissions.ItemModel.Restore, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            List<codeItemModelWarehouse> RestoredLanguages = RestoreItemModelWarehousesDataTable(models);

            try
            {
                DbWMS.SaveChanges();
                DbWMS.SaveChanges();
                return Json(DbWMS.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.ItemModelWarehouseDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        private List<codeItemModelWarehouse> DeleteItemModelWarehousesDataTable(List<codeItemModelWarehouse> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<codeItemModelWarehouse> DeletedItemModelWarehousesDataTable = new List<codeItemModelWarehouse>();

            string query = DbWMS.QueryBuilder(models, Permissions.ItemModel.DeleteGuid, SubmitTypes.Delete, "");

            var languages = DbWMS.Database.SqlQuery<codeItemModelWarehouse>(query).ToList();

            foreach (var language in languages)
            {
                DeletedItemModelWarehousesDataTable.Add(DbWMS.Delete(language, ExecutionTime, Permissions.ItemModel.DeleteGuid, DbCMS));
            }

            return DeletedItemModelWarehousesDataTable;
        }

        private List<codeItemModelWarehouse> RestoreItemModelWarehousesDataTable(List<codeItemModelWarehouse> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<codeItemModelWarehouse> RestoredLanguages = new List<codeItemModelWarehouse>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbWMS.QueryBuilder(models, Permissions.ItemModel.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Languages = DbWMS.Database.SqlQuery<codeItemModelWarehouse>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveItemModelWarehouse(language))
                {
                    RestoredLanguages.Add(DbWMS.Restore(language, Permissions.ItemModel.DeleteGuid, Permissions.ItemModel.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyItemModelWarehouse(Guid PK)
        {
            codeItemModelWarehouse dbModel = new codeItemModelWarehouse();

            var Language = DbWMS.codeItemModelWarehouse.Where(l => l.ItemModelWarehouseGUID == PK).FirstOrDefault();
            var dbLanguage = DbWMS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.codeItemModelWarehouseRowVersion.SequenceEqual(dbModel.codeItemModelWarehouseRowVersion))
            {
                return Json(DbWMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbWMS, dbModel, "LanguagesContainer"));
        }

        private bool ActiveItemModelWarehouse(codeItemModelWarehouse model)
        {
            int LanguageID = DbWMS.codeItemModelWarehouse
                                .Where(x =>
                                            x.WarehouseItemModelGUID == model.WarehouseItemModelGUID &&
                                            x.WarehouseGUID == model.WarehouseGUID &&
                                            x.Active).Count();
            //int LanguageID = DbWMS.codeItemModelWarehouse
            //                      .Where(x =>
            //                                  x.WarehouseItemModelGUID == model.WarehouseItemModelGUID &&
            //                                  x.ItemModelWarehouseGUID != model.ItemModelWarehouseGUID &&
            //                                  x.Active).Count();
            if (LanguageID > 0)
            {
                ModelState.AddModelError("LanguageID", "Item Already exists");
            }

            return (LanguageID > 0);
        }

        #endregion Language

        #region Model Management


        [Route("WMS/ModelManagement/")]
        public ActionResult ModelManagementIndex()
        {
            if (!CMS.HasAction(Permissions.ItemModel.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            return View("~/Areas/WMS/Views/ModelManagement/Index.cshtml");
        }

        [Route("WMS/ModelManagementsDataTable/")]
        public JsonResult ModelManagementDataTable(DataTableRecievedOptions options)
        {

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<ItemInputDetailDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<ItemInputDetailDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            var All = (
              from a in DbWMS.dataItemInputDetail.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.dataItemInput.DeletedOn)).AsExpandable()
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
              join k in DbWMS.userPersonalDetailsLanguage.Where(x => x.Active) on a.LastCustdianNameGUID equals k.UserGUID into LJ5
              from R5 in LJ5.DefaultIfEmpty()
              join L in DbWMS.codeWarehouse.Where(x => x.Active) on a.LastCustdianNameGUID equals L.WarehouseGUID into LJ6
              from R6 in LJ6.DefaultIfEmpty()
              join M in DbWMS.codeWarehouseLanguage.Where(x => x.Active && x.LanguageID == LAN) on R6.WarehouseGUID equals M.WarehouseGUID into LJ7
              from R7 in LJ7.DefaultIfEmpty()
              join N in DbWMS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN && x.codeTablesValues.TableGUID == LookupTables.ItemStatus) on a.ItemStatusGUID equals N.ValueGUID into LJ8
              from R8 in LJ8.DefaultIfEmpty()

              select new ItemInputDetailDataTableModel
                { 
                    ItemInputDetailGUID =a.ItemInputDetailGUID,
                    Active = a.Active,
                    ItemModelWarehouseGUID= a.ItemModelWarehouseGUID,
                    ModelDescription = R3.ModelDescription,
                    ItemDescription =R4.WarehouseItemDescription,
                    BarcodeNumber = a.dataItemInputDeterminant.Where(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.Barcode).Select(x => x.DeterminantValue).FirstOrDefault(),
                    SerialNumber = a.dataItemInputDeterminant.Where(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.SerialNumber).Select(x => x.DeterminantValue).FirstOrDefault(),
                    IME1 = a.dataItemInputDeterminant.Where(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.IME1).Select(x => x.DeterminantValue).FirstOrDefault(),
                    WarehouseOwner = R1.codeWarehouse1.codeWarehouseLanguage.Where(x=>x.Active && x.LanguageID==LAN ).FirstOrDefault().WarehouseDescription,
                    Governorate="",
                    Custodian =a.IsAvaliable==true?"Stock":(a.LastCustdianGUID==WarehouseRequestSourceTypes.Warehouse? R7.WarehouseDescription : R5.FirstName+" "+R5.Surname),
                    ModelStatus= R8.ValueDescription,
                    dataItemInputDetailRowVersion = a.dataItemInputDetailRowVersion
                }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<ItemInputDetailDataTableModel> Result = Mapper.Map<List<ItemInputDetailDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        ////[Route("WMS/ModelManagement/Create/")]
        //public ActionResult ModelManagementCreate()
        //{
        //    if (!CMS.HasAction(Permissions.ItemModel.Create, Apps.WMS))
        //    {
        //        return Json(DbWMS.PermissionError());
        //    }
        //    return PartialView("~/Areas/WMS/Views/ModelManagement/_ItemEntry.cshtml", new ItemInputModel());
        //}

        [Route("WMS/ModelManagement/Update/{PK}")]
        public ActionResult ModelManagementUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.ItemModel.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
          
            //var model = (
            //    from a in DbWMS.dataItemInputDetail.Where(x => x.Active && x.ItemInputDetailGUID==PK).AsExpandable()
            //        //join b in DbWMS.dataItemInputDetail.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.dataItemInput.DeletedOn)) on a.ItemInputGUID equals b.ItemInputGUID into LJ1
            //        //from R1 in LJ1.DefaultIfEmpty()
            //        //join c in DbWMS.codeItemModelWarehouse.Where(x => x.Active) on R1.ItemModelWarehouseGUID equals c.ItemModelWarehouseGUID into LJ2
            //        //from R2 in LJ2.DefaultIfEmpty()
            //        //join d in DbWMS.codeWarehouseItemModel.Where(x => x.Active) on R2.WarehouseItemModelGUID equals d.WarehouseItemModelGUID
            //        //join e in DbWMS.codeWarehouseItemModelLanguage.Where(x => x.LanguageID == LAN && x.Active) on d.WarehouseItemModelGUID equals e.WarehouseItemModelGUID into LJ3
            //        //from R3 in LJ3.DefaultIfEmpty()

            //        //join f in DbWMS.codeWarehouseItem.Where(x => x.Active) on d.WarehouseItemGUID equals f.WarehouseItemGUID
            //        //join g in DbWMS.codeWarehouseItemLanguage.Where(x => x.LanguageID == LAN && x.Active) on f.WarehouseItemGUID equals g.WarehouseItemGUID into LJ4
            //        //from R4 in LJ4.DefaultIfEmpty()


            //    select new dataItemInputDetail
            //    {
            //        PONumber = a.BillNumber,
            //        InputDate = a.InputDate,
            //        PartyTypeGUID = (Guid)a.PartyTypeGUID,
            //        PartyGUID = (Guid)a.PartyGUID,

            //        //ItemInputDetailGUID = R1.ItemInputDetailGUID,
            //        //Active = a.Active,
            //        //ItemInputGUID = a.ItemInputGUID,
            //        //ItemStatusGUID =(Guid) R1.ItemStatusGUID,
            //        //ItemModelWarehouseGUID = (Guid)R1.ItemModelWarehouseGUID,
            //        //Qunatity = R1.Qunatity,
            //        //LocationGUID = R1.LocationGUID,
            //        //PriceTypeGUID =R1.PriceTypeGUID,
            //        //PriceValue = R1.PriceValue,
            //        //InventoryStatusGUID = R1.InventoryStatusGUID,
            //        //ItemBrandModelColorGUID = R1.ItemBrandModelColorGUID,
            //        //Comments = R1.Comments,
            //        //dataItemInputDetailRowVersion = R1.dataItemInputDetailRowVersion,
            //        dataItemInputRowVersion = a.dataItemInputRowVersion,



            //    }).FirstOrDefault();

            if (DbWMS.dataItemInputDetail.Where(x => x.Active && x.ItemInputDetailGUID == PK).FirstOrDefault() == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("ModelManagement", "ModelManagement", new { Area = "WMS" }));

            return View("~/Areas/WMS/Views/ModelManagement/ModelManagement.cshtml", DbWMS.dataItemInputDetail.Where(x => x.Active && x.ItemInputDetailGUID == PK).FirstOrDefault());
        }

        //[HttpPost, ValidateAntiForgeryToken]
        //public ActionResult ModelManagementCreate(ItemInputModel model)
        //{
        //    if (!CMS.HasAction(Permissions.ItemModel.Create, Apps.WMS))
        //    {
        //        return Json(DbWMS.PermissionError());
        //    }
        //    if (!ModelState.IsValid || ActiveItemModel(model)) return PartialView("~/Areas/WMS/Views/ModelManagement/_ModelManagementForm.cshtml", model);

        //    DateTime ExecutionTime = DateTime.Now;

        //    Guid EntityPK = Guid.NewGuid();

        //    dataItemInput ItemInput = Mapper.Map(model, new dataItemInput());
        //    ItemInput.ItemInputGUID = EntityPK;
        //    DbWMS.Create(ItemInput, Permissions.ItemModel.CreateGuid, ExecutionTime, DbCMS);

        //    //codeWarehouseItemModelLanguage Language = Mapper.Map(model, new codeWarehouseItemModelLanguage());
        //    //Language.WarehouseItemModelGUID = EntityPK;

        //    //DbWMS.Create(Language, Permissions.ItemModel.CreateGuid, ExecutionTime, DbCMS);

        //    List<PartialViewModel> Partials = new List<PartialViewModel>();
        //    Partials.Add(Portal.PartialView(EntityPK, DataTableNames.ItemModelLanguagesDataTable, ControllerContext, "LanguagesContainer"));

        //    List<UIButtons> UIButtons = new List<UIButtons>();
        //    UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.ItemModel.Create, Apps.WMS, new UrlHelper(Request.RequestContext).Action("Create", "ModelManagement", new { Area = "WMS" })), Container = "ModelManagementFormControls" });
        //    UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.ItemModel.Update, Apps.WMS), Container = "ModelManagementFormControls" });
        //    UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.ItemModel.Delete, Apps.WMS), Container = "ModelManagementFormControls" });

        //    try
        //    {
        //        DbWMS.SaveChanges();
        //        DbWMS.SaveChanges();
        //        return Json(DbWMS.SingleCreateMessage(DbWMS.PrimaryKeyControl(ItemInput), null, Partials, "", UIButtons));
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(DbWMS.ErrorMessage(ex.Message));
        //    }
        //}

        //[HttpPost, ValidateAntiForgeryToken]
        //public ActionResult ModelManagementUpdate(ItemInputDetailModel model)
        //{
        //    if (!CMS.HasAction(Permissions.ItemModel.Update, Apps.WMS))
        //    {
        //        return Json(DbWMS.PermissionError());
        //    }
        //    if (!ModelState.IsValid || ActiveItemModel(model)) return PartialView("~/Areas/WMS/Views/ModelManagement/_ModelManagementForm.cshtml", model);

        //    DateTime ExecutionTime = DateTime.Now;

        //    codeWarehouseItemModel ItemModel = Mapper.Map(model, new codeWarehouseItemModel());
        //    DbWMS.Update(ItemModel, Permissions.ItemModel.UpdateGuid, ExecutionTime, DbCMS);

        //    var Language = DbWMS.codeWarehouseItemModelLanguage.Where(l => l.WarehouseItemModelGUID == model.WarehouseItemModelGUID && l.LanguageID == LAN && l.Active).FirstOrDefault();

        //    if (Language == null)
        //    {
        //        Language = Mapper.Map(model, Language);
        //        Language.WarehouseItemModelGUID = ItemModel.WarehouseItemModelGUID;
        //        DbWMS.Create(Language, Permissions.ItemModel.CreateGuid, ExecutionTime, DbCMS);
        //    }
        //    else if (Language.ModelDescription != model.ModelDescription)
        //    {
        //        Language = Mapper.Map(model, Language);
        //        DbWMS.Update(Language, Permissions.ItemModel.UpdateGuid, ExecutionTime, DbCMS);
        //    }

        //    try
        //    {
        //        DbWMS.SaveChanges();
        //        DbWMS.SaveChanges();
        //        return Json(DbWMS.SingleUpdateMessage(null, null, DbWMS.RowVersionControls(ItemModel, Language)));
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        return ConcurrencyItemModel(model.WarehouseItemModelGUID);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(DbWMS.ErrorMessage(ex.Message));
        //    }
        //}

        #endregion

        #region Custdion Model History
        public ActionResult WarehosueModelMovementsDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/WMS/Views/ModelMovements/_ModelMovementsDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<ModelMovementModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<ModelMovementModel>(DataTable.Filters);
            }

            var Result = (
                
                from a in DbWMS.dataItemOutputDetail.AsNoTracking().AsExpandable().Where(x => x.Active && x.ItemInputDetailGUID == PK)
                join b in DbWMS.dataItemOutput.Where(x => x.Active  ) on a.ItemOutputGUID equals b.ItemOutputGUID into LJ1 from R1 in LJ1.DefaultIfEmpty()
                join c in DbWMS.codeTablesValues.Where(x => x.Active ) on R1.RequesterGUID equals c.ValueGUID
                join d in DbWMS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on c.ValueGUID equals d.ValueGUID into LJ2
                from R2 in LJ2.DefaultIfEmpty()
                join e in DbWMS.codeTablesValuesLanguages.Where(x=>x.Active && x.LanguageID==LAN ) on a.ItemRequestTypeGUID equals  e.ValueGUID into LJ3
                from R3 in LJ3.DefaultIfEmpty()
                join f in DbWMS.dataItemOutputDetailFlow.Where(x=>x.Active && x.IsLastMove==true) on a.ItemOutputDetailGUID equals  f.ItemOutputDetailGUID 
                join g in DbWMS.codeTablesValuesLanguages.Where(x=>x.Active && x.LanguageID==LAN)  on f.FlowTypeGUID equals  g.ValueGUID into LJ4
                from R4 in LJ4.DefaultIfEmpty()
                join k in DbWMS.userPersonalDetailsLanguage.Where(x => x.Active) on R1.RequesterNameGUID equals k.UserGUID into LJ7
                from R7 in LJ7.DefaultIfEmpty()
                //join L in DbWMS.codeWarehouse.Where(x => x.Active) on R1.RequesterNameGUID equals L.WarehouseGUID
                //join M in DbWMS.codeWarehouseLanguage.Where(x => x.Active && x.LanguageID == LAN) on L.WarehouseGUID equals M.WarehouseGUID into LJ8
                //from R8 in LJ8.DefaultIfEmpty()

                select new ModelMovementModel
                          {
                              CustodianType = R2.ValueDescription,
                              Custodian= R7.FirstName + " " + R7.Surname,
                              ItemOutputDetailGUID = a.ItemOutputDetailGUID,
                              ReleaseType= R3.ValueDescription,
                              Qunatity=a.RequestedQunatity,
                              ExpectedStartDate=a.ExpectedStartDate,
                              ExpectedReturnedDate=a.ExpectedReturenedDate,
                              ActualReturnedDate=a.ActualReturenedDate,
                              DeliveryStatus = R4.ValueDescription, 
                              DeliveryActionDate = f.CreatedDate,
                              Active = a.Active,
                              ItemInputGUID = a.dataItemInputDetail.ItemInputGUID,
                }).Where(Predicate); 


          Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        #endregion



    }
}