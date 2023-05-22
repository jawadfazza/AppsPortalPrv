using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.ViewModels;
using AutoMapper;
using FineUploader;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WMS_DAL.Model;
using WMS_DAL.ViewModels;

namespace AppsPortal.Areas.WMS.Controllers
{
    public class LicenseSubscriptionController : WMSBaseController
    {
        // GET: WMS/LicenseSubscription


        #region LicenseSubscriptionContract


        [Route("WMS/LicenseSubscription/")]
        public ActionResult Index()
        {
            if (!CMS.HasAction(Permissions.LicenseandSubscriptionContracts.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            return View("~/Areas/WMS/Views/LicenseSubscription/Index.cshtml");
        }


        [Route("WMS/WMSLicenseSubscriptionContractDataTable/")]
        public JsonResult WMSLicenseSubscriptionContractDataTable(DataTableRecievedOptions options)
        {

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<LicenseSubscriptionContractDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<LicenseSubscriptionContractDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.LicenseandSubscriptionContracts.CreateGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();
            //Fix
            var All = (

                from a in DbWMS.dataLicenseSubscriptionContract.Where(x => x.Active).AsExpandable()
                join b in DbWMS.codeTablesValuesLanguages.Where(x => (x.Active == true) && x.LanguageID == LAN) on a.ContractClassGUID equals b.ValueGUID into LJ1
                from R1 in LJ1.DefaultIfEmpty()
                join c in DbWMS.codeTablesValuesLanguages.Where(x => (x.Active == true) && x.LanguageID == LAN) on a.ContractTypeGUID equals c.ValueGUID into LJ2
                from R2 in LJ2.DefaultIfEmpty()

                join d in DbWMS.codeTablesValuesLanguages.Where(x => (x.Active == true) && x.LanguageID == LAN) on a.ContractCategoryGUID equals d.ValueGUID into LJ3
                from R3 in LJ3.DefaultIfEmpty()
                join e in DbWMS.codeDutyStationsLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.LocationGUID equals e.DutyStationGUID into LJ4
                from R4 in LJ4.DefaultIfEmpty()

                join f in DbWMS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.RemindDateType equals f.ValueGUID into LJ5
                from R5 in LJ5.DefaultIfEmpty()

                join g in DbWMS.codeBrandLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.BrandGUID equals g.BrandGUID into LJ6
                from R6 in LJ6.DefaultIfEmpty()

                join h in DbWMS.codeWMSVendor.Where(x => x.Active) on a.VendorGUID equals h.VendorGUID into LJ7
                from R7 in LJ7.DefaultIfEmpty()


                select new LicenseSubscriptionContractDataTableModel
                {
                    LicenseSubscriptionContractGUID = a.LicenseSubscriptionContractGUID,
                    Active = a.Active,
                    ContractClass = R1.ValueDescription,
                    ContractClassGUID = a.ContractClassGUID.ToString(),
                    ContractType = R2.ValueDescription,
                    ContractTypeGUID = a.ContractTypeGUID.ToString(),
                    ContractCategory = R3.ValueDescription,
                    ContractCategoryGUID = a.ContractCategoryGUID.ToString(),
                    VendorGUID = a.VendorGUID.ToString(),
                    VendorName = R7.VendorName,
                    LocationGUID = a.LocationGUID.ToString(),
                    Location = R4.DutyStationDescription,
                    PurchaseDate = a.PurchaseDate,
                    StartDate = a.StartDate,
                    ExpiryDate = a.ExpiryDate,
                    RemindDateType = R5.ValueDescription,
                    RemindValue = a.RemindValue,
                    NextRemindDate = a.NextRemindDate,
                    BrandGUID = R6.BrandGUID.ToString(),
                    BrandName = R6.BrandDescription,
                    LocalExchangeRate = a.LocalExchangeRate,
                    LocalCurrencyContractCost = a.LocalCurrencyContractCost,
                    DollarCurrencyContractCost = a.DollarCurrencyContractCost,
                    LatestPONumber = a.LatestPONumber,
                    FlowStatusGUID = a.FlowStatusGUID.ToString(),
                    LastFlowStatusName = a.LastFlowStatusName,
                    VendorInformation = a.VendorInformation,
                    ContractDescription = a.ContractDescription,
                    Comments = a.Comments,
                    ContractNumber = a.ContractNumber,
                    ContractCode = a.ContractCode,




                    dataLicenseSubscriptionContractRowVersion = a.dataLicenseSubscriptionContractRowVersion
                }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<LicenseSubscriptionContractDataTableModel> Result = Mapper.Map<List<LicenseSubscriptionContractDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }


        public ActionResult LicenseSubscriptionContractCreate()
        {
            if (!CMS.HasAction(Permissions.LicenseandSubscriptionContracts.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            return PartialView("~/Areas/WMS/Views/LicenseSubscription/_LicenseSubscriptionForm.cshtml", new LicenseSubscriptionContractUpdateModel { LicenseSubscriptionContractGUID = Guid.Empty });
        }

        [HttpPost]
        public ActionResult LicenseSubscriptionContractCreate(LicenseSubscriptionContractUpdateModel model)

        {
            if (!CMS.HasAction(Permissions.LicenseandSubscriptionContracts.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (!ModelState.IsValid || ActiveLicenseSubscriptionContract(model) || model.ContractNumber == null
                || model.ContractClassGUID == null
                || model.ContractCategoryGUID == null
                || model.ContractTypeGUID == null
                ) return PartialView("~/Areas/WMS/Views/LicenseSubscription/_LicenseSubscriptionForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            dataLicenseSubscriptionContract licence = Mapper.Map(model, new dataLicenseSubscriptionContract());
            licence.LicenseSubscriptionContractGUID = EntityPK;

            DbWMS.Create(licence, Permissions.LicenseandSubscriptionContracts.CreateGuid, ExecutionTime, DbCMS);




            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.WMSLicenseSubscriptionContractDataTable, ControllerContext, "WMSLicenseSubscriptionContractDataTable"));


            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.LicenseandSubscriptionContracts.Create, Apps.WMS, new UrlHelper(Request.RequestContext).Action("Create", "LicenseandSubscriptionContracts", new { Area = "WMS" })), Container = "LicenseandSubscriptionContractDetailFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.LicenseandSubscriptionContracts.Update, Apps.WMS), Container = "LicenseandSubscriptionContractDetailFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.LicenseandSubscriptionContracts.Delete, Apps.WMS), Container = "LicenseandSubscriptionContractDetailFormControls" });

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleUpdateMessage(DataTableNames.WMSLicenseSubscriptionContractDataTable, DbWMS.PrimaryKeyControl(licence), DbWMS.RowVersionControls(Portal.SingleToList(licence))));

            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }


        public ActionResult LicenseSubscriptionContractUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.LicenseandSubscriptionContracts.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            //var model = DbWMS.dataLicenseSubscriptionContract.Find(PK);
            var model = (from a in DbWMS.dataLicenseSubscriptionContract.WherePK(PK)
                             //join b in DbWMS.dataLicenseSubscriptionContractLanguage.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.dataLicenseSubscriptionContract.DeletedOn) && x.LanguageID == LAN)
                             //on a.LicenseSubscriptionContractGUID equals b.LicenseSubscriptionContractGUID into LJ1
                             //from R1 in LJ1.DefaultIfEmpty()
                         select new LicenseSubscriptionContractUpdateModel
                         {
                             LicenseSubscriptionContractGUID = a.LicenseSubscriptionContractGUID,
                             ContractClassGUID = a.ContractClassGUID,
                             ContractCategoryGUID = a.ContractCategoryGUID,
                             ContractTypeGUID = a.ContractTypeGUID,
                             LocationGUID = a.LocationGUID,
                             ContractNumber = a.ContractNumber,
                             PurchaseDate = a.PurchaseDate,
                             StartDate = a.StartDate,
                             ExpiryDate = a.ExpiryDate,
                             RemindDateType = a.RemindDateType,
                             RemindValue = a.RemindValue,
                             NextRemindDate = a.NextRemindDate,
                             LocalExchangeRate = a.LocalExchangeRate,
                             LocalCurrencyContractCost = a.LocalCurrencyContractCost,
                             DollarCurrencyContractCost = a.DollarCurrencyContractCost,
                             BrandGUID = a.BrandGUID,
                             LatestPONumber = a.LatestPONumber,
                             VendorGUID = a.VendorGUID,

                             VendorInformation = a.VendorInformation,
                             ContractDescription = a.ContractDescription,
                             Comments = a.Comments,
                             FlowStatusGUID = a.FlowStatusGUID,
                             LastFlowStatusName = a.LastFlowStatusName,

                             Active = a.Active,
                             dataLicenseSubscriptionContractRowVersion = a.dataLicenseSubscriptionContractRowVersion,

                         }).FirstOrDefault();

            if (model == null) throw new HttpException(401, "Unauthorized access");
            return View("~/Areas/WMS/Views/LicenseSubscription/LicenseSubscription.cshtml", model);



        }


