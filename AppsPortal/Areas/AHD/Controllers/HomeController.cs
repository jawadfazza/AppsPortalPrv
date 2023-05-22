using AHD_DAL.ViewModels;
using AppsPortal.BaseControllers;
using AppsPortal.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.AHD.Controllers
{
    public class HomeController : AHDBaseController
    {
        // GET: AHD/Home
        public ActionResult Index()
        {
            CMS.SetUserToken(UserProfileGUID, Apps.AHD);
            Session[SessionKeys.CurrentApp] = Apps.AHD;
            CMS.BuildUserMenus(UserGUID, LAN);
            if (CMS.HasAction(Permissions.StaffProfile.Update, Apps.ORG))
            {
                return View();
            }
            if (CMS.HasAction(Permissions.InternationalStaffEntitlements.Access, Apps.AHD))
            {
                return View();
            }
            if (CMS.HasAction(Permissions.NationalStaffDangerPayManagement.Access, Apps.AHD))
            {
                return View();
            }
            if (CMS.HasAction(Permissions.HRRestandRecuperationLeave.Access, Apps.AHD))
            {
                return View();
            }
            if (CMS.HasAction(Permissions.StaffOvertime.Access, Apps.AHD))
            {
                return View();
            }
            if (CMS.HasAction(Permissions.VehicleMaintenanceRequest.Access, Apps.AHD))
            {
                return View();
            }
            if (CMS.HasAction(Permissions.StaffProfileSecuritySection.Access, Apps.ORG))
            {
                return View();
            }
            if (CMS.HasAction(Permissions.Shuttle.Access, Apps.SHM))
            {
                return View();
            }
            else

            {
                return Json(DbAHD.PermissionError());
            }
            //var result=DbAHD.codeActionsEntities.ToList();
            //return View();
        }
        [Route("AHD/Entitlements/")]
        public ActionResult EntitlementsHome()
        {
            if (!CMS.HasAction(Permissions.InternationalStaffEntitlements.Access, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            return View("~/Areas/AHD/Views/InternationalStaffEntitlementPeriod/Home.cshtml");
        }

        [Route("AHD/HrForms/")]
        public ActionResult HrFormslIndex()
        {
            if (!CMS.HasAction(Permissions.StaffProfile.Access, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            return View("~/Areas/AHD/Views/Home/HRForms.cshtml");
        }

        #region Org Chart
        public ActionResult OrganizationalChart()
        {
            return View();
        }

        public ActionResult LoadOrganogram()
        {
            List<StaffOrgChart> _myStaffChart = new List<StaffOrgChart>();
            Guid _endAssi = Guid.Parse("B9CD375C-A576-4AA4-8AF4-FF3C1C4E3612");
            Guid represpentitJobTitleGUID = Guid.Parse("9B98F32B-D27D-45A2-9E6F-6A042D884905");
            var _allEmp = DbAHD.v_staffCoreDataOverview.Where(x => x.StaffStatusGUID != _endAssi || (x.JobTitleGUID == represpentitJobTitleGUID && x.StaffStatusGUID != _endAssi)).ToList();
            //var _activeUser = _allEmp.Select(x => x.UserGUID).Distinct().ToList();
            List<string> DepartmentDescription = new List<string>();




            List<string> Department = new List<string>();

            //get the root node
            var headManger = _allEmp.FirstOrDefault(e => e.ReportToGUID == null && e.JobTitleGUID == represpentitJobTitleGUID);

            List<string> headMangerTags = new List<string>();
            //headMangerTags.Add(headManger.DepartmentDescription);
            headMangerTags.Add("top-management");
            StaffOrgChart TopMangerDepartmentNode = new StaffOrgChart
            {
                //id = headManger.DepartmentDescription,
                //name = headManger.DepartmentDescription,
                id = "top-management",
                name = "top-management",
                tags = headMangerTags
            };
            Department.Add(TopMangerDepartmentNode.id);
            _myStaffChart.Add(TopMangerDepartmentNode);

            StaffOrgChart headMangerNode = new StaffOrgChart
            {
                id = headManger.UserGUID.ToString(),
                address = headManger.PermanentAddressEn,
                email = headManger.EmailAddress,
                img = "/Uploads/ORG/StaffPhotos/" + headManger.UserGUID + "/" + headManger.UserGUID + ".jpg",
                name = headManger.FullName,
                phone = headManger.SATPhoneNumber,
                pid = headManger.ReportToGUID.ToString(),
                stpid = TopMangerDepartmentNode.id,
                title = headManger.JobTitle,
                gender = headManger.Gender,
                ContractEndDate = headManger.ContractEndDate.ToString(),
                EmpContractType = headManger.ContractType,
                EmpReqType = headManger.ContractType,
                nationality = headManger.Nationality,
                Department = headManger.DepartmentDescription
            };
            _myStaffChart.Add(headMangerNode);

            string[] Colors = { "dep1", "dep2", "dep3", "dep4" };
            int indexColor = 0;

            //find son's of root
            var HeadsOfDepartments = _allEmp.Where(e => e.ReportToGUID == headManger.UserGUID).ToList();

            //check if this node have childrens 
            foreach (var emp in HeadsOfDepartments)
            {
                //test if HeadsOfDepartments have childrens 
                var emps21 = _allEmp.FirstOrDefault(a => a.ReportToGUID == emp.UserGUID);

                //if Node has childrens then create department node
                if (emps21 != null)
                {
                    //check if we create this department
                    if (TopMangerDepartmentNode.id != emp.DepartmentDescription
                        && Department.FirstOrDefault(a => a == emp.DepartmentDescription) == null)
                    {

                        List<string> departmentNodeTags = new List<string>();
                        departmentNodeTags.Add(Colors[indexColor]);
                        indexColor += 1;
                        indexColor = indexColor % Colors.Length;

                        departmentNodeTags.Add(emp.DepartmentDescription);
                        departmentNodeTags.Add("department");
                        StaffOrgChart departmentNode = new StaffOrgChart
                        {
                            id = emp.DepartmentDescription,
                            name = emp.DepartmentDescription,
                            pid = TopMangerDepartmentNode.id,
                            tags = departmentNodeTags
                        };
                        Department.Add(departmentNode.id);
                        _myStaffChart.Add(departmentNode);
                    }
                    StaffOrgChart headOfDepartmentNode = new StaffOrgChart
                    {
                        id = emp.UserGUID.ToString(),
                        stpid = emp.DepartmentDescription,
                        address = emp.PermanentAddressEn,
                        email = emp.EmailAddress,
                        img = "/Uploads/ORG/StaffPhotos/" + emp.UserGUID + "/" + emp.UserGUID + ".jpg",
                        name = emp.FullName,
                        phone = emp.OfficialMobileNumber,
                        //pid = headManger.ReportToGUID,
                        title = emp.JobTitle,
                        gender = emp.Gender,
                        ContractEndDate = emp.ContractEndDate.ToString(),
                        EmpContractType = emp.ContractType,
                        EmpReqType = emp.RecruitmentType,
                        nationality = emp.Nationality,
                        Department = emp.DepartmentDescription
                    };
                    _myStaffChart.Add(headOfDepartmentNode);


                }
                else
                {

                    //Node doesn't have childrens then it is assistantNode 
                    List<string> assistantNodeTags = new List<string>();
                    assistantNodeTags.Add("assistant");
                    StaffOrgChart assistantNode = new StaffOrgChart
                    {
                        id = emp.UserGUID.ToString(),
                        //stpid = emp.DepartmentDescription,
                        address = emp.PermanentAddressEn,
                        email = emp.EmailAddress,
                        img = "/Uploads/ORG/StaffPhotos/" + emp.UserGUID + "/" + emp.UserGUID + ".jpg",
                        name = emp.FullName,
                        phone = emp.OfficialMobileNumber,
                        title = emp.JobTitle,
                        pid = TopMangerDepartmentNode.id,
                        gender = emp.Gender,
                        ContractEndDate = emp.ContractEndDate.ToString(),
                        EmpContractType = emp.ContractType,
                        EmpReqType = emp.RecruitmentType,
                        nationality = emp.Nationality,
                        tags = assistantNodeTags,
                        Department = emp.DepartmentDescription
                    };
                    _myStaffChart.Add(assistantNode);
                }
            }

            foreach (var e in _allEmp)
            {
                //above we add headManger & HeadsOfDepartments
                //test if node of var not headManger or HeadsOfDepartments add to _myStaffChart
                if (headManger.UserGUID != e.UserGUID
                    && HeadsOfDepartments.FirstOrDefault(w => w.UserGUID == e.UserGUID) == null
                    && _allEmp.FirstOrDefault(a => a.UserGUID == e.ReportToGUID) != null
                    )
                {

                    StaffOrgChart Employee_ = new StaffOrgChart
                    {
                        id = e.UserGUID.ToString(),
                        address = e.PermanentAddressEn,
                        email = e.EmailAddress,
                        img = "/Uploads/ORG/StaffPhotos/" + e.UserGUID + "/" + e.UserGUID + ".jpg",
                        name = e.FullName,
                        phone = e.OfficialMobileNumber,
                        pid = e.ReportToGUID.ToString(),
                        title = e.JobTitle,
                        Department = e.DepartmentDescription
                    };
                    _myStaffChart.Add(Employee_);
                }
            }

            return Json(new { Employee = _myStaffChart }, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult FilterByDepartment(string DepartmentInput)
        //{
        //    //List<StaffOrgChart> FilterStaff = new List<StaffOrgChart>();

        //   var  FilterStaff = DbAHD.v_staffCoreDataOverview.Where(e => e.DepartmentDescription == DepartmentInput).ToList();

        //    //FilterStaff=
        //    return Json(new { Employee = FilterStaff }, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult AllDepartment()
        //{
        //    HashSet<string> Depatments = new HashSet<string>();
        //    foreach (StaffOrgChart item in _myStaffChart)
        //    {
        //        Depatments.Add(item.DepartmentDescription);
        //    }
        //    return Json(new { Employee = Depatments }, JsonRequestBehavior.AllowGet);
        //}


        #endregion

        public JsonResult InitDrillDownChart()
        {
            Guid _active = Guid.Parse("B9CD375C-A576-4AA4-8AF4-FF3C1C4E3611");
            var staff = DbAHD.v_staffCoreDataOverview.Where(x => x.RecruitmentTypeGUID != null && x.StaffStatusGUID == _active).ToList();
            Guid re = Guid.Parse("8A2AE01E-0F57-4178-B05E-E3021A9701A6");
            var staffTypes = DbAHD.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.codeTablesValues.TableGUID == re).ToList();
            var ff = staff.Select(x => x.RecruitmentTypeGUID).ToList();
            var test = staffTypes.Where(x => ff.Contains(x.ValueGUID)).Select(x => new { x.ValueGUID, x.ValueDescription }).ToList();
            series MainData = new series();


            MainData = new series
            {
                name = "Staff By Grade",
                colorByPoint = true,
                data = (from a in staff
                        join b in staffTypes on a.RecruitmentTypeGUID equals b.ValueGUID into LJ1
                        from R1 in LJ1.DefaultIfEmpty()
                        select new
                        {
                            ValueDescription = R1.ValueDescription,
                            UserGUID = a.UserGUID
                        }
                                ).GroupBy(x => x.ValueDescription).
                                Select(x =>
                                    new dataSeries
                                    {
                                        colorByPoint = true,
                                        name = x.Key,
                                        drilldown = x.Key,
                                        y = x.Count(),
                                        //selected = true,
                                        sliced = true
                                    }
                                ).ToArray()
            };


            var DetailsData = (from a in staffTypes
                               join b in staff.Where(x => x.GradeGUID != null) on a.ValueGUID equals b.RecruitmentTypeGUID into LJ1
                               from R1 in LJ1.DefaultIfEmpty()


                               group new { R1.UserGUID } by new { a.ValueDescription, R1.Grade } into grp
                               select new drilldown()
                               {
                                   name = grp.Key.Grade,
                                   id = grp.Key.ValueDescription,
                                   data = (from y in staff
                                                                  .Where(x => x.RecruitmentType == grp.Key.ValueDescription)
                                           group new { y.GradeGUID } by new { y.Grade } into grpData
                                           select new DataDrillDown
                                           {
                                               name = grpData.Key.Grade,
                                               y = grpData.Count()
                                           }
                                           ).ToArray()
                               }
                               ).ToArray();
            var dutyStationGUID = staff.Select(x => x.DutyStationGUID).Distinct();
            var dutyStations = DbAHD.codeDutyStationsLanguages.Where(x => x.LanguageID == LAN && dutyStationGUID.Contains(x.DutyStationGUID)).ToList();
            series MainDataDutyStation = new series();

            MainDataDutyStation = new series
            {
                name = "Staff By Duty Station",
                colorByPoint = true,
                data = (from a in dutyStations
                        join b in staff.Where(x => x.DutyStationGUID != null) on a.DutyStationGUID equals b.DutyStationGUID into LJ1
                        from R1 in LJ1.DefaultIfEmpty()
                        select new
                        {
                            Name = a.DutyStationDescription,
                            UserGUID = R1.UserGUID
                        }
                                ).GroupBy(x => x.Name).
                                Select(x =>
                                    new dataSeries
                                    {
                                        colorByPoint = true,
                                        name = x.Key,
                                        drilldown = x.Key,
                                        y = x.Count(),
                                        //selected = true,
                                        sliced = true
                                    }
                                ).ToArray()
            };


            var DetailsDutyStations = (from a in dutyStations
                                       join b in staff.Where(x => x.DutyStation != null) on a.DutyStationGUID equals b.DutyStationGUID into LJ1
                                       from R1 in LJ1.DefaultIfEmpty()


                                       group new { R1.UserGUID } by new { a.DutyStationDescription, R1.RecruitmentType } into grp
                                       select new drilldown()
                                       {
                                           name = grp.Key.RecruitmentType,
                                           id = grp.Key.DutyStationDescription,
                                           data = (from y in staff
                                                                          .Where(x => x.DutyStation == grp.Key.DutyStationDescription)
                                                   group new { y.RecruitmentTypeGUID } by new { y.RecruitmentType } into grpData
                                                   select new DataDrillDown
                                                   {
                                                       name = grpData.Key.RecruitmentType,
                                                       y = grpData.Count()
                                                   }
                                                   ).ToArray()
                                       }
                               ).ToArray();
            return Json(new { MainReport = MainData, DetailsReport = DetailsData, MainDataDutyStation = MainDataDutyStation, DetailsDutyStations = DetailsDutyStations }, JsonRequestBehavior.AllowGet);


        }

        public class series
        {
            public string name { get; set; }

            public bool colorByPoint { get; set; }
            public dataSeries[] data { get; set; }
        }

        public class dataSeries
        {
            public bool? colorByPoint { get; set; }
            public bool sliced { get; set; }
            public string name { get; set; }
            public decimal? y { get; set; }
            public string drilldown { get; set; }
        }
        public class drilldown
        {
            public string name { get; set; }
            public string id { get; set; }
            public bool colorByPoint { get; set; }
            public DataDrillDown[] data { get; set; }
        }
        public class DataDrillDown
        {
            public string name { set; get; }
            public double y { set; get; }
        }
    }
}