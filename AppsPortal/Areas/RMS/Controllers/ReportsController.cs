
using AppsPortal.BaseControllers;
using Microsoft.Reporting.WebForms;
using RMS_DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace AppsPortal.Areas.RMS.Controllers
{
    public class ReportsController : RMSBaseController
    {

        [HttpGet]
        public ActionResult ReportIndex()
        {

            return View("~/Areas/RMS/Views/Reports/ReportIndex.cshtml", new RMSReportParametersList());
        }

        #region Charts
        [HttpGet]
        public ActionResult Chart()
        {
            ReportViewer reportViewer = new ReportViewer();
            ViewBag.ReportViewer = reportViewer;
            return View("~/Areas/RMS/Views/Reports/Charts.cshtml", new RMSReportParametersList());
        }
        class HieghChartReport
        {
            public List< series> MainReport { get; set; }
        }
        class series
        {
            public string name { set; get; }
            public double[] data { get; set; }
        }


        
        [HttpPost]
        public ActionResult chart(RMSReportParametersList rp)
        {
            var MainData = (from a in DbRMS.dataPrinterLog.AsEnumerable().Where(x => x.LogDateTime>=rp.LogDateTimeStart && x.LogDateTime<=rp.LogDateTimeEnd && x.PrinterConfigurationGUID == rp.PrinterConfigurationGUID && rp.OidGUID.Contains(x.OidGUID))
                            join b in DbRMS.codeOID on a.OidGUID equals b.OidGUID
                            orderby a.LogDateTime
                            group new
                            {
                                b.OIDDescription,
                                a.LogDateTime,
                                a.OidValue
                            } by b.OIDDescription into G
                          
                            select
                            new series
                            {
                                name = G.Key,
                                data = (from x in G select  Convert.ToDouble(x.OidValue) ).ToArray()
                            }).ToList();

            return Json(new HieghChartReport { MainReport= MainData}, JsonRequestBehavior.AllowGet);
        }
      
        #endregion
    }
}