using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using ISS_DAL.Model;

namespace AppsPortal.Areas.ISS.Controllers
{
    public class GetStockOfflineJsonController : Controller
    {
        // GET: ISS/GetStockOfflineJson
        public ISSEntities _db=new ISSEntities();
        [System.Web.Http.HttpGet]
        public JsonResult GetStockItem()
        {
            string LAN = "EN";
            List<StockItemBalanceVM> Final = new List<StockItemBalanceVM>();

            var res = _db.dataItemStockBalance.Where(x => x.dataTrackStockUpload.IsLastUpload == true).ToList();
            var stocks = _db.codeISSStockLanguage.Where(x => x.LanguageID == LAN).OrderBy(x => x.StockDescription).ToList();
            foreach (var item in stocks)
            {
                var myResults = res.Where(x => x.StockGUID == item.StockGUID).ToList();
                StockItemBalanceVM myFinal = new StockItemBalanceVM
                {
                    StockName = item.StockDescription
                };

                List<StockItemQtyVM> itemsInStock = new List<StockItemQtyVM>();
                foreach (var myRes in myResults)
                {
                    StockItemQtyVM myitemsInStock = new StockItemQtyVM
                    {
                        ItemName = myRes.codeISSItem.codeISSItemLanguage.Where(x => x.LanguageID == LAN).Select(x => x.ItemDescription).FirstOrDefault(),
                        Qty = (int)myRes.TotalItem
                    };
                    itemsInStock.Add(myitemsInStock);
                }
                myFinal.stockItems.AddRange(itemsInStock);
                Final.Add(myFinal);


            }

            return Json(Final, JsonRequestBehavior.AllowGet);
        }

            

        public class StockItemBalanceVM
        {
            public StockItemBalanceVM()
            {
                stockItems = new List<StockItemQtyVM>();
            }
            public string StockName { get; set; }

            public List<StockItemQtyVM> stockItems { get; set; }
        }

        public class StockItemQtyVM
        {
            public int Qty { get; set; }



            public string ItemName { get; set; }
        }




        public JsonResult GetStockItemPipline()
        {
            string LAN = "EN";
            List<StockItemBalanceNewVM> Final = new List<StockItemBalanceNewVM>();

            var res = _db.dataItemPipeline.Where(x => x.dataItemPipelineUpload.IsLastUpload == true).ToList();
            var stocks = _db.codeISSStockLanguage.Where(x => x.LanguageID == LAN).OrderBy(x => x.StockDescription).ToList();
            foreach (var item in stocks)
            {
                var myResults = res.Where(x => x.StockGUID == item.StockGUID).ToList();
                StockItemBalanceNewVM myFinal = new StockItemBalanceNewVM
                {
                    StockName = item.StockDescription
                };

                List<StockItemNewQtyVM> itemsInStock = new List<StockItemNewQtyVM>();
                foreach (var myRes in myResults)
                {
                    StockItemNewQtyVM myitemsInStock = new StockItemNewQtyVM
                    {
                        ItemName = myRes.codeISSItem.codeISSItemLanguage.Where(x => x.LanguageID == LAN).Select(x => x.ItemDescription).FirstOrDefault(),
                        Qty = (int)myRes.Quantity,
                        ETA = myRes.ETA.Value.ToShortDateString()
                    };
                    itemsInStock.Add(myitemsInStock);
                }
                myFinal.stockItems.AddRange(itemsInStock);
                Final.Add(myFinal);


            }

            return Json(Final, JsonRequestBehavior.AllowGet) ;
        }


        public class StockItemBalanceNewVM
        {
            public StockItemBalanceNewVM()
            {
                stockItems = new List<StockItemNewQtyVM>();
            }
            public string StockName { get; set; }

            public List<StockItemNewQtyVM> stockItems { get; set; }
        }

        public class StockItemNewQtyVM
        {
            public int Qty { get; set; }

            public string ETA { get; set; }

            public string ItemName { get; set; }
        }

        #region Overview

        public JsonResult GetStockItemOverview()
        {
            string LAN = "EN";
            List<ItemOverviewVM> Final = new List<ItemOverviewVM>();

            var res = _db.dataItemOverview.ToList();

            foreach (var item in res)
            {

                ItemOverviewVM myitemsInStock = new ItemOverviewVM
                {
                    ItemName = item.codeISSItem.codeISSItemLanguage.Where(x => x.LanguageID == LAN).Select(x => x.ItemDescription).FirstOrDefault(),
                    TotalItemInAllStocks = item.TotalItemAllStocks ?? 0,
                    ItemPipline = item.TotalItemPipeLine ?? 0,
                    ItemStockWithPipline = item.TotalItemStockWithPipeLine ?? 0,


                };


                Final.Add(myitemsInStock);


            }

            return Json(Final, JsonRequestBehavior.AllowGet);
        }

        public class ItemOverviewVM
        {

            public string ItemName { get; set; }
            public int? TotalItemInAllStocks { get; set; }
            public int? ItemPipline { get; set; }
            public int? ItemStockWithPipline { get; set; }



        }
        #endregion
    }
}