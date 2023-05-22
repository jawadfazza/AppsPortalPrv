using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
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
using OfficeOpenXml;
using ISS_DAL.Model;
using ISS_DAL.ViewModels;
using FineUploader;
using OfficeOpenXml.Style;
using System.Drawing;
using System.Globalization;

namespace AppsPortal.Areas.ISS.Controllers
{
    public class ISSItemController : ISSBaseController
    {
        #region Item Classification

        [Route("ISS/ISSItem/")]
        public ActionResult ISSItemIndex()
        {
            if (!CMS.HasAction(Permissions.StockItemDistribution.Access, Apps.ISS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/ISS/Views/ISSItem/Index.cshtml");
        }

        [Route("ISS/ISSItemDataTable/")]
        public JsonResult ISSItemDataTable(DataTableRecievedOptions options)
        {

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<ItemDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<ItemDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.StockItemDistribution.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();
            //Fix
            var All = (

                from a in DbISS.codeISSItem.Where(x => x.Active).AsExpandable()
                join b in DbISS.codeISSItemLanguage.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeISSItem.DeletedOn) && x.LanguageID == LAN) on a.ItemGUID equals b.ItemGUID into LJ1
                from R1 in LJ1.DefaultIfEmpty()

                select new ItemDataTableModel
                {
                    ItemGUID = a.ItemGUID,
                    Active = a.Active,
                    ItemDescription = R1.ItemDescription,
                    SequencId=a.SequencId,

                    codeISSItemRowVersion = a.codeISSItemRowVersion
                }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<ItemDataTableModel> Result = Mapper.Map<List<ItemDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).OrderBy(x=>x.SequencId).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("ISS/ISSItem/Create/")]
        public ActionResult ISSItemCreate()
        {
            if (!CMS.HasAction(Permissions.StockItemDistribution.Create, Apps.ISS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/ISS/Views/ISSItem/ISSItem.cshtml", new ItemModel());
        }

        [Route("ISS/ISSItem/Update/{PK}")]
        public ActionResult ISSItemUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.StockItemDistribution.Access, Apps.ISS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var model = (from a in DbISS.codeISSItem.WherePK(PK)
                         join b in DbISS.codeISSItemLanguage.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeISSItem.DeletedOn) && x.LanguageID == LAN)
                         on a.ItemGUID equals b.ItemGUID into LJ1
                         from R1 in LJ1.DefaultIfEmpty()
                         select new ItemModel
                         {
                             ItemGUID = a.ItemGUID,
                             ItemDescription = R1.ItemDescription,

                             Active = a.Active,
                             codeISSItemRowVersion = a.codeISSItemRowVersion,
                             codeISSItemLanguageRowVersion = R1.codeISSItemLanguageRowVersion
                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("ISSItem", "IISItems", new { Area = "ISS" }));

            return View("~/Areas/ISS/Views/ISSItem/ISSItem.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ISSItemCreate(ItemModel model)
        {
            if (!CMS.HasAction(Permissions.StockItemDistribution.Create, Apps.ISS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveISSItem(model)) return PartialView("~/Areas/ISS/Views/ISSItem/_ISSItemForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();
            int seq=DbISS.codeISSItem.Select(x => x.SequencId).Max()+1 ?? 0;
            codeISSItem ISSItem = Mapper.Map(model, new codeISSItem());
            ISSItem.ItemGUID = EntityPK;
            ISSItem.SequencId = seq;
            DbISS.Create(ISSItem, Permissions.StockItemDistribution.CreateGuid, ExecutionTime, DbCMS);

            codeISSItemLanguage Language = Mapper.Map(model, new codeISSItemLanguage());
            Language.ItemLanguageGUID = EntityPK;
            Language.ItemGUID = ISSItem.ItemGUID;

            DbISS.Create(Language, Permissions.StockItemDistribution.CreateGuid, ExecutionTime, DbCMS);



            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.ISSItemDataTable, ControllerContext, "IISItemLanguagesFormControls"));
            
            

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.StockItemDistribution.Create, Apps.ISS, new UrlHelper(Request.RequestContext).Action("Create", "IISItems", new { Area = "ISS" })), Container = "IISItemDetailFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.StockItemDistribution.Update, Apps.ISS), Container = "IISItemDetailFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.StockItemDistribution.Delete, Apps.ISS), Container = "IISItemDetailFormControls" });

