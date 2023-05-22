using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using AppsPortal.BaseControllers;
using Microsoft.Reporting.WebForms;

namespace AppsPortal.Areas.AHD.Controllers
{
    public class StaffContactController : AHDBaseController
    {
        // GET: AHD/StaffContact
        public JsonResult GetStaffInformation()
        {
            string LAN = "EN";
            List<staffInfoVM> staffInfo = new List<staffInfoVM>();
            var res = DbAHD.StaffContactsInformation.ToList();
            staffInfo = res.Select(x => new staffInfoVM
            {
                EmpID = x.StaffinformationID,
                FirstName = x.FirstName,
                FamilyName = x.FamilyName,
                MiddleName = x.MiddleName,
                Prefix = x.Prefix,
                DepartmentName = x.DepartmentName,
                Governorate = x.Governorate,
                DutyStation = x.DutyStation,
                Company = x.Company,
                JobTitle = x.JobTitle,
                Address = x.Address,
                Mobile = x.Mobile1,
                Home = x.Home,
                Emailwork = x.Emailwork,
                EmailHome = x.EmailHome,

                //StockName = x.codeISSStock.codeISSStockLanguages.Select(f => f.codeISSStock.codeISSStockLanguages.Select(d => d.StockDescription).FirstOrDefault()).FirstOrDefault(),
            }
                    ).ToList();
            return Json(staffInfo,JsonRequestBehavior.AllowGet);
        }


        public class staffInfoVM
        {
            public int EmpID { get; set; }
            public string FirstName { get; set; }
            public string FamilyName { get; set; }
            public string MiddleName { get; set; }
            public string Prefix { get; set; }
            public string DepartmentName { get; set; }
            public string Governorate { get; set; }
            public string DutyStation { get; set; }
            public int SupervisorID { get; set; }
            public string Company { get; set; }
            public string JobTitle { get; set; }
            public string Address { get; set; }
            public string Mobile { get; set; }
            public string Home { get; set; }
            public string Emailwork { get; set; }
            public string EmailHome { get; set; }
        }
    }
}