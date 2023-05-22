using AppsPortal.BaseControllers;
using AppsPortal.Library;
using AppsPortal.ViewModels;
using MRS_DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.MRS.Controllers
{
    public class HomeController : MRSBaseController
    {
        class drilldown
        {
            public string name { set; get; }
            public Guid ReferralStatusGUID { set; get; }
            public data2[] data { get; set; }
        }
        class data2
        {
            public double y { set; get; }
        }

        class data1
        {
            public string name { set; get; }
            public string drilldown { set; get; }
            public double y { set; get; }
        }
        class Months
        {
            public string Name { set; get; }
            public int MonthOrder { set; get; }

        }

        class Rainfall
        {
            public string name { set; get; }
            public int[] data { set; get; }
        }

        // GET: MRS/Home
        public ActionResult Index()
        {
            CMS.SetUserToken(UserProfileGUID, Apps.MRS);
            Session[SessionKeys.CurrentApp] = Apps.MRS;
            CMS.BuildUserMenus(UserGUID, LAN);
            return View();
        }
        [Route("MRS/Home/GetMapNVLocations")]
        public JsonResult GetLocations(MRSReportParametersList rp)
        {
            var All = (from a in DbMRS.codeLocations.AsEnumerable()
                       join b in DbMRS.codeLocationsLanguages.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.codeLocations.DeletedOn) && x.LanguageID == LAN) on a.LocationGUID equals b.LocationGUID into LJ1
                       from R1 in LJ1.DefaultIfEmpty()
                       join c in DbMRS.dataNoteVerbale.Where(x => x.VisitDate >= rp.StartDate && x.VisitDate <= rp.EndDate && x.Active) on a.LocationGUID equals c.LocationGUID
                       select new LocationDataTableModel
                       {
                           Latitude = a.Latitude,
                           Longitude = a.Longitude,
                           LocationGUID = a.LocationGUID,
                           LocationDescription = R1.LocationDescription,
                           LocationTypeGUID = DbCMS.codeTablesValuesLanguages.Where(o => o.ValueGUID == a.LocationTypeGUID).Where(ol => ol.LanguageID == LAN).FirstOrDefault().ValueDescription,
                           CountryGUID = DbCMS.codeCountriesLanguages.Where(cl => cl.CountryGUID == a.CountryGUID && cl.LanguageID == LAN).FirstOrDefault().CountryDescription,
                           LocationlevelID = a.LocationlevelID,
                           LocationParentGUID = DbCMS.codeLocationsLanguages.Where(ll => ll.LocationGUID == a.LocationParentGUID && ll.LanguageID == LAN).FirstOrDefault().LocationDescription,
                           codeLocationsRowVersion = a.codeLocationsRowVersion,
                           Active = a.Active
                       }).ToList();
            return Json(All, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Percentage of MFA response
        /// </summary>
        /// <param name="rp"></param>
        /// <returns></returns>
        public ActionResult chart1(MRSReportParametersList rp)
        {
            double Total =  DbMRS.dataNoteVerbale.Where(x => x.VisitDate >= rp.StartDate && x.VisitDate <= rp.EndDate && x.Active).Count();
            var Result = (from a in DbMRS.dataNoteVerbale.Where(x => x.VisitDate >= rp.StartDate && x.VisitDate <= rp.EndDate && x.Active)
                          join b in DbMRS.codeReferralStatusLanguage.Where(x => x.LanguageID == LAN && x.codeReferralStatus.Active && x.codeReferralStatus.ApplicationGUID == Apps.MRS)
                          on a.ReferralStatusGUID equals b.ReferralStatusGUID
                          group b.Description by b.Description
                         ).Select(x =>
                         new data1
                         {
                             name = x.Key,
                             drilldown = x.Key,
                             y = (x.Count()/ Total )*100
                         }
                           ).ToArray();
               
              
            return Json(new { Result = Result }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 2-	Percentage of MFA approval/ disapproval per month
        /// </summary>
        /// <param name="rp"></param>
        /// <returns></returns>
        public ActionResult chart2(MRSReportParametersList rp)
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

            var Result = (from a in DbMRS.dataNoteVerbale.Where(x => x.Active)
                                    join b in DbMRS.codeReferralStatusLanguage.Where(x => x.LanguageID == LAN && x.codeReferralStatus.Active && x.codeReferralStatus.ApplicationGUID == Apps.MRS)
                           on a.ReferralStatusGUID equals b.ReferralStatusGUID
                                    group new
                                    {
                                        b.Description
                                    } by new { b.Description, b.ReferralStatusGUID } into G
                                    select new drilldown
                                    {
                                        name = G.Key.Description,
                                        ReferralStatusGUID = G.Key.ReferralStatusGUID.Value
                                    }

                                    ).ToList();

            Result.ForEach(r =>
                                      r.data = (from a in MonthNames
                                                join b in DbMRS.dataNoteVerbale.Where(x => x.Active && x.ReferralStatusGUID == r.ReferralStatusGUID) on a.MonthOrder equals b.VisitDate.Month into LJ1
                                                from R1 in LJ1.DefaultIfEmpty(new dataNoteVerbale() { ReferralStatusGUID = default(Guid) })
                                                group new { R1.ReferralStatusGUID, a.Name, } by new { a.Name, R1.ReferralStatusGUID, a.MonthOrder } into grp
                                                orderby grp.Key.MonthOrder
                                                select new data2
                                                {
                                                    y = grp.Key.ReferralStatusGUID != Guid.Empty ? grp.Count() : 0
                                                }).ToArray()
                                      );

            return Json(new { Model = Result });
        }

        /// <summary>
        /// Organizations
        /// </summary>
        /// <param name="rp"></param>
        /// <returns></returns>
        public ActionResult chart3(MRSReportParametersList rp)
        {
            var categories = (from a in DbMRS.dataNoteVerbale.Where(x => x.VisitDate >= rp.StartDate && x.VisitDate <= rp.EndDate && x.Active && ( x.ReferralStatusGUID == Status.Approved || x.ReferralStatusGUID == Status.Rejected) && x.Active)
                              join b in DbMRS.codeLocationsLanguages.Where(x => x.LanguageID == LAN && x.codeLocations.Active)
                              on a.LocationGUID equals b.LocationGUID
                              select b).OrderBy(x => x.LocationDescription).Select(x => new { x.LocationDescription, x.LocationGUID }).ToArray();
            var Approved =
                new Rainfall
                {
                    name = "Approved",
                    data = (from a in categories
                            join b in DbMRS.dataNoteVerbale.Where(x => x.VisitDate >= rp.StartDate && x.VisitDate <= rp.EndDate && x.ReferralStatusGUID== Status.Approved &&x.Active) on a.LocationGUID equals b.LocationGUID into LJ1
                            from R1 in LJ1.DefaultIfEmpty(new dataNoteVerbale() { LocationGUID = default(Guid) })
                            group new { R1.LocationGUID } by new { R1.LocationGUID, a.LocationDescription } into grp
                            orderby grp.Key.LocationDescription
                            select grp.Key.LocationGUID != Guid.Empty ? grp.Count() : 0

                    ).ToArray()
                };

            var Rejected =
               new Rainfall
               {
                   name = "Rejected",
                   data = (from a in categories
                           join b in DbMRS.dataNoteVerbale.Where(x => x.VisitDate >= rp.StartDate && x.VisitDate <= rp.EndDate && x.Active && x.ReferralStatusGUID == Status.Rejected && x.Active) on a.LocationGUID equals b.LocationGUID into LJ1
                           from R1 in LJ1.DefaultIfEmpty(new dataNoteVerbale() { LocationGUID =Guid.Empty })
                           group new { R1.LocationGUID } by new { R1.LocationGUID, a.LocationDescription } into grp
                           orderby grp.Key.LocationDescription
                           select grp.Key.LocationGUID != Guid.Empty ? grp.Count() : 0

                   ).ToArray()
               };


            return Json(new { Approved = Approved, Rejected= Rejected, categories = categories.Select(x => x.LocationDescription).ToArray() }, JsonRequestBehavior.AllowGet);

        }

        public class Status
        {
            public static Guid Approved = new Guid("342498cd-c846-4495-8c6a-6991c47ab7a3");
            public static Guid Rejected = new Guid("bdbd4539-619e-4adc-8907-8c0d8bd01fc4");
            public static Guid NoResponse = new Guid("64a87f65-ddc3-47cd-9843-f8a57b4801e0");
            public static Guid Pending = new Guid("e45f8cc0-58a2-4faf-9969-749dd9cebc72");
        }
       
    }
}