            try
            {
                DbISS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbISS.SingleCreateMessage(DbISS.PrimaryKeyControl(ISSItem), DbISS.RowVersionControls(ISSItem, Language), Partials, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbISS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ISSItemUpdate(ItemModel model)
        {
            if (!CMS.HasAction(Permissions.StockItemDistribution.Update, Apps.ISS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveISSItem(model)) return PartialView("~/Areas/ISS/Views/ISSItem/_ISSItemForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            codeISSItem ISSItem = Mapper.Map(model, new codeISSItem());
            DbISS.Update(ISSItem, Permissions.StockItemDistribution.UpdateGuid, ExecutionTime, DbCMS);

            var Language = DbISS.codeISSItemLanguage.Where(l => l.ItemGUID == model.ItemGUID && l.LanguageID == LAN && l.Active).FirstOrDefault();

            if (Language == null)
            {
                Language = Mapper.Map(model, Language);
                Language.ItemGUID = ISSItem.ItemGUID;
                DbISS.Create(Language, Permissions.StockItemDistribution.CreateGuid, ExecutionTime, DbCMS);
            }
            else if (Language.ItemDescription != model.ItemDescription)
            {
                Language = Mapper.Map(model, Language);
                DbISS.Update(Language, Permissions.StockItemDistribution.UpdateGuid, ExecutionTime, DbCMS);
            }

            try
            {
                DbISS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbISS.SingleUpdateMessage(null, null, DbISS.RowVersionControls(ISSItem, Language)));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyISSItem(model.ItemGUID);
            }
            catch (Exception ex)
            {
                return Json(DbISS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ISSItemDelete(codeISSItem model)
        {
            if (!CMS.HasAction(Permissions.StockItemDistribution.Delete, Apps.ISS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeISSItem> DeletedIISItem = DeleteISSItem(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.StockItemDistribution.Restore, Apps.ISS), Container = "ISSItemFormControls" });

            try
            {
                int CommitedRows = DbISS.SaveChanges();
                DbISS.SaveChanges();
                return Json(DbISS.SingleDeleteMessage(CommitedRows, DeletedIISItem.FirstOrDefault(), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyISSItem(model.ItemGUID);
            }
            catch (Exception ex)
            {
                return Json(DbISS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ISSItemRestore(codeISSItem model)
        {
            if (!CMS.HasAction(Permissions.StockItemDistribution.Restore, Apps.ISS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveISSItem(model))
            {
                return Json(DbISS.RecordExists());
            }

            List<codeISSItem> RestoredIISItem = RestoreISSItem(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.StockItemDistribution.Create, Apps.ISS, new UrlHelper(Request.RequestContext).Action("IISItemCreate", "Configuration", new { Area = "ISS" })), Container = "IISItemFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.StockItemDistribution.Update, Apps.ISS), Container = "IISItemFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.StockItemDistribution.Delete, Apps.ISS), Container = "IISItemFormControls" });

            try
            {
                int CommitedRows = DbISS.SaveChanges();
                DbISS.SaveChanges();
                return Json(DbISS.SingleRestoreMessage(CommitedRows, RestoredIISItem, DbISS.PrimaryKeyControl(RestoredIISItem.FirstOrDefault()), Url.Action(DataTableNames.ISSItemDataTable, Portal.GetControllerName(ControllerContext)), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyISSItem(model.ItemGUID);
            }
            catch (Exception ex)
            {
                return Json(DbISS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ISSItemDataTableDelete(List<codeISSItem> models)
        {
            if (!CMS.HasAction(Permissions.StockItemDistribution.Delete, Apps.ISS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeISSItem> DeletedItemClassificaiton = DeleteISSItem(models);

            try
            {
                DbISS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbISS.PartialDeleteMessage(DeletedItemClassificaiton, models, DataTableNames.ISSItemDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbISS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ISSItemDataTableRestore(List<codeISSItem> models)
        {
            if (!CMS.HasAction(Permissions.StockItemDistribution.Restore, Apps.ISS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<codeISSItem> RestoredIISItem = DeleteISSItem(models);

            try
            {
                DbISS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbISS.PartialRestoreMessage(RestoredIISItem, models, DataTableNames.ISSItemDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbISS.ErrorMessage(ex.Message));
            }
        }

        private List<codeISSItem> DeleteISSItem(List<codeISSItem> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<codeISSItem> DeletedIISItem = new List<codeISSItem>();
            //Fix
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = "SELECT WarehouseIISItemGUID,CONVERT(varchar(50), WarehouseIISItemGUID) as C2 ,codeWarehouseIISItemRowVersion FROM code.codeWarehouseIISItem where WarehouseIISItemGUID in (" + string.Join(",", models.Select(x => "'" + x.WarehouseIISItemGUID + "'").ToArray()) + ")";

            string query = DbISS.QueryBuilder(models, Permissions.StockItemDistribution.DeleteGuid, SubmitTypes.Delete, "");

            var Records = DbISS.Database.SqlQuery<codeISSItem>(query).ToList();
            foreach (var record in Records)
            {
                DeletedIISItem.Add(DbISS.Delete(record, ExecutionTime, Permissions.StockItemDistribution.DeleteGuid, DbCMS));
            }

            var Languages = DeletedIISItem.SelectMany(a => a.codeISSItemLanguage).Where(l => l.Active).ToList();
            foreach (var language in Languages)
            {
                DbISS.Delete(language, ExecutionTime, Permissions.StockItemDistribution.DeleteGuid, DbCMS);
            }
            return DeletedIISItem;
        }

        private List<codeISSItem> RestoreISSItem(List<codeISSItem> models)
        {

            DateTime RestoringTime = DateTime.Now;

            List<codeISSItem> RestoredISSItem = new List<codeISSItem>();
            //Fix
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = "SELECT WarehouseIISItemGUID,CONVERT(varchar(50), WarehouseIISItemGUID) as C2 ,codeWarehouseIISItemRowVersion FROM code.codeWarehouseIISItem where WarehouseIISItemGUID in (" + string.Join(",", models.Select(x => "'" + x.WarehouseIISItemGUID + "'").ToArray()) + ")";

            string query = DbISS.QueryBuilder(models, Permissions.StockItemDistribution.DeleteGuid, SubmitTypes.Restore, "");

            var Records = DbISS.Database.SqlQuery<codeISSItem>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveISSItem(record))
                {
                    RestoredISSItem.Add(DbISS.Restore(record, Permissions.StockItemDistribution.DeleteGuid, Permissions.StockItemDistribution.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            var Languages = RestoredISSItem.SelectMany(x => x.codeISSItemLanguage.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var language in Languages)
            {
                DbISS.Restore(language, Permissions.StockItemDistribution.DeleteGuid, Permissions.StockItemDistribution.RestoreGuid, RestoringTime, DbCMS);
            }

            return RestoredISSItem;
        }

        private JsonResult ConcurrencyISSItem(Guid PK)
        {
            ItemModel dbModel = new ItemModel();

            var IISItem = DbISS.codeISSItem.Where(x => x.ItemGUID == PK).FirstOrDefault();
            var dbIISItem = DbISS.Entry(IISItem).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbIISItem, dbModel);

            var Language = DbISS.codeISSItemLanguage.Where(x => x.ItemGUID == PK).Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeISSItem.DeletedOn) && x.LanguageID == LAN).FirstOrDefault();
            var dbLanguage = DbISS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (IISItem.codeISSItemRowVersion.SequenceEqual(dbModel.codeISSItemRowVersion) && Language.codeISSItemLanguageRowVersion.SequenceEqual(dbModel.codeISSItemLanguageRowVersion))
            {
                return Json(DbISS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbISS, dbModel, "LanguagesContainer"));
        }

        private bool ActiveISSItem(Object model)
        {
            codeISSItemLanguage IISItem = Mapper.Map(model, new codeISSItemLanguage());
            int ModelDescription = DbISS.codeISSItemLanguage
                                    .Where(x => x.ItemDescription == IISItem.ItemDescription &&
                                                x.LanguageID == LAN &&
                                                x.Active).Count();
            if (ModelDescription > 0)
            {
                ModelState.AddModelError("ModelDescription", "item is already exists");
            }
            return (ModelDescription > 0);
        }

        #endregion
    }
}