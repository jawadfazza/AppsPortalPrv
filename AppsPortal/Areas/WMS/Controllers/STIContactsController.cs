using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AutoMapper;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using WMS_DAL.Model;
using WMS_DAL.ViewModels;

namespace AppsPortal.Areas.WMS.Controllers
{
    public class STIContactsController : WMSBaseController
    {
        // GET: WMS/STIContacts
        // GET: WMS/SIMPhones

        public ActionResult STIContactsIndex()
        {

            if (!CMS.HasAction(Permissions.STIContacts.Access, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            return View("~/Areas/WMS/Views/STIContacts/Index.cshtml");
        }

        [Route("WMS/STIContactsDataTable/")]
        public JsonResult STIContactsDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<STIContactsDataTable, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<STIContactsDataTable>(DataTable.Filters);
            }




            //Access is authorized by Access Action

            var All = (from a in DbAHD.StaffContactsInformation.AsExpandable()
                       select new STIContactsDataTable
                       {
                           StaffinformationID = a.StaffinformationID.ToString(),
                           FullName = a.FullName,
                           Mobile = a.Mobile1,
                           EmailAddress = a.Emailwork,
                           Active = (bool)a.Active

                       }).Where(Predicate);
            //string v = "SFCH2246E3DC";
            //var result = All.Where(x => x.SerialNumber == v).FirstOrDefault();
            All = SearchHelper.OrderByDynamic(All, "FullName", "ASC");

            List<STIContactsDataTable> Result = Mapper.Map<List<STIContactsDataTable>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }


