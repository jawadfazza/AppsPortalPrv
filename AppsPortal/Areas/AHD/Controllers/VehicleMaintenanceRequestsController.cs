using AHD_DAL.Model;
using AHD_DAL.ViewModels;
using AppsPortal.Areas.AHD.RDLC.AHDDataSetTableAdapters;
using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.ViewModels;
using AutoMapper;
using iTextSharp.text;
using iTextSharp.text.pdf;
using LinqKit;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace AppsPortal.Areas.AHD.Controllers
{
    public class VehicleMaintenanceRequestsController : AHDBaseController
    {
        // GET: AHD/VehicleMaintenanceRequests
        [Route("AHD/VehicleMaintenaceRequestIndex/")]
        public ActionResult VehicleMaintenaceRequestIndex()
        {
            if (!CMS.HasAction(Permissions.VehicleMaintenanceRequest.Access, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }

            return View("~/Areas/AHD/Views/VehicleMaintenanceRequest/Index.cshtml");
        }

        [Route("AHD/VehicleMaintenceRequestDataTable/")]

        public JsonResult VehicleMaintenceRequestDataTable(DataTableRecievedOptions options)
        {

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<VehicleMaintenanceRequestDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<VehicleMaintenanceRequestDataTableModel>(DataTable.Filters);
            }
            var All = (
               from a in DbAHD.dataVehicleMaintenanceRequest.AsExpandable()
               join b in DbAHD.codeVehicle.Where(x => x.Active) on a.VehicleGUID equals b.VehicleGUID into LJ1
               from R1 in LJ1.DefaultIfEmpty()
               join c in DbAHD.codeTablesValuesLanguages.Where(x => x.Active && x.codeTablesValues.TableGUID == LookupTables.AHDVehicleType && x.LanguageID == LAN) on R1.VehicleTypeGUID equals c.ValueGUID into LJ2
               from R2 in LJ2.DefaultIfEmpty()
               join d in DbAHD.codeTablesValuesLanguages.Where(x => x.Active && x.codeTablesValues.TableGUID == LookupTables.AHDVechileModel && x.LanguageID == LAN) on R1.VechileModelGUID equals d.ValueGUID into LJ3
               from R3 in LJ3.DefaultIfEmpty()
               join e in DbAHD.codeTablesValuesLanguages.Where(x => x.Active && x.codeTablesValues.TableGUID == LookupTables.AHDVehileColor && x.LanguageID == LAN) on R1.VehileColorGUID equals e.ValueGUID into LJ4
               from R4 in LJ4.DefaultIfEmpty()
               select new VehicleMaintenanceRequestDataTableModel
               {
                   VehicleMaintenanceRequestGUID = a.VehicleMaintenanceRequestGUID,
                   VehicleGUID = a.VehicleGUID.ToString(),
                   RequestNumber = a.RequestNumber,
                   VehicleNumber = R1.VehicleNumber,
                   VechileModel = R3.ValueDescription,
                   VehicleTypeGUID = R1.VehicleTypeGUID.ToString(),
                   VechileModelGUID = R1.VechileModelGUID.ToString(),
                   VehileColorGUID = R1.VehileColorGUID.ToString(),
                   Active = a.Active,
                   ChassisNumber = R1.ChassisNumber,
                   EngineNumber = R1.EngineNumber,
                   LastRenewalDate = a.LastRenewalDate,
                   VehicleColor = R4.ValueDescription,
                   RequestDate = a.RequestDate,
                   ConfirmationRenewalDate = a.ConfirmationRenewalDate,
                   LastFlowStatusGUID = a.LastFlowStatusGUID.ToString(),
                   LastFlowStatus = a.LastFlowStatus,
                   Comments = a.Comments,
                   dataVehicleMaintenanceRequestRowVersion = a.dataVehicleMaintenanceRequestRowVersion
               }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<VehicleMaintenanceRequestDataTableModel> Result = Mapper.Map<List<VehicleMaintenanceRequestDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).OrderByDescending(x => x.RequestNumber).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("AHD/VehicleMaintenceRequest/Create/")]
        public ActionResult VehicleMaintenceRequestCreate()
        {
            if (!CMS.HasAction(Permissions.VehicleMaintenanceRequest.Create, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            return PartialView("~/Areas/AHD/Views/VehicleMaintenanceRequest/_VehicleMaintenanceRequestForm.cshtml", new VehicleMaintenanceRequesModel { VehicleMaintenanceRequestGUID = Guid.Empty });

        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult VehicleMaintenceRequestCreate(VehicleMaintenanceRequesModel model)
        {
            if (!CMS.HasAction(Permissions.VehicleMaintenanceRequest.Create, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            if (!ModelState.IsValid) return PartialView("~/Areas/AHD/Views/VehicleMaintenanceRequest/_VehicleMaintenanceRequestForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            dataVehicleMaintenanceRequest vehicleMaintenance = Mapper.Map(model, new dataVehicleMaintenanceRequest());
            vehicleMaintenance.VehicleMaintenanceRequestGUID = EntityPK;
            vehicleMaintenance.VehicleGUID = model.VehicleGUID;
            vehicleMaintenance.RequestNumber = model.RequestNumber;
            vehicleMaintenance.RequestYear = model.RequestDate.Value.Year;
            vehicleMaintenance.LastFlowStatusGUID = VehicleMaintenanceRequestFlowStatus.Submitted;
            vehicleMaintenance.LastFlowStatus = "Submitted";
            vehicleMaintenance.Comments = model.Comments;
            vehicleMaintenance.RequestNumber = model.RequestNumber;
            vehicleMaintenance.LastRenewalDate = model.LastRenewalDate;



            dataVehicleMaintenanceRequestFlow flow = new dataVehicleMaintenanceRequestFlow
            {
                VehicleMaintenanceRequestFlowGUID = Guid.NewGuid(),
                VehicleMaintenanceRequestGUID = EntityPK,
                FlowStatusGUID = VehicleMaintenanceRequestFlowStatus.Submitted,
                IsLastAction = true,
                CreateByGUID = UserGUID,
                CreateDate = ExecutionTime,
                OrderId = 1


            };
            DbAHD.Create(vehicleMaintenance, Permissions.VehicleMaintenanceRequest.CreateGuid, ExecutionTime, DbCMS);
            DbAHD.Create(flow, Permissions.VehicleMaintenanceRequest.CreateGuid, ExecutionTime, DbCMS);


            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.VehicleMaintenceRequestDataTable, ControllerContext, "VehicleMaintenanceRequestLanguagesFormControls"));
            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.VehicleMaintenanceRequest.Create, Apps.AHD, new UrlHelper(Request.RequestContext).Action("Create", "VehicleMaintenanceRequests", new { Area = "AHD" })), Container = "VehicleMaintenanceRequestDetailFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.VehicleMaintenanceRequest.Update, Apps.AHD), Container = "VehicleMaintenanceRequestDetailFormControls" });

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();

                return Json(DbAHD.SingleUpdateMessage(DataTableNames.VehicleMaintenceRequestDataTable, DbAHD.PrimaryKeyControl(vehicleMaintenance), DbAHD.RowVersionControls(Portal.SingleToList(vehicleMaintenance))));
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }


        public ActionResult VehicleMaintenceRequestUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.VehicleMaintenanceRequest.Create, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            var model = (from a in DbAHD.dataVehicleMaintenanceRequest.WherePK(PK)

                         select new VehicleMaintenanceRequesModel
                         {
                             VehicleMaintenanceRequestGUID = a.VehicleMaintenanceRequestGUID,
                             VehicleGUID = a.VehicleGUID,
                             RequestNumber = a.RequestNumber,
                             RequestYear = a.RequestYear.ToString(),
                             RequestDate = a.RequestDate,
                             ConfirmationRenewalDate = a.ConfirmationRenewalDate,
                             LastFlowStatusGUID = a.LastFlowStatusGUID,
                             LastFlowStatus = a.LastFlowStatus,
                             LastRenewalDate = a.LastRenewalDate,
                             Comments = a.Comments,
                             Active = a.Active,
                             dataVehicleMaintenanceRequestRowVersion = a.dataVehicleMaintenanceRequestRowVersion
                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("VehicleMaintenanceRequest", "VehicleMaintenanceRequests", new { Area = "AHD" }));
            return PartialView("~/Areas/AHD/Views/VehicleMaintenanceRequest/_VehicleMaintenanceRequestForm.cshtml", model);

        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult VehicleMaintenceRequestUpdate(VehicleMaintenanceRequesModel model)
        {
            if (!CMS.HasAction(Permissions.VehicleMaintenanceRequest.Update, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            if (!ModelState.IsValid) return PartialView("~/Areas/AHD/Views/VehicleMaintenanceRequest/_VehicleMaintenanceRequestForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;


            var renewalRes = DbAHD.dataVehicleMaintenanceRequest.Where(x => x.VehicleMaintenanceRequestGUID == model.VehicleMaintenanceRequestGUID).FirstOrDefault();
            if (model.LastFlowStatusGUID != null && renewalRes.LastFlowStatusGUID != model.LastFlowStatusGUID)
            {
                dataVehicleMaintenanceRequest VehicleMaintenanceRequest = Mapper.Map(model, new dataVehicleMaintenanceRequest());
                if (VehicleMaintenanceRequest.LastFlowStatusGUID == VehicleMaintenanceRequestFlowStatus.Closed)
                {

                    var vechile = DbAHD.codeVehicle.Where(x => x.VehicleGUID == model.VehicleGUID).FirstOrDefault();
                    vechile.LastRenewalDate = model.LastRenewalDate;
                    DbAHD.Update(vechile, Permissions.VehicleMaintenanceRequest.UpdateGuid, ExecutionTime, DbCMS);
                }
                VehicleMaintenanceRequest.LastFlowStatus = DbAHD.codeTablesValuesLanguages.Where(x => x.ValueGUID == model.LastFlowStatusGUID && x.LanguageID == LAN).Select(x => x.ValueDescription).FirstOrDefault();
                var priFlow = DbAHD.dataVehicleMaintenanceRequestFlow.Where(x => x.VehicleMaintenanceRequestGUID == model.VehicleMaintenanceRequestGUID && x.IsLastAction == true).FirstOrDefault();
                priFlow.IsLastAction = false;
                DbAHD.Update(priFlow, Permissions.VehicleMaintenanceRequest.UpdateGuid, ExecutionTime, DbCMS);
                dataVehicleMaintenanceRequestFlow flow = new dataVehicleMaintenanceRequestFlow
                {
                    VehicleMaintenanceRequestFlowGUID = Guid.NewGuid(),
                    VehicleMaintenanceRequestGUID = model.VehicleMaintenanceRequestGUID,
                    FlowStatusGUID = model.LastFlowStatusGUID,
                    IsLastAction = true,
                    CreateByGUID = UserGUID,
                    CreateDate = ExecutionTime,
                    OrderId = priFlow.OrderId + 1


                };

                DbAHD.Update(VehicleMaintenanceRequest, Permissions.VehicleMaintenanceRequest.UpdateGuid, ExecutionTime, DbCMS);
                DbAHD.Create(flow, Permissions.VehicleMaintenanceRequest.CreateGuid, ExecutionTime, DbCMS);
            }
            else
            {
                dataVehicleMaintenanceRequest VehicleMaintenanceRequest = Mapper.Map(model, new dataVehicleMaintenanceRequest());
                if (VehicleMaintenanceRequest.LastFlowStatusGUID == VehicleMaintenanceRequestFlowStatus.Closed)
                {

                    var vechile = DbAHD.codeVehicle.Where(x => x.VehicleGUID == model.VehicleGUID).FirstOrDefault();
                    vechile.LastRenewalDate = model.LastRenewalDate;
                    DbAHD.Update(vechile, Permissions.VehicleMaintenanceRequest.UpdateGuid, ExecutionTime, DbCMS);
                }

                DbAHD.Update(VehicleMaintenanceRequest, Permissions.VehicleMaintenanceRequest.UpdateGuid, ExecutionTime, DbCMS);
            }
            try
            {
                dataVehicleMaintenanceRequest VehicleMaintenanceRequest = Mapper.Map(model, new dataVehicleMaintenanceRequest());
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.SingleUpdateMessage(DataTableNames.VehicleMaintenceRequestDataTable, DbAHD.PrimaryKeyControl(VehicleMaintenanceRequest
), DbAHD.RowVersionControls(Portal.SingleToList(VehicleMaintenanceRequest))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyVehicleMaintenceRequest((Guid)model.VehicleMaintenanceRequestGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult VehicleMaintenceRequestDelete(dataVehicleMaintenanceRequest model)
        {
            if (!CMS.HasAction(Permissions.VehicleMaintenanceRequest.Delete, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            List<dataVehicleMaintenanceRequest> DeletedVehicleMaintenceRequest = DeleteVehicleMaintenceRequest(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.VehicleMaintenanceRequest.Restore, Apps.AHD), Container = "VehicleMaintenceRequestFormControls" });

            try
            {
                int CommitedRows = DbAHD.SaveChanges();
                DbAHD.SaveChanges();
                return Json(DbAHD.SingleDeleteMessage(CommitedRows, DeletedVehicleMaintenceRequest.FirstOrDefault(), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyVehicleMaintenceRequest(model.VehicleMaintenanceRequestGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult VehicleMaintenceRequestRestore(dataVehicleMaintenanceRequest model)
        {
            if (!CMS.HasAction(Permissions.VehicleMaintenanceRequest.Restore, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            if (ActiveVehicleMaintenceRequest(model))
            {
                return Json(DbAHD.RecordExists());
            }

            List<dataVehicleMaintenanceRequest> RestoredVehicleMaintenceRequest = RestoreVehicleMaintenceRequests(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.VehicleMaintenanceRequest.Create, Apps.AHD, new UrlHelper(Request.RequestContext).Action("VehicleMaintenceRequestCreate", "Configuration", new { Area = "AHD" })), Container = "VehicleMaintenceRequestFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.VehicleMaintenanceRequest.Update, Apps.AHD), Container = "VehicleMaintenceRequestFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.VehicleMaintenanceRequest.Delete, Apps.AHD), Container = "VehicleMaintenceRequestFormControls" });

            try
            {
                int CommitedRows = DbAHD.SaveChanges();
                DbAHD.SaveChanges();
                return Json(DbAHD.SingleRestoreMessage(CommitedRows, RestoredVehicleMaintenceRequest, DbAHD.PrimaryKeyControl(RestoredVehicleMaintenceRequest.FirstOrDefault()), Url.Action(DataTableNames.VehicleMaintenceRequestDataTable, Portal.GetControllerName(ControllerContext)), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyVehicleMaintenceRequest(model.VehicleMaintenanceRequestGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult VehicleMaintenceRequestDataTableDelete(List<dataVehicleMaintenanceRequest> models)
        {
            if (!CMS.HasAction(Permissions.VehicleMaintenanceRequest.Delete, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            List<dataVehicleMaintenanceRequest> DeletedVehicleMaintenceRequest = DeleteVehicleMaintenceRequest(models);

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.PartialDeleteMessage(DeletedVehicleMaintenceRequest, models, DataTableNames.VehicleMaintenceRequestDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult VehicleMaintenceRequestDataTableRestore(List<dataVehicleMaintenanceRequest> models)
        {
            if (!CMS.HasAction(Permissions.VehicleMaintenanceRequest.Restore, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            List<dataVehicleMaintenanceRequest> RestoredVehicleMaintenceRequest = DeleteVehicleMaintenceRequest(models);

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.PartialRestoreMessage(RestoredVehicleMaintenceRequest, models, DataTableNames.VehicleMaintenceRequestDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        private List<dataVehicleMaintenanceRequest> DeleteVehicleMaintenceRequest(List<dataVehicleMaintenanceRequest> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<dataVehicleMaintenanceRequest> DeletedVehicleMaintenceRequest = new List<dataVehicleMaintenanceRequest>();
            //Fix
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = "SELECT VehicleMaintenanceRequestGUID,CONVERT(varchar(50), VehicleMaintenanceRequestGUID) as C2 ,dataVehicleMaintenanceRequestRowVersion FROM code.dataVehicleMaintenanceRequest where VehicleMaintenanceRequestGUID in (" + string.Join(",", models.Select(x => "'" + x.VehicleMaintenanceRequestGUID + "'").ToArray()) + ")";

            string query = DbAHD.QueryBuilder(models, Permissions.VehicleMaintenanceRequest.DeleteGuid, SubmitTypes.Delete, "");

            var Records = DbAHD.Database.SqlQuery<dataVehicleMaintenanceRequest>(query).ToList();
            foreach (var record in Records)
            {
                DeletedVehicleMaintenceRequest.Add(DbAHD.Delete(record, ExecutionTime, Permissions.VehicleMaintenanceRequest.DeleteGuid, DbCMS));
            }


            return DeletedVehicleMaintenceRequest;
        }

        private List<dataVehicleMaintenanceRequest> RestoreVehicleMaintenceRequests(List<dataVehicleMaintenanceRequest> models)
        {

            DateTime RestoringTime = DateTime.Now;

            List<dataVehicleMaintenanceRequest> RestoredVehicleMaintenceRequest = new List<dataVehicleMaintenanceRequest>();
            //Fix
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = "SELECT VehicleMaintenanceRequestGUID,CONVERT(varchar(50), VehicleMaintenanceRequestGUID) as C2 ,dataVehicleMaintenanceRequestRowVersion FROM code.dataVehicleMaintenanceRequest where VehicleMaintenanceRequestGUID in (" + string.Join(",", models.Select(x => "'" + x.VehicleMaintenanceRequestGUID + "'").ToArray()) + ")";

            string query = DbAHD.QueryBuilder(models, Permissions.VehicleMaintenanceRequest.DeleteGuid, SubmitTypes.Restore, "");

            var Records = DbAHD.Database.SqlQuery<dataVehicleMaintenanceRequest>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveVehicleMaintenceRequest(record))
                {
                    RestoredVehicleMaintenceRequest.Add(DbAHD.Restore(record, Permissions.VehicleMaintenanceRequest.DeleteGuid, Permissions.VehicleMaintenanceRequest.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredVehicleMaintenceRequest;
        }

        private JsonResult ConcurrencyVehicleMaintenceRequest(Guid PK)
        {
            StaffRenwalResidencyModel dbModel = new StaffRenwalResidencyModel();

            var VehicleMaintenceRequest = DbAHD.dataVehicleMaintenanceRequest.Where(x => x.VehicleMaintenanceRequestGUID == PK).FirstOrDefault();
            var dbVehicleMaintenceRequest = DbAHD.Entry(VehicleMaintenceRequest).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbVehicleMaintenceRequest, dbModel);

            if (VehicleMaintenceRequest.dataVehicleMaintenanceRequestRowVersion.SequenceEqual(dbModel.dataStaffRenwalResidencyRowVersion))
            {
                return Json(DbAHD.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbAHD, dbModel, "LanguagesContainer"));
        }

        private bool ActiveVehicleMaintenceRequest(Object model)
        {
            dataVehicleMaintenanceRequest VehicleMaintenceRequest = Mapper.Map(model, new dataVehicleMaintenanceRequest());
            int ModelDescription = DbAHD.dataVehicleMaintenanceRequest
                                    .Where(x => x.RequestNumber == VehicleMaintenceRequest.RequestNumber &&
                                                x.VehicleMaintenanceRequestGUID == VehicleMaintenceRequest.VehicleMaintenanceRequestGUID &&
                                                x.Active).Count();
            if (ModelDescription > 0)
            {
                ModelState.AddModelError("Vehicle Request", "Vehicle Request is already exists");
            }
            return (ModelDescription > 0);
        }
        #region Reports
        public ActionResult VehicleMaintenanceRequestReport(Guid? VehicleMaintenanceRequestGUID)
        {

            ReportViewer reportViewer = new ReportViewer();
            LocalReport localReport = new LocalReport();
            //localReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"/Reports/VoucherReport.rdlc";
            //localReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Areas/AHD/Rdlc/VehicalForms\VehicleMaintenanceRequest.rdlc";
            localReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Areas/AHD/Rdlc/VehicalForms\VehicleMaintenanceRequest.rdlc";

            v_VehicleMaintenanceRequestTableAdapter result = new v_VehicleMaintenanceRequestTableAdapter();
            var results = result.GetData().Where(c => c.VehicleMaintenanceRequestGUID == VehicleMaintenanceRequestGUID).ToList();

            results = results.ToList();

            if (results == null)
            {
                return PartialView("_Empty");
            }

            DataTable dt = results.ToList().CopyToDataTable();



            localReport.DataSources.Add(new ReportDataSource("DataSet1", dt));


            string reportType = "PDF";
            string mimeType;
            string encoding;
            string fileNameExtension = "pdf";
            //The DeviceInfo settings should be changed based on the reportType 
            string deviceInfo = @"<DeviceInfo>              
                                         <OutputFormat>PDF</OutputFormat>              
                                         <PageWidth>21cm</PageWidth>              
                                         <PageHeight>29.7cm</PageHeight>          
                                         <MarginTop>0cm</MarginTop>          
                                         <MarginLeft>0cm</MarginLeft>            
                                         <MarginRight>0cm</MarginRight>       
                                         <MarginBottom>0cm</MarginBottom></DeviceInfo>";
            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;


            renderedBytes = localReport.Render(
                reportType, deviceInfo, out mimeType, out encoding, out fileNameExtension,
                out streams, out warnings);

            var doc = new Document();
            doc.SetPageSize(iTextSharp.text.PageSize.A4);

            var reader = new PdfReader(renderedBytes);
            using (FileStream fs =
                new FileStream(
                    Server.MapPath("~/Uploads/AHD/Temp/Report_" + Convert.ToString(Session[SessionKeys.UserGUID]) +
                                   ".pdf"), FileMode.Create))
            {
                PdfStamper stamper = new PdfStamper(reader, fs);

                stamper.JavaScript =
                    "var pp = getPrintParams();pp.interactive = pp.constants.interactionLevel.automatic;pp.printerName = getPrintParams().printerName;print(pp);\r";
                stamper.Close();
            }

            reader.Close();
            FileStream fss =
                new FileStream(
                    Server.MapPath("~/Uploads/AHD/Temp/Report_" + Convert.ToString(Session[SessionKeys.UserGUID]) +
                                   ".pdf"), FileMode.Open);
            byte[] bytes = new byte[fss.Length];
            fss.Read(bytes, 0, Convert.ToInt32(fss.Length));
            fss.Close();
            System.IO.File.Delete(
                Server.MapPath("~/Uploads/AHD/Temp/Report_" + Convert.ToString(Session[SessionKeys.UserGUID]) +
                               ".pdf"));

            //Here we returns the file result for view(PDF)

            return File(bytes, "application/pdf");

        }
        #endregion

    }
}