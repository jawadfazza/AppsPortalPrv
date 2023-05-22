using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.PCR.ViewModels;
using PCR_DAL.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.PCR.Controllers
{

    public class HomeController : PCRBaseController
    {
        // GET: PCR/Home
        public ActionResult Index()
        {
            CMS.SetUserToken(UserProfileGUID, Apps.PCR);
            Session[SessionKeys.CurrentApp] = Apps.PCR;
            CMS.BuildUserMenus(UserGUID, LAN);
            return View();
        }

        private PCRReportParametersMultiple FillRP(PCRReportParametersMultiple rp)
        {
            if (rp.DutyStationGUID == null) { rp.DutyStationGUID = DropDownList.SyriaDutyStationsForPCR().Select(x => x.Value).ToList().ConvertAll(Guid.Parse).ToArray(); }
            if (rp.OrganizationInstanceGUID == null) { rp.OrganizationInstanceGUID = DropDownList.OrganizationsInstancesAcronymByProfile().Select(x => x.Value).ToList().ConvertAll(Guid.Parse).ToArray(); }
            if (rp.PartnerCenterGUID == null) { rp.PartnerCenterGUID = DropDownList.PartnerCenterAll(rp.DutyStationGUID, rp.OrganizationInstanceGUID).Select(x => x.Value).ToList().ConvertAll(Guid.Parse).ToArray(); }
            if (rp.CategoryGUID2 == null) { rp.CategoryGUID2 = DropDownList.CategoryPartnerReportLevelChart2(rp.ReportGUID).Select(x => x.Value).ToList().ConvertAll(Guid.Parse).ToArray(); }
            if (rp.CategoryGUID3 == null) { rp.CategoryGUID3 = DropDownList.ParentCategoryPartnerReport(string.Join(",", rp.CategoryGUID2)).Select(x => x.Value).ToList().ConvertAll(Guid.Parse).ToArray(); }
            if (rp.CategoryGUID3.Count() == 1 && rp.CategoryGUID3[0] == Guid.Empty) { rp.CategoryGUID3 = DropDownList.ParentCategoryPartnerReport(string.Join(",", rp.CategoryGUID2)).Select(x => x.Value).ToList().ConvertAll(Guid.Parse).ToArray(); }

            if (rp.GenderGUID == null) { rp.GenderGUID = DropDownList.Genders().Select(x => x.Value).ToList().ConvertAll(Guid.Parse).ToArray(); }
            if (rp.AgeGUID == null) { rp.AgeGUID = DropDownList.AggregationAge(string.Join(",", rp.GenderGUID)).OrderBy(x => x.Value).Select(x => x.Value).ToList().ConvertAll(Guid.Parse).ToArray(); }
            if (rp.ProfileGUID == null) { rp.ProfileGUID = DropDownList.AggregationProfile(string.Join(",", rp.GenderGUID)).OrderBy(x => x.Value).Select(x => x.Value).ToList().ConvertAll(Guid.Parse).ToArray(); }
            if (rp.ReferralReasonGUID == null) { rp.ReferralReasonGUID = DbPCR.codeAggregation.Where(x => x.ReportGUID == ReportGUIDs.Referral).Select(x => x.AggregationGUID).ToArray(); }
            return rp;
        }

        /// <summary>
        /// Duty Stations
        /// </summary>
        /// <param name="rp"></param>
        /// <returns></returns>
        public ActionResult chart1(PCRReportParametersMultiple rp, string Org, string Gov, string Partner, string Cat3)
        {
            List<DataTableFilterOptions> filterOptions = new List<DataTableFilterOptions>();
            if (Org != "") { filterOptions.Add(new DataTableFilterOptions() { Field = "OrganizationInstanceDescription", FieldData = Org, Operation = "cn" }); }
            if (Gov != "") { filterOptions.Add(new DataTableFilterOptions() { Field = "DutyStationDescription", FieldData = Gov, Operation = "cn" }); }
            if (Partner != "") { filterOptions.Add(new DataTableFilterOptions() { Field = "PartnerCenterDescription", FieldData = Partner, Operation = "cn" }); }
            if (Cat3 != "") { filterOptions.Add(new DataTableFilterOptions() { Field = "Category3", FieldData = Cat3, Operation = "cn" }); }
            string title1 = "Registered";
            string title2 = "Assisted";
            if (rp.ReportGUID == ReportGUIDs.Monthly)
            {
                title1 = rp.AggregationGUID.FirstOrDefault() == Guid.Parse("00000000-0000-0000-0000-000000000001") ? "Families of vulnerable persons" : "Individuals with vulnerabilities";
                title2 = "";
                filterOptions.Add(new DataTableFilterOptions() { Field = "Category2", FieldData = "- Registered", Operation = "ne" });
            }

            Expression<Func<RP_PartnerCenter_Result, bool>> Predicate = p => true;
            Predicate = SearchHelper.CreateSearchPredicate<RP_PartnerCenter_Result>(new DataTableFilters() { FilterRules = filterOptions });
            rp = FillRP(rp);
            rp.AgeGUID = rp.FindAge(rp.AgeGUID, rp.GenderGUID);
            rp.ProfileGUID = rp.FindProfile(rp.ProfileGUID, rp.GenderGUID);
            var categories = DbPCR.codeDutyStationsLanguages.Where(x => x.LanguageID == LAN && rp.DutyStationGUID.Contains(x.DutyStationGUID)).OrderBy(x => x.DutyStationDescription).Select(x => new { x.DutyStationDescription, x.DutyStationGUID }).ToArray();
            var Registered =
                new Rainfall
                {
                    name = title1,
                    data = (from a in categories
                            join b in DbPCR.RP_PartnerCenter(
                        rp.EndDate,
                        rp.ReportGUID,
                        string.Join(",", rp.OrganizationInstanceGUID),
                        string.Join(",", rp.PartnerCenterGUID),
                        string.Join(",", rp.CategoryGUID2),
                        string.Join(",", rp.CategoryGUID3),
                        string.Join(",", rp.AggregationGUID),
                        LAN).AsQueryable().Where(Predicate) on a.DutyStationGUID equals b.DutyStationGUID into LJ1
                            from R1 in LJ1.DefaultIfEmpty(new PCR_DAL.Model.RP_PartnerCenter_Result() { DutyStationGUID = default(Guid) })
                            group new { R1.AggregationValue } by new { a.DutyStationGUID, a.DutyStationDescription } into grp
                            orderby grp.Key.DutyStationDescription
                            select grp.Key.DutyStationGUID != Guid.Empty ? grp.Sum(v => v.AggregationValue) : 0

                    ).ToArray()
                };


            List<Guid> AssistedGuids = GetAssisted(rp.CategoryGUID3);


            var Assisted = new Rainfall
            {
                name = title2,
                data = rp.ReportGUID == ReportGUIDs.Cumulative ? (from a in categories
                                                                  join b in DbPCR.RP_PartnerCenter(
                                                              rp.EndDate,
                                                              rp.ReportGUID,
                                                              string.Join(",", rp.OrganizationInstanceGUID),
                                                              string.Join(",", rp.PartnerCenterGUID),
                                                              string.Join(",", rp.CategoryGUID2),
                                                              string.Join(",", AssistedGuids),
                                                              string.Join(",", rp.AggregationGUID),
                                                              LAN).AsQueryable().Where(Predicate) on a.DutyStationGUID equals b.DutyStationGUID into LJ1
                                                                  from R1 in LJ1.DefaultIfEmpty(new PCR_DAL.Model.RP_PartnerCenter_Result() { DutyStationGUID = default(Guid) })
                                                                  group new { R1.AggregationValue } by new { a.DutyStationGUID, a.DutyStationDescription } into grp
                                                                  orderby grp.Key.DutyStationDescription
                                                                  select grp.Key.DutyStationGUID != Guid.Empty ? grp.Sum(v => v.AggregationValue) : 0

                    ).ToArray() : null


            };

            return Json(new { Registered = Registered, Assisted = Assisted, categories = categories.Select(x => x.DutyStationDescription).ToArray() }, JsonRequestBehavior.AllowGet);

        }

        private List<Guid> GetAssisted(Guid[] categoryGUID3)
        {
            List<Guid> AssistedGuids = new List<Guid>();
            foreach (Guid catAssisted in categoryGUID3)
            {
                int num = Convert.ToInt32(catAssisted.ToString().Substring(33)) + 1;
                if (num != 1)
                {
                    if (num >= 100)
                    {
                        AssistedGuids.Add(new Guid("00000000-0000-0000-0000-000000000" + num));
                    }
                    else { AssistedGuids.Add(new Guid("00000000-0000-0000-0000-0000000000" + num)); }
                }
            }

            return AssistedGuids;
        }

        /// <summary>
        /// Organizations
        /// </summary>
        /// <param name="rp"></param>
        /// <returns></returns>
        public ActionResult chart2(PCRReportParametersMultiple rp, string Org, string Gov, string Partner, string Cat3)
        {
            List<DataTableFilterOptions> filterOptions = new List<DataTableFilterOptions>();
            if (Org != "") { filterOptions.Add(new DataTableFilterOptions() { Field = "OrganizationInstanceDescription", FieldData = Org, Operation = "cn" }); }
            if (Gov != "") { filterOptions.Add(new DataTableFilterOptions() { Field = "DutyStationDescription", FieldData = Gov, Operation = "cn" }); }
            if (Partner != "") { filterOptions.Add(new DataTableFilterOptions() { Field = "PartnerCenterDescription", FieldData = Partner, Operation = "cn" }); }
            if (Cat3 != "") { filterOptions.Add(new DataTableFilterOptions() { Field = "Category3", FieldData = Cat3, Operation = "cn" }); }
            string title1 = "Registered";
            string title2 = "Assisted";
            if (rp.ReportGUID == ReportGUIDs.Monthly)
            {
                title1 = rp.AggregationGUID.FirstOrDefault() == Guid.Parse("00000000-0000-0000-0000-000000000001") ? "Families of vulnerable persons" : "Individuals with vulnerabilities";
                title2 = "";
                filterOptions.Add(new DataTableFilterOptions() { Field = "Category2", FieldData = "- Registered", Operation = "ne" });
            }
            List<Guid> guids = new List<Guid> { Guid.Parse("00000000-0000-0000-0000-000000000038"), Guid.Parse("00000000-0000-0000-0000-000000000215"), Guid.Parse("00000000-0000-0000-0000-000000000216") };

            Expression<Func<RP_PartnerCenter_Result, bool>> Predicate = p => true;
            Predicate = SearchHelper.CreateSearchPredicate<RP_PartnerCenter_Result>(new DataTableFilters() { FilterRules = filterOptions });
            rp = FillRP(rp);
            var categories = DbPCR.codeOrganizationsInstancesLanguages.Where(x => x.LanguageID == LAN && rp.OrganizationInstanceGUID.Contains(x.OrganizationInstanceGUID)).OrderBy(x => x.codeOrganizationsInstances.codeOrganizations.OrganizationShortName).Select(x => new { x.codeOrganizationsInstances.codeOrganizations.OrganizationShortName, x.OrganizationInstanceGUID }).ToArray();
            var Registered =
                new Rainfall
                {
                    name = title1,
                    data = (from a in categories
                            join b in DbPCR.RP_PartnerCenter(
                        rp.EndDate,
                        rp.ReportGUID,
                        string.Join(",", rp.OrganizationInstanceGUID),
                        string.Join(",", rp.PartnerCenterGUID),
                        string.Join(",", rp.CategoryGUID2),
                        string.Join(",", rp.CategoryGUID3),
                        string.Join(",", rp.AggregationGUID),
                        LAN).AsQueryable().Where(Predicate) on a.OrganizationInstanceGUID equals b.OrganizationInstanceGUID into LJ1
                            from R1 in LJ1.DefaultIfEmpty(new PCR_DAL.Model.RP_PartnerCenter_Result() { OrganizationInstanceGUID = default(Guid) })
                            group new { R1.AggregationValue } by new { a.OrganizationInstanceGUID, a.OrganizationShortName } into grp
                            orderby grp.Key.OrganizationShortName
                            select grp.Key.OrganizationInstanceGUID != Guid.Empty ? grp.Sum(v => v.AggregationValue) : 0

                    ).ToArray()
                };


            List<Guid> AssistedGuids = GetAssisted(rp.CategoryGUID3);

            var Assisted = new Rainfall
            {
                name = title2,
                data = rp.ReportGUID == ReportGUIDs.Cumulative ? (from a in categories
                                                                  join b in DbPCR.RP_PartnerCenter(
                                                              rp.EndDate,
                                                              rp.ReportGUID,
                                                              string.Join(",", rp.OrganizationInstanceGUID),
                                                              string.Join(",", rp.PartnerCenterGUID),
                                                              string.Join(",", rp.CategoryGUID2),
                                                              string.Join(",", AssistedGuids),
                                                              string.Join(",", rp.AggregationGUID),
                                                              LAN).AsQueryable().Where(Predicate) on a.OrganizationInstanceGUID equals b.OrganizationInstanceGUID into LJ1
                                                                  from R1 in LJ1.DefaultIfEmpty(new PCR_DAL.Model.RP_PartnerCenter_Result() { OrganizationInstanceGUID = default(Guid) })
                                                                  group new { R1.AggregationValue } by new { a.OrganizationInstanceGUID, a.OrganizationShortName } into grp
                                                                  orderby grp.Key.OrganizationShortName
                                                                  select grp.Key.OrganizationInstanceGUID != Guid.Empty ? grp.Sum(v => v.AggregationValue) : 0
                    ).ToArray() : null

            };
            return Json(new { Registered = Registered, Assisted = Assisted, categories = categories.Select(x => x.OrganizationShortName).ToArray() }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Partner Center
        /// </summary>
        /// <param name="rp"></param>
        /// <returns></returns>
        public ActionResult chart3(PCRReportParametersMultiple rp, string Org, string Gov, string Partner, string Cat3)
        {
            List<DataTableFilterOptions> filterOptions = new List<DataTableFilterOptions>();
            if (Org != "") { filterOptions.Add(new DataTableFilterOptions() { Field = "OrganizationInstanceDescription", FieldData = Org, Operation = "cn" }); }
            if (Gov != "") { filterOptions.Add(new DataTableFilterOptions() { Field = "DutyStationDescription", FieldData = Gov, Operation = "cn" }); }
            if (Partner != "") { filterOptions.Add(new DataTableFilterOptions() { Field = "PartnerCenterDescription", FieldData = Partner, Operation = "cn" }); }
            if (Cat3 != "") { filterOptions.Add(new DataTableFilterOptions() { Field = "Category3", FieldData = Cat3, Operation = "cn" }); }
            string title1 = "Registered";
            string title2 = "Assisted";
            if (rp.ReportGUID == ReportGUIDs.Monthly)
            {
                title1 = rp.AggregationGUID.FirstOrDefault() == Guid.Parse("00000000-0000-0000-0000-000000000001") ? "Families of vulnerable persons" : "Individuals with vulnerabilities";
                title2 = "";
                filterOptions.Add(new DataTableFilterOptions() { Field = "Category2", FieldData = "- Registered", Operation = "ne" });
            }

            Expression<Func<RP_PartnerCenter_Result, bool>> Predicate = p => true;
            Predicate = SearchHelper.CreateSearchPredicate<RP_PartnerCenter_Result>(new DataTableFilters() { FilterRules = filterOptions });

            rp = FillRP(rp);
            rp.AgeGUID = rp.FindAge(rp.AgeGUID, rp.GenderGUID);
            rp.ProfileGUID = rp.FindProfile(rp.ProfileGUID, rp.GenderGUID);

            var categories = DbPCR.codePartnerCenterLanguage.Where(x => x.LanguageID == LAN && rp.PartnerCenterGUID.Contains(x.PartnerCenterGUID)).OrderBy(x => x.PartnerCenterDescription).Select(x => new { x.PartnerCenterDescription, x.PartnerCenterGUID }).ToArray();
            if (Org != "")
            {
                var orgGuid = DbPCR.codeOrganizationsInstancesLanguages.Where(x => x.OrganizationInstanceDescription == Org).FirstOrDefault();
                categories = DbPCR.codePartnerCenterLanguage.Where(x => x.codePartnerCenter.OrganizationInstanceGUID == orgGuid.OrganizationInstanceGUID && x.LanguageID == LAN && rp.PartnerCenterGUID.Contains(x.PartnerCenterGUID)).OrderBy(x => x.PartnerCenterDescription).Select(x => new { x.PartnerCenterDescription, x.PartnerCenterGUID }).ToArray();
            }
            if (Gov != "")
            {
                var govGuid = DbPCR.codeDutyStationsLanguages.Where(x => x.DutyStationDescription == Gov).FirstOrDefault();
                categories = DbPCR.codePartnerCenterLanguage.Where(x => x.codePartnerCenter.DutyStationGUID == govGuid.DutyStationGUID && x.LanguageID == LAN && rp.PartnerCenterGUID.Contains(x.PartnerCenterGUID)).OrderBy(x => x.PartnerCenterDescription).Select(x => new { x.PartnerCenterDescription, x.PartnerCenterGUID }).ToArray();
            }

            var Registered =
                new Rainfall
                {
                    name = title1,
                    data = (from a in categories
                            join b in DbPCR.RP_PartnerCenter(
                        rp.EndDate,
                        rp.ReportGUID,
                        string.Join(",", rp.OrganizationInstanceGUID),
                        string.Join(",", rp.PartnerCenterGUID),
                        string.Join(",", rp.CategoryGUID2),
                        string.Join(",", rp.CategoryGUID3),
                        string.Join(",", rp.AggregationGUID),
                        LAN).AsQueryable().Where(Predicate) on a.PartnerCenterGUID equals b.PartnerCenterGUID into LJ1
                            from R1 in LJ1.DefaultIfEmpty(new PCR_DAL.Model.RP_PartnerCenter_Result() { OrganizationInstanceGUID = default(Guid) })
                            group new { R1.AggregationValue } by new { a.PartnerCenterGUID, a.PartnerCenterDescription } into grp
                            orderby grp.Key.PartnerCenterDescription
                            select grp.Key.PartnerCenterGUID != Guid.Empty ? grp.Sum(v => v.AggregationValue) : 0

                    ).ToArray()
                };


            List<Guid> AssistedGuids = GetAssisted(rp.CategoryGUID3);

            var Assisted = new Rainfall
            {
                name = title2,
                data = rp.ReportGUID == ReportGUIDs.Cumulative ? (from a in categories
                                                                  join b in DbPCR.RP_PartnerCenter(
                                                              rp.EndDate,
                                                              rp.ReportGUID,
                                                              string.Join(",", rp.OrganizationInstanceGUID),
                                                              string.Join(",", rp.PartnerCenterGUID),
                                                              string.Join(",", rp.CategoryGUID2),
                                                              string.Join(",", AssistedGuids),
                                                              string.Join(",", rp.AggregationGUID),
                                                              LAN).AsQueryable().Where(Predicate) on a.PartnerCenterGUID equals b.PartnerCenterGUID into LJ1
                                                                  from R1 in LJ1.DefaultIfEmpty(new PCR_DAL.Model.RP_PartnerCenter_Result() { OrganizationInstanceGUID = default(Guid) })
                                                                  group new { R1.AggregationValue } by new { a.PartnerCenterGUID, a.PartnerCenterDescription } into grp
                                                                  orderby grp.Key.PartnerCenterDescription
                                                                  select grp.Key.PartnerCenterGUID != Guid.Empty ? grp.Sum(v => v.AggregationValue) : 0

                    ).ToArray() : null

            };
            return Json(new { Registered = Registered, Assisted = Assisted, categories = categories.Select(x => x.PartnerCenterDescription).ToArray() }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Age 
        /// </summary>
        /// <param name="rp"></param>
        /// <param name="Org"></param>
        /// <param name="Gov"></param>
        /// <returns></returns>
        public ActionResult chart4(PCRReportParametersMultiple rp, string Org, string Gov, string Partner, string Cat3)
        {
            List<DataTableFilterOptions> filterOptions = new List<DataTableFilterOptions>();
            if (Org != "") { filterOptions.Add(new DataTableFilterOptions() { Field = "OrganizationInstanceDescription", FieldData = Org, Operation = "cn" }); }
            if (Gov != "") { filterOptions.Add(new DataTableFilterOptions() { Field = "DutyStationDescription", FieldData = Gov, Operation = "cn" }); }
            if (Partner != "") { filterOptions.Add(new DataTableFilterOptions() { Field = "PartnerCenterDescription", FieldData = Partner, Operation = "cn" }); }
            if (Cat3 != "") { filterOptions.Add(new DataTableFilterOptions() { Field = "Category3", FieldData = Cat3, Operation = "cn" }); }
            string title1 = "Registered";
            string title2 = "Assisted";
            if (rp.ReportGUID == ReportGUIDs.Monthly)
            {
                title1 = rp.AggregationGUID.FirstOrDefault() == Guid.Parse("00000000-0000-0000-0000-000000000001") ? "Families of vulnerable persons" : "Individuals with vulnerabilities";
                title2 = "";
                filterOptions.Add(new DataTableFilterOptions() { Field = "Category2", FieldData = "- Registered", Operation = "ne" });
            }
            Expression<Func<RP_PartnerCenter_Result, bool>> Predicate = p => true;
            Predicate = SearchHelper.CreateSearchPredicate<RP_PartnerCenter_Result>(new DataTableFilters() { FilterRules = filterOptions });


            rp = FillRP(rp);
            rp.AgeGUID = rp.FindAge(rp.AgeGUID, rp.GenderGUID);
            rp.ProfileGUID = rp.FindProfile(rp.ProfileGUID, rp.GenderGUID);

            var categories = DbPCR.codeAggregation.Where(x => rp.AgeGUID.Contains(x.AggregationGUID)).OrderBy(x => x.AggregationDescription).Select(x => new { x.AggregationDescription, x.AggregationGUID }).ToArray();
            var Registered =
                new Rainfall
                {
                    name = title1,
                    data = (from a in categories
                            join b in DbPCR.RP_PartnerCenter(rp.EndDate,
                            rp.ReportGUID, string.Join(",", rp.OrganizationInstanceGUID),
                            string.Join(",", rp.PartnerCenterGUID),
                            string.Join(",", rp.CategoryGUID2),
                            string.Join(",", rp.CategoryGUID3),
                            string.Join(",", rp.AgeGUID),
                            LAN).AsQueryable().Where(Predicate)
                            on a.AggregationGUID equals b.AggregationGUID into LJ1
                            from R1 in LJ1.DefaultIfEmpty(new PCR_DAL.Model.RP_PartnerCenter_Result() { AggregationGUID = default(Guid) })
                            group new { R1.AggregationValue } by new { a.AggregationGUID, a.AggregationDescription } into grp
                            orderby grp.Key.AggregationDescription
                            select grp.Key.AggregationGUID != Guid.Empty ? grp.Sum(v => v.AggregationValue) : 0).ToArray()
                };


            Rainfall RegisteredEnd = null;
            if (Registered.data.Length == 6)
            {
                RegisteredEnd = new Rainfall
                {
                    name = title1,
                    data = new int[] { Registered.data[0]+ Registered.data[1],
               Registered.data[2]+ Registered.data[3],
                Registered.data[4]+ Registered.data[5]}
                };
            }
            if (Registered.data.Length == 3)
            {
                RegisteredEnd = new Rainfall
                {
                    name = title1,
                    data = new int[] {
                       Registered.data[0],
                    Registered.data[1],
                    Registered.data[2]}
                };
            }

            Rainfall AssistedEnd = new Rainfall() { name = title2 };
            if (rp.ReportGUID != ReportGUIDs.Monthly)
            {

                List<Guid> AssistedGuids = GetAssisted(rp.CategoryGUID3);

                var Assisted = new Rainfall
                {
                    name = title2,
                    data = rp.ReportGUID == ReportGUIDs.Cumulative ?
                    (from a in categories
                     join b in DbPCR.RP_PartnerCenter(rp.EndDate,
                     rp.ReportGUID, string.Join(",", rp.OrganizationInstanceGUID),
                     string.Join(",", rp.PartnerCenterGUID),
                     string.Join(",", rp.CategoryGUID2),
                     string.Join(",", AssistedGuids),
                     string.Join(",", rp.AgeGUID),
                     LAN).AsQueryable().Where(Predicate)
                     on a.AggregationGUID equals b.AggregationGUID into LJ1
                     from R1 in LJ1.DefaultIfEmpty(new PCR_DAL.Model.RP_PartnerCenter_Result() { AggregationGUID = default(Guid) })
                     group new { R1.AggregationValue } by new { a.AggregationGUID, a.AggregationDescription } into grp
                     orderby grp.Key.AggregationDescription
                     select grp.Key.AggregationGUID != Guid.Empty ? grp.Sum(v => v.AggregationValue) : 0).ToArray() : null

                };
                if (Registered.data.Length == 6)
                {
                    AssistedEnd = new Rainfall
                    {
                        name = title2,
                        data = new int[] { Assisted.data[0]+ Assisted.data[1],
               Assisted.data[2]+ Assisted.data[3],
                Assisted.data[4]+ Assisted.data[5]}
                    };
                }
                if (Registered.data.Length == 3)
                {
                    AssistedEnd = new Rainfall
                    {
                        name = title2,
                        data = new int[] {
                    Assisted.data[0],
                    Assisted.data[1],
                    Assisted.data[2]}
                    };
                }

            }


            // return Json(new { Result = Result, categories = new string[] { "0-17", "18-59", "60+" } }, JsonRequestBehavior.AllowGet);
            return Json(new { Registered = RegisteredEnd, Assisted = AssistedEnd, categories = new string[] { "0-17", "18-59", "60+" } }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult chart5(PCRReportParametersMultiple rp, string Org, string Gov, string Partner, string Cat3)
        {
            List<DataTableFilterOptions> filterOptions = new List<DataTableFilterOptions>();
            if (Org != "") { filterOptions.Add(new DataTableFilterOptions() { Field = "OrganizationInstanceDescription", FieldData = Org, Operation = "cn" }); }
            if (Gov != "") { filterOptions.Add(new DataTableFilterOptions() { Field = "DutyStationDescription", FieldData = Gov, Operation = "cn" }); }
            if (Partner != "") { filterOptions.Add(new DataTableFilterOptions() { Field = "PartnerCenterDescription", FieldData = Partner, Operation = "cn" }); }
            if (Cat3 != "") { filterOptions.Add(new DataTableFilterOptions() { Field = "Category3", FieldData = Cat3, Operation = "cn" }); }
            string title1 = "Registered";
            string title2 = "Assisted";
            if (rp.ReportGUID == ReportGUIDs.Monthly)
            {
                title1 = rp.AggregationGUID.FirstOrDefault() == Guid.Parse("00000000-0000-0000-0000-000000000001") ? "Families of vulnerable persons" : "Individuals with vulnerabilities";
                title2 = "";
                filterOptions.Add(new DataTableFilterOptions() { Field = "Category2", FieldData = "- Registered", Operation = "ne" });
            }

            Expression<Func<RP_PartnerCenter_Result, bool>> Predicate = p => true;
            Predicate = SearchHelper.CreateSearchPredicate<RP_PartnerCenter_Result>(new DataTableFilters() { FilterRules = filterOptions });

            rp = FillRP(rp);
            rp.AgeGUID = rp.FindAge(rp.AgeGUID, rp.GenderGUID);
            rp.ProfileGUID = rp.FindProfile(rp.ProfileGUID, rp.GenderGUID);

            var categories = DbPCR.codeAggregation.Where(x => rp.ProfileGUID.Contains(x.AggregationGUID)).OrderBy(x => x.AggregationDescription).Select(x => new { x.AggregationDescription, x.AggregationGUID }).ToArray();
            var Registered =
                new Rainfall
                {
                    name = title1,
                    data = (from a in categories
                            join b in DbPCR.RP_PartnerCenter(
                        rp.EndDate,
                        rp.ReportGUID,
                        string.Join(",", rp.OrganizationInstanceGUID),
                        string.Join(",", rp.PartnerCenterGUID),
                        string.Join(",", rp.CategoryGUID2),
                        string.Join(",", rp.CategoryGUID3),
                        string.Join(",", rp.ProfileGUID),
                        LAN).AsQueryable().Where(Predicate) on a.AggregationGUID equals b.AggregationGUID into LJ1
                            from R1 in LJ1.DefaultIfEmpty(new PCR_DAL.Model.RP_PartnerCenter_Result() { AggregationGUID = default(Guid) })
                            group new { R1.AggregationValue } by new { a.AggregationGUID, a.AggregationDescription } into grp
                            orderby grp.Key.AggregationDescription
                            select grp.Key.AggregationGUID != Guid.Empty ? grp.Sum(v => v.AggregationValue) : 0

                    ).ToArray()
                };

            Rainfall RegisteredEnd = new Rainfall();
            if (Registered.data.Length == 10)
            {
                RegisteredEnd = new Rainfall
                {
                    name = title1,
                    data = new int[] {
                Registered.data[2]+ Registered.data[3],
                Registered.data[0]+ Registered.data[1],
                Registered.data[4]+ Registered.data[5],
                Registered.data[6]+ Registered.data[7],
                Registered.data[8]+ Registered.data[9]
                }
                };
            }
            if (Registered.data.Length == 5)
            {
                RegisteredEnd = new Rainfall
                {
                    name = title1,
                    data = new int[] {
                Registered.data[1],
                Registered.data[0],
                Registered.data[2],
                Registered.data[3],
                Registered.data[4]
                }
                };
            }



            Rainfall AssistedEnd = new Rainfall() { name = title2 };
            if (rp.ReportGUID != ReportGUIDs.Monthly)
            {
                List<Guid> AssistedGuids = GetAssisted(rp.CategoryGUID3);

                var Assisted = new Rainfall
                {
                    name = title2,
                    data = rp.ReportGUID == ReportGUIDs.Cumulative ? (from a in categories
                                                                      join b in DbPCR.RP_PartnerCenter(
                                                                  rp.EndDate,
                                                                  rp.ReportGUID,
                                                                  string.Join(",", rp.OrganizationInstanceGUID),
                                                                  string.Join(",", rp.PartnerCenterGUID),
                                                                  string.Join(",", rp.CategoryGUID2),
                                                                  string.Join(",", AssistedGuids),
                                                                  string.Join(",", rp.ProfileGUID),
                                                                  LAN).AsQueryable().Where(Predicate) on a.AggregationGUID equals b.AggregationGUID into LJ1
                                                                      from R1 in LJ1.DefaultIfEmpty(new PCR_DAL.Model.RP_PartnerCenter_Result() { AggregationGUID = default(Guid) })
                                                                      group new { R1.AggregationValue } by new { a.AggregationGUID, a.AggregationDescription } into grp
                                                                      orderby grp.Key.AggregationDescription
                                                                      select grp.Key.AggregationGUID != Guid.Empty ? grp.Sum(v => v.AggregationValue) : 0

                    ).ToArray() : null

                };

                if (Assisted.data.Length == 10)
                {
                    AssistedEnd = new Rainfall
                    {
                        name = title2,
                        data = new int[] {
                Assisted.data[2]+ Assisted.data[3],
                Assisted.data[0]+ Assisted.data[1],
                Assisted.data[4]+ Assisted.data[5],
                Assisted.data[6]+ Assisted.data[7],
                Assisted.data[8]+ Assisted.data[9]
                }
                    };
                }
                if (Assisted.data.Length == 5)
                {
                    AssistedEnd = new Rainfall
                    {
                        name = title2,
                        data = new int[] {
                Assisted.data[1],
                Assisted.data[0],
                Assisted.data[2],
                Assisted.data[3],
                Assisted.data[4]
                }
                    };
                }

            }
            return Json(new { Registered = RegisteredEnd, Assisted = AssistedEnd, categories = new string[] { "IDPs", "Host Community", "IDP Returnees", "REF Returnees (Syrians)", "REF/ASY in Syria" } }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult chart6(PCRReportParametersMultiple rp, string Org, string Gov, string Partner, string Cat3)
        {
            List<DataTableFilterOptions> filterOptions = new List<DataTableFilterOptions>();
            if (Org != "") { filterOptions.Add(new DataTableFilterOptions() { Field = "OrganizationInstanceDescription", FieldData = Org, Operation = "cn" }); }
            if (Gov != "") { filterOptions.Add(new DataTableFilterOptions() { Field = "DutyStationDescription", FieldData = Gov, Operation = "cn" }); }
            if (Partner != "") { filterOptions.Add(new DataTableFilterOptions() { Field = "PartnerCenterDescription", FieldData = Partner, Operation = "cn" }); }
            if (Cat3 != "") { filterOptions.Add(new DataTableFilterOptions() { Field = "Category3", FieldData = Cat3, Operation = "cn" }); }
            string title1 = "Registered";
            string title2 = "Assisted";
            if (rp.ReportGUID == ReportGUIDs.Monthly)
            {
                title1 = rp.AggregationGUID.FirstOrDefault() == Guid.Parse("00000000-0000-0000-0000-000000000001") ? "Families of vulnerable persons" : "Individuals with vulnerabilities";
                title2 = "";
                filterOptions.Add(new DataTableFilterOptions() { Field = "Category2", FieldData = "- Registered", Operation = "ne" });
            }
            Expression<Func<RP_PartnerCenter_Result, bool>> Predicate = p => true;
            Predicate = SearchHelper.CreateSearchPredicate<RP_PartnerCenter_Result>(new DataTableFilters() { FilterRules = filterOptions });

            rp = FillRP(rp);
            rp.AgeGUID = rp.FindAge(rp.AgeGUID, rp.GenderGUID);
            rp.ProfileGUID = rp.FindProfile(rp.ProfileGUID, rp.GenderGUID);

            var categories = DbPCR.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && rp.GenderGUID.Contains(x.ValueGUID)).OrderBy(x => x.ValueDescription).Select(x => new { x.ValueDescription, x.ValueGUID }).ToArray();
            var Registered =
                new Rainfall
                {
                    name = title1,
                    data = (from a in categories
                            join b in DbPCR.codeAggregation on a.ValueGUID equals b.GenderGUID
                            join c in DbPCR.RP_PartnerCenter(
                        rp.EndDate,
                        rp.ReportGUID,
                        string.Join(",", rp.OrganizationInstanceGUID),
                        string.Join(",", rp.PartnerCenterGUID),
                        string.Join(",", rp.CategoryGUID2),
                        string.Join(",", rp.CategoryGUID3),
                        string.Join(",", new Guid[] { Aggregation.Individuals_Male, Aggregation.Individuals_Female }),
                        LAN).AsQueryable().Where(Predicate) on b.AggregationGUID equals c.AggregationGUID into LJ1
                            from R1 in LJ1.DefaultIfEmpty(new PCR_DAL.Model.RP_PartnerCenter_Result() { AggregationGUID = default(Guid) })
                            group new { R1.AggregationValue } by new { a.ValueGUID, a.ValueDescription } into grp
                            orderby grp.Key.ValueDescription
                            select grp.Key.ValueGUID != Guid.Empty ? grp.Sum(v => v.AggregationValue) : 0

                    ).ToArray()
                };


            List<Guid> AssistedGuids = GetAssisted(rp.CategoryGUID3);

            var Assisted = new Rainfall
            {
                name = title2,
                data = rp.ReportGUID == ReportGUIDs.Cumulative ? (from a in categories
                                                                  join b in DbPCR.codeAggregation on a.ValueGUID equals b.GenderGUID
                                                                  join c in DbPCR.RP_PartnerCenter(
                                                              rp.EndDate,
                                                              rp.ReportGUID,
                                                              string.Join(",", rp.OrganizationInstanceGUID),
                                                              string.Join(",", rp.PartnerCenterGUID),
                                                              string.Join(",", rp.CategoryGUID2),
                                                              string.Join(",", AssistedGuids),
                                                              string.Join(",", new Guid[] { Aggregation.Individuals_Male, Aggregation.Individuals_Female }),
                                                              LAN).AsQueryable().Where(Predicate) on b.AggregationGUID equals c.AggregationGUID into LJ1
                                                                  from R1 in LJ1.DefaultIfEmpty(new PCR_DAL.Model.RP_PartnerCenter_Result() { AggregationGUID = default(Guid) })
                                                                  group new { R1.AggregationValue } by new { a.ValueGUID, a.ValueDescription } into grp
                                                                  orderby grp.Key.ValueDescription
                                                                  select grp.Key.ValueGUID != Guid.Empty ? grp.Sum(v => v.AggregationValue) : 0

                    ).ToArray() : null

            };
            return Json(new { Registered = Registered, Assisted = Assisted, categories = categories.Select(x => x.ValueDescription).ToArray() }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Vulnerabilities && Services Chart
        /// </summary>
        /// <param name="rp"></param>
        /// <param name="Org"></param>
        /// <param name="Gov"></param>
        /// <param name="Partner"></param>
        /// <param name="Cat3"></param>
        /// <returns></returns>
        public ActionResult chart7(PCRReportParametersMultiple rp, string Org, string Gov, string Partner, string Cat3)
        {
            List<DataTableFilterOptions> filterOptions = new List<DataTableFilterOptions>();
            if (Org != "") { filterOptions.Add(new DataTableFilterOptions() { Field = "OrganizationInstanceDescription", FieldData = Org, Operation = "cn" }); }
            if (Gov != "") { filterOptions.Add(new DataTableFilterOptions() { Field = "DutyStationDescription", FieldData = Gov, Operation = "cn" }); }
            if (Partner != "") { filterOptions.Add(new DataTableFilterOptions() { Field = "PartnerCenterDescription", FieldData = Partner, Operation = "cn" }); }
            if (Cat3 != "") { filterOptions.Add(new DataTableFilterOptions() { Field = "Category3", FieldData = Cat3, Operation = "cn" }); }
            string title1 = "Registered";
            string title2 = "Assisted";
            rp = FillRP(rp);
            if (rp.ReportGUID == ReportGUIDs.Monthly)
            {
                title1 = rp.AggregationGUID.FirstOrDefault() == Guid.Parse("00000000-0000-0000-0000-000000000001") ? "Families of vulnerable persons" : "Individuals with vulnerabilities";
                title2 = "";
                filterOptions.Add(new DataTableFilterOptions() { Field = "Category2", FieldData = "Channels", Operation = "ne" });
                filterOptions.Add(new DataTableFilterOptions() { Field = "Category2", FieldData = "- Registered", Operation = "ne" });
                //check if the total registered column included in the chart
                if (rp.CategoryGUID2.Where(x => x.ToString() == "00000000-0000-0000-0000-000000000037").FirstOrDefault() == Guid.Empty)
                {
                    var cat2 = rp.CategoryGUID2.ToList();
                    cat2.Add(Guid.Parse("00000000-0000-0000-0000-000000000037"));
                    rp.CategoryGUID2 = cat2.ToArray();

                    var cat3 = rp.CategoryGUID3.ToList();
                    cat3.Add(Guid.Parse("00000000-0000-0000-0000-000000000039"));
                    rp.CategoryGUID3 = cat3.ToArray();
                }
            }
            Expression<Func<RP_PartnerCenter_Result, bool>> Predicate = p => true;
            Predicate = SearchHelper.CreateSearchPredicate<RP_PartnerCenter_Result>(new DataTableFilters() { FilterRules = filterOptions });

            rp.AgeGUID = rp.FindAge(rp.AgeGUID, rp.GenderGUID);
            rp.ProfileGUID = rp.FindProfile(rp.ProfileGUID, rp.GenderGUID);

            List<Guid> guids = new List<Guid> { Guid.Parse("00000000-0000-0000-0000-000000000038"), Guid.Parse("00000000-0000-0000-0000-000000000215"), Guid.Parse("00000000-0000-0000-0000-000000000216") };
            var categories = DbPCR.codeCategoryReport.Where(x => rp.CategoryGUID2.Contains(x.CategoryReportGUID) && x.CategoryReportGUID.ToString() != "00000000-0000-0000-0000-000000000221").OrderBy(x => x.CategoryDescription).Select(x => new { x.CategoryDescription, x.CategoryReportGUID }).ToArray();
            var Registered =
                new Rainfall
                {
                    name = title1,
                    data = (from a in categories
                            join b in DbPCR.codeCategoryReport.Where(x => rp.CategoryGUID3.Contains(x.CategoryReportGUID)) on a.CategoryReportGUID equals b.ParentCategoryReportGUID
                            join c in DbPCR.RP_PartnerCenter(
                        rp.EndDate,
                        rp.ReportGUID,
                        string.Join(",", rp.OrganizationInstanceGUID),
                        string.Join(",", rp.PartnerCenterGUID),
                        string.Join(",", rp.CategoryGUID2),
                        string.Join(",", rp.CategoryGUID3),
                        string.Join(",", rp.AggregationGUID),
                        LAN).AsQueryable().Where(Predicate).Where(x => !guids.Contains(x.CategoryReportGUID)) on b.CategoryReportGUID equals c.CategoryReportGUID into LJ1
                            from R1 in LJ1.DefaultIfEmpty(new PCR_DAL.Model.RP_PartnerCenter_Result() { CategoryReportGUID = default(Guid) })
                            group new { R1.AggregationValue } by new { a.CategoryReportGUID, a.CategoryDescription } into grp
                            orderby grp.Key.CategoryDescription
                            select grp.Key.CategoryReportGUID != Guid.Empty ? grp.Sum(v => v.AggregationValue) : 0
                    ).ToArray()
                };

            List<Guid> AssistedGuids = GetAssisted(rp.CategoryGUID3);

            var Assisted = new Rainfall
            {
                name = title2,
                data = rp.ReportGUID == ReportGUIDs.Cumulative ? (from a in categories
                                                                  join b in DbPCR.codeCategoryReport.Where(x => AssistedGuids.Contains(x.CategoryReportGUID)) on a.CategoryReportGUID equals b.ParentCategoryReportGUID
                                                                  join c in DbPCR.RP_PartnerCenter(
                                                              rp.EndDate,
                                                              rp.ReportGUID,
                                                              string.Join(",", rp.OrganizationInstanceGUID),
                                                              string.Join(",", rp.PartnerCenterGUID),
                                                              string.Join(",", rp.CategoryGUID2),
                                                              string.Join(",", AssistedGuids),
                                                              string.Join(",", rp.AggregationGUID),
                                                              LAN).AsQueryable().Where(Predicate) on b.CategoryReportGUID equals c.CategoryReportGUID into LJ1
                                                                  from R1 in LJ1.DefaultIfEmpty(new PCR_DAL.Model.RP_PartnerCenter_Result() { CategoryReportGUID = default(Guid) })
                                                                  group new { R1.AggregationValue } by new { a.CategoryReportGUID, a.CategoryDescription } into grp
                                                                  orderby grp.Key.CategoryDescription
                                                                  select grp.Key.CategoryReportGUID != Guid.Empty ? grp.Sum(v => v.AggregationValue) : 0
                    ).ToArray() : null
            };
            return Json(new { Registered = Registered, Assisted = Assisted, categories = categories.Select(x => x.CategoryDescription).ToArray() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult chart8(PCRReportParametersMultiple rp, string Cat2, string Org, string Gov, string Partner, string Cat3)
        {
            List<Guid> guids = new List<Guid> { Guid.Parse("00000000-0000-0000-0000-000000000038"), Guid.Parse("00000000-0000-0000-0000-000000000215"), Guid.Parse("00000000-0000-0000-0000-000000000216") };

            List<DataTableFilterOptions> filterOptions = new List<DataTableFilterOptions>();
            if (Org != "") { filterOptions.Add(new DataTableFilterOptions() { Field = "OrganizationInstanceDescription", FieldData = Org, Operation = "cn" }); }
            if (Gov != "") { filterOptions.Add(new DataTableFilterOptions() { Field = "DutyStationDescription", FieldData = Gov, Operation = "cn" }); }
            if (Partner != "") { filterOptions.Add(new DataTableFilterOptions() { Field = "PartnerCenterDescription", FieldData = Partner, Operation = "cn" }); }
            if (Cat3 != "") { filterOptions.Add(new DataTableFilterOptions() { Field = "Category3", FieldData = Cat3, Operation = "cn" }); }
            string title1 = "Registered";
            string title2 = "Assisted";
            if (rp.ReportGUID == ReportGUIDs.Monthly)
            {
                title1 = rp.AggregationGUID.FirstOrDefault() == Guid.Parse("00000000-0000-0000-0000-000000000001") ? "Families of vulnerable persons" : "Individuals with vulnerabilities";
                title2 = "";
                filterOptions.Add(new DataTableFilterOptions() { Field = "Category2", FieldData = "Channels", Operation = "nc" });
            }
            Expression<Func<RP_PartnerCenter_Result, bool>> Predicate = p => true;
            Predicate = SearchHelper.CreateSearchPredicate<RP_PartnerCenter_Result>(new DataTableFilters() { FilterRules = filterOptions });


            rp = FillRP(rp);
            rp.AgeGUID = rp.FindAge(rp.AgeGUID, rp.GenderGUID);
            rp.ProfileGUID = rp.FindProfile(rp.ProfileGUID, rp.GenderGUID);

            var parCategories = DbPCR.codeCategoryReport.Where(x => x.CategoryDescription == Cat2).Select(x => new { x.CategoryDescription, x.CategoryReportGUID }).FirstOrDefault();
            var categories = DbPCR.codeCategoryReport.Where(x => !guids.Contains(x.CategoryReportGUID)).Where(x => x.ParentCategoryReportGUID == parCategories.CategoryReportGUID && rp.CategoryGUID3.Contains(x.CategoryReportGUID)).OrderBy(x => x.CategoryDescription).Select(x => new { x.CategoryDescription, x.CategoryReportGUID }).ToArray();
            var Registered =
                new Rainfall
                {
                    name = title1,
                    data = (from a in categories
                            join c in DbPCR.RP_PartnerCenter(
                        rp.EndDate,
                        rp.ReportGUID,
                        string.Join(",", rp.OrganizationInstanceGUID),
                        string.Join(",", rp.PartnerCenterGUID),
                        string.Join(",", rp.CategoryGUID2),
                        string.Join(",", rp.CategoryGUID3),
                        string.Join(",", rp.AggregationGUID),
                        LAN).AsQueryable().Where(Predicate).Where(x => !guids.Contains(x.CategoryReportGUID)) on a.CategoryReportGUID equals c.CategoryReportGUID into LJ1
                            from R1 in LJ1.DefaultIfEmpty(new PCR_DAL.Model.RP_PartnerCenter_Result() { CategoryReportGUID = default(Guid) })
                            group new { R1.AggregationValue } by new { a.CategoryReportGUID, a.CategoryDescription } into grp
                            orderby grp.Key.CategoryDescription
                            select grp.Key.CategoryReportGUID != Guid.Empty ? grp.Sum(v => v.AggregationValue) : 0
                    ).ToArray()
                };


            List<Guid> AssistedGuids = GetAssisted(rp.CategoryGUID3);

            var categories1 = DbPCR.codeCategoryReport.Where(x => x.ParentCategoryReportGUID == parCategories.CategoryReportGUID && AssistedGuids.Contains(x.CategoryReportGUID)).OrderBy(x => x.CategoryDescription).Select(x => new { x.CategoryDescription, x.CategoryReportGUID }).ToArray();

            var Assisted = new Rainfall
            {
                name = title2,
                data = rp.ReportGUID == ReportGUIDs.Cumulative ? (from a in categories1
                                                                  join c in DbPCR.RP_PartnerCenter(
                                                              rp.EndDate,
                                                              rp.ReportGUID,
                                                              string.Join(",", rp.OrganizationInstanceGUID),
                                                              string.Join(",", rp.PartnerCenterGUID),
                                                              string.Join(",", rp.CategoryGUID2),
                                                              string.Join(",", AssistedGuids),
                                                              string.Join(",", rp.AggregationGUID),
                                                              LAN).AsQueryable().Where(Predicate) on a.CategoryReportGUID equals c.CategoryReportGUID into LJ1
                                                                  from R1 in LJ1.DefaultIfEmpty(new PCR_DAL.Model.RP_PartnerCenter_Result() { CategoryReportGUID = default(Guid) })
                                                                  group new { R1.AggregationValue } by new { a.CategoryReportGUID, a.CategoryDescription } into grp
                                                                  orderby grp.Key.CategoryDescription
                                                                  select grp.Key.CategoryReportGUID != Guid.Empty ? grp.Sum(v => v.AggregationValue) : 0
                    ).ToArray() : null

            };
            return Json(new { Registered = Registered, Assisted = Assisted, categories = categories.Select(x => x.CategoryDescription).ToArray() }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult chart9(PCRReportParametersMultiple rp)
        {

            rp = FillRP(rp);
            List<Guid> guids = new List<Guid> { /*CategoryReport.TotalOfVulnerabilityCodesEntered, CategoryReport.Totalservicesprovided,*/ CategoryReport.NewRegistration };
            var categories = DbPCR.codeCategoryReport.Where(x => guids.Contains(x.CategoryReportGUID)).OrderBy(x => x.CategoryDescription).Select(x => new { x.CategoryDescription, x.CategoryReportGUID }).ToArray();

            var Result =
                new Rainfall
                {
                    name = "Totals",
                    data = (from a in categories
                            join b in DbPCR.RP_PartnerCenter(
                        rp.EndDate,
                        ReportGUIDs.Monthly,
                        string.Join(",", rp.OrganizationInstanceGUID),
                        string.Join(",", rp.PartnerCenterGUID),
                        string.Join(",", new Guid[] { Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000037") }),
                        string.Join(",", new Guid[] {/* CategoryReport.TotalOfVulnerabilityCodesEntered, */CategoryReport.NewRegistration }),
                        string.Join(",", rp.AggregationGUID),
                        LAN).AsQueryable() on a.CategoryReportGUID equals b.CategoryReportGUID into LJ1
                            from R1 in LJ1.DefaultIfEmpty(new PCR_DAL.Model.RP_PartnerCenter_Result() { CategoryReportGUID = default(Guid) })
                            group new { R1.AggregationValue } by new { a.CategoryReportGUID, a.CategoryDescription } into grp
                            orderby grp.Key.CategoryDescription
                            select grp.Key.CategoryReportGUID != Guid.Empty ? grp.Sum(v => v.AggregationValue) : 0

                    ).ToArray()
                };

            //var Total2 =
            //  new Rainfall
            //  {
            //      name = "Total Services Provided",
            //      data = (from a in categories
            //              join b in DbPCR.RP_PartnerCenter(
            //          rp.EndDate,
            //         ReportGUIDs.Cumulative,
            //          string.Join(",", rp.OrganizationInstanceGUID),
            //          string.Join(",", rp.PartnerCenterGUID),
            //          string.Join(",", new Guid[] { Guid.Parse("00000000-0000-0000-0000-000000000219") }),
            //          string.Join(",", CategoryReport.Totalservicesprovided),
            //          string.Join(",", rp.AggregationGUID),
            //          LAN).AsQueryable() on a.CategoryReportGUID equals b.CategoryReportGUID into LJ1
            //              from R1 in LJ1.DefaultIfEmpty(new PCR_DAL.Model.RP_PartnerCenter_Result() { CategoryReportGUID = default(Guid) })
            //              group new { R1.AggregationValue } by new { a.CategoryReportGUID, a.CategoryDescription } into grp
            //              orderby grp.Key.CategoryDescription
            //              select grp.Key.CategoryReportGUID != Guid.Empty ? grp.Sum(v => v.AggregationValue) : 0

            //      ).ToArray()
            //  };

            //Result.data[2] = Total2.data[2];

            return Json(new { Result = Result, categories = categories.Select(x => x.CategoryDescription).ToArray() }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult chart10(PCRReportParametersMultiple rp)
        {
            List<DataTableFilterOptions> filterOptions = new List<DataTableFilterOptions>();

            rp = FillRP(rp);
            rp.AgeGUID = rp.FindAge(rp.AgeGUID, rp.GenderGUID);
            rp.ProfileGUID = rp.FindProfile(rp.ProfileGUID, rp.GenderGUID);

            var categories = DbPCR.codeAggregation.Where(x => rp.AgeGUID.Contains(x.AggregationGUID)).OrderBy(x => x.AggregationDescription).Select(x => new { x.AggregationDescription, x.AggregationGUID }).ToArray();
            var All =
                new Rainfall
                {
                    name = "Totals",
                    data = (from a in categories
                            join b in DbPCR.RP_PartnerCenter(rp.EndDate,
                            ReportGUIDs.Monthly,
                            string.Join(",", rp.OrganizationInstanceGUID),
                            string.Join(",", rp.PartnerCenterGUID),
                       string.Join(",", new Guid[] { Guid.Parse("00000000-0000-0000-0000-000000000037") }),
                        string.Join(",", new Guid[] { CategoryReport.NewRegistration }),
                            string.Join(",", rp.AgeGUID),
                            LAN).AsQueryable()
                            on a.AggregationGUID equals b.AggregationGUID into LJ1
                            from R1 in LJ1.DefaultIfEmpty(new PCR_DAL.Model.RP_PartnerCenter_Result() { AggregationGUID = default(Guid) })
                            group new { R1.AggregationValue } by new { a.AggregationGUID, a.AggregationDescription } into grp
                            orderby grp.Key.AggregationDescription
                            select grp.Key.AggregationGUID != Guid.Empty ? grp.Sum(v => v.AggregationValue) : 0).ToArray()
                };
            Rainfall Result = null;
            if (All.data.Length == 6)
            {
                Result = new Rainfall
                {
                    name = "Totals",
                    data = new int[] { All.data[0]+ All.data[1],
               All.data[2]+ All.data[3],
                All.data[4]+ All.data[5]}
                };
            }
            if (All.data.Length == 3)
            {
                Result = new Rainfall
                {
                    name = "Totals",
                    data = new int[] {
                       All.data[0],
                    All.data[1],
                    All.data[2]}
                };
            }

            return Json(new { Result = Result, categories = new string[] { "0-17", "18-59", "60+" } }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult chart11(PCRReportParametersMultiple rp)
        {
            List<DataTableFilterOptions> filterOptions = new List<DataTableFilterOptions>();

            rp = FillRP(rp);
            rp.AgeGUID = rp.FindAge(rp.AgeGUID, rp.GenderGUID);
            rp.ProfileGUID = rp.FindProfile(rp.ProfileGUID, rp.GenderGUID);

            var categories = DbPCR.codeAggregation.Where(x => rp.ProfileGUID.Contains(x.AggregationGUID)).OrderBy(x => x.AggregationDescription).Select(x => new { x.AggregationDescription, x.AggregationGUID }).ToArray();
            var All =
                new Rainfall
                {
                    name = "Totals",
                    data = (from a in categories
                            join b in DbPCR.RP_PartnerCenter(rp.EndDate,
                            ReportGUIDs.Monthly,
                            string.Join(",", rp.OrganizationInstanceGUID),
                            string.Join(",", rp.PartnerCenterGUID),
                       string.Join(",", new Guid[] { Guid.Parse("00000000-0000-0000-0000-000000000037") }),
                        string.Join(",", new Guid[] { CategoryReport.NewRegistration }),
                            string.Join(",", rp.ProfileGUID),
                            LAN).AsQueryable()
                            on a.AggregationGUID equals b.AggregationGUID into LJ1
                            from R1 in LJ1.DefaultIfEmpty(new PCR_DAL.Model.RP_PartnerCenter_Result() { AggregationGUID = default(Guid) })
                            group new { R1.AggregationValue } by new { a.AggregationGUID, a.AggregationDescription } into grp
                            orderby grp.Key.AggregationDescription
                            select grp.Key.AggregationGUID != Guid.Empty ? grp.Sum(v => v.AggregationValue) : 0).ToArray()
                };
            Rainfall Result = null;
            if (All.data.Length == 10)
            {
                Result = new Rainfall
                {
                    name = "Totals",
                    data = new int[] {
                All.data[2]+ All.data[3],
                All.data[0]+ All.data[1],
                All.data[4]+ All.data[5],
                All.data[6]+ All.data[7],
                All.data[8]+ All.data[9]
                }
                };
            }
            if (All.data.Length == 5)
            {
                Result = new Rainfall
                {
                    name = "Totals",
                    data = new int[] {
                All.data[1],
                All.data[0],
                All.data[2],
                All.data[3],
                All.data[4]
                }
                };
            }

            return Json(new { Result = Result, categories = new string[] { "IDPs", "Host Community", "IDP Returnees", "REF Returnees (Syrians)", "REF/ASY in Syria" } }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult chart12(PCRReportParametersMultiple rp)
        {
            List<DataTableFilterOptions> filterOptions = new List<DataTableFilterOptions>();

            rp = FillRP(rp);
            rp.AgeGUID = rp.FindAge(rp.AgeGUID, rp.GenderGUID);
            rp.ProfileGUID = rp.FindProfile(rp.ProfileGUID, rp.GenderGUID);

            var categories = DbPCR.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && rp.GenderGUID.Contains(x.ValueGUID)).OrderBy(x => x.ValueDescription).Select(x => new { x.ValueDescription, x.ValueGUID }).ToArray();


            var Result =
               new Rainfall
               {
                   name = "Totals",
                   data = (from a in categories
                           join
                           b in DbPCR.codeAggregation on a.ValueGUID equals b.GenderGUID
                           join c in DbPCR.RP_PartnerCenter(
                       rp.EndDate,
                       rp.ReportGUID,
                       string.Join(",", rp.OrganizationInstanceGUID),
                       string.Join(",", rp.PartnerCenterGUID),
                       string.Join(",", new Guid[] { Guid.Parse("00000000-0000-0000-0000-000000000037") }),
                        string.Join(",", new Guid[] { CategoryReport.NewRegistration }),
                       string.Join(",", new Guid[] { Aggregation.Individuals_Male, Aggregation.Individuals_Female }),
                       LAN).AsQueryable() on b.AggregationGUID equals c.AggregationGUID into LJ1
                           from R1 in LJ1.DefaultIfEmpty(new PCR_DAL.Model.RP_PartnerCenter_Result() { AggregationGUID = default(Guid) })
                           group new { R1.AggregationValue } by new { a.ValueGUID, a.ValueDescription } into grp
                           orderby grp.Key.ValueDescription
                           select grp.Key.ValueGUID != Guid.Empty ? grp.Sum(v => v.AggregationValue) : 0

                   ).ToArray()
               };



            return Json(new { Result = Result, categories = categories.Select(x => x.ValueDescription).ToArray() }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult chart13(PCRReportParametersMultiple rp)
        {

            rp = FillRP(rp);
            rp.AgeGUID = rp.FindAge(rp.AgeGUID, rp.GenderGUID);
            rp.ProfileGUID = rp.FindProfile(rp.ProfileGUID, rp.GenderGUID);

            var categories = DbPCR.codePartnerCenterLanguage.Where(x => x.LanguageID == LAN && rp.PartnerCenterGUID.Contains(x.PartnerCenterGUID)).OrderBy(x => x.PartnerCenterDescription).Select(x => new { x.PartnerCenterDescription, x.PartnerCenterGUID }).ToArray();


            var Result =
               new Rainfall
               {
                   name = "Totals",
                   data = (from a in categories
                           join b in DbPCR.RP_PartnerCenter(
                       rp.EndDate,
                       rp.ReportGUID,
                       string.Join(",", rp.OrganizationInstanceGUID),
                       string.Join(",", rp.PartnerCenterGUID),
                     string.Join(",", new Guid[] { Guid.Parse("00000000-0000-0000-0000-000000000037") }),
                        string.Join(",", new Guid[] { CategoryReport.NewRegistration }),
                       string.Join(",", rp.AggregationGUID),
                      LAN).AsQueryable() on a.PartnerCenterGUID equals b.PartnerCenterGUID into LJ1
                           from R1 in LJ1.DefaultIfEmpty(new PCR_DAL.Model.RP_PartnerCenter_Result() { OrganizationInstanceGUID = default(Guid) })
                           group new { R1.AggregationValue } by new { a.PartnerCenterGUID, a.PartnerCenterDescription } into grp
                           orderby grp.Key.PartnerCenterDescription
                           select grp.Key.PartnerCenterGUID != Guid.Empty ? grp.Sum(v => v.AggregationValue) : 0

                   ).ToArray()
               };



            return Json(new { Result = Result, categories = categories.Select(x => x.PartnerCenterDescription).ToArray() }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult chart17(PCRReportParametersMultiple rp)
        {

            rp = FillRP(rp);
            rp.AgeGUID = rp.FindAge(rp.AgeGUID, rp.GenderGUID);
            rp.ProfileGUID = rp.FindProfile(rp.ProfileGUID, rp.GenderGUID);

            var categories = DbPCR.codeDutyStationsLanguages.Where(x => x.LanguageID == LAN && rp.DutyStationGUID.Contains(x.DutyStationGUID)).OrderBy(x => x.DutyStationDescription).Select(x => new { x.DutyStationDescription, x.DutyStationGUID }).ToArray();

            var Result =
               new Rainfall
               {
                   name = "Totals",
                   data = (from a in categories
                           join b in DbPCR.RP_PartnerCenter(
                       rp.EndDate,
                       rp.ReportGUID,
                       string.Join(",", rp.OrganizationInstanceGUID),
                       string.Join(",", rp.PartnerCenterGUID),
                     string.Join(",", new Guid[] { Guid.Parse("00000000-0000-0000-0000-000000000037") }),
                        string.Join(",", new Guid[] { CategoryReport.NewRegistration }),
                       string.Join(",", rp.AggregationGUID),
                       LAN).AsQueryable() on a.DutyStationGUID equals b.DutyStationGUID into LJ1
                           from R1 in LJ1.DefaultIfEmpty(new PCR_DAL.Model.RP_PartnerCenter_Result() { DutyStationGUID = default(Guid) })
                           group new { R1.AggregationValue } by new { a.DutyStationGUID, a.DutyStationDescription } into grp
                           orderby grp.Key.DutyStationDescription
                           select grp.Key.DutyStationGUID != Guid.Empty ? grp.Sum(v => v.AggregationValue) : 0


                   ).ToArray()
               };


            return Json(new { Result = Result, categories = categories.Select(x => x.DutyStationDescription).ToArray() }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult chart18(PCRReportParametersMultiple rp)
        {

            rp = FillRP(rp);
            rp.AgeGUID = rp.FindAge(rp.AgeGUID, rp.GenderGUID);
            rp.ProfileGUID = rp.FindProfile(rp.ProfileGUID, rp.GenderGUID);

            var categories = DbPCR.codeOrganizationsInstancesLanguages.Where(x => x.LanguageID == LAN && rp.OrganizationInstanceGUID.Contains(x.OrganizationInstanceGUID)).OrderBy(x => x.codeOrganizationsInstances.codeOrganizations.OrganizationShortName).Select(x => new { x.codeOrganizationsInstances.codeOrganizations.OrganizationShortName, x.OrganizationInstanceGUID }).ToArray();


            var Result =
               new Rainfall
               {
                   name = "Totals",
                   data = (from a in categories
                           join b in DbPCR.RP_PartnerCenter(
                       rp.EndDate,
                       rp.ReportGUID,
                       string.Join(",", rp.OrganizationInstanceGUID),
                       string.Join(",", rp.PartnerCenterGUID),
                     string.Join(",", new Guid[] { Guid.Parse("00000000-0000-0000-0000-000000000037") }),
                        string.Join(",", new Guid[] { CategoryReport.NewRegistration }),
                       string.Join(",", rp.AggregationGUID),
                       LAN).AsQueryable() on a.OrganizationInstanceGUID equals b.OrganizationInstanceGUID into LJ1
                           from R1 in LJ1.DefaultIfEmpty(new PCR_DAL.Model.RP_PartnerCenter_Result() { OrganizationInstanceGUID = default(Guid) })
                           group new { R1.AggregationValue } by new { a.OrganizationInstanceGUID, a.OrganizationShortName } into grp
                           orderby grp.Key.OrganizationShortName
                           select grp.Key.OrganizationInstanceGUID != Guid.Empty ? grp.Sum(v => v.AggregationValue) : 0


                   ).ToArray()
               };



            return Json(new { Result = Result, categories = categories.Select(x => x.OrganizationShortName).ToArray() }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult chart14(PCRReportParametersMultiple rp)
        {
            try
            {
                rp = FillRP(rp);
            }
            catch (Exception) { }
            var guids = new Guid[] { Guid.Parse("00000000-0000-0000-0000-000000000028"), Guid.Parse("00000000-0000-0000-0000-000000000027") };
            var categories = DbPCR.codeAggregation.Where(x => x.ReportGUID == ReportGUIDs.Referral && !guids.Contains(x.AggregationGUID)).OrderBy(x => x.AggregationDescription).Select(x => new { x.AggregationDescription, x.AggregationGUID }).ToArray();
            // rp.AggregationGUID = categories.Select(x => x.AggregationGUID).ToArray();

            var Result =
              new Rainfall
              {
                  name = "Totals",
                  data = (from a in categories
                          join b in DbPCR.RP_PartnerCenter(
                      rp.EndDate,
                      rp.ReportGUID,
                      string.Join(",", rp.OrganizationInstanceGUID),
                      string.Join(",", rp.PartnerCenterGUID),
                      string.Join(",", new Guid[] { Guid.Parse("00000000-0000-0000-0000-000000000228") }),
                      string.Join(",", new Guid[] { Guid.Parse("00000000-0000-0000-0000-000000000229") }),
                      string.Join(",", rp.ReferralReasonGUID),
                      LAN).AsQueryable().Where(x => !guids.Contains(x.AggregationGUID)) on a.AggregationGUID equals b.AggregationGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty(new PCR_DAL.Model.RP_PartnerCenter_Result() { AggregationGUID = default(Guid) })
                          group new { R1.AggregationValue } by new { a.AggregationGUID, a.AggregationDescription } into grp
                          orderby grp.Key.AggregationDescription
                          select grp.Key.AggregationGUID != Guid.Empty ? grp.Sum(v => v.AggregationValue) : 0

                  ).ToArray()
              };



            return Json(new { Result = Result, categories = categories.Select(x => x.AggregationDescription).ToArray() }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult chart15(PCRReportParametersMultiple rp)
        {
            try
            {
                rp = FillRP(rp);
            }
            catch (Exception) { }
            // string[] categories = new string[] { "_", "Al_Batoul", "Al_Birr", "Al_Nada", "Aoun", "Child_Care", "GOPA", "Governmental_Entity", "Other_entity", "Other_NGO", "SARC", "Social_Care", "SSSD", "Tamayouz" };
            // rp.AggregationGUID = DbPCR.codeAggregation.Where(x => x.ReportGUID == ReportGUIDs.Referral).Select(x => x.AggregationGUID).ToArray();
            var categories = DbPCR.codePartnerCenterLanguage.Where(x => x.LanguageID == LAN && rp.PartnerCenterGUID.Contains(x.PartnerCenterGUID)).OrderBy(x => x.PartnerCenterDescription).Select(x => new { x.PartnerCenterDescription, x.PartnerCenterGUID }).ToArray();


            var Result =
               new Rainfall
               {
                   name = "Totals",
                   data = (from a in categories
                           join b in DbPCR.RP_PartnerCenter(
                       rp.EndDate,
                       rp.ReportGUID,
                       string.Join(",", rp.OrganizationInstanceGUID),
                       string.Join(",", rp.PartnerCenterGUID),
                       string.Join(",", new Guid[] { Guid.Parse("00000000-0000-0000-0000-000000000228") }),
                       string.Join(",", new Guid[] { Guid.Parse("00000000-0000-0000-0000-000000000229") }),
                       string.Join(",", rp.ReferralReasonGUID),
                     LAN).AsQueryable().Where(x => x.AggregationGUID != Aggregation.Total_Referral) on a.PartnerCenterGUID equals b.PartnerCenterGUID into LJ1
                           from R1 in LJ1.DefaultIfEmpty(new PCR_DAL.Model.RP_PartnerCenter_Result() { PartnerCenterGUID = default(Guid) })
                           group new { R1.AggregationValue } by new { a.PartnerCenterGUID, a.PartnerCenterDescription } into grp
                           orderby grp.Key.PartnerCenterDescription
                           select grp.Key.PartnerCenterGUID != Guid.Empty ? grp.Sum(v => v.AggregationValue) : 0

                   ).ToArray()
               };

            return Json(new { Result = Result, categories = categories.Select(x => x.PartnerCenterDescription).ToArray() }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult chart16(PCRReportParametersMultiple rp)
        {
            try
            {
                rp = FillRP(rp);
            }
            catch (Exception) { }
            var categories = DbPCR.codeCategoryReport.Where(x => x.ReportGUID == ReportGUIDs.Monthly && x.ParentCategoryReportGUID.ToString() == "00000000-0000-0000-0000-000000000221").OrderBy(x => x.CategoryDescription).Select(x => new { x.CategoryDescription, x.CategoryReportGUID }).ToArray();
            rp.CategoryGUID3 = categories.Select(x => x.CategoryReportGUID).ToArray();

            var Result = (
                          from b in DbPCR.RP_PartnerCenter(
                      rp.EndDate,
                      ReportGUIDs.Monthly,
                      string.Join(",", rp.OrganizationInstanceGUID),
                      string.Join(",", rp.PartnerCenterGUID),
                      string.Join(",", new Guid[] { Guid.Parse("00000000-0000-0000-0000-000000000221") }),
                      string.Join(",", rp.CategoryGUID3),
                      string.Join(",", new Guid[] { Aggregation.family }),
                      LAN).AsQueryable()

                          group new { b.AggregationValue, b.Category3 } by new { b.CategoryReportGUID, b.Category3 } into grp
                          orderby grp.Key.Category3
                          select new series
                          {
                              name = grp.Key.Category3,
                              y = grp.Sum(v => v.AggregationValue)
                          }

                  ).ToArray();

            return Json(new { Result = Result, categories = categories.Select(x => x.CategoryDescription).ToArray() }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult chart19(PCRReportParametersMultiple rp)
        {

            rp = FillRP(rp);
            rp.AgeGUID = rp.FindAge(rp.AgeGUID, rp.GenderGUID);
            rp.ProfileGUID = rp.FindProfile(rp.ProfileGUID, rp.GenderGUID);

            var categories = DbPCR.codeDutyStationsLanguages.Where(x => x.LanguageID == LAN && rp.DutyStationGUID.Contains(x.DutyStationGUID)).OrderBy(x => x.DutyStationDescription).Select(x => new { x.DutyStationDescription, x.DutyStationGUID }).ToArray();

            var Result =
               new Rainfall
               {
                   name = "Totals",
                   data = (from a in categories
                           join b in DbPCR.RP_PartnerCenter(
                       rp.EndDate,
                       rp.ReportGUID,
                       string.Join(",", rp.OrganizationInstanceGUID),
                       string.Join(",", rp.PartnerCenterGUID),
                       string.Join(",", new Guid[] { Guid.Parse("00000000-0000-0000-0000-000000000228") }),
                       string.Join(",", new Guid[] { Guid.Parse("00000000-0000-0000-0000-000000000229") }),
                       string.Join(",", rp.ReferralReasonGUID),
                       LAN).AsQueryable().Where(x => x.AggregationGUID != Aggregation.Total_Referral) on a.DutyStationGUID equals b.DutyStationGUID into LJ1
                           from R1 in LJ1.DefaultIfEmpty(new PCR_DAL.Model.RP_PartnerCenter_Result() { DutyStationGUID = default(Guid) })
                           group new { R1.AggregationValue } by new { a.DutyStationGUID, a.DutyStationDescription } into grp
                           orderby grp.Key.DutyStationDescription
                           select grp.Key.DutyStationGUID != Guid.Empty ? grp.Sum(v => v.AggregationValue) : 0


                   ).ToArray()
               };


            return Json(new { Result = Result, categories = categories.Select(x => x.DutyStationDescription).ToArray() }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult chart20(PCRReportParametersMultiple rp)
        {

            rp = FillRP(rp);
            rp.AgeGUID = rp.FindAge(rp.AgeGUID, rp.GenderGUID);
            rp.ProfileGUID = rp.FindProfile(rp.ProfileGUID, rp.GenderGUID);

            var categories = DbPCR.codeOrganizationsInstancesLanguages.Where(x => x.LanguageID == LAN && rp.OrganizationInstanceGUID.Contains(x.OrganizationInstanceGUID)).OrderBy(x => x.codeOrganizationsInstances.codeOrganizations.OrganizationShortName).Select(x => new { x.codeOrganizationsInstances.codeOrganizations.OrganizationShortName, x.OrganizationInstanceGUID }).ToArray();


            var Result =
               new Rainfall
               {
                   name = "Totals",
                   data = (from a in categories
                           join b in DbPCR.RP_PartnerCenter(
                       rp.EndDate,
                       rp.ReportGUID,
                       string.Join(",", rp.OrganizationInstanceGUID),
                       string.Join(",", rp.PartnerCenterGUID),
                       string.Join(",", new Guid[] { Guid.Parse("00000000-0000-0000-0000-000000000228") }),
                       string.Join(",", new Guid[] { Guid.Parse("00000000-0000-0000-0000-000000000229") }),
                       string.Join(",", rp.ReferralReasonGUID),
                       LAN).AsQueryable().Where(x => x.AggregationGUID != Aggregation.Total_Referral) on a.OrganizationInstanceGUID equals b.OrganizationInstanceGUID into LJ1
                           from R1 in LJ1.DefaultIfEmpty(new PCR_DAL.Model.RP_PartnerCenter_Result() { OrganizationInstanceGUID = default(Guid) })
                           group new { R1.AggregationValue } by new { a.OrganizationInstanceGUID, a.OrganizationShortName } into grp
                           orderby grp.Key.OrganizationShortName
                           select grp.Key.OrganizationInstanceGUID != Guid.Empty ? grp.Sum(v => v.AggregationValue) : 0


                   ).ToArray()
               };



            return Json(new { Result = Result, categories = categories.Select(x => x.OrganizationShortName).ToArray() }, JsonRequestBehavior.AllowGet);

        }
    }
    class Rainfall
    {
        public string name { set; get; }
        public int[] data { set; get; }
        public string color { set; get; }
    }
    class series
    {
        public string name { set; get; }
        public int y { get; set; }
    }
    class seriesData
    {
        public string name { set; get; }
        public int y { set; get; }
    }
    class data
    {
        public string name { set; get; }
        public string drilldown { set; get; }
        public string id { set; get; }
        public double y { set; get; }
    }
    class drilldown
    {
        public string name { set; get; }
        public string id { set; get; }
        public data[] data { get; set; }
    }
    public class Aggregation
    {
        public static Guid family = Guid.Parse("00000000-0000-0000-0000-000000000001");
        public static Guid Individuals = Guid.Parse("00000000-0000-0000-0000-000000000002");
        public static Guid Individuals_Male = Guid.Parse("00000000-0000-0000-0000-000000000003");
        public static Guid Individuals_Female = Guid.Parse("00000000-0000-0000-0000-000000000004");
        public static Guid Total_Referral = Guid.Parse("00000000-0000-0000-0000-000000000028");
    }
    public class ReportGUIDs
    {
        public static Guid Monthly = Guid.Parse("00000000-0000-0000-0000-000000000001");
        public static Guid Cumulative = Guid.Parse("00000000-0000-0000-0000-000000000002");
        public static Guid GeneralStatistics = Guid.Parse("00000000-0000-0000-0000-000000000003");
        public static Guid Referral = Guid.Parse("00000000-0000-0000-0000-000000000004");
    }

    public class CategoryReport
    {
        public static Guid TotalOfVulnerabilityCodesEntered = Guid.Parse("00000000-0000-0000-0000-000000000215");
        public static Guid NewRegistration = Guid.Parse("00000000-0000-0000-0000-000000000039");
        public static Guid Totalservicesprovided = Guid.Parse("00000000-0000-0000-0000-000000000218");
    }
}