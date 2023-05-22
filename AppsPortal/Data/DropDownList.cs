using AMS_DAL.Model;
using AppsPortal.BaseControllers;
using AppsPortal.Data;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.Models;
using AppsPortal.ViewModels;
using AHD_DAL.Model;
using DAS_DAL.Model;
using PCR_DAL.Model;
using REF_DAL.Model;
using RMS_DAL.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WMS_DAL.Model;
using EMT_DAL.Model;
using PMD_DAL.Model;

namespace AppsPortal.Data
{
    public class DropDownList
    {
        private CMSEntities DbCMS;
        private Guid UserGUID = HttpContext.Current.Session[SessionKeys.UserGUID] != null ? Guid.Parse(HttpContext.Current.Session[SessionKeys.UserGUID].ToString()) : Guid.Empty;
        private Guid UserProfileGUID = HttpContext.Current.Session[SessionKeys.UserProfileGUID] != null ? Guid.Parse(HttpContext.Current.Session[SessionKeys.UserProfileGUID].ToString()) : Guid.Empty;
        private Guid OrganizationInstanceGUID = HttpContext.Current.Session[SessionKeys.OrganizationInstanceGUID] != null ? Guid.Parse(HttpContext.Current.Session[SessionKeys.OrganizationInstanceGUID].ToString()) : Guid.Empty;
        string LAN = Extensions.Languages.CurrentLanguage().ToUpper();

        public DropDownList(CMSEntities DbCMS)
        {
            this.DbCMS = DbCMS;
        }


        public DropDownList()
        {
            this.DbCMS = new CMSEntities();
        }

        #region Drop Down List

        public SelectList Empty()
        {
            List<SelectListItem> Result = new List<SelectListItem> { new SelectListItem { Value = "", Text = "" } };
            return new SelectList(Result, "Value", "Text");
        }

