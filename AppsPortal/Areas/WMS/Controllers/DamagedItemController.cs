using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.ViewModels;
using AutoMapper;
using LinqKit;
using RES_Repo.Globalization;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WMS_DAL.Model;
using WMS_DAL.ViewModels;
namespace AppsPortal.Areas.WMS.Controllers
{
    public class DamagedItemController : WMSBaseController
    {
        #region All Damaged Report 
        [Route("WMS/AllDamagedItemRequests/")]
        public ActionResult AllDamagedItemRequestsIndex()
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Access, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }

            return View("~/Areas/WMS/Views/DamagedItem/ICT/Index.cshtml");
        }

        public JsonResult DamagedItemRequestsByItemDataTable(DataTableRecievedOptions options, Guid PK)
        {

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<STIItemDamagedTrackDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<STIItemDamagedTrackDataTableModel>(DataTable.Filters);
            }
            var currentWarehouseGUID = DbWMS.codeWarehouseFocalPoint.Where(x => x.UserGUID == UserGUID).Select(x => x.WarehouseGUID)
                .FirstOrDefault();
            var currInstance = DbWMS.codeWarehouse.Where(x => x.WarehouseGUID == currentWarehouseGUID).FirstOrDefault();
            var _checkDetailGUIDs = DbWMS.v_EntryMovementDataTable.Where(x => x.OrganizationInstanceGUID == currInstance.OrganizationInstanceGUID).Select(x => x.ItemInputDetailGUID).ToList();
            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.StaffSalaryProcess.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();
            //Fix

            var All = (
                from a in DbWMS.dataItemOutputDetailDamagedTrack.Where(x => x.Active && x.ItemInputDetailGUID == PK).AsExpandable()
                join b in DbWMS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.LastFlowStatusGUID equals b.ValueGUID into LJ1
                from R1 in LJ1.DefaultIfEmpty()
                join c in DbWMS.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.DamagedByGUID equals c.UserGUID into LJ2
                from R2 in LJ2.DefaultIfEmpty()
                join d in DbWMS.v_EntryMovementDataTable.Where(x => x.Active) on a.ItemInputDetailGUID equals d.ItemInputDetailGUID into LJ3
                from R3 in LJ3.DefaultIfEmpty()
                join d in DbWMS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.Active) on a.ICTFocalPointGUID equals d.UserGUID into LJ4
                from R4 in LJ4.DefaultIfEmpty()
                join e in DbWMS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.Active) on a.AdminFocalPointReviewerGUID equals e.UserGUID into LJ5
                from R5 in LJ5.DefaultIfEmpty()
                join f in DbWMS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.Active) on a.FinanceFocalPointReviewerGUID equals f.UserGUID into LJ6
                from R6 in LJ6.DefaultIfEmpty()

                join g in DbWMS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.StaffReimburseStatusGUID equals g.ValueGUID into LJ7
                from R7 in LJ7.DefaultIfEmpty()
                select new STIItemDamagedTrackDataTableModel
                {
                    ItemOutputDetailDamagedTrackGUID = a.ItemOutputDetailDamagedTrackGUID.ToString(),


                    Active = a.Active,
                    DocumentReference = a.DocumentReference,
                    StaffName = R2.FirstName + " " + R2.Surname,
                    DamagedByGUID = a.DamagedByGUID.ToString(),
                    DamagedTypeGUID = a.DamagedTypeGUID.ToString(),

                    DutyStationGUID = a.DutyStationGUID.ToString(),
                    LastFlowStatusGUID = a.LastFlowStatusGUID.ToString(),
                    LastFlowStatus = R1.ValueDescription,
                    IncidentDate = a.DamagedDate,
                    ItemInputDetailGUID = a.ItemInputDetailGUID.ToString(),
                    ItemName = R3.WarehouseItemDescription + " " + R3.ModelDescription,
                    ModelName = R3.ModelDescription,
                    Barcode = R3.BarcodeNumber,
                    SerialNumber = R3.SerialNumber,
                    IMEI = R3.IMEI,
                    GSM = R3.GSM,
                    MAC = R3.MAC,
                    ICTApprovedBy = R4.FirstName + " " + R4.Surname,
                    AdminApprovedBy = R5.FirstName + " " + R5.Surname,
                    FinanceApprovedBy = R6.FirstName + " " + R6.Surname,

                    ReimburseStatus = R7.ValueDescription,
                    ReimbursementAmount = a.ReimbursementAmount.ToString(),


                    //StaffName = R3.FirstName + " " + R3.SurName,





                    dataItemOutputDetailDamagedTrackRowVersion = a.dataItemOutputDetailDamagedTrackRowVersion
                }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<STIItemDamagedTrackDataTableModel> Result = Mapper.Map<List<STIItemDamagedTrackDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).OrderByDescending(x => x.IncidentDate).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }
        [Route("WMS/AllDamagedItemRequestsDataTable/")]
        public JsonResult AllDamagedItemRequestsDataTable(DataTableRecievedOptions options)
        {

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<STIItemDamagedTrackDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<STIItemDamagedTrackDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.StaffSalaryProcess.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();
            //Fix

            var All = (
                from a in DbWMS.dataItemOutputDetailDamagedTrack.Where(x => x.Active).AsExpandable()
                join b in DbWMS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.LastFlowStatusGUID equals b.ValueGUID into LJ1
                from R1 in LJ1.DefaultIfEmpty()
                join c in DbWMS.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.DamagedByGUID equals c.UserGUID into LJ2
                from R2 in LJ2.DefaultIfEmpty()
                join d in DbWMS.v_EntryMovementDataTable.Where(x => x.Active) on a.ItemInputDetailGUID equals d.ItemInputDetailGUID into LJ3
                from R3 in LJ3.DefaultIfEmpty()
                join d in DbWMS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.Active) on a.ICTFocalPointGUID equals d.UserGUID into LJ4
                from R4 in LJ4.DefaultIfEmpty()
                join e in DbWMS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.Active) on a.AdminFocalPointReviewerGUID equals e.UserGUID into LJ5
                from R5 in LJ5.DefaultIfEmpty()
                join f in DbWMS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.Active) on a.FinanceFocalPointReviewerGUID equals f.UserGUID into LJ6
                from R6 in LJ6.DefaultIfEmpty()

                join g in DbWMS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.StaffReimburseStatusGUID equals g.ValueGUID into LJ7
                from R7 in LJ7.DefaultIfEmpty()

                join ee in DbWMS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.DamagedTypeGUID equals ee.ValueGUID into LJ44
                from R44 in LJ44.DefaultIfEmpty()
                join ff in DbWMS.v_StaffAllDataInformation on a.DamagedByGUID equals ff.UserGUID into LJ55
                from R55 in LJ55.DefaultIfEmpty()


                select new STIItemDamagedTrackDataTableModel
                {
                    ItemOutputDetailDamagedTrackGUID = a.ItemOutputDetailDamagedTrackGUID.ToString(),


                    Active = a.Active,
                    DocumentReference = a.DocumentReference,
                    StaffName = R2.FirstName + " " + R2.Surname,
                    DamagedByGUID = a.DamagedByGUID.ToString(),

                    DutyStationGUID = a.DutyStationGUID.ToString(),
                    LastFlowStatusGUID = a.LastFlowStatusGUID.ToString(),
                    LastFlowStatus = R1.ValueDescription,
                    IncidentDate = a.DamagedDate,
                    ItemInputDetailGUID = a.ItemInputDetailGUID.ToString(),
                    ItemName = R3.WarehouseItemDescription + " " + R3.ModelDescription,
                    ModelName = R3.ModelDescription,
                    Barcode = R3.BarcodeNumber,
                    SerialNumber = R3.SerialNumber,
                    Identifier = R3.BarcodeNumber != null ? R3.BarcodeNumber : R3.SerialNumber != null ? R3.GSM : R3.GSM != null ? R3.GSM : R3.IMEI != null ? R3.IMEI : "",
                    IncidentType = R44.ValueDescription,
                    DamagedTypeGUID = a.DamagedTypeGUID.ToString(),
                    DamagedReason = a.DamagedReason,
                    DutyStation = R55.DutyStation,


                    IMEI = R3.IMEI,
                    GSM = R3.GSM,
                    MAC = R3.MAC,
                    ICTApprovedBy = R4.FirstName + " " + R4.Surname,
                    AdminApprovedBy = R5.FirstName + " " + R5.Surname,
                    FinanceApprovedBy = R6.FirstName + " " + R6.Surname,

                    ReimburseStatus = R7.ValueDescription,
                    ReimbursementAmount = a.ReimbursementAmount.ToString(),


                    //StaffName = R3.FirstName + " " + R3.SurName,





                    dataItemOutputDetailDamagedTrackRowVersion = a.dataItemOutputDetailDamagedTrackRowVersion
                }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<STIItemDamagedTrackDataTableModel> Result = Mapper.Map<List<STIItemDamagedTrackDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).OrderByDescending(x => x.IncidentDate).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ICTDamagedItemUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Access, Apps.WMS))
            {
                return Json(DbWMS.PermissionError());
            }
            var _result = DbWMS.dataItemOutputDetailDamagedTrack.Where(x => x.ItemOutputDetailDamagedTrackGUID == PK).FirstOrDefault();
            STIItemDamagedTrackUpdateModel model = Mapper.Map(_result, new STIItemDamagedTrackUpdateModel());
            var _staff = DbWMS.userPersonalDetailsLanguage.Where(x => x.UserGUID == model.DamagedByGUID && x.LanguageID == LAN).FirstOrDefault();
            ViewBag.StaffName = _staff.FirstName + " " + _staff.Surname;
            var _item = DbWMS.v_EntryMovementDataTable.Where(x => x.ItemInputDetailGUID == _result.ItemInputDetailGUID).FirstOrDefault();
            model.ItemName = _item.WarehouseItemDescription + " " + _item.ModelDescription;
            if (_result.ItemConditionWhenReceivedGUID != null)
            {
                model.ItemCondition = DbWMS.codeTablesValuesLanguages.Where(x => x.ValueGUID == _result.ItemConditionWhenReceivedGUID && x.LanguageID == LAN).FirstOrDefault().ValueDescription;
            }
            if (_result.PurposeOfUseGUID != null)
            {
                model.PurposeOfUse = DbWMS.codeTablesValuesLanguages.Where(x => x.ValueGUID == _result.PurposeOfUseGUID && x.LanguageID == LAN).FirstOrDefault().ValueDescription;
            }
            if (_result.StaffReimburseStatusGUID != null)
            {
                var _Reimbursan = DbWMS.codeTablesValuesLanguages.Where(x => x.ValueGUID == _result.StaffReimburseStatusGUID).FirstOrDefault();
                model.ReimburseStatusDecision = _Reimbursan.ValueDescription;
            }
            //ViewBag.ItemOutputDetailDamagedTrackGUID = PK;
            //ViewBag.TotalStaffNotConfirmed = DbWMS.dataItemOutputDetailDamagedTrack.Where(a => a.FlowStatusGUID == NationalStaffDangerPaConfirmationStatus.Pending && a.ItemOutputDetailDamagedTrackGUID == PK).Count();
            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("StaffDamagedItem", "StaffDamagedItems", new { Area = "WMS" }));
            return View("~/Areas/WMS/Views/DamagedItem/ICT/ReviewICTDamagedItemForm.cshtml", model);

        }
        #endregion
        // GET: WMS/DamagedItem
        #region Staff Damaged Item Report

        [Route("WMS/DamagedItem/WorkplaceStaffDamagedItemIndex/")]
        public ActionResult StaffDamagedItemIndex()
        {
            //if (!CMS.HasAction(Permissions.StaffDamagedItem.Access, Apps.WMS))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}

            return View("~/Areas/WMS/Views/DamagedItem/Staff/Index.cshtml");
        }
        [Route("WMS/STIItemDamagedTrackDataTable/")]
        public JsonResult STIItemDamagedTrackDataTable(DataTableRecievedOptions options)
        {

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<STIItemDamagedTrackDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<STIItemDamagedTrackDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.StaffSalaryProcess.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();
            //Fix

            var All = (
                from a in DbWMS.dataItemOutputDetailDamagedTrack.Where(x => x.Active && x.DamagedByGUID == UserGUID).AsExpandable()
                join b in DbWMS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.LastFlowStatusGUID equals b.ValueGUID into LJ1
                from R1 in LJ1.DefaultIfEmpty()
                join c in DbWMS.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.DamagedByGUID equals c.UserGUID into LJ2
                from R2 in LJ2.DefaultIfEmpty()
                join d in DbWMS.v_EntryMovementDataTable.Where(x => x.Active) on a.ItemInputDetailGUID equals d.ItemInputDetailGUID into LJ3
                from R3 in LJ3.DefaultIfEmpty()
                join e in DbWMS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.DamagedTypeGUID equals e.ValueGUID into LJ4
                from R4 in LJ4.DefaultIfEmpty()
                join f in DbWMS.v_StaffAllDataInformation on a.DamagedByGUID equals f.UserGUID into LJ5
                from R5 in LJ5.DefaultIfEmpty()
                select new STIItemDamagedTrackDataTableModel
                {
                    ItemOutputDetailDamagedTrackGUID = a.ItemOutputDetailDamagedTrackGUID.ToString(),


                    Active = a.Active,
                    DocumentReference = a.DocumentReference,
                    StaffName = R2.FirstName + " " + R2.Surname,
                    DamagedByGUID = a.DamagedByGUID.ToString(),
                    IncidentType = R4.ValueDescription,
                    DamagedReason = a.DamagedReason,
                    DutyStation = R5.DutyStation,
                    Identifier = R3.BarcodeNumber != null ? R3.BarcodeNumber : R3.SerialNumber != null ? R3.GSM : R3.GSM != null ? R3.GSM : R3.IMEI != null ? R3.IMEI : "",

                    DutyStationGUID = a.DutyStationGUID.ToString(),
                    LastFlowStatusGUID = a.LastFlowStatusGUID.ToString(),
                    LastFlowStatus = R1.ValueDescription,
                    IncidentDate = a.DamagedDate,
                    ItemInputDetailGUID = a.ItemInputDetailGUID.ToString(),
                    ItemName = R3.WarehouseItemDescription + " " + R3.ModelDescription,
                    ModelName = R3.ModelDescription,
                    Barcode = R3.BarcodeNumber,
                    SerialNumber = R3.SerialNumber,
                    IMEI = R3.IMEI,
                    MAC = R3.MAC,
                    GSM = R3.GSM,




                    //StaffName = R3.FirstName + " " + R3.SurName,





                    dataItemOutputDetailDamagedTrackRowVersion = a.dataItemOutputDetailDamagedTrackRowVersion
                }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<STIItemDamagedTrackDataTableModel> Result = Mapper.Map<List<STIItemDamagedTrackDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).OrderByDescending(x => x.IncidentDate).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }
        //[Route("WMS/StaffDamagedItem/Create/")]

        [Route("WMS/StaffItemDamaged/StaffDamagedItemICTCreate/")]
        public ActionResult StaffDamagedItemICTCreate()
        {
            //if (!CMS.HasAction(Permissions.StaffSalaryProcess.Create, Apps.WMS))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}

            STIItemDamagedTrackUpdateModel model = new STIItemDamagedTrackUpdateModel
            {
                //ItemOutputDetailDamagedTrackGUID = Guid.NewGuid(),

                //AccessLevel = 1,
                //CurrentStep = 1,

                //StaffName = _staff.FirstName+" "+_staff.Surname,




            };
            return View("~/Areas/WMS/Views/DamagedItem/ICT/DamagedItemFormByICT.cshtml", model);
        }
        public ActionResult EditStaffDamagedItemUpdate(Guid PK)
        {
            var _result = DbWMS.dataItemOutputDetailDamagedTrack.Where(x => x.ItemOutputDetailDamagedTrackGUID == PK).FirstOrDefault();

            STIItemDamagedTrackUpdateModel model = Mapper.Map(_result, new STIItemDamagedTrackUpdateModel());
            model.ItemName = DbWMS.v_EntryMovementDataTable.Where(x => x.ItemInputDetailGUID == model.ItemInputDetailGUID).FirstOrDefault().ModelDescription;
            model.IncidentType = DbWMS.codeTablesValuesLanguages.Where(x => x.ValueGUID == model.DamagedTypeGUID && x.LanguageID == LAN && x.Active).FirstOrDefault().ValueDescription;

            //ViewBag.ItemOutputDetailDamagedTrackGUID = PK;
            //ViewBag.TotalStaffNotConfirmed = DbWMS.dataItemOutputDetailDamagedTrack.Where(a => a.FlowStatusGUID == NationalStaffDangerPaConfirmationStatus.Pending && a.ItemOutputDetailDamagedTrackGUID == PK).Count();
            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("StaffDamagedItem", "StaffDamagedItems", new { Area = "WMS" }));
            return View("~/Areas/WMS/Views/DamagedItem/Staff/EditDamagedItemForm.cshtml", model);


        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffDamagedItemICTCreate(STIItemDamagedTrackUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Create, Apps.WMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveStaffDamagedItem(model)) return View("~/Areas/WMS/Views/DamagedItem/ICT/DamagedItemFormByICT.cshtml", model);


            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            dataItemOutputDetailDamagedTrack StaffDamagedItem = new dataItemOutputDetailDamagedTrack();
            StaffDamagedItem.ItemOutputDetailDamagedTrackGUID = EntityPK;
            StaffDamagedItem.DamagedByGUID = model.DamagedByGUID;
            StaffDamagedItem.DamagedTypeGUID = model.DamagedTypeGUID;
            StaffDamagedItem.DamagedDate = model.DamagedDate;
            StaffDamagedItem.ItemInputDetailGUID = model.ItemInputDetailGUID;
            StaffDamagedItem.IncidentLocationOccureed = model.IncidentLocationOccureed;
            StaffDamagedItem.PresentLocation = model.PresentLocation;
            StaffDamagedItem.ItemConditionWhenReceivedGUID = model.ItemConditionWhenReceivedGUID;
            StaffDamagedItem.PurposeOfUseGUID = model.PurposeOfUseGUID;
            StaffDamagedItem.DamagedReason = model.DamagedReason;
            StaffDamagedItem.Comments = model.Comments;
            StaffDamagedItem.LastFlowStatusGUID = DamagedReportFlowStatus.Draft;
            StaffDamagedItem.CreatedByGUID = UserGUID;
            StaffDamagedItem.CreatedDate = ExecutionTime;



            DbWMS.CreateNoAudit(StaffDamagedItem);
            var toChange = DbWMS.dataItemOutputDetailDamagedTrackMovementFlow.Where(x => x.ItemOutputDetailDamagedTrackGUID == StaffDamagedItem.ItemOutputDetailDamagedTrackGUID

                                                            && x.IsLastAction == true).FirstOrDefault();
            if (toChange != null)
            {
                toChange.IsLastAction = false;
                DbWMS.UpdateNoAudit(toChange);
            }
            dataItemOutputDetailDamagedTrackMovementFlow flow = new dataItemOutputDetailDamagedTrackMovementFlow

            {
                ItemOutputDetailDamagedTrackMovementFlowGUID = Guid.NewGuid(),
                ItemOutputDetailDamagedTrackGUID = StaffDamagedItem.ItemOutputDetailDamagedTrackGUID,
                FlowStatusGUID = DamagedReportFlowStatus.Draft,
                IsLastAction = true,
                CreateByGUID = UserGUID,
                CreateDate = ExecutionTime,
                OrderId = 1
            };
            var _check = DbWMS.dataItemInputDetail.Where(x => x.ItemInputDetailGUID == model.ItemInputDetailGUID).FirstOrDefault();
            _check.ItemServiceStatusGUID = WarehouseServiceItemStatus.GS45;
            if (model.DamagedTypeGUID == Guid.Parse("1C44822F-A898-476D-B291-CAF1B0551AC7"))
            {
                _check.ItemStatusGUID = WarehouseItemStatus.Damaged;
            }
            if (model.DamagedTypeGUID == Guid.Parse("2C44822F-A898-476D-B291-CAF1B0551AC7"))
            {
                _check.ItemStatusGUID = WarehouseItemStatus.Lost;
                _check.IsAvaliable = false;
            }
            DbWMS.UpdateNoAudit(_check);

            DbWMS.CreateNoAudit(flow);

            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();

                SubmitDamagedItemReportEmail(StaffDamagedItem, Guid.Parse("d2b7f2ef-7846-4606-a393-679f77ebd41b"), DamagedReportFlowStatus.Draft, 0);

                return Json(DbCMS.SingleCreateMessage());
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [Route("WMS/StaffItemDamaged/Create/")]
        public ActionResult StaffDamagedItemCreate()
        {
            //if (!CMS.HasAction(Permissions.StaffSalaryProcess.Create, Apps.WMS))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}

            var _staff = DbWMS.userPersonalDetailsLanguage.Where(x => x.UserGUID == UserGUID && x.LanguageID == LAN).FirstOrDefault();
            STIItemDamagedTrackUpdateModel model = new STIItemDamagedTrackUpdateModel
            {
                //ItemOutputDetailDamagedTrackGUID = Guid.NewGuid(),

                //AccessLevel = 1,
                //CurrentStep = 1,
                DamagedByGUID = UserGUID,
                //StaffName = _staff.FirstName+" "+_staff.Surname,




            };
            return View("~/Areas/WMS/Views/DamagedItem/Staff/DamagedItemForm.cshtml", model);
        }


        public ActionResult StaffDamagedItemUpdate(Guid PK)
        {
            var _result = DbWMS.dataItemOutputDetailDamagedTrack.Where(x => x.ItemOutputDetailDamagedTrackGUID == PK).FirstOrDefault();
            STIItemDamagedTrackUpdateModel model = Mapper.Map(_result, new STIItemDamagedTrackUpdateModel());
            model.ItemName = DbWMS.v_EntryMovementDataTable.Where(x => x.ItemInputDetailGUID == model.ItemInputDetailGUID).FirstOrDefault().ModelDescription;
            model.IncidentType = DbWMS.codeTablesValuesLanguages.Where(x => x.ValueGUID == model.DamagedTypeGUID && x.LanguageID == LAN && x.Active).FirstOrDefault().ValueDescription;

            //ViewBag.ItemOutputDetailDamagedTrackGUID = PK;
            //ViewBag.TotalStaffNotConfirmed = DbWMS.dataItemOutputDetailDamagedTrack.Where(a => a.FlowStatusGUID == NationalStaffDangerPaConfirmationStatus.Pending && a.ItemOutputDetailDamagedTrackGUID == PK).Count();
            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("StaffDamagedItem", "StaffDamagedItems", new { Area = "WMS" }));
            return View("~/Areas/WMS/Views/DamagedItem/Staff/DamagedItemForm.cshtml", model);

        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffDamagedItemUpdate(STIItemDamagedTrackUpdateModel model)
        {
            //if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Update, Apps.WMS))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}

            if (!ModelState.IsValid) return View("~/Areas/WMS/Views/DamagedItem/DamagedItemForm/DamagedItemForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;


            var StaffDamagedItem = DbWMS.dataItemOutputDetailDamagedTrack.Where(x => x.ItemOutputDetailDamagedTrackGUID == model.ItemOutputDetailDamagedTrackGUID).FirstOrDefault();


            StaffDamagedItem.DamagedDate = model.DamagedDate;
            StaffDamagedItem.ItemInputDetailGUID = model.ItemInputDetailGUID;
            StaffDamagedItem.IncidentLocationOccureed = model.IncidentLocationOccureed;
            StaffDamagedItem.PresentLocation = model.PresentLocation;
            StaffDamagedItem.ItemConditionWhenReceivedGUID = model.ItemConditionWhenReceivedGUID;
            StaffDamagedItem.PurposeOfUseGUID = model.PurposeOfUseGUID;
            StaffDamagedItem.DamagedReason = model.DamagedReason;
            StaffDamagedItem.Comments = model.Comments;
            StaffDamagedItem.LastFlowStatusGUID = DamagedReportFlowStatus.Submitted;
            var _item = DbWMS.dataItemInputDetail.Where(x => x.ItemInputDetailGUID == model.ItemInputDetailGUID).FirstOrDefault();
            StaffDamagedItem.ItemConditionWhenReceivedGUID = _item.ItemConditionGUID;
            StaffDamagedItem.PurposeOfUseGUID = _item.PurposeofuseGUID;



            DbWMS.Update(StaffDamagedItem, Permissions.WarehouseItemsEntry.UpdateGuid, ExecutionTime, DbCMS);
            var toChange = DbWMS.dataItemOutputDetailDamagedTrackMovementFlow.Where(x => x.ItemOutputDetailDamagedTrackGUID == StaffDamagedItem.ItemOutputDetailDamagedTrackGUID

                                                && x.IsLastAction == true).FirstOrDefault();
            if (toChange.FlowStatusGUID == DamagedReportFlowStatus.Draft)
            {
                if (toChange != null)
                {
                    toChange.IsLastAction = false;
                    DbWMS.UpdateNoAudit(toChange);
                }
                dataItemOutputDetailDamagedTrackMovementFlow flow = new dataItemOutputDetailDamagedTrackMovementFlow

                {
                    ItemOutputDetailDamagedTrackMovementFlowGUID = Guid.NewGuid(),
                    ItemOutputDetailDamagedTrackGUID = StaffDamagedItem.ItemOutputDetailDamagedTrackGUID,
                    FlowStatusGUID = DamagedReportFlowStatus.Submitted,
                    IsLastAction = true,
                    CreateByGUID = UserGUID,
                    CreateDate = ExecutionTime,
                    OrderId = toChange.OrderId + 1
                };
                DbWMS.CreateNoAudit(flow);
            }
            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                SubmitDamagedItemReportEmail(StaffDamagedItem, Guid.Parse("d2b7f2ef-7846-4606-a393-679f77ebd41b"), DamagedReportFlowStatus.Submitted, 0);
                return Json(DbCMS.SingleUpdateMessage());
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyStaffDamagedItem(model.ItemOutputDetailDamagedTrackGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffDamagedItemICTUpdate(STIItemDamagedTrackUpdateModel model)
        {
            //if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Update, Apps.WMS))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            dataItemOutputDetailDamagedTrack _modelResult = DbWMS.dataItemOutputDetailDamagedTrack.Where(x => x.ItemOutputDetailDamagedTrackGUID == model.ItemOutputDetailDamagedTrackGUID).FirstOrDefault();
            if (!ModelState.IsValid) return View("~/Areas/WMS/Views/DamagedItem/DamagedItemForm/DamagedItemForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;
            _modelResult.ICTFocalPointComment = model.ICTFocalPointComment;
            _modelResult.ICTFocalPointGUID = UserGUID;
            _modelResult.ICTFocalPointReviewDate = ExecutionTime;
            _modelResult.LastFlowStatusGUID = DamagedReportFlowStatus.ICTVerified;
            //_modelResult.IsStaffReimburse = model.IsStaffReimburse;
            //_modelResult.ReimbursementAmount = model.ReimbursementAmount;

            DbWMS.Update(_modelResult, Permissions.WarehouseItemsEntry.UpdateGuid, ExecutionTime, DbCMS);


            var toChange = DbWMS.dataItemOutputDetailDamagedTrackMovementFlow.Where(x => x.ItemOutputDetailDamagedTrackGUID == _modelResult.ItemOutputDetailDamagedTrackGUID

                && x.IsLastAction == true).FirstOrDefault();
            toChange.IsLastAction = false;
            dataItemOutputDetailDamagedTrackMovementFlow flow = new dataItemOutputDetailDamagedTrackMovementFlow

            {
                ItemOutputDetailDamagedTrackMovementFlowGUID = Guid.NewGuid(),
                ItemOutputDetailDamagedTrackGUID = _modelResult.ItemOutputDetailDamagedTrackGUID,
                FlowStatusGUID = DamagedReportFlowStatus.ICTVerified,
                IsLastAction = true,
                CreateByGUID = UserGUID,
                CreateDate = ExecutionTime,
                OrderId = toChange.OrderId + 1
            };

            DbWMS.UpdateNoAudit(toChange);
            DbWMS.CreateNoAudit(flow);

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.StaffProfile.Create, Apps.WMS, new UrlHelper(Request.RequestContext).Action("StaffProfile/Create", "StaffProfile", new { Area = "ORG" })), Container = "StaffProfileFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.StaffProfile.Update, Apps.WMS), Container = "StaffProfileFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.StaffProfile.Delete, Apps.WMS), Container = "StaffProfileFormControls" });


            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                SubmitDamagedItemReportEmail(_modelResult, Guid.Parse("d2b7f2ef-7846-4606-a393-679f77ebd41b"), DamagedReportFlowStatus.ICTVerified, 0);
                return Json(DbCMS.SingleCreateMessage());

            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyStaffDamagedItem(model.ItemOutputDetailDamagedTrackGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        public ActionResult StaffDamagedItemAdminCreate(STIItemDamagedTrackUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.STIDamagedItemAdminApproval.Confirm, Apps.WMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            dataItemOutputDetailDamagedTrack _modelResult = DbWMS.dataItemOutputDetailDamagedTrack.Where(x => x.ItemOutputDetailDamagedTrackGUID == model.ItemOutputDetailDamagedTrackGUID).FirstOrDefault();
            if (!ModelState.IsValid || (model.StaffReimburseStatusGUID == STIDamagedItemReimbursementDecision.StaffHasToReimburse && (model.ReimbursementAmount == null || model.ReimbursementAmount < 0))) return View("~/Areas/WMS/Views/DamagedItem/Admin/ReviewAdminDamagedItemForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;
            _modelResult.AdminFocalPointReviewerComment = model.AdminFocalPointReviewerComment;
            _modelResult.AdminFocalPointReviewerGUID = UserGUID;
            if (model.AdminFocalPointReviewerDate == null)
            {
                _modelResult.AdminFocalPointReviewerDate = ExecutionTime;
            }
            else
            {
                _modelResult.AdminFocalPointReviewerDate = model.AdminFocalPointReviewerDate;
            }
            if (model.StaffReimburseStatusGUID == STIDamagedItemReimbursementDecision.NoReimburse)
            {
                _modelResult.LastFlowStatusGUID = DamagedReportFlowStatus.Closed;
            }
            else
            {
                _modelResult.LastFlowStatusGUID = DamagedReportFlowStatus.AdminVerified;
            }
            _modelResult.StaffReimburseStatusGUID = model.StaffReimburseStatusGUID;
            _modelResult.ReimbursementAmount = model.ReimbursementAmount;

            DbWMS.Update(_modelResult, Permissions.WarehouseItemsEntry.UpdateGuid, ExecutionTime, DbCMS);


            var toChange = DbWMS.dataItemOutputDetailDamagedTrackMovementFlow.Where(x => x.ItemOutputDetailDamagedTrackGUID == _modelResult.ItemOutputDetailDamagedTrackGUID

                && x.IsLastAction == true).FirstOrDefault();
            toChange.IsLastAction = false;
            DbWMS.UpdateNoAudit(toChange);
            if (model.StaffReimburseStatusGUID == STIDamagedItemReimbursementDecision.NoReimburse)
            {
                dataItemOutputDetailDamagedTrackMovementFlow flow = new dataItemOutputDetailDamagedTrackMovementFlow

                {
                    ItemOutputDetailDamagedTrackMovementFlowGUID = Guid.NewGuid(),
                    ItemOutputDetailDamagedTrackGUID = _modelResult.ItemOutputDetailDamagedTrackGUID,
                    FlowStatusGUID = DamagedReportFlowStatus.Closed,
                    IsLastAction = true,
                    CreateByGUID = UserGUID,
                    CreateDate = ExecutionTime,
                    OrderId = toChange.OrderId + 1
                };
                DbWMS.CreateNoAudit(flow);
            }
            else if (model.StaffReimburseStatusGUID == STIDamagedItemReimbursementDecision.StaffHasToReimburse)
            {
                dataItemOutputDetailDamagedTrackMovementFlow flow = new dataItemOutputDetailDamagedTrackMovementFlow

                {
                    ItemOutputDetailDamagedTrackMovementFlowGUID = Guid.NewGuid(),
                    ItemOutputDetailDamagedTrackGUID = _modelResult.ItemOutputDetailDamagedTrackGUID,
                    FlowStatusGUID = DamagedReportFlowStatus.AdminVerified,
                    IsLastAction = true,
                    CreateByGUID = UserGUID,
                    CreateDate = ExecutionTime,
                    OrderId = toChange.OrderId + 1
                };
                DbWMS.CreateNoAudit(flow);
            }



            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.StaffProfile.Create, Apps.WMS, new UrlHelper(Request.RequestContext).Action("StaffProfile/Create", "StaffProfile", new { Area = "ORG" })), Container = "StaffProfileFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.StaffProfile.Update, Apps.WMS), Container = "StaffProfileFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.StaffProfile.Delete, Apps.WMS), Container = "StaffProfileFormControls" });


            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                SubmitDamagedItemReportEmail(_modelResult, Guid.Parse("d2b7f2ef-7846-4606-a393-679f77ebd41b"), DamagedReportFlowStatus.AdminVerified, 0);
                return Json(DbCMS.SingleCreateMessage());

            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyStaffDamagedItem(model.ItemOutputDetailDamagedTrackGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        public ActionResult StaffDamagedItemFinanceCreate(STIItemDamagedTrackUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.STIDamagedItemFinanceApproval.Confirm, Apps.WMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            dataItemOutputDetailDamagedTrack _modelResult = DbWMS.dataItemOutputDetailDamagedTrack.Where(x => x.ItemOutputDetailDamagedTrackGUID == model.ItemOutputDetailDamagedTrackGUID).FirstOrDefault();
            if (!ModelState.IsValid) return View("~/Areas/WMS/Views/DamagedItem/Finance/ReviewFinanceDamagedItemForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;
            _modelResult.FinanceFocalPointReviewerComment = model.FinanceFocalPointReviewerComment;
            _modelResult.FinanceFocalPointReviewerGUID = UserGUID;
            if (model.FinanceFocalPointReviewerDate == null)
            {
                _modelResult.FinanceFocalPointReviewerDate = ExecutionTime;
            }
            else
            {
                _modelResult.FinanceFocalPointReviewerDate = model.FinanceFocalPointReviewerDate;
            }
            _modelResult.LastFlowStatusGUID = DamagedReportFlowStatus.Closed;
            //_modelResult.IsStaffReimburse = model.IsStaffReimburse;
            //_modelResult.ReimbursementAmount = model.ReimbursementAmount;

            DbWMS.Update(_modelResult, Permissions.WarehouseItemsEntry.UpdateGuid, ExecutionTime, DbCMS);


            var toChange = DbWMS.dataItemOutputDetailDamagedTrackMovementFlow.Where(x => x.ItemOutputDetailDamagedTrackGUID == _modelResult.ItemOutputDetailDamagedTrackGUID

                && x.IsLastAction == true).FirstOrDefault();
            toChange.IsLastAction = false;
            dataItemOutputDetailDamagedTrackMovementFlow flow = new dataItemOutputDetailDamagedTrackMovementFlow

            {
                ItemOutputDetailDamagedTrackMovementFlowGUID = Guid.NewGuid(),
                ItemOutputDetailDamagedTrackGUID = _modelResult.ItemOutputDetailDamagedTrackGUID,
                FlowStatusGUID = DamagedReportFlowStatus.Closed,
                IsLastAction = true,
                CreateByGUID = UserGUID,
                CreateDate = ExecutionTime,
                OrderId = toChange.OrderId + 1
            };

            DbWMS.UpdateNoAudit(toChange);
            DbWMS.CreateNoAudit(flow);

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.StaffProfile.Create, Apps.WMS, new UrlHelper(Request.RequestContext).Action("StaffProfile/Create", "StaffProfile", new { Area = "ORG" })), Container = "StaffProfileFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.StaffProfile.Update, Apps.WMS), Container = "StaffProfileFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.StaffProfile.Delete, Apps.WMS), Container = "StaffProfileFormControls" });


            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                SubmitDamagedItemReportEmail(_modelResult, Guid.Parse("d2b7f2ef-7846-4606-a393-679f77ebd41b"), DamagedReportFlowStatus.FinanceVerifid, 0);
                return Json(DbCMS.SingleCreateMessage());

            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyStaffDamagedItem(model.ItemOutputDetailDamagedTrackGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }



        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffDamagedItemCreate(STIItemDamagedTrackUpdateModel model)
        {
            //if (!CMS.HasAction(Permissions.StaffSalaryProcess.Create, Apps.WMS))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            if (!ModelState.IsValid || ActiveStaffDamagedItem(model)) return PartialView("~/Areas/WMS/Vsiews/StaffDamagedItems/_StaffDamagedItemForm.cshtml", model);


            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            dataItemOutputDetailDamagedTrack StaffDamagedItem = new dataItemOutputDetailDamagedTrack();
            StaffDamagedItem.ItemOutputDetailDamagedTrackGUID = EntityPK;
            StaffDamagedItem.DamagedByGUID = model.DamagedByGUID;
            if (model.DamagedDate == null)
            {
                StaffDamagedItem.DamagedDate = ExecutionTime;
            }
            else
            {
                StaffDamagedItem.DamagedDate = model.DamagedDate;
            }
            StaffDamagedItem.ItemInputDetailGUID = model.ItemInputDetailGUID;
            StaffDamagedItem.IncidentLocationOccureed = model.IncidentLocationOccureed;
            StaffDamagedItem.PresentLocation = model.PresentLocation;
            StaffDamagedItem.DamagedTypeGUID = model.DamagedTypeGUID;
            //StaffDamagedItem.ItemConditionWhenReceivedGUID = model.ItemConditionWhenReceivedGUID;
            //StaffDamagedItem.PurposeOfUseGUID = model.PurposeOfUseGUID;
            StaffDamagedItem.DamagedReason = model.DamagedReason;
            StaffDamagedItem.Comments = model.Comments;
            StaffDamagedItem.LastFlowStatusGUID = DamagedReportFlowStatus.Submitted;
            StaffDamagedItem.CreatedByGUID = UserGUID;
            StaffDamagedItem.CreatedDate = ExecutionTime;
            var _item = DbWMS.dataItemInputDetail.Where(x => x.ItemInputDetailGUID == model.ItemInputDetailGUID).FirstOrDefault();
            StaffDamagedItem.ItemConditionWhenReceivedGUID = _item.ItemConditionGUID;
            StaffDamagedItem.PurposeOfUseGUID = _item.PurposeofuseGUID;
      



            DbWMS.CreateNoAudit(StaffDamagedItem);
            var toChange = DbWMS.dataItemOutputDetailDamagedTrackMovementFlow.Where(x => x.ItemOutputDetailDamagedTrackGUID == StaffDamagedItem.ItemOutputDetailDamagedTrackGUID

                                                            && x.IsLastAction == true).FirstOrDefault();
            if (toChange != null)
            {
                toChange.IsLastAction = false;
                DbWMS.UpdateNoAudit(toChange);
            }
            dataItemOutputDetailDamagedTrackMovementFlow flow = new dataItemOutputDetailDamagedTrackMovementFlow

            {
                ItemOutputDetailDamagedTrackMovementFlowGUID = Guid.NewGuid(),
                ItemOutputDetailDamagedTrackGUID = StaffDamagedItem.ItemOutputDetailDamagedTrackGUID,
                FlowStatusGUID = DamagedReportFlowStatus.Submitted,
                IsLastAction = true,
                CreateByGUID = UserGUID,
                CreateDate = ExecutionTime,
                OrderId = 1
            };
            var _check = DbWMS.dataItemInputDetail.Where(x => x.ItemInputDetailGUID == model.ItemInputDetailGUID).FirstOrDefault();
            _check.ItemServiceStatusGUID = WarehouseServiceItemStatus.GS45;
            if (model.DamagedTypeGUID == Guid.Parse("1C44822F-A898-476D-B291-CAF1B0551AC7"))
            {
                _check.ItemStatusGUID = WarehouseItemStatus.Damaged;
            }
            if (model.DamagedTypeGUID == Guid.Parse("2C44822F-A898-476D-B291-CAF1B0551AC7"))
            {
                _check.ItemStatusGUID = WarehouseItemStatus.Lost;
                _check.IsAvaliable = false;
            }
            DbWMS.UpdateNoAudit(_check);

            DbWMS.CreateNoAudit(flow);
            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                SubmitDamagedItemReportEmail(StaffDamagedItem, Guid.Parse("d2b7f2ef-7846-4606-a393-679f77ebd41b"), DamagedReportFlowStatus.Submitted, 0);

                return Json(DbCMS.SingleUpdateMessage());
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }



        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffDamagedItemDelete(dataItemOutputDetailDamagedTrack model)
        {
            //if (!CMS.HasAction(Permissions.StaffSalaryProcess.Delete, Apps.WMS))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            List<dataItemOutputDetailDamagedTrack> DeletedStaffDamagedItem = DeleteStaffDamagedItem(Portal.SingleToList(model));
            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.StaffSalaryProcess.Restore, Apps.WMS), Container = "StaffDamagedItemFormControls" });
            try
            {
                int CommitedRows = DbWMS.SaveChanges();
                DbWMS.SaveChanges();
                return Json(DbWMS.SingleDeleteMessage(CommitedRows, DeletedStaffDamagedItem.FirstOrDefault(), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyStaffDamagedItem(model.ItemOutputDetailDamagedTrackGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StaffDamagedItemRestore(dataItemOutputDetailDamagedTrack model)
        {
            //if (!CMS.HasAction(Permissions.StaffSalaryProcess.Restore, Apps.WMS))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            if (ActiveStaffDamagedItem(model))
            {
                return Json(DbWMS.RecordExists());
            }
            List<dataItemOutputDetailDamagedTrack> RestoredStaffDamagedItem = RestoreStaffDamagedItems(Portal.SingleToList(model));
            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.StaffSalaryProcess.Create, Apps.WMS, new UrlHelper(Request.RequestContext).Action("StaffDamagedItemCreate", "Configuration", new { Area = "WMS" })), Container = "StaffDamagedItemFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.StaffSalaryProcess.Update, Apps.WMS), Container = "StaffDamagedItemFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.StaffSalaryProcess.Delete, Apps.WMS), Container = "StaffDamagedItemFormControls" });
            try
            {
                int CommitedRows = DbWMS.SaveChanges();
                DbWMS.SaveChanges();
                return Json(DbWMS.SingleRestoreMessage(CommitedRows, RestoredStaffDamagedItem, DbWMS.PrimaryKeyControl(RestoredStaffDamagedItem.FirstOrDefault()), Url.Action(DataTableNames.STIItemDamagedTrackDataTable, Portal.GetControllerName(ControllerContext)), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyStaffDamagedItem(model.ItemOutputDetailDamagedTrackGUID);
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }
        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult STIItemDamagedTrackDataTableModelDelete(List<dataItemOutputDetailDamagedTrack> models)
        {
            //if (!CMS.HasAction(Permissions.StaffSalaryProcess.Delete, Apps.WMS))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            List<dataItemOutputDetailDamagedTrack> DeletedStaffDamagedItem = DeleteStaffDamagedItem(models);
            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.PartialDeleteMessage(DeletedStaffDamagedItem, models, DataTableNames.STIItemDamagedTrackDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult STIItemDamagedTrackDataTableModelRestore(List<dataItemOutputDetailDamagedTrack> models)
        {
            //if (!CMS.HasAction(Permissions.StaffSalaryProcess.Restore, Apps.WMS))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            List<dataItemOutputDetailDamagedTrack> RestoredStaffDamagedItem = DeleteStaffDamagedItem(models);
            try
            {
                DbWMS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbWMS.PartialRestoreMessage(RestoredStaffDamagedItem, models, DataTableNames.STIItemDamagedTrackDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbWMS.ErrorMessage(ex.Message));
            }
        }

        private List<dataItemOutputDetailDamagedTrack> DeleteStaffDamagedItem(List<dataItemOutputDetailDamagedTrack> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<dataItemOutputDetailDamagedTrack> DeletedStaffDamagedItem = new List<dataItemOutputDetailDamagedTrack>();
            //Fix
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = "SELECT ItemOutputDetailDamagedTrackGUID,CONVERT(varchar(50), ItemOutputDetailDamagedTrackGUID) as C2 ,dataItemOutputDetailDamagedTrackRowVersion FROM code.dataItemOutputDetailDamagedTrack where ItemOutputDetailDamagedTrackGUID in (" + string.Join(",", models.Select(x => "'" + x.ItemOutputDetailDamagedTrackGUID + "'").ToArray()) + ")";
            string query = DbWMS.QueryBuilder(models, Permissions.StaffSalaryProcess.DeleteGuid, SubmitTypes.Delete, "");
            var Records = DbWMS.Database.SqlQuery<dataItemOutputDetailDamagedTrack>(query).ToList();
            foreach (var record in Records)
            {
                DeletedStaffDamagedItem.Add(DbWMS.Delete(record, ExecutionTime, Permissions.StaffSalaryProcess.DeleteGuid, DbCMS));
            }
            return DeletedStaffDamagedItem;
        }
        private List<dataItemOutputDetailDamagedTrack> RestoreStaffDamagedItems(List<dataItemOutputDetailDamagedTrack> models)
        {

            DateTime RestoringTime = DateTime.Now;

            List<dataItemOutputDetailDamagedTrack> RestoredStaffDamagedItem = new List<dataItemOutputDetailDamagedTrack>();
            //Fix
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = "SELECT ItemOutputDetailDamagedTrackGUID,CONVERT(varchar(50), ItemOutputDetailDamagedTrackGUID) as C2 ,dataItemOutputDetailDamagedTrackRowVersion FROM code.dataItemOutputDetailDamagedTrack where ItemOutputDetailDamagedTrackGUID in (" + string.Join(",", models.Select(x => "'" + x.ItemOutputDetailDamagedTrackGUID + "'").ToArray()) + ")";

            string query = DbWMS.QueryBuilder(models, Permissions.StaffSalaryProcess.DeleteGuid, SubmitTypes.Restore, "");

            var Records = DbWMS.Database.SqlQuery<dataItemOutputDetailDamagedTrack>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveStaffDamagedItem(record))
                {
                    RestoredStaffDamagedItem.Add(DbWMS.Restore(record, Permissions.StaffSalaryProcess.DeleteGuid, Permissions.StaffSalaryProcess.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredStaffDamagedItem;
        }

        private JsonResult ConcurrencyStaffDamagedItem(Guid PK)
        {
            STIItemDamagedTrackDataTableModel dbModel = new STIItemDamagedTrackDataTableModel();

            var StaffDamagedItem = DbWMS.dataItemOutputDetailDamagedTrack.Where(x => x.ItemOutputDetailDamagedTrackGUID == PK).FirstOrDefault();
            var dbStaffDamagedItem = DbWMS.Entry(StaffDamagedItem).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbStaffDamagedItem, dbModel);

            if (StaffDamagedItem.dataItemOutputDetailDamagedTrackRowVersion.SequenceEqual(dbModel.dataItemOutputDetailDamagedTrackRowVersion))
            {
                return Json(DbWMS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbWMS, dbModel, "LanguagesContainer"));
        }

        private bool ActiveStaffDamagedItem(Object model)
        {
            STIItemDamagedTrackUpdateModel StaffDamagedItem = Mapper.Map(model, new STIItemDamagedTrackUpdateModel());
            int ModelDescription = DbWMS.dataItemOutputDetailDamagedTrack
                                    .Where(x => x.DocumentReference == StaffDamagedItem.DocumentReference &&
                                                x.ItemInputDetailGUID == StaffDamagedItem.ItemInputDetailGUID &&
                                                x.Active).Count();
            if (ModelDescription > 0)
            {
                ModelState.AddModelError("Record ", " already exists");
            }
            return (ModelDescription > 0);
        }


        #endregion

        #region Work Flow
        #region Staff

        #endregion
        #region ICT

        #endregion
        #region Admin

        #endregion

        #region Finance

        #endregion

        #region Email

        public ActionResult VerifyDamagedItemRequest(Guid PK)
        {

            //if (!CMS.HasAction(Permissions.InternationalStaffEntitlements.Access, Apps.WMS))
            //{
            //    return Json(DbWMS.PermissionError());
            //}


            var _damaged = DbWMS.dataItemOutputDetailDamagedTrack.Where(x => x.ItemOutputDetailDamagedTrackGUID == PK).FirstOrDefault();
            STIItemDamagedTrackUpdateModel model = Mapper.Map(_damaged, new STIItemDamagedTrackUpdateModel());
            var _staff = DbWMS.userPersonalDetailsLanguage.Where(x => x.UserGUID == model.DamagedByGUID && x.LanguageID == LAN).FirstOrDefault();
            ViewBag.StaffName = _staff.FirstName + " " + _staff.Surname;
            var _item = DbWMS.v_EntryMovementDataTable.Where(x => x.ItemInputDetailGUID == _damaged.ItemInputDetailGUID).FirstOrDefault();
            model.ItemName = _item.WarehouseItemDescription + " " + _item.ModelDescription;
            if (_damaged.StaffReimburseStatusGUID != null)
            {
                var _Reimbursan = DbWMS.codeTablesValuesLanguages.Where(x => x.ValueGUID == _damaged.StaffReimburseStatusGUID).FirstOrDefault();
                model.ReimburseStatusDecision = _Reimbursan.ValueDescription;
            }

            if (_damaged.ItemConditionWhenReceivedGUID != null)
            {
                model.ItemCondition = DbWMS.codeTablesValuesLanguages.Where(x => x.ValueGUID == _damaged.ItemConditionWhenReceivedGUID && x.LanguageID == LAN).FirstOrDefault().ValueDescription;
            }
            if (_damaged.PurposeOfUseGUID != null)
            {
                model.PurposeOfUse = DbWMS.codeTablesValuesLanguages.Where(x => x.ValueGUID == _damaged.PurposeOfUseGUID && x.LanguageID == LAN).FirstOrDefault().ValueDescription;
            }
            if (_damaged.LastFlowStatusGUID == DamagedReportFlowStatus.Draft)
            {
                if (_damaged.DamagedByGUID != UserGUID)
                {
                    return Json(DbWMS.PermissionError());
                }

                return View("~/Areas/WMS/Views/DamagedItem/Staff/DamagedItemForm.cshtml", model);
            }
            else if (_damaged.LastFlowStatusGUID == DamagedReportFlowStatus.Submitted)
            {
                if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Access, Apps.WMS))
                {
                    return Json(DbWMS.PermissionError());
                }
                var checkItem = DbWMS.v_EntryMovementDataTable.Where(x => x.ItemInputDetailGUID == _damaged.ItemInputDetailGUID).FirstOrDefault();
                var _iteminput = DbWMS.dataItemInputDetail.Where(x => x.ItemInputDetailGUID == _damaged.ItemInputDetailGUID).FirstOrDefault();

                model.MSRPValue = checkItem.ModelAge >= 4 ? (double?)_iteminput.PriceValue : (checkItem.ModelAge < 1 ? 0 : checkItem.ModelAge == 1 ? (double?)_iteminput.PriceValue / 5 : checkItem.ModelAge == 2 ? (double?)_iteminput.PriceValue / 2.5 : checkItem.ModelAge == 3 ? (double?)_iteminput.PriceValue / 1.66 : null);
                model.AcquisitionDate = _iteminput.AcquisitionDate;
                model.PriceValue = _iteminput.PriceValue;
                Guid priceGUID = Guid.Parse("6FFD5528-A577-41E3-960A-013D152DBAB3");

                var allPrice = DbWMS.codeTablesValuesLanguages
                    .Where(x => x.codeTablesValues.TableGUID == priceGUID && x.LanguageID == LAN).ToList();
                if (_iteminput.PriceTypeGUID != null)
                {
                    model.PriceType = allPrice.Where(x => x.ValueGUID == _iteminput.PriceTypeGUID) != null ? allPrice.Where(x => x.ValueGUID == _iteminput.PriceTypeGUID).FirstOrDefault().ValueDescription : "-";
                }

                return View("~/Areas/WMS/Views/DamagedItem/ICT/ReviewICTDamagedItemForm.cshtml", model);
            }
            else if (_damaged.LastFlowStatusGUID == DamagedReportFlowStatus.ICTVerified)
            {
                if (!CMS.HasAction(Permissions.STIDamagedItemAdminApproval.Confirm, Apps.WMS))
                {
                    return Json(DbWMS.PermissionError());

                }
                var checkItem = DbWMS.v_EntryMovementDataTable.Where(x => x.ItemInputDetailGUID == _damaged.ItemInputDetailGUID).FirstOrDefault();
                var _iteminput = DbWMS.dataItemInputDetail.Where(x => x.ItemInputDetailGUID == _damaged.ItemInputDetailGUID).FirstOrDefault();
                model.ICTFocalPoint = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == _damaged.ICTFocalPointGUID).FirstOrDefault().FullName;
                model.MSRPValue = checkItem.ModelAge >= 4 ? (double?)_iteminput.PriceValue : (checkItem.ModelAge < 1 ? 0 : checkItem.ModelAge == 1 ? (double?)_iteminput.PriceValue / 5 : checkItem.ModelAge == 2 ? (double?)_iteminput.PriceValue / 2.5 : checkItem.ModelAge == 3 ? (double?)_iteminput.PriceValue / 1.66 : null);
                model.AcquisitionDate = _iteminput.AcquisitionDate;
                model.PriceValue = _iteminput.PriceValue;
                Guid priceGUID = Guid.Parse("6FFD5528-A577-41E3-960A-013D152DBAB3");

                var allPrice = DbWMS.codeTablesValuesLanguages
                    .Where(x => x.codeTablesValues.TableGUID == priceGUID && x.LanguageID == LAN).ToList();
                if (_iteminput.PriceTypeGUID != null)
                {
                    model.PriceType = allPrice.Where(x => x.ValueGUID == _iteminput.PriceTypeGUID) != null ? allPrice.Where(x => x.ValueGUID == _iteminput.PriceTypeGUID).FirstOrDefault().ValueDescription : "-";
                }
                return View("~/Areas/WMS/Views/DamagedItem/Admin/ReviewAdminDamagedItemForm.cshtml", model);
            }
            else if (_damaged.LastFlowStatusGUID == DamagedReportFlowStatus.AdminVerified)
            {

                if (!CMS.HasAction(Permissions.STIDamagedItemFinanceApproval.Confirm, Apps.WMS))
                {
                    return Json(DbWMS.PermissionError());
                }
                //return Json(DbWMS.PermissionError());
                model.ICTFocalPoint = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == _damaged.ICTFocalPointGUID).FirstOrDefault().FullName;
                model.AdminFocalPoint = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == _damaged.AdminFocalPointReviewerGUID).FirstOrDefault().FullName;
                return View("~/Areas/WMS/Views/DamagedItem/Finance/ReviewFinanceDamagedItemForm.cshtml", model);
            }


            return Json(DbWMS.PermissionError());
            //return View("~/Areas/WMS/Views/MissionRequest/MissionReview/ReviewWarehouseItemsEntryForm.cshtml", model);
        }
        public void SubmitDamagedItemReportEmail(dataItemOutputDetailDamagedTrack _damagedReport, Guid _permissionGUID, Guid _statusGUID, int type)
        {

            var currStaff = DbWMS.userPersonalDetailsLanguage.Where(x => x.UserGUID == _damagedReport.DamagedByGUID
                                                                              && x.LanguageID == LAN).FirstOrDefault();
            var _staff = DbWMS.StaffCoreData.Where(x => x.UserGUID == _damagedReport.DamagedByGUID).FirstOrDefault();

            #region ICT Focal Point
            var _warehouseFocalPoint = DbWMS.codeWarehouseFocalPoint.Where(x => x.codeWarehouse.DutyStationGUID == _staff.DutyStationGUID && x.IsFocalPoint == true).Select(x => x.UserGUID).Distinct().ToList();

            var tempPermGUIDs = DbCMS.userPermissions.Where(x => (x.ActionGUID == _permissionGUID && x.Active == true
                             ) && x.Active).Select(x => x.UserProfileGUID).Distinct().ToList();

            var _userGuids = DbCMS.userProfiles.Where(x => tempPermGUIDs.Contains(x.UserProfileGUID)).Select(x => x.userServiceHistory.UserGUID).ToList();

            var _backupUsers = DbCMS.userServiceHistory.Where(x => _userGuids.Contains(x.UserGUID) && _warehouseFocalPoint.Contains(x.UserGUID)).Select(x => x.EmailAddress).Distinct().ToList();


            string copyICTEmails = string.Join(" ;", _backupUsers);


            #endregion

            var _staffService = DbAHD.userServiceHistory.Where(x => x.UserGUID == _damagedReport.DamagedByGUID).FirstOrDefault();
            var _staffOrgGUID = DbWMS.userProfiles.Where(x => x.ServiceHistoryGUID == _staffService.ServiceHistoryGUID).FirstOrDefault().OrganizationInstanceGUID;

            //var _currManagerAccount = DbWMS.userServiceHistory.Where(x => x.UserGUID == staffCore.ReportToGUID).ToList();
            string URL = "";
            string Anchor = "";
            string Link = "";
            string _staffName = currStaff.FirstName + " " + currStaff.Surname;
            var _pre = DbWMS.StaffCoreData.Where(x => x.UserGUID == _staff.UserGUID).FirstOrDefault();
            string _staffPrefix = "";
            var _item = DbWMS.v_EntryMovementDataTable.Where(x => x.ItemInputDetailGUID == _damagedReport.ItemInputDetailGUID).FirstOrDefault();
            string SubjectMessage = resxEmails.WMSDamagedItemReportSubject.Replace("$itemName", _item.ModelDescription).
                                                                 Replace("$StaffName", _staffName);

            if (_statusGUID == DamagedReportFlowStatus.Draft)
            {
                URL = AppSettingsKeys.Domain + "/WMS/DamagedItem/VerifyDamagedItemRequest/?PK=" + new Portal().GUIDToString(_damagedReport.ItemOutputDetailDamagedTrackGUID);
                Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ConfirmDangerPayReceiving + "</a>";
                Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";

                string _message = resxEmails.WMSICTSubmittedForStaffDamagedItemReportEmail
                    //.Replace("$FullName", _financeFirstName + " " + _financeSurName)
                    .Replace("$VerifyLink", Anchor)
                    .Replace("$StaffName", _staffName)
                    .Replace("$item", _item.WarehouseItemDescription + " " + _item.ModelDescription)
                    .Replace("$BC", _item.BarcodeNumber)
                    .Replace("$SN", _item.SerialNumber)
                    .Replace("$GSM", _item.GSM)
                    .Replace("$IMEI", _item.IMEI)
                    .Replace("$MAC", _item.MAC)
                    ;

                if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                int isRec = 1;
                //var myEmail = currAccount.Select(x => x.EmailAddress).FirstOrDefault();
                string copy_recipients = _staff.EmailAddress;
                Send(copy_recipients, SubjectMessage, _message, isRec, copyICTEmails);
            }

            if (_statusGUID == DamagedReportFlowStatus.Submitted)
            {
                URL = AppSettingsKeys.Domain + "/WMS/DamagedItem/VerifyDamagedItemRequest/?PK=" + new Portal().GUIDToString(_damagedReport.ItemOutputDetailDamagedTrackGUID);
                Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ConfirmDangerPayReceiving + "</a>";
                Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";

                string _message = resxEmails.WMSDamagedItemReportByStaffEmail
                    //.Replace("$FullName", _financeFirstName + " " + _financeSurName)
                    .Replace("$VerifyLink", Anchor)
                    .Replace("$StaffName", _staffName)
                    .Replace("$item", _item.WarehouseItemDescription + " " + _item.ModelDescription)
                    .Replace("$BC", _item.BarcodeNumber)
                    .Replace("$SN", _item.SerialNumber)
                    .Replace("$GSM", _item.GSM)
                    .Replace("$IMEI", _item.IMEI)
                    .Replace("$MAC", _item.MAC)
                    ;

                if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                int isRec = 1;
                //var myEmail = currAccount.Select(x => x.EmailAddress).FirstOrDefault();
                string copy_recipients = _staff.EmailAddress;
                Send(copyICTEmails, SubjectMessage, _message, isRec, copy_recipients);
            }


            else if (_statusGUID == DamagedReportFlowStatus.ICTVerified)
            {
                URL = AppSettingsKeys.Domain + "/WMS/DamagedItem/VerifyDamagedItemRequest/?PK=" + new Portal().GUIDToString(_damagedReport.ItemOutputDetailDamagedTrackGUID);
                Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ConfirmDangerPayReceiving + "</a>";
                Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";
                var _staffICT = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == _damagedReport.ICTFocalPointGUID).FirstOrDefault();
                string _message = resxEmails.WMSDamagedItemReportVerifiedBYICTToAdminEmail
                    //.Replace("$FullName", _financeFirstName + " " + _financeSurName)
                    .Replace("$VerifyLink", Anchor)
                    .Replace("$StaffName", _staffName)
                    .Replace("$ICTName", _staffICT.FullName)
                    .Replace("$item", _item.WarehouseItemDescription + " " + _item.ModelDescription)
                    .Replace("$BC", _item.BarcodeNumber)
                    .Replace("$SN", _item.SerialNumber)
                    .Replace("$GSM", _item.GSM)
                    .Replace("$IMEI", _item.IMEI)
                    .Replace("$MAC", _item.MAC);

                if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                int isRec = 1;
                //var myEmail = currAccount.Select(x => x.EmailAddress).FirstOrDefault();
                List<string> allmail = new List<string>();
                string copy_recipients = _staff.EmailAddress;
                allmail.Add(copy_recipients);
                allmail.AddRange(_backupUsers);


                string copAll = string.Join(";", allmail);
                #region Admin Focal Point
                Guid _adminConfirmGUID = Guid.Parse("10c0e8dc-9443-4d00-9adc-8a457f829467");
                var _tempAdmin = DbCMS.userPermissions.Where(x => (x.ActionGUID == _adminConfirmGUID && x.Active == true
                                 ) && x.Active).Select(x => x.UserProfileGUID).Distinct().ToList();

                var _testAdmis = DbCMS.userProfiles.Where(x => x.OrganizationInstanceGUID == _staffOrgGUID &&
                                       _tempAdmin.Contains(x.UserProfileGUID)).Select(x => x.userServiceHistory.UserGUID).ToList();

                var _adminUser = DbCMS.userServiceHistory.Where(x => _testAdmis.Contains(x.UserGUID)
                             ).Select(x => x.EmailAddress).Distinct().ToList();


                string _adminUses = string.Join(" ;", _adminUser);


                #endregion

                //check copyemails
                Send(_adminUses, SubjectMessage, _message, isRec, copAll);
            }
            else if (_statusGUID == DamagedReportFlowStatus.AdminVerified)
            {
                URL = AppSettingsKeys.Domain + "/WMS/DamagedItem/VerifyDamagedItemRequest/?PK=" + new Portal().GUIDToString(_damagedReport.ItemOutputDetailDamagedTrackGUID);
                Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ConfirmDangerPayReceiving + "</a>";
                Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";
                if (_damagedReport.StaffReimburseStatusGUID == STIDamagedItemReimbursementDecision.StaffHasToReimburse)
                {
                    var _staffICT = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == _damagedReport.ICTFocalPointGUID).FirstOrDefault();
                    var _staffAdmin = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == _damagedReport.AdminFocalPointReviewerGUID).FirstOrDefault();
                    string _message = resxEmails.WMSDamagedItemReportVerifiedByAdminToFinanceEmail
                        //.Replace("$FullName", _financeFirstName + " " + _financeSurName)
                        .Replace("$VerifyLink", Anchor)
                        .Replace("$StaffName", _staffName)
                        .Replace("$item", _item.WarehouseItemDescription + " " + _item.ModelDescription)
                        .Replace("$BC", _item.BarcodeNumber)
                        .Replace("$ICTName", _staffICT.FullName)
                        .Replace("$adminfocalpoint", _staffAdmin.FullName)
                        .Replace("$SN", _item.SerialNumber)
                        .Replace("$GSM", _item.GSM)
                        .Replace("$IMEI", _item.IMEI)
                        .Replace("$MAC", _item.MAC);

                    if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                    int isRec = 1;
                    //var myEmail = currAccount.Select(x => x.EmailAddress).FirstOrDefault();
                    string copy_recipients = _staff.EmailAddress;

                    #region Finance Focal Point
                    Guid _financeConfirmGUID = Guid.Parse("edb30416-81dd-4e1e-8235-5406cd613309");
                    var _tempfinance = DbCMS.userPermissions.Where(x => (x.ActionGUID == _financeConfirmGUID && x.Active == true
                                     ) && x.Active).Select(x => x.UserProfileGUID).Distinct().ToList();

                    var _testfinance = DbCMS.userProfiles.Where(x => x.OrganizationInstanceGUID == _staffOrgGUID &&
                                           _tempfinance.Contains(x.UserProfileGUID)).Select(x => x.userServiceHistory.UserGUID).ToList();

                    var _financeUser = DbCMS.userServiceHistory.Where(x => _testfinance.Contains(x.UserGUID)
                                 ).Select(x => x.EmailAddress).Distinct().ToList();


                    string _financeUses = string.Join(" ;", _financeUser);


                    #endregion
                    #region Admin Focal Point
                    Guid _adminConfirmGUID = Guid.Parse("10c0e8dc-9443-4d00-9adc-8a457f829467");
                    var _tempAdmin = DbCMS.userPermissions.Where(x => (x.ActionGUID == _adminConfirmGUID && x.Active == true
                                     ) && x.Active).Select(x => x.UserProfileGUID).Distinct().ToList();

                    var _testAdmis = DbCMS.userProfiles.Where(x => x.OrganizationInstanceGUID == _staffOrgGUID &&
                                           _tempAdmin.Contains(x.UserProfileGUID)).Select(x => x.userServiceHistory.UserGUID).ToList();

                    var _adminUser = DbCMS.userServiceHistory.Where(x => _testAdmis.Contains(x.UserGUID)
                                 ).Select(x => x.EmailAddress).Distinct().ToList();


                    //string _adminUses = string.Join(" ;", _adminUser);


                    #endregion

                    List<string> allmail = new List<string>();

                    allmail.Add(copy_recipients);
                    allmail.AddRange(_backupUsers);
                    allmail.AddRange(_adminUser);


                    string copAll = string.Join(";", allmail);

                    //string copAll = string.Join(copy_recipients, _backupUsers, _adminUser);
                    //check copyemails
                    Send(_financeUses, SubjectMessage, _message, isRec, copAll);
                }
                else
                {
                    var _staffICT = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == _damagedReport.ICTFocalPointGUID).FirstOrDefault();
                    var _staffAdmin = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == _damagedReport.AdminFocalPointReviewerGUID).FirstOrDefault();
                    string _message = resxEmails.WMSDamagedItemReportApproved
                        //.Replace("$FullName", _financeFirstName + " " + _financeSurName)
                        .Replace("$VerifyLink", Anchor)
                        .Replace("$StaffName", _staffName)
                        .Replace("$item", _item.WarehouseItemDescription + " " + _item.ModelDescription)
                        .Replace("$BC", _item.BarcodeNumber)

                        .Replace("$adminfocalpoint", _staffAdmin.FullName)
                        .Replace("$SN", _item.SerialNumber)
                        .Replace("$GSM", _item.GSM)
                        .Replace("$IMEI", _item.IMEI)
                        .Replace("$MAC", _item.MAC);

                    if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                    int isRec = 1;
                    //var myEmail = currAccount.Select(x => x.EmailAddress).FirstOrDefault();
                    string copy_recipients = _staff.EmailAddress;


                    #region Admin Focal Point
                    Guid _adminConfirmGUID = Guid.Parse("10c0e8dc-9443-4d00-9adc-8a457f829467");
                    var _tempAdmin = DbCMS.userPermissions.Where(x => (x.ActionGUID == _adminConfirmGUID && x.Active == true
                                     ) && x.Active).Select(x => x.UserProfileGUID).Distinct().ToList();

                    var _testAdmis = DbCMS.userProfiles.Where(x => x.OrganizationInstanceGUID == _staffOrgGUID &&
                                           _tempAdmin.Contains(x.UserProfileGUID)).Select(x => x.userServiceHistory.UserGUID).ToList();

                    var _adminUser = DbCMS.userServiceHistory.Where(x => _testAdmis.Contains(x.UserGUID)
                                 ).Select(x => x.EmailAddress).Distinct().ToList();


                    //string _adminUses = string.Join(" ;", _adminUser);


                    #endregion


                    List<string> allmail = new List<string>();

                    //allmail.Add(copy_recipients);
                    allmail.AddRange(_backupUsers);
                    allmail.AddRange(_adminUser);


                    string copAll = string.Join(";", allmail);

                    //string copAll = string.Join(";",_backupUsers, _adminUser);
                    //check copyemails
                    Send(copy_recipients, SubjectMessage, _message, isRec, copAll);

                }
            }

            else if (_statusGUID == DamagedReportFlowStatus.FinanceVerifid)
            {
                URL = AppSettingsKeys.Domain + "/WMS/DamagedItem/VerifyDamagedItemRequest/?PK=" + new Portal().GUIDToString(_damagedReport.ItemOutputDetailDamagedTrackGUID);
                Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ConfirmDangerPayReceiving + "</a>";
                Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";

                var _staffICT = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == _damagedReport.ICTFocalPointGUID).FirstOrDefault();
                var _staffAdmin = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == _damagedReport.AdminFocalPointReviewerGUID).FirstOrDefault();
                var _finance = DbAHD.v_StaffProfileInformation.Where(x => x.UserGUID == _damagedReport.FinanceFocalPointReviewerGUID).FirstOrDefault();
                string _message = resxEmails.WMSDamagedItemReportFromFinanceToStaff
                    //.Replace("$FullName", _financeFirstName + " " + _financeSurName)
                    .Replace("$VerifyLink", Anchor)
                    .Replace("$StaffName", _staffName)
                    .Replace("$item", _item.WarehouseItemDescription + " " + _item.ModelDescription)
                    .Replace("$BC", _item.BarcodeNumber)
                    .Replace("$ICTName", _staffICT.FullName)
                    .Replace("$adminfocalpoint", _staffAdmin.FullName)
                    .Replace("$financefocalpoint", _finance.FullName)
                    .Replace("$ReimbursementAmount", _damagedReport.ReimbursementAmount.ToString())
                    .Replace("$SN", _item.SerialNumber)
                    .Replace("$GSM", _item.GSM)
                    .Replace("$IMEI", _item.IMEI)
                    .Replace("$MAC", _item.MAC);

                if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                int isRec = 1;
                //var myEmail = currAccount.Select(x => x.EmailAddress).FirstOrDefault();
                string copy_recipients = _staff.EmailAddress;

                #region Finance Focal Point
                Guid _financeConfirmGUID = Guid.Parse("edb30416-81dd-4e1e-8235-5406cd613309");
                var _tempfinance = DbCMS.userPermissions.Where(x => (x.ActionGUID == _financeConfirmGUID && x.Active == true
                                 ) && x.Active).Select(x => x.UserProfileGUID).Distinct().ToList();

                var _testfinance = DbCMS.userProfiles.Where(x => x.OrganizationInstanceGUID == _staffOrgGUID &&
                                       _tempfinance.Contains(x.UserProfileGUID)).Select(x => x.userServiceHistory.UserGUID).ToList();

                var _financeUser = DbCMS.userServiceHistory.Where(x => _testfinance.Contains(x.UserGUID)
                             ).Select(x => x.EmailAddress).Distinct().ToList();


                //string _financeUses = string.Join(" ;", _financeUser);


                #endregion
                #region Admin Focal Point
                Guid _adminConfirmGUID = Guid.Parse("10c0e8dc-9443-4d00-9adc-8a457f829467");
                var _tempAdmin = DbCMS.userPermissions.Where(x => (x.ActionGUID == _adminConfirmGUID && x.Active == true
                                 ) && x.Active).Select(x => x.UserProfileGUID).Distinct().ToList();

                var _testAdmis = DbCMS.userProfiles.Where(x => x.OrganizationInstanceGUID == _staffOrgGUID &&
                                       _tempAdmin.Contains(x.UserProfileGUID)).Select(x => x.userServiceHistory.UserGUID).ToList();

                var _adminUser = DbCMS.userServiceHistory.Where(x => _testAdmis.Contains(x.UserGUID)
                             ).Select(x => x.EmailAddress).Distinct().ToList();


                //string _adminUses = string.Join(" ;", _adminUser);


                #endregion

                List<string> allmail = new List<string>();

                //allmail.Add(copy_recipients);
                allmail.AddRange(_backupUsers);
                allmail.AddRange(_adminUser);
                allmail.AddRange(_financeUser);


                string copAll = string.Join(";", allmail);


                // string copAll = string.Join(";", _financeUser, _backupUsers, _adminUser);
                //check copyemails
                Send(copy_recipients, SubjectMessage, _message, isRec, copAll);


            }

        }
        public void Send(string recipients, string subject, string body, int? isRec, string copy_recipients)
        {
            //string copy_recipients = "";
            string blind_copy_recipients = null;
            string body_format = "HTML";
            string importance = "Normal";
            string file_attachments = null;
            string _body = "<div style='font-family:Arial;'>" + body.Replace("\r\n", "<br/>") + "</div>";
            if (LAN == "AR") { _body = "<p align='right'>" + _body + "</p>"; }
            //DbWMS.SendEmailSTI("maksoud@unhcr.org", "", blind_copy_recipients, subject, _body, body_format, importance, file_attachments);
             DbCMS.SendEmailSTI(recipients, copy_recipients, blind_copy_recipients, subject, _body, body_format, importance, file_attachments);
        }
        #endregion

        #endregion
    }
}