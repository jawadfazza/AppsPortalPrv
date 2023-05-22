using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Security.Policy;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.UI;
using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.ViewModels;
using AutoMapper;
using IMS_DAL.Model;
using IMS_DAL.ViewModels;
using LinqKit;
using OfficeOpenXml;
using Org.BouncyCastle.Asn1.Ocsp;

namespace AppsPortal.Areas.IMS.Controllers
{
    public class HomeController : IMSBaseController
    {
        // GET: IMS/Home
        public ActionResult Index()
        {
            if (!CMS.HasAction(Permissions.MissionReportForm.Create, Apps.IMS))
            {
                return Json(DbAHD.PermissionError());
            }
            //var userProfiles = (from a in DbCMS.userProfiles
            //                    where a.Active
            //                    select a.UserProfileGUID).ToList();
            //foreach (var item in userProfiles)
            //{
            //    CMS.UpdateUserPermissionToken(item, Apps.IMS);
            //}

            CMS.SetUserToken(UserProfileGUID, Apps.IMS);
            Session[SessionKeys.CurrentApp] = Apps.IMS;
            CMS.BuildUserMenus(UserGUID, LAN);
            DbCMS.SaveChanges();

            filterDashboardVM model = new filterDashboardVM();
            Guid cancelMission = Guid.Parse("0c44822f-a898-476d-b291-caf1b013aac4");
            Guid ongoing = Guid.Parse("0C44822F-A898-476D-B291-CAF1B013AAC2");
            int today = DateTime.Now.Year;
            model.ActionsTaken= DbIMS.dataMissionActionRequired.Where(x => x.Active).Select(x => x.MissionReportFormGUID).Count();
            model.ActionsOngoing=DbIMS.dataMissionActionTaken.Where(x => x.ActionStatusGUID == MissionActionTakenStatus.Ongoing && x.Active).Select(x => x.MissionActionTakenGUID).Count();
            model.ActionsPlanning = DbIMS.dataMissionActionTaken.Where(x => x.ActionStatusGUID == MissionActionTakenStatus.Planning && x.Active).Select(x => x.MissionActionTakenGUID).Count();
            model.missionsCurrentYear = DbIMS.dataMissionReportForm.Where(x => x.MissionStartDate.Year== today && x.MissionStatusGUID!= cancelMission).Select(x => x.MissionReportFormGUID).Count();
            model.missionsOngoing = DbIMS.dataMissionReportForm.Where(x =>  x.MissionStatusGUID == ongoing).Select(x => x.MissionReportFormGUID).Count();
            return View(model);
        }

        [Route("IMS/MissionCalender/")]
        public ActionResult MissionCalender()
        {

            return View("~/Areas/IMS/Views/Home/MissionsCalender.cshtml");
        }


        [Route("IMS/MissionReports/")]
        public ActionResult MissionReports()
        {

            return View("~/Areas/IMS/Views/Reports/Index.cshtml", new ModelTrackReportModel());
            
        }

        [HttpPost]
        public ActionResult DistributionbyGovernoratesChart(filterDashboardVM rp)
        {
            var missionLocations = (from a in DbIMS.dataMissionReportForm.Where(x=> x.MissionStartDate >= rp.StartDate && x.MissionStartDate <= rp.EndDate)
                join b in DbIMS.codeLocationsLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.GovernorateGUID equals b.LocationGUID
                group new
                {
                    a.MissionReportFormGUID,
                    b.LocationDescription
                } by b.LocationDescription into G
                select new HighChartPieModel
                {
                    name = G.Key,
                    y = G.Count(),
                    selected = true,
                    sliced = true
                }
            ).ToList();



            return Json(new { missionLocations = missionLocations });
        }

        [HttpPost]
        public ActionResult InitMapResult(filterDashboardVM rp)
        {
            var result = DbIMS.dataMissionReportForm.Where(x=>x.CommunityGUID!=null && x.MissionStartDate >= rp.StartDate && x.MissionStartDate <= rp.EndDate).Select(x=>new {x.MissionCode,x.MissionStartDate,x.CommunityGUID,x.longitude,x.Latitude, DISTRICT = x.codeLocations2.codeLocationsLanguages.Where(f=>f.LanguageID==LAN).Select(f=>f.LocationDescription).FirstOrDefault(),x.MissionReportFormGUID}).ToList();
            List<MapVM> data = new List<MapVM>();
            foreach (var item in result)
            {
                MapVM myMap = new MapVM
                {
                    GeoLong = (decimal)item.longitude,
                    GeoLat = (decimal)item.Latitude,
                    DISTRICT = item.DISTRICT,
                    Id = item.MissionReportFormGUID,
                    MissionDetail="Mission Number: "+item.MissionCode+" Start Date:  "+item.MissionStartDate+" Community: "+item.DISTRICT

                };
                data.Add(myMap);
            }


            return Json(data);
        }

