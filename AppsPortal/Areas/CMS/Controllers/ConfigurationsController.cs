using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.Models;
using AppsPortal.SignalR;
using AppsPortal.ViewModels;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace AppsPortal.Controllers
{
    public class ConfigurationsController : PortalBaseController
    {
        [Route("CMS/Configurations/")]
        public ActionResult Index()
        {
            return View("~/Areas/CMS/Views/Configurations/Index.cshtml", null, Session[SessionKeys.OrganizationInstanceGUID].ToString());
        }

        #region Countries Configuration
        public ActionResult CountriesConfigDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/CMS/Views/Configurations/Countries/_DataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            var Result = (from a in DbCMS.codeCountries
                          join b in DbCMS.codeCountriesLanguages.Where(x => x.LanguageID == LAN) on a.CountryGUID equals b.CountryGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()
                          from R2 in a.codeCountriesConfigurations.Where(x => x.OrganizationInstanceGUID == PK && x.Active)

                          orderby R1.CountryDescription
                          select new
                          {
                              R2.CountryConfigurationGUID,
                              R1.CountryDescription,
                              R2.codeCountriesConfigurationsRowVersion
                          });

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult CountriesConfigCreate(string FK)
        {
            Guid _OperationGUID = Guid.Parse(FK);

            var Countries = (from a in DbCMS.codeCountries.Where(x => x.Active)
                             join b in DbCMS.codeCountriesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.CountryGUID equals b.CountryGUID
                             where !DbCMS.codeCountriesConfigurations.Where(x => x.OrganizationInstanceGUID == _OperationGUID && x.Active).Select(x => x.codeCountries).Contains(a)
                             orderby b.CountryDescription
                             select new CheckBoxList
                             {
                                 Value = a.CountryGUID.ToString(),
                                 Description = b.CountryDescription,
                                 SearchKey = a.CountryA3Code
                             }).ToList();

            ConfigurationModel ConfigurationModel = new ConfigurationModel
            {
                ValueGuid = _OperationGUID,
                CheckBoxList = Countries,
            };
            return PartialView("~/Areas/CMS/Views/Configurations/Countries/_Modal.cshtml", ConfigurationModel);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CountriesConfigCreate(ConfigurationModel model)
        {
            //Guid ActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            DateTime ExecutionTime = DateTime.Now;

            List<string> mdList = model.CheckBoxList.Where(x => x.Checked == true).Select(x => x.Value).ToList();
            List<codeCountriesConfigurations> dbList = DbCMS.codeCountriesConfigurations.Where(s => s.OrganizationInstanceGUID == model.ValueGuid && s.Active).ToList();

            var toAddList = mdList.Where(s => !dbList.Select(x => x.CountryGUID.ToString()).Contains(s)).Select(s => new codeCountriesConfigurations
            {
                CountryConfigurationGUID = Guid.NewGuid(),
                OrganizationInstanceGUID = model.ValueGuid,
                CountryGUID = Guid.Parse(s),
            }).ToList();

            DbCMS.CreateBulk(toAddList, Permissions.CountriesConfigurations.CreateGuid, ExecutionTime);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(DataTableNames.CountriesConfigDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return null;
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult CountriesConfigDataTableDelete(List<codeCountriesConfigurations> models)
        {
            // Guid DeleteActionGUID = 
            ;
            DateTime ExecutionTime = DateTime.Now;
            List<codeCountriesConfigurations> DeletedTablesValues = new List<codeCountriesConfigurations>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbCMS.QueryBuilder(models, Permissions.CountriesConfigurations.RemoveGuid, SubmitTypes.Delete, baseQuery);

            var Records = DbCMS.Database.SqlQuery<codeCountriesConfigurations>(query).ToList();
            foreach (var record in Records)
            {
                DeletedTablesValues.Add(DbCMS.Delete(record, ExecutionTime, Permissions.CountriesConfigurations.RemoveGuid));
            }
            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialDeleteMessage(DeletedTablesValues, models, DataTableNames.CountriesConfigDataTable));
            }
            catch (Exception ex)

            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }
        #endregion

        #region Duty Stations Configuration
        public ActionResult DutyStationsConfigDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/CMS/Views/Configurations/DutyStations/_DataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            var Result = (from a in DbCMS.codeDutyStations
                          join b in DbCMS.codeDutyStationsLanguages.Where(x => x.LanguageID == LAN) on a.DutyStationGUID equals b.DutyStationGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()
                          from R2 in a.codeDutyStationsConfigurations.Where(x => x.OrganizationInstanceGUID == PK && x.Active)

                          orderby R1.DutyStationDescription
                          select new
                          {
                              R2.DutyStationConfigurationGUID,
                              R1.DutyStationDescription,
                              R2.codeDutyStationsConfigurationsRowVersion
                          });

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("CMS/Configurations/Organizations/Create/")]
        public ActionResult DutyStationsConfigCreate(string FK)
        {
            Guid _OrganizationInstanceGUID = Guid.Parse(FK);
            //////////////////////////
            /// THINK ABOUT GROUP  ///
            //////////////////////////
            var DutyStations = (from a in DbCMS.codeDutyStations.Where(x => x.Active)
                                join b in DbCMS.codeDutyStationsLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.DutyStationGUID equals b.DutyStationGUID
                                where !DbCMS.codeDutyStationsConfigurations.Where(x => x.OrganizationInstanceGUID == _OrganizationInstanceGUID && x.Active).Select(x => x.codeDutyStations).Contains(a)
                                join c in DbCMS.codeCountriesLanguages.Where(x => x.LanguageID == LAN) on a.CountryGUID equals c.CountryGUID
                                orderby a.CountryGUID, b.DutyStationDescription
                                select new CheckBoxList
                                {
                                    Value = a.DutyStationGUID.ToString(),
                                    Description = b.DutyStationDescription,
                                    GroupDescription = c.CountryDescription
                                }).ToList();

            ConfigurationModel ConfigurationModel = new ConfigurationModel
            {
                ValueGuid = _OrganizationInstanceGUID,
                CheckBoxList = DutyStations,
            };

            return PartialView("~/Areas/CMS/Views/Configurations/DutyStations/_Modal.cshtml", ConfigurationModel);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DutyStationsConfigCreate(ConfigurationModel model)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<string> mdList = model.CheckBoxList.Where(x => x.Checked == true).Select(x => x.Value).ToList();

            var toAddList = mdList.Select(x => new codeDutyStationsConfigurations
            {
                OrganizationInstanceGUID = model.ValueGuid,
                DutyStationConfigurationGUID = Guid.NewGuid(),
                DutyStationGUID = Guid.Parse(x),
            }).ToList();

            DbCMS.CreateBulk(toAddList, Permissions.DutyStationsConfigurations.CreateGuid, ExecutionTime);

            try
            {
                DbCMS.SaveChanges();
                var context = GlobalHost.ConnectionManager.GetHubContext<cmsHub, IClient>();
                context.Clients.AllExcept(UserGUID.ToString()).operationSettingsReload(model.ValueGuid, "Data has been modified, please reload the page.", "AccordionNodeReLoad('" + model.ValueGuid + "');");

                string url = Url.Action("ReloadDutyStationsDDL", "Configurations", new { FK = model.ValueGuid });
                string callBackFunc = "$('#DivDutyStationsDDL').load('" + url + "');";
                return Json(DbCMS.SingleUpdateMessage(DataTableNames.DutyStationsConfigDataTable, null, null, callBackFunc));
            }
            catch (DbUpdateConcurrencyException)
            {
                return null;
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult DutyStationsConfigDataTableDelete(List<codeDutyStationsConfigurations> models)
        {
            Guid DeleteActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            DateTime ExecutionTime = DateTime.Now;
            List<codeDutyStationsConfigurations> DeletedTablesValues = new List<codeDutyStationsConfigurations>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbCMS.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Delete, baseQuery);

            var Records = DbCMS.Database.SqlQuery<codeDutyStationsConfigurations>(query).ToList();
            foreach (var record in Records)
            {
                DeletedTablesValues.Add(DbCMS.Delete(record, ExecutionTime, DeleteActionGUID));
            }
            try
            {
                Guid OrganizationInstanceGUID = DbCMS.codeDutyStationsConfigurations.Find(models.First().DutyStationConfigurationGUID).OrganizationInstanceGUID;
                string url = Url.Action("ReloadDutyStationsDDL", "Configurations", OrganizationInstanceGUID);
                string callBackFunc = "$('#DivDutyStationsDDL').load('" + url + "');";

                DbCMS.SaveChanges();
                return Json(DbCMS.PartialDeleteMessage(DeletedTablesValues, models, DataTableNames.DutyStationsConfigDataTable, callBackFunc));
            }
            catch (Exception ex)

            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        public ActionResult ReloadDutyStationsDDL(Guid FK)
        {
            return PartialView("~/Areas/CMS/Views/Configurations/WorkingDays/_DutyStationsDDL.cshtml", FK);
        }
        #endregion

        #region Departments Configuration

        public ActionResult DepartmentsConfigDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/CMS/Views/Configurations/Departments/_DataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            var Result = (from a in DbCMS.codeDepartmentsConfigurations.Where(x => x.Active && x.OrganizationInstanceGUID == PK)
                          join b in DbCMS.codeDepartmentsLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.DepartmentGUID equals b.DepartmentGUID
                          join c in DbCMS.codeDepartmentsLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.ParentDepartmentGUID equals c.DepartmentGUID into LJ1
                          from l in LJ1.DefaultIfEmpty()
                          join d in DbCMS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.DepartmentTypeGUID equals d.ValueGUID

                          select new
                          {
                              DepartmentConfigurationGUID = a.DepartmentConfigurationGUID,
                              DepartmentDescription = b.DepartmentDescription,
                              DepartmentType = d.ValueDescription,
                              DepartmentParent = l.DepartmentDescription == null ? "-" : l.DepartmentDescription,
                              codeDepartmentsConfigurationsRowVersion = a.codeDepartmentsConfigurationsRowVersion
                          });

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult DepartmentsConfigCreate(string PK)
        {
            return PartialView("~/Areas/CMS/Views/Configurations/Departments/_Modal.cshtml", new codeDepartmentsConfigurations { OrganizationInstanceGUID = Guid.Parse(PK) });
        }

        public ActionResult DepartmentsConfigUpdate(string PK)
        {
            codeDepartmentsConfigurations model = new codeDepartmentsConfigurations();
            Guid _PK = Guid.Parse(PK);
            model = DbCMS.codeDepartmentsConfigurations.Find(_PK);
            return PartialView("~/Areas/CMS/Views/Configurations/Departments/_Modal.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DepartmentsConfigCreate(codeDepartmentsConfigurations model)
        {
            if (!ModelState.IsValid) return PartialView("~/Areas/CMS/Views/Configurations/Departments/_Modal.cshtml", model);

            Guid ActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            DateTime ExecutionTime = DateTime.Now;

            DbCMS.Create(model, ActionGUID, ExecutionTime);

            try
            {
                DbCMS.SaveChanges();
                OrganizationDepartmentsList dbModel = new OrganizationDepartmentsList();

                return Json(DbCMS.SingleCreateMessage(DataTableNames.DepartmentsConfigDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DepartmentsConfigUpdate(codeDepartmentsConfigurations model)
        {
            if (!ModelState.IsValid) return PartialView("~/Areas/CMS/Views/Configurations/Departments/_Modal.cshtml", model);

            Guid ActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            DateTime ExecutionTime = DateTime.Now;

            DbCMS.Update(model, ActionGUID, ExecutionTime);

            try
            {
                DbCMS.SaveChanges();
                OrganizationDepartmentsList dbModel = new OrganizationDepartmentsList();

                return Json(DbCMS.SingleUpdateMessage(DataTableNames.DepartmentsConfigDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult DepartmentsConfigDataTableDelete(List<codeDepartmentsConfigurations> models)
        {
            Guid DeleteActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            DateTime ExecutionTime = DateTime.Now;
            List<codeDepartmentsConfigurations> DeletedDepartments = new List<codeDepartmentsConfigurations>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbCMS.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Delete, baseQuery);

            var Records = DbCMS.Database.SqlQuery<codeDepartmentsConfigurations>(query).ToList();
            foreach (var record in Records)
            {
                DeletedDepartments.Add(DbCMS.Delete(record, ExecutionTime, DeleteActionGUID));
            }
            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialDeleteMessage(DeletedDepartments, models, DataTableNames.DepartmentsConfigDataTable));
            }
            catch (Exception ex)

            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }
        #endregion

        #region Working Days Configuration
        public ActionResult WorkingDaysConfigUpdate(Guid DutyStationGUID, Guid PK)
        {
            Guid DutyStationConfigurationGUID = (from a in DbCMS.codeDutyStationsConfigurations.Where(x => x.OrganizationInstanceGUID == PK && x.DutyStationGUID == DutyStationGUID && x.Active) select a.DutyStationConfigurationGUID).FirstOrDefault();

            var WorkingDays = (from a in DbCMS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN)
                               join b in DbCMS.codeTablesValues.Where(x => x.Active && x.TableGUID == LookupTables.WorkingDays) on a.ValueGUID equals b.ValueGUID
                               join c in DbCMS.codeWorkingDaysConfigurations.Where(x => x.DutyStationConfigurationGUID == (from a in DbCMS.codeDutyStationsConfigurations.Where(y => y.OrganizationInstanceGUID == PK && y.DutyStationGUID == DutyStationGUID && y.Active) select a.DutyStationConfigurationGUID).FirstOrDefault() && x.Active) on a.ValueGUID equals c.DayGUID into LJ1

                               from l in LJ1.DefaultIfEmpty()
                               orderby b.SortID
                               select new WorkingDaysModel
                               {
                                   WorkingDaysConfigurationGUID = l.WorkingDaysConfigurationGUID,
                                   DayGUID = b.ValueGUID,
                                   Day = a.ValueDescription,
                                   FromTime = l.FromTime,
                                   ToTime = l.ToTime,
                                   Status = (l == null) ? false : true,
                                   Active = (l == null) ? false : true,
                                   codeWorkingDaysConfigurationsRowVersion = l.codeWorkingDaysConfigurationsRowVersion
                               }).ToList();
            WorkingDaysConfigModel model = new WorkingDaysConfigModel();
            model.OrganizationInstanceGUID = PK;
            model.DutyStationsGUID = DutyStationGUID;
            model.WorkingDaysModelList = WorkingDays;
            model.DutyStationConfigurationGUID = DutyStationConfigurationGUID;
            return PartialView("~/Areas/CMS/Views/Configurations/WorkingDays/_DataTable.cshtml", model);
        }

        [HttpPost]
        public ActionResult WorkingDaysConfigUpdate(WorkingDaysConfigModel model)
        {
            DateTime ExecutionTime = DateTime.Now;
            Guid CreateActionGUID = Permissions.WorkingDaysConfigurations.CreateGuid;
            Guid UpdateActionGUID = Permissions.WorkingDaysConfigurations.UpdateGuid;
            Guid DeleteActionGUID = Permissions.WorkingDaysConfigurations.RemoveGuid;

            Guid DutyStationConfigurationGUID = (from a in DbCMS.codeDutyStationsConfigurations.Where(x => x.DutyStationGUID == model.DutyStationsGUID && x.OrganizationInstanceGUID == model.OrganizationInstanceGUID&& x.Active) select a.DutyStationConfigurationGUID).FirstOrDefault();

            foreach (var item in model.WorkingDaysModelList)
            {
                item.DutyStationConfigurationGUID = DutyStationConfigurationGUID;
            }

            List<WorkingDaysModel> CheckedWorkingDaysList = model.WorkingDaysModelList.Where(x => x.Status == true).ToList();
            List<WorkingDaysModel> UncheckedWorkingDaysList = model.WorkingDaysModelList.Where(x => x.Status == false).ToList();

            List<WorkingDaysModel> ToAddList = CheckedWorkingDaysList.Where(x => x.WorkingDaysConfigurationGUID == null).ToList();
            List<WorkingDaysModel> ToUpdateList = CheckedWorkingDaysList.Where(x => x.WorkingDaysConfigurationGUID != null).ToList();
            List<WorkingDaysModel> ToRemoveList = UncheckedWorkingDaysList.Where(x => x.WorkingDaysConfigurationGUID != null).ToList();

            List<codeWorkingDaysConfigurations> add = AutoMapper.Mapper.Map(ToAddList, new List<codeWorkingDaysConfigurations>());
            List<codeWorkingDaysConfigurations> update = AutoMapper.Mapper.Map(ToUpdateList, new List<codeWorkingDaysConfigurations>());
            List<codeWorkingDaysConfigurations> remove = AutoMapper.Mapper.Map(ToRemoveList, new List<codeWorkingDaysConfigurations>());

            try
            {
                DbCMS.CreateBulk(add, CreateActionGUID, ExecutionTime);
                DbCMS.UpdateBulk(update, UpdateActionGUID, ExecutionTime);
                DbCMS.DeleteBulk(remove, ExecutionTime, DeleteActionGUID);
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(null, null, null, "LoadUrl('WorkingDaysBody','" + Url.Action("WorkingDaysConfigUpdate", "Configurations", new { DutyStationGUID = model.DutyStationsGUID, PK = model.OrganizationInstanceGUID }) + "')"));

            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }


        #endregion

        #region Gender Configuration
        public ActionResult GenderConfigDataTable(DataTableRecievedOptions options, Guid PK)
        {

            if (options.columns == null) return PartialView("~/Areas/CMS/Views/Configurations/Genders/_DataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            var Result = (from a in DbCMS.codeTablesValues.Where(x => x.TableGUID == LookupTables.Gender && x.Active)
                          join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN) on a.ValueGUID equals b.ValueGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()
                          from R2 in a.codeTablesValuesConfigurations.Where(x => x.OrganizationInstanceGUID == PK && x.ValueGUID == a.ValueGUID && x.Active)
                          orderby R1.ValueDescription
                          select new
                          {
                              R2.TableValueConfigurationGUID,
                              R2.OrganizationInstanceGUID,
                              R1.ValueDescription,
                              R2.codeTablesValuesConfigurationsRowVersion
                          }).ToList();

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GenderConfigCreate(Guid PK)
        {

            var Lookups = (from a in DbCMS.codeTablesValues.Where(x => x.TableGUID == LookupTables.Gender && x.Active)
                           join b in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ValueGUID equals b.ValueGUID
                           where !DbCMS.codeTablesValuesConfigurations.Where(x => x.OrganizationInstanceGUID == PK && x.Active).Select(x => x.codeTablesValues).Contains(a)
                           orderby b.ValueDescription
                           select new CheckBoxList
                           {
                               Value = a.ValueGUID.ToString(),
                               Description = b.ValueDescription
                           }).ToList();

            ConfigurationModel ConfigurationModel = new ConfigurationModel
            {
                ValueGuid = PK,
                CTID = LookupTables.Gender,
                CheckBoxList = Lookups,
            };
            return PartialView("~/Areas/CMS/Views/Configurations/Genders/_Modal.cshtml", ConfigurationModel);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult GenderConfigCreate(ConfigurationModel model)
        {
            //Guid ActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            DateTime ExecutionTime = DateTime.Now;

            List<string> mdList = model.CheckBoxList.Where(x => x.Checked == true).Select(x => x.Value).ToList();
            List<codeTablesValuesConfigurations> dbList = DbCMS.codeTablesValuesConfigurations.Where(x => x.OrganizationInstanceGUID == model.ValueGuid && x.codeTablesValues.TableGUID == model.CTID && x.Active).ToList();

            var toAddList = mdList.Where(s => !dbList.Select(x => x.ValueGUID.ToString()).Contains(s)).Select(s => new codeTablesValuesConfigurations
            {
                TableValueConfigurationGUID = Guid.NewGuid(),
                ValueGUID = Guid.Parse(s),
                OrganizationInstanceGUID = model.ValueGuid,
            }).ToList();

            DbCMS.CreateBulk(toAddList, Permissions.GenderConfigurations.CreateGuid, ExecutionTime);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(DataTableNames.GenderConfigDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return null;
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult GenderConfigDataTableDelete(List<codeTablesValuesConfigurations> models)
        {
            Guid DeleteActionGUID = Permissions.GenderConfigurations.RemoveGuid;
            DateTime ExecutionTime = DateTime.Now;

            List<codeTablesValuesConfigurations> DeletedTablesValues = new List<codeTablesValuesConfigurations>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbCMS.QueryBuilder(models, DeleteActionGUID, SubmitTypes.Delete, baseQuery);

            var Records = DbCMS.Database.SqlQuery<codeTablesValuesConfigurations>(query).ToList();
            foreach (var record in Records)
            {
                DeletedTablesValues.Add(DbCMS.Delete(record, ExecutionTime, DeleteActionGUID));
            }
            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.PartialDeleteMessage(DeletedTablesValues, models, DataTableNames.GenderConfigDataTable));
            }
            catch (Exception ex)

            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }
        #endregion

    }
}