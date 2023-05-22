using AppsPortal.BaseControllers;
using AppsPortal.Library;
using AppsPortal.SHM.ViewModels;
using SHM_DAL.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.SHM.Controllers
{
    public class HomeController : SHMBaseController
    {
        // GET: SHM/Home
        public ActionResult Index()
        {
            CMS.SetUserToken(UserProfileGUID, Apps.SHM);
            Session[SessionKeys.CurrentApp] = Apps.SHM;
            CMS.BuildUserMenus(UserGUID, LAN);
            return View();
        }

        [HttpGet]
        public ActionResult CodeTable()
        {
            return View("~/Areas/SHM/Views/Home/CodeTable.cshtml");
        }

        [HttpPost]
       

        public ActionResult GeShuttlesPerMonth()
        {

            List<Months> MonthNames = new List<Months>()
                 {
                     new Months(){Name="Jan",MonthOrder=1 },
                     new Months(){Name="Feb",MonthOrder=2 },
                     new Months(){Name="Mar",MonthOrder=3 },
                     new Months(){Name="Apr",MonthOrder=4 },
                     new Months(){Name="May",MonthOrder=5 },
                     new Months(){Name="Jun",MonthOrder=6 },
                     new Months(){Name="Jul",MonthOrder=7 },
                     new Months(){Name="Aug",MonthOrder=8 },
                     new Months(){Name="Sep",MonthOrder=9 },
                     new Months(){Name="Oct",MonthOrder=10 },
                     new Months(){Name="Nov",MonthOrder=11 },
                     new Months(){Name="Dec",MonthOrder=12 }
                 };
            var ShuttlesPerMonth = (from a in DbSHM.dataShuttle.Where(x => x.Active)
                                    //join b in DbSHM.dataShuttleRoute.Where(x => x.Active) on a.ShuttleGUID equals b.ShuttleGUID
                                    join c in DbSHM.codeLocationsLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.EndLocationGUID equals c.LocationGUID
                                    group new
                                    {

                                        c.LocationDescription
                                    } by new { c.LocationDescription, a.EndLocationGUID } into G
                                    select new drilldown
                                    {
                                        name = G.Key.LocationDescription,
                                        LocationGUID = G.Key.EndLocationGUID.Value
                                    }

                                    ).ToList();

            //ShuttlesPerMonth.ForEach(r =>
            //                          r.data = (from a in MonthNames
            //                                   // join b in DbSHM.dataShuttleRoute.Where(x => x.Active && x.EndLocationGUID == r.LocationGUID) on a.MonthOrder equals b.dataShuttle.DepartureDateTime.Month into LJ1
            //                                    from R1 in LJ1.DefaultIfEmpty(new dataShuttleRoute() { EndLocationGUID = default(Guid) })
            //                                    group new { R1.EndLocationGUID, a.Name, } by new { a.Name, R1.EndLocationGUID, a.MonthOrder } into grp
            //                                    orderby grp.Key.MonthOrder
            //                                    select new data
            //                                    {
            //                                        y = grp.Key.EndLocationGUID != Guid.Empty ? grp.Count() : 0
            //                                    }).ToArray()
            //                          );

            return Json(new { Model = ShuttlesPerMonth });
        }

        [HttpPost]
        public ActionResult GeTripByVehicle()
        {
            var TripByVehicleCount = (from a in DbSHM.dataShuttle.Where(x => x.Active)
                                      join b in DbSHM.dataShuttleVehicle.Where(x => x.Active) on a.ShuttleGUID equals b.ShuttleGUID
                                      join c in DbSHM.codeVehicle.Where(x => x.Active) on b.VehicleGUID equals c.VehicleGUID
                                      group new
                                      {
                                          b.ShuttleVehicleGUID,
                                          c.VehicleNumber
                                      } by c.VehicleNumber into G
                                      select new HighChartPieModel
                                      {
                                          name = G.Key,
                                          y = G.Count(),
                                          selected = true,
                                          sliced = true
                                      }
                                      ).ToList();
            return Json(new { Model = TripByVehicleCount });
        }

        //public ActionResult GeShuttleStaffPerMonth()
        //{

        //    List<Months> MonthNames = new List<Months>()
        //         {
        //             new Months(){Name="Jan",MonthOrder=1 },
        //             new Months(){Name="Feb",MonthOrder=2 },
        //             new Months(){Name="Mar",MonthOrder=3 },
        //             new Months(){Name="Apr",MonthOrder=4 },
        //             new Months(){Name="May",MonthOrder=5 },
        //             new Months(){Name="Jun",MonthOrder=6 },
        //             new Months(){Name="Jul",MonthOrder=7 },
        //             new Months(){Name="Aug",MonthOrder=8 },
        //             new Months(){Name="Sep",MonthOrder=9 },
        //             new Months(){Name="Oct",MonthOrder=10 },
        //             new Months(){Name="Nov",MonthOrder=11 },
        //             new Months(){Name="Dec",MonthOrder=12 }
        //         };

        //    var staffs = (from a in MonthNames
        //                  join b in DbSHM.dataShuttleStaff.Where(x => x.Active && x.dataShuttle.Active) on a.MonthOrder equals b.dataShuttle.DepartureDateTime.Month into LJ1
        //                  from R1 in LJ1.DefaultIfEmpty(new dataShuttleStaff() { ShuttleStaffGUID = default(Guid) })
        //                  group new { a.MonthOrder } by new { R1.ShuttleGUID, a.MonthOrder } into grp
        //                  orderby grp.Key.MonthOrder
        //                  select new data
        //                  {
        //                      y = grp.Key.ShuttleGUID != Guid.Empty ? grp.Count() : 0
        //                  }).ToArray();

        //    var Vehicle = (from a in MonthNames
        //                   join b in DbSHM.dataShuttleVehicle.Where(x => x.Active && x.dataShuttle.Active) on a.MonthOrder equals b.dataShuttle.DepartureDateTime.Month into LJ1
        //                   from R1 in LJ1.DefaultIfEmpty(new dataShuttleVehicle() { ShuttleVehicleGUID = default(Guid) })
        //                   group new { a.MonthOrder } by new { R1.ShuttleGUID, a.MonthOrder } into grp
        //                   orderby grp.Key.MonthOrder
        //                   select new data
        //                   {
        //                       y = grp.Key.ShuttleGUID != Guid.Empty ? grp.Count() : 0
        //                   }).ToArray();

        //    return Json(new { Staffs = staffs.Take(12), Vehicle = Vehicle.Take(12) });
        //}


    }

    class Months
    {
        public string Name { set; get; }
        public int MonthOrder { set; get; }

    }
    class drilldown
    {
        public string name { set; get; }
        public Guid LocationGUID { set; get; }
        public data[] data { get; set; }
    }
    class data
    {
        public double y { set; get; }
    }
    class HighChartPieModel
    {
        public string name { set; get; }
        public double y { set; get; }
        public bool selected { get; set; }

        public bool sliced { get; set; }
    }
}