        public ActionResult STIContactsCreate()
        {
            if (!CMS.HasAction(Permissions.STIContacts.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            return PartialView("~/Areas/WMS/Views/STIContacts/_ContactForm.cshtml",
                new STIContactsUpdateModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult STIContactsCreate(STIContactsUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.STIContacts.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (!ModelState.IsValid || model.Mobile1 == null || model.Mobile1.Length < 13 || model.FullName == null || model.FirstName == null || model.FamilyName == null) return PartialView("~/Areas/WMS/Views/STIContacts/_ContactForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;
            var lastId = DbAHD.StaffContactsInformation.Select(x => x.StaffinformationID).Max();
            AHD_DAL.Model.StaffContactsInformation toAdd = new AHD_DAL.Model.StaffContactsInformation
            {

                StaffinformationID = lastId + 1,
                FullName = model.FullName,
                FirstName = model.FirstName,
                FamilyName = model.FamilyName,
                MiddleName = model.MiddleName,
                Prefix = "NAN",
                DepartmentName = model.DepartmentName == null ? "Protection" : model.DepartmentName,
                Governorate = model.Governorate == null ? "Damascus" : model.Governorate,
                DutyStation = model.DutyStation == null ? "Damascus" : model.DutyStation,
                Company = "UNHCR-Syria",
                JobTitle = "NAN",
                Address = "NAN",
                Mobile1 = model.Mobile1,
                Home = "NAN",
                work = "NAN",
                Emailwork = model.Emailwork,
                Active = true

            };

            try
            {
                DbAHD.StaffContactsInformation.Add(toAdd);
                DbAHD.SaveChanges();

                return RedirectToAction("ConfirmItemUpdate");
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }
        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult STIContactsDataTableDelete(List<STIContactsDataTable> models)
        {
            if (!CMS.HasAction(Permissions.ItemModel.Delete, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            var ids = models.Select(f => f.StaffinformationID).ToList();
            var allids = DbAHD.StaffContactsInformation.Where(x => ids.Contains(x.StaffinformationID.ToString())).ToList();
            DbAHD.StaffContactsInformation.RemoveRange(allids);

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();

                return Json(DbWMS.PartialDeleteMessage(models, models, DataTableNames.STIContactsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        public ActionResult DeleteContact(int PK)
        {
            if (!CMS.HasAction(Permissions.STIContacts.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            return PartialView("~/Areas/WMS/Views/STIContacts/_DeleteContact.cshtml",
                new STIContactsUpdateModel { StaffinformationID = PK, Active = true });
        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ConfirmDeleteContact(STIContactsDataTable model)
        {
            if (!CMS.HasAction(Permissions.STIContacts.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            DateTime ExecutionTime = DateTime.Now;
            int idToDel = int.Parse(model.StaffinformationID);
            var myModel = DbAHD.StaffContactsInformation.Where(x => x.StaffinformationID == idToDel).FirstOrDefault();
            DbAHD.StaffContactsInformation.Remove(myModel);
            DbAHD.SaveChanges();
            try
            {

                return RedirectToAction("ConfirmItemUpdate");

            }
            catch (Exception ex)
            {
                return Json(new { success = 0 }, JsonRequestBehavior.AllowGet);
            }



        }
        public ActionResult ConfirmItemUpdate()
        {
            return View("~/Areas/WMS/Views/ModelConfirmation/ConfirmItemUpdate.cshtml");
        }

        public ActionResult SyncSTIMobileContactApp()
        {
            var currentPhons = DbWMS.v_StaffContactsListForSTI.ToList();
            var oldPhones = DbAHD.StaffContactsInformation.ToList();
            int maxId = oldPhones.Select(x => x.StaffinformationID).Max();
            List<StaffContactsInformation> toAdd = new List<StaffContactsInformation>();
            List<StaffContactsInformation> toRemove = new List<StaffContactsInformation>();
            foreach (var item in oldPhones.OrderBy(x => x.Emailwork))
            {
                var currtodel = currentPhons.Where(x => x.EmailWork.ToLower() == item.Emailwork.ToLower() && x.Mobile1 == item.Mobile1).FirstOrDefault();
                if (currtodel == null)
                {
                    var toDel = DbWMS.StaffContactsInformation.Where(x => x.StaffinformationID == item.StaffinformationID).FirstOrDefault();
                    toRemove.Add(toDel);
                }


            }
            DbWMS.StaffContactsInformation.RemoveRange(toRemove);
            string _qam = "Qamishli";
            string _coDam = "Damascus CO";
            DbWMS.SaveChanges();
            foreach (var item in currentPhons.OrderBy(x => x.EmailWork))
            {
                var curr = oldPhones.Where(x => x.Emailwork.ToLower() == item.EmailWork.ToLower()).FirstOrDefault();
                if (curr == null)
                {
                    StaffContactsInformation myAdd = new StaffContactsInformation
                    {
                        StaffinformationID = maxId + 1,
                        FullName = item.FullName,
                        FirstName = item.FirstName,
                        FamilyName = item.FamilyName,
                        MiddleName = item.MiddleName,
                        Prefix = item.Prefix,
                        DepartmentName = item.DepartmentName,
                        Governorate = item.Governorate == _qam ? "Al-Hasakeh" : (item.Governorate == _coDam ? "Damascus" : item.Governorate),
                        DutyStation = item.Governorate,
                        SupervisorID = item.SupervisorID,
                        Company = item.Company,
                        JobTitle = item.JobTitle,
                        Address = item.Address,
                        Mobile1 = item.Mobile1,
                        Home = item.Home,
                        work = item.Work,
                        Emailwork = item.EmailWork,

                        Active = true

                    };
                    toAdd.Add(myAdd);

                    maxId++;

                }


            }
            DbWMS.StaffContactsInformation.AddRange(toAdd);
            DbWMS.SaveChanges();

            var allContacts = DbWMS.StaffContactsInformation.ToList();
            var allEmails = allContacts.Select(x => x.Emailwork.ToLower()).Distinct().ToList();

            var allStaff = DbWMS.StaffCoreData.Where(x => allEmails.Contains(x.EmailAddress.ToLower())).ToList();
            DateTime ExecutionTime = DateTime.Now;
            foreach (var item in allContacts)
            {
            
                var staff = allStaff.Where(x => x.EmailAddress.ToLower() == item.Emailwork.ToLower()).FirstOrDefault();
                staff.OfficialMobileNumber = item.Mobile1;
                DbWMS.Update(staff, Permissions.WarehouseItemsEntry.UpdateGuid, ExecutionTime, DbCMS);
            }


            DbWMS.SaveChanges();
            StaffContactsInformation model = new StaffContactsInformation();
            return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);


        }

    }
}