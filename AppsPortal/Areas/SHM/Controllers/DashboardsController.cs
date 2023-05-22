using AppsPortal.BaseControllers;
using AppsPortal.Data;
using AppsPortal.SHM.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.SHM.Controllers
{
    public class DashboardsController : SHMBaseController
    {
        class seriesPie
        {
            public string name { set; get; }

            public double y { get; set; }
        }
        class drilldownLiner
        {
            public string name { set; get; }

            public string guid { get; set; }

            public dataLiner[] data { get; set; }
        }
        class dataLiner
        {
            public double y { set; get; }
        }
        private SHMReportParameters FillRP(SHMReportParameters rp)
        {
            if (rp.StartDate == null) { rp.StartDate = new DateTime(2019, 1, 1); }
            if (rp.EndDate == null) { rp.EndDate = DateTime.Now; }
            if (rp.DutyStationGUID == null) { rp.DutyStationGUID = DropDownList.SyriaDutyStations().Select(x => x.Value).ToList().ConvertAll(Guid.Parse).ToArray(); }
            if (rp.UserDriverGUID == null) { rp.UserDriverGUID = DropDownList.ShuttleDrivers().Select(x => x.Value).ToList().ConvertAll(Guid.Parse).ToArray(); }
            if (rp.UserPassengerGUID == null) { rp.UserPassengerGUID = DropDownList.ShuttlePassanger().Select(x => x.Value).ToList().ConvertAll(Guid.Parse).ToArray(); }
            if (rp.VehicleGUID == null) { rp.VehicleGUID = DropDownList.Vehicles().Select(x => x.Value).ToList().ConvertAll(Guid.Parse).ToArray(); }
            if (rp.CountryGUID == null) { rp.CountryGUID = DropDownList.CountriesSyriaShuttle().Select(x => x.Value).ToList().ConvertAll(Guid.Parse).ToArray(); }
            Guid LocationType = Guid.Parse("C970DDF5-31E5-47A1-BE76-AF15833D4E6A");
            if (rp.StartLocationGUID == null) { rp.StartLocationGUID = DropDownList.LocationsByCountries(rp.CountryGUID, LocationType).Select(x => x.Value).ToList().ConvertAll(Guid.Parse).ToArray(); }
            if (rp.EndLocationGUID == null) { rp.EndLocationGUID = DropDownList.LocationsByCountries(rp.CountryGUID, LocationType).Select(x => x.Value).ToList().ConvertAll(Guid.Parse).ToArray(); }

            return rp;
        }

        // GET: EMT/Dashboards
        public ActionResult Index()
        {
            return View("~/Areas/SHM/Views/Dashboards/Dashboards.cshtml");
        }

        /// <summary>
        /// Shuttle Travel Purposes persantage per type
        /// </summary>
        /// <param name="rp"></param>
        /// <returns></returns>
        public ActionResult GeShuttleTravelPurposes(SHMReportParameters rp)
        {
            rp = FillRP(rp);
            var categories = new DropDownList().ShuttleTravelPurpose().OrderBy(x => x.Text).Select(x => new { x.Text, x.Value }).ToArray();
            var RP_ShuttleReport = DbSHM.RP_ShuttleReport(
                                    rp.StartDate, rp.EndDate, LAN,
                                    string.Join(",", rp.DutyStationGUID),
                                    string.Join(",", rp.UserPassengerGUID),
                                    string.Join(",", rp.UserDriverGUID),
                                    string.Join(",", rp.VehicleGUID),
                                    string.Join(",", rp.StartLocationGUID),
                                    string.Join(",", rp.EndLocationGUID))
               .Select(x =>
             new
             {
                 x.UserGUID,
                 x.ShuttleTravelPurposeGUID
             }
            ).Distinct().ToList();

            double TotalNumberOfUsers = RP_ShuttleReport.Select(x => x.UserGUID).Distinct().Count();
            var Result = (from a in categories
                          join b in RP_ShuttleReport
                           on a.Value equals b.ShuttleTravelPurposeGUID.ToString()
                          group new { a.Value, b.UserGUID } by new { a.Text } into grp

                          select
                          new seriesPie
                          {
                              name = grp.Key.Text,
                              y = (Convert.ToDouble(grp.Count()) / TotalNumberOfUsers) * 100
                          }

                  ).ToArray();

            return Json(new { Result = Result.ToArray(), categories = Result.Select(x => x.name).ToArray() }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Shuttle End Location Per Months
        /// </summary>
        /// <param name="rp"></param>
        /// <returns></returns>
        public ActionResult ShuttleEndLocationPerMonths(SHMReportParameters rp)
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

            rp = FillRP(rp);
            Guid LocationType = Guid.Parse("C970DDF5-31E5-47A1-BE76-AF15833D4E6A");
            var categories = DropDownList.LocationsByCountries(rp.CountryGUID, LocationType).ToList();
            var RP_ShuttleReport = (from a in DbSHM.RP_ShuttleReport(
                          rp.StartDate, rp.EndDate, LAN,
                          string.Join(",", rp.DutyStationGUID),
                          string.Join(",", rp.UserPassengerGUID),
                          string.Join(",", rp.UserDriverGUID),
                          string.Join(",", rp.VehicleGUID),
                          string.Join(",", rp.StartLocationGUID),
                          string.Join(",", rp.EndLocationGUID))
                                    group new { a.EndLocationGUID } by new
                                    {
                                        date = a.DepartureDateTime,
                                        GroupGUID = a.EndLocationGUID,
                                        ShuttleGUID = a.ShuttleGUID
                                    }).Select(x => new MonthData()
                                    {
                                        date = x.Key.date,
                                        GroupGUID = x.Key.GroupGUID.ToString(),
                                        MainGroupGUID = x.Key.ShuttleGUID
                                    })
                        .ToList();

            var Result = (from a in RP_ShuttleReport
                          join c in categories on a.GroupGUID.ToString() equals c.Value
                          group new
                          {
                              c.Text
                          } by new { c.Text, c.Value } into G
                          select new drilldownLiner
                          {
                              name = G.Key.Text,
                              guid = G.Key.Value.ToString()
                          }

                                  ).ToList();

            Result.ForEach(r => r.data = (from a in MonthNames
                                          join b in RP_ShuttleReport.Where(x => x.GroupGUID.ToString().ToString() == r.guid) on a.MonthOrder equals b.date.Month into LJ1
                                          from R1 in LJ1.DefaultIfEmpty(new MonthData() { GroupGUID = "" })
                                          group new { R1.GroupGUID, a.MonthOrder, R1.MainGroupGUID } by new { R1.GroupGUID, a.MonthOrder } into grp
                                          orderby grp.Key.MonthOrder
                                          select new dataLiner
                                          {
                                              y = grp.Key.GroupGUID != "" ? grp.Count() : 0
                                          }).ToArray()
                                      );


            return Json(new { Result = Result, categories = MonthNames.Select(x => x.Name).ToArray() }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Shuttle End Location Per Months
        /// </summary>
        /// <param name="rp"></param>
        /// <returns></returns>
        public ActionResult ShuttleStartLocationPerMonths(SHMReportParameters rp)
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

            rp = FillRP(rp);
            Guid LocationType = Guid.Parse("C970DDF5-31E5-47A1-BE76-AF15833D4E6A");
            var categories = DropDownList.LocationsByCountries(rp.CountryGUID, LocationType).ToList();
            var RP_ShuttleReport = (from a in DbSHM.RP_ShuttleReport(
                         rp.StartDate, rp.EndDate, LAN,
                         string.Join(",", rp.DutyStationGUID),
                         string.Join(",", rp.UserPassengerGUID),
                         string.Join(",", rp.UserDriverGUID),
                         string.Join(",", rp.VehicleGUID),
                         string.Join(",", rp.StartLocationGUID),
                         string.Join(",", rp.EndLocationGUID))
                                    group new { a.EndLocationGUID } by new
                                    {
                                        date = a.DepartureDateTime,
                                        GroupGUID = a.StartLocationGUID,
                                        ShuttleGUID = a.ShuttleGUID
                                    }).Select(x => new MonthData()
                                    {
                                        date = x.Key.date,
                                        GroupGUID = x.Key.GroupGUID.ToString(),
                                        MainGroupGUID = x.Key.ShuttleGUID
                                    })
                       .ToList();

            var Result = (from a in RP_ShuttleReport
                          join c in categories on a.GroupGUID.ToString() equals c.Value
                          group new
                          {
                              c.Text
                          } by new { c.Text, c.Value } into G
                          select new drilldownLiner
                          {
                              name = G.Key.Text,
                              guid = G.Key.Value.ToString()
                          }

                                  ).ToList();

            Result.ForEach(r => r.data = (from a in MonthNames
                                          join b in RP_ShuttleReport.Where(x => x.GroupGUID.ToString().ToString() == r.guid) on a.MonthOrder equals b.date.Month into LJ1
                                          from R1 in LJ1.DefaultIfEmpty(new MonthData() { GroupGUID = "" })
                                          group new { R1.GroupGUID, a.MonthOrder, R1.MainGroupGUID } by new { R1.GroupGUID, a.MonthOrder } into grp
                                          orderby grp.Key.MonthOrder
                                          select new dataLiner
                                          {
                                              y = grp.Key.GroupGUID != "" ? grp.Count() : 0
                                          }).ToArray()
                                      );


            return Json(new { Result = Result, categories = MonthNames.Select(x => x.Name).ToArray() }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Shuttle Travel Purposes persantage per type
        /// </summary>
        /// <param name="rp"></param>
        /// <returns></returns>
        public ActionResult ShuttleVehicles(SHMReportParameters rp)
        {
            rp = FillRP(rp);
            var categories = new DropDownList().Vehicles().OrderBy(x => x.Text).Select(x => new { x.Text, x.Value }).ToArray();
            var RP_ShuttleReport = DbSHM.RP_ShuttleReport(
                                    rp.StartDate, rp.EndDate, LAN,
                                    string.Join(",", rp.DutyStationGUID),
                                    string.Join(",", rp.UserPassengerGUID),
                                    string.Join(",", rp.UserDriverGUID),
                                    string.Join(",", rp.VehicleGUID),
                                    string.Join(",", rp.StartLocationGUID),
                                    string.Join(",", rp.EndLocationGUID))
               .Select(x =>
             new
             {
                 x.VehicleGUID,
                 x.ShuttleGUID
             }
            ).Distinct().ToList();

            double TotalNumberOfShuttles = RP_ShuttleReport.Select(x => x.ShuttleGUID).Distinct().Count();
            var Result = (from a in categories
                          join b in RP_ShuttleReport
                           on a.Value equals b.VehicleGUID.ToString()
                          group new { a.Value } by new { a.Text } into grp

                          select
                          new seriesPie
                          {
                              name = grp.Key.Text,
                              y = (Convert.ToDouble(grp.Count()) / TotalNumberOfShuttles) * 100
                          }

                  ).ToArray();

            return Json(new { Result = Result.ToArray(), categories = Result.Select(x => x.name).ToArray() }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Shuttle End Location Per Months
        /// </summary>
        /// <param name="rp"></param>
        /// <returns></returns>
        public ActionResult ShuttleStaffPurposePerMonths(SHMReportParameters rp)
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

            rp = FillRP(rp);
            var categories = new DropDownList().ShuttleTravelPurpose().OrderBy(x => x.Text).Select(x => new { x.Text, x.Value }).ToArray();
            var RP_ShuttleReport = (from a in DbSHM.RP_ShuttleReport(
                         rp.StartDate, rp.EndDate, LAN,
                         string.Join(",", rp.DutyStationGUID),
                         string.Join(",", rp.UserPassengerGUID),
                         string.Join(",", rp.UserDriverGUID),
                         string.Join(",", rp.VehicleGUID),
                         string.Join(",", rp.StartLocationGUID),
                         string.Join(",", rp.EndLocationGUID))
                                    group new { a.EndLocationGUID } by new
                                    {
                                        date = a.DepartureDateTime,
                                        GroupGUID = a.ShuttleTravelPurposeGUID,
                                        UserGUID = a.UserGUID
                                    }).Select(x => new MonthData()
                                    {
                                        date = x.Key.date,
                                        GroupGUID = x.Key.GroupGUID.ToString(),
                                        MainGroupGUID = x.Key.UserGUID
                                    })
                       .ToList();


            var Result = (from a in RP_ShuttleReport
                          join c in categories on a.GroupGUID.ToString() equals c.Value
                          group new
                          {
                              c.Text
                          } by new { c.Text, c.Value } into G
                          select new drilldownLiner
                          {
                              name = G.Key.Text,
                              guid = G.Key.Value.ToString()
                          }

                                  ).ToList();

            Result.ForEach(r => r.data = (from a in MonthNames
                                          join b in RP_ShuttleReport.Where(x => x.GroupGUID.ToString().ToString() == r.guid) on a.MonthOrder equals b.date.Month into LJ1
                                          from R1 in LJ1.DefaultIfEmpty(new MonthData() { GroupGUID = "" })
                                          group new { R1.GroupGUID, a.MonthOrder, R1.MainGroupGUID } by new { a.Name, R1.GroupGUID, a.MonthOrder } into grp
                                          orderby grp.Key.MonthOrder
                                          select new dataLiner
                                          {
                                              y = grp.Key.GroupGUID != ""? grp.Count() : 0
                                          }).ToArray()
                                      );


            return Json(new { Result = Result, categories = MonthNames.Select(x => x.Name).ToArray() }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Shuttle Vehicle Per Months
        /// </summary>
        /// <param name="rp"></param>
        /// <returns></returns>
        public ActionResult ShuttleVehiclePerMonths(SHMReportParameters rp)
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

            rp = FillRP(rp);
            var categories = new DropDownList().Vehicles().OrderBy(x => x.Text).Select(x => new { x.Text, x.Value }).ToArray();
            var RP_ShuttleReport = (from a in DbSHM.RP_ShuttleReport(
                          rp.StartDate, rp.EndDate, LAN,
                          string.Join(",", rp.DutyStationGUID),
                          string.Join(",", rp.UserPassengerGUID),
                          string.Join(",", rp.UserDriverGUID),
                          string.Join(",", rp.VehicleGUID),
                          string.Join(",", rp.StartLocationGUID),
                          string.Join(",", rp.EndLocationGUID))
                                    group new { a.EndLocationGUID } by new
                                    {
                                        date = a.DepartureDateTime,
                                        GroupGUID = a.VehicleGUID,
                                        ShuttleGUID = a.ShuttleGUID
                                    }).Select(x => new MonthData()
                                    {
                                        date = x.Key.date,
                                        GroupGUID = x.Key.GroupGUID.ToString(),
                                        MainGroupGUID = x.Key.ShuttleGUID
                                    })
                        .ToList();

            var Result = (from a in RP_ShuttleReport
                          join c in categories on a.GroupGUID.ToString() equals c.Value
                          group new
                          {
                              c.Text
                          } by new { c.Text, c.Value } into G
                          select new drilldownLiner
                          {
                              name = G.Key.Text,
                              guid = G.Key.Value.ToString()
                          }

                                  ).ToList();

            Result.ForEach(r => r.data = (from a in MonthNames
                                          join b in RP_ShuttleReport.Where(x => x.GroupGUID.ToString().ToString() == r.guid) on a.MonthOrder equals b.date.Month into LJ1
                                          from R1 in LJ1.DefaultIfEmpty(new MonthData() { GroupGUID = "" })
                                          group new { R1.GroupGUID, a.MonthOrder, R1.MainGroupGUID } by new { a.Name, R1.GroupGUID, a.MonthOrder } into grp
                                          orderby grp.Key.MonthOrder
                                          select new dataLiner
                                          {
                                              y = grp.Key.GroupGUID != "" ? grp.Count() : 0
                                          }).ToArray()
                                      );


            return Json(new { Result = Result, categories = MonthNames.Select(x => x.Name).ToArray() }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Shuttle Drivers
        /// </summary>
        /// <param name="rp"></param>
        /// <returns></returns>
        public ActionResult ShuttleDrivers(SHMReportParameters rp)
        {
            rp = FillRP(rp);
            var categories = new DropDownList().ShuttleDrivers().OrderBy(x => x.Text).Select(x => new { x.Text, x.Value }).ToArray();
            var RP_ShuttleReport = DbSHM.RP_ShuttleReport(
                                    rp.StartDate, rp.EndDate, LAN,
                                    string.Join(",", rp.DutyStationGUID),
                                    string.Join(",", rp.UserPassengerGUID),
                                    string.Join(",", rp.UserDriverGUID),
                                    string.Join(",", rp.VehicleGUID),
                                    string.Join(",", rp.StartLocationGUID),
                                    string.Join(",", rp.EndLocationGUID))
               .Select(x =>
             new
             {
                 x.UserDriverGUID,
                 x.ShuttleGUID
             }
            ).Distinct().ToList();

            double TotalNumberOfDrivers = RP_ShuttleReport.Select(x => x.ShuttleGUID).Distinct().Count();
            var Result = (from a in categories
                          join b in RP_ShuttleReport
                           on a.Value equals b.UserDriverGUID.ToString()
                          group new { a.Value } by new { a.Text } into grp

                          select
                          new seriesPie
                          {
                              name = grp.Key.Text,
                              y = (Convert.ToDouble(grp.Count()) / TotalNumberOfDrivers) * 100
                          }

                  ).ToArray();

            return Json(new { Result = Result.ToArray(), categories = Result.Select(x => x.name).ToArray() }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Shuttle Drivers Per Months
        /// </summary>
        /// <param name="rp"></param>
        /// <returns></returns>
        public ActionResult ShuttleDriverPerMonths(SHMReportParameters rp)
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

            rp = FillRP(rp);
            var categories = new DropDownList().ShuttleDrivers().OrderBy(x => x.Text).Select(x => new { x.Text, x.Value }).ToArray();
            var RP_ShuttleReport = (from a in DbSHM.RP_ShuttleReport(
                                     rp.StartDate, rp.EndDate, LAN,
                                     string.Join(",", rp.DutyStationGUID),
                                     string.Join(",", rp.UserPassengerGUID),
                                     string.Join(",", rp.UserDriverGUID),
                                     string.Join(",", rp.VehicleGUID),
                                     string.Join(",", rp.StartLocationGUID),
                                     string.Join(",", rp.EndLocationGUID))
                                    group new { a.UserDriverGUID } by new
                                    {
                                        date = a.DepartureDateTime,
                                        GroupGUID = a.UserDriverGUID,
                                        ShuttleGUID = a.ShuttleGUID
                                    }).Select(x => new MonthData()
                                    {
                                        date = x.Key.date,
                                        GroupGUID = x.Key.GroupGUID.ToString(),
                                        MainGroupGUID = x.Key.ShuttleGUID
                                    })
                                   .ToList();

            var Result = (from a in RP_ShuttleReport
                          join c in categories on a.GroupGUID.ToString() equals c.Value
                          group new
                          {
                              c.Text
                          } by new { c.Text, c.Value } into G
                          select new drilldownLiner
                          {
                              name = G.Key.Text,
                              guid = G.Key.Value.ToString()
                          }).ToList();

            Result.ForEach(r => r.data = (from a in MonthNames
                                          join b in RP_ShuttleReport.Where(x => x.GroupGUID.ToString().ToString() == r.guid) on a.MonthOrder equals b.date.Month into LJ1
                                          from R1 in LJ1.DefaultIfEmpty(new MonthData() { GroupGUID = "" })
                                          group new { R1.GroupGUID, a.MonthOrder, R1.MainGroupGUID } by new { R1.GroupGUID, a.MonthOrder } into grp
                                          orderby grp.Key.MonthOrder
                                          select new dataLiner
                                          {
                                              y = grp.Key.GroupGUID != "" ? grp.Count() : 0
                                          }).ToArray()
                                      );


            return Json(new { Result = Result, categories = MonthNames.Select(x => x.Name).ToArray() }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Shuttle End Location Per Months
        /// </summary>
        /// <param name="rp"></param>
        /// <returns></returns>
        public ActionResult ShuttleRoutePerMonths(SHMReportParameters rp)
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

            rp = FillRP(rp);
            Guid LocationType = Guid.Parse("C970DDF5-31E5-47A1-BE76-AF15833D4E6A");
            var RP_ShuttleReport = (from a in DbSHM.RP_ShuttleReport(
                          rp.StartDate, rp.EndDate, LAN,
                          string.Join(",", rp.DutyStationGUID),
                          string.Join(",", rp.UserPassengerGUID),
                          string.Join(",", rp.UserDriverGUID),
                          string.Join(",", rp.VehicleGUID),
                          string.Join(",", rp.StartLocationGUID),
                          string.Join(",", rp.EndLocationGUID))
                                    group new { a.EndLocationGUID } by new
                                    {
                                        date = a.DepartureDateTime,
                                        GroupGUID = a.StartLocationGUID+""+a.EndLocationGUID,
                                        ShuttleGUID = a.ShuttleGUID,
                                        Dercription=a.StartLocation+"-"+a.EndLocation,
                                    }).Select(x => new MonthData()
                                    {
                                        date = x.Key.date,
                                        GroupGUID = x.Key.GroupGUID.ToString(),
                                        MainGroupGUID = x.Key.ShuttleGUID,
                                        Dercription=x.Key.Dercription
                                    })
                        .ToList();
            var categories = RP_ShuttleReport.Select(x=>
            new
            {
                Text=x.Dercription,
                Value=x.GroupGUID
            }).Distinct().ToList();
            var Result = (from a in RP_ShuttleReport
                          join c in categories on a.GroupGUID.ToString() equals c.Value
                          group new
                          {
                              c.Text
                          } by new { c.Text, c.Value } into G
                          select new drilldownLiner
                          {
                              name = G.Key.Text,
                              guid = G.Key.Value.ToString()
                          }

                                  ).ToList();

            Result.ForEach(r => r.data = (from a in MonthNames
                                          join b in RP_ShuttleReport.Where(x => x.GroupGUID.ToString().ToString() == r.guid) on a.MonthOrder equals b.date.Month into LJ1
                                          from R1 in LJ1.DefaultIfEmpty(new MonthData() { GroupGUID = "" })
                                          group new { R1.GroupGUID, a.MonthOrder, R1.MainGroupGUID } by new { R1.GroupGUID, a.MonthOrder } into grp
                                          orderby grp.Key.MonthOrder
                                          select new dataLiner
                                          {
                                              y = grp.Key.GroupGUID != "" ? grp.Count() : 0
                                          }).ToArray()
                                      );


            return Json(new { Result = Result, categories = MonthNames.Select(x => x.Name).ToArray() }, JsonRequestBehavior.AllowGet);
        }
    }



    internal class MonthData
    {
        public Guid MainGroupGUID { get; set; }
        public string GroupGUID { get; set; }

        public DateTime date { get; set; }

        public string Dercription { get; set; }
    }
}