        public SelectList OrganizationTypes(bool TextID = false)
        {
            var Result = (from a in DbCMS.codeTables.Where(x => x.TableGUID == LookupTables.OrganizationTypes && x.Active)
                          join b in DbCMS.codeTablesValues.Where(x => x.Active) on a.TableGUID equals b.TableGUID
                          join c in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on b.ValueGUID equals c.ValueGUID
                          orderby a.SortID
                          select new
                          {
                              Value = TextID ? c.ValueDescription : b.ValueGUID.ToString(),
                              Text = c.ValueDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList PartnersList()
        {
            Guid syriaOperation = Guid.Parse("699287E8-754D-4A63-B8DD-5344CFBAFD22");
            //Guid _nat = Guid.Parse("EDCAF08C-A795-41C1-8AF5-5F514B2C6E08");
            //Guid _gov = Guid.Parse("C3A7700C-4748-4ABF-9E1D-A85D03B24BB5");
            var Result = (from a in DbCMS.codeOrganizationsInstances.Where(x => x.Active && x.OperationGUID == syriaOperation)
                          join b in DbCMS.codeOrganizationsLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.OrganizationGUID equals b.OrganizationGUID

                          select new
                          {
                              Value = b.OrganizationGUID,
                              Text = b.OrganizationDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");

        }

        public SelectList PartnersListForPPA()
        {
            Guid syriaOperation = Guid.Parse("699287E8-754D-4A63-B8DD-5344CFBAFD22");
            //Guid _nat = Guid.Parse("EDCAF08C-A795-41C1-8AF5-5F514B2C6E08");
            //Guid _gov = Guid.Parse("C3A7700C-4748-4ABF-9E1D-A85D03B24BB5");
            Guid unhcrGUID = Guid.Parse("f9bd9237-ca5b-4753-a0eb-b1e4c33b1490");

            var Result = (from a in DbCMS.codeOrganizationsInstances.Where(x => x.Active && x.OperationGUID == syriaOperation)
                          join b in DbCMS.codeOrganizationsLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.OrganizationGUID equals b.OrganizationGUID
                          where a.OrganizationGUID != unhcrGUID
                          orderby b.OrganizationDescription
                          select new SelectListItem
                          {
                              Value = b.OrganizationGUID.ToString(),
                              Text = b.OrganizationDescription
                          }).ToList();
            string _unhcrGUID = "f9bd9237-ca5b-4753-a0eb-b1e4c33b1490";
            return new SelectList(Result, "Value", "Text");

        }

        public SelectList Organizations(bool TextID = false)
        {
            var Result = (from a in DbCMS.codeOrganizationsLanguages.Where(x => x.LanguageID == LAN && x.Active)
                          select new
                          {
                              Value = TextID ? a.OrganizationDescription : a.OrganizationGUID.ToString(),
                              Text = a.OrganizationDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList AuthorizedOrganizations(Guid ActionGUID, Guid OrganizationID)
        {
            List<Guid> FactorList = new CMS().GetAuthorizedList(ActionGUID, FactorTypes.Organizations);

            var Result = (from a in DbCMS.codeOrganizationsLanguages.Where(x => x.LanguageID == LAN && x.Active && FactorList.Contains(x.OrganizationGUID))
                          select new
                          {
                              Value = a.OrganizationGUID,
                              Text = a.OrganizationDescription
                          }).ToList();

            bool OrigianlNotExists = Result.Where(x => x.Value == OrganizationID).Count() == 0;

            if (OrganizationID != Guid.Empty && OrigianlNotExists)
            {
                Result = (from a in DbCMS.codeOrganizationsLanguages.Where(x => x.LanguageID == LAN && x.Active && x.OrganizationGUID == OrganizationID)
                          select new
                          {
                              Value = a.OrganizationGUID,
                              Text = a.OrganizationDescription
                          }).ToList();
            }

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList Organizations(Guid OperationGUID)
        {
            var Result = (from a in DbCMS.codeOrganizationsInstances.Where(x => x.Active && x.OperationGUID == OperationGUID)
                          join b in DbCMS.codeOrganizationsLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.OrganizationGUID equals b.OrganizationGUID
                          select new
                          {
                              Value = b.OrganizationGUID,
                              Text = b.OrganizationDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList OrganizationsByOperations(List<Guid> OperationGUIDs)
        {
            var Result = (from a in DbCMS.codeOrganizationsInstances.Where(x => x.Active && OperationGUIDs.Contains(x.OperationGUID.Value))
                          join b in DbCMS.codeOrganizationsLanguages.Where(x => x.LanguageID == LAN & x.Active) on a.OrganizationGUID equals b.OrganizationGUID
                          select new
                          {
                              Value = b.OrganizationGUID.ToString(),
                              Text = b.OrganizationDescription,
                              Group = a.OperationGUID.ToString()
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text", "Group", -1);
        }

        public SelectList OrganizationsInstances()
        {
            var Result = (from a in DbCMS.codeOrganizationsInstances
                          join b in DbCMS.codeOrganizationsInstancesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.OrganizationInstanceGUID equals b.OrganizationInstanceGUID
                          orderby b.OrganizationInstanceDescription
                          select new
                          {
                              Value = a.OrganizationInstanceGUID.ToString(),
                              Text = b.OrganizationInstanceDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList OrganizationsInstancesByProfile()
        {
            var userOperation = (from a in DbCMS.userProfiles.Where(x => x.UserProfileGUID == UserProfileGUID)
                                 join b in DbCMS.codeOrganizationsInstances on a.OrganizationInstanceGUID equals b.OrganizationInstanceGUID
                                 join c in DbCMS.codeOperations on b.OperationGUID equals c.OperationGUID
                                 select new
                                 {
                                     c.OperationGUID
                                 }).FirstOrDefault();
            var Result = (from a in DbCMS.codeOrganizationsInstances.Where(x => x.OperationGUID == userOperation.OperationGUID)
                          join b in DbCMS.codeOrganizationsInstancesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.OrganizationInstanceGUID equals b.OrganizationInstanceGUID
                          orderby b.OrganizationInstanceDescription
                          select new
                          {
                              Value = a.OrganizationInstanceGUID.ToString(),
                              Text = b.OrganizationInstanceDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList OrganizationsInstancesAcronymByProfile()
        {
            var userOperation = (from a in DbCMS.userProfiles.Where(x => x.UserProfileGUID == UserProfileGUID)
                                 join b in DbCMS.codeOrganizationsInstances on a.OrganizationInstanceGUID equals b.OrganizationInstanceGUID
                                 join c in DbCMS.codeOperations on b.OperationGUID equals c.OperationGUID

                                 select new
                                 {
                                     c.OperationGUID
                                 }).FirstOrDefault();
            var Result = (from a in DbCMS.codeOrganizationsInstances.Where(x => x.OperationGUID == userOperation.OperationGUID)
                          join b in DbCMS.codeOrganizationsInstancesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.OrganizationInstanceGUID equals b.OrganizationInstanceGUID
                          join c in DbCMS.codeOrganizations.Where(x => x.Active) on a.OrganizationGUID equals c.OrganizationGUID
                          join d in DbCMS.codePartnerCenter.Where(x => x.Active) on a.OrganizationInstanceGUID equals d.OrganizationInstanceGUID
                          orderby b.OrganizationInstanceDescription
                          select new
                          {
                              Value = a.OrganizationInstanceGUID.ToString(),
                              Text = c.OrganizationShortName
                          }).ToList().Distinct();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList OrganizationsInstancesAcronymByProfileAll()
        {
            var userOperation = (from a in DbCMS.userProfiles.Where(x => x.UserProfileGUID == UserProfileGUID)
                                 join b in DbCMS.codeOrganizationsInstances on a.OrganizationInstanceGUID equals b.OrganizationInstanceGUID
                                 join c in DbCMS.codeOperations on b.OperationGUID equals c.OperationGUID

                                 select new
                                 {
                                     c.OperationGUID
                                 }).FirstOrDefault();
            var Result = (from a in DbCMS.codeOrganizationsInstances.Where(x => x.OperationGUID == userOperation.OperationGUID)
                          join b in DbCMS.codeOrganizationsInstancesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.OrganizationInstanceGUID equals b.OrganizationInstanceGUID
                          join c in DbCMS.codeOrganizations.Where(x => x.Active) on a.OrganizationGUID equals c.OrganizationGUID
                          orderby b.OrganizationInstanceDescription
                          select new
                          {
                              Value = a.OrganizationInstanceGUID.ToString(),
                              Text = c.OrganizationShortName
                          }).ToList().Distinct();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList PartnerReportCompiledEndDate()
        {
            var Result = (from a in DbCMS.dataPartnerReportCompiled.AsEnumerable().Where(x => x.Active && x.EndDate != null)
                          join b in DbCMS.dataFileReport.AsEnumerable().Where(x => x.Active && x.ShowReport == true) on a.FileReportGUID equals b.FileReportGUID
                          orderby a.EndDate descending
                          select new
                          {
                              Value = a.EndDate,
                              Text = a.EndDate.Value.ToString("MMMM yyyy")
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList PartnerReportCompiledEndDateNotPublishedToo()
        {
            var Result = (from a in DbCMS.dataPartnerReportCompiled.AsEnumerable().Where(x => x.Active && x.EndDate != null)
                          orderby a.EndDate descending
                          select new
                          {
                              Value = a.EndDate,
                              Text = a.EndDate.Value.ToString("MMMM yyyy")
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList OrganizationsInstancesByProfileValueString()
        {
            var userOperation = (from a in DbCMS.userProfiles.Where(x => x.UserProfileGUID == UserProfileGUID)
                                 join b in DbCMS.codeOrganizationsInstances on a.OrganizationInstanceGUID equals b.OrganizationInstanceGUID
                                 join c in DbCMS.codeOperations on b.OperationGUID equals c.OperationGUID
                                 select new
                                 {
                                     c.OperationGUID
                                 }).FirstOrDefault();
            var Result = (from a in DbCMS.codeOrganizationsInstances.Where(x => x.OperationGUID == userOperation.OperationGUID)
                          join b in DbCMS.codeOrganizationsInstancesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.OrganizationInstanceGUID equals b.OrganizationInstanceGUID
                          orderby b.OrganizationInstanceDescription
                          select new
                          {
                              Value = b.OrganizationInstanceDescription.ToString(),
                              Text = b.OrganizationInstanceDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList DutyStationsByProfileValueString()
        {
            var userProfile = (from a in DbCMS.userProfiles.Where(x => x.UserProfileGUID == UserProfileGUID)
                               select new
                               {
                                   a.OrganizationInstanceGUID,
                                   a.userServiceHistory.OrganizationGUID,
                               }).FirstOrDefault();

            var Result = (from a in DbCMS.codeDutyStations
                          join b in DbCMS.codeDutyStationsLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.DutyStationGUID equals b.DutyStationGUID
                          join c in DbCMS.codeDutyStationsConfigurations.Where(x => x.Active && x.OrganizationInstanceGUID == userProfile.OrganizationInstanceGUID) on a.DutyStationGUID equals c.DutyStationGUID
                          orderby b.DutyStationDescription
                          select new
                          {
                              Value = b.DutyStationDescription,
                              Text = b.DutyStationDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList AuthorizedOrganizationsInstances(Guid ActionGUID, string SelectedValue = "")
        {
            List<Guid> AuthorizedList = new CMS().GetAuthorizedList(ActionGUID, FactorTypes.OrganizationsInstances);

            var Result = (from a in DbCMS.codeOrganizationsInstances.Where(x => x.Active && AuthorizedList.Contains(x.OrganizationInstanceGUID))
                          join b in DbCMS.codeOrganizationsInstancesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.OrganizationInstanceGUID equals b.OrganizationInstanceGUID
                          orderby b.OrganizationInstanceDescription
                          select new
                          {
                              Value = a.OrganizationInstanceGUID.ToString(),
                              Text = b.OrganizationInstanceDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text", SelectedValue);
        }

        public SelectList OrganizationsInstancesForServiceHistory(Guid ServiceHistoryGUID)
        {

            var Result = (from a in DbCMS.codeOrganizationsInstances.Where(x => x.Active)
                          join b in DbCMS.codeOrganizationsInstancesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.OrganizationInstanceGUID equals b.OrganizationInstanceGUID
                          join c in DbCMS.userServiceHistory.Where(x => x.Active && x.ServiceHistoryGUID == ServiceHistoryGUID) on a.OrganizationGUID equals c.OrganizationGUID
                          orderby b.OrganizationInstanceDescription
                          select new
                          {
                              Value = a.OrganizationInstanceGUID.ToString(),
                              Text = b.OrganizationInstanceDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList OrganizationsInstancesByOrganization(Guid OrganizationGUID)
        {

            var Result = (from a in DbCMS.codeOrganizationsInstances.Where(x => x.Active && x.OrganizationGUID == OrganizationGUID)
                          join b in DbCMS.codeOrganizationsInstancesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.OrganizationInstanceGUID equals b.OrganizationInstanceGUID
                          join c in DbCMS.codeCountriesConfigurations.Where(x => x.Active) on a.OrganizationInstanceGUID equals c.OrganizationInstanceGUID
                          join d in DbCMS.codeDutyStationsConfigurations.Where(x => x.Active) on a.OrganizationInstanceGUID equals d.OrganizationInstanceGUID
                          join e in DbCMS.codeDepartmentsConfigurations.Where(x => x.Active) on a.OrganizationInstanceGUID equals e.OrganizationInstanceGUID
                          select new
                          {
                              Value = b.OrganizationInstanceGUID.ToString(),
                              Text = b.OrganizationInstanceDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList DepartmentConfigurationOneValue(Guid DepartmentConfigurationGUID)
        {

            var Result = (from a in DbCMS.codeDepartmentsLanguages.Where(x => x.Active && x.LanguageID == LAN)
                          join b in DbCMS.codeDepartmentsConfigurations.Where(x => x.DepartmentConfigurationGUID == DepartmentConfigurationGUID) on a.DepartmentGUID equals b.DepartmentGUID
                          select new
                          {
                              Value = b.DepartmentConfigurationGUID.ToString(),
                              Text = a.DepartmentDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList RemoteOrganizationsInstancesByDepartmentConfiguration(Guid OrganizationInstanceGUID)
        {

            var Result = (from a in DbCMS.codeDepartmentsLanguages.Where(x => x.Active && x.LanguageID == LAN)
                          join b in DbCMS.codeDepartmentsConfigurations.Where(x => x.OrganizationInstanceGUID == OrganizationInstanceGUID) on a.DepartmentGUID equals b.DepartmentGUID
                          select new
                          {
                              Value = b.DepartmentConfigurationGUID.ToString(),
                              Text = a.DepartmentDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList OfficeTypes()
        {
            var Result = (from a in DbCMS.codeTables.Where(x => x.TableGUID == LookupTables.OfficeTypes && x.Active)
                          join b in DbCMS.codeTablesValues.Where(x => x.Active) on a.TableGUID equals b.TableGUID
                          join c in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on b.ValueGUID equals c.ValueGUID
                          orderby a.SortID
                          select new
                          {
                              Value = b.ValueGUID.ToString(),
                              Text = c.ValueDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList TokenColour()
        {
            var Result = (from a in DbCMS.codeTables.Where(x => x.TableGUID == LookupTables.TokenColour && x.Active)
                          join b in DbCMS.codeTablesValues.Where(x => x.Active) on a.TableGUID equals b.TableGUID
                          join c in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on b.ValueGUID equals c.ValueGUID
                          orderby a.SortID
                          select new
                          {
                              Value = b.ValueGUID.ToString(),
                              Text = c.ValueDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList Offices()
        {
            var Result = (from a in DbCMS.codeOfficesLanguages.Where(x => x.LanguageID == LAN && x.Active)
                          select new
                          {
                              Value = a.OfficeGUID.ToString(),
                              Text = a.OfficeDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList OfficesByDutyStation(Guid DutyStationGUID)
        {
            var Result = (from a in DbCMS.codeOfficesLanguages.Where(x => x.LanguageID == LAN && x.Active && x.codeOffices.DutyStationGUID == DutyStationGUID)
                          select new
                          {
                              Value = a.OfficeGUID.ToString(),
                              Text = a.OfficeDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList SecurityQuestions()
        {
            var Result = (from a in DbCMS.codeTables.Where(x => x.TableGUID == LookupTables.SecurityQuestions && x.Active)
                          join b in DbCMS.codeTablesValues.Where(x => x.Active) on a.TableGUID equals b.TableGUID
                          join c in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on b.ValueGUID equals c.ValueGUID
                          orderby c.ValueDescription
                          select new
                          {
                              Value = b.ValueGUID.ToString(),
                              Text = c.ValueDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList Sitemaps()
        {
            var Result = (from a in DbCMS.codeSitemaps.Where(x => x.Active)
                          join b in DbCMS.codeSitemapsLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.SitemapGUID equals b.SitemapGUID
                          orderby b.SitemapDescription
                          select new
                          {
                              Value = a.SitemapGUID.ToString(),
                              Text = b.SitemapDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList ApplicationStatus()
        {
            var Result = (from a in DbCMS.codeTables.Where(x => x.TableGUID == LookupTables.ApplicationStatus && x.Active)
                          join b in DbCMS.codeTablesValues.Where(x => x.Active) on a.TableGUID equals b.TableGUID
                          join c in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on b.ValueGUID equals c.ValueGUID
                          orderby a.SortID
                          select new
                          {
                              Value = b.ValueGUID.ToString(),
                              Text = c.ValueDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList ApplicationServerAccesibility()
        {
            List<SelectListItem> Result = new List<SelectListItem> {
                new SelectListItem { Value = "DEV", Text = "Dev" },
                new SelectListItem { Value = "BETA", Text = "Beta Internal" },
                new SelectListItem { Value = "BETAEXT", Text = "Beta External" },
                new SelectListItem { Value = "PRV", Text = "Production Private" },
                new SelectListItem { Value = "EXT", Text = "Production External" },
            };

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList Applications()
        {
            var Result = (from a in DbCMS.codeApplications.Where(a => a.Active)
                          join b in DbCMS.codeApplicationsLanguages.Where(x => x.LanguageID == LAN) on a.ApplicationGUID equals b.ApplicationGUID
                          orderby a.SortID
                          select new
                          {
                              Value = a.ApplicationGUID.ToString(),
                              Text = "[" + a.ApplicationAcrynom + "]" + " " + b.ApplicationDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList ApplicationMenus(Guid? ApplicationGUID)
        {
            var Result = (from a in DbCMS.codeMenus.Where(x => x.ApplicationGUID == ApplicationGUID && x.Active && x.ParentGUID == null)
                          join b in DbCMS.codeMenusLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.MenuGUID equals b.MenuGUID
                          select new
                          {
                              Value = a.MenuGUID.ToString(),
                              Text = b.MenuDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList DepartmentTypes()
        {
            var Result = (from a in DbCMS.codeTables.Where(x => x.TableGUID == LookupTables.DepartmentTypes && x.Active)
                          join b in DbCMS.codeTablesValues.Where(x => x.Active) on a.TableGUID equals b.TableGUID
                          join c in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on b.ValueGUID equals c.ValueGUID
                          orderby a.SortID
                          select new
                          {
                              Value = b.ValueGUID.ToString(),
                              Text = c.ValueDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList LookupValues(Guid TableGuid)
        {
            var Result = (from a in DbCMS.codeTables.Where(x => x.TableGUID == TableGuid && x.Active)
                          join b in DbCMS.codeTablesValues.Where(x => x.Active) on a.TableGUID equals b.TableGUID
                          join c in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on b.ValueGUID equals c.ValueGUID
                          orderby a.SortID
                          select new
                          {
                              Value = b.ValueGUID.ToString(),
                              Text = c.ValueDescription
                          }).OrderBy(x=>x.Text).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList DepartmentsForProfile(Guid UserProfileGUID)
        {
            if (UserProfileGUID == Guid.Empty)
            {
                return Empty();
            }

            var Result = (from a in DbCMS.userProfiles.Where(x => x.UserProfileGUID == UserProfileGUID)
                          select new
                          {
                              a.OrganizationInstanceGUID,
                              a.userServiceHistory.OrganizationGUID,
                          }).FirstOrDefault();

            return Departments(Result.OrganizationInstanceGUID);
        }

        public SelectList DepartmentAppointmentType(Guid? OrganizationInstanceGUID)
        {
            List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.AppointmentType.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();
            var Result = (from a in DbCMS.codeDepartmentsConfigurations.Where(x => x.Active && x.OrganizationInstanceGUID == OrganizationInstanceGUID)
                          join b in DbCMS.codeDepartmentsLanguages.Where(x => x.Active && x.LanguageID == LAN).Where(x => AuthorizedList.Contains(x.DepartmentGUID.ToString())) on a.DepartmentGUID equals b.DepartmentGUID

                          select new
                          {
                              Value = a.DepartmentGUID.ToString(),
                              Text = b.DepartmentDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList Departments(Guid OrganizationInstanceGUID)
        {
            var Result = (from a in DbCMS.codeDepartmentsConfigurations.Where(x => x.Active && x.OrganizationInstanceGUID == OrganizationInstanceGUID)
                          join b in DbCMS.codeDepartmentsLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.DepartmentGUID equals b.DepartmentGUID

                          select new
                          {
                              Value = a.DepartmentGUID.ToString(),
                              Text = b.DepartmentDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList Staffs(Guid OrganizationInstanceGUID)
        {
            var Result = (from a in DbCMS.codeDepartmentsConfigurations.Where(x => x.Active && x.OrganizationInstanceGUID == OrganizationInstanceGUID)
                          join b in DbCMS.codeDepartmentsLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.DepartmentGUID equals b.DepartmentGUID

                          select new
                          {
                              Value = a.DepartmentGUID.ToString(),
                              Text = b.DepartmentDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList Departments()
        {
            var Result = (from a in DbCMS.codeDepartments.Where(x => x.Active)
                          join b in DbCMS.codeDepartmentsLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.DepartmentGUID equals b.DepartmentGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()
                          orderby R1.DepartmentDescription
                          select new
                          {
                              Value = a.DepartmentGUID.ToString(),
                              Text = R1.DepartmentDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList DepartmentsForConfigurations(Guid OrganizationInstanceGUID, Guid DepartmentGUID)
        {
            var Result = (from a in DbCMS.codeDepartments.Where(x => x.Active)
                          join b in DbCMS.codeDepartmentsLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.DepartmentGUID equals b.DepartmentGUID
                          where !DbCMS.codeDepartmentsConfigurations.Where(x => x.OrganizationInstanceGUID == OrganizationInstanceGUID && x.Active).Where(d => d.DepartmentGUID != DepartmentGUID).Select(x => x.codeDepartments).Contains(a)
                          orderby b.DepartmentDescription
                          select new
                          {
                              Value = a.DepartmentGUID.ToString(),
                              Text = b.DepartmentDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList DepartmentsSelected(Guid FK)
        {
            var Result = (from a in DbCMS.codeDepartmentsConfigurations.Where(x => x.OrganizationInstanceGUID == FK && x.Active)
                          join b in DbCMS.codeDepartmentsLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.DepartmentGUID equals b.DepartmentGUID
                          select new
                          {
                              Value = b.DepartmentGUID.ToString(),
                              Text = b.DepartmentDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList TimeZones(string SelectedValue = null)
        {
            ReadOnlyCollection<TimeZoneInfo> zones = TimeZoneInfo.GetSystemTimeZones();
            var Result = (from a in zones
                          select new
                          {
                              Value = a.Id,
                              Text = a.DisplayName
                          }).ToList();

            return new SelectList(Result, "Value", "Text", SelectedValue);
        }

        public SelectList Operations(string SelectedValue = "")
        {
            var Result = (from a in DbCMS.codeOperations.Where(x => x.Active)
                          join b in DbCMS.codeOperationsLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.OperationGUID equals b.OperationGUID
                          orderby b.OperationDescription
                          select new
                          {
                              Value = b.OperationGUID.ToString(),
                              Text = b.OperationDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text", SelectedValue);
        }

        public SelectList Operations(Guid BureaGUID)
        {
            Guid OrganizationInstanceGUID = Guid.Parse(HttpContext.Current.Session[SessionKeys.OrganizationInstanceGUID].ToString());

            var Result = (from a in DbCMS.codeOperations.Where(x => x.BureauGUID == BureaGUID && x.Active)
                          join b in DbCMS.codeOperationsLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.OperationGUID equals b.OperationGUID
                          join c in DbCMS.codeCountriesConfigurations.Where(x => x.Active && x.OrganizationInstanceGUID == OrganizationInstanceGUID) on a.CountryGUID equals c.CountryGUID
                          orderby b.OperationDescription
                          select new
                          {
                              Value = b.OperationGUID.ToString(),
                              Text = b.OperationDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList OperationsForProfile(Guid ServiceHistoryGUID)
        {
            if (ServiceHistoryGUID == Guid.Empty)
            {
                return Empty();
            }

            Guid OrganizationGUID = DbCMS.userServiceHistory.Find(ServiceHistoryGUID).OrganizationGUID;

            return OperationsByOrganizationID(OrganizationGUID);
        }

        public SelectList OperationsByOrganizationID(Guid OrganizationGuid)
        {
            var Result = (from a in DbCMS.codeOrganizationsInstances.Where(x => x.OrganizationGUID == OrganizationGuid && x.Active)
                          join b in DbCMS.codeOperationsLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.OperationGUID equals b.OperationGUID
                          select new
                          {
                              Value = b.OperationGUID.ToString(),
                              Text = b.OperationDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList Governorates(Guid OperationGUID)
        {

            Guid LocationType = Guid.Parse("C970DDF5-31E5-47A1-BE76-AF15833D4E6A"); // Governorate

            var Result = (from a in DbCMS.codeLocations.Where(x => x.LocationTypeGUID == LocationType && x.Active)
                          join b in DbCMS.codeLocationsLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.LocationGUID equals b.LocationGUID
                          join c in DbCMS.codeOperations.Where(x => x.OperationGUID == OperationGUID) on a.CountryGUID equals c.CountryGUID
                          orderby b.LocationDescription
                          select new
                          {
                              Value = a.LocationGUID.ToString(),
                              Text = b.LocationDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList Governorates(List<Guid> OperationGUIDs)
        {
            Guid LocationType = Guid.Parse("00000000-0000-0000-0000-000000000030");

            var Result = (from a in DbCMS.codeLocations.Where(x => x.LocationTypeGUID == LocationType && x.Active)
                          join b in DbCMS.codeLocationsLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.LocationGUID equals b.LocationGUID
                          join c in DbCMS.codeOperations.Where(x => OperationGUIDs.Contains(x.OperationGUID)) on a.CountryGUID equals c.CountryGUID
                          orderby a.CountryGUID, b.LocationDescription
                          select new
                          {
                              Value = a.LocationGUID.ToString(),
                              Text = b.LocationDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList Countries()
        {
            Guid OrganizationInstanceGUID = Guid.Parse(HttpContext.Current.Session[SessionKeys.OrganizationInstanceGUID].ToString());

            var Result = (from a in DbCMS.codeCountriesConfigurations.Where(x => x.OrganizationInstanceGUID == OrganizationInstanceGUID && x.Active)
                          join b in DbCMS.codeCountriesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.CountryGUID equals b.CountryGUID
                          orderby b.CountryDescription
                          select new
                          {
                              Value = a.CountryGUID.ToString(),
                              Text = b.CountryDescription
                          }).ToList();

            //return Countries;
            return new SelectList(Result, "Value", "Text");
        }

        public SelectList CountriesA3Code()
        {
            Guid OrganizationInstanceGUID = Guid.Parse(HttpContext.Current.Session[SessionKeys.OrganizationInstanceGUID].ToString());

            var Result = (from a in DbCMS.codeCountriesConfigurations.Where(x => x.OrganizationInstanceGUID == OrganizationInstanceGUID && x.Active)
                          join b in DbCMS.codeCountriesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.CountryGUID equals b.CountryGUID
                          orderby b.CountryDescription
                          select new
                          {
                              Value = a.codeCountries.CountryA3Code,
                              Text = b.CountryDescription
                          }).ToList();

            //return Countries;
            return new SelectList(Result, "Value", "Text");
        }

        public SelectList DutyStations()
        {
            var Result = (from a in DbCMS.codeDutyStations.Where(x => x.Active)
                          join b in DbCMS.codeDutyStationsLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.DutyStationGUID equals b.DutyStationGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()
                          select new
                          {
                              Value = a.DutyStationGUID.ToString(),
                              Text = R1.DutyStationDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        internal object UpdateDirectSupervisor(string pK1)
        {
            var Result = (from a in DbCMS.StaffCoreData.Where(x => x.Active  && x.StaffStatusGUID.ToString() == "B9CD375C-A576-4AA4-8AF4-FF3C1C4E3611")
                          join b in DbCMS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN)
                          on a.UserGUID equals b.UserGUID
                          orderby b.FirstName, b.Surname
                          select new
                          {
                              Value = a.UserGUID.ToString(),
                              Text = b.FirstName + " " + b.Surname

                          }).ToList();
            return new SelectList(Result, "Value", "Text");
        }

        public SelectList DutyStations(Guid OrganizationInstanceGuid)
        {
            if (OrganizationInstanceGuid == Guid.Empty)
            {
                return Empty();
            }
            var Result = (from a in DbCMS.codeDutyStationsConfigurations.Where(x => x.OrganizationInstanceGUID == OrganizationInstanceGuid && x.Active)
                          join b in DbCMS.codeDutyStationsLanguages.Where(l => l.LanguageID == LAN && l.Active) on a.DutyStationGUID equals b.DutyStationGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()
                          orderby R1.DutyStationDescription
                          select new
                          {
                              Value = a.DutyStationGUID.ToString(),
                              Text = R1.DutyStationDescription
                          }).ToList();
            return new SelectList(Result, "Value", "Text");
        }

        public SelectList PartnerCentersByDutyStation(string OrganizationInstanceGuid)
        {

            if (OrganizationInstanceGuid == "")
            {
                return Empty();
            }
            string[] OrganizationInstanceGuids = OrganizationInstanceGuid.Split(',');
            List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.CommunityCenterCode.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();

            var Result = (from a in DbCMS.codePartnerCenter.Where(x => x.Active && x.OrganizationInstanceGUID.ToString() == OrganizationInstanceGuid).Where(x => AuthorizedList.Contains(x.OrganizationInstanceGUID + "," + x.DutyStationGUID))
                          join b in DbCMS.codePartnerCenterLanguage.Where(l => l.LanguageID == LAN && l.Active) on a.PartnerCenterGUID equals b.PartnerCenterGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()
                          join c in DbCMS.codeDutyStationsLanguages.Where(l => l.LanguageID == LAN && l.Active) on a.DutyStationGUID equals c.DutyStationGUID into LJ2
                          from R2 in LJ2.DefaultIfEmpty()
                          join c in DbCMS.codeOrganizationsInstances.Where(l => l.Active) on a.OrganizationInstanceGUID equals c.OrganizationInstanceGUID into LJ3
                          from R3 in LJ3.DefaultIfEmpty()
                          join d in DbCMS.codeOrganizations.Where(l => l.Active) on R3.OrganizationGUID equals d.OrganizationGUID into LJ4
                          from R4 in LJ4.DefaultIfEmpty()
                          orderby new { R4.OrganizationShortName, R2.DutyStationDescription }
                          select new
                          {
                              Value = a.PartnerCenterGUID.ToString(),
                              Text = "[" + R4.OrganizationShortName + "-" + R2.DutyStationDescription + "] - " + R1.PartnerCenterDescription
                          }).ToList();
            return new SelectList(Result, "Value", "Text");
        }

        public SelectList PartnerCenterByOrganizationInstanceByDutyStation(string OrganizationInstanceGuid, string DutyStationGUID)
        {

            if (OrganizationInstanceGuid == "")
            {
                OrganizationInstanceGuid = string.Join(",", OrganizationsInstancesAcronymByProfile().Select(x => x.Value).ToList().ConvertAll(Guid.Parse).ToArray());
            }
            if (DutyStationGUID == "")
            {
                DutyStationGUID = string.Join(",", DutyStationsByProfile().Select(x => x.Value).ToList().ConvertAll(Guid.Parse).ToArray());
            }
            string[] OrganizationInstanceGuids = OrganizationInstanceGuid.Split(',');
            string[] DutyStationGUIDs = DutyStationGUID.Split(',');
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.PartnerCenter.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();

            var Result = (from a in DbCMS.codePartnerCenter.Where(x => DutyStationGUIDs.Contains(x.DutyStationGUID.ToString()) && OrganizationInstanceGuids.Contains(x.OrganizationInstanceGUID.ToString()) && x.Active)
                          join b in DbCMS.codePartnerCenterLanguage.Where(l => l.LanguageID == LAN && l.Active) on a.PartnerCenterGUID equals b.PartnerCenterGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()
                          join c in DbCMS.codeDutyStationsLanguages.Where(l => l.LanguageID == LAN && l.Active) on a.DutyStationGUID equals c.DutyStationGUID into LJ2
                          from R2 in LJ2.DefaultIfEmpty()
                          join c in DbCMS.codeOrganizationsInstances.Where(l => l.Active) on a.OrganizationInstanceGUID equals c.OrganizationInstanceGUID into LJ3
                          from R3 in LJ3.DefaultIfEmpty()
                          join d in DbCMS.codeOrganizations.Where(l => l.Active) on R3.OrganizationGUID equals d.OrganizationGUID into LJ4
                          from R4 in LJ4.DefaultIfEmpty()
                          orderby new { R4.OrganizationShortName, R2.DutyStationDescription }
                          select new
                          {
                              Value = a.PartnerCenterGUID.ToString(),
                              Text = R1.PartnerCenterDescription,
                              Group = a.OrganizationInstanceGUID + "," + a.DutyStationGUID
                          }).ToList();
            return new SelectList(Result, "Value", "Text", "Group", -1);
        }

        public SelectList DutyStationsForProfile(Guid UserProfileGUID)
        {
            if (UserProfileGUID == Guid.Empty)
            {
                return Empty();
            }

            var Result = (from a in DbCMS.userProfiles.Where(x => x.UserProfileGUID == UserProfileGUID)
                          select new
                          {
                              a.OrganizationInstanceGUID,
                              a.userServiceHistory.OrganizationGUID
                          }).FirstOrDefault();

            return DutyStations(Result.OrganizationInstanceGUID);
        }

        public SelectList DutyStationsByProfile()
        {
            if (UserProfileGUID == Guid.Empty)
            {
                return Empty();
            }

            var Result = (from a in DbCMS.userProfiles.Where(x => x.UserProfileGUID == UserProfileGUID)

                          select new
                          {
                              a.OrganizationInstanceGUID,
                              a.userServiceHistory.OrganizationGUID
                          }).FirstOrDefault();

            return DutyStations(Result.OrganizationInstanceGUID);
        }

        public SelectList DutyStationsSelected(Guid FK)
        {
            var Result = (from a in DbCMS.codeDutyStationsConfigurations.Where(x => x.OrganizationInstanceGUID == FK && x.Active)
                          join b in DbCMS.codeDutyStationsLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.DutyStationGUID equals b.DutyStationGUID
                          select new
                          {
                              Value = b.DutyStationGUID.ToString(),
                              Text = b.DutyStationDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList DutyStationsOneValue(Guid? PK)
        {
            if (PK == Guid.Empty || PK == null)
            {
                return Empty();
            }
            var Result = (from a in DbCMS.codeDutyStations.Where(x => x.DutyStationGUID == PK && x.Active)
                          join b in DbCMS.codeDutyStationsLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.DutyStationGUID equals b.DutyStationGUID
                          select new
                          {
                              Value = b.DutyStationGUID.ToString(),
                              Text = b.DutyStationDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList DutyStationsMultipleValue(Guid? PK)
        {
            if (PK == Guid.Empty || PK == null)
            {
                return Empty();
            }
            if (UserProfileGUID == Guid.Empty)
            {
                return Empty();
            }

            var Result = (from a in DbCMS.userProfiles.Where(x => x.UserProfileGUID == UserProfileGUID)
                          select new
                          {
                              a.OrganizationInstanceGUID,
                              a.userServiceHistory.OrganizationGUID
                          }).FirstOrDefault();
            if (Result != null) { return DutyStations(Result.OrganizationInstanceGUID); }
            else { DutyStationsOneValue(PK); }
            return Empty();
        }

        public SelectList PartnerCenterOneValue(Guid PK)
        {

            var Result = (from a in DbCMS.codePartnerCenter.Where(x => x.PartnerCenterGUID == PK && x.Active)
                          join b in DbCMS.codePartnerCenterLanguage.Where(l => l.LanguageID == LAN && l.Active) on a.PartnerCenterGUID equals b.PartnerCenterGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()
                          join c in DbCMS.codeDutyStationsLanguages.Where(l => l.LanguageID == LAN && l.Active) on a.DutyStationGUID equals c.DutyStationGUID into LJ2
                          from R2 in LJ2.DefaultIfEmpty()
                          orderby R2.DutyStationDescription
                          select new
                          {
                              Value = a.PartnerCenterGUID.ToString(),
                              Text = "[" + R2.DutyStationDescription + "] - " + R1.PartnerCenterDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList PartnerCenterMultipleValues(List<Guid> PKs)
        {
            if (PKs == null)
            {
                PKs = new List<Guid>();
            }
            PKs.Add(Guid.Empty);
            var Result = (from a in DbCMS.codePartnerCenter.Where(x => PKs.Contains(x.PartnerCenterGUID) && x.Active)
                          join b in DbCMS.codePartnerCenterLanguage.Where(l => l.LanguageID == LAN && l.Active) on a.PartnerCenterGUID equals b.PartnerCenterGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()
                          join c in DbCMS.codeDutyStationsLanguages.Where(l => l.LanguageID == LAN && l.Active) on a.DutyStationGUID equals c.DutyStationGUID into LJ2
                          from R2 in LJ2.DefaultIfEmpty()
                          orderby R2.DutyStationDescription
                          select new
                          {
                              Value = a.PartnerCenterGUID.ToString(),
                              Text = "[" + R2.DutyStationDescription + "] - " + R1.PartnerCenterDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList PartnerCenterAll()
        {

            var Result = (from a in DbCMS.codePartnerCenter.Where(x => x.Active)
                          join b in DbCMS.codePartnerCenterLanguage.Where(l => l.LanguageID == LAN && l.Active) on a.PartnerCenterGUID equals b.PartnerCenterGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()
                          join c in DbCMS.codeOrganizationsInstances.Where(l => l.Active) on a.OrganizationInstanceGUID equals c.OrganizationInstanceGUID
                          join d in DbCMS.codeOrganizations.Where(l => l.Active) on c.OrganizationGUID equals d.OrganizationGUID into LJ2
                          from R2 in LJ2.DefaultIfEmpty()
                          orderby R2.OrganizationShortName
                          select new
                          {
                              Value = a.PartnerCenterGUID.ToString(),
                              Text = R1.PartnerCenterDescription

                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList PartnerCenterAll(Guid[] DutyStations, Guid[] Organizations)
        {

            var Result = (from a in DbCMS.codePartnerCenter.Where(x => x.Active && Organizations.Contains(x.OrganizationInstanceGUID) && DutyStations.Contains(x.DutyStationGUID))
                          join b in DbCMS.codePartnerCenterLanguage.Where(l => l.LanguageID == LAN && l.Active) on a.PartnerCenterGUID equals b.PartnerCenterGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()
                          join c in DbCMS.codeOrganizationsInstances.Where(l => l.Active) on a.OrganizationInstanceGUID equals c.OrganizationInstanceGUID
                          join d in DbCMS.codeOrganizations.Where(l => l.Active) on c.OrganizationGUID equals d.OrganizationGUID into LJ2
                          from R2 in LJ2.DefaultIfEmpty()
                          orderby R2.OrganizationShortName
                          select new
                          {
                              Value = a.PartnerCenterGUID.ToString(),
                              Text = R1.PartnerCenterDescription

                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList JobTitles()
        {
            var Result = (from a in DbCMS.codeJobTitles.Where(x => x.Active)
                          join b in DbCMS.codeJobTitlesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.JobTitleGUID equals b.JobTitleGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()
                          orderby R1.JobTitleDescription
                          select new
                          {
                              Value = a.JobTitleGUID.ToString(),
                              Text = R1.JobTitleDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList BloodGroup()
        {
            List<SelectListItem> Result = new List<SelectListItem>() {
                new SelectListItem { Value = "A+", Text = "A+" },
                new SelectListItem { Value = "A-", Text = "A-" },
                new SelectListItem { Value = "AB+", Text = "AB+" },
                new SelectListItem { Value = "AB-", Text = "AB-" },
                new SelectListItem { Value = "B+", Text = "B+" },
                new SelectListItem { Value = "B-", Text = "B-" },
                new SelectListItem { Value = "O+", Text = "O+" },
                new SelectListItem { Value = "O-", Text = "O-" }
            };

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList WorkingTime(string SelectedValue = "")
        {
            List<SelectListItem> Result = new List<SelectListItem>() {
                new SelectListItem { Value = "", Text = "" },
                new SelectListItem { Value = "08:00:00", Text = "08:00" },
                new SelectListItem { Value = "08:30:00", Text = "08:30" },
                new SelectListItem { Value = "09:00:00", Text = "09:00" },
                new SelectListItem { Value = "09:30:00", Text = "09:30" },
                new SelectListItem { Value = "10:00:00", Text = "10:00" },
                new SelectListItem { Value = "10:30:00", Text = "10:30" },
                new SelectListItem { Value = "11:00:00", Text = "11:00" },
                new SelectListItem { Value = "11:30:00", Text = "11:30" },
                new SelectListItem { Value = "12:00:00", Text = "12:00" },
                new SelectListItem { Value = "12:30:00", Text = "12:30" },
                new SelectListItem { Value = "13:00:00", Text = "13:00" },
                new SelectListItem { Value = "13:30:00", Text = "13:30" },
                new SelectListItem { Value = "14:00:00", Text = "14:00" },
                new SelectListItem { Value = "14:30:00", Text = "14:30" },
                new SelectListItem { Value = "15:00:00", Text = "15:00" },
                new SelectListItem { Value = "15:30:00", Text = "15:30" },
                new SelectListItem { Value = "16:00:00", Text = "16:00" },
                new SelectListItem { Value = "16:30:00", Text = "16:30" },
                new SelectListItem { Value = "17:00:00", Text = "17:00" },
                new SelectListItem { Value = "17:30:00", Text = "17:30" },
                new SelectListItem { Value = "18:00:00", Text = "18:00" },
                new SelectListItem { Value = "18:30:00", Text = "18:30" },
                new SelectListItem { Value = "19:00:00", Text = "19:00" },
            };

            return new SelectList(Result, "Value", "Text", SelectedValue);
        }

        public SelectList Nationalities(Guid OrganizationInstanceGUID)
        {

            var Result = (from a in DbCMS.codeCountriesConfigurations.Where(x => x.OrganizationInstanceGUID == OrganizationInstanceGUID && x.Active)
                          join b in DbCMS.codeCountriesLanguages.Where(x => x.LanguageID == LAN & x.Active) on a.CountryGUID equals b.CountryGUID
                          select new
                          {
                              Value = a.CountryGUID.ToString(),
                              Text = b.Nationality
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList Nationalities()
        {
            Guid OrganizationInstanceGUID = Guid.Parse(HttpContext.Current.Session[SessionKeys.OrganizationInstanceGUID].ToString());

            var Result = (from a in DbCMS.codeCountriesConfigurations.Where(x => x.OrganizationInstanceGUID == OrganizationInstanceGUID && x.Active)
                          join b in DbCMS.codeCountriesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.CountryGUID equals b.CountryGUID
                          orderby b.CountryDescription
                          select new
                          {
                              Value = a.CountryGUID.ToString(),
                              Text = b.Nationality
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList NationalitiesProGres()
        {
            Guid OrganizationInstanceGUID = Guid.Parse(HttpContext.Current.Session[SessionKeys.OrganizationInstanceGUID].ToString());

            var Result = (from a in DbCMS.codeCountriesConfigurations.Where(x => x.OrganizationInstanceGUID == OrganizationInstanceGUID && x.Active)
                          join b in DbCMS.codeCountriesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.CountryGUID equals b.CountryGUID
                          orderby b.Nationality
                          select new
                          {
                              Value = a.codeCountries.CountryA3Code,
                              Text = b.Nationality
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public List<CountriesPhoneCode> CountriesPhoneCode()
        {
            Guid OrganizationInstanceGUID = Guid.Parse(HttpContext.Current.Session[SessionKeys.OrganizationInstanceGUID].ToString());

            var Result = (from a in DbCMS.codeCountriesConfigurations.Where(x => x.OrganizationInstanceGUID == OrganizationInstanceGUID && x.Active)
                          join b in DbCMS.codeCountriesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.CountryGUID equals b.CountryGUID into LJ1

                          from R1 in LJ1.DefaultIfEmpty()
                          join c in DbCMS.codeCountries on a.CountryGUID equals c.CountryGUID into LJ2

                          from R2 in LJ2.DefaultIfEmpty().AsEnumerable()
                          orderby R1.CountryDescription

                          select new CountriesPhoneCode
                          {
                              id = R2.PhoneCode,
                              text = R1.CountryDescription,
                              src = R2.CountryA3Code
                          }).ToList();

            return Result;
        }

        public SelectList Genders()
        {
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableName == "Gender" && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList GendersByOrganizationInstance()
        {
            Guid OrganizationInstanceGUID = Guid.Parse(HttpContext.Current.Session[SessionKeys.OrganizationInstanceGUID].ToString());

            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableName == "Gender" && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          join c in DbCMS.codeTablesValuesConfigurations.Where(x => x.Active && x.OrganizationInstanceGUID == OrganizationInstanceGUID) on a.ValueGUID equals c.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList PartnerReports()
        {
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableName == "Partner Reports" && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          orderby a.SortID
                          select new
                          {
                              SortID = a.SortID,
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).OrderBy(x => x.SortID).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList PartnerStandardReports()
        {
            //Guid Cumulative_Table = Guid.Parse("00000000-0000-0000-0000-000000000002");
            //Guid Monthly_Table = Guid.Parse("00000000-0000-0000-0000-000000000001");
            //Guid Monthly_Update = Guid.Parse("00000000-0000-0000-0000-000000000003");

            List<SelectListItem> Result = new List<SelectListItem> {
                new SelectListItem { Value = "00000000-0000-0000-0000-000000000003", Text = "Monthly Update" },
                //new SelectListItem { Value = "00000000-0000-0000-0000-000000000001", Text = "Monthly Table" },
                //new SelectListItem { Value = "00000000-0000-0000-0000-000000000002", Text = "Cumulative Report" }
            };

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList ImportPartnerReports()
        {
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableName == "Partner Reports" && x.Active && x.ValueGUID.ToString() == "00000000-0000-0000-0000-000000000001" || x.ValueGUID.ToString() == "00000000-0000-0000-0000-000000000002")
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList Genders(Guid UserGUID)
        {
            Guid OrganizationInstanceGUID = Guid.Parse(HttpContext.Current.Session[SessionKeys.OrganizationInstanceGUID].ToString());

            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableName == "Gender" && x.Active)
                          join b in DbCMS.codeTablesValuesConfigurations.Where(x => x.OrganizationInstanceGUID == OrganizationInstanceGUID && x.Active) on a.ValueGUID equals b.ValueGUID
                          join c in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals c.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = c.ValueDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList UNHCRBureaus()
        {
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableGUID == LookupTables.UNHCRBureaus && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList Languages()
        {
            var Result = DbCMS.codeLanguages.AsNoTracking()
                                            .Select(x => new
                                            {
                                                Value = x.LanguageID,
                                                Text = x.LanguageNameLocal
                                            }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList Languages(bool IncludeCurrent)
        {
            string _LAN = "";
            if (!IncludeCurrent)
            {
                _LAN = Extensions.Languages.CurrentLanguage().ToUpper();
            }

            var Result = DbCMS.codeLanguages.AsNoTracking().Where(x => x.LanguageID != _LAN && x.Active)
                                            .Select(x => new
                                            {
                                                Value = x.LanguageID,
                                                Text = x.LanguageNameLocal
                                            }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList UserProfiles(Guid _UserProfileGUID)
        {
            try
            {
                Guid _UserGUID = DbCMS.userProfiles.Where(x => x.UserProfileGUID == _UserProfileGUID).FirstOrDefault().userServiceHistory.UserGUID;

                var Result = (from a in DbCMS.userServiceHistory.Where(x => x.Active && x.UserGUID == _UserGUID)
                              join b in DbCMS.userProfiles.Where(x => x.Active) on a.ServiceHistoryGUID equals b.ServiceHistoryGUID
                              join c in DbCMS.codeJobTitlesLanguages.Where(x => x.Active && x.LanguageID == LAN) on b.JobTitleGUID equals c.JobTitleGUID
                              orderby b.FromDate descending
                              select new
                              {
                                  Value = b.UserProfileGUID.ToString(),
                                  Text = c.JobTitleDescription
                              }
                    ).ToList();

                return new SelectList(Result, "Value", "Text");
            }
            catch { return Empty(); }
        }

        public SelectList UserProfiles()
        {
            Guid _UserGUID = DbCMS.userProfiles.Where(x => x.UserProfileGUID == UserProfileGUID).FirstOrDefault().userServiceHistory.UserGUID;

            var Result = (from a in DbCMS.userServiceHistory.Where(x => x.Active && x.UserGUID == _UserGUID)
                          join b in DbCMS.userProfiles.Where(x => x.Active) on a.ServiceHistoryGUID equals b.ServiceHistoryGUID
                          join c in DbCMS.codeJobTitlesLanguages.Where(x => x.Active && x.LanguageID == LAN) on b.JobTitleGUID equals c.JobTitleGUID
                          orderby b.FromDate descending
                          select new
                          {
                              Value = b.UserProfileGUID.ToString(),
                              Text = c.JobTitleDescription
                          }
                ).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList UserProfileNames()
        {
            var Result = (from a in DbCMS.userProfiles.Where(x => x.Active)
                          join b in DbCMS.userServiceHistory.Where(x => x.Active) on a.ServiceHistoryGUID equals b.ServiceHistoryGUID
                          join c in DbCMS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN ) on b.UserGUID equals c.UserGUID
                          orderby a.FromDate descending
                          select new
                          {
                              Value = a.UserProfileGUID.ToString(),
                              Text = c.FirstName + " " + c.Surname + " (" + b.EmailAddress + ")"
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList RecordStatus()
        {
            List<SelectListItem> Result = new List<SelectListItem> {
                new SelectListItem { Value = "true", Text = "Active" },
                new SelectListItem { Value = "false", Text = "Deleted" }
            };

            return new SelectList(Result, "Value", "Text");

        }

        public SelectList CheckBoxRecordStatus()
        {
            List<SelectListItem> Result = new List<SelectListItem> {
                 new SelectListItem { Value = "", Text = "" },
                new SelectListItem { Value = "true", Text = "Yes" },
                new SelectListItem { Value = "false", Text = "No" }
            };

            return new SelectList(Result, "Value", "Text");

        }

        public SelectList IsActive()
        {
            List<SelectListItem> Result = new List<SelectListItem> {
                new SelectListItem { Value = "true", Text = "Active" },
                new SelectListItem { Value = "false", Text = "Inactive" }
            };

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList IsPublic()
        {
            List<SelectListItem> Result = new List<SelectListItem> {
                new SelectListItem { Value = "true", Text = "Public" },
                new SelectListItem { Value = "false", Text = "Authorized" }
            };

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList Actions(Guid? CategoryGUID)
        {
            if (CategoryGUID == null)
            {
                return Empty();
            }

            var Result = (from a in DbCMS.codeActions.Where(x => x.ActionCategoryGUID == CategoryGUID && x.Active)
                          join b in DbCMS.codeActionsVerbsLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ActionVerbGUID equals b.ActionVerbGUID
                          orderby a.ActionID
                          select new
                          {
                              Value = a.ActionGUID.ToString(),
                              Text = b.ActionVerbDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList ActionType(string SelectedValue = null)
        {
            List<SelectListItem> Result = new List<SelectListItem> {
                new SelectListItem { Value = "true", Text = "Auditing Purpose" },
                new SelectListItem { Value = "false", Text = "Authorization Purpose" }
            };

            return new SelectList(Result, "Value", "Text", SelectedValue);
        }

        public SelectList ActionsCategories(Guid? ApplicationGUID, bool IncludeForAudit = false)
        {
            var Result = (from a in DbCMS.codeActionsCategories.Where(a => a.ApplicationGUID == ApplicationGUID.Value)
                          join b in DbCMS.codeActionsCategoriesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ActionCategoryGUID equals b.ActionCategoryGUID
                          where (!IncludeForAudit || a.codeActions.Where(x => x.Active && x.ForAuditPurpose == false).Count() > 0)
                          orderby b.ActionCategoryDescription
                          select new
                          {
                              Value = a.ActionCategoryGUID.ToString(),
                              Text = b.ActionCategoryDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList LocationTypes()
        {
            var Result = (from a in DbCMS.codeTables.Where(x => x.TableGUID == LookupTables.LocationTypes && x.Active)
                          join b in DbCMS.codeTablesValues.Where(x => x.Active) on a.TableGUID equals b.TableGUID
                          join c in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on b.ValueGUID equals c.ValueGUID
                          orderby b.SortID
                          select new
                          {
                              Value = b.ValueGUID.ToString(),
                              Text = c.ValueDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList Locations(Guid CountryGUID, int ParentLevelID)
        {
            if (ParentLevelID == 1)
            {
                var Result = (from a in DbCMS.codeCountriesLanguages.Where(x => x.LanguageID == LAN && x.CountryGUID == CountryGUID)
                              select new
                              {
                                  Value = a.CountryGUID.ToString(),
                                  Text = a.CountryDescription
                              }).ToList();

                return new SelectList(Result, "Value", "Text");
            }
            else
            {
                int Level = ParentLevelID - 1;
                var Result = (from a in DbCMS.codeLocations.Where(x => x.CountryGUID == CountryGUID && x.LocationlevelID == Level && x.Active)
                              join b in DbCMS.codeLocationsLanguages.Where(l => l.LanguageID == LAN && l.Active) on a.LocationGUID equals b.LocationGUID
                              select new
                              {
                                  Value = a.LocationGUID.ToString(),
                                  Text = b.LocationDescription
                              }).ToList();

                return new SelectList(Result, "Value", "Text");
            }
        }

        public SelectList Locations(Guid CountryGUID)
        {

            var Result = (from a in DbCMS.codeLocations.Where(x => x.CountryGUID == CountryGUID && x.Active)
                          join b in DbCMS.codeLocationsLanguages.Where(l => l.LanguageID == LAN && l.Active) on a.LocationGUID equals b.LocationGUID
                          select new
                          {
                              Value = a.LocationGUID.ToString(),
                              Text = b.LocationDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");

        }

        public SelectList LocationsByLocationType(Guid LocationParentGUID, Guid LocationTypeGUID)
        {

            var Result = (from a in DbCMS.codeLocations.Where(x => x.LocationParentGUID == LocationParentGUID && x.Active && x.LocationTypeGUID == LocationTypeGUID)
                          join b in DbCMS.codeLocationsLanguages.Where(l => l.LanguageID == LAN && l.Active) on a.LocationGUID equals b.LocationGUID
                          select new
                          {
                              Value = a.LocationGUID.ToString(),
                              Text = b.LocationDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");

        }

        public SelectList LocationsByCountryType(string[] CountryGUID, Guid LocationTypeGUID)
        {

            var Result = (from a in DbCMS.codeLocations.Where(x => CountryGUID.Contains(x.CountryGUID.ToString()) && x.Active && x.LocationTypeGUID == LocationTypeGUID)
                          join b in DbCMS.codeLocationsLanguages.Where(l => l.LanguageID == LAN && l.Active) on a.LocationGUID equals b.LocationGUID
                          select new
                          {
                              Value = a.LocationGUID.ToString(),
                              Text = b.LocationDescription,
                              Group = a.CountryGUID.ToString()
                          }).ToList();

            return new SelectList(Result, "Value", "Text", "Group", -1);

        }

        public SelectList Locations()
        {
            var Result = (from a in DbCMS.codeLocations.Where(x => x.Active && x.LocationlevelID == 4)
                          join b in DbCMS.codeLocationsLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.LocationGUID equals b.LocationGUID
                          orderby b.LocationDescription
                          select new
                          {
                              Value = a.LocationGUID.ToString(),
                              Text = b.LocationDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList LocationOneValue(Guid? LocationGUID)
        {
            if (LocationGUID == Guid.Empty || LocationGUID == null)
            {
                return Empty();
            }
            var Result = (from a in DbCMS.codeLocations.Where(x => x.Active && x.LocationGUID == LocationGUID)
                          join b in DbCMS.codeLocationsLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.LocationGUID equals b.LocationGUID
                          orderby b.LocationDescription
                          select new
                          {
                              Value = a.LocationGUID.ToString(),
                              Text = b.LocationDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList LocationOneLinkedValues(Guid? LocationGUID)
        {
            if (LocationGUID == Guid.Empty || LocationGUID == null)
            {
                return Empty();
            }
            Guid LocationTypeGUID = Guid.Parse("FCF6CDE3-329A-42A2-82A2-D99909949F82");
            var Result = (from a in DbCMS.codeLocations.Where(x => x.Active && x.LocationGUID == LocationGUID && x.LocationTypeGUID == LocationTypeGUID)
                          select a).FirstOrDefault();

            return SubDistricts(Result.LocationParentGUID.Value);
        }

        public SelectList Levels(int Max)
        {
            List<SelectListItem> List = new List<SelectListItem>();

            for (int i = 0; i <= Max; i++)
            {
                List.Add(new SelectListItem { Value = i.ToString(), Text = "Level " + i.ToString() });
            }

            return new SelectList(List, "Value", "Text");
        }

        public SelectList Factors(Guid ActionCategoryGUID, Guid? FactorGUID = null)
        {
            //query = from staff in query where (staff.name == name1);
            var Result = (from a in DbCMS.codeFactors.Where(x => x.Active)
                          join b in DbCMS.codeFactorsLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.FactorGUID equals b.FactorGUID
                          where !DbCMS.codeActionsCategoriesFactors.Where(x => x.ActionCategoryGUID == ActionCategoryGUID && (FactorGUID.HasValue && x.FactorGUID != FactorGUID) && x.FactorGUID != FactorGUID.Value && x.Active).Select(x => x.FactorGUID).Contains(a.FactorGUID)
                          orderby b.FactorDescription
                          select new
                          {

                              Value = a.FactorGUID.ToString(),
                              Text = b.FactorDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList FactorsForDependency(Guid ActionCategoryGUID, Guid FactorGUID)
        {
            var Result = (from a in DbCMS.codeFactors.Where(x => x.FactorGUID != FactorGUID && x.Active)
                          join b in DbCMS.codeFactorsLanguages.Where(l => l.LanguageID == LAN && l.Active) on a.FactorGUID equals b.FactorGUID
                          join c in DbCMS.codeActionsCategoriesFactors.Where(x => x.ActionCategoryGUID == ActionCategoryGUID && x.Active) on a.FactorGUID equals c.FactorGUID
                          orderby c.FactorTreeLevel
                          select new
                          {
                              Value = c.FactorTreeLevel.ToString(),
                              Text = b.FactorDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList FactorPurpose()
        {
            List<SelectListItem> Result = new List<SelectListItem>
            {
                new SelectListItem{Value = "true", Text = "Value" },
                new SelectListItem{Value = "false", Text = "Grouping" }

            };
            return new SelectList(Result, "Value", "Text");
        }

        public SelectList ServiceHistory()
        {
            var Result = (from a in DbCMS.userServiceHistory.Where(x => x.UserGUID == UserGUID && x.Active)
                          select new
                          {
                              Value = a.ServiceHistoryGUID.ToString(),
                              Text = a.codeOrganizations.codeOrganizationsLanguages.Where(l => l.LanguageID == LAN).FirstOrDefault().OrganizationDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList ServiceHistory(Guid UserGUID)
        {
            var Result = (from a in DbCMS.userServiceHistory.Where(x => x.UserGUID == UserGUID && x.Active)
                          select new
                          {
                              Value = a.ServiceHistoryGUID.ToString(),
                              Text = a.codeOrganizations.codeOrganizationsLanguages.Where(l => l.LanguageID == LAN && l.Active).FirstOrDefault().OrganizationDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList UserJobs()
        {
            return UserJobs(UserGUID);
        }

        public SelectList UserJobs(Guid UserGUID)
        {
            var Result = (from a in DbCMS.userProfiles.Where(x => x.userServiceHistory.UserGUID == UserGUID && x.Active)
                          select new
                          {
                              Value = a.UserProfileGUID.ToString(),
                              Text = a.codeJobTitles.codeJobTitlesLanguages.Where(l => l.LanguageID == LAN && l.Active).FirstOrDefault().JobTitleDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList ManagerTypes()
        {
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableGUID == LookupTables.ManagerTypes && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList Users(bool IncludeMe)
        {
            var Result = (from a in DbCMS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.Active && (IncludeMe || x.UserGUID != UserGUID))
                          orderby a.FirstName, a.Surname
                          select new
                          {
                              Value = a.UserGUID.ToString(),
                              Text = a.FirstName + " " + a.Surname
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList CoreUsers(bool IncludeMe)
        {
            var Result = (from a in DbCMS.StaffCoreData.Where(x => x.Active && (IncludeMe || x.UserGUID != UserGUID) && x.StaffStatusGUID.ToString() == "B9CD375C-A576-4AA4-8AF4-FF3C1C4E3611" && x.OrganizationInstanceGUID==OrganizationInstanceGUID)
                          join b in DbCMS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN)
                          on a.UserGUID equals b.UserGUID
                          orderby b.FirstName, b.Surname
                          select new
                          {
                              Value = a.UserGUID.ToString(),
                              Text = b.FirstName + " " + b.Surname

                          }).ToList();
            return new SelectList(Result, "Value", "Text");
        }

       
        public SelectList CoreUsersFullNameValue(bool IncludeMe)
        {
            var Result = (from a in DbCMS.StaffCoreData.Where(x => x.Active && (IncludeMe || x.UserGUID != UserGUID) && x.StaffStatusGUID.ToString() == "B9CD375C-A576-4AA4-8AF4-FF3C1C4E3611")
                          join b in DbCMS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN)
                          on a.UserGUID equals b.UserGUID
                          orderby b.FirstName, b.Surname
                          select new
                          {
                              Value = b.FirstName + " " + b.Surname,
                              Text = b.FirstName + " " + b.Surname

                          }).ToList();
            return new SelectList(Result, "Value", "Text");
        }
        public SelectList User(Guid? UserGUID)
        {
            using (var DbSHM = new SHM_DAL.Model.SHMEntities())
            {
                if (UserGUID == null) return new DropDownList().Empty();
                var staffCore = DbSHM.StaffCoreData.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
                var Result = (from a in DbSHM.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.Active )
                              join b in DbSHM.StaffCoreData.Where(x => x.OrganizationInstanceGUID== staffCore.OrganizationInstanceGUID) on a.UserGUID equals b.UserGUID
                              select new
                              {
                                  Value = a.UserGUID.ToString(),
                                  Text = a.FirstName + " " + a.Surname
                              }).ToList();

                return new SelectList(Result, "Value", "Text");
            }
        }

        public SelectList OrganizationsInstancesDepartmentStaff(Guid appointmentGuid)
        {
            if (appointmentGuid == Guid.Empty) return new DropDownList().Empty();
            var department = DbCMS.codeAppointmentType.Where(x => x.AppointmentTypeGUID == appointmentGuid).FirstOrDefault();
            var userProfiles = DbCMS.StaffCoreData.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
            var Result = (from a in DbCMS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.Active)
                         
                          join c in DbCMS.StaffCoreData.Where(x => x.DepartmentGUID == department.DepartmentGUID /*&& x.OrganizationInstanceGUID == userProfiles.OrganizationInstanceGUID*/ && x.DutyStationGUID == userProfiles.DutyStationGUID) on a.UserGUID equals c.UserGUID
                          select new
                          {
                              Value = a.UserGUID.ToString(),
                              Text = a.FirstName + " " + a.Surname
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList UserStaff(Guid PartyTypeGUID)
        {
            var Result = (from a in DbCMS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.Active)
                          join b in DbCMS.userServiceHistory on a.UserGUID equals b.UserGUID
                          join c in DbCMS.userProfiles on b.ServiceHistoryGUID equals c.ServiceHistoryGUID
                          select new
                          {
                              Value = a.UserGUID.ToString(),
                              Text = a.FirstName + " " + a.Surname
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList ActionVerbs()
        {
            var Result = (from a in DbCMS.codeActionsVerbsLanguages.Where(x => x.LanguageID == LAN && x.Active)
                          orderby a.ActionVerbDescription
                          select new
                          {
                              Value = a.ActionVerbGUID,
                              Text = a.ActionVerbDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList ActionEntities()
        {
            var Result = (from a in DbCMS.codeActionsEntitiesLanguages.Where(x => x.LanguageID == LAN && x.Active)
                          select new
                          {
                              Value = a.ActionEntityGUID,
                              Text = a.ActionEntityDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList ConditionTypes()
        {

            var Result = (from a in DbCMS.codeConditionTypeLanguage.Where(x => x.LanguageID == LAN && x.Active)
                          select new
                          {
                              Value = a.ConditionTypeGUID.ToString(),
                              Text = a.Description
                          }).ToList();

            //return Conditiontytpes;
            return new SelectList(Result, "Value", "Text");
        }


        public SelectList Referral()
        {
            REFEntities DbREF = new REFEntities();
            var Result = (from a in DbREF.configReferral
                          join b in DbREF.configReferralLanguage.Where(x => x.LanguageID == LAN && x.Active) on a.ReferralGUID equals b.ReferralGUID
                          select new
                          {
                              Value = a.ReferralGUID.ToString(),
                              Text = b.ReferralDescription
                          }).OrderBy(x => x.Text).ToList();

            //return Conditiontytpes;
            return new SelectList(Result, "Value", "Text");
        }

        public SelectList ReferralStep()
        {
            REFEntities DbREF = new REFEntities();
            var Result = (from a in DbREF.configReferralStep
                          join b in DbREF.configReferralStepLanguage.Where(x => x.LanguageID == LAN && x.Active) on a.ReferralStepGUID equals b.ReferralStepGUID
                          select new
                          {
                              Value = a.ReferralStepGUID.ToString(),
                              Text = b.Description
                          }).OrderBy(x => x.Text).ToList();

            //return Conditiontytpes;
            return new SelectList(Result, "Value", "Text");
        }

        public SelectList ReferralStatus(Guid ApplicationGUID)
        {
            var Result = (from a in DbCMS.codeReferralStatus.Where(x => x.ApplicationGUID == ApplicationGUID)
                          join b in DbCMS.codeReferralStatusLanguage.Where(x => x.LanguageID == LAN && x.Active) on a.ReferralStatusGUID equals b.ReferralStatusGUID
                          select new
                          {
                              Value = a.ReferralStatusGUID.ToString(),
                              Text = b.Description
                          }).OrderBy(x => x.Text).ToList();

            //return Conditiontytpes;
            return new SelectList(Result, "Value", "Text");
        }



        public SelectList Sort()
        {
            List<SelectListItem> Result = new List<SelectListItem>() {
                new SelectListItem { Value = "Ascending", Text = "asc" },
                new SelectListItem { Value = "Descending", Text = "desc" }
            };

            return new SelectList(Result, "Value", "Text");
        }


        public SelectList PrinterAccessByDutyStations()
        {
            List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.PrintersConfiguration.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();

            var Result = (from 
                           a in DbCMS.codeDutyStationsLanguages.Where(l => l.LanguageID == LAN && l.Active).Where(x => AuthorizedList.Any(y=>y.Contains(x.DutyStationGUID.ToString())))

                          orderby a.DutyStationDescription
                          select new
                          {
                              Value = a.DutyStationGUID.ToString(),
                              Text = a.DutyStationDescription
                          }).ToList();
            return new SelectList(Result, "Value", "Text");
        }
        public SelectList PrinterModels()
        {
            List<SelectListItem> Result = new List<SelectListItem> {
                new SelectListItem { Value = "00000000-0000-0000-0000-000000000001", Text = "HP" },
                new SelectListItem { Value = "00000000-0000-0000-0000-000000000002", Text = "RICHO" },
            };

            return new SelectList(Result, "Value", "Text");

        }

        public SelectList PrinterTypes()
        {
            List<SelectListItem> Result = new List<SelectListItem> {
                new SelectListItem { Value = "00000000-0000-0000-0000-000000000001", Text = "colored" },
                new SelectListItem { Value = "00000000-0000-0000-0000-000000000002", Text = "B&W" }
            };

            return new SelectList(Result, "Value", "Text");

        }

        public SelectList PPATypes()
        {
            var Result = (from a in DbCMS.codeTables.Where(x => x.TableGUID == LookupTables.PPAType && x.Active)
                          join b in DbCMS.codeTablesValues.Where(x => x.Active) on a.TableGUID equals b.TableGUID
                          join c in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on b.ValueGUID equals c.ValueGUID
                          orderby a.SortID
                          select new
                          {
                              Value = c.ValueGUID,
                              Text = c.ValueDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");

            //List<SelectListItem> Result = new List<SelectListItem> {
            //    new SelectListItem { Value = "00000000-0000-0000-0000-000000000001", Text = "Project Partnership Agreement" },
            //    new SelectListItem { Value = "00000000-0000-0000-0000-000000000002", Text = "Report" }
            //};

            //return new SelectList(Result, "Value", "Text");
        }

        public SelectList PPAAreasOfImplementations()
        {
            var Result = (from a in DbCMS.codeTables.Where(x => x.TableGUID == LookupTables.PPAAreasOfImplementation && x.Active)
                          join b in DbCMS.codeTablesValues.Where(x => x.Active) on a.TableGUID equals b.TableGUID
                          join c in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on b.ValueGUID equals c.ValueGUID
                          orderby a.SortID ascending
                          select new
                          {
                              Value = c.ValueGUID,
                              Text = c.ValueDescription
                          }).ToList();
            return new SelectList(Result, "Value", "Text");

        }

        public SelectList PPATypes2()
        {
            List<SelectListItem> Result = new List<SelectListItem> {
                new SelectListItem { Value = "00000000-0000-0000-0000-000000000001", Text = "Nation Wide" },
                new SelectListItem { Value = "00000000-0000-0000-0000-000000000002", Text = "Area Based Damascus and Rural Damascus" },
                new SelectListItem { Value = "00000000-0000-0000-0000-000000000003", Text = "Area Based Homs" },
                new SelectListItem { Value = "00000000-0000-0000-0000-000000000004", Text = "Area Based Hama" },
                new SelectListItem { Value = "00000000-0000-0000-0000-000000000005", Text = "Area Based Aleppo" },
                new SelectListItem { Value = "00000000-0000-0000-0000-000000000006", Text = "Area Based Sweida" },
                new SelectListItem { Value = "00000000-0000-0000-0000-000000000007", Text = "Area Based Qunitera" },
                new SelectListItem { Value = "00000000-0000-0000-0000-000000000008", Text = "Area Based Dara’a" },
                new SelectListItem { Value = "00000000-0000-0000-0000-000000000009", Text = "Area Based Qamishli" },
                new SelectListItem { Value = "00000000-0000-0000-0000-000000000010", Text = "Area Based Deir-ez-Zour" },
                new SelectListItem { Value = "00000000-0000-0000-0000-000000000011", Text = "Area Based Raqqa" },
                new SelectListItem { Value = "00000000-0000-0000-0000-000000000012", Text = "Area Based Tartous" },
                new SelectListItem { Value = "00000000-0000-0000-0000-000000000013", Text = "Area Based Lattakia" },
                new SelectListItem { Value = "00000000-0000-0000-0000-000000000014", Text = "Area Based Central Damascus" }
            };
            return new SelectList(Result, "Value", "Text");

        }

        public SelectList PPAStatuses()
        {
            //List<SelectListItem> Result = new List<SelectListItem> {
            //    new SelectListItem { Value = "00000000-0000-0000-0000-000000000001", Text = "New" },
            //    new SelectListItem { Value = "00000000-0000-0000-0000-000000000002", Text = "Pending" },
            //    new SelectListItem { Value = "00000000-0000-0000-0000-000000000002", Text = "Completed" }
            //};
            var Result = (from a in DbCMS.codeTables.Where(x => x.TableGUID == LookupTables.PPAStatus && x.Active)
                          join b in DbCMS.codeTablesValues.Where(x => x.Active) on a.TableGUID equals b.TableGUID
                          join c in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on b.ValueGUID equals c.ValueGUID
                          orderby a.SortID
                          select new
                          {
                              Value = c.ValueDescription,
                              Text = c.ValueDescription
                          }).ToList();
            return new SelectList(Result, "Value", "Text");
        }

        public SelectList FileVersionStatuses(List<Guid> SpecificValues = null)
        {
            if (SpecificValues != null && SpecificValues.Count > 0)
            {
                var Result = (from a in DbCMS.codeTables.Where(x => x.TableGUID == LookupTables.PPAFileVersionStatus && x.Active)
                              join b in DbCMS.codeTablesValues.Where(x => x.Active) on a.TableGUID equals b.TableGUID
                              join c in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on b.ValueGUID equals c.ValueGUID
                              where SpecificValues.Contains(c.ValueGUID)
                              orderby a.SortID
                              select new
                              {
                                  Value = c.ValueGUID,
                                  Text = c.ValueDescription
                              }).ToList();
                return new SelectList(Result, "Value", "Text");
            }
            else
            {
                var Result = (from a in DbCMS.codeTables.Where(x => x.TableGUID == LookupTables.PPAFileVersionStatus && x.Active)
                              join b in DbCMS.codeTablesValues.Where(x => x.Active) on a.TableGUID equals b.TableGUID
                              join c in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on b.ValueGUID equals c.ValueGUID
                              orderby a.SortID
                              select new
                              {
                                  Value = c.ValueGUID,
                                  Text = c.ValueDescription
                              }).ToList();
                return new SelectList(Result, "Value", "Text");
            }

        }

        public SelectList PPAOriginalFiles(Guid PPAGUID)
        {
            var Result = (from a in DbCMS.PPAOriginalFile.Where(x => x.PPAGUID == PPAGUID && x.Active)
                          select new
                          {
                              Value = a.PPAOriginalFileGUID,
                              Text = a.FileName
                          }).ToList();
            return new SelectList(Result, "Value", "Text");
        }

        public SelectList PPAFileCategory()
        {
            var Result = (from a in DbCMS.codeTables.Where(x => x.TableGUID == LookupTables.PPAFilesCategories && x.Active)
                          join b in DbCMS.codeTablesValues.Where(x => x.Active) on a.TableGUID equals b.TableGUID
                          join c in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on b.ValueGUID equals c.ValueGUID
                          orderby a.SortID
                          select new
                          {
                              Value = c.ValueGUID,
                              Text = c.ValueDescription
                          }).ToList();
            return new SelectList(Result, "Value", "Text");
        }

        public SelectList PartnerCentersByOrganizationInstance(string OrganizationInstanceGUIDs)
        {
            if (OrganizationInstanceGUIDs == "")
            {
                return Empty();
            }
            List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.CommunityCenterCode.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();
            string[] OrganizationInstanceList = OrganizationInstanceGUIDs.Split(',');
            var Result = (from a in DbCMS.codePartnerCenter.Where(x => x.Active).Where(x => AuthorizedList.Contains(x.DutyStationGUID.ToString()))
                          join b in DbCMS.codePartnerCenterLanguage.Where(l => l.LanguageID == LAN && l.Active) on a.PartnerCenterGUID equals b.PartnerCenterGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()
                          join c in DbCMS.codeOrganizationsInstancesLanguages.Where(x => x.LanguageID == LAN && x.Active && OrganizationInstanceList.Contains(x.OrganizationInstanceGUID.ToString())) on a.OrganizationInstanceGUID equals c.OrganizationInstanceGUID
                          orderby R1.PartnerCenterDescription
                          select new
                          {
                              Value = R1.PartnerCenterGUID,
                              Text = R1.PartnerCenterDescription
                          }).ToList();
            return new SelectList(Result, "Value", "Text");
        }

        public SelectList CategoryPartnerReportLevelString2(Guid ReportGUID)
        {
            if (ReportGUID == Guid.Empty)
            {
                return Empty();
            }
            using (var DbPCR = new PCREntities())
            {
                string[] guids = new string[] { "00000000-0000-0000-0000-000000000221", "00000000-0000-0000-0000-000000000037", "00000000-0000-0000-0000-000000000219" };
                var Result = (from a in DbPCR.codeCategoryReport.Where(x => x.Active && x.CategoryLevel == 2 && x.ReportGUID == ReportGUID && !guids.Contains(x.CategoryReportGUID.ToString()))
                              orderby a.CategoryDescription
                              select new
                              {
                                  Value = a.CategoryDescription,
                                  Text = a.CategoryDescription
                              }).ToList();
                return new SelectList(Result, "Value", "Text");
            }

        }

        public SelectList CategoryPartnerReportLevel2(Guid ReportGUID)
        {
            if (ReportGUID == Guid.Empty)
            {
                return Empty();
            }
            using (var DbPCR = new PCREntities())
            {
                string[] guids = new string[] { "00000000-0000-0000-0000-000000000221", "00000000-0000-0000-0000-000000000037", "00000000-0000-0000-0000-000000000219" };
                var Result = (from a in DbPCR.codeCategoryReport.Where(x => x.Active && x.CategoryLevel == 2 && x.ReportGUID == ReportGUID && !guids.Contains(x.CategoryReportGUID.ToString()))
                              orderby a.CategoryDescription
                              select new
                              {
                                  Value = a.CategoryReportGUID,
                                  Text = a.CategoryDescription
                              }).ToList();
                return new SelectList(Result, "Value", "Text");
            }

        }

        public SelectList CategoryPartnerReportLevel2ForMonthlyUpdatesReport(Guid ReportGUID)
        {
            if (ReportGUID == Guid.Empty)
            {
                return Empty();
            }
            using (var DbPCR = new PCREntities())
            {
                var Result = (from a in DbPCR.codeCategoryReport.Where(x => x.Active && x.CategoryLevel == 3 && x.ReportGUID == ReportGUID && x.CategoryReportGUID.ToString() == "00000000-0000-0000-0000-000000000039")
                              orderby a.CategoryDescription
                              select new
                              {
                                  Value = a.CategoryReportGUID,
                                  Text = a.CategoryDescription
                              }).ToList();
                return new SelectList(Result, "Value", "Text");
            }

        }

        public SelectList CategoryPartnerReportLevelChart2(Guid ReportGUID)
        {
            if (ReportGUID == Guid.Empty)
            {
                return Empty();
            }
            using (var DbPCR = new PCREntities())
            {
                string[] guids = new string[] { "00000000-0000-0000-0000-000000000221" };
                var Result = (from a in DbPCR.codeCategoryReport.Where(x => x.Active && x.CategoryLevel == 2 && x.ReportGUID == ReportGUID && !guids.Contains(x.CategoryReportGUID.ToString()))
                              orderby a.CategoryDescription
                              select new
                              {
                                  Value = a.CategoryReportGUID,
                                  Text = a.CategoryDescription
                              }).ToList();
                return new SelectList(Result, "Value", "Text");
            }

        }

        public SelectList CategoryPartnerReportLevel3(Guid ReportGUID)
        {
            if (ReportGUID == Guid.Empty)
            {
                return Empty();
            }
            using (var DbPCR = new PCREntities())
            {
                List<Guid> guids = new List<Guid> { Guid.Parse("00000000-0000-0000-0000-000000000039"), Guid.Parse("00000000-0000-0000-0000-000000000038"), Guid.Parse("00000000-0000-0000-0000-000000000215"), Guid.Parse("00000000-0000-0000-0000-000000000216"), Guid.Parse("00000000-0000-0000-0000-000000000219") };

                var Result = (from a in DbPCR.codeCategoryReport.Where(x => !guids.Contains(x.CategoryReportGUID)).Where(x => x.Active && x.CategoryLevel == 3 && x.ReportGUID == ReportGUID && !x.CategoryDescription.Contains("assisted") && x.ParentCategoryReportGUID.ToString() != "00000000-0000-0000-0000-000000000221")
                              join b in DbPCR.codeCategoryReport on a.ParentCategoryReportGUID equals b.CategoryReportGUID

                              orderby a.CategoryDescription
                              select new
                              {
                                  Value = a.CategoryReportGUID,
                                  Text = a.CategoryDescription,
                                  Group = a.ParentCategoryReportGUID
                              }).ToList();
                return new SelectList(Result, "Value", "Text", "Group", -1);
            }

        }

        public SelectList ParentCategoryPartnerReport(string CategoryDescription)
        {
            if (CategoryDescription == "")
            {
                return Empty();
            }
            using (var DbPCR = new PCREntities())
            {
                string[] CategoryReportGUIDs = CategoryDescription.Split(',');
                List<Guid> guids = new List<Guid> { Guid.Parse("00000000-0000-0000-0000-000000000038"), Guid.Parse("00000000-0000-0000-0000-000000000215"), Guid.Parse("00000000-0000-0000-0000-000000000216"), Guid.Parse("00000000-0000-0000-0000-000000000219") };

                var Result = (from a in DbPCR.codeCategoryReport.Where(x => x.Active && CategoryReportGUIDs.Contains(x.CategoryReportGUID.ToString()))
                              join b in DbPCR.codeCategoryReport.Where(x => !guids.Contains(x.CategoryReportGUID)).Where(x => x.CategoryLevel == 3 && !x.CategoryDescription.Contains("Assisted") && x.ParentCategoryReportGUID.ToString() != "00000000-0000-0000-0000-000000000221") on a.CategoryReportGUID equals b.ParentCategoryReportGUID
                              orderby b.CategoryDescription
                              select new
                              {
                                  Value = b.CategoryReportGUID,
                                  Text = b.CategoryDescription,
                                  Group = b.ParentCategoryReportGUID
                              }).ToList();
                return new SelectList(Result, "Value", "Text", "Group", -1);
            }
        }

        public SelectList ReportsName()
        {
            List<SelectListItem> Result = new List<SelectListItem>() {
                new SelectListItem { Value = "Report1", Text = "Report1" },
                new SelectListItem { Value = "Report2", Text = "Report2" },
                new SelectListItem { Value = "Report3", Text = "Report3" },
            };
            return new SelectList(Result, "Value", "Text");
        }

        public SelectList ParentAggregation()
        {
            using (var DbPCR = new PCREntities())
            {
                var Result = (from a in DbPCR.codeAggregation.Where(x => x.Active)
                              orderby a.AggregationDescription
                              select new
                              {
                                  Value = a.AggregationGUID,
                                  Text = a.AggregationDescription
                              }).ToList();
                return new SelectList(Result, "Value", "Text");
            }
        }

        public SelectList AggregationIndividualsFamily()
        {
            using (var DbPCR = new PCREntities())
            {
                Guid Individuals = Guid.Parse("00000000-0000-0000-0000-000000000002");
                Guid Family = Guid.Parse("00000000-0000-0000-0000-000000000001");
                var Result = (from a in DbPCR.codeAggregation.Where(x => x.AggregationGUID == Individuals || x.AggregationGUID == Family)
                              orderby a.AggregationDescription
                              select new
                              {
                                  Value = a.AggregationGUID,
                                  Text = a.AggregationDescription
                              }).ToList();
                return new SelectList(Result, "Value", "Text");
            }
        }

        public SelectList AggregationAge(string AgeGuids)
        {
            using (var DbPCR = new PCREntities())
            {
                string[] gender = AgeGuids.Split(',');
                var Result = (from a in DbPCR.codeAggregation.Where(x => x.Active && x.GroupGUID.Value.ToString() == "00000000-0000-0000-0000-000000000003"
                              && gender.Contains(x.GenderGUID.ToString()))
                              orderby a.AggregationGUID
                              select new
                              {
                                  Value = a.AggregationGUID,
                                  Text = a.AggregationDescription
                              }).ToList();
                return new SelectList(Result, "Value", "Text");
            }
        }

        public SelectList AggregationReferral(Guid ReportGUID)
        {
            using (var DbPCR = new PCREntities())
            {
                var Result = (from a in DbPCR.codeAggregation.Where(x => x.Active && x.ReportGUID == ReportGUID && x.AggregationGUID.ToString() != "00000000-0000-0000-0000-000000000028")
                              orderby a.AggregationDescription
                              select new
                              {
                                  Value = a.AggregationGUID,
                                  Text = a.AggregationDescription
                              }).ToList();
                return new SelectList(Result, "Value", "Text");
            }
        }

        public SelectList AggregationAgeDistinct()
        {
            using (var DbPCR = new PCREntities())
            {
                var Result = (from a in DbPCR.codeAggregation.Where(x => x.Active && x.GroupGUID.Value.ToString() == "00000000-0000-0000-0000-000000000003" && x.GenderGUID.ToString() == "688B11E0-24FB-44B8-94CE-D8568C4742C7")
                              orderby a.AggregationGUID
                              select new
                              {
                                  Value = a.AggregationGUID,
                                  Text = a.AggregationDescription.Replace("Female", "").Replace("Male", "").Replace("Age", "")
                              }).ToList();
                Result = Result.Take(3).ToList();
                return new SelectList(Result, "Value", "Text");
            }
        }

        public SelectList AggregationProfile(string AgeGuids)
        {
            using (var DbPCR = new PCREntities())
            {
                string[] gender = AgeGuids.Split(',');
                var Result = (from a in DbPCR.codeAggregation.Where(x => x.Active && x.GroupGUID.Value.ToString() == "00000000-0000-0000-0000-000000000004"
                              && gender.Contains(x.GenderGUID.ToString()))
                              orderby a.AggregationGUID
                              select new
                              {
                                  Value = a.AggregationGUID,
                                  Text = a.AggregationDescription
                              }).ToList();
                return new SelectList(Result, "Value", "Text");
            }
        }

        public SelectList AggregationProfileDistinct()
        {
            using (var DbPCR = new PCREntities())
            {
                var Result = (from a in DbPCR.codeAggregation.Where(x => x.Active && x.GroupGUID.Value.ToString() == "00000000-0000-0000-0000-000000000004" && x.GenderGUID.ToString() == "688B11E0-24FB-44B8-94CE-D8568C4742C7")
                              orderby a.AggregationGUID
                              select new
                              {
                                  Value = a.AggregationGUID,
                                  Text = a.AggregationDescription.Replace("Female", "").Replace("Male", "")
                              }).ToList();
                Result.ForEach(x => x.Text.Replace("Female", "").Replace("Male", ""));
                Result = Result.Take(5).ToList();
                return new SelectList(Result, "Value", "Text");
            }
        }

        public SelectList PrinterOidsIsImport(Guid PrinterConfigurationGUID)
        {
            using (var DbRMS = new RMSEntities())
            {
                var printer = DbRMS.dataPrinterConfiguration.Where(x => x.PrinterConfigurationGUID == PrinterConfigurationGUID).FirstOrDefault();
                var Result = (from a in DbCMS.codeOID.Where(x => x.Active && x.PrinterModelGUID == printer.PrinterModelGUID && x.PrinterTypeGUID == printer.PrinterTypeGUID && x.IsImport)
                              select new
                              {
                                  Value = a.OidGUID,
                                  Text = a.OIDDescription
                              }).ToList();
                return new SelectList(Result, "Value", "Text");
            }
        }

        public SelectList PrinterReportTopic()
        {
            List<SelectListItem> Result = new List<SelectListItem>() {
                new SelectListItem { Value = "2", Text = "General Statistics" },
                new SelectListItem { Value = "1", Text = "Over Printer" }
            };
            return new SelectList(Result, "Value", "Text");
        }

        public SelectList PrinterConfigrations()
        {
            using (var DbRMS = new RMSEntities())
            {

                var Result = (from a in DbRMS.dataPrinterConfiguration.Where(x => x.Active)
                              join b in DbRMS.codeDutyStationsLanguages.Where(x=>x.Active && x.LanguageID==LAN) on a.DutyStationGUID equals b.DutyStationGUID
                              select new
                              {
                                  Value = a.PrinterConfigurationGUID,
                                  Text =  b.DutyStationDescription +" - Office "+ a.OfficeNumber + " - floor " + a.FloorNumber + " - " + a.PrinterName
                              }).ToList();
                return new SelectList(Result, "Value", "Text");
            }
        }


        public SelectList SiteCategories()
        {

            var Result = (from a in DbCMS.codeTables.Where(x => x.TableGUID == LookupTables.OfficeSiteCategories && x.Active)
                          join b in DbCMS.codeTablesValues.Where(x => x.Active) on a.TableGUID equals b.TableGUID
                          join c in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on b.ValueGUID equals c.ValueGUID
                          orderby a.SortID
                          select new
                          {
                              Value = c.ValueGUID,
                              Text = c.ValueDescription
                          }).ToList();
            return new SelectList(Result, "Value", "Text");
        }

        public SelectList DepartmentsForPPA(Guid OrganizationInstanceGUID)
        {
            List<Guid> PPAActiveDep = new List<Guid>();
            PPAActiveDep.Add(Guid.Parse("F32A7B63-59CF-410C-8079-9179FCA1717B"));
            PPAActiveDep.Add(Guid.Parse("982368DE-B2C6-4323-AD28-C57789F9AE45"));
            PPAActiveDep.Add(Guid.Parse("7B85F5F5-DC94-48B9-A70C-E0F084FDF99D"));
            PPAActiveDep.Add(Guid.Parse("D288667E-8692-48B8-9830-AA4B51F8114E"));
            PPAActiveDep.Add(Guid.Parse("AAE17636-7F84-47BD-A464-005C5A4E11B3"));
            PPAActiveDep.Add(Guid.Parse("B500A3E0-8342-41DF-AAEC-E527BA49BA81"));
            PPAActiveDep.Add(Guid.Parse("13F595A8-F303-4674-8797-1BF4FD37F589"));
            PPAActiveDep.Add(Guid.Parse("59E8046E-BBB8-4FAC-804A-DAD17F8CCBB1"));
            PPAActiveDep.Add(Guid.Parse("A587E6C2-7C3F-4481-A7D4-135FAA5FA11E"));
            var Result = (from a in DbCMS.codeDepartmentsConfigurations.Where(x => x.Active && x.OrganizationInstanceGUID == OrganizationInstanceGUID)
                          join b in DbCMS.codeDepartmentsLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.DepartmentGUID equals b.DepartmentGUID
                          where PPAActiveDep.Contains(b.DepartmentGUID)
                          orderby b.DepartmentDescription
                          select new
                          {
                              Value = a.DepartmentGUID.ToString(),
                              Text = b.DepartmentDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList HelpDeskRequestStatuses()
        {
            var Result = (from a in DbCMS.codeTables.Where(x => x.TableGUID == LookupTables.HelpDeskRequestStatus && x.Active)
                          join b in DbCMS.codeTablesValues.Where(x => x.Active) on a.TableGUID equals b.TableGUID
                          join c in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on b.ValueGUID equals c.ValueGUID
                          orderby a.SortID ascending
                          select new
                          {
                              Value = c.ValueGUID,
                              Text = c.ValueDescription
                          }).ToList();
            return new SelectList(Result, "Value", "Text");
        }

        public SelectList HelpDeskApprovalStatuses()
        {
            var Result = (from a in DbCMS.codeTables.Where(x => x.TableGUID == LookupTables.UserHelpDeskApprovalStatuses && x.Active)
                          join b in DbCMS.codeTablesValues.Where(x => x.Active) on a.TableGUID equals b.TableGUID
                          join c in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on b.ValueGUID equals c.ValueGUID
                          orderby a.SortID ascending
                          select new
                          {
                              Value = c.ValueGUID,
                              Text = c.ValueDescription
                          }).ToList();
            return new SelectList(Result, "Value", "Text");
        }

        public SelectList HelpDeskConfigItemsList()
        {
            var Result = (from a in DbCMS.codeTables.Where(x => x.TableGUID == LookupTables.HelpDeskConfigItems && x.Active)
                          join b in DbCMS.codeTablesValues.Where(x => x.Active) on a.TableGUID equals b.TableGUID
                          join c in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on b.ValueGUID equals c.ValueGUID
                          orderby a.SortID ascending
                          select new
                          {
                              Value = c.ValueGUID,
                              Text = c.ValueDescription
                          }).ToList();
            return new SelectList(Result, "Value", "Text");
        }

        public SelectList HelpDeskWorkGroupsList()
        {
            var Result = (from a in DbCMS.codeTables.Where(x => x.TableGUID == LookupTables.HelpDeskWorkGroups && x.Active)
                          join b in DbCMS.codeTablesValues.Where(x => x.Active) on a.TableGUID equals b.TableGUID
                          join c in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on b.ValueGUID equals c.ValueGUID
                          orderby a.SortID ascending
                          select new
                          {
                              Value = c.ValueGUID,
                              Text = c.ValueDescription
                          }).ToList();
            return new SelectList(Result, "Value", "Text");
        }

        public SelectList NumbersList(int Max)
        {
            List<SelectListItem> List = new List<SelectListItem>();

            for (int i = 1; i <= Max; i++)
            {
                List.Add(new SelectListItem { Value = i.ToString(), Text = i.ToString() });
            }

            return new SelectList(List, "Value", "Text");
        }

        public SelectList HelpDeskEstimatedTimeList()
        {
            var Result = (from a in DbCMS.codeTables.Where(x => x.TableGUID == LookupTables.UserHelpDeskEstimatedTime && x.Active)
                          join b in DbCMS.codeTablesValues.Where(x => x.Active) on a.TableGUID equals b.TableGUID
                          join c in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on b.ValueGUID equals c.ValueGUID
                          orderby a.SortID ascending
                          select new
                          {
                              Value = c.ValueGUID,
                              Text = c.ValueDescription
                          }).ToList();
            return new SelectList(Result, "Value", "Text");
        }

        public SelectList HelpDeskCriticalityList()
        {
            var Result = (from a in DbCMS.codeTables.Where(x => x.TableGUID == LookupTables.UserHelpDeskCriticality && x.Active)
                          join b in DbCMS.codeTablesValues.Where(x => x.Active) on a.TableGUID equals b.TableGUID
                          join c in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on b.ValueGUID equals c.ValueGUID
                          orderby a.SortID ascending
                          select new
                          {
                              Value = c.ValueGUID,
                              Text = c.ValueDescription
                          }).ToList();
            return new SelectList(Result, "Value", "Text");
        }

        public SelectList HelpDeskAssignToList()
        {
            List<string> EmailAddresses = new List<string>();
            EmailAddresses.Add("karkoush@unhcr.org");
            EmailAddresses.Add("alfazzaa@unhcr.org");
            EmailAddresses.Add("maksoud@unhcr.org");
            EmailAddresses.Add("shaban@unhcr.org");
            //EmailAddresses.Add("shaban@unhcr.org");

            //EmailAddresses.Add("shaglil@unhcr.org");
            //EmailAddresses.Add("HOMSSI@unhcr.org");
            //EmailAddresses.Add("AYDI@unhcr.org");
            //EmailAddresses.Add("sahhar@unhcr.org");


            var Result = (from a in DbCMS.userProfiles.Where(x => x.Active)
                          join b in DbCMS.userServiceHistory.Where(x => x.Active) on a.ServiceHistoryGUID equals b.ServiceHistoryGUID
                          join c in DbCMS.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on b.UserGUID equals c.UserGUID
                          where EmailAddresses.Contains(b.EmailAddress)
                          select new
                          {
                              Value = a.UserProfileGUID,
                              Text = c.FirstName + " " + c.Surname
                          }).ToList();
            return new SelectList(Result, "Value", "Text");
        }

        #endregion

        #region TBS
        public SelectList BillCycleTypes()
        {
            List<SelectListItem> Result = new List<SelectListItem>() {
                new SelectListItem { Value = "00000000-0000-0000-0000-000000000001", Text = "Cycle Type 1" },
                new SelectListItem { Value = "00000000-0000-0000-0000-000000000002", Text = "Cycle Type 2" }
            };

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList BillTypes()
        {
            List<SelectListItem> Result = new List<SelectListItem>() {
                new SelectListItem { Value = "9D8A1EB9-C2AC-4D78-95FF-874E46074321", Text = "Mobile" },
                new SelectListItem { Value = "B2980068-688D-428D-9E5C-494656BA9D2C", Text = "Landline" }
            };
            return new SelectList(Result, "Value", "Text");
        }

        public SelectList BillTypes(int select)
        {
            if (select == 1)
            {
                List<SelectListItem> Result = new List<SelectListItem>() {

                new SelectListItem { Value = "9D8A1EB9-C2AC-4D78-95FF-874E46074321", Text = "Mobile" },
            };
                return new SelectList(Result, "Value", "Text");
            }
            else
            {
                List<SelectListItem> Result = new List<SelectListItem>() {
                new SelectListItem { Value = "B2980068-688D-428D-9E5C-494656BA9D2C", Text = "Landline" }
                };
                return new SelectList(Result, "Value", "Text");
            }
        }

        public SelectList TelecomCompaniesConfigGuidList()
        {
            if (DbCMS == null)
            {
                DbCMS = new CMSEntities();
            }
            var Result = (from a in DbCMS.codeTelecomCompanyOperation.Where(x => x.Active)
                          join b in DbCMS.configTelecomCompanyOperation.Where(x => x.Active) on a.TelecomCompanyOperationGUID equals b.TelecomCompanyOperationGUID
                          join c in DbCMS.codeOperationsLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.OperationGUID equals c.OperationGUID
                          join d in DbCMS.codeTelecomCompanyLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.TelecomCompanyGUID equals d.TelecomCompanyGUID
                          select new
                          {
                              Value = b.TelecomCompanyOperationConfigGUID,
                              Text = d.TelecomCompanyDescription + " > " + c.OperationDescription
                          }).ToList();
            return new SelectList(Result, "Value", "Text");
        }

        public SelectList TelecomCompaniesConfigGuidForMobilesList()
        {
            var Result = (from a in DbCMS.codeTelecomCompanyOperation.Where(x => x.Active)
                          join b in DbCMS.configTelecomCompanyOperation.Where(x => x.Active) on a.TelecomCompanyOperationGUID equals b.TelecomCompanyOperationGUID
                          join c in DbCMS.codeOperationsLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.OperationGUID equals c.OperationGUID
                          join d in DbCMS.codeTelecomCompanyLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.TelecomCompanyGUID equals d.TelecomCompanyGUID
                          where d.TelecomCompanyGUID.ToString() == "C857ED92-D97A-4A8F-826F-C5D915109483"
                          || d.TelecomCompanyGUID.ToString() == "B5A2245E-5E66-490A-BBF0-D237B2F5C465"
                          select new
                          {
                              Value = b.TelecomCompanyOperationConfigGUID,
                              Text = d.TelecomCompanyDescription + " > " + c.OperationDescription
                          }).ToList();
            return new SelectList(Result, "Value", "Text");
        }

        public SelectList TelecomCompaniesConfigGuidForLandlineList()
        {
            var Result = (from a in DbCMS.codeTelecomCompanyOperation.Where(x => x.Active)
                          join b in DbCMS.configTelecomCompanyOperation.Where(x => x.Active) on a.TelecomCompanyOperationGUID equals b.TelecomCompanyOperationGUID
                          join c in DbCMS.codeOperationsLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.OperationGUID equals c.OperationGUID
                          join d in DbCMS.codeTelecomCompanyLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.TelecomCompanyGUID equals d.TelecomCompanyGUID
                          where d.TelecomCompanyGUID.ToString() == "A0C0A374-1FBD-459F-ADA2-0DFA5A4C0762"
                          select new
                          {
                              Value = b.TelecomCompanyOperationConfigGUID,
                              Text = d.TelecomCompanyDescription + " > " + c.OperationDescription
                          }).ToList();
            return new SelectList(Result, "Value", "Text");
        }

        public SelectList SimCardTypes()
        {
            List<SelectListItem> Result = new List<SelectListItem>() {
                new SelectListItem { Value = "00000000-0000-0000-0000-000000000001", Text = "2G" },
                new SelectListItem { Value = "00000000-0000-0000-0000-000000000002", Text = "3G" }
            };

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList TelecomCompaniesByOperationList()
        {
            var Result = (from a in DbCMS.codeTelecomCompanyOperation.Where(x => x.Active)
                          join b in DbCMS.codeTelecomCompanyLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.TelecomCompanyGUID equals b.TelecomCompanyGUID
                          join c in DbCMS.codeOperationsLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.OperationGUID equals c.OperationGUID
                          select new
                          {
                              Value = a.TelecomCompanyOperationGUID,
                              Text = b.TelecomCompanyDescription + " > " + c.OperationDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList BillingTelecomCompanies()
        {
            var Result = (from a in DbCMS.codeTelecomCompanyLanguages.Where(x => x.LanguageID == LAN && x.Active)
                          select new
                          {
                              Value = a.TelecomCompanyGUID,
                              Text = a.TelecomCompanyDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList BillReports()
        {
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableGUID == LookupTables.BillForType && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");

        }

        public SelectList BillingMonthesForReport()
        {
            List<SelectListItem> Result = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "January" },
                new SelectListItem { Value = "2", Text = "February" },
                new SelectListItem { Value = "3", Text = "March" },
                new SelectListItem { Value = "4", Text = "April" },
                new SelectListItem { Value = "5", Text = "May" },
                new SelectListItem { Value = "6", Text = "June" },
                new SelectListItem { Value = "7", Text = "July" },
                new SelectListItem { Value = "8", Text = "August" },
                new SelectListItem { Value = "9", Text = "September" },
                new SelectListItem { Value = "10", Text = "October" },
                new SelectListItem { Value = "11", Text = "November" },
                new SelectListItem { Value = "12", Text = "December" },
            };
            return new SelectList(Result, "Value", "Text");
        }

        public SelectList BillingYearsForReport()
        {
            List<SelectListItem> Result = new List<SelectListItem>
            {
                new SelectListItem { Value = "2020", Text = "2020" },
                new SelectListItem { Value = "2021", Text = "2021" }
            };
            return new SelectList(Result, "Value", "Text");
        }

        public SelectList BillProcessingMethod()
        {
            List<SelectListItem> Result = new List<SelectListItem>() {
                new SelectListItem { Value = "M", Text = "Manual" },
                new SelectListItem { Value = "A", Text = "Automatic" },
                new SelectListItem { Value = "S", Text = "Scheduled", Disabled= true }
            };
            return new SelectList(Result, "Value", "Text");
        }
        #endregion

        #region WMS
        public SelectList WarehouseLicenseTypes()
        {
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableGUID == LookupTables.WarehouseLicenseType && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList WarehouseCostCenters()
        {
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableGUID == LookupTables.WarehouseCostCenter && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList WarehoueDamagedItemsReimbursementStatus()
        {
            Guid _status = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7596");
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableGUID == _status && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList WarehoueDamagedReportFlowStatus()
        {

            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableGUID == LookupTables.WarehoueDamagedReportFlowStatus && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList StaffSTIItems(Guid UserGUID)
        {
            WMSEntities DbWMS = new WMSEntities();
            var _listInputDetailGUID = DbWMS.dataItemOutputDetailDamagedTrack.Where(x => x.Active).Select(x => x.ItemInputDetailGUID).Distinct().ToList();
            var Result = (from a in DbWMS.v_EntryMovementDataTable.Where(x => x.Active && x.LastCustdianNameGUID == UserGUID && !_listInputDetailGUID.Contains(x.ItemInputDetailGUID))

                          select new
                          {
                              Value = a.ItemInputDetailGUID.ToString(),
                              Text = a.ModelDescription + "(" + (a.BarcodeNumber != null ? "BC: " + a.BarcodeNumber : a.SerialNumber != null ? "SN: " + a.SerialNumber : a.IMEI != null ? "IMEI: " + a.IMEI : a.GSM != null ? "GSM: " + a.GSM : "") + ")"


                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList UsersForWarehouse()
        {
            var _myProfile = DbCMS.userProfiles.Where(x => x.UserProfileGUID == UserProfileGUID).FirstOrDefault();
            var Result = (from a in DbCMS.userProfiles.Where(x => x.OrganizationInstanceGUID == _myProfile.OrganizationInstanceGUID && x.Active)
                          join b in DbCMS.userServiceHistory.Where(x => x.Active) on a.ServiceHistoryGUID equals b.ServiceHistoryGUID

                          join c in DbCMS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.Active) on b.UserGUID equals c.UserGUID

                          select new
                          {
                              Value = b.UserGUID.ToString(),
                              Text = c.FirstName + " " + c.FatherName + " " + c.Surname
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList GovernoratesPerUser()
        {
            var _profile = DbCMS.userProfiles.Where(x => x.UserProfileGUID == UserProfileGUID).FirstOrDefault();
            var _codeorg = DbCMS.codeOrganizationsInstances.Where(x => x.OrganizationInstanceGUID == _profile.OrganizationInstanceGUID).FirstOrDefault();
            Guid LocationType = Guid.Parse("C970DDF5-31E5-47A1-BE76-AF15833D4E6A"); // Governorate

            var Result = (from a in DbCMS.codeLocations.Where(x => x.LocationTypeGUID == LocationType && x.Active)
                          join b in DbCMS.codeLocationsLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.LocationGUID equals b.LocationGUID
                          join c in DbCMS.codeOperations.Where(x => x.OperationGUID == _codeorg.OperationGUID) on a.CountryGUID equals c.CountryGUID
                          orderby b.LocationDescription
                          select new
                          {
                              Value = a.LocationGUID.ToString(),
                              Text = b.LocationDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList CountriesCIMT()
        {
            WMSEntities DbWMS = new WMSEntities();

            Guid OrganizationInstanceGUID = Guid.Parse(HttpContext.Current.Session[SessionKeys.OrganizationInstanceGUID].ToString());

            var Result = (from a in DbWMS.codeCountriesConfigurations.Where(x => x.OrganizationInstanceGUID == OrganizationInstanceGUID && x.Active)
                          join b in DbWMS.codeCountries.Where(x => x.Active && x.CountryNameCIMT != null) on a.CountryGUID equals b.CountryGUID
                          orderby b.CountryNameCIMT
                          select new
                          {
                              Value = a.CountryGUID.ToString(),
                              Text = b.CountryNameCIMT
                          }).ToList();

            //return Countries;
            return new SelectList(Result, "Value", "Text");
        }
        public SelectList CIMTOfficeDutyStation(Guid? countryGUID)
        {
            WMSEntities DbWMS = new WMSEntities();
            var Result = (from a in DbWMS.codeWMSCIMTCountryOffice
                          where a.CountryGUID == countryGUID
                          select new
                          {
                              Value = a.CountryOfficeGUID,
                              Text = a.OfficeSiteName + " " + a.DutyStationCode
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList CodeWarehouseItemFeatureTypes()
        {
            WMSEntities DbWMS = new WMSEntities();


            var Result = (from a in DbWMS.codeWMSFeatureType.Where(x => x.Active)

                          select new
                          {
                              Value = a.FeatureTypeGUID.ToString(),
                              Text = a.Name
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList CodeWarehouseItemFeatureTypesValues(Guid? _featureGUID)
        {
            WMSEntities DbWMS = new WMSEntities();



            if (_featureGUID == Guid.Empty || _featureGUID == null)
            {
                return Empty();
            }
            var Result = (from a in DbWMS.codeWMSFeatureTypeValue.Where(x => x.Active && x.FeatureTypeGUID == _featureGUID)


                          select new
                          {
                              Value = a.FeatureTypeValueGUID.ToString(),
                              Text = a.Name
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList CodeWarehouseItemFeatureDocumentValues()
        {
            WMSEntities DbWMS = new WMSEntities();



            Guid _doc = Guid.Parse("29765244-0925-4237-8834-63f5480d3377");
            var Result = (from a in DbWMS.codeWMSFeatureTypeValue.Where(x => x.Active && x.FeatureTypeGUID == _doc)


                          select new
                          {
                              Value = a.FeatureTypeValueGUID.ToString(),
                              Text = a.Name
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList WMSContractVendors()
        {
            WMSEntities DbWMS = new WMSEntities();



            var Result = (from a in DbWMS.codeWMSVendor.Where(x => x.Active)

                          select new
                          {
                              Value = a.VendorGUID.ToString(),
                              Text = a.VendorName
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList WarehousePOFileTypes()
        {

            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableGUID == LookupTables.WarehouePOFileTypes && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }


        public SelectList WarehoueLicenseSubscriptionContractClass()
        {

            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableGUID == LookupTables.WarehoueLicenseSubscriptionContractClass && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList WarehoueLicenseSubscriptionContractType()
        {

            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableGUID == LookupTables.WarehoueLicenseSubscriptionContractType && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList WarehoueLicenseSubscriptionContractCategory()
        {

            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableGUID == LookupTables.WarehoueLicenseSubscriptionContractCategory && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList WarehoueLicenseSubscriptionRemindDateType()
        {

            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableGUID == LookupTables.WarehoueLicenseSubscriptionRemindDateType && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList WarehouseItemTags()
        {
            WMSEntities DbWMS = new WMSEntities();
            var Result = (from a in DbWMS.codeWMSItemTag.Where(x => x.Active)

                          select new
                          {
                              Value = a.ItemTagGUID.ToString(),
                              Text = a.TagName
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList WarehouseDocumentFileType()
        {
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableGUID == LookupTables.WarehoueItemDocumentType && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList WarehouseStaff(Guid PartyTypeGUID)
        {
            var _myProfile = DbCMS.userProfiles.Where(x => x.UserProfileGUID == UserProfileGUID).FirstOrDefault();
            var Result = (from a in DbCMS.userProfiles.Where(x => x.OrganizationInstanceGUID == _myProfile.OrganizationInstanceGUID && x.Active)
                          join b in DbCMS.userServiceHistory.Where(x => x.Active) on a.ServiceHistoryGUID equals b.ServiceHistoryGUID

                          join c in DbCMS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.Active) on b.UserGUID equals c.UserGUID

                          select new
                          {
                              Value = b.UserGUID.ToString(),
                              Text = c.FirstName + " " + c.Surname
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
            //var Result = (from a in DbCMS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.Active)


            //              select new
            //              {
            //                  Value = a.UserGUID.ToString(),
            //                  Text = a.FirstName + " " + a.Surname
            //              }).ToList();

            //return new SelectList(Result, "Value", "Text");
        }

        public SelectList WarehouseReports()
        {


            var Result = (from a in DbCMS.dataWarehosueReport.Where(x => x.Active)

                          select new
                          {
                              Value = a.WarehosueReportId.ToString(),
                              Text = a.ReportName
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList WarehouseModelRequester(Guid RequesterGUID)
        {

            if (RequesterGUID == WarehouseRequestSourceTypes.Staff)
            {

                var _myProfile = DbCMS.userProfiles.Where(x => x.UserProfileGUID == UserProfileGUID).FirstOrDefault();
                var Result = (from a in DbCMS.userProfiles.Where(x => x.OrganizationInstanceGUID == _myProfile.OrganizationInstanceGUID && x.Active)
                              join b in DbCMS.userServiceHistory.Where(x => x.Active) on a.ServiceHistoryGUID equals b.ServiceHistoryGUID

                              join c in DbCMS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.Active) on b.UserGUID equals c.UserGUID

                              select new
                              {
                                  Value = b.UserGUID.ToString(),
                                  Text = c.FirstName + " " + c.Surname
                              }).ToList();

                return new SelectList(Result, "Value", "Text");
            }
            else if (RequesterGUID == WarehouseRequestSourceTypes.Warehouse)
            {
                var currentWarehouseGUID = DbCMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).Select(x => x.WarehouseGUID)
                    .FirstOrDefault();
                var _currwarehouse = DbCMS.codeWarehouse.Where(x => x.WarehouseGUID == currentWarehouseGUID).FirstOrDefault();
                var Result = (from a in DbCMS.codeWarehouse.Where(x => x.Active &&
                              //((x.WarehouseGUID== _currwarehouse.WarehouseGUID ) || x.ParentGUID== _currwarehouse.WarehouseGUID || x.WarehouseGUID==_currwarehouse.ParentGUID)
                              (x.ParentGUID == _currwarehouse.ParentGUID) || (x.WarehouseGUID == _currwarehouse.WarehouseGUID && x.ParentGUID == null) || (x.WarehouseGUID == _currwarehouse.WarehouseGUID)
                              )
                              join b in DbCMS.codeWarehouseLanguage.Where(x => x.LanguageID == LAN && x.Active) on a.WarehouseGUID equals b.WarehouseGUID
                              select new
                              {
                                  Value = a.WarehouseGUID.ToString(),
                                  Text = b.WarehouseDescription
                              }).Distinct().ToList();

                return new SelectList(Result, "Value", "Text");
            }

            else if (RequesterGUID == WarehouseRequestSourceTypes.OtherRequester)
            {
                var Result = (from a in DbCMS.codeWarehouseRequesterType
                              join b in DbCMS.codeWarehouseRequesterTypeLanguage.Where(x => x.LanguageID == LAN && x.Active) on a.WarehouseRequesterTypeGUID equals b.WarehouseRequesterTypeGUID
                              select new
                              {
                                  Value = a.WarehouseRequesterTypeGUID.ToString(),
                                  Text = b.WarehouseRequesterTypeDescription
                              }).Distinct().ToList();

                return new SelectList(Result, "Value", "Text");
            }
            else if (RequesterGUID == WarehouseRequestSourceTypes.Vehicle)
            {
                var Result = (from a in DbCMS.codeWarehouseVehicle
                              join b in DbCMS.codeWarehouseVehicleLanguage.Where(x => x.LanguageID == LAN && x.Active) on a.WarehouseVehicleGUID equals b.WarehouseVehicleGUID
                              select new
                              {
                                  Value = a.WarehouseVehicleGUID.ToString(),
                                  Text = b.VehicleDescription
                              }).Distinct().ToList();

                return new SelectList(Result, "Value", "Text");
            }
            return null;
        }
        //public SelectList WarehouseModelRequester(Guid RequesterGUID)
        //{

        //    if (RequesterGUID == WarehouseRequestSourceTypes.Staff)
        //    {
        //        var Result = (from a in DbCMS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.Active)
        //                      join b in DbCMS.userServiceHistory on a.UserGUID equals b.UserGUID

        //                      select new
        //                      {
        //                          Value = a.UserGUID.ToString(),
        //                          Text = a.FirstName + " " + a.Surname
        //                      }).ToList();

        //        return new SelectList(Result, "Value", "Text");
        //    }
        //    else if (RequesterGUID == WarehouseRequestSourceTypes.Warehouse)
        //    {
        //        var Result = (from a in DbCMS.codeWarehouse.Where(x => x.Active)
        //                      join b in DbCMS.codeWarehouseLanguage.Where(x => x.LanguageID == LAN && x.Active) on a.WarehouseGUID equals b.WarehouseGUID
        //                      select new
        //                      {
        //                          Value = a.WarehouseGUID.ToString(),
        //                          Text = b.WarehouseDescription
        //                      }).Distinct().ToList();

        //        return new SelectList(Result, "Value", "Text");
        //    }

        //    else if (RequesterGUID == WarehouseRequestSourceTypes.OtherRequester)
        //    {
        //        var Result = (from a in DbCMS.codeWarehouseRequesterType
        //                      join b in DbCMS.codeWarehouseRequesterTypeLanguage.Where(x => x.LanguageID == LAN && x.Active) on a.WarehouseRequesterTypeGUID equals b.WarehouseRequesterTypeGUID
        //                      select new
        //                      {
        //                          Value = a.WarehouseRequesterTypeGUID.ToString(),
        //                          Text = b.WarehouseRequesterTypeDescription
        //                      }).Distinct().ToList();

        //        return new SelectList(Result, "Value", "Text");
        //    }
        //    else if (RequesterGUID == WarehouseRequestSourceTypes.Vehicle)
        //    {
        //        var Result = (from a in DbCMS.codeWarehouseVehicle
        //                      join b in DbCMS.codeWarehouseVehicleLanguage.Where(x => x.LanguageID == LAN && x.Active) on a.WarehouseVehicleGUID equals b.WarehouseVehicleGUID
        //                      select new
        //                      {
        //                          Value = a.WarehouseVehicleGUID.ToString(),
        //                          Text = b.VehicleDescription
        //                      }).Distinct().ToList();

        //        return new SelectList(Result, "Value", "Text");
        //    }
        //    return null;
        //}
        public SelectList ModelDeterminants(Guid ItemModelWarehouseGUID)
        {
            WMSEntities DbWMS = new WMSEntities();

            var CurrentWarehouseGUID = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).Select(x => x.WarehouseGUID)
                  .ToList();

            var Result = (from a in DbWMS.dataItemInputDeterminant.Where(x => x.Active)
                          join b in DbWMS.dataItemInputDetail.Where(x => x.Active && x.IsAvaliable == true && x.ItemModelWarehouseGUID == ItemModelWarehouseGUID) on a.ItemInputDetailGUID equals b.ItemInputDetailGUID
                          join c in DbWMS.dataItemTransfer.Where(x => x.Active && x.IsLastTransfer == true && CurrentWarehouseGUID.Contains(x.DestionationGUID)) on b.ItemInputDetailGUID equals c.ItemInputDetailGUID

                          select new
                          {
                              Value = a.ItemInputDetailGUID.ToString(),
                              Text = a.DeterminantValue
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList GetModelDeterminants()
        {
            WMSEntities DbWMS = new WMSEntities();

            var CurrentWarehouseGUID = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).Select(x => x.WarehouseGUID)
                .ToList();

            var Result = (from a in DbWMS.dataItemInputDeterminant.Where(x => x.Active)
                          join b in DbWMS.dataItemInputDetail.Where(x => x.Active && x.IsAvaliable == true && x.LastFlowTypeGUID == WarehouseRequestFlowType.Returned) on a.ItemInputDetailGUID equals b.ItemInputDetailGUID
                          join c in DbWMS.dataItemTransfer.Where(x => x.Active && x.IsLastTransfer == true && CurrentWarehouseGUID.Contains(x.DestionationGUID)) on b.ItemInputDetailGUID equals c.ItemInputDetailGUID


                          select new
                          {
                              Value = a.ItemInputDetailGUID.ToString(),
                              Text = a.DeterminantValue
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList GetModelAllDeterminants()
        {
            WMSEntities DbWMS = new WMSEntities();

            Guid barcode = Guid.Parse("D5E2A4DF-C67E-4F4D-A242-7E33C981EC11");
            Guid serialNumber = Guid.Parse("D5E2A4DF-C67E-4F4D-A242-7E33C981EC12");
            var Result = (from a in DbWMS.dataItemInputDeterminant.Where(x => x.Active && (x.codeWarehouseItemModelDeterminant.DeterminantGUID == barcode || x.codeWarehouseItemModelDeterminant.DeterminantGUID == serialNumber))
                          join b in DbWMS.dataItemInputDetail.Where(x => x.Active) on a.ItemInputDetailGUID equals b.ItemInputDetailGUID



                          select new
                          {
                              Value = a.ItemInputDetailGUID.ToString(),
                              Text = a.DeterminantValue
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList GetConsumableItem()
        {
            WMSEntities DbWMS = new WMSEntities();

            var CurrentWarehouseGUID = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).Select(x => x.WarehouseGUID)
                .ToList();

            var Result = (from a in DbWMS.codeItemModelWarehouse.Where(x => x.Active && CurrentWarehouseGUID.Contains(x.WarehouseGUID))
                          join b in DbWMS.codeWarehouseItemModelLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.WarehouseItemModelGUID equals b.WarehouseItemModelGUID


                          select new
                          {
                              Value = a.ItemModelWarehouseGUID.ToString(),
                              Text = b.ModelDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList Warehouses(Guid PartyTypeGUID)
        {
            var Result = (from a in DbCMS.codeWarehouseLanguage.Where(x => x.LanguageID == LAN && x.Active)
                          join b in DbCMS.codeWarehouse on a.WarehouseGUID equals b.WarehouseGUID

                          select new
                          {
                              Value = a.WarehouseGUID.ToString(),
                              Text = a.WarehouseDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList WarehousesVehicals()
        {
            var Result = (from a in DbCMS.codeWarehouseVehicleLanguage.Where(x => x.Active && x.LanguageID == LAN)
                          select new
                          {
                              Value = a.WarehouseVehicleGUID,
                              Text = a.VehicleDescription
                          }).ToList();
            return new SelectList(Result, "Value", "Text");
        }

        public SelectList WarehousePartners()
        {
            WMSEntities DbWMS = new WMSEntities();

            var Result = (from a in DbWMS.codeWMSPartner.Where(x => x.Active)
                          select new
                          {
                              Value = a.PartnerGUID.ToString(),
                              Text = a.PartnerName
                          }).OrderBy(x => x.Text).ToList();

            return new SelectList(Result, "Value", "Text");

        }




        public SelectList WarehouseRequsterTypes()
        {
            var Result = (from a in DbCMS.codeTables.Where(x => x.TableGUID == LookupTables.WarehouseRequsterTypes && x.Active)
                          join b in DbCMS.codeTablesValues.Where(x => x.Active) on a.TableGUID equals b.TableGUID
                          join c in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on b.ValueGUID equals c.ValueGUID
                          orderby b.SortID
                          select new
                          {
                              Value = b.ValueGUID.ToString(),
                              Text = c.ValueDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");


        }
        public SelectList WarehouseModelDamagedTypes()
        {
            var Result = (from a in DbCMS.codeTables.Where(x => x.TableGUID == LookupTables.WarehouseModelDamagedTypes && x.Active)
                          join b in DbCMS.codeTablesValues.Where(x => x.Active) on a.TableGUID equals b.TableGUID
                          join c in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on b.ValueGUID equals c.ValueGUID
                          orderby b.SortID
                          select new
                          {
                              Value = b.ValueGUID.ToString(),
                              Text = c.ValueDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");


        }
        public SelectList WarehouseEntrySourceTypes()
        {
            var Result = (from a in DbCMS.codeTables.Where(x => x.TableGUID == LookupTables.WarehouseEntrySourceTypes && x.Active)
                          join b in DbCMS.codeTablesValues.Where(x => x.Active) on a.TableGUID equals b.TableGUID
                          join c in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on b.ValueGUID equals c.ValueGUID
                          orderby b.SortID
                          select new
                          {
                              Value = b.ValueGUID.ToString(),
                              Text = c.ValueDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");


        }
        public SelectList WarehouseConsumableRequsterTypes()
        {
            var Result = (from a in DbCMS.codeTables.Where(x => x.TableGUID == LookupTables.WarehouseConsumableRequsterTypes && x.Active)
                          join b in DbCMS.codeTablesValues.Where(x => x.Active) on a.TableGUID equals b.TableGUID
                          join c in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on b.ValueGUID equals c.ValueGUID
                          orderby b.SortID
                          select new
                          {
                              Value = b.ValueGUID.ToString(),
                              Text = c.ValueDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");


        }



        //public SelectList PartyName(Guid PartyTypeGUID)
        //{
        //    var Result = (from a in DbCMS.codePartyType.Where(a => a.Active)
        //        join b in DbCMS.codePartyTypeLanguage.Where(x => x.LanguageID == LAN) on a.PartyTypeGUID equals b.PartyTypeGUID
        //        orderby a.SortID
        //        select new
        //        {
        //            Value = a.PartyTypeGUID.ToString(),
        //            Text = b.PartyTypeDescription
        //        }).ToList();

        //    return new SelectList(Result, "Value", "Text");
        //}
        public SelectList WarehouseItems(Guid WarehouseItemClassificationUID)
        {
            var Result = (from a in DbCMS.codeWarehouseItem.Where(x => x.Active && x.WarehouseItemClassificationGUID == WarehouseItemClassificationUID
                                                                       )
                          join b in DbCMS.codeWarehouseItemLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.WarehouseItemGUID equals b.WarehouseItemGUID
                          select new
                          {
                              Value = b.WarehouseItemGUID,
                              Text = b.WarehouseItemDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }


        public SelectList Districts(Guid GovernorateGUID)
        {
            var Result = (from a in DbCMS.codeLocations.Where(x => x.Active && x.LocationParentGUID == GovernorateGUID
                )
                          join b in DbCMS.codeLocationsLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.LocationGUID equals b.LocationGUID
                          orderby b.LocationDescription
                          select new
                          {
                              Value = b.LocationGUID,
                              Text = b.LocationDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList Districts()
        {
            var Result = (from a in DbCMS.codeLocations.Where(x => x.Active)
                          join b in DbCMS.codeLocationsLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.LocationGUID equals b.LocationGUID
                          select new
                          {
                              Value = b.LocationGUID,
                              Text = b.LocationDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }



        public SelectList SubDistricts(Guid DistrictGUID)
        {
            var Result = (from a in DbCMS.codeLocations.Where(x => x.Active && x.LocationParentGUID == DistrictGUID
                )
                          join b in DbCMS.codeLocationsLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.LocationGUID equals b.LocationGUID
                          orderby b.LocationDescription
                          select new
                          {
                              Value = b.LocationGUID,
                              Text = b.LocationDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList Communities(Guid SubDistrictGUID)
        {
            var Result = (from a in DbCMS.codeLocations.Where(x => x.Active && x.LocationParentGUID == SubDistrictGUID
                )
                          join b in DbCMS.codeLocationsLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.LocationGUID equals b.LocationGUID
                          orderby b.LocationDescription
                          select new
                          {
                              Value = b.LocationGUID,
                              Text = b.LocationDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList WarehouseItems()
        {
            var Result = (from a in DbCMS.codeWarehouseItem.Where(x => x.Active)
                          join b in DbCMS.codeWarehouseItemLanguage.Where(x => x.LanguageID == LAN && x.Active) on a.WarehouseItemGUID equals b.WarehouseItemGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()
                          orderby R1.WarehouseItemDescription
                          select new
                          {
                              Value = a.WarehouseItemGUID.ToString(),
                              Text = R1.WarehouseItemDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList ModelDeterminants()
        {
            var Result = (from a in DbCMS.codeTables.Where(x => x.TableGUID == LookupTables.LocationTypes && x.Active)
                          join b in DbCMS.codeTablesValues.Where(x => x.Active) on a.TableGUID equals b.TableGUID
                          join c in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on b.ValueGUID equals c.ValueGUID
                          orderby b.SortID
                          select new
                          {
                              Value = b.ValueGUID.ToString(),
                              Text = c.ValueDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList MovementStatus()
        {
            Guid itemStatus = Guid.Parse("0C44822F-A898-476D-B291-CAF1B055AAC6");
            var Result = (from a in DbCMS.codeTables.Where(x => x.TableGUID == itemStatus && x.Active)
                          join b in DbCMS.codeTablesValues.Where(x => x.Active) on a.TableGUID equals b.TableGUID
                          join c in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on b.ValueGUID equals c.ValueGUID
                          orderby b.SortID
                          select new
                          {
                              Value = b.ValueGUID.ToString(),
                              Text = c.ValueDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList ItemVerificationStatus()
        {

            var Result = (from a in DbCMS.codeTables.Where(x => x.TableGUID == LookupTables.ItemVerificationStatus && x.Active)
                          join b in DbCMS.codeTablesValues.Where(x => x.Active) on a.TableGUID equals b.TableGUID
                          join c in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on b.ValueGUID equals c.ValueGUID
                          orderby b.SortID
                          select new
                          {
                              Value = b.ValueGUID.ToString(),
                              Text = c.ValueDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList ModelStatus()
        {
            Guid itemStatus = Guid.Parse("675DE853-151B-4C2F-93F4-DA1434EEE761");
            var Result = (from a in DbCMS.codeTables.Where(x => x.TableGUID == itemStatus && x.Active)
                          join b in DbCMS.codeTablesValues.Where(x => x.Active) on a.TableGUID equals b.TableGUID
                          join c in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on b.ValueGUID equals c.ValueGUID
                          orderby b.SortID
                          select new
                          {
                              Value = b.ValueGUID.ToString(),
                              Text = c.ValueDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList Brands()
        {
            var Result = (from a in DbCMS.codeBrand.Where(x => x.Active)
                          join b in DbCMS.codeBrandLanguage.Where(x => x.LanguageID == LAN && x.Active) on a.BrandGUID equals b.BrandGUID
                          orderby b.BrandDescription
                          select new
                          {
                              Value = a.BrandGUID.ToString(),
                              Text = b.BrandDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList Items()
        {
            var Result = (from a in DbCMS.codeWarehouseItem.Where(x => x.Active)
                          join b in DbCMS.codeWarehouseItemLanguage.Where(x => x.LanguageID == LAN && x.Active) on a.WarehouseItemGUID equals b.WarehouseItemGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()
                          orderby R1.WarehouseItemDescription
                          select new
                          {
                              Value = a.WarehouseItemGUID.ToString(),
                              Text = R1.WarehouseItemDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList ItemClassification()
        {
            Guid ICTGuid = Guid.Parse("9317FBDA-E360-45CC-A064-11E6A21C1E17");
            var Result = (from a in DbCMS.codeWarehouseItemClassification.Where(x => x.Active && x.WarehouseTypeGUID == ICTGuid)
                          join b in DbCMS.codeWarehouseItemClassificationLanguage.Where(x => x.LanguageID == LAN && x.Active) on a.WarehouseItemClassificationGUID equals b.WarehouseItemClassificationGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()
                          orderby R1.WarehouseItemClassificationDescription
                          select new
                          {
                              Value = a.WarehouseItemClassificationGUID.ToString(),
                              Text = R1.WarehouseItemClassificationDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList WarehouseItemModels()
        {
            //Guid damGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            var currentWarehouse = DbCMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID)
                    .FirstOrDefault();

            var allParents = DbCMS.codeWarehouse.Where(x => x.ParentGUID == null).Select(x => x.WarehouseGUID).Distinct().ToList();
            if (currentWarehouse.codeWarehouse.ParentGUID == null)
            {

                var currInstance = DbCMS.codeWarehouse.Where(x => (x.WarehouseGUID == currentWarehouse.codeWarehouse.WarehouseGUID && x.ParentGUID == null)).FirstOrDefault();

                var Result = (from a in DbCMS.codeItemModelWarehouse.Where(x => x.Active && x.WarehouseGUID == currInstance.WarehouseGUID && x.codeWarehouseItemModel.codeWarehouseItem.IsDeterminanted == true)
                              join b in DbCMS.codeWarehouseItemModelLanguage.Where(x => x.Active && x.LanguageID == LAN
                              && x.ModelDescription != null) on a.WarehouseItemModelGUID equals b.WarehouseItemModelGUID into LJ1
                              from R1 in LJ1.DefaultIfEmpty()

                                  //orderby a.codeWarehouseItemModel.codeWarehouseItemModelLanguage.Where(x=>x.LanguageID==LAN).Select(x=>x.ModelDescription)
                              select new
                              {
                                  Value = a.ItemModelWarehouseGUID.ToString(),
                                  Text = R1.ModelDescription
                              }).OrderBy(x => x.Text).Distinct().ToList();

                return new SelectList(Result, "Value", "Text");

            }
            else
            {
                var currInstance = DbCMS.codeWarehouse.Where(x => (currentWarehouse.codeWarehouse.ParentGUID != null && (currentWarehouse.codeWarehouse.ParentGUID != null && x.ParentGUID == null && allParents.Contains((Guid)currentWarehouse.codeWarehouse.ParentGUID)))).FirstOrDefault();

                var Result = (from a in DbCMS.codeItemModelWarehouse.Where(x => x.Active && x.WarehouseGUID == currInstance.WarehouseGUID && x.codeWarehouseItemModel.codeWarehouseItem.IsDeterminanted == true)
                              join b in DbCMS.codeWarehouseItemModelLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.WarehouseItemModelGUID equals b.WarehouseItemModelGUID into LJ1
                              from R1 in LJ1.DefaultIfEmpty()

                                  //orderby a.codeWarehouseItemModel.codeWarehouseItemModelLanguage.Where(x=>x.LanguageID==LAN).Select(x=>x.ModelDescription)
                              select new
                              {
                                  Value = a.ItemModelWarehouseGUID.ToString(),
                                  Text = R1.ModelDescription
                              }).OrderBy(x => x.Text).Distinct().ToList();

                return new SelectList(Result, "Value", "Text");

            }


        }


        //public SelectList WarehouseItemModels()
        //{


        //    var Result = (from a in DbCMS.codeWarehouseItemModelLanguage.Where(x => x.Active && x.LanguageID == LAN)
        //                  join b in DbCMS.codeItemModelWarehouse.Where(x => x.Active) on a.WarehouseItemModelGUID equals b.WarehouseItemModelGUID into LJ1
        //                  from R1 in LJ1.DefaultIfEmpty()

        //                      //orderby a.codeWarehouseItemModel.codeWarehouseItemModelLanguage.Where(x=>x.LanguageID==LAN).Select(x=>x.ModelDescription)
        //                  select new
        //                  {
        //                      Value = R1.ItemModelWarehouseGUID.ToString(),
        //                      Text = a.ModelDescription
        //                  }).OrderBy(x => x.Text).ToList();

        //    return new SelectList(Result, "Value", "Text");

        //}


        public SelectList WarehouseModels()
        {

            var Result = (from a in DbCMS.codeWarehouseItemModelLanguage.Where(x => x.Active && x.LanguageID == LAN && x.codeWarehouseItemModel.codeWarehouseItem.IsDeterminanted != false)
                          join b in DbCMS.codeItemModelWarehouse.Where(x => x.Active) on a.WarehouseItemModelGUID equals b.WarehouseItemModelGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()

                              //orderby a.codeWarehouseItemModel.codeWarehouseItemModelLanguage.Where(x=>x.LanguageID==LAN).Select(x=>x.ModelDescription)
                          select new
                          {
                              Value = R1.ItemModelWarehouseGUID.ToString(),
                              Text = a.ModelDescription
                          }).OrderBy(x => x.Text).ToList();

            return new SelectList(Result, "Value", "Text");

        }
        public SelectList WarehouseConsumableModels()
        {

            var Result = (from a in DbCMS.codeWarehouseItemModelLanguage.Where(x => x.Active && x.LanguageID == LAN && x.codeWarehouseItemModel.codeWarehouseItem.IsDeterminanted == false)


                              //orderby a.codeWarehouseItemModel.codeWarehouseItemModelLanguage.Where(x=>x.LanguageID==LAN).Select(x=>x.ModelDescription)
                          select new
                          {
                              Value = a.WarehouseItemModelGUID.ToString(),
                              Text = a.ModelDescription
                          }).OrderBy(x => x.Text).ToList();

            return new SelectList(Result, "Value", "Text");

        }
        public SelectList WarehouseUserWarehouses()
        {
            WMSEntities DbWMS = new WMSEntities();

            var Result = (from a in DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID)
                          join b in DbWMS.codeWarehouseLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.WarehouseGUID equals b.WarehouseGUID into LJ1

                          from R1 in LJ1.DefaultIfEmpty()

                              //orderby a.codeWarehouseItemModel.codeWarehouseItemModelLanguage.Where(x=>x.LanguageID==LAN).Select(x=>x.ModelDescription)
                          select new
                          {
                              Value = a.WarehouseGUID.ToString(),
                              Text = R1.WarehouseDescription
                          }).OrderBy(x => x.Text).ToList();

            return new SelectList(Result, "Value", "Text");

        }
        public SelectList WarehouseConsumableModels(Guid warehouseGUID)
        {

            var Result = (from a in DbCMS.codeItemModelWarehouse.Where(x => x.Active && x.codeWarehouseItemModel.codeWarehouseItem.IsDeterminanted == false && x.WarehouseGUID == warehouseGUID)
                          join b in DbCMS.codeWarehouseItemModelLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.WarehouseItemModelGUID equals b.WarehouseItemModelGUID into LJ1

                          from R1 in LJ1.DefaultIfEmpty()

                              //orderby a.codeWarehouseItemModel.codeWarehouseItemModelLanguage.Where(x=>x.LanguageID==LAN).Select(x=>x.ModelDescription)
                          select new
                          {
                              Value = a.ItemModelWarehouseGUID.ToString(),
                              Text = R1.ModelDescription
                          }).OrderBy(x => x.Text).ToList();

            return new SelectList(Result, "Value", "Text");

        }
        public SelectList WarehouseNotifyStaffByEmail()
        {
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableGUID == LookupTables.WarehouseNotifyStaff && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList WarehouseReleaseTypes()
        {
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableGUID == LookupTables.ReleaseTypes && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList WarehouseItemRelationTypes()
        {
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableGUID == LookupTables.ItemRelationTypes && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList WarehouseItemStatus()
        {
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableGUID == LookupTables.ItemStatus && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList WarehouseItemServiceStatus()
        {
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableGUID == LookupTables.ItemServiceStatus && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList WarehouseItemCondition()
        {
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableGUID == LookupTables.ItemConditions && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList WarehousePurposeofuse()
        {
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableGUID == LookupTables.WarehousePorpouseofUse && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList WarehouseItemInputParentKit()
        {
            WMSEntities DbWMS = new WMSEntities();

            var currentWarehouseGUID = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).Select(x => x.WarehouseGUID)
           .FirstOrDefault();
            var currInstance = DbWMS.codeWarehouse.Where(x => x.WarehouseGUID == currentWarehouseGUID).FirstOrDefault();

            var _checkdetGUIDs = DbWMS.v_EntryMovementDataTable.Where(x => x.OrganizationInstanceGUID == currInstance.OrganizationInstanceGUID &&
          //warehouseAuthGUIDs.Contains((Guid)x.WarehouseOwnerGUID) && 
          (x.IsDeterminanted == true || x.IsDeterminanted == null)).Select(x => x.ItemInputDetailGUID).Distinct();

            var Result = (from a in DbWMS.dataItemInputDetail.Where(x => x.Active && _checkdetGUIDs.Contains(x.ItemInputDetailGUID)
                          //&& x.codeItemModelWarehouse.codeWarehouseItemModel.ItemModelRelationTypeGUID == LookupTables.ItemRelationMainKit
                          )
                          join b in DbWMS.dataItemInputDeterminant.Where(x => x.Active) on a.ItemInputDetailGUID equals b.ItemInputDetailGUID


                          select new
                          {
                              Value = a.ItemInputDetailGUID.ToString(),
                              Text = b.DeterminantValue
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList InventoryStatus()
        {
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableGUID == LookupTables.InventoryStatus && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }


        public SelectList PriceTypes()
        {
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableGUID == LookupTables.PriceTypes && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList ColorsNames()
        {
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableGUID == LookupTables.ColorNames && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList WarehouseItemDeterminants()
        {
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableGUID == LookupTables.ModelDeterminants && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList ModelDeliveryStatus()
        {
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableGUID == LookupTables.ModelDeliveryStatus && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList Warehouses()
        {


            var currentWarehouse = DbCMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).FirstOrDefault();

            //var _currwarehouse = DbCMS.codeWarehouse.Where(x => x.WarehouseGUID == currentWarehouseGUID).FirstOrDefault();
            var allwarehosus = DbCMS.codeWarehouse.Where(x => x.WarehouseGUID == currentWarehouse.codeWarehouse.WarehouseGUID || x.ParentGUID == currentWarehouse.WarehouseGUID || (x.WarehouseGUID == currentWarehouse.codeWarehouse.ParentGUID) ||
               (x.ParentGUID != null && x.ParentGUID == currentWarehouse.codeWarehouse.ParentGUID)).Select(x => x.WarehouseGUID).ToList();

            var Result = (from a in DbCMS.codeWarehouse.Where(x => x.Active
                          && allwarehosus.Contains(x.WarehouseGUID))
                              //&& (x.WarehouseGUID == _currwarehouse.WarehouseGUID || x.ParentGUID == _currwarehouse.WarehouseGUID || x.WarehouseGUID == _currwarehouse.ParentGUID))
                              //&& (x.ParentGUID == _currwarehouse.ParentGUID) || (x.WarehouseGUID == _currwarehouse.WarehouseGUID && x.ParentGUID == null) || (x.WarehouseGUID == _currwarehouse.WarehouseGUID))
                          join b in DbCMS.codeWarehouseLanguage.Where(x => x.LanguageID == LAN && x.Active) on a.WarehouseGUID equals b.WarehouseGUID
                          select new
                          {
                              Value = a.WarehouseGUID.ToString(),
                              Text = b.WarehouseDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
            //var Result = (from a in DbCMS.codeWarehouse.Where(x => x.Active)
            //              join b in DbCMS.codeWarehouseLanguage.Where(x => x.LanguageID == LAN && x.Active) on a.WarehouseGUID equals b.WarehouseGUID
            //              select new
            //              {
            //                  Value = a.WarehouseGUID.ToString(),
            //                  Text = b.WarehouseDescription
            //              }).Distinct().ToList();

            //return new SelectList(Result, "Value", "Text");
        }


        public SelectList WarehouseSIMPackageSize()
        {
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableGUID == LookupTables.SIMPackageSize && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription,
                              SortID = a.SortID
                          }).Distinct().OrderBy(x => x.SortID).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList WarehouseLocation()
        {
            WMSEntities DbWMS = new WMSEntities();

            var _profile = DbWMS.userProfiles.Where(x => x.UserProfileGUID == UserProfileGUID).FirstOrDefault();
            var Result = (from a in DbWMS.codeWarehouseLocation.Where(x => x.Active && x.OrgnanizationInstanceGUID == _profile.OrganizationInstanceGUID)
                          join b in DbWMS.codeWarehouseLocationLanguage.Where(x => x.LanguageID == LAN && x.Active) on a.WarehouseLocationGUID equals b.WarehouseLocationGUID
                          select new
                          {
                              Value = a.WarehouseLocationGUID.ToString(),
                              Text = b.WarehouseLocationDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList WarehouseSubLocation()
        {
            var Result = (from a in DbCMS.codeWarehouseSubLocation.Where(x => x.Active)
                          join b in DbCMS.codeWarehouseSubLocationLanguage.Where(x => x.LanguageID == LAN && x.Active) on a.WarehouseSubLocationGUID equals b.WarehouseSubLocationGUID
                          select new
                          {
                              Value = a.WarehouseSubLocationGUID.ToString(),
                              Text = b.WarehouseSubLocationDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList WarehousePrinters()
        {
            //zz
            WMSEntities DbWMS = new WMSEntities();
            var CurrentWarehouseGUID = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).Select(x => x.WarehouseGUID)
                .ToList();
            Guid generalequ = Guid.Parse("0F3F63BF-8BB0-477D-8377-98B24BA5116F");


            var Result = (from a in DbWMS.dataItemInputDetail.Where(x =>
            x.codeItemModelWarehouse.codeWarehouseItemModel.codeWarehouseItem.codeWarehouseItemClassification.WarehouseItemClassificationGUID == generalequ
            && x.Active && CurrentWarehouseGUID.Contains(x.dataItemTransfer.Where(f => f.IsLastTransfer == true).FirstOrDefault().DestionationGUID) && x.IsDeterminanted == true)
                          join b in DbWMS.dataItemInputDeterminant.Where(x => x.Active
                           && (x.codeWarehouseItemModelDeterminant.DeterminantGUID ==
                                                                 ItemDeterminants.SerialNumber
                                                                 || x.codeWarehouseItemModelDeterminant.DeterminantGUID ==
                                                                 ItemDeterminants.Barcode)) on a.ItemInputDetailGUID equals b.ItemInputDetailGUID
                          select new
                          {
                              Value = b.ItemInputDeterminantGUID.ToString(),
                              Text = b.DeterminantValue
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }


        public SelectList OtherRequester()
        {
            var Result = (from a in DbCMS.codeWarehouseRequesterType
                          join b in DbCMS.codeWarehouseRequesterTypeLanguage.Where(x => x.LanguageID == LAN && x.Active) on a.WarehouseRequesterTypeGUID equals b.WarehouseRequesterTypeGUID
                          select new
                          {
                              Value = a.WarehouseRequesterTypeGUID.ToString(),
                              Text = b.WarehouseRequesterTypeDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");

        }



        public SelectList WarehouseItemKitModels()
        {

            var Result = (from a in DbCMS.codeWarehouseItemModel.Where(x => x.Active && x.ItemModelRelationTypeGUID == LookupTables.ItemRelationMainKit)
                          join b in DbCMS.codeWarehouseItemModelLanguage.Where(x => x.LanguageID == LAN && x.Active) on a.WarehouseItemModelGUID equals b.WarehouseItemModelGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()
                          orderby R1.ModelDescription
                          select new
                          {
                              Value = a.WarehouseItemModelGUID.ToString(),
                              Text = R1.ModelDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList FloorNumber()
        {
            List<SelectListItem> Result = new List<SelectListItem>() {
                new SelectListItem { Value = "0", Text = "0" },
                new SelectListItem { Value = "1", Text = "1" },
                new SelectListItem { Value = "2", Text = "2" },
                new SelectListItem { Value = "3", Text = "3" },
                new SelectListItem { Value = "4", Text = "4" },
                new SelectListItem { Value = "5", Text = "5" }, };
            return new SelectList(Result, "Value", "Text");
        }

        #endregion

        #region IMS
        public SelectList MissionReportDocumentType()
        {
            Guid myTable = Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3999");
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.TableGUID == myTable && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList SyiraDutyStations()
        {
            Guid syriaGuid = Guid.Parse("710BFD1B-50CC-4F1E-92A9-A70583CFA5E0");
            var Result = (from a in DbCMS.codeDutyStations.Where(x => x.Active && x.CountryGUID == syriaGuid)
                          join b in DbCMS.codeDutyStationsLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.DutyStationGUID equals b.DutyStationGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()
                          select new
                          {
                              Value = a.DutyStationGUID.ToString(),
                              Text = R1.DutyStationDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList AllDistricts()
        {
            var Result = (from a in DbCMS.codeLocations.Where(x => x.Active && x.LocationlevelID == 2)
                          join b in DbCMS.codeLocationsLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.LocationGUID equals b.LocationGUID
                          select new
                          {
                              Value = b.LocationGUID,
                              Text = b.LocationDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList MissionOfficeSource()
        {

            var Result = (from a in DbCMS.codeMissionOfficeSource.Where(x => x.Active)
                          join b in DbCMS.codeMissionOfficeSourceLanguage.Where(x => x.LanguageID == LAN && x.Active) on a.MissionOfficeSourceGUID equals b.MissionOfficeSourceGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()
                          select new
                          {
                              Value = a.MissionOfficeSourceGUID.ToString(),
                              Text = R1.MissionOfficeSourceDescription
                          }).OrderBy(x => x.Text).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList AllSubDistricts()
        {
            var Result = (from a in DbCMS.codeLocations.Where(x => x.Active && x.LocationlevelID == 3
                )
                          join b in DbCMS.codeLocationsLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.LocationGUID equals b.LocationGUID
                          orderby b.LocationDescription
                          select new
                          {
                              Value = b.LocationGUID,
                              Text = b.LocationDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList AllCommunities()
        {
            var Result = (from a in DbCMS.codeLocations.Where(x => x.Active && x.LocationlevelID == 4
                )
                          join b in DbCMS.codeLocationsLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.LocationGUID equals b.LocationGUID
                          orderby b.LocationDescription
                          select new
                          {
                              Value = b.LocationGUID,
                              Text = b.LocationDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList IMSMissionFormCode()
        {
            var Result = (from a in DbCMS.dataMissionReportForm.Where(x => x.Active)
                          orderby a.MissionNumber
                          select new
                          {
                              Value = a.MissionCode,
                              Text = a.MissionCode
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList SyriaDutyStations()
        {
            Guid syriaCountryGUID = Guid.Parse("710BFD1B-50CC-4F1E-92A9-A70583CFA5E0");
            var Result = (from a in DbCMS.codeDutyStations.Where(x => x.Active && x.CountryGUID == syriaCountryGUID)
                          join b in DbCMS.codeDutyStationsLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.DutyStationGUID equals b.DutyStationGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()
                          orderby R1.DutyStationDescription
                          select new SelectListDutyStation
                          {
                              Value = a.DutyStationGUID.ToString(),
                              Text = R1.DutyStationDescription,
                              Group = ""
                          }).ToList();
            Result.ForEach(y => y.Group = DbCMS.codePartnerCenter.Where(x => x.DutyStationGUID.ToString() == y.Value && x.Active).Select(x => x.OrganizationInstanceGUID).Distinct().ToArray().Length > 0 ?
            string.Join(",", DbCMS.codePartnerCenter.Where(x => x.DutyStationGUID.ToString() == y.Value && x.Active).Select(x => x.OrganizationInstanceGUID).Distinct().ToArray()) : Guid.Empty.ToString());

            SelectList sl = new SelectList(Result, "Value", "Text", "Group", -1);
            return sl;
        }

        public SelectList SyriaDutyStationsForPCR()
        {
            Guid syriaCountryGUID = Guid.Parse("710BFD1B-50CC-4F1E-92A9-A70583CFA5E0");
            var Result = (from a in DbCMS.codeDutyStations.Where(x => x.Active && x.CountryGUID == syriaCountryGUID)
                          join b in DbCMS.codeDutyStationsLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.DutyStationGUID equals b.DutyStationGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()
                          where  R1.DutyStationDescription != "Qamishli"
                          orderby R1.DutyStationDescription
                          select new SelectListDutyStation
                          {
                              Value = a.DutyStationGUID.ToString(),
                              Text = R1.DutyStationDescription,
                              Group = ""
                          }).ToList();
            Result.ForEach(y => y.Group = DbCMS.codePartnerCenter.Where(x => x.DutyStationGUID.ToString() == y.Value && x.Active).Select(x => x.OrganizationInstanceGUID).Distinct().ToArray().Length > 0 ?
            string.Join(",", DbCMS.codePartnerCenter.Where(x => x.DutyStationGUID.ToString() == y.Value && x.Active).Select(x => x.OrganizationInstanceGUID).Distinct().ToArray()) : Guid.Empty.ToString());

            SelectList sl = new SelectList(Result, "Value", "Text", "Group", -1);
            return sl;
        }
        class SelectListDutyStation
        {
            public string Value { get; set; }
            public string Text { get; set; }
            public string Group { get; set; }

        }
        public SelectList MissionStatus()
        {
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableGUID == LookupTables.IMSMissionStatus && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          orderby b.ValueDescription
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }



        public SelectList VisitObjectives()
        {
            Guid otherGuid = Guid.Parse("CEC1E764-1282-4C71-B4D3-00063D470001");
            var Result = (from a in DbCMS.codeIMSVisitObjectiveLanguage.Where(x => x.LanguageID == LAN && x.Active && x.VisitObjectiveGUID != otherGuid)
                          orderby a.VisitObjectiveDescription
                          select new
                          {
                              Value = a.VisitObjectiveGUID.ToString(),
                              Text = a.VisitObjectiveDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList codeVisitObjectives()
        {
            Guid otherGuid = Guid.Parse("CEC1E764-1282-4C71-B4D3-00063D470001");
            var Result = (from a in DbCMS.codeIMSVisitObjectiveLanguage.Where(x => x.LanguageID == LAN && x.Active && x.VisitObjectiveGUID != otherGuid)
                          orderby a.VisitObjectiveDescription
                          select new
                          {
                              Value = a.VisitObjectiveDescription.ToString(),
                              Text = a.VisitObjectiveDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList codeHumanitarianNeeds()
        {
            Guid otherGuid = Guid.Parse("4EC1E764-1282-4C71-B4D3-00069D080112");

            var Result = (from a in DbCMS.codeIMSHumanitarianNeedLanguage.Where(x => x.LanguageID == LAN && x.Active && x.HumanitarianNeedGUID != otherGuid)
                          orderby a.HumanitarianNeedeDescription
                          select new
                          {
                              Value = a.HumanitarianNeedeDescription.ToString(),
                              Text = a.HumanitarianNeedeDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList codeOngoingResponses()
        {
            Guid otherGuid = Guid.Parse("CEC1E764-1282-4C71-B4D3-00069D080113");
            var Result = (from a in DbCMS.codeIMSOngoingResponseLanguage.Where(x => x.LanguageID == LAN && x.Active && x.OngoingResponseGUID != otherGuid)
                          orderby a.OngoingResponseDescription
                          select new
                          {
                              Value = a.OngoingResponseDescription.ToString(),
                              Text = a.OngoingResponseDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList codeDepartments()
        {
            var Result = (from a in DbCMS.codeDepartments.Where(x => x.Active)
                          join b in DbCMS.codeDepartmentsLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.DepartmentGUID equals b.DepartmentGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()
                          orderby R1.DepartmentDescription
                          select new
                          {
                              Value = R1.DepartmentDescription,
                              Text = R1.DepartmentDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList codeUsers(bool IncludeMe)
        {
            var Result = (from a in DbCMS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.Active && (IncludeMe || x.UserGUID != UserGUID))
                          orderby a.FirstName, a.Surname
                          select new
                          {
                              Value = a.FirstName + " " + a.Surname,
                              Text = a.FirstName + " " + a.Surname
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList codeMissionChallenges()
        {
            Guid otherGuid = Guid.Parse("4EC1E764-1282-4C71-B4D3-02069D0801B6");
            var Result = (from a in DbCMS.codeIMSMissionChallengeLanguage.Where(x => x.LanguageID == LAN && x.Active && x.MissionChallengeGUID != otherGuid)
                          orderby a.MissionChallengeDescription
                          select new
                          {
                              Value = a.MissionChallengeDescription.ToString(),
                              Text = a.MissionChallengeDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList HumanitarianNeeds()
        {
            Guid otherGuid = Guid.Parse("4EC1E764-1282-4C71-B4D3-00069D080112");

            var Result = (from a in DbCMS.codeIMSHumanitarianNeedLanguage.Where(x => x.LanguageID == LAN && x.Active && x.HumanitarianNeedGUID != otherGuid)
                          orderby a.HumanitarianNeedeDescription
                          select new
                          {
                              Value = a.HumanitarianNeedGUID.ToString(),
                              Text = a.HumanitarianNeedeDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList OngoingResponses()
        {
            Guid otherGuid = Guid.Parse("CEC1E764-1282-4C71-B4D3-00069D080113");
            var Result = (from a in DbCMS.codeIMSOngoingResponseLanguage.Where(x => x.LanguageID == LAN && x.Active && x.OngoingResponseGUID != otherGuid)
                          orderby a.OngoingResponseDescription
                          select new
                          {
                              Value = a.OngoingResponseGUID.ToString(),
                              Text = a.OngoingResponseDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }


        public SelectList MissionChallenges()
        {
            Guid otherGuid = Guid.Parse("4EC1E764-1282-4C71-B4D3-02069D0801B6");
            var Result = (from a in DbCMS.codeIMSMissionChallengeLanguage.Where(x => x.LanguageID == LAN && x.Active && x.MissionChallengeGUID != otherGuid)
                          orderby a.MissionChallengeDescription
                          select new
                          {
                              Value = a.MissionChallengeGUID.ToString(),
                              Text = a.MissionChallengeDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList MissionActionTakenStatus()
        {
            var Result = (from a in DbCMS.codeTables.Where(x => x.TableGUID == LookupTables.IMSMissionActionTakenStatus && x.Active)
                          join b in DbCMS.codeTablesValues.Where(x => x.Active) on a.TableGUID equals b.TableGUID
                          join c in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on b.ValueGUID equals c.ValueGUID
                          orderby a.SortID
                          select new
                          {
                              Value = b.ValueGUID.ToString(),
                              Text = c.ValueDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }
        #endregion

        #region ISS
        public SelectList StockInsightItems()
        {

            var Result = (from a in DbCMS.codeISSItem.Where(x => x.Active)
                          join b in DbCMS.codeISSItemLanguage.Where(x => x.LanguageID == LAN && x.Active) on a.ItemGUID equals b.ItemGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()
                          orderby R1.ItemDescription
                          select new
                          {
                              Value = a.ItemGUID.ToString(),
                              Text = R1.ItemDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList StockInsighsStocks()
        {

            var Result = (from a in DbCMS.codeISSStock.Where(x => x.Active)
                          join b in DbCMS.codeISSStockLanguage.Where(x => x.LanguageID == LAN && x.Active) on a.StockGUID equals b.StockGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()
                          orderby R1.StockDescription
                          select new
                          {
                              Value = a.StockGUID.ToString(),
                              Text = R1.StockDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }
        #endregion

        #region EMT
        public SelectList MedicalItemsSequance()
        {
            using (var DbEMT = new EMTEntities())
            {
                var Result = (from a in DbEMT.spMedicalItem("EN")
                              orderby a.BrandName
                              select new
                              {
                                  Value = a.Sequance.ToString(),
                                  Text = a.BrandName + ", " + a.DoseQuantity
                              }).ToList();

                return new SelectList(Result, "Value", "Text");
            }
        }

        public SelectList MedicalItemGroupSequance()
        {
            using(var DbEMT = new EMTEntities()){
                var Result = (from a in DbEMT.spMedicalItem(LAN)
                              orderby a.BrandName
                              select new
                              {
                                  Value = a.Sequance,
                                  Text = a.BrandName + ", " + a.DoseQuantity,
                                  Group = a.MedicalPharmacologicalFormGUID + "," + a.MedicalTreatmentGUID + "," + a.MedicalGenericNameGUID
                              }).ToList();
                return new SelectList(Result, "Value", "Text", "Group", -1);
            }
        }

        public SelectList MedicalItems()
        {
            using (var DbEMT = new EMTEntities())
            {
                var Result = (from a in DbEMT.spMedicalItem("EN")
                              orderby a.BrandName
                              select new
                              {
                                  Value = a.MedicalItemGUID.ToString(),
                                  Text = a.BrandName + ", " + a.DoseQuantity
                              }).ToList();

                return new SelectList(Result, "Value", "Text");
            }
        }

        public SelectList MedicalItemGroup()
        {
            using (var DbEMT = new EMTEntities())
            {
                var Result = (from a in DbEMT.spMedicalItem(LAN)
                              orderby a.BrandName
                              select new
                              {
                                  Value = a.MedicalItemGUID,
                                  Text = a.BrandName + ", " + a.DoseQuantity,
                                  Group = a.MedicalPharmacologicalFormGUID + "," + a.MedicalTreatmentGUID + "," + a.MedicalGenericNameGUID
                              }).ToList();
                return new SelectList(Result, "Value", "Text", "Group", -1);
            }
        }

        public SelectList MedicalItemTransferDetails(string PK)
        {
            if (PK == null) { return Empty(); }
            Guid MedicalItemGUID = Guid.Parse(PK.Split(',')[0]);
            Guid MedicalPharmacyGUID = Guid.Parse(PK.Split(',')[1]);
            if (PK == null) { return Empty(); }
            var Result = (from a in DbCMS.dataMedicalItemTransferDetail.Where(x => x.dataMedicalItemTransfer.MedicalPharmacyGUID == MedicalPharmacyGUID && x.MedicalItemGUID == MedicalItemGUID && x.Active && x.RemainingItems > 0)
                          select new
                          {
                              date = a.dataMedicalItemInputDetail.ExpirationDate,
                              Value = a.MedicalItemTransferDetailGUID.ToString(),
                              Text = "Batch Number : " + a.dataMedicalItemInputDetail.BatchNumber + " - Remaining Items : " + a.RemainingItems
                          }).OrderBy(x => x.date).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList MedicalItemTransferDetailOne(Guid? PK)
        {
            if (PK == Guid.Empty || PK == null) { return Empty(); }
            var Result = (from a in DbCMS.dataMedicalItemTransferDetail.Where(x => x.MedicalItemTransferDetailGUID == PK && x.Active )
                          select new
                          {
                              date = a.dataMedicalItemInputDetail.ExpirationDate,
                              Value = a.MedicalItemTransferDetailGUID.ToString(),
                              Text = "Batch Number : " + a.dataMedicalItemInputDetail.BatchNumber + " - Remaining Items : " + a.RemainingItems
                          }).OrderBy(x => x.date).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList MedicalItemInputDetails(string PK)
        {
            if (PK == null) { return Empty(); }
            Guid MedicalItemGUID = Guid.Parse(PK.Split(',')[0]);
            Guid OrganizationInstanceGUID = Guid.Parse(PK.Split(',')[1]);
            var Result = (from a in DbCMS.dataMedicalItemInputDetail.Where(x => x.MedicalItemGUID == MedicalItemGUID && x.dataMedicalItemInput.ProcuredByOrganizationInstanceGUID == OrganizationInstanceGUID && x.Active && x.RemainingItems > 0 && x.Confirmed)
                          select new
                          {
                              date = a.ExpirationDate,
                              Value = a.MedicalItemInputDetailGUID.ToString(),
                              Text = "Batch Number : " + a.BatchNumber + " - Remaining Items : " + a.RemainingItems
                          }).OrderBy(x => x.date).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList MedicalItemInputDetailOne(Guid? PK)
        {
            if (PK == Guid.Empty || PK == null) { return Empty(); }

            var Result = (from a in DbCMS.dataMedicalItemInputDetail.Where(x => x.MedicalItemInputDetailGUID == PK && x.Active)
                          select new
                          {
                              date = a.ExpirationDate,
                              Value = a.MedicalItemInputDetailGUID.ToString(),
                              Text = "Batch Number : " + a.BatchNumber + " - Remaining Items : " + a.RemainingItems
                          }).OrderBy(x => x.date).ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList MedicalGenericName()
        {
            var Result = (from a in DbCMS.codeMedicalGenericName.Where(x => x.Active)
                          join b in DbCMS.codeMedicalGenericNameLanguage.Where(x => x.LanguageID == LAN && x.Active) on a.MedicalGenericNameGUID equals b.MedicalGenericNameGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()
                          orderby R1.MedicalGenericNameDescription
                          select new
                          {
                              Value = a.MedicalGenericNameGUID.ToString(),
                              Text = R1.MedicalGenericNameDescription + " - " + a.Dose + " - " + a.Form
                          }).OrderBy(x=>x.Text).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList MedicalSource()
        {
            List<SelectListItem> Result = new List<SelectListItem>() {
                new SelectListItem { Value = "00000000-0000-0000-0000-000000000001", Text = LAN=="EN"? "Imported":"مستورد" },
                new SelectListItem { Value = "00000000-0000-0000-0000-000000000002", Text = LAN=="EN"?"Local":"محلي" },
                };
            return new SelectList(Result, "Value", "Text");
        }

        public SelectList DiseaseType()
        {
            List<SelectListItem> Result = new List<SelectListItem>() {
                new SelectListItem { Value = "00000000-0000-0000-0000-000000000001", Text = LAN=="EN"? "Acute":"حاد" },
                new SelectListItem { Value = "00000000-0000-0000-0000-000000000002", Text = LAN=="EN"?"Chronic":"مزمن" },
                 new SelectListItem { Value = "00000000-0000-0000-0000-000000000003", Text = LAN=="EN"?"Mixed":"مختلط" },
                };
            return new SelectList(Result, "Value", "Text");
        }


        public SelectList BeneficiaryType()
        {
            //List<SelectListItem> Result = new List<SelectListItem>() {
            //    new SelectListItem { Value = "00000000-0000-0000-0000-000000000001", Text = LAN=="EN"? "Asylum Seeker":"طالب لجوء" },
            //    new SelectListItem { Value = "00000000-0000-0000-0000-000000000002", Text = LAN=="EN"?"Host Community":"المجتمع المضيف" },
            //    new SelectListItem { Value = "00000000-0000-0000-0000-000000000003", Text = LAN=="EN"?"Refugee":"لاجئ" },
            //    new SelectListItem { Value = "00000000-0000-0000-0000-000000000004", Text = LAN=="EN"?"Returnee":"العائدين" },
            //    new SelectListItem { Value = "00000000-0000-0000-0000-000000000005", Text = LAN=="EN"?"IDP":"النازحين" },
            //    new SelectListItem { Value = "00000000-0000-0000-0000-000000000006", Text = LAN=="EN"?"IDP Returnee":"النازحين العائدين" },
            //    };
            var Result = this.LookupValues(LookupTables.BeneficiaryType);
            return new SelectList(Result, "Value", "Text");
        }

        public SelectList MedicalManufacturer()
        {
            var Result = (from a in DbCMS.codeMedicalManufacturer.Where(x => x.Active)
                          join b in DbCMS.codeMedicalManufacturerLanguage.Where(x => x.LanguageID == LAN && x.Active) on a.MedicalManufacturerGUID equals b.MedicalManufacturerGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()
                          orderby R1.MedicalManufacturerDescription
                          select new
                          {
                              Value = a.MedicalManufacturerGUID.ToString(),
                              Text = R1.MedicalManufacturerDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList MedicalPharmacy()
        {
            //Access is authorized by Access Action
            List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.MedicalPharmacy.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();

            var Result = (from a in DbCMS.codeMedicalPharmacy.Where(x => x.Active).Where(x => AuthorizedList.Contains(x.MedicalPharmacyGUID.ToString()))
                          join b in DbCMS.codeMedicalPharmacyLanguage.Where(x => x.LanguageID == LAN && x.Active) on a.MedicalPharmacyGUID equals b.MedicalPharmacyGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()
                          join c in DbCMS.codeOrganizationsInstances.Where(x => x.Active) on a.OrganizationInstanceGUID equals c.OrganizationInstanceGUID into LJ2
                          from R2 in LJ2.DefaultIfEmpty()
                          join c in DbCMS.codeOrganizations.Where(x => x.Active) on R2.OrganizationGUID equals c.OrganizationGUID into LJ3
                          from R3 in LJ3.DefaultIfEmpty()
                          orderby R3.OrganizationShortName
                          select new
                          {
                              Value = a.MedicalPharmacyGUID.ToString(),
                              Text = (a.MainWarehouse ? "W - " : "P - ") + R3.OrganizationShortName + " - " + R1.MedicalPharmacyDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList MedicalPharmacyGrorp()
        {
            //Access is authorized by Access Action
            List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.MedicalPharmacy.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();

            var Result = (from a in DbCMS.codeMedicalPharmacy.Where(x => x.Active).Where(x => AuthorizedList.Contains(x.MedicalPharmacyGUID.ToString()))
                          join b in DbCMS.codeMedicalPharmacyLanguage.Where(x => x.LanguageID == LAN && x.Active) on a.MedicalPharmacyGUID equals b.MedicalPharmacyGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()
                          join c in DbCMS.codeOrganizationsInstances.Where(x => x.Active) on a.OrganizationInstanceGUID equals c.OrganizationInstanceGUID into LJ2
                          from R2 in LJ2.DefaultIfEmpty()
                          join c in DbCMS.codeOrganizations.Where(x => x.Active) on R2.OrganizationGUID equals c.OrganizationGUID into LJ3
                          from R3 in LJ3.DefaultIfEmpty()
                          orderby R3.OrganizationShortName
                          select new
                          {
                              Value = a.MedicalPharmacyGUID.ToString(),
                              Text = (a.MainWarehouse ? "W - " : "P - ") + R1.MedicalPharmacyDescription,
                              Group = a.OrganizationInstanceGUID
                          }).ToList();

            return new SelectList(Result, "Value", "Text", "Group", -1);
        }
        public SelectList MedicalPharmacyMain(bool main)
        {
            List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.MedicalPharmacy.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();

            var Result = (from a in DbCMS.codeMedicalPharmacy.Where(x => x.Active && x.MainWarehouse == main).Where(x => AuthorizedList.Contains(x.MedicalPharmacyGUID.ToString()))
                          join b in DbCMS.codeMedicalPharmacyLanguage.Where(x => x.LanguageID == LAN && x.Active) on a.MedicalPharmacyGUID equals b.MedicalPharmacyGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()
                          join c in DbCMS.codeOrganizationsInstances.Where(x => x.Active) on a.OrganizationInstanceGUID equals c.OrganizationInstanceGUID into LJ2
                          from R2 in LJ2.DefaultIfEmpty()
                          join c in DbCMS.codeOrganizations.Where(x => x.Active) on R2.OrganizationGUID equals c.OrganizationGUID into LJ3
                          from R3 in LJ3.DefaultIfEmpty()
                          orderby R3.OrganizationShortName
                          select new
                          {
                              Value = a.MedicalPharmacyGUID.ToString(),
                              Text = (a.MainWarehouse ? "W - " : "P - ") + R3.OrganizationShortName + " - " + R1.MedicalPharmacyDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }


        public SelectList MedicalPharmacyByOrganizationInsatance(Guid? OrganizationInstanceGUID, bool main)
        {
            List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.MedicalPharmacy.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();
            if (OrganizationInstanceGUID != null)
            {
                var Result = (from a in DbCMS.codeMedicalPharmacy.Where(x => x.Active && x.OrganizationInstanceGUID == OrganizationInstanceGUID && x.MainWarehouse == main).Where(x => AuthorizedList.Contains(x.MedicalPharmacyGUID.ToString()))
                              join b in DbCMS.codeMedicalPharmacyLanguage.Where(x => x.LanguageID == LAN && x.Active) on a.MedicalPharmacyGUID equals b.MedicalPharmacyGUID into LJ1
                              from R1 in LJ1.DefaultIfEmpty()
                              join c in DbCMS.codeOrganizationsInstances.Where(x => x.Active) on a.OrganizationInstanceGUID equals c.OrganizationInstanceGUID into LJ2
                              from R2 in LJ2.DefaultIfEmpty()
                              join c in DbCMS.codeOrganizations.Where(x => x.Active) on R2.OrganizationGUID equals c.OrganizationGUID into LJ3
                              from R3 in LJ3.DefaultIfEmpty()
                              orderby R3.OrganizationShortName
                              select new
                              {
                                  Value = a.MedicalPharmacyGUID.ToString(),
                                  Text = (a.MainWarehouse ? "W - " : "P - ") + R3.OrganizationShortName + " - " + R1.MedicalPharmacyDescription
                              }).ToList();

                return new SelectList(Result, "Value", "Text");
            }
            else
            {
                return Empty();
            }
        }
        public SelectList MedicalPharmacyByOrganizationInsatanceExcludeTransferSourcePharmacy(/*Guid OrganizationInstanceGUID,*/ Guid SourcePharmacy)
        {
            List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.MedicalPharmacy.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();

            var Result = (from a in DbCMS.codeMedicalPharmacy.Where(x => x.Active /*&& x.OrganizationInstanceGUID == OrganizationInstanceGUID*/ && x.MedicalPharmacyGUID != SourcePharmacy).Where(x => AuthorizedList.Contains(x.MedicalPharmacyGUID.ToString()))
                          join b in DbCMS.codeMedicalPharmacyLanguage.Where(x => x.LanguageID == LAN && x.Active) on a.MedicalPharmacyGUID equals b.MedicalPharmacyGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()
                          join c in DbCMS.codeOrganizationsInstances.Where(x => x.Active) on a.OrganizationInstanceGUID equals c.OrganizationInstanceGUID into LJ2
                          from R2 in LJ2.DefaultIfEmpty()
                          join c in DbCMS.codeOrganizations.Where(x => x.Active) on R2.OrganizationGUID equals c.OrganizationGUID into LJ3
                          from R3 in LJ3.DefaultIfEmpty()
                          orderby R3.OrganizationShortName
                          select new
                          {
                              Value = a.MedicalPharmacyGUID.ToString(),
                              Text = (a.MainWarehouse ? "W - " : "P - ") + R3.OrganizationShortName + " - " + R1.MedicalPharmacyDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }


        public SelectList MedicalBeneficiary(Guid? PK)
        {
            if (PK == Guid.Empty || PK == null) { return Empty(); }
            var Result = (from a in DbCMS.dataMedicalBeneficiary.Where(x => x.MedicalBeneficiaryGUID == PK && x.Active)
                          select new
                          {
                              Value = a.MedicalBeneficiaryGUID.ToString(),
                              Text = "UNHCRNumber : " + a.UNHCRNumber + " - IDNumber : " + a.IDNumber
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList MedicalReportTopic()
        {
            List<SelectListItem> Result = new List<SelectListItem>() {
                new SelectListItem { Value = "6", Text = "Dashboard Summary" ,Selected =true,Group=new SelectListGroup(){ Name=""} },
                new SelectListItem { Value = "1", Text = "Dispensing Medicines to POC",Group=new SelectListGroup(){ Name="P"} },
                new SelectListItem { Value = "2", Text = "Cost of Dispensing Medicines" ,Group=new SelectListGroup(){ Name="P"}},
                new SelectListItem { Value = "3", Text = "Prrescriptions Dispensed" ,Group=new SelectListGroup(){ Name="P"}},
                new SelectListItem { Value = "4", Text = "Dispatched Medicines To Warehouses" ,Group=new SelectListGroup(){ Name="W"}},
                new SelectListItem { Value = "5", Text = "Transferred Medicines To Pharmacies" ,Group=new SelectListGroup(){ Name="P"}},
            };
            return new SelectList(Result, "Value", "Text");
        }
        //
        public SelectList MedicalReportTopicRELC()
        {
            List<SelectListItem> Result = new List<SelectListItem>() {
                new SelectListItem { Value = "0", Text =  "Dispensing By Individuals",Group=new SelectListGroup(){ Name="P"} },
                new SelectListItem { Value = "1", Text =  "Dispensing By Items" ,Group=new SelectListGroup(){ Name="P"}},
                new SelectListItem { Value = "7", Text =  "Dispensing By Items Pharmacy Matrix",Group=new SelectListGroup(){ Name="P"} },
                 new SelectListItem { Value = "2", Text = "Warehouse Remaining Items",Group=new SelectListGroup(){ Name="W"} },
                 new SelectListItem { Value = "3", Text = "Warehouse Items Details",Group=new SelectListGroup(){ Name="W"} },
                 new SelectListItem { Value = "6", Text = "Warehouse Remaining Items Matrix",Group=new SelectListGroup(){ Name="W"} },
                 new SelectListItem { Value = "4", Text = "Pharmacy Remaining Items",Group=new SelectListGroup(){ Name="P"} },
                 new SelectListItem { Value = "5", Text = "Pharmacy Remaining Items Matrix",Group=new SelectListGroup(){ Name="P"} },
                 new SelectListItem { Value = "8", Text = "Warehouse Expired Medicine",Group=new SelectListGroup(){ Name="W"} },
                 new SelectListItem { Value = "9", Text = "Pharmacy Expired Medicine",Group=new SelectListGroup(){ Name="P"} },
                 new SelectListItem { Value = "10", Text = "Pharmacy Inventory",Group=new SelectListGroup(){ Name="P"} },
                 new SelectListItem { Value = "11", Text = "Warehouse Inventory",Group=new SelectListGroup(){ Name="W"} },
                new SelectListItem { Value = "12", Text = "Beneficiary Details",Group=new SelectListGroup(){ Name="P"} },
                new SelectListItem { Value = "13", Text = "Calaculate Closing Balance Warehouse",Group=new SelectListGroup(){ Name="W"} },
                new SelectListItem { Value = "14", Text = "Calaculate Closing Balance Pharmacy",Group=new SelectListGroup(){ Name="P"} },
            };

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList OrganizationsInstancesPharmacyAcronymByProfileAll()
        {
            List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.MedicalPharmacy.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();
            var userOperation = (from a in DbCMS.userProfiles.Where(x => x.UserProfileGUID == UserProfileGUID)
                                 join b in DbCMS.codeOrganizationsInstances on a.OrganizationInstanceGUID equals b.OrganizationInstanceGUID
                                 join c in DbCMS.codeOperations on b.OperationGUID equals c.OperationGUID

                                 select new
                                 {
                                     c.OperationGUID
                                 }).FirstOrDefault();
            var Result = (from a in DbCMS.codeOrganizationsInstances.Where(x => x.OperationGUID == userOperation.OperationGUID)
                          join b in DbCMS.codeOrganizationsInstancesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.OrganizationInstanceGUID equals b.OrganizationInstanceGUID
                          join c in DbCMS.codeOrganizations.Where(x => x.Active) on a.OrganizationGUID equals c.OrganizationGUID
                          join d in DbCMS.codeMedicalPharmacy.Where(x => AuthorizedList.Contains(x.MedicalPharmacyGUID.ToString())) on a.OrganizationInstanceGUID equals d.OrganizationInstanceGUID
                          orderby b.OrganizationInstanceDescription
                          select new
                          {
                              Value = a.OrganizationInstanceGUID.ToString(),
                              Text = c.OrganizationShortName
                          }).ToList().Distinct();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList PharmacyByOrganizationInstance(string OrganizationInstanceGuid)
        {
            if (OrganizationInstanceGuid.Split('|').Count() == 3)
            {
                string report = OrganizationInstanceGuid.Split('|')[1] != "" ? OrganizationInstanceGuid.Split('|')[1] : "-";
                if (report != "-")
                {
                    string str = OrganizationInstanceGuid.Split('|')[2] == "D" ?
                        MedicalReportTopic().Items.Cast<SelectListItem>().Where(x => x.Value == report).FirstOrDefault().Group.Name :
                        MedicalReportTopicRELC().Items.Cast<SelectListItem>().Where(x => x.Value == report).FirstOrDefault().Group.Name;

                    bool IsWarehouse = str == "W" ? true : false;
                    string[] OrganizationInstanceGuids = OrganizationInstanceGuid.Split('|')[0].Split(',');

                    List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.MedicalPharmacy.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();

                    var Result = (from a in DbCMS.codeMedicalPharmacy.Where(x => OrganizationInstanceGuids.Contains(x.OrganizationInstanceGUID.ToString()) && x.Active && x.MainWarehouse == IsWarehouse).Where(x => AuthorizedList.Contains(x.MedicalPharmacyGUID.ToString()))
                                  join b in DbCMS.codeMedicalPharmacyLanguage.Where(l => l.LanguageID == LAN && l.Active) on a.MedicalPharmacyGUID equals b.MedicalPharmacyGUID into LJ1
                                  from R1 in LJ1.DefaultIfEmpty()
                                  join c in DbCMS.codeOrganizationsInstances.Where(l => l.Active) on a.OrganizationInstanceGUID equals c.OrganizationInstanceGUID into LJ3
                                  from R3 in LJ3.DefaultIfEmpty()
                                  join d in DbCMS.codeOrganizations.Where(l => l.Active) on R3.OrganizationGUID equals d.OrganizationGUID into LJ4
                                  from R4 in LJ4.DefaultIfEmpty()
                                  orderby new { R4.OrganizationShortName }
                                  select new
                                  {
                                      Value = a.MedicalPharmacyGUID.ToString(),
                                      Text = R1.MedicalPharmacyDescription,
                                      Group = a.OrganizationInstanceGUID
                                  }).OrderBy(x => x.Text).ToList();
                    return new SelectList(Result, "Value", "Text", "Group", -1);
                }
                else
                {
                    string[] OrganizationInstanceGuids = OrganizationInstanceGuid.Split(',');

                    List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.MedicalPharmacy.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();

                    var Result = (from a in DbCMS.codeMedicalPharmacy.Where(x => OrganizationInstanceGuids.Contains(x.OrganizationInstanceGUID.ToString()) && x.Active).Where(x => AuthorizedList.Contains(x.MedicalPharmacyGUID.ToString()))
                                  join b in DbCMS.codeMedicalPharmacyLanguage.Where(l => l.LanguageID == LAN && l.Active) on a.MedicalPharmacyGUID equals b.MedicalPharmacyGUID into LJ1
                                  from R1 in LJ1.DefaultIfEmpty()
                                  join c in DbCMS.codeOrganizationsInstances.Where(l => l.Active) on a.OrganizationInstanceGUID equals c.OrganizationInstanceGUID into LJ3
                                  from R3 in LJ3.DefaultIfEmpty()
                                  join d in DbCMS.codeOrganizations.Where(l => l.Active) on R3.OrganizationGUID equals d.OrganizationGUID into LJ4
                                  from R4 in LJ4.DefaultIfEmpty()
                                  orderby new { R4.OrganizationShortName }
                                  select new
                                  {
                                      Value = a.MedicalPharmacyGUID.ToString(),
                                      Text = R1.MedicalPharmacyDescription,
                                      Group = a.OrganizationInstanceGUID
                                  }).OrderBy(x => x.Text).ToList();
                    return new SelectList(Result, "Value", "Text", "Group", -1);
                }
            }
            return new SelectList(null, "Value", "Text", "Group", -1);

        }
        public SelectList MedicalItemsByTreatment(string TreatmentGUIDS)
        {
            string[] Treatment = TreatmentGUIDS.Split(',');
            var Result = (from a in DbCMS.spMedicalItem(LAN).Where(x => Treatment.Contains(x.MedicalTreatmentGUID.ToString()))
                          orderby a.BrandName
                          select new
                          {
                              Value = a.MedicalItemGUID.ToString(),
                              Text = a.BrandName + " - Dose: " + a.DoseQuantity,
                              Group = a.MedicalManufacturerGUID + "," + a.MedicalTreatmentGUID
                          }).ToList();

            return new SelectList(Result, "Value", "Text", "Group", -1);
        }
        public SelectList MedicalItemsByManufacturer(string ManufacturerGUIDS)
        {
            string[] Manufacturer = ManufacturerGUIDS.Split(',');
            var Result = (from a in DbCMS.spMedicalItem(LAN).Where(x => Manufacturer.Contains(x.MedicalManufacturerGUID.ToString()))
                          orderby a.BrandName
                          select new
                          {
                              Value = a.MedicalItemGUID.ToString(),
                              Text = a.BrandName + " - Dose: " + a.DoseQuantity,
                              Group = a.MedicalManufacturerGUID + "," + a.MedicalTreatmentGUID
                          }).ToList();

            return new SelectList(Result, "Value", "Text", "Group", -1);
        }
        public SelectList MedicalItemsByTreatmentByPharmacologicalForm(string PharmacologicalFormGUIDS, string TreatmentGUIDS,string MedicalGenericNameGUID)
        {
            using (var DbEMT = new EMTEntities())
            {
                if (PharmacologicalFormGUIDS == "")
                {
                    PharmacologicalFormGUIDS = string.Join(",", MedicalManufacturer().Select(x => x.Value).ToList().ConvertAll(Guid.Parse).ToArray());
                }
                if (TreatmentGUIDS == "")
                {
                    TreatmentGUIDS = string.Join(",", LookupValues(LookupTables.MedicalTreatment).Select(x => x.Value).ToList().ConvertAll(Guid.Parse).ToArray());
                }
                if (MedicalGenericNameGUID == "")
                {
                    MedicalGenericNameGUID = string.Join(",", MedicalGenericName().Select(x => x.Value).ToList().ConvertAll(Guid.Parse).ToArray());
                }
                string[] PharmacologicalFormGUIDSList = PharmacologicalFormGUIDS.Split(',');
                string[] TreatmentGUIDSList = TreatmentGUIDS.Split(',');
                string[] MedicalGenericNameGUIDSList = MedicalGenericNameGUID.Split(',');



                var Result = (from a in DbEMT.spMedicalItem(LAN).Where(x => TreatmentGUIDSList.Contains(x.MedicalTreatmentGUID.ToString()) &&
                                                                            PharmacologicalFormGUIDSList.Contains(x.MedicalPharmacologicalFormGUID.ToString()) &&
                                                                            MedicalGenericNameGUIDSList.Contains(x.MedicalGenericNameGUID.ToString()))
                              orderby a.BrandName
                              select new
                              {
                                  Value = a.Sequance,
                                  Text = a.BrandName + " - Dose: " + a.DoseQuantity,
                                  Group = a.MedicalPharmacologicalFormGUID + "," + a.MedicalTreatmentGUID + "," +a.MedicalGenericNameGUID
                              }).ToList();

                return new SelectList(Result, "Value", "Text", "Group", -1);
            }
        }
        public SelectList DiscrepancyType()
        {
            List<SelectListItem> Result = new List<SelectListItem>() {
                new SelectListItem { Value = "1", Text = LAN=="EN"? "Periodic inventory":"جرد دوري" },
                new SelectListItem { Value = "2", Text =  LAN=="EN"?  "Annual inventory":"جرد سنوي"},

            };
            return new SelectList(Result, "Value", "Text");
        }

        public SelectList DeliveryStatus()
        {
            List<SelectListItem> Result = new List<SelectListItem> {
                new SelectListItem { Value = "true", Text = "Completed" },
                new SelectListItem { Value = "false", Text = "Pending" }
            };

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList MedicalPharmacyAll()
        {
            List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.MedicalPharmacy.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();

            var Result = (from a in DbCMS.codeMedicalPharmacy.Where(x => x.Active).Where(x => AuthorizedList.Contains(x.MedicalPharmacyGUID.ToString()))
                          join b in DbCMS.codeMedicalPharmacyLanguage.Where(x => x.LanguageID == LAN && x.Active) on a.MedicalPharmacyGUID equals b.MedicalPharmacyGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()
                          join c in DbCMS.codeOrganizationsInstances.Where(x => x.Active) on a.OrganizationInstanceGUID equals c.OrganizationInstanceGUID into LJ2
                          from R2 in LJ2.DefaultIfEmpty()
                          join c in DbCMS.codeOrganizations.Where(x => x.Active) on R2.OrganizationGUID equals c.OrganizationGUID into LJ3
                          from R3 in LJ3.DefaultIfEmpty()
                          orderby R3.OrganizationShortName
                          select new
                          {
                              Value = a.MedicalPharmacyGUID.ToString(),
                              Text = (a.MainWarehouse ? "(W) " : "(P) ") + R3.OrganizationShortName + " - " + R1.MedicalPharmacyDescription
                          }).OrderBy(x => x.Text).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList DocumentType()
        {
            List<SelectListItem> Result = new List<SelectListItem>() {
                new SelectListItem { Value = "0", Text =  "Family Doc" },
                new SelectListItem { Value = "1", Text =  "ID Card" },
                new SelectListItem { Value = "3", Text =  "Partner Issued ID Card" },
                new SelectListItem { Value = "4", Text =  "UNHCR ID Card" },
                 new SelectListItem { Value = "5", Text = "Other" },
            };
            return new SelectList(Result, "Value", "Text");
        }
        #endregion

        #region AHD

        public SelectList AbsenceDuration()
        {
            List<SelectListItem> Result = new List<SelectListItem>() {
                new SelectListItem { Value = "1", Text = "Full Day" },
                new SelectListItem { Value = "0.5", Text = "Half Day" },
            };
            return new SelectList(Result, "Value", "Text");
        }
        public SelectList EntitlementDocumentTypes()
        {
            Guid myTable = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e9756");
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.TableGUID == myTable && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }
        //public SelectList InternationalStaffUsers(bool IncludeMe)
        //{
        //    Guid internationalGUID = Guid.Parse("FC4B2E79-2B97-4252-A50B-915B07A12310");
        //    Guid _activeStatus = Guid.Parse("B9CD375C-A576-4AA4-8AF4-FF3C1C4E3611");
        //    var staffCoreGUID = DbCMS.StaffCoreData.Where(x => x.RecruitmentTypeGUID == internationalGUID && x.Active && x.StaffStatusGUID == _activeStatus).Select(x => x.UserGUID).ToList();
        //    var Result = (from a in DbCMS.userPersonalDetailsLanguage.Where(x => staffCoreGUID.Contains(x.UserGUID) && x.LanguageID == LAN && x.Active && (IncludeMe || x.UserGUID != UserGUID))
        //                  orderby a.FirstName, a.Surname
        //                  select new
        //                  {
        //                      Value = a.UserGUID.ToString(),
        //                      Text = a.FirstName + " " + a.Surname
        //                  }).ToList();

        //    return new SelectList(Result, "Value", "Text");
        //}
        public SelectList AdminFilterStaffByDutyStation(Guid _dutyStationGUID)
        {
            AHDEntities DbAHD = new AHDEntities();

            var Result = (from a in DbAHD.v_StaffProfileInformation.Where(x => x.DutyStationGUID == _dutyStationGUID)


                          select new
                          {
                              Value = a.UserGUID.ToString(),
                              Text = a.FullName
                          }).OrderBy(x => x.Text).ToList();

            return new SelectList(Result, "Value", "Text");
        }
            public SelectList StaffMissionFlowStatus()
        {
            Guid _nvGUID = Guid.Parse("29591f83-32a4-49a5-b284-2a8efbc44453");
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableGUID == _nvGUID && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");

        }
        public SelectList AHDMissionTravelTypes()
        {
            Guid _nvGUID = Guid.Parse("29591f83-32a4-49a5-b284-2a8efbc46628");
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableGUID == _nvGUID && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList MissionItineraryTravelTypes()
        {
            Guid _nvGUID = Guid.Parse("66CD375C-A576-4AA4-8AF4-FF3C1C5E7886");
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableGUID == _nvGUID && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");

        }
        public SelectList AHDMissionTypes()
        {
            Guid _nvGUID = Guid.Parse("29591f83-32a4-49a5-b284-2a8efbc46683");
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableGUID == _nvGUID && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList AHDMissionDocumentTypes()
        {
            Guid _nvGUID = Guid.Parse("29591f83-32a4-49a5-b284-2a8efbc46616");
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableGUID == _nvGUID && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList AHDMissionModeOfTravel()
        {
            Guid _nvGUID = Guid.Parse("de719230-8a19-4c85-9f73-b6509a2e1b0f");
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableGUID == _nvGUID && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList AHDMissionAccommodationProvided()
        {
            Guid _nvGUID = Guid.Parse("c16d61d2-ec78-4f3c-bfbb-260fcc3c672d");
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableGUID == _nvGUID && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList AHDMissionMealsProvided()
        {
            Guid _nvGUID = Guid.Parse("9ae0302a-8eba-4385-a6b0-838d6b6020e1");
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableGUID == _nvGUID && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList AHDMissionTravelerType()
        {
            Guid _nvGUID = Guid.Parse("2dac5d96-e6a3-48c1-b3f5-17bfd9f62966");
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableGUID == _nvGUID && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList AllInternationalStaffUsers()
        {
            Guid internationalGUID = Guid.Parse("FC4B2E79-2B97-4252-A50B-915B07A12310");

            var staffCoreGUID = DbCMS.StaffCoreData.Where(x => x.RecruitmentTypeGUID == internationalGUID && x.Active).Select(x => x.UserGUID).ToList();
            var Result = (from a in DbCMS.userPersonalDetailsLanguage.Where(x => staffCoreGUID.Contains(x.UserGUID) && x.LanguageID == LAN && x.Active)
                          orderby a.FirstName, a.Surname
                          select new
                          {
                              Value = a.UserGUID.ToString(),
                              Text = a.FirstName + " " + a.Surname
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList AllStaffUsers()
        {


            var staffCoreGUID = DbCMS.StaffCoreData.Where(x => x.Active).Select(x => x.UserGUID).ToList();
            var Result = (from a in DbCMS.userPersonalDetailsLanguage.Where(x => staffCoreGUID.Contains(x.UserGUID) && x.LanguageID == LAN && x.Active)
                          orderby a.FirstName, a.Surname
                          select new
                          {
                              Value = a.UserGUID.ToString(),
                              Text = a.FirstName + " " + a.Surname
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList StaffEntitlmentsMovementStatus()
        {
            Guid myTable = Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3676");
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.TableGUID == myTable && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList EntitlementPeriods()
        {
            AHDEntities DbAHD = new AHDEntities();
            var Result = (from a in DbAHD.dataAHDPeriodEntitlement.Where(x => x.Active)

                          select new
                          {

                              Value = a.PeriodEntitlementGUID.ToString(),
                              Text = a.MonthName
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");


        }
        
        public SelectList StaffBankDestenationFlowStatus()
        {
            Guid _nvGUID = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7339");
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableGUID == _nvGUID && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");

        }
        public SelectList StaffOvertimeFlowStatus()
        {
            Guid _nvGUID = Guid.Parse("66CD375C-A576-4AA4-8AF4-FF3C1C5E7369");
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableGUID == _nvGUID && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }


        public SelectList AdminHRStaff()
        {
            AHDEntities DbAHD = new AHDEntities();
            Guid active = Guid.Parse("B9CD375C-A576-4AA4-8AF4-FF3C1C4E3611");
            var Result = (from a in DbAHD.v_StaffProfileInformation.Where(x => x.StaffStatusGUID == active)


                          select new
                          {
                              Value = a.UserGUID.ToString(),
                              Text = a.FullName
                          }).OrderBy(x => x.Text).ToList();

            //return Conditiontytpes;
            return new SelectList(Result, "Value", "Text");
        }

        public SelectList AdminHRAllStaff()
        {
            AHDEntities DbAHD = new AHDEntities();

            var Result = (from a in DbAHD.v_StaffProfileInformation


                          select new
                          {
                              Value = a.UserGUID.ToString(),
                              Text = a.FullName
                          }).OrderBy(x => x.Text).ToList();

            //return Conditiontytpes;
            return new SelectList(Result, "Value", "Text");
        }
        public SelectList StaffEntitlmentsPerferredPaymentMethod()
        {
            Guid myTable = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7579");
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.TableGUID == myTable && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList AHDActionFlowStatus()
        {
            Guid myTable = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7547");
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.TableGUID == myTable && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList AHDOvertivemWorkingDayType()
        {
            Guid myTable = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7835");
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.TableGUID == myTable && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription,
                              SortID = a.SortID
                          }).OrderBy(x => x.SortID).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList AHDOvertivemStaffStep()
        {
            Guid myTable = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7866");
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.TableGUID == myTable && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription,
                              SortID = a.SortID
                          }).OrderBy(x => x.SortID).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList AHDYears()
        {
            AHDEntities DbAHD = new AHDEntities();

            var Result = (from a in DbAHD.codeYear


                          select new
                          {
                              Value = a.Year.ToString(),
                              Text = a.Year
                          }).OrderBy(x => x.Text).ToList();

            //return Conditiontytpes;
            return new SelectList(Result, "Value", "Text");
        }
        public SelectList EntitlementFlowStatus()
        {
            Guid myTable = Guid.Parse("B9CD375C-A576-4AA4-8AF4-FF3C1C4E3676");
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.TableGUID == myTable && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList AHDMonths()
        {
            AHDEntities DbAHD = new AHDEntities();

            var Result = (from a in DbAHD.codeMonth


                          select new
                          {
                              Value = a.MonthCode.ToString(),
                              Text = a.MonthCode,
                              Id = a.MonthId
                          }).OrderBy(x => x.Id).ToList();

            //return Conditiontytpes;
            return new SelectList(Result, "Value", "Text");
        }
        public SelectList EntitlemenstCertifingUsers(Guid staffGUID)
        {

            Guid hrcertify = Guid.Parse("008478dc-7086-4261-a711-fcb0d3cf1787");
            var repAccess = DbCMS.userPermissions.Where(x => x.ActionGUID == hrcertify && x.Active == true).Select(x => x.UserProfileGUID).ToList();
            var userRepGUids = DbCMS.userProfiles.Where(x => repAccess.Contains(x.UserProfileGUID)).Select(x => x.userServiceHistory.UserGUID).ToList();



            var Result = (from a in DbCMS.userPersonalDetailsLanguage.Where(x => (x.UserGUID != staffGUID) && (x.UserGUID != UserGUID) && userRepGUids.Contains(x.UserGUID) && x.LanguageID == LAN && x.Active && (x.UserGUID != UserGUID))
                          orderby a.FirstName, a.Surname
                          select new
                          {
                              Value = a.UserGUID.ToString(),
                              Text = a.FirstName + " " + a.Surname
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList EntitlemenstAuthorizeFinanceUsers()
        {
            Guid hrcertify = Guid.Parse("b08322f1-5fcf-4f63-95b3-0c51f7ec8e85");
            var repAccess = DbCMS.userPermissions.Where(x => x.ActionGUID == hrcertify && x.Active == true).Select(x => x.UserProfileGUID).ToList();
            var userRepGUids = DbCMS.userProfiles.Where(x => repAccess.Contains(x.UserProfileGUID)).Select(x => x.userServiceHistory.UserGUID).ToList();



            var Result = (from a in DbCMS.userPersonalDetailsLanguage.Where(x => userRepGUids.Contains(x.UserGUID) && x.LanguageID == LAN && x.Active && (x.UserGUID != UserGUID))
                          orderby a.FirstName, a.Surname
                          select new
                          {
                              Value = a.UserGUID.ToString(),
                              Text = a.FirstName + " " + a.Surname
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList BLOMShuttleTraverls(Guid blomShuttleDelegationDateGUID)
        {
            AHDEntities DbAHD = new AHDEntities();

            var Result = (from a in DbAHD.dataBlomShuttleDelegationTraveler.Where(x => x.BlomShuttleDelegationDateGUID == blomShuttleDelegationDateGUID && x.Active)
                          join b in DbAHD.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.Active) on a.StaffGUID equals b.UserGUID
                          select new
                          {
                              Value = a.BlomShuttleDelegationTravelerGUID,
                              Text = b.FirstName + " " + b.Surname
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }


        public SelectList BLOMBankBranchNames()
        {
            Guid myTable = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7884");
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.TableGUID == myTable && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList StaffShuttleDelegationTypes()
        {
            Guid myTable = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7334");
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.TableGUID == myTable && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList NVInternationStaffFormTypes()
        {
            Guid _nvGUID = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c4e7999");
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableGUID == _nvGUID && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList InternationalStaffLeaveTypes()
        {
            Guid myTable = Guid.Parse("a1a0a314-388c-4e21-ab91-b919439aa797");
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.TableGUID == myTable && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList DangerPayStaffConfirmationStatus()
        {
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableGUID == LookupTables.DangerPayStaffStatus && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList StaffLeaveType()
        {
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableGUID == LookupTables.StaffLeaveType && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }

        #region AHD
        public SelectList AHDVehicles()
        {

            var Result = (from a in DbCMS.codeVehicle.Where(x => x.Active)

                          select new
                          {
                              Value = a.VehicleGUID.ToString(),
                              Text = a.VehicleNumber
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList VehicleRequestFormStatus()
        {

            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableGUID == LookupTables.AHDVehicleMaintenanceRequestFlowStatus && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList StaffRenewalResidencyFormStatus()
        {

            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableGUID == LookupTables.AHDRenewalStaffRenewalResidencyFormStatus && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }

        #endregion

        #endregion

        #region DAS
        #region Archive

        public SelectList ArchiveReportTopicRELC()
        {
            List<SelectListItem> Result = new List<SelectListItem>() {
                new SelectListItem { Value = "0", Text =  "User Documents Updates Summary" },
                new SelectListItem { Value = "1", Text =  "User Documents Updates Details" },
            };

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList DASSoftFieldListSource()
        {

            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableGUID == LookupTables.DASSoftFieldListSource && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList DASSDocumentSourceTypes()
        {
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableGUID == LookupTables.DASDocumentSourceTypes && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList DASDocumentAllCabinetsShelfs()
        {
            DASEntities DbDAS = new DASEntities();
            var Result = (from a in DbDAS.codeDASDocumentCabinetShelf.Where(x => x.Active)

                          select new
                          {
                              Value = a.DocumentCabinetShelfGUID.ToString(),
                              Text = a.ShelfNumber
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList DASDocumentTags(Guid PK1)
        {
            DASEntities DbDAS = new DASEntities();
            var Result = (from a in DbDAS.codeDASTemplateTypeDocumentTag.Where(x => x.Active && x.TemplateTypeDocumentGUID == PK1)

                          select new
                          {
                              Value = a.TemplateTypeDocumentTagGUID.ToString(),
                              Text = a.TagName
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList DASDocumentCabinets()
        {
            List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.RefugeeScannedDocument.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();

            DASEntities DbDAS = new DASEntities();
            var Result = (from a in DbDAS.codeDASDocumentCabinet.Where(x => x.Active).Where(x => AuthorizedList.Contains(x.OrganizationInstanceGUID + "," + x.DutyStationGUID))

                          select new
                          {
                              Value = a.DocumentCabinetGUID.ToString(),
                              Text = a.DocumentCabinetName
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList DASDocumentCabinetsShelfs(Guid PK1)
        {
            DASEntities DbDAS = new DASEntities();
            var Result = (from a in DbDAS.codeDASDocumentCabinetShelf.Where(x => x.Active && x.DocumentCabinetGUID == PK1)

                          select new
                          {
                              Value = a.DocumentCabinetShelfGUID.ToString(),
                              Text = a.ShelfNumber
                          }).Distinct().OrderBy(x=>x.Text).ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList DASTemplateType()
        {
            DASEntities DbDAS = new DASEntities();
            var Result = (from a in DbDAS.codeDASTemplateType.Where(x => x.Active)

                          select new
                          {
                              Value = a.TemplateTypeGUID.ToString(),
                              Text = a.TemplateName
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList DASTemplateDocumentType()
        {
            DASEntities DbDAS = new DASEntities();
            var Result = (from a in DbDAS.codeDASTemplateTypeDocument.Where(x => x.Active)

                          select new
                          {
                              Value = a.TemplateTypeDocumentGUID.ToString(),
                              Text = a.DocumentName
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList DASTemplateDocumentTypeByPK(Guid PK)
        {
            DASEntities DbDAS = new DASEntities();
            var Result = (from a in DbDAS.codeDASTemplateTypeDocument.Where(x => x.Active && x.TemplateTypeGUID==PK)

                          select new
                          {
                              Value = a.TemplateTypeDocumentGUID.ToString(),
                              Text = a.DocumentName
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList FileReferenceTypes(Guid? TemplateTypeGUID)
        {
            DASEntities DbDAS = new DASEntities();

            var _template = DbDAS.codeDASTemplateType.Where(x => x.TemplateTypeGUID == TemplateTypeGUID).FirstOrDefault();
            Guid PartyTypeGUID = TemplateTypeGUID ==null? DASTemplateOwnerTypes.Refugee:(Guid)_template.ReferenceLinkTypeGUID;

            if (PartyTypeGUID == DASTemplateOwnerTypes.Staff)
            {

                AHDEntities DbAHD = new AHDEntities();
                Guid active = Guid.Parse("B9CD375C-A576-4AA4-8AF4-FF3C1C4E3611");
                var Result = (from a in DbAHD.v_StaffProfileInformation.Where(x => x.StaffStatusGUID == active)


                              select new
                              {
                                  Value = a.UserGUID.ToString(),
                                  Text = a.FullName
                              }).OrderBy(x => x.Text).ToList();

                //return Conditiontytpes;
                return new SelectList(Result, "Value", "Text");
            }
            else if (PartyTypeGUID == DASTemplateOwnerTypes.Refugee)
            {
                var Result = (from a in DbDAS.dataFile.Where(x => x.Active)
                              select new
                              {
                                  Value = a.FileGUID.ToString(),
                                  Text = a.FileNumber
                              }).Take(10).Distinct().ToList();
                return new SelectList(Result, "Value", "Text");
            }

            return null;
        }
        public SelectList DASAdminHRStaff()
        {
            AHDEntities DbAHD = new AHDEntities();
            Guid active = Guid.Parse("B9CD375C-A576-4AA4-8AF4-FF3C1C4E3611");
            var Result = (from a in DbAHD.v_StaffProfileInformation.Where(x => x.StaffStatusGUID == active)


                          select new
                          {
                              Value = a.UserGUID.ToString(),
                              Text = a.FullName
                          }).OrderBy(x => x.Text).ToList();

            //return Conditiontytpes;
            return new SelectList(Result, "Value", "Text");
        }
        public SelectList DASDataRefugeeFileTop()
        {
            DASEntities DbDAS = new DASEntities();
            var Result = (from a in DbDAS.dataFile.Where(x => x.Active)

                          select new
                          {
                              Value = a.FileGUID.ToString(),
                              Text = a.FileNumber
                          }).Take(100).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList DASDataRefugeeFile()
        {
            DASEntities DbDAS = new DASEntities();
            var Result = (from a in DbDAS.dataFile.Where(x => x.Active)

                          select new
                          {
                              Value = a.FileGUID.ToString(),
                              Text = a.FileNumber
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList DASTemplateTargetType()
        {

            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableGUID == LookupTables.DASTemplateOwnerType && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList DASSoftFieldType()
        {

            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableGUID == LookupTables.DASSoftFieldType && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }

        #endregion
        public SelectList DASMovementFileStatus()
        {

            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableGUID == LookupTables.DASFileMovementStatus && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }


        public SelectList DASFileTransferLocation()
        {
            DASEntities DbDAS = new DASEntities();
            var Result = (from a in DbDAS.codeDASTransferLocation.Where(x => x.Active)

                          select new
                          {
                              Value = a.TransferLocationGUID.ToString(),
                              Text = a.TransferLocationName
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList DASproGresSites()
        {
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableGUID == LookupTables.DASproGresSiteOwner && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList DASUsersTransferAuthorized()
        {
            DASEntities DbDAS = new DASEntities();
            var userGUIDs = DbDAS.codeDASDestinationUnitFocalPoint.Select(x => x.UserGUID).Distinct().ToList();
            var Result = (from a in DbCMS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.Active == true && x.Active && (x.UserGUID != UserGUID)
                          && userGUIDs.Contains(x.UserGUID))
                          orderby a.FirstName, a.Surname
                          select new
                          {
                              Value = a.UserGUID.ToString(),
                              Text = a.FirstName + " " + a.Surname
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList DASFileMovementStatus()
        {
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableGUID == LookupTables.DASFileMovementStatus && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }


        public SelectList DASFileSiteOwners()
        {

            Guid site = Guid.Parse("B9CD375C-A576-4AA4-8AF4-FF3C1C4E3133");
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableGUID == site && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }


        public SelectList DASDASDestinationUnit()
        {
            DASEntities DbDAS = new DASEntities();
            var Result = (from a in DbDAS.codeDASDestinationUnit

                          select new
                          {
                              Value = a.DestinationUnitGUID.ToString(),
                              Text = a.DestinationUnitName
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList DASDocumentCustodianType()
        {
            var Result = (from a in DbCMS.codeTables.Where(x => x.TableGUID == LookupTables.DASDocumentCustodianType && x.Active)
                          join b in DbCMS.codeTablesValues.Where(x => x.Active) on a.TableGUID equals b.TableGUID
                          join c in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on b.ValueGUID equals c.ValueGUID
                          orderby b.SortID
                          select new
                          {
                              Value = b.ValueGUID.ToString(),
                              Text = c.ValueDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");


        }
        #endregion

        #region TTT
        public SelectList TenderTypes()
        {
            var Result = (from a in DbCMS.codeTables.Where(x => x.TableGUID == LookupTables.TenderTypes && x.Active)
                          join b in DbCMS.codeTablesValues.Where(x => x.Active) on a.TableGUID equals b.TableGUID
                          join c in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on b.ValueGUID equals c.ValueGUID
                          orderby a.SortID
                          select new
                          {
                              Value = b.ValueGUID.ToString(),
                              Text = c.ValueDescription
                          }).ToList();
            return new SelectList(Result, "Value", "Text");
        }

        public SelectList TenderBudgetSources()
        {
            var Result = (from a in DbCMS.codeTables.Where(x => x.TableGUID == LookupTables.TenderBudgetSources && x.Active)
                          join b in DbCMS.codeTablesValues.Where(x => x.Active) on a.TableGUID equals b.TableGUID
                          join c in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on b.ValueGUID equals c.ValueGUID
                          orderby a.SortID
                          select new
                          {
                              Value = b.ValueGUID.ToString(),
                              Text = c.ValueDescription
                          }).ToList();
            return new SelectList(Result, "Value", "Text");
        }

        public SelectList TenderIdentities()
        {
            var Result = (from a in DbCMS.codeTables.Where(x => x.TableGUID == LookupTables.TenderIdentity && x.Active)
                          join b in DbCMS.codeTablesValues.Where(x => x.Active) on a.TableGUID equals b.TableGUID
                          join c in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on b.ValueGUID equals c.ValueGUID
                          orderby a.SortID
                          select new
                          {
                              Value = b.ValueGUID.ToString(),
                              Text = c.ValueDescription
                          }).ToList();
            return new SelectList(Result, "Value", "Text");
        }

        public SelectList TenderEndorsmentRequired()
        {
            var Result = (from a in DbCMS.codeTables.Where(x => x.TableGUID == LookupTables.TenderEndorsmentRequired && x.Active)
                          join b in DbCMS.codeTablesValues.Where(x => x.Active) on a.TableGUID equals b.TableGUID
                          join c in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on b.ValueGUID equals c.ValueGUID
                          orderby a.SortID
                          select new
                          {
                              Value = b.ValueGUID.ToString(),
                              Text = c.ValueDescription
                          }).ToList();
            return new SelectList(Result, "Value", "Text");
        }

        public SelectList TenderHofoSupplyRep()
        {
            var Result = (from a in DbCMS.codeTables.Where(x => x.TableGUID == LookupTables.TenderHoFoSupplyRep && x.Active)
                          join b in DbCMS.codeTablesValues.Where(x => x.Active) on a.TableGUID equals b.TableGUID
                          join c in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on b.ValueGUID equals c.ValueGUID
                          orderby a.SortID
                          select new
                          {
                              Value = b.ValueGUID.ToString(),
                              Text = c.ValueDescription
                          }).ToList();
            return new SelectList(Result, "Value", "Text");
        }

        public SelectList TenderAwardedCompany()
        {
            var Result = (from a in DbCMS.codeTables.Where(x => x.TableGUID == LookupTables.TenderAwardedCompanies && x.Active)
                          join b in DbCMS.codeTablesValues.Where(x => x.Active) on a.TableGUID equals b.TableGUID
                          join c in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on b.ValueGUID equals c.ValueGUID
                          orderby a.SortID
                          select new SelectListItem
                          {
                              Value = b.ValueGUID.ToString(),
                              Text = c.ValueDescription,
                          }).ToList();
            return new SelectList(Result, "Value", "Text");
        }

        public SelectList TenderAmountCurrency()
        {
            //comment
            List<SelectListItem> Result = new List<SelectListItem>
            {
                new SelectListItem { Value = "SYR", Text = "SYR" },
                new SelectListItem { Value = "USD", Text = "USD" },
                new SelectListItem { Value = "EUR", Text = "EUR" },
            };
            return new SelectList(Result, "Value", "Text");
        }

        public SelectList TenderFA1()
        {
            var Result = (from a in DbCMS.codeTables.Where(x => x.TableGUID == LookupTables.TenderFA1 && x.Active)
                          join b in DbCMS.codeTablesValues.Where(x => x.Active) on a.TableGUID equals b.TableGUID
                          join c in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on b.ValueGUID equals c.ValueGUID
                          orderby a.SortID ascending
                          select new
                          {
                              Value = b.ValueGUID.ToString(),
                              Text = c.ValueDescription
                          }).ToList();
            return new SelectList(Result, "Value", "Text");
        }

        public SelectList TenderBoxNumbers()
        {
            List<SelectListItem> Result = new List<SelectListItem>();
            for (int i = 1; i <= 13; i++)
            {
                Result.Add(new SelectListItem { Value = i.ToString(), Text = i.ToString() });
            }
            Result.Add(new SelectListItem { Value = "Without Tender Box", Text = "Without Tender Box" });
            return new SelectList(Result, "Value", "Text");

        }

        public SelectList UsersForTender(bool IncludeMe)
        {
            Guid supplyDepartment = Guid.Parse("1EEC1267-0E8F-4414-9699-B664DEE21C79");
            Guid staffIsActiveGUID = Guid.Parse("B9CD375C-A576-4AA4-8AF4-FF3C1C4E3611");
            var Result = (from a in DbCMS.StaffCoreData.Where(x => x.Active)
                          join b in DbCMS.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.UserGUID equals b.UserGUID
                          where (a.DepartmentGUID == supplyDepartment || a.EmailAddress.ToLower() == "karkoush@unhcr.org")
                          //&& a.StaffStatusGUID == staffIsActiveGUID
                          select new
                          {
                              Value = a.UserGUID.ToString(),
                              Text = b.FirstName + " " + b.Surname
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList TenderStatuses()
        {
            var Result = (from a in DbCMS.codeTables.Where(x => x.TableGUID == LookupTables.TenderStatuses && x.Active)
                          join b in DbCMS.codeTablesValues.Where(x => x.Active) on a.TableGUID equals b.TableGUID
                          join c in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on b.ValueGUID equals c.ValueGUID
                          orderby a.SortID ascending
                          select new
                          {
                              Value = b.ValueGUID.ToString(),
                              Text = c.ValueDescription
                          }).ToList();
            return new SelectList(Result, "Value", "Text");
        }

        public SelectList TenderResolutions()
        {
            var Result = (from a in DbCMS.codeTables.Where(x => x.TableGUID == LookupTables.TenderResolutions && x.Active)
                          join b in DbCMS.codeTablesValues.Where(x => x.Active) on a.TableGUID equals b.TableGUID
                          join c in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on b.ValueGUID equals c.ValueGUID
                          orderby a.SortID ascending
                          select new
                          {
                              Value = b.ValueGUID.ToString(),
                              Text = c.ValueDescription
                          }).ToList();
            return new SelectList(Result, "Value", "Text");
        }

        public SelectList DoNotifyBOC()
        {
            List<SelectListItem> Result = new List<SelectListItem>();
            Result.Add(new SelectListItem { Value = "true", Text = "Yes" });
            Result.Add(new SelectListItem { Value = "false", Text = "No" });
            return new SelectList(Result, "Value", "Text");
        }
        #endregion

        #region ORG
        public SelectList StaffEligbilityPaymentStatus()
        {
            Guid myTable = Guid.Parse("2dac5d96-e6a3-48c1-b3f5-17bfd9f62881");
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.TableGUID == myTable && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList StaffPositionTypes()
        {

            Guid myTable = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c4e7444");
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.TableGUID == myTable && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();


            return new SelectList(Result, "Value", "Text");
        }

        public SelectList ProfileStaffFeedbackFlowStatus(int? flowStep)
        {

            Guid myTable = Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3977");
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.TableGUID == myTable && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active &&
                          (flowStep == 4 ? (x.codeTablesValues.SortID == 2) : (x.codeTablesValues.SortID == 3))
                          ) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList ProfileStaffFeedbackTypes(int? type)
        {
            Guid myTable = Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3555");
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.TableGUID == myTable && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active && (type == 1 ? x.codeTablesValues.SortID == 2 : true)) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList StaffServiceProvidedTypes()
        {
            Guid myTable = Guid.Parse("55962180-3634-44a4-874c-db7c3481d66a");
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.TableGUID == myTable && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList StaffServiceProvidedFlowStatus()
        {
            Guid myTable = Guid.Parse("55962180-3634-44a4-874c-db7c3481d888");
            Guid pending = Guid.Parse("55962180-3634-44a4-874c-db7c3481d888");
            Guid _confimed = Guid.Parse("55962180-3634-44a4-874c-db7c3481d882");
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.TableGUID == myTable && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList StaffRelativeType()
        {
            Guid myTable = Guid.Parse("946a4dda-1745-46f9-8e56-4db1374c46e4");
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.TableGUID == myTable && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList StaffOnlineTrainingType()
        {
            Guid myTable = Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c4e7479");
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.TableGUID == myTable && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList StaffRecuitmentTypes()
        {
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableGUID == LookupTables.StaffRecruitmentType && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList StaffContractTypes()
        {
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableGUID == LookupTables.StaffContractTypes && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList StaffGrades()
        {
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableGUID == LookupTables.StaffGradeTypes && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList BanksNames()
        {
            AHDEntities DbAHD = new AHDEntities();
            var Result = (from a in DbAHD.codeBank.Where(x => x.Active)
                          join b in DbAHD.codeBankLanguage.Where(x => x.LanguageID == LAN && x.Active) on a.BankGUID equals b.BankGUID
                          select new
                          {
                              Value = a.BankGUID.ToString(),
                              Text = b.BankDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }

        #endregion

        #region IDC
        public SelectList IDCardIssueReason()
        {
            var Result = (from a in DbCMS.codeCardIssueReason.Where(x => x.Active)
                          orderby a.Seq
                          select new
                          {
                              Value = a.IssueCode,
                              Text = a.IssueDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList IDCardSyriaDutyStations()
        {
            //Access is authorized by Access Action
            List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.CardIssued.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();

            Guid syriaCountryGUID = Guid.Parse("710BFD1B-50CC-4F1E-92A9-A70583CFA5E0");
            var Result = (from a in DbCMS.codeDutyStations.Where(x => x.Active && x.CountryGUID == syriaCountryGUID && AuthorizedList.Contains("e156c022-ec72-4a5a-be09-163bd85c68ef," + x.DutyStationGUID.ToString()))
                          join b in DbCMS.codeDutyStationsLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.DutyStationGUID equals b.DutyStationGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()
                          orderby R1.DutyStationDescription
                          select new SelectListDutyStation
                          {
                              Value = a.DutyStationGUID.ToString(),
                              Text = R1.DutyStationDescription,
                          }).ToList();

            SelectList sl = new SelectList(Result, "Value", "Text");
            return sl;
        }

        public SelectList CategoryDetermination()
        {
            List<SelectListItem> Result = new List<SelectListItem>() {
                new SelectListItem { Value = "REF", Text = LAN=="EN"? "Refugee":"لاجىء" },
                new SelectListItem { Value = "ASR", Text = LAN=="EN"?"Asylum Seeker":"طالب لجوء" },
                };
            return new SelectList(Result, "Value", "Text");
        }

        #endregion

        #region AHD
        public SelectList AHDEntitlementTypes()
        {


            var Result = (from a in DbCMS.codeAHDEntitlementType.Where(x => x.Active)


                          select new
                          {
                              Value = a.EntitlementTypeGUID.ToString(),
                              Text = a.EntitlementTypeName
                          }).OrderBy(x => x.Text).ToList();

            //return Conditiontytpes;
            return new SelectList(Result, "Value", "Text");
        }
        public SelectList AHDInternationalAttendanceNotRRALTypes()
        {


            var Result = (from a in DbCMS.codeAHDInternationalStaffAttendanceType.Where(x => x.Active &&
                          (x.InternationalStaffAttendanceTypeGUID != coddeInternationalStaffAttendanceTypeAttendanceTable.RR
                          ) && (x.InternationalStaffAttendanceTypeGUID != coddeInternationalStaffAttendanceTypeAttendanceTable.AnnualLeave)
                          && x.InternationalStaffAttendanceTypeGUID != coddeInternationalStaffAttendanceTypeAttendanceTable.TravelTime
                          )


                          select new
                          {
                              Value = a.InternationalStaffAttendanceTypeGUID.ToString(),
                              Text = a.AttendanceTypeName
                          }).OrderBy(x => x.Text).ToList();

            //return Conditiontytpes;
            return new SelectList(Result, "Value", "Text");
        }
        public SelectList AHDInternationalAttendanceTypes()
        {


            var Result = (from a in DbCMS.codeAHDInternationalStaffAttendanceType.Where(x => x.Active
                          //&& (x.InternationalStaffAttendanceTypeGUID != coddeInternationalStaffAttendanceTypeAttendanceTable.Weekend
                          //) && (x.InternationalStaffAttendanceTypeGUID != coddeInternationalStaffAttendanceTypeAttendanceTable.OfficialHoliday
                          //)
                          )


                          select new
                          {
                              Value = a.InternationalStaffAttendanceTypeGUID.ToString(),
                              Text = a.AttendanceTypeName
                          }).OrderBy(x => x.Text).ToList();

            //return Conditiontytpes;
            return new SelectList(Result, "Value", "Text");
        }
        public SelectList InternationalStaffForMissionsApprovalUsers(Guid StaffGUID)
        {
            AHDEntities DbAHD = new AHDEntities();
            Guid internationalGUID = Guid.Parse("FC4B2E79-2B97-4252-A50B-915B07A12310");
            Guid _activeStatus = Guid.Parse("B9CD375C-A576-4AA4-8AF4-FF3C1C4E3611");


            var Result = (from a in DbAHD.v_StaffProfileInformation.Where(x => (x.UserGUID != UserGUID)
                          && x.RecruitmentTypeGUID == internationalGUID
                          && x.StaffStatusGUID == _activeStatus
                          && x.UserGUID != StaffGUID)
                          orderby a.FullName
                          select new
                          {
                              Value = a.UserGUID.ToString(),
                              Text = a.FullName
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList InternationalStaffUsers(bool IncludeMe)
        {
            AHDEntities DbAHD = new AHDEntities();
            Guid internationalGUID = Guid.Parse("FC4B2E79-2B97-4252-A50B-915B07A12310");
            Guid _activeStatus = Guid.Parse("B9CD375C-A576-4AA4-8AF4-FF3C1C4E3611");
            // var staffCoreGUID = DbCMS.StaffCoreData.Where(x => x.RecruitmentTypeGUID == internationalGUID && x.Active && x.StaffStatusGUID== _activeStatus) .Select(x => x.UserGUID).ToList();
            var Result = (from a in DbAHD.v_StaffProfileInformation.Where(x =>
                  x.RecruitmentTypeGUID == internationalGUID
                          && x.StaffStatusGUID == _activeStatus
                          )
                          orderby a.FullName
                          select new
                          {
                              Value = a.UserGUID.ToString(),
                              Text = a.FullName
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList HRRepresentiaveAcessStaffUsers(bool IncludeMe)
        {
            Guid repGUID = Guid.Parse("B3BDF367-3F74-45E4-AF0A-1AEE013C0E81");
            var repAccess = DbCMS.userPermissions.Where(x => x.ActionGUID == repGUID && x.Active == true).Select(x => x.UserProfileGUID).ToList();
            var userRepGUids = DbCMS.userProfiles.Where(x => repAccess.Contains(x.UserProfileGUID)).Select(x => x.userServiceHistory.UserGUID).ToList();



            var Result = (from a in DbCMS.userPersonalDetailsLanguage.Where(x => userRepGUids.Contains(x.UserGUID) && x.LanguageID == LAN && x.Active && (IncludeMe || x.UserGUID != UserGUID))
                          orderby a.FirstName, a.Surname
                          select new
                          {
                              Value = a.UserGUID.ToString(),
                              Text = a.FirstName + " " + a.Surname
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList InternationalStaffLeaveFlowStatus()
        {
            Guid myTable = Guid.Parse("B9CD375C-A576-4AA4-8AF4-FF3C1C4E3632");
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.TableGUID == myTable && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }




        #endregion

        #region DAS
        public SelectList DASScanningTypes()
        {
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableGUID == LookupTables.DASScanningType && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList DASPaperSize()
        {
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableGUID == LookupTables.DASPaperSize && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList DASPaperFormat()
        {
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableGUID == LookupTables.DASPaperFormat && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList DASColorMode()
        {
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableGUID == LookupTables.DASColorMod && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList DASResolution()
        {
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableGUID == LookupTables.DASResolution && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }

        #endregion

        #region SHM

        public SelectList LebanonPhones()
        {
            //Syria
            if (OrganizationInstanceGUID.ToString().ToUpper() == "E156C022-EC72-4A5A-BE09-163BD85C68EF")
            {
                List<SelectListItem> Result = new List<SelectListItem>() {
                new SelectListItem { Value = "961 76 500 269", Text = "961 76 500 269" },
                new SelectListItem { Value = "961 76 320 979", Text = "961 76 320 979" },
                new SelectListItem { Value = "961 81 314 307", Text = "961 81 314 307" },
                new SelectListItem { Value = "962 799 676 538", Text = "962 799 676 538" },
                new SelectListItem { Value = "962 798 365 543", Text = "962 798 365 543" },
            };
                return new SelectList(Result, "Value", "Text");
            }
            else
            {
                List<SelectListItem> Result = new List<SelectListItem>()
                {
                     new SelectListItem { Value = "", Text = "" },
                };
                return new SelectList(Result, "Value", "Text");
            }
           
        }
        public SelectList ShuttleTravelPurpose()
        {
            var Result = (from a in DbCMS.codeShuttleTravelPurpose.Where(x => x.Active)
                          join b in DbCMS.codeShuttleTravelPurposeLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.ShuttleTravelPurposeGUID equals b.ShuttleTravelPurposeGUID
                          orderby a.Priority
                          select new
                          {
                              Value = a.ShuttleTravelPurposeGUID,
                              Text = b.ShuttleTravelPurposeDescription
                          }).ToList();
            return new SelectList(Result, "Value", "Text");
        }

        public SelectList CountriesSyriaShuttle()
        {
            //Syria
            if(OrganizationInstanceGUID.ToString().ToUpper()== "E156C022-EC72-4A5A-BE09-163BD85C68EF")
            {
                List<string> Countries = new List<string>() {
                "710BFD1B-50CC-4F1E-92A9-A70583CFA5E0",
                "205D00BD-E143-4611-BBBB-C7FB87E4EC84",
                "14CF2195-F451-42E8-8F3D-06BFB3ED080B",
                "A42AA954-2A8A-48A9-BC96-02E18006E04A",
                "C5EB9AF3-428E-4F53-94EA-EA85680293EB",
                "49d6de79-5c8d-4ca3-a967-7ed75f14428a",
                "4f9ced04-27ac-4a99-a34e-2143791c9571",
                "AF6CF68C-D190-4EB4-A1B8-69E6FA02CB81"};
                var Result = (from a in DbCMS.codeCountriesConfigurations.Where(x => x.OrganizationInstanceGUID == OrganizationInstanceGUID && x.Active)
                              join b in DbCMS.codeCountriesLanguages.Where(x => x.LanguageID == LAN && x.Active && Countries.Contains(x.CountryGUID.ToString())) on a.CountryGUID equals b.CountryGUID
                              orderby b.CountryDescription
                              select new
                              {
                                  Value = a.CountryGUID.ToString(),
                                  Text = b.CountryDescription
                              }).ToList();
                return new SelectList(Result, "Value", "Text");
            }
            else
            {
                var Result = (from a in DbCMS.codeCountriesConfigurations.Where(x => x.OrganizationInstanceGUID == OrganizationInstanceGUID && x.Active)
                              join b in DbCMS.codeCountriesLanguages.Where(x => x.LanguageID == LAN && x.Active ) on a.CountryGUID equals b.CountryGUID
                              orderby b.CountryDescription
                              select new
                              {
                                  Value = a.CountryGUID.ToString(),
                                  Text = b.CountryDescription
                              }).ToList();
                return new SelectList(Result, "Value", "Text");
            }
           

            //return Countries;
            
        }

        public SelectList ShuttlePassanger()
        {
            var Result = (from a in DbCMS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.Active)
                          select new
                          {
                              Value = a.UserGUID.ToString(),
                              Text = a.FirstName + " " + a.Surname
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList ShuttleDrivers()
        {
            using (var DbSHM = new SHM_DAL.Model.SHMEntities())
            {
                string[] DriverJobTitleGUIDs = { "7FFA5C6F-FA42-4DC4-A072-2BD5C4ED53C8", "1994A889-1F22-43C0-BF40-759143CFF286" };
                var Result = (from a in DbSHM.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.Active)
                              join b in DbSHM.StaffCoreData.Where(x => DriverJobTitleGUIDs.Contains(x.JobTitleGUID.ToString().ToUpper()) && x.Active && x.OrganizationInstanceGUID==OrganizationInstanceGUID) on a.UserGUID equals b.UserGUID
                              select new
                              {
                                  Value = a.UserGUID.ToString(),
                                  Text = a.FirstName + " " + a.Surname
                              }).ToList();

                return new SelectList(Result, "Value", "Text");
            }
        }
        public SelectList Vehicles()
        {
            var Result = (from a in DbCMS.codeVehicle.Where(x => x.Active && x.OrganizationInstanceGUID== OrganizationInstanceGUID)
                          select new
                          {
                              Value = a.VehicleGUID,
                              Text = a.VehicleNumber
                          }).ToList();
            return new SelectList(Result, "Value", "Text");
        }

        public SelectList LocationsByCountries(Guid[] CountryGUIDs, Guid LocationTypeGUID)
        {

            var Result = (from a in DbCMS.codeLocations.Where(x => CountryGUIDs.Contains(x.CountryGUID) && x.Active && x.LocationTypeGUID == LocationTypeGUID)
                          join b in DbCMS.codeLocationsLanguages.Where(l => l.LanguageID == LAN && l.Active) on a.LocationGUID equals b.LocationGUID
                          select new
                          {
                              Value = a.LocationGUID.ToString(),
                              Text = b.LocationDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");

        }


        public SelectList ShuttleReportTopic()
        {
            List<SelectListItem> Result = new List<SelectListItem>() {
                new SelectListItem { Value = "1", Text = "Dashboard Summary " ,Selected =true},
            };
            return new SelectList(Result, "Value", "Text");
        }
        public SelectList ShuttleReportTopicRDLC()
        {
            List<SelectListItem> Result = new List<SelectListItem>() {
                new SelectListItem { Value = "0", Text = "Shuttle Info" ,Selected =true},
                new SelectListItem { Value = "1", Text = "Memo UN S.M" ,Selected =true},
            };
            return new SelectList(Result, "Value", "Text");
        }
        #endregion

        #region AMS
        public SelectList SchedulerReports()
        {
            List<SelectListItem> Result = new List<SelectListItem> {
                new SelectListItem { Value = "1", Text = "Appointment Over View" } };

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList AppointmentTypeCalender(Guid AppointmentTypeGUID)
        {
            if (AppointmentTypeGUID == Guid.Empty) return new DropDownList().Empty();
            var department = DbCMS.codeAppointmentType.Where(x => x.AppointmentTypeGUID == AppointmentTypeGUID).FirstOrDefault();
            var userProfiles = DbCMS.userProfiles.Where(x => x.UserProfileGUID == UserProfileGUID).FirstOrDefault();

            AMSEntities DbAMS = new AMSEntities();

            var Result = (from a in DbAMS.dataAppointmentTypeCalendar.Where(x => x.Active &&
                        x.codeAppointmentType.DepartmentGUID == department.DepartmentGUID
                        && x.OrganizationInstanceGUID == userProfiles.OrganizationInstanceGUID
                        && x.DutyStationGUID == userProfiles.DutyStationGUID
                        && x.EventStartDate > DateTime.Now
                        && x.AppointmentTypeGUID == AppointmentTypeGUID)
                          orderby a.EventStartDate
                          select new
                          {
                              Value = a.AppointmentTypeCalenderGUID.ToString(),
                              Text = a.PreventAppointments ? "Appointments was Prevented on " + a.EventStartDate : "Slots Available: " + (a.SlotAvailable - a.SlotClosed).ToString() + ", [" + a.EventStartDate + " , " + a.EventEndDate + "]"
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList AppointmentTypeCalender(Guid AppointmentTypeGUID, Guid AppointmentGUID)
        {
            if (AppointmentTypeGUID == Guid.Empty) return new DropDownList().Empty();
            var department = DbCMS.codeAppointmentType.Where(x => x.AppointmentTypeGUID == AppointmentTypeGUID).FirstOrDefault();
            var userProfiles = DbCMS.userProfiles.Where(x => x.UserProfileGUID == UserProfileGUID).FirstOrDefault();

            AMSEntities DbAMS = new AMSEntities();

            var Result = (from a in DbAMS.dataAppointmentTypeCalendar.Where(x => x.Active &&
                        x.codeAppointmentType.DepartmentGUID == department.DepartmentGUID
                        && x.OrganizationInstanceGUID == userProfiles.OrganizationInstanceGUID
                        && x.DutyStationGUID == userProfiles.DutyStationGUID
                        && x.EventStartDate > DateTime.Now)
                          orderby a.EventStartDate
                          select new
                          {
                              Value = a.AppointmentTypeCalenderGUID.ToString(),
                              Text = a.PreventAppointments ? "Appointments was Prevented on " + a.EventStartDate : "Slots Available: " + (a.SlotAvailable - a.SlotClosed).ToString() + ", [" + a.EventStartDate + " , " + a.EventEndDate + "]"
                          }).ToList();
            if (AppointmentTypeGUID != Guid.Empty)
            {
                var SlotSelected = (from a in DbAMS.dataAppointmentTypeCalendar
                                    join b in DbAMS.dataAppointmentCalendarHolded.Where(x => x.AppointmentGUID == AppointmentGUID) on a.AppointmentTypeCalenderGUID equals b.AppointmentTypeCalenderGUID into LJ1
                                    from R1 in LJ1.DefaultIfEmpty()
                                    select new
                                    {
                                        Value = a.AppointmentTypeCalenderGUID.ToString(),
                                        Text = a.PreventAppointments ? "Appointments was Prevented on " + a.EventStartDate : "Slots Available: " + (a.SlotAvailable - a.SlotClosed).ToString() + ", [" + a.EventStartDate + " , " + a.EventEndDate + "]"
                                    }).FirstOrDefault();
                Result.Add(SlotSelected);
            }
            return new SelectList(Result, "Value", "Text");
        }

        public SelectList AppointmentTypes()
        {
            List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.AppointmentType.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();
            var Result = (from a in DbCMS.codeAppointmentTypeLanguage.Where(x => x.LanguageID == LAN && x.Active).Where(x => x.LanguageID == LAN && x.Active).Where(x => AuthorizedList.Contains(x.codeAppointmentType.DepartmentGUID.ToString()))
                          join b in DbCMS.codeDepartments.Where(x => x.Active) on a.codeAppointmentType.DepartmentGUID equals b.DepartmentGUID
                          orderby a.AppointmentTypeDescription
                          select new
                          {
                              Value = a.AppointmentTypeGUID.ToString(),
                              Text = "[" + b.DepartmentCode + "] " + a.AppointmentTypeDescription
                          }).OrderBy(x => x.Text).ToList();

            //return Conditiontytpes;
            return new SelectList(Result, "Value", "Text");
        }

        public SelectList AppointmentTypesByDepartment(Guid DepartmentGUID)
        {
            List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.AppointmentType.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();
            var Result = (from a in DbCMS.codeAppointmentTypeLanguage.Where(x => x.LanguageID == LAN && x.Active).Where(x => x.LanguageID == LAN && x.Active).Where(x => AuthorizedList.Contains(x.codeAppointmentType.DepartmentGUID.ToString()))
                          join b in DbCMS.codeDepartments.Where(x => x.Active && x.DepartmentGUID == DepartmentGUID) on a.codeAppointmentType.DepartmentGUID equals b.DepartmentGUID
                          orderby a.AppointmentTypeDescription
                          select new
                          {
                              Value = a.AppointmentTypeGUID.ToString(),
                              Text = "[" + b.DepartmentCode + "] " + a.AppointmentTypeDescription
                          }).OrderBy(x => x.Text).ToList();

            //return Conditiontytpes;
            return new SelectList(Result, "Value", "Text");
        }

        public SelectList AppointmentType(Guid? AppointmentTypeGUID)
        {
            if (AppointmentTypeGUID == null) return new DropDownList().Empty();
            var Result = (from a in DbCMS.codeAppointmentTypeLanguage.Where(x => x.LanguageID == LAN && x.Active).Where(x => x.LanguageID == LAN && x.Active).Where(x => x.AppointmentTypeGUID == AppointmentTypeGUID)
                          join b in DbCMS.codeDepartments.Where(x => x.Active) on a.codeAppointmentType.DepartmentGUID equals b.DepartmentGUID
                          orderby a.AppointmentTypeDescription
                          select new
                          {
                              Value = a.AppointmentTypeGUID.ToString(),
                              Text = "[" + b.DepartmentCode + "] " + a.AppointmentTypeDescription
                          }).OrderBy(x => x.Text).ToList();

            //return Conditiontytpes;
            return new SelectList(Result, "Value", "Text");
        }

        public SelectList AppointmentReportTopicRELC()
        {
            List<SelectListItem> Result = new List<SelectListItem>() {
                new SelectListItem { Value = "0", Text =  "Appointment By Date" },
                new SelectListItem { Value = "1", Text =  "Appointment By Execution Time" },
                new SelectListItem { Value = "2", Text =  "Appointment Overview" },
                new SelectListItem { Value = "3", Text =  "Appointments Slot" },
            };

            return new SelectList(Result, "Value", "Text");
        }
        #endregion

        #region ORG
        public SelectList StaffSalaryInAdvanceStatus()
        {
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableGUID == LookupTables.StaffSalaryInAdvanceStatus && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList StaffConfirmationStatus()
        {

            Guid myTable = Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3747");
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.TableGUID == myTable && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();


            return new SelectList(Result, "Value", "Text");
        }
        public SelectList PhoneHolderTypes(int type)
        {
            Guid staffTypeGUID = Guid.Parse("20aab4c0-a6f8-4f87-9316-94d1d5f5bd8f");
            AHDEntities DbAHD = new AHDEntities();
            if (type == 0)
            {
                var Result = (from a in DbAHD.codeAHDPhoneHolderType.Where(x => x.PhoneHolderTypeGUID != staffTypeGUID && x.Active)

                              select new
                              {
                                  Value = a.PhoneHolderTypeGUID.ToString(),
                                  Text = a.Name
                              }).Distinct().ToList();

                return new SelectList(Result, "Value", "Text");
            }
            else
            {
                var Result = (from a in DbAHD.codeAHDPhoneHolderType.Where(x => x.Active)

                              select new
                              {
                                  Value = a.PhoneHolderTypeGUID.ToString(),
                                  Text = a.Name
                              }).Distinct().ToList();

                return new SelectList(Result, "Value", "Text");
            }
        }

        public SelectList StaffDocumentTypes()
        {
            Guid myTable = Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3733");
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.TableGUID == myTable && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList RRLeavePlanStatus()
        {
            Guid myTable = Guid.Parse("71f78782-d09c-4535-9366-cf803057cb08");
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.TableGUID == myTable && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList RRLeaveDocumentTypes()
        {
            Guid myTable = Guid.Parse("2d0b59f3-347b-4fa1-8793-a7741d4c35bd");
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.TableGUID == myTable && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList StaffOfficeTypes()
        {
            Guid myTable = Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c332244");
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.TableGUID == myTable && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList SyriaStaffDutyStation()
        {
            Guid syriaCountryGUID = Guid.Parse("710BFD1B-50CC-4F1E-92A9-A70583CFA5E0");
            var Result = (from a in DbCMS.codeDutyStations.Where(x => x.Active && x.CountryGUID == syriaCountryGUID && x.IsDutyStation == true)
                          join b in DbCMS.codeDutyStationsLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.DutyStationGUID equals b.DutyStationGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()
                          orderby R1.DutyStationDescription
                          select new SelectListDutyStation
                          {
                              Value = a.DutyStationGUID.ToString(),
                              Text = R1.DutyStationDescription,

                          }).ToList();
            return new SelectList(Result, "Value", "Text");
        }

        public SelectList StaffPrefix()
        {
            Guid myTable = Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3777");
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.TableGUID == myTable && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList StaffDepartments()
        {
            var Result = (from a in DbCMS.codeDepartmentsLanguages.Where(x => x.Active && x.LanguageID == LAN)


                          select new
                          {
                              Value = a.DepartmentGUID.ToString(),
                              Text = a.DepartmentDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList StaffStatus()
        {
            Guid myTable = Guid.Parse("b9cd375c-a576-4aa4-8af4-ff3c1c4e3611");
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.TableGUID == myTable && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }




        #endregion

        #region 4WS 
        public SelectList FWSHubs()
        {
            var Result = (from a in DbCMS.code4WSHub.Where(x => x.Active)
                          select new
                          {
                              Value = a.HubGUID,
                              Text = a.HubDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList FWSPartners()
        {
            var Result = (from a in DbCMS.code4WSPartner.Where(x => x.Active)
                          select new
                          {
                              Value = a.PartnerGUID,
                              Text = a.PartnerDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList FWSLocationLong(string selectedValue = null)
        {
            var Result = (from a in DbCMS.code4WSLocation.Where(x => x.Active)
                          select new
                          {
                              Value = a.LocationGUID,
                              Text = a.Location
                          }).ToList();

            return new SelectList(Result, "Value", "Text", selectedValue);
        }

        public SelectList FWSReportingMonth()
        {
            List<SelectListItem> Result = new List<SelectListItem>() {
                new SelectListItem { Value = "1", Text = "01 - January" },
                new SelectListItem { Value = "2", Text = "02 - February" },
                new SelectListItem { Value = "3", Text = "03 - March" },
                new SelectListItem { Value = "4", Text = "04 - April" },
                new SelectListItem { Value = "5", Text = "05 - May" },
                new SelectListItem { Value = "6", Text = "06 - June" },
                new SelectListItem { Value = "7", Text = "07 - July" },
                new SelectListItem { Value = "8", Text = "08 - August" },
                new SelectListItem { Value = "9", Text = "09 - September" },
                new SelectListItem { Value = "10", Text = "10 - October" },
                new SelectListItem { Value = "11", Text = "11 - November" },
                new SelectListItem { Value = "12", Text = "12 - December" },
            };
            return new SelectList(Result, "Value", "Text");
        }

        public SelectList FWSHRPProject()
        {
            List<SelectListItem> Result = new List<SelectListItem>() {
                new SelectListItem { Value = "true", Text = "Yes" },
                new SelectListItem { Value = "false", Text = "No" }
            };
            return new SelectList(Result, "Value", "Text");
        }

        public SelectList FWSActivitiesLong()
        {
            var Result = (from a in DbCMS.code4WSActivity.Where(x => x.Active)
                          select new
                          {
                              Value = a.SubActivity,
                              Text = a.SubActivity
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList FWSDeliveryMode()
        {
            var Result = (from a in DbCMS.code4WSActivityTag.Where(x => x.Active)
                          select new
                          {
                              Value = a.ActivityTagGUID,
                              Text = a.ActivityTagDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList FWSUnits()
        {
            var Result = (from a in DbCMS.code4WSActivity.Where(x => x.Active)
                          select new
                          {
                              Value = a.FWSUnit,
                              Text = a.FWSUnit
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList FWSAnalysisUnits()
        {
            var Result = (from a in DbCMS.code4WSActivity.Where(x => x.Active)
                          select new
                          {
                              Value = a.FWSUnitDetails,
                              Text = a.FWSUnitDetails
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList FWSBeneficiariesTypes()
        {
            var Result = (from a in DbCMS.code4WSBeneficiaryType.Where(x => x.Active)
                          select new
                          {
                              Value = a.BeneficiaryTypeGUID,
                              Text = a.BeneficiaryTypeDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList FWSPWD()
        {
            List<SelectListItem> Result = new List<SelectListItem>() {
                new SelectListItem { Value = "true", Text = "Yes" },
                new SelectListItem { Value = "false", Text = "No" }
            };
            return new SelectList(Result, "Value", "Text");
        }

        public SelectList FWSSubSectors()
        {
            var Result = (from a in DbCMS.code4WSActivity.Where(x => x.Active)
                          select new
                          {
                              Value = a.SubSectorEn,
                              Text = a.SubSectorEn
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList FWSActivities()
        {
            var Result = (from a in DbCMS.code4WSActivity.Where(x => x.Active)
                          select new
                          {
                              Value = a.ActivityEn,
                              Text = a.ActivityEn
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList FWSSubActivities()
        {
            var Result = (from a in DbCMS.code4WSActivity.Where(x => x.Active)
                          select new
                          {
                              Value = a.SubActivityOrg,
                              Text = a.SubActivityOrg
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList DataApprovalStatus()
        {
            List<SelectListItem> Result = new List<SelectListItem> {
                new SelectListItem { Value = "true", Text = "Approved" },
                new SelectListItem { Value = "false", Text = "Pending Approval" }
            };

            return new SelectList(Result, "Value", "Text");

        }

        public SelectList FWSFacilities()
        {
            var Result = (from a in DbCMS.code4WSFacility.Where(x => x.Active)
                          select new
                          {
                              Value = a.FacilityGUID,
                              Text = a.FacilityCode
                          }).ToList();

            return new SelectList(Result, "Value", "Text");

        }

        public SelectList FWSSurvivorsOfExplosiveHazards()
        {
            List<SelectListItem> Result = new List<SelectListItem>() {
                new SelectListItem { Value = "true", Text = "Yes" },
                new SelectListItem { Value = "false", Text = "No" }
            };
            return new SelectList(Result, "Value", "Text");
        }

        #endregion

        #region COV

        public SelectList COVGovernorates()
        {
            var Result = (from a in DbCMS.codeOchaLocation
                          select new
                          {
                              Value = a.admin1Pcode,
                              Text = a.admin1Name_en
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList COVDistricts(string admin1Pcode)
        {
            var Result = (from a in DbCMS.codeOchaLocation
                          where a.admin1Pcode == admin1Pcode
                          select new
                          {
                              Value = a.admin2Pcode,
                              Text = a.admin2Name_en
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList COVSubDistricts(string admin2Pcode)
        {
            var Result = (from a in DbCMS.codeOchaLocation
                          where a.admin2Pcode == admin2Pcode
                          select new
                          {
                              Value = a.admin3Pcode,
                              Text = a.admin3Name_en
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList COVCommunities(string admin3Pcode)
        {
            var Result = (from a in DbCMS.codeOchaLocation
                          where a.admin3Pcode == admin3Pcode
                          select new
                          {
                              Value = a.admin4Pcode,
                              Text = a.admin4Name_en
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList COVNeighborhoods(string admin4Pcode)
        {
            var Result = (from a in DbCMS.codeOchaLocation
                          where a.admin4Pcode == admin4Pcode
                          select new
                          {
                              Value = a.admin5Pcode,
                              Text = a.admin5Name_en
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList COVObjectives()
        {
            var Result = (from a in DbCMS.codeCovObjective.Where(x => x.Active)
                          select new
                          {
                              Value = a.ObjectiveGUID,
                              Text = a.ObjectiveDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList COVOutputs(Guid ObjectiveGUID)
        {
            var Result = (from a in DbCMS.codeCovOutput.Where(x => x.Active)
                          where a.ObjectiveGUID == ObjectiveGUID
                          select new
                          {
                              Value = a.OutputGUID,
                              Text = a.OutputDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList COVOutputs()
        {
            var Result = (from a in DbCMS.codeCovOutput.Where(x => x.Active)
                          select new
                          {
                              Value = a.OutputGUID,
                              Text = a.OutputDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList COVIndicators(Guid OutputGUID)
        {
            var Result = (from a in DbCMS.codeCovIndicator.Where(x => x.Active)
                          where a.OutputGUID == OutputGUID
                          select new
                          {
                              Value = a.IndicatorGUID,
                              Text = a.IndicatorDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList COVIndicators()
        {
            var Result = (from a in DbCMS.codeCovIndicator.Where(x => x.Active)
                          select new
                          {
                              Value = a.IndicatorGUID,
                              Text = a.IndicatorDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList COVIndicatorUnit(Guid IndicatorGUID)
        {
            var Result = (from a in DbCMS.codeCovIndicator.Where(x => x.Active)
                          where a.IndicatorGUID == IndicatorGUID
                          select new
                          {
                              Value = a.IndicatorGUID,
                              Text = a.CovIndicatorUnit
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList COVIndicatorUnit()
        {
            var Result = (from a in DbCMS.codeCovIndicator.Where(x => x.Active)
                          select new
                          {
                              Value = a.IndicatorGUID,
                              Text = a.CovIndicatorUnit
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList COVUnits()
        {
            var Result = (from a in DbCMS.codeCovUnit.Where(x => x.Active)
                          select new
                          {
                              Value = a.CovUnitGUID,
                              Text = a.CovUnitItems + " - " + a.CovUnitDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList COVUNHCRStrategies()
        {
            List<SelectListItem> Result = new List<SelectListItem>() {
                new SelectListItem { Value = "1", Text = "1" },
                new SelectListItem { Value = "2", Text = "2" },
                new SelectListItem { Value = "3", Text = "3" },
                new SelectListItem { Value = "4", Text = "4" }
            };
            return new SelectList(Result, "Value", "Text");
        }

        public SelectList COVDirectActivities()
        {
            List<SelectListItem> Result = new List<SelectListItem>();
            Result.Add(new SelectListItem { Value = "true", Text = "Yes" });
            Result.Add(new SelectListItem { Value = "false", Text = "No" });
            return new SelectList(Result, "Value", "Text");
        }

        public SelectList COVAssociated()
        {
            List<SelectListItem> Result = new List<SelectListItem>();
            Result.Add(new SelectListItem { Value = "true", Text = "Yes" });
            Result.Add(new SelectListItem { Value = "false", Text = "No" });
            return new SelectList(Result, "Value", "Text");
        }

        public SelectList COVStatuses()
        {
            List<SelectListItem> Result = new List<SelectListItem>
            {
                new SelectListItem { Value = "Ongoing", Text = "Ongoing" },
                new SelectListItem { Value = "Completed", Text = "Completed" },
                new SelectListItem { Value = "In-Progress", Text = "In-Progress" },
                new SelectListItem { Value = "Assessed", Text = "Assessed" }
            };
            return new SelectList(Result, "Value", "Text");
        }

        public SelectList COVTechnicalUnit()
        {
            var Result = (from a in DbCMS.codeTables.Where(x => x.TableGUID == LookupTables.COVResponseTechnicalUnit && x.Active)
                          join b in DbCMS.codeTablesValues.Where(x => x.Active) on a.TableGUID equals b.TableGUID
                          join c in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on b.ValueGUID equals c.ValueGUID
                          orderby a.SortID
                          select new
                          {
                              Value = b.ValueGUID.ToString(),
                              Text = c.ValueDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList COVMonthes()
        {
            List<SelectListItem> Result = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "January" },
                new SelectListItem { Value = "2", Text = "February" },
                new SelectListItem { Value = "3", Text = "March" },
                new SelectListItem { Value = "4", Text = "April" },
                new SelectListItem { Value = "5", Text = "May" },
                new SelectListItem { Value = "6", Text = "June" },
                new SelectListItem { Value = "7", Text = "July" },
                new SelectListItem { Value = "8", Text = "August" },
                new SelectListItem { Value = "9", Text = "September" },
                new SelectListItem { Value = "10", Text = "October" },
                new SelectListItem { Value = "11", Text = "November" },
                new SelectListItem { Value = "12", Text = "December" },
            };
            return new SelectList(Result, "Value", "Text");
        }

        #endregion

        #region OSA 
        public SelectList OfficesByDutyStation(Guid? DutyStationGUID)
        {
            var Result = (from a in DbCMS.codeOfficesLanguages.Where(x => x.LanguageID == LAN && x.Active && x.codeOffices.DutyStationGUID == DutyStationGUID)
                          select new
                          {
                              Value = a.OfficeGUID.ToString(),
                              Text = a.OfficeDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList OfficeFloorByOffice(Guid? OfficeGUID)
        {
            var Result = (from a in DbCMS.codeOfficeFloor.Where(x => x.Active && x.OfficeGUID == OfficeGUID)
                          select new
                          {
                              Value = a.OfficeFloorGUID.ToString(),
                              Text = a.FloorNumber
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList OfficeFloorRoomByOfficeFloor(Guid? OfficeFloorGUID)
        {
            var Result = (from a in DbCMS.codeOfficeFloorRoom.Where(x => x.Active && x.OfficeFloorGUID == OfficeFloorGUID)
                          select new
                          {
                              Value = a.OfficeFloorRoomGUID.ToString(),
                              Text = a.RoomNumber
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList ShuttleDepartureMorningTime()
        {
            List<SelectListItem> Result = new List<SelectListItem> {
                new SelectListItem { Text = "09:00 AM", Value = "9" },
                new SelectListItem { Text = "10:00 AM", Value = "10" },

            };

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList ShuttleDepartureEveningTime()
        {
            List<SelectListItem> Result = new List<SelectListItem> {
                new SelectListItem { Text = "03:00 PM", Value = "3" },
                new SelectListItem { Text = "04:00 PM", Value = "4" },

            };

            return new SelectList(Result, "Value", "Text");
        }
        #endregion

        #region GTP
        public SelectList GTPCategories()
        {
            var Result = (from a in DbCMS.codeGTPCategory
                          orderby a.SortID ascending
                          select new
                          {
                              Value = a.GTPCategoryGUID,
                              Text = a.GTPCategoryDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList GTPEligibility()
        {
            List<SelectListItem> Result = new List<SelectListItem>();
            Result.Add(new SelectListItem { Value = "true", Text = "Eligible" });
            Result.Add(new SelectListItem { Value = "false", Text = "Not Eligible" });
            return new SelectList(Result, "Value", "Text");
        }
        public SelectList GTPGenders()
        {
            List<SelectListItem> Result = new List<SelectListItem>();
            Result.Add(new SelectListItem { Value = "M", Text = "Male" });
            Result.Add(new SelectListItem { Value = "F", Text = "Female" });
            Result.Add(new SelectListItem { Value = "N/A", Text = "No Selection" });
            return new SelectList(Result, "Value", "Text");
        }
        public SelectList GTPMaritalStatuses()
        {
            var Result = (from a in DbCMS.codeTables.Where(x => x.TableGUID == LookupTables.GTPMaritalStatus && x.Active)
                          join b in DbCMS.codeTablesValues.Where(x => x.Active) on a.TableGUID equals b.TableGUID
                          join c in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on b.ValueGUID equals c.ValueGUID
                          orderby a.SortID ascending
                          select new
                          {
                              Value = b.ValueGUID.ToString(),
                              Text = c.ValueDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList GTPCountries()
        {
            Guid exclude = Guid.Parse("133a31c3-2c72-4bce-889d-3e5164931fb3");
            var Result = (from a in DbCMS.codeCountriesLanguages.Where(x => x.LanguageID == "EN" && x.Active)
                          where a.CountryGUID != exclude
                          orderby a.CountryDescription
                          select new
                          {
                              Value = a.CountryGUID,
                              Text = a.CountryDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList GTPCountries(Guid? Selected)
        {
            Guid exclude = Guid.Parse("133a31c3-2c72-4bce-889d-3e5164931fb3");
            var Result = (from a in DbCMS.codeCountriesLanguages.Where(x => x.LanguageID == "EN" && x.Active)
                          where a.CountryGUID != exclude
                          orderby a.CountryDescription
                          select new
                          {
                              Value = a.CountryGUID,
                              Text = a.CountryDescription
                          }).ToList();
            if (Selected.HasValue)
            {
                return new SelectList(Result, "Value", "Text", Selected.Value.ToString());
            }
            else
            {
                return new SelectList(Result, "Value", "Text");
            }

        }

        public SelectList GTPCities(Guid CountryGUID)
        {
            Guid LocationTypeGuid = Guid.Parse("C970DDF5-31E5-47A1-BE76-AF15833D4E6A"); // Governorate

            var Result = (from a in DbCMS.codeCities.Where(x => x.Active)
                          where a.CountryGUID == CountryGUID
                          orderby a.CityDescription
                          select new
                          {
                              Value = a.CityGUID,
                              Text = a.CityDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList GTPCities(Guid CountryGUID, Guid? Selected)
        {
            var Result = (from a in DbCMS.codeCities.Where(x => x.Active)
                          where a.CountryGUID == CountryGUID
                          orderby a.CityDescription
                          select new
                          {
                              Value = a.CityGUID,
                              Text = a.CityDescription
                          }).Distinct().ToList();

            if (Selected.HasValue)
            {
                return new SelectList(Result, "Value", "Text", Selected.ToString());
            }
            else
            {
                return new SelectList(Result, "Value", "Text");
            }
        }
        public SelectList GTPCities()
        {
            Guid LocationTypeGuid = Guid.Parse("C970DDF5-31E5-47A1-BE76-AF15833D4E6A"); // Governorate

            var Result = (from a in DbCMS.codeCities.Where(x => x.Active)
                          orderby a.CityDescription
                          select new
                          {
                              Value = a.CityGUID,
                              Text = a.CityDescription
                          }).Distinct().ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList GTPNationalities()
        {
            Guid exclude = Guid.Parse("133a31c3-2c72-4bce-889d-3e5164931fb3");
            var Result = (from a in DbCMS.codeCountriesLanguages.Where(x => x.LanguageID == "EN" && x.Active)
                          where a.CountryGUID != exclude
                          orderby a.Nationality
                          select new
                          {
                              Value = a.CountryGUID,
                              Text = a.Nationality
                          }).ToList();
            return new SelectList(Result, "Value", "Text");

        }
        public SelectList GTPEmploymentTypes()
        {
            var Result = (from a in DbCMS.codeTables.Where(x => x.TableGUID == LookupTables.GTPEmploymentTypes && x.Active)
                          join b in DbCMS.codeTablesValues.Where(x => x.Active) on a.TableGUID equals b.TableGUID
                          join c in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on b.ValueGUID equals c.ValueGUID
                          orderby a.SortID ascending
                          select new
                          {
                              Value = b.ValueGUID.ToString(),
                              Text = c.ValueDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList GTPEmploymentTypes(Guid TypeOfEmployementGUID)
        {
            var Result = (from a in DbCMS.codeTables.Where(x => x.TableGUID == LookupTables.GTPEmploymentTypes && x.Active)
                          join b in DbCMS.codeTablesValues.Where(x => x.Active) on a.TableGUID equals b.TableGUID
                          join c in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on b.ValueGUID equals c.ValueGUID
                          orderby a.SortID ascending
                          select new
                          {
                              Value = b.ValueGUID.ToString(),
                              Text = c.ValueDescription,
                          }).ToList();

            return new SelectList(Result, "Value", "Text", TypeOfEmployementGUID.ToString());
        }
        public SelectList GTPBusinessTypes()
        {
            var Result = (from a in DbCMS.codeTables.Where(x => x.TableGUID == LookupTables.GTPTypeOfBusiness && x.Active)
                          join b in DbCMS.codeTablesValues.Where(x => x.Active) on a.TableGUID equals b.TableGUID
                          join c in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on b.ValueGUID equals c.ValueGUID
                          orderby a.SortID ascending
                          select new
                          {
                              Value = b.ValueGUID.ToString(),
                              Text = c.ValueDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList GTPBusinessTypes(Guid? Selected)
        {
            var Result = (from a in DbCMS.codeTables.Where(x => x.TableGUID == LookupTables.GTPTypeOfBusiness && x.Active)
                          join b in DbCMS.codeTablesValues.Where(x => x.Active) on a.TableGUID equals b.TableGUID
                          join c in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on b.ValueGUID equals c.ValueGUID
                          orderby a.SortID ascending
                          select new
                          {
                              Value = b.ValueGUID.ToString(),
                              Text = c.ValueDescription
                          }).ToList();
            if (Selected.HasValue)
            {
                return new SelectList(Result, "Value", "Text", Selected.Value.ToString());
            }
            else
            {
                return new SelectList(Result, "Value", "Text");
            }
        }

        public SelectList GTPContractTypes()
        {
            var Result = (from a in DbCMS.codeTables.Where(x => x.TableGUID == LookupTables.GTPContractType && x.Active)
                          join b in DbCMS.codeTablesValues.Where(x => x.Active) on a.TableGUID equals b.TableGUID
                          join c in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on b.ValueGUID equals c.ValueGUID
                          orderby a.SortID ascending
                          select new
                          {
                              Value = b.ValueGUID.ToString(),
                              Text = c.ValueDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList GTPContractTypes(Guid Selected)
        {
            var Result = (from a in DbCMS.codeTables.Where(x => x.TableGUID == LookupTables.GTPContractType && x.Active)
                          join b in DbCMS.codeTablesValues.Where(x => x.Active) on a.TableGUID equals b.TableGUID
                          join c in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on b.ValueGUID equals c.ValueGUID
                          orderby a.SortID ascending
                          select new
                          {
                              Value = b.ValueGUID.ToString(),
                              Text = c.ValueDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text", Selected.ToString());
        }

        public SelectList GTPTrainingMethodologies()
        {
            var Result = (from a in DbCMS.codeTables.Where(x => x.TableGUID == LookupTables.GTPTrainingMethodology && x.Active)
                          join b in DbCMS.codeTablesValues.Where(x => x.Active) on a.TableGUID equals b.TableGUID
                          join c in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on b.ValueGUID equals c.ValueGUID
                          orderby a.SortID ascending
                          select new
                          {
                              Value = b.ValueGUID.ToString(),
                              Text = c.ValueDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList GTPTrainingMethodologies(Guid? Selected)
        {
            var Result = (from a in DbCMS.codeTables.Where(x => x.TableGUID == LookupTables.GTPTrainingMethodology && x.Active)
                          join b in DbCMS.codeTablesValues.Where(x => x.Active) on a.TableGUID equals b.TableGUID
                          join c in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on b.ValueGUID equals c.ValueGUID
                          orderby a.SortID ascending
                          select new
                          {
                              Value = b.ValueGUID.ToString(),
                              Text = c.ValueDescription
                          }).ToList();
            if (Selected.HasValue)
            {
                return new SelectList(Result, "Value", "Text", Selected.Value.ToString());

            }
            else
            {
                return new SelectList(Result, "Value", "Text");

            }
        }

        public SelectList GTPEducationLevels()
        {
            var Result = (from a in DbCMS.codeTables.Where(x => x.TableGUID == LookupTables.GTPEducationLevel && x.Active)
                          join b in DbCMS.codeTablesValues.Where(x => x.Active) on a.TableGUID equals b.TableGUID
                          join c in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on b.ValueGUID equals c.ValueGUID
                          orderby a.SortID ascending
                          select new
                          {
                              Value = b.ValueGUID.ToString(),
                              Text = c.ValueDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList GTPEducationLevels(Guid? Selected)
        {
            var Result = (from a in DbCMS.codeTables.Where(x => x.TableGUID == LookupTables.GTPEducationLevel && x.Active)
                          join b in DbCMS.codeTablesValues.Where(x => x.Active) on a.TableGUID equals b.TableGUID
                          join c in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on b.ValueGUID equals c.ValueGUID
                          orderby a.SortID ascending
                          select new
                          {
                              Value = b.ValueGUID.ToString(),
                              Text = c.ValueDescription
                          }).ToList();
            if (Selected.HasValue)
            {
                return new SelectList(Result, "Value", "Text", Selected.Value.ToString());
            }
            else
            {
                return new SelectList(Result, "Value", "Text");
            }

        }

        public SelectList GTPSkillLevels()
        {
            var Result = (from a in DbCMS.codeTables.Where(x => x.TableGUID == LookupTables.GTPSkillLevel && x.Active)
                          join b in DbCMS.codeTablesValues.Where(x => x.Active) on a.TableGUID equals b.TableGUID
                          join c in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on b.ValueGUID equals c.ValueGUID
                          orderby a.SortID ascending
                          select new
                          {
                              Value = b.ValueGUID.ToString(),
                              Text = c.ValueDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList GTPSkillLevels(Guid? Selected)
        {
            var Result = (from a in DbCMS.codeTables.Where(x => x.TableGUID == LookupTables.GTPSkillLevel && x.Active)
                          join b in DbCMS.codeTablesValues.Where(x => x.Active) on a.TableGUID equals b.TableGUID
                          join c in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on b.ValueGUID equals c.ValueGUID
                          orderby a.SortID ascending
                          select new
                          {
                              Value = b.ValueGUID.ToString(),
                              Text = c.ValueDescription
                          }).ToList();
            if (Selected.HasValue)
            {
                return new SelectList(Result, "Value", "Text", Selected.ToString());
            }
            else
            {
                return new SelectList(Result, "Value", "Text");
            }

        }
        public SelectList GTPLanguageLevels()
        {
            var Result = (from a in DbCMS.codeTables.Where(x => x.TableGUID == LookupTables.GTPLanguageLevel && x.Active)
                          join b in DbCMS.codeTablesValues.Where(x => x.Active) on a.TableGUID equals b.TableGUID
                          join c in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on b.ValueGUID equals c.ValueGUID
                          orderby a.SortID ascending
                          select new
                          {
                              Value = b.ValueGUID.ToString(),
                              Text = c.ValueDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList GTPVacancyHears()
        {
            var Result = (from a in DbCMS.codeTables.Where(x => x.TableGUID == LookupTables.GTPVacancyHear && x.Active)
                          join b in DbCMS.codeTablesValues.Where(x => x.Active) on a.TableGUID equals b.TableGUID
                          join c in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on b.ValueGUID equals c.ValueGUID
                          orderby a.SortID ascending
                          select new
                          {
                              Value = b.ValueGUID.ToString(),
                              Text = c.ValueDescription
                          }).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList GTPLanguages()
        {
            var Result = (from a in DbCMS.codeLanguages.Where(x => x.Active)
                          orderby a.SortID
                          select new
                          {
                              Value = a.LanguageID,
                              Text = a.LanguageNameEN
                          }).ToList();
            return new SelectList(Result, "Value", "Text");
        }

        public SelectList GTPLanguages(string Selected)
        {
            var Result = (from a in DbCMS.codeLanguages.Where(x => x.Active)
                          orderby a.SortID
                          select new
                          {
                              Value = a.LanguageID,
                              Text = a.LanguageNameEN
                          }).ToList();
            return new SelectList(Result, "Value", "Text", Selected.ToString());

        }
        public SelectList GTPPreferredPhoneNumber()
        {
            List<SelectListItem> Result = new List<SelectListItem>();
            Result.Add(new SelectListItem { Value = "1", Text = "Home" });
            Result.Add(new SelectListItem { Value = "2", Text = "Business" });
            Result.Add(new SelectListItem { Value = "3", Text = "Mobile" });
            return new SelectList(Result, "Value", "Text");
        }
        public SelectList GTPPreferredEmailAddress()
        {
            List<SelectListItem> Result = new List<SelectListItem>();
            Result.Add(new SelectListItem { Value = "1", Text = "Home" });
            Result.Add(new SelectListItem { Value = "2", Text = "Business" });
            return new SelectList(Result, "Value", "Text");
        }
        public SelectList GTPPreferredContactMethod()
        {
            List<SelectListItem> Result = new List<SelectListItem>();
            Result.Add(new SelectListItem { Value = "1", Text = "Email" });
            Result.Add(new SelectListItem { Value = "2", Text = "Phone" });
            Result.Add(new SelectListItem { Value = "3", Text = "Post Mail" });
            return new SelectList(Result, "Value", "Text");
        }
        public SelectList GTPYesNo()
        {
            List<SelectListItem> Result = new List<SelectListItem>();
            Result.Add(new SelectListItem { Value = "true", Text = "Yes" });
            Result.Add(new SelectListItem { Value = "false", Text = "No" });
            return new SelectList(Result, "Value", "Text");
        }

        public SelectList GTPCompleted()
        {
            List<SelectListItem> Result = new List<SelectListItem>();
            Result.Add(new SelectListItem { Value = "true", Text = "Completed" });
            Result.Add(new SelectListItem { Value = "false", Text = "Not Completed" });
            return new SelectList(Result, "Value", "Text");
        }

        public SelectList GTPYesNo(bool Selected)
        {
            List<SelectListItem> Result = new List<SelectListItem>();
            Result.Add(new SelectListItem { Value = "true", Text = "Yes" });
            Result.Add(new SelectListItem { Value = "false", Text = "No" });
            return new SelectList(Result, "Value", "Text", Selected);
        }
        public SelectList GTPFullTimePartTime()
        {
            List<SelectListItem> Result = new List<SelectListItem>();
            Result.Add(new SelectListItem { Value = "1", Text = "Full Time" });
            Result.Add(new SelectListItem { Value = "2", Text = "Part Time" });
            return new SelectList(Result, "Value", "Text");
        }
        public SelectList GTPFullTimePartTime(int selection)
        {
            //List<SelectListItem> Result = new List<SelectListItem>();
            //if (selection == 1)
            //{
            //    Result.Add(new SelectListItem { Value = "1", Text = "Full Time", Selected = true });
            //    Result.Add(new SelectListItem { Value = "2", Text = "Part Time" });
            //}
            //else if (selection == 2)
            //{
            //    Result.Add(new SelectListItem { Value = "1", Text = "Full Time" });
            //    Result.Add(new SelectListItem { Value = "2", Text = "Part Time", Selected = true });
            //}
            //else
            //{
            //    Result.Add(new SelectListItem { Value = "1", Text = "Full Time" });
            //    Result.Add(new SelectListItem { Value = "2", Text = "Part Time" });
            //}
            List<SelectListItem> Result = new List<SelectListItem>();
            Result.Add(new SelectListItem { Value = "1", Text = "Full Time" });
            Result.Add(new SelectListItem { Value = "2", Text = "Part Time" });
            return new SelectList(Result, "Value", "Text", selection.ToString());
        }

        public SelectList StaffGrades(Guid? Selected)
        {
            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.codeTables.TableGUID == LookupTables.StaffGradeTypes && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              Value = a.ValueGUID.ToString(),
                              Text = b.ValueDescription
                          }).Distinct().ToList();
            if (Selected.HasValue)
            {
                return new SelectList(Result, "Value", "Text", Selected.Value.ToString());

            }
            else
            {
                return new SelectList(Result, "Value", "Text");

            }
        }
        #endregion

        #region PMD
        public SelectList PMDReportTopicRELC()
        {
            List<SelectListItem> Result = new List<SelectListItem>() {
                new SelectListItem { Value = "0", Text =  "Summary Report Yearly" },
                new SelectListItem { Value = "1", Text =  "Summary Report Monthly" },
                new SelectListItem { Value = "2", Text =  "Items Distribution Details" },
                new SelectListItem { Value = "3", Text =  "Items Dispatch Details" },
                new SelectListItem { Value = "4", Text =  "Items Transfer Details" },
                new SelectListItem { Value = "5", Text =  "Items Lost and Damaged Details" },

            };

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList PMDGovernorates()
        {
            var Result = (from a in DbCMS.codeOchaLocation
                          select new
                          {
                              Value = a.admin1Pcode,
                              Text = LAN == "AR" ? a.admin1Name_ar : a.admin1Name_en
                          }).Distinct().OrderBy(x => x.Text).ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList PMDGovernorateGuids()
        {
            var Result = (from a in DbCMS.codeOchaLocation
                          select new
                          {
                              Value = a.GovernorateGUID.ToString(),
                              Text = LAN == "AR" ? a.admin1Name_ar : a.admin1Name_en
                          }).Distinct().OrderBy(x => x.Text).ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList PMDDistricts(string admin1Pcode)
        {
            var Result = (from a in DbCMS.codeOchaLocation
                          where a.admin1Pcode == admin1Pcode
                          select new
                          {
                              Value = a.admin2Pcode,
                              Text = LAN == "AR" ? a.admin2Name_ar : a.admin2Name_en
                          }).Distinct().OrderBy(x => x.Text).ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList PMDSubDistricts(string admin2Pcode)
        {
            var Result = (from a in DbCMS.codeOchaLocation
                          where a.admin2Pcode == admin2Pcode
                          select new
                          {
                              Value = a.admin3Pcode,
                              Text = LAN == "AR" ? a.admin3Name_ar : a.admin3Name_en
                          }).Distinct().OrderBy(x => x.Text).ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList PMDCommunities(string admin3Pcode)
        {
            var Result = (from a in DbCMS.codeOchaLocation
                          where a.admin3Pcode == admin3Pcode
                          select new
                          {
                              Value = a.admin4Pcode,
                              Text = LAN == "AR" ? a.admin4Name_ar : a.admin4Name_en
                          }).Distinct().OrderBy(x => x.Text).ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList PMDCommunitiesByAdmin1Pcode(string admin1Pcode)
        {
            var Result = (from a in DbCMS.codeOchaLocation
                          where a.admin1Pcode == admin1Pcode
                          select new
                          {
                              Value = a.admin4Pcode,
                              Text = LAN == "AR" ? a.admin4Name_ar : a.admin4Name_en
                          }).Distinct().OrderBy(x => x.Text).ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList PMDNeighborhoods(string admin4Pcode)
        {
            var Result = (from a in DbCMS.codeOchaLocation
                          where a.admin4Pcode == admin4Pcode
                          select new
                          {
                              Value = a.admin5Pcode,
                              Text = LAN == "AR" ? a.admin5Name_ar : a.admin5Name_en
                          }).Distinct().OrderBy(x => x.Text).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList PMDCommunityCenters(string admin4Pcode)
        {
            var Result = (from a in DbCMS.codePmdCommunityCenters
                          where a.admin4Pcode == admin4Pcode
                          select new
                          {
                              Value = a.PMDCommunityCenterGUID,
                              Text = a.PMDCommunityCenter
                          }).Distinct().OrderBy(x => x.Text).ToList();

            return new SelectList(Result, "Value", "Text");
        }


        public SelectList PMDObjectives()
        {
            var Result = (from a in DbCMS.codePmdObjectiveLanguages.Where(x => x.Active)
                          where a.LanguageID == LAN
                          select new
                          {
                              Value = a.ObjectiveGUID,
                              Text = a.ObjectiveDescription
                          }).Distinct().OrderBy(x => x.Text).ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList PMDOutputs(Guid ObjectiveGUID)
        {
            var Result = (from a in DbCMS.codePmdOutputs
                          join b in DbCMS.codePmdOutputLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.OutputGUID equals b.OutputGUID
                          where a.ObjectiveGUID == ObjectiveGUID
                          select new
                          {
                              Value = a.OutputGUID,
                              Text = b.OutputDescription
                          }).Distinct().OrderBy(x => x.Text).ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList PMDOutputs()
        {

            var Result = (from a in DbCMS.codePmdOutputs
                          join b in DbCMS.codePmdOutputLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.OutputGUID equals b.OutputGUID
                          select new
                          {
                              Value = a.OutputGUID,
                              Text = b.OutputDescription
                          }).Distinct().OrderBy(x => x.Text).ToList();

            return new SelectList(Result, "Value", "Text");
        }
        public SelectList PMDIndicators(Guid OutputGUID)
        {
            var Result = (from a in DbCMS.codePmdIndicators
                          join b in DbCMS.codePmdIndicatorLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.IndicatorGUID equals b.IndicatorGUID
                          where a.OutputGUID == OutputGUID
                          select new
                          {
                              Value = a.IndicatorGUID,
                              Text = b.IndicatorDescription
                          }).Distinct().OrderBy(x => x.Text).ToList();


            return new SelectList(Result, "Value", "Text");
        }
        public SelectList PMDIndicators()
        {
            var Result = (from a in DbCMS.codePmdIndicators
                          join b in DbCMS.codePmdIndicatorLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.IndicatorGUID equals b.IndicatorGUID
                          select new
                          {
                              Value = a.IndicatorGUID,
                              Text = b.IndicatorDescription
                          }).Distinct().OrderBy(x => x.Text).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList PMDDirectActivities()
        {
            List<SelectListItem> Result = new List<SelectListItem>();
            if (LAN == "AR")
            {
                Result.Add(new SelectListItem { Value = "true", Text = "نعم" });
                Result.Add(new SelectListItem { Value = "false", Text = "لا" });
            }
            else
            {
                Result.Add(new SelectListItem { Value = "true", Text = "Yes" });
                Result.Add(new SelectListItem { Value = "false", Text = "No" });
            }

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList PMDObjectiveStatuses(Guid ObjectiveGUID)
        {
            var Result = (from a in DbCMS.dataPartnerMonitoringObjectiveStatus.Where(x => x.Active)
                          join b in DbCMS.codePmdObjectiveStatusLanguage.Where(x => x.LanguageID == LAN) on a.ObjectiveStatusGUID equals b.PmdObjectiveStatusGUID
                          where a.ObjectiveGUID == ObjectiveGUID
                          select new
                          {
                              Value = a.ObjectiveStatusGUID,
                              Text = b.StatusDescription
                          }).Distinct().OrderBy(x => x.Text).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList PMDImplementingPartners()
        {
            List<Guid> partners = new List<Guid>();
            partners.Add(Guid.Parse("50A27E68-C33C-46EB-A5BF-F2A28BF4B77B"));
            partners.Add(Guid.Parse("D3A556C4-BE2A-4A96-A875-569A55144EF0"));
            partners.Add(Guid.Parse("DC89E59A-1FAA-4C41-9CD2-06DFBF22D246"));
            partners.Add(Guid.Parse("D0ADD1D1-7683-444E-8EC1-3702AA35868A"));
            partners.Add(Guid.Parse("34CDD83B-16BF-4C20-9E6D-CFC13F543CE5"));
            partners.Add(Guid.Parse("419D9F24-A539-4962-A551-4A64A50111B6"));
            partners.Add(Guid.Parse("8E484921-89D3-41C6-B45E-8D51BFF194E8"));
            partners.Add(Guid.Parse("D3F59141-9318-4420-A5A1-0E52BEB66D9B"));
            partners.Add(Guid.Parse("FDFCB878-C881-4B87-BD4F-744C4F51AD19"));
            partners.Add(Guid.Parse("408357F7-6F52-4DE5-A1AA-E87264191601"));
            partners.Add(Guid.Parse("A52B718F-ADD4-4636-B9CE-ED08F48DC389"));
            partners.Add(Guid.Parse("838537F2-4AA3-4873-880E-B36994E6F3FF"));
            partners.Add(Guid.Parse("34D6C57B-87ED-477E-891A-3B4CA4704F35"));
            partners.Add(Guid.Parse("0EDD38DA-F601-4617-B6FF-1FF12F65EBC4"));
            partners.Add(Guid.Parse("84FDA223-435B-41D1-8D11-9681BB0B986C"));
            partners.Add(Guid.Parse("682A458F-085E-4A97-8B2C-CDA4C1736C33"));
            partners.Add(Guid.Parse("03E6D704-07E2-4AE8-8EB4-BD0B52281D50"));
            partners.Add(Guid.Parse("19E1CD6D-22E7-475C-8A9C-54ABEDD46652"));
            partners.Add(Guid.Parse("E156C022-EC72-4A5A-BE09-163BD85C68EF"));
            partners.Add(Guid.Parse("CC952846-173E-452C-90A6-FA8832059652"));
            partners.Add(Guid.Parse("8FEC0216-2ED0-47C0-B2B0-C60106DC8A05"));
            partners.Add(Guid.Parse("59737681-787D-412C-BEE7-3C62986734FA"));
            partners.Add(Guid.Parse("7BCB2B1B-7342-448C-BA65-943E40307D21"));
            var Result = (from a in DbCMS.codeOrganizationsInstances.Where(x => x.Active)
                          join b in DbCMS.codeOrganizationsInstancesLanguages.Where(x => x.Active) on a.OrganizationInstanceGUID equals b.OrganizationInstanceGUID
                          where b.LanguageID == LAN && partners.Contains(a.OrganizationInstanceGUID)
                          select new
                          {
                              Value = a.OrganizationInstanceGUID,
                              Text = b.OrganizationInstanceDescription
                          }).Distinct().OrderBy(x => x.Text).ToList();
            //var Result = (from a in DbCMS.codePmdPartner.Where(x => x.Active)
            //              select new
            //              {
            //                  Value = a.PartnerGUID,
            //                  Text = a.PartnerDescription
            //              }).Distinct().OrderBy(x => x.Text).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList PMDUnits()
        {
            var Result = (from a in DbCMS.codePmdUnitOfAchievementLanguages.Where(x => x.Active)
                          where a.LanguageID == LAN
                          select new
                          {
                              Value = a.UnitOfAchievementGUID,
                              Text = a.UnitOfAchievementDescription
                          }).Distinct().OrderBy(x => x.Text).ToList();

            return new SelectList(Result, "Value", "Text");
        }

        public SelectList PMD2022Units()
        {
            using (var DbPMD = new PMDEntities())
            {
                var Result = (from a in DbPMD.codePmd2022UnitOfAchievementLanguages.Where(x => x.Active && x.codePmd2022UnitOfAchievement.UnitOfAchievementCategory == "Domestic")
                              where a.LanguageID == LAN
                              select new
                              {
                                  Value = a.UnitOfAchievementGUID,
                                  Text = a.UnitOfAchievementDescription
                              }).Distinct().OrderBy(x => x.Text).ToList();

                return new SelectList(Result, "Value", "Text");
            }

        }

        public SelectList PMD2023Units()
        {
            using (var DbPMD = new PMDEntities())
            {
                var Result = (from a in DbPMD.codePmd2023UnitOfAchievementLanguages.Where(x => x.Active && x.LanguageID == "EN" && x.codePmd2023UnitOfAchievement.UnitOfAchievementCategory == "CRI")
                              where a.LanguageID == LAN
                              select new
                              {
                                  Value = a.UnitOfAchievementGUID,
                                  Text = a.UnitOfAchievementDescription
                              }).Distinct().OrderBy(x => x.Text).ToList();

                return new SelectList(Result, "Value", "Text");
            }

        }

        public SelectList PMDTemplateTypes()
        {
            List<SelectListItem> Result = new List<SelectListItem>();

            Result.Add(new SelectListItem { Value = "Main", Text = "Main Template" });
            Result.Add(new SelectListItem { Value = "Health", Text = "Health Template" });
            Result.Add(new SelectListItem { Value = "CRI", Text = "CRI Template" });

            return new SelectList(Result, "Value", "Text");
        }


        public SelectList PMDWarehouse(string admin4Pcode, Guid ImplementingPartnerGUID)
        {
            using (var DbPMD = new PMD_DAL.Model.PMDEntities())
            {
                if (admin4Pcode != "")
                {
                    var Result = (from a in DbPMD.codePmdWarehouse.Where(x => x.admin4Pcode == admin4Pcode && x.ImplementingPartnerGUID == ImplementingPartnerGUID && x.Active)
                                  join b in DbPMD.codePmdWarehouseLanguage.Where(x => x.LanguageID == LAN && x.Active) on a.PmdWarehouseGUID equals b.PmdWarehouseGUID

                                  select new
                                  {
                                      Value = a.PmdWarehouseGUID,
                                      Text = b.PMDWarehouseDescription
                                  }).Distinct().OrderBy(x => x.Text).ToList();

                    return new SelectList(Result, "Value", "Text");
                }
                else
                {
                    var Result = (from a in DbPMD.codePmdWarehouse.Where(x =>  x.ImplementingPartnerGUID == ImplementingPartnerGUID && x.Active)
                                  join b in DbPMD.codePmdWarehouseLanguage.Where(x => x.LanguageID == LAN && x.Active) on a.PmdWarehouseGUID equals b.PmdWarehouseGUID

                                  select new
                                  {
                                      Value = a.PmdWarehouseGUID,
                                      Text = b.PMDWarehouseDescription
                                  }).Distinct().OrderBy(x => x.Text).ToList();

                    return new SelectList(Result, "Value", "Text");
                }
            }
        }
        #endregion

    }
}

