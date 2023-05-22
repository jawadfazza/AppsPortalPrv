using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.ViewModels;
using AutoMapper;
using EMT_DAL.Model;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.EMT.Controllers
{
    public class MedicalItemInputSupplysController : EMTBaseController
    {
        #region Medical Item Inputs

        public ActionResult Index()
        {
            return View();
        }

        [Route("EMT/MedicalItemInputSupplys/")]
        public ActionResult MedicalItemInputSupplysIndex()
        {
            if (!CMS.HasAction(Permissions.MedicalItemInputSupply.Access, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/EMT/Views/MedicalItemInputSupplys/Index.cshtml");
        }

        [Route("EMT/MedicalItemInputSupplysDataTable/")]
        public JsonResult MedicalItemInputSupplysDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<MedicalItemInputSupplysDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<MedicalItemInputSupplysDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.MedicalPharmacy.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();

            var All = (from a in DbEMT.dataMedicalItemInputSupply
                      
                       select new MedicalItemInputSupplysDataTableModel
                       {
                           MedicalItemInputSupplyGUID = a.MedicalItemInputSupplyGUID,
                           DispatchDate = a.DispatchDate,
                           PurchaseOrder=a.PurchaseOrder,
                           SupplierName=a.SupplierName,
                           Active = a.Active,
                           dataMedicalItemInputSupplyRowVersion = a.dataMedicalItemInputSupplyRowVersion
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<MedicalItemInputSupplysDataTableModel> Result = Mapper.Map<List<MedicalItemInputSupplysDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult MedicalItemInputSupplyDetailsViewDataTable(DataTableRecievedOptions options)
        {
            if (options.columns == null) return PartialView("~/Areas/EMT/Views/MedicalItemInputSupplys/MedicalItemInputSupplyDetailsDataTable.cshtml");

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<MedicalItemInputSupplyDetailsDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<MedicalItemInputSupplyDetailsDataTableModel>(DataTable.Filters);
            }
            bool CostViewAuthorization = DbCMS.userServiceHistory.Where(x => x.UserGUID == UserGUID).FirstOrDefault().EmailAddress.ToUpper().EndsWith("UNHCR.ORG");

            var All = (from a in DbEMT.dataMedicalItemInputSupplyDetail.AsNoTracking().AsExpandable().Where(x =>  x.Active)
                       join b in DbEMT.codeMedicalItem on a.MedicalItemGUID equals b.MedicalItemGUID
                       select new MedicalItemInputSupplyDetailsDataTableModel
                       {
                           MedicalItemInputSupplyDetailGUID = a.MedicalItemInputSupplyDetailGUID,
                           BatchNumber = a.BatchNumber,
                           ExpirationDate = a.ExpirationDate,
                           PriceOfSmallestUnit = CostViewAuthorization ? a.PriceOfSmallestUnit : 0,
                           PriceOfPackingUnit = CostViewAuthorization ? a.PriceOfPackingUnit : 0,
                           QuantityByPackingUnit = a.QuantityByPackingUnit ,
                           QuantityBySmallestUnit = a.QuantityBySmallestUnit,
                           RemainingItems = a.RemainingItems,
                           MedicalItemGUID = a.MedicalItemGUID.ToString(),
                           BrandName = b.BrandName + ", " + b.DoseQuantity,
                           MedicalGenericNameDescription = b.codeMedicalGenericName.codeMedicalGenericNameLanguage.Where(x => x.LanguageID == LAN && x.Active).FirstOrDefault().MedicalGenericNameDescription.Replace("+", " "),
                           MedicalGenericNameGUID = b.codeMedicalGenericName.MedicalGenericNameGUID.ToString(),
                           ManufacturingDate = a.ManufacturingDate,
                           MedicalItemInputSupplyGUID = a.MedicalItemInputSupplyGUID,
                           DispatchDate = a.dataMedicalItemInputSupply.DispatchDate,
                           Active = a.Active,
                           dataMedicalItemInputSupplyDetailRowVersion = a.dataMedicalItemInputSupplyDetailRowVersion
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<MedicalItemInputSupplyDetailsDataTableModel> Result = Mapper.Map<List<MedicalItemInputSupplyDetailsDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult MedicalItemInputSupplyTransfer(Guid FK)
        {
            if (!CMS.HasAction(Permissions.MedicalItemInputSupply.Create, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var model = new MedicalItemInputUpdateModel()
            {
                medicalItemInputSupplyDetailsDataTableModels = (from a in DbEMT.dataMedicalItemInputSupplyDetail.Where(x => x.MedicalItemInputSupplyGUID == FK && x.Active)
                                                                join b in DbEMT.codeMedicalItem on a.MedicalItemGUID equals b.MedicalItemGUID
                                                                orderby b.BrandName
                                                                select
                   new MedicalItemInputSupplyDetailsDataTableModel
                   {
                       BrandName = b.BrandName,
                       MedicalItemInputSupplyDetailGUID = a.MedicalItemInputSupplyDetailGUID,
                       BatchNumber = a.BatchNumber,
                       QuantityByPackingUnit = 0,
                       RemainingItems=a.RemainingItems,
                       MedicalItemGUID=a.MedicalItemGUID.ToString()
                   }
                ).ToList()
            };
            return PartialView("~/Areas/EMT/Views/MedicalItemInputSupplys/_MedicalItemInputSupplyTransferUpdateModal.cshtml", model);

        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalItemInputSupplyTransferCreate(MedicalItemInputUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.MedicalItemInput.Create, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveMedicalItemInput(model)) return PartialView("~/Areas/EMT/Views/MedicalItemInputSupplys/_MedicalItemInputSupplyTransferUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

           

            dataMedicalItemInput MedicalItemInput = DbEMT.dataMedicalItemInput.Where(x => x.ProcuredByOrganizationInstanceGUID == model.ProcuredByOrganizationInstanceGUID && x.DeliveryDate == model.DeliveryDate && x.MedicalPharmacyGUID==model.MedicalPharmacyGUID  && x.Active).FirstOrDefault();
            Guid EntityPK = Guid.NewGuid();
            if (MedicalItemInput == null)
            {
                MedicalItemInput = Mapper.Map(model, new dataMedicalItemInput());
                MedicalItemInput.MedicalItemInputGUID = EntityPK;
                MedicalItemInput.ProvidedByOrganizationInstanceGUID = Guid.Parse("E156C022-EC72-4A5A-BE09-163BD85C68EF");
                DbEMT.Create(MedicalItemInput, Permissions.MedicalItemInput.CreateGuid, ExecutionTime, DbCMS);
            }
            else
            {
                 EntityPK = MedicalItemInput.MedicalItemInputGUID;
            }
            foreach (var item in model.medicalItemInputSupplyDetailsDataTableModels)
            {
                if (item.QuantityByPackingUnit != 0)
                {
                    var Source = DbEMT.dataMedicalItemInputSupplyDetail.Where(x => x.MedicalItemInputSupplyDetailGUID == item.MedicalItemInputSupplyDetailGUID).FirstOrDefault();
                    Source.RemainingItems = Source.RemainingItems - item.QuantityByPackingUnit;

                    dataMedicalItemInputDetail Destination = DbEMT.dataMedicalItemInputDetail.Where(x => x.MedicalItemGUID.ToString() == item.MedicalItemGUID&& x.BatchNumber == item.BatchNumber && x.PriceOfSmallestUnit == x.PriceOfSmallestUnit && x.Active && x.MedicalItemInputGUID==EntityPK).FirstOrDefault();
                    if (Destination == null)
                    {
                        Destination = new dataMedicalItemInputDetail();
                        Mapper.Map(Source, Destination);
                        Destination.QuantityByPackingUnit = item.QuantityByPackingUnit;
                        Destination.RemainingItems = item.QuantityByPackingUnit;
                        Destination.QuantityBySmallestUnit = Convert.ToInt32(item.QuantityByPackingUnit * (Source.QuantityBySmallestUnit / Source.QuantityByPackingUnit));

                        Destination.MedicalItemInputGUID = EntityPK;
                        DbEMT.Create(Destination, Permissions.MedicalItemInput.CreateGuid, ExecutionTime, DbCMS);
                    }
                    else
                    {
                        Destination.QuantityBySmallestUnit = Destination.QuantityBySmallestUnit + Convert.ToInt32(item.QuantityByPackingUnit * Destination.codeMedicalItem.PackingUnit.Value);
                        Destination.QuantityByPackingUnit = Destination.QuantityByPackingUnit + item.QuantityByPackingUnit;
                        Destination.RemainingItems = Destination.RemainingItems+ item.QuantityByPackingUnit;
                    }

                    //Check If the Item Quantity Threshold exist
                    dataItemQuantityThreshold itemQuantityThreshold = DbEMT.dataItemQuantityThreshold.Where(x => x.MedicalItemGUID.ToString() == item.MedicalItemGUID && x.MedicalPharmacyGUID == MedicalItemInput.MedicalPharmacyGUID && x.Active).FirstOrDefault();
                    if (itemQuantityThreshold == null)
                    {
                        itemQuantityThreshold = new dataItemQuantityThreshold();
                        itemQuantityThreshold.ItemQuantityThresholdGUID = Guid.NewGuid();
                        itemQuantityThreshold.MedicalItemGUID = Guid.Parse( item.MedicalItemGUID);
                        itemQuantityThreshold.QuantityThreshold = 0;
                        itemQuantityThreshold.QuantityTotalRemainingItems = item.QuantityByPackingUnit;
                        itemQuantityThreshold.MedicalPharmacyGUID = MedicalItemInput.MedicalPharmacyGUID.Value;
                        DbEMT.Create(itemQuantityThreshold, Permissions.MedicalItem.CreateGuid, ExecutionTime, DbCMS);
                        DbEMT.SaveChanges();
                    }
                    else
                    {
                        itemQuantityThreshold.QuantityTotalRemainingItems = itemQuantityThreshold.QuantityTotalRemainingItems + item.QuantityByPackingUnit;
                    }
                    //Update code Medical Item Remaining Items Quantity
                    var MedicalItem = DbEMT.codeMedicalItem.Where(x => x.MedicalItemGUID.ToString() == item.MedicalItemGUID).FirstOrDefault();
                    MedicalItem.RemainingItemsQuantity = MedicalItem.RemainingItemsQuantity+item.QuantityByPackingUnit;
                    itemQuantityThreshold.codeMedicalItem.TotalDispatchedItems = MedicalItem.TotalDispatchedItems + item.QuantityByPackingUnit;

                }
            }

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleCreateMessage(DbEMT.PrimaryKeyControl(MedicalItemInput), DbEMT.RowVersionControls(new List<dataMedicalItemInput> { MedicalItemInput }), null, "", null));
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        private bool ActiveMedicalItemInput(MedicalItemInputUpdateModel model)
        {
            bool valid = false;
            int index = 0;
            foreach(var item in model.medicalItemInputSupplyDetailsDataTableModels)
            {
                if (item.QuantityByPackingUnit > item.RemainingItems)
                {
                    ModelState.AddModelError("medicalItemInputSupplyDetailsDataTableModels["+ index + "].QuantityByPackingUnit", "Quantity of Packing Unit :"+ item.QuantityByPackingUnit + ", Grater Than Remaining Item"+ item.RemainingItems);
                    valid = true;
                }
                index++;
            }

            return valid;
        }

        [Route("EMT/MedicalItemInputSupplys/Create/")]
        public ActionResult MedicalItemInputSupplyCreate()
        {
            if (!CMS.HasAction(Permissions.MedicalItemInputSupply.Create, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/EMT/Views/MedicalItemInputSupplys/MedicalItemInputSupply.cshtml", new MedicalItemInputSupplyUpdateModel());
        }

        [Route("EMT/MedicalItemInputSupplys/Update/{PK}")]
        public ActionResult MedicalItemInputSupplyUpdate(Guid PK)
        {
            //if (!CMS.HasAction(Permissions.MedicalItemInputSupply.Access, Apps.EMT))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            var Audit = DbEMT.spAuditHistory(LAN, PK).OrderBy(x => x.ExecutionTime).FirstOrDefault();
            if (Audit == null)
            {
                Audit = DbEMT.spAuditHistoryOld("EN", PK).OrderBy(x => x.ExecutionTime).Select(x =>
                            new spAuditHistory_Result()
                            {
                                ExecutedBy = x.ExecutedBy,
                                ExecutionTime = x.ExecutionTime
                            }).FirstOrDefault();
            }
            var model = (from a in DbEMT.dataMedicalItemInputSupply.WherePK(PK)
                         select new MedicalItemInputSupplyUpdateModel
                         {
                             MedicalItemInputSupplyGUID = a.MedicalItemInputSupplyGUID,
                             DispatchDate = a.DispatchDate,
                             Comments=a.Comments,
                             PurchaseOrder=a.PurchaseOrder,
                             SupplierName=a.SupplierName,
                             Active = a.Active,
                             dataMedicalItemInputSupplyRowVersion = a.dataMedicalItemInputSupplyRowVersion,
                             CreatedBy = Audit.ExecutedBy,
                             CreatedOn = Audit.ExecutionTime,
                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("MedicalItemInputSupply", "MedicalItemInputSupplys", new { Area = "EMT" }));

            return View("~/Areas/EMT/Views/MedicalItemInputSupplys/MedicalItemInputSupply.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalItemInputSupplyCreate(MedicalItemInputSupplyUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.MedicalItemInputSupply.Create, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveMedicalItemInputSupply(model)) return PartialView("~/Areas/EMT/Views/MedicalItemInputSupplys/_MedicalItemInputSupplyForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            dataMedicalItemInputSupply MedicalItemInputSupply = Mapper.Map(model, new dataMedicalItemInputSupply());
            MedicalItemInputSupply.MedicalItemInputSupplyGUID = EntityPK;
            DbEMT.Create(MedicalItemInputSupply, Permissions.MedicalItemInputSupply.CreateGuid, ExecutionTime, DbCMS);


            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.MedicalItemInputSupplyDetailsDataTable, ControllerContext, "MedicalItemInputSupplyDetailsContainer"));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.MedicalItemInputSupply.Create, Apps.EMT, new UrlHelper(Request.RequestContext).Action("Create", "MedicalItemInputSupplys", new { Area = "EMT" })), Container = "MedicalItemInputSupplyFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.MedicalItemInputSupply.Update, Apps.EMT), Container = "MedicalItemInputSupplyFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.MedicalItemInputSupply.Delete, Apps.EMT), Container = "MedicalItemInputSupplyFormControls" });

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleCreateMessage(DbEMT.PrimaryKeyControl(MedicalItemInputSupply), DbEMT.RowVersionControls(new List<dataMedicalItemInputSupply> { MedicalItemInputSupply }), Partials, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalItemInputSupplyUpdate(MedicalItemInputSupplyUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.MedicalItemInputSupply.Update, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveMedicalItemInputSupply(model)) return PartialView("~/Areas/EMT/Views/MedicalItemInputSupplys/_MedicalItemInputSupplyForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            dataMedicalItemInputSupply MedicalItemInputSupply = Mapper.Map(model, new dataMedicalItemInputSupply());
            DbEMT.Update(MedicalItemInputSupply, Permissions.MedicalItemInputSupply.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleUpdateMessage(null, null, DbEMT.RowVersionControls(new List<dataMedicalItemInputSupply> { MedicalItemInputSupply })));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyMedicalItemInputSupply(model.MedicalItemInputSupplyGUID);
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalItemInputSupplyDelete(dataMedicalItemInputSupply model)
        {
            if (!CMS.HasAction(Permissions.MedicalItemInputSupply.Delete, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            List<dataMedicalItemInputSupply> DeletedMedicalItemInputSupply = DeleteMedicalItemInputSupplys(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.MedicalItemInputSupply.Restore, Apps.EMT), Container = "MedicalItemInputSupplyFormControls" });

            try
            {
                int CommitedRows = DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleDeleteMessage(CommitedRows, DeletedMedicalItemInputSupply.FirstOrDefault(), "MedicalItemInputSupplyDetailsContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyMedicalItemInputSupply(model.MedicalItemInputSupplyGUID);
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }

        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalItemInputSupplyRestore(dataMedicalItemInputSupply model)
        {
            if (!CMS.HasAction(Permissions.MedicalItemInputSupply.Restore, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveMedicalItemInputSupply(model))
            {
                return Json(DbEMT.RecordExists());
            }

            List<dataMedicalItemInputSupply> RestoredMedicalItemInputSupplys = RestoreMedicalItemInputSupplys(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.MedicalItemInputSupply.Create, Apps.EMT, new UrlHelper(Request.RequestContext).Action("MedicalItemInputSupplyCreate", "Configuration", new { Area = "EMT" })), Container = "MedicalItemInputSupplyFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.MedicalItemInputSupply.Update, Apps.EMT), Container = "MedicalItemInputSupplyFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.MedicalItemInputSupply.Delete, Apps.EMT), Container = "MedicalItemInputSupplyFormControls" });

            try
            {
                int CommitedRows = DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleRestoreMessage(CommitedRows, RestoredMedicalItemInputSupplys, DbEMT.PrimaryKeyControl(RestoredMedicalItemInputSupplys.FirstOrDefault()), Url.Action(DataTableNames.MedicalItemInputSupplyDetailsDataTable, Portal.GetControllerName(ControllerContext)), "MedicalItemInputSupplyDetailsContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyMedicalItemInputSupply(model.MedicalItemInputSupplyGUID);
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult MedicalItemInputSupplysDataTableDelete(List<dataMedicalItemInputSupply> models)
        {
            if (!CMS.HasAction(Permissions.MedicalItemInputSupply.Delete, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataMedicalItemInputSupply> DeletedMedicalItemInputSupplys = DeleteMedicalItemInputSupplys(models);

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.PartialDeleteMessage(DeletedMedicalItemInputSupplys, models, DataTableNames.MedicalItemInputSupplysDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult MedicalItemInputSupplysDataTableRestore(List<dataMedicalItemInputSupply> models)
        {
            if (!CMS.HasAction(Permissions.MedicalItemInputSupply.Restore, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataMedicalItemInputSupply> RestoredMedicalItemInputSupplys = RestoreMedicalItemInputSupplys(models);

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.PartialRestoreMessage(RestoredMedicalItemInputSupplys, models, DataTableNames.MedicalItemInputSupplysDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        private List<dataMedicalItemInputSupply> DeleteMedicalItemInputSupplys(List<dataMedicalItemInputSupply> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<dataMedicalItemInputSupply> DeletedMedicalItemInputSupplys = new List<dataMedicalItemInputSupply>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbEMT.QueryBuilder(models, Permissions.MedicalItemInputSupply.DeleteGuid, SubmitTypes.Delete, baseQuery);

            var Records = DbEMT.Database.SqlQuery<dataMedicalItemInputSupply>(query).ToList();
            foreach (var record in Records)
            {

                    DeletedMedicalItemInputSupplys.Add(DbEMT.Delete(record, ExecutionTime, Permissions.MedicalItemInputSupply.DeleteGuid, DbCMS));
                
            }

            var MedicalItemInputSupplyDetails = DeletedMedicalItemInputSupplys.SelectMany(a => a.dataMedicalItemInputSupplyDetail).Where(l => l.Active).ToList();
            foreach (var MedicalItemInputSupplyDetail in MedicalItemInputSupplyDetails)
            {
                DbEMT.Delete(MedicalItemInputSupplyDetail, ExecutionTime, Permissions.MedicalItemInputSupply.DeleteGuid, DbCMS);
            }
            return DeletedMedicalItemInputSupplys;
        }

        private List<dataMedicalItemInputSupply> RestoreMedicalItemInputSupplys(List<dataMedicalItemInputSupply> models)
        {

            DateTime RestoringTime = DateTime.Now;

            List<dataMedicalItemInputSupply> RestoredMedicalItemInputSupplys = new List<dataMedicalItemInputSupply>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbEMT.QueryBuilder(models, Permissions.MedicalItemInputSupply.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Records = DbEMT.Database.SqlQuery<dataMedicalItemInputSupply>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveMedicalItemInputSupply(record))
                {
                    RestoredMedicalItemInputSupplys.Add(DbEMT.Restore(record, Permissions.MedicalItemInputSupply.DeleteGuid, Permissions.MedicalItemInputSupply.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            var MedicalItemInputSupplyDetails = RestoredMedicalItemInputSupplys.SelectMany(x => x.dataMedicalItemInputSupplyDetail.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var MedicalItemInputSupplyDetail in MedicalItemInputSupplyDetails)
            {
                DbEMT.Restore(MedicalItemInputSupplyDetail, Permissions.MedicalItemInputSupply.DeleteGuid, Permissions.MedicalItemInputSupply.RestoreGuid, RestoringTime, DbCMS);

            }

            return RestoredMedicalItemInputSupplys;
        }

        private JsonResult ConcurrencyMedicalItemInputSupply(Guid PK)
        {
            MedicalItemInputSupplyUpdateModel dbModel = new MedicalItemInputSupplyUpdateModel();

            var MedicalItemInputSupply = DbEMT.dataMedicalItemInputSupply.Where(x => x.MedicalItemInputSupplyGUID == PK).FirstOrDefault();
            var dbMedicalItemInputSupply = DbEMT.Entry(MedicalItemInputSupply).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbMedicalItemInputSupply, dbModel);

            var MedicalItemInputSupplyDetail = DbEMT.dataMedicalItemInputSupplyDetail.Where(x => x.MedicalItemInputSupplyGUID == PK).Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.dataMedicalItemInputSupply.DeletedOn)).FirstOrDefault();
            var dbMedicalItemInputSupplyDetail = DbEMT.Entry(MedicalItemInputSupplyDetail).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbMedicalItemInputSupplyDetail, dbModel);

            if (MedicalItemInputSupply.dataMedicalItemInputSupplyRowVersion.SequenceEqual(dbModel.dataMedicalItemInputSupplyRowVersion))
            {
                return Json(DbEMT.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbEMT, dbModel, "MedicalItemInputSupplyDetailsContainer"));
        }

        private bool ActiveMedicalItemInputSupply(Object model)
        {
            return false;
        }

        #endregion

        #region  Medical Item Input Details

        //[Route("EMT/MedicalItemInputSupplyDetailsDataTable/{PK}")]
        public ActionResult MedicalItemInputSupplyDetailsDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/EMT/Views/MedicalItemInputSupplys/_MedicalItemInputSupplyDetailsDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<MedicalItemInputSupplyDetailsDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<MedicalItemInputSupplyDetailsDataTableModel>(DataTable.Filters);
            }

            var Result = (from a in  DbEMT.dataMedicalItemInputSupplyDetail.AsNoTracking().AsExpandable().Where(x =>  x.MedicalItemInputSupplyGUID == PK)
                          join b in DbEMT.codeMedicalItem on a.MedicalItemGUID equals b.MedicalItemGUID
                              select new MedicalItemInputSupplyDetailsDataTableModel
                              {
                                  MedicalItemInputSupplyDetailGUID=  a.MedicalItemInputSupplyDetailGUID,
                                  BatchNumber= a.BatchNumber,
                                  ExpirationDate= a.ExpirationDate,
                                  PriceOfSmallestUnit=a.PriceOfSmallestUnit,
                                  PriceOfPackingUnit=a.PriceOfPackingUnit,
                                  QuantityByPackingUnit=a.QuantityByPackingUnit,
                                  QuantityBySmallestUnit=a.QuantityBySmallestUnit,
                                  RemainingItems=a.RemainingItems,
                                  MedicalItemGUID=a.MedicalItemGUID.ToString(),
                                  BrandName =b.BrandName + ", "+ b.DoseQuantity,
                                  ManufacturingDate=a.ManufacturingDate,
                                  MedicalItemInputSupplyGUID=a.MedicalItemInputSupplyGUID,
                                  MedicalGenericNameDescription = b.codeMedicalGenericName.codeMedicalGenericNameLanguage.Where(x => x.LanguageID == LAN && x.Active).FirstOrDefault().MedicalGenericNameDescription.Replace("+", " "),

                                  Active = a.Active,
                                  dataMedicalItemInputSupplyDetailRowVersion=a.dataMedicalItemInputSupplyDetailRowVersion
                              }).Where(Predicate);

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult MedicalItemInputSupplyDetailCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.MedicalItemInputSupply.Create, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/EMT/Views/MedicalItemInputSupplys/_MedicalItemInputSupplyDetailsUpdateModal.cshtml",
                new dataMedicalItemInputSupplyDetail { MedicalItemInputSupplyGUID = FK });
        }

        public ActionResult MedicalItemInputSupplyDetailUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.MedicalItemInputSupply.Access, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/EMT/Views/MedicalItemInputSupplys/_MedicalItemInputSupplyDetailsUpdateModal.cshtml", DbEMT.dataMedicalItemInputSupplyDetail.Find(PK));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalItemInputSupplyDetailCreate(dataMedicalItemInputSupplyDetail model)
        {
            if (!CMS.HasAction(Permissions.MedicalItemInputSupply.Create, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveMedicalItemInputSupplyDetail(model)) return PartialView("~/Areas/EMT/Views/MedicalItemInputSupplys/_MedicalItemInputSupplyDetailsUpdateModal.cshtml", model);
            var Item = DbEMT.codeMedicalItem.Where(x => x.MedicalItemGUID == model.MedicalItemGUID).FirstOrDefault();
            DateTime ExecutionTime = DateTime.Now;
            model.MedicalItemInputSupplyDetailGUID = Guid.NewGuid();
           
            model.PriceOfPackingUnit = model.PriceOfSmallestUnit * Item.PackingUnit.Value;
            model.QuantityByPackingUnit = Convert.ToInt32( model.QuantityBySmallestUnit /  Item.PackingUnit.Value);
            model.RemainingItems = model.QuantityByPackingUnit;
            DbEMT.Create(model, Permissions.MedicalItemInputSupply.CreateGuid, ExecutionTime, DbCMS);           

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleUpdateMessage(DataTableNames.MedicalItemInputSupplyDetailsDataTable, DbEMT.PrimaryKeyControl(model), DbEMT.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalItemInputSupplyDetailUpdate(dataMedicalItemInputSupplyDetail model)
        {
            if (!CMS.HasAction(Permissions.MedicalItemInputSupply.Update, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveMedicalItemInputSupplyDetail(model)) return PartialView("~/Areas/EMT/Views/MedicalItemInputSupplys/_MedicalItemInputSupplyDetailsUpdateModal.cshtml", model);

            var MedicalItemInputSupply = DbEMT.dataMedicalItemInputSupply.Where(x => x.MedicalItemInputSupplyGUID == model.MedicalItemInputSupplyGUID).FirstOrDefault();
            var Item = DbEMT.codeMedicalItem.Where(x => x.MedicalItemGUID == model.MedicalItemGUID).FirstOrDefault();

            DateTime ExecutionTime = DateTime.Now;
            var itemSupplyDetails = DbEMT.dataMedicalItemInputSupplyDetail.Where(x => x.MedicalItemInputSupplyDetailGUID == model.MedicalItemInputSupplyDetailGUID && x.Active).ToList().FirstOrDefault();
            model.PriceOfPackingUnit = model.PriceOfSmallestUnit * Item.PackingUnit.Value;
            model.QuantityByPackingUnit = Convert.ToInt32(model.QuantityBySmallestUnit / Item.PackingUnit.Value);

            //in case the PackingUnit changed
            if (Convert.ToInt32(itemSupplyDetails.QuantityBySmallestUnit / Item.PackingUnit.Value) != itemSupplyDetails.QuantityByPackingUnit)
            {
                double changeCal = (itemSupplyDetails.QuantityBySmallestUnit / itemSupplyDetails.QuantityByPackingUnit) / Item.PackingUnit.Value;
                model.RemainingItems =(int) ((itemSupplyDetails.RemainingItems * changeCal) +
                    (model.QuantityByPackingUnit - itemSupplyDetails.QuantityByPackingUnit * changeCal));
            }
            //in case the quaintity changed and  PackingUnit still the same 
            if (( itemSupplyDetails.QuantityBySmallestUnit / itemSupplyDetails.QuantityByPackingUnit) == Item.PackingUnit.Value &&
                model.QuantityByPackingUnit!= itemSupplyDetails.QuantityByPackingUnit)
            {
                model.RemainingItems = itemSupplyDetails.RemainingItems + (model.QuantityByPackingUnit - itemSupplyDetails.QuantityByPackingUnit);
            }

            UpdateUnitPrice(model.PriceOfSmallestUnit,model.PriceOfPackingUnit, model.MedicalItemInputSupplyDetailGUID);
            UpdateUnitExpirationDate(model.ExpirationDate.Value, model.MedicalItemInputSupplyDetailGUID);
            DbEMT.Update(model, Permissions.MedicalItemInputSupply.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleUpdateMessage(DataTableNames.MedicalItemInputSupplyDetailsDataTable,
                    DbEMT.PrimaryKeyControl(model),
                    DbEMT.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyMedicalItemInputSupplyDetail(model.MedicalItemInputSupplyDetailGUID);
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }

        }

        private void UpdateUnitPrice(double PriceOfSmallestUnit , double  priceOfPackingUnit, Guid medicalItemInputSupplyDetailGUID)
        {
            var inputItems = DbEMT.dataMedicalItemInputDetail.Where(x => x.MedicalItemInputSupplyDetailGUID == medicalItemInputSupplyDetailGUID).ToList();
           foreach(var item in inputItems)
            {
                item.PriceOfPackingUnit = priceOfPackingUnit;
                item.PriceOfSmallestUnit = PriceOfSmallestUnit;

            }
            
        }

        private void UpdateUnitExpirationDate(DateTime date, Guid medicalItemInputSupplyDetailGUID)
        {
            var inputItems = DbEMT.dataMedicalItemInputDetail.Where(x => x.MedicalItemInputSupplyDetailGUID == medicalItemInputSupplyDetailGUID).ToList();
            foreach (var item in inputItems)
            {
                item.ExpirationDate = date;
            }

        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalItemInputSupplyDetailDelete(dataMedicalItemInputSupplyDetail model)
        {
            if (!CMS.HasAction(Permissions.MedicalItemInputSupply.Delete, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            var MedicalItemInputSupply = DbEMT.dataMedicalItemInputSupply.Where(x => x.MedicalItemInputSupplyGUID == model.MedicalItemInputSupplyGUID).FirstOrDefault();

            List<dataMedicalItemInputSupplyDetail> DeletedLanguages = DeleteMedicalItemInputSupplyDetails(new List<dataMedicalItemInputSupplyDetail> { model });

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleDeleteMessage(DeletedLanguages, DataTableNames.MedicalItemInputSupplyDetailsDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyMedicalItemInputSupplyDetail(model.MedicalItemInputSupplyDetailGUID);
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }

        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MedicalItemInputSupplyDetailRestore(dataMedicalItemInputSupplyDetail model)
        {
            if (!CMS.HasAction(Permissions.MedicalItemInputSupply.Restore, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveMedicalItemInputSupplyDetail(model))
            {
                return Json(DbEMT.RecordExists());
            }

            List<dataMedicalItemInputSupplyDetail> RestoredLanguages = RestoreMedicalItemInputSupplyDetails(Portal.SingleToList(model));

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.SingleRestoreMessage(RestoredLanguages, DataTableNames.MedicalItemInputSupplyDetailsDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyMedicalItemInputSupplyDetail(model.MedicalItemInputSupplyDetailGUID);
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult MedicalItemInputSupplyDetailsDataTableDelete(List<dataMedicalItemInputSupplyDetail> models)
        {
            if (!CMS.HasAction(Permissions.MedicalItemInputSupply.Delete, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            Guid MedicalItemInputSupplyDetailGUID = models.FirstOrDefault().MedicalItemInputSupplyDetailGUID;
            var MedicalItemInputSupply = DbEMT.dataMedicalItemInputSupply.Where(x => x.dataMedicalItemInputSupplyDetail.FirstOrDefault().MedicalItemInputSupplyDetailGUID == MedicalItemInputSupplyDetailGUID).FirstOrDefault();
          
                List<dataMedicalItemInputSupplyDetail> DeletedLanguages = DeleteMedicalItemInputSupplyDetails(models);
              
                try
                {
                    DbEMT.SaveChanges();
                    DbCMS.SaveChanges();
                    return Json(DbEMT.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.MedicalItemInputSupplyDetailsDataTable));
                }
                catch (Exception ex)
                {
                    return Json(DbEMT.ErrorMessage(ex.Message));
                }
           

        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult MedicalItemInputSupplyDetailsDataTableRestore(List<dataMedicalItemInputSupplyDetail> models)
        {
            if (!CMS.HasAction(Permissions.MedicalItemInputSupply.Restore, Apps.EMT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataMedicalItemInputSupplyDetail> RestoredLanguages = RestoreMedicalItemInputSupplyDetails(models);

            try
            {
                DbEMT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbEMT.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.MedicalItemInputSupplyDetailsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbEMT.ErrorMessage(ex.Message));
            }
        }

        private List<dataMedicalItemInputSupplyDetail> DeleteMedicalItemInputSupplyDetails(List<dataMedicalItemInputSupplyDetail> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<dataMedicalItemInputSupplyDetail> DeletedMedicalItemInputSupplyDetails = new List<dataMedicalItemInputSupplyDetail>();

            string query = DbEMT.QueryBuilder(models, Permissions.MedicalItemInputSupply.DeleteGuid, SubmitTypes.Delete, "");

            var MedicalItemInputSupplyDetails = DbEMT.Database.SqlQuery<dataMedicalItemInputSupplyDetail>(query).ToList();

            //var MedicalItemInputSupply = DbEMT.dataMedicalItemInputSupply.Where(x => x.MedicalItemInputSupplyGUID == models.FirstOrDefault().MedicalItemInputSupplyGUID).FirstOrDefault();
            foreach (var MedicalItemInputSupplyDetail in MedicalItemInputSupplyDetails)
            {
                DeletedMedicalItemInputSupplyDetails.Add(DbEMT.Delete(MedicalItemInputSupplyDetail, ExecutionTime, Permissions.MedicalItemInputSupply.DeleteGuid, DbCMS));
            }

            return DeletedMedicalItemInputSupplyDetails;
        }

        private List<dataMedicalItemInputSupplyDetail> RestoreMedicalItemInputSupplyDetails(List<dataMedicalItemInputSupplyDetail> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<dataMedicalItemInputSupplyDetail> RestoredLanguages = new List<dataMedicalItemInputSupplyDetail>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbEMT.QueryBuilder(models, Permissions.MedicalItemInputSupply.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var MedicalItemInputSupplyDetails = DbEMT.Database.SqlQuery<dataMedicalItemInputSupplyDetail>(query).ToList();

            //var MedicalItemInputSupply = DbEMT.dataMedicalItemInputSupply.Where(x => x.MedicalItemInputSupplyGUID == models.FirstOrDefault().MedicalItemInputSupplyGUID).FirstOrDefault();
            foreach (var MedicalItemInputSupplyDetail in MedicalItemInputSupplyDetails)
            {
                if (!ActiveMedicalItemInputSupplyDetail(MedicalItemInputSupplyDetail))
                {
                    RestoredLanguages.Add(DbEMT.Restore(MedicalItemInputSupplyDetail, Permissions.MedicalItemInputSupply.DeleteGuid, Permissions.MedicalItemInputSupply.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyMedicalItemInputSupplyDetail(Guid PK)
        {
            dataMedicalItemInputSupplyDetail dbModel = new dataMedicalItemInputSupplyDetail();

            var Language = DbEMT.dataMedicalItemInputSupplyDetail.Where(l => l.MedicalItemInputSupplyDetailGUID == PK).FirstOrDefault();
            var dbLanguage = DbEMT.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.dataMedicalItemInputSupplyDetailRowVersion.SequenceEqual(dbModel.dataMedicalItemInputSupplyDetailRowVersion))
            {
                return Json(DbEMT.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbEMT, dbModel, "MedicalItemInputSupplyDetailsContainer"));
        }

        private bool ActiveMedicalItemInputSupplyDetail(dataMedicalItemInputSupplyDetail model)
        {
            var Item = DbEMT.codeMedicalItem.Where(x => x.MedicalItemGUID == model.MedicalItemGUID).FirstOrDefault();
            int ItemInputDetail = DbEMT.dataMedicalItemInputSupplyDetail
                                  .Where(x => x.MedicalItemInputSupplyDetailGUID != model.MedicalItemInputSupplyDetailGUID &&
                                  x.MedicalItemInputSupplyGUID == model.MedicalItemInputSupplyGUID &&
                                              x.MedicalItemGUID == model.MedicalItemGUID &&
                                               x.BatchNumber == model.BatchNumber &&
                                               x.PriceOfSmallestUnit == model.PriceOfSmallestUnit &&
                                              x.Active).Count();
            if (ItemInputDetail > 0)
            {
                ModelState.AddModelError("MedicalItemInputSupplyDetailGUID", "Item already exists"); //From resource ?????? Amer  
            }
            if (Item.PackingUnit == 0)
            {
                ModelState.AddModelError("QuantityByPackingUnit", "Packing Unit = 0 Quantity Can't be calculated");
                ModelState.AddModelError("PriceOfPackingUnit", "Packing Unit = 0 Price Can't be calculated");
                ItemInputDetail++;
            }
            //dataMedicalItemInputSupply itemInputSupply = DbEMT.dataMedicalItemInputSupply.Where(x => x.MedicalItemInputSupplyGUID == model.MedicalItemInputSupplyGUID).FirstOrDefault();
            //double period  = (model.ExpirationDate.Value - model.ManufacturingDate.Value).TotalDays;
            //double periodRemaining = ( model.ExpirationDate.Value - itemInputSupply.DispatchDate).TotalDays;
            //double cal1 = (periodRemaining / period);
            //double cal2 = (2.0 / 3.0);
            //if (cal1 <= cal2)
            //{
            //    ModelState.AddModelError("ExpirationDate", "The Remaining Expire of Item Should Be  >=  2/3 of Expire Date");
            //    ItemInputDetail++;
            //}

            return (ItemInputDetail > 0);
        }

        #endregion

        #region  query 

        #endregion


        #region Transfer History
    
        public JsonResult LoadingWarehouseTransfer(Guid PK)
        {
            List<TransferDetailsDataTableModel> transfers = new List<TransferDetailsDataTableModel>();

            var Result = (from a in DbEMT.dataMedicalItemInputDetail.AsNoTracking().AsExpandable().Where(x => x.MedicalItemInputSupplyDetailGUID == PK && x.Active && x.QuantityByPackingTransfer == null)
                           join b in DbEMT.codeMedicalItem on a.MedicalItemGUID equals b.MedicalItemGUID
                           join c in DbEMT.codeMedicalPharmacyLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.dataMedicalItemInput.MedicalPharmacyGUID equals c.MedicalPharmacyGUID

                           select new TransferDetailsDataTableModel
                           {
                               QuantityByPackingUnit = a.QuantityByPackingUnit,
                               QuantityByPackingTransferUnit = a.QuantityByPackingUnit,
                               RemainingItems = a.RemainingItems.Value,
                               Pharmacy = c.MedicalPharmacyDescription,
                               DeliveryDate = a.dataMedicalItemInput.DeliveryDate.Value
                           }).ToList();

            transfers.AddRange(Result);
            var all = transfers.OrderBy(x => x.DeliveryDate).ToList();
            return Json(new { transfers = all }, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}