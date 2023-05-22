using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.ViewModels;
using AutoMapper;
using LinqKit;
using OfficeOpenXml;
using RMS_DAL.Model;
using SnmpSharpNet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.RMS.Controllers
{
    public class PrinterConfigurationController : RMSBaseController
    {

        #region Printer Configuration
        [Route("RMS/PrinterConfiguration")]
        public ActionResult PrinterConfigurationIndex()
        {
            return View("~/Areas/RMS/Views/PrinterConfigurations/Index.cshtml");
        }

        [Route("RMS/PrinterTree")]
        public ActionResult PrinterTreeIndex()
        {
            return View("~/Areas/RMS/Views/PrinterConfigurations/PrinterTree.cshtml");
        }

        [Route("RMS/PrinterConfiguration/PrintersConfigurationDataTable/")]
        public ActionResult PrintersConfigurationDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<PrintersDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<PrintersDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.PrintersConfiguration.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();
            var All = (from a in DbRMS.dataPrinterConfiguration.AsNoTracking().AsExpandable().Where(x=> AuthorizedList.Contains(x.OrganizationInstanceGUID+","+ x.DutyStationGUID))
                          join b in DbRMS.codeDutyStationsLanguages.AsNoTracking().AsExpandable().Where(x => x.Active && x.LanguageID == LAN) on a.DutyStationGUID equals b.DutyStationGUID
                          join c in DbRMS.codeOrganizationsInstancesLanguages.AsNoTracking().AsExpandable().Where(x => x.Active && x.LanguageID == LAN) on a.OrganizationInstanceGUID equals c.OrganizationInstanceGUID
                          select new PrintersDataTableModel
                          {
                              PrinterConfigurationGUID = a.PrinterConfigurationGUID,
                              PrinterModelGUID = a.PrinterModelGUID,
                              PrinterTypeGUID = a.PrinterTypeGUID.Value,
                              PrinterName = a.PrinterName,
                              IPAddress = a.IpAddress,
                              OrganizationInstanceGUID = a.OrganizationInstanceGUID,
                              DutyStationDescription = b.DutyStationDescription,
                              DutyStationGUID = a.DutyStationGUID,
                              FloorNumber = a.FloorNumber,
                              OfficeNumber = a.OfficeNumber,
                              Active = a.Active,
                              dataPrinterConfigurationRowVersion = a.dataPrinterConfigurationRowVersion

                          }).Where(Predicate);


            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<PrintersDataTableModel> Result = Mapper.Map<List<PrintersDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }


        [Route("RMS/PrinterConfiguration/Create")]
        public ActionResult PrinterConfigurationCreate()
        {
            return View("~/Areas/RMS/Views/PrinterConfigurations/PrinterConfiguration.cshtml", new PrintersConfigUpdateModel());
        }

        [Route("RMS/PrinterConfiguration/Update/{PK}")]
        public ActionResult PrinterConfigurationUpdate(Guid PK)
        {
            var model = (from a in DbRMS.dataPrinterConfiguration.WherePK(PK)
                         select new PrintersConfigUpdateModel
                         {
                             PrinterModelGUID=a.PrinterModelGUID,
                             PrinterTypeGUID=a.PrinterTypeGUID.Value,
                             PrinterConfigurationGUID = a.PrinterConfigurationGUID,
                             PrinterName = a.PrinterName,
                             IPAddress = a.IpAddress,
                             OrganizationInstanceGUID = a.OrganizationInstanceGUID,
                             DutyStationGUID = a.DutyStationGUID,
                             BuildingGUID=a.BuildingGUID,
                             FloorNumber = a.FloorNumber,
                             OfficeNumber = a.OfficeNumber,
                             Active = a.Active,
                             dataPrinterConfigurationRowVersion = a.dataPrinterConfigurationRowVersion,
                             
                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("RMS/PrinterConfiguration", "PrinterConfiguration", new { Area = "RMS" }));
            return View("~/Areas/RMS/Views/PrinterConfigurations/PrinterConfiguration.cshtml", model);
        }

        [Route("RMS/PrinterConfiguration/UpdateTree/{PK}")]
        public ActionResult PrinterConfigurationTreeUpdate(Guid PK)
        {
            var model = (from a in DbRMS.dataPrinterConfiguration.WherePK(PK)
                         select new PrintersConfigUpdateModel
                         {
                             PrinterModelGUID = a.PrinterModelGUID,
                             PrinterTypeGUID = a.PrinterTypeGUID.Value,
                             PrinterConfigurationGUID = a.PrinterConfigurationGUID,
                             PrinterName = a.PrinterName,
                             IPAddress = a.IpAddress,
                             OrganizationInstanceGUID = a.OrganizationInstanceGUID,
                             DutyStationGUID = a.DutyStationGUID,
                             BuildingGUID = a.BuildingGUID,
                             FloorNumber = a.FloorNumber,
                             OfficeNumber = a.OfficeNumber,
                             Active = a.Active,
                             dataPrinterConfigurationRowVersion = a.dataPrinterConfigurationRowVersion,

                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("RMS/PrinterConfiguration", "PrinterConfiguration", new { Area = "RMS" }));
            return PartialView("~/Areas/RMS/Views/PrinterConfigurations/_PrinterConfigurationTreeModel.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PrinterConfigurationCreate(PrintersConfigUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.PrintersConfiguration.Create, Apps.RMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActivePrintersConfiguration(model)) { return PartialView("~/Areas/RMS/Views/PrinterConfigurations/_PrinterConfigurationForm.cshtml", model); }

            DateTime ExecutionTime = DateTime.Now;
            Guid EntityPK = Guid.NewGuid();
            if (model.PrinterConfigurationGUID != Guid.Empty)
            {
                EntityPK = model.PrinterConfigurationGUID;
            }

            dataPrinterConfiguration dataPrinterConfiguration = Mapper.Map(model, new dataPrinterConfiguration());
            dataPrinterConfiguration.PrinterConfigurationGUID = EntityPK;

            dataPrinterConfiguration.PrinterConfigurationGUID = Guid.NewGuid();/////////////change to dynamic

            DbRMS.Create(dataPrinterConfiguration, Permissions.PrintersConfiguration.CreateGuid, ExecutionTime, DbCMS);

            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.PrinterOidsDataTable, ControllerContext, "PrinterOidsContainer"));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.PrintersConfiguration.Create, Apps.RMS, new UrlHelper(Request.RequestContext).Action("PrinterConfiguration/Create", "PrinterConfiguration", new { Area = "RMS" })), Container = "PrinterConfigurationFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.PrintersConfiguration.Update, Apps.RMS), Container = "PrinterConfigurationFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.PrintersConfiguration.Delete, Apps.RMS), Container = "PrinterConfigurationFormControls" });

            try
            {
                DbRMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbRMS.SingleCreateMessage(DbRMS.PrimaryKeyControl(dataPrinterConfiguration), DbRMS.RowVersionControls(Portal.SingleToList(dataPrinterConfiguration)), null, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbRMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PrinterConfigurationUpdate(PrintersConfigUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.PrintersConfiguration.Update, Apps.RMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActivePrintersConfiguration(model)) { return PartialView("~/Areas/RMS/Views/PrinterConfigurations/_PrinterConfigurationForm.cshtml", model); }

            DateTime ExecutionTime = DateTime.Now;
            dataPrinterConfiguration dataPrinterConfiguration = Mapper.Map(model, new dataPrinterConfiguration());

            DbRMS.Update(dataPrinterConfiguration, Permissions.PrintersConfiguration.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbRMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbRMS.SingleUpdateMessage(null, null, DbRMS.RowVersionControls(Portal.SingleToList(dataPrinterConfiguration))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return null;
                //return ConcrrencyStaffPhones(model.StaffSimGUID);
            }
            catch (Exception ex)
            {
                return Json(DbRMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PrinterConfigurationDelete(dataPrinterConfiguration model)
        {
            if (!CMS.HasAction(Permissions.PrintersConfiguration.Delete, Apps.RMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataPrinterConfiguration> DeletedPrintersConfiguration = DeletePrintersConfigurations(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.PrintersConfiguration.Restore, Apps.RMS), Container = "PrinterConfigurationFormControls" });

            try
            {
                int CommitedRows = DbRMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbRMS.SingleDeleteMessage(CommitedRows, DeletedPrintersConfiguration.FirstOrDefault(), "PrinterOidsContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyPrintersConfiguration(model.PrinterConfigurationGUID);
            }
            catch (Exception ex)
            {
                return Json(DbRMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PrinterConfigurationRestore(dataPrinterConfiguration model)
        {
            if (!CMS.HasAction(Permissions.PrintersConfiguration.Restore, Apps.RMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActivePrintersConfiguration(model))
            {
                return Json(DbRMS.RecordExists());
            }

            List<dataPrinterConfiguration> RestoredPrintersConfigurations = RestorePrintersConfigurations(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.PrintersConfiguration.Create, Apps.RMS, new UrlHelper(Request.RequestContext).Action("PrintersConfigurationCreate", "Configuration", new { Area = "RMS" })), Container = "PrinterConfigurationFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.PrintersConfiguration.Update, Apps.RMS), Container = "PrinterConfigurationFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.PrintersConfiguration.Delete, Apps.RMS), Container = "PrinterConfigurationFormControls" });

            try
            {
                int CommitedRows = DbRMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbRMS.SingleRestoreMessage(CommitedRows, RestoredPrintersConfigurations, DbRMS.PrimaryKeyControl(RestoredPrintersConfigurations.FirstOrDefault()), Url.Action(DataTableNames.PrinterOidsDataTable, Portal.GetControllerName(ControllerContext)), "PrinterOidsContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyPrintersConfiguration(model.PrinterConfigurationGUID);
            }
            catch (Exception ex)
            {
                return Json(DbRMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult PrintersConfigurationDataTableDelete(List<dataPrinterConfiguration> models)
        {
            if (!CMS.HasAction(Permissions.PrintersConfiguration.Delete, Apps.RMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataPrinterConfiguration> DeletedPrintersConfigurations = DeletePrintersConfigurations(models);

            try
            {
                DbRMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbRMS.PartialDeleteMessage(DeletedPrintersConfigurations, models, DataTableNames.PrintersConfigurationDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbRMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult PrintersConfigurationDataTableRestore(List<dataPrinterConfiguration> models)
        {
            if (!CMS.HasAction(Permissions.PrintersConfiguration.Restore, Apps.RMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataPrinterConfiguration> RestoredPrintersConfigurations = RestorePrintersConfigurations(models);

            try
            {
                DbRMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbRMS.PartialRestoreMessage(RestoredPrintersConfigurations, models, DataTableNames.PrintersConfigurationDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbRMS.ErrorMessage(ex.Message));
            }
        }

        private List<dataPrinterConfiguration> DeletePrintersConfigurations(List<dataPrinterConfiguration> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<dataPrinterConfiguration> DeletedPrintersConfigurations = new List<dataPrinterConfiguration>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbRMS.QueryBuilder(models, Permissions.PrintersConfiguration.DeleteGuid, SubmitTypes.Delete, baseQuery);

            var Records = DbRMS.Database.SqlQuery<dataPrinterConfiguration>(query).ToList();
            foreach (var record in Records)
            {
                DeletedPrintersConfigurations.Add(DbRMS.Delete(record, ExecutionTime, Permissions.PrintersConfiguration.DeleteGuid, DbCMS));
            }

            var PrinterOIDs = DeletedPrintersConfigurations.SelectMany(a => a.dataPrinterOID).Where(l => l.Active).ToList();
            foreach (var language in PrinterOIDs)
            {
                DbRMS.Delete(language, ExecutionTime, Permissions.PrintersConfiguration.DeleteGuid, DbCMS);
            }
            return DeletedPrintersConfigurations;
        }

        private List<dataPrinterConfiguration> RestorePrintersConfigurations(List<dataPrinterConfiguration> models)
        {

            DateTime RestoringTime = DateTime.Now;

            List<dataPrinterConfiguration> RestoredPrintersConfigurations = new List<dataPrinterConfiguration>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbRMS.QueryBuilder(models, Permissions.PrintersConfiguration.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Records = DbRMS.Database.SqlQuery<dataPrinterConfiguration>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActivePrintersConfiguration(record))
                {
                    RestoredPrintersConfigurations.Add(DbRMS.Restore(record, Permissions.PrintersConfiguration.DeleteGuid, Permissions.PrintersConfiguration.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            var PrinterOIDs = RestoredPrintersConfigurations.SelectMany(x => x.dataPrinterOID.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var language in PrinterOIDs)
            {
                DbRMS.Restore(language, Permissions.PrintersConfiguration.DeleteGuid, Permissions.PrintersConfiguration.RestoreGuid, RestoringTime, DbCMS);
            }

            return RestoredPrintersConfigurations;
        }

        private JsonResult ConcurrencyPrintersConfiguration(Guid PK)
        {
            PrintersConfigUpdateModel dbModel = new PrintersConfigUpdateModel();

            var PrintersConfiguration = DbRMS.dataPrinterConfiguration.Where(x => x.PrinterConfigurationGUID == PK).FirstOrDefault();
            var dbPrintersConfiguration = DbRMS.Entry(PrintersConfiguration).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbPrintersConfiguration, dbModel);

            var Language = DbRMS.dataPrinterOID.Where(x => x.PrinterOidGUID == PK).Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.dataPrinterConfiguration.DeletedOn)).FirstOrDefault();
            var dbLanguage = DbRMS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (PrintersConfiguration.dataPrinterConfigurationRowVersion.SequenceEqual(dbModel.dataPrinterConfigurationRowVersion) )
            {
                return Json(DbRMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbRMS, dbModel, "PrinterOidsContainer"));
        }

        private bool ActivePrintersConfiguration(Object model)
        {
            PrintersConfigUpdateModel PrintersConfiguration = Mapper.Map(model, new PrintersConfigUpdateModel());
            int PrintersConfigurationDescription = DbRMS.dataPrinterConfiguration
                                    .Where(x => x.PrinterConfigurationGUID!= PrintersConfiguration.PrinterConfigurationGUID &&
                                    x.IpAddress == PrintersConfiguration.IPAddress &&
                                                x.Active).Count();
            if (PrintersConfigurationDescription > 0)
            {
                ModelState.AddModelError("PrintersConfigurationDescription", "PrintersConfiguration is already exists");
            }
            return (PrintersConfigurationDescription > 0);
        }


        public JsonResult LoadingOID(Guid PK)
        {
            var Printer = DbRMS.dataPrinterConfiguration.Where(x => x.PrinterConfigurationGUID == PK).FirstOrDefault();
            SimpleSnmp snmp = new SimpleSnmp(Printer.IpAddress);
            if (!snmp.Valid)
            {
                Console.WriteLine("SNMP agent host name/ip address is invalid.");
            }
            IPAddress address = IPAddress.Parse(Printer.IpAddress);
            UdpTarget target = new UdpTarget(address, 161, 2000, 1);

            var Oid = DbRMS.codeOID.Where(x => x.PrinterModelGUID == Printer.PrinterModelGUID && x.PrinterTypeGUID == Printer.PrinterTypeGUID).ToList();
            // Pdu class used for all requests
            Pdu pdu = new Pdu(PduType.Get);
            Oid.ForEach(x => pdu.VbList.Add(x.OIDNumber)); 

            Dictionary<Oid, AsnType> result = snmp.Get(SnmpVersion.Ver2, pdu);
            List<OidsValue> oidsValues = new List<OidsValue>();
            if (result != null)
            {
               
                Console.WriteLine("No results received.");
                Oid.ForEach(x => oidsValues.Add(new OidsValue
                {
                    OIDDescription=x.OIDDescription,
                    OIDNumber=x.OIDNumber,
                    OidsVal= result[new Oid(x.OIDNumber)].ToString()
                }));

            }
            return Json(new { oidsValues = oidsValues }, JsonRequestBehavior.AllowGet);
        }
        class OidsValue
        {
            public string OIDNumber { get; set; }
            public string OIDDescription { get; set; }

            public string OidsVal { get; set; }
        }

        #endregion

        #region Printer OID

        //[Route("RMS/dataPrinterOIDsDataTable/{PK}")]
        public ActionResult PrinterOIDsDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/RMS/Views/PrinterConfigurations/_PrinterOidsDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<dataPrinterOID, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<dataPrinterOID>(DataTable.Filters);
            }

            var Result = DbRMS.dataPrinterOID.AsNoTracking().AsExpandable().Where(x => x.PrinterConfigurationGUID == PK).Where(Predicate)
                              .Select(x => new
                              {
                                  x.PrinterOidGUID,
                                  x.PrinterConfigurationGUID,
                                  x.codeOID.OIDDescription,
                                  x.codeOID.OIDNumber,
                                  x.Active,
                                  x.dataPrinterOIDRowVersion
                              });

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult PrinterOIDCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.PrintersConfiguration.Create, Apps.RMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/RMS/Views/PrinterConfigurations/_PrinterOidUpdateModal.cshtml",
                new dataPrinterOID { PrinterConfigurationGUID = FK });
        }

        public ActionResult PrinterOIDUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.PrintersConfiguration.Access, Apps.RMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/RMS/Views/PrinterConfigurations/_PrinterOidUpdateModal.cshtml", DbRMS.dataPrinterOID.Find(PK));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PrinterOIDCreate(dataPrinterOID model)
        {
            if (!CMS.HasAction(Permissions.PrintersConfiguration.Create, Apps.RMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActivedataPrinterOID(model)) return PartialView("~/Areas/RMS/Views/PrinterConfigurations/_PrinterOidUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbRMS.Create(model, Permissions.PrintersConfiguration.CreateGuid, ExecutionTime, DbCMS);

            try
            {
                DbRMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbRMS.SingleUpdateMessage(DataTableNames.PrinterOidsDataTable, DbRMS.PrimaryKeyControl(model), DbRMS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbRMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PrinterOIDUpdate(dataPrinterOID model)
        {
            if (!CMS.HasAction(Permissions.PrintersConfiguration.Update, Apps.RMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActivedataPrinterOID(model)) return PartialView("~/Areas/RMS/Views/PrinterConfigurations/_PrinterOidUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbRMS.Update(model, Permissions.PrintersConfiguration.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbRMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbRMS.SingleUpdateMessage(DataTableNames.PrinterOidsDataTable,
                    DbRMS.PrimaryKeyControl(model),
                    DbRMS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencydataPrinterOID(model.PrinterOidGUID);
            }
            catch (Exception ex)
            {
                return Json(DbRMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PrinterOIDDelete(dataPrinterOID model)
        {
            if (!CMS.HasAction(Permissions.PrintersConfiguration.Delete, Apps.RMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataPrinterOID> DeletedLanguages = DeletedataPrinterOIDs(new List<dataPrinterOID> { model });

            try
            {
                DbRMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbRMS.SingleDeleteMessage(DeletedLanguages, DataTableNames.PrinterOidsDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencydataPrinterOID(model.PrinterOidGUID);
            }
            catch (Exception ex)
            {
                return Json(DbRMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PrinterOIDRestore(dataPrinterOID model)
        {
            if (!CMS.HasAction(Permissions.PrintersConfiguration.Restore, Apps.RMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActivedataPrinterOID(model))
            {
                return Json(DbRMS.RecordExists());
            }

            List<dataPrinterOID> RestoredLanguages = RestoredataPrinterOIDs(Portal.SingleToList(model));

            try
            {
                DbRMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbRMS.SingleRestoreMessage(RestoredLanguages, DataTableNames.PrinterOidsDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencydataPrinterOID(model.PrinterOidGUID);
            }
            catch (Exception ex)
            {
                return Json(DbRMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult PrinterOIDsDataTableDelete(List<dataPrinterOID> models)
        {
            if (!CMS.HasAction(Permissions.PrintersConfiguration.Delete, Apps.RMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataPrinterOID> DeletedLanguages = DeletedataPrinterOIDs(models);

            try
            {
                DbRMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbRMS.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.PrinterOidsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbRMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult PrinterOIDsDataTableRestore(List<dataPrinterOID> models)
        {
            if (!CMS.HasAction(Permissions.PrintersConfiguration.Restore, Apps.RMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataPrinterOID> RestoredLanguages = RestoredataPrinterOIDs(models);

            try
            {
                DbRMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbRMS.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.PrinterOidsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbRMS.ErrorMessage(ex.Message));
            }
        }

        private List<dataPrinterOID> DeletedataPrinterOIDs(List<dataPrinterOID> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<dataPrinterOID> DeleteddataPrinterOIDs = new List<dataPrinterOID>();

            string query = DbRMS.QueryBuilder(models, Permissions.PrintersConfiguration.DeleteGuid, SubmitTypes.Delete, "");

            var languages = DbRMS.Database.SqlQuery<dataPrinterOID>(query).ToList();

            foreach (var language in languages)
            {
                DeleteddataPrinterOIDs.Add(DbRMS.Delete(language, ExecutionTime, Permissions.PrintersConfiguration.DeleteGuid, DbCMS));
            }

            return DeleteddataPrinterOIDs;
        }

        private List<dataPrinterOID> RestoredataPrinterOIDs(List<dataPrinterOID> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<dataPrinterOID> RestoredLanguages = new List<dataPrinterOID>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbRMS.QueryBuilder(models, Permissions.PrintersConfiguration.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Languages = DbRMS.Database.SqlQuery<dataPrinterOID>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActivedataPrinterOID(language))
                {
                    RestoredLanguages.Add(DbRMS.Restore(language, Permissions.PrintersConfiguration.DeleteGuid, Permissions.PrintersConfiguration.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencydataPrinterOID(Guid PK)
        {
            dataPrinterOID dbModel = new dataPrinterOID();

            var Language = DbRMS.dataPrinterOID.Where(l => l.PrinterOidGUID == PK).FirstOrDefault();
            var dbLanguage = DbRMS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.dataPrinterOIDRowVersion.SequenceEqual(dbModel.dataPrinterOIDRowVersion))
            {
                return Json(DbRMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbRMS, dbModel, "PrinterOidsContainer"));
        }

        private bool ActivedataPrinterOID(dataPrinterOID model)
        {
            int LanguageID = DbRMS.dataPrinterOID
                                  .Where(x =>
                                              x.OidGUID == model.OidGUID &&
                                              x.PrinterOidGUID != model.PrinterOidGUID &&
                                              x.Active).Count();
            if (LanguageID > 0)
            {
                ModelState.AddModelError("PrinterOIDGUID", "OID already exists"); //From resource ?????? Amer  
            }

            return (LanguageID > 0);
        }

        #endregion

        #region Printer Tree

        public class PrinterOIDs
        {
            public Guid OIDGUID { get; set; }
            public Guid PrinterGUID { get; set; }
            public string Value { get; set; }
            public Guid? Type { get; set; }
        }

        public class OIDType
        {
            public static Guid RunningTimeStatus = Guid.Parse("8D749720-9036-4D53-A90D-002E6621A001");
            public static Guid TrayStatus = Guid.Parse("8D749720-9036-4D53-A90D-002E6621A002");
            public static Guid CartridgeType = Guid.Parse("8D749720-9036-4D53-A90D-002E6621A003");
            public static Guid SupplyLevel = Guid.Parse("8D749720-9036-4D53-A90D-002E6621A004");
            public static Guid PrinterName = Guid.Parse("8D749720-9036-4D53-A90D-002E6621A005");
            public static Guid FunctionStatus = Guid.Parse("8D749720-9036-4D53-A90D-002E6621A006");
            public static Guid PaperCount = Guid.Parse("8D749720-9036-4D53-A90D-002E6621A007");
        }

        public ActionResult CheckPrinters()
        {
            using (var DbRMS = new RMSEntities())
            {
                DateTime ExcutionTime = DateTime.Now;
                var printers = DbRMS.dataPrinterConfiguration.Where(x => x.Active).ToList();

                foreach (var printer in printers)
                {
                    SimpleSnmp snmp = new SimpleSnmp(printer.IpAddress);
                    if (!snmp.Valid)
                    {
                        Console.WriteLine("SNMP agent host name/ip address is invalid.");
                    }
                    IPAddress address = IPAddress.Parse(printer.IpAddress);
                    UdpTarget target = new UdpTarget(address, 161, 2000, 1);

                    // Pdu class used for all requests
                    var Oids = DbRMS.codeOID.Where(x => x.PrinterTypeGUID == printer.PrinterTypeGUID && x.PrinterModelGUID == printer.PrinterModelGUID).ToList();

                    Pdu pdu = new Pdu(PduType.Get);
                    foreach (var Oid in Oids)
                    {
                        pdu.VbList.Add(Oid.OIDNumber); //sysDescr
                    }
                    Dictionary<Oid, AsnType> result = snmp.Get(SnmpVersion.Ver2, pdu);

                    if (result != null)
                    {
                        string OIDNumber = Oids.Where(x => x.OIDTypeGUID == OIDType.PrinterName & x.Active).FirstOrDefault().OIDNumber;

                        printer.PrinterName = result[new Oid(OIDNumber)].ToString();

                        var OidReferralStatus = DbRMS.dataOidReferralStatus.ToList();
                        var ReferralStatus = DbRMS.codeReferralStatus.Where(x => x.ApplicationGUID == Apps.RMS).ToList();

                        foreach (var Oid in Oids)
                        {                          
                           
                            try
                            {
                                if (Oid.IsImport)
                                {
                                    DbRMS.dataPrinterLog.Add(new dataPrinterLog
                                    {
                                        PrinterLogGUID = Guid.NewGuid(),
                                        LogDateTime = ExcutionTime,
                                        Active = true,
                                        OidGUID = Oid.OidGUID,
                                        PrinterConfigurationGUID = printer.PrinterConfigurationGUID,
                                        OidValue = result[new Oid(Oid.OIDNumber)].ToString()
                                    });
                                }
                            }
                            catch { }
                        }
                    }
                    DbRMS.SaveChanges();
                }
            }
            return new JsonResult { Data = true , JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult GetPrintersTree(Guid DutyStaionGUID)
        {
            return new JsonResult { Data = PermissionsTreeRowMaterials(DutyStaionGUID), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public object[] PermissionsTreeRowMaterials(Guid DutyStaionGUID)
        {
            Guid EmptyGUID = Guid.Empty;
            var dutystation = DbRMS.codeDutyStations.Where(x => x.DutyStationGUID == DutyStaionGUID).FirstOrDefault();
            var Offices = (from a in DbRMS.codeOfficesLanguages.Where(l => l.LanguageID == LAN && l.codeOffices.CountryGUID == dutystation.CountryGUID && l.Active)
                           join b in DbRMS.dataPrinterConfiguration.Where(x => x.Active && x.DutyStationGUID == DutyStaionGUID) on a.OfficeGUID equals b.BuildingGUID
                           orderby a.OfficeDescription
                           select new TreeNode
                           {
                               ID = a.OfficeGUID.ToString(),
                               Text = a.OfficeDescription,
                               ParentID = null
                           }).Distinct().ToList();

            var Floors = (from a in DbRMS.dataPrinterConfiguration.Distinct().Where(x => x.Active && x.DutyStationGUID == DutyStaionGUID && x.FloorNumber != "" )
                          join b in DbRMS.codeOfficesLanguages.Where(l => l.LanguageID == LAN && l.codeOffices.CountryGUID == dutystation.CountryGUID && l.Active) on a.BuildingGUID equals b.OfficeGUID
                          select new TreeNode
                          {
                              ID = "00000000-0000-0000-0000-00000000000" + a.FloorNumber + b.OfficeGUID.ToString(),
                              Text = a.FloorNumber + " - Floor",
                              ParentID = b.OfficeGUID.ToString(),

                          }).Distinct().ToList();

            var Printers = (from a in DbRMS.dataPrinterConfiguration.Where(x => x.Active && x.DutyStationGUID == DutyStaionGUID )
                            join b in DbRMS.codeOfficesLanguages.Where(l => l.LanguageID == LAN) on a.BuildingGUID equals b.OfficeGUID
                            select new TreeNode
                            {
                                ID = a.PrinterConfigurationGUID.ToString(),
                                Text = /*"<b>Ip Address : " + a.IpAddress +*/ ", Printer : " + a.PrinterName+", Room :" +a.OfficeNumber +( a.IsContainProblem.Value ? ", Status: Warning</b>" : "</b>"),
                                ParentID = "00000000-0000-0000-0000-00000000000" + a.FloorNumber + b.OfficeGUID.ToString(),

                            }).Distinct().ToList();

            DateTime MaxDatetime = (from d in DbRMS.dataPrinterLog   select d.LogDateTime).Max();
            var OIDs = (from a in DbRMS.dataPrinterConfiguration.Where(x => x.Active && x.DutyStationGUID == DutyStaionGUID)
                        from b in DbRMS.codeOID
                        where (a.PrinterTypeGUID == b.PrinterTypeGUID && a.PrinterModelGUID == b.PrinterModelGUID && b.Active)
                        from c in DbRMS.dataPrinterLog.Where(x => x.LogDateTime == MaxDatetime
                       && a.PrinterConfigurationGUID == x.PrinterConfigurationGUID
                       && b.OidGUID == x.OidGUID)
                        select new TreeNode
                        {
                            ID = b.OidGUID.ToString(),
                            Text = b.OIDDescription,
                            ParentID = a.PrinterConfigurationGUID.ToString(),
                            Value = c.OidValue,
                            Type = b.OIDTypeGUID.ToString()

                        }).Distinct().ToList();



            object[] R = new object[4];
            R[0] = Offices;
            R[1] = Floors;
            R[2] = Printers;
            R[3] = OIDs;

            return R;
        }
        public class TreeNode
        {
            public string ID { get; set; }
            public string Text { get; set; }
            public string ParentID { get; set; }
            public string Value { get; set; }
            public string Type { get; set; }
        }

        #endregion

    }
}