        public ActionResult GetMissionsByVisitObjective()
        {
            var result = (from a in DbIMS.dataMissionReportForm.Where(x => x.Active)
                join b in DbIMS.dataMissionReportFormVisitObjective.Where(x => x.Active) on a.MissionReportFormGUID equals b.MissionReportFormGUID
                          join c in DbIMS.codeIMSVisitObjectiveLanguage.Where(x => x.Active && x.LanguageID==LAN) on b.VisitObjectiveGUID equals c.VisitObjectiveGUID
                          group new
                {
                    b.VisitObjectiveGUID,
                    c.VisitObjectiveDescription
                } by c.VisitObjectiveDescription into G
                select new HighChartPieModel
                {
                    name = G.Key,
                    y = G.Count(),
                    selected = true,
                    sliced = true
                }
            ).Take(10).OrderByDescending(x=>x.y).ToList();
            return Json(new { Model = result });
        }


        public ActionResult GeTMissionsPerMonth()
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
            var model = (from a in DbIMS.dataMissionReportForm.Where(x => x.Active)

                                    join b in DbIMS.codeLocationsLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.GovernorateGUID equals b.LocationGUID
                                    group new
                                    {

                                        b.LocationDescription
                                    } by new { b.LocationDescription, b.LocationGUID } into G
                                    select new drilldown
                                    {
                                        name = G.Key.LocationDescription,
                                        Guid = G.Key.LocationGUID
                                    }

                                    ).ToList();

            model.ForEach(r =>
                                      r.data = (from a in MonthNames
                                                join b in DbIMS.dataMissionReportForm.Where(x => x.Active && x.GovernorateGUID == r.Guid) on a.MonthOrder equals b.MissionStartDate.Month into LJ1
                                                from R1 in LJ1.DefaultIfEmpty(new dataMissionReportForm() { MissionReportFormGUID = default(Guid) })
                                                group new { a.MonthOrder } by new { R1.MissionReportFormGUID, a.MonthOrder } into grp
                                                orderby grp.Key.MonthOrder
                                                select new data
                                                {
                                                    y = grp.Key.MissionReportFormGUID != Guid.Empty ? grp.Count() : 0
                                                }).ToArray()
                                      );

            return Json(new { Model = model });
        }

        public ActionResult DistributionPerLocationAndMonth(filterDashboardVM rp)
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

            var Result = (from a in DbIMS.dataMissionReportForm.Where(x => x.Active && x.MissionStartDate >= rp.StartDate && x.MissionStartDate <= rp.EndDate)
                          join b in DbIMS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN && x.codeTablesValues.TableGUID == LookupTables.IMSMissionStatus) on a.MissionStatusGUID equals b.ValueGUID

