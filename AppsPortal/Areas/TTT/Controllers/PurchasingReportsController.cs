using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AutoMapper;
using TTT_DAL.Model;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using FineUploader;
using System.IO;
using OfficeOpenXml;
using System.Data;
using System.Reflection;
using static AppsPortal.Areas.TTT.Views.PurchasingReports.TTTDataSet;
using AppsPortal.Areas.TTT.Views.PurchasingReports.TTTDataSetTableAdapters;
using Microsoft.Reporting.WebForms;
using AppsPortal.Areas.TTT.Views.PurchasingReports;
using System.Net.Mime;
using System.Configuration;
using MimeDetective;
using AppsPortal.Library.MimeDetective;

namespace AppsPortal.Areas.TTT.Controllers
{
    public class PurchasingReportsController : TTTBaseController
    {
        #region  Medical Distribution Restriction 

        public ActionResult Index()
        {
            return View();
        }

        [Route("TTT/PurchasingReports/")]
        public ActionResult PurchasingReportsIndex()
        {
            if (!CMS.HasAction(Permissions.PurchasingReport.Access, Apps.TTT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/TTT/Views/PurchasingReports/Index.cshtml");
        }

        //[Route("TTT/PurchasingReportsDataTable/{PK}")]
        public ActionResult PurchasingReportsDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<PurchasingReportsDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<PurchasingReportsDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.PurchasingReport.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();

            var All = (from a in DbTTT.dataPurchasingReport.AsNoTracking().AsExpandable()/*.Where(x => AuthorizedList.Contains(x.ProvideByOrganizationInstanceGUID.ToString()))*/
                       select new PurchasingReportsDataTableModel
                       {
                           PurchasingReportGUID = a.PurchasingReportGUID,
                           ReportID=a.ReportID,
                           PuchaseOrder=a.PuchaseOrder,
                           BudgetDateFrom=a.BudgetDateFrom,
                           BudgetDateTo=a.BudgetDateTo,
                           BgtYr=a.BgtYr,
                           CostCentre=a.CostCentre,
                           Active = a.Active,
                           dataPurchasingReportRowVersion = a.dataPurchasingReportRowVersion
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<PurchasingReportsDataTableModel> Result = Mapper.Map<List<PurchasingReportsDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult PurchasingReportCreate()
        {
            if (!CMS.HasAction(Permissions.PurchasingReport.Create, Apps.TTT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/TTT/Views/PurchasingReports/_FileUpload.cshtml",new dataPurchasingReport());
        }

        public ActionResult PurchasingReportUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.PurchasingReport.Access, Apps.TTT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var Audit = DbCMS.spAuditHistory(LAN, PK).OrderBy(x => x.ExecutionTime).FirstOrDefault();
            var model = DbTTT.dataPurchasingReport.AsEnumerable().Where(x => x.PurchasingReportGUID == PK).Select(x =>
                new PurchasingReportUpdateModel()
                {
                    Active = x.Active,
                    dataPurchasingReportRowVersion = x.dataPurchasingReportRowVersion,
                    PurchasingReportGUID = x.PurchasingReportGUID,
                }).FirstOrDefault();
            return PartialView("~/Areas/TTT/Views/PurchasingReports/_PurchasingReportsUpdateModal.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PurchasingReportCreate(PurchasingReportUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.PurchasingReport.Create, Apps.TTT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            dataPurchasingReport PurchasingReport = Mapper.Map(model, new dataPurchasingReport());
            if (!ModelState.IsValid || ActivePurchasingReport(PurchasingReport)) return PartialView("~/Areas/TTT/Views/PurchasingReports/_PurchasingReportsUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbTTT.Create(PurchasingReport, Permissions.PurchasingReport.CreateGuid, ExecutionTime, DbCMS);


            try
            {
                DbTTT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbTTT.SingleUpdateMessage(DataTableNames.PurchasingReportsDataTable,
                    DbTTT.PrimaryKeyControl(PurchasingReport),
                    DbTTT.RowVersionControls(Portal.SingleToList(PurchasingReport))));
            }
            catch (Exception ex)
            {
                return Json(DbTTT.ErrorMessage(ex.Message));
            }
        }

      

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PurchasingReportDelete(dataPurchasingReport model)
        {
            if (!CMS.HasAction(Permissions.PurchasingReport.Delete, Apps.TTT))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            List<dataPurchasingReport> DeletedLanguages = DeletePurchasingReports(new List<dataPurchasingReport> { model });

            try
            {
                DbTTT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbTTT.SingleDeleteMessage(DeletedLanguages, DataTableNames.PurchasingReportsDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyPurchasingReport(model.PurchasingReportGUID);
            }
            catch (Exception ex)
            {
                return Json(DbTTT.ErrorMessage(ex.Message));
            }

        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult PurchasingReportRestore(dataPurchasingReport model)
        {
            if (!CMS.HasAction(Permissions.PurchasingReport.Restore, Apps.TTT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActivePurchasingReport(model))
            {
                return Json(DbTTT.RecordExists());
            }

            List<dataPurchasingReport> RestoredLanguages = RestorePurchasingReports(Portal.SingleToList(model));

            try
            {
                DbTTT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbTTT.SingleRestoreMessage(RestoredLanguages, DataTableNames.PurchasingReportsDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyPurchasingReport(model.PurchasingReportGUID);
            }
            catch (Exception ex)
            {
                return Json(DbTTT.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult PurchasingReportsDataTableDelete(List<dataPurchasingReport> models)
        {
            if (!CMS.HasAction(Permissions.PurchasingReport.Delete, Apps.TTT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataPurchasingReport> DeletedLanguages = DeletePurchasingReports(models);

            try
            {
                DbTTT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbTTT.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.PurchasingReportsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbTTT.ErrorMessage(ex.Message));
            }

        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult PurchasingReportsDataTableRestore(List<dataPurchasingReport> models)
        {
            if (!CMS.HasAction(Permissions.PurchasingReport.Restore, Apps.TTT))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataPurchasingReport> RestoredLanguages = RestorePurchasingReports(models);

            try
            {
                DbTTT.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbTTT.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.PurchasingReportsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbTTT.ErrorMessage(ex.Message));
            }
        }

        private List<dataPurchasingReport> DeletePurchasingReports(List<dataPurchasingReport> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<dataPurchasingReport> DeletedPurchasingReports = new List<dataPurchasingReport>();

            string query = DbTTT.QueryBuilder(models, Permissions.PurchasingReport.DeleteGuid, SubmitTypes.Delete, "");

            var PurchasingReports = DbTTT.Database.SqlQuery<dataPurchasingReport>(query).ToList();

            //var PurchasingReport = DbTTT.dataPurchasingReport.Where(x => x.PurchasingReportGUID == models.FirstOrDefault().PurchasingReportGUID).FirstOrDefault();
            foreach (var PurchasingReport in PurchasingReports)
            {
                DeletedPurchasingReports.Add(DbTTT.Delete(PurchasingReport, ExecutionTime, Permissions.PurchasingReport.DeleteGuid, DbCMS));

            }

            var DeletedRequisitions = DeletedPurchasingReports.SelectMany(a => a.dataRequisition).Where(l => l.Active).ToList();
            foreach (var Requisition in DeletedRequisitions)
            {
                DbTTT.Delete(Requisition, ExecutionTime, Permissions.PurchasingReport.DeleteGuid, DbCMS);
            }
            //var DeletedPurchasingReportsItem = DeletedPurchasingReports.SelectMany(a => a.dataUnitBuyerItem).Where(l => l.Active).ToList();
            //foreach (var ithm in DeletedPurchasingReportsItem)
            //{
            //    DbTTT.Delete(ithm, ExecutionTime, Permissions.PurchasingReport.DeleteGuid, DbCMS);
            //}
            return DeletedPurchasingReports;
        }

        private List<dataPurchasingReport> RestorePurchasingReports(List<dataPurchasingReport> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<dataPurchasingReport> RestoredLanguages = new List<dataPurchasingReport>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbTTT.QueryBuilder(models, Permissions.PurchasingReport.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var PurchasingReports = DbTTT.Database.SqlQuery<dataPurchasingReport>(query).ToList();

            //var PurchasingReport = DbTTT.dataPurchasingReport.Where(x => x.PurchasingReportGUID == models.FirstOrDefault().PurchasingReportGUID).FirstOrDefault();
            foreach (var PurchasingReport in PurchasingReports)
            {
                if (!ActivePurchasingReport(PurchasingReport))
                {
                    RestoredLanguages.Add(DbTTT.Restore(PurchasingReport, Permissions.PurchasingReport.DeleteGuid, Permissions.PurchasingReport.RestoreGuid, RestoringTime, DbCMS));
                }
            }
            //var RestoreVoucher = PurchasingReports.SelectMany(a => a.dataVoucher).Where(l =>! l.Active).ToList();
            //foreach (var voucher in RestoreVoucher)
            //{
            //    DbTTT.Restore(voucher, Permissions.PurchasingReport.DeleteGuid, Permissions.PurchasingReport.RestoreGuid, RestoringTime, DbCMS);
            //}
            //var RestoreUnitBuyerItems = PurchasingReports.SelectMany(a => a.dataUnitBuyerItem).Where(l =>! l.Active).ToList();
            //foreach (var item in RestoreUnitBuyerItems)
            //{
            //    DbTTT.Restore(item, Permissions.PurchasingReport.DeleteGuid, Permissions.PurchasingReport.RestoreGuid, RestoringTime, DbCMS);       
            //        }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyPurchasingReport(Guid PK)
        {
            dataPurchasingReport dbModel = new dataPurchasingReport();

            var Language = DbTTT.dataPurchasingReport.Where(l => l.PurchasingReportGUID == PK).FirstOrDefault();
            var dbLanguage = DbTTT.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.dataPurchasingReportRowVersion.SequenceEqual(dbModel.dataPurchasingReportRowVersion))
            {
                return Json(DbTTT.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbTTT, dbModel, "PurchasingReportsContainer"));
        }

        private bool ActivePurchasingReport(dataPurchasingReport model)
        {
            int LanguageID = DbTTT.dataPurchasingReport
                                  .Where(x => x.PurchasingReportGUID == model.PurchasingReportGUID &&
                                              x.PurchasingReportGUID != model.PurchasingReportGUID &&
                                              x.Active).Count();
            if (LanguageID > 0)
            {
                ModelState.AddModelError("PurchasingReportGUID", "Medical Item already exists"); //From resource ?????? Amer  
            }

            return (LanguageID > 0);
        }

     

        [HttpPost]
        public FineUploaderResult UploadFiles(FineUpload upload)
        {
            string error = "";
            if (FileTypeValidator.IsExcel(upload.InputStream))
            {
                string FilePath = ConfigurationManager.AppSettings["DataFolder"] + "\\Uploads\\TTT\\"+ Guid.NewGuid() + ".xlsx";
                try
                {
                    upload.SaveAs(FilePath);
                    using (ExcelPackage package = new ExcelPackage(new FileInfo(FilePath)))
                    {
                        ExcelWorksheet workSheet = package.Workbook.Worksheets[1];
                        workSheet.View.TabSelected = true;
                        if (workSheet != null)
                        {
                            int totalRows = workSheet.Dimension.End.Row;
                            int BgtYr = Convert.ToInt32(workSheet.Cells["A6"].Value.ToString().Split('=')[1]);
                            int CostCentre = Convert.ToInt32(workSheet.Cells["B6"].Value.ToString().Split('=')[1]);
                            dataPurchasingReport purchasingReport = DbTTT.dataPurchasingReport.Where(x => x.CostCentre == CostCentre && x.BgtYr == BgtYr).FirstOrDefault();
                            if (purchasingReport == null)
                            {
                                purchasingReport = new dataPurchasingReport();
                                purchasingReport.PurchasingReportGUID = Guid.NewGuid();
                                purchasingReport.ReportID = workSheet.Cells["B2"].Value.ToString();
                                purchasingReport.BusinessUnit = workSheet.Cells["B4"].Value.ToString();
                                purchasingReport.Buyer = workSheet.Cells["D4"].Value.ToString();
                                purchasingReport.Vendor = workSheet.Cells["F4"].Value.ToString();
                                purchasingReport.PuchaseOrder = workSheet.Cells["H4"].Value != null ? workSheet.Cells["H4"].Value.ToString() : "";
                                purchasingReport.BudgetDateFrom = Convert.ToDateTime(workSheet.Cells["B5"].Value.ToString());
                                purchasingReport.BudgetDateTo = Convert.ToDateTime(workSheet.Cells["D5"].Value.ToString());
                                purchasingReport.RemainingAmount = workSheet.Cells["G5"].Value != null ? Convert.ToDouble(workSheet.Cells["G5"].Value.ToString()) : 0;
                                purchasingReport.BgtYr = BgtYr;
                                purchasingReport.CostCentre = CostCentre;

                                DbTTT.Create(purchasingReport, Permissions.PurchasingReport.CreateGuid, DateTime.Now, DbCMS);
                            }

                            //dataRequisition  && dataPurchaseOrder Import
                            if (workSheet.Cells["B8"].Value.ToString() == "REQ ID")
                            {
                                var REQ_IDs = DbTTT.dataRequisition.Where(x => x.dataPurchasingReport.CostCentre == CostCentre
                                             && x.dataPurchasingReport.BgtYr == BgtYr).Select(x => new
                                             {
                                                 x.REQ_ID,
                                                 x.L_S_D,
                                                 x.RequisitionGUID
                                             }).ToList();
                                var PO_IDs = (from a in DbTTT.dataPurchaseOrder
                                              join b in DbTTT.dataRequisition.Where(x => x.dataPurchasingReport.CostCentre == CostCentre
                                              && x.dataPurchasingReport.BgtYr == BgtYr)
                                              on a.RequisitionGUID equals b.RequisitionGUID
                                              select new
                                              {
                                                  a.PO_ID,
                                                  b.L_S_D,
                                                  a.PurchaseOrderGUID,
                                                  a.RequisitionGUID

                                              }
                                              ).ToList();
                                List<dataRequisition> requisitions = new List<dataRequisition>();
                                List<dataPurchaseOrder> purchaseOrders = new List<dataPurchaseOrder>();
                                Guid RequisitionGUID = Guid.NewGuid();
                                for (int i = 9; i < totalRows; i++)
                                {
                                    if (ChecKRootRow(workSheet, i))
                                    {
                                        try
                                        {

                                            dataRequisition requisition = new dataRequisition();
                                            requisition.PurchasingReportGUID = purchasingReport.PurchasingReportGUID;
                                            requisition.Unit = workSheet.Cells["A" + i].Value.ToString();
                                            requisition.REQ_ID = workSheet.Cells["B" + i].Value.ToString();
                                            requisition.L_S_D = workSheet.Cells["C" + i].Value.ToString();
                                            requisition.Requestor = workSheet.Cells["D" + i].Value.ToString();
                                            requisition.Item_Description = workSheet.Cells["E" + i].Value.ToString();
                                            requisition.Liq = workSheet.Cells["F" + i].Value.ToString();
                                            requisition.B_St = workSheet.Cells["G" + i].Value.ToString();
                                            requisition.R_St = workSheet.Cells["H" + i].Value.ToString();
                                            requisition.Req_Monetary_Amount = Convert.ToDouble(workSheet.Cells["I" + i].Value.ToString().Split('.')[0]);
                                            requisition.Orig_Req_Amt = Convert.ToDouble(workSheet.Cells["J" + i].Value);
                                            requisition.Liquidated_Amt = Convert.ToDouble(workSheet.Cells["L" + i].Value.ToString());
                                            requisition.Remaining_Amt = Convert.ToDouble(workSheet.Cells["N" + i].Value.ToString());
                                            requisition.Total_PO_Amt = Convert.ToDouble(workSheet.Cells["O" + i].Value.ToString());
                                            requisition.Active = true;
                                            //Check if the recourd exist or not 
                                            var REQ_ID = REQ_IDs.Where(x => x.REQ_ID == requisition.REQ_ID && x.L_S_D == requisition.L_S_D).FirstOrDefault();
                                            if (REQ_ID == null)
                                            {
                                                RequisitionGUID = Guid.NewGuid();
                                                requisition.RequisitionGUID = RequisitionGUID;
                                                requisitions.Add(requisition);
                                            }
                                            else
                                            {
                                                RequisitionGUID = REQ_ID.RequisitionGUID;
                                            }

                                        }
                                        catch
                                        {
                                        }
                                    }

                                    if (CheckIfPurchaseOrder(workSheet, i))
                                    {
                                        try
                                        {
                                            dataPurchaseOrder purchaseOrder = new dataPurchaseOrder();
                                            purchaseOrder.RequisitionGUID = RequisitionGUID;
                                            purchaseOrder.PO_ID = workSheet.Cells["B" + i].Value.ToString();
                                            purchaseOrder.L_S_D = workSheet.Cells["C" + i].Value.ToString();
                                            purchaseOrder.Vendor = workSheet.Cells["D" + i].Value.ToString();
                                            purchaseOrder.Am_O = workSheet.Cells["E" + i].Value.ToString();
                                            purchaseOrder.Fin = workSheet.Cells["F" + i].Value.ToString();
                                            purchaseOrder.Liq = workSheet.Cells["G" + i].Value.ToString();
                                            purchaseOrder.PO_St = workSheet.Cells["H" + i].Value.ToString();
                                            purchaseOrder.Monetary_Amount = Convert.ToDouble(workSheet.Cells["I" + i].Value.ToString().Split('.')[0]);
                                            purchaseOrder.USD_Amount = Convert.ToDouble(workSheet.Cells["J" + i].Value);
                                            purchaseOrder.Active = true;
                                            //Check if the recourd exist or not 
                                            var PO_ID = PO_IDs.Where(x => x.PO_ID == purchaseOrder.PO_ID && x.RequisitionGUID == RequisitionGUID).FirstOrDefault();
                                            if (PO_ID == null)
                                            {
                                                purchaseOrder.PurchaseOrderGUID = Guid.NewGuid();
                                                purchaseOrders.Add(purchaseOrder);
                                            }

                                        }
                                        catch
                                        {

                                        }

                                    }
                                    Console.WriteLine(i);
                                }
                                // DbTTT.Create(purchasingReport, Permissions.PurchasingReport.CreateGuid, DateTime.Now, DbCMS);
                                DbTTT.CreateBulk(requisitions, Permissions.PurchasingReport.CreateGuid, DateTime.Now, DbCMS);
                                DbTTT.CreateBulk(purchaseOrders, Permissions.PurchasingReport.CreateGuid, DateTime.Now, DbCMS);
                            }
                            //dataPurchaseOrderVoucher  && dataPurchaseOrderItem Import
                            if (workSheet.Cells["B8"].Value.ToString() == "PO ID")
                            {
                                List<dataPurchaseOrderVoucher> vouchers = new List<dataPurchaseOrderVoucher>();
                                List<dataPurchaseOrderItem> Items = new List<dataPurchaseOrderItem>();
                                Guid PurchaseOrderItemGUID = Guid.NewGuid();

                                var PO_IDs = (from a in DbTTT.dataPurchaseOrder
                                              join b in DbTTT.dataRequisition.Where(x => x.dataPurchasingReport.CostCentre == CostCentre
                                              && x.dataPurchasingReport.BgtYr == BgtYr)
                                              on a.RequisitionGUID equals b.RequisitionGUID
                                              select new
                                              {
                                                  a.PO_ID,
                                                  b.L_S_D,
                                                  a.PurchaseOrderGUID
                                              }
                                              ).ToList();
                                var POItems = (from a in DbTTT.dataPurchaseOrderItem
                                               join c in DbTTT.dataPurchaseOrder on a.PurchaseOrderGUID equals c.PurchaseOrderGUID
                                               join b in DbTTT.dataRequisition.Where(x => x.dataPurchasingReport.CostCentre == CostCentre
                                               && x.dataPurchasingReport.BgtYr == BgtYr)
                                               on c.RequisitionGUID equals b.RequisitionGUID
                                               select a).Select(x => new
                                               {
                                                   x.PO_ID,
                                                   x.L_S_D,
                                                   x.PurchaseOrderGUID,
                                                   x.PurchaseOrderItemGUID
                                               }).ToList();
                                var VoucherReults = (from a in DbTTT.dataPurchaseOrderVoucher
                                                     join d in DbTTT.dataPurchaseOrderItem on a.PurchaseOrderItemGUID equals d.PurchaseOrderItemGUID
                                                     join c in DbTTT.dataPurchaseOrder on d.PurchaseOrderGUID equals c.PurchaseOrderGUID
                                                     join b in DbTTT.dataRequisition.Where(x => x.dataPurchasingReport.CostCentre == CostCentre
                                                     && x.dataPurchasingReport.BgtYr == BgtYr)
                                                     on c.RequisitionGUID equals b.RequisitionGUID
                                                     select a).Select(x => new
                                                     {
                                                         x.PurchaseOrderItemGUID,
                                                         x.PurchaseOrderVoucherGUID,
                                                         x.Voucher,
                                                         x.Invoice_Number
                                                     }).ToList();
                                for (int i = 9; i < totalRows; i++)
                                {
                                    if (ChecKRootRow(workSheet, i))
                                    {
                                        try
                                        {
                                            bool CellShift = workSheet.Cells["B" + i].Value == null ? true : false;
                                            dataPurchaseOrderItem Item = new dataPurchaseOrderItem();
                                            Item.Unit = workSheet.Cells["A" + i].Value.ToString();
                                            Item.PO_ID = !CellShift ? workSheet.Cells["B" + i].Value.ToString() : workSheet.Cells["C" + i].Value.ToString();
                                            Item.L_S_D = !CellShift ? workSheet.Cells["C" + i].Value.ToString() : workSheet.Cells["D" + i].Value.ToString();
                                            Item.Buyer = !CellShift ? workSheet.Cells["D" + i].Value.ToString() : workSheet.Cells["E" + i].Value.ToString();
                                            Item.ItemDescription = !CellShift ? workSheet.Cells["E" + i].Value.ToString() : workSheet.Cells["F" + i].Value.ToString();
                                            Item.PO_St = !CellShift ? workSheet.Cells["F" + i].Value.ToString() : workSheet.Cells["G" + i].Value.ToString();
                                            Item.Am_O = !CellShift ? workSheet.Cells["G" + i].Value.ToString() : workSheet.Cells["H" + i].Value.ToString();
                                            Item.PO_Liq = !CellShift ? workSheet.Cells["H" + i].Value.ToString() : workSheet.Cells["I" + i].Value.ToString();
                                            Item.PO_CUR = !CellShift ? workSheet.Cells["I" + i].Value.ToString() : workSheet.Cells["J" + i].Value.ToString();
                                            Item.Vendor = !CellShift ? workSheet.Cells["J" + i].Value.ToString() : workSheet.Cells["K" + i].Value.ToString();
                                            Item.Purc_Ord_Amt = !CellShift ? Convert.ToDouble(workSheet.Cells["K" + i].Value.ToString()) : Convert.ToDouble(workSheet.Cells["L" + i].Value.ToString());
                                            Item.Liquidated_Amt = Convert.ToDouble(workSheet.Cells["M" + i].Value.ToString());
                                            Item.Remaining_Amt = Convert.ToDouble(workSheet.Cells["O" + i].Value.ToString());
                                            Item.Vouchered_Amt = Convert.ToDouble(workSheet.Cells["P" + i].Value.ToString());
                                            Item.Active = true;
                                            var PO_ID = PO_IDs.Where(x => x.PO_ID == Item.PO_ID && x.L_S_D == Item.L_S_D).FirstOrDefault();
                                            var POItem = POItems.Where(x => x.PO_ID == Item.PO_ID && x.L_S_D == Item.L_S_D).FirstOrDefault();
                                            if (PO_ID != null && POItem == null)
                                            {
                                                PurchaseOrderItemGUID = Guid.NewGuid();
                                                Item.PurchaseOrderItemGUID = PurchaseOrderItemGUID;
                                                Item.PurchaseOrderGUID = PO_ID.PurchaseOrderGUID;
                                                Items.Add(Item);
                                            }
                                            if (PO_ID != null && POItem != null)
                                            {
                                                PurchaseOrderItemGUID = POItem.PurchaseOrderItemGUID;
                                            }
                                            if (PO_ID == null && POItem == null)
                                            {
                                                PurchaseOrderItemGUID = Guid.Empty;
                                            }
                                        }
                                        catch
                                        {
                                        }
                                    }
                                    if (CheckIfRowIsVoucher(workSheet, i))
                                    {
                                        try
                                        {

                                            if (PurchaseOrderItemGUID != Guid.Empty)
                                            {

                                                dataPurchaseOrderVoucher voucher = new dataPurchaseOrderVoucher();
                                                voucher.PurchaseOrderVoucherGUID = Guid.NewGuid();
                                                voucher.PurchaseOrderItemGUID = PurchaseOrderItemGUID;
                                                voucher.Voucher = workSheet.Cells["C" + i].Value.ToString();
                                                voucher.L_D = Convert.ToDateTime(workSheet.Cells["D" + i].Value.ToString());
                                                voucher.Invoice_Number = workSheet.Cells["E" + i].Value.ToString();
                                                voucher.Monetary_Amount = Convert.ToDouble(workSheet.Cells["F" + i].Value.ToString().Split('.')[0]);
                                                voucher.USD_Amount = Convert.ToDouble(workSheet.Cells["G" + i].Value.ToString());
                                                voucher.Fin = workSheet.Cells["H" + i].Value.ToString();
                                                voucher.AS_ = workSheet.Cells["I" + i].Value.ToString();
                                                voucher.MS = workSheet.Cells["G" + i].Value.ToString();
                                                voucher.HBS = workSheet.Cells["K" + i].Value.ToString();
                                                voucher.DTS = workSheet.Cells["L" + i].Value.ToString();
                                                voucher.Active = true;

                                                var voucherResult = VoucherReults.Where(x => x.Voucher == voucher.Voucher && x.Invoice_Number == voucher.Invoice_Number && x.PurchaseOrderItemGUID == PurchaseOrderItemGUID).FirstOrDefault();
                                                if (voucherResult == null)
                                                {
                                                    vouchers.Add(voucher);
                                                }

                                            }
                                        }
                                        catch
                                        {

                                        }

                                    }
                                    Console.WriteLine(i);
                                }
                                DbTTT.CreateBulk(vouchers, Permissions.PurchasingReport.CreateGuid, DateTime.Now, DbCMS);
                                DbTTT.CreateBulk(Items, Permissions.PurchasingReport.CreateGuid, DateTime.Now, DbCMS);

                            }

                            DbTTT.SaveChanges();
                                DbCMS.SaveChanges();
                            System.IO.File.Delete(FilePath);
                        }
                    }
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                }
            }
            else
            {
                error = "Error";
            }
            return new FineUploaderResult((error == "" ? true : false), new { Error = error, path = "", success = (error == "" ? true : false) });
        }

      

        private bool CheckIfRowIsVoucher(ExcelWorksheet workSheet, int index)
        {
            bool Checked = true;
            if (workSheet.Cells["A" + index].Value != null && workSheet.Cells["B" + index].Value != null) { Checked = false; }
            if (workSheet.Cells["A" + index].Value != null && workSheet.Cells["B" + index].Value == null) { Checked = false; }
            if (workSheet.Cells["A" + index].Value == null && workSheet.Cells["B" + index].Value == null) { Checked = false; }
            if (workSheet.Cells["A" + index].Value != null)
            {
                if (workSheet.Cells["A" + index].Value.ToString() == "VCHR BU") { Checked = false; }
            }
            if (workSheet.Cells["B" + index].Value != null)
            {
                if (workSheet.Cells["B" + index].Value.ToString() == "--------------------------------------------------------------------------------------------------------------------------------") { Checked = false; }
            }
            return Checked;
        }



        private bool ChecKRootRow(ExcelWorksheet workSheet,int index)
        {
            bool Checked = true;
            //if (workSheet.Cells["A" + index].Value != null && workSheet.Cells["B" + index].Value == null){Checked=false;}
            if (workSheet.Cells["A" + index].Value == null && workSheet.Cells["B" + index].Value != null) { Checked = false; }
            if (workSheet.Cells["A" + index].Value == null && workSheet.Cells["B" + index].Value == null) { Checked = false; }
            if (workSheet.Cells["A" + index].Value != null)
            {
                if (workSheet.Cells["A" + index].Value.ToString() == "VCHR BU") { Checked = false; }
            }
            if (workSheet.Cells["B" + index].Value != null)
            {
                if (workSheet.Cells["B" + index].Value.ToString() == "--------------------------------------------------------------------------------------------------------------------------------") { Checked = false; }
            }
            return Checked;
        }

        private bool CheckIfPurchaseOrder(ExcelWorksheet workSheet, int index)
        {
            bool Checked = true;
            if (workSheet.Cells["A" + index].Value != null && workSheet.Cells["B" + index].Value != null) { Checked = false; }
            if (workSheet.Cells["A" + index].Value != null && workSheet.Cells["B" + index].Value == null) { Checked = false; }
            if (workSheet.Cells["A" + index].Value == null && workSheet.Cells["B" + index].Value == null) { Checked = false; }
            if (workSheet.Cells["A" + index].Value != null)
            {
                if (workSheet.Cells["B" + index].Value.ToString() == "PO ID") { Checked = false; }
            }
            if (workSheet.Cells["B" + index].Value != null)
            {
                if (workSheet.Cells["B" + index].Value.ToString() == "--------------------------------------------------------------------------------------------------------------------------------") { Checked = false; }
            }
            return Checked;
        }

        public void Download(Guid PurchasingReportGUID)
        {
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.ZoomMode = ZoomMode.Percent;
            reportViewer.ZoomPercent = 90;
            reportViewer.Width = System.Web.UI.WebControls.Unit.Pixel(850);
            reportViewer.Height = System.Web.UI.WebControls.Unit.Pixel(600);
            reportViewer.AsyncRendering = true;
            reportViewer.LocalReport.DataSources.Clear();

            CostCenterTableAdapter appointmentsSlipTableAdapter = new CostCenterTableAdapter();
            TTTDataSet TTTDataSet = new TTTDataSet();
            appointmentsSlipTableAdapter.Fill(TTTDataSet.CostCenter, PurchasingReportGUID);
            ReportDataSource reportDataSource = new ReportDataSource("CostCenter", TTTDataSet.Tables["CostCenter"]);
            reportViewer.LocalReport.DataSources.Add(reportDataSource);
            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Areas/TTT/Views/PurchasingReports\CostCenter.rdlc";

            Warning[] warnings;
            string[] streamIds;
            string contentType;
            string encoding;
            string extension;

            //Export the RDLC Report to Byte Array.
            byte[] bytes = reportViewer.LocalReport.Render("EXCEL", null, out contentType, out encoding, out extension, out streamIds, out warnings);

            //Download the RDLC Report in Word, Excel, PDF and Image formats.
            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "";
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.ContentType = contentType;
            Response.AppendHeader("Content-Disposition", "attachment; filename=RDLC." + extension);
            Response.BinaryWrite(bytes);
            Response.Flush();
            Response.End();

        }



        #endregion
    }
}