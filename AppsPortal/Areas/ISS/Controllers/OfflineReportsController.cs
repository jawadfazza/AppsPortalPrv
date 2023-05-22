using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISS_DAL.Model;
using ISS_DAL.ViewModels;
using AppsPortal.BaseControllers;

namespace AppsPortal.Areas.ISS.Controllers
{
    public class OfflineReportsController : ISSBaseController
    {
        // GET: ISS/OfflineReports
        [Route("ISS/GetOfflineReportsItem/")]
        public ActionResult GetOfflineReportsItem()
        {

            var All = (

               from a in DbISS.dataItemStockBalance.Where(x => x.Active && x.dataTrackStockUpload.IsLastUpload == true)
               join b in DbISS.codeISSItemLanguage.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.DeletedOn) && x.LanguageID == LAN) on a.ItemGUID equals b.ItemGUID
               join c in DbISS.codeISSStockLanguage.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.DeletedOn) && x.LanguageID == LAN) on a.StockGUID equals c.StockGUID

               into LJ1
               from R1 in LJ1.DefaultIfEmpty()
               join d in DbISS.codeISSStock on R1.StockGUID equals d.StockGUID into LJ2
               from R2 in LJ2.DefaultIfEmpty()

               select new StockItemDistributionDataTableModel
               {
                   ItemStockBalanceGUID = a.ItemStockBalanceGUID,
                   Active = a.Active,
                   TrackStockUploadGUID = a.TrackStockUploadGUID,
                   ItemGUID = a.ItemGUID.ToString(),
                   StockGUID = a.StockGUID.ToString(),
                   ItemName = b.ItemDescription,
                   StockName = R1.StockDescription,
                   TotalItem = a.TotalItem,
                   OrderId = R2.OrderId,

                   dataItemStockBalanceRowVersion = a.dataItemStockBalanceRowVersion
               }).ToList();
            var lastUpload = DbISS.dataTrackStockUpload.Where(x => x.IsLastUpload == true).FirstOrDefault();
            ViewBag.LastUploadDate = lastUpload.UploadDate.Value.ToShortDateString();
            // var result = DbISS.dataItemStockBalance.Select(x=>new { x.codeISSItem.codeISSItemLanguage.});
            return View("~/Areas/ISS/Views/OfflineReports/ItemStock.cshtml", All.OrderBy(x => x.OrderId).ThenBy(x => x.ItemName));
            ///return View(result.ToList());
        }

        [Route("ISS/GetOfflineReportsItemPipline/")]

        public ActionResult GetOfflineReportsItemPipline()
        {

            var lastUpload = DbISS.dataItemPipelineUpload.Where(x => x.IsLastUpload == true).FirstOrDefault();
            ViewBag.LastUploadDate = lastUpload.UploadDate.Value.ToShortDateString();


            var All = (

            from a in DbISS.dataItemPipeline.Where(x => x.Active && x.dataItemPipelineUpload.IsLastUpload == true)
            join b in DbISS.codeISSItemLanguage.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.DeletedOn) && x.LanguageID == LAN) on a.ItemGUID equals b.ItemGUID into LJ1
            from R1 in LJ1.DefaultIfEmpty()
            join c in DbISS.codeISSStockLanguage.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.DeletedOn) && x.LanguageID == LAN) on a.StockGUID equals c.StockGUID
            join d in DbISS.codeISSStock on c.StockGUID equals d.StockGUID into LJ2
            from R2 in LJ2.DefaultIfEmpty()


            select new ItemPipelineDataTableModel
            {
                ItemPipelineGUID = a.ItemPipelineGUID,
                Active = a.Active,
                ItemPipelineUploadGUID = a.ItemPipelineUploadGUID,
                ItemGUID = a.ItemGUID.ToString(),
                StockGUID = a.StockGUID.ToString(),
                StockName = c.StockDescription,
                ItemName = R1.ItemDescription,
                OrderId = R2.OrderId,
                ETA = a.ETA,
                Quantity = a.Quantity,

                dataItemPipelineRowVersion = a.dataItemPipelineRowVersion
            }).ToList();


            // var result = DbISS.dataItemStockBalance.Select(x=>new { x.codeISSItem.codeISSItemLanguage.});
            return View("~/Areas/ISS/Views/OfflineReports/ItemPipline.cshtml", All.OrderBy(x => x.OrderId).ThenBy(x => x.ItemName));
            ///return View(result.ToList());
        }
        [Route("ISS/GetOfflineReportsItemOverview/")]

        public ActionResult GetOfflineReportsItemOverview()
        {

            var All = (

      from a in DbISS.dataItemOverview.Where(x => x.Active)
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
      });


            return View("~/Areas/ISS/Views/OfflineReports/ItemOverview.cshtml", All.OrderBy(x => x.ItemName));

        }
    }
}