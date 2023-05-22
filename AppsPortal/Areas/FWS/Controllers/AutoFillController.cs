using AppsPortal.BaseControllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.FWS.Controllers
{
    public class AutoFillController : FWSBaseController
    {

        public JsonResult GetLocationsByLong(string LocationLong)
        {
            var result = (from a in DbFWS.code4WSLocation.AsNoTracking()
                          where a.LocationGUID.ToString() == LocationLong
                          select new
                          {
                              a.Admin1RefName,
                              a.admin1Pcode,
                              a.Admin2RefName,
                              a.admin2Pcode,
                              a.Admin3RefName,
                              a.admin3Pcode,
                              a.Admin4RefName,
                              a.admin4Pcode2,
                              a.NeibourhoodRefName,
                              a.NeibourhoodPcode2
                          }).FirstOrDefault();
            return Json(new
            {
                Admin1Name = result.Admin1RefName,
                Admin1Code = result.admin1Pcode,
                Admin2Name = result.Admin2RefName,
                Admin2Code = result.admin2Pcode,
                Admin3Name = result.Admin3RefName,
                Admin3Code = result.admin3Pcode,
                Admin4Name = result.Admin4RefName,
                Admin4Code = result.admin4Pcode2,
                NeibourhoodRefName = result.NeibourhoodRefName,
                NeibourhoodPcode2 = result.NeibourhoodPcode2
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUnitByActivityLong(string ActivityLong)
        {
            var result = (from a in DbFWS.code4WSActivity
                          join b in DbFWS.code4WSSubSector on a.SubSectorID equals b.SubSectorID
                          where a.SubActivity == ActivityLong
                          select new
                          {
                              FWSUnit = a.FWSUnit,
                              FWSAnalysisUnit = a.FWSUnitDetails,
                              FWSSubSector = a.SubSectorEn,
                              FWSActivity = a.ActivityEn,
                              FWSSubActivity = a.SubActivityOrg,
                              SubSectorID = a.SubSectorID,
                              SubSectorGUID = b.SubSectorGUID
                          }).Distinct().FirstOrDefault();

            return Json(new
            {
                FWSUnit = result.FWSUnit,
                FWSAnalysisUnit = result.FWSAnalysisUnit,
                FWSSubSector = result.FWSSubSector,
                FWSActivity = result.FWSActivity,
                FWSSubActivity = result.FWSSubActivity,
                SubSectorID = result.SubSectorID,
                SubSectorGUID = result.SubSectorGUID
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            return View();
        }
    }
}