                          group new
                {

                    b.ValueDescription
                } by new { b.ValueDescription, b.ValueGUID } into G
                select new drilldown
                {
                    name = G.Key.ValueDescription,
                    Guid = G.Key.ValueGUID
                }

            ).ToList();

            Result.ForEach(r =>
                    r.data = (from a in MonthNames
                        join b in DbIMS.dataMissionReportForm.Where(x => x.Active && x.MissionStatusGUID == r.Guid &&  x.MissionStartDate >= rp.StartDate && x.MissionStartDate <= rp.EndDate) on a.MonthOrder equals b.MissionStartDate.Month into LJ1
                        from R1 in LJ1.DefaultIfEmpty(new dataMissionReportForm() { MissionReportFormGUID = default(Guid) ,MissionStatusGUID = default(Guid)})
                        group new { a.Name,R1.MissionStatusGUID } by new { a.Name, R1.MissionStatusGUID, a.MonthOrder } into grp
                        orderby grp.Key.MonthOrder
                        select new data
                        {
                            y = grp.Key.MissionStatusGUID != Guid.Empty ? grp.Count() : 0
                        }).ToArray()
                );
                                   

            return Json(new { Model = Result });
        }
        public ActionResult GetMissionsByMonth()
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
            var result = (from a in MonthNames
                join b in DbIMS.dataMissionReportForm.Where(x => x.Active) on a.MonthOrder equals b.MissionStartDate
                    .Month into LJ1
                from R1 in LJ1.DefaultIfEmpty(new dataMissionReportForm() {MissionReportFormGUID = default(Guid)})
                group new {a.MonthOrder} by new {R1.MissionReportFormGUID, a.MonthOrder}
                into grp
                orderby grp.Key.MonthOrder
                select new data
                {
                    y = grp.Key.MissionReportFormGUID != Guid.Empty ? grp.Count() : 0
                }).ToArray();

           

            return Json(new { data = result });


        }

        public JsonResult InitCalander()
        {
            List<CalanderVM> result = new List<CalanderVM>();
            var missionForms=DbIMS.dataMissionReportForm.ToList();
            var missionDates = missionForms.ToList().Select(x => x.MissionStartDate.ToShortDateString()).Distinct();
            Guid syriaCountryGUID = Guid.Parse("710BFD1B-50CC-4F1E-92A9-A70583CFA5E0");
           var dutyStations= DbIMS.codeMissionOfficeSourceLanguage.Where(x =>
                x.LanguageID == LAN && x.Active ).ToList();
            string format = "yyyy-MM-dd";
            foreach (var item in missionDates)
            {
                foreach (var myDuty in dutyStations)
                {
                    if (myDuty.MissionOfficeSourceDescription == "Damascus")
                    {
                        if (missionForms.Where(x => x.MissionStartDate.ToShortDateString() == item
                                                    && x.MissionOfficeSourceGUID == myDuty.MissionOfficeSourceGUID)
                                .Select(x => x.MissionReportFormGUID).Count() > 0)
                        {
                            DateTime time = Convert.ToDateTime(item);             // Use current time.
                            // Use this format.
                            var myDate = time.ToString(format);


                            CalanderVM myResult = new CalanderVM
                            {
                                id= myDuty.MissionOfficeSourceGUID.ToString()+"x"+item,
                                title = "Damascus: " + " " + missionForms.Where(x => x.MissionStartDate.ToShortDateString() == item
                                                                                     && x.MissionOfficeSourceGUID == myDuty.MissionOfficeSourceGUID)
                                            .Select(x => x.MissionReportFormGUID).Count(),
                                start = myDate.ToString(),
                                end = null,
                                d = time.Day,
                                m = time.Month,
                                y = time.Year,
                                backgroundColor = "#f44271",
                                borderColor = "#00c0ef"
                            };
                            result.Add(myResult);

                        }
                    
                    }

                    if (myDuty.MissionOfficeSourceDescription == "Aleppo")
                    {
                        if (missionForms.Where(x => x.MissionStartDate.ToShortDateString() == item
                                                    && x.MissionOfficeSourceGUID == myDuty.MissionOfficeSourceGUID)
                                .Select(x => x.MissionReportFormGUID).Count() > 0)
                        {
                            DateTime time = Convert.ToDateTime(item);            // Use current time.
                            // Use this format.
                            var myDate = time.ToString(format);


                            CalanderVM myResult = new CalanderVM
                            {
                                id = myDuty.MissionOfficeSourceGUID.ToString() + "x" + item,
                                title = "Aleppo: " + " " + missionForms.Where(x => x.MissionStartDate.ToShortDateString() == item
                                                                                   && x.MissionOfficeSourceGUID == myDuty.MissionOfficeSourceGUID)
                                            .Select(x => x.MissionReportFormGUID).Count(),
                                start = myDate.ToString(),
                                end = null,
                                d = time.Day,
                                m = time.Month,
                                y = time.Year,
                                backgroundColor = "#DEBDC3",
                                borderColor = "#00c0ef"
                            };
                            result.Add(myResult);
                        }

                    
                    }

                    if (myDuty.MissionOfficeSourceDescription == "Homs")
                    {
                        if (missionForms.Where(x => x.MissionStartDate.ToShortDateString() == item
                                                    && x.MissionOfficeSourceGUID == myDuty.MissionOfficeSourceGUID)
                                .Select(x => x.MissionReportFormGUID).Count() > 0)
                        {
                            DateTime time = Convert.ToDateTime(item);              // Use current time.
                            // Use this format.
                            var myDate = time.ToString(format);


                            CalanderVM myResult = new CalanderVM
                            {
                                id = myDuty.MissionOfficeSourceGUID.ToString() + "x" + item,
                                title = "Homs: " + " " + missionForms.Where(x => x.MissionStartDate.ToShortDateString() == item
                                                                                 && x.MissionOfficeSourceGUID == myDuty.MissionOfficeSourceGUID)
                                            .Select(x => x.MissionReportFormGUID).Count(),
                                start = myDate.ToString(),
                                end = null,
                                d = time.Day,
                                m = time.Month,
                                y = time.Year,
                                backgroundColor = "#3c8dbc",
                                borderColor = "#00c0ef"
                            };
                            result.Add(myResult);
                        }

                       
                    }

                    if (myDuty.MissionOfficeSourceDescription == "Tartous")
                    {
                        if (missionForms.Where(x => x.MissionStartDate.ToShortDateString() == item
                                                    && x.MissionOfficeSourceGUID == myDuty.MissionOfficeSourceGUID)
                                .Select(x => x.MissionReportFormGUID).Count() > 0)
                        {
                            DateTime time = Convert.ToDateTime(item);              // Use current time.
                            // Use this format.
                            var myDate = time.ToString(format);


                            CalanderVM myResult = new CalanderVM
                            {
                                id = myDuty.MissionOfficeSourceGUID.ToString() + "x" + item,
                                title = "Tartus: " + " " + missionForms.Where(x => x.MissionStartDate.ToShortDateString() == item
                                                                                   && x.MissionOfficeSourceGUID == myDuty.MissionOfficeSourceGUID)
                                            .Select(x => x.MissionReportFormGUID).Count(),
                                start = myDate.ToString(),
                                end = null,
                                d = time.Day,
                                m = time.Month,
                                y = time.Year,
                                backgroundColor = "#3c8dbc",
                                borderColor = "#00c0ef"
                            };
                            result.Add(myResult);
                        }

                        
                    }

                    if (myDuty.MissionOfficeSourceDescription == "Rural Damascus")
                    {
                        if (missionForms.Where(x => x.MissionStartDate.ToShortDateString() == item
                                                    && x.MissionOfficeSourceGUID == myDuty.MissionOfficeSourceGUID)
                                .Select(x => x.MissionReportFormGUID).Count() > 0)
                        {
                            DateTime time = Convert.ToDateTime(item);              // Use current time.
                            // Use this format.
                            var myDate = time.ToString(format);


                            CalanderVM myResult = new CalanderVM
                            {
                                id = myDuty.MissionOfficeSourceGUID.ToString() + "x" + item,
                                title = "Rural Damascus: " + " " + missionForms.Where(x => x.MissionStartDate.ToShortDateString() == item
                                                                                           && x.MissionOfficeSourceGUID == myDuty.MissionOfficeSourceGUID)
                                            .Select(x => x.MissionReportFormGUID).Count(),
                                start = myDate.ToString(),
                                end = null,
                                d = time.Day,
                                m = time.Month,
                                y = time.Year,
                                backgroundColor = "#9daccb",
                                borderColor = "#00c0ef"
                            };
                            result.Add(myResult);
                        }

                       
                    }

                    if (myDuty.MissionOfficeSourceDescription == "Qamishli")
                    {
                        if (missionForms.Where(x => x.MissionStartDate.ToShortDateString() == item
                                                    && x.MissionOfficeSourceGUID == myDuty.MissionOfficeSourceGUID)
                                .Select(x => x.MissionReportFormGUID).Count() > 0)
                        {

                            DateTime time = Convert.ToDateTime(item);              // Use current time.
                            // Use this format.
                            var myDate = time.ToString(format);


                            CalanderVM myResult = new CalanderVM
                            {
                                id = myDuty.MissionOfficeSourceGUID.ToString() + "x" + item,
                                title = "Qamishli: " + " " + missionForms.Where(x => x.MissionStartDate.ToShortDateString() == item
                                                                                     && x.MissionOfficeSourceGUID == myDuty.MissionOfficeSourceGUID)
                                            .Select(x => x.MissionReportFormGUID).Count(),
                                start = myDate.ToString(),
                                end = null,
                                d = time.Day,
                                m = time.Month,
                                y = time.Year,
                                backgroundColor = "#f44141",
                                borderColor = "#00c0ef"
                            };
                            result.Add(myResult);
                        }

                    }

                    if (myDuty.MissionOfficeSourceDescription == "Sweida")
                    {
                        if (missionForms.Where(x => x.MissionStartDate.ToShortDateString() == item
                                                    && x.MissionOfficeSourceGUID == myDuty.MissionOfficeSourceGUID)
                                .Select(x => x.MissionReportFormGUID).Count() > 0)
                        {
                            DateTime time = Convert.ToDateTime(item);              // Use current time.
                            // Use this format.
                            var myDate = time.ToString(format);


                            CalanderVM myResult = new CalanderVM
                            {
                                id = myDuty.MissionOfficeSourceGUID.ToString() + "x" + item,
                                title = "Sweida: " + " " + missionForms.Where(x => x.MissionStartDate.ToShortDateString() == item
                                                                                   && x.MissionOfficeSourceGUID == myDuty.MissionOfficeSourceGUID)
                                            .Select(x => x.MissionReportFormGUID).Count(),
                                start = myDate.ToString(),
                                end = null,
                                d = time.Day,
                                m = time.Month,
                                y = time.Year,
                                backgroundColor = "#3d6ad6",
                                borderColor = "#00c0ef"
                            };
                            result.Add(myResult);
                        }

                     
                    }

                    if (myDuty.MissionOfficeSourceDescription == "Daraa")
                    {
                        if (missionForms.Where(x => x.MissionStartDate.ToShortDateString() == item
                                                    && x.MissionOfficeSourceGUID == myDuty.MissionOfficeSourceGUID)
                                .Select(x => x.MissionReportFormGUID).Count() > 0)
                        {
                            DateTime time = Convert.ToDateTime(item);              // Use current time.
                            // Use this format.
                            var myDate = time.ToString(format);


                            CalanderVM myResult = new CalanderVM
                            {
                                id = myDuty.MissionOfficeSourceGUID.ToString() + "x" + item,
                                title = "Daraa: " + " " + missionForms.Where(x => x.MissionStartDate.ToShortDateString() == item
                                                                                  && x.MissionOfficeSourceGUID == myDuty.MissionOfficeSourceGUID)
                                            .Select(x => x.MissionReportFormGUID).Count(),
                                start = myDate.ToString(),
                                end = null,
                                d = time.Day,
                                m = time.Month,
                                y = time.Year,
                                backgroundColor = "#13bda6",
                                borderColor = "#00c0ef"
                            };
                            result.Add(myResult);
                        }

                       
                    }

                    if (myDuty.MissionOfficeSourceDescription == "Lattakia")
                    {
                        if (missionForms.Where(x => x.MissionStartDate.ToShortDateString() == item
                                                    && x.MissionOfficeSourceGUID == myDuty.MissionOfficeSourceGUID)
                                .Select(x => x.MissionReportFormGUID).Count() > 0)
                        {
                            DateTime time = Convert.ToDateTime(item);              // Use current time.
                            // Use this format.
                            var myDate = time.ToString(format);


                            CalanderVM myResult = new CalanderVM
                            {
                                id = myDuty.MissionOfficeSourceGUID.ToString() + "x" + item,
                                title = "Lattakia: " + " " + missionForms.Where(x => x.MissionStartDate.ToShortDateString() == item
                                                                                     && x.MissionOfficeSourceGUID == myDuty.MissionOfficeSourceGUID)
                                            .Select(x => x.MissionReportFormGUID).Count(),
                                start = myDate.ToString(),
                                end = null,
                                d = time.Day,
                                m = time.Month,
                                y = time.Year,
                                backgroundColor = "#999966",
                                borderColor = "#00c0ef"
                            };
                            result.Add(myResult);
                        }

                      
                    }

                } 
            }


            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public class filterDashboardVM
        {
      
            public System.DateTime? StartDate { get; set; }
           
            public System.DateTime? EndDate { get; set; }

            public int? ActionsOngoing { get; set; }

            public int? ActionsTaken { get; set; }
            public int? ActionsPlanning { get; set; }
            public int? missionsCurrentYear { get; set; }
            public int? missionsOngoing { get; set; }
            
        }

        public ActionResult GetMissionDetails(string PK)
        {

            //StringBuilder ReasonHistory = new StringBuilder();
            //var result = DbIMS.dataMissionReportForm.Where(x => x.DutyStationGUID.ToString() == PK).ToList();
            //foreach (item x in result)
            //{
            //    ReasonHistory.Append("-" + x.LastApointmentDate);
            //    ReasonHistory.AppendLine();
            //    ReasonHistory.Append("-" + x.Reason);
            //    ReasonHistory.AppendLine();
            //    ReasonHistory.AppendLine();

            //}
           
            Guid dutyGuid =Guid.Parse(PK.Substring(0, PK.IndexOf("x")))  ;
            string date =PK.Substring(PK.IndexOf("x")+1 ).ToString();
            DateTime getDate = DateTime.ParseExact(date.ToString(), "MM/dd/yyyy", null);
            DateTime date1 = Convert.ToDateTime(date);
            var model = (

              from a in DbIMS.dataMissionReportForm.ToList().Where(x => x.MissionOfficeSourceGUID == dutyGuid
                                                                        && x.MissionStartDate.ToShortDateString() == date1.ToShortDateString())
              join b in DbIMS.codeLocationsLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.DistrictGUID equals b.LocationGUID into LJ1
              from R1 in LJ1.DefaultIfEmpty()
              join c in DbIMS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN && x.codeTablesValues.TableGUID == LookupTables.IMSMissionStatus) on a.MissionStatusGUID equals c.ValueGUID into LJ2
              from R2 in LJ2.DefaultIfEmpty()

              join d in DbIMS.codeLocationsLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.SubDistrictGUID equals d.LocationGUID into LJ3
              from R3 in LJ3.DefaultIfEmpty()

              join d in DbIMS.codeLocationsLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.CommunityGUID equals d.LocationGUID into LJ4
              from R4 in LJ4.DefaultIfEmpty()

              join e in DbIMS.codeLocationsLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.GovernorateGUID equals e.LocationGUID into LJ5
              from R5 in LJ5.DefaultIfEmpty()

              join f in DbIMS.codeMissionOfficeSourceLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.MissionOfficeSourceGUID equals f.MissionOfficeSourceGUID into LJ6
              from R6 in LJ6.DefaultIfEmpty()


              join g in DbIMS.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.MissionLeaderGUID equals g.UserGUID into LJ7
              from R7 in LJ7.DefaultIfEmpty()



              select new MissionReportFormDataTableModel
              {
                  MissionReportFormGUID = a.MissionReportFormGUID,

                  GovernorateGUID = a.GovernorateGUID.ToString(),
                  DistrictGUID = a.DistrictGUID.ToString(),
                  SubDistrictGUID = a.SubDistrictGUID.ToString(),
                  MissionStatusGUID = a.MissionStatusGUID.ToString(),
                  Address = a.Address,
                  CommunityGUID = a.CommunityGUID.ToString(),
                  DutyStation = R6.MissionOfficeSourceDescription,
                  Governorate = R5.LocationDescription,
                  District = R1.LocationDescription,
                  SubDistrict = R3.LocationDescription,
                  MissionLeaderGUID = a.MissionLeaderGUID.ToString(),
                  MissionLeader = R7.FirstName + " " + R7.Surname,



                  Community = R4.LocationDescription,
                  MissionStartDate = a.MissionStartDate,
                  MissionEndDate = a.MissionEndDate,
                  MissionCode = a.MissionCode,
                  MissionNumber = a.MissionNumber,
                  VisitObjectiveName = a.dataMissionReportFormVisitObjective.Select(x => x.VisitObjectiveName).FirstOrDefault(),
                  CreatedBy = a.userAccounts1.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).Select(x => x.FirstName).FirstOrDefault() + " " + a.userAccounts1.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).Select(x => x.Surname).FirstOrDefault(),
                  CreatedByGUID = a.CreatedByGUID.ToString(),
                  Active = a.Active,
                  MissionStatus = R2.ValueDescription,
                  dataMissionReportFormRowVersion = a.dataMissionReportFormRowVersion
              }).ToList();
            //DateTime missiondate = DateTime.ParseExact(getDate.ToString(), "yyyy-MM-dd HH:mm:ss", null);
            //DateTime missiondate = DateTime.ParseExact(date, "yyyy-MM-dd HH:mm:ss", null);
            // DateTime missiondate = DateTime.ParseExact(date, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
           
            return PartialView("~/Areas/IMS/Views/Home/_MissionDetails.cshtml", model);
        }

        #region Map
        public class MapVM
        {

            public Guid Id { get; set; }

            public string MissionDetail { get; set; }

            public string DISTRICT { get; set; }
            public string Partner { get; set; }
            public decimal GeoLong { get; set; }

            public decimal GeoLat { get; set; }

            

        }
        #endregion
    }
}