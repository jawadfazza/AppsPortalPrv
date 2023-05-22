using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using RES_Repo.Globalization;
using AppsPortal.Library;
using AppsPortal.Models;
using AppsPortal.ViewModels;
using AutoMapper;
using LinqKit;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Resources;
using System.Web;
using System.Web.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.IO;

namespace AppsPortal.Controllers
{
    public class AuditController : PortalBaseController
    {

        //Temp function to create Audit before startup
        public ActionResult CreateAudit()
        {
            //this is a temp function to create audit for the exists records
            // still not competed
            List<dataAuditActions> daa = new List<dataAuditActions>();

            DbCMS.codeApplications.ToList().ForEach(x => daa.Add(new dataAuditActions
            {
                AuditGUID = Guid.NewGuid(),
                ActionGUID = Permissions.Applications.CreateGuid,
                UserProfileGUID = Guid.Parse("C6091E22-7720-46A3-B6B2-E57E09122D28"),
                RecordGUID = x.ApplicationGUID,
                ExecutionTime = DateTime.Now,
            }));

            DbCMS.codeApplicationsLanguages.ToList().ForEach(x => daa.Add(new dataAuditActions
            {
                AuditGUID = Guid.NewGuid(),
                ActionGUID = Permissions.Applications.CreateGuid,
                UserProfileGUID = Guid.Parse("C6091E22-7720-46A3-B6B2-E57E09122D28"),
                RecordGUID = x.ApplicationLanguageGUID,
                ExecutionTime = DateTime.Now,
            }));


            DbCMS.dataAuditActions.AddRange(daa);
            DbCMS.SaveChanges();
            return View();
        }


        #region Audit Record Resolver

