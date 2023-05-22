using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.ViewModels;
using AutoMapper;
using LinqKit;
using OfficeOpenXml;
using RMS_DAL.Model;
using SnmpSharpNet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.RMS.Controllers
{
    public class HomeController : RMSBaseController
    {
        // GET: AMS/Home
        public ActionResult Index()
        {
            CMS.SetUserToken(UserProfileGUID, Apps.RMS);
            Session[SessionKeys.CurrentApp] = Apps.RMS;
            CMS.BuildUserMenus(UserGUID, LAN);
            return View();
        }
        public class OIDType
        {
            public static Guid RunningTimeStatus = Guid.Parse("8D749720-9036-4D53-A90D-002E6621A001");
            public static Guid TrayStatus = Guid.Parse("8D749720-9036-4D53-A90D-002E6621A002");
            public static Guid CartridgeType = Guid.Parse("8D749720-9036-4D53-A90D-002E6621A003");
            public static Guid SupplyLevel = Guid.Parse("8D749720-9036-4D53-A90D-002E6621A004");
            public static Guid PrinterName = Guid.Parse("8D749720-9036-4D53-A90D-002E6621A005");
            public static Guid FunctionStatus = Guid.Parse("8D749720-9036-4D53-A90D-002E6621A006");
            public static Guid PaperCount = Guid.Parse("8D749720-9036-4D53-A90D-002E6621A007");
        }
        #region Class
        class HieghChartReport
        {
            public string[] categories { set; get; }
            public List<series> MainReport { get; set; }
        }
        class series
        {
            public string name { set; get; }
            public int[] data { get; set; }

            public int y { get; set; }
        }

        class data
        {
            public double y { set; get; }
        }

        public class Category
        {
            public string CatName { get; set; }
            public string CatValue { get; set; }
        }

        #endregion

        [HttpPost]
        public ActionResult SupplyLevelChart(RMSReportParametersList rp)
        {

            var MainData = (from a in DbRMS.PrinterLogs(rp.LogDateTimeStart,rp.LogDateTimeEnd,rp.PrinterConfigurationGUID, OIDType.SupplyLevel)

                            orderby a.LogDateTime
                            group new
                            {
                                a.OIDDescription,
                                a.LogDateTime,
                                a.OidValue
                            } by a.OIDDescription into G

                            select
                            new series
                            {
                                name = G.Key,
                                data = (from x in G select Convert.ToInt32(x.OidValue)).ToArray()
                            }).ToList();

            return Json(new HieghChartReport { MainReport = MainData }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult PaperCountChart(RMSReportParametersList rp)
        {
            var MainData = (from a in DbRMS.PrinterLogs(rp.LogDateTimeStart, rp.LogDateTimeEnd, rp.PrinterConfigurationGUID, OIDType.PaperCount)
                            orderby a.LogDateTime
                            group new
                            {
                                a.OIDDescription,
                                a.LogDateTime,
                                a.OidValue
                            } by a.OIDDescription into G

                            select
                            new series
                            {
                                name = G.Key,
                                data = (from x in G select Convert.ToInt32(x.OidValue)).ToArray()
                            }).ToList();

            return Json(new HieghChartReport { MainReport = MainData }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult FunctionStatusChart(RMSReportParametersList rp)
        {

            List<Category> Categories = new List<Category>()
                 {
                     new Category(){CatName="unknown",CatValue="1" },
                     new Category(){CatName="running",CatValue="2" },
                     new Category(){CatName="warning",CatValue="3" },
                     new Category(){CatName="testing",CatValue="4" },
                     new Category(){CatName="down",CatValue="5" },

                 };
            var OIdNames = (from a in DbRMS.dataPrinterConfiguration.Where(x => x.PrinterConfigurationGUID == rp.PrinterConfigurationGUID)
                            from b in DbRMS.codeOID.Where(x => x.OIDTypeGUID == OIDType.FunctionStatus && x.PrinterModelGUID==a.PrinterModelGUID && x.PrinterTypeGUID==a.PrinterTypeGUID && x.Active ) 

                            select new
                            {
                                b.OidGUID,
                                b.OIDDescription,
                                b.PrinterModelGUID,
                                b.PrinterTypeGUID
                            }
                          ).ToArray();
            List<series> allData = new List<series>();
            var OidGUID = OIdNames.Select(x => x.OidGUID).ToList();
            int indexVal = 1;
            foreach(var oid in OIdNames)
            {
                var MainData = (from a in Categories
                                join b in DbRMS.PrinterLogs(rp.LogDateTimeStart, rp.LogDateTimeEnd, rp.PrinterConfigurationGUID, OIDType.FunctionStatus).Where(x=> x.OidGUID==oid.OidGUID) on a.CatValue equals b.OidValue into LJ1
                                from R1 in LJ1.DefaultIfEmpty(new PrinterLogs_Result() )
                                orderby a.CatValue
                                group 
                                    a.CatValue  by new { a.CatValue,R1.OidGUID} into G
                                
                                select G.Key.OidGUID != Guid.Empty ? G.Count() : 0).ToArray();
                allData.Add(new series {name=oid.OIDDescription ,data=MainData });
                indexVal++;
            }
                          
          

            return Json(new HieghChartReport { MainReport = allData, categories = Categories.Select(x=>x.CatName).ToArray() }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult FunctionStatusChartPie(RMSReportParametersList rp)
        {

            List<Category> Categories = new List<Category>()
                 {
                     new Category(){CatName="unknown",CatValue="1" },
                     new Category(){CatName="running",CatValue="2" },
                     new Category(){CatName="warning",CatValue="3" },
                     new Category(){CatName="testing",CatValue="4" },
                     new Category(){CatName="down",CatValue="5" },
                 };
            
            var MainData = (from a in Categories
                            join b in DbRMS.PrinterLogs(rp.LogDateTimeStart, rp.LogDateTimeEnd, rp.PrinterConfigurationGUID, OIDType.FunctionStatus) on a.CatValue equals b.OidValue 
                            group a.CatName by a.CatName into G
                            orderby G.Key
                            select new series
                            {
                                name = G.Key,
                                y = G.Count()
                            }).ToList();

            return Json(new HieghChartReport { MainReport = MainData }, JsonRequestBehavior.AllowGet);
        }

        //PrinterInkCartridge
        [HttpPost]
        public ActionResult PrinterInkCartridge(RMSReportParametersList rp)
        {

            var MainData = (from a in DbRMS.dataPrinterInkCartridge.Where(x=>x.ChangeDate>=rp.LogDateTimeStart && x.ChangeDate<=rp.LogDateTimeEnd)
                            join b in DbRMS.codeOID on a.OidGUID equals b.OidGUID
                            join c in DbRMS.codeTablesValuesLanguages.Where(x=>x.Active && x.LanguageID==LAN) on b.PrinterTypeGUID equals c.ValueGUID
                            join d in DbRMS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on b.PrinterModelGUID equals d.ValueGUID
                            group (b.OIDDescription+"-"+c.ValueDescription+"-"+d.ValueDescription) by (b.OIDDescription + "-" + c.ValueDescription + "-" + d.ValueDescription) into G
                            orderby G.Key
                            select new series
                            {
                                name = G.Key,
                                y = G.Count()
                            }).ToList();

            return Json(new HieghChartReport { MainReport = MainData }, JsonRequestBehavior.AllowGet);
        }

        class series1
        {
            public string name { set; get; }
            public string drilldown { set; get; }
            public data1 data { get; set; }
        }
        class drilldown1
        {
            public string name { set; get; }
            public string id { set; get; }
            public bool colorByPoint { get; set; }
            public data1[] data { get; set; }
        }
        class data1
        {
            public string name { set; get; }
            public string drilldown { set; get; }
            public double y { set; get; }
        }

        [HttpPost]
        public ActionResult PrinterInkCartridgeColumn(RMSReportParametersList rp)
        {

            var MainData = (from a in DbRMS.dataPrinterInkCartridge.Where(x => x.ChangeDate >= rp.LogDateTimeStart && x.ChangeDate <= rp.LogDateTimeEnd)
                            join b in DbRMS.codeOID on a.OidGUID equals b.OidGUID
                            join c in DbRMS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on b.PrinterTypeGUID equals c.ValueGUID
                            join d in DbRMS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on b.PrinterModelGUID equals d.ValueGUID 
                            
                          select new
                {
                     name= c.ValueDescription + "-" + d.ValueDescription,
                    a.Quantity
                }).GroupBy(x => x.name).Select(x => 
                     new data1
                    {
                        name = x.Key,
                        drilldown = x.Key,
                        y = x.Sum(y => y.Quantity.Value),

                    }
                ).ToArray();

            var DetailsData = (from a in DbRMS.dataPrinterInkCartridge.Where(x => x.ChangeDate >= rp.LogDateTimeStart && x.ChangeDate <= rp.LogDateTimeEnd)
                               join b in DbRMS.codeOID on a.OidGUID equals b.OidGUID
                               join c in DbRMS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on b.PrinterTypeGUID equals c.ValueGUID
                               join d in DbRMS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on b.PrinterModelGUID equals d.ValueGUID

                               select new
                               {
                                   name = c.ValueDescription + "-" + d.ValueDescription,
                                  b.OIDDescription,
                                  a.Quantity
                }).ToArray();

            List<drilldown1> details = new List<drilldown1>();
            foreach (var cat in MainData)
            {
                details.Add(new drilldown1
                {
                    id = cat.name,
                    name = cat.name,
                    data = DetailsData.Where(x => x.name == cat.name).GroupBy(x => x.OIDDescription).Select(x =>
                  new data1
                  {
                      name = x.Key,
                      y = x.Sum(y => y.Quantity.Value),

                  }).ToArray()
                });

            }
            return Json(new { MainReport = MainData, DetailsReport = details }, JsonRequestBehavior.AllowGet);
        }
    }
}
