using AppsPortal.BaseControllers;
using AppsPortal.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.ORG.Controllers
{
    public class DashboardController : ORGBaseController
    {
        public class OrganicChartMode
        {
            public string id { get; set; }
            public string parentId { get; set; }
            public string category { get; set; }
            public string name { get; set; }
            public string title { get; set; }
            public string phone { get; set; }
            public string mail { get; set; }
            public string adress { get; set; }
            public string image { get; set; }
            //public string secondParenId { get;set; }
        }

        public ActionResult Index()
        {
            if (!CMS.HasAction(Permissions.StaffProfile.Access, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            return View("~/Areas/ORG/Views/Dashboard/Index.cshtml");
        }

        public JsonResult FullOrganicChartData()
        {
            if (!CMS.HasAction(Permissions.StaffProfile.Access, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            List<OrganicChartMode> organicChartModes = new List<OrganicChartMode>();

            List<OrganicChartMode> depMap = (from a in DbORG.codeDepartmentsConfigurations.Where(x => x.Active)
                                             join b in DbORG.codeDepartmentsLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.DepartmentGUID equals b.DepartmentGUID into LJ1
                                             from RJ1 in LJ1.DefaultIfEmpty()
                                             select new OrganicChartMode
                                             {
                                                 id = a.DepartmentGUID.ToString(),
                                                 parentId = a.DepartmentParentStaffGUID.HasValue ? a.DepartmentParentStaffGUID.Value.ToString() : a.ParentDepartmentGUID.HasValue ? a.ParentDepartmentGUID.Value.ToString() : null,
                                                 category = "Department",
                                                 name = RJ1.DepartmentDescription,
                                                 title = RJ1.DepartmentDescription,
                                                 phone = "",
                                                 mail = "",
                                                 adress = "",
                                                 image = "/Assets/Images/img.png"
                                             }).ToList();

            organicChartModes.AddRange(depMap);

            List<OrganicChartMode> staffMap = (from a in DbORG.StaffCoreData.Where(x => x.Active)
                                               join b in DbORG.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.UserGUID equals b.UserGUID into LJ1
                                               from RJ1 in LJ1.DefaultIfEmpty()
                                               join c in DbORG.codeJobTitlesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.JobTitleGUID equals c.JobTitleGUID into LJ2
                                               from RJ2 in LJ2.DefaultIfEmpty()
                                               select new OrganicChartMode
                                               {
                                                   id = a.UserGUID.ToString(),
                                                   parentId = a.PositionInOrganigram == "UnderDepartment" ? a.DepartmentGUID.ToString() : a.ReportToGUID.HasValue ? a.ReportToGUID.Value.ToString() : null,
                                                   category = "Staff",
                                                   title = RJ2.JobTitleDescription,
                                                   image = "/Assets/Images/img.png",
                                                   mail = a.EmailAddress,
                                                   adress = "",
                                                   name = RJ1.FirstName + " " + RJ1.Surname,
                                                   phone = ""
                                               }).ToList();

            organicChartModes.AddRange(staffMap);

            return Json(new { source = organicChartModes }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult StaffOnlyOrganicChartData()
        {
            if (!CMS.HasAction(Permissions.StaffProfile.Access, Apps.ORG))
            {
                return Json(DbORG.PermissionError());
            }
            List<OrganicChartMode> staffMap = (from a in DbORG.StaffCoreData.Where(x => x.Active)
                                               join b in DbORG.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.UserGUID equals b.UserGUID into LJ1
                                               from RJ1 in LJ1.DefaultIfEmpty()
                                               join c in DbORG.codeJobTitlesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.JobTitleGUID equals c.JobTitleGUID into LJ2
                                               from RJ2 in LJ2.DefaultIfEmpty()
                                               select new OrganicChartMode
                                               {
                                                   id = a.UserGUID.ToString(),
                                                   parentId = a.ReportToGUID.HasValue ? a.ReportToGUID.Value.ToString() : null,
                                                   category = "Staff",
                                                   title = RJ2.JobTitleDescription,
                                                   image = "/Assets/Images/img.png",
                                                   mail = a.EmailAddress,
                                                   adress = "",
                                                   name = RJ1.FirstName + " " + RJ1.Surname,
                                                   phone = ""
                                               }).ToList();

            staffMap.AddRange(staffMap);

            return Json(new { source = staffMap }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DepartmentsOnlyOrganicChartData()
        {

            List<OrganicChartMode> depMap = (from a in DbORG.codeDepartmentsConfigurations.Where(x => x.Active)
                                             join b in DbORG.codeDepartmentsLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.DepartmentGUID equals b.DepartmentGUID into LJ1
                                             from RJ1 in LJ1.DefaultIfEmpty()
                                             select new OrganicChartMode
                                             {
                                                 id = a.DepartmentGUID.ToString(),
                                                 parentId = a.ParentDepartmentGUID.HasValue ? a.ParentDepartmentGUID.Value.ToString() : null,
                                                 category = "Department",
                                                 name = RJ1.DepartmentDescription,
                                                 title = RJ1.DepartmentDescription,
                                                 phone = "",
                                                 mail = "",
                                                 adress = "",
                                                 image = "/Assets/Images/img.png"
                                             }).ToList();



            return Json(new { source = depMap }, JsonRequestBehavior.AllowGet);
        }
    }
}