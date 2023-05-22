using AppsPortal.BaseControllers;
using AppsPortal.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WMS_DAL.ViewModels;

namespace AppsPortal.Areas.WMS.Controllers
{
    public class HomeController : WMSBaseController
    {
        // GET: WMS/Home
        public ActionResult Index()
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Update, Apps.WMS))
            {
                return Json(DbAHD.PermissionError());
            }
            CMS.SetUserToken(UserProfileGUID, Apps.WMS);
            Session[SessionKeys.CurrentApp] = Apps.WMS;
            Guid orginstanceGUID = (Guid)Session[SessionKeys.OrganizationInstanceGUID];
            CMS.BuildUserMenus(UserGUID, LAN);
            var warehouseGuid = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).Select(x => x.WarehouseGUID).FirstOrDefault();
            var pending = DbWMS.v_EntryMovementDataTable.Where(x => x.LastFlowTypeGUID == WarehouseRequestFlowType.PendingConfirmed
                                                           && x.OrganizationInstanceGUID == orginstanceGUID
                                                    ).AsQueryable();
            PendingRequestCheckVM model = new PendingRequestCheckVM();
            model.pendingForMyWarehosue = pending.Where(x => x.LastCustdianNameGUID == warehouseGuid
              ).Count();
            model.PendingForOtherWarehouse = pending.Where(x =>
                    x.LastCustdianGUID == WarehouseRequestSourceTypes.Warehouse &&
                    x.LastCustdianNameGUID != warehouseGuid

                    )

                .Count();
            model.PendingForStaff = pending.Where(x =>
                    x.LastCustdianGUID == WarehouseRequestSourceTypes.Staff

                    )
                .Select(x => x.LastCustdianNameGUID).Distinct().Count();


            DateTime today = DateTime.Now;
            string custType = "Staff";

            model.DelayInReturn = DbWMS.v_trackItemOutputFlow.Where(x => x.LastCustodian == custType
                                                                   && x.ExpectedReturenedDate <= today
                                                                   && x.OrganizationInstanceGUID == orginstanceGUID
                                                                   && x.IsLastAction == true
                                                                  && x.LastFlowTypeGUID == WarehouseRequestFlowType.Confirmed).Count();


            return View(model);
        }
        public ActionResult Reports()
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Update, Apps.WMS))
            {
                return Json(DbAHD.PermissionError());
            }
            CMS.SetUserToken(UserProfileGUID, Apps.WMS);
            Session[SessionKeys.CurrentApp] = Apps.WMS;
            CMS.BuildUserMenus(UserGUID, LAN);
            return View();
        }

        public ActionResult Configuration()
        {
            CMS.SetUserToken(UserProfileGUID, Apps.WMS);
            Session[SessionKeys.CurrentApp] = Apps.WMS;
            CMS.BuildUserMenus(UserGUID, LAN);
            return View();
        }
        #region Home

        [HttpPost]
        public ActionResult Chart1(PendingRequestCheckVM rp)
        {

            Guid orginstanceGUID = (Guid)Session[SessionKeys.OrganizationInstanceGUID];
            var issuLocations = (from a in DbWMS.v_trackItemOutputFlow.Where(x => x.FlowCreatedDate >= rp.StartDate && x.FlowCreatedDate <= rp.EndDate && x.OrganizationInstanceGUID == orginstanceGUID)
                                 join b in DbWMS.codeWarehouseLocationLanguage.Where(x => x.LanguageID == LAN && x.Active) on a.LastWarehouseLocationGUID.ToString() equals b.WarehouseLocationGUID.ToString()
                                 group new
                                 {
                                     a.ItemOutputDetailGUID,
                                     b.WarehouseLocationDescription
                                 } by b.WarehouseLocationDescription into G
                                 select new HighChartPieModel
                                 {
                                     name = G.Key,
                                     y = G.Count(),
                                     selected = true,
                                     sliced = true
                                 }
            ).ToList();
            return Json(new { issuLocations = issuLocations });
        }

        public ActionResult chart2(PendingRequestCheckVM rp)
        {
            Guid orginstanceGUID = (Guid)Session[SessionKeys.OrganizationInstanceGUID];
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

            var Result = (from a in DbWMS.v_trackItemOutputFlow.Where(x => x.FlowCreatedDate >= rp.StartDate && x.FlowCreatedDate <= rp.EndDate && x.OrganizationInstanceGUID == orginstanceGUID
                           && x.IsLastAction == true)
                          join b in DbWMS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN && x.codeTablesValues.TableGUID == LookupTables.WarehouseFlowType) on a.LastFlowTypeGUID equals b.ValueGUID

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
                              join b in DbWMS.v_trackItemOutputFlow.Where(x => x.LastFlowTypeGUID == r.Guid && x.FlowCreatedDate >= rp.StartDate && x.FlowCreatedDate <= rp.EndDate && x.OrganizationInstanceGUID == orginstanceGUID) on a.MonthOrder equals b.FlowCreatedDate.Month into LJ1
                              from R1 in LJ1.DefaultIfEmpty(new WMS_DAL.Model.v_trackItemOutputFlow() { ItemOutputDetailGUID = default(Guid), LastFlowTypeGUID = default(Guid) })
                              group new { a.Name, R1.LastFlowTypeGUID } by new { a.Name, R1.LastFlowTypeGUID, a.MonthOrder } into grp
                              orderby grp.Key.MonthOrder
                              select new data
                              {
                                  y = grp.Key.LastFlowTypeGUID != Guid.Empty ? grp.Count() : 0
                              }).ToArray()
                );


            return Json(new { Model = Result });
        }
        public JsonResult InitCalander(PendingRequestCheckVM rp)
        {
            Guid orginstanceGUID = (Guid)Session[SessionKeys.OrganizationInstanceGUID];
            List<CalanderVM> result = new List<CalanderVM>();
            var flows = DbWMS.v_trackItemOutputFlow.Where(x => x.IsLastAction == true && x.OrganizationInstanceGUID == orginstanceGUID
            ).ToList();
            var flowsDates = flows.ToList().Select(x => x.FlowCreatedDate.ToShortDateString()).Distinct();
            Guid syriaCountryGUID = Guid.Parse("710BFD1B-50CC-4F1E-92A9-A70583CFA5E0");
            var warehouses = DbWMS.codeWarehouseLocationLanguage.Where(x =>
                  x.LanguageID == LAN && x.Active).ToList();
            string format = "yyyy-MM-dd";
            foreach (var item in flowsDates)
            {
                DateTime time = Convert.ToDateTime(item);             // Use current time.
                // Use this format.
                var myDate = time.ToString(format);
                string setdate = time.ToString("dd/MM/yyyy");
                foreach (var myWarehoue in warehouses)
                {
                    var myFlows = flows.Where(x => x.LastWarehouseLocationGUID == myWarehoue.WarehouseLocationGUID).ToList();
                    if (myWarehoue.WarehouseLocationDescription == "Damascus")
                    {

                        if (myFlows.Where(x => x.FlowCreatedDate.ToShortDateString() == item
                                                    && x.LastWarehouseLocationGUID == myWarehoue.WarehouseLocationGUID)
                                .Select(x => x.ItemOutputDetailFlowGUID).Count() > 0)
                        {



                            CalanderVM myResult = new CalanderVM
                            {
                                id = myWarehoue.WarehouseLocationGUID.ToString() + "x" + setdate,
                                title = "Damascus: " + " " + myFlows.Where(x => x.FlowCreatedDate.ToShortDateString() == item
                                                                                && x.LastWarehouseLocationGUID == myWarehoue.WarehouseLocationGUID)
                                            .Select(x => x.ItemOutputDetailFlowGUID).Count(),
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

                    if (myWarehoue.WarehouseLocationDescription == "Aleppo")
                    {
                        if (myFlows.Where(x => x.FlowCreatedDate.ToShortDateString() == item
                                               && x.LastWarehouseLocationGUID == myWarehoue.WarehouseLocationGUID)
                                .Select(x => x.ItemOutputDetailFlowGUID).Count() > 0)
                        {



                            CalanderVM myResult = new CalanderVM
                            {
                                id = myWarehoue.WarehouseLocationGUID.ToString() + "x" + setdate,
                                title = "Aleppo: " + " " + flows.Where(x => x.FlowCreatedDate.ToShortDateString() == item
                                                                            && x.LastWarehouseLocationGUID == myWarehoue.WarehouseLocationGUID)
                                            .Select(x => x.ItemOutputDetailFlowGUID).Count(),
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

                    if (myWarehoue.WarehouseLocationDescription == "Homs")
                    {
                        if (myFlows.Where(x => x.FlowCreatedDate.ToShortDateString() == item
                                               && x.LastWarehouseLocationGUID == myWarehoue.WarehouseLocationGUID)
                                .Select(x => x.ItemOutputDetailFlowGUID).Count() > 0)
                        {



                            CalanderVM myResult = new CalanderVM
                            {
                                id = myWarehoue.WarehouseLocationGUID.ToString() + "x" + setdate,
                                title = "Homs: " + " " + flows.Where(x => x.FlowCreatedDate.ToShortDateString() == item
                                                                          && x.LastWarehouseLocationGUID == myWarehoue.WarehouseLocationGUID)
                                            .Select(x => x.ItemOutputDetailFlowGUID).Count(),
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

                    if (myWarehoue.WarehouseLocationDescription == "Tartous")
                    {
                        if (myFlows.Where(x => x.FlowCreatedDate.ToShortDateString() == item
                                               && x.LastWarehouseLocationGUID == myWarehoue.WarehouseLocationGUID)
                                .Select(x => x.ItemOutputDetailFlowGUID).Count() > 0)
                        {


                            CalanderVM myResult = new CalanderVM
                            {
                                id = myWarehoue.WarehouseLocationGUID.ToString() + "x" + setdate,
                                title = "Tartous: " + " " + flows.Where(x => x.FlowCreatedDate.ToShortDateString() == item
                                                                             && x.LastWarehouseLocationGUID == myWarehoue.WarehouseLocationGUID)
                                            .Select(x => x.ItemOutputDetailFlowGUID).Count(),
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

                    if (myWarehoue.WarehouseLocationDescription == "Al-Qamishli")
                    {
                        if (myFlows.Where(x => x.FlowCreatedDate.ToShortDateString() == item
                                               && x.LastWarehouseLocationGUID == myWarehoue.WarehouseLocationGUID)
                                .Select(x => x.ItemOutputDetailFlowGUID).Count() > 0)
                        {



                            CalanderVM myResult = new CalanderVM
                            {
                                id = myWarehoue.WarehouseLocationGUID.ToString() + "x" + setdate,
                                title = "Al-Qamishli: " + " " + flows.Where(x => x.FlowCreatedDate.ToShortDateString() == item
                                                                                 && x.LastWarehouseLocationGUID == myWarehoue.WarehouseLocationGUID)
                                            .Select(x => x.ItemOutputDetailFlowGUID).Count(),
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

                    if (myWarehoue.WarehouseLocationDescription == "Suwayda")
                    {
                        if (myFlows.Where(x => x.FlowCreatedDate.ToShortDateString() == item
                                               && x.LastWarehouseLocationGUID == myWarehoue.WarehouseLocationGUID)
                                .Select(x => x.ItemOutputDetailFlowGUID).Count() > 0)
                        {



                            CalanderVM myResult = new CalanderVM
                            {
                                id = myWarehoue.WarehouseLocationGUID.ToString() + "x" + setdate,
                                title = "Suwayda: " + " " + flows.Where(x => x.FlowCreatedDate.ToShortDateString() == item
                                                                             && x.LastWarehouseLocationGUID == myWarehoue.WarehouseLocationGUID)
                                            .Select(x => x.ItemOutputDetailFlowGUID).Count(),
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

                    if (myWarehoue.WarehouseLocationDescription == "Hamah")
                    {
                        if (myFlows.Where(x => x.FlowCreatedDate.ToShortDateString() == item
                                               && x.LastWarehouseLocationGUID == myWarehoue.WarehouseLocationGUID)
                                .Select(x => x.ItemOutputDetailFlowGUID).Count() > 0)
                        {



                            CalanderVM myResult = new CalanderVM
                            {
                                id = myWarehoue.WarehouseLocationGUID.ToString() + "x" + setdate,
                                title = "Hamah: " + " " + flows.Where(x => x.FlowCreatedDate.ToShortDateString() == item
                                                                           && x.LastWarehouseLocationGUID == myWarehoue.WarehouseLocationGUID)
                                            .Select(x => x.ItemOutputDetailFlowGUID).Count(),
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
                    if (myWarehoue.WarehouseLocationDescription == "Port Of Spain")
                    {
                        if (myFlows.Where(x => x.FlowCreatedDate.ToShortDateString() == item
                                               && x.LastWarehouseLocationGUID == myWarehoue.WarehouseLocationGUID)
                                .Select(x => x.ItemOutputDetailFlowGUID).Count() > 0)
                        {



                            CalanderVM myResult = new CalanderVM
                            {
                                id = myWarehoue.WarehouseLocationGUID.ToString() + "x" + setdate,
                                title = "Port Of Spain: " + " " + flows.Where(x => x.FlowCreatedDate.ToShortDateString() == item
                                                                           && x.LastWarehouseLocationGUID == myWarehoue.WarehouseLocationGUID)
                                            .Select(x => x.ItemOutputDetailFlowGUID).Count(),
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



                }
            }


            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion


    }
}