        #region CMS
        private List<Guid> GetEffectedRecords(Guid ActionGUID, DateTime ExecutionTime)
        {
            List<Guid> Result = DbCMS.dataAuditActions.Where(x => x.ActionGUID == ActionGUID && x.ExecutionTime == ExecutionTime).Select(x => x.RecordGUID).ToList();
            return Result;
        }
        public ActionResult JSAuditcodeApplications(Guid ActionGUID, DateTime ExecutionTime)
        {
            List<Guid> EffectedRecords = GetEffectedRecords(ActionGUID, ExecutionTime);

            var Result = DbCMS.codeApplications.AsNoTracking().Where(x => EffectedRecords.Contains(x.ApplicationGUID))
                .Select(x => new { Message = x.ApplicationAcrynom }).ToList();

            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult JSAuditcodeApplicationsLanguages(Guid ActionGUID, DateTime ExecutionTime)
        {
            List<Guid> EffectedRecords = GetEffectedRecords(ActionGUID, ExecutionTime);

            var Result = DbCMS.codeApplicationsLanguages.AsNoTracking().Where(x => EffectedRecords.Contains(x.ApplicationLanguageGUID)).Select(x => new { Message = "[" + x.LanguageID + "] " + x.ApplicationDescription }).ToList();

            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult JSAuditcodeCountriesConfigurations(Guid ActionGUID, DateTime ExecutionTime)
        {
            List<Guid> EffectedRecords = GetEffectedRecords(ActionGUID, ExecutionTime);

            var Result = (from a in DbCMS.codeCountriesConfigurations.Where(x => EffectedRecords.Contains(x.CountryConfigurationGUID))
                          join b in DbCMS.codeCountriesLanguages.Where(x => x.LanguageID == LAN) on a.CountryGUID equals b.CountryGUID
                          join c in DbCMS.codeOrganizationsInstancesLanguages.Where(x => x.LanguageID == LAN) on a.OrganizationInstanceGUID equals c.OrganizationInstanceGUID
                          orderby b.CountryDescription
                          select new { Message = c.OrganizationInstanceDescription + " > " + b.CountryDescription }).ToList();

            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult JSAuditcodeActions(Guid ActionGUID, DateTime ExecutionTime)
        {
            List<Guid> EffectedRecords = GetEffectedRecords(ActionGUID, ExecutionTime);

            var Result = (from a in DbCMS.codeActions.Where(x => EffectedRecords.Contains(x.ActionGUID))
                          join b in DbCMS.codeActionsLanguages.Where(x => x.LanguageID == LAN) on a.ActionGUID equals b.ActionGUID
                          join c in DbCMS.codeActionsCategories on a.ActionCategoryGUID equals c.ActionCategoryGUID
                          join d in DbCMS.codeActionsCategoriesLanguages on c.ActionCategoryGUID equals d.ActionCategoryGUID
                          join e in DbCMS.codeActionsVerbs on a.ActionVerbGUID equals e.ActionVerbGUID
                          join f in DbCMS.codeActionsVerbsLanguages.Where(x => x.LanguageID == LAN) on e.ActionVerbGUID equals f.ActionVerbGUID
                          join j in DbCMS.codeApplications on c.ApplicationGUID equals j.ApplicationGUID
                          join h in DbCMS.codeApplicationsLanguages.Where(x => x.LanguageID == LAN) on j.ApplicationGUID equals h.ApplicationGUID
                          select new { Message = h.ApplicationDescription + " > " + d.ActionCategoryDescription + " > " + f.ActionVerbDescription }).ToList();

            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult JSAuditcodeActionsCategories(Guid ActionGUID, DateTime ExecutionTime)
        {
            List<Guid> EffectedRecords = GetEffectedRecords(ActionGUID, ExecutionTime);

            var Result = (from a in DbCMS.codeActionsCategories.Where(x => EffectedRecords.Contains(x.ActionCategoryGUID))
                          join b in DbCMS.codeActionsCategoriesLanguages.Where(x => x.LanguageID == LAN) on a.ActionCategoryGUID equals b.ActionCategoryGUID
                          join c in DbCMS.codeApplicationsLanguages on a.ApplicationGUID equals c.ApplicationGUID
                          select new { Message = "[" + b.LanguageID + "] " + c.ApplicationDescription + " > " + b.ActionCategoryDescription }).ToList();

            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult JSAuditcodeActionsCategoriesFactors(Guid ActionGUID, DateTime ExecutionTime)
        {
            List<Guid> EffectedRecords = GetEffectedRecords(ActionGUID, ExecutionTime);

            var Result = (from a in DbCMS.codeActionsCategoriesFactors.Where(x => EffectedRecords.Contains(x.ActionCategoryFactorGUID))
                          join b in DbCMS.codeActionsCategoriesLanguages.Where(x => x.LanguageID == LAN) on a.ActionCategoryGUID equals b.ActionCategoryGUID
                          join c in DbCMS.codeFactorsLanguages.Where(x => x.LanguageID == LAN) on a.FactorGUID equals c.FactorGUID
                          select new { Message = b.ActionCategoryDescription + " > " + c.FactorDescription }).ToList();

            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult JSAuditcodeActionsCategoriesLanguages(Guid ActionGUID, DateTime ExecutionTime)
        {
            List<Guid> EffectedRecords = GetEffectedRecords(ActionGUID, ExecutionTime);

            var Result = (from a in DbCMS.codeActionsCategoriesLanguages.Where(x => x.LanguageID == LAN && EffectedRecords.Contains(x.ActionCategoryLanguageGUID))
                          select new { Message = "[" + a.LanguageID + "] " + a.ActionCategoryDescription }).ToList();

            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult JSAuditcodeActionsEntities(Guid ActionGUID, DateTime ExecutionTime)
        {
            List<Guid> EffectedRecords = GetEffectedRecords(ActionGUID, ExecutionTime);

            var Result = (from a in DbCMS.codeActionsEntitiesLanguages.Where(x => x.LanguageID == LAN && EffectedRecords.Contains(x.ActionEntityGUID))
                          select new { Message = a.ActionEntityDescription }).ToList();

            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult JSAuditcodeActionsEntitiesLanguages(Guid ActionGUID, DateTime ExecutionTime)
        {
            List<Guid> EffectedRecords = GetEffectedRecords(ActionGUID, ExecutionTime);

            var Result = (from a in DbCMS.codeActionsEntitiesLanguages.Where(x => x.LanguageID == LAN && EffectedRecords.Contains(x.ActionEntityLanguageGUID))
                          select new { Message = "[" + a.LanguageID + "] " + a.ActionEntityDescription }).ToList();

            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult JSAuditcodeActionsLanguages(Guid ActionGUID, DateTime ExecutionTime)
        {
            List<Guid> EffectedRecords = GetEffectedRecords(ActionGUID, ExecutionTime);

            var Result = (from a in DbCMS.codeActionsLanguages.Where(x => x.LanguageID == LAN && EffectedRecords.Contains(x.ActionLanguageGUID))
                          select new { Message = "[" + a.LanguageID + "]" + a.ActionDetails }).ToList();

            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult JSAuditcodeActionsVerbs(Guid ActionGUID, DateTime ExecutionTime)
        {
            List<Guid> EffectedRecords = GetEffectedRecords(ActionGUID, ExecutionTime);

            var Result = (from a in DbCMS.codeActionsVerbsLanguages.Where(x => x.LanguageID == LAN && EffectedRecords.Contains(x.ActionVerbGUID))
                          select new { Message = a.ActionVerbDescription }).ToList();

            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult JSAuditcodeActionsVerbsLanguages(Guid ActionGUID, DateTime ExecutionTime)
        {
            List<Guid> EffectedRecords = GetEffectedRecords(ActionGUID, ExecutionTime);

            var Result = (from a in DbCMS.codeActionsVerbsLanguages.Where(x => x.LanguageID == LAN && EffectedRecords.Contains(x.ActionVerbLanguageGUID))
                          select new { Message = "[" + a.LanguageID + "]" + a.ActionVerbDescription }).ToList();

            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult JSAuditcodeCountries(Guid ActionGUID, DateTime ExecutionTime)
        {
            List<Guid> EffectedRecords = GetEffectedRecords(ActionGUID, ExecutionTime);

            var Result = (from a in DbCMS.codeCountriesLanguages.Where(x => x.LanguageID == LAN && EffectedRecords.Contains(x.CountryLanguageGUID))
                          select new { Message = "[" + a.LanguageID + "]" + a.CountryDescription }).ToList();

            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult JSAuditcodeCountriesLanguages(Guid ActionGUID, DateTime ExecutionTime)
        {
            List<Guid> EffectedRecords = GetEffectedRecords(ActionGUID, ExecutionTime);

            var Result = (from a in DbCMS.codeCountriesLanguages.Where(x => x.LanguageID == LAN && EffectedRecords.Contains(x.CountryGUID))
                          select new { Message = a.CountryDescription }).ToList();

            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult JSAuditcodeDepartments(Guid ActionGUID, DateTime ExecutionTime)
        {
            List<Guid> EffectedRecords = GetEffectedRecords(ActionGUID, ExecutionTime);

            var Result = (from a in DbCMS.codeDepartmentsLanguages.Where(x => x.LanguageID == LAN && EffectedRecords.Contains(x.DepartmentGUID))
                          select new { Message = "[" + a.LanguageID + "]" + a.DepartmentDescription }).ToList();

            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult JSAuditcodeDepartmentsConfigurations(Guid ActionGUID, DateTime ExecutionTime)
        {
            List<Guid> EffectedRecords = GetEffectedRecords(ActionGUID, ExecutionTime);

            var Result = (from a in DbCMS.codeDepartmentsConfigurations.Where(x => EffectedRecords.Contains(x.DepartmentConfigurationGUID))
                          join b in DbCMS.codeDepartmentsLanguages.Where(x => x.LanguageID == LAN) on a.DepartmentGUID equals b.DepartmentGUID
                          join c in DbCMS.codeOrganizationsInstancesLanguages.Where(x => x.LanguageID == LAN) on a.OrganizationInstanceGUID equals c.OrganizationInstanceGUID
                          orderby b.DepartmentDescription
                          select new { Message = c.OrganizationInstanceDescription + " > " + b.DepartmentDescription }).ToList();

            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult JSAuditcodeDepartmentsLanguages(Guid ActionGUID, DateTime ExecutionTime)
        {
            List<Guid> EffectedRecords = GetEffectedRecords(ActionGUID, ExecutionTime);

            var Result = (from a in DbCMS.codeDepartmentsLanguages.Where(x => x.LanguageID == LAN && EffectedRecords.Contains(x.DepartmentLanguageGUID))
                          select new { Message = a.DepartmentDescription }).ToList();

            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult JSAuditcodeDutyStations(Guid ActionGUID, DateTime ExecutionTime)
        {
            List<Guid> EffectedRecords = GetEffectedRecords(ActionGUID, ExecutionTime);

            var Result = (from a in DbCMS.codeDutyStationsLanguages.Where(x => x.LanguageID == LAN && EffectedRecords.Contains(x.DutyStationLanguageGUID))
                          select new { Message = a.DutyStationDescription }).ToList();

            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult JSAuditcodeDutyStationsConfigurations(Guid ActionGUID, DateTime ExecutionTime)
        {
            List<Guid> EffectedRecords = GetEffectedRecords(ActionGUID, ExecutionTime);

            var Result = (from a in DbCMS.codeDutyStationsConfigurations.Where(x => EffectedRecords.Contains(x.DutyStationConfigurationGUID))
                          join b in DbCMS.codeDutyStationsLanguages.Where(x => x.LanguageID == LAN) on a.DutyStationGUID equals b.DutyStationGUID
                          join c in DbCMS.codeOrganizationsInstancesLanguages.Where(x => x.LanguageID == LAN) on a.OrganizationInstanceGUID equals c.OrganizationInstanceGUID
                          orderby b.DutyStationDescription
                          select new { Message = c.OrganizationInstanceDescription + " > " + b.DutyStationDescription }).ToList();

            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult JSAuditcodeDutyStationsLanguages(Guid ActionGUID, DateTime ExecutionTime)
        {
            List<Guid> EffectedRecords = GetEffectedRecords(ActionGUID, ExecutionTime);

            var Result = (from a in DbCMS.codeDutyStationsLanguages.Where(x => x.LanguageID == LAN && EffectedRecords.Contains(x.DutyStationLanguageGUID))
                          select new { Message = "[" + a.LanguageID + "] " + a.DutyStationDescription }).ToList();

            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult JSAuditcodeFactors(Guid ActionGUID, DateTime ExecutionTime)
        {
            List<Guid> EffectedRecords = GetEffectedRecords(ActionGUID, ExecutionTime);

            var Result = (from a in DbCMS.codeFactorsLanguages.Where(x => x.LanguageID == LAN && EffectedRecords.Contains(x.FactorGUID))
                          select new { Message = a.FactorDescription }).ToList();

            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult JSAuditcodeFactorsLanguages(Guid ActionGUID, DateTime ExecutionTime)
        {
            List<Guid> EffectedRecords = GetEffectedRecords(ActionGUID, ExecutionTime);

            var Result = (from a in DbCMS.codeFactorsLanguages.Where(x => x.LanguageID == LAN && EffectedRecords.Contains(x.FactorLanguageGUID))
                          select new { Message = "[" + a.LanguageID + "] " + a.FactorDescription }).ToList();

            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult JSAuditcodeJobTitles(Guid ActionGUID, DateTime ExecutionTime)
        {
            List<Guid> EffectedRecords = GetEffectedRecords(ActionGUID, ExecutionTime);

            var Result = (from a in DbCMS.codeJobTitlesLanguages.Where(x => x.LanguageID == LAN && EffectedRecords.Contains(x.JobTitleGUID))
                          select new { Message = a.JobTitleDescription }).ToList();

            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult JSAuditcodeJobTitlesLanguages(Guid ActionGUID, DateTime ExecutionTime)
        {
            List<Guid> EffectedRecords = GetEffectedRecords(ActionGUID, ExecutionTime);

            var Result = (from a in DbCMS.codeJobTitlesLanguages.Where(x => x.LanguageID == LAN && EffectedRecords.Contains(x.JobTitleLanguageGUID))
                          select new { Message = "[" + a.LanguageID + "] " + a.JobTitleDescription }).ToList();

            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult JSAuditcodeLocations(Guid ActionGUID, DateTime ExecutionTime)
        {
            List<Guid> EffectedRecords = GetEffectedRecords(ActionGUID, ExecutionTime);

            var Result = (from a in DbCMS.codeLocationsLanguages.Where(x => x.LanguageID == LAN && EffectedRecords.Contains(x.LocationGUID))
                          select new { Message = a.LocationDescription }).ToList();

            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult JSAuditcodeLocationsLanguages(Guid ActionGUID, DateTime ExecutionTime)
        {
            List<Guid> EffectedRecords = GetEffectedRecords(ActionGUID, ExecutionTime);

            var Result = (from a in DbCMS.codeLocationsLanguages.Where(x => x.LanguageID == LAN && EffectedRecords.Contains(x.LocationLanguageGUID))
                          select new { Message = "[" + a.LanguageID + "] " + a.LocationDescription }).ToList();

            return Json(Result, JsonRequestBehavior.AllowGet);
        }


        public ActionResult JSAuditcodeMenus(Guid ActionGUID, DateTime ExecutionTime)
        {
            List<Guid> EffectedRecords = GetEffectedRecords(ActionGUID, ExecutionTime);

            var Result = (from a in DbCMS.codeMenusLanguages.Where(x => x.LanguageID == LAN && EffectedRecords.Contains(x.MenuGUID))
                          select new { Message = a.MenuDescription }).ToList();

            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult JSAuditcodeMenusLanguages(Guid ActionGUID, DateTime ExecutionTime)
        {
            List<Guid> EffectedRecords = GetEffectedRecords(ActionGUID, ExecutionTime);

            var Result = (from a in DbCMS.codeMenusLanguages.Where(x => x.LanguageID == LAN && EffectedRecords.Contains(x.MenuLanguageGUID))
                          select new { Message = "[" + a.LanguageID + "] " + a.MenuDescription }).ToList();

            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult JSAuditcodeOffices(Guid ActionGUID, DateTime ExecutionTime)
        {
            List<Guid> EffectedRecords = GetEffectedRecords(ActionGUID, ExecutionTime);

            var Result = (from a in DbCMS.codeOfficesLanguages.Where(x => x.LanguageID == LAN && EffectedRecords.Contains(x.OfficeGUID))
                          select new { Message = a.OfficeDescription }).ToList();

            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult JSAuditcodeOfficesConfigurations(Guid ActionGUID, DateTime ExecutionTime)
        {
            List<Guid> EffectedRecords = GetEffectedRecords(ActionGUID, ExecutionTime);

            var Result = (from a in DbCMS.codeOfficesConfigurations.Where(x => EffectedRecords.Contains(x.OfficeConfigurationGUID))
                          join b in DbCMS.codeOfficesLanguages.Where(x => x.LanguageID == LAN) on a.OfficeGUID equals b.OfficeGUID
                          join c in DbCMS.codeOrganizationsInstancesLanguages.Where(x => x.LanguageID == LAN) on a.OrganizationInstanceGUID equals c.OrganizationInstanceGUID
                          orderby b.OfficeDescription
                          select new { Message = c.OrganizationInstanceDescription + " > " + b.OfficeDescription }).ToList();

            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult JSAuditcodeOfficesLanguages(Guid ActionGUID, DateTime ExecutionTime)
        {
            List<Guid> EffectedRecords = GetEffectedRecords(ActionGUID, ExecutionTime);

            var Result = (from a in DbCMS.codeOfficesLanguages.Where(x => x.LanguageID == LAN && EffectedRecords.Contains(x.OfficeLanguageGUID))
                          select new { Message = "[" + a.LanguageID + "] " + a.OfficeDescription }).ToList();

            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult JSAuditcodeOperations(Guid ActionGUID, DateTime ExecutionTime)
        {
            List<Guid> EffectedRecords = GetEffectedRecords(ActionGUID, ExecutionTime);
            var Result = DbCMS.codeOperationsLanguages.Where(x => EffectedRecords.Contains(x.OperationGUID) && x.LanguageID == LAN).Select(x => new { Message = x.OperationDescription }).ToList();
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult JSAuditcodeOperationsLanguages(Guid ActionGUID, DateTime ExecutionTime)
        {
            List<Guid> EffectedRecords = GetEffectedRecords(ActionGUID, ExecutionTime);
            var Result = DbCMS.codeOperationsLanguages.AsNoTracking().Where(x => EffectedRecords.Contains(x.OperationLanguageGUID)).Select(x => new { Message = "[" + x.LanguageID + "] " + x.OperationDescription }).ToList();
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult JSAuditcodeOrganizations(Guid ActionGUID, DateTime ExecutionTime)
        {
            List<Guid> EffectedRecords = GetEffectedRecords(ActionGUID, ExecutionTime);
            var Result = DbCMS.codeOrganizations.Where(x => EffectedRecords.Contains(x.OrganizationGUID)).Select(x => new { Message = x.OrganizationShortName }).ToList();
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult JSAuditcodeOrganizationsInstances(Guid ActionGUID, DateTime ExecutionTime)
        {
            List<Guid> EffectedRecords = GetEffectedRecords(ActionGUID, ExecutionTime);
            var Result = DbCMS.codeOrganizationsInstancesLanguages.Where(x => EffectedRecords.Contains(x.OrganizationInstanceGUID) && x.LanguageID == LAN).Select(x => new { Message = x.OrganizationInstanceDescription }).ToList();
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult JSAuditcodeOrganizationsInstancesLanguages(Guid ActionGUID, DateTime ExecutionTime)
        {
            List<Guid> EffectedRecords = GetEffectedRecords(ActionGUID, ExecutionTime);
            var Result = DbCMS.codeOrganizationsInstancesLanguages.Where(x => EffectedRecords.Contains(x.OrganizationInstanceLanguageGUID) && x.LanguageID == LAN).Select(x => new { Message = "[" + x.LanguageID + "] " + x.OrganizationInstanceDescription }).ToList();
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult JSAuditcodeOrganizationsLanguages(Guid ActionGUID, DateTime ExecutionTime)
        {
            List<Guid> EffectedRecords = GetEffectedRecords(ActionGUID, ExecutionTime);
            var Result = DbCMS.codeOrganizationsLanguages.Where(x => EffectedRecords.Contains(x.OrganizationLanguageGUID) && x.LanguageID == LAN).Select(x => new { Message = "[" + x.LanguageID + "] " + x.OrganizationDescription }).ToList();
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult JSAuditcodeSitemaps(Guid ActionGUID, DateTime ExecutionTime)
        {
            List<Guid> EffectedRecords = GetEffectedRecords(ActionGUID, ExecutionTime);
            var Result = DbCMS.codeSitemapsLanguages.Where(x => EffectedRecords.Contains(x.SitemapGUID) && x.LanguageID == LAN).Select(x => new { Message = x.SitemapDescription }).ToList();
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult JSAuditcodeSitemapsLanguages(Guid ActionGUID, DateTime ExecutionTime)
        {
            List<Guid> EffectedRecords = GetEffectedRecords(ActionGUID, ExecutionTime);
            var Result = DbCMS.codeSitemapsLanguages.AsNoTracking().Where(x => EffectedRecords.Contains(x.SitemapLanguagesGUID)).Select(x => new { Message = "[" + x.LanguageID + "] " + x.SitemapDescription }).ToList();
            return Json(Result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult JSAuditcodeTables(Guid ActionGUID, DateTime ExecutionTime)
        {
            List<Guid> EffectedRecords = GetEffectedRecords(ActionGUID, ExecutionTime);
            var Result = DbCMS.codeTables.Where(x => EffectedRecords.Contains(x.TableGUID)).Select(x => new { Message = x.TableName }).ToList();
            return Json(Result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult JSAuditcodeTablesValues(Guid ActionGUID, DateTime ExecutionTime)
        {
            List<Guid> EffectedRecords = GetEffectedRecords(ActionGUID, ExecutionTime);
            var Result = DbCMS.codeTablesValuesLanguages.Where(x => EffectedRecords.Contains(x.ValueGUID) && x.LanguageID == LAN).Select(x => new { Message = x.codeTablesValues.codeTables.TableName + " > " + x.ValueDescription }).ToList();
            return Json(Result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult JSAuditcodeTablesValuesConfigurations(Guid ActionGUID, DateTime ExecutionTime)
        {
            List<Guid> EffectedRecords = GetEffectedRecords(ActionGUID, ExecutionTime);

            var Result = (from a in DbCMS.codeTablesValuesConfigurations.Where(x => EffectedRecords.Contains(x.TableValueConfigurationGUID))
                          join b in DbCMS.codeOrganizationsInstancesLanguages.Where(x => x.LanguageID == LAN) on a.OrganizationInstanceGUID equals b.OrganizationInstanceGUID
                          join c in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN) on a.ValueGUID equals c.ValueGUID
                          select new { Message = b.OrganizationInstanceDescription + " > " + c.ValueDescription }).ToList();

            return Json(Result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult JSAuditcodeTablesValuesLanguages(Guid ActionGUID, DateTime ExecutionTime)
        {
            List<Guid> EffectedRecords = GetEffectedRecords(ActionGUID, ExecutionTime);
            var Result = DbCMS.codeTablesValuesLanguages.Where(x => EffectedRecords.Contains(x.TableValueLanguageGUID)).Select(x => new { Message = "[" + x.LanguageID + "] " + x.ValueDescription }).ToList();
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult JSAuditcodeWorkingDaysConfigurations(Guid ActionGUID, DateTime ExecutionTime)
        {
            List<Guid> EffectedRecords = GetEffectedRecords(ActionGUID, ExecutionTime);
            var Result = (from a in DbCMS.codeWorkingDaysConfigurations.Where(x => EffectedRecords.Contains(x.WorkingDaysConfigurationGUID))
                          join b in DbCMS.codeDutyStationsConfigurations on a.DutyStationConfigurationGUID equals b.DutyStationConfigurationGUID
                          join c in DbCMS.codeOrganizationsInstancesLanguages.Where(x => x.LanguageID == LAN) on b.OrganizationInstanceGUID equals c.OrganizationInstanceGUID
                          join d in DbCMS.codeDutyStationsLanguages.Where(x => x.LanguageID == LAN) on b.DutyStationGUID equals d.DutyStationGUID
                          join e in DbCMS.codeTablesValues on a.DayGUID equals e.ValueGUID
                          join f in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN) on e.ValueGUID equals f.ValueGUID
                          orderby e.SortID
                          select new { Message = c.OrganizationInstanceDescription + " > " + d.DutyStationDescription + " > " + f.ValueDescription + ": " + a.FromTime.Hours + ":" + a.FromTime.Minutes + " - " + a.ToTime.Hours + ":" + a.ToTime.Minutes }).ToList();

            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult JSAudituserAccounts(Guid ActionGUID, DateTime ExecutionTime)
        {
            List<Guid> EffectedRecords = GetEffectedRecords(ActionGUID, ExecutionTime);
            var Result = (from a in DbCMS.userPersonalDetailsLanguage.Where(x => EffectedRecords.Contains(x.UserGUID) && x.LanguageID == LAN)
                          select new { Message = a.FirstName + " " + a.Surname }).ToList();
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult JSAudituserContactDetails(Guid ActionGUID, DateTime ExecutionTime)
        {
            List<Guid> EffectedRecords = GetEffectedRecords(ActionGUID, ExecutionTime);
            var Result = (from a in DbCMS.userContactDetails.Where(x => EffectedRecords.Contains(x.UserGUID))
                          select new { Message = a.PrimaryMobileCountryCode + " " + a.PrimaryMobileNumber }).ToList();
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult JSAudituserHomeAddress(Guid ActionGUID, DateTime ExecutionTime)
        {
            List<Guid> EffectedRecords = GetEffectedRecords(ActionGUID, ExecutionTime);
            var Result = (from a in DbCMS.userHomeAddressLanguage.Where(x => EffectedRecords.Contains(x.UserGUID) && x.LanguageID == LAN)
                          select new { Message = a.HomeAddress }).ToList();
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult JSAudituserHomeAddressLanguage(Guid ActionGUID, DateTime ExecutionTime)
        {
            List<Guid> EffectedRecords = GetEffectedRecords(ActionGUID, ExecutionTime);
            var Result = (from a in DbCMS.userHomeAddressLanguage.Where(x => EffectedRecords.Contains(x.UserGUID) && x.LanguageID == LAN)
                          select new { Message = "[" + a.LanguageID + "] " + a.HomeAddress }).ToList();
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult JSAudituserManagersHistory(Guid ActionGUID, DateTime ExecutionTime)
        {
            List<Guid> EffectedRecords = GetEffectedRecords(ActionGUID, ExecutionTime);
            var Result = (from a in DbCMS.userManagersHistory.Where(x => EffectedRecords.Contains(x.UserManagersHistoryGUID))
                          join b in DbCMS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN) on a.ManagerGUID equals b.UserGUID
                          join c in DbCMS.codeTablesValues.Where(x => x.TableGUID == LookupTables.ManagerTypes) on a.ManagerTypeGUID equals c.ValueGUID
                          join d in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN) on c.ValueGUID equals d.ValueGUID
                          select new { Message = d.ValueDescription + " " + b.FirstName + " " + b.Surname }).ToList();
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult JSAudituserPasswords(Guid ActionGUID, DateTime ExecutionTime)
        {
            List<Guid> EffectedRecords = GetEffectedRecords(ActionGUID, ExecutionTime);
            return Json(new { Message = "Confidential" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult JSAudituserPersonalDetails(Guid ActionGUID, DateTime ExecutionTime)
        {
            List<Guid> EffectedRecords = GetEffectedRecords(ActionGUID, ExecutionTime);
            var Result = (from a in DbCMS.userPersonalDetailsLanguage.Where(x => EffectedRecords.Contains(x.UserGUID) && x.LanguageID == LAN)
                          select new { Message = a.FirstName + " " + a.Surname }).ToList();
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult JSAudituserPersonalDetailsLanguage(Guid ActionGUID, DateTime ExecutionTime)
        {
            List<Guid> EffectedRecords = GetEffectedRecords(ActionGUID, ExecutionTime);
            var Result = (from a in DbCMS.userPersonalDetailsLanguage.Where(x => EffectedRecords.Contains(x.PersonalDetailsLanguageGUID) && x.LanguageID == LAN)
                          select new { Message = "[" + a.LanguageID + "] " + a.FirstName + " " + a.Surname }).ToList();
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult JSAudituserProfiles(Guid ActionGUID, DateTime ExecutionTime)
        {
            List<Guid> EffectedRecords = GetEffectedRecords(ActionGUID, ExecutionTime);
            var Result = (from a in DbCMS.userProfiles.Where(x => EffectedRecords.Contains(x.UserProfileGUID))
                          join b in DbCMS.codeOrganizationsInstancesLanguages.Where(x => x.LanguageID == LAN) on a.OrganizationInstanceGUID equals b.OrganizationInstanceGUID
                          join c in DbCMS.codeJobTitlesLanguages.Where(x => x.LanguageID == LAN) on a.JobTitleGUID equals c.JobTitleGUID
                          select new { Message = b.OrganizationInstanceDescription + " > " + c.JobTitleDescription }).ToList();
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult JSAudituserServiceHistory(Guid ActionGUID, DateTime ExecutionTime)
        {
            List<Guid> EffectedRecords = GetEffectedRecords(ActionGUID, ExecutionTime);
            var Result = (from a in DbCMS.userServiceHistory.Where(x => EffectedRecords.Contains(x.ServiceHistoryGUID))
                          join b in DbCMS.codeOrganizationsLanguages.Where(x => x.LanguageID == LAN) on a.OrganizationGUID equals b.OrganizationGUID
                          select new { Message = b.OrganizationDescription }).ToList();
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult JSAudituserStepsHistory(Guid ActionGUID, DateTime ExecutionTime)
        {
            List<Guid> EffectedRecords = GetEffectedRecords(ActionGUID, ExecutionTime);
            var Result = (from a in DbCMS.userStepsHistory.Where(x => EffectedRecords.Contains(x.UserStepsHistoryGUID))
                          select new { Message = a.Step }).ToList();
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult JSAudituserWorkAddress(Guid ActionGUID, DateTime ExecutionTime)
        {
            List<Guid> EffectedRecords = GetEffectedRecords(ActionGUID, ExecutionTime);
            var Result = (from a in DbCMS.userWorkAddressLanguage.Where(x => EffectedRecords.Contains(x.WorkAddressLanguageGUID) && x.LanguageID == LAN)
                          select new { Message = a.AreaName + " > " + a.BuildingName }).ToList();
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult JSAudituserWorkAddressLanguage(Guid ActionGUID, DateTime ExecutionTime)
        {

            List<Guid> EffectedRecords = GetEffectedRecords(ActionGUID, ExecutionTime);
            var Result = (from a in DbCMS.userWorkAddressLanguage.Where(x => EffectedRecords.Contains(x.WorkAddressLanguageGUID) && x.LanguageID == LAN)
                          select new { Message = "[" + a.LanguageID + "] " + a.AreaName + " > " + a.BuildingName }).ToList();
            return Json(Result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region EMT
        public ActionResult JSAuditdataMedicalBeneficiaryItemOut(Guid ActionGUID, DateTime ExecutionTime)
        {
            List<Guid> EffectedRecords = GetEffectedRecords(ActionGUID, ExecutionTime);

            var Result = DbEMT.dataMedicalBeneficiaryItemOut.AsNoTracking().Where(x => EffectedRecords.Contains(x.MedicalBeneficiaryItemOutGUID))
                .Select(x => new { Message = x.dataMedicalBeneficiary.IDNumber + x.dataMedicalBeneficiary.UNHCRNumber +" "+ x.dataMedicalBeneficiary.RefugeeFullName }).ToList();

            return Json(Result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult JSAuditdataMedicalBeneficiaryItemOutDetail(Guid ActionGUID, DateTime ExecutionTime)
        {
            List<Guid> EffectedRecords = GetEffectedRecords(ActionGUID, ExecutionTime);

            var Result =(from a in DbEMT.dataMedicalBeneficiaryItemOutDetail.AsNoTracking().Where(x => EffectedRecords.Contains(x.MedicalBeneficiaryItemOutDetailGUID))
                         join b in DbEMT.codeMedicalItem on a.MedicalItemGUID equals b.MedicalItemGUID
                         select new { Message = "(" + a.dataMedicalBeneficiaryItemOut.dataMedicalBeneficiary.IDNumber + a.dataMedicalBeneficiaryItemOut.dataMedicalBeneficiary.UNHCRNumber +" "+ a.dataMedicalBeneficiaryItemOut.dataMedicalBeneficiary.RefugeeFullName + ") > " + b.BrandName +" "+b.DoseQuantity}).ToList();

            return Json(Result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #endregion

        #region Global Audit Report
        [Route("Audit/GlobalAuditHistory/")]
        public ActionResult AuditHistory()
        {
            return View("~/Views/Audit/GlobalAuditReport.cshtml", new AuditReportFilterModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult GlobalAuditReport(AuditReportFilterModel Filter)
        {
            //System.Threading.Thread.Sleep(1000);

            var Result = DbCMS.spAuditReport(LAN, Filter.From, Filter.To, Filter.ActionCategoryGUID, Filter.ActionGUID, Filter.ActionVerbGUID, Filter.ExecutedByUserGUID, Filter.ExecutedByUserProfileGUID, Filter.FieldName, Filter.BeforeChange, Filter.AfterChange, Filter.OrganizationGUID, Filter.RankID, Filter.OrderBy)
                           .GroupBy(x => new { x.ActionGUID, x.ExecutionTime, x.ActionDescription, x.jsFunction, x.ExecutedBy, x.JobTitleDescription, x.OrganizationInstanceDescription })
                           .Select(x => new AuditModel
                           {
                               ActionDescription = x.Key.ActionDescription,
                               ExecutionTime = x.Key.ExecutionTime,
                               ActionGUID = x.Key.ActionGUID,
                               ExecutedBy = x.Key.ExecutedBy,
                               JobTitleDescription = x.Key.JobTitleDescription,
                               OrganizationInstanceDescription = x.Key.OrganizationInstanceDescription,
                               jsFunction = x.Key.jsFunction,
                               UpdatedFields = x.Where(y => y.FieldName != null).Select(z => new AuditFieldsModel { FieldName = z.FieldName, AfterChange = z.AfterChange, BeforeChange = z.BeforeChange }).Take(20).ToList()
                           });

            if (Filter.OrderBy == "ASC")
            {
                return Json(Result.OrderBy(x => x.ExecutionTime).ToList(), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(Result.OrderByDescending(x => x.ExecutionTime).ToList(), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Audit History on Interface
        public ActionResult GetAuditHistory(Guid RecordGUID)
        {
            List<AuditModel> model = DbCMS.spAuditHistory(LAN, RecordGUID)
                .GroupBy(x => new { x.ExecutionTime, x.ActionDescription, x.ExecutedBy, x.JobTitleDescription, x.OrganizationInstanceDescription })
                .Select(x => new AuditModel
                {
                    ActionDescription = x.Key.ActionDescription,
                    ExecutionTime = x.Key.ExecutionTime,
                    ExecutedBy = x.Key.ExecutedBy,
                    JobTitleDescription = x.Key.JobTitleDescription,
                    OrganizationInstanceDescription = x.Key.OrganizationInstanceDescription,
                    UpdatedFields = x.Where(y => y.FieldName != null).Select(z => new AuditFieldsModel { FieldName = z.FieldName, AfterChange = z.AfterChange, BeforeChange = z.BeforeChange }).ToList()
                }).ToList();


            return PartialView("~/Views/Audit/_AuditDetails.cshtml", model);
        }

        public ActionResult GetAuditHistoryGlobalizationVersion(Guid ParentGUID, List<Guid> ChildrenActions, List<Guid> ChildrenGUIDs)
        {

            string _ChildrenGUIDs = string.Join(",", ChildrenGUIDs);
            string _ChildrenActions = string.Join(",", ChildrenActions);


            List<AuditModel> model = DbCMS.spAuditHistoryGlobalizationVersion(LAN, ParentGUID, _ChildrenActions, _ChildrenGUIDs)
                                          .GroupBy(x => new { x.ExecutionTime, x.ActionDescription, x.ExecutedBy, x.JobTitleDescription, x.OrganizationInstanceDescription })
                                          .Select(x => new AuditModel
                                          {
                                              ActionDescription = x.Key.ActionDescription,
                                              ExecutionTime = x.Key.ExecutionTime,
                                              ExecutedBy = x.Key.ExecutedBy,
                                              JobTitleDescription = x.Key.JobTitleDescription,
                                              OrganizationInstanceDescription = x.Key.OrganizationInstanceDescription,
                                              UpdatedFields = x.Where(y => y.FieldName != null).Select(z => new AuditFieldsModel { FieldName = z.FieldName, AfterChange = z.AfterChange, BeforeChange = z.BeforeChange }).ToList()
                                          }).ToList();

            return PartialView("~/Views/Audit/_AuditDetails.cshtml", model);
        }
        #endregion

        #region Login History
        [Route("Audit/LoginHistory/")]
        public ActionResult LoginHistory()
        {
            return View("~/Views/Audit/LoginAuditReport.cshtml");
        }

        [Route("Audit/AuditLoginsDataTable/")]
        public JsonResult AuditLoginsDataTable(DataTableRecievedOptions options)
        {

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<LoginDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<LoginDataTableModel>(DataTable.Filters);
            }

            var All = (from a in DbCMS.dataAuditLogins.AsExpandable()
                       join b in DbCMS.userProfiles on a.UserProfileGUID equals b.UserProfileGUID into LJ1
                       from R1 in LJ1.DefaultIfEmpty()
                       join c in DbCMS.userServiceHistory on R1.ServiceHistoryGUID equals c.ServiceHistoryGUID into LJ2
                       from R2 in LJ2.DefaultIfEmpty()
                       join d in DbCMS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.Active) on R2.UserGUID equals d.UserGUID into LJ3
                       from R3 in LJ3.DefaultIfEmpty()
                       join e in DbCMS.codeJobTitlesLanguages.Where(x => x.LanguageID == LAN && x.Active) on R1.JobTitleGUID equals e.JobTitleGUID into LJ4
                       from R4 in LJ4.DefaultIfEmpty()
                       select new LoginDataTableModel
                       {
                           UserAuditGUID = a.UserAuditGUID,
                           FullName = R3.FirstName + " " + R3.Surname,
                           JobTitleDescription = R4.JobTitleDescription,
                           EmailAddress = R2.EmailAddress,
                           LoginTime = a.LoginTime,
                           LogoutTime = a.LogoutTime,
                           Browser = a.Browser,
                           Environment = a.Environment,
                           ComputerName = a.ComputerName,
                           MobileDeviceManufacturer = a.MobileDeviceManufacturer,
                           MobileDeviceModel = a.MobileDeviceModel,
                           UserHostAddress = a.UserHostAddress,
                           IsMobileDevice = a.IsMobileDevice,
                           CountryCode = a.CountryCode,
                           CountryName = a.CountryName,
                           City = a.City,
                           TimeZone = a.TimeZone,
                           Latitude = a.Latitude,
                           Longitude = a.Longitude,
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<LoginDataTableModel> Result = Mapper.Map<List<LoginDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Application Access Audit

        [Route("Audit/ApplicationAccessAuditReport/")]
        public ActionResult ApplicationAccessAuditReport()
        {
            return View("~/Views/Audit/ApplicationAccessAuditReport.cshtml");
        }

        [Route("Audit/ApplicationAccessAuditDataTable/")]
        public JsonResult ApplicationAccessAuditDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<ApplicationAccessDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<ApplicationAccessDataTableModel>(DataTable.Filters);
            }

            var All = (from a in DbCMS.dataApplicationAccessAudit.AsExpandable().Where(x => x.Active)
                       join b in DbCMS.StaffCoreData.AsExpandable() on a.UserGUID equals b.UserGUID
                       join c in DbCMS.codeApplications.AsExpandable() on a.ApplicationGUID equals c.ApplicationGUID
                       join d in DbCMS.codeApplicationsLanguages.AsExpandable().Where(x => x.Active && x.LanguageID == LAN) on a.ApplicationGUID equals d.ApplicationGUID
                       join e in DbCMS.userPersonalDetailsLanguage.AsExpandable().Where(x => x.Active && x.LanguageID == LAN) on a.UserGUID equals e.UserGUID
                       select new ApplicationAccessDataTableModel
                       {
                           AppAccessAuditGUID = a.AppAccessAuditGUID,
                           UserGUID = a.UserGUID,
                           FullName = e.FirstName + " " + e.Surname,
                           EmailAddress = b.EmailAddress,
                           ApplicationGUID = c.ApplicationGUID,
                           ApplicationDescription = d.ApplicationDescription,
                           ApplicationAcrynom = c.ApplicationAcrynom,
                           AccessedOn = a.AccessedOn
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<ApplicationAccessDataTableModel> Result = Mapper.Map<List<ApplicationAccessDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("Audit/ApplicationAccessAuditCreate/")]
        public ActionResult ApplicationAccessAuditCreate(Guid ApplicationGUID)
        {
            dataApplicationAccessAudit model = new dataApplicationAccessAudit();
            model.ApplicationGUID = ApplicationGUID;
            model.UserGUID = UserGUID;
            model.AccessedOn = DateTime.Now;

            DbCMS.CreateNoAudit(model);

            try { DbCMS.SaveChanges(); } catch { }

            return Json(new { success = true });

        }


        public ActionResult GenerateFullExcelReport()
        {
            var All = (from a in DbCMS.dataApplicationAccessAudit.AsExpandable().Where(x => x.Active)
                       join b in DbCMS.StaffCoreData.AsExpandable() on a.UserGUID equals b.UserGUID
                       join c in DbCMS.codeApplications.AsExpandable() on a.ApplicationGUID equals c.ApplicationGUID
                       join d in DbCMS.codeApplicationsLanguages.AsExpandable().Where(x => x.Active && x.LanguageID == LAN) on a.ApplicationGUID equals d.ApplicationGUID
                       join e in DbCMS.userPersonalDetailsLanguage.AsExpandable().Where(x => x.Active && x.LanguageID == LAN) on a.UserGUID equals e.UserGUID
                       orderby a.AccessedOn descending
                       select new ApplicationAccessDataTableModel
                       {
                           AppAccessAuditGUID = a.AppAccessAuditGUID,
                           UserGUID = a.UserGUID,
                           FullName = e.FirstName + " " + e.Surname,
                           EmailAddress = b.EmailAddress,
                           ApplicationGUID = c.ApplicationGUID,
                           ApplicationDescription = d.ApplicationDescription,
                           ApplicationAcrynom = c.ApplicationAcrynom,
                           AccessedOn = a.AccessedOn
                       }).ToList();

            // Creating an instance 
            // of ExcelPackage 
            ExcelPackage excel = new ExcelPackage();

            // name of the sheet 
            var workSheet = excel.Workbook.Worksheets.Add("Sheet1");

            // setting the properties 
            // of the work sheet  
            workSheet.TabColor = System.Drawing.Color.Black;
            workSheet.DefaultRowHeight = 12;

            // Setting the properties 
            // of the first row 
            workSheet.Row(1).Height = 20;
            workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(1).Style.Font.Bold = true;

            // Header of the Excel sheet 
            workSheet.Cells[1, 1].Value = "Full Name";
            workSheet.Cells[1, 2].Value = "Email Address";
            workSheet.Cells[1, 3].Value = "Application";
            workSheet.Cells[1, 4].Value = "Application Acrynom";
            workSheet.Cells[1, 5].Value = "Accessed On";

            // Inserting the article data into excel 
            // sheet by using the for each loop 
            // As we have values to the first row  
            // we will start with second row 
            int recordIndex = 2;

            foreach (var record in All)
            {
                workSheet.Cells[recordIndex, 1].Value = record.FullName;
                workSheet.Cells[recordIndex, 2].Value = record.EmailAddress;
                workSheet.Cells[recordIndex, 3].Value = record.ApplicationDescription;
                workSheet.Cells[recordIndex, 4].Value = record.ApplicationAcrynom;
                workSheet.Cells[recordIndex, 5].Value = record.AccessedOn.ToShortDateString();
                recordIndex++;
            }

            // By default, the column width is not  
            // set to auto fit for the content 
            // of the range, so we are using 
            // AutoFit() method here.  
            workSheet.Column(1).AutoFit();
            workSheet.Column(2).AutoFit();
            workSheet.Column(3).AutoFit();
            workSheet.Column(4).AutoFit();
            workSheet.Column(5).AutoFit();


            return File(excel.GetAsByteArray(), "application/octet-stream", "Applications Access Audit.xlsx");

        }
        #endregion

        #region User Permission Audit
        [Route("Audit/UserPermissionsAuditReport/")]
        public ActionResult UserPermissionsAuditReport()
        {
            return View("~/Views/Audit/UserPermissionsAuditReport.cshtml");
        }

        [Route("Audit/UserPermissionsAuditDataTable/")]
        public JsonResult UserPermissionsAuditDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<UserPermissionsAuditDataTable, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<UserPermissionsAuditDataTable>(DataTable.Filters);
            }

            var All = (from a in DbCMS.v_currentUserPermissions.AsExpandable()
                       select new UserPermissionsAuditDataTable
                       {
                           UserGUID = a.UserGUID,
                           FullName = a.FullName,
                           EmailAddress = a.EmailAddress,
                           ActionCategoryDescription = a.ActionCategoryDescription,
                           ActionEntityDescription = a.ActionEntityDescription,
                           ActionVerbDescription = a.ActionVerbDescription,
                           ApplicationAcrynom = a.ApplicationAcrynom,
                           ApplicationDescription = a.ApplicationDescription,
                           ApplicationGUID = a.ApplicationGUID.ToString()
                       }).Where(Predicate);


            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<UserPermissionsAuditDataTable> Result = Mapper.Map<List<UserPermissionsAuditDataTable>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}