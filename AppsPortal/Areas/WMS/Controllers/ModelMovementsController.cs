using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.Library.MimeDetective;
using AppsPortal.Models;
using AppsPortal.ViewModels;
using AutoMapper;
using FineUploader;
using LinqKit;
using Microsoft.Ajax.Utilities;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using RES_Repo.Globalization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using WMS_DAL.Model;
using WMS_DAL.ViewModels;

namespace AppsPortal.Areas.WMS.Controllers
{
    public class ModelMovementsController : WMSBaseController
    {
        #region Models Entry Movements 
        //Model Entry to track items entry 

        [Route("WMS/ModelEntryMovements/")]
        public ActionResult ModelEntryMovementIndex()
        {

            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Access, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            return View("~/Areas/WMS/Views/ModelEntryMovements/Index.cshtml");
        }

        [Route("WMS/WarehouseModelEntryMovementsDataTable/")]
        public JsonResult WarehouseModelEntryMovementsDataTable(DataTableRecievedOptions options)
        {
            var e = Request;

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<WarehouseModelEntryMovementDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<WarehouseModelEntryMovementDataTableModel>(DataTable.Filters);
            }
            var currentWarehouseGUID = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).Select(x => x.WarehouseGUID)
                    .FirstOrDefault();
            var currInstance = DbWMS.codeWarehouse.Where(x => x.WarehouseGUID == currentWarehouseGUID).FirstOrDefault();
            //var CurrentWarehouseGUID = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).Select(x => x.WarehouseGUID)
            //    .ToList();
            //var warehouseAuthGUIDs = DbWMS.codeWarehouse
            //    .Where(x => CurrentWarehouseGUID.Contains(x.WarehouseGUID) || CurrentWarehouseGUID.Contains(x.ParentGUID))
            //    .Select(x => x.WarehouseGUID).ToList();



            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.WarehouseItemsEntry.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();


            var All = (from a in DbWMS.v_EntryMovementDataTable.AsExpandable().Where(x => x.OrganizationInstanceGUID == currInstance.OrganizationInstanceGUID &&
            //warehouseAuthGUIDs.Contains((Guid)x.WarehouseOwnerGUID) && 
            //AuthorizedList.Contains(x.OrganizationInstanceGUID.ToString()) && 
                        (x.IsDeterminanted == true || x.IsDeterminanted == null)
                        && x.ModelDescription != null)
                       select new WarehouseModelEntryMovementDataTableModel
                       {


                           ItemInputDetailGUID = a.ItemInputDetailGUID,
                           Active = (bool)a.Active,
                           EmailAddress = a.EmailAddress,
                           LastVerifiedByGUID = a.LastVerifiedByGUID.ToString(),
                           VerificationStatusGUID = a.VerificationStatusGUID.ToString(),
                           ItemModelWarehouseGUID = a.ItemModelWarehouseGUID.ToString(),
                           ModelDescription = a.ModelDescription,
                           ItemDescription = a.WarehouseItemDescription,
                           WarehouseItemClassificationGUID = a.WarehouseItemClassificationGUID.ToString(),
                           WarehouseItemGUID = a.WarehouseItemGuid.ToString(),
                           BrandGUID = a.BrandGUID.ToString(),
                           WarehouseOwnerGUID = a.WarehouseOwnerGUID.ToString(),
                           BudgetYear = a.BudgetYear.ToString(),
                           CostCenterGUID = a.CostCenterGUID.ToString(),
                           LicenseTypeGUID = a.LicenseTypeGUID.ToString(),
                           LastExpiryDate = a.LastExpiryDate,
                           LastIssueDate = a.LastIssueDate,
                           SequenceNumber = a.SequenceNumber,
                           RecruitmentTypeName = a.RecruitmentTypeName,
                           RecruitmentTypeGUID = a.RecruitmentTypeGUID.ToString(),
                           Identifier = a.BarcodeNumber.Length > 0 ? a.BarcodeNumber : (a.SerialNumber.Length > 0 ? a.SerialNumber : (a.IMEI1.Length > 0 ? a.IMEI1 : (a.GSM.Length > 0 ? a.GSM : (a.MAC.Length > 0 ? a.MAC : (a.SequenceNumber.Length > 0 ? a.SequenceNumber : ""))))),
                           //Identifier =( (a.BarcodeNumber != null ||a.BarcodeNumber!="") ? a.BarcodeNumber : (a.SerialNumber!=null || a.SerialNumber!="")?a.SerialNumber:(a.IMEI1!=null|a.IMEI!="")?a.IMEI1:(a.GSM!=null||a.GSM!="")?a.GSM:(a.MAC!=null||a.MAC!="")?a.MAC:(a.SequenceNumber!=null ||a.SequenceNumber!="")?a.SequenceNumber:""),

                           BarcodeNumber = a.BarcodeNumber,
                           SerialNumber = a.SerialNumber,
                           BillNumber = a.BillNumber,
                           DepartmentDescription = a.DepartmentDescription,
                           DepartmentGUID = a.DepartmentGUID.ToString(),
                           IME1 = a.IMEI,
                           GSM = a.GSM,
                           MAC = a.MAC,
                           ModelAge = a.ModelAge,
                           MSRPID = a.MSRPID,

                           ItemCondition = a.ItemCondition,
                           ItemConditionGUID = a.ItemConditionGUID.ToString(),
                           ItemServiceStatusGUID = a.ItemServiceStatusGUID.ToString(),
                           ServiceItemStatus = a.ServiceItemStatus,
                           WarehouseOwner = a.WarehosueOwnerName,
                           Governorate = a.LastLocation,
                           WarehouseLocationGUID = a.WarehouseLocationGUID.ToString(),
                           CustodianStaffGUID = a.LastCustdianNameGUID.ToString(),
                           LastCustdianGUID = a.LastCustdianGUID.ToString(),
                           CustodianWarehouseGUID = a.LastCustdianNameGUID.ToString(),
                           Custodian = a.LastCustodianName,
                           ModelStatus = a.ItemStatus,
                           ModelStatusGUID = a.ItemStatusGUID.ToString(),
                           CreatedDate = a.CreatedDate,
                           PurposeofuseGUID = a.PurposeofuseGUID,
                           TransferFromCountry = a.CountryTransferFrom,
                           TransferToCountry = a.CountryTransferTo,
                           TransferToCountryGUID = a.TransferToCountryGUID.ToString(),
                           TransferFromCountryGUID = a.TransferFromCountryGUID.ToString(),
                           ItemImage = a.WarehouseItemModelGUID + ".jpg",


                           Purposeofuse = a.Purposeofuse,
                           CreatedBy = a.CreatedBy,
                           CreatedByGUID = a.CreatedByGUID.ToString(),
                           Comments = a.Comments,
                           DeliveryStatus = a.LastFlow,
                           DeliveryStatusGUID = a.LastFlowTypeGUID.ToString(),
                           LastVerifiedDate = a.LastVerifiedDate,
                           VerifiedBy = a.VerifiedBy,
                           dataItemInputDetailRowVersion = a.dataItemInputDetailRowVersion
                       }).Where(Predicate);



            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);
            List<WarehouseModelEntryMovementDataTableModel> Result = new List<WarehouseModelEntryMovementDataTableModel>();


            if (All.Count() == 0)
            {


                return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
            }
            if (All.Count() <= 10)
            {
                Result = All.ToList();
            }
            else
            {
                Result = Mapper.Map<List<WarehouseModelEntryMovementDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());
            }
            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }
        #region Check Bulk
        public ActionResult WarehouseModelEntryMovementsCheckCustdion()
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            return PartialView("~/Areas/WMS/Views/ModelEntryMovements/_CheckCustodian.cshtml", new ReleaseSingleItemUpdateModalUpdateModel());
        }
        public JsonResult GetCustodianItems(Guid RequesterNameGUID)
        {
            var custodians = DbWMS.dataItemInputDetail.Where(x => x.LastCustdianNameGUID == RequesterNameGUID).ToList();
            if (custodians.Count > 0)
            {
                List<ItemCustodianVM> model = custodians.Select(x => new ItemCustodianVM
                {
                    ModelName = x.codeItemModelWarehouse.codeWarehouseItemModel.codeWarehouseItemModelLanguage.Where(f => f.LanguageID == LAN).Select(f => f.ModelDescription).FirstOrDefault(),
                    ItemName = x.codeItemModelWarehouse.codeWarehouseItemModel.codeWarehouseItem.codeWarehouseItemLanguage.Where(f => f.LanguageID == LAN).Select(f => f.WarehouseItemDescription).FirstOrDefault(),
                    BarcodeNumber = x.dataItemInputDeterminant.Where(f => f.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.Barcode).Select(f => f.DeterminantValue).FirstOrDefault(),
                    SerialNumber = x.dataItemInputDeterminant.Where(f => f.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.SerialNumber).Select(f => f.DeterminantValue).FirstOrDefault(),
                    IMEI1 = x.dataItemInputDeterminant.Where(f => f.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.IME1).Select(f => f.DeterminantValue).FirstOrDefault(),
                    GSM = x.dataItemInputDeterminant.Where(f => f.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.GSM).Select(f => f.DeterminantValue).FirstOrDefault(),
                    MSRPID = x.dataItemInputDeterminant.Where(f => f.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.MSRPID).Select(f => f.DeterminantValue).FirstOrDefault(),
                    ItemInputDetailGUID = x.ItemInputDetailGUID
                }).ToList();
                return Json(new { model = model }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { model = new List<ItemCustodianVM>() }, JsonRequestBehavior.AllowGet);
        }


        #endregion
        public ActionResult WarehouseModelEntryMovementsCreate()
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            return PartialView("~/Areas/WMS/Views/ModelEntryMovements/_ModelEntryMovementForm.cshtml", new ItemInputDetailModel { ItemInputDetailGUID = Guid.Empty });
        }

        public ActionResult WarehouseModelEntryMovementsUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Access, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }

            var myDetail = DbWMS.dataItemInputDetail.Find(PK);

            var input = DbWMS.dataItemInput.Where(x => x.ItemInputGUID == myDetail.ItemInputGUID).FirstOrDefault();
            DateTime ExecutionTime = DateTime.Now;
            var check = DbWMS.v_EntryMovementDataTable.Where(x => x.ItemInputDetailGUID == PK).FirstOrDefault();

            ItemInputDetailModel EntryModel = Mapper.Map(myDetail, new ItemInputDetailModel());
            EntryModel.SourceGUID = input.SourceGUID;
            EntryModel.ModelAge = check.ModelAge;
            EntryModel.LastReservedByGUID = myDetail.LastReservedByGUID;
            EntryModel.LastReservedByTypeGUID = myDetail.LastReservedByTypeGUID;
            EntryModel.LastReservedDate = myDetail.LastReservedDate;
            EntryModel.DepreciatedYearsText = check.ModelAge >= 4 ? "100%" : (check.ModelAge < 1 ? "0%" : check.ModelAge == 1 ? "20%" : check.ModelAge == 2 ? "40%" : check.ModelAge == 3 ? "60%" : "-");
            EntryModel.WarehouseOwnerGUID = myDetail.dataItemTransfer.Where(x => x.IsLastTransfer == true)
                .Select(x => x.DestionationGUID).FirstOrDefault();
            var determinantModel = DbWMS.dataItemInputDeterminant.Where(x => x.ItemInputDetailGUID == PK).ToList();

            var _warehouse = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
            EntryModel.IsOwner = false;
            if (_warehouse != null)
            {
                if (_warehouse.WarehouseGUID == check.WarehouseOwnerGUID || _warehouse.codeWarehouse.ParentGUID == null)
                {
                    EntryModel.IsOwner = true;

                }
            }
            var Determinants = DbWMS.codeTablesValuesLanguages.Where(x =>
                x.codeTablesValues.TableGUID == LookupTables.ModelDeterminants
                && x.LanguageID == LAN
                && x.Active).ToList();
            foreach (var item in determinantModel)
            {
                ModelDeterminantVM currentDet = new ModelDeterminantVM
                {
                    DeterminantValue = item.DeterminantValue,
                    WarehouseItemModelDeterminantGUID = (Guid)item.WarehouseItemModelDeterminantGUID,
                    DeterminantName = Determinants.Where(x => x.ValueGUID == item.codeWarehouseItemModelDeterminant.DeterminantGUID).Select(x => x.ValueDescription).FirstOrDefault(),
                };
                EntryModel.ModelDeterminantVM.Add(currentDet);


            }


            return PartialView("~/Areas/WMS/Views/ModelEntryMovements/_EditModelEntryMovementForm.cshtml", EntryModel);

        }
        public string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult WarehouseModelEntryMovementsCreate(ItemInputDetailModel model)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            dataItemInputDetail EntryModel = Mapper.Map(model, new dataItemInputDetail());
            if (!ModelState.IsValid && ActiveEntryModelDetail(EntryModel)) return PartialView("~/Areas/WMS/Views/ModelEntryMovements/_ModelEntryMovementForm.cshtml", model);
            if (ActiveCheckDeterminantEntryModelDetail(model)) return PartialView("~/Areas/WMS/Views/ModelEntryMovements/_ModelEntryMovementForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;
            dataItemInput EntryModelMaster = Mapper.Map(model, new dataItemInput());
            Guid EntityMasterPK = Guid.NewGuid();
            EntryModelMaster.ItemInputGUID = EntityMasterPK;
            EntryModelMaster.CreatedByGUID = UserGUID;
            EntryModelMaster.CreatedDate = ExecutionTime;
            EntryModelMaster.InputDate = ExecutionTime;


            var CurrentWarehouse = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID)
                .FirstOrDefault();
            List<dataItemInputDeterminant> allModelDeterminants = new List<dataItemInputDeterminant>();

            Guid EntityPK = Guid.NewGuid();
            if (EntryModel.Qunatity == 0)
                EntryModel.Qunatity = 1;

            EntryModel.ItemInputDetailGUID = EntityPK;
            EntryModel.ItemInputGUID = EntryModelMaster.ItemInputGUID;
            EntryModel.ItemStatusGUID = WarehouseItemStatus.Functionting;
            EntryModel.ItemServiceStatusGUID = WarehouseServiceItemStatus.InStock;
            EntryModel.ItemConditionGUID = WarehouseItemCondition.New;
            EntryModel.AcquisitionDate = model.AcquisitionDate;
            EntryModel.IsAvaliable = true;
            EntryModel.CreatedByGUID = UserGUID;
            EntryModel.CreatedDate = ExecutionTime;
            EntryModel.DamagedComment = model.DamagedComment;
            EntryModel.DamagedByGUID = model.DamagedByGUID;
            EntryModel.LastReservedDate = model.LastReservedDate;
            EntryModel.LastReservedByGUID = model.LastReservedByGUID;

            EntryModel.DamagedPart = model.DamagedPart;
            EntryModel.DamagedDate = model.DamagedDate;

            if (model.PurposeofuseGUID == null)
            {

                EntryModel.PurposeofuseGUID = Guid.Parse("8A2AE01E-0F57-4178-B05E-E3021A2301A9");

            }

            EntryModel.LastCustdianGUID = WarehouseRequestSourceTypes.Warehouse;
            EntryModel.LastCustdianNameGUID = CurrentWarehouse.WarehouseGUID;
            EntryModel.LastFlowTypeGUID = WarehouseRequestFlowType.Returned;
            EntryModel.WarehouseOwnerGUID = CurrentWarehouse.WarehouseGUID;
            EntryModel.IsDeterminanted = true;
            //UpdateNew
            EntryModel.LastCustodianName = CurrentWarehouse.codeWarehouse.codeWarehouseLanguage.Where(x => x.LanguageID == LAN).Select(x => x.WarehouseDescription).FirstOrDefault();
            var _check = DbWMS.codeItemModelWarehouse.Where(x => x.ItemModelWarehouseGUID == model.ItemModelWarehouseGUID).Select(x => x.codeWarehouseItemModel.codeWarehouseItem.WarehouseItemClassificationGUID).FirstOrDefault();

            if (_check != null && _check == Guid.Parse("a66c83ab-50fc-4316-ab31-6e823761ee46"))
            {
                EntryModel.LastWarehouseLocationGUID = CurrentWarehouse.codeWarehouse.WarehouseLocationGUID;
                EntryModelMaster.SourceGUID = Guid.Parse("D5E2A4DF-C67E-4F4D-A242-7E33C331EC99");
                EntryModel.SequenceNumber = RandomString(30, false);
                var _modelGUID = DbWMS.codeItemModelWarehouse.Where(x => x.ItemModelWarehouseGUID == model.ItemModelWarehouseGUID).
                    Select(x => x.WarehouseItemModelGUID).FirstOrDefault();
                var _determinGUID = DbWMS.codeWarehouseItemModelDeterminant.Where(x => x.WarehouseItemModelGUID == _modelGUID).Select(x => x.WarehouseItemModelDeterminantGUID).FirstOrDefault();
                dataItemInputDeterminant toAdddet = new dataItemInputDeterminant
                {
                    ItemInputDeterminantGUID = Guid.NewGuid(),
                    ItemInputDetailGUID = EntryModel.ItemInputDetailGUID,
                    WarehouseItemModelDeterminantGUID = _determinGUID,
                    DeterminantValue = EntryModel.SequenceNumber,
                    CreatedByGUID = UserGUID,
                    CreatedDate = ExecutionTime,
                    Active = true,
                };
                DbWMS.Create(toAdddet, Permissions.WarehouseItemsEntry.CreateGuid, ExecutionTime, DbCMS);




            }
            else
            {
                EntryModel.LastWarehouseLocationGUID = CurrentWarehouse.codeWarehouse.WarehouseLocationGUID;
                model.ModelDeterminantVM.Where(x => x.DeterminantValue != null && x.DeterminantValue.ToString().Length > 4).ForEach(myDet =>
                         allModelDeterminants.Add(new dataItemInputDeterminant
                         {
                             ItemInputDeterminantGUID = Guid.NewGuid(),
                             ItemInputDetailGUID = EntryModel.ItemInputDetailGUID,
                             WarehouseItemModelDeterminantGUID = myDet.WarehouseItemModelDeterminantGUID,
                             DeterminantValue = myDet.DeterminantValue,
                             CreatedByGUID = UserGUID,
                             CreatedDate = ExecutionTime,
                             Active = true,
                         })
                  );

                //UpdateNew

                foreach (var item in model.ModelDeterminantVM.Where(x => x.DeterminantValue != null && x.DeterminantValue.ToString().Length > 4))
                {
                    if (item.DeterminantName == "Barcode")
                    {
                        EntryModel.BarcodeNumber = item.DeterminantValue;
                    }
                    else if (item.DeterminantName == "Serial Number")
                    {
                        EntryModel.SerialNumber = item.DeterminantValue;
                    }
                    else if (item.DeterminantName == "IMEI")
                    {
                        EntryModel.IMEI1 = item.DeterminantValue;
                    }
                    else if (item.DeterminantName == "IMEI2")
                    {
                        EntryModel.IMEI2 = item.DeterminantValue;
                    }
                    else if (item.DeterminantName == "GSM")
                    {
                        EntryModel.GSM = item.DeterminantValue;
                    }
                    else if (item.DeterminantName == "MAC")
                    {
                        EntryModel.MAC = item.DeterminantValue;
                    }
                    else if (item.DeterminantName == "MSRP ID")
                    {
                        EntryModel.MAC = item.DeterminantValue;
                    }
                    else if (item.DeterminantName == "Sequence Number")
                    {
                        EntryModel.SequenceNumber = item.DeterminantValue;
                    }
                }
                //EntryModel.LastLocationGUID = CurrentWarehouse.codeWarehouse.LocationGUID;






                DbWMS.CreateBulk(allModelDeterminants, Permissions.WarehouseItemsEntry.CreateGuid, DateTime.Now, DbCMS);
            }
            DbWMS.Create(EntryModelMaster, Permissions.WarehouseItemsEntry.CreateGuid, ExecutionTime, DbCMS);
            DbWMS.Create(EntryModel, Permissions.WarehouseItemsEntry.CreateGuid, ExecutionTime, DbCMS);
            dataItemTransfer TransferModel = new dataItemTransfer();
            TransferModel.ItemTransferGUID = Guid.NewGuid();
            TransferModel.ItemInputDetailGUID = EntryModel.ItemInputDetailGUID;
            TransferModel.TransferDate = ExecutionTime;
            TransferModel.SourceGUID = CurrentWarehouse.WarehouseGUID;
            TransferModel.DestionationGUID = CurrentWarehouse.WarehouseGUID;

            TransferModel.TransferedByGUID = UserGUID;
            TransferModel.IsLastTransfer = true;
            DbWMS.Create(TransferModel, Permissions.WarehouseItemsEntry.CreateGuid, ExecutionTime, DbCMS);
            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleUpdateMessage(DataTableNames.WarehouseModelEntryMovementsDataTable, DbWMS.PrimaryKeyControl(EntryModel), DbWMS.RowVersionControls(Portal.SingleToList(EntryModel))));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult WarehouseModelEntryMovementsUpdate(ItemInputDetailModel model)
        {

            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Update, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (!ModelState.IsValid) return PartialView("~/Areas/WMS/Views/ModelEntryMovements/_EditModelEntryMovementForm.cshtml", model);
            if (model.ItemStatusGUID == null || model.ItemStatusGUID == Guid.Empty ||
                model.ItemServiceStatusGUID == null || model.ItemServiceStatusGUID == Guid.Empty) return PartialView("~/Areas/WMS/Views/ModelEntryMovements/_EditModelEntryMovementForm.cshtml", model);
            DateTime ExecutionTime = DateTime.Now;
            dataItemInput itemInput =
                DbWMS.dataItemInput.Where(x => x.ItemInputGUID == model.ItemInputGUID).FirstOrDefault();
            itemInput.BillNumber = model.BillNumber;
            itemInput.SourceGUID = model.SourceGUID;
            if (model.IsDeterminanted == null)
                model.IsDeterminanted = true;

            dataItemInputDetail EntryModel = Mapper.Map(model, new dataItemInputDetail());
            List<dataItemInputDeterminant> allModelDeterminants = new List<dataItemInputDeterminant>();
            var itemInputdetail = DbWMS.dataItemInputDetail.Where(x => x.ItemInputDetailGUID == EntryModel.ItemInputDetailGUID).FirstOrDefault();
            var detmintantToChange = DbWMS.dataItemInputDeterminant.Where(x => x.ItemInputDetailGUID == EntryModel.ItemInputDetailGUID).ToList();
            //if (model.ItemStatusGUID == WarehouseItemStatus.Damaged ||
            //    model.ItemStatusGUID == WarehouseItemStatus.GS45 || model.ItemStatusGUID == WarehouseItemStatus.Lost
            //    || model.ItemStatusGUID == WarehouseItemStatus.Disposed || model.ItemStatusGUID == WarehouseItemStatus.ForDisposal
            //    ||model.ItemStatusGUID== WarehouseItemStatus.Stopped)
            //    //check elham
            //    EntryModel.IsAvaliable = false;
            //else
            //{
            //    EntryModel.IsAvaliable = true;
            //    //EntryModel.ItemServiceStatusGUID = WarehouseServiceItemStatus.;
            //}
            #region ownership 

            var currentWarehouse = itemInputdetail.dataItemTransfer.Where(x => x.IsLastTransfer == true)
                .Select(x => x.DestionationGUID).FirstOrDefault();
            if (model.WarehouseOwnerGUID != currentWarehouse)
            {
                var CurrentWarehouseGUID = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).Select(x => x.WarehouseGUID)
                    .FirstOrDefault();
                if (EntryModel.LastCustdianGUID == WarehouseRequestSourceTypes.Warehouse)
                {
                    EntryModel.LastCustdianNameGUID = model.WarehouseOwnerGUID;
                }
                var toChangeTransfer = (from a in DbWMS.dataItemTransfer.Where(x => x.ItemInputDetailGUID == EntryModel.ItemInputDetailGUID) select a).ToList();
                toChangeTransfer.ForEach(x => x.IsLastTransfer = false);
                dataItemTransfer toAddtrans = new dataItemTransfer();

                //foreach (var tran in allPriviousTransfers)
                //{
                //    tran.IsLastTransfer = false;
                //}
                //DbWMS.UpdateBulk(allPriviousTransfers, Permissions.WarehouseItemsEntry.UpdateGuid, ExecutionTime, DbCMS);

                dataItemTransfer changeTransferModel = new dataItemTransfer();
                changeTransferModel.ItemTransferGUID = Guid.NewGuid();
                changeTransferModel.ItemInputDetailGUID = EntryModel.ItemInputDetailGUID;
                changeTransferModel.TransferDate = ExecutionTime;
                changeTransferModel.SourceGUID = currentWarehouse;
                changeTransferModel.DestionationGUID = model.WarehouseOwnerGUID;
                changeTransferModel.TransferedByGUID = UserGUID;
                changeTransferModel.IsLastTransfer = true;
                DbWMS.Create(changeTransferModel, Permissions.WarehouseItemsEntry.UpdateGuid, ExecutionTime, DbCMS);
            }


            #endregion

            DbWMS.Update(itemInput, Permissions.WarehouseItemsEntry.UpdateGuid, ExecutionTime, DbCMS);
            var _check = DbWMS.codeItemModelWarehouse.Where(x => x.ItemModelWarehouseGUID == model.ItemModelWarehouseGUID).Select(x => x.codeWarehouseItemModel.codeWarehouseItem.WarehouseItemClassificationGUID).FirstOrDefault();

            if (_check != null && _check != Guid.Parse("a66c83ab-50fc-4316-ab31-6e823761ee46"))
            {

                var toUpdate = detmintantToChange.Where(x => x.DeterminantValue.ToString().Length > 4).ToList();

                foreach (var myDet in toUpdate)
                {
                    myDet.DeterminantValue = model.ModelDeterminantVM
                        .Where(x => x.WarehouseItemModelDeterminantGUID == myDet.WarehouseItemModelDeterminantGUID)
                        .Select(x => x.DeterminantValue).FirstOrDefault();
                }
                foreach (var item in toUpdate)
                {
                    if (item.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.Barcode)
                    {
                        EntryModel.BarcodeNumber = item.DeterminantValue;
                    }
                    if (item.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.SerialNumber)
                    {
                        EntryModel.SerialNumber = item.DeterminantValue;
                    }
                    if (item.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.IME1)
                    {
                        EntryModel.IMEI1 = item.DeterminantValue;
                    }
                    if (item.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.IME2)
                    {
                        EntryModel.IMEI2 = item.DeterminantValue;
                    }
                    if (item.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.GSM)
                    {
                        EntryModel.GSM = item.DeterminantValue;
                    }
                    if (item.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.MAC)
                    {
                        EntryModel.MAC = item.DeterminantValue;
                    }
                    if (item.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.SeqNumber)
                    {
                        EntryModel.SequenceNumber = item.DeterminantValue;
                    }
                    if (item.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.MSRPID)
                    {
                        EntryModel.MSRPID = item.DeterminantValue;
                    }
                }
                DbWMS.UpdateBulk(toUpdate, Permissions.WarehouseItemsEntry.UpdateGuid, ExecutionTime, DbCMS);
            }
            EntryModel.DamagedComment = model.DamagedComment;
            EntryModel.DamagedByGUID = model.DamagedByGUID;

            EntryModel.DamagedPart = model.DamagedPart;
            EntryModel.DamagedDate = model.DamagedDate;

            EntryModel.LastReservedByGUID = model.LastReservedByGUID;
            EntryModel.LastReservedByTypeGUID = model.LastReservedByTypeGUID;
            EntryModel.LastReservedDate = model.LastReservedDate;

            DbWMS.Update(EntryModel, Permissions.WarehouseItemsEntry.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleUpdateMessage(DataTableNames.WarehouseModelEntryMovementsDataTable,
                    DbWMS.PrimaryKeyControl(EntryModel),
                    DbWMS.RowVersionControls(Portal.SingleToList(EntryModel))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyEntryModelDetail((Guid)model.ItemInputDetailGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult WarehouseModelEntryMovementsDelete(dataItemInputDetail model)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Delete, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            List<dataItemInputDetail> DeletedLanguages = DeleteEntryModelDetails(new List<dataItemInputDetail> { model });

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleDeleteMessage(DeletedLanguages, DataTableNames.WarehouseModelEntryMovementsDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyEntryModelDetail(model.ItemInputDetailGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult WarehouseModelEntryMovementsRestore(dataItemInputDetail model)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Restore, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (ActiveEntryModelDetail(model))
            {
                return Json(DbWMS.RecordExists());
            }

            List<dataItemInputDetail> RestoredLanguages = RestoreEntryModelDetails(Portal.SingleToList(model));

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleRestoreMessage(RestoredLanguages, DataTableNames.WarehouseModelEntryMovementsDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyEntryModelDetail(model.ItemInputDetailGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }
        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult WarehouseModelEntryMovementsDataTableDelete(List<dataItemInputDetail> models)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Delete, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            List<dataItemInputDetail> DeletedLanguages = DeleteEntryModelDetails(models);

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.WarehouseModelEntryMovementsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult WarehouseModelEntryMovementsDataTableCheck(List<dataItemInputDetail> models)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Delete, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            try
            {
                return PartialView("~/Areas/WMS/Views/ModelEntryMovements/_CheckBulkItems.cshtml", models);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult WarehouseModelEntryMovementsDataTableRestore(List<dataItemInputDetail> models)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Restore, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            List<dataItemInputDetail> RestoredLanguages = RestoreEntryModelDetails(models);

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.WarehouseModelEntryMovementsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        private List<dataItemInputDetail> DeleteEntryModelDetails(List<dataItemInputDetail> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<dataItemInputDetail> DeletedEntryModelDetails = new List<dataItemInputDetail>();

            string query = DbWMS.QueryBuilder(models, Permissions.WarehouseItemsEntry.DeleteGuid, SubmitTypes.Delete, "");

            var Records = DbWMS.Database.SqlQuery<dataItemInputDetail>(query).ToList();
            var myInputGuid = models.Select(x => x.ItemInputDetailGUID).FirstOrDefault();
            var inputGuid = DbWMS.dataItemInputDetail.Where(x => x.ItemInputDetailGUID == myInputGuid)
                .Select(x => x.ItemInputGUID).FirstOrDefault();

            var input = DbWMS.dataItemInput.Where(x => x.ItemInputGUID == inputGuid)
                .FirstOrDefault();
            input.Active = false;
            DbWMS.SaveChanges();



            foreach (var record in Records)
            {
                DeletedEntryModelDetails.Add(DbWMS.Delete(record, ExecutionTime, Permissions.WarehouseItemsEntry.DeleteGuid, DbCMS));
            }

            var determinants = DeletedEntryModelDetails.SelectMany(a => a.dataItemInputDeterminant).Where(l => l.Active).ToList();
            foreach (var determinant in determinants)
            {
                DbWMS.Delete(determinant, ExecutionTime, Permissions.WarehouseItemsEntry.DeleteGuid, DbCMS);
            }

            var transfers = DeletedEntryModelDetails.SelectMany(a => a.dataItemTransfer).Where(l => l.Active).ToList();
            foreach (var transfer in transfers)
            {
                DbWMS.Delete(transfer, ExecutionTime, Permissions.WarehouseItemsEntry.DeleteGuid, DbCMS);
            }
            var outputDetails = DeletedEntryModelDetails.SelectMany(a => a.dataItemOutputDetail).Where(l => l.Active).ToList();
            foreach (var detail in outputDetails)
            {
                DbWMS.Delete(detail, ExecutionTime, Permissions.WarehouseItemsEntry.DeleteGuid, DbCMS);
            }

            var damagedTracks = DeletedEntryModelDetails.SelectMany(a => a.dataItemOutputDetailDamagedTrack).Where(l => l.Active).ToList();
            foreach (var damaged in damagedTracks)
            {
                DbWMS.Delete(damaged, ExecutionTime, Permissions.WarehouseItemsEntry.DeleteGuid, DbCMS);
            }

            return DeletedEntryModelDetails;
        }

        private List<dataItemInputDetail> RestoreEntryModelDetails(List<dataItemInputDetail> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<dataItemInputDetail> RestoredLanguages = new List<dataItemInputDetail>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbWMS.QueryBuilder(models, Permissions.WarehouseItemsEntry.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Languages = DbWMS.Database.SqlQuery<dataItemInputDetail>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveEntryModelDetail(language))
                {
                    RestoredLanguages.Add(DbWMS.Restore(language, Permissions.WarehouseItemsEntry.DeleteGuid, Permissions.WarehouseItemsEntry.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyEntryModelDetail(Guid PK)
        {
            dataItemInputDetail dbModel = new dataItemInputDetail();

            var inputdetail = DbWMS.dataItemInputDetail.Where(a => a.ItemInputDetailGUID == PK).FirstOrDefault();
            var dbinputdetail = DbWMS.Entry(inputdetail).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbinputdetail, dbModel);



            if (inputdetail.dataItemInputDetailRowVersion.SequenceEqual(dbModel.dataItemInputDetailRowVersion))
            {
                return Json(DbCMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbCMS, dbModel, "LanguagesContainer"));
        }

        private bool ActiveEntryModelDetail(object model)
        {
            dataItemInputDetail EntryDetailModel = Mapper.Map(model, new dataItemInputDetail());




            int LanguageID = DbWMS.dataItemInputDetail.Count(x =>
               x.ItemInputDetailGUID == EntryDetailModel.ItemInputDetailGUID &&
               x.ItemInputGUID == EntryDetailModel.ItemInputGUID &&
                x.Active);
            if (LanguageID > 0)
            {
                ModelState.AddModelError("LanguageID", "Model Already exists");
            }

            return (LanguageID > 0);
        }

        private bool ActiveCheckDeterminantEntryModelDetail(object model)
        {
            ItemInputDetailModel EntryDetailModel = Mapper.Map(model, new ItemInputDetailModel());

            var checkCodes = EntryDetailModel.ModelDeterminantVM.Where(x => x.DeterminantValue != null).Select(x => x.DeterminantValue).ToList();
            var LanguageID = DbWMS.dataItemInputDeterminant.Where(x => checkCodes.Contains(x.DeterminantValue)
                && x.ItemInputDetailGUID != EntryDetailModel.ItemInputDetailGUID
                                                                       && x.Active
                )
                .ToList().Count;
            List<string> NotDuplicated = new List<string>();
            foreach (var item in checkCodes)
            {
                if (item.Length < 4)
                {
                    ModelState.AddModelError("LanguageID", "Determinants are wrong");
                    return true;
                }
                if (NotDuplicated.Contains(item))
                {
                    ModelState.AddModelError("LanguageID", "Duplication in your Determinants");
                    return true;
                }
                else
                {
                    NotDuplicated.Add(item);

                }
            }

            //int LanguageID = DbWMS.dataItemInputDetail.ToList().Count(x =>
            //    (x.dataItemInputDeterminant
            //         .Where(d => EntryDetailModel.ModelDeterminantVM.Select(f => f.DeterminantValue).Contains(d.DeterminantValue)).Count() > 0

            //    ) 
            //    && x.Active &&
            //    x.ItemInputDetailGUID != EntryDetailModel.ItemInputDetailGUID &&

            //    x.Active);
            if (LanguageID > 0)
            {
                ModelState.AddModelError("LanguageID", "Model Already exists");
            }

            return (LanguageID > 0);
        }

        #endregion

        #region Lists
        public ActionResult CheckItemType(Guid _ItemModelWarehouseGUID)
        {
            var _check = DbWMS.codeItemModelWarehouse.Where(x => x.ItemModelWarehouseGUID == _ItemModelWarehouseGUID).Select(x => x.codeWarehouseItemModel.codeWarehouseItem.WarehouseItemClassificationGUID).FirstOrDefault();
            int IsLicense = 0;
            if (_check != null && _check == Guid.Parse("a66c83ab-50fc-4316-ab31-6e823761ee46"))
            {
                IsLicense = 1;
            }
            return Json(new { IsLicense = IsLicense }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult GetModelDeterminats(Guid ItemModelWarehouseGUID)
        {
            ItemInputDetailModel model = new ItemInputDetailModel();

            var myDet = (from a in DbWMS.codeItemModelWarehouse.Where(x => x.Active && x.ItemModelWarehouseGUID == ItemModelWarehouseGUID)
                         join b in DbWMS.codeWarehouseItemModel.Where(x => x.Active) on a.WarehouseItemModelGUID equals b.WarehouseItemModelGUID
                         join c in DbWMS.codeWarehouseItemModelDeterminant.Where(x => x.Active) on b.WarehouseItemModelGUID equals c.WarehouseItemModelGUID
                         join d in DbWMS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on c.DeterminantGUID equals d.ValueGUID
                         select new ModelDeterminantVM
                         {
                             WarehouseItemModelDeterminantGUID = (Guid)c.WarehouseItemModelDeterminantGUID,
                             DeterminantName = d.ValueDescription
                         }).ToList();
            model.ModelDeterminantVM = myDet;
            return PartialView("~/Areas/WMS/Views/ModelEntryMovements/_EntryModelModelDeterminant.cshtml", model);
        }
        #endregion

        #region Release
        #region Custdion Model History


        public ActionResult WarehouseModelReleaseMovementsDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/WMS/Views/ModelMovements/_DamagedModelsDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<WarehouseModelReleaseMovementDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<WarehouseModelReleaseMovementDataTableModel>(DataTable.Filters);
            }


            var All = (

                from a in DbWMS.dataItemOutputDetail.AsNoTracking().AsExpandable().Where(x => x.Active && x.ItemInputDetailGUID == PK)
                join b in DbWMS.dataItemOutput.Where(x => x.Active) on a.ItemOutputGUID equals b.ItemOutputGUID into LJ1
                from R1 in LJ1.DefaultIfEmpty()
                join c in DbWMS.codeTablesValues.Where(x => x.Active) on R1.RequesterGUID equals c.ValueGUID
                join d in DbWMS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on c.ValueGUID equals d.ValueGUID into LJ2
                from R2 in LJ2.DefaultIfEmpty()
                join e in DbWMS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.ItemRequestTypeGUID equals e.ValueGUID into LJ3
                from R3 in LJ3.DefaultIfEmpty()
                join f in DbWMS.dataItemOutputDetailFlow.Where(x => x.Active && x.IsLastMove == true) on a.ItemOutputDetailGUID equals f.ItemOutputDetailGUID
                join g in DbWMS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on f.FlowTypeGUID equals g.ValueGUID into LJ4
                from R4 in LJ4.DefaultIfEmpty()

                join k in DbWMS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN) on R1.RequesterNameGUID equals k.UserGUID into LJ7
                from R7 in LJ7.DefaultIfEmpty()



                join M in DbWMS.codeWarehouseLanguage.Where(x => x.Active && x.LanguageID == LAN) on R1.RequesterNameGUID equals M.WarehouseGUID into LJ8
                from R8 in LJ8.DefaultIfEmpty()

                join N in DbWMS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN) on a.CreatedByGUID equals N.UserGUID into LJ9
                from R9 in LJ9.DefaultIfEmpty()

                join O in DbWMS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN) on a.RetunedByGUID equals O.UserGUID into LJ10
                from R10 in LJ10.DefaultIfEmpty()


                join p in DbWMS.dataItemInputDetail.Where(x => x.Active) on a.ItemInputDetailGUID equals p.ItemInputDetailGUID into LJ11
                from R11 in LJ11.DefaultIfEmpty()

                join r in DbWMS.codeWarehouseRequesterTypeLanguage.Where(x => x.Active && x.LanguageID == LAN) on R1.RequesterNameGUID equals r.WarehouseRequesterTypeGUID into LJ12
                from R12 in LJ12.DefaultIfEmpty()

                join yy in DbWMS.codeWarehouseVehicleLanguage.Where(x => x.Active && x.LanguageID == LAN) on R1.RequesterNameGUID equals yy.WarehouseVehicleGUID into LJ13
                from R13 in LJ13.DefaultIfEmpty()

                join z in DbWMS.codeWMSPartner.Where(x => x.Active) on R1.RequesterNameGUID equals z.PartnerGUID into LJ14
                from R14 in LJ14.DefaultIfEmpty()


                join zz in DbWMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on R1.RequesterNameGUID equals zz.ValueGUID into LJ15
                from R15 in LJ15.DefaultIfEmpty()

                join xy in DbWMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ItemStatusGUID equals xy.ValueGUID into LJ16
                from R16 in LJ16.DefaultIfEmpty()
                join xf in DbWMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ItemStatuOnReturnGUID equals xf.ValueGUID into LJ17
                from R17 in LJ17.DefaultIfEmpty()
                select new WarehouseModelReleaseMovementDataTableModel
                {
                    CustodianType = R2.ValueDescription,
                    Custodian = R1.RequesterGUID == WarehouseRequestSourceTypes.Staff ? R7.FirstName + " " + R7.Surname : (R1.RequesterGUID == WarehouseRequestSourceTypes.Warehouse ? R8.WarehouseDescription : R1.RequesterGUID == WarehouseRequestSourceTypes.OtherRequester ? R12.WarehouseRequesterTypeDescription : (R1.RequesterGUID == WarehouseRequestSourceTypes.Vehicle ? R13.VehicleDescription : R1.RequesterGUID == WarehouseRequestSourceTypes.Partner ? R14.PartnerName : R1.RequesterGUID == WarehouseRequestSourceTypes.GS45 ? R15.ValueDescription : null)),
                    ItemOutputDetailGUID = a.ItemOutputDetailGUID,
                    ReleaseType = R3.ValueDescription,
                    Qunatity = a.RequestedQunatity,
                    ExpectedStartDate = a.ExpectedStartDate,
                    ExpectedReturenedDate = a.ExpectedReturenedDate,
                    ActualReturenedDate = a.ActualReturenedDate,
                    DeliveryStatus = R4.ValueDescription,
                    ItemOutputDetailFlowGUID = f.ItemOutputDetailFlowGUID,
                    DeliveryActionDate = f.CreatedDate,
                    Active = a.Active,
                    Comments = a.Comments,
                    IssuedBy = R9.FirstName + " " + R9.Surname,
                    IssuedDate = a.CreatedDate,
                    ParentItemModelWarehouseGUID = R11.ParentItemModelWarehouseGUID,
                    ReturnedBy = R10.FirstName + " " + R10.Surname,
                    ItemStatus = R16.ValueDescription,
                    StatusOnReturn = R17.ValueDescription,
                    ReturnedDate = a.ReturnedDate,
                    ItemInputGUID = a.dataItemInputDetail.ItemInputGUID,
                    dataItemOutputDetailRowVersion = a.dataItemOutputDetailRowVersion

                }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<WarehouseModelReleaseMovementDataTableModel> Result = Mapper.Map<List<WarehouseModelReleaseMovementDataTableModel>>(All.OrderByDescending(x => x.IssuedDate).Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result.OrderByDescending(x => x.ExpectedStartDate)), JsonRequestBehavior.AllowGet);
        }

        public ActionResult WarehouseModelReleaseMovementsCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }

            var dataItemInputDetail = DbWMS.dataItemInputDetail.Find(FK);
            if (dataItemInputDetail.IsAvaliable == false)
            {

                return Json(DbWMS.PermissionError());
            }

            ReleaseSingleItemUpdateModalUpdateModel model = new ReleaseSingleItemUpdateModalUpdateModel
            {
                ItemInputDetailGUID = FK,
                ItemOutputDetailGUID = Guid.Empty,
                HasCharger = false,
                HasBag = false,
                HasLaptopMouse = false,
                HasHeadPhone = false,
                HasHeadsetUSB = false,
                HasHeadsetStereoJack = false,
                HasBackbag = false,
                HasHandbag = false,
                HasFlashMemory = false,
                HasRoaming = false,
                HasInternationalAccess = false,
                ParentItemModelWarehouseGUID = dataItemInputDetail.ParentItemModelWarehouseGUID,
                isChildrenKit = dataItemInputDetail.codeItemModelWarehouse.codeWarehouseItemModel.ItemModelRelationTypeGUID == LookupTables.ItemRelationChildrenKit
            };

            return PartialView("~/Areas/WMS/Views/ModelReleaseMovements/_ModelReleaseMovementsModel.cshtml",
                model);
        }


        #region track Movemnets
        public ActionResult WarehouseModelTrackMovements(Guid PK)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }



            var myDetail = DbWMS.dataItemInputDetail.Find(PK);
            var input = DbWMS.dataItemInput.Where(x => x.ItemInputGUID == myDetail.ItemInputGUID).FirstOrDefault();

            ItemInputDetailModel EntryModel = Mapper.Map(myDetail, new ItemInputDetailModel());
            EntryModel.SourceGUID = input.SourceGUID;
            EntryModel.BillNumber = input.BillNumber;
            var _warehouse = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
            var check = DbWMS.v_EntryMovementDataTable.Where(x => x.ItemInputDetailGUID == PK).FirstOrDefault();
            EntryModel.Location = check.LastLocation;
            EntryModel.WarehouseName = check.WarehosueOwnerName;

            EntryModel.IsOwner = false;
            if (_warehouse != null)
            {
                 if (_warehouse.WarehouseGUID == check.WarehouseOwnerGUID || _warehouse.codeWarehouse.ParentGUID==null)
                {
                    EntryModel.IsOwner = true;

                }
            }
           
            EntryModel.DepreciatedYearsText = check.ModelAge >= 4 ? "100%" : (check.ModelAge < 1 ? "0%" : check.ModelAge == 1 ? "20%" : check.ModelAge == 2 ? "40%" : check.ModelAge == 3 ? "60%" : "-");

            var determinantModel = DbWMS.dataItemInputDeterminant.Where(x => x.ItemInputDetailGUID == PK).ToList();
            EntryModel.WarehouseOwnerGUID = myDetail.dataItemTransfer.Where(x => x.IsLastTransfer == true)
                .Select(x => x.DestionationGUID).FirstOrDefault();

            var Determinants = DbWMS.codeTablesValuesLanguages.Where(x =>
                x.codeTablesValues.TableGUID == LookupTables.ModelDeterminants
                && x.LanguageID == LAN
                && x.Active).ToList();
            string _value = "";

            _value = determinantModel.Where(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID ==
                                                                 ItemDeterminants.Barcode
                                                                ).Select(x => x.DeterminantValue)
                                                             .FirstOrDefault();
            if (!string.IsNullOrEmpty(_value) && _value != null)
            {
                _value = "BC= " + _value;
            }
            if (string.IsNullOrEmpty(_value) || _value == null)
            {
                _value = determinantModel.Where(x =>
                                                               x.codeWarehouseItemModelDeterminant.DeterminantGUID ==
                                                               ItemDeterminants.SerialNumber
                                                             ).Select(x => x.DeterminantValue)
                                                          .FirstOrDefault();
                if (!string.IsNullOrEmpty(_value) && _value != null)
                {
                    _value = "SN= " + _value;
                }
            }
            if (string.IsNullOrEmpty(_value) || _value == null)
            {
                _value = determinantModel.Where(x =>
                                                                x.codeWarehouseItemModelDeterminant.DeterminantGUID ==
                                                                ItemDeterminants.GSM
                                                             ).Select(x => x.DeterminantValue)
                                                          .FirstOrDefault();
                if (!string.IsNullOrEmpty(_value) && _value != null)
                {
                    _value = "GSM= " + _value;
                }
            }
            if (string.IsNullOrEmpty(_value) || _value == null)
            {
                _value = determinantModel.Where(x =>
                                                                x.codeWarehouseItemModelDeterminant.DeterminantGUID ==
                                                                ItemDeterminants.IME1
                                                             ).Select(x => x.DeterminantValue)
                                                          .FirstOrDefault();
                if (!string.IsNullOrEmpty(_value) && _value != null)
                {
                    _value = "IMEI: " + _value;
                }
            }
            if (string.IsNullOrEmpty(_value) || _value == null)
            {
                _value = determinantModel.Where(x =>
                                                                  x.codeWarehouseItemModelDeterminant.DeterminantGUID ==
                                                                  ItemDeterminants.IME2
                                                             ).Select(x => x.DeterminantValue)
                                                          .FirstOrDefault();

                if (!string.IsNullOrEmpty(_value) && _value != null)
                {
                    _value = "IMEI= " + _value;
                }
            }
            if (string.IsNullOrEmpty(_value) || _value == null)
            {
                _value = determinantModel.Where(x =>
                                                                  x.codeWarehouseItemModelDeterminant.DeterminantGUID ==
                                                                  ItemDeterminants.MAC
                                                             ).Select(x => x.DeterminantValue)
                                                          .FirstOrDefault();
                if (!string.IsNullOrEmpty(_value) && _value != null)
                {
                    _value = "MAC= " + _value;
                }
            }
            if (string.IsNullOrEmpty(_value) || _value == null)
            {
                _value = determinantModel.Where(x =>
                                                                 x.codeWarehouseItemModelDeterminant.DeterminantGUID ==
                                                                 ItemDeterminants.SeqNumber
                                                             ).Select(x => x.DeterminantValue)
                                                          .FirstOrDefault();
                if (!string.IsNullOrEmpty(_value) && _value != null)
                {
                    _value = "SEQ= " + _value;
                }
            }
            if (EntryModel.ModelNameDetermiant == null)

                EntryModel.ModelNameDetermiant = myDetail.codeItemModelWarehouse.codeWarehouseItemModel
                                                             .codeWarehouseItemModelLanguage.Select(x => x.ModelDescription)
                                                             .FirstOrDefault()
                                                         + " :" + _value;


            foreach (var item in determinantModel)
            {
                ModelDeterminantVM currentDet = new ModelDeterminantVM
                {
                    DeterminantValue = item.DeterminantValue,
                    WarehouseItemModelDeterminantGUID = (Guid)item.WarehouseItemModelDeterminantGUID,
                    DeterminantName = Determinants.Where(x => x.ValueGUID == item.codeWarehouseItemModelDeterminant.DeterminantGUID).Select(x => x.ValueDescription).FirstOrDefault(),
                };
                EntryModel.ModelDeterminantVM.Add(currentDet);


            }

            EntryModel.DamagedReportName = EntryModel.ItemInputDetailGUID + ".pdf";
            EntryModel.ItemImage = myDetail.codeItemModelWarehouse.WarehouseItemModelGUID + ".jpg";
            var detail = DbWMS.v_EntryMovementDataTable.Where(x => x.ItemInputDetailGUID == PK).FirstOrDefault();
            EntryModel.ItemClassification = detail.WarehouseItemClassificationDescription;
            EntryModel.ItemSubClassification = detail.WarehouseItemDescription;
            EntryModel.ItemBrand = detail.BrandDescription;

            //if (EntryModel.IsAvaliable == true)
            //    ViewBag.IsReleased = false;
            //else
            //{
            //    ViewBag.IsReleased = true;
            //}

            return View("~/Areas/WMS/Views/ModelEntryMovements/ModelEntryMovements.cshtml", EntryModel);
        }

        #endregion

        #region Release Bulck
        public ActionResult WarehouseReleaseBulkItemCreate()
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }



            ReleaseBulkItemUpdateModalUpdateModel model = new ReleaseBulkItemUpdateModalUpdateModel
            {

                ItemOutputDetailGUID = Guid.Empty,

            };

            return PartialView("~/Areas/WMS/Views/ModelReleaseMovements/_ReleaseBulkItems.cshtml",
                model);
        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult WarehouseReleaseBulkItemCreate(ReleaseBulkItemUpdateModalUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (model.ItemStatusGUID == null || model.ItemStatusGUID == Guid.Empty)
            {
                return Json(DbWMS.PermissionError());
            }



            #region Check det
            if (string.IsNullOrEmpty(model.SerialNumbers))
            {
                return Json(new { success = 0 }, JsonRequestBehavior.AllowGet);
            }

            var res = model.SerialNumbers;
            string[] splitInput = System.Text.RegularExpressions.Regex.Split(res, "\r\n");



            var myCurrentWarehous_GUID = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).Select(x => x.WarehouseGUID)
              .ToList();

            var Result = (from a in DbWMS.dataItemInputDeterminant.Where(x => x.Active)
                          join b in DbWMS.dataItemInputDetail.Where(x => x.Active && x.IsAvaliable == true && x.LastFlowTypeGUID == WarehouseRequestFlowType.Returned) on a.ItemInputDetailGUID equals b.ItemInputDetailGUID
                          join c in DbWMS.dataItemTransfer.Where(x => x.Active && x.IsLastTransfer == true && myCurrentWarehous_GUID.Contains(x.DestionationGUID)) on b.ItemInputDetailGUID equals c.ItemInputDetailGUID


                          select new
                          {
                              Value = a.ItemInputDetailGUID,
                              Text = a.DeterminantValue
                          }).ToList();



            List<Guid?> filesToAdd = new List<Guid?>();
            List<string> filesNotAdd = new List<string>();

            for (int i = 0; i < splitInput.Length; i++)
            {
                string current = splitInput[i];
                if (splitInput[i] == null || splitInput[i] == "")
                    continue;
                var _check = Result.Where(x => x.Text == current).FirstOrDefault();
                if (_check == null)
                {
                    filesNotAdd.Add(current);
                    continue;
                }



                model.ItemInputModelGuids.Add((Guid)_check.Value);

            }
            if (filesNotAdd.Count() > 0)
            {
                var _not = String.Join(", ", filesNotAdd.ToArray());

                ModelState.AddModelError("Release Description", "The following Items not avaiable at your stock :" + _not);
                return PartialView("~/Areas/WMS/Views/ModelReleaseMovements/_ReleaseBulkItems.cshtml",
                    model);

            }
            //else
            //{
            //    model.ItemInputModelGuids =(Guid?) filesToAdd.Select(x=>x);
            //}
            #endregion

            if (model.ItemInputModelGuids == null || model.ItemInputModelGuids.Count == 0)
            {
                ModelState.AddModelError("Release Description", "You did not Chose any items to release ");
                return PartialView("~/Areas/WMS/Views/ModelReleaseMovements/_ReleaseBulkItems.cshtml",
                    model);

            }


            Guid EntityPK = Guid.NewGuid();
            DateTime ExecutionTime = DateTime.Now;
            dataItemOutput ReleaseModel = Mapper.Map(model, new dataItemOutput());
            ReleaseModel.ItemOutputGUID = EntityPK;
            ReleaseModel.CreatedByGUID = UserGUID;
            ReleaseModel.CreatedDate = ExecutionTime;

            ReleaseModel.OutputNumber = DbWMS.dataItemOutput.Select(x => x.OutputNumber).Max() + 1 ?? 1;
            DbWMS.Create(ReleaseModel, Permissions.WarehouseItemsRelease.CreateGuid, ExecutionTime, DbCMS);
            List<dataItemOutputDetail> itemReleaseDetailsToAdd = new List<dataItemOutputDetail>();
            var myCurrentWarehouseGUID = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).Select(x => x.WarehouseGUID)
                .FirstOrDefault();
            List<dataItemTransfer> toAddtrans = new List<dataItemTransfer>();

            foreach (var item in model.ItemInputModelGuids)
            {
                #region main Item
                dataItemOutputDetail ReleaseModelDetail = Mapper.Map(model, new dataItemOutputDetail());
                ReleaseModelDetail.ItemOutputDetailGUID = Guid.NewGuid();
                if (ReleaseModelDetail.RequestedQunatity == null)
                    ReleaseModelDetail.RequestedQunatity = 1;
                //if (model.ExpectedStartDate == null)
                //{
                //    ReleaseModelDetail.ExpectedStartDate = DateTime.Now;
                //}

                if (model.ItemRequestTypeGUID == null)
                {
                    ReleaseModelDetail.ItemRequestTypeGUID = LookupTables.ItemRequestTypeLongTerm;
                }
                ReleaseModelDetail.CreatedByGUID = UserGUID;
                ReleaseModelDetail.ItemInputDetailGUID = item;
                ReleaseModelDetail.ItemOutputGUID = ReleaseModel.ItemOutputGUID;
                ReleaseModelDetail.CreatedDate = ExecutionTime;
                ReleaseModelDetail.Comments = ReleaseModel.Comments;
                ReleaseModelDetail.ExpectedReturenedDate = model.ExpectedReturenedDate;
                ReleaseModelDetail.WarehouseLocationGUID = model.WarehouseLocationGUID;
                ReleaseModelDetail.ItemStatusGUID = (Guid)model.ItemStatusGUID;
                itemReleaseDetailsToAdd.Add(ReleaseModelDetail);
                dataItemInputDetail InputDetail = DbWMS.dataItemInputDetail.Find(item);
                //dataItemOutput itemOutput = DbWMS.dataItemOutput.Find(ReleaseModelDetail.ItemOutputGUID);
                if (ReleaseModel.RequesterGUID != WarehouseRequestSourceTypes.Warehouse)
                {
                    InputDetail.IsAvaliable = false;
                    InputDetail.ItemServiceStatusGUID = WarehouseServiceItemStatus.InService;
                }
                InputDetail.LastCustdianGUID = (Guid)ReleaseModel.RequesterGUID;
                InputDetail.LastCustdianNameGUID = ReleaseModel.RequesterNameGUID;
                InputDetail.LastWarehouseLocationGUID = model.WarehouseLocationGUID;
                if (ReleaseModel.RequesterGUID != WarehouseRequestSourceTypes.Warehouse)
                {

                    InputDetail.ItemConditionGUID = WarehouseItemCondition.Used;
                }

                InputDetail.LastCustodianName = CheckCustodianName((Guid)ReleaseModel.RequesterGUID, (Guid)ReleaseModel.RequesterNameGUID);


                var Chcektransfer = DbWMS.dataItemTransfer.Where(x => x.ItemInputDetailGUID == InputDetail.ItemInputDetailGUID
                                                                      && x.IsLastTransfer == true).FirstOrDefault();

                if (Chcektransfer.codeWarehouse.WarehouseLocationGUID != model.WarehouseLocationGUID || (
                    model.RequesterGUID == WarehouseRequestSourceTypes.Warehouse))
                {
                    var CurrentWarehouseGUID = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).Select(x => x.WarehouseGUID)
                        .FirstOrDefault();

                    var toChangeTransfer = (from a in DbWMS.dataItemTransfer.Where(x => x.ItemInputDetailGUID == InputDetail.ItemInputDetailGUID) select a).ToList();
                    toChangeTransfer.ForEach(x => x.IsLastTransfer = false);


                    //foreach (var tran in allPriviousTransfers)
                    //{
                    //    tran.IsLastTransfer = false;
                    //}
                    //DbWMS.UpdateBulk(allPriviousTransfers, Permissions.WarehouseItemsEntry.UpdateGuid, ExecutionTime, DbCMS);
                    var warehouse = DbWMS.codeWarehouse.Where(x => x.WarehouseLocationGUID == model.WarehouseLocationGUID).FirstOrDefault();
                    dataItemTransfer changeTransferModel = new dataItemTransfer();
                    changeTransferModel.ItemTransferGUID = Guid.NewGuid();
                    changeTransferModel.ItemInputDetailGUID = InputDetail.ItemInputDetailGUID;
                    changeTransferModel.TransferDate = ExecutionTime;
                    changeTransferModel.SourceGUID = myCurrentWarehouseGUID;
                    changeTransferModel.DestionationGUID = warehouse.WarehouseGUID;
                    changeTransferModel.TransferedByGUID = UserGUID;
                    changeTransferModel.IsLastTransfer = true;
                    toAddtrans.Add(changeTransferModel);

                }



                if (ReleaseModel.RequesterGUID == WarehouseRequestSourceTypes.OtherRequester
                    || ReleaseModel.RequesterGUID == WarehouseRequestSourceTypes.Vehicle
                    || ReleaseModel.RequesterGUID == WarehouseRequestSourceTypes.Partner
                    || ReleaseModel.RequesterGUID == WarehouseRequestSourceTypes.GS45)
                {
                    InputDetail.LastFlowTypeGUID = WarehouseRequestFlowType.Confirmed;
                }
                else
                {
                    ////if (model.ExpectedStartDate < DateTime.Now)
                    ////{
                    ////    InputDetail.LastFlowTypeGUID = WarehouseRequestFlowType.Confirmed;
                    ////}
                    ////else
                    ////{
                    InputDetail.LastFlowTypeGUID = WarehouseRequestFlowType.PendingConfirmed;

                    //}
                }
                if (ReleaseModel.RequesterGUID == WarehouseRequestSourceTypes.GS45)
                {
                    InputDetail.ItemServiceStatusGUID = WarehouseServiceItemStatus.GS45;
                }
                InputDetail.ItemStatusGUID = (Guid)model.ItemStatusGUID;
                DbWMS.Update(InputDetail, Permissions.WarehouseItemsRelease.UpdateGuid, ExecutionTime, DbCMS);
                List<dataItemOutputDetailFlow> flows = DbWMS.dataItemOutputDetailFlow
                    .Where(x => x.dataItemOutputDetail.dataItemInputDetail.ItemInputDetailGUID == item).ToList();
                var flowDetailsPriv = (from a in DbWMS.dataItemOutputDetailFlow.Where(x => x.dataItemOutputDetail.dataItemInputDetail.ItemInputDetailGUID == InputDetail.ItemInputDetailGUID) select a).ToList();
                flowDetailsPriv.ForEach(x => x.IsLastAction = false);
                //foreach (var flow in flows)
                //{
                //    flow.IsLastAction = false;

                //}
                //DbWMS.UpdateBulk(flows, Permissions.WarehouseItemsEntry.UpdateGuid, ExecutionTime, DbCMS);
                int? order = DbWMS.dataItemOutputDetailFlow
                    .Where(x => x.dataItemOutputDetail.ItemInputDetailGUID == item)
                    .Select(x => x.OrderId).Max() + 1;
                if (order == null)
                    order = 1;
                dataItemOutputDetailFlow outputDetailFlow = new dataItemOutputDetailFlow();

                outputDetailFlow.ItemOutputDetailFlowGUID = Guid.NewGuid();
                outputDetailFlow.ItemOutputDetailGUID = ReleaseModelDetail.ItemOutputDetailGUID;
                if (ReleaseModel.RequesterGUID == WarehouseRequestSourceTypes.OtherRequester
                    || ReleaseModel.RequesterGUID == WarehouseRequestSourceTypes.Vehicle
                    || ReleaseModel.RequesterGUID == WarehouseRequestSourceTypes.Partner
                    || ReleaseModel.RequesterGUID == WarehouseRequestSourceTypes.GS45)
                {
                    outputDetailFlow.FlowTypeGUID = WarehouseRequestFlowType.Confirmed;
                }
                else
                {
                    //if (model.ExpectedStartDate < DateTime.Now)
                    //{
                    //    outputDetailFlow.FlowTypeGUID = WarehouseRequestFlowType.Confirmed;
                    //}
                    //else
                    //{
                    outputDetailFlow.FlowTypeGUID = WarehouseRequestFlowType.PendingConfirmed;

                    //}
                }


                outputDetailFlow.ItemStatuGUID = (Guid)model.ItemStatusGUID;
                outputDetailFlow.CreatedDate = DateTime.Now;
                outputDetailFlow.IsLastAction = true;
                outputDetailFlow.IsLastMove = true;
                outputDetailFlow.CreatedByGUID = UserGUID;
                outputDetailFlow.Active = true;
                outputDetailFlow.OrderId = order;
                DbWMS.Create(outputDetailFlow, Permissions.WarehouseItemsEntry.CreateGuid, ExecutionTime, DbCMS);
                if (ReleaseModel.RequesterGUID == WarehouseRequestSourceTypes.Warehouse)
                {
                    var CurrentWarehouseGUID = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).Select(x => x.WarehouseGUID)
                        .FirstOrDefault();




                    #endregion

                    #region Children
                    List<dataItemInputDetail> ChildreninputDetails = DbWMS.dataItemInputDetail.Where(x => x.ParentItemModelWarehouseGUID == item).ToList();
                    List<dataItemOutputDetail> childrenDetails = new List<dataItemOutputDetail>();
                    foreach (var children in ChildreninputDetails)
                    {
                        dataItemOutputDetail myChildrendOutputDetail = Mapper.Map(model, new dataItemOutputDetail());
                        myChildrendOutputDetail.ItemOutputDetailGUID = Guid.NewGuid();
                        if (myChildrendOutputDetail.RequestedQunatity == null)
                            myChildrendOutputDetail.RequestedQunatity = 1;
                        myChildrendOutputDetail.CreatedByGUID = UserGUID;
                        myChildrendOutputDetail.ItemOutputGUID = ReleaseModel.ItemOutputGUID;
                        myChildrendOutputDetail.CreatedDate = ExecutionTime;
                        myChildrendOutputDetail.Comments = ReleaseModel.Comments;
                        myChildrendOutputDetail.ItemInputDetailGUID = children.ItemInputDetailGUID;
                        childrenDetails.Add(myChildrendOutputDetail);

                        if (ReleaseModel.RequesterGUID != WarehouseRequestSourceTypes.Warehouse)
                        {
                            children.IsAvaliable = false;
                            children.ItemServiceStatusGUID = WarehouseServiceItemStatus.InService;
                        }

                        children.LastCustdianGUID = (Guid)ReleaseModel.RequesterGUID;
                        //children.LastLocationGUID = InputDetail.LastLocationGUID;
                        children.LastWarehouseLocationGUID = InputDetail.LastWarehouseLocationGUID;
                        children.LastCustdianNameGUID = ReleaseModel.RequesterNameGUID;
                        children.ItemConditionGUID = WarehouseItemCondition.Used;
                        children.LastCustodianName = CheckCustodianName((Guid)ReleaseModel.RequesterGUID, (Guid)ReleaseModel.RequesterNameGUID);
                        if (ReleaseModel.RequesterGUID == WarehouseRequestSourceTypes.OtherRequester
                            || ReleaseModel.RequesterGUID == WarehouseRequestSourceTypes.Vehicle
                            || ReleaseModel.RequesterGUID == WarehouseRequestSourceTypes.Partner
                            || ReleaseModel.RequesterGUID == WarehouseRequestSourceTypes.GS45)
                        {
                            children.LastFlowTypeGUID = WarehouseRequestFlowType.Confirmed;
                        }
                        else
                        {
                            //if (model.ExpectedStartDate < DateTime.Now)
                            //{
                            //    children.LastFlowTypeGUID = WarehouseRequestFlowType.Confirmed;
                            //    }
                            //else
                            //{
                            children.LastFlowTypeGUID = WarehouseRequestFlowType.PendingConfirmed;

                            //}
                        }
                        //children.LastFlowTypeGUID = WarehouseRequestFlowType.Confirmed;

                        DbWMS.Update(children, Permissions.WarehouseItemsRelease.UpdateGuid, ExecutionTime, DbCMS);


                        List<dataItemOutputDetailFlow> myChldrenflows = DbWMS.dataItemOutputDetailFlow
                            .Where(x => x.dataItemOutputDetail.dataItemInputDetail.ItemInputDetailGUID == children.ItemInputDetailGUID).ToList();


                        var privFlows = (from a in DbWMS.dataItemOutputDetailFlow.Where(x => x.dataItemOutputDetail.dataItemInputDetail.ItemInputDetailGUID == children.ItemInputDetailGUID) select a).ToList();
                        privFlows.ForEach(x => x.IsLastAction = false);

                        //foreach (var flow in myChldrenflows)
                        //{
                        //    flow.IsLastAction = false;

                        //}
                        //DbWMS.UpdateBulk(myChldrenflows, Permissions.WarehouseItemsEntry.UpdateGuid, ExecutionTime, DbCMS);
                        int? myOrder = DbWMS.dataItemOutputDetailFlow
                                         .Where(x => x.dataItemOutputDetail.ItemInputDetailGUID == children.ItemInputDetailGUID)
                                         .Select(x => x.OrderId).Max() + 1;
                        if (myOrder == null)
                            myOrder = 1;
                        dataItemOutputDetailFlow myoutputDetailFlow = new dataItemOutputDetailFlow();

                        myoutputDetailFlow.ItemOutputDetailFlowGUID = Guid.NewGuid();
                        myoutputDetailFlow.ItemOutputDetailGUID = myChildrendOutputDetail.ItemOutputDetailGUID;
                        if (ReleaseModel.RequesterGUID == WarehouseRequestSourceTypes.OtherRequester
                            || ReleaseModel.RequesterGUID == WarehouseRequestSourceTypes.Vehicle
                            || ReleaseModel.RequesterGUID == WarehouseRequestSourceTypes.Partner
                            || ReleaseModel.RequesterGUID == WarehouseRequestSourceTypes.GS45
                            )
                        {
                            myoutputDetailFlow.FlowTypeGUID = WarehouseRequestFlowType.Confirmed;
                        }
                        else
                        {
                            //if (model.ExpectedStartDate < DateTime.Now)
                            //{
                            //    myoutputDetailFlow.FlowTypeGUID = WarehouseRequestFlowType.Confirmed;
                            //    }
                            //else
                            //{
                            myoutputDetailFlow.FlowTypeGUID = WarehouseRequestFlowType.PendingConfirmed;

                            //}
                        }

                        myoutputDetailFlow.ItemStatuGUID = (Guid)model.ItemStatusGUID;
                        myoutputDetailFlow.CreatedDate = DateTime.Now;
                        myoutputDetailFlow.IsLastAction = true;
                        myoutputDetailFlow.IsLastMove = true;
                        myoutputDetailFlow.CreatedByGUID = UserGUID;
                        myoutputDetailFlow.Active = true;
                        myoutputDetailFlow.OrderId = myOrder;
                        DbWMS.Create(myoutputDetailFlow, Permissions.WarehouseItemsEntry.CreateGuid, ExecutionTime, DbCMS);
                        if (ReleaseModel.RequesterGUID == WarehouseRequestSourceTypes.Warehouse)
                        {
                            var childernCurrentWarehouseGUID = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID)
                                .Select(x => x.WarehouseGUID)
                                .FirstOrDefault();

                            foreach (var myTransfer in ChildreninputDetails)
                            {
                                //List<dataItemTransfer> myChildrenTransfer = DbWMS.dataItemTransfer
                                //    .Where(x => x.ItemInputDetailGUID == myTransfer.ItemInputDetailGUID).ToList();

                                var myChildrenTransfer = (from a in DbWMS.dataItemTransfer.Where(x => x.ItemInputDetailGUID == myTransfer.ItemInputDetailGUID) select a).ToList();
                                myChildrenTransfer.ForEach(x => x.IsLastTransfer = false);

                                //foreach (var transfer in myChildrenTransfer)
                                //{
                                //    transfer.IsLastTransfer = false;
                                //}

                                //DbWMS.UpdateBulk(myChildrenTransfer, Permissions.WarehouseItemsEntry.UpdateGuid, ExecutionTime,
                                //    DbCMS);
                                dataItemTransfer myTransferModel = new dataItemTransfer();
                                myTransferModel.ItemTransferGUID = Guid.NewGuid();
                                myTransferModel.ItemInputDetailGUID = myChildrendOutputDetail.ItemInputDetailGUID;
                                myTransferModel.TransferDate = ExecutionTime;
                                myTransferModel.SourceGUID = CurrentWarehouseGUID;
                                myTransferModel.DestionationGUID = ReleaseModel.RequesterNameGUID;
                                myTransferModel.TransferedByGUID = UserGUID;
                                myTransferModel.IsLastTransfer = true;
                                DbWMS.Create(myTransferModel, Permissions.WarehouseItemsEntry.CreateGuid, ExecutionTime, DbCMS);

                            }
                        }


                    }

                    if (childrenDetails.Count > 0)
                    {
                        DbWMS.CreateBulk(childrenDetails, Permissions.WarehouseItemsRelease.CreateGuid, ExecutionTime, DbCMS);
                    }
                    #endregion



                }
                DbWMS.CreateBulk(itemReleaseDetailsToAdd, Permissions.WarehouseItemsRelease.CreateGuid, ExecutionTime, DbCMS);
            }

            DbWMS.CreateBulk(toAddtrans, Permissions.WarehouseItemsEntry.CreateGuid, ExecutionTime, DbCMS);
            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();

                var currentWarehouse = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).Select(x => x.WarehouseGUID).FirstOrDefault();
                if ((ReleaseModel.RequesterGUID == WarehouseRequestSourceTypes.Warehouse
                      && currentWarehouse != ReleaseModel.RequesterNameGUID
                      ) ||
                     ReleaseModel.RequesterGUID == WarehouseRequestSourceTypes.Staff)
                {
                    int toSend = 1;
                    if (model.NotifyStaffByEmailGUID == null ||
                        model.NotifyStaffByEmailGUID == Guid.Empty
                        || model.NotifyStaffByEmailGUID == Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7854"))
                    {


                        if (ReleaseModel.RequesterGUID == WarehouseRequestSourceTypes.Staff)
                        {
                            var _staffToCheck = DbWMS.StaffCoreData.Where(x => x.UserGUID == ReleaseModel.RequesterNameGUID).
                                FirstOrDefault();
                            if (_staffToCheck != null && _staffToCheck.JobTitleGUID == Guid.Parse("9B98F32B-D27D-45A2-9E6F-6A042D884905"))
                            {
                                toSend = 0;
                            }
                        }
                        if (toSend == 1)
                        {


                            SendConfirmationReceivingModelEmailForBulk(ReleaseModel.ItemOutputGUID);
                        }
                    }


                }


                if ((ReleaseModel.RequesterGUID == WarehouseRequestSourceTypes.Staff) && (model.ItemInputModelGuids.Contains(Guid.Parse("FA27E544-A88F-4CA0-86E0-C8B4BF40AF44")) || (model.ItemInputModelGuids.Contains(Guid.Parse("56723035-36DE-4A91-8584-2B12FC4EE99C")))))
                {
                    var staff = DbWMS.StaffCoreData.Where(x => x.UserGUID == ReleaseModel.RequesterNameGUID).FirstOrDefault();
                    if (staff != null)
                    {
                        var checkSTI = DbWMS.v_StaffContactsListForSTI.Where(x => x.EmailWork == staff.EmailAddress).FirstOrDefault();
                        // var myContact = DbWMS.StaffContactsInformation.Where(x => x.Emailwork.ToLower() == myStaff.EmailAddress.ToLower()).FirstOrDefault();
                        if (checkSTI == null)
                        {
                            staff.OfficialMobileNumber = null;
                        }
                        if (checkSTI != null)
                        {
                            staff.OfficialMobileNumber = checkSTI.Mobile11;
                        }





                        DbWMS.Update(staff, Permissions.WarehouseItemsEntry.UpdateGuid, ExecutionTime, DbCMS);

                        DbWMS.SaveChanges();
                    }
                }
                var _checkStaffIssuer = DbAHD.StaffCoreData.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
                if (_checkStaffIssuer != null)
                {
                    DbWMS.sp_UpdatePhonesToHR();
                }



                return Json(DbWMS.SingleUpdateMessage(DataTableNames.WarehouseModelEntryMovementsDataTable, DbWMS.PrimaryKeyControl(ReleaseModel), DbWMS.RowVersionControls(Portal.SingleToList(ReleaseModel))));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }

        }
        #endregion
        public ActionResult WarehouseModelReleaseMovementsUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            var myDetail = DbWMS.dataItemOutputDetail.Find(PK);
            ReleaseSingleItemUpdateModalUpdateModel ReleaseDetailModel = Mapper.Map(myDetail, new ReleaseSingleItemUpdateModalUpdateModel());
            ReleaseDetailModel.RequesterGUID = (Guid)myDetail.dataItemOutput.RequesterGUID;
            ReleaseDetailModel.RequesterNameGUID = (Guid)myDetail.dataItemOutput.RequesterNameGUID;
            ReleaseDetailModel.LastFlowTypeGUID = (Guid)myDetail.dataItemInputDetail.LastFlowTypeGUID;
            ReleaseDetailModel.ItemStatusGUID = myDetail.ItemStatusGUID;
            ReleaseDetailModel.CurrentFlowTypeGUID = myDetail.dataItemOutputDetailFlow.FirstOrDefault(x => x.IsLastMove == true).FlowTypeGUID;

            ReleaseDetailModel.SIMPackageSizeGUID = myDetail.SIMPackageSizeGUID;

            ReleaseDetailModel.ItemStatusGUID = myDetail.ItemStatusGUID;
            //if (ReleaseDetailModel.RequesterGUID == WarehouseRequestSourceTypes.Staff)
            //{
            //    DbWMS.userPersonalDetailsLanguage.Where(x => x.UserGUID == myDetail.dataItemOutput.RequesterNameGUID).Select(x=)
            //}
            return PartialView("~/Areas/WMS/Views/ModelReleaseMovements/_ModelReleaseMovementsModel.cshtml", ReleaseDetailModel);
        }

        public void UpdateHRContacts()
        {

        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult WarehouseModelReleaseMovementsCreate(ReleaseSingleItemUpdateModalUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (model.ItemStatusGUID == Guid.Empty || model.ItemStatusGUID == null)
            {
                return Json(DbWMS.PermissionError());

            }
            if (model.ItemRequestTypeGUID == LookupTables.ItemRequestTypeLongTerm)
            {
                var chcekIfAny = DbWMS.dataItemReservation
                    .Where(x => x.Active &&
                                ((x.ReservationEndDate >= x.ReservationStartDate &&
                x.ReservationEndDate <= x.ReservationEndDate) ||
                                (
                                    x.ReservationEndDate <= x.ReservationStartDate &&
                                    x.ReservationEndDate >= x.ReservationEndDate
                                 )
                                 )
                                         && x.ItemInputDetailGUID == model.ItemInputDetailGUID
                                         && x.ReservedNameForGUID != model.RequesterNameGUID).FirstOrDefault();
                if (chcekIfAny != null)
                {
                    ModelState.AddModelError("Release Description", "There is Resveration in the same period ");
                    return PartialView("~/Areas/WMS/Views/ModelReleaseMovements/_ModelReleaseMovementsModel.cshtml",
                        model);
                }
            }

            if (model.ItemRequestTypeGUID == LookupTables.ItemRequestTypeShortTerm)
            {
                var reservation = DbWMS.dataItemReservation.Where(x => x.Active
                    && x.ItemInputDetailGUID == model.ItemInputDetailGUID
                    && x.ReservedNameForGUID != model.RequesterNameGUID
                    && ((x.ReservationStartDate >= model.ExpectedStartDate
                         && x.ReservationEndDate <= model.ExpectedReturenedDate
                         ))).FirstOrDefault();
                if (reservation != null)
                {
                    ModelState.AddModelError("Release Description", "There is Resveration in the same period ");
                    return PartialView("~/Areas/WMS/Views/ModelReleaseMovements/_ModelReleaseMovementsModel.cshtml",
                        model);
                }
            }
            Guid EntityPK = Guid.NewGuid();
            DateTime ExecutionTime = DateTime.Now;
            dataItemOutput ReleaseModel = Mapper.Map(model, new dataItemOutput());
            ReleaseModel.ItemOutputGUID = EntityPK;
            ReleaseModel.CreatedByGUID = UserGUID;
            ReleaseModel.CreatedDate = ExecutionTime;



            ReleaseModel.OutputNumber = DbWMS.dataItemOutput.Select(x => x.OutputNumber).Max() + 1 ?? 1;
            DbWMS.Create(ReleaseModel, Permissions.WarehouseItemsRelease.CreateGuid, ExecutionTime, DbCMS);
            dataItemOutputDetail ReleaseModelDetail = Mapper.Map(model, new dataItemOutputDetail());
            ReleaseModelDetail.ItemOutputDetailGUID = Guid.NewGuid();
            ReleaseModelDetail.SIMPackageSizeGUID = model.SIMPackageSizeGUID;
            ReleaseModelDetail.ItemStatusGUID = model.ItemStatusGUID;
            //if(model.SIMPackageSizeGUID!=null && model.SIMPackageSizeGUID != Guid.Empty)
            //{
            //    ReleaseModelDetail.SIMPackageSize =DbWMS.codeTablesValuesLanguages.Where(x=>x.ValueGUID==model.SIMPackageSizeGUID
            //    && x.LanguageID==LAN).FirstOrDefault().ValueDescription;

            //}

            if (ReleaseModelDetail.RequestedQunatity == null)
                ReleaseModelDetail.RequestedQunatity = 1;
            ReleaseModelDetail.CreatedByGUID = UserGUID;
            //if (model.ExpectedStartDate == null)
            //{
            //    ReleaseModelDetail.ExpectedStartDate = DateTime.Now;
            //}
            if (model.ItemRequestTypeGUID == null)
            {
                ReleaseModelDetail.ItemRequestTypeGUID = LookupTables.ItemRequestTypeLongTerm;
            }
            ReleaseModelDetail.ItemOutputGUID = ReleaseModel.ItemOutputGUID;
            ReleaseModelDetail.CreatedDate = ExecutionTime;
            ReleaseModelDetail.ExpectedReturenedDate = model.ExpectedReturenedDate;
            ReleaseModelDetail.Comments = ReleaseModel.Comments;
            ReleaseModelDetail.ItemTagGUID = model.ItemTagGUID;
            DbWMS.Create(ReleaseModelDetail, Permissions.WarehouseItemsRelease.CreateGuid, ExecutionTime, DbCMS);
            //chlidren 
            List<dataItemInputDetail> ChildreninputDetails = DbWMS.dataItemInputDetail
                .Where(x => x.ParentItemModelWarehouseGUID == model.ItemInputDetailGUID).ToList();
            List<dataItemOutputDetail> childrenDetails = new List<dataItemOutputDetail>();
            foreach (var item in ChildreninputDetails)
            {
                dataItemOutputDetail myChildrendOutputDetail = Mapper.Map(model, new dataItemOutputDetail());
                myChildrendOutputDetail.ItemOutputDetailGUID = Guid.NewGuid();
                if (myChildrendOutputDetail.RequestedQunatity == null)
                    myChildrendOutputDetail.RequestedQunatity = 1;
                myChildrendOutputDetail.CreatedByGUID = UserGUID;
                myChildrendOutputDetail.ItemOutputGUID = ReleaseModel.ItemOutputGUID;
                myChildrendOutputDetail.CreatedDate = ExecutionTime;
                myChildrendOutputDetail.Comments = ReleaseModel.Comments;
                myChildrendOutputDetail.ItemInputDetailGUID = item.ItemInputDetailGUID;
                childrenDetails.Add(myChildrendOutputDetail);

                if (ReleaseModel.RequesterGUID != WarehouseRequestSourceTypes.Warehouse)
                {
                    item.IsAvaliable = false;
                    item.ItemServiceStatusGUID = WarehouseServiceItemStatus.InService;
                }

                item.LastCustdianGUID = (Guid)ReleaseModel.RequesterGUID;
                item.LastCustdianNameGUID = ReleaseModel.RequesterNameGUID;
                item.LastCustodianName = CheckCustodianName((Guid)ReleaseModel.RequesterGUID, (Guid)ReleaseModel.RequesterNameGUID);
                item.ItemConditionGUID = WarehouseItemCondition.Used;
                item.ItemStatusGUID = (Guid)model.ItemStatusGUID;
                //item.LastLocationGUID = item.LastLocationGUID;
                item.LastWarehouseLocationGUID = item.LastWarehouseLocationGUID;
                item.LastFlowTypeGUID = WarehouseRequestFlowType.PendingConfirmed;
                DbWMS.Update(item, Permissions.WarehouseItemsRelease.UpdateGuid, ExecutionTime, DbCMS);
                //List<dataItemOutputDetailFlow> myChildren = DbWMS.dataItemOutputDetailFlow
                //    .Where(x => x.dataItemOutputDetail.dataItemInputDetail.ItemInputDetailGUID == item.ItemInputDetailGUID).ToList();
                var myChildren = (from a in DbWMS.dataItemOutputDetailFlow.Where(x => x.dataItemOutputDetail.dataItemInputDetail.ItemInputDetailGUID == item.ItemInputDetailGUID) select a).ToList();
                myChildren.ForEach(x => x.IsLastAction = false);
                //foreach (var flow in myChldrenflows)
                //{
                //    flow.IsLastAction = false;

                //}
                //DbWMS.UpdateBulk(myChldrenflows, Permissions.WarehouseItemsEntry.UpdateGuid, ExecutionTime, DbCMS);
                int? myOrder = DbWMS.dataItemOutputDetailFlow
                                 .Where(x => x.dataItemOutputDetail.ItemInputDetailGUID == item.ItemInputDetailGUID)
                                 .Select(x => x.OrderId).Max() + 1;
                if (myOrder == null)
                    myOrder = 1;
                dataItemOutputDetailFlow myoutputDetailFlow = new dataItemOutputDetailFlow();
                myoutputDetailFlow.ItemOutputDetailFlowGUID = Guid.NewGuid();
                myoutputDetailFlow.ItemOutputDetailGUID = myChildrendOutputDetail.ItemOutputDetailGUID;
                myoutputDetailFlow.FlowTypeGUID = WarehouseRequestFlowType.PendingConfirmed;
                myoutputDetailFlow.ItemStatuGUID = WarehouseItemStatus.Functionting;
                myoutputDetailFlow.CreatedDate = DateTime.Now;
                myoutputDetailFlow.IsLastAction = true;
                myoutputDetailFlow.IsLastMove = true;
                myoutputDetailFlow.CreatedByGUID = UserGUID;
                myoutputDetailFlow.Active = true;
                myoutputDetailFlow.OrderId = myOrder;
                DbWMS.Create(myoutputDetailFlow, Permissions.WarehouseItemsEntry.CreateGuid, ExecutionTime, DbCMS);
                if (ReleaseModel.RequesterGUID == WarehouseRequestSourceTypes.Warehouse)
                {
                    var CurrentWarehouseGUID = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID)
                        .Select(x => x.WarehouseGUID)
                        .FirstOrDefault();
                    foreach (var myTransfer in ChildreninputDetails)
                    {

                        var myChildrenTransfer = (from a in DbWMS.dataItemTransfer.Where(x => x.ItemInputDetailGUID == myTransfer.ItemInputDetailGUID) select a).ToList();
                        myChildrenTransfer.ForEach(x => x.IsLastTransfer = false);
                        dataItemTransfer myTransferModel = new dataItemTransfer();
                        myTransferModel.ItemTransferGUID = Guid.NewGuid();
                        myTransferModel.ItemInputDetailGUID = myChildrendOutputDetail.ItemInputDetailGUID;
                        myTransferModel.TransferDate = ExecutionTime;
                        myTransferModel.SourceGUID = CurrentWarehouseGUID;
                        myTransferModel.DestionationGUID = ReleaseModel.RequesterNameGUID;
                        myTransferModel.TransferedByGUID = UserGUID;
                        myTransferModel.IsLastTransfer = true;
                        DbWMS.Create(myTransferModel, Permissions.WarehouseItemsEntry.CreateGuid, ExecutionTime, DbCMS);

                    }
                }


            }

            if (childrenDetails.Count > 0)
            {
                DbWMS.CreateBulk(childrenDetails, Permissions.WarehouseItemsRelease.CreateGuid, ExecutionTime, DbCMS);
            }


            dataItemInputDetail InputDetail = DbWMS.dataItemInputDetail.Where(x => x.ItemInputDetailGUID == ReleaseModelDetail.ItemInputDetailGUID).FirstOrDefault();
            //dataItemOutput itemOutput = DbWMS.dataItemOutput.Find(ReleaseModelDetail.ItemOutputGUID);
            if (ReleaseModel.RequesterGUID != WarehouseRequestSourceTypes.Warehouse)
            {
                InputDetail.IsAvaliable = false;
                InputDetail.ItemServiceStatusGUID = WarehouseServiceItemStatus.InService;

            }
            //if (ReleaseModel.RequesterGUID == WarehouseRequestSourceTypes.OtherRequester)
            //{
            //    if (ReleaseModel.RequesterNameGUID == Guid.Parse("0A31225F-2064-4F02-93D2-FE1519CF6AC7"))
            //    {
            //        InputDetail.ItemStatusGUID = WarehouseItemStatus.Disposed;
            //        InputDetail.ItemServiceStatusGUID = WarehouseServiceItemStatus.Disposed;
            //        InputDetail.IsAvaliable = false;
            //    }
            //}
            InputDetail.ItemStatusGUID = (Guid)model.ItemStatusGUID;

            if (ReleaseModel.RequesterGUID == WarehouseRequestSourceTypes.GS45)
            {

                if (ReleaseModel.RequesterNameGUID == Guid.Parse("0A31225F-2064-4F02-93D2-FE1519CF6AC7"))
                {

                    InputDetail.ItemServiceStatusGUID = WarehouseServiceItemStatus.GS45;

                }
            }

            InputDetail.LastCustdianGUID = (Guid)ReleaseModel.RequesterGUID;
            //InputDetail.LastLocationGUID = model.LocationGUID;
            InputDetail.LastWarehouseLocationGUID = model.WarehouseLocationGUID;
            InputDetail.LastCustdianNameGUID = ReleaseModel.RequesterNameGUID;

            InputDetail.LastExpiryDate = model.LicenseExpiryDate;
            InputDetail.LastIssueDate = model.LicenseStartDate;

            InputDetail.LastCustdianNameGUID = ReleaseModel.RequesterNameGUID;
            InputDetail.LastCustodianName = CheckCustodianName((Guid)ReleaseModel.RequesterGUID, (Guid)ReleaseModel.RequesterNameGUID);
            if (ReleaseModel.RequesterGUID != WarehouseRequestSourceTypes.Warehouse)
            {
                InputDetail.ItemConditionGUID = WarehouseItemCondition.Used;
            }
            InputDetail.LastCustodianName = CheckCustodianName((Guid)ReleaseModel.RequesterGUID, (Guid)ReleaseModel.RequesterNameGUID);

            if (ReleaseModel.RequesterGUID == WarehouseRequestSourceTypes.OtherRequester
                || ReleaseModel.RequesterGUID == WarehouseRequestSourceTypes.Vehicle
                || ReleaseModel.RequesterGUID == WarehouseRequestSourceTypes.Partner
                || ReleaseModel.RequesterGUID == WarehouseRequestSourceTypes.GS45)
            {
                InputDetail.LastFlowTypeGUID = WarehouseRequestFlowType.Confirmed;
            }
            else
            {
                InputDetail.LastFlowTypeGUID = WarehouseRequestFlowType.PendingConfirmed;
            }
            if (ReleaseModel.RequesterGUID == WarehouseRequestSourceTypes.GS45)
            {
                InputDetail.ItemServiceStatusGUID = WarehouseServiceItemStatus.GS45;
            }
            DbWMS.Update(InputDetail, Permissions.WarehouseItemsRelease.UpdateGuid, ExecutionTime, DbCMS);

            //List<dataItemOutputDetailFlow> flows = DbWMS.dataItemOutputDetailFlow
            //    .Where(x => x.dataItemOutputDetail.dataItemInputDetail.ItemInputDetailGUID == ReleaseModelDetail.ItemInputDetailGUID).ToList();

            var flows = (from a in DbWMS.dataItemOutputDetailFlow.Where(x => x.dataItemOutputDetail.dataItemInputDetail.ItemInputDetailGUID == ReleaseModelDetail.ItemInputDetailGUID) select a).ToList();
            flows.ForEach(x => x.IsLastAction = false);
            int? order = DbWMS.dataItemOutputDetailFlow
                .Where(x => x.dataItemOutputDetail.ItemInputDetailGUID == ReleaseModelDetail.ItemInputDetailGUID)
                .Select(x => x.OrderId).Max() + 1;
            if (order == null)
                order = 1;
            dataItemOutputDetailFlow outputDetailFlow = new dataItemOutputDetailFlow();

            outputDetailFlow.ItemOutputDetailFlowGUID = Guid.NewGuid();
            outputDetailFlow.ItemOutputDetailGUID = ReleaseModelDetail.ItemOutputDetailGUID;

            if (ReleaseModel.RequesterGUID == WarehouseRequestSourceTypes.OtherRequester
                || ReleaseModel.RequesterGUID == WarehouseRequestSourceTypes.Vehicle ||
                ReleaseModel.RequesterGUID == WarehouseRequestSourceTypes.Partner ||
                ReleaseModel.RequesterGUID == WarehouseRequestSourceTypes.GS45)
            {
                outputDetailFlow.FlowTypeGUID = WarehouseRequestFlowType.Confirmed;
            }
            else
            {
                outputDetailFlow.FlowTypeGUID = WarehouseRequestFlowType.PendingConfirmed;

            }



            outputDetailFlow.ItemStatuGUID = (Guid)model.ItemStatusGUID;
            outputDetailFlow.CreatedDate = DateTime.Now;
            outputDetailFlow.IsLastAction = true;
            outputDetailFlow.IsLastMove = true;
            outputDetailFlow.CreatedByGUID = UserGUID;
            outputDetailFlow.Active = true;
            outputDetailFlow.OrderId = order;

            DbWMS.Create(outputDetailFlow, Permissions.WarehouseItemsEntry.CreateGuid, ExecutionTime, DbCMS);
            var myCurrentWarehouseGUID = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).Select(x => x.WarehouseGUID)
                .FirstOrDefault();

            var Chcektransfer = DbWMS.dataItemTransfer.Where(x => x.ItemInputDetailGUID == InputDetail.ItemInputDetailGUID
                                                             && x.IsLastTransfer == true).FirstOrDefault();
            if (((Chcektransfer.codeWarehouse.WarehouseLocationGUID != model.WarehouseLocationGUID) || (ReleaseModel.RequesterGUID == WarehouseRequestSourceTypes.Warehouse)))
            {
                Guid inputDetailGUid = InputDetail.ItemInputDetailGUID;
                //List<dataItemTransfer> allPriviousTransfers = DbWMS.dataItemTransfer
                //    .Where(x => x.ItemInputDetailGUID == ReleaseModelDetail.ItemInputDetailGUID
                //                && x.IsLastTransfer==true).ToList();


                var allPriviousTransfers = (from a in DbWMS.dataItemTransfer.Where(x => x.ItemInputDetailGUID == ReleaseModelDetail.ItemInputDetailGUID
                                                                                   && x.IsLastTransfer == true)
                                            select a).ToList();
                allPriviousTransfers.ForEach(x => x.IsLastTransfer = false);


                //foreach (var item in allPriviousTransfers)
                //{
                //    item.IsLastTransfer = false;
                //    //DbWMS.SaveChanges();
                //   DbWMS.Update(item, Permissions.WarehouseItemsEntry.UpdateGuid, ExecutionTime, DbCMS);
                //    DbWMS.SaveChanges();
                //}
                //List<dataItemTransfer> update = AutoMapper.Mapper.Map(allPriviousTransfers, new List<dataItemTransfer>());
                //DbCMS.UpdateBulk(update, Permissions.WarehouseItemsRelease.UpdateGuid, ExecutionTime);


                var warehouse = DbWMS.codeWarehouse.Where(x => x.WarehouseLocationGUID == model.WarehouseLocationGUID).FirstOrDefault();
                dataItemTransfer changeTransferModel = new dataItemTransfer();
                changeTransferModel.ItemTransferGUID = Guid.NewGuid();
                changeTransferModel.ItemInputDetailGUID = ReleaseModelDetail.ItemInputDetailGUID;
                changeTransferModel.TransferDate = ExecutionTime;
                changeTransferModel.SourceGUID = myCurrentWarehouseGUID;

                changeTransferModel.DestionationGUID = warehouse.WarehouseGUID;
                changeTransferModel.TransferedByGUID = UserGUID;
                changeTransferModel.IsLastTransfer = true;
                DbWMS.Create(changeTransferModel, Permissions.WarehouseItemsEntry.CreateGuid, ExecutionTime, DbCMS);
                //  DbWMS.SaveChanges();

            }


            try
            {

                DbWMS.SaveChanges();
                DbCMS.SaveChanges();

                var currentWarehouse = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).Select(x => x.WarehouseGUID).FirstOrDefault();
                if ((ReleaseModelDetail.dataItemOutput.RequesterGUID == WarehouseRequestSourceTypes.Warehouse
                         && currentWarehouse != ReleaseModelDetail.dataItemOutput.RequesterNameGUID) ||
                   ReleaseModelDetail.dataItemOutput.RequesterGUID == WarehouseRequestSourceTypes.Staff)
                {
                    int toSend = 1;
                    if (model.NotifyStaffByEmailGUID == null ||
                        model.NotifyStaffByEmailGUID == Guid.Empty
                        || model.NotifyStaffByEmailGUID == Guid.Parse("66cd375c-a576-4aa4-8af4-ff3c1c5e7854"))
                    {


                        if (ReleaseModelDetail.dataItemOutput.RequesterGUID == WarehouseRequestSourceTypes.Staff)
                        {
                            var _staffToCheck = DbWMS.StaffCoreData.Where(x => x.UserGUID == ReleaseModel.RequesterNameGUID).
                                FirstOrDefault();
                            if (_staffToCheck != null && _staffToCheck.JobTitleGUID == Guid.Parse("9B98F32B-D27D-45A2-9E6F-6A042D884905"))
                            {
                                toSend = 0;
                            }
                        }
                        if (toSend == 1)
                        {


                            SendConfirmationReceivingModelEmail(ReleaseModelDetail);
                        }
                    }


                }




                var staff = DbWMS.StaffCoreData.Where(x => x.UserGUID == InputDetail.LastCustdianNameGUID).FirstOrDefault();
                if (staff != null)
                {
                    var checkSTI = DbWMS.v_StaffContactsListForSTI.Where(x => x.EmailWork.ToLower() == staff.EmailAddress.ToLower()).FirstOrDefault();
                    // var myContact = DbWMS.StaffContactsInformation.Where(x => x.Emailwork.ToLower() == myStaff.EmailAddress.ToLower()).FirstOrDefault();
                    if (checkSTI == null)
                    {
                        staff.OfficialMobileNumber = null;
                    }
                    if (checkSTI != null)
                    {
                        staff.OfficialMobileNumber = checkSTI.Mobile11;
                    }



                    DbWMS.Update(staff, Permissions.WarehouseItemsEntry.UpdateGuid, ExecutionTime, DbCMS);

                    DbWMS.SaveChanges();
                }
                var _checkStaffIssuer = DbAHD.StaffCoreData.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
                if (_checkStaffIssuer != null)
                {
                    DbWMS.sp_UpdatePhonesToHR();
                }


                string callBackFunc = "CheckCreateNewRelease('1')";
                return Json(DbCMS.SingleUpdateMessage(DataTableNames.WarehouseModelReleaseMovementsDataTable, DbWMS.PrimaryKeyControl(ReleaseModelDetail), DbWMS.RowVersionControls(Portal.SingleToList(ReleaseModelDetail)), callBackFunc));

                //ViewBag.IsReleased = true;
                //return Json(DbWMS.SingleUpdateMessage(DataTableNames.WarehouseModelReleaseMovementsDataTable, DbWMS.PrimaryKeyControl(ReleaseModelDetail), DbWMS.RowVersionControls(Portal.SingleToList(ReleaseModelDetail))));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }

        }

        public string CheckCustodianName(Guid typeGUID, Guid nameGUID)
        {
            string str = "";
            if (typeGUID == WarehouseRequestSourceTypes.Warehouse)
            {
                str = DbWMS.codeWarehouseLanguage.Where(x => x.WarehouseGUID == nameGUID && x.LanguageID == LAN).Select(x => x.WarehouseDescription).FirstOrDefault();
            }
            if (typeGUID == WarehouseRequestSourceTypes.Staff)
            {
                str = DbWMS.userPersonalDetailsLanguage.Where(x => x.UserGUID == nameGUID && x.LanguageID == LAN).Select(x => x.FirstName).FirstOrDefault() + " " + DbWMS.userPersonalDetailsLanguage.Where(x => x.UserGUID == nameGUID && x.LanguageID == LAN).Select(x => x.Surname).FirstOrDefault();
            }
            if (typeGUID == WarehouseRequestSourceTypes.OtherRequester)
            {
                str = DbWMS.codeWarehouseRequesterTypeLanguage.Where(x => x.WarehouseRequesterTypeGUID == nameGUID && x.LanguageID == LAN).Select(x => x.WarehouseRequesterTypeDescription).FirstOrDefault();
            }
            if (typeGUID == WarehouseRequestSourceTypes.Vehicle)
            {
                str = DbWMS.codeWarehouseVehicleLanguage.Where(x => x.WarehouseVehicleGUID == nameGUID && x.LanguageID == LAN).Select(x => x.VehicleDescription).FirstOrDefault();
            }
            if (typeGUID == WarehouseRequestSourceTypes.Partner)
            {
                str = DbWMS.codeWMSPartner.Where(x => x.PartnerGUID == nameGUID).Select(x => x.PartnerName).FirstOrDefault();
            }
            if (typeGUID == WarehouseRequestSourceTypes.GS45)
            {
                str = DbWMS.codeTablesValuesLanguages.Where(x => x.ValueGUID == nameGUID).Select(x => x.ValueDescription).FirstOrDefault();
            }
            return str;
        }



        #endregion

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult WarehouseModelReleaseMovementsUpdate(ReleaseSingleItemUpdateModalUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Update, Apps.WMS) || model.ItemStatusGUID == null || model.ItemStatusGUID == Guid.Empty)
            {
                return Json(DbWMS.PermissionError());
            }
            // if (!ModelState.IsValid || ActiveReleaseModel(model)) return PartialView("~/Areas/WMS/Views/ReleaseModels/_ReleaseModelForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            var output = DbWMS.dataItemOutput.Find(model.ItemOutputGUID);
            output.RequesterGUID = model.RequesterGUID;
            output.RequesterNameGUID = model.RequesterNameGUID;
            var outputdetail = DbWMS.dataItemOutputDetail.Find(model.ItemOutputDetailGUID);
            outputdetail.ExpectedStartDate = model.ExpectedStartDate;
            outputdetail.ExpectedReturenedDate = model.ExpectedReturenedDate;

            outputdetail.LocationGUID = model.LocationGUID;
            outputdetail.SIMPackageSizeGUID = model.SIMPackageSizeGUID;

            if (model.SIMPackageSizeGUID != null && model.SIMPackageSizeGUID != Guid.Empty)
            {
                outputdetail.SIMPackageSize = DbWMS.codeTablesValuesLanguages.Where(x => x.ValueGUID == model.SIMPackageSizeGUID
                     && x.LanguageID == LAN).FirstOrDefault().ValueDescription;

            }

            //outputdetail.PackageSize = model.PackageSize;
            //outputdetail.SIMPackageSize = model.SIMPackageSize;
            outputdetail.HasBag = model.HasBag;
            outputdetail.HasCharger = model.HasCharger;
            outputdetail.HasHeadPhone = model.HasHeadPhone;
            outputdetail.HasLaptopMouse = model.HasLaptopMouse;
            outputdetail.HasRoaming = model.HasRoaming;
            outputdetail.HasInternationalAccess = model.HasInternationalAccess;

            outputdetail.HasHeadsetUSB = model.HasHeadsetUSB;
            outputdetail.HasHeadsetStereoJack = model.HasHeadsetStereoJack;
            outputdetail.HasBackbag = model.HasBackbag;
            outputdetail.HasHandbag = model.HasHandbag;
            outputdetail.HasFlashMemory = model.HasFlashMemory;
            outputdetail.ItemTagGUID = model.ItemTagGUID;
            outputdetail.ItemStatusGUID = (Guid)model.ItemStatusGUID;



            outputdetail.WarehouseLocationGUID = model.WarehouseLocationGUID;
            outputdetail.ActualReturenedDate = model.ActualReturenedDate;
            //if (model.Comments != null)
            outputdetail.Comments = model.Comments;
            var flow = DbWMS.dataItemOutputDetailFlow.Where(x => x.ItemOutputDetailGUID == model.ItemOutputDetailGUID
                                                      && x.Active && x.IsLastAction == true).FirstOrDefault();
            if (flow != null && (flow.FlowTypeGUID == Guid.Parse("0C44822F-A898-476D-B291-CAF1B055AAC6") || flow.FlowTypeGUID == Guid.Parse("0C44822F-A898-476D-B291-CAF1B055AAC2")))
            {
                var input = DbWMS.dataItemInputDetail.Find(model.ItemInputDetailGUID);
                input.LastCustdianGUID = model.RequesterGUID;
                input.LastCustdianNameGUID = model.RequesterNameGUID;
                input.LastCustodianName = CheckCustodianName((Guid)model.RequesterGUID, (Guid)model.RequesterNameGUID);
                input.LastWarehouseLocationGUID = model.WarehouseLocationGUID;
                DbWMS.Update(input, Permissions.WarehouseItemsRelease.UpdateGuid, ExecutionTime, DbCMS);


                if (flow != null && (input.LastWarehouseLocationGUID != model.WarehouseLocationGUID || (
                        model.RequesterGUID == WarehouseRequestSourceTypes.Warehouse)))
                {
                    List<dataItemTransfer> allPriviousTransfers = DbWMS.dataItemTransfer
                        .Where(x => x.ItemInputDetailGUID == input.ItemInputDetailGUID).ToList();
                    var myCurrentWarehouseGUID = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).Select(x => x.WarehouseGUID)
                        .FirstOrDefault();
                    List<dataItemTransfer> toAddtrans = new List<dataItemTransfer>();
                    foreach (var tran in allPriviousTransfers)
                    {
                        tran.IsLastTransfer = false;
                    }
                    DbWMS.UpdateBulk(allPriviousTransfers, Permissions.WarehouseItemsEntry.UpdateGuid, ExecutionTime, DbCMS);
                    var warehouse = DbWMS.codeWarehouse.Where(x => x.WarehouseLocationGUID == model.WarehouseLocationGUID).FirstOrDefault();
                    dataItemTransfer changeTransferModel = new dataItemTransfer();
                    changeTransferModel.ItemTransferGUID = Guid.NewGuid();
                    changeTransferModel.ItemInputDetailGUID = input.ItemInputDetailGUID;
                    changeTransferModel.TransferDate = ExecutionTime;
                    changeTransferModel.SourceGUID = myCurrentWarehouseGUID;
                    changeTransferModel.DestionationGUID = warehouse.WarehouseGUID;
                    changeTransferModel.TransferedByGUID = UserGUID;
                    changeTransferModel.IsLastTransfer = true;
                    DbWMS.Create(changeTransferModel, Permissions.WarehouseItemsRelease.CreateGuid, ExecutionTime, DbCMS);


                }
            }
            DbWMS.Update(outputdetail, Permissions.WarehouseItemsRelease.UpdateGuid, ExecutionTime, DbCMS);
            var myInputmodel = DbWMS.dataItemInputDetail.Where(x => x.ItemInputDetailGUID == model.ItemInputDetailGUID).FirstOrDefault();
            if (myInputmodel.LastCustdianNameGUID == model.RequesterNameGUID)
            {
                myInputmodel.ItemStatusGUID = (Guid)model.ItemStatusGUID;
                DbWMS.Update(myInputmodel, Permissions.WarehouseItemsRelease.UpdateGuid, ExecutionTime, DbCMS);
            }

            // List<dataItemTransfer> update = AutoMapper.Mapper.Map(allPriviousTransfers, new List<dataItemTransfer>());

            DbWMS.Update(output, Permissions.WarehouseItemsRelease.UpdateGuid, ExecutionTime, DbCMS);


            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                if (output.RequesterGUID == WarehouseRequestSourceTypes.Warehouse ||
                    output.RequesterGUID == WarehouseRequestSourceTypes.Staff)
                {
                    //if (flow != null && (flow.FlowTypeGUID == Guid.Parse("0C44822F-A898-476D-B291-CAF1B055AAC6")))
                    //{
                    //    //sx
                    //    SendConfirmationReceivingModelEmail(outputdetail);
                    //}
                }
                return Json(DbWMS.SingleUpdateMessage("WarehouseModelReleaseMovementsDataTable", null, DbWMS.RowVersionControls(Portal.SingleToList(output))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyReleaseModel((Guid)model.ItemOutputDetailGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ReleaseModelRestore(dataItemOutputDetail model)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Restore, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (ActiveReleaseModel(model))
            {
                return Json(DbWMS.RecordExists());
            }

            List<dataItemOutputDetail> RestoredReleaseModel = RestoreReleaseModel(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.WarehouseItemsRelease.Create, Apps.WMS, new UrlHelper(Request.RequestContext).Action("WarehouseModelReleaseMovementsCreate", "ModelMovements", new { Area = "WMS" })), Container = "ReleaseModelFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.WarehouseItemsRelease.Update, Apps.WMS), Container = "ReleaseModelFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.WarehouseItemsRelease.Delete, Apps.WMS), Container = "ReleaseModelFormControls" });

            try
            {
                int CommitedRows = DbWMS.SaveChanges();
                DbWMS.SaveChanges();
                return Json(DbWMS.SingleRestoreMessage(CommitedRows, RestoredReleaseModel, DbWMS.PrimaryKeyControl(RestoredReleaseModel.FirstOrDefault()), null, null, UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyReleaseModel(model.ItemOutputDetailGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]


        public JsonResult WarehouseModelReleaseMovementsDataTableDelete(List<dataItemOutputDetail> models)
        {

            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Delete, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            List<dataItemOutputDetail> DeletedReleaseModel = DeleteReleaseModel(models);
            var itemOutputGuids = DeletedReleaseModel.Select(x => x.ItemOutputDetailGUID).ToList();
            var myflows = DbWMS.dataItemOutputDetailFlow.Where(x => itemOutputGuids.Contains((Guid)x.ItemOutputDetailGUID))
                .ToList();
            var myNotificaiton = DbWMS.dataItemOutputNotification.Where(x => itemOutputGuids.Contains((Guid)x.ItemOutputDetailGUID))
              .ToList();
            //zzz
            var CurrentWarehouseGUID = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).Select(x => x.WarehouseGUID)
                .FirstOrDefault();
            DateTime ExecutionTime = DateTime.Now;
            foreach (var myItem in itemOutputGuids)
            {
                var itemOutputDetail = DbWMS.dataItemOutputDetail.Where(x => x.ItemOutputDetailGUID == myItem)
                    .FirstOrDefault();
                var itemInput = DbWMS.dataItemInputDetail
                    .Where(x => x.ItemInputDetailGUID == itemOutputDetail.ItemInputDetailGUID)
                    .FirstOrDefault();

                var detMax = myflows.Where(x => x.ItemOutputDetailGUID == myItem).Select(x => x.OrderId).Max();
                dataItemOutputDetailFlow CurrentOne = myflows
                    .Where(x => x.ItemOutputDetailGUID == myItem && x.OrderId == detMax).FirstOrDefault();

                if (CurrentOne.FlowTypeGUID == WarehouseRequestFlowType.PendingConfirmed ||
                    CurrentOne.FlowTypeGUID == WarehouseRequestFlowType.Confirmed)
                {
                    List<dataItemOutputDetailFlow> allFlows =
                        myflows.Where(x => x.ItemOutputDetailGUID == myItem).ToList();
                    DbWMS.dataItemOutputDetailFlow.RemoveRange(allFlows);
                    DbWMS.dataItemOutputDetail.Remove(itemOutputDetail);

                    itemInput.LastCustdianGUID = WarehouseRequestSourceTypes.Warehouse;
                    itemInput.LastFlowTypeGUID = WarehouseRequestFlowType.Confirmed;
                    itemInput.LastCustdianNameGUID = CurrentWarehouseGUID;
                    itemInput.WarehouseOwnerGUID = CurrentWarehouseGUID;
                    itemInput.IsAvaliable = true;
                    itemInput.ItemServiceStatusGUID = WarehouseServiceItemStatus.InStock; ;
                    DbWMS.Update(itemInput, Permissions.WarehouseItemsRelease.UpdateGuid, ExecutionTime, DbCMS);
                }

                if (CurrentOne.FlowTypeGUID == WarehouseRequestFlowType.Returned)
                {
                    List<dataItemOutputDetailFlow> allFlows =
                        myflows.Where(x => x.ItemOutputDetailGUID == myItem).ToList();
                    DbWMS.dataItemOutputDetailFlow.RemoveRange(allFlows);
                    DbWMS.dataItemOutputDetail.Remove(itemOutputDetail);
                    DbWMS.dataItemOutputNotification.RemoveRange(myNotificaiton);


                }
            }



            //DbWMS.UpdateBulk(flows, Permissions.WarehouseItemsEntry.UpdateGuid, ExecutionTime, DbCMS);

            //here we have to check item status if avaiable or not and change status
            // dataItemInputDetail intputDetail=DbWMS.dataItemInputDetail.Find()
            //models.Where(x=>x.)
            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.PartialDeleteMessage(DeletedReleaseModel, models, DataTableNames.WarehouseModelReleaseMovementsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult WarehouseModelReleaseMovementsRestore(List<dataItemOutputDetail> models)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Restore, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            List<dataItemOutputDetail> RestoredReleaseModel = RestoreReleaseModel(models);

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.PartialRestoreMessage(RestoredReleaseModel, models, DataTableNames.WarehouseModelReleaseMovementsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        private List<dataItemOutputDetail> DeleteReleaseModel(List<dataItemOutputDetail> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<dataItemOutputDetail> DeletedReleaseModel = new List<dataItemOutputDetail>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbWMS.QueryBuilder(models, Permissions.WarehouseItemsRelease.DeleteGuid, SubmitTypes.Delete, baseQuery);

            var Records = DbWMS.Database.SqlQuery<dataItemOutputDetail>(query).ToList();
            foreach (var record in Records)
            {
                DeletedReleaseModel.Add(DbWMS.Delete(record, ExecutionTime, Permissions.WarehouseItemsRelease.DeleteGuid, DbCMS));
            }

            //var Languages = DeletedReleaseModel.SelectMany(a => a.dataItemOutputLanguage).Where(l => l.Active).ToList();
            //foreach (var language in Languages)
            //{
            //    DbWMS.Delete(language, ExecutionTime, Permissions.WarehouseItemsRelease.DeleteGuid, DbCMS);
            //}
            return DeletedReleaseModel;
        }
        private List<dataItemOutputDetail> RestoreReleaseModel(List<dataItemOutputDetail> models)
        {

            DateTime RestoringTime = DateTime.Now;

            List<dataItemOutputDetail> RestoredReleaseModel = new List<dataItemOutputDetail>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbWMS.QueryBuilder(models, Permissions.WarehouseItemsRelease.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Records = DbWMS.Database.SqlQuery<dataItemOutputDetail>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveDamagedModel(record))
                {
                    RestoredReleaseModel.Add(DbWMS.Restore(record, Permissions.WarehouseItemsRelease.DeleteGuid, Permissions.WarehouseItemsRelease.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            //var Languages = RestoredReleaseModel.SelectMany(x => x.dataItemOutputLanguage.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            //foreach (var language in Languages)
            //{
            //    DbWMS.Restore(language, Permissions.WarehouseItemsRelease.DeleteGuid, Permissions.WarehouseItemsRelease.RestoreGuid, RestoringTime, DbCMS);
            //}

            return RestoredReleaseModel;
        }

        private JsonResult ConcurrencyReleaseModel(Guid PK)
        {
            dataItemOutputDetail dbModel = new dataItemOutputDetail();

            var ReleaseModel = DbWMS.dataItemOutputDetail.Where(x => x.ItemOutputDetailGUID == PK).FirstOrDefault();
            var dbReleaseModel = DbWMS.Entry(ReleaseModel).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbReleaseModel, dbModel);

            //var Language = DbWMS.dataItemOutputLanguage.Where(x => x.ItemOutputGUID == PK).Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.dataItemOutput.DeletedOn) && x.LanguageID == LAN).FirstOrDefault();
            //var dbLanguage = DbWMS.Entry(Language).GetDatabaseValues().ToObject();
            //dbModel = Mapper.Map(dbLanguage, dbModel);

            //if (ReleaseModel.dataItemOutputRowVersion.SequenceEqual(dbModel.dataItemOutputRowVersion) && Language.dataItemOutputLanguageRowVersion.SequenceEqual(dbModel.dataItemOutputLanguageRowVersion))
            //{
            //    return Json(DbWMS.PermissionError());
            //}

            return Json(JsonMessages.ConcurrencyError(DbWMS, dbModel, "ReleaseModelDetailsContainer"));
        }

        private bool ActiveReleaseModel(Object model)
        {
            dataItemOutputDetail ReleaseModel = Mapper.Map(model, new dataItemOutputDetail());
            int ModelDescription = DbWMS.dataItemOutputDetail
                .Where(x => x.ItemInputDetailGUID == ReleaseModel.ItemInputDetailGUID &&
                            x.ExpectedStartDate == ReleaseModel.ExpectedStartDate &&



                            x.Active).Count();
            if (ModelDescription > 0)
            {
                ModelState.AddModelError("Release Description", "This record is already exists");
            }
            return (ModelDescription > 0);
        }
        #endregion

        #region Flow Details



        public ActionResult WarehosueReleaseModelFlowCreate(Guid? PK)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            return PartialView("~/Areas/WMS/Views/ReleaseModelFlows/_ReleaseModelFlowUpdateModal.cshtml",
                   new ItemOutputDetailFlowModel { ItemOutputDetailGUID = PK });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult WarehosueReleaseModelFlowCreate(ItemOutputDetailFlowModel model)
        {
            if (model.CreatedDate == null || model.ItemStatusGUID == null || model.ItemStatusGUID == Guid.Empty)
            {
                return PartialView("~/Areas/WMS/Views/ReleaseModelFlows/_ReleaseModelFlowUpdateModal.cshtml",
                    new ItemOutputDetailFlowModel { ItemOutputDetailGUID = model.ItemOutputDetailGUID });
            }
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            dataItemOutputDetailFlow myModel = Mapper.Map(model, new dataItemOutputDetailFlow());
            List<dataItemTransfer> toAddTransfer = new List<dataItemTransfer>();

            // if (!ModelState.IsValid || ActiveDamagedModelFlow(model)) return PartialView("~/Areas/WMS/Views/ReleaseModelFlows/_ReleaseModelFlowUpdateModal.cshtml", model);
            DateTime ExecutionTime = DateTime.Now;
            dataItemOutputDetail outputDetail = DbWMS.dataItemOutputDetail.Find(myModel.ItemOutputDetailGUID);
            dataItemInputDetail input = DbWMS.dataItemInputDetail.Find(outputDetail.ItemInputDetailGUID);

            var _oldItemModelWarehouseGUID = input.ItemModelWarehouseGUID;
            var _oldLastCustdianGUID = input.LastCustdianGUID;
            var _olLastCustdianNameGUID = input.LastCustdianNameGUID;


            var _oldInput = DbWMS.dataItemInputDetail.Where(x => x.ItemInputDetailGUID == outputDetail.ItemInputDetailGUID).FirstOrDefault();


            int? order = DbWMS.dataItemOutputDetailFlow
                             .Where(x => x.dataItemOutputDetail.ItemInputDetailGUID == outputDetail.ItemInputDetailGUID)
                             .Select(x => x.OrderId).Max() + 1;
            if (order == null)
                order = 1;
            List<dataItemOutputDetailFlow> flows = DbWMS.dataItemOutputDetailFlow
                .Where(x => x.ItemOutputDetailGUID == myModel.ItemOutputDetailGUID).ToList();
            foreach (var item in flows)
            {
                item.IsLastAction = false;
                item.IsLastMove = false;
            }
            var myCurrentWarehouseGUID = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).Select(x => x.WarehouseGUID)
              .FirstOrDefault();
            var mywarehosue = DbWMS.codeWarehouseLanguage.Where(x => x.WarehouseGUID == myCurrentWarehouseGUID && x.LanguageID == LAN).FirstOrDefault();
            //FOR PARENTS
            //XXX
            var myoutputDetail = DbWMS.dataItemOutputDetail
                .Where(x => x.ItemOutputDetailGUID == myModel.ItemOutputDetailGUID).FirstOrDefault();
            //List<dataItemInputDetail> ItemInputs = DbWMS.dataItemInputDetail
            //    .Where(x => x.ParentItemModelWarehouseGUID == myoutputDetail.ItemInputDetailGUID).ToList();
            List<dataItemOutputDetail> childrenDetails = DbWMS.dataItemOutputDetail.Where(
                x => x.dataItemInputDetail.ParentItemModelWarehouseGUID == myoutputDetail.ItemInputDetailGUID
            ).ToList();
            foreach (var myOutputChildren in childrenDetails.Where(x => x.ItemOutputDetailGUID != myModel.ItemOutputDetailGUID))
            {
                List<dataItemOutputDetailFlow> myflows = DbWMS.dataItemOutputDetailFlow
                    .Where(x => x.ItemOutputDetailGUID == myOutputChildren.ItemOutputDetailGUID).ToList();
                foreach (var flowchildren in myflows)
                {
                    flowchildren.IsLastAction = false;
                    flowchildren.IsLastMove = false;
                }
                DbWMS.SaveChanges();
                int? myorder = DbWMS.dataItemOutputDetailFlow
                                 .Where(x => x.dataItemOutputDetail.ItemInputDetailGUID == myOutputChildren.ItemInputDetailGUID)
                                 .Select(x => x.OrderId).Max() + 1;
                if (myorder == null)
                    myorder = 1;
                dataItemOutputDetailFlow myFlowDetailFlow = new dataItemOutputDetailFlow();
                myFlowDetailFlow.ItemOutputDetailFlowGUID = Guid.NewGuid();
                myFlowDetailFlow.ItemOutputDetailGUID = myOutputChildren.ItemOutputDetailGUID;
                myFlowDetailFlow.FlowTypeGUID = WarehouseRequestFlowType.Returned;
                myFlowDetailFlow.CreatedDate = ExecutionTime;
                myFlowDetailFlow.ItemStatuGUID = WarehouseItemStatus.Functionting;
                myFlowDetailFlow.IsLastAction = true;
                myFlowDetailFlow.IsLastMove = true;
                myFlowDetailFlow.OrderId = myorder;
                myFlowDetailFlow.CreatedByGUID = UserGUID;
                myFlowDetailFlow.Active = true;
                DbWMS.CreateNoAudit(myFlowDetailFlow);
                dataItemOutputDetail itemOutputDetailChildren = DbWMS.dataItemOutputDetail
                    .Where(x => x.ItemOutputDetailGUID == myOutputChildren.ItemOutputDetailGUID).FirstOrDefault();
                itemOutputDetailChildren.ActualReturenedDate = myModel.CreatedDate;
                itemOutputDetailChildren.Comments = model.Comments;
                itemOutputDetailChildren.RetunedByGUID = UserGUID;
                itemOutputDetailChildren.ReturnedDate = myModel.CreatedDate;
                itemOutputDetailChildren.ActualReturenedDate = myModel.CreatedDate;
                DbWMS.Update(itemOutputDetailChildren, Permissions.WarehouseItemsRelease.UpdateGuid, ExecutionTime, DbCMS);

                dataItemInputDetail inputDetail = DbWMS.dataItemInputDetail
                        .Where(x => x.ItemInputDetailGUID == itemOutputDetailChildren.ItemInputDetailGUID).FirstOrDefault();
                inputDetail.IsAvaliable = true;
                inputDetail.ItemServiceStatusGUID = WarehouseServiceItemStatus.InStock;
                if (model.WarehouseLocationGUID != null && model.WarehouseLocationGUID != input.LastWarehouseLocationGUID)
                {
                    var pritransfer = DbWMS.dataItemTransfer.Where(x => x.ItemInputDetailGUID == itemOutputDetailChildren.ItemInputDetailGUID
                                                                        && x.IsLastTransfer == true).FirstOrDefault();
                    var toChangeTransfer = (from a in DbWMS.dataItemTransfer.Where(x => x.ItemInputDetailGUID == itemOutputDetailChildren.ItemInputDetailGUID) select a).ToList();
                    toChangeTransfer.ForEach(x => x.IsLastTransfer = false);
                    var warehouse = DbWMS.codeWarehouse.Where(x => x.WarehouseLocationGUID == model.WarehouseLocationGUID).FirstOrDefault();
                    dataItemTransfer changeTransferModel = new dataItemTransfer();
                    changeTransferModel.ItemTransferGUID = Guid.NewGuid();
                    changeTransferModel.ItemInputDetailGUID = input.ItemInputDetailGUID;
                    changeTransferModel.TransferDate = ExecutionTime;
                    changeTransferModel.SourceGUID = pritransfer.DestionationGUID;
                    changeTransferModel.DestionationGUID = warehouse.WarehouseGUID;
                    changeTransferModel.TransferedByGUID = UserGUID;
                    changeTransferModel.IsLastTransfer = true;
                    DbWMS.Create(changeTransferModel, Permissions.WarehouseItemsRelease.UpdateGuid, ExecutionTime, DbCMS);
                    inputDetail.LastWarehouseLocationGUID = model.WarehouseLocationGUID;
                    toAddTransfer.Add(changeTransferModel);
                }
                inputDetail.LastCustdianGUID = WarehouseRequestSourceTypes.Warehouse;
                inputDetail.LastCustdianNameGUID = myCurrentWarehouseGUID;
                inputDetail.LastFlowTypeGUID = WarehouseRequestFlowType.Returned;
                inputDetail.LastCustodianName = mywarehosue.WarehouseDescription;
                DbWMS.Update(inputDetail, Permissions.WarehouseItemsRelease.UpdateGuid, ExecutionTime, DbCMS);
            }
            myModel.ItemOutputDetailFlowGUID = Guid.NewGuid();
            myModel.FlowTypeGUID = WarehouseRequestFlowType.Returned;
            //myModel.CreatedDate = myModel.CreatedDate;
            myModel.ItemStatuGUID = WarehouseItemStatus.Functionting;
            myModel.IsLastAction = true;
            myModel.IsLastMove = true;
            myModel.CreatedByGUID = UserGUID;
            myModel.Active = true;
            myModel.OrderId = order;
            DbWMS.Create(myModel, Permissions.WarehouseItemsRelease.CreateGuid, ExecutionTime, DbCMS);
            outputDetail.ActualReturenedDate = myModel.CreatedDate;
            if (model.Comments != null)
                outputDetail.Comments = outputDetail.Comments + " " + model.Comments;
            outputDetail.RetunedByGUID = UserGUID;
            outputDetail.ReturnedDate = myModel.CreatedDate;


            if (model.WarehouseLocationGUID != null)
            {
                outputDetail.WarehouseLocationOnReturnGUID = model.WarehouseLocationGUID;
            }
            outputDetail.ItemStatuOnReturnGUID = model.ItemStatusGUID;
            DbWMS.Update(outputDetail, Permissions.WarehouseItemsRelease.UpdateGuid, ExecutionTime, DbCMS);
            var CurrentWarehouseGUID = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).Select(x => x.WarehouseGUID)
                .FirstOrDefault();
            if (model.WarehouseLocationGUID != null && model.WarehouseLocationGUID != input.LastWarehouseLocationGUID)
            {
                var pritransfer = DbWMS.dataItemTransfer.Where(x => x.ItemInputDetailGUID == input.ItemInputDetailGUID
                                                                 && x.IsLastTransfer == true).FirstOrDefault();
                var toChangeTransfer = (from a in DbWMS.dataItemTransfer.Where(x => x.ItemInputDetailGUID == outputDetail.ItemInputDetailGUID) select a).ToList();
                toChangeTransfer.ForEach(x => x.IsLastTransfer = false);
                var warehouse = DbWMS.codeWarehouse.Where(x => x.WarehouseLocationGUID == model.WarehouseLocationGUID).FirstOrDefault();
                dataItemTransfer changeTransferModel = new dataItemTransfer();
                changeTransferModel.ItemTransferGUID = Guid.NewGuid();
                changeTransferModel.ItemInputDetailGUID = input.ItemInputDetailGUID;
                changeTransferModel.TransferDate = ExecutionTime;
                changeTransferModel.SourceGUID = pritransfer.DestionationGUID;
                changeTransferModel.DestionationGUID = warehouse.WarehouseGUID;
                changeTransferModel.TransferedByGUID = UserGUID;
                changeTransferModel.IsLastTransfer = true;
                toAddTransfer.Add(changeTransferModel);
                input.LastWarehouseLocationGUID = model.WarehouseLocationGUID;
            }

            input.IsAvaliable = true;

            input.ItemStatusGUID = (Guid)model.ItemStatusGUID;
            input.ItemServiceStatusGUID = WarehouseServiceItemStatus.InStock;
            input.LastCustdianGUID = WarehouseRequestSourceTypes.Warehouse;
            input.LastCustdianNameGUID = CurrentWarehouseGUID;
            input.LastCustodianName = mywarehosue.WarehouseDescription;
            input.LastFlowTypeGUID = WarehouseRequestFlowType.Returned;
            DbWMS.Update(input, Permissions.WarehouseItemsRelease.UpdateGuid, ExecutionTime, DbCMS);
            DbWMS.CreateBulk(toAddTransfer, Permissions.WarehouseItemsRelease.UpdateGuid, ExecutionTime, DbCMS);
            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                var detail = DbWMS.dataItemOutputDetail.Where(x => x.ItemOutputDetailGUID == model.ItemOutputDetailGUID)
                    .FirstOrDefault();

                if (detail.dataItemOutput.RequesterGUID == WarehouseRequestSourceTypes.Staff)
                {
                    SendEmailToConfirmReturnItemToStock((Guid)model.ItemOutputDetailGUID);
                }

                if (_oldLastCustdianGUID == WarehouseRequestSourceTypes.Staff && (_oldItemModelWarehouseGUID == Guid.Parse("FA27E544-A88F-4CA0-86E0-C8B4BF40AF44") || _oldItemModelWarehouseGUID == Guid.Parse("56723035-36DE-4A91-8584-2B12FC4EE99C")))
                {

                    var myStaff = DbWMS.StaffCoreData.Where(x => x.UserGUID == _olLastCustdianNameGUID).FirstOrDefault();
                    if (myStaff != null)
                    {
                        var checkSTI = DbWMS.v_StaffContactsListForSTI.Where(x => x.EmailWork == myStaff.EmailAddress).FirstOrDefault();

                        // var myContact = DbWMS.StaffContactsInformation.Where(x => x.Emailwork.ToLower() == myStaff.EmailAddress.ToLower()).FirstOrDefault();
                        if (checkSTI == null)
                        {
                            myStaff.OfficialMobileNumber = null;
                        }
                        if (checkSTI != null && checkSTI.Mobile11 != myStaff.OfficialMobileNumber)
                        {
                            myStaff.OfficialMobileNumber = checkSTI.Mobile11;
                        }

                        DbWMS.Update(myStaff, Permissions.WarehouseItemsRelease.UpdateGuid, ExecutionTime, DbCMS);
                        DbWMS.SaveChanges();

                    }
                }
                var _checkStaffIssuer = DbAHD.StaffCoreData.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
                if (_checkStaffIssuer != null)
                {
                    DbWMS.sp_UpdatePhonesToHR();
                }



                string callBackFunc = "CheckCreateNewRelease('0')";
                return Json(DbCMS.SingleUpdateMessage(DataTableNames.WarehouseModelReleaseMovementsDataTable, DbWMS.PrimaryKeyControl(myModel), DbWMS.RowVersionControls(Portal.SingleToList(myModel)), callBackFunc));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }
        private bool ActiveReleaseModelFlow(dataItemOutputDetailFlow model)
        {

            if (model.ItemStatuGUID == null || model.ItemStatuGUID == Guid.Empty)
            {
                ModelState.AddModelError("LanguageID", "Kindly select item status");
                return (false);
            }
            //int LanguageID = DbWMS.dataItemOutputDetailFlow
            //    .Where(x =>

            //        x.ItemOutputDetailGUID == model.ItemOutputDetailGUID &&
            //        x.Active == true).Count();
            //if (LanguageID > 0)
            //{
            //    ModelState.AddModelError("LanguageID", "Model Already exists");
            //}

            return (true);
        }

        #endregion

        #region DamagedItems

        public ActionResult WarehouseModelDamagedMovementDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/WMS/Views/DamagedModels/_DamagedModelsDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<WarehouseModelDamagedMovementDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<WarehouseModelDamagedMovementDataTableModel>(DataTable.Filters);
            }

            var Result = (

                from a in DbWMS.dataItemOutputDetailDamagedTrack.AsNoTracking().AsExpandable().Where(x => x.Active && x.ItemInputDetailGUID == PK)
                join b in DbWMS.dataItemInputDetail.Where(x => x.Active) on a.ItemInputDetailGUID equals b.ItemInputDetailGUID into LJ1
                from R1 in LJ1.DefaultIfEmpty()
                join c in DbWMS.codeTablesValues.Where(x => x.Active) on a.DamagedByGUID equals c.ValueGUID
                join d in DbWMS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on c.ValueGUID equals d.ValueGUID into LJ2
                from R2 in LJ2.DefaultIfEmpty()


                join e in DbWMS.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.DamagedByNameGUID equals e.UserGUID into LJ3
                from R3 in LJ3.DefaultIfEmpty()
                    //join L in DbWMS.codeWarehouse.Where(x => x.Active) on R1.RequesterNameGUID equals L.WarehouseGUID
                    //join M in DbWMS.codeWarehouseLanguage.Where(x => x.Active && x.LanguageID == LAN) on L.WarehouseGUID equals M.WarehouseGUID into LJ8
                    //from R8 in LJ8.DefaultIfEmpty()

                select new WarehouseModelDamagedMovementDataTableModel
                {

                    ItemOutputDetailDamagedTrackGUID = a.ItemOutputDetailDamagedTrackGUID,
                    DamagedBy = R2.ValueDescription,
                    DamagedByName = R3.FirstName + " " + R3.Surname,

                    Active = a.Active,
                    ItemInputDetailGUID = a.ItemInputDetailGUID,
                    dataItemOutputDetailDamagedTrackRowVersion = a.dataItemOutputDetailDamagedTrackRowVersion,

                }).Where(Predicate);


            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult WarehouseModelDamagedMovementsCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }

            WarehouseModelDamageMovementsUpdateModel model = new WarehouseModelDamageMovementsUpdateModel
            {
                ItemInputDetailGUID = FK,
                ItemOutputDetailDamagedTrackGUID = Guid.Empty,
            };

            return PartialView("~/Areas/WMS/Views/DamagedModels/_DamagedModelUpdateModal.cshtml",
                model);
        }
        public ActionResult WarehouseModelDamagedMovementsUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }


            var myDetail = DbWMS.dataItemOutputDetailDamagedTrack.Find(PK);


            WarehouseModelDamageMovementsUpdateModel model = Mapper.Map(myDetail, new WarehouseModelDamageMovementsUpdateModel());



            return PartialView("~/Areas/WMS/Views/DamagedModels/_DamagedModelUpdateModal.cshtml", model);
        }

        [HttpPost]
        public ActionResult WarehouseModelDamagedMovementsUpload(FineUpload upload, WarehouseModelDamageMovementsUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            string error = "Error ";

            if (FileTypeValidator.IsPDF(upload.InputStream) || FileTypeValidator.IsImage(upload.InputStream) ||
                FileTypeValidator.IsExcel(upload.InputStream) ||
                FileTypeValidator.IsWord(upload.InputStream)
                )
            {
                var _stearm = upload.InputStream;
                string _ext = Path.GetExtension(upload.FileName).Split('.')[1];
                Guid EntityPK = Guid.NewGuid();
                var filePath = Server.MapPath("~\\Areas\\WMS\\UploadedDocuments\\DamagedItemReports\\");
                string extension = Path.GetExtension(upload.FileName);
                string fullFileName = filePath + "\\" + EntityPK + _ext;

                using (var fileStream = System.IO.File.Create(fullFileName))
                {
                    upload.InputStream.Seek(0, SeekOrigin.Begin);
                    upload.InputStream.CopyTo(fileStream);
                }





                DateTime ExecutionTime = DateTime.Now;
                dataItemOutputDetailDamagedTrack DamagedItemModel = Mapper.Map(model, new dataItemOutputDetailDamagedTrack());
                DamagedItemModel.ItemOutputDetailDamagedTrackGUID = EntityPK;
                DamagedItemModel.CreatedByGUID = UserGUID;
                DamagedItemModel.CreatedDate = ExecutionTime;

                DbWMS.Create(DamagedItemModel, Permissions.WarehouseItemsRelease.CreateGuid, ExecutionTime, DbCMS);


                dataItemInputDetail InputDetail = DbWMS.dataItemInputDetail.Find(model.ItemInputDetailGUID);
                //dataItemOutput itemOutput = DbWMS.dataItemOutput.Find(ReleaseModelDetail.ItemOutputGUID);
                InputDetail.IsAvaliable = false;
                InputDetail.ItemServiceStatusGUID = WarehouseServiceItemStatus.InService;
                if (DamagedItemModel.DamagedTypeGUID == WarhosueModelDamagedTypes.Damaged)
                    InputDetail.ItemStatusGUID = WarehouseItemStatus.Damaged;
                else
                {
                    InputDetail.ItemStatusGUID = WarehouseItemStatus.Lost;
                }
                //InputDetail.LastCustdianGUID = (Guid)DamagedItemModel.DamagedByGUID;
                InputDetail.DamagedByGUID = DamagedItemModel.DamagedByNameGUID;
                InputDetail.DamagedDate = DamagedItemModel.DamagedDate;


                DbWMS.Update(InputDetail, Permissions.WarehouseItemsRelease.UpdateGuid, ExecutionTime, DbCMS);




                try
                {
                    DbWMS.SaveChanges();
                    DbCMS.SaveChanges();
                    return Json(DbWMS.SingleUpdateMessage(DataTableNames.WarehouseModelDamagedMovementDataTable, DbWMS.PrimaryKeyControl(DamagedItemModel), DbWMS.RowVersionControls(Portal.SingleToList(DamagedItemModel))));
                }
                catch (Exception ex)
                {
                    return Json(DbWMS.ErrorMessage(ex.Message));
                }

            }
            return new FineUploaderResult((error == "" ? true : false), new { Error = error, path = "", success = (error == "" ? true : false) });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult WarehouseModelDamagedMovementsCreate(WarehouseModelDamageMovementsUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }

            //if (ActiveReleaseModelDetail(ReleaseModelDetail)) return PartialView("~/Areas/WMS/Views/ReleaseModelDetails/_ReleaseModelDetailUpdateModal.cshtml", model);


            Guid EntityPK = Guid.NewGuid();
            DateTime ExecutionTime = DateTime.Now;
            dataItemOutputDetailDamagedTrack DamagedItemModel = Mapper.Map(model, new dataItemOutputDetailDamagedTrack());
            DamagedItemModel.ItemOutputDetailDamagedTrackGUID = EntityPK;
            DamagedItemModel.CreatedByGUID = UserGUID;
            DamagedItemModel.CreatedDate = ExecutionTime;

            DbWMS.Create(DamagedItemModel, Permissions.WarehouseItemsRelease.CreateGuid, ExecutionTime, DbCMS);


            dataItemInputDetail InputDetail = DbWMS.dataItemInputDetail.Find(model.ItemInputDetailGUID);
            //dataItemOutput itemOutput = DbWMS.dataItemOutput.Find(ReleaseModelDetail.ItemOutputGUID);
            InputDetail.IsAvaliable = false;
            InputDetail.ItemServiceStatusGUID = WarehouseServiceItemStatus.InService;
            if (DamagedItemModel.DamagedTypeGUID == WarhosueModelDamagedTypes.Damaged)
                InputDetail.ItemStatusGUID = WarehouseItemStatus.Damaged;
            else
            {
                InputDetail.ItemStatusGUID = WarehouseItemStatus.Lost;
            }
            InputDetail.DamagedByGUID = DamagedItemModel.DamagedByNameGUID;
            InputDetail.DamagedDate = DamagedItemModel.DamagedDate;


            DbWMS.Update(InputDetail, Permissions.WarehouseItemsRelease.UpdateGuid, ExecutionTime, DbCMS);




            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleUpdateMessage(DataTableNames.WarehouseModelDamagedMovementDataTable, DbWMS.PrimaryKeyControl(DamagedItemModel), DbWMS.RowVersionControls(Portal.SingleToList(DamagedItemModel))));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult WarehouseModelDamagedMovementsUpdate(WarehouseModelDamageMovementsUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Update, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            // if (!ModelState.IsValid || ActiveReleaseModel(model)) return PartialView("~/Areas/WMS/Views/ReleaseModels/_ReleaseModelForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            dataItemOutputDetailDamagedTrack damagedTrackModel = DbWMS.dataItemOutputDetailDamagedTrack.Where(x => x.ItemOutputDetailDamagedTrackGUID == model.ItemOutputDetailDamagedTrackGUID).FirstOrDefault();// Mapper.Map(model, new dataItemOutputDetailDamagedTrack());
            damagedTrackModel.DamagedTypeGUID = model.DamagedTypeGUID;
            damagedTrackModel.DamagedByGUID = model.DamagedByGUID;
            damagedTrackModel.DamagedByNameGUID = model.DamagedByNameGUID;
            damagedTrackModel.LocationGUID = model.LocationGUID;
            damagedTrackModel.DocumentReference = model.DocumentReference;
            damagedTrackModel.DamagedDate = model.DamagedDate;
            damagedTrackModel.PresentLocation = model.PresentLocation;
            damagedTrackModel.DamagedReason = model.DamagedReason;
            damagedTrackModel.Comments = model.Comments;
            DbWMS.Update(damagedTrackModel, Permissions.WarehouseItemsRelease.UpdateGuid, ExecutionTime, DbCMS);

            var myInputDetails = DbWMS.dataItemInputDetail
                .Where(x => x.ItemInputDetailGUID == damagedTrackModel.ItemInputDetailGUID).FirstOrDefault();

            if (damagedTrackModel.DamagedTypeGUID == WarhosueModelDamagedTypes.Damaged)
                myInputDetails.ItemStatusGUID = WarehouseItemStatus.Damaged;
            else
            {
                myInputDetails.ItemStatusGUID = WarehouseItemStatus.Lost;
            }

            DbWMS.Update(myInputDetails, Permissions.WarehouseItemsRelease.UpdateGuid, ExecutionTime, DbCMS);


            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleUpdateMessage(null, null, DbWMS.RowVersionControls(Portal.SingleToList(damagedTrackModel))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyDamagedModel((Guid)model.ItemOutputDetailDamagedTrackGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DamagedModelDelete(dataItemOutputDetailDamagedTrack model)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Delete, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            List<dataItemOutputDetailDamagedTrack> DeletedReleaseModel = DeleteDamagedModel(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.WarehouseItemsRelease.Restore, Apps.WMS), Container = "DamagedModelFormControls" });

            try
            {
                int CommitedRows = DbWMS.SaveChanges();
                DbWMS.SaveChanges();
                return Json(DbWMS.SingleDeleteMessage(CommitedRows, DeletedReleaseModel.FirstOrDefault(), "DamagedModelFormControls", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyDamagedModel(model.ItemOutputDetailDamagedTrackGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DamagedModelRestore(dataItemOutputDetailDamagedTrack model)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Restore, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (ActiveDamagedModel(model))
            {
                return Json(DbWMS.RecordExists());
            }

            List<dataItemOutputDetailDamagedTrack> RestoredDamagedModel = RestoreDamagedModel(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.WarehouseItemsRelease.Create, Apps.WMS, new UrlHelper(Request.RequestContext).Action("WarehouseModelDamagedMovementsCreate", "ModelMovements", new { Area = "WMS" })), Container = "DamagedModelFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.WarehouseItemsRelease.Update, Apps.WMS), Container = "ReleaseModelFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.WarehouseItemsRelease.Delete, Apps.WMS), Container = "ReleaseModelFormControls" });

            try
            {
                int CommitedRows = DbWMS.SaveChanges();
                DbWMS.SaveChanges();
                return Json(DbWMS.SingleRestoreMessage(CommitedRows, RestoredDamagedModel, DbWMS.PrimaryKeyControl(RestoredDamagedModel.FirstOrDefault()), null, null, UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyDamagedModel(model.ItemOutputDetailDamagedTrackGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]


        public JsonResult WarehouseModelDamagedMovementDataTableDelete(List<dataItemOutputDetailDamagedTrack> models)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Delete, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            List<dataItemOutputDetailDamagedTrack> DeletedReleaseModel = DeleteDamagedModel(models);

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.PartialDeleteMessage(DeletedReleaseModel, models, DataTableNames.WarehouseModelDamagedMovementDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult WarehouseModelDamagedMovementsRestore(List<dataItemOutputDetailDamagedTrack> models)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Restore, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            List<dataItemOutputDetailDamagedTrack> RestoredDamagedModel = RestoreDamagedModel(models);

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.PartialRestoreMessage(RestoredDamagedModel, models, DataTableNames.WarehouseItemReleasesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        private List<dataItemOutputDetailDamagedTrack> DeleteDamagedModel(List<dataItemOutputDetailDamagedTrack> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<dataItemOutputDetailDamagedTrack> DeletedDamagedModel = new List<dataItemOutputDetailDamagedTrack>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbWMS.QueryBuilder(models, Permissions.WarehouseItemsRelease.DeleteGuid, SubmitTypes.Delete, baseQuery);

            var Records = DbWMS.Database.SqlQuery<dataItemOutputDetailDamagedTrack>(query).ToList();
            foreach (var record in Records)
            {
                DeletedDamagedModel.Add(DbWMS.Delete(record, ExecutionTime, Permissions.WarehouseItemsRelease.DeleteGuid, DbCMS));
            }

            //var Languages = DeletedReleaseModel.SelectMany(a => a.dataItemOutputLanguage).Where(l => l.Active).ToList();
            //foreach (var language in Languages)
            //{
            //    DbWMS.Delete(language, ExecutionTime, Permissions.WarehouseItemsRelease.DeleteGuid, DbCMS);
            //}
            return DeletedDamagedModel;
        }

        private List<dataItemOutputDetailDamagedTrack> RestoreDamagedModel(List<dataItemOutputDetailDamagedTrack> models)
        {

            DateTime RestoringTime = DateTime.Now;

            List<dataItemOutputDetailDamagedTrack> RestoredDamagedModel = new List<dataItemOutputDetailDamagedTrack>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbWMS.QueryBuilder(models, Permissions.WarehouseItemsRelease.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Records = DbWMS.Database.SqlQuery<dataItemOutputDetailDamagedTrack>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveDamagedModel(record))
                {
                    RestoredDamagedModel.Add(DbWMS.Restore(record, Permissions.WarehouseItemsRelease.DeleteGuid, Permissions.WarehouseItemsRelease.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            //var Languages = RestoredReleaseModel.SelectMany(x => x.dataItemOutputLanguage.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            //foreach (var language in Languages)
            //{
            //    DbWMS.Restore(language, Permissions.WarehouseItemsRelease.DeleteGuid, Permissions.WarehouseItemsRelease.RestoreGuid, RestoringTime, DbCMS);
            //}

            return RestoredDamagedModel;
        }

        private JsonResult ConcurrencyDamagedModel(Guid PK)
        {
            dataItemOutputDetailDamagedTrack dbModel = new dataItemOutputDetailDamagedTrack();

            var DamagedModel = DbWMS.dataItemOutputDetailDamagedTrack.Where(x => x.ItemOutputDetailDamagedTrackGUID == PK).FirstOrDefault();
            var dbDamagedModel = DbWMS.Entry(DamagedModel).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbDamagedModel, dbModel);

            //var Language = DbWMS.dataItemOutputLanguage.Where(x => x.ItemOutputGUID == PK).Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.dataItemOutput.DeletedOn) && x.LanguageID == LAN).FirstOrDefault();
            //var dbLanguage = DbWMS.Entry(Language).GetDatabaseValues().ToObject();
            //dbModel = Mapper.Map(dbLanguage, dbModel);

            //if (ReleaseModel.dataItemOutputRowVersion.SequenceEqual(dbModel.dataItemOutputRowVersion) && Language.dataItemOutputLanguageRowVersion.SequenceEqual(dbModel.dataItemOutputLanguageRowVersion))
            //{
            //    return Json(DbWMS.PermissionError());
            //}

            return Json(JsonMessages.ConcurrencyError(DbWMS, dbModel, "DamagedModelDetailsContainer"));
        }

        private bool ActiveDamagedModel(Object model)
        {
            dataItemOutputDetailDamagedTrack damagedModel = Mapper.Map(model, new dataItemOutputDetailDamagedTrack());
            int ModelDescription = DbWMS.dataItemOutputDetailDamagedTrack
                                    .Where(x => x.DamagedReason == damagedModel.DamagedReason &&
                                                x.ItemInputDetailGUID == damagedModel.ItemInputDetailGUID &&
                                                x.ItemOutputDetailDamagedTrackGUID != damagedModel.ItemOutputDetailDamagedTrackGUID &&


                                                x.Active).Count();
            if (ModelDescription > 0)
            {
                ModelState.AddModelError("Damaged Description", "This record is already exists");
            }
            return (ModelDescription > 0);
        }
        #endregion

        #region Reports


        #endregion

        #region Bulk Upload Entry


        [HttpGet]
        public ActionResult UploadFiles()
        {
            return PartialView("~/Areas/WMS/Views/ModelEntryMovements/_BulkEntryModelUpdateModal.cshtml",
                new dataItemInput());
        }
        [HttpPost]
        public FineUploaderResult UploadFiles(FineUpload upload)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Create, Apps.WMS))
            {
                return new FineUploaderResult(false, new { path = Upload(upload), success = false });
            }
            string error = "Error ";

            if (FileTypeValidator.IsPDF(upload.InputStream) || FileTypeValidator.IsImage(upload.InputStream) ||
                FileTypeValidator.IsExcel(upload.InputStream) ||
                FileTypeValidator.IsWord(upload.InputStream)
                )
            {
                return new FineUploaderResult(true, new { path = Upload(upload), success = true });
            }
            return new FineUploaderResult((error == "" ? true : false), new { Error = error, path = "", success = (error == "" ? true : false) });
        }

        public string Upload(FineUpload upload)
        {
            var _stearm = upload.InputStream;
            string _ext = Path.GetExtension(upload.FileName).Split('.')[1];

            dataItemInput itemInput = new dataItemInput();
            itemInput.ItemInputGUID = Guid.NewGuid();
            string FilePath = Server.MapPath("~/Uploads/WMS/EntryModels/" + itemInput.ItemInputGUID + ".xlsx");

            using (var fileStream = System.IO.File.Create(FilePath))
            {
                upload.InputStream.Seek(0, SeekOrigin.Begin);
                upload.InputStream.CopyTo(fileStream);
            }
            if (_ext.ToLower() == "xls" || _ext.ToLower() == "xlsx")
            {
                using (ExcelPackage package = new ExcelPackage(new FileInfo(FilePath)))
                {
                    ExcelWorksheet workSheet = package.Workbook.Worksheets[1];
                    List<dataItemInputDetail> itemInputDetails = new List<dataItemInputDetail>();
                    var itemInputDeterminants = DbWMS.dataItemInputDeterminant.ToList();
                    var models = DbWMS.codeWarehouseItemModelLanguage.ToList();

                    bool ok = Validate(workSheet);

                    package.Save();
                    //byte[] fileBytes = System.IO.File.ReadAllBytes(FilePath);
                    //string fileName = "MonthlyTable " + DateTime.Now.ToString("dddd, dd MMMM yyyy") + ".xlsx";te
                    //return System.IO.File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                    if (ok)
                    {
                        int totalRows = workSheet.Dimension.End.Row;
                        List<dataItemInput> allInputs = DbWMS.dataItemInput.ToList();
                        List<dataItemInput> ToAddItemInputs = new List<dataItemInput>();
                        var warehouseItemModelDeterminants = DbWMS.codeWarehouseItemModelDeterminant.ToList();
                        List<dataItemInputDeterminant> ToAddModelDeterminants = new List<dataItemInputDeterminant>();
                        List<dataItemInputDetail> ToAddInputDetails = new List<dataItemInputDetail>();
                        List<dataItemTransfer> ToAddInputTransfer = new List<dataItemTransfer>();
                        var warehouseGUID = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID)
                            .FirstOrDefault().WarehouseGUID;
                        var myWarehouse = DbWMS.codeWarehouseLanguage.Where(x => x.LanguageID == LAN && x.WarehouseGUID == warehouseGUID)
                            .FirstOrDefault();
                        var allmodelLang = DbWMS.codeWarehouseItemModelLanguage.ToList();
                        //var priceTypes=DbWMS.codeTablesValuesLanguages.Where(x=>x.)

                        var allModesl = DbWMS.codeItemModelWarehouse.Where(x => x.WarehouseGUID == warehouseGUID)
                            .ToList();
                        for (int i = 2; i <= totalRows; i++)
                        {
                            dataItemInput ToAddInput = new dataItemInput();
                            var ModelName = workSheet.Cells["A" + i].Value;
                            if (ModelName == null)
                                break;

                            var checkModel = allmodelLang.Where(x => x.ModelDescription.ToString().ToLower() == ModelName.ToString().ToLower()).FirstOrDefault();
                            if (checkModel == null)
                                continue;
                            var myItemModelWarehouseGUID = allModesl.Where(x =>
                               x.WarehouseItemModelGUID == checkModel.WarehouseItemModelGUID).Select(x => x.ItemModelWarehouseGUID).FirstOrDefault();
                            var WarhouseItemModelGUID = allModesl.Where(x =>
                                    x.WarehouseItemModelGUID == checkModel.WarehouseItemModelGUID)
                                .Select(x => x.codeWarehouseItemModel.WarehouseItemModelGUID).FirstOrDefault();
                            if (myItemModelWarehouseGUID == null)
                            {
                                continue;
                            }

                            var PoNumber = workSheet.Cells["B" + i].Value;
                            var Source = workSheet.Cells["C" + i].Value;
                            var currentBarcode = workSheet.Cells["D" + i].Value;
                            var currentSerial = workSheet.Cells["E" + i].Value;
                            var currentIME = workSheet.Cells["F" + i].Value;
                            var currentGSM = workSheet.Cells["G" + i].Value;
                            var Governorate = workSheet.Cells["H" + i].Value;
                            var SubLocation = workSheet.Cells["I" + i].Value;
                            var InventoryStatus = workSheet.Cells["J" + i].Value;
                            var PriceType = workSheet.Cells["K" + i].Value;
                            var PriceValue = workSheet.Cells["L" + i].Value;
                            var ModelColor = workSheet.Cells["M" + i].Value;

                            var currdate = workSheet.Cells["N" + i].Value;




                            //var inputdate = workSheet.Cells["N" + i].Value;
                            var Warranty = Convert.ToInt32(workSheet.Cells["O" + i].Value);
                            var currentMAC = workSheet.Cells["Q" + i].Value;
                            var currentSeq = workSheet.Cells["R" + i].Value;

                            //var currentPhone = workSheet.Cells["R" + i].Value;

                            int? Depression = Convert.ToInt32(workSheet.Cells["P" + i].Value);
                            Guid priceGUID = Guid.Parse("6FFD5528-A577-41E3-960A-013D152DBAB3");
                            var CurrentItemInputGUID = Guid.Empty;
                            var sourceGuid = Guid.Parse("D5E2A4DF-C67E-4F4D-A242-7E33C381EC99");
                            var ColorGUID = Guid.Parse("a40ec252-622e-4ff1-9ef4-e328c7a3cec4");
                            var AllSources = DbWMS.codeTablesValuesLanguages.Where(x => x.codeTablesValues.TableGUID == sourceGuid
                                                                                      && x.LanguageID == LAN).ToList();

                            var allPrice = DbWMS.codeTablesValuesLanguages
                                .Where(x => x.codeTablesValues.TableGUID == priceGUID && x.LanguageID == LAN).ToList();
                            var allColors = DbWMS.codeTablesValuesLanguages
                                .Where(x => x.codeTablesValues.TableGUID == ColorGUID && x.LanguageID == LAN).ToList();


                            var allLocations = DbWMS.codeLocationsLanguages.Where(x => x.Active && x.LanguageID == LAN)
                                .ToList();
                            Guid inven = Guid.Parse("675DE853-151B-4C2F-93F4-DA1434EEE765");
                            var allInventoryStatus = DbWMS.codeTablesValuesLanguages
                                .Where(x => x.codeTablesValues.TableGUID == inven && x.LanguageID == LAN).ToList();

                            var lastSequenceNumber = allInputs.Select(x => x.SequenceNumber).Max();
                            DateTime ExecutionTime = DateTime.Now;
                            if (PoNumber != null && allInputs.Where(x => x.BillNumber == PoNumber).FirstOrDefault() == null)
                            {

                                var checkAdded = ToAddItemInputs.Where(x => x.BillNumber == PoNumber).FirstOrDefault();
                                if (checkAdded == null)
                                {

                                    lastSequenceNumber++;



                                    ToAddInput = new dataItemInput
                                    {
                                        ItemInputGUID = Guid.NewGuid(),

                                        WarehouseSiteGUID = warehouseGUID,
                                        InputDate = DateTime.Now,
                                        BillNumber = PoNumber != null ? PoNumber.ToString() : null,
                                        SequenceNumber = lastSequenceNumber,
                                        CreatedByGUID = UserGUID,
                                        CreatedDate = ExecutionTime,
                                    };
                                    if (Source != null)
                                    {
                                        var sourceValue = AllSources
                                            .Where(x => x.ValueDescription.ToString().ToLower() == Source.ToString().ToLower()).Select(x => x.ValueGUID)
                                            .FirstOrDefault();
                                        ToAddInput.SourceGUID = sourceValue;
                                    }
                                    CurrentItemInputGUID = ToAddInput.ItemInputGUID;
                                    ToAddItemInputs.Add(ToAddInput);

                                }
                                else
                                {
                                    CurrentItemInputGUID = checkAdded.ItemInputGUID;
                                }
                            }
                            else
                            {
                                if (PoNumber != null)
                                {
                                    var checkInput = allInputs.Where(x => x.BillNumber == PoNumber).FirstOrDefault();
                                    CurrentItemInputGUID = checkInput.ItemInputGUID;
                                }
                                else
                                {


                                    //if (mysource != null)
                                    //    source = mysource.ValueGUID;
                                    ToAddInput = new dataItemInput
                                    {
                                        ItemInputGUID = Guid.NewGuid(),

                                        WarehouseSiteGUID = warehouseGUID,
                                        InputDate = DateTime.Now,

                                        SequenceNumber = lastSequenceNumber,
                                        CreatedByGUID = UserGUID,
                                        CreatedDate = ExecutionTime,
                                    };
                                    CurrentItemInputGUID = ToAddInput.ItemInputGUID;
                                    if (Source != null)
                                    {
                                        var mysource = AllSources.Where(x => x.ValueDescription.ToString().ToLower() == Source.ToString().ToLower())
                                            .FirstOrDefault();
                                        if (mysource != null)
                                            ToAddInput.SourceGUID = mysource.ValueGUID;

                                    }
                                    ToAddItemInputs.Add(ToAddInput);

                                }
                            }

                            if (ModelName != null)
                            {



                                Guid EntityPK = Guid.NewGuid();
                                //Guid dollar = Guid.Parse("A2B0C8DC-5C9C-41F4-AB88-B1B4152C7A95");
                                var mylocation = allLocations.Where(x => x.LocationDescription == Governorate)
                                    .Select(x => x.LocationGUID).FirstOrDefault();
                                Guid inventoryStatusGuid = Guid.Empty;
                                if (InventoryStatus != null)
                                {
                                    inventoryStatusGuid = allInventoryStatus
                                        .Where(x => x.ValueDescription.ToString().ToLower() ==
                                                    InventoryStatus.ToString().ToLower()).Select(x => x.ValueGUID)
                                        .FirstOrDefault();
                                }

                                Guid generalUseGUid = Guid.Parse("8A2AE01E-0F57-4178-B05E-E3021A2301A9");
                                var currentWarehouseGUID = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).Select(x => x.WarehouseGUID)
                                                      .FirstOrDefault();
                                var currwaehouse = DbWMS.codeWarehouse.Where(x => (x.ParentGUID == currentWarehouseGUID) || (x.WarehouseGUID == currentWarehouseGUID && x.ParentGUID == null)).FirstOrDefault();


                                // Guid damascusWarehosue = Guid.Parse("7E208D24-8F61-4403-A7A7-C1C2A4BE55B4");
                                dataItemInputDetail toAddInputDetail = new dataItemInputDetail
                                {
                                    ItemInputDetailGUID = EntityPK,
                                    ItemInputGUID = CurrentItemInputGUID,
                                    ItemStatusGUID = WarehouseItemStatus.Functionting,
                                    ItemServiceStatusGUID = WarehouseServiceItemStatus.InStock,
                                    ItemConditionGUID = WarehouseItemCondition.New,
                                    IsAvaliable = true,
                                    //AcquisitionDate= inputdate == null ? DateTime.Now : (DateTime)inputdate,
                                    CreatedByGUID = UserGUID,
                                    CreatedDate = ExecutionTime,
                                    IsDeterminanted = true,

                                    LastCustdianGUID = WarehouseRequestSourceTypes.Warehouse,
                                    LastFlowTypeGUID = WarehouseRequestFlowType.Returned,
                                    LastCustdianNameGUID = warehouseGUID,
                                    WarehouseOwnerGUID = warehouseGUID,
                                    Warrantylength = Warranty,
                                    DepreciatedYears = Depression,
                                    PurposeofuseGUID = generalUseGUid,
                                    LastWarehouseLocationGUID = currwaehouse.WarehouseLocationGUID,
                                    BarcodeNumber = currentBarcode != null ? currentBarcode.ToString() : null,
                                    SerialNumber = currentSerial != null ? currentSerial.ToString() : null,
                                    IMEI1 = currentIME != null ? currentIME.ToString() : null,
                                    GSM = currentGSM != null ? currentGSM.ToString() : null,
                                    MAC = currentMAC != null ? currentMAC.ToString() : null,
                                    SequenceNumber = currentSeq != null ? currentSeq.ToString() : null,
                                    LastCustodianName = myWarehouse.WarehouseDescription,
                                    ItemModelWarehouseGUID = myItemModelWarehouseGUID,

                                };
                                if (inven != Guid.Empty)
                                {
                                    toAddInputDetail.InventoryStatusGUID = inventoryStatusGuid;
                                }

                                if (ModelColor != null)
                                {

                                    var colorGUID = allColors
                                        .Where(x => x.ValueDescription.ToString().ToLower() == ModelColor.ToString().ToLower()).Select(x => x.ValueGUID)
                                        .FirstOrDefault();
                                    if (colorGUID != null && colorGUID != Guid.Empty)
                                    {
                                        toAddInputDetail.ColorGUID = colorGUID;

                                    }

                                }

                                if (currdate != null)
                                {



                                    string[] arrDate = currdate.ToString().Split('/');
                                    string day = arrDate[0].ToString();
                                    if (day.Length == 1)
                                    {
                                        day = "0" + day;
                                    }
                                    string month = arrDate[1].ToString();
                                    if (month.Length == 1)
                                    {
                                        month = "0" + month;
                                    }
                                    string year = arrDate[2].ToString();
                                    if (year.Length == 2)
                                    {
                                        year = "20" + year;
                                    }
                                    string temp = day + "/" + month + "/" + year;

                                    DateTime inputdate = DateTime.ParseExact(temp.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);

                                    toAddInputDetail.AcquisitionDate = inputdate;
                                }
                                if (mylocation != null && mylocation != Guid.Empty)
                                {
                                    toAddInputDetail.LocationGUID = mylocation;

                                }


                                if (PriceValue != null)
                                {
                                    var newPrcie = PriceValue.ToString().Trim();
                                    var newValue = decimal.Parse(newPrcie, NumberStyles.AllowDecimalPoint |
                                                             NumberStyles.AllowExponent |
                                                             NumberStyles.AllowLeadingSign,
                                        CultureInfo.InvariantCulture);
                                    toAddInputDetail.PriceValue = newValue;
                                    toAddInputDetail.PriceTypeGUID = allPrice.Where(x => x.ValueDescription.ToString().ToLower() == PriceType.ToString().ToLower()).Select(x => x.ValueGUID).FirstOrDefault();
                                }
                                ToAddInputDetails.Add(toAddInputDetail);
                                if (currentBarcode != null && currentBarcode.ToString().Length > 4)
                                {
                                    var myWarehouseItemModelDeterminantGUID = warehouseItemModelDeterminants
                                        .Where(x => x.DeterminantGUID == ItemDeterminants.Barcode &&
                                                    x.WarehouseItemModelGUID == WarhouseItemModelGUID)
                                        .Select(x => x.WarehouseItemModelDeterminantGUID).FirstOrDefault();
                                    if (myWarehouseItemModelDeterminantGUID != null && myWarehouseItemModelDeterminantGUID != Guid.Empty)
                                    {
                                        dataItemInputDeterminant toAddBarcode = new dataItemInputDeterminant
                                        {
                                            ItemInputDeterminantGUID = Guid.NewGuid(),
                                            ItemInputDetailGUID = toAddInputDetail.ItemInputDetailGUID,
                                            WarehouseItemModelDeterminantGUID = myWarehouseItemModelDeterminantGUID,
                                            DeterminantValue = currentBarcode.ToString(),
                                            CreatedByGUID = UserGUID,
                                            CreatedDate = ExecutionTime,
                                        };
                                        ToAddModelDeterminants.Add(toAddBarcode);

                                    }

                                }
                                if (currentSerial != null && currentSerial.ToString().Length > 4)
                                {
                                    var myWarehouseItemModelDeterminantGUID = warehouseItemModelDeterminants
                                        .Where(x => x.DeterminantGUID == ItemDeterminants.SerialNumber &&
                                                    x.WarehouseItemModelGUID == WarhouseItemModelGUID)
                                        .Select(x => x.WarehouseItemModelDeterminantGUID).FirstOrDefault();
                                    if (myWarehouseItemModelDeterminantGUID != null && myWarehouseItemModelDeterminantGUID != Guid.Empty)
                                    {
                                        dataItemInputDeterminant toAddSerial = new dataItemInputDeterminant
                                        {
                                            ItemInputDeterminantGUID = Guid.NewGuid(),
                                            ItemInputDetailGUID = toAddInputDetail.ItemInputDetailGUID,
                                            WarehouseItemModelDeterminantGUID = myWarehouseItemModelDeterminantGUID,

                                            DeterminantValue = currentSerial.ToString(),
                                            CreatedByGUID = UserGUID,
                                            CreatedDate = ExecutionTime,
                                        };
                                        ToAddModelDeterminants.Add(toAddSerial);

                                    }


                                }
                                if (currentMAC != null && currentMAC.ToString().Length > 4)
                                {
                                    var myWarehouseItemModelDeterminantGUID = warehouseItemModelDeterminants
                                        .Where(x => x.DeterminantGUID == ItemDeterminants.MAC &&
                                                    x.WarehouseItemModelGUID == WarhouseItemModelGUID)
                                        .Select(x => x.WarehouseItemModelDeterminantGUID).FirstOrDefault();
                                    if (myWarehouseItemModelDeterminantGUID != null && myWarehouseItemModelDeterminantGUID != Guid.Empty)
                                    {
                                        dataItemInputDeterminant toAddMAC = new dataItemInputDeterminant
                                        {
                                            ItemInputDeterminantGUID = Guid.NewGuid(),
                                            ItemInputDetailGUID = toAddInputDetail.ItemInputDetailGUID,
                                            WarehouseItemModelDeterminantGUID = myWarehouseItemModelDeterminantGUID,

                                            DeterminantValue = currentMAC.ToString(),
                                            CreatedByGUID = UserGUID,
                                            CreatedDate = ExecutionTime,
                                        };
                                        ToAddModelDeterminants.Add(toAddMAC);

                                    }


                                }
                                if (currentSeq != null && currentSeq.ToString().Length > 4)
                                {
                                    var myWarehouseItemModelDeterminantGUID = warehouseItemModelDeterminants
                                        .Where(x => x.DeterminantGUID == ItemDeterminants.SeqNumber &&
                                                    x.WarehouseItemModelGUID == WarhouseItemModelGUID)
                                        .Select(x => x.WarehouseItemModelDeterminantGUID).FirstOrDefault();
                                    if (myWarehouseItemModelDeterminantGUID != null && myWarehouseItemModelDeterminantGUID != Guid.Empty)
                                    {
                                        dataItemInputDeterminant toAddSeq = new dataItemInputDeterminant
                                        {
                                            ItemInputDeterminantGUID = Guid.NewGuid(),
                                            ItemInputDetailGUID = toAddInputDetail.ItemInputDetailGUID,
                                            WarehouseItemModelDeterminantGUID = myWarehouseItemModelDeterminantGUID,

                                            DeterminantValue = currentSeq.ToString(),
                                            CreatedByGUID = UserGUID,
                                            CreatedDate = ExecutionTime,
                                        };
                                        ToAddModelDeterminants.Add(toAddSeq);

                                    }


                                }
                                //if (currentPhone != null && currentPhone.ToString().Length > 4)
                                //{
                                //    var myWarehouseItemModelDeterminantGUID = warehouseItemModelDeterminants
                                //        .Where(x => x.DeterminantGUID == ItemDeterminants.Phone &&
                                //                    x.WarehouseItemModelGUID == WarhouseItemModelGUID)
                                //        .Select(x => x.WarehouseItemModelDeterminantGUID).FirstOrDefault();
                                //    if (myWarehouseItemModelDeterminantGUID != null && myWarehouseItemModelDeterminantGUID != Guid.Empty)
                                //    {
                                //        dataItemInputDeterminant toAddPhone = new dataItemInputDeterminant
                                //        {
                                //            ItemInputDeterminantGUID = Guid.NewGuid(),
                                //            ItemInputDetailGUID = toAddInputDetail.ItemInputDetailGUID,
                                //            WarehouseItemModelDeterminantGUID = myWarehouseItemModelDeterminantGUID,

                                //            DeterminantValue = currentPhone.ToString(),
                                //            CreatedByGUID = UserGUID,
                                //            CreatedDate = ExecutionTime,
                                //        };
                                //        ToAddModelDeterminants.Add(toAddPhone);

                                //    }


                                //}


                                if (currentIME != null && currentIME.ToString().Length > 4)
                                {
                                    var myWarehouseItemModelDeterminantGUID = warehouseItemModelDeterminants
                                        .Where(x => x.DeterminantGUID == ItemDeterminants.IME1 &&
                                                    x.WarehouseItemModelGUID == WarhouseItemModelGUID)
                                        .Select(x => x.WarehouseItemModelDeterminantGUID).FirstOrDefault();
                                    if (myWarehouseItemModelDeterminantGUID != null && myWarehouseItemModelDeterminantGUID != Guid.Empty)
                                    {
                                        dataItemInputDeterminant toAddIME1 = new dataItemInputDeterminant
                                        {
                                            ItemInputDeterminantGUID = Guid.NewGuid(),
                                            ItemInputDetailGUID = toAddInputDetail.ItemInputDetailGUID,
                                            WarehouseItemModelDeterminantGUID = myWarehouseItemModelDeterminantGUID,
                                            DeterminantValue = currentIME.ToString(),
                                            CreatedByGUID = UserGUID,
                                            CreatedDate = ExecutionTime,
                                        };
                                        ToAddModelDeterminants.Add(toAddIME1);
                                    }




                                }


                                if (currentGSM != null && currentGSM.ToString().Length > 4)
                                {
                                    var myWarehouseItemModelDeterminantGUID = warehouseItemModelDeterminants
                                        .Where(x => x.DeterminantGUID == ItemDeterminants.GSM &&
                                                    x.WarehouseItemModelGUID == WarhouseItemModelGUID)
                                        .Select(x => x.WarehouseItemModelDeterminantGUID).FirstOrDefault();
                                    if (myWarehouseItemModelDeterminantGUID != null && myWarehouseItemModelDeterminantGUID != Guid.Empty)
                                    {
                                        dataItemInputDeterminant toAddGSM = new dataItemInputDeterminant
                                        {
                                            ItemInputDeterminantGUID = Guid.NewGuid(),
                                            ItemInputDetailGUID = toAddInputDetail.ItemInputDetailGUID,
                                            WarehouseItemModelDeterminantGUID = myWarehouseItemModelDeterminantGUID,
                                            DeterminantValue = currentGSM.ToString(),
                                            CreatedByGUID = UserGUID,
                                            CreatedDate = ExecutionTime,
                                        };
                                        ToAddModelDeterminants.Add(toAddGSM);
                                    }

                                }

                                dataItemTransfer TransferModel = new dataItemTransfer
                                {
                                    ItemTransferGUID = Guid.NewGuid(),
                                    ItemInputDetailGUID = toAddInputDetail.ItemInputDetailGUID,
                                    TransferDate = ExecutionTime,
                                    SourceGUID = warehouseGUID,
                                    DestionationGUID = warehouseGUID,
                                    TransferedByGUID = UserGUID,
                                    IsLastTransfer = true,
                                };
                                ToAddInputTransfer.Add(TransferModel);
                            }

                        }

                        DbWMS.CreateBulk(ToAddItemInputs, Permissions.WarehouseItemsEntry.CreateGuid, DateTime.Now, DbCMS);


                        DbWMS.CreateBulk(ToAddInputDetails, Permissions.WarehouseItemsEntry.CreateGuid, DateTime.Now, DbCMS);


                        DbWMS.CreateBulk(ToAddModelDeterminants, Permissions.WarehouseItemsEntry.CreateGuid, DateTime.Now, DbCMS);


                        DbWMS.CreateBulk(ToAddInputTransfer, Permissions.WarehouseItemsEntry.CreateGuid, DateTime.Now, DbCMS);


                        try
                        {
                            DbWMS.SaveChanges();
                            DbCMS.SaveChanges();
                            // return Json(DbWMS.SingleUpdateMessage(DataTableNames.WarehouseModelEntryMovementsDataTable, DbWMS.PrimaryKeyControl(EntryModel), DbWMS.RowVersionControls(Portal.SingleToList(EntryModel))));
                        }
                        catch (Exception ex)
                        {
                            var error = DbWMS.ErrorMessage(ex.Message);
                        }
                    }
                }
            }
            return "../Uploads/WMS/EntryModels/" + itemInput.ItemInputGUID + ".xlsx";
        }
        public static DateTime FromExcelSerialDate(int SerialDate)
        {
            if (SerialDate > 59) SerialDate -= 1; //Excel/Lotus 2/29/1900 bug   
            return new DateTime(1899, 12, 31).AddDays(SerialDate);
        }

        public FileResult ShowProcessdEntryUploadFile(string FilePath)
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(FilePath);
            string fileName = "EntryUpload " + DateTime.Now.ToString("dddd, dd MMMM yyyy") + ".xlsx";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        /// <summary>
        /// Validate th Excel 
        /// </summary>
        /// <param name="workSheet"></param>
        /// <returns></returns>
        private bool Validate(ExcelWorksheet workSheet)
        {
            var itemInputDeterminants = DbWMS.dataItemInputDeterminant.Where(x => x.Active).ToList();
            var models = DbWMS.codeWarehouseItemModelLanguage.ToList();
            int totalRows = workSheet.Dimension.End.Row;
            bool valid = true;
            for (int i = 2; i < totalRows; i++)
            {
                var ModelName = workSheet.Cells["A" + i].Value;
                if (ModelName == null)
                    break;

                var currentBarcode = workSheet.Cells["D" + i].Value;
                var currentSerial = workSheet.Cells["E" + i].Value;
                var currentIME = workSheet.Cells["F" + i].Value;
                var currentGSM = workSheet.Cells["G" + i].Value;

                var currentMAC = workSheet.Cells["Q" + i].Value;
                var currentSeq = workSheet.Cells["R" + i].Value;
                //var currentPhone = workSheet.Cells["R" + i].Value;
                var CheckDet = itemInputDeterminants.Where(x =>
                    x.DeterminantValue == currentBarcode
                    || x.DeterminantValue == currentSerial
                    || x.DeterminantValue == currentIME
                    || x.DeterminantValue == currentGSM
                    || x.DeterminantValue == currentMAC
                    || x.DeterminantValue == currentSeq
                    //|| x.DeterminantValue == currentPhone
                    ).FirstOrDefault();
                string replacement = Regex.Replace(ModelName.ToString().ToLower(), @"\t|\n|\r", "");
                var CheckModelName = models.Where(x => x.ModelDescription.ToString().ToLower() == replacement.ToString().ToLower()).FirstOrDefault();
                var iputDate = workSheet.Cells["N" + i].Value;


                if ((CheckDet != null && CheckDet.DeterminantValue != null) || CheckModelName == null || (currentBarcode == null && currentSerial == null && currentIME == null &&
                    currentMAC == null && currentGSM == null && currentSeq == null))
                {
                    //workSheet.Cells[cellStr].Style.Fill.BackgroundColor.SetColor(Color.Red);
                    //workSheet.Cells[cellStr].Style.Font.Bold = true;

                    workSheet.Cells["A" + i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    workSheet.Cells["A" + i].Style.Fill.BackgroundColor.SetColor(Color.Red);

                    valid = false;
                }
                else
                {
                    workSheet.Cells["A" + i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    workSheet.Cells["A" + i].Style.Fill.BackgroundColor.SetColor(Color.Green);
                }
            }

            return valid;

        }
        #endregion

        #region Items Reservation

        public ActionResult WarehouseModelReservationMovementDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/WMS/Views/ModelReservation/_ModelReservationUpdateModal.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<WarehouseModelReservationMovementDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<WarehouseModelReservationMovementDataTableModel>(DataTable.Filters);
            }

            var Result = (

                from a in DbWMS.dataItemReservation.AsNoTracking().AsExpandable().Where(x => x.Active && x.ItemInputDetailGUID == PK)
                join b in DbWMS.dataItemInputDetail.Where(x => x.Active) on a.ItemInputDetailGUID equals b.ItemInputDetailGUID into LJ1
                from R1 in LJ1.DefaultIfEmpty()
                join c in DbWMS.codeTablesValues.Where(x => x.Active) on a.ReservedForGUID equals c.ValueGUID
                join d in DbWMS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on c.ValueGUID equals d.ValueGUID into LJ2
                from R2 in LJ2.DefaultIfEmpty()


                join e in DbWMS.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.ReservedNameForGUID equals e.UserGUID into LJ3
                from R3 in LJ3.DefaultIfEmpty()
                    //join L in DbWMS.codeWarehouse.Where(x => x.Active) on R1.RequesterNameGUID equals L.WarehouseGUID
                    //join M in DbWMS.codeWarehouseLanguage.Where(x => x.Active && x.LanguageID == LAN) on L.WarehouseGUID equals M.WarehouseGUID into LJ8
                    //from R8 in LJ8.DefaultIfEmpty()

                select new WarehouseModelReservationMovementDataTableModel
                {

                    ItemReservationGUID = a.ItemReservationGUID,
                    ItemInputDetailGUID = R1.ItemInputDetailGUID,
                    ReservationStartDate = a.ReservationStartDate,
                    ReservationEndDate = a.ReservationEndDate,


                    ReservedFor = R2.ValueDescription,
                    ReservedNameFor = R3.FirstName + " " + R3.Surname,

                    Active = a.Active,

                    dataItemReservationRowVersion = a.dataItemReservationRowVersion,

                }).Where(Predicate);


            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }


        public ActionResult WarehouseModelReservationMovementsCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            WarehouseModelReservationMovementsUpdateModel model = new WarehouseModelReservationMovementsUpdateModel
            {
                ItemInputDetailGUID = FK,
                ItemReservationGUID = Guid.Empty,
            };

            return PartialView("~/Areas/WMS/Views/ModelReservation/_ModelReservationUpdateModal.cshtml", model);
        }

        public ActionResult WarehouseModelReservationMovementsUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            var myDetail = DbWMS.dataItemReservation.Find(PK);
            WarehouseModelReservationMovementsUpdateModel model = Mapper.Map(myDetail, new WarehouseModelReservationMovementsUpdateModel());

            return PartialView("~/Areas/WMS/Views/ModelReservation/_ModelReservationUpdateModal.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult WarehouseModelReservationMovementsCreate(WarehouseModelReservationMovementsUpdateModel model)
        {

            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            Guid EntityPK = Guid.NewGuid();
            DateTime ExecutionTime = DateTime.Now;
            dataItemReservation ReservationItemModel = Mapper.Map(model, new dataItemReservation());
            ReservationItemModel.ItemReservationGUID = EntityPK;
            ReservationItemModel.CreatedByGUID = UserGUID;
            ReservationItemModel.CreatedDate = ExecutionTime;
            var _item = DbWMS.dataItemInputDetail.Where(x => x.ItemInputDetailGUID == model.ItemInputDetailGUID).FirstOrDefault();
            _item.ItemStatusGUID = Guid.Parse("675de853-151b-4c2f-93f4-da1434eee789");
            _item.LastReservedByGUID = model.ReservedForGUID;
            _item.LastReservedDate = ExecutionTime;
            DbWMS.Create(ReservationItemModel, Permissions.WarehouseItemsRelease.CreateGuid, ExecutionTime, DbCMS);
            DbWMS.Update(_item, Permissions.WarehouseItemsRelease.CreateGuid, ExecutionTime, DbCMS);

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleUpdateMessage(DataTableNames.WarehouseModelReservationMovementDataTable, DbWMS.PrimaryKeyControl(ReservationItemModel), DbWMS.RowVersionControls(Portal.SingleToList(ReservationItemModel))));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult WarehouseModelReservationMovementsUpdate(WarehouseModelReservationMovementsUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Update, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            // if (!ModelState.IsValid || ActiveReleaseModel(model)) return PartialView("~/Areas/WMS/Views/ReleaseModels/_ReleaseModelForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            dataItemReservation ReservationModel = Mapper.Map(model, new dataItemReservation());

            DbWMS.Update(ReservationModel, Permissions.WarehouseItemsRelease.UpdateGuid, ExecutionTime, DbCMS);



            DbWMS.Update(ReservationModel, Permissions.WarehouseItemsRelease.UpdateGuid, ExecutionTime, DbCMS);


            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleUpdateMessage(null, null, DbWMS.RowVersionControls(Portal.SingleToList(ReservationModel))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyReservationModel((Guid)model.ItemReservationGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ReservationModelDelete(dataItemReservation model)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Delete, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            List<dataItemReservation> DeletedReleaseModel = DeleteReservationModel(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.WarehouseItemsRelease.Restore, Apps.WMS), Container = "ResverationModelFormControls" });

            try
            {
                int CommitedRows = DbWMS.SaveChanges();
                DbWMS.SaveChanges();
                return Json(DbWMS.SingleDeleteMessage(CommitedRows, DeletedReleaseModel.FirstOrDefault(), "ResverationModelFormControls", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyReservationModel(model.ItemReservationGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ReservationModelRestore(dataItemReservation model)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Restore, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (ActiveReservationModel(model))
            {
                return Json(DbWMS.RecordExists());
            }

            List<dataItemReservation> RestoredResverationModel = RestoreReservationModel(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.WarehouseItemsRelease.Create, Apps.WMS, new UrlHelper(Request.RequestContext).Action("WarehouseModelReservationMovementsCreate", "ModelMovements", new { Area = "WMS" })), Container = "ReservationModelFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.WarehouseItemsRelease.Update, Apps.WMS), Container = "ResverationModelFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.WarehouseItemsRelease.Delete, Apps.WMS), Container = "ResverationModelFormControls" });

            try
            {
                int CommitedRows = DbWMS.SaveChanges();
                DbWMS.SaveChanges();
                return Json(DbWMS.SingleRestoreMessage(CommitedRows, RestoredResverationModel, DbWMS.PrimaryKeyControl(RestoredResverationModel.FirstOrDefault()), null, null, UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyReservationModel(model.ItemReservationGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]


        public JsonResult WarehouseModelReservationMovementDataTableDelete(List<dataItemReservation> models)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Delete, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            List<dataItemReservation> DeletedReservationModel = DeleteReservationModel(models);

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.PartialDeleteMessage(DeletedReservationModel, models, DataTableNames.WarehouseModelReservationMovementDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult WarehouseModelReservationMovementsRestore(List<dataItemReservation> models)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Restore, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            List<dataItemReservation> RestoredReservationModel = RestoreReservationModel(models);

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.PartialRestoreMessage(RestoredReservationModel, models, DataTableNames.WarehouseModelReservationMovementDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        private List<dataItemReservation> DeleteReservationModel(List<dataItemReservation> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<dataItemReservation> DeletedDamagedModel = new List<dataItemReservation>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbWMS.QueryBuilder(models, Permissions.WarehouseItemsRelease.DeleteGuid, SubmitTypes.Delete, baseQuery);

            var Records = DbWMS.Database.SqlQuery<dataItemReservation>(query).ToList();
            foreach (var record in Records)
            {
                DeletedDamagedModel.Add(DbWMS.Delete(record, ExecutionTime, Permissions.WarehouseItemsRelease.DeleteGuid, DbCMS));
            }

            //var Languages = DeletedReleaseModel.SelectMany(a => a.dataItemOutputLanguage).Where(l => l.Active).ToList();
            //foreach (var language in Languages)
            //{
            //    DbWMS.Delete(language, ExecutionTime, Permissions.WarehouseItemsRelease.DeleteGuid, DbCMS);
            //}
            return DeletedDamagedModel;
        }

        private List<dataItemReservation> RestoreReservationModel(List<dataItemReservation> models)
        {

            DateTime RestoringTime = DateTime.Now;

            List<dataItemReservation> RestoredReservationModel = new List<dataItemReservation>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbWMS.QueryBuilder(models, Permissions.WarehouseItemsRelease.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Records = DbWMS.Database.SqlQuery<dataItemReservation>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveReservationModel(record))
                {
                    RestoredReservationModel.Add(DbWMS.Restore(record, Permissions.WarehouseItemsRelease.DeleteGuid, Permissions.WarehouseItemsRelease.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            //var Languages = RestoredReleaseModel.SelectMany(x => x.dataItemOutputLanguage.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            //foreach (var language in Languages)
            //{
            //    DbWMS.Restore(language, Permissions.WarehouseItemsRelease.DeleteGuid, Permissions.WarehouseItemsRelease.RestoreGuid, RestoringTime, DbCMS);
            //}

            return RestoredReservationModel;
        }

        private JsonResult ConcurrencyReservationModel(Guid PK)
        {
            dataItemReservation dbModel = new dataItemReservation();

            var ReservationModel = DbWMS.dataItemReservation.Where(x => x.ItemReservationGUID == PK).FirstOrDefault();
            var dbReservationModel = DbWMS.Entry(ReservationModel).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbReservationModel, dbModel);

            //var Language = DbWMS.dataItemOutputLanguage.Where(x => x.ItemOutputGUID == PK).Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.dataItemOutput.DeletedOn) && x.LanguageID == LAN).FirstOrDefault();
            //var dbLanguage = DbWMS.Entry(Language).GetDatabaseValues().ToObject();
            //dbModel = Mapper.Map(dbLanguage, dbModel);

            //if (ReleaseModel.dataItemOutputRowVersion.SequenceEqual(dbModel.dataItemOutputRowVersion) && Language.dataItemOutputLanguageRowVersion.SequenceEqual(dbModel.dataItemOutputLanguageRowVersion))
            //{
            //    return Json(DbWMS.PermissionError());
            //}

            return Json(JsonMessages.ConcurrencyError(DbWMS, dbModel, "ReservationModelDetailsContainer"));
        }

        private bool ActiveReservationModel(Object model)
        {
            dataItemReservation damagedModel = Mapper.Map(model, new dataItemReservation());
            int ModelDescription = DbWMS.dataItemReservation
                                    .Where(x =>
                                                x.ItemInputDetailGUID == damagedModel.ItemInputDetailGUID &&
                                                x.ReservationStartDate == damagedModel.ReservationStartDate &&


                                                x.Active).Count();
            if (ModelDescription > 0)
            {
                ModelState.AddModelError("Reservation Description", "This record is already exists");
            }
            return (ModelDescription > 0);
        }




        #endregion

        #region Confirm Receiving Models
        //yyyzz

        public ActionResult ConfirmItemUpdate()
        {



            return View("~/Areas/WMS/Views/ModelConfirmation/ConfirmItemUpdate.cshtml");


        }
        public ActionResult ConfirmItem(Guid id)
        {
            if (id != null && id != Guid.Empty)
            {


                var myFlow = DbWMS.dataItemOutputDetailFlow.Where(x => x.ItemOutputDetailGUID == id
                                                                       && x.Active
                                                                       && x.IsLastAction == true
                                                                       && x.FlowTypeGUID ==
                                                                       WarehouseRequestFlowType.PendingConfirmed)
                    .FirstOrDefault();
                if (myFlow == null)
                {
                    ReleaseSingleItemUpdateModalUpdateModel mymodel = new ReleaseSingleItemUpdateModalUpdateModel();
                    return View("~/Areas/WMS/Views/ModelConfirmation/ConfirmReceivingModelEmail.cshtml", mymodel);
                }

                if (myFlow.dataItemOutputDetail.dataItemOutput.RequesterGUID == WarehouseRequestSourceTypes.Staff)
                {
                    if (myFlow.dataItemOutputDetail.dataItemOutput.RequesterNameGUID != UserGUID)
                    {
                        return Json(DbWMS.PermissionError());
                    }
                }
                else if (myFlow.dataItemOutputDetail.dataItemOutput.RequesterGUID ==
                         WarehouseRequestSourceTypes.Warehouse)
                {
                    if (DbWMS.codeWarehouseFocalPoint.Where(x =>
                            x.WarehouseGUID == myFlow.dataItemOutputDetail.dataItemOutput.RequesterNameGUID
                            && x.UserGUID == UserGUID).FirstOrDefault() == null)
                    {
                        return Json(DbWMS.PermissionError());
                    }
                }
            }

            ReleaseSingleItemUpdateModalUpdateModel model = new ReleaseSingleItemUpdateModalUpdateModel();
            return View("~/Areas/WMS/Views/ModelConfirmation/ConfirmReceivingModelEmail.cshtml", model);


        }

        public ActionResult ConfirmReceivingModelEmail(Guid id)
        {
            var myFlow = DbWMS.dataItemOutputDetailFlow.Where(x => x.ItemOutputDetailGUID == id
                                                                   && x.Active
                                                                   && x.IsLastAction == true
                                                                && x.FlowTypeGUID ==
                                                                   WarehouseRequestFlowType.PendingConfirmed).FirstOrDefault();
            if (myFlow == null)
            {

                ReleaseSingleItemUpdateModalUpdateModel mymodel = new ReleaseSingleItemUpdateModalUpdateModel();
                return View("~/Areas/WMS/Views/ModelConfirmation/ConfirmReceivingModelEmail.cshtml", mymodel);

            }
            if (myFlow.dataItemOutputDetail.dataItemOutput.RequesterGUID == WarehouseRequestSourceTypes.Staff)
            {
                if (myFlow.dataItemOutputDetail.dataItemOutput.RequesterNameGUID != UserGUID)
                {
                    return Json(DbWMS.PermissionError());
                }
            }
            else if (myFlow.dataItemOutputDetail.dataItemOutput.RequesterGUID == WarehouseRequestSourceTypes.Warehouse)
            {
                if (DbWMS.codeWarehouseFocalPoint.Where(x =>
                        x.WarehouseGUID == myFlow.dataItemOutputDetail.dataItemOutput.RequesterNameGUID
                        && x.UserGUID == UserGUID).FirstOrDefault() == null)
                {
                    return Json(DbWMS.PermissionError());
                }
            }

            if (myFlow != null)
            {
                dataItemOutputDetailFlow myNewFlow = new dataItemOutputDetailFlow();
                // if (!ModelState.IsValid || ActiveDamagedModelFlow(model)) return PartialView("~/Areas/WMS/Views/ReleaseModelFlows/_ReleaseModelFlowUpdateModal.cshtml", model);
                DateTime ExecutionTime = DateTime.Now;
                List<dataItemOutputDetailFlow> flows = DbWMS.dataItemOutputDetailFlow
                    .Where(x => x.ItemOutputDetailGUID == id).ToList();

                var itemOutputDetail = DbWMS.dataItemOutputDetail
                    .Where(x => x.ItemOutputDetailGUID == id).FirstOrDefault();

                foreach (var item in flows)
                {
                    item.IsLastAction = false;
                    item.IsLastMove = false;
                }

                DbWMS.SaveChanges();
                var myoutputDetail = DbWMS.dataItemOutputDetail
                    .Where(x => x.ItemOutputDetailGUID == id).FirstOrDefault();
                //List<dataItemInputDetail> ItemInputs = DbWMS.dataItemInputDetail
                //    .Where(x => x.ParentItemModelWarehouseGUID == myoutputDetail.ItemInputDetailGUID).ToList();
                List<dataItemOutputDetail> childrenDetails = DbWMS.dataItemOutputDetail.Where(
                    x => x.dataItemOutput.OutputNumber == myoutputDetail.dataItemOutput.OutputNumber
                ).ToList();


                //Children 
                foreach (var myOutputChildren in childrenDetails.Where(x =>
                    x.ItemOutputDetailGUID != id))
                {
                    List<dataItemOutputDetailFlow> myflows = DbWMS.dataItemOutputDetailFlow
                        .Where(x => x.ItemOutputDetailGUID == myOutputChildren.ItemOutputDetailGUID).ToList();
                    foreach (var flowchildren in myflows)
                    {
                        flowchildren.IsLastAction = false;
                        flowchildren.IsLastMove = false;
                    }

                    DbWMS.SaveChanges();
                    int? myorder = DbWMS.dataItemOutputDetailFlow
                                       .Where(x => x.dataItemOutputDetail.ItemInputDetailGUID ==
                                                   myOutputChildren.ItemInputDetailGUID)
                                       .Select(x => x.OrderId).Max() + 1;
                    if (myorder == null)
                        myorder = 1;
                    dataItemOutputDetailFlow myFlowDetailFlow = new dataItemOutputDetailFlow();
                    myFlowDetailFlow.ItemOutputDetailFlowGUID = Guid.NewGuid();
                    myFlowDetailFlow.ItemOutputDetailGUID = myOutputChildren.ItemOutputDetailGUID;
                    myFlowDetailFlow.FlowTypeGUID = WarehouseRequestFlowType.Confirmed;
                    myFlowDetailFlow.CreatedDate = ExecutionTime;
                    myFlowDetailFlow.ItemStatuGUID = WarehouseItemStatus.Functionting;
                    myFlowDetailFlow.IsLastAction = true;
                    myFlowDetailFlow.IsLastMove = true;
                    myFlowDetailFlow.OrderId = myorder;
                    myFlowDetailFlow.CreatedByGUID = UserGUID;
                    myFlowDetailFlow.Active = true;
                    DbWMS.CreateNoAudit(myFlowDetailFlow);
                    var detailChildren = DbWMS.dataItemOutputDetail
                        .Where(x => x.ItemOutputDetailGUID == myOutputChildren.ItemOutputDetailGUID)
                        .FirstOrDefault();
                    var myChildreninput = DbWMS.dataItemInputDetail.Find(detailChildren.ItemInputDetailGUID);
                    myChildreninput.LastFlowTypeGUID = WarehouseRequestFlowType.Confirmed;
                    DbWMS.UpdateNoAudit(myChildreninput);



                }

                int? order = DbWMS.dataItemOutputDetailFlow
                                 .Where(x => x.dataItemOutputDetail.ItemInputDetailGUID ==
                                             itemOutputDetail.ItemInputDetailGUID)
                                 .Select(x => x.OrderId).Max() + 1;
                if (order == null)
                    order = 1;
                //DbWMS.Up(flows, Permissions.WarehouseItemsEntry.UpdateGuid, ExecutionTime, DbCMS);

                myNewFlow.ItemOutputDetailFlowGUID = Guid.NewGuid();
                myNewFlow.FlowTypeGUID = WarehouseRequestFlowType.Confirmed;
                myNewFlow.ItemOutputDetailGUID = id;
                myNewFlow.CreatedDate = ExecutionTime;
                myNewFlow.ItemStatuGUID = WarehouseItemStatus.Functionting;
                myNewFlow.IsLastAction = true;
                myNewFlow.IsLastMove = true;
                myNewFlow.CreatedByGUID = UserGUID;
                myNewFlow.Active = true;
                myNewFlow.OrderId = order;
                DbWMS.CreateNoAudit(myNewFlow);
                var input = DbWMS.dataItemInputDetail.Find(itemOutputDetail.ItemInputDetailGUID);
                input.LastFlowTypeGUID = WarehouseRequestFlowType.Confirmed;
                DbWMS.UpdateNoAudit(input);



                try
                {
                    DbWMS.SaveChanges();
                    DbCMS.SaveChanges();
                    return RedirectToAction("ConfirmItem", new { id = id });
                    //return View("Login");

                    //return View("ModelConfirmationComplate");
                    //return Json(DbWMS.SingleUpdateMessage(DataTableNames.WarehouseModelReleaseMovementsDataTable,
                    //    DbWMS.PrimaryKeyControl(myModel), DbWMS.RowVersionControls(Portal.SingleToList(myModel))));
                }
                catch (Exception ex)
                {
                    return Json(DbWMS.ErrorMessage(ex.Message));
                }
            }
            return RedirectToAction("Index", "Home", false);

        }
        #endregion

        #region Confirm Receiving Bulk Items
        //[HttpPost, ValidateAntiForgeryToken]



        public ActionResult ConfirmReceivingBulkModelsEmail(List<Guid> guids)
        {
            //if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Create, Apps.WMS))
            //{
            //    return Json(DbWMS.PermissionError());
            //}
            //dataItemOutputDetailFlow myModel = Mapper.Map(model, new dataItemOutputDetailFlow());
            var model = DbWMS.dataItemOutputDetail.Where(x => guids.Contains(x.ItemOutputDetailGUID)).ToList();
            var myware = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
            if (model.FirstOrDefault().dataItemOutput.RequesterNameGUID == UserGUID ||
                (model.FirstOrDefault().dataItemOutput.RequesterGUID == WarehouseRequestSourceTypes.Warehouse && (myware != null && model.FirstOrDefault().dataItemOutput.RequesterNameGUID == myware.WarehouseGUID))
                )
            {
                var requester = model.Select(x => x.dataItemOutput.RequesterGUID).FirstOrDefault();
                List<dataItemOutputDetailFlow> allflows = DbWMS.dataItemOutputDetailFlow
                    .Where(x => guids.Contains((Guid)x.ItemOutputDetailGUID)
                                && x.FlowTypeGUID == WarehouseRequestFlowType.PendingConfirmed


                                 && x.IsLastAction == true).ToList();
                allflows = allflows.Where(x =>
                                (x.dataItemOutputDetail.dataItemOutput.RequesterNameGUID == UserGUID
                                || (model.FirstOrDefault().dataItemOutput.RequesterGUID == WarehouseRequestSourceTypes.Warehouse &&
                                myware != null && model.FirstOrDefault().dataItemOutput.RequesterNameGUID == myware.WarehouseGUID)
                                )).ToList();

                if (allflows.Count > 0)
                {
                    var outputGuids = allflows.Select(x => x.ItemOutputDetailGUID).ToList();
                    var inputGuids = model.Select(x => x.ItemInputDetailGUID).ToList();
                    foreach (var myRelease in model.Where(x => outputGuids.Contains(x.ItemOutputDetailGUID)))
                    {
                        DateTime ExecutionTime = DateTime.Now;
                        List<dataItemOutputDetailFlow> flows = DbWMS.dataItemOutputDetailFlow
                            .Where(x => x.ItemOutputDetailGUID == myRelease.ItemOutputDetailGUID
                                        ).ToList();

                        var itemOutputDetail = DbWMS.dataItemOutputDetail
                            .Where(x => x.ItemOutputDetailGUID == myRelease.ItemOutputDetailGUID).FirstOrDefault();

                        foreach (var flow in flows)
                        {
                            flow.IsLastAction = false;
                            flow.IsLastMove = false;
                        }

                        DbWMS.SaveChanges();
                        var childrenDetails = DbWMS.dataItemOutputDetail
                                      .Where(x => inputGuids.Contains(x.dataItemInputDetail.ParentItemModelWarehouseGUID)).ToList();



                        //Children 
                        foreach (var myOutputChildren in childrenDetails.Where(x => x.ItemOutputDetailGUID != myRelease.ItemOutputDetailGUID))
                        {
                            List<dataItemOutputDetailFlow> myflows = DbWMS.dataItemOutputDetailFlow
                                .Where(x => x.ItemOutputDetailGUID == myOutputChildren.ItemOutputDetailGUID).ToList();
                            foreach (var flowchildren in myflows)
                            {
                                flowchildren.IsLastAction = false;
                                flowchildren.IsLastMove = false;
                            }

                            DbWMS.SaveChanges();
                            int? myorder = DbWMS.dataItemOutputDetailFlow
                                             .Where(x => x.dataItemOutputDetail.ItemInputDetailGUID == myOutputChildren.ItemInputDetailGUID)
                                             .Select(x => x.OrderId).Max() + 1;
                            if (myorder == null)
                                myorder = 1;
                            dataItemOutputDetailFlow myFlowDetailFlow = new dataItemOutputDetailFlow();
                            myFlowDetailFlow.ItemOutputDetailFlowGUID = Guid.NewGuid();
                            myFlowDetailFlow.ItemOutputDetailGUID = myOutputChildren.ItemOutputDetailGUID;
                            if (requester == WarehouseRequestSourceTypes.Warehouse)
                            {
                                myFlowDetailFlow.FlowTypeGUID = WarehouseRequestFlowType.Returned;
                            }
                            else
                            {
                                myFlowDetailFlow.FlowTypeGUID = WarehouseRequestFlowType.Confirmed;
                            }

                            myFlowDetailFlow.FlowTypeGUID = WarehouseRequestFlowType.Confirmed;
                            myFlowDetailFlow.CreatedDate = ExecutionTime;
                            myFlowDetailFlow.ItemStatuGUID = WarehouseItemStatus.Functionting;
                            myFlowDetailFlow.IsLastAction = true;
                            myFlowDetailFlow.IsLastMove = true;
                            myFlowDetailFlow.OrderId = myorder;
                            myFlowDetailFlow.CreatedByGUID = UserGUID;
                            myFlowDetailFlow.Active = true;
                            DbWMS.CreateNoAudit(myFlowDetailFlow);
                            var detailChildren = DbWMS.dataItemOutputDetail.Where(x => x.ItemOutputDetailGUID == myOutputChildren.ItemOutputDetailGUID)
                                .FirstOrDefault();
                            var myChildreninput = DbWMS.dataItemInputDetail.Find(detailChildren.ItemInputDetailGUID);
                            if (requester == WarehouseRequestSourceTypes.Warehouse)
                            {
                                myChildreninput.LastFlowTypeGUID = WarehouseRequestFlowType.Returned;
                            }
                            else
                            {
                                myChildreninput.LastFlowTypeGUID = WarehouseRequestFlowType.Confirmed;
                            }

                            DbWMS.UpdateNoAudit(myChildreninput);



                        }

                        int? order = DbWMS.dataItemOutputDetailFlow
                                         .Where(x => x.dataItemOutputDetail.ItemInputDetailGUID == itemOutputDetail.ItemInputDetailGUID)
                                         .Select(x => x.OrderId).Max() + 1;
                        if (order == null)
                            order = 1;
                        //DbWMS.Up(flows, Permissions.WarehouseItemsEntry.UpdateGuid, ExecutionTime, DbCMS);
                        dataItemOutputDetailFlow myModel = new dataItemOutputDetailFlow();
                        myModel.ItemOutputDetailFlowGUID = Guid.NewGuid();
                        if (requester == WarehouseRequestSourceTypes.Warehouse)
                        {
                            myModel.FlowTypeGUID = WarehouseRequestFlowType.Returned;
                        }
                        else
                        {
                            myModel.FlowTypeGUID = WarehouseRequestFlowType.Confirmed;
                        }
                        myModel.ItemOutputDetailGUID = myRelease.ItemOutputDetailGUID;

                        myModel.CreatedDate = ExecutionTime;
                        myModel.ItemStatuGUID = WarehouseItemStatus.Functionting;
                        myModel.IsLastAction = true;
                        myModel.IsLastMove = true;
                        myModel.CreatedByGUID = UserGUID;
                        myModel.Active = true;
                        myModel.OrderId = order;
                        DbWMS.CreateNoAudit(myModel);
                        var input = DbWMS.dataItemInputDetail.Find(myRelease.ItemInputDetailGUID);
                        if (requester == WarehouseRequestSourceTypes.Warehouse)
                        {
                            input.LastFlowTypeGUID = WarehouseRequestFlowType.Returned;
                        }
                        else
                        {
                            input.LastFlowTypeGUID = WarehouseRequestFlowType.Confirmed;
                        }

                        DbWMS.UpdateNoAudit(input);




                    }
                    // if (!ModelState.IsValid || ActiveDamagedModelFlow(model)) return PartialView("~/Areas/WMS/Views/ReleaseModelFlows/_ReleaseModelFlowUpdateModal.cshtml", model);

                    try
                    {
                        DbWMS.SaveChanges();
                        DbCMS.SaveChanges();
                        //z0

                        SendItemConfirmationReceiving(guids);

                        return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);
                        //return RedirectToAction("ConfirmReceivingModelEmail", new { id = model.Select(f => f.ItemOutputDetailGUID).FirstOrDefault() });
                    }

                    catch (Exception ex)
                    {

                        return Json(DbWMS.ErrorMessage(ex.Message));
                    }
                }
                return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = 0 }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ConfirmReceivingCustodianBulkModelEmail(Guid myId)
        {
            //ss

            var myFlow = DbWMS.dataItemOutputDetailFlow.Where(x => x.dataItemOutputDetail.dataItemInputDetail.LastCustdianNameGUID == myId
                                                                   && x.Active
                                                                   && x.IsLastAction == true
                                                                   && x.FlowTypeGUID ==
                                                                   WarehouseRequestFlowType.PendingConfirmed).ToList();
            if (myFlow.Select(x => x.dataItemOutputDetail.dataItemOutput.RequesterGUID).FirstOrDefault() == WarehouseRequestSourceTypes.Staff)
            {
                if (myFlow.Select(x => x.dataItemOutputDetail.dataItemOutput.RequesterNameGUID).FirstOrDefault() != UserGUID)
                {
                    return Json(DbWMS.PermissionError());
                }
            }
            else if (myFlow.Select(x => x.dataItemOutputDetail.dataItemOutput.RequesterGUID).FirstOrDefault() == WarehouseRequestSourceTypes.Warehouse)
            {
                if (DbWMS.codeWarehouseFocalPoint.Where(x =>
                        x.WarehouseGUID == myFlow.Select(f => f.dataItemOutputDetail.dataItemOutput.RequesterNameGUID).FirstOrDefault()
                        && x.UserGUID == UserGUID).FirstOrDefault() == null)
                {
                    return Json(DbWMS.PermissionError());
                }
            }
            if (myFlow != null && myFlow.Count > 0)
            {
                var myFlowGuis = myFlow.Select(x => x.ItemOutputDetailGUID).ToList();

                var myDetail = DbWMS.dataItemOutputDetail.Where(x => x.dataItemInputDetail.LastCustdianNameGUID == myId
                                                                     && myFlowGuis.Contains(x.ItemOutputDetailGUID)).ToList();

                var determinantModel = DbWMS.dataItemInputDeterminant
                    .ToList();
                List<ReleaseSingleItemUpdateModalUpdateModel> model = new List<ReleaseSingleItemUpdateModalUpdateModel>();
                foreach (var item in myDetail)
                {
                    ReleaseSingleItemUpdateModalUpdateModel mymodel = new ReleaseSingleItemUpdateModalUpdateModel();
                    mymodel.ItemInputDetailGUID = item.ItemInputDetailGUID;
                    mymodel.RequesterNameGUID = myId;
                    // mymodel.ItemOutputGUID = id;
                    mymodel.ItemOutputDetailGUID = item.ItemOutputDetailGUID;
                    mymodel.ModelDescription = item.dataItemInputDetail.codeItemModelWarehouse.codeWarehouseItemModel
                        .codeWarehouseItemModelLanguage.Where(x => x.LanguageID == LAN).Select(x => x.ModelDescription)
                        .FirstOrDefault();
                    mymodel.BarcodeNumber = determinantModel
                        .Where(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.Barcode && x.ItemInputDetailGUID == item.ItemInputDetailGUID)
                        .Select(x => x.DeterminantValue).FirstOrDefault();
                    mymodel.SerialNumber = determinantModel
                        .Where(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.SerialNumber && x.ItemInputDetailGUID == item.ItemInputDetailGUID)
                        .Select(x => x.DeterminantValue).FirstOrDefault();
                    mymodel.GSM = determinantModel
                        .Where(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.GSM && x.ItemInputDetailGUID == item.ItemInputDetailGUID)
                        .Select(x => x.DeterminantValue).FirstOrDefault();
                    mymodel.IME1 = determinantModel
                        .Where(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.IME1 && x.ItemInputDetailGUID == item.ItemInputDetailGUID)
                        .Select(x => x.DeterminantValue).FirstOrDefault();
                    mymodel.MAC = determinantModel
                        .Where(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.MAC && x.ItemInputDetailGUID == item.ItemInputDetailGUID)
                        .Select(x => x.DeterminantValue).FirstOrDefault();
                    mymodel.SequenceNumber = determinantModel
              .Where(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.SeqNumber && x.ItemInputDetailGUID == item.ItemInputDetailGUID)
              .Select(x => x.DeterminantValue).FirstOrDefault();
                    mymodel.Validation = true;
                    model.Add(mymodel);


                }


                ViewBag.RequesterNameGUID = myId;



                return View("~/Areas/WMS/Views/ModelConfirmation/ConfirmReceivingBulkModelsByCustdoianEmail.cshtml", model);
            }

            return View("~/Areas/WMS/Views/ModelConfirmation/ConfirmReceivingBulkModelsByCustdoianEmail.cshtml", new ReleaseSingleItemUpdateModalUpdateModel());

        }

        public ActionResult ConfirmReceivingBulkModelEmail(Guid id)
        {
            //ss

            var myFlow = DbWMS.dataItemOutputDetailFlow.Where(x => x.dataItemOutputDetail.ItemOutputGUID == id
                                                                   && x.Active
                                                                   && x.IsLastAction == true
                                                                   && x.FlowTypeGUID ==
                                                                   WarehouseRequestFlowType.PendingConfirmed).ToList();
            if (myFlow.Select(x => x.dataItemOutputDetail.dataItemOutput.RequesterGUID).FirstOrDefault() == WarehouseRequestSourceTypes.Staff)
            {
                if (myFlow.Select(x => x.dataItemOutputDetail.dataItemOutput.RequesterNameGUID).FirstOrDefault() != UserGUID)
                {
                    return Json(DbWMS.PermissionError());
                }
            }
            else if (myFlow.Select(x => x.dataItemOutputDetail.dataItemOutput.RequesterGUID).FirstOrDefault() == WarehouseRequestSourceTypes.Warehouse)
            {
                var requesterNameGUID = myFlow.Select(f => f.dataItemOutputDetail.dataItemOutput.RequesterNameGUID).FirstOrDefault();
                var focalPointGUID = DbWMS.codeWarehouseFocalPoint.Where(x =>
                      x.WarehouseGUID == requesterNameGUID
                      && x.UserGUID == UserGUID).FirstOrDefault();
                if (focalPointGUID == null)
                {
                    return Json(DbWMS.PermissionError());
                }
            }
            if (myFlow != null && myFlow.Count > 0)
            {
                var myFlowGuis = myFlow.Select(x => x.ItemOutputDetailGUID).ToList();

                var myDetail = DbWMS.dataItemOutputDetail.Where(x => x.ItemOutputGUID == id
                                                                     && myFlowGuis.Contains(x.ItemOutputDetailGUID)).ToList();

                var determinantModel = DbWMS.dataItemInputDeterminant
                    .ToList();
                List<ReleaseSingleItemUpdateModalUpdateModel> model = new List<ReleaseSingleItemUpdateModalUpdateModel>();
                foreach (var item in myDetail)
                {
                    ReleaseSingleItemUpdateModalUpdateModel mymodel = new ReleaseSingleItemUpdateModalUpdateModel();
                    mymodel.ItemInputDetailGUID = item.ItemInputDetailGUID;
                    mymodel.ItemOutputGUID = id;
                    mymodel.ItemOutputDetailGUID = item.ItemOutputDetailGUID;
                    mymodel.ExpectedStartDate = item.ExpectedStartDate;

                    mymodel.ExpectedReturenedDate = item.ExpectedReturenedDate;
                    mymodel.IssuedBy = item.userAccounts.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).Select(x => x.FirstName).FirstOrDefault() + " " + item.userAccounts.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).Select(x => x.Surname).FirstOrDefault();
                    mymodel.WarehouseItemDescription = item.dataItemInputDetail.codeItemModelWarehouse.codeWarehouseItemModel
                        .codeWarehouseItem.codeWarehouseItemLanguage.Where(x => x.LanguageID == LAN).Select(x => x.WarehouseItemDescription)
                        .FirstOrDefault();
                    mymodel.BrandDescription = item.dataItemInputDetail.codeItemModelWarehouse.codeWarehouseItemModel
                        .codeBrand.codeBrandLanguage.Where(x => x.LanguageID == LAN).Select(x => x.BrandDescription)
                        .FirstOrDefault();

                    //zz
                    mymodel.WarehouseItemDescription = item.dataItemInputDetail.codeItemModelWarehouse.codeWarehouseItemModel
                        .codeWarehouseItem.codeWarehouseItemLanguage.Where(x => x.LanguageID == LAN).Select(x => x.WarehouseItemDescription)
                        .FirstOrDefault();
                    mymodel.BrandDescription = item.dataItemInputDetail.codeItemModelWarehouse.codeWarehouseItemModel
                        .codeBrand.codeBrandLanguage.Where(x => x.LanguageID == LAN).Select(x => x.BrandDescription)
                        .FirstOrDefault();
                    mymodel.ModelDescription = item.dataItemInputDetail.codeItemModelWarehouse.codeWarehouseItemModel
                        .codeWarehouseItemModelLanguage.Where(x => x.LanguageID == LAN).Select(x => x.ModelDescription)
                        .FirstOrDefault();
                    mymodel.BarcodeNumber = determinantModel
                        .Where(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.Barcode && x.ItemInputDetailGUID == item.ItemInputDetailGUID)
                        .Select(x => x.DeterminantValue).FirstOrDefault();
                    mymodel.SerialNumber = determinantModel
                        .Where(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.SerialNumber && x.ItemInputDetailGUID == item.ItemInputDetailGUID)
                        .Select(x => x.DeterminantValue).FirstOrDefault();
                    mymodel.GSM = determinantModel
                        .Where(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.GSM && x.ItemInputDetailGUID == item.ItemInputDetailGUID)
                        .Select(x => x.DeterminantValue).FirstOrDefault();
                    mymodel.IME1 = determinantModel
                        .Where(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.IME1 && x.ItemInputDetailGUID == item.ItemInputDetailGUID)
                        .Select(x => x.DeterminantValue).FirstOrDefault();
                    mymodel.MAC = determinantModel
                   .Where(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.MAC && x.ItemInputDetailGUID == item.ItemInputDetailGUID)
                   .Select(x => x.DeterminantValue).FirstOrDefault();
                    mymodel.SequenceNumber = determinantModel
            .Where(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.SeqNumber && x.ItemInputDetailGUID == item.ItemInputDetailGUID)
            .Select(x => x.DeterminantValue).FirstOrDefault();
                    mymodel.Validation = true;
                    model.Add(mymodel);


                }


                ViewBag.ItemOutputGUID = id;



                return View("~/Areas/WMS/Views/ModelConfirmation/ConfirmReceivingBulkModelsEmail.cshtml", model);
            }

            return View("~/Areas/WMS/Views/ModelConfirmation/ConfirmReceivingModelEmail.cshtml", new ReleaseSingleItemUpdateModalUpdateModel());

        }





        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ConfirmReceivingBulkModelEmail(List<ReleaseSingleItemUpdateModalUpdateModel> model)
        {
            //if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Create, Apps.WMS))
            //{
            //    return Json(DbWMS.PermissionError());
            //}
            //dataItemOutputDetailFlow myModel = Mapper.Map(model, new dataItemOutputDetailFlow());
            // if (!ModelState.IsValid || ActiveDamagedModelFlow(model)) return PartialView("~/Areas/WMS/Views/ReleaseModelFlows/_ReleaseModelFlowUpdateModal.cshtml", model);
            DateTime ExecutionTime = DateTime.Now;
            List<dataItemOutputDetailFlow> flowDetails = new List<dataItemOutputDetailFlow>();
            foreach (var item in model)
            {
                List<dataItemOutputDetailFlow> flows = DbWMS.dataItemOutputDetailFlow
               .Where(x => x.ItemOutputDetailGUID == item.ItemOutputDetailGUID).ToList();

                var itemOutputDetail = DbWMS.dataItemOutputDetail
                    .Where(x => x.ItemOutputDetailGUID == item.ItemInputDetailGUID).FirstOrDefault();

                foreach (var flow in flows)
                {
                    flow.IsLastAction = false;
                    flow.IsLastMove = false;
                }

                DbWMS.SaveChanges();
                var myoutputDetail = DbWMS.dataItemOutputDetail
        .Where(x => x.ItemOutputDetailGUID == item.ItemOutputDetailGUID).FirstOrDefault();
                //List<dataItemInputDetail> ItemInputs = DbWMS.dataItemInputDetail
                //    .Where(x => x.ParentItemModelWarehouseGUID == myoutputDetail.ItemInputDetailGUID).ToList();
                List<dataItemOutputDetail> childrenDetails = DbWMS.dataItemOutputDetail.Where(
                    x => x.dataItemOutput.OutputNumber == myoutputDetail.dataItemOutput.OutputNumber
                ).ToList();


                //Children 
                foreach (var myOutputChildren in childrenDetails.Where(x => x.ItemOutputDetailGUID != item.ItemOutputDetailGUID))
                {
                    List<dataItemOutputDetailFlow> myflows = DbWMS.dataItemOutputDetailFlow
                        .Where(x => x.ItemOutputDetailGUID == myOutputChildren.ItemOutputDetailGUID).ToList();
                    foreach (var flowchildren in myflows)
                    {
                        flowchildren.IsLastAction = false;
                        flowchildren.IsLastMove = false;
                    }

                    DbWMS.SaveChanges();
                    int? myorder = DbWMS.dataItemOutputDetailFlow
                                     .Where(x => x.dataItemOutputDetail.ItemInputDetailGUID == myOutputChildren.ItemInputDetailGUID)
                                     .Select(x => x.OrderId).Max() + 1;
                    if (myorder == null)
                        myorder = 1;
                    dataItemOutputDetailFlow myFlowDetailFlow = new dataItemOutputDetailFlow();
                    myFlowDetailFlow.ItemOutputDetailFlowGUID = Guid.NewGuid();
                    myFlowDetailFlow.ItemOutputDetailGUID = myOutputChildren.ItemOutputDetailGUID;
                    myFlowDetailFlow.FlowTypeGUID = WarehouseRequestFlowType.Confirmed;
                    myFlowDetailFlow.CreatedDate = ExecutionTime;
                    myFlowDetailFlow.ItemStatuGUID = WarehouseItemStatus.Functionting;
                    myFlowDetailFlow.IsLastAction = true;
                    myFlowDetailFlow.IsLastMove = true;
                    myFlowDetailFlow.OrderId = myorder;
                    myFlowDetailFlow.CreatedByGUID = UserGUID;
                    myFlowDetailFlow.Active = true;
                    DbWMS.CreateNoAudit(myFlowDetailFlow);
                    var detailChildren = DbWMS.dataItemOutputDetail.Where(x => x.ItemOutputDetailGUID == myOutputChildren.ItemOutputDetailGUID)
                        .FirstOrDefault();
                    var myChildreninput = DbWMS.dataItemInputDetail.Find(detailChildren.ItemInputDetailGUID);
                    myChildreninput.LastFlowTypeGUID = WarehouseRequestFlowType.Confirmed;
                    DbWMS.UpdateNoAudit(myChildreninput);



                }

                int? order = DbWMS.dataItemOutputDetailFlow
                                 .Where(x => x.dataItemOutputDetail.ItemInputDetailGUID == itemOutputDetail.ItemInputDetailGUID)
                                 .Select(x => x.OrderId).Max() + 1;
                if (order == null)
                    order = 1;
                //DbWMS.Up(flows, Permissions.WarehouseItemsEntry.UpdateGuid, ExecutionTime, DbCMS);
                dataItemOutputDetailFlow flowDetail = new dataItemOutputDetailFlow();
                flowDetail.ItemOutputDetailFlowGUID = Guid.NewGuid();
                flowDetail.FlowTypeGUID = WarehouseRequestFlowType.Confirmed;
                flowDetail.ItemOutputDetailGUID = item.ItemOutputDetailGUID;

                flowDetail.CreatedDate = ExecutionTime;
                flowDetail.ItemStatuGUID = WarehouseItemStatus.Functionting;
                flowDetail.IsLastAction = true;
                flowDetail.IsLastMove = true;
                flowDetail.CreatedByGUID = UserGUID;
                flowDetail.Active = true;
                flowDetail.OrderId = order;
                //flowDetails.Add(flowDetail);
                DbWMS.CreateNoAudit(flowDetail);
                var input = DbWMS.dataItemInputDetail.Find(item.ItemInputDetailGUID);
                input.LastFlowTypeGUID = WarehouseRequestFlowType.Confirmed;
                DbWMS.UpdateNoAudit(input);


            }



            try
            {

                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return RedirectToAction("ConfirmReceivingModelEmail", new { id = model.Select(x => x.ItemOutputDetailGUID).FirstOrDefault() });
                //return View("Login");

                //return View("ModelConfirmationComplate");
                //return Json(DbWMS.SingleUpdateMessage(DataTableNames.WarehouseModelReleaseMovementsDataTable, DbWMS.PrimaryKeyControl(myModel), DbWMS.RowVersionControls(Portal.SingleToList(myModel))));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }



        }

        #endregion


        #region mails
        //first
        public void SendConfirmationReceivingModelEmail(dataItemOutputDetail model)
        {

            string URL = AppSettingsKeys.Domain + "/WMS/ModelMovements/ConfirmReceivingBulkModelEmail/?id=" + new Portal().GUIDToString(model.ItemOutputGUID);
            string Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ConfirmItemReceiving + "</a>";
            string Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";
            string standardURL = AppSettingsKeys.Domain + "/WMS/ModelMovements/GetICTSOPFile/";
            string LinkStandard = "<a href='" + standardURL + "' target='_blank'>" + "Syria ICT equipment SOP" + "</a>";
            List<userServiceHistory> users = new List<userServiceHistory>();
            if (model.dataItemOutput.RequesterGUID == WarehouseRequestSourceTypes.Warehouse)
            {
                var warehouseFocalPointsGUIDs = DbWMS.codeWarehouseFocalPoint.Where(x => x.WarehouseGUID == model.dataItemOutput.RequesterNameGUID
                && x.IsFocalPoint == true)
                      .Select(x => x.UserGUID).ToList();
                users = DbCMS.userServiceHistory.Where(x => warehouseFocalPointsGUIDs.Contains(x.UserGUID)).ToList();
            }

            else if (model.dataItemOutput.RequesterGUID == WarehouseRequestSourceTypes.Staff)
            {
                users = DbCMS.userServiceHistory.Where(x => x.UserGUID == model.dataItemOutput.RequesterNameGUID)
                    .ToList();
            }
            var CheckChildren = DbWMS.dataItemInputDetail.Where(x => x.ParentItemModelWarehouseGUID == model.ItemInputDetailGUID).ToList();

            string children = "";
            if (CheckChildren.Count > 0)
            {
                var iteminputGUIDs = CheckChildren.Select(x => x.ItemInputDetailGUID).Distinct();
                var itemInputDetails = DbWMS.dataItemInputDetail.Where(x => iteminputGUIDs.Contains(x.ItemInputDetailGUID)).ToList();
                foreach (var mychildren in iteminputGUIDs)
                {
                    var temp = itemInputDetails.Where(x => x.ItemInputDetailGUID == mychildren).FirstOrDefault();
                    children = children + "Item Name :" + temp.codeItemModelWarehouse.codeWarehouseItemModel.codeWarehouseItemModelLanguage.Where(f => f.LanguageID == LAN).FirstOrDefault().ModelDescription + "  ";
                    if (temp.dataItemInputDeterminant.Where(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.Barcode).Select(x => x.DeterminantValue).FirstOrDefault() != null)
                    {
                        children = children + "Barcode Number" + temp.dataItemInputDeterminant.Where(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.Barcode).Select(x => x.DeterminantValue).FirstOrDefault() + " , ";

                    }
                    else if (temp.dataItemInputDeterminant.Where(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.SerialNumber).Select(x => x.DeterminantValue).FirstOrDefault() != null)
                    {
                        children = children + "SerialNumber " + temp.dataItemInputDeterminant.Where(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.SerialNumber).Select(x => x.DeterminantValue).FirstOrDefault() + " , ";

                    }
                    else if (temp.dataItemInputDeterminant.Where(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.GSM).Select(x => x.DeterminantValue).FirstOrDefault() != null)
                    {
                        children = children + "GSM " + temp.dataItemInputDeterminant.Where(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.GSM).Select(x => x.DeterminantValue).FirstOrDefault() + " , ";

                    }
                    else if (temp.dataItemInputDeterminant.Where(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.IME1).Select(x => x.DeterminantValue).FirstOrDefault() != null)
                    {
                        children = children + "IME1 " + temp.dataItemInputDeterminant.Where(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.IME1).Select(x => x.DeterminantValue).FirstOrDefault() + " , ";

                    }
                    children = children + "   ";

                }

            }
            var BarcodeNumber = model.dataItemInputDetail.dataItemInputDeterminant
           .Where(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.Barcode)
           .Select(x => x.DeterminantValue).FirstOrDefault();
            if (string.IsNullOrEmpty(BarcodeNumber))
                BarcodeNumber = "-";
            var SerialNumber = model.dataItemInputDetail.dataItemInputDeterminant
                .Where(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.SerialNumber)
                .Select(x => x.DeterminantValue).FirstOrDefault();
            if (string.IsNullOrEmpty(SerialNumber))
                SerialNumber = "-";
            var IME1 = model.dataItemInputDetail.dataItemInputDeterminant
                .Where(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.IME1)
                .Select(x => x.DeterminantValue).FirstOrDefault();
            if (string.IsNullOrEmpty(IME1))
                IME1 = "-";
            var GSM = model.dataItemInputDetail.dataItemInputDeterminant
                .Where(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.GSM)
                .Select(x => x.DeterminantValue).FirstOrDefault();
            var MAC = model.dataItemInputDetail.dataItemInputDeterminant
             .Where(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.MAC)
             .Select(x => x.DeterminantValue).FirstOrDefault();
            string det = "";
            if (BarcodeNumber != null)
                det += "Barcode Number: " + BarcodeNumber + ", ";
            if (SerialNumber != null)

                det += "Serial Number: " + SerialNumber + ", ";
            if (IME1 != null)

                det += "IME1: " + IME1 + ", ";
            if (GSM != null)

                det += "GSM Number: " + GSM + ", ";
            if (MAC != null)

                det += "MAC Number: " + MAC + ", ";
            string modelName = model.dataItemInputDetail.codeItemModelWarehouse.codeWarehouseItemModel
                .codeWarehouseItemModelLanguage.Where(x => x.LanguageID == LAN).Select(x => x.ModelDescription)
                .FirstOrDefault();
            string itemName = model.dataItemInputDetail.codeItemModelWarehouse.codeWarehouseItemModel.codeWarehouseItem
                .codeWarehouseItemLanguage.Where(x => x.LanguageID == LAN).Select(f => f.WarehouseItemDescription).FirstOrDefault();

            string brandName = model.dataItemInputDetail.codeItemModelWarehouse.codeWarehouseItemModel.codeBrand.codeBrandLanguage.Where(x => x.LanguageID == LAN)
                .Select(f => f.BrandDescription).FirstOrDefault();
            string expectedStartDate = "";
            string expectedReturnDate = "";
            if (model.ExpectedStartDate != null)
                expectedStartDate = model.ExpectedStartDate.Value.ToLongDateString();
            if (model.ExpectedReturenedDate != null)
                expectedReturnDate = model.ExpectedReturenedDate.Value.ToLongDateString();

            var itemoutput =
                DbWMS.dataItemOutputDetail.Where(x => x.ItemOutputDetailGUID == model.ItemOutputDetailGUID)
                    .FirstOrDefault();
            var personal = DbWMS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN
                                                                         && x.UserGUID == itemoutput.CreatedByGUID).FirstOrDefault();
            string fullName = personal.FirstName + " " + personal.Surname;


            dataItemOutputNotification notification = new dataItemOutputNotification();
            notification.ItemOutputDetailGUID = model.ItemOutputDetailGUID;
            notification.NotificationMessage = "Verification Item(1)";
            notification.NotificationTypeGUID = WarehosueNotificationType.Notification;
            SendNotification(notification);
            string table = "<br><table class='table table-bordered' style='border-collapse: collapse; width: 900px'><tr><th style='border: 1px solid  #333; vertical-align:middle'>Description (SC)</th><th style='border: 1px solid  #333; vertical-align: middle'>Brand</th><th style='border: 1px solid  #333; vertical-align: middle'>Item</th><th style='border: 1px solid  #333; vertical-align: middle'>Barcode</th><th style='border: 1px solid  #333; vertical-align: middle'>SN </th><th style='border: 1px solid  #333; vertical-align: middle'>IMEI </th><th style='border: 1px solid  #333; vertical-align: middle'>GSM </th><th style='border: 1px solid  #333; vertical-align: middle'>MAC </th><th style='border: 1px solid  #333; vertical-align: middle'>Issue Date</th><th style='border: 1px solid  #333; vertical-align: middle'>Return Date</th><th style='border: 1px solid  #333; vertical-align: middle'>Issued By</th><th style='border: 1px solid  #333; vertical-align: middle'>Notes</th></tr><tr><td style='border: 1px solid  #333; vertical-align: middle'>" + itemName + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + brandName + "</td><td style='border: 1px solid  #333; vertical-align: middle'> " + modelName + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + BarcodeNumber + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + SerialNumber + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + IME1 + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + GSM + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + MAC + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + expectedStartDate + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + expectedReturnDate + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + fullName + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + model.Comments + "" + children + "</td></tr></table>";

            var IssuedUser = DbWMS.userPersonalDetailsLanguage.Where(x => x.UserGUID == UserGUID
                                                                          && x.LanguageID == LAN).FirstOrDefault();
            var _issuerCoreEmail = DbCMS.userServiceHistory.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
            var _profile = DbWMS.userProfiles.Where(x => x.UserProfileGUID == UserProfileGUID).FirstOrDefault();
            if (model.dataItemOutput.RequesterGUID == WarehouseRequestSourceTypes.Staff)
            {
                foreach (var user in users)
                {
                    if (_profile.OrganizationInstanceGUID == Guid.Parse("E156C022-EC72-4A5A-BE09-163BD85C68EF"))
                    {
                        string _message = resxEmails.ItemModelVerification
                        .Replace("$FullName", user.userAccounts.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).Select(x => x.FirstName).FirstOrDefault() + " " + user.userAccounts.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).Select(x => x.Surname).FirstOrDefault())
                        .Replace("$VerifyLink", Anchor)
                        .Replace("$Items", table)
                        .Replace("$VerifyStandard", LinkStandard)
                            .Replace("$IssuedUser", "Sent by: " + IssuedUser.FirstName + " " + IssuedUser.Surname);
                        if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                        int isRec = 1;
                        if (user.EmailAddress == "maksoud@unhcr.org")
                            isRec = 0;
                        Send(user.EmailAddress, resxEmails.ItemModelVerificationSubject, _message, isRec, _issuerCoreEmail.EmailAddress);
                    }
                    else
                    {
                        string _message = resxEmails.ItemModelVerificationGeneral
                        .Replace("$FullName", user.userAccounts.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).Select(x => x.FirstName).FirstOrDefault() + " " + user.userAccounts.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).Select(x => x.Surname).FirstOrDefault())
                        .Replace("$VerifyLink", Anchor)
                        .Replace("$Items", table)
                        .Replace("$VerifyStandard", LinkStandard)
                            .Replace("$IssuedUser", "Sent by: " + IssuedUser.FirstName + " " + IssuedUser.Surname);
                        if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                        int isRec = 1;
                        if (user.EmailAddress == "maksoud@unhcr.org")
                            isRec = 0;
                        Send(user.EmailAddress, resxEmails.ItemModelVerificationSubject, _message, isRec, _issuerCoreEmail.EmailAddress);

                    }
                }
            }
            else if (model.dataItemOutput.RequesterGUID == WarehouseRequestSourceTypes.Warehouse)
            {
                foreach (var user in users)
                {
                    if (_profile.OrganizationInstanceGUID == Guid.Parse("E156C022-EC72-4A5A-BE09-163BD85C68EF"))
                    {
                        string _message = resxEmails.warehosueCustodianItemConfirmation
                    .Replace("$FullName", user.userAccounts.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).Select(x => x.FirstName).FirstOrDefault() + " " + user.userAccounts.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).Select(x => x.Surname).FirstOrDefault())
                    .Replace("$VerifyLink", Anchor)
                    .Replace("$Items", table)
                    .Replace("$VerifyStandard", LinkStandard)
                        .Replace("$IssuedUser", "Sent by: " + IssuedUser.FirstName + " " + IssuedUser.Surname);
                        if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                        int isRec = 1;
                        if (user.EmailAddress == "maksoud@unhcr.org")
                            isRec = 0;
                        Send(user.EmailAddress, resxEmails.ItemModelVerificationSubject, _message, isRec, _issuerCoreEmail.EmailAddress);
                    }
                    else
                    {
                        string _message = resxEmails.warehosueCustodianItemConfirmationGeneral
                      .Replace("$FullName", user.userAccounts.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).Select(x => x.FirstName).FirstOrDefault() + " " + user.userAccounts.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).Select(x => x.Surname).FirstOrDefault())
                      .Replace("$VerifyLink", Anchor)
                      .Replace("$Items", table)
                      .Replace("$VerifyStandard", LinkStandard)
                          .Replace("$IssuedUser", "Sent by: " + IssuedUser.FirstName + " " + IssuedUser.Surname);
                        if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                        int isRec = 1;
                        if (user.EmailAddress == "maksoud@unhcr.org")
                            isRec = 0;
                        Send(user.EmailAddress, resxEmails.ItemModelVerificationSubject, _message, isRec, _issuerCoreEmail.EmailAddress);

                    }
                }

            }
        }
        public void SendConfirmationReceivingModelEmailForBulk(Guid myGuid)
        {
            var myItemOutputDetails = DbWMS.dataItemOutputDetail.Where(x => x.ItemOutputGUID == myGuid).ToList();
            Guid createdByGuid = (Guid)myItemOutputDetails.Select(x => x.CreatedByGUID).FirstOrDefault();

            string URL = AppSettingsKeys.Domain + "/WMS/ModelMovements/ConfirmReceivingBulkModelEmail/?id=" + new Portal().GUIDToString(myGuid);
            string standardURL = AppSettingsKeys.Domain + "/WMS/ModelMovements/GetICTSOPFile/";
            string LinkStandard = "<a href='" + standardURL + "' target='_blank'>" + "Syria ICT equipment SOP" + "</a>";
            string Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ConfirmItemReceiving + "</a>";
            string Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";

            List<userServiceHistory> users = new List<userServiceHistory>();
            if (myItemOutputDetails.Select(x => x.dataItemOutput.RequesterGUID).FirstOrDefault() == WarehouseRequestSourceTypes.Warehouse)
            {
                var requesterNameGUID =
                    myItemOutputDetails.Select(f => f.dataItemOutput.RequesterNameGUID).FirstOrDefault();
                var warehouseFocalPointsGUIDs = DbWMS.codeWarehouseFocalPoint.Where(x => x.WarehouseGUID == requesterNameGUID
                && x.IsFocalPoint == true)
                      .Select(x => x.UserGUID).ToList();
                users = DbCMS.userServiceHistory.Where(x => warehouseFocalPointsGUIDs.Contains(x.UserGUID)).ToList();
            }

            else if (myItemOutputDetails.Select(x => x.dataItemOutput.RequesterGUID).FirstOrDefault() == WarehouseRequestSourceTypes.Staff)
            {
                Guid itemGuid = (Guid)myItemOutputDetails.Select(f => f.dataItemOutput.RequesterNameGUID).FirstOrDefault();
                users = DbCMS.userServiceHistory.Where(x => x.UserGUID == itemGuid)
                    .ToList();
            }

            var personal = DbWMS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN
                                                                        && x.UserGUID == createdByGuid).FirstOrDefault();
            string fullName = personal.FirstName + " " + personal.Surname;


            string modelsToConfirm = "";
            string table = "<br><table class='table table-bordered' style='border-collapse: collapse; width: 900px'><tr><th style='border: 1px solid  #333; vertical-align:middle'>Description (SC)</th><th style='border: 1px solid  #333; vertical-align: middle'>Brand</th><th style='border: 1px solid  #333; vertical-align: middle'>Item</th><th style='border: 1px solid  #333; vertical-align: middle'>Barcode</th><th style='border: 1px solid  #333; vertical-align: middle'>SN </th><th style='border: 1px solid  #333; vertical-align: middle'>IMEI </th><th style='border: 1px solid  #333; vertical-align: middle'>GSM </th><th style='border: 1px solid  #333; vertical-align: middle'>MAC </th><th style='border: 1px solid  #333; vertical-align: middle'>Issue Date</th><th style='border: 1px solid  #333; vertical-align: middle'>Return Date</th><th style='border: 1px solid  #333; vertical-align: middle'>Issued By</th><th style='border: 1px solid  #333; vertical-align: middle'>Notes</th></tr>";

            var detAll = DbWMS.dataItemInputDeterminant.ToList();
            //var itemModelAll = DbWMS.codeWarehouseItemModelLanguage.ToList();
            //var itemsAll = DbWMS.codeWarehouseItemLanguage.ToList();
            var inputs = DbWMS.dataItemInputDetail.AsQueryable();

            foreach (var item in myItemOutputDetails)
            {
                var CheckChildren = DbWMS.dataItemInputDetail.Where(x => x.ParentItemModelWarehouseGUID == item.ItemInputDetailGUID).ToList();

                string children = "";
                if (CheckChildren.Count > 0)
                {
                    var iteminputGUIDs = CheckChildren.Select(x => x.ItemInputDetailGUID).Distinct();
                    var itemInputDetails = DbWMS.dataItemInputDetail.Where(x => iteminputGUIDs.Contains(x.ItemInputDetailGUID)).ToList();
                    foreach (var mychildren in iteminputGUIDs)
                    {
                        var temp = itemInputDetails.Where(x => x.ItemInputDetailGUID == mychildren).FirstOrDefault();
                        children = children + "Item Name :" + temp.codeItemModelWarehouse.codeWarehouseItemModel.codeWarehouseItemModelLanguage.Where(f => f.LanguageID == LAN).FirstOrDefault().ModelDescription + "  ";
                        if (temp.dataItemInputDeterminant.Where(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.Barcode).Select(x => x.DeterminantValue).FirstOrDefault() != null)
                        {
                            children = children + "Barcode Number" + temp.dataItemInputDeterminant.Where(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.Barcode).Select(x => x.DeterminantValue).FirstOrDefault() + " , ";

                        }
                        else if (temp.dataItemInputDeterminant.Where(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.SerialNumber).Select(x => x.DeterminantValue).FirstOrDefault() != null)
                        {
                            children = children + "SerialNumber " + temp.dataItemInputDeterminant.Where(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.SerialNumber).Select(x => x.DeterminantValue).FirstOrDefault() + " , ";

                        }
                        else if (temp.dataItemInputDeterminant.Where(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.GSM).Select(x => x.DeterminantValue).FirstOrDefault() != null)
                        {
                            children = children + "GSM " + temp.dataItemInputDeterminant.Where(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.GSM).Select(x => x.DeterminantValue).FirstOrDefault() + " , ";

                        }
                        else if (temp.dataItemInputDeterminant.Where(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.IME1).Select(x => x.DeterminantValue).FirstOrDefault() != null)
                        {
                            children = children + "IME1 " + temp.dataItemInputDeterminant.Where(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.IME1).Select(x => x.DeterminantValue).FirstOrDefault() + " , ";

                        }
                        children = children + "   ";

                    }

                }


                string BarcodeNumber = detAll.Where(x => x.ItemInputDetailGUID == item.ItemInputDetailGUID
                                                         && x.codeWarehouseItemModelDeterminant.DeterminantGUID ==
                                                         ItemDeterminants.Barcode).Select(x => x.DeterminantValue)
                    .FirstOrDefault();
                if (string.IsNullOrEmpty(BarcodeNumber))
                    BarcodeNumber = "-";

                string SerialNumber = detAll
                    .Where(x => x.ItemInputDetailGUID == item.ItemInputDetailGUID
                                && x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.SerialNumber).Select(x => x.DeterminantValue)
                    .FirstOrDefault();
                if (string.IsNullOrEmpty(SerialNumber))
                    SerialNumber = "-";

                string IMEI = detAll
                    .Where(x => x.ItemInputDetailGUID == item.ItemInputDetailGUID
                                && x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.IME1).Select(x => x.DeterminantValue)
                    .FirstOrDefault();
                if (string.IsNullOrEmpty(IMEI))
                    IMEI = "-";

                string GSM = detAll
             .Where(x => x.ItemInputDetailGUID == item.ItemInputDetailGUID
                         && x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.GSM).Select(x => x.DeterminantValue)
             .FirstOrDefault();
                if (string.IsNullOrEmpty(GSM))
                    GSM = "-";


                string MAC = detAll
             .Where(x => x.ItemInputDetailGUID == item.ItemInputDetailGUID
                         && x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.MAC).Select(x => x.DeterminantValue)
             .FirstOrDefault();
                if (string.IsNullOrEmpty(MAC))
                    MAC = "-";


                string modelName = inputs.Where(x => x.ItemInputDetailGUID == item.ItemInputDetailGUID).Select(x =>
                     x.codeItemModelWarehouse.codeWarehouseItemModel.codeWarehouseItemModelLanguage.Where(f => f.LanguageID == LAN)
                         .Select(f => f.ModelDescription).FirstOrDefault()).FirstOrDefault();
                string itemName = inputs.Where(x => x.ItemInputDetailGUID == item.ItemInputDetailGUID).Select(x =>
                    x.codeItemModelWarehouse.codeWarehouseItemModel.codeWarehouseItem.codeWarehouseItemLanguage.Where(f => f.LanguageID == LAN)
                        .Select(f => f.WarehouseItemDescription).FirstOrDefault()).FirstOrDefault();

                string brandName = inputs.Where(x => x.ItemInputDetailGUID == item.ItemInputDetailGUID).Select(x =>
                    x.codeItemModelWarehouse.codeWarehouseItemModel.codeBrand.codeBrandLanguage.Where(f => f.LanguageID == LAN)
                        .Select(f => f.BrandDescription).FirstOrDefault()).FirstOrDefault();
                string expectedStartDate = "";
                string expectedReturnDate = "";
                if (item.ExpectedStartDate != null)
                    expectedStartDate = item.ExpectedStartDate.Value.ToLongDateString();
                if (item.ExpectedReturenedDate != null)
                    expectedReturnDate = item.ExpectedReturenedDate.Value.ToLongDateString();

                //string modelName = itemModelAll.Where(x => x.LanguageID == LAN &&
                //                                           x.WarehouseItemModelGUID == item.code).FirstOrDefault().ModelDescription;
                //string itemName = itemsAll.Where(x=>x.LanguageID==LAN).Select(f => f.WarehouseItemDescription).FirstOrDefault();
                table += "<tr><td style='border: 1px solid  #333; vertical-align: middle'>" + itemName + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + brandName + "</td><td style='border: 1px solid  #333; vertical-align: middle'> " + modelName + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + BarcodeNumber + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + SerialNumber + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + IMEI + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + GSM + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + MAC + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + expectedStartDate + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + expectedReturnDate + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + fullName + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + item.Comments + " " + children + "</td></tr>";
                //modelsToConfirm += "Model  :"+item.dataItemInputDetail.codeItemModelWarehouse.codeWarehouseItemModel.codeWarehouseItemModelLanguage.Where(x=>x.LanguageID==LAN).FirstOrDefault().ModelDescription
                //             +" Barcode Number/Serial Number :"+item.dataItemInputDetail.dataItemInputDeterminant.Where(x=>x.ItemInputDetailGUID==item.ItemInputDetailGUID).FirstOrDefault().DeterminantValue+"<br/>";
                //xx
                dataItemOutputNotification notification = new dataItemOutputNotification();
                notification.ItemOutputDetailGUID = item.ItemOutputDetailGUID;
                notification.NotificationMessage = "Verification Item(1)";
                notification.NotificationTypeGUID = WarehosueNotificationType.Notification;
                SendNotification(notification);

            }
            var IssuedUser = DbWMS.userPersonalDetailsLanguage.Where(x => x.UserGUID == UserGUID
                                                                          && x.LanguageID == LAN).FirstOrDefault();
            var _issuerCoreEmail = DbCMS.userServiceHistory.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
            var _profile = DbWMS.userProfiles.Where(x => x.UserProfileGUID == UserProfileGUID).FirstOrDefault();
            table += "</table>";
            if (myItemOutputDetails.Select(x => x.dataItemOutput.RequesterGUID).FirstOrDefault() == WarehouseRequestSourceTypes.Staff)
            {
                foreach (var user in users)
                {
                    if (_profile.OrganizationInstanceGUID == Guid.Parse("E156C022-EC72-4A5A-BE09-163BD85C68EF"))
                    {
                        string _message = resxEmails.ItemModelVerification
                            .Replace("$FullName", user.userAccounts.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).Select(x => x.FirstName).FirstOrDefault() + " " + user.userAccounts.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).Select(x => x.Surname).FirstOrDefault())
                            .Replace("$VerifyLink", Anchor)
                            .Replace("$Items", table)
                            .Replace("$IssuedUser", "Sent by: " + IssuedUser.FirstName + " " + IssuedUser.Surname)

                            .Replace("$VerifyStandard", LinkStandard);
                        if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                        int isRec = 1;
                        if (user.EmailAddress == "maksoud@unhcr.org")
                            isRec = 0;
                        Send(user.EmailAddress, resxEmails.ItemModelVerificationSubject, _message, isRec, _issuerCoreEmail.EmailAddress);
                    }
                    else
                    {
                        string _message = resxEmails.ItemModelVerificationGeneral
                            .Replace("$FullName", user.userAccounts.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).Select(x => x.FirstName).FirstOrDefault() + " " + user.userAccounts.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).Select(x => x.Surname).FirstOrDefault())
                            .Replace("$VerifyLink", Anchor)
                            .Replace("$Items", table)
                            .Replace("$IssuedUser", "Sent by: " + IssuedUser.FirstName + " " + IssuedUser.Surname)

                            .Replace("$VerifyStandard", LinkStandard);
                        if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                        int isRec = 1;
                        if (user.EmailAddress == "maksoud@unhcr.org")
                            isRec = 0;
                        Send(user.EmailAddress, resxEmails.ItemModelVerificationSubject, _message, isRec, _issuerCoreEmail.EmailAddress);

                    }
                }
            }
            else if (myItemOutputDetails.Select(x => x.dataItemOutput.RequesterGUID).FirstOrDefault() == WarehouseRequestSourceTypes.Warehouse)
            {
                foreach (var user in users)
                {
                    if (_profile.OrganizationInstanceGUID == Guid.Parse("E156C022-EC72-4A5A-BE09-163BD85C68EF"))
                    {

                        string _message = resxEmails.warehosueCustodianItemConfirmation
                            .Replace("$FullName", user.userAccounts.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).Select(x => x.FirstName).FirstOrDefault() + " " + user.userAccounts.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).Select(x => x.Surname).FirstOrDefault())
                            .Replace("$VerifyLink", Anchor)
                            .Replace("$Items", table)
                            .Replace("$IssuedUser", "Sent by: " + IssuedUser.FirstName + " " + IssuedUser.Surname)

                            ;
                        if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                        int isRec = 1;
                        if (user.EmailAddress == "maksoud@unhcr.org")
                            isRec = 0;
                        Send(user.EmailAddress, resxEmails.ItemModelVerificationSubject, _message, isRec, _issuerCoreEmail.EmailAddress);
                    }
                    else
                    {
                        string _message = resxEmails.warehosueCustodianItemConfirmationGeneral
                       .Replace("$FullName", user.userAccounts.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).Select(x => x.FirstName).FirstOrDefault() + " " + user.userAccounts.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).Select(x => x.Surname).FirstOrDefault())
                       .Replace("$VerifyLink", Anchor)
                       .Replace("$Items", table)
                       .Replace("$IssuedUser", "Sent by: " + IssuedUser.FirstName + " " + IssuedUser.Surname)

                       ;
                        if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                        int isRec = 1;
                        if (user.EmailAddress == "maksoud@unhcr.org")
                            isRec = 0;
                        Send(user.EmailAddress, resxEmails.ItemModelVerificationSubject, _message, isRec, _issuerCoreEmail.EmailAddress);
                    }
                }
            }

        }



        #region Confirmation Receiving Items

        public void SendItemConfirmationReceiving(List<Guid> guids)
        {
            var myItemOutputDetails = DbWMS.dataItemOutputDetail.Where(x => guids.Contains(x.ItemOutputDetailGUID)).ToList();
            var myCustGUID = myItemOutputDetails.Select(x => x.dataItemOutput.RequesterNameGUID).FirstOrDefault();
            var pendingConfirmaion = DbWMS.v_trackItemOutputFlow.Where(x => x.LastCustdianNameGUID == myCustGUID
                                                                         && x.IsLastAction == true
                                                                         && x.LastFlowTypeGUID == WarehouseRequestFlowType.PendingConfirmed)
                .ToList();


            string URL = AppSettingsKeys.Domain + "/WMS/ModelMovements/ConfirmReceivingBulkModelsByCustdoianEmail/?id=" + new Portal().GUIDToString(myCustGUID);

            string Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ConfirmItemReceiving + "</a>";


            var myItemOutputDetailGuids = myItemOutputDetails.Select(x => x.ItemOutputDetailGUID).ToList();

            var flows = DbWMS.dataItemOutputDetailFlow
                .Where(x => myItemOutputDetailGuids.Contains((Guid)x.ItemOutputDetailGUID)).ToList();
            var flowsGuids = flows.Select(x => x.ItemOutputDetailGUID).ToList();

            var fowsAll =
                myItemOutputDetails.Where(x => flowsGuids.Contains(x.ItemOutputDetailGUID)).ToList();
            //var flowsConfirmed = flows.Where(x => x.FlowTypeGUID == WarehouseRequestFlowType.Confirmed
            //                                      && x.IsLastAction == true).Select(x => x.ItemOutputDetailGUID)
            //    .ToList();
            //var flowsPendingConfirmed = flows.Where(x => x.FlowTypeGUID == WarehouseRequestFlowType.PendingConfirmed
            //                                      && x.IsLastAction == true).Select(x => x.ItemOutputDetailGUID)
            //    .ToList();

            // var  myItemOutputDetailsConfirmed =
            //     myItemOutputDetails.Where(x => flows.Contains(x.ItemOutputDetailGUID)).ToList();
            //var  myItemOutputDetailsNotConfirmed =
            //     myItemOutputDetails.Where(x => flowsPendingConfirmed.Contains(x.ItemOutputDetailGUID)).ToList();
            List<userServiceHistory> users = new List<userServiceHistory>();
            if (myItemOutputDetails.Select(x => x.dataItemOutput.RequesterGUID).FirstOrDefault() == WarehouseRequestSourceTypes.Warehouse)
            {
                var requesterNameGUID =
                    myItemOutputDetails.Select(f => f.dataItemOutput.RequesterNameGUID).FirstOrDefault();
                var warehouseFocalPointsGUIDs = DbWMS.codeWarehouseFocalPoint.Where(x => x.WarehouseGUID == requesterNameGUID
                && x.IsFocalPoint == true)
                      .Select(x => x.UserGUID).ToList();
                users = DbCMS.userServiceHistory.Where(x => warehouseFocalPointsGUIDs.Contains(x.UserGUID)).ToList();
            }

            else if (myItemOutputDetails.Select(x => x.dataItemOutput.RequesterGUID).FirstOrDefault() == WarehouseRequestSourceTypes.Staff)
            {
                Guid itemGuid = (Guid)myItemOutputDetails.Select(f => f.dataItemOutput.RequesterNameGUID).FirstOrDefault();
                users = DbCMS.userServiceHistory.Where(x => x.UserGUID == itemGuid)
                    .ToList();
            }

            string modelsToConfirm = "";

            string table = "<br><table class='table table-bordered' style='border-collapse: collapse; width: 900px'><tr><th style='border: 1px solid  #333; vertical-align:middle;'>Is Confirmed</th><th style='border: 1px solid  #333; vertical-align:middle'>Description (SC)</th><th style='border: 1px solid  #333; vertical-align: middle'>Brand</th><th style='border: 1px solid  #333; vertical-align: middle'>Item</th><th style='border: 1px solid  #333; vertical-align: middle'>Barcode</th><th style='border: 1px solid  #333; vertical-align: middle'>SN</th><th style='border: 1px solid  #333; vertical-align: middle'>IMEI </th><th style='border: 1px solid  #333; vertical-align: middle'>GSM </th><th style='border: 1px solid  #333; vertical-align: middle'>MAC </th><th style='border: 1px solid  #333; vertical-align: middle'>Issue Date</th><th style='border: 1px solid  #333; vertical-align: middle'>Return Date</th><th style='border: 1px solid  #333; vertical-align: middle'>Issued By</th><th style='border: 1px solid  #333; vertical-align: middle'>Notes</th></tr>";
            //string NotConfirmedtable = "<br><table class='table table-bordered' style='border-collapse: collapse; width: 900px'><tr><th style='border: 1px solid  #333; vertical-align:middle'>Confirmed</th><th style='border: 1px solid  #333; vertical-align:middle'>Barcode</th><th style='border: 1px solid  #333; vertical-align: middle'>SN</th><th style='border: 1px solid  #333; vertical-align: middle'>IMEI</th><th style='border: 1px solid  #333; vertical-align: middle'>Model</th><th style='border: 1px solid  #333; vertical-align: middle'>Desc</th><th style='border: 1px solid  #333; vertical-align: middle'>Issue Date</th><th style='border: 1px solid  #333; vertical-align: middle'>Return Date</th></tr>";
            var detAll = DbWMS.dataItemInputDeterminant.ToList();
            //var itemModelAll = DbWMS.codeWarehouseItemModelLanguage.ToList();
            //var itemsAll = DbWMS.codeWarehouseItemLanguage.ToList();
            var inputs = DbWMS.dataItemInputDetail.AsQueryable();



            foreach (var item in fowsAll)
            {

                string BarcodeNumber = detAll.Where(x => x.ItemInputDetailGUID == item.ItemInputDetailGUID
                                                         && x.codeWarehouseItemModelDeterminant.DeterminantGUID ==
                                                         ItemDeterminants.Barcode).Select(x => x.DeterminantValue)
                    .FirstOrDefault();
                if (string.IsNullOrEmpty(BarcodeNumber))
                    BarcodeNumber = "-";

                string SerialNumber = detAll
                    .Where(x => x.ItemInputDetailGUID == item.ItemInputDetailGUID
                                && x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.SerialNumber).Select(x => x.DeterminantValue)
                    .FirstOrDefault();
                if (string.IsNullOrEmpty(SerialNumber))
                    SerialNumber = "-";

                string IMEI = detAll
                    .Where(x => x.ItemInputDetailGUID == item.ItemInputDetailGUID
                                && x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.IME1).Select(x => x.DeterminantValue)
                    .FirstOrDefault();
                if (string.IsNullOrEmpty(IMEI))
                    IMEI = "-";

                string GSM = detAll
                    .Where(x => x.ItemInputDetailGUID == item.ItemInputDetailGUID
                                && x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.GSM).Select(x => x.DeterminantValue)
                    .FirstOrDefault();
                if (string.IsNullOrEmpty(GSM))
                    GSM = "-";
                string MAC = detAll
                    .Where(x => x.ItemInputDetailGUID == item.ItemInputDetailGUID
                                && x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.MAC).Select(x => x.DeterminantValue)
                    .FirstOrDefault();
                if (string.IsNullOrEmpty(MAC))
                    MAC = "-";

                string modelName = inputs.Where(x => x.ItemInputDetailGUID == item.ItemInputDetailGUID).Select(x =>
                     x.codeItemModelWarehouse.codeWarehouseItemModel.codeWarehouseItemModelLanguage.Where(f => f.LanguageID == LAN)
                         .Select(f => f.ModelDescription).FirstOrDefault()).FirstOrDefault();
                string itemName = inputs.Where(x => x.ItemInputDetailGUID == item.ItemInputDetailGUID).Select(x =>
                    x.codeItemModelWarehouse.codeWarehouseItemModel.codeWarehouseItem.codeWarehouseItemLanguage.Where(f => f.LanguageID == LAN)
                        .Select(f => f.WarehouseItemDescription).FirstOrDefault()).FirstOrDefault();

                string brandName = inputs.Where(x => x.ItemInputDetailGUID == item.ItemInputDetailGUID).Select(x =>
                    x.codeItemModelWarehouse.codeWarehouseItemModel.codeBrand.codeBrandLanguage.Where(f => f.LanguageID == LAN)
                        .Select(f => f.BrandDescription).FirstOrDefault()).FirstOrDefault();
                string expectedStartDate = "";
                string expectedReturnDate = "";
                if (item.ExpectedStartDate != null)
                    expectedStartDate = item.ExpectedStartDate.Value.ToLongDateString();
                if (item.ExpectedReturenedDate != null)
                    expectedReturnDate = item.ExpectedReturenedDate.Value.ToLongDateString();
                string isConfirmed = "Yes";
                var flowTypeGUID = flows.Where(x => x.IsLastAction == true
                                                    && x.ItemOutputDetailGUID == item.ItemOutputDetailGUID).Select(x => x.FlowTypeGUID)
                    .FirstOrDefault();
                if (flowTypeGUID == WarehouseRequestFlowType.PendingConfirmed)
                {
                    isConfirmed = "No";
                }

                //string modelName = itemModelAll.Where(x => x.LanguageID == LAN &&
                //                                           x.WarehouseItemModelGUID == item.code).FirstOrDefault().ModelDescription;
                //string itemName = itemsAll.Where(x=>x.LanguageID==LAN).Select(f => f.WarehouseItemDescription).FirstOrDefault();
                var personal = DbWMS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN
                                                                            && x.UserGUID == item.CreatedByGUID).FirstOrDefault();
                string fullName = personal.FirstName + " " + personal.Surname;

                table += "<tr><td style='border: 1px solid  #333; vertical-align: middle'>" + isConfirmed + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + itemName + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + brandName + "</td><td style='border: 1px solid  #333; vertical-align: middle'> " + modelName + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + BarcodeNumber + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + SerialNumber + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + IMEI + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + GSM + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + MAC + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + expectedStartDate + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + expectedReturnDate + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + fullName + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + item.Comments + "</td></tr>";


                //modelsToConfirm += "Model  :"+item.dataItemInputDetail.codeItemModelWarehouse.codeWarehouseItemModel.codeWarehouseItemModelLanguage.Where(x=>x.LanguageID==LAN).FirstOrDefault().ModelDescription
                //             +" Barcode Number/Serial Number :"+item.dataItemInputDetail.dataItemInputDeterminant.Where(x=>x.ItemInputDetailGUID==item.ItemInputDetailGUID).FirstOrDefault().DeterminantValue+"<br/>";


            }
            ///zzzmyCustGUID;

            foreach (var item in pendingConfirmaion)
            {

                string BarcodeNumber = item.BarcodeNumber;
                if (string.IsNullOrEmpty(BarcodeNumber))
                    BarcodeNumber = "-";

                string SerialNumber = item.SerialNumber;
                if (string.IsNullOrEmpty(SerialNumber))
                    SerialNumber = "-";

                string IMEI = item.IMEI1;
                if (string.IsNullOrEmpty(IMEI))
                    IMEI = "-";
                string GSM = item.GSM;
                if (string.IsNullOrEmpty(GSM))
                    GSM = "-";
                string MAC = item.MAC;
                if (string.IsNullOrEmpty(MAC))
                    MAC = "-";

                string itemName = inputs.Where(x => x.ItemInputDetailGUID == item.ItemInputDetailGUID).Select(x =>
                    x.codeItemModelWarehouse.codeWarehouseItemModel.codeWarehouseItem.codeWarehouseItemLanguage.Where(f => f.LanguageID == LAN)
                        .Select(f => f.WarehouseItemDescription).FirstOrDefault()).FirstOrDefault();

                string brandName = inputs.Where(x => x.ItemInputDetailGUID == item.ItemInputDetailGUID).Select(x =>
                    x.codeItemModelWarehouse.codeWarehouseItemModel.codeBrand.codeBrandLanguage.Where(f => f.LanguageID == LAN)
                        .Select(f => f.BrandDescription).FirstOrDefault()).FirstOrDefault();

                string modelName = item.ModelDescription;

                string expectedStartDate = "";
                string expectedReturnDate = "";
                if (item.ExpectedStartDate != null)
                    expectedStartDate = item.ExpectedStartDate.Value.ToLongDateString();
                if (item.ExpectedReturenedDate != null)
                    expectedReturnDate = item.ExpectedReturenedDate.Value.ToLongDateString();
                table += "<tr><td style='border: 1px solid  #333; vertical-align: middle'>No</td><td style='border: 1px solid  #333; vertical-align: middle'>" + itemName + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + brandName + "</td><td style='border: 1px solid  #333; vertical-align: middle'> " + modelName + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + BarcodeNumber + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + SerialNumber + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + IMEI + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + GSM + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + MAC + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + expectedStartDate + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + expectedReturnDate + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + item.FlowCreatedByName + "</td></tr>";

            }


            table += "</table>";
            if (pendingConfirmaion.Count == 0)
                Anchor = "";
            var _profile = DbWMS.userProfiles.Where(x => x.UserProfileGUID == UserProfileGUID).FirstOrDefault();
            foreach (var user in users)
            {
                if (_profile.OrganizationInstanceGUID == Guid.Parse("E156C022-EC72-4A5A-BE09-163BD85C68EF"))
                {


                    string _message = resxEmails.ItemConfirmationReceiving
                        .Replace("$FullName", user.userAccounts.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).Select(x => x.FirstName).FirstOrDefault() + " " + user.userAccounts.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).Select(x => x.Surname).FirstOrDefault())

                        .Replace("$Items", table)
                        .Replace("$ConfirmPendingDeliveryItemsLink", Anchor)
                        .Replace("$ItemNotConfirmed", "");
                    if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                    int isRec = 1;
                    if (user.EmailAddress == "maksoud@unhcr.org")
                        isRec = 0;
                    Send(user.EmailAddress, resxEmails.WarehouseConfirmReceivingItemsSubject, _message, isRec, "");
                }

                else
                {

                    string _message = resxEmails.ItemConfirmationReceivingGeneral
                        .Replace("$FullName", user.userAccounts.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).Select(x => x.FirstName).FirstOrDefault() + " " + user.userAccounts.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).Select(x => x.Surname).FirstOrDefault())

                        .Replace("$Items", table)
                        .Replace("$ConfirmPendingDeliveryItemsLink", Anchor)
                        .Replace("$ItemNotConfirmed", "");
                    if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                    int isRec = 1;
                    if (user.EmailAddress == "maksoud@unhcr.org")
                        isRec = 0;
                    Send(user.EmailAddress, resxEmails.WarehouseConfirmReceivingItemsSubject, _message, isRec, "");
                }
            }
        }

        public void SendEmailToConfirmReturnItemToStock(Guid myGuid)
        {


            var myItemOutputDetails = DbWMS.dataItemOutputDetail.Where(x => x.ItemOutputDetailGUID == myGuid).ToList();
            List<userServiceHistory> users = new List<userServiceHistory>();
            if (myItemOutputDetails.Select(x => x.dataItemOutput.RequesterGUID).FirstOrDefault() == WarehouseRequestSourceTypes.Warehouse)
            {
                var requesterNameGUID =
                    myItemOutputDetails.Select(f => f.dataItemOutput.RequesterNameGUID).FirstOrDefault();
                var warehouseFocalPointsGUIDs = DbWMS.codeWarehouseFocalPoint.Where(x => x.WarehouseGUID == requesterNameGUID
                && x.IsFocalPoint == true)
                      .Select(x => x.UserGUID).ToList();
                users = DbCMS.userServiceHistory.Where(x => warehouseFocalPointsGUIDs.Contains(x.UserGUID)).ToList();
            }

            else if (myItemOutputDetails.Select(x => x.dataItemOutput.RequesterGUID).FirstOrDefault() == WarehouseRequestSourceTypes.Staff)
            {
                Guid itemGuid = (Guid)myItemOutputDetails.Select(f => f.dataItemOutput.RequesterNameGUID).FirstOrDefault();
                users = DbCMS.userServiceHistory.Where(x => x.UserGUID == itemGuid)
                    .ToList();
            }

            string modelsToConfirm = "";
            string table =
               "<br><table class='table table-bordered' style='border-collapse: collapse; width: 900px'><tr><th style='border: 1px solid  #333; vertical-align:middle'>Description (SC)</th><th style='border: 1px solid  #333; vertical-align: middle'>Brand</th><th style='border: 1px solid  #333; vertical-align: middle'>Item</th><th style='border: 1px solid  #333; vertical-align: middle'>Barcode</th><th style='border: 1px solid  #333; vertical-align: middle'>SN </th><th style='border: 1px solid  #333; vertical-align: middle'>IMEI </th><th style='border: 1px solid  #333; vertical-align: middle'>GSM </th><th style='border: 1px solid  #333; vertical-align: middle'>MAC </th><th style='border: 1px solid  #333; vertical-align: middle'>Issue Date</th><th style='border: 1px solid  #333; vertical-align: middle'>Return Date</th><th style='border: 1px solid  #333; vertical-align: middle'>Issued By</th><th style='border: 1px solid  #333; vertical-align: middle'>Notes</th></tr>";

            //string table = "<br><table class='table table-bordered' style='border-collapse: collapse; width: 900px'><tr><th style='border: 1px solid  #333; vertical-align:middle'>Barcode</th><th style='border: 1px solid  #333; vertical-align: middle'>SN</th><th style='border: 1px solid  #333; vertical-align: middle'>IMEI</th><th style='border: 1px solid  #333; vertical-align: middle'>Model</th><th style='border: 1px solid  #333; vertical-align: middle'>Desc</th><th style='border: 1px solid  #333; vertical-align: middle'>Issue Date</th><th style='border: 1px solid  #333; vertical-align: middle'>Return Date</th></tr>";
            string NotConfirmedtable = "<br><table class='table table-bordered' style='border-collapse: collapse; width: 900px'><tr><th style='border: 1px solid  #333; vertical-align:middle'>Barcode</th><th style='border: 1px solid  #333; vertical-align: middle'>SN</th><th style='border: 1px solid  #333; vertical-align: middle'>IMEI</th><th style='border: 1px solid  #333; vertical-align: middle'>Model</th><th style='border: 1px solid  #333; vertical-align: middle'>Desc</th><th style='border: 1px solid  #333; vertical-align: middle'>Issue Date</th><th style='border: 1px solid  #333; vertical-align: middle'>Return Date</th></tr>";
            var detAll = DbWMS.dataItemInputDeterminant.ToList();
            //var itemModelAll = DbWMS.codeWarehouseItemModelLanguage.ToList();
            //var itemsAll = DbWMS.codeWarehouseItemLanguage.ToList();
            var inputs = DbWMS.dataItemInputDetail.AsQueryable();

            foreach (var item in myItemOutputDetails)
            {

                string BarcodeNumber = detAll.Where(x => x.ItemInputDetailGUID == item.ItemInputDetailGUID
                                                         && x.codeWarehouseItemModelDeterminant.DeterminantGUID ==
                                                         ItemDeterminants.Barcode).Select(x => x.DeterminantValue)
                    .FirstOrDefault();
                if (string.IsNullOrEmpty(BarcodeNumber))
                    BarcodeNumber = "-";

                var personal = DbWMS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN
                                                                            && x.UserGUID == item.CreatedByGUID).FirstOrDefault();
                string fullName = personal.FirstName + " " + personal.Surname;

                string SerialNumber = detAll
                    .Where(x => x.ItemInputDetailGUID == item.ItemInputDetailGUID
                                && x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.SerialNumber).Select(x => x.DeterminantValue)
                    .FirstOrDefault();
                if (string.IsNullOrEmpty(SerialNumber))
                    SerialNumber = "-";

                string brandName = inputs.Where(x => x.ItemInputDetailGUID == item.ItemInputDetailGUID).Select(x =>
                    x.codeItemModelWarehouse.codeWarehouseItemModel.codeBrand.codeBrandLanguage.Where(f => f.LanguageID == LAN)
                        .Select(f => f.BrandDescription).FirstOrDefault()).FirstOrDefault();

                string IMEI = detAll
                    .Where(x => x.ItemInputDetailGUID == item.ItemInputDetailGUID
                                && x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.IME1).Select(x => x.DeterminantValue)
                    .FirstOrDefault();
                if (string.IsNullOrEmpty(IMEI))
                    IMEI = "-";
                string GSM = detAll
             .Where(x => x.ItemInputDetailGUID == item.ItemInputDetailGUID
                         && x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.GSM).Select(x => x.DeterminantValue)
             .FirstOrDefault();
                if (string.IsNullOrEmpty(GSM))
                    GSM = "-";
                string MAC = detAll
                    .Where(x => x.ItemInputDetailGUID == item.ItemInputDetailGUID
                                && x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.MAC).Select(x => x.DeterminantValue)
                    .FirstOrDefault();
                if (string.IsNullOrEmpty(MAC))
                    MAC = "-";

                string modelName = inputs.Where(x => x.ItemInputDetailGUID == item.ItemInputDetailGUID).Select(x =>
                     x.codeItemModelWarehouse.codeWarehouseItemModel.codeWarehouseItemModelLanguage.Where(f => f.LanguageID == LAN)
                         .Select(f => f.ModelDescription).FirstOrDefault()).FirstOrDefault();
                string itemName = inputs.Where(x => x.ItemInputDetailGUID == item.ItemInputDetailGUID).Select(x =>
                    x.codeItemModelWarehouse.codeWarehouseItemModel.codeWarehouseItem.codeWarehouseItemLanguage.Where(f => f.LanguageID == LAN)
                        .Select(f => f.WarehouseItemDescription).FirstOrDefault()).FirstOrDefault();
                string expectedStartDate = "";
                string actualReturn = "";
                if (item.ExpectedStartDate != null)
                    expectedStartDate = item.ExpectedStartDate.Value.ToLongDateString();
                if (item.ReturnedDate != null)
                    actualReturn = item.ReturnedDate.Value.ToLongDateString();
                else
                    actualReturn = DateTime.Now.ToLongTimeString();


                string Comments = item.Comments;
                //string modelName = itemModelAll.Where(x => x.LanguageID == LAN &&
                //                                           x.WarehouseItemModelGUID == item.code).FirstOrDefault().ModelDescription;
                //string itemName = itemsAll.Where(x=>x.LanguageID==LAN).Select(f => f.WarehouseItemDescription).FirstOrDefault();

                table += "<tr><td style='border: 1px solid  #333; vertical-align: middle'>" + itemName + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + brandName + "</td><td style='border: 1px solid  #333; vertical-align: middle'> " + modelName + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + BarcodeNumber + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + SerialNumber + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + IMEI + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + GSM + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + MAC + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + expectedStartDate + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + actualReturn + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + fullName + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + Comments + "</td></tr>";


                //modelsToConfirm += "Model  :"+item.dataItemInputDetail.codeItemModelWarehouse.codeWarehouseItemModel.codeWarehouseItemModelLanguage.Where(x=>x.LanguageID==LAN).FirstOrDefault().ModelDescription
                //             +" Barcode Number/Serial Number :"+item.dataItemInputDetail.dataItemInputDeterminant.Where(x=>x.ItemInputDetailGUID==item.ItemInputDetailGUID).FirstOrDefault().DeterminantValue+"<br/>";


            }



            table += "</table>";
            var IssuedUser = DbWMS.userPersonalDetailsLanguage.Where(x => x.UserGUID == UserGUID
                                                                            && x.LanguageID == LAN).FirstOrDefault();
            var _issuerCoreEmail = DbCMS.userServiceHistory.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
            var _profile = DbWMS.userProfiles.Where(x => x.UserProfileGUID == UserProfileGUID).FirstOrDefault();
            foreach (var user in users)
            {
                if (_profile.OrganizationInstanceGUID == Guid.Parse("E156C022-EC72-4A5A-BE09-163BD85C68EF"))
                {
                    string _message = resxEmails.ConfirmReturnItemToStock
                    .Replace("$FullName", user.userAccounts.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).Select(x => x.FirstName).FirstOrDefault() + " " + user.userAccounts.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).Select(x => x.Surname).FirstOrDefault())
                .Replace("$IssuedUser", "Sent by: " + IssuedUser.FirstName + " " + IssuedUser.Surname)
                    .Replace("$Items", table);
                    if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                    int isRec = 1;
                    if (user.EmailAddress == "maksoud@unhcr.org")
                        isRec = 0;
                    Send(user.EmailAddress, resxEmails.ConfirmReturnItemToStockSubject, _message, isRec, _issuerCoreEmail.EmailAddress);
                }
                else
                {

                    string _message = resxEmails.ConfirmReturnItemToStockGeneral
                  .Replace("$FullName", user.userAccounts.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).Select(x => x.FirstName).FirstOrDefault() + " " + user.userAccounts.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).Select(x => x.Surname).FirstOrDefault())
              .Replace("$IssuedUser", "Sent by: " + IssuedUser.FirstName + " " + IssuedUser.Surname)
                  .Replace("$Items", table);
                    if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                    int isRec = 1;
                    if (user.EmailAddress == "maksoud@unhcr.org")
                        isRec = 0;
                    Send(user.EmailAddress, resxEmails.ConfirmReturnItemToStockSubject, _message, isRec, _issuerCoreEmail.EmailAddress);
                }
            }
        }





        #endregion

        #region send mail for Reteriv bulk
        public void SendConfirmReteriveBulkItems(List<Guid?> guids)
        {

            var allmyItemOutputDetails = DbWMS.dataItemOutputDetail.Where(x => guids.Contains(x.ItemOutputDetailGUID)).ToList();

            var requesterguids = allmyItemOutputDetails.Select(x => x.dataItemOutput.RequesterGUID).Distinct().ToList();
            foreach (var requester in requesterguids)
            {
                var myouputDet = allmyItemOutputDetails.Where(x => x.dataItemOutput.RequesterGUID == requester).ToList();
                List<userServiceHistory> users = new List<userServiceHistory>();
                if (requester == WarehouseRequestSourceTypes.Warehouse)
                {
                    var requesterNameGUID =
                        myouputDet.Select(f => f.dataItemOutput.RequesterNameGUID).FirstOrDefault();
                    var warehouseFocalPointsGUIDs = DbWMS.codeWarehouseFocalPoint.Where(x => x.WarehouseGUID == requesterNameGUID
                    && x.IsFocalPoint == true)
                          .Select(x => x.UserGUID).ToList();
                    users = DbCMS.userServiceHistory.Where(x => warehouseFocalPointsGUIDs.Contains(x.UserGUID)).ToList();
                }

                else if (requester == WarehouseRequestSourceTypes.Staff)
                {
                    Guid itemGuid = (Guid)myouputDet.Select(f => f.dataItemOutput.RequesterNameGUID).FirstOrDefault();
                    users = DbCMS.userServiceHistory.Where(x => x.UserGUID == itemGuid)
                        .ToList();
                }

                string modelsToConfirm = "";
                string table =
                    "<br><table class='table table-bordered' style='border-collapse: collapse; width: 900px'><tr><th style='border: 1px solid  #333; vertical-align:middle'>Description (SC)</th><th style='border: 1px solid  #333; vertical-align: middle'>Brand</th><th style='border: 1px solid  #333; vertical-align: middle'>Item</th><th style='border: 1px solid  #333; vertical-align: middle'>Barcode</th><th style='border: 1px solid  #333; vertical-align: middle'>SN </th><th style='border: 1px solid  #333; vertical-align: middle'>IMEI </th><th style='border: 1px solid  #333; vertical-align: middle'>GSM </th><th style='border: 1px solid  #333; vertical-align: middle'>MAC </th><th style='border: 1px solid  #333; vertical-align: middle'>Issue Date</th><th style='border: 1px solid  #333; vertical-align: middle'>Return Date</th><th style='border: 1px solid  #333; vertical-align: middle'>Issued By</th><th style='border: 1px solid  #333; vertical-align: middle'>Comments</th></tr>";


                var detAll = DbWMS.dataItemInputDeterminant.ToList();
                //var itemModelAll = DbWMS.codeWarehouseItemModelLanguage.ToList();
                //var itemsAll = DbWMS.codeWarehouseItemLanguage.ToList();
                var inputs = DbWMS.dataItemInputDetail.AsQueryable();

                foreach (var item in myouputDet)
                {

                    string BarcodeNumber = detAll.Where(x => x.ItemInputDetailGUID == item.ItemInputDetailGUID
                                                             && x.codeWarehouseItemModelDeterminant.DeterminantGUID ==
                                                             ItemDeterminants.Barcode).Select(x => x.DeterminantValue)
                        .FirstOrDefault();
                    if (string.IsNullOrEmpty(BarcodeNumber))
                        BarcodeNumber = "-";

                    string SerialNumber = detAll
                        .Where(x => x.ItemInputDetailGUID == item.ItemInputDetailGUID
                                    && x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.SerialNumber).Select(x => x.DeterminantValue)
                        .FirstOrDefault();
                    if (string.IsNullOrEmpty(SerialNumber))
                        SerialNumber = "-";

                    string IMEI = detAll
                        .Where(x => x.ItemInputDetailGUID == item.ItemInputDetailGUID
                                    && x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.IME1).Select(x => x.DeterminantValue)
                        .FirstOrDefault();
                    if (string.IsNullOrEmpty(IMEI))
                        IMEI = "-";
                    string GSM = detAll
             .Where(x => x.ItemInputDetailGUID == item.ItemInputDetailGUID
                         && x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.GSM).Select(x => x.DeterminantValue)
             .FirstOrDefault();
                    if (string.IsNullOrEmpty(GSM))
                        GSM = "-";
                    string MAC = detAll
                        .Where(x => x.ItemInputDetailGUID == item.ItemInputDetailGUID
                                    && x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.MAC).Select(x => x.DeterminantValue)
                        .FirstOrDefault();
                    if (string.IsNullOrEmpty(MAC))
                        MAC = "-";
                    string modelName = inputs.Where(x => x.ItemInputDetailGUID == item.ItemInputDetailGUID).Select(x =>
                         x.codeItemModelWarehouse.codeWarehouseItemModel.codeWarehouseItemModelLanguage.Where(f => f.LanguageID == LAN)
                             .Select(f => f.ModelDescription).FirstOrDefault()).FirstOrDefault();
                    string itemName = inputs.Where(x => x.ItemInputDetailGUID == item.ItemInputDetailGUID).Select(x =>
                        x.codeItemModelWarehouse.codeWarehouseItemModel.codeWarehouseItem.codeWarehouseItemLanguage.Where(f => f.LanguageID == LAN)
                            .Select(f => f.WarehouseItemDescription).FirstOrDefault()).FirstOrDefault();

                    string brandName = inputs.Where(x => x.ItemInputDetailGUID == item.ItemInputDetailGUID).Select(x =>
                        x.codeItemModelWarehouse.codeWarehouseItemModel.codeBrand.codeBrandLanguage.Where(f => f.LanguageID == LAN)
                            .Select(f => f.BrandDescription).FirstOrDefault()).FirstOrDefault();


                    var personal = DbWMS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN
                                                                                && x.UserGUID == item.CreatedByGUID).FirstOrDefault();
                    string fullName = personal.FirstName + " " + personal.Surname;
                    string expectedStartDate = "";
                    string expectedReturnDate = "";
                    if (item.ExpectedStartDate != null)
                        expectedStartDate = item.ExpectedStartDate.Value.ToLongDateString();
                    if (item.ExpectedReturenedDate != null)
                        expectedReturnDate = item.ExpectedReturenedDate.Value.ToLongDateString();
                    //string modelName = itemModelAll.Where(x => x.LanguageID == LAN &&
                    //                                           x.WarehouseItemModelGUID == item.code).FirstOrDefault().ModelDescription;
                    //string itemName = itemsAll.Where(x=>x.LanguageID==LAN).Select(f => f.WarehouseItemDescription).FirstOrDefault();

                    table += "<tr><td style='border: 1px solid  #333; vertical-align: middle'>" + itemName + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + brandName + "</td><td style='border: 1px solid  #333; vertical-align: middle'> " + modelName + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + BarcodeNumber + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + SerialNumber + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + IMEI + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + GSM + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + MAC + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + expectedStartDate + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + expectedReturnDate + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + fullName + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + item.Comments + "</td></tr>";


                    //modelsToConfirm += "Model  :"+item.dataItemInputDetail.codeItemModelWarehouse.codeWarehouseItemModel.codeWarehouseItemModelLanguage.Where(x=>x.LanguageID==LAN).FirstOrDefault().ModelDescription
                    //             +" Barcode Number/Serial Number :"+item.dataItemInputDetail.dataItemInputDeterminant.Where(x=>x.ItemInputDetailGUID==item.ItemInputDetailGUID).FirstOrDefault().DeterminantValue+"<br/>";


                }

                table += "</table>";
                var IssuedUser = DbWMS.userPersonalDetailsLanguage.Where(x => x.UserGUID == UserGUID
                                                                              && x.LanguageID == LAN).FirstOrDefault();

                var _issuerCoreEmail = DbCMS.userServiceHistory.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
                var _profile = DbWMS.userProfiles.Where(x => x.UserProfileGUID == UserProfileGUID).FirstOrDefault();
                foreach (var user in users)
                {
                    if (_profile.OrganizationInstanceGUID == Guid.Parse("E156C022-EC72-4A5A-BE09-163BD85C68EF"))
                    {
                        string _message = resxEmails.ConfirmReturnItemToStock
                        .Replace("$FullName", user.userAccounts.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).Select(x => x.FirstName).FirstOrDefault() + " " + user.userAccounts.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).Select(x => x.Surname).FirstOrDefault())
                        .Replace("$IssuedUser", "Sent by: " + IssuedUser.FirstName + " " + IssuedUser.Surname)
                        .Replace("$Items", table);
                        if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                        int isRec = 1;
                        if (user.EmailAddress == "maksoud@unhcr.org")
                            isRec = 0;
                        Send(user.EmailAddress, resxEmails.ConfirmReturnItemToStockSubject, _message, isRec, _issuerCoreEmail.EmailAddress);
                    }
                    else
                    {
                        string _message = resxEmails.ConfirmReturnItemToStockGeneral
                  .Replace("$FullName", user.userAccounts.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).Select(x => x.FirstName).FirstOrDefault() + " " + user.userAccounts.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).Select(x => x.Surname).FirstOrDefault())
                  .Replace("$IssuedUser", "Sent by: " + IssuedUser.FirstName + " " + IssuedUser.Surname)
                  .Replace("$Items", table);
                        if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                        int isRec = 1;
                        if (user.EmailAddress == "maksoud@unhcr.org")
                            isRec = 0;
                        Send(user.EmailAddress, resxEmails.ConfirmReturnItemToStockSubject, _message, isRec, _issuerCoreEmail.EmailAddress);
                    }
                }

            }


        }
        #endregion

        #region Email Pedning Confirmation 

        public void SendEmailReminderForPendingConfirmationBulkItems(Guid myGuid)
        {
            var flowsOutputGuids = DbWMS.dataItemOutputDetailFlow.Where(x =>
                    x.dataItemOutputDetail.dataItemInputDetail.LastCustdianNameGUID == myGuid
                    && x.IsLastAction == true
                    && x.FlowTypeGUID == WarehouseRequestFlowType.PendingConfirmed)
                .Select(x => x.dataItemOutputDetail.ItemOutputDetailGUID).ToList();

            var myItemOutputDetails = DbWMS.dataItemOutputDetail.Where(x => flowsOutputGuids.Contains(x.ItemOutputDetailGUID)
                                                                            && x.dataItemInputDetail.LastFlowTypeGUID == WarehouseRequestFlowType.PendingConfirmed).ToList();

            string URL = AppSettingsKeys.Domain + "/WMS/ModelMovements/ConfirmReceivingBulkModelsByCustdoianEmail/?id=" + myGuid.ToString();
            string standardURL = AppSettingsKeys.Domain + "/WMS/ModelMovements/GetICTSOPFile/";
            string LinkStandard = "<a href='" + standardURL + "' target='_blank'>" + "Syria ICT equipment SOP" + "</a>";
            string Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ConfirmItemReceiving + "</a>";
            string Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";

            List<userServiceHistory> users = new List<userServiceHistory>();
            if (myItemOutputDetails.Select(x => x.dataItemOutput.RequesterGUID).FirstOrDefault() == WarehouseRequestSourceTypes.Warehouse)
            {
                var requesterNameGUID =
                    myItemOutputDetails.Select(f => f.dataItemOutput.RequesterNameGUID).FirstOrDefault();
                var warehouseFocalPointsGUIDs = DbWMS.codeWarehouseFocalPoint.Where(x => x.WarehouseGUID == requesterNameGUID
                && x.IsFocalPoint == true)
                      .Select(x => x.UserGUID).ToList();
                users = DbCMS.userServiceHistory.Where(x => warehouseFocalPointsGUIDs.Contains(x.UserGUID)).ToList();
            }

            else if (myItemOutputDetails.Select(x => x.dataItemOutput.RequesterGUID).FirstOrDefault() == WarehouseRequestSourceTypes.Staff)
            {
                Guid itemGuid = (Guid)myItemOutputDetails.Select(f => f.dataItemOutput.RequesterNameGUID).FirstOrDefault();
                users = DbCMS.userServiceHistory.Where(x => x.UserGUID == itemGuid)
                    .ToList();
            }
            string modelsToConfirm = "";
            string table =
                "<br><table class='table table-bordered' style='border-collapse: collapse; width: 900px'><tr><th style='border: 1px solid  #333; vertical-align:middle'>Description (SC)</th><th style='border: 1px solid  #333; vertical-align: middle'>Brand</th><th style='border: 1px solid  #333; vertical-align: middle'>Item</th><th style='border: 1px solid  #333; vertical-align: middle'>Barcode</th><th style='border: 1px solid  #333; vertical-align: middle'>SN </th><th style='border: 1px solid  #333; vertical-align: middle'>IMEI </th><th style='border: 1px solid  #333; vertical-align: middle'>GSM </th><th style='border: 1px solid  #333; vertical-align: middle'>MAC </th><th style='border: 1px solid  #333; vertical-align: middle'>Issue Date</th><th style='border: 1px solid  #333; vertical-align: middle'>Return Date</th><th style='border: 1px solid  #333; vertical-align: middle'>Issued By</th><th style='border: 1px solid  #333; vertical-align: middle'>Notes</th></tr>";
            var detAll = DbWMS.dataItemInputDeterminant.ToList();
            //var itemModelAll = DbWMS.codeWarehouseItemModelLanguage.ToList();
            //var itemsAll = DbWMS.codeWarehouseItemLanguage.ToList();
            var inputs = DbWMS.dataItemInputDetail.AsQueryable();
            foreach (var item in myItemOutputDetails)
            {
                var personal = DbWMS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN
                                                                            && x.UserGUID == item.CreatedByGUID).FirstOrDefault();
                string fullName = personal.FirstName + " " + personal.Surname;

                string BarcodeNumber = detAll.Where(x => x.ItemInputDetailGUID == item.ItemInputDetailGUID
                                                         && x.codeWarehouseItemModelDeterminant.DeterminantGUID ==
                                                         ItemDeterminants.Barcode).Select(x => x.DeterminantValue).FirstOrDefault();
                if (string.IsNullOrEmpty(BarcodeNumber))
                    BarcodeNumber = "-";
                string SerialNumber = detAll
                    .Where(x => x.ItemInputDetailGUID == item.ItemInputDetailGUID
                                && x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.SerialNumber).Select(x => x.DeterminantValue)
                    .FirstOrDefault();
                if (string.IsNullOrEmpty(SerialNumber))
                    SerialNumber = "-";
                string IMEI = detAll
                    .Where(x => x.ItemInputDetailGUID == item.ItemInputDetailGUID
                                && x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.IME1).Select(x => x.DeterminantValue)
                    .FirstOrDefault();
                if (string.IsNullOrEmpty(IMEI))
                    IMEI = "-";

                string GSM = detAll
             .Where(x => x.ItemInputDetailGUID == item.ItemInputDetailGUID
                         && x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.GSM).Select(x => x.DeterminantValue)
             .FirstOrDefault();
                if (string.IsNullOrEmpty(GSM))
                    GSM = "-";
                string MAC = detAll
                    .Where(x => x.ItemInputDetailGUID == item.ItemInputDetailGUID
                                && x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.MAC).Select(x => x.DeterminantValue)
                    .FirstOrDefault();
                if (string.IsNullOrEmpty(MAC))
                    MAC = "-";

                string modelName = inputs.Where(x => x.ItemInputDetailGUID == item.ItemInputDetailGUID).Select(x =>
                     x.codeItemModelWarehouse.codeWarehouseItemModel.codeWarehouseItemModelLanguage.Where(f => f.LanguageID == LAN)
                         .Select(f => f.ModelDescription).FirstOrDefault()).FirstOrDefault();
                string itemName = inputs.Where(x => x.ItemInputDetailGUID == item.ItemInputDetailGUID).Select(x =>
                    x.codeItemModelWarehouse.codeWarehouseItemModel.codeWarehouseItem.codeWarehouseItemLanguage.Where(f => f.LanguageID == LAN)
                        .Select(f => f.WarehouseItemDescription).FirstOrDefault()).FirstOrDefault();

                string brandName = inputs.Where(x => x.ItemInputDetailGUID == item.ItemInputDetailGUID).Select(x =>
                    x.codeItemModelWarehouse.codeWarehouseItemModel.codeBrand.codeBrandLanguage.Where(f => f.LanguageID == LAN)
                        .Select(f => f.BrandDescription).FirstOrDefault()).FirstOrDefault();
                string expectedStartDate = "";
                string expectedReturnDate = "";
                if (item.ExpectedStartDate != null)
                    expectedStartDate = item.ExpectedStartDate.Value.ToLongDateString();
                if (item.ExpectedReturenedDate != null)
                    expectedReturnDate = item.ExpectedReturenedDate.Value.ToLongDateString();
                //string modelName = itemModelAll.Where(x => x.LanguageID == LAN &&
                //                                           x.WarehouseItemModelGUID == item.code).FirstOrDefault().ModelDescription;
                //string itemName = itemsAll.Where(x=>x.LanguageID==LAN).Select(f => f.WarehouseItemDescription).FirstOrDefault();

                dataItemOutputNotification notification = new dataItemOutputNotification();
                notification.ItemOutputDetailGUID = item.ItemOutputDetailGUID;
                notification.NotificationMessage = "Reminder to confirm";
                notification.NotificationTypeGUID = WarehosueNotificationType.Reminder;
                SendNotification(notification);
                table += "<tr><td style='border: 1px solid  #333; vertical-align: middle'>" + itemName + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + brandName + "</td><td style='border: 1px solid  #333; vertical-align: middle'> " + modelName + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + BarcodeNumber + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + SerialNumber + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + IMEI + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + GSM + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + MAC + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + expectedStartDate + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + expectedReturnDate + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + fullName + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + item.Comments + "</td></tr>";
            }
            table += "</table>";
            var IssuedUser = DbWMS.userPersonalDetailsLanguage.Where(x => x.UserGUID == UserGUID
                                                                          && x.LanguageID == LAN).FirstOrDefault();
            var _issuerCoreEmail = DbCMS.userServiceHistory.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
            if (myItemOutputDetails.Select(x => x.dataItemOutput.RequesterGUID).FirstOrDefault() == WarehouseRequestSourceTypes.Staff)
            {
                var _profile = DbWMS.userProfiles.Where(x => x.UserProfileGUID == UserProfileGUID).FirstOrDefault();
                foreach (var user in users)
                {
                    if (_profile.OrganizationInstanceGUID == Guid.Parse("E156C022-EC72-4A5A-BE09-163BD85C68EF"))
                    {
                        string _message = resxEmails.ItemModelVerification
                        .Replace("$FullName", user.userAccounts.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).Select(x => x.FirstName).FirstOrDefault() + " " + user.userAccounts.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).Select(x => x.Surname).FirstOrDefault())
                        .Replace("$VerifyLink", Anchor)
                        .Replace("$Items", table)
                        .Replace("$IssuedUser", "Sent by: " + IssuedUser.FirstName + " " + IssuedUser.Surname)

                        .Replace("$VerifyStandard", LinkStandard);
                        if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                        int isRec = 1;
                        if (user.EmailAddress == "maksoud@unhcr.org")
                            isRec = 0;
                        Send(user.EmailAddress, resxEmails.ItemModelVerificationSubject, _message, isRec, _issuerCoreEmail.EmailAddress);
                    }
                    else
                    {
                        string _message = resxEmails.ItemModelVerificationGeneral
                     .Replace("$FullName", user.userAccounts.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).Select(x => x.FirstName).FirstOrDefault() + " " + user.userAccounts.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).Select(x => x.Surname).FirstOrDefault())
                     .Replace("$VerifyLink", Anchor)
                     .Replace("$Items", table)
                     .Replace("$IssuedUser", "Sent by: " + IssuedUser.FirstName + " " + IssuedUser.Surname)

                     .Replace("$VerifyStandard", LinkStandard);
                        if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                        int isRec = 1;
                        if (user.EmailAddress == "maksoud@unhcr.org")
                            isRec = 0;
                        Send(user.EmailAddress, resxEmails.ItemModelVerificationSubject, _message, isRec, _issuerCoreEmail.EmailAddress);
                    }
                }
            }
            else if (myItemOutputDetails.Select(x => x.dataItemOutput.RequesterGUID).FirstOrDefault() == WarehouseRequestSourceTypes.Warehouse)
            {
                foreach (var user in users)
                {
                    string _message = resxEmails.warehosueCustodianItemConfirmation
                        .Replace("$FullName", user.userAccounts.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).Select(x => x.FirstName).FirstOrDefault() + " " + user.userAccounts.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).Select(x => x.Surname).FirstOrDefault())
                        .Replace("$VerifyLink", Anchor)
                        .Replace("$Items", table)
                        .Replace("$IssuedUser", "Sent by: " + IssuedUser.FirstName + " " + IssuedUser.Surname);
                    if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                    int isRec = 1;
                    if (user.EmailAddress == "maksoud@unhcr.org")
                        isRec = 0;
                    Send(user.EmailAddress, resxEmails.ItemModelVerificationSubject, _message, isRec, _issuerCoreEmail.EmailAddress);
                }

            }
        }

        #endregion

        public void SendEmailReminderForBulkItems(List<Guid> guids)
        {

            var myItemOutputDetails = DbWMS.dataItemOutputDetail.Where(x => guids.Contains(x.ItemOutputDetailGUID)).ToList();
            var requesters = myItemOutputDetails.Select(x => x.dataItemOutput.RequesterNameGUID).Distinct().ToList();
            var _issuerCoreEmail = DbCMS.userServiceHistory.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
            string URL = AppSettingsKeys.Domain + "/WMS/ModelMovements/ConfirmReceivingBulkModelEmail/?id=";

            string Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ReminderForReturningItems + "</a>";
            string Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";
            var IssuedUser = DbWMS.userPersonalDetailsLanguage.Where(x => x.UserGUID == UserGUID
                                                                          && x.LanguageID == LAN).FirstOrDefault();
            var warehouse = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
            var warehouseName = warehouse.codeWarehouse.codeWarehouseLanguage.Select(x => x.WarehouseDescription).FirstOrDefault();
            var detAll = DbWMS.dataItemInputDeterminant.ToList();
            //var itemModelAll = DbWMS.codeWarehouseItemModelLanguage.ToList();
            //var itemsAll = DbWMS.codeWarehouseItemLanguage.ToList();
            var inputs = DbWMS.dataItemInputDetail.AsQueryable();
            //check you send same table for all users 

            //    wrong change

            foreach (var requester in requesters)
            {

                var currentDetails = myItemOutputDetails.Where(x => x.dataItemOutput.RequesterNameGUID == requester).ToList();
                string modelsToConfirm = "";
                string table =
                    "<br><table class='table table-bordered' style='border-collapse: collapse; width: 900px'><tr><th style='border: 1px solid  #333; vertical-align:middle'>Description (SC)</th><th style='border: 1px solid  #333; vertical-align: middle'>Brand</th><th style='border: 1px solid  #333; vertical-align: middle'>Item</th><th style='border: 1px solid  #333; vertical-align: middle'>Barcode</th><th style='border: 1px solid  #333; vertical-align: middle'>SN </th><th style='border: 1px solid  #333; vertical-align: middle'>IMEI </th><th style='border: 1px solid  #333; vertical-align: middle'>GSM </th><th style='border: 1px solid  #333; vertical-align: middle'>MAC </th><th style='border: 1px solid  #333; vertical-align: middle'>Issue Date</th><th style='border: 1px solid  #333; vertical-align: middle'>Return Date</th><th style='border: 1px solid  #333; vertical-align: middle'>Issued By</th><th style='border: 1px solid  #333; vertical-align: middle'>Notes</th></tr>";

                foreach (var item in currentDetails)
                {
                    string BarcodeNumber = detAll.Where(x => x.ItemInputDetailGUID == item.ItemInputDetailGUID
                                                           && x.codeWarehouseItemModelDeterminant.DeterminantGUID ==
                                                           ItemDeterminants.Barcode).Select(x => x.DeterminantValue)
                      .FirstOrDefault();

                    string SerialNumber = detAll
                        .Where(x => x.ItemInputDetailGUID == item.ItemInputDetailGUID
                                    && x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.SerialNumber).Select(x => x.DeterminantValue)
                        .FirstOrDefault();
                    string IMEI = detAll
                        .Where(x => x.ItemInputDetailGUID == item.ItemInputDetailGUID
                                    && x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.IME1).Select(x => x.DeterminantValue)
                        .FirstOrDefault();
                    string GSM = detAll
             .Where(x => x.ItemInputDetailGUID == item.ItemInputDetailGUID
                         && x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.GSM).Select(x => x.DeterminantValue)
             .FirstOrDefault();
                    if (string.IsNullOrEmpty(GSM))
                        GSM = "-";
                    string MAC = detAll
                        .Where(x => x.ItemInputDetailGUID == item.ItemInputDetailGUID
                                    && x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.MAC).Select(x => x.DeterminantValue)
                        .FirstOrDefault();
                    if (string.IsNullOrEmpty(MAC))
                        MAC = "-";

                    string modelName = inputs.Where(x => x.ItemInputDetailGUID == item.ItemInputDetailGUID).Select(x =>
                         x.codeItemModelWarehouse.codeWarehouseItemModel.codeWarehouseItemModelLanguage.Where(f => f.LanguageID == LAN)
                             .Select(f => f.ModelDescription).FirstOrDefault()).FirstOrDefault();
                    string itemName = inputs.Where(x => x.ItemInputDetailGUID == item.ItemInputDetailGUID).Select(x =>
                        x.codeItemModelWarehouse.codeWarehouseItemModel.codeWarehouseItem.codeWarehouseItemLanguage.Where(f => f.LanguageID == LAN)
                            .Select(f => f.WarehouseItemDescription).FirstOrDefault()).FirstOrDefault();
                    string Brandname = inputs.Where(x => x.ItemInputDetailGUID == item.ItemInputDetailGUID).Select(x =>
                        x.codeItemModelWarehouse.codeWarehouseItemModel.codeBrand.codeBrandLanguage.Where(f => f.LanguageID == LAN)
                            .Select(f => f.BrandDescription).FirstOrDefault()).FirstOrDefault();
                    //string modelName = itemModelAll.Where(x => x.LanguageID == LAN &&
                    //                                           x.WarehouseItemModelGUID == item.code).FirstOrDefault().ModelDescription;
                    //string itemName = itemsAll.Where(x=>x.LanguageID==LAN).Select(f => f.WarehouseItemDescription).FirstOrDefault();

                    string expectedStartDate = "";
                    string expectedReturnDate = "";
                    if (item.ExpectedStartDate != null)
                        expectedStartDate = item.ExpectedStartDate.Value.ToLongDateString();
                    if (item.ExpectedReturenedDate != null)
                        expectedReturnDate = item.ExpectedReturenedDate.Value.ToLongDateString();
                    var personal = DbWMS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN
                                                                                && x.UserGUID == item.CreatedByGUID).FirstOrDefault();
                    string fullName = personal.FirstName + " " + personal.Surname;
                    string brandName = inputs.Where(x => x.ItemInputDetailGUID == item.ItemInputDetailGUID).Select(x =>
                        x.codeItemModelWarehouse.codeWarehouseItemModel.codeBrand.codeBrandLanguage.Where(f => f.LanguageID == LAN)
                            .Select(f => f.BrandDescription).FirstOrDefault()).FirstOrDefault();

                    table += "<tr><td style='border: 1px solid  #333; vertical-align: middle'>" + itemName + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + brandName + "</td><td style='border: 1px solid  #333; vertical-align: middle'> " + modelName + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + BarcodeNumber + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + SerialNumber + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + IMEI + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + GSM + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + MAC + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + expectedStartDate + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + expectedReturnDate + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + fullName + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + item.Comments + "</td></tr>";
                    dataItemOutputNotification notification = new dataItemOutputNotification();
                    notification.ItemOutputDetailGUID = item.ItemOutputDetailGUID;
                    notification.NotificationMessage = "Reminder To return item(s) to stock";
                    notification.NotificationTypeGUID = WarehosueNotificationType.Reminder;
                    SendNotification(notification);
                }
                table += "</table>";
                List<userServiceHistory> users = new List<userServiceHistory>();
                if (myItemOutputDetails.Where(x => x.dataItemOutput.RequesterNameGUID == requester).Select(x => x.dataItemOutput.RequesterGUID).FirstOrDefault() == WarehouseRequestSourceTypes.Warehouse)
                {
                    var requesterNameGUID =
                        myItemOutputDetails.Select(f => f.dataItemOutput.RequesterNameGUID).FirstOrDefault();
                    var warehouseFocalPointsGUIDs = DbWMS.codeWarehouseFocalPoint.Where(x => x.WarehouseGUID == requesterNameGUID
                    && x.IsFocalPoint == true)
                        .Select(x => x.UserGUID).ToList();
                    users = DbCMS.userServiceHistory.Where(x => warehouseFocalPointsGUIDs.Contains(x.UserGUID)).ToList();
                    var _profile = DbWMS.userProfiles.Where(x => x.UserProfileGUID == UserProfileGUID).FirstOrDefault();
                    foreach (var user in users)
                    {
                        if (_profile.OrganizationInstanceGUID == Guid.Parse("E156C022-EC72-4A5A-BE09-163BD85C68EF"))
                        {

                            string _message = resxEmails.WarehouseCustodianReturnItemsReminder
                                .Replace("$FullName", user.userAccounts.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).Select(x => x.FirstName).FirstOrDefault() + " " + user.userAccounts.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).Select(x => x.Surname).FirstOrDefault())
                                .Replace("$VerifyLink", Anchor)
                                .Replace("$Items", table)
                                .Replace("$currentStock", warehouseName)
                                .Replace("$Link", Link)
                                .Replace("$IssuedUser", "Sent by: " + IssuedUser.FirstName + " " + IssuedUser.Surname)
                            ;
                            if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }

                            int isRec = 1;
                            if (user.EmailAddress == "maksoud@unhcr.org")
                                isRec = 0;
                            Send(user.EmailAddress, resxEmails.ICTRemindeRreturningItems, _message, isRec, _issuerCoreEmail.EmailAddress);
                        }
                        else
                        {
                            string _message = resxEmails.WarehouseCustodianReturnItemsReminderGeneral
                                                           .Replace("$FullName", user.userAccounts.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).Select(x => x.FirstName).FirstOrDefault() + " " + user.userAccounts.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).Select(x => x.Surname).FirstOrDefault())
                                                           .Replace("$VerifyLink", Anchor)
                                                           .Replace("$Items", table)
                                                           .Replace("$currentStock", warehouseName)
                                                           .Replace("$Link", Link)
                                                           .Replace("$IssuedUser", "Sent by: " + IssuedUser.FirstName + " " + IssuedUser.Surname)
                                                       ;
                            if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }

                            int isRec = 1;
                            if (user.EmailAddress == "maksoud@unhcr.org")
                                isRec = 0;
                            Send(user.EmailAddress, resxEmails.ICTRemindeRreturningItems, _message, isRec, _issuerCoreEmail.EmailAddress);
                        }
                    }
                }

                else if (myItemOutputDetails.Where(x => x.dataItemOutput.RequesterNameGUID == requester).Select(x => x.dataItemOutput.RequesterGUID).FirstOrDefault() == WarehouseRequestSourceTypes.Staff)
                {
                    List<Guid?> itemGuids = myItemOutputDetails.Select(f => f.dataItemOutput.RequesterNameGUID).ToList();
                    users = DbCMS.userServiceHistory.Where(x => itemGuids.Contains(x.UserGUID) && x.Active)
                        .ToList();
                    var _profile = DbWMS.userProfiles.Where(x => x.UserProfileGUID == UserProfileGUID).FirstOrDefault();
                    foreach (var user in users)
                    {
                        if (_profile.OrganizationInstanceGUID == Guid.Parse("E156C022-EC72-4A5A-BE09-163BD85C68EF"))
                        {
                            string _message = resxEmails.CustodianReturningItemReminder
                                .Replace("$FullName", user.userAccounts.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).Select(x => x.FirstName).FirstOrDefault() + " " + user.userAccounts.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).Select(x => x.Surname).FirstOrDefault())
                                .Replace("$VerifyLink", Anchor)
                                .Replace("$Items", table)
                                .Replace("$Link", Link)
                                .Replace("$IssuedUser", "Sent by: " + IssuedUser.FirstName + " " + IssuedUser.Surname)
                            ;
                            if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }

                            int isRec = 1;
                            if (user.EmailAddress == "maksoud@unhcr.org")
                                isRec = 0;
                            Send(user.EmailAddress, resxEmails.ICTRemindeRreturningItems, _message, isRec, _issuerCoreEmail.EmailAddress);
                        }
                        else
                        {
                            string _message = resxEmails.CustodianReturningItemReminderGeneral
                     .Replace("$FullName", user.userAccounts.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).Select(x => x.FirstName).FirstOrDefault() + " " + user.userAccounts.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).Select(x => x.Surname).FirstOrDefault())
                     .Replace("$VerifyLink", Anchor)
                     .Replace("$Items", table)
                     .Replace("$Link", Link)
                     .Replace("$IssuedUser", "Sent by: " + IssuedUser.FirstName + " " + IssuedUser.Surname)
                 ;
                            if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }

                            int isRec = 1;
                            if (user.EmailAddress == "maksoud@unhcr.org")
                                isRec = 0;
                            Send(user.EmailAddress, resxEmails.ICTRemindeRreturningItems, _message, isRec, _issuerCoreEmail.EmailAddress);
                        }
                    }
                }


            }





        }

        public void SendEmailReminderForDelayItemsInReturn(List<Guid> guids)
        {
            var myItemOutputDetails = DbWMS.dataItemOutputDetail.Where(x => guids.Contains(x.ItemOutputDetailGUID)).ToList();
            var _issuerCoreEmail = DbCMS.userServiceHistory.Where(x => x.UserGUID == UserGUID).FirstOrDefault();

            string URL = AppSettingsKeys.Domain + "/WMS/ModelMovements/ConfirmReceivingBulkModelEmail/?id=";

            string Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ReminderForReturningItems + "</a>";
            string Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";

            List<userServiceHistory> users = new List<userServiceHistory>();
            List<Guid?> itemGuids = myItemOutputDetails.Select(f => f.dataItemOutput.RequesterNameGUID).ToList();
            users = DbCMS.userServiceHistory.Where(x => itemGuids.Contains(x.UserGUID)).Distinct()
                .ToList();
            var detAll = DbWMS.dataItemInputDeterminant.ToList();
            //var itemModelAll = DbWMS.codeWarehouseItemModelLanguage.ToList();
            //var itemsAll = DbWMS.codeWarehouseItemLanguage.ToList();
            var inputs = DbWMS.dataItemInputDetail.AsQueryable();
            //s

            var IssuedUser = DbWMS.userPersonalDetailsLanguage.Where(x => x.UserGUID == UserGUID
                                                                          && x.LanguageID == LAN).FirstOrDefault();
            foreach (var item in users)
            {
                string BarcodeNumber = "";
                string modelsToConfirm = "";
                string SerialNumber = "";
                string IMEI = "";
                string GSM = "";
                string MAC = "";
                string Comments = "";
                string modelName = "";
                string itemName = "";
                string Brandname = "";
                string expectedStartDate = "";
                string expectedReturnDate = "";
                string fullName = "";
                string brandName = "";
                Guid itemOutputGuid = Guid.Empty;
                string table =
                    "<br><table class='table table-bordered' style='border-collapse: collapse; width: 900px'><tr><th style='border: 1px solid  #333; vertical-align:middle'>Description (SC)</th><th style='border: 1px solid  #333; vertical-align: middle'>Brand</th><th style='border: 1px solid  #333; vertical-align: middle'>Item</th><th style='border: 1px solid  #333; vertical-align: middle'>Barcode</th><th style='border: 1px solid  #333; vertical-align: middle'>SN </th><th style='border: 1px solid  #333; vertical-align: middle'>IMEI </th><th style='border: 1px solid  #333; vertical-align: middle'>GSM </th><th style='border: 1px solid  #333; vertical-align: middle'>MAC </th><th style='border: 1px solid  #333; vertical-align: middle'>Issue Date</th><th style='border: 1px solid  #333; vertical-align: middle'>Return Date</th><th style='border: 1px solid  #333; vertical-align: middle'>Issued By</th><th style='border: 1px solid  #333; vertical-align: middle'>Notes</th></tr>";

                foreach (var myItemou in myItemOutputDetails.Where(x => x.dataItemOutput.RequesterNameGUID == item.UserGUID).ToList())
                {
                    BarcodeNumber = detAll.Where(x => x.ItemInputDetailGUID == myItemou.ItemInputDetailGUID
                                                        && x.codeWarehouseItemModelDeterminant.DeterminantGUID ==
                                                        ItemDeterminants.Barcode).Select(x => x.DeterminantValue)
                   .FirstOrDefault();
                    if (string.IsNullOrEmpty(BarcodeNumber))
                        BarcodeNumber = "-";


                    SerialNumber = detAll
                       .Where(x => x.ItemInputDetailGUID == myItemou.ItemInputDetailGUID
                                   && x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.SerialNumber).Select(x => x.DeterminantValue)
                       .FirstOrDefault();
                    if (string.IsNullOrEmpty(SerialNumber))
                        SerialNumber = "-";
                    IMEI = detAll
                        .Where(x => x.ItemInputDetailGUID == myItemou.ItemInputDetailGUID
                                    && x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.IME1).Select(x => x.DeterminantValue)
                        .FirstOrDefault();

                    if (string.IsNullOrEmpty(IMEI))
                        IMEI = "-";

                    GSM = detAll
             .Where(x => x.ItemInputDetailGUID == myItemou.ItemInputDetailGUID
                         && x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.GSM).Select(x => x.DeterminantValue)
             .FirstOrDefault();
                    if (string.IsNullOrEmpty(GSM))
                        GSM = "-";
                    MAC = detAll
                       .Where(x => x.ItemInputDetailGUID == myItemou.ItemInputDetailGUID
                                   && x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.MAC).Select(x => x.DeterminantValue)
                       .FirstOrDefault();
                    if (string.IsNullOrEmpty(MAC))
                        MAC = "-";

                    modelName = inputs.Where(x => x.ItemInputDetailGUID == myItemou.ItemInputDetailGUID).Select(x =>
                         x.codeItemModelWarehouse.codeWarehouseItemModel.codeWarehouseItemModelLanguage.Where(f => f.LanguageID == LAN)
                             .Select(f => f.ModelDescription).FirstOrDefault()).FirstOrDefault();
                    itemName = inputs.Where(x => x.ItemInputDetailGUID == myItemou.ItemInputDetailGUID).Select(x =>
                       x.codeItemModelWarehouse.codeWarehouseItemModel.codeWarehouseItem.codeWarehouseItemLanguage.Where(f => f.LanguageID == LAN)
                           .Select(f => f.WarehouseItemDescription).FirstOrDefault()).FirstOrDefault();
                    Brandname = inputs.Where(x => x.ItemInputDetailGUID == myItemou.ItemInputDetailGUID).Select(x =>
                       x.codeItemModelWarehouse.codeWarehouseItemModel.codeBrand.codeBrandLanguage.Where(f => f.LanguageID == LAN)
                           .Select(f => f.BrandDescription).FirstOrDefault()).FirstOrDefault();
                    //string modelName = itemModelAll.Where(x => x.LanguageID == LAN &&
                    //                                           x.WarehouseItemModelGUID == item.code).FirstOrDefault().ModelDescription;
                    //string itemName = itemsAll.Where(x=>x.LanguageID==LAN).Select(f => f.WarehouseItemDescription).FirstOrDefault();

                    expectedStartDate = "";
                    expectedReturnDate = "";
                    if (myItemou.ExpectedStartDate != null)
                        expectedStartDate = myItemou.ExpectedStartDate.Value.ToLongDateString();
                    if (myItemou.ExpectedReturenedDate != null)
                        expectedReturnDate = myItemou.ExpectedReturenedDate.Value.ToLongDateString();
                    var personal = DbWMS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN
                                                                                && x.UserGUID == myItemou.CreatedByGUID).FirstOrDefault();
                    fullName = personal.FirstName + " " + personal.Surname;
                    brandName = inputs.Where(x => x.ItemInputDetailGUID == myItemou.ItemInputDetailGUID).Select(x =>
                       x.codeItemModelWarehouse.codeWarehouseItemModel.codeBrand.codeBrandLanguage.Where(f => f.LanguageID == LAN)
                           .Select(f => f.BrandDescription).FirstOrDefault()).FirstOrDefault();
                    itemOutputGuid = myItemou.ItemOutputDetailGUID;
                    Comments = myItemou.Comments;

                }




                table += "<tr><td style='border: 1px solid  #333; vertical-align: middle'>" + itemName + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + brandName + "</td><td style='border: 1px solid  #333; vertical-align: middle'> " + modelName + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + BarcodeNumber + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + SerialNumber + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + IMEI + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + GSM + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + MAC + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + expectedStartDate + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + expectedReturnDate + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + fullName + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + Comments + "</td></tr>";
                dataItemOutputNotification notification = new dataItemOutputNotification();
                notification.ItemOutputDetailGUID = itemOutputGuid;
                notification.NotificationMessage = "Reminder for delay item(s) ";
                notification.NotificationTypeGUID = WarehosueNotificationType.Reminder;
                SendNotification(notification);
                table += "</table>";
                var _profile = DbWMS.userProfiles.Where(x => x.UserProfileGUID == UserProfileGUID).FirstOrDefault();
                if (_profile.OrganizationInstanceGUID == Guid.Parse("E156C022-EC72-4A5A-BE09-163BD85C68EF"))
                {
                    string _message = resxEmails.WarehouseReminderForDelayItemInReturn
                        .Replace("$FullName", item.userAccounts.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).Select(x => x.FirstName).FirstOrDefault() + " " + item.userAccounts.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).Select(x => x.Surname).FirstOrDefault())

                        .Replace("$Items", table)

                        .Replace("$IssuedUser", "Sent by: " + IssuedUser.FirstName + " " + IssuedUser.Surname)
                    ;
                    if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }


                    Send(item.EmailAddress, resxEmails.ICTRemindeRreturningItems, _message, 1, _issuerCoreEmail.EmailAddress);
                }
                else
                {

                    string _message = resxEmails.WarehouseReminderForDelayItemInReturnGeneral
                             .Replace("$FullName", item.userAccounts.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).Select(x => x.FirstName).FirstOrDefault() + " " + item.userAccounts.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).Select(x => x.Surname).FirstOrDefault())

                             .Replace("$Items", table)

                             .Replace("$IssuedUser", "Sent by: " + IssuedUser.FirstName + " " + IssuedUser.Surname)
                         ;
                    if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }


                    Send(item.EmailAddress, resxEmails.ICTRemindeRreturningItems, _message, 1, _issuerCoreEmail.EmailAddress);

                }

                //modelsToConfirm += "Model  :"+item.dataItemInputDetail.codeItemModelWarehouse.codeWarehouseItemModel.codeWarehouseItemModelLanguage.Where(x=>x.LanguageID==LAN).FirstOrDefault().ModelDescription
                //             +" Barcode Number/Serial Number :"+item.dataItemInputDetail.dataItemInputDeterminant.Where(x=>x.ItemInputDetailGUID==item.ItemInputDetailGUID).FirstOrDefault().DeterminantValue+"<br/>";


            }




        }
        public void Send(string recipients, string subject, string body, int? isRec, string otheruser)
        {
            var _profile = DbWMS.userProfiles.Where(x => x.UserProfileGUID == UserProfileGUID).FirstOrDefault();
            //"syrdasti@unhcr.org"
            var _emailGroup = DbWMS.codeWMSEmailGroup.Where(x => x.OrgnanizationInstanceGUID == _profile.OrganizationInstanceGUID).FirstOrDefault();
            string copysyrda = (_emailGroup != null ? _emailGroup.EmailAddressName : null) + ";" + otheruser;
            string copy_recipients = string.Join(" ;", copysyrda);
            // string copy_recipients = "";
            //if (isRec == 1)
            //{
            //    copy_recipients = "sahhar@unhcr.org;AYDI@unhcr.org;rampersa@unhcr.org;isac@unhcr.org;";
            //}

            string blind_copy_recipients = null;
            string body_format = "HTML";
            string importance = "Normal";
            string file_attachments = null;
            string _body = "<div style='font-family:Arial;'>" + body.Replace("\r\n", "<br/>") + "</div>";
            if (LAN == "AR") { _body = "<p align='right'>" + _body + "</p>"; }
            // DbCMS.SendEmailSTI("maksoud@unhcr.org", "maksoud@unhcr.org", "maksoud@unhcr.org", subject, _body, body_format, importance, file_attachments);
            var check_Send = (_emailGroup != null ? _emailGroup.EmailAddressName : null);
            DbCMS.SendEmailGeneral(recipients, copy_recipients, blind_copy_recipients, subject, check_Send, _body, body_format, importance, file_attachments);
        }
        #endregion

        #region Send Reminders
        public ActionResult WarehosueReleaseModelNotificationCreate(Guid? PK)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            return PartialView("~/Areas/WMS/Views/ReleaseModelNotification/_ReleaseModelNotificationUpdateModal.cshtml",
                   new ItemOutputDetailNotificationModel { ItemOutputDetailGUID = PK });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult WarehosueReleaseModelNotificationCreate(ItemOutputDetailNotificationModel model)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            dataItemOutputNotification myModel = Mapper.Map(model, new dataItemOutputNotification());
            // if (!ModelState.IsValid || ActiveDamagedModelFlow(model)) return PartialView("~/Areas/WMS/Views/ReleaseModelFlows/_ReleaseModelFlowUpdateModal.cshtml", model);
            DateTime ExecutionTime = DateTime.Now;
            var _issuerCoreEmail = DbCMS.userServiceHistory.Where(x => x.UserGUID == UserGUID).FirstOrDefault();

            myModel.ItemOutputNotificationGUID = Guid.NewGuid();
            myModel.NotficationDate = ExecutionTime;


            myModel.CreatedDate = ExecutionTime;

            myModel.CreatedByGUID = UserGUID;
            myModel.Active = true;
            DbWMS.Create(myModel, Permissions.WarehouseItemsRelease.CreateGuid, ExecutionTime, DbCMS);

            var mydataItemOutputDetail = DbWMS.dataItemOutputDetail
                .Where(x => x.ItemOutputDetailGUID == model.ItemOutputDetailGUID).FirstOrDefault();

            string URL = AppSettingsKeys.Domain + "/WMS/ModelMovements/ConfirmReceivingModelEmail/?id=" + new Portal().GUIDToString(model.ItemOutputDetailGUID);
            string Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ConfirmItemReceiving + "</a>";
            string Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";
            Models.userServiceHistory user = DbCMS.userServiceHistory
                .Where(x => x.UserGUID == mydataItemOutputDetail.dataItemOutput.RequesterNameGUID)
                .FirstOrDefault();

            var BarcodeNumber = mydataItemOutputDetail.dataItemInputDetail.dataItemInputDeterminant
                .Where(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.Barcode)
                .Select(x => x.DeterminantValue).FirstOrDefault();
            var SerialNumber = mydataItemOutputDetail.dataItemInputDetail.dataItemInputDeterminant
                .Where(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.SerialNumber)
                .Select(x => x.DeterminantValue).FirstOrDefault();
            var IME1 = mydataItemOutputDetail.dataItemInputDetail.dataItemInputDeterminant
                .Where(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.IME1)
                .Select(x => x.DeterminantValue).FirstOrDefault();
            var GSM = mydataItemOutputDetail.dataItemInputDetail.dataItemInputDeterminant
                .Where(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.IME1)
                .Select(x => x.DeterminantValue).FirstOrDefault();
            var MAC = mydataItemOutputDetail.dataItemInputDetail.dataItemInputDeterminant
        .Where(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.MAC)
        .Select(x => x.DeterminantValue).FirstOrDefault();

            string det = "";
            if (BarcodeNumber != null)
                det += "Barcode Number: " + BarcodeNumber + ", ";
            if (SerialNumber != null)

                det += "Serial Number: " + SerialNumber + ", ";
            if (IME1 != null)

                det += "IME1: " + IME1 + ", ";
            if (GSM != null)

                det += "GSM Number: " + GSM + ", ";
            if (MAC != null)

                det += "MAC: " + MAC + ", ";

            var _profile = DbWMS.userProfiles.Where(x => x.UserProfileGUID == UserProfileGUID).FirstOrDefault();

            if (_profile.OrganizationInstanceGUID == Guid.Parse("E156C022-EC72-4A5A-BE09-163BD85C68EF"))
            {

                string _message = resxEmails.ModelConfirmationReminder
                .Replace("$FullName", user.userAccounts.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).Select(x => x.FirstName).FirstOrDefault() + " " + user.userAccounts.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).Select(x => x.Surname).FirstOrDefault())
                .Replace("$VerifyLink", Anchor)
                .Replace("$Items", mydataItemOutputDetail.dataItemInputDetail.codeItemModelWarehouse.codeWarehouseItemModel.codeWarehouseItemModelLanguage.Where(x => x.LanguageID == LAN).Select(x => x.ModelDescription).FirstOrDefault() + " " + det)
                .Replace("$Link", Link);
                if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                int isRec = 1;
                if (user.EmailAddress == "maksoud@unhcr.org")
                    isRec = 0;
                Send(user.EmailAddress, resxEmails.ItemModelVerificationSubject, _message, isRec, _issuerCoreEmail.EmailAddress);

            }
            else
            {

                string _message = resxEmails.ModelConfirmationReminderGeneral
                .Replace("$FullName", user.userAccounts.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).Select(x => x.FirstName).FirstOrDefault() + " " + user.userAccounts.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).Select(x => x.Surname).FirstOrDefault())
                .Replace("$VerifyLink", Anchor)
                .Replace("$Items", mydataItemOutputDetail.dataItemInputDetail.codeItemModelWarehouse.codeWarehouseItemModel.codeWarehouseItemModelLanguage.Where(x => x.LanguageID == LAN).Select(x => x.ModelDescription).FirstOrDefault() + " " + det)
                .Replace("$Link", Link);
                if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                int isRec = 1;
                if (user.EmailAddress == "maksoud@unhcr.org")
                    isRec = 0;
                Send(user.EmailAddress, resxEmails.ItemModelVerificationSubject, _message, isRec, _issuerCoreEmail.EmailAddress);

            }

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleUpdateMessage(DataTableNames.WarehouseModelReleaseMovementsDataTable, DbWMS.PrimaryKeyControl(myModel), DbWMS.RowVersionControls(Portal.SingleToList(myModel))));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }
        #endregion

        #region Confirmation by ICT 

        public ActionResult WarehosueReleaseModelCancelRequest(Guid PK)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            return PartialView("~/Areas/WMS/Views/ModelConfirmation/_WarehosueReleaseModelCancelRequest.cshtml",
                new dataItemOutputDetail { ItemOutputDetailGUID = PK });
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult WarehosueReleaseModelCancelRequest(dataItemOutputDetail model)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            dataItemOutputDetailFlow myModel = Mapper.Map(model, new dataItemOutputDetailFlow());
            // if (!ModelState.IsValid || ActiveDamagedModelFlow(model)) return PartialView("~/Areas/WMS/Views/ReleaseModelFlows/_ReleaseModelFlowUpdateModal.cshtml", model);
            DateTime ExecutionTime = DateTime.Now;

            List<dataItemOutputDetailFlow> flows = DbWMS.dataItemOutputDetailFlow
                .Where(x => x.ItemOutputDetailGUID == myModel.ItemOutputDetailGUID).ToList();



            foreach (var item in flows)
            {
                item.IsLastAction = false;
                item.IsLastMove = false;
            }

            DbWMS.SaveChanges();
            var myoutputDetail = DbWMS.dataItemOutputDetail
                .Where(x => x.ItemOutputDetailGUID == myModel.ItemOutputDetailGUID).FirstOrDefault();
            var _inputold = DbWMS.dataItemInputDetail.Where(x => x.ItemInputDetailGUID == myoutputDetail.ItemInputDetailGUID).FirstOrDefault();
            var _lastcustGUID = _inputold.LastCustdianNameGUID;
            //List<dataItemInputDetail> ItemInputs = DbWMS.dataItemInputDetail
            //    .Where(x => x.ParentItemModelWarehouseGUID == myoutputDetail.ItemInputDetailGUID).ToList();
            List<dataItemOutputDetail> childrenDetails = DbWMS.dataItemOutputDetail.Where(
                x => x.dataItemInputDetail.ParentItemModelWarehouseGUID == myoutputDetail.dataItemInputDetail.ItemModelWarehouseGUID
            ).ToList();
            var myCurrentWarehouseGUID = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).Select(x => x.WarehouseGUID)
                .FirstOrDefault();
            var myWarehouse = DbWMS.codeWarehouseLanguage.Where(x => x.WarehouseGUID == myCurrentWarehouseGUID
                                && x.LanguageID == LAN).FirstOrDefault();
            foreach (var myOutputChildren in childrenDetails.Where(x => x.ItemOutputDetailGUID != myModel.ItemOutputDetailGUID))
            {
                List<dataItemOutputDetailFlow> myflows = DbWMS.dataItemOutputDetailFlow
                    .Where(x => x.ItemOutputDetailGUID == myOutputChildren.ItemOutputDetailGUID).ToList();
                foreach (var flowchildren in myflows)
                {
                    flowchildren.IsLastAction = false;
                    flowchildren.IsLastMove = false;
                }

                DbWMS.SaveChanges();
                int? myorder = DbWMS.dataItemOutputDetailFlow
                                 .Where(x => x.dataItemOutputDetail.ItemInputDetailGUID == myOutputChildren.ItemInputDetailGUID)
                                 .Select(x => x.OrderId).Max() + 1;
                if (myorder == null)
                    myorder = 1;
                dataItemOutputDetailFlow myFlowDetailFlow = new dataItemOutputDetailFlow();
                myFlowDetailFlow.ItemOutputDetailFlowGUID = Guid.NewGuid();
                myFlowDetailFlow.ItemOutputDetailGUID = myOutputChildren.ItemOutputDetailGUID;
                myFlowDetailFlow.FlowTypeGUID = WarehouseRequestFlowType.Cancelled;
                myFlowDetailFlow.CreatedDate = ExecutionTime;
                myFlowDetailFlow.ItemStatuGUID = WarehouseItemStatus.Functionting;
                myFlowDetailFlow.IsLastAction = true;
                myFlowDetailFlow.IsLastMove = true;
                myFlowDetailFlow.OrderId = myorder;
                myFlowDetailFlow.CreatedByGUID = UserGUID;
                myFlowDetailFlow.Active = true;
                DbWMS.CreateNoAudit(myFlowDetailFlow);
                var detailChildren = DbWMS.dataItemOutputDetail.Where(x => x.ItemOutputDetailGUID == myOutputChildren.ItemOutputDetailGUID)
                    .FirstOrDefault();
                var myChildreninput = DbWMS.dataItemInputDetail.Find(detailChildren.ItemInputDetailGUID);
                myChildreninput.LastFlowTypeGUID = WarehouseRequestFlowType.Returned;

                myChildreninput.IsAvaliable = true;

                myChildreninput.ItemServiceStatusGUID = WarehouseServiceItemStatus.InStock;
                myChildreninput.LastCustdianGUID = WarehouseRequestSourceTypes.Warehouse;
                myChildreninput.LastCustdianNameGUID = myCurrentWarehouseGUID;
                myChildreninput.LastCustodianName = myWarehouse.WarehouseDescription;
                DbWMS.UpdateNoAudit(myChildreninput);



            }
            //DbWMS.Up(flows, Permissions.WarehouseItemsEntry.UpdateGuid, ExecutionTime, DbCMS);
            var itemOutputdetail = DbWMS.dataItemOutputDetail
                .Where(x => x.ItemOutputDetailGUID == myModel.ItemOutputDetailGUID).FirstOrDefault();
            int? order = DbWMS.dataItemOutputDetailFlow
                             .Where(x => x.dataItemOutputDetail.ItemInputDetailGUID == itemOutputdetail.ItemInputDetailGUID)
                             .Select(x => x.OrderId).Max() + 1;
            if (order == null)
                order = 1;

            myModel.ItemOutputDetailFlowGUID = Guid.NewGuid();
            myModel.FlowTypeGUID = WarehouseRequestFlowType.Cancelled;

            myModel.CreatedDate = ExecutionTime;
            myModel.ItemStatuGUID = WarehouseItemStatus.Functionting;
            myModel.IsLastAction = true;
            myModel.IsLastMove = true;
            myModel.OrderId = order;
            myModel.CreatedByGUID = UserGUID;
            myModel.Active = true;
            DbWMS.CreateNoAudit(myModel);
            var outputDetail = DbWMS.dataItemOutputDetail.Where(x => x.ItemOutputDetailGUID == model.ItemOutputDetailGUID)
                .FirstOrDefault();
            var input = DbWMS.dataItemInputDetail.Find(outputDetail.ItemInputDetailGUID);
            var oldInput = input;

            //inputDetail.LastLocationGUID = ;


            input.LastFlowTypeGUID = WarehouseRequestFlowType.Returned;
            input.IsAvaliable = true;
            input.ItemServiceStatusGUID = WarehouseServiceItemStatus.InStock;
            input.LastCustdianGUID = WarehouseRequestSourceTypes.Warehouse;
            input.LastWarehouseLocationGUID = myWarehouse.codeWarehouse.WarehouseLocationGUID;
            input.LastCustdianNameGUID = myCurrentWarehouseGUID;
            input.LastCustodianName = myWarehouse.WarehouseDescription;
            DbWMS.UpdateNoAudit(input);
            #region transers
            List<dataItemTransfer> allPriviousTransfers = DbWMS.dataItemTransfer
                               .Where(x => x.ItemInputDetailGUID == input.ItemInputDetailGUID).ToList();

            List<dataItemTransfer> toAddtrans = new List<dataItemTransfer>();
            foreach (var tran in allPriviousTransfers)
            {
                tran.IsLastTransfer = false;
            }
            DbWMS.UpdateBulk(allPriviousTransfers, Permissions.WarehouseItemsEntry.UpdateGuid, ExecutionTime, DbCMS);
            //var warehouse = DbWMS.codeWarehouse.Where(x => x.WarehouseLocationGUID == test.WarehouseLocationGUID).FirstOrDefault();
            dataItemTransfer changeTransferModel = new dataItemTransfer();
            changeTransferModel.ItemTransferGUID = Guid.NewGuid();
            changeTransferModel.ItemInputDetailGUID = input.ItemInputDetailGUID;
            changeTransferModel.TransferDate = ExecutionTime;
            changeTransferModel.SourceGUID = myCurrentWarehouseGUID;
            changeTransferModel.DestionationGUID = myWarehouse.WarehouseGUID;
            changeTransferModel.TransferedByGUID = UserGUID;
            changeTransferModel.IsLastTransfer = true;
            DbWMS.Create(changeTransferModel, Permissions.WarehouseItemsEntry.UpdateGuid, ExecutionTime, DbCMS);

            #endregion




            try
            {

                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                //return RedirectToAction("ConfirmReceivingModelEmail", new { id = model.ItemOutputDetailGUID });
                //return View("Login");
                dataItemOutputDetail itemOutputDetail = DbWMS.dataItemOutputDetail.Find(model.ItemOutputDetailGUID);

                if (_inputold != null && (_inputold.ItemModelWarehouseGUID == Guid.Parse("FA27E544-A88F-4CA0-86E0-C8B4BF40AF44") || _inputold.ItemModelWarehouseGUID == Guid.Parse("56723035-36DE-4A91-8584-2B12FC4EE99C")))
                {

                    var myStaff = DbWMS.StaffCoreData.Where(x => x.UserGUID == _lastcustGUID).FirstOrDefault();
                    if (myStaff != null)
                    {
                        var checkSTI = DbWMS.v_StaffContactsListForSTI.Where(x => x.EmailWork == myStaff.EmailAddress).FirstOrDefault();
                        // var myContact = DbWMS.StaffContactsInformation.Where(x => x.Emailwork.ToLower() == myStaff.EmailAddress.ToLower()).FirstOrDefault();
                        if (checkSTI == null)
                        {
                            myStaff.OfficialMobileNumber = null;
                        }
                        if (checkSTI != null && checkSTI.Mobile11 != myStaff.OfficialMobileNumber)
                        {
                            myStaff.OfficialMobileNumber = checkSTI.Mobile11;
                        }


                        //var myContact = DbWMS.StaffContactsInformation.Where(x => x.Emailwork.ToLower() == myStaff.EmailAddress.ToLower()).FirstOrDefault();
                        //if (myContact.Mobile1 == myStaff.OfficialMobileNumber)
                        //{
                        //    myStaff.OfficialMobileNumber = null;
                        //}

                        DbWMS.Update(myStaff, Permissions.WarehouseItemsRelease.UpdateGuid, ExecutionTime, DbCMS);

                        DbWMS.SaveChanges();
                        DbCMS.SaveChanges();
                    }

                }
                var _checkStaffIssuer = DbAHD.StaffCoreData.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
                if (_checkStaffIssuer != null)
                {
                    DbWMS.sp_UpdatePhonesToHR();
                }


                return View("~/Areas/WMS/Views/ModelConfirmation/ConfirmReceivingModelEmail.cshtml", new ReleaseSingleItemUpdateModalUpdateModel());

            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }



        }

        public ActionResult WarehosueReleaseModelICTConfimrationCreate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            return PartialView("~/Areas/WMS/Views/ModelConfirmation/_WarehosueReleaseModelICTConfimrationCreate.cshtml",
                new dataItemOutputDetail { ItemOutputDetailGUID = PK });
        }
        [HttpPost, ValidateAntiForgeryToken]

        public ActionResult WarehosueReleaseModelICTConfimrationCreate(dataItemOutputDetail model)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            dataItemOutputDetailFlow myModel = Mapper.Map(model, new dataItemOutputDetailFlow());
            // if (!ModelState.IsValid || ActiveDamagedModelFlow(model)) return PartialView("~/Areas/WMS/Views/ReleaseModelFlows/_ReleaseModelFlowUpdateModal.cshtml", model);
            DateTime ExecutionTime = DateTime.Now;
            List<dataItemOutputDetailFlow> flows = DbWMS.dataItemOutputDetailFlow
                .Where(x => x.ItemOutputDetailGUID == myModel.ItemOutputDetailGUID).ToList();



            foreach (var item in flows)
            {
                item.IsLastAction = false;
                item.IsLastMove = false;
            }

            DbWMS.SaveChanges();
            var myoutputDetail = DbWMS.dataItemOutputDetail
                .Where(x => x.ItemOutputDetailGUID == myModel.ItemOutputDetailGUID).FirstOrDefault();

            var children = DbWMS.dataItemInputDetail.Where(x => x.ItemInputDetailGUID == model.ItemInputDetailGUID).ToList();


            //List<dataItemInputDetail> ItemInputs = DbWMS.dataItemInputDetail
            //    .Where(x => x.ParentItemModelWarehouseGUID == myoutputDetail.ItemInputDetailGUID).ToList();
            //List<dataItemOutputDetail> childrenDetails = DbWMS.dataItemOutputDetail.Where(
            //    x => x.dataItemOutput.OutputNumber == myoutputDetail.dataItemOutput.OutputNumber
            //).ToList();
            //if they need to update all outputnumber uncomment
            //foreach (var myOutputChildren in childrenDetails.Where(x => x.ItemOutputDetailGUID != myModel.ItemOutputDetailGUID))
            //{
            //    List<dataItemOutputDetailFlow> myflows = DbWMS.dataItemOutputDetailFlow
            //        .Where(x => x.ItemOutputDetailGUID == myOutputChildren.ItemOutputDetailGUID).ToList();
            //    foreach (var flowchildren in myflows)
            //    {
            //        flowchildren.IsLastAction = false;
            //        flowchildren.IsLastMove = false;
            //    }

            //    DbWMS.SaveChanges();
            //    int? myorder = DbWMS.dataItemOutputDetailFlow
            //                     .Where(x => x.dataItemOutputDetail.ItemInputDetailGUID == myOutputChildren.ItemInputDetailGUID)
            //                     .Select(x => x.OrderId).Max() + 1;
            //    if (myorder == null)
            //        myorder = 1;
            //    dataItemOutputDetailFlow myFlowDetailFlow = new dataItemOutputDetailFlow();
            //    myFlowDetailFlow.ItemOutputDetailFlowGUID = Guid.NewGuid();
            //    myFlowDetailFlow.ItemOutputDetailGUID = myOutputChildren.ItemOutputDetailGUID;
            //    if (myoutputDetail.dataItemOutput.RequesterGUID == WarehouseRequestSourceTypes.Staff)
            //    {
            //        myFlowDetailFlow.FlowTypeGUID = WarehouseRequestFlowType.Confirmed;
            //    }
            //    else if (myoutputDetail.dataItemOutput.RequesterGUID == WarehouseRequestSourceTypes.Warehouse)
            //    {
            //        myFlowDetailFlow.FlowTypeGUID = WarehouseRequestFlowType.Returned;
            //    }
            //    myFlowDetailFlow.CreatedDate = ExecutionTime;
            //    myFlowDetailFlow.ItemStatuGUID = WarehouseItemStatus.Functionting;
            //    myFlowDetailFlow.IsLastAction = true;
            //    myFlowDetailFlow.IsLastMove = true;
            //    myFlowDetailFlow.OrderId = myorder;
            //    myFlowDetailFlow.CreatedByGUID = UserGUID;
            //    myFlowDetailFlow.Active = true;
            //    DbWMS.CreateNoAudit(myFlowDetailFlow);
            //    var detailChildren = DbWMS.dataItemOutputDetail.Where(x => x.ItemOutputDetailGUID == myOutputChildren.ItemOutputDetailGUID)
            //        .FirstOrDefault();
            //    var myChildreninput = DbWMS.dataItemInputDetail.Find(detailChildren.ItemInputDetailGUID);
            //    if (myoutputDetail.dataItemOutput.RequesterGUID == WarehouseRequestSourceTypes.Staff)
            //    {
            //        myChildreninput.LastFlowTypeGUID = WarehouseRequestFlowType.Confirmed;
            //    }
            //    else if (myoutputDetail.dataItemOutput.RequesterGUID == WarehouseRequestSourceTypes.Warehouse)
            //    {
            //        myChildreninput.LastFlowTypeGUID = WarehouseRequestFlowType.Returned;
            //    }
            //        DbWMS.UpdateNoAudit(myChildreninput);



            //}
            //DbWMS.Up(flows, Permissions.WarehouseItemsEntry.UpdateGuid, ExecutionTime, DbCMS);
            var itemOutputdetail = DbWMS.dataItemOutputDetail
                .Where(x => x.ItemOutputDetailGUID == myModel.ItemOutputDetailGUID).FirstOrDefault();
            int? order = DbWMS.dataItemOutputDetailFlow
                             .Where(x => x.dataItemOutputDetail.ItemInputDetailGUID == itemOutputdetail.ItemInputDetailGUID)
                             .Select(x => x.OrderId).Max() + 1;
            if (order == null)
                order = 1;

            myModel.ItemOutputDetailFlowGUID = Guid.NewGuid();
            if (myoutputDetail.dataItemOutput.RequesterGUID == WarehouseRequestSourceTypes.Staff)
            {
                myModel.FlowTypeGUID = WarehouseRequestFlowType.Confirmed;
            }
            if (myoutputDetail.dataItemOutput.RequesterGUID == WarehouseRequestSourceTypes.Warehouse)
            {
                myModel.FlowTypeGUID = WarehouseRequestFlowType.Returned;
            }

            myModel.CreatedDate = ExecutionTime;
            myModel.ItemStatuGUID = WarehouseItemStatus.Functionting;
            myModel.IsLastAction = true;
            myModel.IsLastMove = true;
            myModel.OrderId = order;
            myModel.CreatedByGUID = UserGUID;
            myModel.Active = true;
            DbWMS.CreateNoAudit(myModel);
            var outputDetail = DbWMS.dataItemOutputDetail.Where(x => x.ItemOutputDetailGUID == model.ItemOutputDetailGUID)
                .FirstOrDefault();
            var input = DbWMS.dataItemInputDetail.Find(outputDetail.ItemInputDetailGUID);
            if (myoutputDetail.dataItemOutput.RequesterGUID == WarehouseRequestSourceTypes.Staff)
            {
                input.LastFlowTypeGUID = WarehouseRequestFlowType.Confirmed;
            }
            if (myoutputDetail.dataItemOutput.RequesterGUID == WarehouseRequestSourceTypes.Warehouse)
            {
                input.LastFlowTypeGUID = WarehouseRequestFlowType.Returned;
            }

            DbWMS.UpdateNoAudit(input);



            try
            {

                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                //return RedirectToAction("ConfirmReceivingModelEmail", new { id = model.ItemOutputDetailGUID });
                //return View("Login");
                dataItemOutputDetail itemOutputDetail = DbWMS.dataItemOutputDetail.Find(model.ItemOutputDetailGUID);
                //return View("ModelConfirmationComplate");
                return View("~/Areas/WMS/Views/ModelConfirmation/ConfirmReceivingModelEmail.cshtml", new ReleaseSingleItemUpdateModalUpdateModel());
                //return RedirectToAction("ConfirmReceivingModelEmail", new { id = model.ItemOutputDetailGUID });
                //return Json(DbWMS.SingleUpdateMessage(DataTableNames.WarehouseModelReleaseMovementsDataTable, DbWMS.PrimaryKeyControl(itemOutputDetail), DbWMS.RowVersionControls(Portal.SingleToList(itemOutputDetail))));
                // return Json(DbWMS.SingleUpdateMessage(DataTableNames.WarehouseModelReleaseMovementsDataTable, DbWMS.PrimaryKeyControl(myModel), DbWMS.RowVersionControls(Portal.SingleToList(myModel))));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }



        }

        #endregion

        #region Upload damaged report 

        public ActionResult UpladDamagedModelReport(ItemInputDetailModel model)
        {
            if (model.ItemStatusGUID == null)
            {
                return PartialView("~/Areas/WMS/Views/ModelEntryMovements/_EditModelEntryMovementForm.cshtml", model);

            }

            if (model.UploadDamagedReportFile == null)
            {


                return PartialView("~/Areas/WMS/Views/ModelEntryMovements/_EditModelEntryMovementForm.cshtml", model);

            }
            else
            {
                Guid EntityPK = Guid.NewGuid();
                var filePath = Server.MapPath("~\\Uploads\\WMS\\DamagedItemReports\\");
                string extension = Path.GetExtension(model.UploadDamagedReportFile.FileName);
                string fullFileName = filePath + "\\" + EntityPK + extension;
                model.UploadDamagedReportFile.SaveAs(fullFileName);
                //MissionReport.ReportExtensionType = extension;



                DateTime ExecutionTime = DateTime.Now;
                dataItemOutputDetailDamagedTrack DamagedItemModel = Mapper.Map(model, new dataItemOutputDetailDamagedTrack());
                DamagedItemModel.ItemOutputDetailDamagedTrackGUID = EntityPK;
                DamagedItemModel.ItemInputDetailGUID = (Guid)model.ItemInputDetailGUID;
                DamagedItemModel.DamagedTypeGUID = (Guid)model.ItemStatusGUID;
                DamagedItemModel.CreatedByGUID = UserGUID;
                DamagedItemModel.CreatedDate = ExecutionTime;
                var focal = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
                var warehouse = DbWMS.codeWarehouseLanguage.Where(x => x.WarehouseGUID == focal.WarehouseGUID && x.LanguageID == LAN).FirstOrDefault();
                DbWMS.Create(DamagedItemModel, Permissions.WarehouseItemsRelease.CreateGuid, ExecutionTime, DbCMS);
                var inputDetail = DbWMS.dataItemInputDetail.Where(x => x.ItemInputDetailGUID == model.ItemInputDetailGUID)
                    .FirstOrDefault();
                inputDetail.IsAvaliable = false;
                inputDetail.ItemServiceStatusGUID = WarehouseServiceItemStatus.InService;
                //inputDetail.LastCustodianName = warehouse.WarehouseDescription;
                //inputDetail.LastCustdianNameGUID = warehouse.WarehouseGUID;
                //inputDetail.lastcu = warehouse.WarehouseGUID;
                inputDetail.ItemStatusGUID = (Guid)model.ItemStatusGUID;
                DbWMS.Update(inputDetail, Permissions.WarehouseItemsEntry.UpdateGuid, ExecutionTime, DbCMS);

                try
                {
                    DbWMS.SaveChanges();
                    DbCMS.SaveChanges();
                    return PartialView("~/Areas/WMS/Views/ModelEntryMovements/_EditModelEntryMovementForm.cshtml", model);
                }
                catch (Exception ex)
                {
                    return Json(DbWMS.ErrorMessage(ex.Message));
                }
            }
            return PartialView("~/Areas/WMS/Views/ModelEntryMovements/_EditModelEntryMovementForm.cshtml", model);
        }
        #endregion

        #region Reterive Items
        public ActionResult ConfirmReceivingBulkModelsByCustdoianEmail(Guid id)
        {
            //ss
            List<ReleaseSingleItemUpdateModalUpdateModel> model = new List<ReleaseSingleItemUpdateModalUpdateModel>();
            var myFlow = DbWMS.dataItemOutputDetailFlow.Where(x => x.dataItemOutputDetail.dataItemInputDetail.LastCustdianNameGUID == id
                                                                   && x.Active
                                                                   && x.IsLastAction == true
                                                                   && x.FlowTypeGUID ==
                                                                   WarehouseRequestFlowType.PendingConfirmed).ToList();
            if (myFlow.Select(x => x.dataItemOutputDetail.dataItemOutput.RequesterGUID).FirstOrDefault() == WarehouseRequestSourceTypes.Staff)
            {
                if (myFlow.Select(x => x.dataItemOutputDetail.dataItemOutput.RequesterNameGUID).FirstOrDefault() != UserGUID)
                {
                    return Json(DbWMS.PermissionError());
                }
            }
            else if (myFlow.Select(x => x.dataItemOutputDetail.dataItemOutput.RequesterGUID).FirstOrDefault() == WarehouseRequestSourceTypes.Warehouse)
            {
                var focal = DbWMS.codeWarehouseFocalPoint.ToList().Where(x =>
                    x.WarehouseGUID == myFlow.Select(f => f.dataItemOutputDetail.dataItemOutput.RequesterNameGUID)
                        .FirstOrDefault()
                    && x.UserGUID == UserGUID).FirstOrDefault();
                if (focal == null)
                {
                    return Json(DbWMS.PermissionError());
                }
            }
            if (myFlow != null && myFlow.Count > 0)
            {
                var myFlowGuis = myFlow.Select(x => x.ItemOutputDetailGUID).ToList();

                var myDetail = DbWMS.dataItemOutputDetail.Where(x => x.dataItemInputDetail.LastCustdianNameGUID == id
                                                                     && myFlowGuis.Contains(x.ItemOutputDetailGUID)).ToList();

                var determinantModel = DbWMS.dataItemInputDeterminant
                    .ToList();

                foreach (var item in myDetail)
                {
                    ReleaseSingleItemUpdateModalUpdateModel mymodel = new ReleaseSingleItemUpdateModalUpdateModel();
                    mymodel.ItemInputDetailGUID = item.ItemInputDetailGUID;
                    //mymodel.ItemOutputGUID = id;
                    mymodel.RequesterNameGUID = id;
                    mymodel.ExpectedStartDate = item.ExpectedStartDate;
                    mymodel.ExpectedReturenedDate = item.ExpectedReturenedDate;
                    mymodel.IssuedBy = item.userAccounts.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).Select(x => x.FirstName).FirstOrDefault() + " " + item.userAccounts.userPersonalDetails.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN).Select(x => x.Surname).FirstOrDefault();
                    mymodel.WarehouseItemDescription = item.dataItemInputDetail.codeItemModelWarehouse.codeWarehouseItemModel
                        .codeWarehouseItem.codeWarehouseItemLanguage.Where(x => x.LanguageID == LAN).Select(x => x.WarehouseItemDescription)
                        .FirstOrDefault();
                    mymodel.BrandDescription = item.dataItemInputDetail.codeItemModelWarehouse.codeWarehouseItemModel
                        .codeBrand.codeBrandLanguage.Where(x => x.LanguageID == LAN).Select(x => x.BrandDescription)
                        .FirstOrDefault();

                    mymodel.ItemOutputDetailGUID = item.ItemOutputDetailGUID;
                    mymodel.ModelDescription = item.dataItemInputDetail.codeItemModelWarehouse.codeWarehouseItemModel
                        .codeWarehouseItemModelLanguage.Where(x => x.LanguageID == LAN).Select(x => x.ModelDescription)
                        .FirstOrDefault();
                    mymodel.BarcodeNumber = determinantModel
                        .Where(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.Barcode && x.ItemInputDetailGUID == item.ItemInputDetailGUID)
                        .Select(x => x.DeterminantValue).FirstOrDefault();
                    mymodel.SerialNumber = determinantModel
                        .Where(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.SerialNumber && x.ItemInputDetailGUID == item.ItemInputDetailGUID)
                        .Select(x => x.DeterminantValue).FirstOrDefault();
                    mymodel.GSM = determinantModel
                        .Where(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.GSM && x.ItemInputDetailGUID == item.ItemInputDetailGUID)
                        .Select(x => x.DeterminantValue).FirstOrDefault();
                    mymodel.IME1 = determinantModel
                        .Where(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.IME1 && x.ItemInputDetailGUID == item.ItemInputDetailGUID)
                        .Select(x => x.DeterminantValue).FirstOrDefault();
                    mymodel.GSM = determinantModel
                 .Where(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.GSM && x.ItemInputDetailGUID == item.ItemInputDetailGUID)
                 .Select(x => x.DeterminantValue).FirstOrDefault();
                    mymodel.MAC = determinantModel
        .Where(x => x.codeWarehouseItemModelDeterminant.DeterminantGUID == ItemDeterminants.MAC && x.ItemInputDetailGUID == item.ItemInputDetailGUID)
        .Select(x => x.DeterminantValue).FirstOrDefault();
                    mymodel.Validation = true;
                    model.Add(mymodel);


                }


                ViewBag.RequesterGUID = id;



                return View("~/Areas/WMS/Views/ModelConfirmation/ConfirmReceivingBulkModelsByCustdoianEmail.cshtml", model);
            }

            return View("~/Areas/WMS/Views/ModelConfirmation/ConfirmReceivingBulkModelsByCustdoianEmail.cshtml", model);

        }


        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult WarehouseModelEntryMovementsDataTableReminderPendingConfirmationBulkItems(List<dataItemInputDetail> models)
        {

            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            var guids = models.Select(f => f.ItemInputDetailGUID);
            var myModels = DbWMS.dataItemOutputDetailFlow.Where(x =>
                guids.Contains(x.dataItemOutputDetail.dataItemInputDetail.ItemInputDetailGUID)

                && x.IsLastAction == true
                && x.FlowTypeGUID == WarehouseRequestFlowType.PendingConfirmed).ToList();
            var EntryModel = DbWMS.dataItemInputDetail.Where(x => guids.Contains(x.ItemInputDetailGUID)).FirstOrDefault();
            if (myModels.Count > 0)
            {
                var myCustToConfirmGUIDs = myModels.Select(x => x.dataItemOutputDetail.dataItemInputDetail.LastCustdianNameGUID).Distinct().ToList();
                foreach (var item in myCustToConfirmGUIDs)
                {
                    SendEmailReminderForPendingConfirmationBulkItems((Guid)item);
                }




                return Json(DbWMS.SingleUpdateMessage(DataTableNames.WarehouseModelEntryMovementsDataTable, DbWMS.PrimaryKeyControl(EntryModel), DbWMS.RowVersionControls(Portal.SingleToList(EntryModel))));
            }

            try
            {

                return Json(DbWMS.ErrorMessage("Warrning:Reminder will just send  for pending confirmation records"));

            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage("no Error"));
            }
        }


        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult WarehouseModelEntryMovementsDataTableReminderCustodianBulkItems(List<dataItemInputDetail> models)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            var guids = models.Select(f => f.ItemInputDetailGUID);
            var myModels = DbWMS.dataItemOutputDetailFlow.Where(x =>
                guids.Contains(x.dataItemOutputDetail.dataItemInputDetail.ItemInputDetailGUID)

                && x.IsLastAction == true
                && x.FlowTypeGUID == WarehouseRequestFlowType.Confirmed).ToList();
            var EntryModel = DbWMS.dataItemInputDetail.Where(x => guids.Contains(x.ItemInputDetailGUID)).FirstOrDefault();
            if (myModels.Count > 0)
            {

                SendEmailReminderForBulkItems(myModels.Select(x => (Guid)x.ItemOutputDetailGUID).ToList());


                return Json(DbWMS.SingleUpdateMessage(DataTableNames.WarehouseModelEntryMovementsDataTable, DbWMS.PrimaryKeyControl(EntryModel), DbWMS.RowVersionControls(Portal.SingleToList(EntryModel))));
            }

            try
            {

                return Json(DbWMS.ErrorMessage("Warrning:Reminder will send for  confirmd items"));

            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage("no Error"));
            }
        }
        #region Retrieve Bulk Items
        [HttpGet]
        public ActionResult GetRetrieveBulkItems()
        {
            return PartialView("~/Areas/WMS/Views/ReleaseModelFlows/_RetrieveItems.cshtml", new ItemOutputDetailFlowModel());
        }
        [Route("WMS/ModelMovements/RetrieveBulkItems/")]
        [HttpPost]
        public ActionResult WarehouseModelEntryMovementsDataTableRetrieveBulkItems(List<Guid> models, ItemOutputDetailFlowModel test)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (models.Count > 0 && test.CreatedDate != null || test.ItemStatusGUID != null || test.ItemStatusGUID != Guid.Empty)
            {
                //foooo

                var guids = models;

                var myModels = DbWMS.dataItemOutputDetailFlow.Where(x =>
                    guids.Contains(x.dataItemOutputDetail.dataItemInputDetail.ItemInputDetailGUID)
                    && x.IsLastAction == true
                    && x.FlowTypeGUID == WarehouseRequestFlowType.Confirmed).ToList();
                var myItemOutputDetailGuids = myModels.Select(x => x.ItemOutputDetailGUID).ToList();
                //  List<dataItemOutputDetailFlow> allCurrentFlow = new List<dataItemOutputDetailFlow>();
                DateTime ExecutionTime = DateTime.Now;
                if (myModels.Count > 0)
                {
                    List<dataItemTransfer> transfers = new List<dataItemTransfer>();
                    var CurrentWarehouse = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID)
                          .FirstOrDefault();
                    var CurrentWarehouseGUID = CurrentWarehouse.WarehouseGUID;
                    var Warehousename = CurrentWarehouse.codeWarehouse.codeWarehouseLanguage.Select(f => f.WarehouseDescription).FirstOrDefault();
                    foreach (var myModel in myModels)
                    {
                        dataItemOutputDetail outputDetail = DbWMS.dataItemOutputDetail.Find(myModel.ItemOutputDetailGUID);
                        dataItemInputDetail input = DbWMS.dataItemInputDetail.Find(outputDetail.ItemInputDetailGUID);
                        int? order = DbWMS.dataItemOutputDetailFlow
                                         .Where(x => x.dataItemOutputDetail.ItemInputDetailGUID == outputDetail.ItemInputDetailGUID)
                                         .Select(x => x.OrderId).Max() + 1;
                        if (order == null)
                            order = 1;
                        //List<dataItemOutputDetailFlow> flows = DbWMS.dataItemOutputDetailFlow
                        //    .Where(x => x.ItemOutputDetailGUID == myModel.ItemOutputDetailGUID).ToList();
                        var myFlows = (from a in DbWMS.dataItemOutputDetailFlow.Where(x => x.ItemOutputDetailGUID == myModel.ItemOutputDetailGUID) select a).ToList();
                        myFlows.ForEach(x => x.IsLastAction = false);
                        myFlows.ForEach(x => x.IsLastMove = false);
                        DbWMS.UpdateBulk(myFlows, Permissions.WarehouseItemsRelease.CreateGuid, ExecutionTime, DbCMS);
                        var myoutputDetail = DbWMS.dataItemOutputDetail
                            .Where(x => x.ItemOutputDetailGUID == myModel.ItemOutputDetailGUID).FirstOrDefault();
                        //List<dataItemInputDetail> ItemInputs = DbWMS.dataItemInputDetail
                        //    .Where(x => x.ParentItemModelWarehouseGUID == myoutputDetail.ItemInputDetailGUID).ToList();
                        List<dataItemOutputDetail> childrenDetails = DbWMS.dataItemOutputDetail.Where(
                            x => x.dataItemInputDetail.ParentItemModelWarehouseGUID == myoutputDetail.ItemInputDetailGUID
                        ).ToList();
                        //
                        foreach (var myOutputChildren in childrenDetails.Where(x => x.ItemOutputDetailGUID != myModel.ItemOutputDetailGUID))
                        {
                            List<dataItemOutputDetailFlow> myflowchildren = DbWMS.dataItemOutputDetailFlow
                                .Where(x => x.ItemOutputDetailGUID == myOutputChildren.ItemOutputDetailGUID).ToList();
                            myflowchildren.ForEach(x => x.IsLastAction = false);
                            myflowchildren.ForEach(x => x.IsLastMove = false);
                            //foreach (var flowchildren in myflows)
                            //{
                            //    flowchildren.IsLastAction = false;
                            //    flowchildren.IsLastMove = false;
                            //}

                            DbWMS.SaveChanges();
                            int? myorder = DbWMS.dataItemOutputDetailFlow
                                             .Where(x => x.dataItemOutputDetail.ItemInputDetailGUID == myOutputChildren.ItemInputDetailGUID)
                                             .Select(x => x.OrderId).Max() + 1;
                            if (myorder == null)
                                myorder = 1;
                            dataItemOutputDetailFlow myFlowDetailFlow = new dataItemOutputDetailFlow();
                            myFlowDetailFlow.ItemOutputDetailFlowGUID = Guid.NewGuid();
                            myFlowDetailFlow.ItemOutputDetailGUID = myOutputChildren.ItemOutputDetailGUID;
                            myFlowDetailFlow.FlowTypeGUID = WarehouseRequestFlowType.Returned;
                            myFlowDetailFlow.CreatedDate = ExecutionTime;
                            myFlowDetailFlow.ItemStatuGUID = test.ItemStatusGUID;
                            myFlowDetailFlow.IsLastAction = true;
                            myFlowDetailFlow.IsLastMove = true;
                            myFlowDetailFlow.OrderId = myorder;
                            myFlowDetailFlow.CreatedByGUID = UserGUID;
                            myFlowDetailFlow.Active = true;
                            DbWMS.CreateNoAudit(myFlowDetailFlow);

                            dataItemOutputDetail itemOutputDetailChildren = DbWMS.dataItemOutputDetail
                                .Where(x => x.ItemOutputDetailGUID == myOutputChildren.ItemOutputDetailGUID).FirstOrDefault();

                            itemOutputDetailChildren.ActualReturenedDate = ExecutionTime;

                            itemOutputDetailChildren.RetunedByGUID = UserGUID;
                            itemOutputDetailChildren.ReturnedDate = ExecutionTime;


                            DbWMS.Update(itemOutputDetailChildren, Permissions.WarehouseItemsRelease.UpdateGuid, ExecutionTime, DbCMS);



                            var myCurrentWarehouseGUID = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).Select(x => x.WarehouseGUID)
                                .FirstOrDefault();

                            dataItemInputDetail inputDetail = DbWMS.dataItemInputDetail
                                    .Where(x => x.ItemInputDetailGUID == itemOutputDetailChildren.ItemInputDetailGUID).FirstOrDefault();
                            inputDetail.IsAvaliable = true;
                            inputDetail.ItemServiceStatusGUID = WarehouseServiceItemStatus.InStock;
                            inputDetail.ItemStatusGUID = (Guid)test.ItemStatusGUID;
                            inputDetail.LastCustdianGUID = WarehouseRequestSourceTypes.Warehouse;
                            //inputDetail.LastLocationGUID = ;
                            inputDetail.LastCustdianNameGUID = myCurrentWarehouseGUID;
                            inputDetail.LastFlowTypeGUID = WarehouseRequestFlowType.Returned;
                            DbWMS.Update(inputDetail, Permissions.WarehouseItemsRelease.UpdateGuid, ExecutionTime, DbCMS);





                        }

                        dataItemOutputDetailFlow myCurrentFlow = new dataItemOutputDetailFlow();
                        myCurrentFlow.ItemOutputDetailFlowGUID = Guid.NewGuid();
                        myCurrentFlow.ItemOutputDetailGUID = myModel.ItemOutputDetailGUID;
                        myCurrentFlow.FlowTypeGUID = WarehouseRequestFlowType.Returned;
                        myCurrentFlow.CreatedDate = ExecutionTime;
                        myCurrentFlow.ItemStatuGUID = test.ItemStatusGUID;
                        myCurrentFlow.IsLastAction = true;
                        myCurrentFlow.IsLastMove = true;
                        myCurrentFlow.CreatedByGUID = UserGUID;
                        myCurrentFlow.Active = true;
                        myCurrentFlow.OrderId = order;

                        DbWMS.Create(myCurrentFlow, Permissions.WarehouseItemsRelease.CreateGuid, ExecutionTime, DbCMS);


                        outputDetail.ActualReturenedDate = test.CreatedDate;
                        outputDetail.Comments = test.Comments;


                        if ((input.LastWarehouseLocationGUID != test.WarehouseLocationGUID))
                        {
                            List<dataItemTransfer> allPriviousTransfers = DbWMS.dataItemTransfer
                                .Where(x => x.ItemInputDetailGUID == input.ItemInputDetailGUID).ToList();
                            var myCurrentWarehouseGUID = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).Select(x => x.WarehouseGUID)
                                .FirstOrDefault();
                            List<dataItemTransfer> toAddtrans = new List<dataItemTransfer>();
                            foreach (var tran in allPriviousTransfers)
                            {
                                tran.IsLastTransfer = false;
                            }
                            DbWMS.UpdateBulk(allPriviousTransfers, Permissions.WarehouseItemsEntry.UpdateGuid, ExecutionTime, DbCMS);
                            var warehouse = DbWMS.codeWarehouse.Where(x => x.WarehouseLocationGUID == test.WarehouseLocationGUID).FirstOrDefault();
                            dataItemTransfer changeTransferModel = new dataItemTransfer();
                            changeTransferModel.ItemTransferGUID = Guid.NewGuid();
                            changeTransferModel.ItemInputDetailGUID = input.ItemInputDetailGUID;
                            changeTransferModel.TransferDate = ExecutionTime;
                            changeTransferModel.SourceGUID = myCurrentWarehouseGUID;
                            changeTransferModel.DestionationGUID = warehouse.WarehouseGUID;
                            changeTransferModel.TransferedByGUID = UserGUID;
                            changeTransferModel.IsLastTransfer = true;
                            DbWMS.Create(changeTransferModel, Permissions.WarehouseItemsEntry.UpdateGuid, ExecutionTime, DbCMS);
                        }
                        outputDetail.Comments = test.Comments;
                        outputDetail.RetunedByGUID = UserGUID;
                        outputDetail.ReturnedDate = test.CreatedDate;
                        outputDetail.ItemStatuOnReturnGUID = test.ItemStatusGUID;
                        outputDetail.ActualReturenedDate = test.CreatedDate;
                        DbWMS.Update(outputDetail, Permissions.WarehouseItemsRelease.UpdateGuid, ExecutionTime, DbCMS);

                        input.IsAvaliable = true;

                        input.ItemServiceStatusGUID = WarehouseServiceItemStatus.InStock;
                        input.LastCustdianGUID = WarehouseRequestSourceTypes.Warehouse;
                        input.LastCustdianNameGUID = CurrentWarehouseGUID;
                        input.LastFlowTypeGUID = WarehouseRequestFlowType.Returned;
                        input.ItemStatusGUID = (Guid)test.ItemStatusGUID;
                        input.LastCustodianName = Warehousename;
                        DbWMS.Update(input, Permissions.WarehouseItemsRelease.UpdateGuid, ExecutionTime, DbCMS);
                        DbWMS.SaveChanges();
                        DbCMS.SaveChanges();
                    }
                    var EntryModel = DbWMS.dataItemInputDetail.Where(x => guids.Contains(x.ItemInputDetailGUID)).FirstOrDefault();
                    try
                    {


                        if (myItemOutputDetailGuids.Count > 0)
                        {
                            SendConfirmReteriveBulkItems(myItemOutputDetailGuids);
                        }
                        var _checkStaffIssuer = DbAHD.StaffCoreData.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
                        if (_checkStaffIssuer != null)
                        {
                            DbWMS.sp_UpdatePhonesToHR();
                        }


                        return RedirectToAction("ConfirmItemUpdate");

                    }
                    catch (Exception ex)
                    {
                        return Json(DbWMS.ErrorMessage("no Error"));
                    }

                }
                return Json(DbWMS.ErrorMessage("Warnning:Chose items to reterive and fill required fields"));
            }
            return Json(DbWMS.ErrorMessage("Warnning:Some of the records  not confirmed"));


        }
        #endregion
        #endregion

        #region Verification item(s)

        public ActionResult GetVerificationPeriodPerSite(Guid PK)
        {
            WarehouseItemVerficationPeriodModel model = new WarehouseItemVerficationPeriodModel { ItemVerificationPeriodGUID = PK };
            return PartialView("~/Areas/WMS/Views/ItemVerificationPeriod/_VerificationPeriodPerSite.cshtml", model);
        }



        public ActionResult WarehouseTrackModelVerifiationPerSite(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/WMS/Views/ModelReleaseMovements/_VerificationPeriodPerSite.cshtml", new WarehouseItemVerficationPeriodModel { ItemVerificationPeriodGUID = PK });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<TrackWarehouseVeriifationItem, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<TrackWarehouseVeriifationItem>(DataTable.Filters);
            }
            Guid orgGuid = (Guid)Session[SessionKeys.OrganizationInstanceGUID];
            var v_EntryMovementDataTables = DbWMS.v_EntryMovementDataTable.Where(x => x.IsDeterminanted == true && x.OrganizationInstanceGUID == orgGuid).AsQueryable();
            var Result = (

                    from a in DbWMS.dataItemVerificationWarehousePeriod.AsNoTracking().AsExpandable().Where(x => x.ItemVerificationPeriodGUID == PK)
                    join b in DbWMS.codeWarehouseLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.WarehouseGUID equals b.WarehouseGUID into LJ1
                    from R1 in LJ1.DefaultIfEmpty()
                        //join c in DbWMS.v_EntryMovementDataTable.Where(x=>x.IsDeterminanted==true && x.Active==true) on a.WarehouseGUID equals c.WarehouseOwnerGUID into LJ2 
                        //from R2 in LJ2.DefaultIfEmpty()

                    select new TrackWarehouseVeriifationItem
                    {
                        VerificationWarehousePeriodGUID = (Guid)a.VerificationWarehousePeriodGUID,
                        WarehouseName = R1.WarehouseDescription,
                        TotalItem = v_EntryMovementDataTables.Where(x => x.WarehouseOwnerGUID == R1.WarehouseGUID).Select(x => x.ItemInputDetailGUID).Distinct().Count(),
                        TotalItemVerified = v_EntryMovementDataTables.Where(x => x.WarehouseOwnerGUID == R1.WarehouseGUID && x.VerificationStatusGUID == ItemVerificationStatus.Confirmed).Select(x => x.ItemInputDetailGUID).Distinct().Count(),
                        TotalItemNotVerified = v_EntryMovementDataTables.Where(x => x.WarehouseOwnerGUID == R1.WarehouseGUID && x.VerificationStatusGUID == ItemVerificationStatus.Pending).Select(x => x.ItemInputDetailGUID).Distinct().Count(),
                        dataItemVerificationWarehousePeriodVersion = a.dataItemVerificationWarehousePeriodRowVersion,

                    }).Where(Predicate);



            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }


        public ActionResult WarehouseItemVerficationDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/WMS/Views/ModelMovements/_DamagedModelsDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<WarehouseItemVerficationDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<WarehouseItemVerficationDataTableModel>(DataTable.Filters);
            }

            var All = (

                from a in DbWMS.dataItemInputDetail.AsNoTracking().AsExpandable().Where(x => x.Active && x.ItemInputDetailGUID == PK)
                join b in DbWMS.dataItemVerificationPeriodDetail.Where(x => x.Active) on a.ItemInputDetailGUID equals b.ItemInputDetailGUID
                join c in DbWMS.dataItemVerificationWarehousePeriod on b.VerificationWarehousePeriodGUID equals c.VerificationWarehousePeriodGUID

                join d in DbWMS.dataItemVerificationPeriod on c.ItemVerificationPeriodGUID equals d.ItemVerificationPeriodGUID into LJ1
                from R1 in LJ1.DefaultIfEmpty()

                join e in DbWMS.codeWarehouseLanguage.Where(x => x.LanguageID == LAN) on c.WarehouseGUID equals e.WarehouseGUID into LJ2
                from R2 in LJ2.DefaultIfEmpty()
                join f in DbWMS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN) on b.CreateByGUID equals f.UserGUID into LJ3
                from R3 in LJ3.DefaultIfEmpty()


                select new WarehouseItemVerficationDataTableModel
                {
                    VerificationPeriodName = R1.VerificationPeriodName,
                    OrderId = R1.OrderId,
                    ItemVerificationPeriodDetailGUID = b.ItemVerificationPeriodDetailGUID.ToString(),
                    IsLastPeriod = R1.IsLastPeriod,
                    IsClosed = R1.IsClosed,
                    WarehouseName = R2.WarehouseDescription,
                    VerifyDate = b.VerifyDate,
                    VerifyBy = R3.FirstName + " " + R3.Surname,
                    Active = a.Active,
                    Comments = a.Comments,
                    dataItemVerificationPeriodDetailVersion = b.dataItemVerificationPeriodDetailRowVersion




                }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<WarehouseItemVerficationDataTableModel> Result = Mapper.Map<List<WarehouseItemVerficationDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result.OrderByDescending(x => x.OrderId)), JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult GetVerificationItem()
        {
            return PartialView("~/Areas/WMS/Views/VerificationItem/_VerificationItems.cshtml", new ItemInputVerificationModel());
        }

        [Route("WMS/ModelMovements/VerificationItem/")]
        [HttpPost]
        public ActionResult WarehouseModelEntryMovementsDataTableVerificationItem(List<Guid> models, ItemInputVerificationModel test)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (models.Count > 0 && test.CreateDate != null)
            {
                var verificationPeriod = DbWMS.dataItemVerificationPeriod.Where(x => x.IsLastPeriod == true).FirstOrDefault();
                if (verificationPeriod == null)
                    return Json(DbWMS.ErrorMessage("Warnning:No open verification period"));
                DateTime ExecutionTime = DateTime.Now;
                foreach (var myModel in models)
                {
                    var inputdet = DbWMS.dataItemInputDetail.Where(x => x.ItemInputDetailGUID == myModel).FirstOrDefault();
                    inputdet.LastVerifiedDate = test.CreateDate;
                    inputdet.LastVerifiedByGUID = UserGUID;
                    inputdet.IsVerified = true;
                    inputdet.VerificationStatusGUID = ItemVerificationStatus.Confirmed;
                    inputdet.Comments = String.Join(inputdet.Comments, ", ", test.Comments);

                    var verwarePeriod = DbWMS.dataItemVerificationWarehousePeriod.Where(x => x.WarehouseGUID == inputdet.WarehouseOwnerGUID).FirstOrDefault();

                    dataItemVerificationPeriodDetail periodDetail = new dataItemVerificationPeriodDetail
                    {
                        ItemVerificationPeriodDetailGUID = Guid.NewGuid(),
                        VerificationWarehousePeriodGUID = verwarePeriod.VerificationWarehousePeriodGUID,
                        ItemInputDetailGUID = myModel,
                        VerifyDate = ExecutionTime,
                        CreateByGUID = UserGUID,
                        Comments = test.Comments,


                    };

                    try
                    {
                        DbWMS.Update(inputdet, Permissions.WarehouseItemsRelease.UpdateGuid, ExecutionTime, DbCMS);
                        DbWMS.Create(periodDetail, Permissions.WarehouseItemsRelease.CreateGuid, ExecutionTime, DbCMS);

                        DbWMS.SaveChanges();
                        DbCMS.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        return Json(DbWMS.ErrorMessage(ex.Message));
                    }


                }
                return RedirectToAction("ConfirmItemUpdate");
            }
            else
                return Json(DbWMS.ErrorMessage("Warnning:Chose items to reterive and fill required fields"));




        }
        #endregion
        #region Change Item Status For Disposal
        [HttpGet]
        public ActionResult GetChangeBulkItemStatus()
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }

            ItemBulkStatusVM model = new ItemBulkStatusVM
            {

                ItemInputDetailGUID = Guid.Empty,

            };

            return PartialView("~/Areas/WMS/Views/BulkItemStatus/_ChangeBulkItem.cshtml",
                model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ChangeBulkItemStatusCreate(ItemBulkStatusVM model)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }

            if (model.ItemInputModelGuids == null || model.ItemInputModelGuids.Count == 0 || model.ItemStatusGUID == null)
            {
                ModelState.AddModelError("Release Description", "Please provide item status and items to change  ");
                return PartialView("~/Areas/WMS/Views/BulkItemStatus/_ChangeBulkItem.cshtml",
                   model);

            }

            DateTime ExecutionTime = DateTime.Now;
            foreach (var item in model.ItemInputModelGuids)
            {

                var InputDetail = DbWMS.dataItemInputDetail.Where(x => x.ItemInputDetailGUID == item).FirstOrDefault();
                InputDetail.ItemStatusGUID = model.ItemStatusGUID;
                DbWMS.Update(InputDetail, Permissions.WarehouseItemsRelease.UpdateGuid, ExecutionTime, DbCMS);
            }


            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                dataItemInputDetail EntryModel = DbWMS.dataItemInputDetail.Where(x => x.ItemInputDetailGUID == model.ItemInputModelGuids.FirstOrDefault()).FirstOrDefault();

                return Json(DbWMS.SingleUpdateMessage(DataTableNames.WarehouseModelEntryMovementsDataTable, DbWMS.PrimaryKeyControl(EntryModel), DbWMS.RowVersionControls(Portal.SingleToList(EntryModel))));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }

        }








        #endregion

        #region Transfer Items between custodians
        [HttpGet]
        public ActionResult GetExchnageBulkItems()
        {
            return PartialView("~/Areas/WMS/Views/ReleaseModelFlows/_ExchnageBulkItems.cshtml", new ReleaseSingleItemUpdateModalUpdateModel());
        }

        [Route("WMS/ModelMovements/ExchnageBulkItems/")]

        [HttpPost]
        public ActionResult WarehouseModelEntryMovementsDataTableExchnageBulkItems(List<Guid> models, ReleaseSingleItemUpdateModalUpdateModel test)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }

            if (models.Count > 0 && test.RequesterGUID != null && test.RequesterNameGUID != null && test.WarehouseLocationGUID != null)
            {
                var guids = models;


                var myModels = DbWMS.dataItemOutputDetailFlow.Where(x =>
                    guids.Contains(x.dataItemOutputDetail.dataItemInputDetail.ItemInputDetailGUID)

                    && x.IsLastAction == true
                    && x.IsLastMove == true
                    && (x.FlowTypeGUID == WarehouseRequestFlowType.Confirmed
                    || x.FlowTypeGUID == WarehouseRequestFlowType.Returned

                    )).ToList();
                var myItemOutputDetailGuids = myModels.Select(x => x.ItemOutputDetailGUID).ToList();
                DateTime ExecutionTime = DateTime.Now;
                if (myModels.Count > 0)
                {
                    #region Retertive items
                    foreach (var myModel in myModels)
                    {
                        dataItemOutputDetail outputDetail = DbWMS.dataItemOutputDetail.Find(myModel.ItemOutputDetailGUID);
                        dataItemInputDetail input = DbWMS.dataItemInputDetail.Find(outputDetail.ItemInputDetailGUID);
                        int? order = DbWMS.dataItemOutputDetailFlow
                                         .Where(x => x.dataItemOutputDetail.ItemInputDetailGUID == outputDetail.ItemInputDetailGUID)
                                         .Select(x => x.OrderId).Max() + 1;
                        if (order == null)
                            order = 1;
                        //List<dataItemOutputDetailFlow> flows = DbWMS.dataItemOutputDetailFlow
                        //    .Where(x => x.ItemOutputDetailGUID == myModel.ItemOutputDetailGUID).ToList();
                        var myFlows = (from a in DbWMS.dataItemOutputDetailFlow.Where(x => x.ItemOutputDetailGUID == myModel.ItemOutputDetailGUID) select a).ToList();
                        myFlows.ForEach(x => x.IsLastAction = false);
                        //check
                        myFlows.ForEach(x => x.IsLastMove = false);

                        var myoutputDetail = DbWMS.dataItemOutputDetail
                            .Where(x => x.ItemOutputDetailGUID == myModel.ItemOutputDetailGUID).FirstOrDefault();
                        List<dataItemOutputDetail> childrenDetails = DbWMS.dataItemOutputDetail.Where(
                               x => x.dataItemInputDetail.ParentItemModelWarehouseGUID == myoutputDetail.ItemInputDetailGUID
                           ).ToList();

                        dataItemOutputDetailFlow myCurrentFlow = new dataItemOutputDetailFlow();
                        myCurrentFlow.ItemOutputDetailFlowGUID = Guid.NewGuid();
                        myCurrentFlow.ItemOutputDetailGUID = myModel.ItemOutputDetailGUID;
                        myCurrentFlow.FlowTypeGUID = WarehouseRequestFlowType.Returned;
                        myCurrentFlow.CreatedDate = ExecutionTime;
                        myCurrentFlow.ItemStatuGUID = WarehouseItemStatus.Functionting;
                        myCurrentFlow.IsLastAction = false;
                        myCurrentFlow.IsLastMove = false;
                        myCurrentFlow.CreatedByGUID = UserGUID;
                        myCurrentFlow.Active = true;
                        myCurrentFlow.OrderId = order;
                        DbWMS.Create(myCurrentFlow, Permissions.WarehouseItemsRelease.CreateGuid, ExecutionTime, DbCMS);

                        if (test.CreatedDate != null)
                        {
                            outputDetail.ActualReturenedDate = test.CreatedDate;
                        }
                        else
                            outputDetail.ActualReturenedDate = ExecutionTime;

                        outputDetail.Comments = test.Comments;
                        if ((input.LastWarehouseLocationGUID != test.WarehouseLocationGUID))
                        {
                            List<dataItemTransfer> allPriviousTransfers = DbWMS.dataItemTransfer
                                .Where(x => x.ItemInputDetailGUID == input.ItemInputDetailGUID).ToList();


                            foreach (var tran in allPriviousTransfers)
                            {
                                tran.IsLastTransfer = false;
                            }
                            var mycurrentWarhouseGUID = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).Select(x => x.WarehouseGUID)
                                        .FirstOrDefault();
                            DbWMS.UpdateBulk(allPriviousTransfers, Permissions.WarehouseItemsEntry.UpdateGuid, ExecutionTime, DbCMS);
                            var warehouse = DbWMS.codeWarehouse.Where(x => x.WarehouseLocationGUID == test.WarehouseLocationGUID).FirstOrDefault();
                            dataItemTransfer changeTransferModel = new dataItemTransfer();
                            changeTransferModel.ItemTransferGUID = Guid.NewGuid();
                            changeTransferModel.ItemInputDetailGUID = input.ItemInputDetailGUID;
                            changeTransferModel.TransferDate = ExecutionTime;
                            changeTransferModel.SourceGUID = mycurrentWarhouseGUID;
                            changeTransferModel.DestionationGUID = warehouse.WarehouseGUID;
                            changeTransferModel.TransferedByGUID = UserGUID;
                            changeTransferModel.IsLastTransfer = true;
                            DbWMS.Create(changeTransferModel, Permissions.WarehouseItemsEntry.UpdateGuid, ExecutionTime, DbCMS);
                        }
                        outputDetail.Comments = test.Comments;
                        outputDetail.RetunedByGUID = UserGUID;
                        outputDetail.ReturnedDate = ExecutionTime;
                        DbWMS.Update(outputDetail, Permissions.WarehouseItemsRelease.UpdateGuid, ExecutionTime, DbCMS);
                        var CurrentWarehouseGUID = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).Select(x => x.WarehouseGUID)
                            .FirstOrDefault();
                        input.IsAvaliable = true;
                        input.ItemServiceStatusGUID = WarehouseServiceItemStatus.InStock;
                        input.LastCustdianGUID = WarehouseRequestSourceTypes.Warehouse;
                        input.LastCustdianNameGUID = CurrentWarehouseGUID;
                        input.LastFlowTypeGUID = WarehouseRequestFlowType.Returned;
                        DbWMS.Update(input, Permissions.WarehouseItemsRelease.UpdateGuid, ExecutionTime, DbCMS);
                        try
                        {
                            DbWMS.SaveChanges();
                            DbCMS.SaveChanges();

                        }
                        catch (Exception)
                        {

                            throw;
                        }

                    }

                    #endregion

                    #region exchnage items
                    Guid EntityPK = Guid.NewGuid();

                    dataItemOutput ReleaseModel = new dataItemOutput();
                    ReleaseModel.ItemOutputGUID = EntityPK;
                    ReleaseModel.CreatedByGUID = UserGUID;
                    ReleaseModel.CreatedDate = ExecutionTime;
                    ReleaseModel.RequesterNameGUID = test.RequesterNameGUID;
                    ReleaseModel.RequesterGUID = test.RequesterGUID;

                    ReleaseModel.OutputNumber = DbWMS.dataItemOutput.Select(x => x.OutputNumber).Max() + 1 ?? 1;
                    DbWMS.Create(ReleaseModel, Permissions.WarehouseItemsRelease.CreateGuid, ExecutionTime, DbCMS);

                    List<dataItemOutputDetail> itemReleaseDetailsToAdd = new List<dataItemOutputDetail>();
                    var myCurrentWarehouseGUID = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).Select(x => x.WarehouseGUID)
                        .FirstOrDefault();


                    foreach (var item in models)
                    {
                        #region main Item
                        dataItemOutputDetail ReleaseModelDetail = new dataItemOutputDetail();
                        ReleaseModelDetail.ItemOutputDetailGUID = Guid.NewGuid();
                        if (ReleaseModelDetail.RequestedQunatity == null)
                            ReleaseModelDetail.RequestedQunatity = 1;
                        //if (model.ExpectedStartDate == null)
                        //{
                        //    ReleaseModelDetail.ExpectedStartDate = DateTime.Now;
                        //}

                        if (test.ItemRequestTypeGUID == null)
                        {
                            ReleaseModelDetail.ItemRequestTypeGUID = LookupTables.ItemRequestTypeLongTerm;
                        }
                        else
                            ReleaseModelDetail.ItemRequestTypeGUID = test.ItemRequestTypeGUID;
                        ReleaseModelDetail.CreatedByGUID = UserGUID;
                        ReleaseModelDetail.ItemInputDetailGUID = item;
                        ReleaseModelDetail.ItemOutputGUID = ReleaseModel.ItemOutputGUID;
                        ReleaseModelDetail.CreatedDate = ExecutionTime;
                        ReleaseModelDetail.RequestedQunatity = 1;

                        ReleaseModelDetail.Comments = ReleaseModel.Comments;
                        ReleaseModelDetail.ExpectedStartDate = test.ExpectedStartDate;
                        ReleaseModelDetail.ExpectedReturenedDate = test.ExpectedReturenedDate;
                        ReleaseModelDetail.WarehouseLocationGUID = test.WarehouseLocationGUID;
                        itemReleaseDetailsToAdd.Add(ReleaseModelDetail);


                        dataItemInputDetail InputDetail = DbWMS.dataItemInputDetail.Find(item);
                        //dataItemOutput itemOutput = DbWMS.dataItemOutput.Find(ReleaseModelDetail.ItemOutputGUID);
                        if (ReleaseModel.RequesterGUID != WarehouseRequestSourceTypes.Warehouse)
                        {
                            InputDetail.IsAvaliable = false;
                            InputDetail.ItemServiceStatusGUID = WarehouseServiceItemStatus.InService;
                        }

                        InputDetail.LastCustdianGUID = (Guid)ReleaseModel.RequesterGUID;
                        InputDetail.LastCustdianNameGUID = ReleaseModel.RequesterNameGUID;
                        InputDetail.LastWarehouseLocationGUID = test.WarehouseLocationGUID;

                        InputDetail.LastCustodianName = CheckCustodianName((Guid)ReleaseModel.RequesterGUID, (Guid)ReleaseModel.RequesterNameGUID);


                        var Chcektransfer = DbWMS.dataItemTransfer.Where(x => x.ItemInputDetailGUID == InputDetail.ItemInputDetailGUID
                                                                              && x.IsLastTransfer == true).FirstOrDefault();



                        if (ReleaseModel.RequesterGUID == WarehouseRequestSourceTypes.OtherRequester
                            || ReleaseModel.RequesterGUID == WarehouseRequestSourceTypes.Vehicle
                            || ReleaseModel.RequesterGUID == WarehouseRequestSourceTypes.Partner
                            || ReleaseModel.RequesterGUID == WarehouseRequestSourceTypes.GS45)
                        {
                            InputDetail.LastFlowTypeGUID = WarehouseRequestFlowType.Confirmed;
                        }
                        else
                        {
                            InputDetail.LastFlowTypeGUID = WarehouseRequestFlowType.PendingConfirmed;

                        }
                        DbWMS.Update(InputDetail, Permissions.WarehouseItemsRelease.UpdateGuid, ExecutionTime, DbCMS);
                        List<dataItemOutputDetailFlow> flows = DbWMS.dataItemOutputDetailFlow
                            .Where(x => x.dataItemOutputDetail.dataItemInputDetail.ItemInputDetailGUID == item).ToList();
                        var flowDetailsPriv = (from a in DbWMS.dataItemOutputDetailFlow.Where(x => x.dataItemOutputDetail.dataItemInputDetail.ItemInputDetailGUID == InputDetail.ItemInputDetailGUID) select a).ToList();
                        flowDetailsPriv.ForEach(x => x.IsLastAction = false);
                        int? order = DbWMS.dataItemOutputDetailFlow
                             .Where(x => x.dataItemOutputDetail.ItemInputDetailGUID == item)
                             .Select(x => x.OrderId).Max() + 1;
                        if (order == null)
                            order = 1;
                        dataItemOutputDetailFlow outputDetailFlow = new dataItemOutputDetailFlow();

                        outputDetailFlow.ItemOutputDetailFlowGUID = Guid.NewGuid();
                        outputDetailFlow.ItemOutputDetailGUID = ReleaseModelDetail.ItemOutputDetailGUID;
                        if (ReleaseModel.RequesterGUID == WarehouseRequestSourceTypes.OtherRequester
                            || ReleaseModel.RequesterGUID == WarehouseRequestSourceTypes.Vehicle
                            || ReleaseModel.RequesterGUID == WarehouseRequestSourceTypes.Partner

                            || ReleaseModel.RequesterGUID == WarehouseRequestSourceTypes.GS45)
                        {
                            outputDetailFlow.FlowTypeGUID = WarehouseRequestFlowType.Confirmed;
                        }
                        else
                        {
                            outputDetailFlow.FlowTypeGUID = WarehouseRequestFlowType.PendingConfirmed;
                        }


                        outputDetailFlow.ItemStatuGUID = WarehouseItemStatus.Functionting;
                        outputDetailFlow.CreatedDate = DateTime.Now;
                        outputDetailFlow.IsLastAction = true;
                        outputDetailFlow.IsLastMove = true;
                        outputDetailFlow.CreatedByGUID = UserGUID;
                        outputDetailFlow.Active = true;
                        outputDetailFlow.OrderId = order;
                        DbWMS.Create(outputDetailFlow, Permissions.WarehouseItemsEntry.CreateGuid, ExecutionTime, DbCMS);
                        if (ReleaseModel.RequesterGUID == WarehouseRequestSourceTypes.Warehouse)
                        {
                            var CurrentWarehouseGUID = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).Select(x => x.WarehouseGUID)
                                .FirstOrDefault();


                            #endregion

                            #region Children
                            List<dataItemInputDetail> ChildreninputDetails = DbWMS.dataItemInputDetail.Where(x => x.ParentItemModelWarehouseGUID == item).ToList();
                            List<dataItemOutputDetail> childrenDetails = new List<dataItemOutputDetail>();
                            foreach (var children in ChildreninputDetails)
                            {
                                dataItemOutputDetail myChildrendOutputDetail = new dataItemOutputDetail();
                                myChildrendOutputDetail.ItemOutputDetailGUID = Guid.NewGuid();
                                if (myChildrendOutputDetail.RequestedQunatity == null)
                                    myChildrendOutputDetail.RequestedQunatity = 1;
                                myChildrendOutputDetail.CreatedByGUID = UserGUID;
                                myChildrendOutputDetail.ExpectedStartDate = test.ExpectedStartDate;
                                myChildrendOutputDetail.WarehouseLocationGUID = test.WarehouseLocationGUID;
                                myChildrendOutputDetail.ItemOutputGUID = ReleaseModel.ItemOutputGUID;
                                myChildrendOutputDetail.CreatedDate = ExecutionTime;
                                myChildrendOutputDetail.Comments = ReleaseModel.Comments;
                                myChildrendOutputDetail.ItemInputDetailGUID = children.ItemInputDetailGUID;
                                childrenDetails.Add(myChildrendOutputDetail);

                                if (ReleaseModel.RequesterGUID != WarehouseRequestSourceTypes.Warehouse)
                                {
                                    children.IsAvaliable = false;
                                    children.ItemServiceStatusGUID = WarehouseServiceItemStatus.InService;
                                }


                                children.LastCustdianGUID = (Guid)ReleaseModel.RequesterGUID;
                                //children.LastLocationGUID = InputDetail.LastLocationGUID;
                                children.LastWarehouseLocationGUID = InputDetail.LastWarehouseLocationGUID;
                                children.LastCustdianNameGUID = ReleaseModel.RequesterNameGUID;
                                children.LastCustodianName = CheckCustodianName((Guid)ReleaseModel.RequesterGUID, (Guid)ReleaseModel.RequesterNameGUID);
                                if (ReleaseModel.RequesterGUID == WarehouseRequestSourceTypes.OtherRequester
                                    || ReleaseModel.RequesterGUID == WarehouseRequestSourceTypes.Vehicle
                                    || ReleaseModel.RequesterGUID == WarehouseRequestSourceTypes.Partner
                                    || ReleaseModel.RequesterGUID == WarehouseRequestSourceTypes.GS45)
                                {
                                    children.LastFlowTypeGUID = WarehouseRequestFlowType.Confirmed;
                                }
                                else
                                {
                                    //if (model.ExpectedStartDate < DateTime.Now)
                                    //{
                                    //    children.LastFlowTypeGUID = WarehouseRequestFlowType.Confirmed;
                                    //    }
                                    //else
                                    //{
                                    children.LastFlowTypeGUID = WarehouseRequestFlowType.PendingConfirmed;

                                    //}
                                }
                                //children.LastFlowTypeGUID = WarehouseRequestFlowType.Confirmed;

                                DbWMS.Update(children, Permissions.WarehouseItemsRelease.UpdateGuid, ExecutionTime, DbCMS);


                                List<dataItemOutputDetailFlow> myChldrenflows = DbWMS.dataItemOutputDetailFlow
                                    .Where(x => x.dataItemOutputDetail.dataItemInputDetail.ItemInputDetailGUID == children.ItemInputDetailGUID).ToList();


                                var privFlows = (from a in DbWMS.dataItemOutputDetailFlow.Where(x => x.dataItemOutputDetail.dataItemInputDetail.ItemInputDetailGUID == children.ItemInputDetailGUID) select a).ToList();
                                privFlows.ForEach(x => x.IsLastAction = false);

                                //foreach (var flow in myChldrenflows)
                                //{
                                //    flow.IsLastAction = false;

                                //}
                                //DbWMS.UpdateBulk(myChldrenflows, Permissions.WarehouseItemsEntry.UpdateGuid, ExecutionTime, DbCMS);
                                int? myOrder = DbWMS.dataItemOutputDetailFlow
                                                 .Where(x => x.dataItemOutputDetail.ItemInputDetailGUID == children.ItemInputDetailGUID)
                                                 .Select(x => x.OrderId).Max() + 1;
                                if (myOrder == null)
                                    myOrder = 1;
                                dataItemOutputDetailFlow myoutputDetailFlow = new dataItemOutputDetailFlow();

                                myoutputDetailFlow.ItemOutputDetailFlowGUID = Guid.NewGuid();
                                myoutputDetailFlow.ItemOutputDetailGUID = myChildrendOutputDetail.ItemOutputDetailGUID;
                                if (ReleaseModel.RequesterGUID == WarehouseRequestSourceTypes.OtherRequester
                                    || ReleaseModel.RequesterGUID == WarehouseRequestSourceTypes.Vehicle
                                    || ReleaseModel.RequesterGUID == WarehouseRequestSourceTypes.Partner
                                    || ReleaseModel.RequesterGUID == WarehouseRequestSourceTypes.GS45)
                                {
                                    myoutputDetailFlow.FlowTypeGUID = WarehouseRequestFlowType.Confirmed;
                                }
                                else
                                {
                                    //if (model.ExpectedStartDate < DateTime.Now)
                                    //{
                                    //    myoutputDetailFlow.FlowTypeGUID = WarehouseRequestFlowType.Confirmed;
                                    //    }
                                    //else
                                    //{
                                    myoutputDetailFlow.FlowTypeGUID = WarehouseRequestFlowType.PendingConfirmed;

                                    //}
                                }

                                myoutputDetailFlow.ItemStatuGUID = WarehouseItemStatus.Functionting;
                                myoutputDetailFlow.CreatedDate = DateTime.Now;
                                myoutputDetailFlow.IsLastAction = true;
                                myoutputDetailFlow.IsLastMove = true;
                                myoutputDetailFlow.CreatedByGUID = UserGUID;
                                myoutputDetailFlow.Active = true;
                                myoutputDetailFlow.OrderId = myOrder;
                                DbWMS.Create(myoutputDetailFlow, Permissions.WarehouseItemsEntry.CreateGuid, ExecutionTime, DbCMS);
                                if (ReleaseModel.RequesterGUID == WarehouseRequestSourceTypes.Warehouse)
                                {
                                    var childernCurrentWarehouseGUID = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID)
                                        .Select(x => x.WarehouseGUID)
                                        .FirstOrDefault();

                                    foreach (var myTransfer in ChildreninputDetails)
                                    {
                                        //List<dataItemTransfer> myChildrenTransfer = DbWMS.dataItemTransfer
                                        //    .Where(x => x.ItemInputDetailGUID == myTransfer.ItemInputDetailGUID).ToList();

                                        var myChildrenTransfer = (from a in DbWMS.dataItemTransfer.Where(x => x.ItemInputDetailGUID == myTransfer.ItemInputDetailGUID) select a).ToList();
                                        myChildrenTransfer.ForEach(x => x.IsLastTransfer = false);

                                        //foreach (var transfer in myChildrenTransfer)
                                        //{
                                        //    transfer.IsLastTransfer = false;
                                        //}

                                        //DbWMS.UpdateBulk(myChildrenTransfer, Permissions.WarehouseItemsEntry.UpdateGuid, ExecutionTime,
                                        //    DbCMS);
                                        dataItemTransfer myTransferModel = new dataItemTransfer();
                                        myTransferModel.ItemTransferGUID = Guid.NewGuid();
                                        myTransferModel.ItemInputDetailGUID = myChildrendOutputDetail.ItemInputDetailGUID;
                                        myTransferModel.TransferDate = ExecutionTime;
                                        myTransferModel.SourceGUID = CurrentWarehouseGUID;
                                        myTransferModel.DestionationGUID = ReleaseModel.RequesterNameGUID;
                                        myTransferModel.TransferedByGUID = UserGUID;
                                        myTransferModel.IsLastTransfer = true;
                                        DbWMS.Create(myTransferModel, Permissions.WarehouseItemsEntry.CreateGuid, ExecutionTime, DbCMS);

                                    }
                                }
                            }

                            if (childrenDetails.Count > 0)
                            {
                                DbWMS.CreateBulk(childrenDetails, Permissions.WarehouseItemsRelease.CreateGuid, ExecutionTime, DbCMS);
                            }
                            #endregion

                        }
                        DbWMS.CreateBulk(itemReleaseDetailsToAdd, Permissions.WarehouseItemsRelease.CreateGuid, ExecutionTime, DbCMS);
                    }
                    #endregion

                    var EntryModel = DbWMS.dataItemInputDetail.Where(x => guids.Contains(x.ItemInputDetailGUID)).FirstOrDefault();
                    try
                    {

                        DbWMS.SaveChanges();
                        DbCMS.SaveChanges();
                        if (myItemOutputDetailGuids.Count > 0)
                        {
                            SendConfirmReteriveBulkItems(myItemOutputDetailGuids);
                        }


                        var currentWarehouse = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).Select(x => x.WarehouseGUID).FirstOrDefault();
                        if ((ReleaseModel.RequesterGUID == WarehouseRequestSourceTypes.Warehouse
                              && currentWarehouse != ReleaseModel.RequesterNameGUID
                              ) ||
                             ReleaseModel.RequesterGUID == WarehouseRequestSourceTypes.Staff)
                        {
                            SendConfirmationReceivingModelEmailForBulk(ReleaseModel.ItemOutputGUID);
                        }
                        return RedirectToAction("ConfirmItemUpdate");
                    }
                    catch (Exception ex)
                    {
                        return Json(DbWMS.ErrorMessage("no Error"));
                    }

                }

            }

            return Json(DbWMS.ErrorMessage("Warnning:Some of the records  not confirmed"));


        }
        #endregion



        #region test

        //public ActionResult test()
        //{
        //    var checkUpdate = DbWMS.toDelete.ToList();
        //    var itemOutputs = DbWMS.dataItemOutputDetail.ToList();
        //    List<dataItemInputDetail> itemInputs = DbWMS.dataItemInputDetail.ToList();
        //    DateTime ExecutionTime = DateTime.Now;
        //    List<dataItemTransfer> alltrans = new List<dataItemTransfer>();
        //    var CurrentWarehouseGUID = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).Select(x => x.WarehouseGUID)
        //        .FirstOrDefault();
        //    var warehouses = DbWMS.codeWarehouse.ToList();
        //    foreach (var item in checkUpdate)
        //    {
        //        var itemoutput = itemOutputs.Where(x => x.ItemOutputDetailGUID == item.itemOutputDetailGUID).FirstOrDefault();
        //        List<dataItemTransfer> allPriviousTransfers = DbWMS.dataItemTransfer
        //            .Where(x => x.ItemInputDetailGUID == itemoutput.ItemInputDetailGUID).ToList();
        //        var input = itemInputs.Where(x => x.ItemInputDetailGUID == itemoutput.ItemInputDetailGUID).FirstOrDefault();
        //        input.LastWarehouseLocationGUID = item.WarehouseLocationGUID;
        //        DbWMS.Update(input, Permissions.WarehouseItemsEntry.UpdateGuid, ExecutionTime, DbCMS);
        //        foreach (var transfer in allPriviousTransfers)
        //        {
        //            transfer.IsLastTransfer = false;
        //        }
        //        DbWMS.UpdateBulk(allPriviousTransfers, Permissions.WarehouseItemsEntry.UpdateGuid, ExecutionTime, DbCMS);
        //        var myWhareuse = warehouses.Where(x => x.WarehouseLocationGUID == item.WarehouseLocationGUID).FirstOrDefault();
        //        dataItemTransfer TransferModel = new dataItemTransfer();
        //        TransferModel.ItemTransferGUID = Guid.NewGuid();
        //        TransferModel.ItemInputDetailGUID = itemoutput.ItemInputDetailGUID;
        //        TransferModel.TransferDate = ExecutionTime;
        //        TransferModel.SourceGUID = CurrentWarehouseGUID;
        //        TransferModel.DestionationGUID = myWhareuse.WarehouseGUID;
        //        TransferModel.TransferedByGUID = UserGUID;
        //        TransferModel.IsLastTransfer = true;
        //        alltrans.Add(TransferModel);



        //    }
        //    DbWMS.CreateBulk(alltrans, Permissions.WarehouseItemsEntry.CreateGuid, ExecutionTime, DbCMS);
        //    try
        //    {
        //        DbWMS.SaveChanges();
        //        DbCMS.SaveChanges();
        //        return View();
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(DbWMS.ErrorMessage(ex.Message));
        //    }




        //}
        #endregion
        #region Download File

        public ActionResult GetICTSOPFile()
        {
            string sourceFile = Server.MapPath("~/Areas/WMS/Templates/Standard_Operating_Procedure_for_issuing_ICT_equipment_to_staff.pdf");
            string DisFolder =
                Server.MapPath("~/Areas/WMS/temp/Standard_Operating_Procedure_for_issuing_ICT_equipment_to_staff" + DateTime.Now.ToBinary() + ".pdf");
            System.IO.File.Copy(sourceFile, DisFolder);
            byte[] fileBytes = System.IO.File.ReadAllBytes(DisFolder);
            string fileName = "/Standard_Operating_Procedure_for_issuing_ICT_equipment_to_staff " + DateTime.Now.ToString("yyMMdd") + ".pdf";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
        #endregion

        #region item Bulk Data table 

        [Route("WMS/FlowIndex/")]
        public ActionResult FlowIndex()
        {

            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Access, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            return View("~/Areas/WMS/Views/ReleaseModelFlows/FlowIndex.cshtml");
        }



        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult WarehouseModelTrackFlowMovementDataTableReminderForDelayInReturnItemsToStock(List<WarehouseModelTrackFlowMovementDataTableModel> models)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            var guids = models.Select(f => f.ItemOutputDetailFlowGUID);
            var myModels = DbWMS.dataItemOutputDetailFlow.Where(x =>
                guids.Contains(x.ItemOutputDetailFlowGUID)

                && x.IsLastAction == true
                && (x.FlowTypeGUID == WarehouseRequestFlowType.Confirmed
                || x.FlowTypeGUID == WarehouseRequestFlowType.PendingConfirmed)).ToList();
            var inputGuid = myModels.Select(x => x.dataItemOutputDetail.ItemInputDetailGUID).FirstOrDefault();
            var EntryModel = DbWMS.dataItemInputDetail.Where(x => x.ItemInputDetailGUID == inputGuid).FirstOrDefault();
            if (myModels.Count > 0)
            {
                SendEmailReminderForDelayItemsInReturn(myModels.Select(x => (Guid)x.ItemOutputDetailGUID).ToList());
                return Json(DbWMS.SingleUpdateMessage(DataTableNames.WarehouseModelEntryMovementsDataTable, DbWMS.PrimaryKeyControl(EntryModel), DbWMS.RowVersionControls(Portal.SingleToList(EntryModel))));
            }
            try
            {
                return Json(DbWMS.ErrorMessage("Warrning:Reminder will send  just for confirmd items"));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage("no Error"));
            }
        }

        [Route("WMS/WarehouseModelTrackFlowMovementDataTable/")]

        public ActionResult WarehouseModelTrackFlowMovementDataTable(DataTableRecievedOptions options)
        {


            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<WarehouseModelTrackFlowMovementDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<WarehouseModelTrackFlowMovementDataTableModel>(DataTable.Filters);
            }
            DateTime today = DateTime.Now;
            string custType = "Staff";
            Guid orgGuid = (Guid)Session[SessionKeys.OrganizationInstanceGUID];
            var All = (

                  from a in DbWMS.v_trackItemOutputFlow.Where(x => x.LastCustodian == custType
                                                                  && x.ExpectedReturenedDate <= today
                                                                  && x.IsLastAction == true
                                                                  && x.OrganizationInstanceGUID == orgGuid
                                                                 && x.LastFlowTypeGUID == WarehouseRequestFlowType.Confirmed).AsExpandable()

                  select new WarehouseModelTrackFlowMovementDataTableModel
                  {
                      ItemOutputDetailGUID = a.ItemOutputDetailGUID,
                      ItemOutputDetailFlowGUID = a.ItemOutputDetailFlowGUID,
                      ModelDescription = a.ModelDescription,
                      LastCustodianName = a.LastCustodianName,
                      IsAvaliable = a.IsAvaliable,
                      LastCustodian = a.LastCustodian,
                      BarcodeNumber = a.BarcodeNumber,
                      SerialNumber = a.SerialNumber,
                      IMEI = a.IMEI1,
                      GSM = a.GSM,
                      MAC = a.MAC,

                      LastLocation = a.LastLocation,
                      LastFlow = a.LastFlow,
                      ItemStatus = a.ItemStatus,
                      ExpectedStartDate = a.ExpectedStartDate,
                      ExpectedReturenedDate = a.ExpectedReturenedDate,
                      ActualReturenedDate = a.ActualReturenedDate,
                      Comments = a.CommentsReleas,
                      IssuedBy = a.ReleasedByName,

                      OutputCustodianName = a.OutputCustodianName,
                      OutputCustodian = a.OutputCustodian,
                      IsLastAction = a.IsLastAction,

                      FlowCreatedDate = a.FlowCreatedDate,
                      FlowTypeName = a.FlowTypeName,
                      FlowCreatedByName = a.FlowCreatedByName,



                  }).Where(Predicate);



            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<WarehouseModelTrackFlowMovementDataTableModel> Result = Mapper.Map<List<WarehouseModelTrackFlowMovementDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);



        }
        #endregion

        #region Reports Weekly

        #endregion

        #region Pending requests

        public ActionResult InitCheckPendingRequest()
        {
            var warehouseGuid = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).Select(x => x.WarehouseGUID).FirstOrDefault();
            var pending = DbWMS.v_trackItemOutputFlow.Where(x => x.LastFlowTypeGUID == WarehouseRequestFlowType.PendingConfirmed
                                                    ).AsQueryable();
            PendingRequestCheckVM model = new PendingRequestCheckVM();
            model.pendingForMyWarehosue = pending.Where(x => x.LastCustdianNameGUID == warehouseGuid).Count();
            model.PendingForOtherWarehouse = pending.Where(x =>
                    x.LastCustdianGUID == WarehouseRequestSourceTypes.Warehouse &&
                    x.LastCustdianNameGUID != warehouseGuid)
                .Count();
            model.PendingForStaff = pending.Where(x =>
                    x.LastCustdianGUID == WarehouseRequestSourceTypes.Staff
                    )
                .Count();
            model.PendingForStaff = pending.Where(x =>
                 x.LastCustdianGUID == WarehouseRequestSourceTypes.Staff
                 )
             .Count();

            DateTime today = DateTime.Now;
            string custType = "Staff";

            model.DelayInReturn = DbWMS.v_trackItemOutputFlow.Where(x => x.LastCustodian == custType
                                                                   && x.ExpectedReturenedDate <= today
                                                                   && x.IsLastAction == true
                                                                  && x.LastFlowTypeGUID == WarehouseRequestFlowType.Confirmed).Count();


            return PartialView("~/Areas/WMS/Views/ReleaseModelNotification/_NotificaitonItemSummary.cshtml", model);
            //return Json(new { success = 1, model = model }, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region Determinant

        public ActionResult ItemDeterminantUpdate(Guid FK)
        {

            var myDetail = DbWMS.dataItemInputDetail.Where(x => x.ItemInputDetailGUID == FK)
                .FirstOrDefault();
            var determinantModel = DbWMS.dataItemInputDeterminant.Where(x => x.ItemInputDetailGUID == FK).ToList();
            var determinantModelDeterminantGUIDs =
                determinantModel.Select(x => x.WarehouseItemModelDeterminantGUID).ToList();

            var dete = DbWMS.codeWarehouseItemModelDeterminant.Where(x => x.Active
                                                                          && x.codeWarehouseItemModel.WarehouseItemModelGUID == myDetail.codeItemModelWarehouse.codeWarehouseItemModel.WarehouseItemModelGUID
                                                                 && !determinantModelDeterminantGUIDs.Contains(
                                                                     x.WarehouseItemModelDeterminantGUID)).ToList();
            Determinants myDeterminants = new Determinants();
            myDeterminants.ItemInputDetailGUID = FK;

            foreach (var item in dete)
            {
                if (item.DeterminantGUID == ItemDeterminants.Barcode)
                {
                    myDeterminants.IsHasBarcode = true;
                    myDeterminants.BarcodeGUID = item.WarehouseItemModelDeterminantGUID;
                }
                if (item.DeterminantGUID == ItemDeterminants.SerialNumber)
                {
                    myDeterminants.IsHasSerialNumber = true;
                    myDeterminants.SerilaGUID = item.WarehouseItemModelDeterminantGUID;
                }
                if (item.DeterminantGUID == ItemDeterminants.IME1)
                {
                    myDeterminants.IsHasIMEI1 = true;
                    myDeterminants.IMEI1GUID = item.WarehouseItemModelDeterminantGUID;
                }
                if (item.DeterminantGUID == ItemDeterminants.IME2)
                {
                    myDeterminants.IsHasIMEI2 = true;
                    myDeterminants.IMEI2GUID = item.WarehouseItemModelDeterminantGUID;
                }
                if (item.DeterminantGUID == ItemDeterminants.GSM)
                {
                    myDeterminants.IsHasGSM = true;
                    myDeterminants.GSMGUID = item.WarehouseItemModelDeterminantGUID;
                }
                if (item.DeterminantGUID == ItemDeterminants.MAC)
                {
                    myDeterminants.IsHasMAC = true;
                    myDeterminants.MACGUID = item.WarehouseItemModelDeterminantGUID;
                }
                if (item.DeterminantGUID == ItemDeterminants.MSRPID)
                {
                    myDeterminants.IsHasMSRPID = true;
                    myDeterminants.MSRPIDGUID = item.WarehouseItemModelDeterminantGUID;
                }
                if (item.DeterminantGUID == ItemDeterminants.SeqNumber)
                {
                    myDeterminants.IsHasSeq = true;
                    myDeterminants.SeqenceNumberGUID = item.WarehouseItemModelDeterminantGUID;

                }
            }

            //var myDet = (from a in DbWMS.codeItemModelWarehouse.Where(x => x.Active && x.ItemModelWarehouseGUID == myDetail.ItemModelWarehouseGUID
            //    )
            //             join b in DbWMS.codeWarehouseItemModel.Where(x => x.Active) on a.WarehouseItemModelGUID equals b.WarehouseItemModelGUID
            //             join c in DbWMS.codeWarehouseItemModelDeterminant.Where(x => x.Active
            //                                                                          && !determinantModelDeterminantGUIDs.Contains(x.WarehouseItemModelDeterminantGUID)) on b.WarehouseItemModelGUID equals c.WarehouseItemModelGUID
            //             join d in DbWMS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on c.DeterminantGUID equals d.ValueGUID
            //             select new ModelDeterminantOtherVM
            //             {
            //                 WarehouseItemModelDeterminantGUID = (Guid)c.WarehouseItemModelDeterminantGUID,
            //                 DeterminantName = d.ValueDescription,
            //                 ItemInputDetailGUID = myDetail.ItemInputDetailGUID
            //             }).ToList();
            //List<ModelDeterminantOtherVM> model = myDet;
            //if(my)
            return PartialView("~/Areas/WMS/Views/ModelEntryMovements/_OtherDeterminants.cshtml", myDeterminants);


        }



        [HttpPost]
        public ActionResult ItemDeterminantUpdate(Determinants model)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Access, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            DateTime ExecutionTime = DateTime.Now;

            List<dataItemInputDeterminant> allModelDeterminants = new List<dataItemInputDeterminant>();
            dataItemInputDetail inputdetail = DbWMS.dataItemInputDetail.Where(x => x.ItemInputDetailGUID == model.ItemInputDetailGUID).FirstOrDefault();
            if (model.BarcodeNumber != null && model.BarcodeNumber.Length > 4)
            {
                dataItemInputDeterminant myDet = new dataItemInputDeterminant
                {
                    ItemInputDeterminantGUID = Guid.NewGuid(),
                    ItemInputDetailGUID = model.ItemInputDetailGUID,
                    WarehouseItemModelDeterminantGUID = model.BarcodeGUID,
                    DeterminantValue = model.BarcodeNumber,
                    CreatedByGUID = UserGUID,
                    CreatedDate = ExecutionTime,
                    Active = true,
                };
                inputdetail.BarcodeNumber = model.BarcodeNumber;

                allModelDeterminants.Add(myDet);
            }
            if (model.SerialNumber != null && model.SerialNumber.Length > 4)
            {
                dataItemInputDeterminant myDet = new dataItemInputDeterminant
                {
                    ItemInputDeterminantGUID = Guid.NewGuid(),
                    ItemInputDetailGUID = model.ItemInputDetailGUID,
                    WarehouseItemModelDeterminantGUID = model.SerilaGUID,
                    DeterminantValue = model.SerialNumber,
                    CreatedByGUID = UserGUID,
                    CreatedDate = ExecutionTime,
                    Active = true,
                };
                inputdetail.SerialNumber = model.SerialNumber;

                allModelDeterminants.Add(myDet);
            }
            if (model.IMEI1 != null && model.IMEI1.Length > 4)
            {
                dataItemInputDeterminant myDet = new dataItemInputDeterminant
                {
                    ItemInputDeterminantGUID = Guid.NewGuid(),
                    ItemInputDetailGUID = model.ItemInputDetailGUID,
                    WarehouseItemModelDeterminantGUID = model.IMEI1GUID,
                    DeterminantValue = model.IMEI1,
                    CreatedByGUID = UserGUID,
                    CreatedDate = ExecutionTime,
                    Active = true,
                };

                inputdetail.IMEI1 = model.IMEI1;
                allModelDeterminants.Add(myDet);
            }
            if (model.IMEI2 != null && model.IMEI2.Length > 4)
            {
                dataItemInputDeterminant myDet = new dataItemInputDeterminant
                {
                    ItemInputDeterminantGUID = Guid.NewGuid(),
                    ItemInputDetailGUID = model.ItemInputDetailGUID,
                    WarehouseItemModelDeterminantGUID = model.IMEI2GUID,
                    DeterminantValue = model.IMEI2,
                    CreatedByGUID = UserGUID,
                    CreatedDate = ExecutionTime,
                    Active = true,
                };
                inputdetail.IMEI2 = model.IMEI2;

                allModelDeterminants.Add(myDet);
            }
            if (model.GSM != null && model.GSM.Length > 4)
            {
                dataItemInputDeterminant myDet = new dataItemInputDeterminant
                {
                    ItemInputDeterminantGUID = Guid.NewGuid(),
                    ItemInputDetailGUID = model.ItemInputDetailGUID,
                    WarehouseItemModelDeterminantGUID = model.GSMGUID,
                    DeterminantValue = model.GSM,
                    CreatedByGUID = UserGUID,
                    CreatedDate = ExecutionTime,
                    Active = true,
                };

                inputdetail.GSM = model.GSM;
                allModelDeterminants.Add(myDet);
            }
            if (model.MAC != null && model.MAC.Length > 4)
            {
                dataItemInputDeterminant myDet = new dataItemInputDeterminant
                {
                    ItemInputDeterminantGUID = Guid.NewGuid(),

                    ItemInputDetailGUID = model.ItemInputDetailGUID,
                    WarehouseItemModelDeterminantGUID = model.MACGUID,
                    DeterminantValue = model.MAC,
                    CreatedByGUID = UserGUID,
                    CreatedDate = ExecutionTime,
                    Active = true,
                };
                inputdetail.MAC = model.MAC;

                allModelDeterminants.Add(myDet);
            }
            if (model.SequenceNumber != null && model.SequenceNumber.Length > 4)
            {
                dataItemInputDeterminant myDet = new dataItemInputDeterminant
                {
                    ItemInputDeterminantGUID = Guid.NewGuid(),

                    ItemInputDetailGUID = model.ItemInputDetailGUID,
                    WarehouseItemModelDeterminantGUID = model.SeqenceNumberGUID,
                    DeterminantValue = model.SequenceNumber,
                    CreatedByGUID = UserGUID,
                    CreatedDate = ExecutionTime,
                    Active = true,
                };
                inputdetail.SequenceNumber = model.SequenceNumber;

                allModelDeterminants.Add(myDet);
            }
            if (model.MSRPID != null && model.MSRPID.Length > 4)
            {
                dataItemInputDeterminant myDet = new dataItemInputDeterminant
                {
                    ItemInputDeterminantGUID = Guid.NewGuid(),

                    ItemInputDetailGUID = model.ItemInputDetailGUID,
                    WarehouseItemModelDeterminantGUID = model.MSRPIDGUID,
                    DeterminantValue = model.MSRPID,
                    CreatedByGUID = UserGUID,
                    CreatedDate = ExecutionTime,
                    Active = true,
                };
                inputdetail.MAC = model.MAC;

                allModelDeterminants.Add(myDet);
            }
            DbWMS.Update(inputdetail, Permissions.WarehouseItemsEntry.UpdateGuid, ExecutionTime, DbCMS);
            //model.Where(x => x.DeterminantValue != null && x.DeterminantValue.ToString().Length > 4).ForEach(myDet =>
            //    allModelDeterminants.Add(new dataItemInputDeterminant
            //    {
            //        ItemInputDeterminantGUID = Guid.NewGuid(),
            //        ItemInputDetailGUID = myDet.ItemInputDetailGUID,
            //        WarehouseItemModelDeterminantGUID = myDet.WarehouseItemModelDeterminantGUID,
            //        DeterminantValue = myDet.DeterminantValue,
            //        CreatedByGUID = UserGUID,
            //        CreatedDate = ExecutionTime,
            //        Active = true,
            //    })
            //);
            DbWMS.CreateBulk(allModelDeterminants, Permissions.WarehouseItemsEntry.CreateGuid, DateTime.Now, DbCMS);
            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();

                return RedirectToAction("ConfirmItemUpdate");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
        #endregion

        #region Notificaiton

        public void SendNotification(dataItemOutputNotification model)
        {
            DateTime ExecutionTime = DateTime.Now;
            if (model.NotificationTypeGUID == WarehosueNotificationType.Reminder)
            {
                model.NotificationNumber = DbWMS.dataItemOutputNotification.Where(x => x.ItemOutputDetailGUID == model.ItemOutputDetailGUID)
                       .Select(x => x.NotificationNumber).Max() == null
                       ? 0
                       : DbWMS.dataItemOutputNotification.Where(x => x.ItemOutputDetailGUID == model.ItemOutputDetailGUID)
                             .Select(x => x.NotificationNumber).Max() + 1;


            }
            else
            {
                model.NotificationNumber = 1;
            }
            dataItemOutputNotification myNotificaiton = new dataItemOutputNotification
            {
                ItemOutputNotificationGUID = Guid.NewGuid(),
                ItemOutputDetailGUID = model.ItemOutputDetailGUID,
                NotificationMessage = model.NotificationMessage,
                NotificationTypeGUID = model.NotificationTypeGUID,
                NotificationNumber = model.NotificationNumber,
                NotficationDate = ExecutionTime,
                IsRecevied = false,
                CreatedByGUID = UserGUID,
                CreatedDate = ExecutionTime,
                Active = true,
            };

            DbWMS.Create(myNotificaiton, Permissions.WarehouseItemsEntry.CreateGuid, ExecutionTime, DbCMS);
            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        #endregion
        #region Upload damaged report

        //[Route("WMS/ModelMovements/UploadDamgedReport/{PK}")]
        public ActionResult UploadDamgedReport(Guid PK)
        {

            UploadDamagedItemVM model = new UploadDamagedItemVM();
            model.ItemInputDetailGUID = PK;
            return PartialView("~/Areas/WMS/Views/DamagedModels/_UploadDamagedReport.cshtml", model);
        }
        [HttpPost]
        public FineUploaderResult UploadDamagedReport(FineUpload upload, UploadDamagedItemVM model)
        {
            string error = "Error ";

            if (FileTypeValidator.IsPDF(upload.InputStream) || FileTypeValidator.IsImage(upload.InputStream) ||
                FileTypeValidator.IsExcel(upload.InputStream) ||
                FileTypeValidator.IsWord(upload.InputStream)
                )
            {
                return new FineUploaderResult(true, new { path = UploadDamaged(upload, model), success = true });
            }
            return new FineUploaderResult((error == "" ? true : false), new { Error = error, path = "", success = (error == "" ? true : false) });
        }

        public string UploadDamaged(FineUpload upload, UploadDamagedItemVM model)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Access, Apps.WMS))
            {
                return "Error";
            }
            if (upload != null)
            {
                var detail = DbWMS.dataItemInputDetail.Where(x => x.ItemInputDetailGUID == model.ItemInputDetailGUID).FirstOrDefault();
                detail.IsAvaliable = false;
                detail.ItemStatusGUID = WarehouseItemStatus.Damaged;
                DateTime ExecutionTime = DateTime.Now;
                DbWMS.Update(detail, Permissions.WarehouseItemsEntry.UpdateGuid, ExecutionTime, DbCMS);

                var filePath = Server.MapPath("~\\Uploads\\WMS\\DamagedItemReports\\");
                string extension = Path.GetExtension(upload.FileName);
                string fullFileName = filePath + "\\" + model.ItemInputDetailGUID + extension;
                upload.SaveAs(fullFileName);
                try
                {
                    DbWMS.SaveChanges();
                    DbCMS.SaveChanges();

                }
                catch (Exception)
                {

                    throw;
                }


            }

            return "done";
        }

        #endregion

        #region Pending warehouse 
        [Route("WMS/PendingVerificationWarehouseIndex/")]
        public ActionResult PendingVerificationWarehouseIndex()
        {

            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Access, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            return View("~/Areas/WMS/Views/ModelReleaseMovements/PendingVerificationMyWarehouse.cshtml");
        }

        [Route("WMS/WarehousePendingVerificationMyWarehouseDataTable/")]
        public JsonResult WarehousePendingVerificationMyWarehouseDataTable(DataTableRecievedOptions options)
        {


            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<WarehouseModelEntryMovementDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<WarehouseModelEntryMovementDataTableModel>(DataTable.Filters);
            }
            var CurrentWarehouseGUID = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).Select(x => x.WarehouseGUID)
                .FirstOrDefault();
            //var warehouseAuthGUIDs = DbWMS.codeWarehouse
            //    .Where(x => x.WarehouseGUID == CurrentWarehouseGUID || x.ParentGUID == CurrentWarehouseGUID)
            //    .Select(x => x.WarehouseGUID).ToList();

            string custType = "Warehouse";
            //Access is authorized by Access Action
            Guid orgGuid = (Guid)Session[SessionKeys.OrganizationInstanceGUID];
            var All = (from a in DbWMS.v_EntryMovementDataTable.AsExpandable().Where(x => x.WarehouseOwnerGUID == CurrentWarehouseGUID
                       && x.LastFlowTypeGUID == WarehouseRequestFlowType.PendingConfirmed
                       && x.OrganizationInstanceGUID == orgGuid
                       && x.LastCustodian == custType)
                       select new WarehouseModelEntryMovementDataTableModel
                       {

                           ItemInputDetailGUID = a.ItemInputDetailGUID,
                           Active = (bool)a.Active,
                           ItemModelWarehouseGUID = a.ItemModelWarehouseGUID.ToString(),
                           ModelDescription = a.ModelDescription,

                           ItemDescription = a.WarehouseItemDescription,

                           WarehouseItemClassificationGUID = a.WarehouseItemClassificationGUID.ToString(),
                           WarehouseItemGUID = a.WarehouseItemGuid.ToString(),
                           BrandGUID = a.BrandGUID.ToString(),
                           WarehouseOwnerGUID = a.WarehouseOwnerGUID.ToString(),
                           BarcodeNumber = a.BarcodeNumber,
                           SerialNumber = a.SerialNumber,
                           IME1 = a.IMEI,
                           GSM = a.GSM,
                           MAC = a.MAC,
                           SequenceNumber = a.SequenceNumber,
                           ItemCondition = a.ItemCondition,
                           ItemConditionGUID = a.ItemConditionGUID.ToString(),
                           ItemServiceStatusGUID = a.ItemServiceStatusGUID.ToString(),
                           ServiceItemStatus = a.ServiceItemStatus,
                           WarehouseOwner = a.WarehosueOwnerName,
                           Governorate = a.LastLocation,
                           WarehouseLocationGUID = a.WarehouseLocationGUID.ToString(),
                           CustodianStaffGUID = a.LastCustdianNameGUID.ToString(),
                           CustodianWarehouseGUID = a.LastCustdianNameGUID.ToString(),
                           ModelAge = a.ModelAge,

                           Custodian = a.LastCustodianName,
                           ModelStatus = a.ItemStatus,
                           ModelStatusGUID = a.ItemStatusGUID.ToString(),
                           CreatedDate = a.CreatedDate,
                           PurposeofuseGUID = a.PurposeofuseGUID,
                           Purposeofuse = a.Purposeofuse,
                           CreatedBy = a.CreatedBy,
                           CreatedByGUID = a.CreatedByGUID.ToString(),
                           Comments = a.Comments,
                           DeliveryStatus = a.LastFlow,
                           DeliveryStatusGUID = a.LastFlowTypeGUID.ToString(),
                           dataItemInputDetailRowVersion = a.dataItemInputDetailRowVersion
                       }).Where(Predicate);


            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<WarehouseModelEntryMovementDataTableModel> Result = Mapper.Map<List<WarehouseModelEntryMovementDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("WMS/PendingVerificationStaffIndex/")]
        public ActionResult PendingVerificationStaffIndex()
        {

            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Access, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            return View("~/Areas/WMS/Views/ModelReleaseMovements/PendingVerificationStaff.cshtml");
        }

        [Route("WMS/WarehousePendingVerificationStaffDataTable/")]
        public JsonResult WarehousePendingVerificationStaffDataTable(DataTableRecievedOptions options)
        {


            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<WarehouseModelEntryMovementDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<WarehouseModelEntryMovementDataTableModel>(DataTable.Filters);
            }
            var CurrentWarehouseGUID = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).Select(x => x.WarehouseGUID)
                .FirstOrDefault();
            //var warehouseAuthGUIDs = DbWMS.codeWarehouse
            //    .Where(x => x.WarehouseGUID == CurrentWarehouseGUID || x.ParentGUID == CurrentWarehouseGUID)
            //    .Select(x => x.WarehouseGUID).ToList();

            string custType = "Staff";
            //Access is authorized by Access Action
            Guid orgGuid = (Guid)Session[SessionKeys.OrganizationInstanceGUID];
            var All = (from a in DbWMS.v_EntryMovementDataTable.AsExpandable().Where(x => x.LastFlowTypeGUID == WarehouseRequestFlowType.PendingConfirmed
                       && x.OrganizationInstanceGUID == orgGuid
                       && x.LastCustodian == custType)
                       select new WarehouseModelEntryMovementDataTableModel
                       {

                           ItemInputDetailGUID = a.ItemInputDetailGUID,
                           Active = (bool)a.Active,
                           ItemModelWarehouseGUID = a.ItemModelWarehouseGUID.ToString(),
                           ModelDescription = a.ModelDescription,

                           ItemDescription = a.WarehouseItemDescription,

                           WarehouseItemClassificationGUID = a.WarehouseItemClassificationGUID.ToString(),
                           WarehouseItemGUID = a.WarehouseItemGuid.ToString(),
                           BrandGUID = a.BrandGUID.ToString(),
                           WarehouseOwnerGUID = a.WarehouseOwnerGUID.ToString(),
                           BarcodeNumber = a.BarcodeNumber,
                           SerialNumber = a.SerialNumber,
                           IME1 = a.IMEI,
                           GSM = a.GSM,
                           MAC = a.MAC,
                           SequenceNumber = a.SequenceNumber,
                           ItemCondition = a.ItemCondition,
                           ItemConditionGUID = a.ItemConditionGUID.ToString(),
                           ItemServiceStatusGUID = a.ItemServiceStatusGUID.ToString(),
                           ServiceItemStatus = a.ServiceItemStatus,
                           WarehouseOwner = a.WarehosueOwnerName,
                           Governorate = a.LastLocation,
                           WarehouseLocationGUID = a.WarehouseLocationGUID.ToString(),
                           CustodianStaffGUID = a.LastCustdianNameGUID.ToString(),
                           CustodianWarehouseGUID = a.LastCustdianNameGUID.ToString(),

                           Custodian = a.LastCustodianName,
                           ModelStatus = a.ItemStatus,
                           ModelStatusGUID = a.ItemStatusGUID.ToString(),
                           CreatedDate = a.CreatedDate,
                           PurposeofuseGUID = a.PurposeofuseGUID,
                           Purposeofuse = a.Purposeofuse,
                           CreatedBy = a.CreatedBy,
                           CreatedByGUID = a.CreatedByGUID.ToString(),
                           Comments = a.Comments,
                           DeliveryStatus = a.LastFlow,
                           DeliveryStatusGUID = a.LastFlowTypeGUID.ToString(),
                           dataItemInputDetailRowVersion = a.dataItemInputDetailRowVersion
                       }).Where(Predicate);


            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<WarehouseModelEntryMovementDataTableModel> Result = Mapper.Map<List<WarehouseModelEntryMovementDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("WMS/PendingVerificationOtherWarehouse/")]
        public ActionResult PendingVerificationOtherWarehouseIndex()
        {

            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Access, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            return View("~/Areas/WMS/Views/ModelReleaseMovements/PendingVerificationOtherWarehouse.cshtml");
        }

        [Route("WMS/WarehousePendingVerificationOtherDataTable/")]
        public JsonResult WarehousePendingVerificationOtherDataTable(DataTableRecievedOptions options)
        {


            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<WarehouseModelEntryMovementDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<WarehouseModelEntryMovementDataTableModel>(DataTable.Filters);
            }
            var CurrentWarehouseGUID = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).Select(x => x.WarehouseGUID)
                .FirstOrDefault();
            var warehouseAuthGUIDs = DbWMS.codeWarehouse
                .Where(x => x.WarehouseGUID == CurrentWarehouseGUID || x.ParentGUID == CurrentWarehouseGUID)
                .Select(x => x.WarehouseGUID).ToList();

            string custType = "Warehouse";
            Guid orgGuid = (Guid)Session[SessionKeys.OrganizationInstanceGUID];
            //Access is authorized by Access Action

            var All = (from a in DbWMS.v_EntryMovementDataTable.AsExpandable().Where(x => warehouseAuthGUIDs.Contains((Guid)x.WarehouseOwnerGUID)
                        && x.LastFlowTypeGUID == WarehouseRequestFlowType.PendingConfirmed
                       && x.LastCustodian == custType
                        && x.OrganizationInstanceGUID == orgGuid
                       && x.WarehouseOwnerGUID != CurrentWarehouseGUID)
                       select new WarehouseModelEntryMovementDataTableModel
                       {

                           ItemInputDetailGUID = a.ItemInputDetailGUID,
                           Active = (bool)a.Active,
                           ItemModelWarehouseGUID = a.ItemModelWarehouseGUID.ToString(),
                           ModelDescription = a.ModelDescription,

                           ItemDescription = a.WarehouseItemDescription,

                           WarehouseItemClassificationGUID = a.WarehouseItemClassificationGUID.ToString(),
                           WarehouseItemGUID = a.WarehouseItemGuid.ToString(),
                           BrandGUID = a.BrandGUID.ToString(),
                           WarehouseOwnerGUID = a.WarehouseOwnerGUID.ToString(),
                           BarcodeNumber = a.BarcodeNumber,
                           SerialNumber = a.SerialNumber,
                           IME1 = a.IMEI,
                           GSM = a.GSM,
                           MAC = a.MAC,
                           SequenceNumber = a.SequenceNumber,
                           ItemCondition = a.ItemCondition,
                           ItemConditionGUID = a.ItemConditionGUID.ToString(),
                           ItemServiceStatusGUID = a.ItemServiceStatusGUID.ToString(),
                           ServiceItemStatus = a.ServiceItemStatus,
                           WarehouseOwner = a.WarehosueOwnerName,
                           Governorate = a.LastLocation,
                           WarehouseLocationGUID = a.WarehouseLocationGUID.ToString(),
                           CustodianStaffGUID = a.LastCustdianNameGUID.ToString(),
                           CustodianWarehouseGUID = a.LastCustdianNameGUID.ToString(),

                           Custodian = a.LastCustodianName,
                           ModelStatus = a.ItemStatus,
                           ModelStatusGUID = a.ItemStatusGUID.ToString(),
                           CreatedDate = a.CreatedDate,
                           PurposeofuseGUID = a.PurposeofuseGUID,
                           Purposeofuse = a.Purposeofuse,
                           CreatedBy = a.CreatedBy,
                           CreatedByGUID = a.CreatedByGUID.ToString(),
                           Comments = a.Comments,
                           DeliveryStatus = a.LastFlow,
                           DeliveryStatusGUID = a.LastFlowTypeGUID.ToString(),
                           dataItemInputDetailRowVersion = a.dataItemInputDetailRowVersion
                       }).Where(Predicate);


            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<WarehouseModelEntryMovementDataTableModel> Result = Mapper.Map<List<WarehouseModelEntryMovementDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }


        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult WarehousePendingVerificationStaffDataTableReminderPendingConfirmationBulkItems(List<WarehouseModelEntryMovementDataTableModel> models)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            var guids = models.Select(f => f.ItemInputDetailGUID);
            var myModels = DbWMS.dataItemOutputDetailFlow.Where(x =>
                guids.Contains(x.dataItemOutputDetail.dataItemInputDetail.ItemInputDetailGUID)

                && x.IsLastAction == true
                && x.FlowTypeGUID == WarehouseRequestFlowType.PendingConfirmed).ToList();
            var EntryModel = DbWMS.dataItemInputDetail.Where(x => guids.Contains(x.ItemInputDetailGUID)).FirstOrDefault();
            if (myModels.Count > 0)
            {
                var myCustToConfirmGUIDs = myModels.Select(x => x.dataItemOutputDetail.dataItemInputDetail.LastCustdianNameGUID).Distinct().ToList();
                foreach (var item in myCustToConfirmGUIDs)
                {
                    SendEmailReminderForPendingConfirmationBulkItems((Guid)item);
                }




                return Json(DbWMS.SingleUpdateMessage(DataTableNames.WarehousePendingVerificationStaffDataTable, DbWMS.PrimaryKeyControl(EntryModel), DbWMS.RowVersionControls(Portal.SingleToList(EntryModel))));
            }

            try
            {

                return Json(DbWMS.ErrorMessage("Warrning:Reminder will just send  for pending confirmation records"));

            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage("no Error"));
            }
        }

        #endregion

        #region Track Movement Flow
        [Route("WMS/TrackItemMovementDailyIndex/")]
        public ActionResult TrackItemMovementDailyIndex()
        {

            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Access, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            return View("~/Areas/WMS/Views/TrackItemFlow/TrackItemMovementDailyIndex.cshtml");
        }




        [Route("WMS/WarehouseItemTrackFlowDailyDataTable/")]

        public ActionResult WarehouseItemTrackFlowDailyDataTable(DataTableRecievedOptions options)
        {


            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<WarehouseModelTrackFlowMovementDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<WarehouseModelTrackFlowMovementDataTableModel>(DataTable.Filters);
            }
            var CurrentWarehouseGUID = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).Select(x => x.WarehouseGUID)
       .FirstOrDefault();
            var warehouseAuthGUIDs = DbWMS.codeWarehouse
                .Where(x => x.WarehouseGUID == CurrentWarehouseGUID || x.ParentGUID == CurrentWarehouseGUID)
                .Select(x => x.WarehouseGUID).ToList();
            DateTime today = DateTime.Now.AddDays(-1);
            Guid orgGuid = (Guid)Session[SessionKeys.OrganizationInstanceGUID];
            var All = (

                  from a in DbWMS.v_trackItemOutputFlow.Where(x => x.FlowCreatedDate >= today && x.OrganizationInstanceGUID == orgGuid).AsExpandable()

                  select new WarehouseModelTrackFlowMovementDataTableModel
                  {
                      ItemOutputDetailGUID = a.ItemOutputDetailGUID,
                      ItemOutputDetailFlowGUID = a.ItemOutputDetailFlowGUID,
                      ItemInputDetailGUID = a.ItemInputDetailGUID,
                      ModelDescription = a.ModelDescription,
                      LastCustodianName = a.LastCustodianName,
                      LastCustdianNameGUID = a.LastCustdianNameGUID.ToString(),
                      LastCustdianGUID = a.LastCustdianGUID.ToString(),
                      BrandGUID = a.BrandGUID.ToString(),
                      ItemDescription = a.WarehouseItemDescription,
                      WarehouseItemClassificationGUID = a.WarehouseItemClassificationGUID.ToString(),
                      WarehouseItemGUID = a.WarehouseItemGuid.ToString(),
                      BarcodeNumber = a.BarcodeNumber,
                      SerialNumber = a.SerialNumber,
                      IME1 = a.IMEI1,
                      GSM = a.GSM,
                      MAC = a.MAC,
                      Governorate = a.LastLocation,
                      WarehouseLocationGUID = a.WarehouseLocationGUID.ToString(),
                      CustodianStaffGUID = a.LastCustdianNameGUID.ToString(),
                      CustodianWarehouseGUID = a.LastCustdianNameGUID.ToString(),
                      IsAvaliable = a.IsAvaliable,
                      LastCustodian = a.LastCustodian,
                      LastLocation = a.LastLocation,
                      LastFlow = a.LastFlow,
                      ItemStatus = a.ItemStatus,
                      ExpectedStartDate = a.ExpectedStartDate,
                      ExpectedReturenedDate = a.ExpectedReturenedDate,
                      ActualReturenedDate = a.ActualReturenedDate,
                      Comments = a.CommentsReleas,
                      IssuedBy = a.ReleasedByName,
                      OutputCustodianName = a.OutputCustodianName,
                      OutputCustodian = a.OutputCustodian,
                      IsLastAction = a.IsLastAction,
                      FlowCreatedDate = a.FlowCreatedDate,
                      FlowTypeName = a.FlowTypeName,
                      FlowCreatedByName = a.FlowCreatedByName,
                  }).Where(Predicate);



            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<WarehouseModelTrackFlowMovementDataTableModel> Result = Mapper.Map<List<WarehouseModelTrackFlowMovementDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result.OrderByDescending(x => x.FlowCreatedDate)), JsonRequestBehavior.AllowGet);



        }

        [Route("WMS/TrackItemMovementWeeklyIndex/")]
        public ActionResult TrackItemMovementWeeklyIndex()
        {

            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Access, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            return View("~/Areas/WMS/Views/TrackItemFlow/TrackItemMovementWeeklyIndex.cshtml");
        }



        [Route("WMS/WarehouseItemTrackFlowWeeklyDataTable/")]

        public ActionResult WarehouseItemTrackFlowWeeklyDataTable(DataTableRecievedOptions options)
        {

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<WarehouseModelTrackFlowMovementDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<WarehouseModelTrackFlowMovementDataTableModel>(DataTable.Filters);
            }
            var CurrentWarehouseGUID = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).Select(x => x.WarehouseGUID)
       .FirstOrDefault();
            var warehouseAuthGUIDs = DbWMS.codeWarehouse
                .Where(x => x.WarehouseGUID == CurrentWarehouseGUID || x.ParentGUID == CurrentWarehouseGUID)
                .Select(x => x.WarehouseGUID).ToList();
            DateTime today = DateTime.Now.Date.AddDays(-7);
            Guid orgGuid = (Guid)Session[SessionKeys.OrganizationInstanceGUID];
            var All = (

                  from a in DbWMS.v_trackItemOutputFlow.Where(x => x.FlowCreatedDate >= today && x.OrganizationInstanceGUID == orgGuid).AsExpandable()

                  select new WarehouseModelTrackFlowMovementDataTableModel
                  {
                      ItemOutputDetailGUID = a.ItemOutputDetailGUID,
                      ItemInputDetailGUID = a.ItemInputDetailGUID,
                      ItemOutputDetailFlowGUID = a.ItemOutputDetailFlowGUID,
                      ModelDescription = a.ModelDescription,
                      ItemModelWarehouseGUID = a.ItemModelWarehouseGUID.ToString(),
                      CreatedByGUID = a.ReleasedByGUID.ToString(),
                      Comments = a.CommentsInput,

                      Custodian = a.LastCustodianName,
                      LastCustodianName = a.LastCustodianName,
                      LastCustdianNameGUID = a.LastCustdianNameGUID.ToString(),
                      LastCustdianGUID = a.LastCustdianGUID.ToString(),
                      BrandGUID = a.BrandGUID.ToString(),
                      ItemDescription = a.WarehouseItemDescription,
                      WarehouseItemClassificationGUID = a.WarehouseItemClassificationGUID.ToString(),
                      WarehouseItemGUID = a.WarehouseItemGuid.ToString(),
                      BarcodeNumber = a.BarcodeNumber,
                      SerialNumber = a.SerialNumber,
                      IME1 = a.IMEI1,
                      GSM = a.GSM,
                      MAC = a.MAC,

                      Governorate = a.LastLocation,
                      WarehouseLocationGUID = a.WarehouseLocationGUID.ToString(),
                      CustodianStaffGUID = a.LastCustdianNameGUID.ToString(),
                      CustodianWarehouseGUID = a.LastCustdianNameGUID.ToString(),
                      IsAvaliable = a.IsAvaliable,
                      LastCustodian = a.LastCustodian,
                      LastLocation = a.LastLocation,
                      LastFlow = a.LastFlow,
                      ItemStatus = a.ItemStatus,
                      ExpectedStartDate = a.ExpectedStartDate,
                      ExpectedReturenedDate = a.ExpectedReturenedDate,
                      ActualReturenedDate = a.ActualReturenedDate,

                      IssuedBy = a.ReleasedByName,
                      OutputCustodianName = a.OutputCustodianName,
                      OutputCustodian = a.OutputCustodian,
                      IsLastAction = a.IsLastAction,
                      FlowCreatedDate = a.FlowCreatedDate,
                      FlowTypeName = a.FlowTypeName,
                      FlowCreatedByName = a.FlowCreatedByName,

                  }).Where(Predicate);



            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<WarehouseModelTrackFlowMovementDataTableModel> Result = Mapper.Map<List<WarehouseModelTrackFlowMovementDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result.OrderByDescending(x => x.FlowCreatedDate)), JsonRequestBehavior.AllowGet);



        }


        [Route("WMS/TrackItemMovementMonthlyIndex/")]
        public ActionResult TrackItemMovementMonthlyIndex()
        {

            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Access, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            return View("~/Areas/WMS/Views/TrackItemFlow/TrackItemMovementMonthlyIndex.cshtml");
        }



        [Route("WMS/WarehouseItemTrackFlowMonthlyDataTable/")]

        public ActionResult WarehouseItemTrackFlowMonthlyDataTable(DataTableRecievedOptions options)
        {


            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<WarehouseModelTrackFlowMovementDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<WarehouseModelTrackFlowMovementDataTableModel>(DataTable.Filters);
            }
            var CurrentWarehouseGUID = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).Select(x => x.WarehouseGUID)
       .FirstOrDefault();
            var warehouseAuthGUIDs = DbWMS.codeWarehouse
                .Where(x => x.WarehouseGUID == CurrentWarehouseGUID || x.ParentGUID == CurrentWarehouseGUID)
                .Select(x => x.WarehouseGUID).ToList();
            DateTime today = DateTime.Now.Date.AddDays(-30);
            Guid orgGuid = (Guid)Session[SessionKeys.OrganizationInstanceGUID];
            var All = (

                  from a in DbWMS.v_trackItemOutputFlow.Where(x => x.FlowCreatedDate >= today && x.OrganizationInstanceGUID == orgGuid).AsExpandable()

                  select new WarehouseModelTrackFlowMovementDataTableModel
                  {
                      ItemOutputDetailGUID = a.ItemOutputDetailGUID,
                      ItemOutputDetailFlowGUID = a.ItemOutputDetailFlowGUID,
                      ModelDescription = a.ModelDescription,
                      ItemModelWarehouseGUID = a.ItemModelWarehouseGUID.ToString(),
                      CreatedByGUID = a.ReleasedByGUID.ToString(),
                      Comments = a.CommentsInput,
                      Custodian = a.LastCustodianName,
                      LastCustodianName = a.LastCustodianName,
                      LastCustdianNameGUID = a.LastCustdianNameGUID.ToString(),
                      LastCustdianGUID = a.LastCustdianGUID.ToString(),
                      BrandGUID = a.BrandGUID.ToString(),
                      ItemDescription = a.WarehouseItemDescription,
                      WarehouseItemClassificationGUID = a.WarehouseItemClassificationGUID.ToString(),
                      WarehouseItemGUID = a.WarehouseItemGuid.ToString(),
                      BarcodeNumber = a.BarcodeNumber,
                      SerialNumber = a.SerialNumber,
                      IME1 = a.IMEI1,
                      GSM = a.GSM,

                      MAC = a.MAC,
                      Governorate = a.LastLocation,
                      WarehouseLocationGUID = a.WarehouseLocationGUID.ToString(),
                      CustodianStaffGUID = a.LastCustdianNameGUID.ToString(),
                      CustodianWarehouseGUID = a.LastCustdianNameGUID.ToString(),
                      IsAvaliable = a.IsAvaliable,
                      LastCustodian = a.LastCustodian,
                      LastLocation = a.LastLocation,
                      LastFlow = a.LastFlow,
                      ItemStatus = a.ItemStatus,
                      ExpectedStartDate = a.ExpectedStartDate,
                      ExpectedReturenedDate = a.ExpectedReturenedDate,
                      ActualReturenedDate = a.ActualReturenedDate,

                      IssuedBy = a.ReleasedByName,
                      OutputCustodianName = a.OutputCustodianName,
                      OutputCustodian = a.OutputCustodian,
                      IsLastAction = a.IsLastAction,
                      FlowCreatedDate = a.FlowCreatedDate,
                      FlowTypeName = a.FlowTypeName,
                      FlowCreatedByName = a.FlowCreatedByName,



                  }).Where(Predicate);



            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<WarehouseModelTrackFlowMovementDataTableModel> Result = Mapper.Map<List<WarehouseModelTrackFlowMovementDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result.OrderByDescending(x => x.FlowCreatedDate)), JsonRequestBehavior.AllowGet);



        }

        [Route("WMS/TrackItemMovementAllIndex/")]
        public ActionResult TrackItemMovementAllIndex()
        {

            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Access, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            return View("~/Areas/WMS/Views/TrackItemFlow/TrackItemMovementAllIndex.cshtml");
        }



        [Route("WMS/WarehouseItemTrackFlowALLDataTable/")]

        public ActionResult WarehouseItemTrackFlowALLDataTable(DataTableRecievedOptions options)
        {


            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<WarehouseModelTrackFlowMovementDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<WarehouseModelTrackFlowMovementDataTableModel>(DataTable.Filters);
            }
            var CurrentWarehouseGUID = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).Select(x => x.WarehouseGUID)
       .FirstOrDefault();
            var warehouseAuthGUIDs = DbWMS.codeWarehouse
                .Where(x => x.WarehouseGUID == CurrentWarehouseGUID || x.ParentGUID == CurrentWarehouseGUID)
                .Select(x => x.WarehouseGUID).ToList();
            Guid orgGuid = (Guid)Session[SessionKeys.OrganizationInstanceGUID];

            var All = (

                  from a in DbWMS.v_trackItemOutputFlow.Where(x => x.OrganizationInstanceGUID == orgGuid).AsExpandable()

                  select new WarehouseModelTrackFlowMovementDataTableModel
                  {
                      ItemOutputDetailGUID = a.ItemOutputDetailGUID,
                      ItemOutputDetailFlowGUID = a.ItemOutputDetailFlowGUID,
                      ModelDescription = a.ModelDescription,
                      ItemModelWarehouseGUID = a.ItemModelWarehouseGUID.ToString(),
                      CreatedByGUID = a.ReleasedByGUID.ToString(),
                      Comments = a.CommentsInput,
                      ItemInputDetailGUID = a.ItemInputDetailGUID,

                      Custodian = a.LastCustodianName,
                      LastCustodianName = a.LastCustodianName,
                      LastCustdianNameGUID = a.LastCustdianNameGUID.ToString(),
                      LastCustdianGUID = a.LastCustdianGUID.ToString(),
                      BrandGUID = a.BrandGUID.ToString(),
                      ItemDescription = a.WarehouseItemDescription,
                      WarehouseItemClassificationGUID = a.WarehouseItemClassificationGUID.ToString(),
                      WarehouseItemGUID = a.WarehouseItemGuid.ToString(),
                      BarcodeNumber = a.BarcodeNumber,
                      SerialNumber = a.SerialNumber,
                      IME1 = a.IMEI1,
                      GSM = a.GSM,
                      MAC = a.MAC,

                      Governorate = a.LastLocation,
                      WarehouseLocationGUID = a.WarehouseLocationGUID.ToString(),
                      CustodianStaffGUID = a.LastCustdianNameGUID.ToString(),
                      CustodianWarehouseGUID = a.LastCustdianNameGUID.ToString(),
                      IsAvaliable = a.IsAvaliable,
                      LastCustodian = a.LastCustodian,
                      LastLocation = a.LastLocation,
                      LastFlow = a.LastFlow,
                      ItemStatus = a.ItemStatus,
                      ExpectedStartDate = a.ExpectedStartDate,
                      ExpectedReturenedDate = a.ExpectedReturenedDate,
                      ActualReturenedDate = a.ActualReturenedDate,

                      IssuedBy = a.ReleasedByName,
                      OutputCustodianName = a.OutputCustodianName,
                      OutputCustodian = a.OutputCustodian,
                      IsLastAction = a.IsLastAction,
                      FlowCreatedDate = a.FlowCreatedDate,
                      FlowTypeName = a.FlowTypeName,
                      FlowCreatedByName = a.FlowCreatedByName,

                  }).Where(Predicate);



            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<WarehouseModelTrackFlowMovementDataTableModel> Result = Mapper.Map<List<WarehouseModelTrackFlowMovementDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result.OrderByDescending(x => x.FlowCreatedDate)), JsonRequestBehavior.AllowGet);



        }
        #endregion
        #region Remindres
        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult WarehousePendingVerificationStaffDataTableReminderStaffPendingBulkItems(List<v_EntryMovementDataTable> models)
        {

            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            var guids = models.Select(f => f.ItemInputDetailGUID);
            var myModels = DbWMS.dataItemOutputDetailFlow.Where(x =>
                guids.Contains(x.dataItemOutputDetail.dataItemInputDetail.ItemInputDetailGUID)

                && x.IsLastAction == true
                && x.FlowTypeGUID == WarehouseRequestFlowType.PendingConfirmed).ToList();
            var EntryModel = DbWMS.dataItemInputDetail.Where(x => guids.Contains(x.ItemInputDetailGUID)).FirstOrDefault();
            if (myModels.Count > 0)
            {
                var myCustToConfirmGUIDs = myModels.Select(x => x.dataItemOutputDetail.dataItemInputDetail.LastCustdianNameGUID).Distinct().ToList();
                foreach (var item in myCustToConfirmGUIDs)
                {
                    SendEmailReminderForPendingConfirmationBulkItems((Guid)item);
                }




                return Json(DbWMS.SingleUpdateMessage(DataTableNames.WarehouseModelEntryMovementsDataTable, DbWMS.PrimaryKeyControl(EntryModel), DbWMS.RowVersionControls(Portal.SingleToList(EntryModel))));
            }

            try
            {

                return Json(DbWMS.ErrorMessage("Warrning:Reminder will just send  for pending confirmation records"));

            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage("no Error"));
            }
        }
        #endregion


        #region Verificaiton Page




        [Route("WMS/ItemVerificationPeriod/")]
        public ActionResult ItemVerificationPeriod()
        {

            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Access, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            return View("~/Areas/WMS/Views/ItemVerificationPeriod/Index.cshtml");
        }

        [Route("WMS/ItemVerificationPeriodsDataTable/")]
        public JsonResult ItemVerificationPeriodsDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<WarehouseItemVerficationPeriodDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<WarehouseItemVerficationPeriodDataTableModel>(DataTable.Filters);
            }
            var CurrentWarehouseGUID = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).Select(x => x.WarehouseGUID)
                .ToList();
            var warehouseAuthGUIDs = DbWMS.codeWarehouse
                .Where(x => CurrentWarehouseGUID.Contains(x.WarehouseGUID) || CurrentWarehouseGUID.Contains(x.ParentGUID))
                .Select(x => x.WarehouseGUID).ToList();


            //Access is authorized by Access Action

            var All = (from a in DbWMS.dataItemVerificationPeriod.AsExpandable()
                       select new WarehouseItemVerficationPeriodDataTableModel
                       {

                           ItemVerificationPeriodGUID = a.ItemVerificationPeriodGUID.ToString(),
                           Active = (bool)a.Active,
                           VerificationStartDate = a.VerificationStartDate,
                           VerificationEndDate = a.VerificationEndDate,
                           VerificationPeriodName = a.VerificationPeriodName,
                           IsLastPeriod = a.IsLastPeriod,
                           IsClosed = a.IsClosed,
                           OrderId = a.OrderId,


                           dataItemVerificationPeriodVersion = a.dataItemVerificationPeriodRowVersion
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<WarehouseItemVerficationPeriodDataTableModel> Result = Mapper.Map<List<WarehouseItemVerficationPeriodDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }
        public ActionResult WarehouseItemVerificationPeriodCreate()
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }



            return PartialView("~/Areas/WMS/Views/ItemVerificationPeriod/_ItemVerificationPeriodModel.cshtml",
                new WarehouseItemVerficationPeriodModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult WarehouseItemVerificationPeriodCreate(WarehouseItemVerficationPeriodModel model)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsRelease.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (model.VerificationStartDate != null && model.VerificationEndDate != null)
            {
                DateTime ExecutionTime = DateTime.Now;
                var priPeriods = DbWMS.dataItemVerificationPeriod.ToList();
                priPeriods.ForEach(x => x.IsLastPeriod = false);
                priPeriods.ForEach(x => x.IsClosed = true);
                DbWMS.UpdateBulk(priPeriods, Permissions.WarehouseItemsEntry.UpdateGuid, ExecutionTime, DbCMS);

                var priWarehusePerids = DbWMS.dataItemVerificationWarehousePeriod.ToList();
                priWarehusePerids.ForEach(x => x.IsClosed = true);
                DbWMS.UpdateBulk(priWarehusePerids, Permissions.WarehouseItemsEntry.UpdateGuid, ExecutionTime, DbCMS);

                Guid EntityPK = Guid.NewGuid();
                dataItemVerificationPeriod verificationPeriod = Mapper.Map(model, new dataItemVerificationPeriod());

                verificationPeriod.ItemVerificationPeriodGUID = EntityPK;
                verificationPeriod.VerificationPeriodName = DateTime.Now.Year + "-" + model.VerificationStartDate.Value.Month;
                verificationPeriod.IsClosed = false;
                verificationPeriod.IsLastPeriod = true;


                verificationPeriod.OrderId = priPeriods.Select(x => x.OrderId).Max() + 1 ?? 1;
                DbWMS.Create(verificationPeriod, Permissions.WarehouseItemsRelease.CreateGuid, ExecutionTime, DbCMS);
                var codewarehouese = DbWMS.codeWarehouse.ToList();
                List<dataItemVerificationWarehousePeriod> warehousePeriods = new List<dataItemVerificationWarehousePeriod>();
                foreach (var item in codewarehouese)
                {
                    dataItemVerificationWarehousePeriod warehousePeriod = new dataItemVerificationWarehousePeriod
                    {
                        VerificationWarehousePeriodGUID = Guid.NewGuid(),
                        WarehouseGUID = item.WarehouseGUID,
                        ItemVerificationPeriodGUID = EntityPK,
                        IsClosed = false,

                    };
                    warehousePeriods.Add(warehousePeriod);
                }
                DbWMS.CreateBulk(warehousePeriods, Permissions.WarehouseItemsRelease.CreateGuid, ExecutionTime, DbCMS);
                var detinput = DbWMS.dataItemInputDetail.Where(x => x.IsDeterminanted == true).ToList();

                detinput.ForEach(x => x.IsVerified = false);
                detinput.ForEach(x => x.VerificationStatusGUID = ItemVerificationStatus.Pending);
                DbWMS.UpdateBulk(detinput, Permissions.WarehouseItemsEntry.UpdateGuid, ExecutionTime, DbCMS);




                try
                {
                    DbWMS.SaveChanges();
                    DbCMS.SaveChanges();



                    return Json(DbCMS.SingleUpdateMessage(DataTableNames.ItemVerificationPeriodsDataTable, DbWMS.PrimaryKeyControl(verificationPeriod), DbWMS.RowVersionControls(Portal.SingleToList(verificationPeriod)), null));

                    //ViewBag.IsReleased = true;
                    //return Json(DbWMS.SingleUpdateMessage(DataTableNames.WarehouseModelReleaseMovementsDataTable, DbWMS.PrimaryKeyControl(ReleaseModelDetail), DbWMS.RowVersionControls(Portal.SingleToList(ReleaseModelDetail))));
                }
                catch (Exception ex)
                {
                    return Json(DbWMS.ErrorMessage(ex.Message));
                }
            }
            else
                return Json(new { success = 0 }, JsonRequestBehavior.AllowGet);

        }
        #endregion
        #region Advanced Search 

        public ActionResult GetReleaseMultidSearch()
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            return View("~/Areas/WMS/Views/ModelReleaseMovements/ReleaseMultidSearch.cshtml", new ReleaseMultiSearchUpdateModel());
        }

        [Route("WMS/WarehouseModelTrackReleaseHistoricals/")]
        public JsonResult WarehouseModelTrackReleaseHistoricalsDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<WarehouseModelTrackStaffHistoricalDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<WarehouseModelTrackStaffHistoricalDataTableModel>(DataTable.Filters);
            }
            var CurrentWarehouseGUID = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).Select(x => x.WarehouseGUID)
                .ToList();
            var warehouseAuthGUIDs = DbWMS.codeWarehouse
                .Where(x => CurrentWarehouseGUID.Contains(x.WarehouseGUID) || CurrentWarehouseGUID.Contains(x.ParentGUID))
                .Select(x => x.WarehouseGUID).ToList();


            //Access is authorized by Access Action
            Guid orgGuid = (Guid)Session[SessionKeys.OrganizationInstanceGUID];
            var All = (from a in DbWMS.v_trackStaffHistorical.Where(x => x.OrganizationInstanceGUID == orgGuid).AsExpandable()
                       select new WarehouseModelTrackStaffHistoricalDataTableModel
                       {

                           ItemOutputDetailGUID = a.ItemOutputDetailGUID,
                           ItemInputGUID = a.ItemInputDetailGUID,

                           dataItemOutputDetailRowVersion = a.dataItemOutputDetailRowVersion,
                           ItemModelWarehouseGUID = a.ItemModelWarehouseGUID.ToString(),
                           ModelDescription = a.ModelDescription,
                           ItemDescription = a.WarehouseItemDescription,
                           WarehouseItemClassificationGUID = a.WarehouseItemClassificationGUID.ToString(),
                           WarehouseItemGUID = a.WarehouseItemGuid.ToString(),
                           BrandGUID = a.BrandGUID.ToString(),
                           LastCustodianName = a.LastCustodianName,
                           CommentsReleas = a.CommentsReleas,
                           Status = a.LastCustodian == "Warehouse" ? "Returned" : (a.RequesterNameGUID != a.LastCustdianNameGUID ? "Returned" : a.LastFlow),
                           ReturnDate = a.ActualReturenedDate,
                           IssueDate = a.ExpectedStartDate,
                           CreatedDate = a.CreatedDate,
                           LicenseStartDate = a.LicenseStartDate,
                           LicenseExpiryDate = a.LicenseExpiryDate,



                           BarcodeNumber = a.BarcodeNumber,
                           SerialNumber = a.SerialNumber,
                           RequesterNameGUID = a.RequesterNameGUID.ToString(),

                           IME1 = a.IMEI,
                           GSM = a.GSM,
                           MAC = a.MAC,



                           ServiceItemStatus = a.ServiceItemStatus,
                           WarehouseOwner = a.WarehosueOwnerName,
                           Governorate = a.LastLocation,
                           WarehouseLocationGUID = a.WarehouseLocationGUID.ToString(),
                           CustodianStaffGUID = a.LastCustdianNameGUID.ToString(),
                           LastCustdianGUID = a.LastCustdianGUID.ToString(),
                           CustodianWarehouseGUID = a.LastCustdianNameGUID.ToString(),
                           Custodian = a.LastCustodianName,
                           ModelStatus = a.ItemStatus,

                           Comments = a.Comments,
                           DeliveryStatus = a.LastFlow,
                           DeliveryStatusGUID = a.LastFlowTypeGUID.ToString(),


                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<WarehouseModelTrackStaffHistoricalDataTableModel> Result = Mapper.Map<List<WarehouseModelTrackStaffHistoricalDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Uploaded Documents 
        public ActionResult WarehouseItemUploadedDocumentsDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/WMS/Views/ModelMovements/_DamagedModelsDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<WarehouseItemUploadedDocumentsDataTable, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<WarehouseItemUploadedDocumentsDataTable>(DataTable.Filters);
            }

            var All = (

                from a in DbWMS.dataItemIntpuDetailUploadedDocument.AsNoTracking().AsExpandable().Where(x => x.Active && x.ItemInputDetailGUID == PK)
                join b in DbWMS.dataItemInputDetail.Where(x => x.Active) on a.ItemInputDetailGUID equals b.ItemInputDetailGUID into LJ1
                from R1 in LJ1.DefaultIfEmpty()

                select new WarehouseItemUploadedDocumentsDataTable
                {
                    ItemIntpuDetailUploadedDocumentGUID = a.ItemIntpuDetailUploadedDocumentGUID.ToString(),
                    ItemInputDetailGUID = a.ItemInputDetailGUID.ToString(),
                    DocumentName = a.DocumentName,
                    FileTypeGUID = a.FileTypeGUID.ToString(),
                    DocumentOrderId = a.DocumentOrderId,
                    Active = a.Active,
                    dataItemIntpuDetailUploadedDocumentRowVersion = a.dataItemIntpuDetailUploadedDocumentRowVersion,


                }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<WarehouseItemUploadedDocumentsDataTable> Result = Mapper.Map<List<WarehouseItemUploadedDocumentsDataTable>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result.OrderByDescending(x => x.DocumentOrderId)), JsonRequestBehavior.AllowGet);
        }
        #region Upload 
        [HttpGet]
        public ActionResult UploadItemDocuments(Guid ItemInputDetailGUID)
        {
            return PartialView("~/Areas/WMS/Views/ItemUploadedDocument/_ItemUploadDocument.cshtml",
                new dataItemIntpuDetailUploadedDocument { ItemInputDetailGUID = ItemInputDetailGUID });
        }
        [HttpPost]
        public FineUploaderResult UploadItemDocuments(FineUpload upload, Guid ItemInputDetailGUID, Guid? FileTypeGUID, string Comments, string DocumentName)
        {

            return new FineUploaderResult(true, new { path = UploadDocument(upload, ItemInputDetailGUID, FileTypeGUID, Comments, DocumentName), success = true });
        }

        public string UploadDocument(FineUpload upload, Guid ItemInputDetailGUID, Guid? FileTypeGUID, string Comments, string DocumentName)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Access, Apps.WMS))
            {
                return "~/Areas/WMS/UploadedDocuments/" + ItemInputDetailGUID + ".xlsx";
            }
            string error = "Error ";


            var _stearm = upload.InputStream;
            DateTime ExecutionTime = DateTime.Now;
            //string _ext = Path.GetExtension(upload.FileName).Split('.')[1];
            dataItemIntpuDetailUploadedDocument documentUplod = new dataItemIntpuDetailUploadedDocument();
            documentUplod.ItemIntpuDetailUploadedDocumentGUID = Guid.NewGuid();
            //string FilePath = Server.MapPath("~/Areas/WMS/UploadedDocuments/" + documentUplod.ItemIntpuDetailUploadedDocumentGUID + _ext);

            string _ext = Path.GetExtension(upload.FileName).Split('.')[1];
            string FolderPath = Server.MapPath("~/Areas/WMS/UploadedDocuments/" + ItemInputDetailGUID.ToString());
            Directory.CreateDirectory(FolderPath);
            int LatestFileVersion = 0;
            //try { LatestFileVersion = (from a in DbPPA.PPAFileVersion.Where(x => x.PPAOriginalFileGUID == PPAOriginalFileGUID && x.FileActionByUserGUID == UserGUID) select a.FileVersion).Max(); } catch { }
            //if (LatestFileVersion == -1) LatestFileVersion = 0;



            string FilePath = FolderPath + "/" + documentUplod.ItemIntpuDetailUploadedDocumentGUID.ToString() + "." + _ext;

            using (var fileStream = System.IO.File.Create(FilePath))
            {
                upload.InputStream.Seek(0, SeekOrigin.Begin);
                upload.InputStream.CopyTo(fileStream);
            }
            documentUplod.ItemInputDetailGUID = ItemInputDetailGUID;
            documentUplod.DocumentOrderId = DbWMS.dataItemIntpuDetailUploadedDocument.Where(x => x.ItemInputDetailGUID == ItemInputDetailGUID).Select(f => f.DocumentOrderId).Max() != null ? DbWMS.dataItemIntpuDetailUploadedDocument.Where(x => x.ItemInputDetailGUID == ItemInputDetailGUID).Select(f => f.DocumentOrderId).Max() + 1 : 1;
            documentUplod.DocumentName = DocumentName;
            documentUplod.Comments = Comments;
            documentUplod.FileTypeGUID = FileTypeGUID;
            //documentUplod.Comments = ItemInputDetailGUID;
            documentUplod.CreatedByGUID = UserGUID;
            documentUplod.CreatedDate = ExecutionTime;
            DbWMS.Create(documentUplod, Permissions.WarehouseItemsRelease.CreateGuid, ExecutionTime, DbCMS);
            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
            }
            catch (Exception)
            {

                throw;
            }


            //Server.MapPath("~/Areas/WMS/temp/Standard_Operating_Procedure_for_issuing_ICT_equipment_to_staff" + DateTime.Now.ToBinary() + ".pdf");


            return "~/Areas/WMS/UploadedDocuments/" + documentUplod.ItemIntpuDetailUploadedDocumentGUID + ".xlsx";
        }




        #endregion
        #endregion

        #region STaff Items
        //[Route("WMS/StaffCustodyWarehosueItemsDataTable/{PK}")]
        public JsonResult StaffCustodyWarehosueItemsDataTable(DataTableRecievedOptions options, Guid PK)
        {
            var e = Request;

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<WarehouseModelEntryMovementDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<WarehouseModelEntryMovementDataTableModel>(DataTable.Filters);
            }


            Guid orgGuid = (Guid)Session[SessionKeys.OrganizationInstanceGUID];


            var All = (from a in DbWMS.v_EntryMovementDataTable.Where(x => x.LastCustdianNameGUID == PK && x.OrganizationInstanceGUID == orgGuid).AsExpandable().Where(x =>
                  //warehouseAuthGUIDs.Contains((Guid)x.WarehouseOwnerGUID) && 
                  (x.IsDeterminanted == true || x.IsDeterminanted == null))
                       select new WarehouseModelEntryMovementDataTableModel
                       {


                           ItemInputDetailGUID = a.ItemInputDetailGUID,
                           Active = (bool)a.Active,
                           LastVerifiedByGUID = a.LastVerifiedByGUID.ToString(),
                           VerificationStatusGUID = a.VerificationStatusGUID.ToString(),
                           ItemModelWarehouseGUID = a.ItemModelWarehouseGUID.ToString(),

                           ModelDescription = a.ModelDescription,
                           ItemDescription = a.WarehouseItemDescription,
                           WarehouseItemClassificationGUID = a.WarehouseItemClassificationGUID.ToString(),
                           WarehouseItemGUID = a.WarehouseItemGuid.ToString(),
                           BrandGUID = a.BrandGUID.ToString(),
                           WarehouseOwnerGUID = a.WarehouseOwnerGUID.ToString(),
                           BarcodeNumber = a.BarcodeNumber,
                           SerialNumber = a.SerialNumber,
                           BillNumber = a.BillNumber,
                           IME1 = a.IMEI,
                           GSM = a.GSM,
                           MAC = a.MAC,
                           Identifier = a.BarcodeNumber.Length > 0 ? a.BarcodeNumber : (a.SerialNumber.Length > 0 ? a.SerialNumber : (a.IMEI1.Length > 0 ? a.IMEI1 : (a.GSM.Length > 0 ? a.GSM : (a.MAC.Length > 0 ? a.MAC : (a.SequenceNumber.Length > 0 ? a.SequenceNumber : ""))))),
                           MSRPID = a.MSRPID,
                           ItemCondition = a.ItemCondition,
                           ItemConditionGUID = a.ItemConditionGUID.ToString(),
                           ItemServiceStatusGUID = a.ItemServiceStatusGUID.ToString(),
                           ServiceItemStatus = a.ServiceItemStatus,
                           WarehouseOwner = a.WarehosueOwnerName,
                           Governorate = a.LastLocation,
                           WarehouseLocationGUID = a.WarehouseLocationGUID.ToString(),
                           CustodianStaffGUID = a.LastCustdianNameGUID.ToString(),
                           LastCustdianGUID = a.LastCustdianGUID.ToString(),
                           CustodianWarehouseGUID = a.LastCustdianNameGUID.ToString(),
                           Custodian = a.LastCustodianName,
                           ModelStatus = a.ItemStatus,
                           ModelStatusGUID = a.ItemStatusGUID.ToString(),
                           CreatedDate = a.CreatedDate,
                           PurposeofuseGUID = a.PurposeofuseGUID,



                           Purposeofuse = a.Purposeofuse,
                           CreatedBy = a.CreatedBy,
                           CreatedByGUID = a.CreatedByGUID.ToString(),
                           Comments = a.Comments,
                           DeliveryStatus = a.LastFlow,
                           DeliveryStatusGUID = a.LastFlowTypeGUID.ToString(),

                           dataItemInputDetailRowVersion = a.dataItemInputDetailRowVersion
                       }).Where(Predicate);



            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<WarehouseModelEntryMovementDataTableModel> Result = Mapper.Map<List<WarehouseModelEntryMovementDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }


        #endregion

        public ActionResult ExportStaffSTIItems(Guid id)
        {
            Guid orgGuid = (Guid)Session[SessionKeys.OrganizationInstanceGUID];
            var result = DbWMS.v_EntryMovementDataTable.Where(x => x.LastCustdianNameGUID == id && x.OrganizationInstanceGUID == orgGuid).ToList();

            if (result.Count > 0)
            {
                string sourceFile = Server.MapPath("~/Areas/WMS/Templates/StaffSTIItems.xlsx");
                string DisFolder =
                    Server.MapPath("~/Areas/WMS/temp/Staff_STI_Items_Report" + DateTime.Now.ToBinary() + ".xlsx");
                System.IO.File.Copy(sourceFile, DisFolder);
                using (ExcelPackage package = new ExcelPackage(new FileInfo(DisFolder)))
                {
                    var cx = package.Workbook.Worksheets.ToList();
                    ExcelWorksheet workSheet = package.Workbook.Worksheets[1];

                    DataTable dt = new DataTable();
                    dt.Columns.Add("Model Name", typeof(string));
                    dt.Columns.Add("Barcode", typeof(string));
                    dt.Columns.Add("SN", typeof(string));
                    dt.Columns.Add("IMEI", typeof(string));
                    dt.Columns.Add("GSM", typeof(string));
                    dt.Columns.Add("Confirmation Status", typeof(string));


                    foreach (var item in result)
                    {

                        DataRow dr;
                        dr = dt.NewRow();
                        dr[0] = item.ModelDescription;
                        dr[1] = item.BarcodeNumber;
                        dr[2] = item.SerialNumber;
                        dr[3] = item.IMEI;
                        dr[4] = item.GSM;
                        dr[5] = item.LastFlow;

                        dt.Rows.Add(dr);
                    }
                    workSheet.Cells["B5"].LoadFromDataTable(dt, true);
                    workSheet.Cells["B1"].Value = "STI Items For " + result.Select(x => x.LastCustodianName).FirstOrDefault();

                    package.Save();
                }

                byte[] fileBytes = System.IO.File.ReadAllBytes(DisFolder);

                string fileName = "STI Items For " + result.Select(x => x.LastCustodianName).FirstOrDefault() + ".xlsx";
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }

            string success = "No Data Available for this Staff";
            return Json(new { success = success }, JsonRequestBehavior.AllowGet);

        }

        #region Item Features
        public ActionResult WarehouseItemDetailFeatureDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/WMS/Views/ItemDetailFeature/_ItemDetailFeatureDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<WarehouseItemDetailFeatureDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<WarehouseItemDetailFeatureDataTableModel>(DataTable.Filters);
            }

            var Result = (

                from a in DbWMS.dataItemInputDetailFeature.AsNoTracking().AsExpandable().Where(x => x.Active && x.ItemInputDetailGUID == PK)
                join b in DbWMS.dataItemInputDetail.Where(x => x.Active) on a.ItemInputDetailGUID equals b.ItemInputDetailGUID into LJ1
                from R1 in LJ1.DefaultIfEmpty()
                join c in DbWMS.codeWMSFeatureTypeValue.Where(x => x.Active) on a.FeatureTypeValueGUID equals c.FeatureTypeValueGUID into LJ4
                from R4 in LJ4.DefaultIfEmpty()
                join d in DbWMS.codeWMSFeatureType.Where(x => x.Active) on R4.FeatureTypeGUID equals d.FeatureTypeGUID into LJ2
                from R2 in LJ2.DefaultIfEmpty()


                join e in DbWMS.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.CreatedByGUID equals e.UserGUID into LJ3
                from R3 in LJ3.DefaultIfEmpty()
                    //join L in DbWMS.codeWarehouse.Where(x => x.Active) on R1.RequesterNameGUID equals L.WarehouseGUID
                    //join M in DbWMS.codeWarehouseLanguage.Where(x => x.Active && x.LanguageID == LAN) on L.WarehouseGUID equals M.WarehouseGUID into LJ8
                    //from R8 in LJ8.DefaultIfEmpty()

                select new WarehouseItemDetailFeatureDataTableModel
                {

                    ItemInputDetailFeatureGUID = a.ItemInputDetailFeatureGUID,
                    ItemInputDetailGUID = a.ItemInputDetailGUID,
                    FeatureTypeValueGUID = a.FeatureTypeValueGUID.ToString(),
                    FeatureTypeGUID = R4.FeatureTypeGUID.ToString(),
                    FileExtType = a.FileExtType,
                    FeatureTypeCategory = R4.Name,
                    FeatureName = R2.Name,
                    StartDate = a.StartDate,
                    EndDate = a.EndDate,


                    FeatureValue = a.FeatureValue,
                    CreateBy = R3.FirstName + " " + R3.Surname,

                    Active = a.Active,

                    dataItemInputDetailFeatureRowVersion = a.dataItemInputDetailFeatureRowVersion,

                }).Where(Predicate);


            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }


        //[Route("WMS/StaffBankAccountDataTable/{PK}")]


        public ActionResult WarehouseItemDetailFeatureCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            return PartialView("~/Areas/WMS/Views/ItemDetailFeature/_ItemDetailFeatureUpdateModal.cshtml",
                new WarehouseItemDetailFeatureUpdateModel { ItemInputDetailGUID = FK });
        }

        public ActionResult DownloadItemFeatureFile(Guid id)
        {



            var model = DbWMS.dataItemInputDetailFeature.Where(x => x.ItemInputDetailFeatureGUID == id).FirstOrDefault();
            var fullPath = model.ItemInputDetailFeatureGUID + "." + model.FileExtType;


            string sourceFile = Server.MapPath("~/Areas/WMS/UploadedDocuments/ItemFeatures/" + "/" + fullPath);


            byte[] fileBytes = System.IO.File.ReadAllBytes(sourceFile);

            string fileName = DateTime.Now.ToString("yyMMdd") + fullPath;
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);








            // return Json(DbCMS.SingleUpdateMessage(null, null, null, null, "Please Wait...."));
        }



        [HttpPost]
        public FineUploaderResult UploadWarehouseItemDetailFeatures(FineUpload upload, Guid ItemInputDetailGUID, Guid FeatureTypeValueGUID, string FeatureValue, DateTime? StartDate, DateTime? EndDate, string Comments)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Access, Apps.WMS))
            {
                return new FineUploaderResult(true, new { path = UploadDocument(upload, Guid.Empty, Guid.Empty, "", StartDate, EndDate, Comments), success = true });
            }
            string error = "Error ";

            if (FileTypeValidator.IsPDF(upload.InputStream) || FileTypeValidator.IsImage(upload.InputStream) ||
                FileTypeValidator.IsExcel(upload.InputStream) ||
                FileTypeValidator.IsWord(upload.InputStream)
                )
            {
                return new FineUploaderResult(true, new { path = UploadDocument(upload, ItemInputDetailGUID, FeatureTypeValueGUID, FeatureValue, StartDate, EndDate, Comments), success = true });
            }
            return new FineUploaderResult((error == "" ? true : false), new { Error = error, path = "", success = (error == "" ? true : false) });
        }

        public string UploadDocument(FineUpload upload, Guid ItemInputDetailGUID, Guid FeatureTypeValueGUID, string FeatureValue, DateTime? StartDate, DateTime? EndDate, string Comments)
        {
            if (ItemInputDetailGUID == Guid.Empty)
            {
                return "~/Areas/WMS/UploadedDocuments/ItemFeatures/" + ItemInputDetailGUID + ".xlsx";
            }
            var _stearm = upload.InputStream;
            DateTime ExecutionTime = DateTime.Now;
            //string _ext = Path.GetExtension(upload.FileName).Split('.')[1];
            dataItemInputDetailFeature _itemFeature = new dataItemInputDetailFeature();
            _itemFeature.ItemInputDetailFeatureGUID = Guid.NewGuid();


            string _ext = Path.GetExtension(upload.FileName).Split('.')[1];

            string FolderPath = Server.MapPath("/Areas/WMS/UploadedDocuments/ItemFeatures/");
            Directory.CreateDirectory(FolderPath);
            //int LatestFileVersion = 0;
            //try { LatestFileVersion = (from a in DbPPA.PPAFileVersion.Where(x => x.PPAOriginalFileGUID == PPAOriginalFileGUID && x.FileActionByItemInputDetailGUID == ItemInputDetailGUID) select a.FileVersion).Max(); } catch { }
            //if (LatestFileVersion == -1) LatestFileVersion = 0;



            string FilePath = FolderPath + "/" + _itemFeature.ItemInputDetailFeatureGUID.ToString() + "." + _ext;

            using (var fileStream = System.IO.File.Create(FilePath))
            {
                upload.InputStream.Seek(0, SeekOrigin.Begin);
                upload.InputStream.CopyTo(fileStream);
            }
            _itemFeature.ItemInputDetailGUID = ItemInputDetailGUID;
            _itemFeature.FeatureTypeValueGUID = FeatureTypeValueGUID;
            _itemFeature.FileExtType = _ext;
            _itemFeature.FeatureValue = FeatureValue;
            _itemFeature.StartDate = StartDate;
            _itemFeature.EndDate = EndDate;
            _itemFeature.CreateDate = ExecutionTime;
            _itemFeature.Comments = Comments;
            _itemFeature.CreatedByGUID = UserGUID;

            //_itemFeature.Comments = ItemInputDetailGUID;
            //_itemFeature.CreatedByGUID = ItemInputDetailGUID;
            //_itemFeature.CreatedDate = ExecutionTime;
            DbWMS.Create(_itemFeature, Permissions.WarehouseItemsEntry.CreateGuid, ExecutionTime, DbCMS);
            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
            }
            catch (Exception)
            {

                throw;
            }


            //Server.MapPath("~/Areas/WMS/temp/Standard_Operating_Procedure_for_issuing_ICT_equipment_to_staff" + DateTime.Now.ToBinary() + ".pdf");


            return "~/Areas/WMS/UploadedDocuments/ItemFeatures/" + _itemFeature.ItemInputDetailFeatureGUID + ".xlsx";
        }




        public ActionResult WarehouseItemDetailFeatureUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Access, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            WarehouseItemDetailFeatureUpdateModel model = DbWMS.dataItemInputDetailFeature.Where(x => x.ItemInputDetailFeatureGUID == PK).Select(f => new WarehouseItemDetailFeatureUpdateModel
            {

                ItemInputDetailGUID = (Guid)f.ItemInputDetailGUID,
                FeatureTypeValueGUID = f.FeatureTypeValueGUID,
                FileExtType = f.FileExtType,
                FeatureValue = f.FeatureValue,
                ItemInputDetailFeatureGUID = f.ItemInputDetailFeatureGUID,
                EndDate = f.EndDate,
                StartDate = f.StartDate,
                CreatedByGUID = f.CreatedByGUID,
                CreateDate = f.CreateDate,
                Comments = f.Comments,

                Active = f.Active

            }).FirstOrDefault();
            return PartialView("~/Areas/WMS/Views/ItemDetailFeature/_ItemDetailFeatureUpdateModal.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult WarehouseItemDetailFeatureCreate(dataItemInputDetailFeature model)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Create, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if ((model.FeatureTypeValueGUID == null)) return PartialView("~/Areas/WMS/Views/ItemDetailFeature/_ItemDetailFeatureUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbWMS.Create(model, Permissions.WarehouseItemsEntry.CreateGuid, ExecutionTime, DbCMS);

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleUpdateMessage(DataTableNames.WarehouseItemDetailFeatureDataTable, DbWMS.PrimaryKeyControl(model), DbWMS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult WarehouseItemDetailFeatureUpdate(dataItemInputDetailFeature model)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Update, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (!ModelState.IsValid || model.ItemInputDetailGUID == null || model.FeatureTypeValueGUID == null) return PartialView("~/Areas/WMS/Views/ItemDetailFeature/StaffBankUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbWMS.Update(model, Permissions.WarehouseItemsEntry.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleUpdateMessage(DataTableNames.WarehouseItemDetailFeatureDataTable,
                    DbWMS.PrimaryKeyControl(model),
                    DbWMS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyWarehouseItemDetailFeature(model.ItemInputDetailFeatureGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult WarehouseItemDetailFeatureDelete(dataItemInputDetailFeature model)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Delete, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            List<dataItemInputDetailFeature> DeletedLanguages = DeleteWarehouseItemDetailFeature(new List<dataItemInputDetailFeature> { model });

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleDeleteMessage(DeletedLanguages, DataTableNames.WarehouseItemDetailFeatureDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyWarehouseItemDetailFeature(model.ItemInputDetailFeatureGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult WarehouseItemDetailFeatureRestore(dataItemInputDetailFeature model)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Restore, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            if (ActiveWarehouseItemDetailFeature(model))
            {
                return Json(DbWMS.RecordExists());
            }

            List<dataItemInputDetailFeature> RestoredLanguages = RestoreWarehouseItemDetailFeature(Portal.SingleToList(model));

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.SingleRestoreMessage(RestoredLanguages, DataTableNames.WarehouseItemDetailFeatureDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyWarehouseItemDetailFeature(model.ItemInputDetailFeatureGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult WarehouseItemDetailFeatureDataTableDelete(List<dataItemInputDetailFeature> models)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Delete, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            List<dataItemInputDetailFeature> DeletedLanguages = DeleteWarehouseItemDetailFeature(models);

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.WarehouseItemDetailFeatureDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult WarehouseItemDetailFeatureDataTableModelRestore(List<dataItemInputDetailFeature> models)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Restore, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            List<dataItemInputDetailFeature> RestoredLanguages = RestoreWarehouseItemDetailFeature(models);

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.WarehouseItemDetailFeatureDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        private List<dataItemInputDetailFeature> DeleteWarehouseItemDetailFeature(List<dataItemInputDetailFeature> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<dataItemInputDetailFeature> DeletedStaffBankAccount = new List<dataItemInputDetailFeature>();

            string query = DbWMS.QueryBuilder(models, Permissions.WarehouseItemsEntry.DeleteGuid, SubmitTypes.Delete, "");

            var languages = DbWMS.Database.SqlQuery<dataItemInputDetailFeature>(query).ToList();

            foreach (var language in languages)
            {
                DeletedStaffBankAccount.Add(DbWMS.Delete(language, ExecutionTime, Permissions.WarehouseItemsEntry.DeleteGuid, DbCMS));
            }

            return DeletedStaffBankAccount;
        }

        private List<dataItemInputDetailFeature> RestoreWarehouseItemDetailFeature(List<dataItemInputDetailFeature> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<dataItemInputDetailFeature> RestoredLanguages = new List<dataItemInputDetailFeature>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbWMS.QueryBuilder(models, Permissions.WarehouseItemsEntry.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Languages = DbWMS.Database.SqlQuery<dataItemInputDetailFeature>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveWarehouseItemDetailFeature(language))
                {
                    RestoredLanguages.Add(DbWMS.Restore(language, Permissions.WarehouseItemsEntry.DeleteGuid, Permissions.WarehouseItemsEntry.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyWarehouseItemDetailFeature(Guid PK)
        {
            dataItemInputDetailFeature dbModel = new dataItemInputDetailFeature();

            var Language = DbWMS.dataItemInputDetailFeature.Where(l => l.ItemInputDetailFeatureGUID == PK).FirstOrDefault();
            var dbLanguage = DbWMS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.dataItemInputDetailFeatureRowVersion.SequenceEqual(dbModel.dataItemInputDetailFeatureRowVersion))
            {
                return Json(DbWMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbWMS, dbModel, "LanguagesContainer"));
        }

        private bool ActiveWarehouseItemDetailFeature(dataItemInputDetailFeature model)
        {
            int LanguageID = DbWMS.dataItemInputDetailFeature
                                  .Where(x =>
                                              x.ItemInputDetailGUID == model.ItemInputDetailGUID &&
                                              x.FeatureTypeValueGUID == model.FeatureTypeValueGUID &&
                                              x.StartDate == model.StartDate &&

                                              x.Active).Count();
            if (LanguageID > 0)
            {
                //Fix
                ModelState.AddModelError("LanguageID", "Already Exist");
            }

            return (LanguageID > 0);
        }
        #endregion


        public ActionResult GetTemplateFile()
        {
            string sourceFile = Server.MapPath("~/Areas/WMS/Templates/ModelEntryTemplate.xlsx");
            string DisFolder =
                Server.MapPath("~/Areas/WMS/temp/ModelEntryTemplate" + DateTime.Now.ToBinary() + ".xlsx");
            System.IO.File.Copy(sourceFile, DisFolder);
            byte[] fileBytes = System.IO.File.ReadAllBytes(DisFolder);
            string fileName = "ModelEntryTemplate " + DateTime.Now.ToString("yyMMdd") + ".xlsx";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }


    }
}