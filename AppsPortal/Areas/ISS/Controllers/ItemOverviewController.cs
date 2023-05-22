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
using System.Data.Entity.Validation;

namespace AppsPortal.Areas.ISS.Controllers
{
    public class ItemOverviewController : ISSBaseController
    {
        // GET: ISS/ItemOverview
        [Route("ISS/ItemOverview/")]
        public ActionResult ItemOverviewIndex()
        {
            if (!CMS.HasAction(Permissions.StockItemDistribution.Access, Apps.ISS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/ISS/Views/ItemOverview/Index.cshtml");
        }

        [Route("ISS/ISSItemOverviewDataTable/")]
        public JsonResult ISSItemOverviewDataTable(DataTableRecievedOptions options)
        {

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<ItemOverviewPipelineDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<ItemOverviewPipelineDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.StockItemDistribution.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();
            //Fix
            var All = (

                from a in DbISS.dataItemOverview.Where(x => x.Active ).AsExpandable()
                join b in DbISS.codeISSItemLanguage.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.DeletedOn) && x.LanguageID == LAN) on a.ItemGUID equals b.ItemGUID


                into LJ1
                from R1 in LJ1.DefaultIfEmpty()

                select new ItemOverviewPipelineDataTableModel
                {
                    ItemOverviewGUID = a.ItemOverviewGUID,
                    Active = a.Active,
                    TotalItemAllStocks = a.TotalItemAllStocks,
                    TotalItemPipeLine = a.TotalItemPipeLine,
                    TotalItemStockWithPipeLine = a.TotalItemStockWithPipeLine,
                    TotalReservedForEmergency = a.TotalReservedForEmergency,
                    ItemGUID = a.ItemGUID.ToString(),
                    
ItemName = R1.ItemDescription,
                    dataItemOverviewRowVersion = a.dataItemOverviewRowVersion
                }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<ItemOverviewPipelineDataTableModel> Result = Mapper.Map<List<ItemOverviewPipelineDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        #region Import

        
        public ActionResult ItemOverviewCreate()
        {
            if (!CMS.HasAction(Permissions.StockItemDistribution.Create, Apps.ISS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/ISS/Views/ItemOverview/_ItemOverviewUpdateModal.cshtml", new dataItemOverview());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ItemOverviewCreate(dataItemOverview model)
        {
            if (!CMS.HasAction(Permissions.StockItemDistribution.Create, Apps.ISS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();
            Guid stockAllGUid = Guid.Parse("972cbdc5-6444-49da-b71f-af1375df1ea4");
            var piplines = DbISS.dataItemPipeline.Where(x=>x.dataItemPipelineUpload.IsLastUpload==true
                 && x.StockGUID == stockAllGUid).ToList();
            var databanalce = DbISS.dataItemStockBalance.Where(x=>x.dataTrackStockUpload.IsLastUpload==true
             && x.StockGUID== stockAllGUid).ToList();
            var items = DbISS.codeISSItem.ToList();
            var toRemove=DbISS.dataItemOverview.ToList();
            DbISS.dataItemOverview.RemoveRange(toRemove);
            List<dataItemOverview> itemOverviews = new List<dataItemOverview>();
            foreach (var item in items)
            {
                dataItemOverview myItem = new dataItemOverview
                {
                    ItemOverviewGUID = Guid.NewGuid(),
                    ItemGUID = item.ItemGUID,
                    TotalItemAllStocks = databanalce.Where(x => x.ItemGUID == item.ItemGUID).Select(x => x.TotalItem).FirstOrDefault(),
                TotalItemPipeLine = piplines.Where(x => x.ItemGUID == item.ItemGUID).Select(x => x.Quantity).FirstOrDefault(),
                    TotalItemStockWithPipeLine = databanalce.Where(x => x.ItemGUID == item.ItemGUID).Select(x => x.TotalItem).FirstOrDefault()+ piplines.Where(x => x.ItemGUID == item.ItemGUID).Select(x => x.Quantity).FirstOrDefault(),
                    Active=true,

                };
                itemOverviews.Add(myItem);
            }

            DbISS.CreateBulk(itemOverviews, Permissions.StockItemDistribution.CreateGuid, DateTime.Now, DbCMS);
            try
            {
                DbISS.SaveChanges();
                DbCMS.SaveChanges();
                dataItemOverview itemOverview = DbISS.dataItemOverview.FirstOrDefault();

                return Json(DbISS.SingleUpdateMessage(DataTableNames.ISSItemOverviewDataTable, DbISS.PrimaryKeyControl(itemOverview), DbISS.RowVersionControls(Portal.SingleToList(itemOverview))));
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
        }

        #endregion

    }
}