using AppsPortal.BaseControllers;
using AppsPortal.Data;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.Models;
using AppsPortal.ViewModels;
using AutoMapper;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mime;
using System.Web.Mvc;

namespace AppsPortal.Controllers
{
    public class CatalogController : PortalBaseController
    {
        
        public ActionResult Index()
        {
            //As if a user opened PTD application
            Session[SessionKeys.CurrentApp] = Guid.Parse("26CBBBF1-9EF9-4A33-BF4F-D0BA3B2A1AAD");

            
            var ValueGenderList = new[]
            {
                new { ValueGUID = "M", ValueDescription = "Male" },
                new { ValueGUID = "F", ValueDescription = "Female" }
            }.ToList();

            var Countries = new[]
            {
                new { ValueGUID = "IRQ", ValueDescription = "Republic of Iraq" },
                new { ValueGUID = "AFG", ValueDescription = "Republic of Afghanistan" },
                new { ValueGUID = "SOM", ValueDescription = "Somalia" }
            }.ToList();

            ViewBag.GenderList = ValueGenderList;
            ViewBag.Countries = Countries;

            return View("index");
        }

        public ActionResult Partial(string PK)
        {
            var ValueList = new[]
            {
                new { ValueGUID = "M", ValueDescription = "Male" },
                new { ValueGUID = "F", ValueDescription = "Female" }
            }.ToList();
            ViewBag.List = ValueList;


            if (!string.IsNullOrEmpty(PK))
            {
                Repository<Test> model = new Repository<Test>(DbCMS);
                //var test = model.GetByID(Guid.Parse(PK));
                var test = DbCMS.GetByID<Test>(Guid.Parse(PK));
                if (test != null)
                {
                    return PartialView("_FormPartialView", test);
                }
                else
                {
                    return PartialView("_NoRecordFound");
                }
            }
            else
            {
                return PartialView("_FormPartialView", new Test());
            }
        }

        public ActionResult Update(string PK)
        {
            var ValueList = new[]
            {
                new { ValueGUID = "M", ValueDescription = "Male" },
                new { ValueGUID = "F", ValueDescription = "Female" }
            }.ToList();
            ViewBag.List = ValueList;


            if (!string.IsNullOrEmpty(PK))
            {
                Repository<Test> model = new Repository<Test>(DbCMS);
                //var test = model.GetByID(Guid.Parse(PK));
                var test = DbCMS.GetByID<Test>(Guid.Parse(PK));
                if (test != null)
                {
                    return View("Update", test);
                }
                else
                {
                    return View("NoRecordFound");
                }
            }
            else
            {
                return View("Update", new Test());
            }
        }

        public ActionResult TestDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<Test, bool>> TestPredicate;
            TestPredicate = b => true;

            if (DataTable.Filters.FilterRules != null)
            {
                TestPredicate = SearchHelper.CreateSearchPredicate<Test>(DataTable.Filters);
            }

            var All = DbCMS.Test.AsExpandable().Where(TestPredicate)
                .Select(f => new TestDataTable
                {
                    PK = f.IndividualID.ToString(),
                    GivenName = f.GivenName,
                    DateOfBirth = f.DateOfBirth.ToString(),
                    sexcode = f.sexcode,
                    GenderDescription = f.TestLookUp.GenderDescription,
                    OriginCountryCode = f.OriginCountryCode,
                    RegistrationDate = f.RegistrationDate.ToString(),
                    RV = f.TestRowVersion
                });

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);
            List<TestDataTable> Result = Mapper.Map<List<TestDataTable>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveChanges(Test model, string SubmitType)
        {
            if (ModelState.IsValid)
            {
                Repository<Test> Test = new Repository<Test>(DbCMS);
                Guid ActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");

                switch (SubmitType)
                {
                    case SubmitTypes.Create:
                        #region Create
                        model.IndividualID = Guid.NewGuid();
                        DbCMS.Create(model, ActionGUID);
                        DbCMS.SaveChanges();
                        return Json(new { succeeded = true, Message = "Record added successfully" });//From resource. Ayas 
                    #endregion

                    case SubmitTypes.Update:
                        #region Edit
                        DbCMS.Update(model, ActionGUID);
                        try
                        {
                            DbCMS.SaveChanges();
                        }
                        catch (DbUpdateConcurrencyException ex)
                        {
                            return Json(
                                new
                                {
                                    succeeded = false,
                                    concurrency = true,
                                    Message = "Data has been modified by another user. <a href='javascript:void(0)' onclick='window.location.reload(); '> Reload Page</a>"
                                });
                            //ViewBag.ErrorSummary = "Data has been modified by another user. <a href='javascript:void(0)' onclick='window.location.reload(); '> Reload Page</a>";
                            //return Content("Data has been modified by another user. <a href='javascript:void(0)' onclick='window.location.reload(); '> Reload Page</a>", MediaTypeNames.Text.Html);
                            //return PartialView("_ErrorSummary");
                            //((System.Data.Entity.Validation.DbEntityValidationException)ex).EntityValidationErrors
                        }
                        catch (Exception ex)
                        {

                        }
                        //The if below is for demonstration, if it Model/Ajax then return JSON, else, return RedirectToAction

                        if (Request.IsAjaxRequest())
                        {
                            return Json(new { succeeded = true, Message = "Record updated successfully" });//From resource. Ayas
                        }
                        else
                        {
                            //This code is smelly. Why redirect and hit the db again to bring the same data?. Ayas
                            return RedirectToAction("Update", "Catalog", new { ID = model.IndividualID });
                        } 
                    #endregion

                    case SubmitTypes.Delete:
                        SelectedRecords selectedRecords = new SelectedRecords();
                        selectedRecords.RV.Add(string.Join(",", model.TestRowVersion));

                        DateTime ExcecutionTime = DateTime.Now;

                        //List<Guid> DeletedRecords = new Repository<Test>(DbCMS).Delete(selectedRecords, null, ExcecutionTime, ActionGUID);
                        DbCMS.SaveChanges();
                        //return Json(Portal.RecordDeletedMessage(selectedRecords, DeletedRecords,string.Empty));
                        return null;
                    case SubmitTypes.Restore:
                        return null;
                    default:
                        return null;
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).ToList();
                ViewBag.Title = "New Modal Title";//Get it frm Resource. Ayas
                return PartialView("_FormPartialView", model);// Partial(id);
            }
        }

        [HttpPost]
        public ActionResult TestDataTableDelete(string[] DeleteID)
        {
            return null;

            //return Json(new Repository<Test>(DbCMS).DataTableDelete(DeleteID));

            //Guid ActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            //Repository<Test> Individuals = new Repository<Test>(DbCMS);
            //return Json(new { succeeded = true, Message = string.Format("{0} of {1} Records deleted successfully", Individuals.Delete(IDs, ActionGUID), (IDs.Length / 37)) });
        }
    }
}