        [HttpPost]
        public ActionResult LicenseSubscriptionContractUpdate(LicenseSubscriptionContractUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.LicenseandSubscriptionContracts.Update, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (!ModelState.IsValid) return PartialView("~/Areas/WMS/Views/LicenseSubscription/_LicenseSubscriptionForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            dataLicenseSubscriptionContract licence = Mapper.Map(model, new dataLicenseSubscriptionContract());
            DbWMS.Update(licence, Permissions.LicenseandSubscriptionContracts.UpdateGuid, ExecutionTime, DbCMS);




            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleUpdateMessage(DataTableNames.WarehouseModelEntryMovementsDataTable, DbWMS.PrimaryKeyControl(licence), DbWMS.RowVersionControls(Portal.SingleToList(licence))));

            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyLicenseSubscriptionContract(model.LicenseSubscriptionContractGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult LicenseSubscriptionContractDelete(dataLicenseSubscriptionContract model)
        {
            if (!CMS.HasAction(Permissions.LicenseandSubscriptionContracts.Delete, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            List<dataLicenseSubscriptionContract> DeletedLicenseandSubscriptionContract = DeleteItem(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.LicenseandSubscriptionContracts.Restore, Apps.WMS), Container = "ItemFormControls" });

            try
            {
                int CommitedRows = DbWMS.SaveChanges();
                DbWMS.SaveChanges();
                return Json(DbWMS.SingleDeleteMessage(CommitedRows, DeletedLicenseandSubscriptionContract.FirstOrDefault(), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyLicenseSubscriptionContract(model.LicenseSubscriptionContractGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ItemRestore(dataLicenseSubscriptionContract model)
        {
            if (!CMS.HasAction(Permissions.LicenseandSubscriptionContracts.Restore, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (ActiveLicenseSubscriptionContract(model))
            {
                return Json(DbWMS.RecordExists());
            }

            List<dataLicenseSubscriptionContract> RestoredLicenseandSubscriptionContract = RestoreItems(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.LicenseandSubscriptionContracts.Create, Apps.WMS, new UrlHelper(Request.RequestContext).Action("LicenseandSubscriptionContractCreate", "Configuration", new { Area = "WMS" })), Container = "LicenseandSubscriptionContractFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.LicenseandSubscriptionContracts.Update, Apps.WMS), Container = "LicenseandSubscriptionContractFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.LicenseandSubscriptionContracts.Delete, Apps.WMS), Container = "LicenseandSubscriptionContractFormControls" });

            try
            {
                int CommitedRows = DbWMS.SaveChanges();
                DbWMS.SaveChanges();
                return Json(DbWMS.SingleRestoreMessage(CommitedRows, RestoredLicenseandSubscriptionContract, DbWMS.PrimaryKeyControl(RestoredLicenseandSubscriptionContract.FirstOrDefault()), Url.Action(DataTableNames.WMSLicenseSubscriptionContractDataTable, Portal.GetControllerName(ControllerContext)), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyLicenseSubscriptionContract(model.LicenseSubscriptionContractGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult WMSLicenseSubscriptionContractDataTableDelete(List<dataLicenseSubscriptionContract> models)
        {
            if (!CMS.HasAction(Permissions.LicenseandSubscriptionContracts.Delete, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            List<dataLicenseSubscriptionContract> DeletedItem = DeleteItem(models);

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.PartialDeleteMessage(DeletedItem, models, DataTableNames.WMSLicenseSubscriptionContractDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ItemDataTableRestore(List<dataLicenseSubscriptionContract> models)
        {
            if (!CMS.HasAction(Permissions.LicenseandSubscriptionContracts.Restore, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            List<dataLicenseSubscriptionContract> RestoredLicenseandSubscriptionContract = DeleteItem(models);

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.PartialRestoreMessage(RestoredLicenseandSubscriptionContract, models, DataTableNames.WMSLicenseSubscriptionContractDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        private List<dataLicenseSubscriptionContract> DeleteItem(List<dataLicenseSubscriptionContract> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<dataLicenseSubscriptionContract> DeletedLicenseandSubscriptionContract = new List<dataLicenseSubscriptionContract>();
            //Fix
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = "SELECT WarehouseLicenseandSubscriptionContractGUID,CONVERT(varchar(50), WarehouseLicenseandSubscriptionContractGUID) as C2 ,dataLicenseSubscriptionContractModelRowVersion FROM code.dataLicenseSubscriptionContractModel where WarehouseLicenseandSubscriptionContractGUID in (" + string.Join(",", models.Select(x => "'" + x.WarehouseLicenseandSubscriptionContractGUID + "'").ToArray()) + ")";

            string query = DbWMS.QueryBuilder(models, Permissions.LicenseandSubscriptionContracts.DeleteGuid, SubmitTypes.Delete, "");

            var Records = DbWMS.Database.SqlQuery<dataLicenseSubscriptionContract>(query).ToList();
            foreach (var record in Records)
            {
                DeletedLicenseandSubscriptionContract.Add(DbWMS.Delete(record, ExecutionTime, Permissions.LicenseandSubscriptionContracts.DeleteGuid, DbCMS));
            }

            return DeletedLicenseandSubscriptionContract;
        }

        private List<dataLicenseSubscriptionContract> RestoreItems(List<dataLicenseSubscriptionContract> models)
        {

            DateTime RestoringTime = DateTime.Now;

            List<dataLicenseSubscriptionContract> RestoredItem = new List<dataLicenseSubscriptionContract>();
            //Fix
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = "SELECT WarehouseLicenseandSubscriptionContractGUID,CONVERT(varchar(50), WarehouseLicenseandSubscriptionContractGUID) as C2 ,dataLicenseSubscriptionContractModelRowVersion FROM code.dataLicenseSubscriptionContractModel where WarehouseLicenseandSubscriptionContractGUID in (" + string.Join(",", models.Select(x => "'" + x.WarehouseLicenseandSubscriptionContractGUID + "'").ToArray()) + ")";

            string query = DbWMS.QueryBuilder(models, Permissions.LicenseandSubscriptionContracts.DeleteGuid, SubmitTypes.Restore, "");

            var Records = DbWMS.Database.SqlQuery<dataLicenseSubscriptionContract>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveLicenseSubscriptionContract(record))
                {
                    RestoredItem.Add(DbWMS.Restore(record, Permissions.LicenseandSubscriptionContracts.DeleteGuid, Permissions.LicenseandSubscriptionContracts.RestoreGuid, RestoringTime, DbCMS));
                }
            }



            return RestoredItem;
        }

        private JsonResult ConcrrencyLicenseSubscriptionContract(Guid PK)
        {
            dataLicenseSubscriptionContract dbModel = new dataLicenseSubscriptionContract();

            var inputdetail = DbWMS.dataLicenseSubscriptionContract.Where(a => a.LicenseSubscriptionContractGUID == PK).FirstOrDefault();
            var dbinputdetail = DbWMS.Entry(inputdetail).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbinputdetail, dbModel);



            if (inputdetail.dataLicenseSubscriptionContractRowVersion.SequenceEqual(dbModel.dataLicenseSubscriptionContractRowVersion))
            {
                return Json(DbCMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbCMS, dbModel, "LanguagesContainer"));
        }

        private bool ActiveLicenseSubscriptionContract(Object model)
        {
            dataLicenseSubscriptionContract LicenseandSubscriptionContract = Mapper.Map(model, new dataLicenseSubscriptionContract());
            int ModelDescription = DbWMS.dataLicenseSubscriptionContract
                                    .Where(x => x.ContractNumber == LicenseandSubscriptionContract.ContractNumber &&

                                                x.Active).Count();
            if (ModelDescription > 0)
            {
                ModelState.AddModelError("ModelDescription", "Item Contract is already exists");
            }
            return (ModelDescription > 0);
        }

        #endregion

        #region PO History 


        //[Route("WMS/StaffBankAccountDataTable/{PK}")]
        public ActionResult WMSContractPOHistoryDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/WMS/Views/LicenseSubscription/_PurchaseOrderContractDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<WMSPurchaseOrderContractDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<WMSPurchaseOrderContractDataTableModel>(DataTable.Filters);
            }


            var Result = (from a in DbWMS.dataLicenseSubscriptionContractPO.AsExpandable().Where(x => x.Active && (x.LicenseSubscriptionContractGUID == PK))

                          join b in DbWMS.userPersonalDetailsLanguage.AsExpandable().Where(x => x.Active && x.LanguageID == LAN) on a.CreateByGUID equals b.UserGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()

                          join c in DbWMS.userPersonalDetailsLanguage.AsExpandable().Where(x => x.Active && x.LanguageID == LAN) on a.UpdateByGUID equals c.UserGUID into LJ2
                          from R2 in LJ2.DefaultIfEmpty()
                          select new WMSPurchaseOrderContractDataTableModel
                          {
                              LicenseSubscriptionContractPOGUID = a.LicenseSubscriptionContractPOGUID,
                              LicenseSubscriptionContractGUID = a.LicenseSubscriptionContractGUID.ToString(),
                              PONumber = a.PONumber,
                              StartDate = a.StartDate,

                              ExpiryDate = a.ExpiryDate,
                              PODescription = a.PODescription,
                              Price = a.Price,
                              CreateDate = a.CreateDate,
                              UpdateDate = a.UpdateDate,
                              Comments = a.Comments,
                              CreateBy = R1.FirstName + " " + R1.Surname,
                              UpdateBy = R2.FirstName + " " + R2.Surname,
                              Active = a.Active,
                              dataLicenseSubscriptionContractPORowVersion = a.dataLicenseSubscriptionContractPORowVersion

                          }).Where(Predicate);


            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult PurchaseOrderContractCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.LicenseandSubscriptionContracts.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            return PartialView("~/Areas/WMS/Views/LicenseSubscription/_PurchaseOrderContractUpdateModal.cshtml",
                new WMSPurchaseOrderContractUpdateModel { LicenseSubscriptionContractGUID = FK, LicenseSubscriptionContractPOGUID = Guid.Empty });
        }





        public ActionResult PurchaseOrderContractUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.LicenseandSubscriptionContracts.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            WMSPurchaseOrderContractUpdateModel model = DbWMS.dataLicenseSubscriptionContractPO.Where(x => x.LicenseSubscriptionContractPOGUID == PK).Select(f => new WMSPurchaseOrderContractUpdateModel
            {

                LicenseSubscriptionContractPOGUID = (Guid)f.LicenseSubscriptionContractPOGUID,
                LicenseSubscriptionContractGUID = f.LicenseSubscriptionContractGUID,
                PONumber = f.PONumber,
                StartDate = f.StartDate,
                ExpiryDate = f.ExpiryDate,
                PODescription = f.PODescription,
                Price = f.Price,
                CreateDate = f.CreateDate,
                CreateByGUID = f.CreateByGUID,
                UpdateByGUID = f.UpdateByGUID,
                UpdateDate = f.UpdateDate,
                Comments = f.Comments,

                Active = f.Active

            }).FirstOrDefault();
            return PartialView("~/Areas/WMS/Views/LicenseSubscription/_PurchaseOrderContractUpdateModal.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PurchaseOrderContractCreate(dataLicenseSubscriptionContractPO model)
        {
            if (!CMS.HasAction(Permissions.LicenseandSubscriptionContracts.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (!ModelState.IsValid || (model.PONumber == null || (model.StartDate >= model.ExpiryDate) || model.Price == null)) return PartialView("~/Areas/WMS/Views/LicenseSubscription/_PurchaseOrderContractUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;
            model.CreateDate = ExecutionTime;
            model.CreateByGUID = UserGUID;

            DbWMS.Create(model, Permissions.LicenseandSubscriptionContracts.CreateGuid, ExecutionTime, DbCMS);

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleUpdateMessage(DataTableNames.WMSContractPOHistoryDataTable, DbWMS.PrimaryKeyControl(model), DbWMS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PurchaseOrderContractUpdate(dataLicenseSubscriptionContractPO model)
        {
            if (!CMS.HasAction(Permissions.LicenseandSubscriptionContracts.Update, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (!ModelState.IsValid || (model.PONumber == null || (model.StartDate >= model.ExpiryDate) || model.Price == null)) return PartialView("~/Areas/WMS/Views/LicenseSubscription/_PurchaseOrderContractUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;
            model.UpdateByGUID = UserGUID;
            model.UpdateDate = ExecutionTime;

            DbWMS.Update(model, Permissions.LicenseandSubscriptionContracts.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleUpdateMessage(DataTableNames.WMSContractPOHistoryDataTable,
                    DbWMS.PrimaryKeyControl(model),
                    DbWMS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyPurchaseOrderContract((Guid)model.LicenseSubscriptionContractGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PurchaseOrderContractDelete(dataLicenseSubscriptionContractPO model)
        {
            if (!CMS.HasAction(Permissions.LicenseandSubscriptionContracts.Delete, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            List<dataLicenseSubscriptionContractPO> DeletedLanguages = DeletePurchaseOrderContract(new List<dataLicenseSubscriptionContractPO> { model });

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleDeleteMessage(DeletedLanguages, DataTableNames.WMSContractPOHistoryDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyPurchaseOrderContract((Guid)model.LicenseSubscriptionContractGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PurchaseOrderContractRestore(dataLicenseSubscriptionContractPO model)
        {
            if (!CMS.HasAction(Permissions.LicenseandSubscriptionContracts.Restore, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (ActivePurchaseOrderContract(model))
            {
                return Json(DbWMS.RecordExists());
            }

            List<dataLicenseSubscriptionContractPO> RestoredLanguages = RestorePurchaseOrderContract(Portal.SingleToList(model));

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleRestoreMessage(RestoredLanguages, DataTableNames.WMSContractPOHistoryDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyPurchaseOrderContract((Guid)model.LicenseSubscriptionContractGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult WMSContractPOHistoryDataTableDelete(List<dataLicenseSubscriptionContractPO> models)
        {
            if (!CMS.HasAction(Permissions.LicenseandSubscriptionContracts.Delete, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            List<dataLicenseSubscriptionContractPO> DeletedLanguages = DeletePurchaseOrderContract(models);

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.WMSContractPOHistoryDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult WMSPurchaseOrderContractDataTableModelRestore(List<dataLicenseSubscriptionContractPO> models)
        {
            if (!CMS.HasAction(Permissions.LicenseandSubscriptionContracts.Restore, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            List<dataLicenseSubscriptionContractPO> RestoredLanguages = RestorePurchaseOrderContract(models);

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.WMSContractPOHistoryDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        private List<dataLicenseSubscriptionContractPO> DeletePurchaseOrderContract(List<dataLicenseSubscriptionContractPO> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<dataLicenseSubscriptionContractPO> DeletedStaffBankAccount = new List<dataLicenseSubscriptionContractPO>();

            string query = DbWMS.QueryBuilder(models, Permissions.LicenseandSubscriptionContracts.DeleteGuid, SubmitTypes.Delete, "");

            var languages = DbWMS.Database.SqlQuery<dataLicenseSubscriptionContractPO>(query).ToList();

            foreach (var language in languages)
            {
                DeletedStaffBankAccount.Add(DbWMS.Delete(language, ExecutionTime, Permissions.LicenseandSubscriptionContracts.DeleteGuid, DbCMS));
            }

            return DeletedStaffBankAccount;
        }

        private List<dataLicenseSubscriptionContractPO> RestorePurchaseOrderContract(List<dataLicenseSubscriptionContractPO> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<dataLicenseSubscriptionContractPO> RestoredLanguages = new List<dataLicenseSubscriptionContractPO>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbWMS.QueryBuilder(models, Permissions.LicenseandSubscriptionContracts.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Languages = DbWMS.Database.SqlQuery<dataLicenseSubscriptionContractPO>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActivePurchaseOrderContract(language))
                {
                    RestoredLanguages.Add(DbWMS.Restore(language, Permissions.LicenseandSubscriptionContracts.DeleteGuid, Permissions.LicenseandSubscriptionContracts.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyPurchaseOrderContract(Guid PK)
        {
            dataLicenseSubscriptionContractPO dbModel = new dataLicenseSubscriptionContractPO();

            var Language = DbWMS.dataLicenseSubscriptionContractPO.Where(l => l.LicenseSubscriptionContractGUID == PK).FirstOrDefault();
            var dbLanguage = DbWMS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.dataLicenseSubscriptionContractPORowVersion.SequenceEqual(dbModel.dataLicenseSubscriptionContractPORowVersion))
            {
                return Json(DbWMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbWMS, dbModel, "LanguagesContainer"));
        }

        private bool ActivePurchaseOrderContract(dataLicenseSubscriptionContractPO model)
        {
            int LanguageID = DbWMS.dataLicenseSubscriptionContractPO
                                  .Where(x =>
                                              x.LicenseSubscriptionContractGUID == model.LicenseSubscriptionContractGUID &&
                                              x.PONumber == model.PONumber &&
                                              x.StartDate == model.StartDate &&

                                              x.Active).Count();
            if (LanguageID > 0)
            {
                //Fix
                ModelState.AddModelError("LanguageID", "Already Exist");
            }

            return (LanguageID > 0);
        }

        #region Upload

        [HttpGet]
        public ActionResult UploadGetPurchaseOrderDocuments(Guid id)
        {
            return PartialView("~/Areas/WMS/Views/LicenseSubscription/_PurchaseOrderUploadFile.cshtml",
                new dataLicenseSubscriptionContractPOFile { LicenseSubscriptionContractPOGUID = id });
        }


        [HttpPost]
        public FineUploaderResult UploadPurchaseOrderDocumentsCreate(FineUpload upload, Guid? _myPOGUID, Guid? _FileTypeGUID, string _Comments)
        {

            return new FineUploaderResult(true, new { path = UploadDocument(upload, _myPOGUID, _FileTypeGUID, _Comments), success = true });
        }

        public string UploadDocument(FineUpload upload, Guid? _myPOGUID, Guid? _FileTypeGUID, string _Comments)
        {
            var _stearm = upload.InputStream;
            DateTime ExecutionTime = DateTime.Now;
            //string _ext = Path.GetExtension(upload.FileName).Split('.')[1];
            dataLicenseSubscriptionContractPOFile uploadPO = new dataLicenseSubscriptionContractPOFile();
            uploadPO.LicenseSubscriptionContractPOFileGUID = Guid.NewGuid();
            uploadPO.LicenseSubscriptionContractPOGUID = _myPOGUID;
            uploadPO.FileTypeGUID = _FileTypeGUID;
            uploadPO.CreateByGUID = UserGUID;
            uploadPO.CreateDate = ExecutionTime;
            uploadPO.Comments = _Comments;
            //string FilePath = Server.MapPath("~/Areas/IMS/UploadedDocuments/" + documentUplod.ItemIntpuDetailUploadedDocumentGUID + _ext);

            string _ext = Path.GetExtension(upload.FileName).Split('.')[1];

            string FolderPath = Server.MapPath("~/Areas/WMS/UploadedDocuments/LicenseSubscription/");
            //Directory.CreateDirectory(FolderPath);
            //int LatestFileVersion = 0;
            //try { LatestFileVersion = (from a in DbPPA.PPAFileVersion.Where(x => x.PPAOriginalFileGUID == PPAOriginalFileGUID && x.FileActionByUserGUID == UserGUID) select a.FileVersion).Max(); } catch { }
            //if (LatestFileVersion == -1) LatestFileVersion = 0;



            string FilePath = FolderPath + "/" + uploadPO.LicenseSubscriptionContractPOFileGUID.ToString() + "." + _ext;

            using (var fileStream = System.IO.File.Create(FilePath))
            {
                upload.InputStream.Seek(0, SeekOrigin.Begin);
                upload.InputStream.CopyTo(fileStream);
            }


            uploadPO.FileExtension = _ext;
            uploadPO.CreateByGUID = UserGUID;
            uploadPO.CreateDate = DateTime.Now;


            //documentUplod.Comments = ItemInputDetailGUID;
            //documentUplod.CreatedByGUID = UserGUID;
            //documentUplod.CreatedDate = ExecutionTime;
            DbWMS.Create(uploadPO, Permissions.LicenseandSubscriptionContracts.CreateGuid, ExecutionTime, DbCMS);
            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
            }
            catch (Exception)
            {

                throw;
            }


            //Server.MapPath("~/Areas/IMS/temp/Standard_Operating_Procedure_for_issuing_ICT_equipment_to_staff" + DateTime.Now.ToBinary() + ".pdf");


            return "~/Areas/WMS/UploadedDocuments/LicenseSubscription/" + uploadPO.LicenseSubscriptionContractPOFileGUID + _ext;
        }

        #endregion

        #endregion PO   

        #region PO File History

        public ActionResult LicenseSubscriptionPOFIleHistoryIndex(Guid id)
        {
            if (!CMS.HasAction(Permissions.LicenseandSubscriptionContracts.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            return PartialView("~/Areas/WMS/Views/LicenseSubscription/_PurchaseOrderFileHistory.cshtml", new dataLicenseSubscriptionContractPOFile { LicenseSubscriptionContractPOGUID = id });
        }

        public ActionResult WMSContractPOFileUploadHistoryDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/WMS/Views/LicenseSubscription/_PurchaseOrderContractDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<WMSLicenseSubscriptionContractPOFileDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<WMSLicenseSubscriptionContractPOFileDataTableModel>(DataTable.Filters);
            }


            var Result = (from a in DbWMS.dataLicenseSubscriptionContractPOFile.AsExpandable().Where(x => x.Active && (x.LicenseSubscriptionContractPOGUID == PK))

                          join b in DbWMS.userPersonalDetailsLanguage.AsExpandable().Where(x => x.Active && x.LanguageID == LAN) on a.CreateByGUID equals b.UserGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()

                          join c in DbWMS.userPersonalDetailsLanguage.AsExpandable().Where(x => x.Active && x.LanguageID == LAN) on a.UpdateByGUID equals c.UserGUID into LJ2
                          from R2 in LJ2.DefaultIfEmpty()

                          join d in DbWMS.codeTablesValuesLanguages.AsExpandable().Where(x => x.Active && x.LanguageID == LAN && x.codeTablesValues.TableGUID == LookupTables.WarehouePOFileTypes) on a.FileTypeGUID equals d.ValueGUID into LJ3
                          from R3 in LJ3.DefaultIfEmpty()
                          select new WMSLicenseSubscriptionContractPOFileDataTableModel
                          {
                              LicenseSubscriptionContractPOFileGUID = a.LicenseSubscriptionContractPOFileGUID,
                              LicenseSubscriptionContractPOGUID = a.LicenseSubscriptionContractPOGUID.ToString(),
                              FileTypeGUID = a.FileTypeGUID.ToString(),
                              FileExtension = a.FileExtension,
                              ContractFileName = a.ContractFileName,
                              CreateDate = a.CreateDate,
                              FileType = R3.ValueDescription,




                              UpdateDate = a.UpdateDate,
                              Comments = a.Comments,
                              CreateBy = R1.FirstName + " " + R1.Surname,
                              UpdateBy = R2.FirstName + " " + R2.Surname,
                              Active = a.Active,
                              dataLicenseSubscriptionContractPOFileRowVersion = a.dataLicenseSubscriptionContractPOFileRowVersion

                          }).Where(Predicate);


            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult DownloadPOFileDocument(Guid id)
        {



            var model = DbWMS.dataLicenseSubscriptionContractPOFile.Where(x => x.LicenseSubscriptionContractPOFileGUID == id).FirstOrDefault();
            var fullPath = model.LicenseSubscriptionContractPOFileGUID + "." + model.FileExtension;


            string sourceFile = Server.MapPath("~/Areas/WMS/UploadedDocuments/LicenseSubscription/" + fullPath);


            byte[] fileBytes = System.IO.File.ReadAllBytes(sourceFile);

            string fileName = DateTime.Now.ToString("yyMMdd") + model.LicenseSubscriptionContractPOFileGUID + "." + model.FileExtension;
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);

            // return Json(DbCMS.SingleUpdateMessage(null, null, null, null, "Please Wait...."));
        }
        #endregion

        #region STI ITem likned 


        //[Route("WMS/StaffBankAccountDataTable/{PK}")]
        public ActionResult WMSContractSTIItemsDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/WMS/Views/LicenseSubscription/_STIItemDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<WMSContractSTIItemDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<WMSContractSTIItemDataTableModel>(DataTable.Filters);
            }


            var Result = (from a in DbWMS.dataLicenseSubscriptionContractSTIItem.AsExpandable().Where(x => x.Active && (x.LicenseSubscriptionContractGUID == PK))

                          join b in DbWMS.userPersonalDetailsLanguage.AsExpandable().Where(x => x.Active && x.LanguageID == LAN) on a.CreateByGUID equals b.UserGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()

                          join c in DbWMS.userPersonalDetailsLanguage.AsExpandable().Where(x => x.Active && x.LanguageID == LAN) on a.UpdateByGUID equals c.UserGUID into LJ2
                          from R2 in LJ2.DefaultIfEmpty()
                          select new WMSContractSTIItemDataTableModel
                          {
                              LicenseSubscriptionContractSTIItemGUID = a.LicenseSubscriptionContractSTIItemGUID,
                              LicenseSubscriptionContractGUID = a.LicenseSubscriptionContractGUID.ToString(),
                              ItemInputDetailGUID = a.ItemInputDetailGUID.ToString(),
                              LicenseSubscriptionContractPOGUID = a.LicenseSubscriptionContractPOGUID.ToString(),
                              ModelName = a.ModelName,
                              BarcodeNumber = a.BarcodeNumber,
                              SerialNumber = a.SerialNumber,
                              GSMNumber = a.GSMNumber,


                              //PONumber = a.PONumber,
                              StartDate = a.StartDate,

                              ExpiryDate = a.ExpiryDate,

                              CreateDate = a.CreateDate,
                              UpdateDate = a.UpdateDate,
                              Comments = a.Comments,
                              CreateBy = R1.FirstName + " " + R1.Surname,
                              UpdateBy = R2.FirstName + " " + R2.Surname,
                              Active = a.Active,
                              dataLicenseSubscriptionContractSTIItemRowVersion = a.dataLicenseSubscriptionContractSTIItemRowVersion

                          }).Where(Predicate);


            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ContractSTIItemsCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.LicenseandSubscriptionContracts.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            return PartialView("~/Areas/WMS/Views/LicenseSubscription/_STIItemUpdateModal.cshtml",
                new WMSContractSTIItemUpdateModel { LicenseSubscriptionContractGUID = FK, LicenseSubscriptionContractSTIItemGUID = Guid.Empty });
        }





        public ActionResult ContractSTIItemsUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.LicenseandSubscriptionContracts.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            WMSContractSTIItemUpdateModel model = DbWMS.dataLicenseSubscriptionContractSTIItem.Where(x => x.LicenseSubscriptionContractSTIItemGUID == PK).Select(f => new WMSContractSTIItemUpdateModel
            {

                LicenseSubscriptionContractSTIItemGUID = (Guid)f.LicenseSubscriptionContractSTIItemGUID,
                LicenseSubscriptionContractGUID = f.LicenseSubscriptionContractGUID,
                ItemInputDetailGUID = f.ItemInputDetailGUID,
                ModelName = f.ModelName,
                BarcodeNumber = f.BarcodeNumber,
                SerialNumber = f.SerialNumber,
                GSMNumber = f.GSMNumber,
                LicenseSubscriptionContractPOGUID = f.LicenseSubscriptionContractPOGUID,

                StartDate = f.StartDate,
                ExpiryDate = f.ExpiryDate,

                CreateDate = f.CreateDate,
                CreateByGUID = f.CreateByGUID,
                UpdateByGUID = f.UpdateByGUID,
                UpdateDate = f.UpdateDate,
                Comments = f.Comments,

                Active = f.Active

            }).FirstOrDefault();
            return PartialView("~/Areas/WMS/Views/LicenseSubscription/_STIItemUpdateModal.cshtml", model);
        }

        [HttpPost]
        public ActionResult ContractSTIItemsCreate(WMSContractSTIItemUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.LicenseandSubscriptionContracts.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (!ModelState.IsValid || model.ItemInputDetailGUID == null || model.ItemInputDetailGUID == Guid.Empty) return PartialView("~/Areas/WMS/Views/LicenseSubscription/_STIItemUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            dataLicenseSubscriptionContractSTIItem myModel = Mapper.Map(model, new dataLicenseSubscriptionContractSTIItem());
            myModel.CreateByGUID = UserGUID;
            var check = DbWMS.v_EntryMovementDataTable.Where(x => x.ItemInputDetailGUID == model.ItemInputDetailGUID).FirstOrDefault();
            myModel.CreateDate = ExecutionTime;
            myModel.ModelName = check.ModelDescription;
            myModel.BarcodeNumber = check.BarcodeNumber;
            myModel.SerialNumber = check.SerialNumber;

            DbWMS.Create(myModel, Permissions.LicenseandSubscriptionContracts.CreateGuid, ExecutionTime, DbCMS);

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleUpdateMessage(DataTableNames.WMSContractSTIItemsDataTable, DbWMS.PrimaryKeyControl(myModel), DbWMS.RowVersionControls(Portal.SingleToList(myModel))));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost]
        public ActionResult ContractSTIItemsUpdate(WMSContractSTIItemUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.LicenseandSubscriptionContracts.Update, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (!ModelState.IsValid || model.ItemInputDetailGUID == null || model.ItemInputDetailGUID == Guid.Empty) return PartialView("~/Areas/WMS/Views/LicenseSubscription/_STIItemUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;
            dataLicenseSubscriptionContractSTIItem myModel = Mapper.Map(model, new dataLicenseSubscriptionContractSTIItem());
            myModel.UpdateByGUID = UserGUID;
            myModel.UpdateDate = ExecutionTime;


            DbWMS.Update(myModel, Permissions.LicenseandSubscriptionContracts.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleUpdateMessage(DataTableNames.WMSContractSTIItemsDataTable,
                    DbWMS.PrimaryKeyControl(myModel),
                    DbWMS.RowVersionControls(Portal.SingleToList(myModel))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyContractSTIItems((Guid)model.LicenseSubscriptionContractGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ContractSTIItemsDelete(dataLicenseSubscriptionContractSTIItem model)
        {
            if (!CMS.HasAction(Permissions.LicenseandSubscriptionContracts.Delete, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            List<dataLicenseSubscriptionContractSTIItem> DeletedLanguages = DeleteContractSTIItems(new List<dataLicenseSubscriptionContractSTIItem> { model });

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleDeleteMessage(DeletedLanguages, DataTableNames.WMSContractSTIItemsDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyPurchaseOrderContract((Guid)model.LicenseSubscriptionContractGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ContractSTIItemsRestore(dataLicenseSubscriptionContractSTIItem model)
        {
            if (!CMS.HasAction(Permissions.LicenseandSubscriptionContracts.Restore, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (ActiveContractSTIItems(model))
            {
                return Json(DbWMS.RecordExists());
            }

            List<dataLicenseSubscriptionContractSTIItem> RestoredLanguages = RestoreContractSTIItems(Portal.SingleToList(model));

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleRestoreMessage(RestoredLanguages, DataTableNames.WMSContractSTIItemsDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyPurchaseOrderContract((Guid)model.LicenseSubscriptionContractGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ContractSTIItemsDataTableDelete(List<dataLicenseSubscriptionContractSTIItem> models)
        {
            if (!CMS.HasAction(Permissions.LicenseandSubscriptionContracts.Delete, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            List<dataLicenseSubscriptionContractSTIItem> DeletedLanguages = DeleteContractSTIItems(models);

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.WMSContractSTIItemsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ContractSTIItemsDataTableModelRestore(List<dataLicenseSubscriptionContractSTIItem> models)
        {
            if (!CMS.HasAction(Permissions.LicenseandSubscriptionContracts.Restore, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            List<dataLicenseSubscriptionContractSTIItem> RestoredLanguages = RestoreContractSTIItems(models);

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.WMSContractSTIItemsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        private List<dataLicenseSubscriptionContractSTIItem> DeleteContractSTIItems(List<dataLicenseSubscriptionContractSTIItem> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<dataLicenseSubscriptionContractSTIItem> DeletedStaffBankAccount = new List<dataLicenseSubscriptionContractSTIItem>();

            string query = DbWMS.QueryBuilder(models, Permissions.LicenseandSubscriptionContracts.DeleteGuid, SubmitTypes.Delete, "");

            var languages = DbWMS.Database.SqlQuery<dataLicenseSubscriptionContractSTIItem>(query).ToList();

            foreach (var language in languages)
            {
                DeletedStaffBankAccount.Add(DbWMS.Delete(language, ExecutionTime, Permissions.LicenseandSubscriptionContracts.DeleteGuid, DbCMS));
            }

            return DeletedStaffBankAccount;
        }

        private List<dataLicenseSubscriptionContractSTIItem> RestoreContractSTIItems(List<dataLicenseSubscriptionContractSTIItem> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<dataLicenseSubscriptionContractSTIItem> RestoredLanguages = new List<dataLicenseSubscriptionContractSTIItem>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbWMS.QueryBuilder(models, Permissions.LicenseandSubscriptionContracts.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Languages = DbWMS.Database.SqlQuery<dataLicenseSubscriptionContractSTIItem>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveContractSTIItems(language))
                {
                    RestoredLanguages.Add(DbWMS.Restore(language, Permissions.LicenseandSubscriptionContracts.DeleteGuid, Permissions.LicenseandSubscriptionContracts.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyContractSTIItems(Guid PK)
        {
            dataLicenseSubscriptionContractSTIItem dbModel = new dataLicenseSubscriptionContractSTIItem();

            var Language = DbWMS.dataLicenseSubscriptionContractSTIItem.Where(l => l.LicenseSubscriptionContractGUID == PK).FirstOrDefault();
            var dbLanguage = DbWMS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.dataLicenseSubscriptionContractSTIItemRowVersion.SequenceEqual(dbModel.dataLicenseSubscriptionContractSTIItemRowVersion))
            {
                return Json(DbWMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbWMS, dbModel, "LanguagesContainer"));
        }

        private bool ActiveContractSTIItems(dataLicenseSubscriptionContractSTIItem model)
        {
            int LanguageID = DbWMS.dataLicenseSubscriptionContractSTIItem
                                  .Where(x =>
                                              x.LicenseSubscriptionContractGUID == model.LicenseSubscriptionContractGUID &&
                                              x.ItemInputDetailGUID == model.ItemInputDetailGUID &&
                                              x.StartDate == model.StartDate &&

                                              x.Active).Count();
            if (LanguageID > 0)
            {
                //Fix
                ModelState.AddModelError("LanguageID", "Already Exist");
            }

            return (LanguageID > 0);
        }
        public ActionResult GetSTIITemInformation(Guid ItemInputDetailGUID)
        {
            WMSContractSTIItemUpdateModel model = new WMSContractSTIItemUpdateModel();
            var iteminfo = DbWMS.v_EntryMovementDataTable.Where(x => x.ItemInputDetailGUID == ItemInputDetailGUID).FirstOrDefault();
            model.ItemInputDetailGUID = ItemInputDetailGUID;
            model.BarcodeNumber = iteminfo.BarcodeNumber;
            model.SerialNumber = iteminfo.SerialNumber;
            model.ModelName = iteminfo.ModelDescription;



            return PartialView("~/Areas/WMS/Views/LicenseSubscription/_ItemDetail.cshtml", model);
        }

        #endregion PO   
    }
}