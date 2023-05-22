using AppsPortal.BaseControllers;
using AppsPortal.Data;
using AppsPortal.Extensions;
using AppsPortal.Library;
using EMT_DAL.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.EMT.Controllers
{
    public class DashboardsController : EMTBaseController
    {
        class Rainfall
        {
            public string name { set; get; }
            public double[] data { set; get; }
        }
        class series
        {
            public string name { set; get; }
            public bool colorByPoint { get; set; }
            public DataSeries[] data { get; set; }
        }

        class seriesPie
        {
            public string name { set; get; }

            public double y { get; set; }
        }
        class DataSeries
        {
            public string name { set; get; }
            public string drilldown { set; get; }
            public double y { set; get; }
            public bool selected { get; set; }
            public bool sliced { get; set; }
            public bool colorByPoint { get; set; }
        }
        class drilldown
        {
            public string name { set; get; }
            public string id { set; get; }
            public bool colorByPoint { get; set; }
            public DataDrillDown[] data { get; set; }

            public string guid { get; set; }

            public dataLiner[] dataLiner { get; set; }
        }
        class DataDrillDown
        {
            public string name { set; get; }
            public double y { set; get; }
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

        class Months
        {
            public string Name { set; get; }
            public int MonthOrder { set; get; }

        }

        class AgeGroup
        {
            public int age { set; get; }
            public int y { set; get; }
        }

        // GET: EMT/Dashboards
        public ActionResult Index()
        {
            return View();
        }

        private List<ChartResult> RemoveZeroValue(double[] Values, string[] Keys)
        {
            List<ChartResult> results = new List<ChartResult>();
            for (int i = 0; i < Keys.Length; i++)
            {
                results.Add(new ChartResult(Keys[i], Values[i],null));
            }
            return results.Where(x => x.Value1 != 0).ToList();
        }
        private List<ChartResult> RemoveZeroMultiColumnValue(double[] Values1, double[] Values2, string[] Keys)
        {
            List<ChartResult> results = new List<ChartResult>();
            for (int i = 0; i < Keys.Length; i++)
            {
                results.Add(new ChartResult(Keys[i], Values1[i], Values2[i]));
            }
            return results.Where(x => x.Value1 != 0 || x.Value2 != 0).ToList();
        }
        private class HeatMapZeroFilter
        {
            public ArrayList fullArray { get; set; }
            public ArrayList KeyX { get; set; }
        }
        private HeatMapZeroFilter RemoveZeroValue(List<int>[] Values, string[] KeysX,string[] KeysY)
        {
            ArrayList KeysXAdd = new ArrayList();
            ArrayList fullArray = new ArrayList();
            for (int i = 0; i < KeysX.Length; i++)
            {
                var count = Values.Where(x => x[0] == i && x[2] != 0).Count();
                if (count > 0)
                {
                    KeysXAdd.Add(KeysX[i]);
                    var result = Values.Where(x => x[0] == i).ToList();
                    result.ForEach(x => x[0] = KeysXAdd.Count - 1);
                    fullArray.AddRange(result);
                }

            }
         
            return new HeatMapZeroFilter
            {
                fullArray = fullArray,
                KeyX = KeysXAdd
            };


        }

        class ChartResult
        {
            public ChartResult(string key, double value1, double? value2)
            {
                this.Key = key;
                this.Value1 = value1;
                this.Value2 = value2;
            }
            public string Key { get; set; }
            public double Value1 { get; set; }
            public double? Value2 { get; set; }
        }

        private EMTReportParametersList FillRP(EMTReportParametersList rp)
        {
            if (rp.StartDate == null) { rp.StartDate = new DateTime(2019, 1, 1); }
            if (rp.EndDate == null) { rp.EndDate = DateTime.Now; } else { rp.EndDate = rp.EndDate.Value.AddHours(23).AddMinutes(59); }
            if (rp.OrganizationInstanceGUID == null) { rp.OrganizationInstanceGUID = DropDownList.OrganizationsInstancesPharmacyAcronymByProfileAll().Select(x => x.Value).ToList().ConvertAll(Guid.Parse).ToArray(); }
            if (rp.MedicalPharmacyGUID == null) { rp.MedicalPharmacyGUID = DropDownList.PharmacyByOrganizationInstance(string.Join(",", rp.OrganizationInstanceGUID)).Select(x => x.Value).ToList().ConvertAll(Guid.Parse).ToArray(); }
            if (rp.Sequance == null) { rp.Sequance = DropDownList.MedicalItemsSequance().Select(x => x.Value).ToList().ConvertAll(int.Parse).ToArray(); }
            if (rp.MedicalPharmacologicalFormGUID == null) { rp.MedicalPharmacologicalFormGUID = DropDownList.LookupValues(LookupTables.MedicalPharmacologicalForm).Select(x => x.Value).ToList().ConvertAll(Guid.Parse).ToArray(); }

            return rp;
        }

        private Expression<Func<RP_ConsumptionMedicine_Result, bool>> MoreFilterConsumptionMedicine(EMTReportParametersList rp)
        {
            List<DataTableFilterOptions> filterOptions = new List<DataTableFilterOptions>();
            if (rp.MedicalBeneficiaryGUID != Guid.Empty) { filterOptions.Add(new DataTableFilterOptions() { Field = "MedicalBeneficiaryGUID", FieldData = rp.MedicalBeneficiaryGUID.ToString(), Operation = "ne" }); }
            Expression<Func<RP_ConsumptionMedicine_Result, bool>> Predicate = p => true;
            Predicate = SearchHelper.CreateSearchPredicate<RP_ConsumptionMedicine_Result>(new DataTableFilters() { FilterRules = filterOptions });
            return Predicate;
        }

        private Expression<Func<RP_PrescriptionsDispensed_Result, bool>> MoreFilterPrescriptionsDispensed(EMTReportParametersList rp)
        {
            List<DataTableFilterOptions> filterOptions = new List<DataTableFilterOptions>();
            if (rp.MedicalBeneficiaryGUID != Guid.Empty) { filterOptions.Add(new DataTableFilterOptions() { Field = "MedicalBeneficiaryGUID", FieldData = rp.MedicalBeneficiaryGUID.ToString(), Operation = "ne" }); }
            Expression<Func<RP_PrescriptionsDispensed_Result, bool>> Predicate = p => true;
            Predicate = SearchHelper.CreateSearchPredicate<RP_PrescriptionsDispensed_Result>(new DataTableFilters() { FilterRules = filterOptions });
            return Predicate;
        }

        ////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// The consumption of medicines at a certain period of time (week, two weeks, month, quarter …etc  in items ( packing unit and smallest units) 
        /// </summary>
        /// <param name="rp"> Report Parameters List</param>
        /// <returns></returns>
        public ActionResult ConsumptionMedicines(EMTReportParametersList rp)
        {
            rp = FillRP(rp);

            var RP_ConsumptionMedicine = ((List<RP_ConsumptionMedicine_Result>)Session["RP_ConsumptionMedicine"]).AsQueryable().Where(MoreFilterConsumptionMedicine(rp)).ToList();

            var categories = DbEMT.codeMedicalItem.Where(x =>  rp.Sequance.Contains(x.Sequance) &&x.Active).OrderBy(x => x.BrandName).Select(x => new { x.BrandName, x.MedicalItemGUID }).ToArray();
            var Result =
               new Rainfall
               {
                   name = "Consumption Of Medicines",
                   data = (from a in categories
                           join b in RP_ConsumptionMedicine
                            on a.MedicalItemGUID equals b.MedicalItemGUID into LJ1
                           from R1 in LJ1.DefaultIfEmpty(new RP_ConsumptionMedicine_Result() { MedicalItemGUID = default(Guid) })
                           group new { R1.QuantityByPackingUnit } by new { a.MedicalItemGUID, a.BrandName } into grp
                           orderby grp.Key.BrandName
                           select grp.Key.MedicalItemGUID != Guid.Empty ? grp.Sum(v => v.QuantityByPackingUnit) : 0
                   ).ToArray()
               };



            string[] cat = categories.Select(x => x.BrandName).ToArray();
            if (!rp.IncludeZero)
            {
                List<ChartResult> chartResult = RemoveZeroValue(Result.data, categories.Select(x => x.BrandName).ToArray());
                Result.data = chartResult.Select(x => x.Value1).ToArray();
                cat = chartResult.Select(x => x.Key).ToArray();
            }

            return Json(new { Result = Result, categories = cat }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// The consumption of medicines at a certain period of time (week, two weeks, month, quarter …etc  in items ( packing unit and smallest units) 
        /// </summary>
        /// <param name="rp"> Report Parameters List</param>
        /// <returns></returns>
        public ActionResult ConsumptionMedicinesByBeneficiariesType(EMTReportParametersList rp)
        {
            rp = FillRP(rp);
            var categories = new DropDownList().BeneficiaryType().OrderBy(x => x.Text).Select(x => new { x.Text, x.Value }).ToArray();
            var RP_ConsumptionMedicine = ((List<RP_ConsumptionMedicine_Result>)Session["RP_ConsumptionMedicine"]).AsQueryable().Where(MoreFilterConsumptionMedicine(rp)).ToList();
            var Result = 
               new Rainfall
               {
                   name = "Consumption Of Medicines By Beneficiaries Type",
                   data = (from a in categories
                           join b in RP_ConsumptionMedicine
                            on a.Value equals b.BeneficiaryTypeGUID.ToString() into LJ1
                           from R1 in LJ1.DefaultIfEmpty(new RP_ConsumptionMedicine_Result() { BeneficiaryTypeGUID = default(Guid)})
                           group new { R1.QuantityByPackingUnit } by new { R1.BeneficiaryTypeGUID , a.Text } into grp
                           orderby grp.Key.Text
                           select grp.Key.BeneficiaryTypeGUID != Guid.Empty ? grp.Sum(v => v.QuantityByPackingUnit) : 0
                   ).ToArray()
               };
            string[] cat = categories.Select(x => x.Text).ToArray();
            if (!rp.IncludeZero)
            {
                List<ChartResult> chartResult = RemoveZeroValue(Result.data, categories.Select(x => x.Text).ToArray());
                Result.data = chartResult.Select(x => x.Value1).ToArray();
                cat = chartResult.Select(x => x.Key).ToArray();
            }

            return Json(new { Result = Result, categories = cat }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// The Consumption Medicines By Beneficiaries Type Heat 
        /// Map at a certain period of time (week, two weeks, month, quarter …etc  in items ( packing unit and smallest units) 
        /// </summary>
        /// <param name="rp"> Report Parameters List</param>
        /// <returns></returns>
        public ActionResult ConsumptionMedicinesByBeneficiariesTypeHeatMap(EMTReportParametersList rp)
        {
            rp = FillRP(rp);
            var categoriesX = DbEMT.codeMedicalItem.Where(x => rp.Sequance.Contains(x.Sequance)).OrderBy(x => x.BrandName).Select(x => new { x.BrandName, x.MedicalItemGUID }).ToArray();
            var categoriesY = new DropDownList().BeneficiaryType().OrderBy(x => x.Text).Select(x => new { x.Text }).ToArray();
            string MedicalBeneficiaryGUID = rp.MedicalBeneficiaryGUID != Guid.Empty ? rp.MedicalBeneficiaryGUID.ToString() : null;
              List<int>[] results = (from a in DbEMT.RP_ConsumptionMedicineByBeneficiariesTypeHeatMap(
                               string.Join(",", rp.MedicalPharmacyGUID),
                               string.Join(",", rp.Sequance),
                               string.Join(",", rp.MedicalPharmacologicalFormGUID),
                               rp.StartDate, rp.EndDate , rp.MedicalBeneficiaryGUID != null ? rp.MedicalBeneficiaryGUID.ToString() : null , LAN)
                                     orderby a.BrandName, a.BeneficiaryType
                                     select new
                                     {
                                         a.BrandNameID,
                                         a.BeneficiaryTypeID,
                                        
                                         a.Total
                                     }).ToList().Select(x => new List<int>
                                      {
                                          x.BrandNameID,
                                          x.BeneficiaryTypeID,
                                          x.Total
                                      }).ToArray();

            HeatMapZeroFilter arrayList = RemoveZeroValue(results, categoriesX.Select(x => x.BrandName).ToArray(), categoriesY.Select(x => x.Text).ToArray());
          //  var _categoriesX = categoriesX.Select(x => x.BrandName).ToArray();




            return Json(new { Result = arrayList.fullArray, categoriesX = arrayList.KeyX, categoriesY=categoriesY.Select(x=>x.Text).ToArray() }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// The Consumption Medicines By Beneficiaries Type Heat 
        /// Map at a certain period of time (week, two weeks, month, quarter …etc  in items ( packing unit and smallest units) 
        /// </summary>
        /// <param name="rp"> Report Parameters List</param>
        /// <returns></returns>
        public ActionResult ConsumptionMedicinesByRouteAdministration(EMTReportParametersList rp)
        {
            rp = FillRP(rp);
            var categories = DbEMT.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active && x.codeTablesValues.TableGUID == LookupTables.MedicalRouteAdministration).ToList();
            var RP_ConsumptionMedicine = ((List<RP_ConsumptionMedicine_Result>)Session["RP_ConsumptionMedicine"]).AsQueryable().Where(MoreFilterConsumptionMedicine(rp)).ToList();

            var MainData = new series();
            if (rp.IncludeZero)
            {
                MainData = new series
                {
                    name = "Route Administration",
                    colorByPoint = true,
                    data = (from a in categories
                            join b in RP_ConsumptionMedicine on a.ValueGUID equals b.MedicalRouteAdministrationGUID into LJ1
                            from R1 in LJ1.DefaultIfEmpty(new RP_ConsumptionMedicine_Result() { MedicalRouteAdministrationGUID = default(Guid), QuantityByPackingUnit = 0 })
                            select new
                            {
                                a.ValueDescription,
                                R1.QuantityByPackingUnit
                            }
                                  ).GroupBy(x => x.ValueDescription).
                                  Select(x =>
                                      new DataSeries
                                      {
                                          colorByPoint = true,
                                          name = x.Key,
                                          drilldown = x.Key,
                                          y = x.Sum(y => y.QuantityByPackingUnit),
                                          //selected = true,
                                          sliced = true
                                      }
                                  ).ToArray()
                };
            }
            else
            {
                MainData = new series
                {
                    name = "Route Administration",
                    colorByPoint = true,
                    data = (from a in categories
                            join b in RP_ConsumptionMedicine on a.ValueGUID equals b.MedicalRouteAdministrationGUID 
                            select new
                            {
                                a.ValueDescription,
                                b.QuantityByPackingUnit
                            }
                                 ).GroupBy(x => x.ValueDescription).
                                 Select(x =>
                                     new DataSeries
                                     {
                                         colorByPoint = true,
                                         name = x.Key,
                                         drilldown = x.Key,
                                         y = x.Sum(y => y.QuantityByPackingUnit),
                                         //selected = true,
                                         sliced = true
                                     }
                                 ).ToArray()
                };
            }

            var DetailsData = (from a in categories
                               join b in RP_ConsumptionMedicine on a.ValueGUID equals b.MedicalRouteAdministrationGUID into LJ1
                               from R1 in LJ1.DefaultIfEmpty(new RP_ConsumptionMedicine_Result() { MedicalRouteAdministrationGUID = default(Guid), QuantityByPackingUnit = 0 })
                               group new { R1.QuantityByPackingUnit } by new { a.ValueDescription, R1.BrandName } into grp
                               select new drilldown()
                               {
                                   name = grp.Key.BrandName,
                                   id = grp.Key.ValueDescription,
                                   data = (from y in RP_ConsumptionMedicine
                                                                  .Where(x => x.MedicalRouteAdministration == grp.Key.ValueDescription)
                                                                  group new { y.QuantityByPackingUnit } by new { y.BrandName } into grpData
                                                                  select new DataDrillDown
                                                                  {
                                                                      name = grpData.Key.BrandName,
                                                                      y= grpData.Sum(v => v.QuantityByPackingUnit)
                                                                  }
                                           ).ToArray()
                               }
                               ).ToArray();
            return Json(new { MainReport = MainData, DetailsReport = DetailsData }, JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// The Consumption Medicines By PharmacologicalForm  
        /// Map at a certain period of time (week, two weeks, month, quarter …etc  in items ( packing unit and smallest units) 
        /// </summary>
        /// <param name="rp"> Report Parameters List</param>
        /// <returns></returns>
        public ActionResult ConsumptionMedicinesByClassification(EMTReportParametersList rp)
        {
            rp = FillRP(rp);
            var categories = new DropDownList().LookupValues(LookupTables.MedicalTreatment).ToList();
            var RP_ConsumptionMedicine = ((List<RP_ConsumptionMedicine_Result>)Session["RP_ConsumptionMedicine"]).AsQueryable().Where(MoreFilterConsumptionMedicine(rp)).ToList();
            var MainData = new series();
            if (rp.IncludeZero)
            {
                MainData = new series
                {
                    name = "Classification",
                    colorByPoint = true,
                    data = (from a in categories
                            join b in RP_ConsumptionMedicine on a.Value equals b.MedicalTreatmentGUID.ToString() into LJ1
                            from R1 in LJ1.DefaultIfEmpty(new RP_ConsumptionMedicine_Result() { MedicalTreatmentGUID = default(Guid), QuantityByPackingUnit = 0 })
                            select new
                            {
                                a.Text,
                                R1.QuantityByPackingUnit
                            }
                               ).GroupBy(x => x.Text).
                               Select(x =>
                                   new DataSeries
                                   {
                                       colorByPoint = true,
                                       name = x.Key,
                                       drilldown = x.Key,
                                       y = x.Sum(y => y.QuantityByPackingUnit),
                                       ////selected = true,
                                       sliced = true
                                   }
                               ).ToArray()
                };
            }
            else
            {
                MainData = new series
                {
                    name = "Classification",
                    colorByPoint = true,
                    data = (from a in categories
                            join b in RP_ConsumptionMedicine on a.Value equals b.MedicalTreatmentGUID.ToString() 
                            select new
                            {
                                a.Text,
                                b.QuantityByPackingUnit
                            }
                             ).GroupBy(x => x.Text).
                             Select(x =>
                                 new DataSeries
                                 {
                                     colorByPoint = true,
                                     name = x.Key,
                                     drilldown = x.Key,
                                     y = x.Sum(y => y.QuantityByPackingUnit),
                                     //selected = true,
                                     sliced = true
                                 }
                             ).ToArray()
                };
            }

            var DetailsData = (from a in categories
                               join b in RP_ConsumptionMedicine on a.Value equals b.MedicalTreatmentGUID.ToString() into LJ1
                               from R1 in LJ1.DefaultIfEmpty(new RP_ConsumptionMedicine_Result() { MedicalRouteAdministrationGUID = default(Guid), QuantityByPackingUnit = 0 })
                               group new { R1.QuantityByPackingUnit } by new { a.Text, R1.BrandName } into grp
                               select new drilldown()
                               {
                                   name = grp.Key.BrandName,
                                   id = grp.Key.Text,
                                   data = (from y in RP_ConsumptionMedicine
                                                                  .Where(x => x.ClassificationDescription == grp.Key.Text)
                                           group new { y.QuantityByPackingUnit } by new { y.BrandName } into grpData
                                           select new DataDrillDown
                                           {
                                               
                                               name = grpData.Key.BrandName,
                                               y = grpData.Sum(v => v.QuantityByPackingUnit)
                                           }
                                           ).ToArray()
                               }
                               ).ToArray();


            return Json(new { MainReport = MainData, DetailsReport = DetailsData }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// The Consumption Medicines By Pharmacy  
        /// Map at a certain period of time (week, two weeks, month, quarter …etc  in items ( packing unit and smallest units) 
        /// </summary>
        /// <param name="rp"> Report Parameters List</param>
        /// <returns></returns>
        public ActionResult ConsumptionMedicinesByPharmacy(EMTReportParametersList rp)
        {
            rp = FillRP(rp);
            var categories = DbEMT.codeMedicalPharmacyLanguage.Where(x => x.LanguageID == LAN && x.Active && !x.codeMedicalPharmacy.MainWarehouse).ToList();
            var RP_ConsumptionMedicine = ((List<RP_ConsumptionMedicine_Result>)Session["RP_ConsumptionMedicine"]).AsQueryable().Where(MoreFilterConsumptionMedicine(rp)).ToList();
            var MainData = new series();
            if (rp.IncludeZero)
            {
                MainData = new series
                {
                    name = "Pharmacy",
                    colorByPoint = true,
                    data = (from a in categories
                            join b in RP_ConsumptionMedicine on a.MedicalPharmacyGUID equals b.MedicalPharmacyGUID into LJ1
                            from R1 in LJ1.DefaultIfEmpty(new RP_ConsumptionMedicine_Result() { MedicalPharmacyGUID = default(Guid), QuantityByPackingUnit = 0 })
                            select new
                            {
                                a.MedicalPharmacyDescription,
                                R1.QuantityByPackingUnit
                            }
                               ).GroupBy(x => x.MedicalPharmacyDescription).
                               Select(x =>
                                   new DataSeries
                                   {
                                       colorByPoint = true,
                                       name = x.Key,
                                       drilldown = x.Key,
                                       y = x.Sum(y => y.QuantityByPackingUnit),
                                       //selected = true,
                                       sliced = true
                                   }
                               ).ToArray()
                };
            }
            else
            {
                MainData = new series
                {
                    name = "Pharmacy",
                    colorByPoint = true,
                    data = (from a in categories
                            join b in RP_ConsumptionMedicine on a.MedicalPharmacyGUID equals b.MedicalPharmacyGUID
                            select new
                            {
                                a.MedicalPharmacyDescription,
                                b.QuantityByPackingUnit
                            }
                              ).GroupBy(x => x.MedicalPharmacyDescription).
                              Select(x =>
                                  new DataSeries
                                  {
                                      colorByPoint = true,
                                      name = x.Key,
                                      drilldown = x.Key,
                                      y = x.Sum(y => y.QuantityByPackingUnit),
                                      //selected = true,
                                      sliced = true
                                  }
                              ).ToArray()
                };
            }

            var DetailsData = (from a in categories
                               join b in RP_ConsumptionMedicine on a.MedicalPharmacyGUID equals b.MedicalPharmacyGUID into LJ1
                               from R1 in LJ1.DefaultIfEmpty(new RP_ConsumptionMedicine_Result() { MedicalItemGUID = default(Guid), QuantityByPackingUnit = 0 })
                               group new { R1.QuantityByPackingUnit } by new { a.MedicalPharmacyDescription, R1.BrandName } into grp
                               select new drilldown()
                               {
                                   name = grp.Key.BrandName,
                                   id = grp.Key.MedicalPharmacyDescription,
                                   data = (from y in RP_ConsumptionMedicine
                                                                  .Where(x => x.MedicalPharmacyDescription == grp.Key.MedicalPharmacyDescription)
                                           group new { y.QuantityByPackingUnit } by new { y.BrandName } into grpData
                                           select new DataDrillDown
                                           {
                                               name = grpData.Key.BrandName,
                                               y = grpData.Sum(v => v.QuantityByPackingUnit)
                                           }
                                           ).ToArray()
                               }
                               ).ToArray();
            return Json(new { MainReport = MainData, DetailsReport = DetailsData }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// The Consumption Medicines By Gender  
        /// Map at a certain period of time (week, two weeks, month, quarter …etc  in items ( packing unit and smallest units) 
        /// </summary>
        /// <param name="rp"> Report Parameters List</param>
        /// <returns></returns>
        public ActionResult ConsumptionMedicinesByGender(EMTReportParametersList rp)
        {
            rp = FillRP(rp);
            var categories = DbEMT.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active && x.codeTablesValues.TableGUID==LookupTables.Gender).ToList();
            var RP_ConsumptionMedicine = ((List<RP_ConsumptionMedicine_Result>)Session["RP_ConsumptionMedicine"]).AsQueryable().Where(MoreFilterConsumptionMedicine(rp)).ToList();
            var MainData = new series();
            if (rp.IncludeZero)
            {
                MainData = new series
                {
                    name = "Gender",
                    colorByPoint = true,
                    data = (from a in categories
                            join b in RP_ConsumptionMedicine on a.ValueGUID equals b.GenderGUID into LJ1
                            from R1 in LJ1.DefaultIfEmpty(new RP_ConsumptionMedicine_Result() { GenderGUID = default(Guid), QuantityByPackingUnit = 0 })
                            select new
                            {
                                a.ValueDescription,
                                R1.QuantityByPackingUnit
                            }
                               ).GroupBy(x => x.ValueDescription).
                               Select(x =>
                                   new DataSeries
                                   {
                                       colorByPoint = true,
                                       name = x.Key,
                                       drilldown = x.Key,
                                       y = x.Sum(y => y.QuantityByPackingUnit),
                                       //selected = true,
                                       sliced = true
                                   }
                               ).ToArray()
                };
            }
            else
            {
                MainData = new series
                {
                    name = "Gender",
                    colorByPoint = true,
                    data = (from a in categories
                            join b in RP_ConsumptionMedicine on a.ValueGUID equals b.GenderGUID
                            select new
                            {
                                a.ValueDescription,
                                b.QuantityByPackingUnit
                            }
                              ).GroupBy(x => x.ValueDescription).
                              Select(x =>
                                  new DataSeries
                                  {
                                      colorByPoint = true,
                                      name = x.Key,
                                      drilldown = x.Key,
                                      y = x.Sum(y => y.QuantityByPackingUnit),
                                      //selected = true,
                                      sliced = true
                                  }
                              ).ToArray()
                };
            }


            var DetailsData = (from a in categories
                               join b in RP_ConsumptionMedicine on a.ValueGUID equals b.GenderGUID into LJ1
                               from R1 in LJ1.DefaultIfEmpty(new RP_ConsumptionMedicine_Result() { MedicalItemGUID = default(Guid), QuantityByPackingUnit = 0 })
                               group new { R1.QuantityByPackingUnit } by new { a.ValueDescription, R1.BrandName ,a.ValueGUID} into grp
                               select new drilldown()
                               {
                                   name = grp.Key.BrandName,
                                   id = grp.Key.ValueDescription,
                                   data = (from y in RP_ConsumptionMedicine
                                                                  .Where(x => x.GenderGUID == grp.Key.ValueGUID)
                                           group new { y.QuantityByPackingUnit } by new { y.BrandName } into grpData
                                           select new DataDrillDown
                                           {
                                               name = grpData.Key.BrandName,
                                               y = grpData.Sum(v => v.QuantityByPackingUnit)
                                           }
                                           ).ToArray()
                               }
                               ).ToArray();
            return Json(new { MainReport = MainData, DetailsReport = DetailsData }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Consumption Medicines Over Total Items
        /// </summary>
        /// <param name="rp"></param>
        /// <returns></returns>
        public ActionResult ConsumptionMedicinesOverTotalItems(EMTReportParametersList rp)
        {
            rp = FillRP(rp);
            var RP_Transfer = ((List<RP_TransferMedicine_Result>)Session["RP_TransferMedicine"]).AsQueryable().ToList();

            double QuantityTotal = RP_Transfer.Select(x => x.QuantityByPackingUnit).Sum();
            double RemainingQuantityTotal = RP_Transfer.Select(x => x.RemainingItems).Sum();

            seriesPie RemainingQuantity = new seriesPie() { name = "Remaining Quantity", y = (RemainingQuantityTotal / QuantityTotal) * 100 };
            seriesPie ConsumptionQuantity = new seriesPie() { name = "Consumption Quantity", y = ((QuantityTotal-RemainingQuantityTotal) / QuantityTotal) * 100 };
            List<seriesPie> Result = new List<seriesPie>();
            Result.Add(RemainingQuantity);
            Result.Add(ConsumptionQuantity);
            return Json(new { Result = Result.ToArray(), categories = Result.Select(x => x.name).ToArray() }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// The Consumption Medicines By Gender  
        /// Map at a certain period of time (week, two weeks, month, quarter …etc  in items ( packing unit and smallest units) 
        /// </summary>
        /// <param name="rp"> Report Parameters List</param>
        /// <returns></returns>
        public ActionResult ConsumptionMedicinesByAgeGroup(EMTReportParametersList rp)
        {
            rp = FillRP(rp);
            var RP_ConsumptionMedicine = ((List<RP_ConsumptionMedicine_Result>)Session["RP_ConsumptionMedicine"]).AsQueryable().Where(MoreFilterConsumptionMedicine(rp)).ToList();
            var Result = ( from a in RP_ConsumptionMedicine.Where(x=>x.Brithday!=null)
                            //get the date of the birthday this year
                            let age = (DateTime.Now-a.Brithday.Value ).Days/365

                            group new { a.QuantityByPackingUnit } by new {  age } into grp
                            select new ArrayList
                            {
                                grp.Key.age,
                                grp.Sum(x=>x.QuantityByPackingUnit)
                            }).ToArray();

            return Json(new { Result = Result }, JsonRequestBehavior.AllowGet);
        }



        ////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// The Consumption Medicine sCost By Pharmacy
        /// Map at a certain period of time (week, two weeks, month, quarter …etc  in items ( packing unit and smallest units) 
        /// </summary>
        /// <param name="rp"> Report Parameters List</param>
        /// <returns></returns>
        public ActionResult ConsumptionMedicinesCostByPharmacy(EMTReportParametersList rp)
        {
            bool CostViewAuthorization = DbCMS.userServiceHistory.Where(x => x.UserGUID == UserGUID).FirstOrDefault().EmailAddress.ToUpper().EndsWith("UNHCR.ORG");

            rp = FillRP(rp);
            var categories = DbEMT.codeMedicalPharmacyLanguage.Where(x => x.LanguageID == LAN && x.Active && !x.codeMedicalPharmacy.MainWarehouse).ToList();
            var RP_ConsumptionMedicine = ((List<RP_ConsumptionMedicine_Result>)Session["RP_ConsumptionMedicine"]).AsQueryable().Where(MoreFilterConsumptionMedicine(rp)).ToList();
            var MainData = new series();
            if (rp.IncludeZero)
            {
                MainData = new series
                {
                    name = "Pharmacy",
                    colorByPoint = true,
                    data = (from a in categories
                            join b in RP_ConsumptionMedicine on a.MedicalPharmacyGUID equals b.MedicalPharmacyGUID into LJ1
                            from R1 in LJ1.DefaultIfEmpty(new RP_ConsumptionMedicine_Result() { MedicalPharmacyGUID = default(Guid), Cost = 0 })
                            select new
                            {
                                a.MedicalPharmacyDescription,
                                Cost = R1.PriceOfPackingUnit * R1.QuantityByPackingUnit
                            }
                               ).GroupBy(x => x.MedicalPharmacyDescription).
                               Select(x =>
                                   new DataSeries
                                   {
                                       colorByPoint = true,
                                       name = x.Key,
                                       drilldown = x.Key,
                                       y =CostViewAuthorization? Math.Round( x.Sum(y => y.Cost),2):0,
                                       //selected = true,
                                       sliced = true
                                   }
                               ).ToArray()
                };
            }
            else
            {
                MainData = new series
                {
                    name = "Pharmacy",
                    colorByPoint = true,
                    data = (from a in categories
                            join b in RP_ConsumptionMedicine on a.MedicalPharmacyGUID equals b.MedicalPharmacyGUID
                            select new
                            {
                                a.MedicalPharmacyDescription,
                                Cost= b.PriceOfPackingUnit * b.QuantityByPackingUnit
                            }
                             ).GroupBy(x => x.MedicalPharmacyDescription).
                             Select(x =>
                                 new DataSeries
                                 {
                                     colorByPoint = true,
                                     name = x.Key,
                                     drilldown = x.Key,
                                     y = CostViewAuthorization? Math.Round(x.Sum(y => y.Cost), 2):0,
                                     //selected = true,
                                     sliced = true
                                 }
                             ).ToArray()
                };
            }


            var DetailsData = (from a in categories
                               join b in RP_ConsumptionMedicine on a.MedicalPharmacyGUID equals b.MedicalPharmacyGUID into LJ1
                               from R1 in LJ1.DefaultIfEmpty(new RP_ConsumptionMedicine_Result() { MedicalItemGUID = default(Guid), Cost = 0 })
                               group new { R1.QuantityByPackingUnit } by new { a.MedicalPharmacyDescription, R1.BrandName } into grp
                               select new drilldown()
                               {
                                   name = grp.Key.BrandName,
                                   id = grp.Key.MedicalPharmacyDescription,
                                   data = (from y in RP_ConsumptionMedicine
                                                                  .Where(x => x.MedicalPharmacyDescription == grp.Key.MedicalPharmacyDescription)
                                           group new { y.Cost } by new { y.BrandName } into grpData
                                           select new DataDrillDown
                                           {
                                               name = grpData.Key.BrandName,
                                               y =CostViewAuthorization? Math.Round(grpData.Sum(v => v.Cost.Value)):0
                                           }
                                           ).ToArray()
                               }
                               ).ToArray();
            return Json(new { MainReport = MainData, DetailsReport = DetailsData }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// The Consumption Medicine sCost By Partner
        /// Map at a certain period of time (week, two weeks, month, quarter …etc  in items ( packing unit and smallest units) 
        /// </summary>
        /// <param name="rp"> Report Parameters List</param>
        /// <returns></returns>
        public ActionResult ConsumptionMedicinesCostByPartner(EMTReportParametersList rp)
        {
            bool CostViewAuthorization = DbCMS.userServiceHistory.Where(x => x.UserGUID == UserGUID).FirstOrDefault().EmailAddress.ToUpper().EndsWith("UNHCR.ORG");

            rp = FillRP(rp);
            var categories = DbEMT.codeOrganizationsInstancesLanguages.Where(x => x.LanguageID == LAN && rp.OrganizationInstanceGUID.Contains(x.OrganizationInstanceGUID)).OrderBy(x => x.OrganizationInstanceDescription).Select(x => new { x.OrganizationInstanceDescription, x.OrganizationInstanceGUID }).ToArray();
            var RP_ConsumptionMedicine = ((List<RP_ConsumptionMedicine_Result>)Session["RP_ConsumptionMedicine"]).AsQueryable().Where(MoreFilterConsumptionMedicine(rp)).ToList();
            var MainData = new series();
            if (rp.IncludeZero)
            {
                MainData = new series
                {
                    name = "Partner",
                    colorByPoint = true,
                    data = (from a in categories
                            join b in RP_ConsumptionMedicine on a.OrganizationInstanceGUID equals b.OrganizationInstanceGUID into LJ1
                            from R1 in LJ1.DefaultIfEmpty(new RP_ConsumptionMedicine_Result() { OrganizationInstanceGUID = default(Guid), Cost = 0 })
                            select new
                            {
                                a.OrganizationInstanceDescription,
                                Cost = R1.PriceOfPackingUnit * R1.QuantityByPackingUnit
                            }
                               ).GroupBy(x => x.OrganizationInstanceDescription).
                               Select(x =>
                                   new DataSeries
                                   {
                                       colorByPoint = true,
                                       name = x.Key,
                                       drilldown = x.Key,
                                       y =CostViewAuthorization? Math.Round(x.Sum(y => y.Cost), 2):0,
                                       //selected = true,
                                       sliced = true
                                   }
                               ).ToArray()
                };
            }
            else
            {
                MainData = new series
                {
                    name = "Partner",
                    colorByPoint = true,
                    data = (from a in categories
                            join b in RP_ConsumptionMedicine on a.OrganizationInstanceGUID equals b.OrganizationInstanceGUID
                            select new
                            {
                                a.OrganizationInstanceDescription,
                                Cost = b.PriceOfPackingUnit * b.QuantityByPackingUnit
                            }
                             ).GroupBy(x => x.OrganizationInstanceDescription).
                             Select(x =>
                                 new DataSeries
                                 {
                                     colorByPoint = true,
                                     name = x.Key,
                                     drilldown = x.Key,
                                     y =CostViewAuthorization? Math.Round(x.Sum(y => y.Cost), 2):0,
                                     //selected = true,
                                     sliced = true
                                 }
                             ).ToArray()
                };
            }

            var DetailsData = (from a in categories
                               join b in RP_ConsumptionMedicine on a.OrganizationInstanceGUID equals b.OrganizationInstanceGUID into LJ1
                               from R1 in LJ1.DefaultIfEmpty(new RP_ConsumptionMedicine_Result() { MedicalItemGUID = default(Guid), Cost = 0 })
                               group new { R1.QuantityByPackingUnit } by new { a.OrganizationInstanceDescription, a.OrganizationInstanceGUID, R1.BrandName } into grp
                               select new drilldown()
                               {
                                   name = grp.Key.BrandName,
                                   id = grp.Key.OrganizationInstanceDescription,
                                   data = (from y in RP_ConsumptionMedicine
                                                                  .Where(x => x.OrganizationInstanceGUID == grp.Key.OrganizationInstanceGUID)
                                           group new { y.Cost } by new { y.BrandName } into grpData
                                           select new DataDrillDown
                                           {
                                               name = grpData.Key.BrandName,
                                               y =CostViewAuthorization? Math.Round(grpData.Sum(v => v.Cost.Value),2):0
                                           }
                                           ).ToArray()
                               }
                               ).ToArray();
            return Json(new { MainReport = MainData, DetailsReport = DetailsData }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Consumption Medicines Cost Per Months
        /// </summary>
        /// <param name="rp"></param>
        /// <returns></returns>
        public ActionResult ConsumptionMedicinesCostPerMonths(EMTReportParametersList rp)
        {
            bool CostViewAuthorization = DbCMS.userServiceHistory.Where(x => x.UserGUID == UserGUID).FirstOrDefault().EmailAddress.ToUpper().EndsWith("UNHCR.ORG");

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
            var categories = DbEMT.codeMedicalPharmacyLanguage.Where(x => x.LanguageID == LAN && x.Active && !x.codeMedicalPharmacy.MainWarehouse).ToList();
            var RP_ConsumptionMedicine = ((List<RP_ConsumptionMedicine_Result>)Session["RP_ConsumptionMedicine"]).AsQueryable().Where(MoreFilterConsumptionMedicine(rp)).ToList();

            var Result = (from a in RP_ConsumptionMedicine
                          join c in categories on a.MedicalPharmacyGUID equals c.MedicalPharmacyGUID
                          group new
                          {
                              c.MedicalPharmacyDescription
                          } by new { c.MedicalPharmacyDescription, c.MedicalPharmacyGUID } into G
                          select new drilldownLiner
                          {
                              name = G.Key.MedicalPharmacyDescription,
                              guid = G.Key.MedicalPharmacyGUID.ToString()
                          }

                                  ).ToList();

            Result.ForEach(r => r.data = (from a in MonthNames
                                          join b in RP_ConsumptionMedicine.Where(x => x.MedicalPharmacyGUID.ToString().ToString() == r.guid) on a.MonthOrder equals b.DeliveryDate.Month into LJ1
                                          from R1 in LJ1.DefaultIfEmpty(new RP_ConsumptionMedicine_Result() { MedicalPharmacyGUID = default(Guid) })
                                          group new { Cost=R1.PriceOfPackingUnit * R1.QuantityByPackingUnit } by new { a.Name, R1.MedicalPharmacyGUID, a.MonthOrder } into grp
                                          orderby grp.Key.MonthOrder
                                          select new dataLiner
                                          {
                                              y =CostViewAuthorization? (grp.Key.MedicalPharmacyGUID != Guid.Empty ? (int) Math.Round(grp.Sum(x => x.Cost ),2) : 0):0
                                          }).ToArray()
                                      );


            return Json(new { Result = Result, categories = MonthNames.Select(x => x.Name).ToArray() }, JsonRequestBehavior.AllowGet);
        }


        /////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Number Of Prescriptions Dispensed By Pharmacy
        /// </summary>
        /// <param name="rp"> Report Parameters List</param>
        /// <returns></returns>
        public ActionResult NumberOfPrescriptionsDispensedByPharmacy(EMTReportParametersList rp)
        {
            rp = FillRP(rp);
            var categories = DbEMT.codeMedicalPharmacyLanguage.Where(x => x.LanguageID == LAN && rp.MedicalPharmacyGUID.Contains(x.MedicalPharmacyGUID) && !x.codeMedicalPharmacy.MainWarehouse).OrderBy(x => x.MedicalPharmacyDescription).Select(x => new { x.MedicalPharmacyDescription, x.MedicalPharmacyGUID }).ToArray();
            //dataMedicalBeneficiaryItemOut.Where(MoreFilterBeneficiaryItemOut(rp)).Where(x => x.DeliveryDate >= rp.StartDate && x.DeliveryDate <= rp.EndDate && x.Active && ! x.codeMedicalPharmacy.MainWarehouse).Where(x=> rp.MedicalPharmacyGUID.Contains(x.MedicalPharmacyGUID))
            var RP_PrescriptionsDispensed = ((List<RP_PrescriptionsDispensed_Result>) Session["RP_PrescriptionsDispensed"]).AsQueryable().Where(x => !x.MainWarehouse).Where(MoreFilterPrescriptionsDispensed(rp)).ToList();

            var Result =
               new Rainfall
               {
                   name = "Number Of Prescriptions Dispensed by Pharmacies",
                   data = (from a in categories
                           join b in RP_PrescriptionsDispensed
                            on a.MedicalPharmacyGUID equals b.MedicalPharmacyGUID into LJ1
                           from R1 in LJ1.DefaultIfEmpty(new RP_PrescriptionsDispensed_Result() { MedicalPharmacyGUID = default(Guid) })
                           group new { R1.MedicalPharmacyGUID } by new { R1.MedicalPharmacyGUID, a.MedicalPharmacyDescription } into grp
                           orderby grp.Key.MedicalPharmacyDescription
                           select grp.Key.MedicalPharmacyGUID != Guid.Empty ? Convert.ToDouble(grp.Count()) : 0


                   ).ToArray()
               };
            string[] cat = categories.Select(x => x.MedicalPharmacyDescription).ToArray();
            if (!rp.IncludeZero)
            {
                List<ChartResult> chartResult = RemoveZeroValue(Result.data, categories.Select(x => x.MedicalPharmacyDescription).ToArray());
                Result.data = chartResult.Select(x => x.Value1).ToArray();
                cat = chartResult.Select(x => x.Key).ToArray();
            }

            return Json(new { Result = Result, categories = cat }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Number Of Prescriptions Dispensed By Organizations Instances
        /// </summary>
        /// <param name="rp"></param>
        /// <returns></returns>
        public ActionResult NumberOfPrescriptionsDispensedByOrganizationsInstances(EMTReportParametersList rp)
        {
            rp = FillRP(rp);
            var categories = DbEMT.codeOrganizationsInstancesLanguages.Where(x => x.LanguageID == LAN && rp.OrganizationInstanceGUID.Contains(x.OrganizationInstanceGUID)).OrderBy(x => x.OrganizationInstanceDescription).Select(x => new { x.OrganizationInstanceDescription, x.OrganizationInstanceGUID }).ToArray();
            var RP_PrescriptionsDispensed = ((List<RP_PrescriptionsDispensed_Result>) Session["RP_PrescriptionsDispensed"]).AsQueryable().Where(x => !x.MainWarehouse).Where(MoreFilterPrescriptionsDispensed(rp)).ToList();

            var Result =
               new Rainfall
               {
                   name = "Number Of Prescriptions Dispensed by Partner",
                   data = (from a in categories
                           join b in RP_PrescriptionsDispensed
                            on a.OrganizationInstanceGUID equals b.OrganizationInstanceGUID into LJ1
                           from R1 in LJ1.DefaultIfEmpty(new RP_PrescriptionsDispensed_Result() { OrganizationInstanceGUID = default(Guid) })

                           group new { R1.OrganizationInstanceGUID } by new { R1.OrganizationInstanceGUID, a.OrganizationInstanceDescription } into grp
                           orderby grp.Key.OrganizationInstanceDescription
                           select grp.Key.OrganizationInstanceGUID != Guid.Empty ? Convert.ToDouble(grp.Count()) : 0


                   ).ToArray()
               };
            string[] cat = categories.Select(x => x.OrganizationInstanceDescription).ToArray();
            if (!rp.IncludeZero)
            {
                List<ChartResult> chartResult = RemoveZeroValue(Result.data, categories.Select(x => x.OrganizationInstanceDescription).ToArray());
                Result.data = chartResult.Select(x => x.Value1).ToArray();
                cat = chartResult.Select(x => x.Key).ToArray();
            }

            return Json(new { Result = Result, categories = cat }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Number Of Prescriptions Dispensed By Beneficiary Type
        /// </summary>
        /// <param name="rp"> Report Parameters List</param>
        /// <returns></returns>
        public ActionResult NumberOfPrescriptionsDispensedByBeneficiaryType(EMTReportParametersList rp)
        {
            rp = FillRP(rp);
            var categories = new DropDownList().BeneficiaryType().OrderBy(x => x.Text).Select(x => new { x.Text, x.Value }).ToArray();
            //dataMedicalBeneficiaryItemOut.Where(MoreFilterBeneficiaryItemOut(rp)).Where(x => x.DeliveryDate >= rp.StartDate && x.DeliveryDate <= rp.EndDate && x.Active && ! x.codeMedicalPharmacy.MainWarehouse).Where(x=> rp.MedicalPharmacyGUID.Contains(x.MedicalPharmacyGUID))
            var RP_PrescriptionsDispensed = ((List<RP_PrescriptionsDispensed_Result>) Session["RP_PrescriptionsDispensed"]).AsQueryable().Where(x => !x.MainWarehouse).Where(MoreFilterPrescriptionsDispensed(rp)).ToList();

            var Result =
               new Rainfall
               {
                   name = "Number Of Prescriptions Dispensed By Beneficiary Type",
                   data = (from a in categories
                           join b in RP_PrescriptionsDispensed
                            on a.Value equals b.BeneficiaryTypeGUID.ToString() into LJ1
                           from R1 in LJ1.DefaultIfEmpty(new RP_PrescriptionsDispensed_Result() { BeneficiaryTypeGUID = default(Guid) })
                           group new { R1.BeneficiaryTypeGUID } by new { R1.BeneficiaryTypeGUID, a.Text } into grp
                           orderby grp.Key.Text
                           select grp.Key.BeneficiaryTypeGUID != Guid.Empty ? Convert.ToDouble(grp.Count()) : 0


                   ).ToArray()
               };
            string[] cat = categories.Select(x => x.Text).ToArray();
            if (!rp.IncludeZero)
            {
                List<ChartResult> chartResult = RemoveZeroValue(Result.data, categories.Select(x => x.Text).ToArray());
                Result.data = chartResult.Select(x => x.Value1).ToArray();
                cat = chartResult.Select(x => x.Key).ToArray();
            }

            return Json(new { Result = Result, categories = cat }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Number Of Prescriptions Dispensed Per Months
        /// </summary>
        /// <param name="rp"></param>
        /// <returns></returns>
        public ActionResult NumberOfPrescriptionsDispensedPerMonths(EMTReportParametersList rp)
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
            var categories = new DropDownList().BeneficiaryType().OrderBy(x => x.Text).Select(x => new { x.Text, x.Value }).ToArray();
            var RP_PrescriptionsDispensed = ((List<RP_PrescriptionsDispensed_Result>) Session["RP_PrescriptionsDispensed"]).AsQueryable().Where(x => !x.MainWarehouse).Where(MoreFilterPrescriptionsDispensed(rp)).ToList();

            var Result = (from a in RP_PrescriptionsDispensed
                                    join c in categories on a.BeneficiaryTypeGUID.ToString() equals c.Value
                                    group new
                                    {
                                        c.Text
                                    } by new { c.Text, c.Value } into G
                                    select new drilldownLiner
                                    {
                                        name = G.Key.Text,
                                        guid = G.Key.Value
                                    }

                                  ).ToList();

            Result.ForEach(r => r.data = (from a in MonthNames
                                                     join b in RP_PrescriptionsDispensed.Where(x =>  x.BeneficiaryTypeGUID.ToString() == r.guid) on a.MonthOrder equals b.DeliveryDate.Month into LJ1
                                                from R1 in LJ1.DefaultIfEmpty(new RP_PrescriptionsDispensed_Result() { BeneficiaryTypeGUID = default(Guid) })
                                                group new { R1.BeneficiaryTypeGUID, a.Name, } by new { a.Name, R1.BeneficiaryTypeGUID, a.MonthOrder } into grp
                                                orderby grp.Key.MonthOrder
                                                select new dataLiner
                                                {
                                                    y = grp.Key.BeneficiaryTypeGUID != Guid.Empty ? grp.Count() : 0
                                                }).ToArray()
                                      );


            return Json(new { Result = Result, categories = MonthNames.Select(x=>x.Name).ToArray() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult NumberOfPrescriptionsDiseaseTypePerMonths(EMTReportParametersList rp)
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
            var categories = new DropDownList().DiseaseType().OrderBy(x => x.Text).Select(x => new { x.Text, x.Value }).ToArray();
            var RP_PrescriptionsDispensed = ((List<RP_PrescriptionsDispensed_Result>) Session["RP_PrescriptionsDispensed"]).AsQueryable().Where(x => !x.MainWarehouse).Where(MoreFilterPrescriptionsDispensed(rp)).ToList();

            var Result = (from a in RP_PrescriptionsDispensed
                          join c in categories on a.DiseaseTypeGUID.ToString() equals c.Value
                          group new
                          {
                              c.Text
                          } by new { c.Text, c.Value } into G
                          select new drilldownLiner
                          {
                              name = G.Key.Text,
                              guid = G.Key.Value
                          }

                                  ).ToList();

            Result.ForEach(r => r.data = (from a in MonthNames
                                          join b in RP_PrescriptionsDispensed.Where(x => x.DiseaseTypeGUID.ToString() == r.guid) on a.MonthOrder equals b.DeliveryDate.Month into LJ1
                                          from R1 in LJ1.DefaultIfEmpty(new RP_PrescriptionsDispensed_Result() { DiseaseTypeGUID = default(Guid) })
                                          group new { R1.DiseaseTypeGUID, a.Name, } by new { a.Name, R1.DiseaseTypeGUID, a.MonthOrder } into grp
                                          orderby grp.Key.MonthOrder
                                          select new dataLiner
                                          {
                                              y = grp.Key.DiseaseTypeGUID != Guid.Empty ? grp.Count() : 0
                                          }).ToArray()
                                      );


            return Json(new { Result = Result, categories = MonthNames.Select(x => x.Name).ToArray() }, JsonRequestBehavior.AllowGet);
        }


        /////////////////////////////////////////////////////////////////////////////////////
        ///
        /// 
        /// 
        /// Dispatched
        /// 
        /// 
        /// 
        /// <summary>
        /// The Dispatched of medicines at a certain period of time (week, two weeks, month, quarter …etc  in items ( packing unit and smallest units) 
        /// </summary>
        /// <param name="rp"> Report Parameters List</param>
        /// <returns></returns>
        public ActionResult DispatchedMedicines(EMTReportParametersList rp)
        {
            rp = FillRP(rp);

            var RP_DispatchedMedicine = DbEMT.RP_DispatchedMedicine(
                                      string.Join(",", rp.MedicalPharmacyGUID),
                                      string.Join(",", rp.Sequance),
                                      string.Join(",", rp.MedicalPharmacologicalFormGUID),
                                      rp.StartDate, rp.EndDate, LAN).ToList();

            var categories = DbEMT.codeMedicalItem.Where(x => rp.Sequance.Contains(x.Sequance)).OrderBy(x => x.BrandName).Select(x => new { x.BrandName, x.MedicalItemGUID }).ToArray();
            var DispatchedQuantity =
               new Rainfall
               {
                   name = "Dispatched Quantity",
                   data = (from a in categories
                           join b in RP_DispatchedMedicine
                            on a.MedicalItemGUID equals b.MedicalItemGUID into LJ1
                           from R1 in LJ1.DefaultIfEmpty(new RP_DispatchedMedicine_Result() { MedicalItemGUID = default(Guid) })
                           group new { R1.QuantityByPackingUnit } by new { a.MedicalItemGUID, a.BrandName } into grp
                           orderby grp.Key.BrandName
                           select grp.Key.MedicalItemGUID != Guid.Empty ? Convert.ToDouble( grp.Sum(v => v.QuantityByPackingUnit)): 0
                   ).ToArray()
               };
            var RemainingQuantity =
              new Rainfall
              {
                  name = "Remaining Quantity",
                  data = (from a in categories
                          join b in RP_DispatchedMedicine
                           on a.MedicalItemGUID equals b.MedicalItemGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty(new RP_DispatchedMedicine_Result() { MedicalItemGUID = default(Guid),RemainingItems=0 })
                          group new { R1.RemainingItems } by new { a.MedicalItemGUID, a.BrandName } into grp
                          orderby grp.Key.BrandName
                          select grp.Key.MedicalItemGUID != Guid.Empty ? Convert.ToDouble(grp.Sum(v => v.RemainingItems.Value)) : 0
                  ).ToArray()
              };


            string[] cat = categories.Select(x => x.BrandName).ToArray();
            if (!rp.IncludeZero)
            {
                List<ChartResult> chartResult = RemoveZeroMultiColumnValue(RemainingQuantity.data, DispatchedQuantity.data, categories.Select(x => x.BrandName).ToArray());
                RemainingQuantity.data = chartResult.Select(x => x.Value1).ToArray();
                DispatchedQuantity.data = chartResult.Select(x => x.Value2.Value).ToArray();
                cat = chartResult.Select(x => x.Key).ToArray();
            }
           

            return Json(new { DispatchedQuantity = DispatchedQuantity, RemainingQuantity=RemainingQuantity, categories = cat}, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// The Dispatched Medicines By Route Administration  
        /// Map at a certain period of time (week, two weeks, month, quarter …etc  in items ( packing unit and smallest units) 
        /// </summary>
        /// <param name="rp"> Report Parameters List</param>
        /// <returns></returns>
        public ActionResult DispatchedMedicinesByRouteAdministration(EMTReportParametersList rp)
        {
            rp = FillRP(rp);
            var categories = DbEMT.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active && x.codeTablesValues.TableGUID == LookupTables.MedicalRouteAdministration).ToList();
            var RP_DispatchedMedicine = DbEMT.RP_DispatchedMedicine(
                                                string.Join(",", rp.MedicalPharmacyGUID),
                                                string.Join(",", rp.Sequance),
                                                string.Join(",", rp.MedicalPharmacologicalFormGUID),
                                                rp.StartDate, rp.EndDate, LAN).ToList();
            var MainData = new series();
            if (rp.IncludeZero)
            {
                MainData = new series
                {
                    name = "Route Administration",
                    colorByPoint = true,
                    data = (from a in categories
                            join b in RP_DispatchedMedicine on a.ValueGUID equals b.MedicalRouteAdministrationGUID into LJ1
                            from R1 in LJ1.DefaultIfEmpty(new RP_DispatchedMedicine_Result() { MedicalRouteAdministrationGUID = default(Guid), QuantityByPackingUnit = 0 })
                            select new
                            {
                                a.ValueDescription,
                                R1.QuantityByPackingUnit
                            }
                               ).GroupBy(x => x.ValueDescription).
                               Select(x =>
                                   new DataSeries
                                   {
                                       colorByPoint = true,
                                       name = x.Key,
                                       drilldown = x.Key,
                                       y = x.Sum(y => y.QuantityByPackingUnit),
                                       //selected = true,
                                       sliced = true
                                   }
                               ).ToArray()
                };
            }
            else
            {
                MainData = new series
                {
                    name = "Route Administration",
                    colorByPoint = true,
                    data = (from a in categories
                            join b in RP_DispatchedMedicine on a.ValueGUID equals b.MedicalRouteAdministrationGUID 
                            select new
                            {
                                a.ValueDescription,
                                b.QuantityByPackingUnit
                            }
               ).GroupBy(x => x.ValueDescription).
               Select(x =>
                   new DataSeries
                   {
                       colorByPoint = true,
                       name = x.Key,
                       drilldown = x.Key,
                       y = x.Sum(y => y.QuantityByPackingUnit),
                       //selected = true,
                       sliced = true
                   }
               ).ToArray()
                };
            }


            var DetailsData = (from a in categories
                               join b in RP_DispatchedMedicine on a.ValueGUID equals b.MedicalRouteAdministrationGUID into LJ1
                               from R1 in LJ1.DefaultIfEmpty(new RP_DispatchedMedicine_Result() { MedicalRouteAdministrationGUID = default(Guid), QuantityByPackingUnit = 0 })
                               group new { R1.QuantityByPackingUnit } by new { a.ValueDescription, R1.BrandName } into grp
                               select new drilldown()
                               {
                                   name = grp.Key.BrandName,
                                   id = grp.Key.ValueDescription,
                                   data = (from y in RP_DispatchedMedicine
                                                                  .Where(x => x.MedicalRouteAdministration == grp.Key.ValueDescription)
                                           group new { y.QuantityByPackingUnit } by new { y.BrandName } into grpData
                                           select new DataDrillDown
                                           {
                                               name = grpData.Key.BrandName,
                                               y = grpData.Sum(v => v.QuantityByPackingUnit)
                                           }
                                           ).ToArray()
                               }
                               ).ToArray();
            return Json(new { MainReport = MainData, DetailsReport = DetailsData }, JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// The Dispatched Medicines By PharmacologicalForm  
        /// Map at a certain period of time (week, two weeks, month, quarter …etc  in items ( packing unit and smallest units) 
        /// </summary>
        /// <param name="rp"> Report Parameters List</param>
        /// <returns></returns>
        public ActionResult DispatchedMedicinesByPharmacologicalForm(EMTReportParametersList rp)
        {
            rp = FillRP(rp);
            var categories = DropDownList.LookupValues(LookupTables.MedicalPharmacologicalForm).ToList();
            var RP_DispatchedMedicine = DbEMT.RP_DispatchedMedicine(
                                                string.Join(",", rp.MedicalPharmacyGUID),
                                                string.Join(",", rp.Sequance),
                                                string.Join(",", rp.MedicalPharmacologicalFormGUID),
                                                rp.StartDate, rp.EndDate, LAN).AsQueryable().ToList();
            var MainData = new series();
            if (rp.IncludeZero)
            {
                MainData = new series
                {
                    name = "PharmacologicalForm",
                    colorByPoint = true,
                    data = (from a in categories
                            join b in RP_DispatchedMedicine on a.Value equals b.MedicalPharmacologicalFormGUID.ToString() into LJ1
                            from R1 in LJ1.DefaultIfEmpty(new RP_DispatchedMedicine_Result() { MedicalRouteAdministrationGUID = default(Guid), QuantityByPackingUnit = 0 })
                            select new
                            {
                                a.Text,
                                R1.QuantityByPackingUnit
                            }
                               ).GroupBy(x => x.Text).
                               Select(x =>
                                   new DataSeries
                                   {
                                       colorByPoint = true,
                                       name = x.Key,
                                       drilldown = x.Key,
                                       y = x.Sum(y => y.QuantityByPackingUnit),
                                       //selected = true,
                                       sliced = true
                                   }
                               ).ToArray()
                };
            }
            else
            {
                MainData = new series
                {
                    name = "PharmacologicalForm",
                    colorByPoint = true,
                    data = (from a in categories
                            join b in RP_DispatchedMedicine on a.Value equals b.MedicalPharmacologicalFormGUID.ToString()
                            select new
                            {
                                a.Text,
                                b.QuantityByPackingUnit
                            }
                              ).GroupBy(x => x.Text).
                              Select(x =>
                                  new DataSeries
                                  {
                                      colorByPoint = true,
                                      name = x.Key,
                                      drilldown = x.Key,
                                      y = x.Sum(y => y.QuantityByPackingUnit),
                                      //selected = true,
                                      sliced = true
                                  }
                              ).ToArray()
                };
            }


            var DetailsData = (from a in categories
                               join b in RP_DispatchedMedicine on a.Value equals b.MedicalPharmacologicalFormGUID.ToString() into LJ1
                               from R1 in LJ1.DefaultIfEmpty(new RP_DispatchedMedicine_Result() { MedicalRouteAdministrationGUID = default(Guid), QuantityByPackingUnit = 0 })
                               group new { R1.QuantityByPackingUnit } by new { a.Text, R1.BrandName } into grp
                               select new drilldown()
                               {
                                   name = grp.Key.BrandName,
                                   id = grp.Key.Text,
                                   data = (from y in RP_DispatchedMedicine
                                                                  .Where(x => x.PharmacologicalFormDescription == grp.Key.Text)
                                           group new { y.QuantityByPackingUnit } by new { y.BrandName } into grpData
                                           select new DataDrillDown
                                           {
                                               name = grpData.Key.BrandName,
                                               y = grpData.Sum(v => v.QuantityByPackingUnit)
                                           }
                                           ).ToArray()
                               }
                               ).ToArray();
            return Json(new { MainReport = MainData, DetailsReport = DetailsData }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// The Dispatched Medicines By Pharmacy  
        /// Map at a certain period of time (week, two weeks, month, quarter …etc  in items ( packing unit and smallest units) 
        /// </summary>
        /// <param name="rp"> Report Parameters List</param>
        /// <returns></returns>
        public ActionResult DispatchedMedicinesByPharmacy(EMTReportParametersList rp)
        {
            rp = FillRP(rp);
            var categories = DbEMT.codeMedicalPharmacyLanguage.Where(x => x.LanguageID == LAN && x.Active && x.codeMedicalPharmacy.MainWarehouse).ToList();
            var RP_DispatchedMedicine = DbEMT.RP_DispatchedMedicine(
                                                string.Join(",", rp.MedicalPharmacyGUID),
                                                string.Join(",", rp.Sequance),
                                                string.Join(",", rp.MedicalPharmacologicalFormGUID),
                                                rp.StartDate, rp.EndDate, LAN).AsQueryable().ToList();
            var MainData = new series();
            if (rp.IncludeZero)
            {
                MainData = new series
                {
                    name = "Pharmacy",
                    colorByPoint = true,
                    data = (from a in categories
                            join b in RP_DispatchedMedicine on a.MedicalPharmacyGUID equals b.MedicalPharmacyGUID into LJ1
                            from R1 in LJ1.DefaultIfEmpty(new RP_DispatchedMedicine_Result() { MedicalPharmacyGUID = default(Guid), QuantityByPackingUnit = 0 })
                            select new
                            {
                                a.MedicalPharmacyDescription,
                                R1.QuantityByPackingUnit
                            }
                               ).GroupBy(x => x.MedicalPharmacyDescription).
                               Select(x =>
                                   new DataSeries
                                   {
                                       colorByPoint = true,
                                       name = x.Key,
                                       drilldown = x.Key,
                                       y = x.Sum(y => y.QuantityByPackingUnit),
                                       //selected = true,
                                       sliced = true
                                   }
                               ).ToArray()
                };
            }
            else
            {
                MainData = new series
                {
                    name = "Pharmacy",
                    colorByPoint = true,
                    data = (from a in categories
                            join b in RP_DispatchedMedicine on a.MedicalPharmacyGUID equals b.MedicalPharmacyGUID 
                            select new
                            {
                                a.MedicalPharmacyDescription,
                                b.QuantityByPackingUnit
                            }
                             ).GroupBy(x => x.MedicalPharmacyDescription).
                             Select(x =>
                                 new DataSeries
                                 {
                                     colorByPoint = true,
                                     name = x.Key,
                                     drilldown = x.Key,
                                     y = x.Sum(y => y.QuantityByPackingUnit),
                                     //selected = true,
                                     sliced = true
                                 }
                             ).ToArray()
                };
            }


            var DetailsData = (from a in categories
                               join b in RP_DispatchedMedicine on a.MedicalPharmacyGUID equals b.MedicalPharmacyGUID into LJ1
                               from R1 in LJ1.DefaultIfEmpty(new RP_DispatchedMedicine_Result() { MedicalItemGUID = default(Guid), QuantityByPackingUnit = 0 })
                               group new { R1.QuantityByPackingUnit } by new { a.MedicalPharmacyDescription, R1.BrandName } into grp
                               select new drilldown()
                               {
                                   name = grp.Key.BrandName,
                                   id = grp.Key.MedicalPharmacyDescription,
                                   data = (from y in RP_DispatchedMedicine
                                                                  .Where(x => x.MedicalPharmacyDescription == grp.Key.MedicalPharmacyDescription)
                                           group new { y.QuantityByPackingUnit } by new { y.BrandName } into grpData
                                           select new DataDrillDown
                                           {
                                               name = grpData.Key.BrandName,
                                               y = grpData.Sum(v => v.QuantityByPackingUnit)
                                           }
                                           ).ToArray()
                               }
                               ).ToArray();
            return Json(new { MainReport = MainData, DetailsReport = DetailsData }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// The Consumption Medicine sCost By Pharmacy
        /// Map at a certain period of time (week, two weeks, month, quarter …etc  in items ( packing unit and smallest units) 
        /// </summary>
        /// <param name="rp"> Report Parameters List</param>
        /// <returns></returns>
        public ActionResult DispatchedMedicinesCostByPharmacy(EMTReportParametersList rp)
        {
            rp = FillRP(rp);
            var categories = DbEMT.codeMedicalPharmacyLanguage.Where(x => x.LanguageID == LAN && x.Active && x.codeMedicalPharmacy.MainWarehouse).ToList();
            var RP_DispatchedMedicine = DbEMT.RP_DispatchedMedicine(
                                                string.Join(",", rp.MedicalPharmacyGUID),
                                                string.Join(",", rp.Sequance),
                                                string.Join(",", rp.MedicalPharmacologicalFormGUID),
                                                rp.StartDate, rp.EndDate, LAN).AsQueryable().ToList();
            var MainData = new series();
            if (rp.IncludeZero)
            {
                MainData = new series
                {
                    name = "Pharmacy",
                    colorByPoint = true,
                    data = (from a in categories
                            join b in RP_DispatchedMedicine on a.MedicalPharmacyGUID equals b.MedicalPharmacyGUID into LJ1
                            from R1 in LJ1.DefaultIfEmpty(new RP_DispatchedMedicine_Result() { MedicalPharmacyGUID = default(Guid) })
                            select new
                            {
                                a.MedicalPharmacyDescription,
                                Cost = R1.QuantityByPackingUnit * R1.PriceOfPackingUnit
                            }
                               ).GroupBy(x => x.MedicalPharmacyDescription).
                               Select(x =>
                                   new DataSeries
                                   {
                                       colorByPoint = true,
                                       name = x.Key,
                                       drilldown = x.Key,
                                       y = x.Sum(y => y.Cost),
                                       //selected = true,
                                       sliced = true
                                   }
                               ).ToArray()
                };
            }
            else
            {
                MainData = new series
                {
                    name = "Pharmacy",
                    colorByPoint = true,
                    data = (from a in categories
                            join b in RP_DispatchedMedicine on a.MedicalPharmacyGUID equals b.MedicalPharmacyGUID 
                            select new
                            {
                                a.MedicalPharmacyDescription,
                                Cost = b.QuantityByPackingUnit * b.PriceOfPackingUnit
                            }
                              ).GroupBy(x => x.MedicalPharmacyDescription).
                              Select(x =>
                                  new DataSeries
                                  {
                                      colorByPoint = true,
                                      name = x.Key,
                                      drilldown = x.Key,
                                      y = x.Sum(y => y.Cost),
                                      //selected = true,
                                      sliced = true
                                  }
                              ).ToArray()
                };
            }


            var DetailsData = (from a in categories
                               join b in RP_DispatchedMedicine on a.MedicalPharmacyGUID equals b.MedicalPharmacyGUID into LJ1
                               from R1 in LJ1.DefaultIfEmpty(new RP_DispatchedMedicine_Result() { MedicalItemGUID = default(Guid) })
                               group new { R1.QuantityByPackingUnit } by new { a.MedicalPharmacyDescription, R1.BrandName } into grp
                               select new drilldown()
                               {
                                   name = grp.Key.BrandName,
                                   id = grp.Key.MedicalPharmacyDescription,
                                   data = (from y in RP_DispatchedMedicine
                                                                  .Where(x => x.MedicalPharmacyDescription == grp.Key.MedicalPharmacyDescription)
                                           group new { Cost =  y.PriceOfPackingUnit  * y.QuantityByPackingUnit } by new { y.BrandName } into grpData
                                           select new DataDrillDown
                                           {
                                               name = grpData.Key.BrandName,
                                               y = grpData.Sum(v => v.Cost)
                                           }
                                           ).ToArray()
                               }
                               ).ToArray();
            return Json(new { MainReport = MainData, DetailsReport = DetailsData }, JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// The Dispatched Medicine sCost By Partner
        /// Map at a certain period of time (week, two weeks, month, quarter …etc  in items ( packing unit and smallest units) 
        /// </summary>
        /// <param name="rp"> Report Parameters List</param>
        /// <returns></returns>
        public ActionResult DispatchedMedicinesCostByPartner(EMTReportParametersList rp)
        {
            rp = FillRP(rp);
            var categories = DbEMT.codeOrganizationsInstancesLanguages.Where(x => x.LanguageID == LAN && rp.OrganizationInstanceGUID.Contains(x.OrganizationInstanceGUID)).OrderBy(x => x.OrganizationInstanceDescription).Select(x => new { x.OrganizationInstanceDescription, x.OrganizationInstanceGUID }).ToArray();
            var RP_DispatchedMedicine = DbEMT.RP_DispatchedMedicine(
                                                string.Join(",", rp.MedicalPharmacyGUID),
                                                string.Join(",", rp.Sequance),
                                                string.Join(",", rp.MedicalPharmacologicalFormGUID),
                                                rp.StartDate, rp.EndDate, LAN).AsQueryable().ToList();

            var MainData = new series();
            if (rp.IncludeZero)
            {
                MainData = new series
                {
                    name = "Partner",
                    colorByPoint = true,
                    data = (from a in categories
                            join b in RP_DispatchedMedicine on a.OrganizationInstanceGUID equals b.OrganizationInstanceGUID into LJ1
                            from R1 in LJ1.DefaultIfEmpty(new RP_DispatchedMedicine_Result() { OrganizationInstanceGUID = default(Guid) })
                            select new
                            {
                                a.OrganizationInstanceDescription,
                                Cost = R1.QuantityByPackingUnit * R1.PriceOfPackingUnit
                            }
                               ).GroupBy(x => x.OrganizationInstanceDescription).
                               Select(x =>
                                   new DataSeries
                                   {
                                       colorByPoint = true,
                                       name = x.Key,

                                       drilldown = x.Key,
                                       y = x.Sum(y => y.Cost),
                                       //selected = true,
                                       sliced = true
                                   }
                               ).ToArray()
                };
            }
            else
            {
                MainData = new series
                {
                    name = "Partner",
                    colorByPoint = true,
                    data = (from a in categories
                            join b in RP_DispatchedMedicine on a.OrganizationInstanceGUID equals b.OrganizationInstanceGUID
                            select new
                            {
                                a.OrganizationInstanceDescription,
                                Cost = b.QuantityByPackingUnit * b.PriceOfPackingUnit
                            }
                              ).GroupBy(x => x.OrganizationInstanceDescription).
                              Select(x =>
                                  new DataSeries
                                  {
                                      colorByPoint = true,
                                      name = x.Key,

                                      drilldown = x.Key,
                                      y = x.Sum(y => y.Cost),
                                      //selected = true,
                                      sliced = true
                                  }
                              ).ToArray()
                };
            }

            var DetailsData = (from a in categories
                               join b in RP_DispatchedMedicine on a.OrganizationInstanceGUID equals b.OrganizationInstanceGUID into LJ1
                               from R1 in LJ1.DefaultIfEmpty(new RP_DispatchedMedicine_Result() { MedicalItemGUID = default(Guid) })
                               group new { R1.QuantityByPackingUnit } by new { a.OrganizationInstanceDescription, a.OrganizationInstanceGUID, R1.BrandName } into grp
                               select new drilldown()
                               {
                                   name = grp.Key.BrandName,
                                   id = grp.Key.OrganizationInstanceDescription,
                                   data = (from y in RP_DispatchedMedicine
                                                                  .Where(x => x.OrganizationInstanceGUID == grp.Key.OrganizationInstanceGUID)
                                           group new { Cost = y.PriceOfPackingUnit * y.QuantityByPackingUnit  } by new { y.BrandName } into grpData
                                           select new DataDrillDown
                                           {
                                               name = grpData.Key.BrandName,
                                               y = grpData.Sum(v => v.Cost)
                                           }
                                           ).ToArray()
                               }
                               ).ToArray();
            return Json(new { MainReport = MainData, DetailsReport = DetailsData }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// The Dispatched Medicines By Pharmacy  
        /// Map at a certain period of time (week, two weeks, month, quarter …etc  in items ( packing unit and smallest units) 
        /// </summary>
        /// <param name="rp"> Report Parameters List</param>
        /// <returns></returns>
        public ActionResult DispatchedMedicinesByPartner(EMTReportParametersList rp)
        {
            rp = FillRP(rp);
            var categories = DbEMT.codeOrganizationsInstancesLanguages.Where(x => x.LanguageID == LAN && rp.OrganizationInstanceGUID.Contains(x.OrganizationInstanceGUID)).OrderBy(x => x.OrganizationInstanceDescription).Select(x => new { x.OrganizationInstanceDescription, x.OrganizationInstanceGUID }).ToArray();
            var RP_DispatchedMedicine = DbEMT.RP_DispatchedMedicine(
                                                string.Join(",", rp.MedicalPharmacyGUID),
                                                string.Join(",", rp.Sequance),
                                                string.Join(",", rp.MedicalPharmacologicalFormGUID),
                                                rp.StartDate, rp.EndDate, LAN).AsQueryable().ToList();
            var MainData = new series();
            if (rp.IncludeZero)
            {
                MainData = new series
                {
                    name = "Pharmacy",
                    colorByPoint = true,
                    data = (from a in categories
                            join b in RP_DispatchedMedicine on a.OrganizationInstanceGUID equals b.OrganizationInstanceGUID into LJ1
                            from R1 in LJ1.DefaultIfEmpty(new RP_DispatchedMedicine_Result() { MedicalPharmacyGUID = default(Guid), QuantityByPackingUnit = 0 })
                            select new
                            {
                                a.OrganizationInstanceDescription,
                                R1.QuantityByPackingUnit
                            }
                               ).GroupBy(x => x.OrganizationInstanceDescription).
                               Select(x =>
                                   new DataSeries
                                   {
                                       colorByPoint = true,
                                       name = x.Key,
                                       drilldown = x.Key,
                                       y = x.Sum(y => y.QuantityByPackingUnit),
                                       //selected = true,
                                       sliced = true
                                   }
                               ).ToArray()
                };
            }
            else
            {
                MainData = new series
                {
                    name = "Pharmacy",
                    colorByPoint = true,
                    data = (from a in categories
                            join b in RP_DispatchedMedicine on a.OrganizationInstanceGUID equals b.OrganizationInstanceGUID 
                            select new
                            {
                                a.OrganizationInstanceDescription,
                                b.QuantityByPackingUnit
                            }
                              ).GroupBy(x => x.OrganizationInstanceDescription).
                              Select(x =>
                                  new DataSeries
                                  {
                                      colorByPoint = true,
                                      name = x.Key,
                                      drilldown = x.Key,
                                      y = x.Sum(y => y.QuantityByPackingUnit),
                                      //selected = true,
                                      sliced = true
                                  }
                              ).ToArray()
                };
            }

            var DetailsData = (from a in categories
                               join b in RP_DispatchedMedicine on a.OrganizationInstanceGUID equals b.OrganizationInstanceGUID into LJ1
                               from R1 in LJ1.DefaultIfEmpty(new RP_DispatchedMedicine_Result() { MedicalItemGUID = default(Guid), QuantityByPackingUnit = 0 })
                               group new { R1.QuantityByPackingUnit } by new { a.OrganizationInstanceDescription, R1.BrandName,a.OrganizationInstanceGUID } into grp
                               select new drilldown()
                               {
                                   name = grp.Key.BrandName,
                                   id = grp.Key.OrganizationInstanceDescription,
                                   data = (from y in RP_DispatchedMedicine
                                                                  .Where(x => x.OrganizationInstanceGUID == grp.Key.OrganizationInstanceGUID)
                                           group new { y.QuantityByPackingUnit } by new { y.BrandName } into grpData
                                           select new DataDrillDown
                                           {
                                               name = grpData.Key.BrandName,
                                               y = grpData.Sum(v => v.QuantityByPackingUnit)
                                           }
                                           ).ToArray()
                               }
                               ).ToArray();
            return Json(new { MainReport = MainData, DetailsReport = DetailsData }, JsonRequestBehavior.AllowGet);

        }


        ////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// The Transfer Medicines By Pharmacy  
        /// Map at a certain period of time (week, two weeks, month, quarter …etc  in items ( packing unit and smallest units) 
        /// </summary>
        /// <param name="rp"> Report Parameters List</param>
        /// <returns></returns>
        public ActionResult TransferMedicinesByPharmacy(EMTReportParametersList rp)
        {
            rp = FillRP(rp);
            var categories = DbEMT.codeMedicalPharmacyLanguage.Where(x => x.LanguageID == LAN && x.Active && ! x.codeMedicalPharmacy.MainWarehouse).ToList();
            var RP_TransferMedicine = ((List<RP_TransferMedicine_Result>)Session["RP_TransferMedicine"]).AsQueryable().ToList();
            var MainData = new series();
            if (rp.IncludeZero)
            {
                MainData = new series
                {
                    name = "Pharmacy",
                    colorByPoint = true,
                    data = (from a in categories
                            join b in RP_TransferMedicine on a.MedicalPharmacyGUID equals b.MedicalPharmacyGUID into LJ1
                            from R1 in LJ1.DefaultIfEmpty(new RP_TransferMedicine_Result() { MedicalPharmacyGUID = default(Guid), QuantityByPackingUnit = 0 })
                            select new
                            {
                                a.MedicalPharmacyDescription,
                                R1.QuantityByPackingUnit
                            }
                               ).GroupBy(x => x.MedicalPharmacyDescription).
                               Select(x =>
                                   new DataSeries
                                   {
                                       colorByPoint = true,
                                       name = x.Key,
                                       drilldown = x.Key,
                                       y = x.Sum(y => y.QuantityByPackingUnit),
                                       //selected = true,
                                       sliced = true
                                   }
                               ).ToArray()
                };
            }
            else
            {
                MainData = new series
                {
                    name = "Pharmacy",
                    colorByPoint = true,
                    data = (from a in categories
                            join b in RP_TransferMedicine on a.MedicalPharmacyGUID equals b.MedicalPharmacyGUID 
                            select new
                            {
                                a.MedicalPharmacyDescription,
                                b.QuantityByPackingUnit
                            }
                              ).GroupBy(x => x.MedicalPharmacyDescription).
                              Select(x =>
                                  new DataSeries
                                  {
                                      colorByPoint = true,
                                      name = x.Key,
                                      drilldown = x.Key,
                                      y = x.Sum(y => y.QuantityByPackingUnit),
                                      //selected = true,
                                      sliced = true
                                  }
                              ).ToArray()
                };
            }

            var DetailsData = (from a in categories
                               join b in RP_TransferMedicine on a.MedicalPharmacyGUID equals b.MedicalPharmacyGUID into LJ1
                               from R1 in LJ1.DefaultIfEmpty(new RP_TransferMedicine_Result() { MedicalItemGUID = default(Guid), QuantityByPackingUnit = 0 })
                               group new { R1.QuantityByPackingUnit } by new { a.MedicalPharmacyDescription, R1.BrandName } into grp
                               select new drilldown()
                               {
                                   name = grp.Key.BrandName,
                                   id = grp.Key.MedicalPharmacyDescription,
                                   data = (from y in RP_TransferMedicine
                                                                  .Where(x => x.MedicalPharmacyDescription == grp.Key.MedicalPharmacyDescription)
                                           group new { y.QuantityByPackingUnit } by new { y.BrandName } into grpData
                                           select new DataDrillDown
                                           {
                                               name = grpData.Key.BrandName,
                                               y = grpData.Sum(v => v.QuantityByPackingUnit)
                                           }
                                           ).ToArray()
                               }
                               ).ToArray();

            
            return Json(new { MainReport = MainData, DetailsReport = DetailsData }, JsonRequestBehavior.AllowGet);

        }
        
        /// <summary>
        /// The Consumption Medicine sCost By Pharmacy
        /// Map at a certain period of time (week, two weeks, month, quarter …etc  in items ( packing unit and smallest units) 
        /// </summary>
        /// <param name="rp"> Report Parameters List</param>
        /// <returns></returns>
        public ActionResult TransferMedicinesRemainingItemsByPharmacy(EMTReportParametersList rp)
        {
            rp = FillRP(rp);
            var categories = DbEMT.codeMedicalPharmacyLanguage.Where(x => x.LanguageID == LAN && x.Active && !x.codeMedicalPharmacy.MainWarehouse).ToList();
            var RP_TransferMedicine = ((List<RP_TransferMedicine_Result>)Session["RP_TransferMedicine"]).AsQueryable().ToList();
            var MainData = new series();
            if (rp.IncludeZero)
            {
                MainData = new series
                {
                    name = "Pharmacy",
                    colorByPoint = true,
                    data = (from a in categories
                            join b in RP_TransferMedicine on a.MedicalPharmacyGUID equals b.MedicalPharmacyGUID into LJ1
                            from R1 in LJ1.DefaultIfEmpty(new RP_TransferMedicine_Result() { MedicalPharmacyGUID = default(Guid), QuantityByPackingUnit = 0 })
                            select new
                            {
                                a.MedicalPharmacyDescription,
                                R1.RemainingItems
                            }
                               ).GroupBy(x => x.MedicalPharmacyDescription).
                               Select(x =>
                                   new DataSeries
                                   {
                                       colorByPoint = true,
                                       name = x.Key,
                                       drilldown = x.Key,
                                       y = x.Sum(y => y.RemainingItems),
                                       //selected = true,
                                       sliced = true
                                   }
                               ).ToArray()
                };
            }
            else
            {
                MainData = new series
                {
                    name = "Pharmacy",
                    colorByPoint = true,
                    data = (from a in categories
                            join b in RP_TransferMedicine on a.MedicalPharmacyGUID equals b.MedicalPharmacyGUID 
                            select new
                            {
                                a.MedicalPharmacyDescription,
                                b.RemainingItems
                            }
                             ).GroupBy(x => x.MedicalPharmacyDescription).
                             Select(x =>
                                 new DataSeries
                                 {
                                     colorByPoint = true,
                                     name = x.Key,
                                     drilldown = x.Key,
                                     y = x.Sum(y => y.RemainingItems),
                                     //selected = true,
                                     sliced = true
                                 }
                             ).ToArray()
                };
            }

            var DetailsData = (from a in categories
                               join b in RP_TransferMedicine on a.MedicalPharmacyGUID equals b.MedicalPharmacyGUID into LJ1
                               from R1 in LJ1.DefaultIfEmpty(new RP_TransferMedicine_Result() { MedicalItemGUID = default(Guid), QuantityByPackingUnit = 0 })
                               group new { R1.QuantityByPackingUnit } by new { a.MedicalPharmacyDescription, R1.BrandName } into grp
                               select new drilldown()
                               {
                                   name = grp.Key.BrandName,
                                   id = grp.Key.MedicalPharmacyDescription,
                                   data = (from y in RP_TransferMedicine
                                                                  .Where(x => x.MedicalPharmacyDescription == grp.Key.MedicalPharmacyDescription)
                                           group new { y.RemainingItems } by new { y.BrandName } into grpData
                                           select new DataDrillDown
                                           {
                                               name = grpData.Key.BrandName,
                                               y = grpData.Sum(v => v.RemainingItems)
                                           }
                                           ).ToArray()
                               }
                               ).ToArray();
            return Json(new { MainReport = MainData, DetailsReport = DetailsData }, JsonRequestBehavior.AllowGet);

        }


        ////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Total Number Of Beneficiaries
        /// </summary>
        /// <param name="rp"> Report Parameters List</param>
        /// <returns></returns>
        public ActionResult TotalNumberOfBeneficiaries(EMTReportParametersList rp)
        {
            List<RP_PrescriptionsDispensed_Result> RP_PrescriptionsDispensed = ((List<RP_PrescriptionsDispensed_Result>) Session["RP_PrescriptionsDispensed"]);

            double[] val =new double[] { (from a in RP_PrescriptionsDispensed select a.MedicalBeneficiaryGUID).Count() };
            var Result =
               new Rainfall
               {
                   name = "Total Number Of Beneficiaries",
                   data =  val
               };
            return Json(new { Result = Result, categories = new List<string>() { "Total Number Of Beneficiaries" } }, JsonRequestBehavior.AllowGet);
        }

       

        /// <summary>
        /// Total Number Of Beneficiaries By Gender
        /// </summary>
        /// <param name="rp"> Report Parameters List</param>
        /// <returns></returns>
        public ActionResult TotalNumberOfBeneficiariesByGenderPercentage(EMTReportParametersList rp)
        {
            rp = FillRP(rp);
            var categories = DbEMT.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active && x.codeTablesValues.TableGUID == LookupTables.Gender).ToList();
            var RP_PrescriptionsDispensed = ((List<RP_PrescriptionsDispensed_Result>) Session["RP_PrescriptionsDispensed"]).AsQueryable().Where(x => !x.MainWarehouse).Where(MoreFilterPrescriptionsDispensed(rp))
                .Select(x=> 
             new
             {
                 x.MedicalBeneficiaryGUID,
                 x.GenderGUID
             }
            ).Distinct().ToList();

            double TotalNumberOfBeneficiaries = RP_PrescriptionsDispensed.Select(x => x.MedicalBeneficiaryGUID).Count();
            var Result = (from a in categories
                          join b in RP_PrescriptionsDispensed
                           on a.ValueGUID equals b.GenderGUID 
                          group new { a.ValueGUID , b.MedicalBeneficiaryGUID} by new { a.ValueGUID, a.ValueDescription } into grp

                          select
                          new seriesPie
                          {
                              name = grp.Key.ValueDescription,
                              y = (Convert.ToDouble(grp.Count())/ TotalNumberOfBeneficiaries) * 100
                          }

                  ).ToArray();
           
            return Json(new { Result = Result.ToArray(), categories = Result.Select(x => x.name).ToArray() }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Total Number Of Beneficiaries By Nationality Percentage
        /// </summary>
        /// <param name="rp"> Report Parameters List</param>
        /// <returns></returns>
        public ActionResult TotalNumberOfBeneficiariesByNationalityPercentage(EMTReportParametersList rp)
        {
            rp = FillRP(rp);
            var categories = DbEMT.codeCountriesLanguages.Where(x => x.LanguageID == LAN && x.codeCountries.Active ).ToList();
            var RP_PrescriptionsDispensed = ((List<RP_PrescriptionsDispensed_Result>) Session["RP_PrescriptionsDispensed"]).AsQueryable().Where(x => !x.MainWarehouse).Where(MoreFilterPrescriptionsDispensed(rp)).Select(x =>
             new
             {
                 x.MedicalBeneficiaryGUID,
                 x.NationalityCode
             }
            ).Distinct().ToList();

            double TotalNumberOfBeneficiaries = RP_PrescriptionsDispensed.Select(x => x.MedicalBeneficiaryGUID).Count();
            var Result = (from a in categories
                          join b in RP_PrescriptionsDispensed
                           on a.codeCountries.CountryA3Code equals b.NationalityCode
                          group new { a.codeCountries.CountryA3Code, b.MedicalBeneficiaryGUID } by new { a.codeCountries.CountryA3Code, a.CountryDescription } into grp

                          select
                          new seriesPie
                          {
                              name = grp.Key.CountryDescription,
                              y = (Convert.ToDouble(grp.Count()) / TotalNumberOfBeneficiaries) * 100
                          }

                  ).OrderBy(x=>x.y).ToArray();

            return Json(new { Result = Result.ToArray(), categories = Result.Select(x => x.name).ToArray() }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Total Number Of Beneficiaries By Nationality Percentage
        /// </summary>
        /// <param name="rp"> Report Parameters List</param>
        /// <returns></returns>
        public ActionResult TotalNumberOfBeneficiariesByAgeGroupPercentage(EMTReportParametersList rp)
        {
            rp = FillRP(rp);
            var RP_PrescriptionsDispensed = ((List<RP_PrescriptionsDispensed_Result>) Session["RP_PrescriptionsDispensed"]).AsQueryable().Where(x => !x.MainWarehouse).Where(MoreFilterPrescriptionsDispensed(rp)).Select(x =>
             new
             {
                 x.MedicalBeneficiaryGUID,
                 x.Brithday
             }
            ).Distinct().ToList();

            double TotalNumberOfBeneficiaries = RP_PrescriptionsDispensed.Select(x => x.MedicalBeneficiaryGUID).Count();
            var Result = (from a in RP_PrescriptionsDispensed.Where(x=>x.Brithday!=null)
                         
                          let age = (DateTime.Now - a.Brithday.Value).Days / 365
                          let AgeGroup = age <=5 ?"[0-5]": age>5&&age<=17 ? "[5-17]": age > 17 && age <= 59 ? "[18-59]" : "[60+]"
                          group new {  a.MedicalBeneficiaryGUID } by new {  AgeGroup } into grp
                          
                          select
                          new seriesPie
                          {
                              name = grp.Key.AgeGroup,
                              y = (Convert.ToDouble(grp.Count()) / TotalNumberOfBeneficiaries) * 100
                          }

                  ).OrderBy(x=>x.name).ToArray();

            return Json(new { Result = Result.ToArray(), categories = Result.Select(x => x.name).OrderBy(x => x).ToArray() }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Total Number Of Beneficiaries By Disease Percentage
        /// </summary>
        /// <param name="rp"> Report Parameters List</param>
        /// <returns></returns>
        public ActionResult TotalNumberOfBeneficiariesByDiseasePercentage(EMTReportParametersList rp)
        {
            rp = FillRP(rp);
            var categories = new DropDownList().DiseaseType().OrderBy(x => x.Text).Select(x => new { x.Text, x.Value }).ToArray();
            var RP_PrescriptionsDispensed = ((List<RP_PrescriptionsDispensed_Result>) Session["RP_PrescriptionsDispensed"]).AsQueryable().Where(x => !x.MainWarehouse).Where(MoreFilterPrescriptionsDispensed(rp)).Select(x =>
             new
             {
                 x.MedicalBeneficiaryGUID,
                 x.DiseaseTypeGUID
             }
            ).Distinct().ToList();

            double TotalNumberOfBeneficiaries = RP_PrescriptionsDispensed.Select(x => x.MedicalBeneficiaryGUID).Distinct().Count();
            var Result = (from a in categories
                          join b in RP_PrescriptionsDispensed
                           on a.Value equals b.DiseaseTypeGUID.ToString()
                          group new { a.Value, b.MedicalBeneficiaryGUID } by new { a.Text } into grp

                          select
                          new seriesPie
                          {
                              name = grp.Key.Text,
                              y = (Convert.ToDouble(grp.Count()) / TotalNumberOfBeneficiaries) * 100
                          }

                  ).ToArray();

            return Json(new { Result = Result.ToArray(), categories = Result.Select(x => x.name).ToArray() }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Total Number Of Beneficiaries By Partner Percentage
        /// </summary>
        /// <param name="rp"> Report Parameters List</param>
        /// <returns></returns>
        public ActionResult TotalNumberOfBeneficiariesByPartnerPercentage(EMTReportParametersList rp)
        {
            rp = FillRP(rp);
            var categories = DbEMT.codeOrganizationsInstancesLanguages.Where(x => x.LanguageID == LAN && rp.OrganizationInstanceGUID.Contains(x.OrganizationInstanceGUID)).OrderBy(x => x.OrganizationInstanceDescription).Select(x => new { x.OrganizationInstanceDescription, x.OrganizationInstanceGUID }).ToArray();
            var RP_PrescriptionsDispensed = ((List<RP_PrescriptionsDispensed_Result>) Session["RP_PrescriptionsDispensed"]).AsQueryable().Where(x => !x.MainWarehouse).Where(MoreFilterPrescriptionsDispensed(rp)).Select(x =>
             new
             {
                 x.MedicalBeneficiaryGUID,
                 x.OrganizationInstanceGUID
             }
            ).Distinct().ToList();

            double TotalNumberOfBeneficiaries = RP_PrescriptionsDispensed.Select(x => x.MedicalBeneficiaryGUID).Count();
            var Result = (from a in categories
                          join b in RP_PrescriptionsDispensed
                           on a.OrganizationInstanceGUID equals b.OrganizationInstanceGUID
                          group new { a.OrganizationInstanceGUID, b.MedicalBeneficiaryGUID } by new { a.OrganizationInstanceDescription } into grp

                          select
                          new seriesPie
                          {
                              name = grp.Key.OrganizationInstanceDescription,
                              y = (Convert.ToDouble(grp.Count()) / TotalNumberOfBeneficiaries) * 100
                          }

                  ).ToArray();

            return Json(new { Result = Result.ToArray(), categories = Result.Select(x => x.name).ToArray() }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Total Number Of Beneficiaries By Pharmacy Percentage
        /// </summary>
        /// <param name="rp"> Report Parameters List</param>
        /// <returns></returns>
        public ActionResult TotalNumberOfBeneficiariesByPharmacyPercentage(EMTReportParametersList rp)
        {
            rp = FillRP(rp);
            var categories = DbEMT.codeMedicalPharmacyLanguage.Where(x => x.LanguageID == LAN && x.Active && !x.codeMedicalPharmacy.MainWarehouse).ToList();
            var RP_PrescriptionsDispensed = ((List<RP_PrescriptionsDispensed_Result>) Session["RP_PrescriptionsDispensed"]).AsQueryable().Where(x => !x.MainWarehouse).Where(MoreFilterPrescriptionsDispensed(rp)).Select(x =>
             new
             {
                 x.MedicalBeneficiaryGUID,
                 x.MedicalPharmacyGUID
             }
            ).Distinct().ToList();

            double TotalNumberOfBeneficiaries = RP_PrescriptionsDispensed.Select(x => x.MedicalBeneficiaryGUID).Count();
            var Result = (from a in categories
                          join b in RP_PrescriptionsDispensed
                           on a.MedicalPharmacyGUID equals b.MedicalPharmacyGUID
                          group new { a.MedicalPharmacyGUID } by new { a.MedicalPharmacyDescription } into grp

                          select
                          new seriesPie
                          {
                              name = grp.Key.MedicalPharmacyDescription,
                              y = (Convert.ToDouble(grp.Count()) / TotalNumberOfBeneficiaries) * 100
                          }

                  ).ToArray();

            return Json(new { Result = Result.ToArray(), categories = Result.Select(x => x.name).ToArray() }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Beneficiaries Group By Prescription Iteration Percentage
        /// </summary>
        /// <param name="rp"> Report Parameters List</param>
        /// <returns></returns>
        public ActionResult BeneficiariesGroupByPrescriptionIterationPercentage(EMTReportParametersList rp)
        {
            rp = FillRP(rp);
            var RP_PrescriptionsDispensed = ((List<RP_PrescriptionsDispensed_Result>) Session["RP_PrescriptionsDispensed"]).AsQueryable().Where(x => !x.MainWarehouse).Where(MoreFilterPrescriptionsDispensed(rp)).Select(x =>
             new
             {
                 x.MedicalBeneficiaryGUID,

             }
            ).ToList();

            double TotalNumberOfBeneficiaries = RP_PrescriptionsDispensed.Select(x => x.MedicalBeneficiaryGUID).Distinct().Count();
            var Result = (from a in RP_PrescriptionsDispensed

                          group new { a.MedicalBeneficiaryGUID } by new { a.MedicalBeneficiaryGUID } into grp

                          select
                          new seriesPie
                          {
                              name = grp.Key.MedicalBeneficiaryGUID,
                              y = Convert.ToDouble(grp.Count())
                          }
                  ).GroupBy(x=> x.y ).Select(x=> new seriesPie
                  {
                      name=x.Key.ToString(),
                      y= Convert.ToDouble(x.Count())
                  }).ToArray();

            return Json(new { Result = Result.ToArray(), categories = Result.Select(x => x.name).ToArray() }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Consumption Medicines Per Months
        /// </summary>
        /// <param name="rp"></param>
        /// <returns></returns>
        public ActionResult ConsumptionMedicinesPerMonths(EMTReportParametersList rp)
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
            var categories = DbEMT.codeMedicalPharmacyLanguage.Where(x => x.LanguageID == LAN && x.Active && !x.codeMedicalPharmacy.MainWarehouse).ToList();
            var RP_ConsumptionMedicine = ((List<RP_ConsumptionMedicine_Result>)Session["RP_ConsumptionMedicine"]).AsQueryable().Where(MoreFilterConsumptionMedicine(rp)).ToList();

            var Result = (from a in RP_ConsumptionMedicine
                          join c in categories on a.MedicalPharmacyGUID equals c.MedicalPharmacyGUID
                          group new
                          {
                              c.MedicalPharmacyDescription
                          } by new { c.MedicalPharmacyDescription, c.MedicalPharmacyGUID } into G
                          select new drilldownLiner
                          {
                              name = G.Key.MedicalPharmacyDescription,
                              guid = G.Key.MedicalPharmacyGUID.ToString()
                          }

                                  ).ToList();

            Result.ForEach(r => r.data = (from a in MonthNames
                                          join b in RP_ConsumptionMedicine.Where(x => x.MedicalPharmacyGUID.ToString().ToString() == r.guid) on a.MonthOrder equals b.DeliveryDate.Month into LJ1
                                          from R1 in LJ1.DefaultIfEmpty(new RP_ConsumptionMedicine_Result() { MedicalPharmacyGUID = default(Guid) })
                                          group new { R1.QuantityByPackingUnit } by new { a.Name, R1.MedicalPharmacyGUID, a.MonthOrder } into grp
                                          orderby grp.Key.MonthOrder
                                          select new dataLiner
                                          {
                                              y = grp.Key.MedicalPharmacyGUID != Guid.Empty ? (int)grp.Sum(x => x.QuantityByPackingUnit) : 0
                                          }).ToArray()
                                      );


            return Json(new { Result = Result, categories = MonthNames.Select(x => x.Name).ToArray() }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Consumption By Categories Of Medicines
        /// </summary>
        /// <param name="rp"> Report Parameters List</param>
        /// <returns></returns>
        public ActionResult ConsumptionByCategoriesOfMedicines(EMTReportParametersList rp)
        {
            rp = FillRP(rp);
            var categories = new DropDownList().LookupValues(LookupTables.MedicalTreatment).ToList();
            var RP_ConsumptionMedicine = ((List<RP_ConsumptionMedicine_Result>)Session["RP_ConsumptionMedicine"]).AsQueryable().Where(MoreFilterConsumptionMedicine(rp)).ToList();

            double TotalNumberOfCunsumption = RP_ConsumptionMedicine.Select(x => x.QuantityByPackingUnit).Sum();

            var Result = (from a in categories
                          join b in RP_ConsumptionMedicine
                           on a.Value equals b.MedicalTreatmentGUID.ToString()
                          group new {  b.MedicalTreatmentGUID, b.QuantityByPackingUnit } by new { a.Text } into grp

                          select
                          new seriesPie
                          {
                              name = grp.Key.Text,
                              y = (Convert.ToDouble( grp.Sum(x=>x.QuantityByPackingUnit)) / TotalNumberOfCunsumption) * 100
                          }

                  ).ToArray().OrderBy(x=>x.y);

            return Json(new { Result = Result.ToArray(), categories = Result.Select(x => x.name).ToArray() }, JsonRequestBehavior.AllowGet);
        }
       
        /// <summary>
        /// Consumption By Cost Of Medicines
        /// </summary>
        /// <param name="rp"> Report Parameters List</param>
        /// <returns></returns>
        public ActionResult ConsumptionByCostOfMedicines(EMTReportParametersList rp)
        {
            rp = FillRP(rp);
            var categories = DbEMT.codeMedicalItem.Where(x => rp.Sequance.Contains(x.Sequance) && x.Active).OrderBy(x => x.BrandName).Select(x => new { x.BrandName, x.MedicalItemGUID }).ToArray();
            var RP_ConsumptionMedicine = ((List<RP_ConsumptionMedicine_Result>)Session["RP_ConsumptionMedicine"]).AsQueryable().Where(MoreFilterConsumptionMedicine(rp)).ToList();

            double TotalNumberOfCunsumption = RP_ConsumptionMedicine.Select(x => x.QuantityByPackingUnit).Sum();

            var Result = (from a in categories
                          join b in RP_ConsumptionMedicine
                           on a.MedicalItemGUID equals b.MedicalItemGUID
                          group new { b.MedicalItemGUID, b.QuantityByPackingUnit } by new { a.BrandName } into grp

                          select
                          new seriesPie
                          {
                              name = grp.Key.BrandName,
                              y = (Convert.ToDouble(grp.Sum(x => x.QuantityByPackingUnit)) / TotalNumberOfCunsumption) * 100
                          }

                  ).ToArray().OrderBy(x => x.y);

            return Json(new { Result = Result.ToArray(), categories = Result.Select(x => x.name).ToArray() }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 9)	Stock  of medicines (percentage % ) by expiration date categories total  and by partner. 
        ///-	Expired.
        ///-/	Less than 3 months.
        ///-	6-3 months.
        ///-	More than 6 months.
        /// </summary>
        /// <param name="rp"> Report Parameters List</param>
        /// <returns></returns>
        public ActionResult ExpirationDateByPartner(EMTReportParametersList rp)
        {
            rp = FillRP(rp);
            var categories = DbEMT.codeOrganizationsInstancesLanguages.Where(x => x.LanguageID == LAN && rp.OrganizationInstanceGUID.Contains(x.OrganizationInstanceGUID)).OrderBy(x => x.OrganizationInstanceDescription).Select(x => new { x.OrganizationInstanceDescription, x.OrganizationInstanceGUID }).ToArray();

            var RP_TransferMedicine = ((List<RP_TransferMedicine_Result>)Session["RP_TransferMedicine"]).AsQueryable().ToList();

            var DispatchedQuantity = (from a in categories
                                      join b in RP_TransferMedicine
                                       on a.OrganizationInstanceGUID equals b.OrganizationInstanceGUID
                                      let Expiration = (b.ExpirationDate.Value - DateTime.Now ).Days / 31
                                      let ExpirationGroup = Expiration <= 0 ? "Expired" : Expiration <= 3 ? "Less than 3 months" : Expiration > 3 && Expiration <= 6 ? "6-3 months " : "More than 6 months"

                                      group new { b.RemainingItems, a.OrganizationInstanceGUID, ExpirationGroup } by new { a.OrganizationInstanceGUID, a.OrganizationInstanceDescription, ExpirationGroup } into grp
                                      orderby grp.Key.OrganizationInstanceDescription
                                      select new Rainfall
                                      {
                                          name = grp.Key.ExpirationGroup,
                                          data = grp.Where(x=>x.OrganizationInstanceGUID==grp.Key.OrganizationInstanceGUID && x.ExpirationGroup==grp.Key.ExpirationGroup).GroupBy(x=>x.OrganizationInstanceGUID).Select(x => x.Sum(y=>Convert.ToDouble( y.RemainingItems))).ToArray()
                                      }

                   ).ToArray();

            return Json(new { Result = DispatchedQuantity, categories = "" }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 10)	Percentage %  of stagnant items from the overall stock  (total ) and per partner.
        /// </summary>
        /// <param name="rp"> Report Parameters List</param>
        /// <returns></returns>
        public ActionResult StagnantItemsByPartner(EMTReportParametersList rp)
        {
            rp = FillRP(rp);
            var categories = DbEMT.codeOrganizationsInstancesLanguages.Where(x => x.LanguageID == LAN && rp.OrganizationInstanceGUID.Contains(x.OrganizationInstanceGUID)).OrderBy(x => x.OrganizationInstanceDescription).Select(x => new { x.OrganizationInstanceDescription, x.OrganizationInstanceGUID }).ToArray();

            var RP_TransferMedicine = ((List<RP_TransferMedicine_Result>)Session["RP_TransferMedicine"]).AsQueryable().ToList();

            var DispatchedQuantity = (from a in categories
                                      join b in RP_TransferMedicine
                                       on a.OrganizationInstanceGUID equals b.OrganizationInstanceGUID
                                      let UsedQuantity = (b.QuantityByPackingUnit - b.RemainingItems) 
                                      let UsedQuantityGroup = UsedQuantity == 0 ? "Stagnant Items" : "Other Items"

                                      group new { b.RemainingItems, a.OrganizationInstanceGUID, UsedQuantityGroup } by new { a.OrganizationInstanceGUID, a.OrganizationInstanceDescription, UsedQuantityGroup } into grp
                                      orderby grp.Key.OrganizationInstanceDescription
                                      select new Rainfall
                                      {
                                          name = grp.Key.UsedQuantityGroup,
                                          data = grp.Where(x => x.OrganizationInstanceGUID == grp.Key.OrganizationInstanceGUID && x.UsedQuantityGroup == grp.Key.UsedQuantityGroup).GroupBy(x => x.OrganizationInstanceGUID).Select(x => x.Sum(y => Convert.ToDouble(y.RemainingItems))).ToArray()
                                      }

                   ).ToArray();

            return Json(new
            {
                Result = DispatchedQuantity,
                categories = (from a in categories
                              join b in RP_TransferMedicine
                               on a.OrganizationInstanceGUID equals b.OrganizationInstanceGUID
                              select a.OrganizationInstanceDescription).ToArray()
            }, JsonRequestBehavior.AllowGet);

        }
        
        /// <summary>
        /// The Consumption Medicines By Gender  
        /// Map at a certain period of time (week, two weeks, month, quarter …etc  in items ( packing unit and smallest units) 
        /// </summary>
        /// <param name="rp"> Report Parameters List</param>
        /// <returns></returns>
        public ActionResult TotalNumberOfBeneficiaryGroupByAge(EMTReportParametersList rp)
        {
            rp = FillRP(rp);
            var RP_ConsumptionMedicine = ((List<RP_ConsumptionMedicine_Result>)Session["RP_ConsumptionMedicine"]).AsQueryable().Where(MoreFilterConsumptionMedicine(rp)).ToList();
            var Result = (from a in RP_ConsumptionMedicine.Where(x => x.Brithday != null)
                              //get the date of the birthday this year
                          let age = (DateTime.Now - a.Brithday.Value).Days / 365

                          group new { a.MedicalBeneficiaryGUID } by new { age } into grp
                          select new ArrayList
                            {
                                grp.Key.age,
                                grp.Distinct().Count()
                            }).ToArray();

           return Json(new { Result = Result }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetPrescriptionsDispensed(EMTReportParametersList rp)
        {
            rp = FillRP(rp);
            List<RP_PrescriptionsDispensed_Result> RP_PrescriptionsDispensed = new List<RP_PrescriptionsDispensed_Result>();
            if (Session["RP_PrescriptionsDispensed"] == null)
            {
                RP_PrescriptionsDispensed = DbEMT.RP_PrescriptionsDispensed(string.Join(",", rp.MedicalPharmacyGUID), rp.StartDate, rp.EndDate, LAN).AsQueryable().Where(x => !x.MainWarehouse).Where(MoreFilterPrescriptionsDispensed(rp)).ToList();
                Session["RP_PrescriptionsDispensed"] = RP_PrescriptionsDispensed;
            }

            return Json(new
            {
                Result = true
            });
        }

        public ActionResult GetConsumptionMedicine(EMTReportParametersList rp)
        {
            rp = FillRP(rp);
            List<RP_ConsumptionMedicine_Result> RP_ConsumptionMedicine = new List<RP_ConsumptionMedicine_Result>();
            if (Session["RP_ConsumptionMedicine"] == null)
            {
                RP_ConsumptionMedicine = DbEMT.RP_ConsumptionMedicine(
                                                string.Join(",", rp.MedicalPharmacyGUID),
                                                string.Join(",", rp.Sequance),
                                                string.Join(",", rp.MedicalPharmacologicalFormGUID),
                                                rp.StartDate, rp.EndDate, LAN).ToList();
                Session["RP_ConsumptionMedicine"] = RP_ConsumptionMedicine;
            }

            return Json(new
            {
                Result = true
            });
        }

        public ActionResult GetTransferMedicine(EMTReportParametersList rp)
        {
            rp = FillRP(rp);
            List<RP_TransferMedicine_Result> RP_TransferMedicine = new List<RP_TransferMedicine_Result>();
            if (Session["RP_TransferMedicine"] == null)
            {
                RP_TransferMedicine = DbEMT.RP_TransferMedicine(
                                                string.Join(",", rp.MedicalPharmacyGUID),
                                                string.Join(",", rp.Sequance),
                                                string.Join(",", rp.MedicalPharmacologicalFormGUID),
                                                rp.StartDate, rp.EndDate, LAN).ToList();
                Session["RP_TransferMedicine"] = RP_TransferMedicine;
            }

            return Json(new
            {
                Result = true
            });
        }
        public ActionResult GetDispatchedMedicine(EMTReportParametersList rp)
        {
            rp = FillRP(rp);
            List<RP_DispatchedMedicine_Result> RP_DispatchedMedicine = new List<RP_DispatchedMedicine_Result>();
            if (Session["RP_DispatchedMedicine"] == null)
            {
                RP_DispatchedMedicine = DbEMT.RP_DispatchedMedicine(
                                                string.Join(",", rp.MedicalPharmacyGUID),
                                                string.Join(",", rp.Sequance),
                                                string.Join(",", rp.MedicalPharmacologicalFormGUID),
                                                rp.StartDate, rp.EndDate, LAN).ToList();
                Session["RP_DispatchedMedicine"] = RP_DispatchedMedicine;
            }

            return Json(new
            {
                Result = true
            });
        }

        //
        public ActionResult KillSessions()
        {
            Session["RP_PrescriptionsDispensed"] = null;
            Session["RP_TransferMedicine"] = null;
            Session["RP_ConsumptionMedicine"] = null;
            Session["RP_DispatchedMedicine"] = null;
            return Json(new
            {
                Result = true
            });
         }

    }
}