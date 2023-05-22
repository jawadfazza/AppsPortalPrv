using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.Services;
using AppsPortal.ViewModels;
using AutoMapper;
using DAS_DAL.Model;
using DAS_DAL.ViewModels;
using FineUploader;
using iTextSharp.text;
using iTextSharp.text.pdf;
using LinqKit;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using RES_Repo.Globalization;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Image = iTextSharp.text.Image;

namespace AppsPortal.Areas.DAS.Controllers
{
    public class ScanDocumentController : DASBaseController
    {
        private Images imagesServices = new Images();

        private ConvertToPDF MyDocServices = new ConvertToPDF();

        #region My Files
        [Route("DAS/MyFilesIndex/")]
        public ActionResult MyFilesIndex()
        {
            if (!CMS.HasAction(Permissions.RefugeeScannedDocument.Access, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/DAS/Views/MyFiles/Index.cshtml");
        }

        [Route("DAS/DASUserFilesDataTable/")]
        public JsonResult DASUserFilesDataTable(DataTableRecievedOptions options)
        {

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<UserFileDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {

                Predicate = SearchHelper.CreateSearchPredicate<UserFileDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.RefugeeScannedDocument.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();
            //Fix
            //Guid filingUnitGUID = Guid.Parse("bb915454-05fb-4e4d-b033-e01c7c790997");
            Guid QamshilyGUID = Guid.Parse("B1E5916E-59A6-40A5-B5B0-DAE4126CC8C9");
            var filingUnitUserGUID = DbDAS.codeDASDestinationUnitFocalPoint.Where(f => f.UserGUID == UserGUID
            //&& f.DestinationUnitGUID == filingUnitGUID
            )
                .Select(f => f.UserGUID).FirstOrDefault();

            var UserUnit = DbDAS.codeDASDestinationUnitFocalPoint.Where(f => f.UserGUID == UserGUID);
            var UserUnitGUID = UserUnit.Select(f => f.DestinationUnitGUID).FirstOrDefault();
            //var filingUnitUserGUID = UserUnit.Where(f =>  f.DestinationUnitGUID == filingUnitGUID)
            //  .Select(f => f.UserGUID).FirstOrDefault();           
            var All = (

                from a in DbDAS.dataFile.Where(x => x.Active && x.LastCustodianTypeNameGUID == UserGUID).AsQueryable().AsExpandable()

                select new UserFileDataTableModel
                {
                    FileGUID = a.FileGUID.ToString(),
                    Active = a.Active,

                    FileNumber = a.FileNumber,
                    LastTransferFromNameGUID = a.LastTransferFromNameGUID.ToString(),
                    LastTransferFromName = a.LastTransferFromName,
                    LastCustodianTypeGUID = a.LastCustodianTypeGUID.ToString(),
                    ProcessStatusName = a.ProcessStatusName,
                    CaseLocation = a.LastTransferLocationName,
                    TransferLocationGUID = a.LastTransferLocationGUID.ToString(),
                    LastDestinationUnitGUID = a.LastDestinationUnitGUID.ToString(),
                    SiteOwner = a.SiteOwnerName,
                    LastUnitName = a.LastUnitName,
                    CaseSize = a.CaseSize,
                    RefugeeStatus = a.RefugeeStatus,
                    OrigionCountry = a.OrigionCountry,
                    FileMergeStatusGUID = a.FileMergeStatusGUID.ToString(),
                    FileMergeStatus = a.FileMergeStatus,
                    //LastCustodianType = a.LastCustodianType,
                    CurrentOwner = a.LastCustodianTypeName,
                    LastCustodianTypeNameGUID = a.LastCustodianTypeNameGUID.ToString(),
                    //UserTransferStatus = a.LastCustodianTypeGUID == DASDocumentCustodianType.UNIT? a.LastFlowStatusGUID == ScanDocumentTransferFlowStatus.Pending ?a.LastCustodianTypeGUID== FileLocationMovement ?"fun":"as": "Other":"Others",
                    UserTransferStatus = (a.LastFlowStatusGUID == null && UserGUID == filingUnitUserGUID) ||
                    ((a.LastFlowStatusGUID == ScanDocumentTransferFlowStatus.Confirmed && a.LastCustodianTypeNameGUID == UserGUID) || (a.LastDestinationUnitGUID == UserUnit.FirstOrDefault().DestinationUnitGUID && UserUnit.FirstOrDefault().IsSupervisor == true))
                    ? "ToTransfer" : ((a.LastFlowStatusGUID == ScanDocumentTransferFlowStatus.Pending && (a.LastCustodianTypeNameGUID == UserGUID || (a.LastCustodianTypeGUID == DASDocumentCustodianType.UNIT && a.LastCustodianTypeNameGUID == UserUnitGUID))) ? "ToConfirm" : "ToRequest"),
                    LastFlowStatusName = a.LastFlowStatusName,
                    //LastCustodianTypeGUID = a.LastCustodianTypeGUID.ToString(),
                    LatestAppointmentDate = a.LatestAppointmentDate,
                    TotalDaysReminigForAppointment = a.TotalDaysReminigForAppointment,
                    IsRequested = a.IsRequested,
                    LastRequesterName = a.LastRequesterName,
                    LastRequesterNameGUID = a.LastRequesterNameGUID.ToString(),
                    RequestStatusName = a.RequestStatusName,

                    LastFlowStatusGUID = a.LastFlowStatusGUID.ToString(),
                    dataFileRowVersion = a.dataFileRowVersion,

                }).Where(Predicate);

            All = All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<UserFileDataTableModel> Result = Mapper.Map<List<UserFileDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());


            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }
        #endregion
        // GET: DAS/ScannDocument
        #region Scann Document

        [Route("DAS/RefugeeScannedDocument/")]
        public ActionResult RefugeeScannedDocumentIndex()
        {
            if (!CMS.HasAction(Permissions.RefugeeScannedDocument.Access, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            Guid filingUnitGUID = Guid.Parse("bb915454-05fb-4e4d-b033-e01c7c790997");
            var unit = DbDAS.codeDASDestinationUnitFocalPoint.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
            if (unit.DestinationUnitGUID == filingUnitGUID)
            {
                ViewBag.IsFiling = 1;
            }
            else
                ViewBag.IsFiling = 0;
            return View("~/Areas/DAS/Views/RefugeeScannedDocument/Index.cshtml");
        }

        public ActionResult CasesAppointments()
        {
            if (!CMS.HasAction(Permissions.RefugeeScannedDocument.Create, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/DAS/Views/CaseAppointment/Index.cshtml");
        }

        [Route("DAS/RefugeeCasesHaveScheduledAppointmentDataTable/")]
        public JsonResult RefugeeCasesHaveScheduledAppointmentDataTable(DataTableRecievedOptions options)
        {


            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<RefugeeCasesHaveScheduledAppointmentDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<RefugeeCasesHaveScheduledAppointmentDataTableModel>(DataTable.Filters);
            }

            var All = (

                from a in DbDAS.dataFile.Where(x => x.Active).AsExpandable()
                join b in DbDAS.dataSchedulerCasesHaveAppointment.Where(x => (x.Active == true)
                && x.AppointmentDate != null) on a.FileNumber equals b.FileNumber into LJ1
                from R1 in LJ1.DefaultIfEmpty()
                select new RefugeeCasesHaveScheduledAppointmentDataTableModel
                {
                    FileGUID = a.FileGUID.ToString(),
                    AppointmentDate = R1.AppointmentDate,
                    AppointmentRecordedDate = R1.RecordedDate,
                    Comments = R1.Comments,
                    Active = a.Active,
                    FileNumber = a.FileNumber,
                    LastCustodianType = a.LastCustodianType,
                    LastCustodianTypeName = a.LastCustodianTypeName,
                    LastFlowStatusName = a.LastFlowStatusName,
                    LastCustodianTypeGUID = a.LastCustodianTypeGUID.ToString(),
                    LastCustodianTypeNameGUID = a.LastCustodianTypeNameGUID.ToString(),
                    LastFlowStatusGUID = a.LastFlowStatusGUID.ToString(),
                    dataFileRowVersion = a.dataFileRowVersion,

                }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<RefugeeCasesHaveScheduledAppointmentDataTableModel> Result = Mapper.Map<List<RefugeeCasesHaveScheduledAppointmentDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result.OrderBy(f => f.FileNumber)), JsonRequestBehavior.AllowGet);
        }
        public ActionResult FilterBulkCases()
        {
            if (!CMS.HasAction(Permissions.RefugeeScannedDocument.Create, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }



            return PartialView("~/Areas/DAS/Views/RefugeeScannedDocument/_FilterBulkCases.cshtml",
                new RefugeeCasesDataTableFilter());
        }

        [Route("DAS/RefugeeBulkCasesFiltersBulkCaseSearch/")]
        public JsonResult RefugeeBulkCasesFiltersBulkCaseSearch(RefugeeCasesDataTableFilter model)
        {
            DataTableOptions DataTable = new DataTableOptions();
            string[] splitInput = System.Text.RegularExpressions.Regex.Split(model.cases, "\r\n");
            List<string> filterdCases = new List<string>();
            for (int i = 0; i < splitInput.Length; i++)
            {
                if (splitInput[i] == null)
                    continue;

                filterdCases.Add(splitInput[i]);


            }


            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.RefugeeScannedDocument.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();
            //Fix
            //Guid filingUnitGUID = Guid.Parse("bb915454-05fb-4e4d-b033-e01c7c790997");
            var filingUnitUserGUID = DbDAS.codeDASDestinationUnitFocalPoint.Where(f => f.UserGUID == UserGUID
            //&& f.DestinationUnitGUID == filingUnitGUID
            )
                .Select(f => f.UserGUID).FirstOrDefault();

            var UserUnit = DbDAS.codeDASDestinationUnitFocalPoint.Where(f => f.UserGUID == UserGUID);
            var UserUnitGUID = UserUnit.Select(f => f.DestinationUnitGUID).FirstOrDefault();
            //var filingUnitUserGUID = UserUnit.Where(f =>  f.DestinationUnitGUID == filingUnitGUID)
            //  .Select(f => f.UserGUID).FirstOrDefault();

            var All = (

                from a in DbDAS.dataFile.Where(x => x.Active && filterdCases.Contains(x.FileNumber)).AsQueryable().AsExpandable()

                select new FileDataTableModel
                {
                    FileGUID = a.FileGUID.ToString(),

                    Active = a.Active,
                    FileNumber = a.FileNumber,
                    LastTransferFromNameGUID = a.LastTransferFromNameGUID.ToString(),
                    LastTransferFromName = a.LastTransferFromName,
                    LastCustodianTypeGUID = a.LastCustodianTypeGUID.ToString(),
                    ProcessStatusName = a.ProcessStatusName,
                    CaseSize = a.CaseSize,
                    RefugeeStatus = a.RefugeeStatus,
                    OrigionCountry = a.OrigionCountry,
                    CaseLocation = a.LastTransferLocationName,
                    //LastCustodianType = a.LastCustodianType,
                    CurrentOwner = a.LastCustodianTypeName,
                    LastCustodianTypeNameGUID = a.LastCustodianTypeNameGUID.ToString(),
                    //UserTransferStatus = a.LastCustodianTypeGUID == DASDocumentCustodianType.UNIT? a.LastFlowStatusGUID == ScanDocumentTransferFlowStatus.Pending ?a.LastCustodianTypeGUID== FileLocationMovement ?"fun":"as": "Other":"Others",
                    UserTransferStatus = (a.LastFlowStatusGUID == null && UserGUID == filingUnitUserGUID) ||
                    (a.LastFlowStatusGUID == ScanDocumentTransferFlowStatus.Confirmed && a.LastCustodianTypeNameGUID == UserGUID)
                    ? "ToTransfer" : ((a.LastFlowStatusGUID == ScanDocumentTransferFlowStatus.Pending && (a.LastCustodianTypeNameGUID == UserGUID || (a.LastCustodianTypeGUID == DASDocumentCustodianType.UNIT && a.LastCustodianTypeNameGUID == UserUnitGUID))) ? "ToConfirm" : ""),
                    //UserTransferStatus = a.LastFlowStatusGUID == null && UserGUID == filingUnitUserGUID ? "ToTransfer" : (
                    //               a.LastCustodianTypeGUID == DASDocumentCustodianType.UNIT
                    //                   ? (a.LastFlowStatusGUID == ScanDocumentTransferFlowStatus.Pending ?
                    //                             (FileLocationMovement == a.LastCustodianTypeNameGUID ? "ToConfirm" : "")
                    //                             : a.LastFlowStatusGUID == ScanDocumentTransferFlowStatus.Confirmed || a.LastFlowStatusGUID == ScanDocumentTransferFlowStatus.Returned ? (
                    //                                 FileLocationMovement == a.LastCustodianTypeNameGUID != null ? "ToTransfer" : "") : "")
                    //                   : (a.LastCustodianTypeGUID == DASDocumentCustodianType.Staff ? (a.LastFlowStatusGUID == ScanDocumentTransferFlowStatus.Pending && a.LastCustodianTypeNameGUID == UserGUID ? "ToConfirm" : ((a.LastFlowStatusGUID == ScanDocumentTransferFlowStatus.Confirmed || a.LastFlowStatusGUID == ScanDocumentTransferFlowStatus.Returned) && a.LastCustodianTypeNameGUID == UserGUID ? "ToTransfer" : "")) : (""))),






                    LastFlowStatusName = a.LastFlowStatusName,
                    //LastCustodianTypeGUID = a.LastCustodianTypeGUID.ToString(),
                    LatestAppointmentDate = a.LatestAppointmentDate,
                    TotalDaysReminigForAppointment = a.TotalDaysReminigForAppointment,
                    IsRequested = a.IsRequested,
                    LastRequesterName = a.LastRequesterName,
                    LastRequesterNameGUID = a.LastRequesterNameGUID.ToString(),
                    RequestStatusName = a.RequestStatusName,

                    LastFlowStatusGUID = a.LastFlowStatusGUID.ToString(),
                    dataFileRowVersion = a.dataFileRowVersion,

                });

            // All = All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<FileDataTableModel> Result = Mapper.Map<List<FileDataTableModel>>(All.OrderBy(f => f.FileNumber).Skip(1).Take(1000).ToList());


            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("DAS/RefugeeScannedDocumentDataTable/")]
        public JsonResult RefugeeScannedDocumentDataTable(DataTableRecievedOptions options)
        {

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<FileDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {

                Predicate = SearchHelper.CreateSearchPredicate<FileDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.RefugeeScannedDocument.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();
            //Fix
            Guid filingUnitGUID = Guid.Parse("bb915454-05fb-4e4d-b033-e01c7c790997");
            var filingUnitUserGUID = DbDAS.codeDASDestinationUnitFocalPoint.Where(f => f.UserGUID == UserGUID
            && f.DestinationUnitGUID == filingUnitGUID)
                .Select(f => f.UserGUID).FirstOrDefault();

            var UserUnit = DbDAS.codeDASDestinationUnitFocalPoint.Where(f => f.UserGUID == UserGUID);
            var UserUnitGUID = UserUnit.Select(f => f.DestinationUnitGUID).FirstOrDefault();
            //var filingUnitUserGUID = UserUnit.Where(f =>  f.DestinationUnitGUID == filingUnitGUID)
            //  .Select(f => f.UserGUID).FirstOrDefault();           
            var All = (

                from a in DbDAS.dataFile.Where(x => x.Active).AsQueryable().AsExpandable()
                select new FileDataTableModel
                {
                    FileGUID = a.FileGUID.ToString(),
                    OwnedByStaff = a.LastCustodianTypeNameGUID == UserGUID ? true : false,
                    Active = a.Active,
                    SiteCode = a.SiteCode,
                    SiteGUID = a.SiteGUID.ToString(),
                    FileNumber = a.FileNumber,
                    LastTransferFromNameGUID = a.LastTransferFromNameGUID.ToString(),
                    LastTransferFromName = a.LastTransferFromName,
                    LastCustodianTypeGUID = a.LastCustodianTypeGUID.ToString(),
                    ProcessStatusName = a.ProcessStatusName,
                    CaseLocation = a.LastTransferLocationName,
                    TransferLocationGUID = a.LastTransferLocationGUID.ToString(),
                    LastDestinationUnitGUID = a.LastDestinationUnitGUID.ToString(),
                    LastUnitName = a.LastUnitName,
                    FileMergeStatusGUID = a.FileMergeStatusGUID.ToString(),
                    FileMergeStatus = a.FileMergeStatus,
                    CaseSize = a.CaseSize,
                    SiteOwner = a.SiteOwnerName,
                    RefugeeStatus = a.RefugeeStatus,
                    OrigionCountry = a.OrigionCountry,
                    //LastCustodianType = a.LastCustodianType,
                    CurrentOwner = a.LastCustodianTypeName == null ? "Filing Unit" : a.LastCustodianTypeName,
                    LastCustodianTypeNameGUID = a.LastCustodianTypeNameGUID.ToString(),
                    //UserTransferStatus = a.LastCustodianTypeGUID == DASDocumentCustodianType.UNIT? a.LastFlowStatusGUID == ScanDocumentTransferFlowStatus.Pending ?a.LastCustodianTypeGUID== FileLocationMovement ?"fun":"as": "Other":"Others",
                    UserTransferStatus = (a.LastFlowStatusGUID == null && UserGUID == filingUnitUserGUID) ||
                    (a.LastFlowStatusGUID == ScanDocumentTransferFlowStatus.Confirmed && a.LastCustodianTypeNameGUID == UserGUID)
                    ? "ToTransfer" : ((a.LastFlowStatusGUID == ScanDocumentTransferFlowStatus.Pending && (a.LastCustodianTypeNameGUID == UserGUID || (a.LastCustodianTypeGUID == DASDocumentCustodianType.UNIT && a.LastCustodianTypeNameGUID == UserUnitGUID))) ? "ToConfirm" : "ToRequest"),
                    //UserTransferStatus = a.LastFlowStatusGUID == null && UserGUID == filingUnitUserGUID ? "ToTransfer" : (
                    //               a.LastCustodianTypeGUID == DASDocumentCustodianType.UNIT
                    //                   ? (a.LastFlowStatusGUID == ScanDocumentTransferFlowStatus.Pending ?
                    //                             (FileLocationMovement == a.LastCustodianTypeNameGUID ? "ToConfirm" : "")
                    //                             : a.LastFlowStatusGUID == ScanDocumentTransferFlowStatus.Confirmed || a.LastFlowStatusGUID == ScanDocumentTransferFlowStatus.Returned ? (
                    //                                 FileLocationMovement == a.LastCustodianTypeNameGUID != null ? "ToTransfer" : "") : "")
                    //                   : (a.LastCustodianTypeGUID == DASDocumentCustodianType.Staff ? (a.LastFlowStatusGUID == ScanDocumentTransferFlowStatus.Pending && a.LastCustodianTypeNameGUID == UserGUID ? "ToConfirm" : ((a.LastFlowStatusGUID == ScanDocumentTransferFlowStatus.Confirmed || a.LastFlowStatusGUID == ScanDocumentTransferFlowStatus.Returned) && a.LastCustodianTypeNameGUID == UserGUID ? "ToTransfer" : "")) : (""))),






                    LastFlowStatusName = a.LastFlowStatusName,
                    //LastCustodianTypeGUID = a.LastCustodianTypeGUID.ToString(),
                    LatestAppointmentDate = a.LatestAppointmentDate,
                    TotalDaysReminigForAppointment = a.TotalDaysReminigForAppointment,
                    IsRequested = a.IsRequested,
                    LastRequesterName = a.LastRequesterName,
                    LastRequesterNameGUID = a.LastRequesterNameGUID.ToString(),
                    RequestDate = a.dataFileRequest.FirstOrDefault(x => x.IsLastRequest == true).RequestDate,
                    RequestStatusName = a.RequestStatusName,

                    LastFlowStatusGUID = a.LastFlowStatusGUID.ToString(),
                    dataFileRowVersion = a.dataFileRowVersion,

                }).Where(Predicate);

            All = All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<FileDataTableModel> Result = Mapper.Map<List<FileDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());


            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("DAS/RefugeeScannedDocumentCreate/")]
        public ActionResult RefugeeScannedDocumentCreate(string id)
        {
            if (!CMS.HasAction(Permissions.RefugeeScannedDocument.Create, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var result = DbDAS.dataFile.Where(x => x.FileNumber == id).FirstOrDefault();
            return View("~/Areas/DAS/Views/RefugeeScannedDocument/RefugeeScannedDocument.cshtml", new ScannNewDocumentModel { FileNumber = id, FileGUID = result.FileGUID });
        }

        [Route("DAS/RefugeeScannedDocument/Update/{PK}")]
        public ActionResult RefugeeScannedDocumentUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.RefugeeScannedDocument.Access, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var model = (from a in DbDAS.dataFile.WherePK(PK)
                         join b in DbDAS.dataScannDocument.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.dataFile.DeletedOn)) on a.FileGUID equals b.FileGUID into LJ1
                         from R1 in LJ1.DefaultIfEmpty()
                         select new ScannNewDocumentModel
                         {
                             FileGUID = a.FileGUID,
                             FileNumber = a.FileNumber,
                             ScannDocumentGUID = R1.ScannDocumentGUID,
                             OwnedByStaff = a.LastCustodianTypeNameGUID == UserGUID ? true : false,
                             Active = a.Active,
                             dataFileRowVersion = a.dataFileRowVersion,
                             dataScannDocumentRowVersion = R1.dataScannDocumentRowVersion
                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("RefugeeScannedDocument", "ItemModels", new { Area = "DAS" }));

            return View("~/Areas/DAS/Views/RefugeeScannedDocument/RefugeeScannedDocument.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult RefugeeScannedDocumentCreate(RefugeeScannDocumentDataTableModel model)
        {
            if (!CMS.HasAction(Permissions.RefugeeScannedDocument.Create, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveRefugeeScannedDocument(model)) return PartialView("~/Areas/DAS/Views/RefugeeScannedDocument/_RefugeeScannedDocumentForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            dataFile RefugeeScannedDocument = Mapper.Map(model, new dataFile());
            //RefugeeScannedDocument.WarehouseRefugeeScannedDocumentGUID = EntityPK;
            //RefugeeScannedDocument.WarehouseTypeGUID = Guid.Parse("9317FBDA-E360-45CC-A064-11E6A21C1E17");
            DbDAS.Create(RefugeeScannedDocument, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);




            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.ItemModelLanguagesDataTable, ControllerContext, "ItemModelLanguagesFormControls"));
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.ItemModelDeterminantsDataTable, ControllerContext, "ModelsDeterminantsFormControls"));
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.ItemModelWarehouseDataTable, ControllerContext, "ModelsWarehousesFormControls"));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.RefugeeScannedDocument.Create, Apps.DAS, new UrlHelper(Request.RequestContext).Action("Create", "ItemModels", new { Area = "DAS" })), Container = "ItemModelDetailFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.RefugeeScannedDocument.Update, Apps.DAS), Container = "ItemModelDetailFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.RefugeeScannedDocument.Delete, Apps.DAS), Container = "ItemModelDetailFormControls" });

            try
            {
                DbDAS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbDAS.SingleCreateMessage(DbDAS.PrimaryKeyControl(RefugeeScannedDocument), DbDAS.RowVersionControls(RefugeeScannedDocument, RefugeeScannedDocument), Partials, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult RefugeeScannedDocumentUpdate(RefugeeScannDocumentDataTableModel model)
        {
            if (!CMS.HasAction(Permissions.RefugeeScannedDocument.Update, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveRefugeeScannedDocument(model)) return PartialView("~/Areas/DAS/Views/RefugeeScannedDocument/_RefugeeScannedDocumentForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            dataFile ItemModel = Mapper.Map(model, new dataFile());
            DbDAS.Update(ItemModel, Permissions.RefugeeScannedDocument.UpdateGuid, ExecutionTime, DbCMS);

            /// var Language = DbDAS.dataFileLanguage.Where(l => l.WarehouseRefugeeScannedDocumentGUID == model.WarehouseRefugeeScannedDocumentGUID && l.LanguageID == LAN && l.Active).FirstOrDefault();

            //if (Language == null)
            //{
            //    Language = Mapper.Map(model, Language);
            //    Language.WarehouseRefugeeScannedDocumentGUID = ItemModel.WarehouseRefugeeScannedDocumentGUID;
            //    DbDAS.Create(Language, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);
            //}
            //else if (Language.WarehouseRefugeeScannedDocumentDescription != model.WarehouseRefugeeScannedDocumentDescription)
            //{
            //    Language = Mapper.Map(model, Language);
            //    DbDAS.Update(Language, Permissions.RefugeeScannedDocument.UpdateGuid, ExecutionTime, DbCMS);
            //}

            try
            {
                DbDAS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbDAS.SingleUpdateMessage(null, null, DbDAS.RowVersionControls(ItemModel, ItemModel)));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyRefugeeScannedDocument(Guid.Parse(model.ScannDocumentGUID));
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult RefugeeScannedDocumentDelete(dataFile model)
        {
            if (!CMS.HasAction(Permissions.RefugeeScannedDocument.Delete, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataFile> DeletedItemModel = DeleteRefugeeScannedDocument(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.RefugeeScannedDocument.Restore, Apps.DAS), Container = "RefugeeScannedDocumentFormControls" });

            try
            {
                int CommitedRows = DbDAS.SaveChanges();
                DbDAS.SaveChanges();
                return Json(DbDAS.SingleDeleteMessage(CommitedRows, DeletedItemModel.FirstOrDefault(), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyRefugeeScannedDocument(model.FileGUID);
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult RefugeeScannedDocumentRestore(dataFile model)
        {
            if (!CMS.HasAction(Permissions.RefugeeScannedDocument.Restore, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveRefugeeScannedDocument(model))
            {
                return Json(DbDAS.RecordExists());
            }

            List<dataFile> RestoredItemModel = RestoreRefugeeScannedDocument(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.RefugeeScannedDocument.Create, Apps.DAS, new UrlHelper(Request.RequestContext).Action("ItemModelCreate", "Configuration", new { Area = "DAS" })), Container = "ItemModelFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.RefugeeScannedDocument.Update, Apps.DAS), Container = "ItemModelFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.RefugeeScannedDocument.Delete, Apps.DAS), Container = "ItemModelFormControls" });

            try
            {
                int CommitedRows = DbDAS.SaveChanges();
                DbDAS.SaveChanges();
                return Json(DbDAS.SingleRestoreMessage(CommitedRows, RestoredItemModel, DbDAS.PrimaryKeyControl(RestoredItemModel.FirstOrDefault()), Url.Action(DataTableNames.RefugeeScannedDocumentDataTable, Portal.GetControllerName(ControllerContext)), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyRefugeeScannedDocument(model.FileGUID);
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult RefugeeScannedDocumentDataTableDelete(List<dataFile> models)
        {
            if (!CMS.HasAction(Permissions.RefugeeScannedDocument.Delete, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataFile> DeletedItemClassificaiton = DeleteRefugeeScannedDocument(models);

            try
            {
                DbDAS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbDAS.PartialDeleteMessage(DeletedItemClassificaiton, models, DataTableNames.RefugeeScannedDocumentDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult RefugeeScannedDocumentDataTableRestore(List<dataFile> models)
        {
            if (!CMS.HasAction(Permissions.RefugeeScannedDocument.Restore, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataFile> RestoredItemModel = DeleteRefugeeScannedDocument(models);

            try
            {
                DbDAS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbDAS.PartialRestoreMessage(RestoredItemModel, models, DataTableNames.RefugeeScannedDocumentDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }

        private List<dataFile> DeleteRefugeeScannedDocument(List<dataFile> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<dataFile> DeletedItemModel = new List<dataFile>();
            //Fix
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = "SELECT WarehouseItemModelGUID,CONVERT(varchar(50), WarehouseItemModelGUID) as C2 ,codeWarehouseItemModelRowVersion FROM code.codeWarehouseItemModel where WarehouseItemModelGUID in (" + string.Join(",", models.Select(x => "'" + x.WarehouseItemModelGUID + "'").ToArray()) + ")";

            string query = DbDAS.QueryBuilder(models, Permissions.RefugeeScannedDocument.DeleteGuid, SubmitTypes.Delete, "");

            var Records = DbDAS.Database.SqlQuery<dataFile>(query).ToList();
            foreach (var record in Records)
            {
                DeletedItemModel.Add(DbDAS.Delete(record, ExecutionTime, Permissions.RefugeeScannedDocument.DeleteGuid, DbCMS));
            }

            //var Languages = DeletedItemModel.SelectMany(a => a.dataFileLanguage).Where(l => l.Active).ToList();
            //foreach (var language in Languages)
            //{
            //    DbDAS.Delete(language, ExecutionTime, Permissions.RefugeeScannedDocument.DeleteGuid, DbCMS);
            //}
            return DeletedItemModel;
        }

        private List<dataFile> RestoreRefugeeScannedDocument(List<dataFile> models)
        {

            DateTime RestoringTime = DateTime.Now;

            List<dataFile> RestoredRefugeeScannedDocument = new List<dataFile>();
            //Fix
            //Select the table and all the factors from other tables into one query.
            //string baseQuery = "SELECT WarehouseItemModelGUID,CONVERT(varchar(50), WarehouseItemModelGUID) as C2 ,codeWarehouseItemModelRowVersion FROM code.codeWarehouseItemModel where WarehouseItemModelGUID in (" + string.Join(",", models.Select(x => "'" + x.WarehouseItemModelGUID + "'").ToArray()) + ")";

            string query = DbDAS.QueryBuilder(models, Permissions.RefugeeScannedDocument.DeleteGuid, SubmitTypes.Restore, "");

            var Records = DbDAS.Database.SqlQuery<dataFile>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveRefugeeScannedDocument(record))
                {
                    RestoredRefugeeScannedDocument.Add(DbDAS.Restore(record, Permissions.RefugeeScannedDocument.DeleteGuid, Permissions.RefugeeScannedDocument.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            //var Languages = RestoredRefugeeScannedDocument.SelectMany(x => x.dataFileLanguage.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            //foreach (var language in Languages)
            //{
            //    DbDAS.Restore(language, Permissions.RefugeeScannedDocument.DeleteGuid, Permissions.RefugeeScannedDocument.RestoreGuid, RestoringTime, DbCMS);
            //}

            return RestoredRefugeeScannedDocument;
        }

        private JsonResult ConcurrencyRefugeeScannedDocument(Guid PK)
        {
            RefugeeScannDocumentDataTableModel dbModel = new RefugeeScannDocumentDataTableModel();

            var ItemModel = DbDAS.dataFile.Where(x => x.FileGUID == PK).FirstOrDefault();
            var dbItemModel = DbDAS.Entry(ItemModel).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbItemModel, dbModel);

            // var Language = DbDAS.dataFileLanguage.Where(x => x.WarehouseRefugeeScannedDocumentGUID == PK).Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.dataFile.DeletedOn) && x.LanguageID == LAN).FirstOrDefault();
            //var dbLanguage = DbDAS.Entry(Language).GetDatabaseValues().ToObject();
            //dbModel = Mapper.Map(dbLanguage, dbModel);

            //if (ItemModel.dataFileRowVersion.SequenceEqual(dbModel.dataFileRowVersion) && Language.dataFileLanguageRowVersion.SequenceEqual(dbModel.dataFileLanguageRowVersion))
            //{
            //    return Json(DbDAS.PermissionError());
            //}

            return Json(JsonMessages.ConcurrencyError(DbDAS, dbModel, "LanguagesContainer"));
        }

        private bool ActiveRefugeeScannedDocument(Object model)
        {
            dataFile ItemModel = Mapper.Map(model, new dataFile());
            int ModelDescription = DbDAS.dataFile
                                    .Where(x => x.FileGUID == ItemModel.FileGUID
                                          && x.FileNumber != "" &&
                                                x.Active).Count();
            if (ModelDescription > 0)
            {
                ModelState.AddModelError("File", "File is already exists");
            }
            return (ModelDescription > 0);
        }

        #endregion
        #region Search By Case
        [Route("DAS/RefugeeScannedDocument/SearchFile/")]
        public ActionResult SearchFile()
        {
            // DbDAS.dataScannDocumentMetaData.Where(x => x.ScannDocumentGUID == FK).ToList();
            return PartialView("~/Areas/DAS/Views/RefugeeScannedDocument/_SearchFile.cshtml");

        }

        public ActionResult FileSearchCreate(string id)
        {
            string[] Files = null;
            var caseProGres = DbPRG.dataProcessGroup.Where(x => x.ProcessingGroupNumber == id).FirstOrDefault();
            if (caseProGres == null)
            {
                return Json(new { success = 0 }, JsonRequestBehavior.AllowGet);
            }
            var myFile = DbDAS.dataFile.Where(x => x.FileNumber == id).FirstOrDefault();
            if (myFile == null)
            {
                DateTime ExecutionTime = DateTime.Now;
                dataFile newFile = new dataFile
                {
                    FileGUID = caseProGres.ProcessingGroupGUID,
                    FileNumber = caseProGres.ProcessingGroupNumber,
                    ProcessStatusName = caseProGres.ProcessingGroupStatusCode,

                };
                DbDAS.Create(newFile, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);
                DbDAS.SaveChanges();
                DbCMS.SaveChanges();

            }
            CaseInformatioModel result = DbDAS.dataFile.Where(x => x.FileNumber == id).Select(x => new CaseInformatioModel
            {
                FileNumber = x.FileNumber,

                FileGUID = x.FileGUID,

            }).FirstOrDefault();
            if (result == null)
            {
                return Json(new { success = 0 }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);
            //return View("~/Areas/DAS/Views/ScanDocument/FileSearchInformation.cshtml", result.FileNumber);
            return RedirectToAction("FileSearchInformation", new { CaseInformatioModel = result });
            //return Json(new { success = 0 }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult FileSearchInformation(string id)
        {
            return View("~/Areas/DAS/Views/RefugeeScannedDocument/RefugeeScannedDocument.cshtml");
        }
        public JsonResult SearchByCase(string searchKey)
        {
            string[] Files = null;
            var caseProGres = DbPRG.dataProcessGroup.Where(x => x.ProcessingGroupNumber == searchKey).FirstOrDefault();
            if (caseProGres == null)
            {
                return Json(new { success = 0 }, JsonRequestBehavior.AllowGet);
            }
            var myFile = DbDAS.dataFile.Where(x => x.FileNumber == searchKey).FirstOrDefault();
            if (myFile == null)
            {
                DateTime ExecutionTime = DateTime.Now;
                dataFile newFile = new dataFile
                {
                    FileGUID = caseProGres.ProcessingGroupGUID,
                    FileNumber = caseProGres.ProcessingGroupNumber,
                    ProcessStatusName = caseProGres.ProcessingGroupStatusCode,

                };
                DbDAS.Create(newFile, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);
                DbDAS.SaveChanges();
                DbCMS.SaveChanges();

            }
            var result = DbDAS.dataFile.Where(x => x.FileNumber == searchKey).Select(x => new CaseInformatioModel
            {
                FileNumber = x.FileNumber,

                FileGUID = x.FileGUID,

            }).FirstOrDefault();
            if (result == null)
            {
                return Json(new { success = 0 }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var myScann = DbDAS.dataScannDocument.Where(x => x.FileGUID == result.FileGUID).FirstOrDefault();

                if (myScann != null)
                {
                    var VersionNumber = DbDAS.dataScannDocumentVersionHistory.Where(x => x.ScannDocumentGUID == myScann.ScannDocumentGUID).OrderBy(x => x.VersionNumber).Select(x => x.VersionNumber).ToList();

                    var codeCabinets = DbDAS.codeDASDocumentCabinet.Select(x => new { Guid = x.DocumentCabinetGUID, Name = x.DocumentCabinetName }).ToList();
                    var codeMetaDatas = DbDAS.codeDASDocumentMetaData.Select(x => new { Guid = x.DocumentMetaDataGUID, Name = x.DocumentMetaDataName }).ToList();
                    var codeCabinetShelfs = DbDAS.codeDASDocumentCabinetShelf.Select(x => new { Guid = x.DocumentCabinetShelfGUID, Name = x.ShelfNumber }).ToList();
                    var ScannDocumentGUID = DbDAS.dataScannDocument.Select(x => new { Guid = x.ScannDocumentGUID, Name = x.dataFile.FileNumber }).ToList();
                    int isNew = 0;

                    if (myScann == null)
                    {
                        isNew = 1;
                    }
                    var currentUserScannerSetting = DbDAS.dataDefaultScannerSetting.
                            Select(x => new { x.ScanningType, x.PaperFormate, x.Resolution, x.PaperSize, x.ColorMode }).
                            FirstOrDefault();
                    return Json(new
                    {
                        success = 1,
                        codeCabinets = codeCabinets,
                        codeCabinetShelfs = codeCabinetShelfs,
                        result = result,
                        isNew = isNew,
                        Files = Files,
                        currentUserScannerSetting = currentUserScannerSetting,
                        VersionNumber = VersionNumber,
                        codeMetaDatas = codeMetaDatas,
                        ScannDocumentGUID = ScannDocumentGUID
                    }, JsonRequestBehavior.AllowGet);

                }
                else
                {


                    var codeCabinets = DbDAS.codeDASDocumentCabinet.Select(x => new { Guid = x.DocumentCabinetGUID, Name = x.DocumentCabinetName }).ToList();
                    var codeMetaDatas = DbDAS.codeDASDocumentMetaData.Select(x => new { Guid = x.DocumentMetaDataGUID, Name = x.DocumentMetaDataName }).ToList();
                    var codeCabinetShelfs = DbDAS.codeDASDocumentCabinetShelf.Select(x => new { Guid = x.DocumentCabinetShelfGUID, Name = x.ShelfNumber }).ToList();
                    int isNew = 0;

                    if (myScann == null)
                    {
                        isNew = 1;
                    }
                    var currentUserScannerSetting = DbDAS.dataDefaultScannerSetting.
                            Select(x => new { x.ScanningType, x.PaperFormate, x.Resolution, x.PaperSize, x.ColorMode }).
                            FirstOrDefault();
                    return Json(new
                    {
                        success = 1,
                        codeCabinets = codeCabinets,
                        codeCabinetShelfs = codeCabinetShelfs,
                        result = result,
                        isNew = isNew,
                        Files = Files,
                        currentUserScannerSetting = currentUserScannerSetting,

                        codeMetaDatas = codeMetaDatas,

                    }, JsonRequestBehavior.AllowGet);

                }
            }
        }

        #endregion

        #region Check Images list 

        public JsonResult SaveImagesSannned(string mysearchKey, int New)
        {
            ///for encryption image
            Guid userGuid = UserGUID;
            string path = Server.MapPath("~/Areas/DAS/Documents/" + mysearchKey);
            var mycase = DbDAS.dataFile.Where(x => x.FileNumber == mysearchKey).FirstOrDefault();
            dataScannDocument MyScanDocument = new dataScannDocument();
            int NumberPageMax = 0;
            List<dataScannDocumentImage> ToAddImages = new List<dataScannDocumentImage>();
            if (New == 1)
            {
                //delet folder if is exit 
                if (Directory.Exists(path))
                {
                    string[] files_Tem = Directory.GetFiles(path);
                    if (files_Tem.Count() > 0)
                    {
                        foreach (var file in files_Tem)
                        {
                            System.IO.File.Delete(file);
                        }
                    }
                }
                else
                {
                    Directory.CreateDirectory(path);
                }
                MyScanDocument = new dataScannDocument
                {
                    ScannDocumentGUID = Guid.NewGuid(),
                    FileGUID = mycase.FileGUID,
                    CreateByGUID = userGuid,
                    CreateDate = DateTime.Now,
                    Active = true,
                    DeletedOn = null,
                    DocumentCabinetShelfGUID = null
                };
                DbDAS.dataScannDocument.Add(MyScanDocument);
                DbDAS.SaveChanges();

            }
            else
            {
                MyScanDocument = DbDAS.dataScannDocument.Where(x => x.FileGUID == mycase.FileGUID).FirstOrDefault();
                var images = DbDAS.dataScannDocumentImage.Where(x => x.ScannDocumentGUID == MyScanDocument.ScannDocumentGUID).OrderBy(x => x.ImageNumber).ToList();
                NumberPageMax = (int)images[images.Count() - 1].ImageNumber + 1;
            }
            //     
            // key Encryption
            byte[] Key = Encoding.UTF8.GetBytes("asdf!@#$1234ASDF");
            HttpFileCollectionBase files = Request.Files;
            //function Encryptio
            imagesServices.EncryptionImage(files, path, Key);

            //save images in database
            for (var i = 0; i < files.Count; i++)
            {
                HttpPostedFileBase file = files[i];
                Guid myImageGuid = Guid.NewGuid();
                dataScannDocumentImage myCurrImage = new dataScannDocumentImage
                {
                    ScannDocumentImageGUID = myImageGuid,
                    ScannDocumentGUID = MyScanDocument.ScannDocumentGUID,
                    CreateByGUID = userGuid,
                    CreateDate = DateTime.Now,
                    ImageName = Path.GetFileName(file.FileName),
                    ImageNumber = i + NumberPageMax
                };
                ToAddImages.Add(myCurrImage);
            }

            DbDAS.dataScannDocumentImage.AddRange(ToAddImages);
            DbDAS.SaveChanges();

            return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);
        }
        //save final images


        public JsonResult SaveFinal(CaseInformatioModel search, Guid ShelfGUID, List<Guid> MetaDataDocumentsGUIDs)
        {
            /// define destination Path to save Images
            string DestFolder = Path.Combine(Server.MapPath("~/Areas/DAS/Documents/"), search.FileNumber);
            // define datascandocument and datascandocumentimages for old datascanddocument
            var MYOldDataScanDocument = DbDAS.dataScannDocument.Where(x => x.FileGUID == search.FileGUID).FirstOrDefault();
            dataScannDocument mydoc = DbDAS.dataScannDocument.Where(x => x.FileGUID == search.FileGUID).FirstOrDefault();
            if (ShelfGUID != null)
            {
                mydoc.DocumentCabinetShelfGUID = ShelfGUID;

            }
            var MyoldDataScanDocumentImages = DbDAS.dataScannDocumentImage.Where(x => x.ScannDocumentGUID == MYOldDataScanDocument.ScannDocumentGUID).OrderBy(x => x.ImageNumber).ToList();
            ///meta data documet 
            if (MetaDataDocumentsGUIDs != null)
            {
                List<dataScannDocumentMetaData> ToAddMetaData = new List<dataScannDocumentMetaData>();
                foreach (var item in MetaDataDocumentsGUIDs)
                {
                    dataScannDocumentMetaData MyCurrentMetaDataImage = new dataScannDocumentMetaData
                    {
                        ScannDocumentMetaDataGUID = Guid.NewGuid(),
                        ScannDocumentGUID = MYOldDataScanDocument.ScannDocumentGUID,
                        DocumentMetaDataGUID = item,
                        //CreateDate = DateTime.Now,
                        Active = true,
                        DeletedOn = null
                    };
                    ToAddMetaData.Add(MyCurrentMetaDataImage);
                }
                DbDAS.dataScannDocumentMetaData.AddRange(ToAddMetaData);
                DbDAS.SaveChanges();
            }
            // end meta data doumet
            Guid userGuid = UserGUID;
            int VersionNumberCase = 0;
            /// create version for updated datascandocument and add images to version number
            if (MYOldDataScanDocument != null)
            {
                string PathVersion = Server.MapPath("~/Areas/DAS/Documents/VersiondataScannDocument/") + search.FileNumber;
                string PathVersionNumber = "";
                if (!Directory.Exists(PathVersion))
                {
                    Directory.CreateDirectory(PathVersion);
                }

                /////correct ??~~~~~~~~~~~~~  chose document Guid not File GUID below 
                ///

                var VersionScandocument = DbDAS.dataScannDocumentVersionHistory.Where(x => x.ScannDocumentGUID == search.FileGUID
                         && x.IsCurrentVersion == true).ToList();
                if (VersionScandocument != null && VersionScandocument.Count > 0)
                {
                    int LastVersionNumber = (int)VersionScandocument[VersionScandocument.Count - 1].VersionNumber;
                    foreach (var item in VersionScandocument)
                    {
                        item.IsCurrentVersion = false;
                    }

                    // create version history for old datascandocument
                    dataScannDocumentVersionHistory myDocVersion = new dataScannDocumentVersionHistory
                    {
                        ScannDocumentVersionHistoryGUID = Guid.NewGuid(),
                        ScannDocumentGUID = mydoc.ScannDocumentGUID,
                        CreateByGUID = userGuid,
                        VersionNumber = LastVersionNumber + 1,
                        IsCurrentVersion = true,
                        CreateDate = DateTime.Now,
                        Active = true,
                        DeletedOn = null
                    };
                    DbDAS.dataScannDocumentVersionHistory.Add(myDocVersion);
                    DbDAS.SaveChanges();

                    PathVersionNumber = Server.MapPath("~/Areas/DAS/Documents/VersiondataScannDocument/" + search.FileNumber + "/") + myDocVersion.VersionNumber;
                    VersionNumberCase = (int)myDocVersion.VersionNumber;
                    Directory.CreateDirectory(PathVersionNumber);
                }
                else
                {
                    PathVersionNumber = Server.MapPath("~/Areas/DAS/Documents/VersionDataScannDocuments/" + search.FileNumber + "/") + 1;
                    VersionNumberCase = 1;
                    Directory.CreateDirectory(PathVersionNumber);
                    dataScannDocumentVersionHistory myDocVersion = new dataScannDocumentVersionHistory
                    {
                        ScannDocumentVersionHistoryGUID = Guid.NewGuid(),
                        ScannDocumentGUID = mydoc.ScannDocumentGUID,
                        CreateByGUID = userGuid,
                        VersionNumber = 1,
                        IsCurrentVersion = true,
                        CreateDate = DateTime.Now,
                        Active = true,
                        DeletedOn = null
                    };
                    DbDAS.dataScannDocumentVersionHistory.Add(myDocVersion);
                    DbDAS.SaveChanges();

                }
                //add images to Version history
                if (Directory.Exists(PathVersionNumber))
                {
                    string[] files_Tem = Directory.GetFiles(PathVersionNumber);
                    if (files_Tem.Count() > 0)
                    {
                        foreach (var file in files_Tem)
                        {
                            System.IO.File.Delete(file);
                        }

                    }
                }
                string[] Files = Directory.GetFiles(DestFolder);
                var VersionCurrent = DbDAS.dataScannDocumentVersionHistory.Where(x => x.IsCurrentVersion == true).FirstOrDefault();
                List<dataScannDocumentImageVersionHistory> AddImagesToversionNumber = new List<dataScannDocumentImageVersionHistory>();
                if (MyoldDataScanDocumentImages.Count > 0 && MyoldDataScanDocumentImages != null)
                {
                    for (int i = 0; i < MyoldDataScanDocumentImages.Count; i++)
                    {
                        dataScannDocumentImageVersionHistory myDocVersionImage = new dataScannDocumentImageVersionHistory
                        {
                            ScannDocumentImageVersionHistoryGUID = Guid.NewGuid(),
                            ScannDocumentVersionHistoryGUID = VersionCurrent.ScannDocumentVersionHistoryGUID,
                            CreateByGUID = MyoldDataScanDocumentImages[i].CreateByGUID,
                            CreateDate = MyoldDataScanDocumentImages[i].CreateDate,
                            ImageName = MyoldDataScanDocumentImages[i].ImageName,
                            ImageNumber = MyoldDataScanDocumentImages[i].ImageNumber
                        };
                        string DestPathImage = Path.Combine(PathVersionNumber, Path.GetFileName(Files[i]));
                        string SoursePathImage = Path.Combine(DestFolder, Path.GetFileName(Files[i]));
                        System.IO.File.Copy(SoursePathImage, DestPathImage);
                        // add images to list 
                        AddImagesToversionNumber.Add(myDocVersionImage);
                    }
                    DbDAS.dataScannDocumentImageVersionHistory.AddRange(AddImagesToversionNumber);

                }

            }
            //convert to pdf 
            string ImagesPath = Server.MapPath("~/Areas/DAS//DEC/" + userGuid + "/" + search.FileNumber);

            string PDFName = VersionNumberCase + ".pdf";
            string PDFPath = Server.MapPath("~/Areas/DAS//Documents/VersionPDF/" + search.FileNumber + "/");
            Directory.CreateDirectory(PDFPath);

            if (VersionNumberCase != 0)
            {
                MyDocServices.ConvertToPdf(ImagesPath, PDFPath, PDFName);
            }
            //   delete folder which contain images after convert to pdf
            if (Directory.Exists(ImagesPath))
            {
                string[] files_Tem = Directory.GetFiles(ImagesPath);
                if (files_Tem.Count() > 0)
                {
                    foreach (var file in files_Tem)
                    {
                        System.IO.File.Delete(file);
                    }
                }
                //System.IO.File.Delete(ImagesPath);
            }

            var mycase = DbDAS.dataFile.Where(x => x.FileNumber == search.FileNumber).FirstOrDefault();
            var myDoc = DbDAS.dataScannDocument.Where(x => x.FileGUID == mycase.FileGUID).FirstOrDefault();
            var oldFlows = DbDAS.dataScannDocumentStatusFlow.Where(x => x.ScannDocumentGUID == myDoc.ScannDocumentGUID
              && x.IsLastAction == true).ToList();
            foreach (var item in oldFlows)
            {
                item.IsLastAction = false;
            }
            dataScannDocumentStatusFlow newFlow = new dataScannDocumentStatusFlow
            {
                ScannDocumentStatusFlowGUID = Guid.NewGuid(),
                ScannDocumentGUID = myDoc.ScannDocumentGUID,
                DocumentFlowStatusGUID = ScanDocumentStatus.Pending,
                CreateDate = DateTime.Now,
                IsLastAction = true,
                Active = true
            };
            DbDAS.dataScannDocumentStatusFlow.Add(newFlow);
            DbDAS.SaveChanges();
            return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);
        }
        //Read Images from TEMsearch
        public JsonResult ReadImagesFromTem(string data)
        {
            /// for decryption images
            byte[] Key = Encoding.UTF8.GetBytes("asdf!@#$1234ASDF");
            RijndaelManaged rmCryp = new RijndaelManaged();
            string path = Server.MapPath("~/Areas/DAS/Documents/" + data);
            Guid userGuid = UserGUID;
            string pathDecryption = Server.MapPath("~/Areas/DAS/DEC/" + userGuid + "/" + data);
            if (Directory.Exists(pathDecryption))
            {
                string[] files_Tem = Directory.GetFiles(pathDecryption);
                if (files_Tem.Count() > 0)
                {
                    foreach (var file in files_Tem)
                    {
                        System.IO.File.Delete(file);
                    }

                }
            }

            Directory.CreateDirectory(pathDecryption);
            //get images from foler 
            string[] files = Directory.GetFiles(path);
            // for decryption images

            imagesServices.DecryptionImages(files, pathDecryption, Key);
            ////end
            string filename;
            List<string> filename1 = new List<string>();
            List<dataScannDocumentImage> myTempImages = new List<dataScannDocumentImage>();
            var mycase = DbDAS.dataFile.Where(x => x.FileNumber == data).FirstOrDefault();
            var MydatascanDocument = DbDAS.dataScannDocument.Where(x => x.FileGUID == mycase.FileGUID).FirstOrDefault();
            myTempImages = DbDAS.dataScannDocumentImage.Where(x => x.ScannDocumentGUID == MydatascanDocument.ScannDocumentGUID).OrderBy(x => x.ImageNumber).ToList();
            for (int i = 0; i < myTempImages.Count; i++)
            {
                filename = myTempImages[i].ImageName;
                filename1.Add(filename);
            }
            return Json(new { success = 1, filename1 = filename1, userGuid = userGuid }, JsonRequestBehavior.AllowGet);
        }


        //order images
        public JsonResult UpdateItem(List<string> imageIds)
        {
            foreach (var itemId in imageIds)
            {
                try
                {
                    dataScannDocumentImage item = DbDAS.dataScannDocumentImage.Where(x => x.ImageName == itemId).FirstOrDefault();
                    item.ImageNumber = imageIds.IndexOf(itemId);
                }
                catch
                {

                }
                DbDAS.SaveChanges();
            }

            return Json(true);
        }
        public JsonResult ConvertToPdf(string Document)

        {
            string PdfName = Document + ".pdf";
            string pdfpath = Server.MapPath("~/Areas/DAS/Documents/PDF/");
            Guid userGuid = UserGUID;
            string imagepath = Server.MapPath("~/Areas/DAS/DEC/" + userGuid + "/" + Document);
            string filename = pdfpath + PdfName;
            //Create a new document
            iTextSharp.text.Document Doc = new iTextSharp.text.Document(PageSize.A4);
            // FileStream fs = new FileStream(pdfpath, FileMode.Create, FileAccess.Write, FileShare.None));
            PdfWriter writer = PdfWriter.GetInstance(Doc, new FileStream(filename, FileMode.Create));

            //Open the PDF for writing
            Doc.Open();

            //give folder from your local system that contains images
            string[] Files = System.IO.Directory.GetFiles(imagepath);
            foreach (var file in Files)
            {

                var image = Image.GetInstance(file);
                image.Alignment = iTextSharp.text.Image.ALIGN_CENTER;
                if (image.Height > image.Width)
                {
                    //Maximum height is 800 pixels.
                    float percentage = 0.0f;
                    percentage = 700 / image.Height;
                    image.ScalePercent(percentage * 100);
                }
                else
                {
                    //Maximum width is 600 pixels.
                    float percentage = 0.0f;
                    percentage = 540 / image.Width;
                    image.ScalePercent(percentage * 100);
                }
                //  image.SetAbsolutePosition((PageSize.A4.Width - image.ScaledWidth) / 2, (PageSize.A4.Height - image.ScaledHeight) / 2);
                Doc.Add(image);
            }


            //Close the PDF
            Doc.Close();


            return Json(true);
        }
        public JsonResult ConvertVersionToPdf(string Document, int Version)

        {
            Guid userGuid = UserGUID;

            // get path Images which convert to pdf
            string ImagesPath = Server.MapPath("~/Areas/DAS/DEC/" + userGuid + "/" + Document);

            string PDFName = Version + ".pdf";
            string PDFPath = Server.MapPath("~/Areas/DAS/Documents/VersionPDF/" + Document + "/");
            Directory.CreateDirectory(PDFPath);

            if (Version != 0)
            {
                MyDocServices.ConvertToPdf(ImagesPath, PDFPath, PDFName);
            }
            //delete folder which contain images after convert to pdf
            if (Directory.Exists(ImagesPath))
            {
                string[] files_Tem = Directory.GetFiles(ImagesPath);
                if (files_Tem.Count() > 0)
                {
                    foreach (var file in files_Tem)
                    {
                        System.IO.File.Delete(file);
                    }
                }
                //System.IO.File.Delete(ImagesPath);
            }
            return Json(true);
        }
        #endregion




        #region Contex Menu
        public JsonResult DeleteImage(Guid CaseNumber, string ImageId)
        {
            dataArchiveTemplateDocumentImage item = DbDAS.dataArchiveTemplateDocumentImage.Where(x => x.ImageName == ImageId).FirstOrDefault();
            string ImagePath = Server.MapPath("~/Areas/DAS/Documents/") + CaseNumber + "/" + item.ImageName;
            //delete image from Folder
            imagesServices.DeleteImage(ImagePath);

            //delete image from database DAS
            var MyDocument = DbDAS.dataArchiveTemplateDocument.Where(x => x.ArchiveTemplateDocumentGUID == CaseNumber).FirstOrDefault();
            List<dataArchiveTemplateDocumentImage> myTempImages = DbDAS.dataArchiveTemplateDocumentImage.Where(x => x.ArchiveTemplateDocumentGUID == MyDocument.ArchiveTemplateDocumentGUID).OrderBy(x => x.ImageNumber).ToList();
            for (int i = (int)(item.ImageNumber + 1); i < myTempImages.Count; i++)
            {
                myTempImages[i].ImageNumber = myTempImages[i].ImageNumber - 1;
            }
            DbDAS.dataArchiveTemplateDocumentImage.Remove(item);
            DbDAS.SaveChanges();
            return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SavwNewSacanImages(string mysearchKey, string ImageId, string dir_scan)
        {
            int NumberImageselected = 0;
            Guid userGuid = UserGUID;
            string path = Server.MapPath("~/Areas/DAS/Documents/" + mysearchKey);
            var mycase = DbDAS.dataFile.Where(x => x.FileNumber == mysearchKey).FirstOrDefault();
            var MyDocument = DbDAS.dataScannDocument.Where(x => x.FileGUID == mycase.FileGUID).FirstOrDefault();
            List<dataScannDocumentImage> OldTempImages = DbDAS.dataScannDocumentImage.Where(x => x.ScannDocumentGUID == MyDocument.ScannDocumentGUID).OrderBy(x => x.ImageNumber).ToList();
            //read imagenumber foe selected image
            NumberImageselected = (int)OldTempImages.Where(x => x.ImageName == ImageId).FirstOrDefault().ImageNumber;
            ////update number image after new scann
            int CountFiles = Request.Files.Count;
            if (dir_scan == "NewScanafter")
            {

                for (int i = NumberImageselected + 1; i < OldTempImages.Count; i++)
                {

                    OldTempImages[i].ImageNumber = OldTempImages[i].ImageNumber + CountFiles;
                }
                NumberImageselected = NumberImageselected + 1;
            }
            else
            {

                for (int i = NumberImageselected; i < OldTempImages.Count; i++)
                {
                    OldTempImages[i].ImageNumber = OldTempImages[i].ImageNumber + CountFiles;
                }
                NumberImageselected = CountFiles + NumberImageselected - 1;
            }
            DbDAS.SaveChanges();
            /// add images to temp after new scan
            List<dataScannDocumentImage> ToAddImages = new List<dataScannDocumentImage>();
            //save imagesin folder after encryption
            byte[] Key = Encoding.UTF8.GetBytes("asdf!@#$1234ASDF");
            HttpFileCollectionBase files = Request.Files;
            //function Encryptio
            imagesServices.EncryptionImage(files, path, Key);


            for (var i = 0; i < files.Count; i++)
            {
                HttpPostedFileBase file = files[i];
                Guid myImageGuid = Guid.NewGuid();
                if (dir_scan == "NewScanafter")
                {
                    dataScannDocumentImage myCurrImage = new dataScannDocumentImage
                    {
                        ScannDocumentImageGUID = myImageGuid,
                        ScannDocumentGUID = MyDocument.ScannDocumentGUID,
                        CreateByGUID = userGuid,
                        CreateDate = DateTime.Now,
                        ImageName = Path.GetFileName(file.FileName),
                        ImageNumber = i + NumberImageselected
                    };
                    ToAddImages.Add(myCurrImage);
                }
                else
                {

                    dataScannDocumentImage myCurrImage = new dataScannDocumentImage
                    {

                        ScannDocumentImageGUID = myImageGuid,
                        ScannDocumentGUID = MyDocument.ScannDocumentGUID,
                        CreateByGUID = userGuid,
                        CreateDate = DateTime.Now,
                        ImageName = Path.GetFileName(file.FileName),
                        ImageNumber = NumberImageselected - i
                    };
                    ToAddImages.Add(myCurrImage);
                }
            }

            DbDAS.dataScannDocumentImage.AddRange(ToAddImages);
            DbDAS.SaveChanges();
            return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);
        }
        // this function for save metadata
        public JsonResult SaveMetaDataImages(List<Guid> MetaDataGUIDs, string ImageId)
        {
            if (MetaDataGUIDs != null && ImageId != null)
            {
                List<dataScannDocumentImageMetaData> ToAddMetaData = new List<dataScannDocumentImageMetaData>();
                var ScannDocumentImageGUID = DbDAS.dataScannDocumentImage.Where(x => x.ImageName == ImageId).Select(x => x.ScannDocumentImageGUID).FirstOrDefault();

                foreach (var item in MetaDataGUIDs)
                {

                    dataScannDocumentImageMetaData MyCurrentMetaDataImage = new dataScannDocumentImageMetaData
                    {
                        ScannDocumentMetaDataGUID = Guid.NewGuid(),
                        ScannDocumentImageGUID = ScannDocumentImageGUID,
                        DocumentMetaDataGUID = item,
                        CreateDate = DateTime.Now,
                        Active = true,
                        DeletedOn = null
                    };
                    ToAddMetaData.Add(MyCurrentMetaDataImage);

                }


                DbDAS.dataScannDocumentImageMetaData.AddRange(ToAddMetaData);
                DbDAS.SaveChanges();
                return Json(true);
            }
            else
                return Json(false);
        }
        #endregion
        //this region for merge file
        #region
        public JsonResult MergeFile(Guid ScannDocumentGUID, string searchKey)
        {
            string FileNumber = DbDAS.dataScannDocument.Where(x => x.ScannDocumentGUID == ScannDocumentGUID).Select(x => x.dataFile.FileNumber).FirstOrDefault();
            string[] Files = Directory.GetFiles(Server.MapPath("~/Areas/DAS/Documents/" + FileNumber));
            string DestPath = Server.MapPath("~/Areas/DAS/Documents/" + searchKey + "/" + FileNumber);
            List<string> filename = new List<string>();
            if (Directory.Exists(DestPath))
            {
                string[] files_Tem = Directory.GetFiles(DestPath);
                if (files_Tem.Count() > 0)
                {
                    foreach (var file in files_Tem)
                    {
                        System.IO.File.Delete(file);
                    }

                }
            }
            else
            {
                Directory.CreateDirectory(DestPath);
            }
            foreach (var file in Files)
            {
                string DestImage = Path.Combine(DestPath, Path.GetFileName(file));
                string SourceImage = Path.GetFullPath(file);
                System.IO.File.Copy(SourceImage, DestImage);
                filename.Add(Path.GetFileName(file));
            }
            string path = Server.MapPath("~/Areas/DAS/Documents/" + FileNumber);
            Guid userGuid = UserGUID;
            string pathDecryption = Server.MapPath("~/Areas/DAS/DEC_MERGE/" + userGuid + "/" + FileNumber);

            if (Directory.Exists(pathDecryption))
            {
                string[] files_Tem = Directory.GetFiles(pathDecryption);
                if (files_Tem.Count() > 0)
                {
                    foreach (var file in files_Tem)
                    {
                        System.IO.File.Delete(file);
                    }

                }
            }
            else
            {
                Directory.CreateDirectory(pathDecryption);
            }
            //for decryption images
            string[] files = Directory.GetFiles(path);
            byte[] Key = Encoding.UTF8.GetBytes("asdf!@#$1234ASDF");
            imagesServices.DecryptionImages(files, pathDecryption, Key);
            return Json(new { success = 1, filename = filename, FileNumber = FileNumber, userGuid = userGuid }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Meta Data
        public ActionResult DocumentMetaDataCreate(Guid FK)
        {
            // DbDAS.dataScannDocumentMetaData.Where(x => x.ScannDocumentGUID == FK).ToList();
            return PartialView("~/Areas/DAS/Views/RefugeeScannedDocument/_DocumentMetaData.cshtml");

        }
        #endregion

        #region Scanner Settings

        public ActionResult GetScannerSettings()
        {
            if (!CMS.HasAction(Permissions.RefugeeScannedDocument.Create, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/DAS/Views/RefugeeScannedDocument/_ScannerSettings.cshtml", new ScannerSettingVM { });
        }

        public JsonResult InitScannerSettings()
        {

            var ScanningTypes = DbDAS.codeTablesValuesLanguages.Where(x => x.codeTablesValues.TableGUID == ScannerSettings.ScanningType
            && x.LanguageID == LAN)
                .Select(c => new { Guid = c.ValueGUID, Name = c.ValueDescription }).ToList();

            var PaperSize = DbDAS.codeTablesValuesLanguages.Where(x => x.codeTablesValues.TableGUID == ScannerSettings.PaperSize && x.LanguageID == LAN)
                .Select(c => new { Guid = c.ValueGUID, Name = c.ValueDescription }).ToList();
            var PaperFormat = DbDAS.codeTablesValuesLanguages.Where(x => x.codeTablesValues.TableGUID == ScannerSettings.PaperFormat && x.LanguageID == LAN)
                .Select(c => new { Guid = c.ValueGUID, Name = c.ValueDescription }).ToList();

            var ColorMode = DbDAS.codeTablesValuesLanguages.Where(x => x.codeTablesValues.TableGUID == ScannerSettings.ColorMod && x.LanguageID == LAN)
                 .Select(c => new { Guid = c.ValueGUID, Name = c.ValueDescription }).ToList();

            var Resolution = DbDAS.codeTablesValuesLanguages.Where(x => x.codeTablesValues.TableGUID == ScannerSettings.Resolution && x.LanguageID == LAN)
                .Select(c => new { Guid = c.ValueGUID, Name = c.ValueDescription }).ToList();
            return Json(new { ScanningTypes = ScanningTypes, PaperSize = PaperSize, PaperFormat = PaperFormat, ColorMode = ColorMode, Resolution = Resolution }, JsonRequestBehavior.AllowGet);
            //  return Json(new { ScanningTypes = ScanningTypes, PaperFormat = PaperFormat } , JsonRequestBehavior.AllowGet);

        }
        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ScannerSettingsCreate(ScannerSettingVM scannerSetting)
        {
            int success = 0;

            if (scannerSetting.ColorMode != null && scannerSetting.PaperSize != null && scannerSetting.Resolution != null && scannerSetting.ScanningType != null && scannerSetting.PaperFormate != null)
            {
                //remove all settings
                var toRemove = DbDAS.dataDefaultScannerSetting.ToList();
                DbDAS.dataDefaultScannerSetting.RemoveRange(toRemove);
                DbDAS.SaveChanges();
                success = 1;
                //add new settings
                //Guid userGuid = (Guid)Session[SessionKeys.UserGuidKey];
                dataDefaultScannerSetting model = new dataDefaultScannerSetting
                {
                    ScannerSettingGUID = Guid.NewGuid(),
                    ScanningType = scannerSetting.ScanningType,
                    PaperFormate = scannerSetting.PaperFormate,
                    Resolution = scannerSetting.Resolution,
                    PaperSize = scannerSetting.PaperSize,
                    ColorMode = scannerSetting.ColorMode,
                    UserGUID=UserGUID
                };
                DbDAS.dataDefaultScannerSetting.Add(model);
                DbDAS.SaveChanges();
                var currentUserScannerSetting = DbDAS.dataDefaultScannerSetting.
                   Select(x => new { x.ScanningType, x.PaperFormate, x.Resolution, x.PaperSize, x.ColorMode }).
                   FirstOrDefault();
                return Json(new { success = success, currentUserScannerSetting = currentUserScannerSetting }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = success }, JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region Transfer File 
        public ActionResult FileTransferHistoryDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/DAS/Views/RefugeeScannedDocument/_DocumentMetaData.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<FileTransferHistoryDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<FileTransferHistoryDataTableModel>(DataTable.Filters);
            }

            var Result = (

                from a in DbDAS.dataScannDocumentTransfer.Where(x => x.dataFileRequest.FileGUID == PK).AsExpandable()
                join b in DbDAS.dataScannDocumentTransferFlow.Where(x => x.Active && x.IsLastAction == true) on a.ScannDocumentTransferGUID equals b.ScannDocumentTransferGUID into LJ1
                from R1 in LJ1.DefaultIfEmpty()
                join e in DbDAS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on R1.DocumentFlowStatusGUID equals e.ValueGUID into LJ4
                from R4 in LJ4.DefaultIfEmpty()

                join c in DbDAS.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.CreateByGUID equals c.UserGUID into LJ2
                from R2 in LJ2.DefaultIfEmpty()

                join d in DbDAS.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.ReceiveByGUID equals d.UserGUID into LJ3
                from R3 in LJ3.DefaultIfEmpty()

                select new FileTransferHistoryDataTableModel
                {
                    ScannDocumentTransferGUID = a.ScannDocumentTransferGUID.ToString(),
                    CustodianName = a.CustodianName,
                    CustodianType = a.CustodianType,
                    TransferDate = a.TransferDate,
                    ReceiveDate = a.ReceiveDate,
                    DeliveryStatus = R4.ValueDescription,
                    TransferBy = R2.FirstName + " " + R2.Surname,
                    ReceiveBy = R3.FirstName + " " + R3.Surname,
                    RequesterGUID = a.RequesterGUID.ToString(),
                    RequesterTypeGUID = a.RequesterTypeGUID.ToString(),
                    ReceiveByGUID = a.ReceiveByGUID.ToString(),
                    CreateByGUID = a.CreateByGUID.ToString(),
                    dataScannDocumentTransferRowVersion = a.dataScannDocumentTransferRowVersion,
                    Active = a.Active





                }).Where(Predicate);

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTransferFile(string FK)
        {
            if (!CMS.HasAction(Permissions.RefugeeScannedDocument.Create, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var myCase = DbDAS.dataFile.Where(x => x.FileNumber == FK).FirstOrDefault();
            return PartialView("~/Areas/DAS/Views/RefugeeScannedDocument/_TransferFile.cshtml", new TransferFileModel { FileGUID = myCase.FileGUID });
        }
        public ActionResult RequestFileCreate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.RefugeeScannedDocument.Create, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var myCase = DbDAS.dataFile.Where(x => x.FileGUID == PK).FirstOrDefault();
            return PartialView("~/Areas/DAS/Views/FileRequest/_RequestFile.cshtml",
                new FileRequestModel { FileGUID = myCase.FileGUID });
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult RequestFileCreate(FileRequestModel transferFile)
        {
            if (!CMS.HasAction(Permissions.RefugeeScannedDocument.Create, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }


            if (transferFile != null)
            {


                var fileRequstGUIDs = DbDAS.dataFileRequest.Where(x => x.FileGUID == transferFile.FileGUID
                && (x.dataFile.IsRequested == true)
                  ).Select(x => x.FileGUID).ToList();
                var myModels = DbDAS.dataFile.Where(x =>
                   x.FileGUID == transferFile.FileGUID
                    && (x.LastCustodianTypeNameGUID != UserGUID || x.LastTransferFromNameGUID == null)
                    && !fileRequstGUIDs.Contains(x.FileGUID)).ToList();
                string custName = "";
                string typeName = "";

                var myStaff = DbDAS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN
                && x.UserGUID == UserGUID).FirstOrDefault();
                custName = myStaff.FirstName + " " + myStaff.Surname;
                typeName = "Staff";



                var myFiles = DbDAS.dataFile.Where(x => x.FileGUID == transferFile.FileGUID).ToList();
                var myFileGUIDs = myModels.Select(x => x.FileGUID).ToList();
                DateTime ExecutionTime = DateTime.Now;
                if (myModels.Count > 0)
                {
                    List<dataFileRequest> ToAddrequests = new List<dataFileRequest>();
                    #region Retertive items
                    foreach (var myModel in myModels)
                    {
                        var priRequst = DbDAS.dataFileRequest.Where(x => x.FileGUID == myModel.FileGUID).ToList();

                        priRequst.ForEach(x => x.IsLastRequest = false);
                        var myCurrentFile = DbDAS.dataFile.Where(x => x.FileGUID == myModel.FileGUID).FirstOrDefault();
                        myCurrentFile.IsRequested = true;
                        myCurrentFile.RequestStatusGUID = FileRequestStatus.Requested;
                        myCurrentFile.RequestStatusName = "Requested";
                        myCurrentFile.LastRequesterName = custName;
                        myCurrentFile.LastRequesterNameGUID = UserGUID;
                        DbDAS.Update(myCurrentFile, Permissions.RefugeeScannedDocument.UpdateGuid, ExecutionTime, DbCMS);


                        dataFileRequest myRequest = new dataFileRequest
                        {
                            FileRequestGUID = Guid.NewGuid(),
                            FileGUID = myModel.FileGUID,

                            RequesterTypeGUID = DASDocumentCustodianType.Staff,
                            RequesterGUID = UserGUID,
                            RequestDate = transferFile.RequestDate,
                            RequestDurationDate = transferFile.RequestDurationDate,
                            RequestStatusGUID = FileRequestStatus.Requested,
                            IsLastRequest = true,
                            OrderId = priRequst.Select(x => x.OrderId).Max() + 1 ?? 1,
                            Comments = transferFile.Comments,
                            RequsterFromNameGUID = myFiles.Where(x => x.FileGUID == myModel.FileGUID).Select(x => x.LastRequesterNameGUID).FirstOrDefault(),
                            RequsterFromName = myFiles.Where(x => x.FileGUID == myModel.FileGUID).Select(x => x.LastCustodianTypeName).FirstOrDefault(),
                            RequsterName = custName,


                        };
                        ToAddrequests.Add(myRequest);
                        DbDAS.Create(myRequest, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);

                        try
                        {
                            DbDAS.SaveChanges();
                            DbCMS.SaveChanges();






                        }
                        catch (Exception e)
                        {

                            throw;
                        }

                    }

                    #endregion
                    SendConfirmationRequestingFileEmail(ToAddrequests);
                    var myFile = DbDAS.dataFile.Where(x => x.FileGUID == transferFile.FileGUID).FirstOrDefault();
                    return Json(DbDAS.SingleUpdateMessage(DataTableNames.RefugeeScannedDocumentDataTable, DbDAS.PrimaryKeyControl(myFile), DbDAS.RowVersionControls(Portal.SingleToList(myFile))));
                }

            }
            return Json(new { success = 0 }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TransferFileCreate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.RefugeeScannedDocument.Create, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var myCase = DbDAS.dataFile.Where(x => x.FileGUID == PK).FirstOrDefault();
            return PartialView("~/Areas/DAS/Views/RefugeeScannedDocument/_TransferFile.cshtml",
                new TransferFileModel { FileGUID = myCase.FileGUID, FileRequestGUID = myCase.LastRequesterNameGUID });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TransferFileCreate(TransferFileModel transferFile)
        {
            int success = 0;
            DateTime ExecutionTime = DateTime.Now;
            var myFile = DbDAS.dataFile.Where(x => x.FileGUID == transferFile.FileGUID).FirstOrDefault();
            var unit = DbDAS.codeDASDestinationUnitFocalPoint.Where(x => x.UserGUID == transferFile.LastCustodianTypeNameGUID).FirstOrDefault();
            dataFileRequest fileRequest = new dataFileRequest();
            if (transferFile.FileRequestGUID == null)
            {
                var priRequst = DbDAS.dataFileRequest.Where(x => x.FileGUID == transferFile.FileGUID).ToList();

                priRequst.ForEach(x => x.IsLastRequest = false);
                var myStaff = DbDAS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN
                        && x.UserGUID == transferFile.LastCustodianTypeNameGUID).FirstOrDefault();
                string custName = myStaff.FirstName + " " + myStaff.Surname;
                string typeName = "Staff";



                fileRequest = new dataFileRequest
                {

                    FileRequestGUID = Guid.NewGuid(),
                    FileGUID = transferFile.FileGUID,
                    RequesterTypeGUID = transferFile.LastCustodianTypeGUID,
                    RequesterGUID = transferFile.LastCustodianTypeNameGUID,
                    RequestDate = transferFile.RequestDate != null ? transferFile.RequestDate : ExecutionTime,
                    RequestDurationDate = transferFile.RequestDurationDate,
                    RequestStatusGUID = FileRequestStatus.Delivered,
                    IsLastRequest = true,
                    OrderId = priRequst.Select(x => x.OrderId).Max() + 1 ?? 1,
                    Comments = transferFile.Comments,
                    RequsterFromNameGUID = myFile.LastCustodianTypeNameGUID,
                    RequsterFromName = myFile.LastCustodianTypeName,
                    RequsterName = custName,

                };
                DbDAS.Create(fileRequest, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);



            }
            else
            {
                fileRequest = DbDAS.dataFileRequest.Where(x => x.FileGUID == transferFile.FileGUID && x.IsLastRequest == true).FirstOrDefault();
                fileRequest.RequestStatusGUID = FileRequestStatus.Delivered;

                DbDAS.Update(fileRequest, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);
            }
            if (transferFile.FileGUID != null)
            {
                string custType = "";
                string custName = "";
                var oldTransfer = DbDAS.dataScannDocumentTransferFlow.Where(x => x.dataScannDocumentTransfer.dataFileRequest.FileGUID == transferFile.FileGUID).ToList();
                oldTransfer.ForEach(x => x.IsLastAction = false);

                dataScannDocumentTransfer transfer = new dataScannDocumentTransfer
                {
                    ScannDocumentTransferGUID = Guid.NewGuid(),
                    FileRequestGUID = fileRequest.FileRequestGUID,
                    RequesterTypeGUID = fileRequest.RequesterTypeGUID,
                    RequesterGUID = fileRequest.RequesterGUID,
                    TransferDate = ExecutionTime,
                    CreateByGUID = UserGUID,
                    CreateDate = ExecutionTime,

                    OrderId = oldTransfer.Select(x => x.OrderId).Max() != null ? oldTransfer.Select(x => x.OrderId).Max() + 1 : 1,

                };

                dataScannDocumentTransferFlow flow = new dataScannDocumentTransferFlow
                {
                    ScannDocumentTransferFlowGUID = Guid.NewGuid(),
                    ScannDocumentTransferGUID = transfer.ScannDocumentTransferGUID,
                    DocumentFlowStatusGUID = ScanDocumentTransferFlowStatus.Pending,
                    IsLastAction = true,
                    CreateByGUID = UserGUID,
                    CreateDate = ExecutionTime

                };

                var priLocations = DbDAS.dataFileLocationMovement.Where(x => x.FileGUID == transferFile.FileGUID && x.IsLastAction == true).ToList();
                priLocations.ForEach(f => f.IsLastAction = false);
                DbDAS.UpdateBulk(priLocations, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);

                if (transferFile.TransferLocationGUID != null)
                {
                    dataFileLocationMovement movement = new dataFileLocationMovement
                    {
                        FileLocationMovementGUID = Guid.NewGuid(),
                        FileGUID = transferFile.FileGUID,
                        TransferLocationGUID = transferFile.TransferLocationGUID,
                        IsLastAction = true,
                        ActionDate = ExecutionTime,
                        CreatedByGUID = UserGUID
                    };
                    DbDAS.Create(movement, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);
                }


                DbDAS.Create(flow, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);
                DbDAS.UpdateBulk(oldTransfer, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);
                var file = DbDAS.dataFile.Where(x => x.FileGUID == transferFile.FileGUID).FirstOrDefault();
                //if (fileRequest.RequesterTypeGUID == DASDocumentCustodianType.Staff)
                //{
                var myStaff = DbDAS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.UserGUID == fileRequest.RequesterGUID).FirstOrDefault();
                file.LastCustodianTypeName = myStaff.FirstName + " " + myStaff.Surname;
                var myLocation = DbDAS.codeDASTransferLocation.Where(x => x.TransferLocationGUID == transferFile.TransferLocationGUID).FirstOrDefault(); ;
                if (transferFile != null)
                {
                    file.LastTransferLocationGUID = transferFile.TransferLocationGUID;
                }
                //file.LastTransferLocationName = myLocation.TransferLocationName;
                file.LastCustodianType = "Staff";
                custType = "Staff";
                custName = myStaff.FirstName + " " + myStaff.Surname;
                file.LastCustodianTypeGUID = fileRequest.RequesterTypeGUID;
                file.LastCustodianTypeNameGUID = fileRequest.RequesterGUID;
                file.LastFlowStatusName = "Pending";
                file.LastFlowStatusGUID = ScanDocumentTransferFlowStatus.Pending;
                file.RequestStatusGUID = FileRequestStatus.Delivered;
                file.RequestStatusName = "Delivered";
                file.IsRequested = false;
                file.LastUnitName = unit.codeDASDestinationUnit.DestinationUnitName;
                file.LastDestinationUnitGUID = unit.DestinationUnitGUID;


                var myTransfer = DbDAS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.UserGUID == UserGUID).FirstOrDefault();
                file.LastTransferFromName = myTransfer.FirstName + " " + myTransfer.Surname;
                file.LastTransferFromNameGUID = UserGUID;



                DbDAS.Update(file, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);

                transfer.CustodianName = custName;
                transfer.CustodianType = custType;
                transfer.TransferFromName = myFile.LastCustodianTypeName;



                DbDAS.Create(transfer, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);

                try
                {
                    DbDAS.SaveChanges();
                    DbCMS.SaveChanges();
                    if (fileRequest.RequesterTypeGUID != null && fileRequest.RequesterTypeGUID == DASDocumentCustodianType.Staff)
                    {
                        SendConfirmationReceivingFileEmail((Guid)file.LastCustodianTypeNameGUID, transferFile.FileGUID,
                          transfer.ScannDocumentTransferGUID, 1);
                    }

                    return Json(DbDAS.SingleUpdateMessage(DataTableNames.RefugeeScannedDocumentDataTable, DbDAS.PrimaryKeyControl(file), DbDAS.RowVersionControls(Portal.SingleToList(file))));
                    //return Json(DbDAS.SingleCreateMessage(DbDAS.PrimaryKeyControl(transfer), DbDAS.RowVersionControls(transfer, transfer), null, "", null));
                }
                catch (Exception e)
                {
                    return Json(DbDAS.ErrorMessage(e.Message));
                }
            }
            return Json(new { success = success }, JsonRequestBehavior.AllowGet);
        }



        #endregion
        #region Confirm Receiving File
        public ActionResult ConfirmReceivingBulkFiles(Guid id)
        {
            var files = DbDAS.dataFile.Where(x => x.LastCustodianTypeNameGUID == id
             && x.LastFlowStatusGUID == ScanDocumentTransferFlowStatus.Pending).ToList();
            if (files.Count <= 0)
            {
                throw new HttpException(401, "Unauthorized access");
            }
            //if(DangerPay.FlowStatusGUID == NationalStaffDangerPaConfirmationStatus.Confirmed)
            //{
            //    return View("~/Areas/DAS/Views/NationalStaffDangerPayDetail/DangerPayConfirmed.cshtml");
            //}
            List<ConfirmReceivingFileModel> models = new List<ConfirmReceivingFileModel>();
            foreach (var item in files)
            {
                ConfirmReceivingFileModel myModel = new ConfirmReceivingFileModel
                {
                    FileGUID = item.FileGUID,
                    FileNumber = item.FileNumber,
                    LastCustodianTypeNameGUID = item.LastCustodianTypeNameGUID,
                    Validation = true
                };
                models.Add(myModel);
            }
            return View("~/Areas/DAS/Views/FileTransfer/ConfirmReceivingBulkFiles.cshtml", models);
        }

        public ActionResult ConfirmReceivingFile(Guid id)
        {
            var scannFileTransferFlow = DbDAS.dataScannDocumentTransferFlow.Where(x => x.ScannDocumentTransferGUID == id
            && x.IsLastAction == true).FirstOrDefault();
            if (scannFileTransferFlow.DocumentFlowStatusGUID != ScanDocumentTransferFlowStatus.Pending || UserGUID != scannFileTransferFlow.dataScannDocumentTransfer.RequesterGUID)
            {
                throw new HttpException(401, "Unauthorized access");
            }
            //if(DangerPay.FlowStatusGUID == NationalStaffDangerPaConfirmationStatus.Confirmed)
            //{
            //    return View("~/Areas/DAS/Views/NationalStaffDangerPayDetail/DangerPayConfirmed.cshtml");
            //}
            return View("~/Areas/DAS/Views/FileTransfer/ConfirmReceivingFile.cshtml", new ConfirmReceivingFileModel
            {
                ScannDocumentTransferGUID = id,
                DocumentFlowStatusGUID = scannFileTransferFlow.DocumentFlowStatusGUID,
                FileNumber = scannFileTransferFlow.dataScannDocumentTransfer.dataFileRequest.dataFile.FileNumber
            });
        }

        public JsonResult ConfirmReceivingFileCreate(Guid guid)
        {
            var _detail = DbDAS.dataScannDocumentTransferFlow.Where(x => x.ScannDocumentTransferGUID == guid
            && x.IsLastAction == true).FirstOrDefault();
            if (_detail.DocumentFlowStatusGUID != ScanDocumentTransferFlowStatus.Pending && UserGUID != _detail.dataScannDocumentTransfer.RequesterGUID)
            {
                return Json(new { success = 0 }, JsonRequestBehavior.AllowGet);
            }
            DateTime ExecutionTime = DateTime.Now;
            _detail.IsLastAction = false;



            var myFile = DbDAS.dataFile.Where(x => x.FileGUID == _detail.dataScannDocumentTransfer.dataFileRequest.FileGUID).FirstOrDefault();

            myFile.LastFlowStatusGUID = ScanDocumentTransferFlowStatus.Confirmed;
            myFile.LastFlowStatusName = "Confirmed";
            DbDAS.Update(_detail, Permissions.RefugeeScannedDocument.UpdateGuid, ExecutionTime, DbCMS);
            DbDAS.Update(myFile, Permissions.RefugeeScannedDocument.UpdateGuid, ExecutionTime, DbCMS);

            dataScannDocumentTransferFlow newFlow = new dataScannDocumentTransferFlow
            {
                ScannDocumentTransferFlowGUID = Guid.NewGuid(),
                ScannDocumentTransferGUID = guid,
                DocumentFlowStatusGUID = ScanDocumentTransferFlowStatus.Confirmed,
                IsLastAction = true,
                CreateByGUID = UserGUID,
                CreateDate = ExecutionTime

            };

            DbDAS.Create(newFlow, Permissions.RefugeeScannedDocument.UpdateGuid, ExecutionTime, DbCMS);



            try
            {
                DbDAS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);
                //return Json(DbDAS.SingleUpdateMessage(DataTableNames.NationalStaffDangerPayManagementDataTable, DbAHD.PrimaryKeyControl(model), DbAHD.RowVersionControls(Portal.SingleToList(NationalStaffDangerPayInformation))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return Json(new { success = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult ConfirmReceivingBulkModelsEmail(List<Guid> guids)
        {
            var _details = DbDAS.dataFile.Where(x => guids.Contains(x.FileGUID) && x.LastFlowStatusGUID == ScanDocumentTransferFlowStatus.Pending
            ).ToList();
            var custGUID = _details.Select(f => f.LastCustodianTypeNameGUID).FirstOrDefault();
            if (_details.Count <= 0)
            {
                return Json(new { success = 0 }, JsonRequestBehavior.AllowGet);
            }
            DateTime ExecutionTime = DateTime.Now;
            foreach (var _detail in _details)
            {
                var _myFlow = DbDAS.dataScannDocumentTransferFlow.Where(x =>
                x.dataScannDocumentTransfer.dataFileRequest.dataFile.FileGUID == _detail.FileGUID
                && x.dataScannDocumentTransfer.RequesterGUID == custGUID
                && x.DocumentFlowStatusGUID == ScanDocumentTransferFlowStatus.Pending
             && x.IsLastAction == true).FirstOrDefault();
                _myFlow.IsLastAction = false;
                var myFile = DbDAS.dataFile.Where(x => x.FileGUID == _detail.FileGUID).FirstOrDefault();
                myFile.LastFlowStatusGUID = ScanDocumentTransferFlowStatus.Confirmed;
                myFile.LastFlowStatusName = "Confirmed";
                DbDAS.Update(_detail, Permissions.RefugeeScannedDocument.UpdateGuid, ExecutionTime, DbCMS);
                DbDAS.Update(myFile, Permissions.RefugeeScannedDocument.UpdateGuid, ExecutionTime, DbCMS);

                dataScannDocumentTransferFlow newFlow = new dataScannDocumentTransferFlow
                {
                    ScannDocumentTransferFlowGUID = Guid.NewGuid(),
                    ScannDocumentTransferGUID = _myFlow.ScannDocumentTransferGUID,
                    DocumentFlowStatusGUID = ScanDocumentTransferFlowStatus.Confirmed,
                    IsLastAction = true,
                    CreateByGUID = UserGUID,
                    CreateDate = ExecutionTime

                };

                DbDAS.Create(newFlow, Permissions.RefugeeScannedDocument.UpdateGuid, ExecutionTime, DbCMS);



                try
                {
                    DbDAS.SaveChanges();
                    DbCMS.SaveChanges();

                    //return Json(DbAHD.SingleUpdateMessage(DataTableNames.NationalStaffDangerPayManagementDataTable, DbAHD.PrimaryKeyControl(model), DbAHD.RowVersionControls(Portal.SingleToList(NationalStaffDangerPayInformation))));
                }
                catch (DbUpdateConcurrencyException)
                {
                    return Json(new { success = 0 }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);

        }



        #endregion
        #region Mails

        public void SendConfirmationTransferBulkFilesEmail(TransferFileModel model, List<Guid> scannDocumentTransferGUIDs, int? type)
        {
            string URL = "";
            string Anchor = "";
            string Link = "";
            string table = "";
            DateTime ExecutionTime = DateTime.Now;

            table = "<br><table class='table table-bordered' style='border-collapse: collapse; width: 900px'><tr><th style='border: 1px solid  #333; vertical-align:middle'>File Number</th><th style='border: 1px solid  #333; vertical-align:middle'>Transferred  by</th></tr>";
            var files = DbDAS.dataScannDocumentTransfer.AsQueryable();
            string issued = "";
            foreach (var item in scannDocumentTransferGUIDs.Distinct())
            {
                var file = files.Where(x => x.ScannDocumentTransferGUID == item).FirstOrDefault();
                var issuedBy = DbDAS.userPersonalDetailsLanguage.Where(x => x.UserGUID == file.CreateByGUID
                                                                  && x.LanguageID == LAN).FirstOrDefault();
                issued = issuedBy.FirstName + " " + issuedBy.Surname;
                table += "<tr><td style='border: 1px solid  #333; vertical-align: middle'>" + file.dataFileRequest.dataFile.FileNumber + "</td><td style='border: 1px solid  #333; vertical-align: middle'>" + issuedBy.FirstName + " " + issuedBy.Surname + "</td></tr>";
            }
            table += "</table>";
            var myUser = DbDAS.userPersonalDetailsLanguage.Where(x => x.UserGUID == model.RequesterGUID
                                                                          && x.LanguageID == LAN).FirstOrDefault();
            var myAccount = DbDAS.userServiceHistory.Where(x => x.UserGUID == model.RequesterGUID).FirstOrDefault();
            string SubjectMessage = resxEmails.DASFilteTrackingConfirmReceiveBulkFilesSubject;
            //foreach (var user in allUsers)
            //{
            //ffx

            URL = AppSettingsKeys.Domain + "/DAS/ScanDocument/ConfirmUserBulkFilesRecipt/?id=" + new Portal().GUIDToString(model.RequesterGUID);
            Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ClickToConfirm + "</a>";
            Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";
            //string myFirstName = myUser.FirstName;
            //string mySurName = myUser.Surname;

            string _message = resxEmails.DASConfirmBulkFileReceiving
                .Replace("$FullName", myUser.FirstName + " " + myUser.Surname)
                .Replace("$VerifyLink", Anchor)
                .Replace("$IssuedUser", issued)
                .Replace("$table", table)
                  .Replace("$issueDate", ExecutionTime.ToShortDateString()
                )
                ;
            if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
            int isRec = 1;
            var myEmail = myAccount.EmailAddress;
            SendMailGeneral(myEmail, SubjectMessage, _message, isRec);


        }

        public void SendConfirmationReceivingFileEmail(Guid LastCustodianTypeNameGUID, Guid? FileGUID, Guid ScannDocumentTransferGUID, int? type)
        {
            string URL = "";
            string Anchor = "";
            string Link = "";
            var scannFile = DbDAS.dataScannDocumentTransfer.Where(x => x.ScannDocumentTransferGUID == ScannDocumentTransferGUID).FirstOrDefault();


            var myUser = DbDAS.userPersonalDetailsLanguage.Where(x => x.UserGUID == LastCustodianTypeNameGUID
                                                                          && x.LanguageID == LAN).FirstOrDefault();
            var issuedBy = DbDAS.userPersonalDetailsLanguage.Where(x => x.UserGUID == scannFile.CreateByGUID
                                                                      && x.LanguageID == LAN).FirstOrDefault();

            var myAccount = DbDAS.userServiceHistory.Where(x => x.UserGUID == LastCustodianTypeNameGUID).FirstOrDefault();

            if (type == 2)
            {
                string SubjectMessage = resxEmails.DASReturnFilesReminder.Replace
                                  ("$FileNumber", scannFile.dataFileRequest.dataFile.FileNumber);
                //foreach (var user in allUsers)
                //{

                URL = AppSettingsKeys.Domain + "/DAS/ScanDocument/ConfirmUserBulkFilesRecipt/?id=" + new Portal().GUIDToString(LastCustodianTypeNameGUID);
                Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ClickToConfirm + "</a>";
                Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";
                string _message = resxEmails.DASFileReturnToFilingUnit
                .Replace("$FullName", myUser.FirstName + " " + myUser.Surname)
                 .Replace("$VerifyLink", Anchor)
                .Replace("$IssuedUser", issuedBy.FirstName + " " + issuedBy.Surname)
                .Replace("$issueDate", scannFile.CreateDate.Value.ToLongDateString())
                .Replace("$FileNumber", scannFile.dataFileRequest.dataFile.FileNumber)
                ;
                if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                int isRec = 1;
                var myEmail = myAccount.EmailAddress;
                SendMailGeneral(myEmail, SubjectMessage, _message, isRec);
            }
            else if (type == 1)
            {

                string SubjectMessage = resxEmails.DASFilteTrackingConfirmReceivingSubject.Replace
                    ("$FileNumber", scannFile.dataFileRequest.dataFile.FileNumber);
                URL = AppSettingsKeys.Domain + "/DAS/ScanDocument/ConfirmReceivingFile/?id=" + new Portal().GUIDToString(scannFile.ScannDocumentTransferGUID);
                Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ClickToConfirm + "</a>";
                Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";
                string _message = resxEmails.DASFilteTrackingConfirmReceiving
                    .Replace("$FullName", myUser.FirstName + " " + myUser.Surname)
                    .Replace("$VerifyLink", Anchor)
                    .Replace("$IssuedUser", issuedBy.FirstName + " " + issuedBy.Surname)
                      .Replace("$issueDate", scannFile.CreateDate.Value.ToLongDateString())
                    .Replace("$FileNumber", scannFile.dataFileRequest.dataFile.FileNumber)
                    ;
                if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                int isRec = 1;
                var myEmail = myAccount.EmailAddress;
                SendMailGeneral(myEmail, SubjectMessage, _message, isRec);
            }
        }

        public void SendConfirmationRequestingFileEmail(List<dataFileRequest> models)
        {
            string URL = "";
            string Anchor = "";
            string Link = "";
            var fileRequestGUIDs = models.Select(f => f.FileGUID).ToList();
            var selectedFiels = DbDAS.dataFile.Where(x => fileRequestGUIDs.Contains(x.FileGUID)).ToList();
            var dataFilesCustoidansGUIDs = selectedFiels.Select(x => x.LastCustodianTypeNameGUID).ToList();
            Guid filingUnitGUID = Guid.Parse("bb915454-05fb-4e4d-b033-e01c7c790997");
            var filingUsers = DbDAS.codeDASDestinationUnitFocalPoint.Where(a => a.DestinationUnitGUID == filingUnitGUID && a.Active==true).ToList();
            var lastCusts = selectedFiels.Select(x => x.LastCustodianTypeNameGUID).ToList();
            var allcustdiaons = selectedFiels.Where(x => lastCusts.Contains(x.LastCustodianTypeNameGUID) && x.LastCustodianTypeNameGUID != null).Select(x => x.LastCustodianTypeNameGUID).Distinct().ToList();

            var casesForFiling = selectedFiels.Where(x => x.LastCustodianTypeNameGUID == null).Select(x => x.FileGUID).Distinct().ToList();
            DateTime nowDate = DateTime.Now;
            foreach (var item in allcustdiaons)
            {
                var filesToRequest = selectedFiels.Where(x => x.LastCustodianTypeNameGUID == item).ToList();


                var myCustdianInfo = DbDAS.userPersonalDetailsLanguage.Where(x => x.UserGUID == item
                                                          && x.LanguageID == LAN).FirstOrDefault();

                var myCustodianAccount = DbDAS.userServiceHistory.Where(x => x.UserGUID == item).FirstOrDefault();

                var RequestedBy = DbDAS.userPersonalDetailsLanguage.Where(x => x.UserGUID == UserGUID
                                                                   && x.LanguageID == LAN).FirstOrDefault();
                var table = "<br><table class='table table-bordered' style='border-collapse: collapse; width: 900px'><tr><th style='border: 1px solid  #333; vertical-align:middle'>File Number(s)</th></tr>";

                string issued = "";
                foreach (var myCurrentFile in filesToRequest)
                {
                    var file = selectedFiels.Where(x => x.FileGUID == myCurrentFile.FileGUID).FirstOrDefault();

                    table += "<tr><td style='border: 1px solid  #333; vertical-align: middle'>" + file.FileNumber + "</td></tr>";
                }
                table += "</table>";


                //if (custdiaon.LastCustodianTypeNameGUID == null)
                //{
                //    foreach (var myUser in filingUsers)
                //    {
                //        var myFilingUserInfo = DbDAS.userPersonalDetailsLanguage.Where(x => x.UserGUID == myUser.UserGUID
                //                                && x.LanguageID == LAN).FirstOrDefault();
                //        var myFilingUserInfoAccount = DbDAS.userServiceHistory.Where(x => x.UserGUID == myUser.UserGUID).FirstOrDefault();
                //        string SubjectMessage = resxEmails.DASRequestFilesFrom
                //  .Replace("$fileNumber", item.FileNumber)
                //  .Replace("$requester", issuedBy.FirstName + " " + issuedBy.Surname);
                //        //foreach (var user in allUsers)
                //        //{

                //        URL = AppSettingsKeys.Domain + "/DAS/ScanDocument/ConfirmRequestPyhsicalFile/?id=" + new Portal().GUIDToString(myRequester.FileRequestGUID);
                //        Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ClickToConfirm + "</a>";
                //        Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";
                //        //string myFirstName = myUser.FirstName;
                //        //string mySurName = myUser.Surname;

                //        string _message = resxEmails.DASRequestPysicalFile
                //            .Replace("$FullName", myFilingUserInfo.FirstName + " " + myFilingUserInfo.Surname)
                //             .Replace("$VerifyLink", Anchor)


                //             .Replace("$requester", issuedBy.FirstName + " " + issuedBy.Surname)
                //            .Replace("$FileNumber", item.FileNumber)
                //            ;
                //        if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                //        int isRec = 1;
                //        var myEmail = myFilingUserInfoAccount.EmailAddress;
                //        SendMailGeneral(myEmail, SubjectMessage, _message, isRec);
                //    }
                //}

                //else {
                string SubjectMessage = resxEmails.DASRequestFilesFrom;
                //foreach (var user in allUsers)
                //{

                URL = AppSettingsKeys.Domain + "/DAS/ScanDocument/ConfirmRequestPyhsicalFile/?id=" + new Portal().GUIDToString(item);
                Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ClickToConfirm + "</a>";
                Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";
                string _message = resxEmails.DASRequestPysicalFile
                     .Replace("$FullName", myCustdianInfo.FirstName + " " + myCustdianInfo.Surname)
                      .Replace("$VerifyLink", Anchor)


                      .Replace("$requester", RequestedBy.FirstName + " " + RequestedBy.Surname)
                     .Replace("$RequestDate", nowDate.ToShortDateString())
                     .Replace("$files", table)
                     ;
                if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                int isRec = 1;
                var myEmail = myCustodianAccount.EmailAddress;
                SendMailGeneral(myEmail, SubjectMessage, _message, isRec);
                //}

            }
            if (casesForFiling.Count > 0)
            {

                var RequestedBy = DbDAS.userPersonalDetailsLanguage.Where(x => x.UserGUID == UserGUID
                                                                   && x.LanguageID == LAN).FirstOrDefault();
                var table = "<br><table class='table table-bordered' style='border-collapse: collapse; width: 900px'><tr><th style='border: 1px solid  #333; vertical-align:middle'>File Number(s)</th></tr>";

                string issued = "";
                foreach (var myCurrentFile in casesForFiling)
                {
                    var file = selectedFiels.Where(x => x.FileGUID == myCurrentFile).FirstOrDefault();

                    table += "<tr><td style='border: 1px solid  #333; vertical-align: middle'>" + file.FileNumber + "</td></tr>";
                }
                table += "</table>";

                foreach (var myUser in filingUsers)
                {
                    var myFilingUserInfo = DbDAS.userPersonalDetailsLanguage.Where(x => x.UserGUID == myUser.UserGUID
                                            && x.LanguageID == LAN).FirstOrDefault();
                    var myFilingUserInfoAccount = DbDAS.userServiceHistory.Where(x => x.UserGUID == myUser.UserGUID).FirstOrDefault();
                    string SubjectMessage = resxEmails.DASRequestFilesFrom;
                    //foreach (var user in allUsers)
                    //{

                    URL = AppSettingsKeys.Domain + "/DAS/ScanDocument/ConfirmRequestPyhsicalFile/?id=" + new Portal().GUIDToString(filingUnitGUID);

                    //string myFirstName = myUser.FirstName;
                    //string mySurName = myUser.Surname;

                    Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.ClickToConfirm + "</a>";
                    Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";
                    string _message = resxEmails.DASRequestPysicalFile
                         .Replace("$FullName", myFilingUserInfo.FirstName + " " + myFilingUserInfo.Surname)
                          .Replace("$VerifyLink", Anchor)


                          .Replace("$requester", RequestedBy.FirstName + " " + RequestedBy.Surname)
                         .Replace("$RequestDate", nowDate.ToShortDateString())
                         .Replace("$files", table);
                    if (LAN == "AR") { _message = "<p align='right'>" + _message + "</p>"; }
                    int isRec = 1;
                    var myEmail = myFilingUserInfoAccount.EmailAddress;
                    SendMailGeneral(myEmail, SubjectMessage, _message, isRec);
                }


            }


        }
        //public void Send(string recipients, string subject, string body, int? isRec)
        //{
        //    string copy_recipients = "";
        //    string blind_copy_recipients = null;
        //    string body_format = "HTML";
        //    string importance = "Normal";
        //    string file_attachments = null;
        //    string _body = "<div style='font-family:Arial;'>" + body.Replace("\r\n", "<br/>") + "</div>";
        //    if (LAN == "AR") { _body = "<p align='right'>" + _body + "</p>"; }
        //    //DbCMS.SendEmail("maksoud@unhcr.org", copy_recipients, blind_copy_recipients, subject, _body, body_format, importance, file_attachments);
        //    DbCMS.SendEmail(recipients, copy_recipients, blind_copy_recipients, subject, _body, body_format, importance, file_attachments);
        //}
        public void SendMailGeneral(string recipients, string subject, string body, int? isRec)
        {
            string copy_recipients = "";
            string blind_copy_recipients = null;
            string body_format = "HTML";
            string importance = "Normal";
            string file_attachments = null;
            string _body = "<div style='font-family:Arial;'>" + body.Replace("\r\n", "<br/>") + "</div>";
            if (LAN == "AR") { _body = "<p align='right'>" + _body + "</p>"; }
           // DbCMS.SendEmailGeneral("maksoud@unhcr.org", copy_recipients, blind_copy_recipients, subject, "UNHCR Syria EFTS <Applications@unhcr.org> ", _body, body_format, importance, file_attachments);
            DbCMS.SendEmailGeneral(recipients, copy_recipients, blind_copy_recipients, subject, "UNHCR Syria EFTS <Applications@unhcr.org> ", _body, body_format, importance, file_attachments);
        }
        #endregion


        #region Confirm

        public ActionResult ConfirmReciveFileCreate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.RefugeeScannedDocument.Access, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return PartialView("~/Areas/DAS/Views/FileTransfer/_ConfirmReciveFile.cshtml",
                new ConfirmReturnFileModel { FileGUID = PK, Active = true });
        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ConfirmReciveFileCreate(ConfirmReturnFileModel model)
        {
            if (!CMS.HasAction(Permissions.RefugeeScannedDocument.Create, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            DateTime ExecutionTime = DateTime.Now;
            dataFile myModel = DbDAS.dataFile.Where(x => x.FileGUID == model.FileGUID).FirstOrDefault();
            dataScannDocumentTransferFlow flow = DbDAS.dataScannDocumentTransferFlow.Where(x => x.dataScannDocumentTransfer.dataFileRequest.FileGUID == model.FileGUID
            && x.IsLastAction == true).FirstOrDefault();

            if (flow.DocumentFlowStatusGUID == ScanDocumentTransferFlowStatus.Returned)
            {
                throw new HttpException(401, "Unauthorized access");
            }
            dataScannDocumentTransferFlow newFlow = new dataScannDocumentTransferFlow
            {
                ScannDocumentTransferFlowGUID = Guid.NewGuid(),
                ScannDocumentTransferGUID = flow.ScannDocumentTransferGUID,
                DocumentFlowStatusGUID = ScanDocumentTransferFlowStatus.Confirmed,
                IsLastAction = true,
                CreateByGUID = UserGUID,
                CreateDate = ExecutionTime

            };
            DbDAS.Create(newFlow, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);
            flow.IsLastAction = false;
            DbDAS.Update(flow, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);

            myModel.LastFlowStatusName = "Confirmed";
            myModel.LastFlowStatusGUID = ScanDocumentTransferFlowStatus.Confirmed;
            DbDAS.Update(myModel, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);
            try
            {

                DbDAS.SaveChanges();
                DbCMS.SaveChanges();
                return RedirectToAction("ConfirmRequestFiles", new { result = "Cofirmed" });

            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }



        }



        public ActionResult FileConfimrationReturnCreate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.RefugeeScannedDocument.Access, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            return PartialView("~/Areas/DAS/Views/FileTransfer/_ReturnFileConfimration.cshtml",
                new ConfirmReturnFileModel { FileGUID = PK, Active = true });
        }

        [HttpPost, ValidateAntiForgeryToken]

        public ActionResult FileConfimrationReturnCreate(ConfirmReturnFileModel model)
        {
            if (!CMS.HasAction(Permissions.RefugeeScannedDocument.Create, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            DateTime ExecutionTime = DateTime.Now;
            dataFile myModel = DbDAS.dataFile.Where(x => x.FileGUID == model.FileGUID).FirstOrDefault();
            dataScannDocumentTransferFlow flow = DbDAS.dataScannDocumentTransferFlow.Where(x => x.dataScannDocumentTransfer.dataFileRequest.FileGUID == model.FileGUID
            && x.IsLastAction == true).FirstOrDefault();
            dataScannDocumentTransfer myTransferflow = DbDAS.dataScannDocumentTransfer.Where(x => x.ScannDocumentTransferGUID == flow.ScannDocumentTransferGUID).FirstOrDefault();

            myTransferflow.ReceiveDate = ExecutionTime;
            myTransferflow.ReceiveByGUID = UserGUID;
            DbDAS.Update(myTransferflow, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);
            if (flow.DocumentFlowStatusGUID == ScanDocumentTransferFlowStatus.Returned)
            {
                throw new HttpException(401, "Unauthorized access");
            }
            // if (!ModelState.IsValid || ActiveDamagedModelFlow(model)) return PartialView("~/Areas/DAS/Views/ReleaseModelFlows/_ReleaseModelFlowUpdateModal.cshtml", model);

            dataScannDocumentTransferFlow newFlow = new dataScannDocumentTransferFlow
            {
                ScannDocumentTransferFlowGUID = Guid.NewGuid(),
                ScannDocumentTransferGUID = flow.ScannDocumentTransferGUID,
                DocumentFlowStatusGUID = ScanDocumentTransferFlowStatus.Returned,
                IsLastAction = true,
                CreateByGUID = UserGUID,
                CreateDate = model.FileReturnDate

            };
            DbDAS.Create(newFlow, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);
            flow.IsLastAction = false;
            DbDAS.Update(flow, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);
            myModel.LastCustodianType = "Unit";
            myModel.LastCustodianTypeName = "Filing Room";
            myModel.LastCustodianTypeGUID = DASDocumentCustodianType.UNIT;
            myModel.LastCustodianTypeNameGUID = DASDocumentUnitName.FilingUnit;
            myModel.LastFlowStatusName = "Returned";
            myModel.LastFlowStatusGUID = ScanDocumentTransferFlowStatus.Returned;
            DbDAS.Update(myModel, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);



            try
            {

                DbDAS.SaveChanges();
                DbCMS.SaveChanges();

                return Json(DbDAS.SingleUpdateMessage(DataTableNames.RefugeeScannedDocumentDataTable, DbDAS.PrimaryKeyControl(myModel), DbDAS.RowVersionControls(Portal.SingleToList(myModel))));
                //return Json(DbDAS.SingleUpdateMessage(DataTableNames.RefugeeScannedDocumentDataTable, DbDAS.PrimaryKeyControl(myModel), DbDAS.RowVersionControls(Portal.SingleToList(myModel))));
                // return Json(DbDAS.SingleCreateMessage(DbDAS.PrimaryKeyControl(myModel), DbDAS.RowVersionControls(myModel, myModel), null, "", null)); 
                // return View("~/Areas/DAS/Views/ModelConfirmation/ConfirmReceivingModelEmail.cshtml", new ConfirmReturnFileModel());

            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }



        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult RefugeeScannedDocumentDataTableReminderPendingConfirmationBulkFiles(List<FileDataTableModel> models)
        {

            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Create, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var unit = DbDAS.codeDASDestinationUnitFocalPoint.Where(x => x.UserGUID == UserGUID && x.Active==true).FirstOrDefault();
            Guid filingUnitGUID = Guid.Parse("bb915454-05fb-4e4d-b033-e01c7c790997");
            //if (unit.DestinationUnitGUID != filingUnitGUID)
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            var guids = models.Select(f => f.FileGUID).ToList();
            var myModels = DbDAS.dataScannDocumentTransferFlow.Where(x =>
                guids.Contains(x.dataScannDocumentTransfer.dataFileRequest.FileGUID.ToString())

                && x.IsLastAction == true
                && x.DocumentFlowStatusGUID == ScanDocumentTransferFlowStatus.Pending
                ).ToList();

            if (myModels.Count > 0)
            {

                foreach (var item in myModels)
                {
                    SendConfirmationReceivingFileEmail((Guid)item.dataScannDocumentTransfer.RequesterGUID, item.dataScannDocumentTransfer.dataFileRequest.FileGUID,
                              item.dataScannDocumentTransfer.ScannDocumentTransferGUID, 1);
                }

                var myModel = myModels.FirstOrDefault();


                return Json(DbDAS.SingleUpdateMessage(DataTableNames.ReferralLanguagesDataTable, DbDAS.PrimaryKeyControl(myModel), DbDAS.RowVersionControls(Portal.SingleToList(myModel))));
            }

            try
            {

                return Json(DbDAS.ErrorMessage("Warrning:Reminder will just send  for pending confirmation records"));

            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage("no Error"));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult RefugeeScannedDocumentDataTableReminderReturnBulkFiles(List<FileDataTableModel> models)
        {

            if (!CMS.HasAction(Permissions.WarehouseItemsEntry.Create, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var guids = models.Select(f => f.FileGUID).ToList();
            var myModels = DbDAS.dataScannDocumentTransferFlow.Where(x =>
                guids.Contains(x.dataScannDocumentTransfer.dataFileRequest.FileGUID.ToString())

                && x.IsLastAction == true
                && (x.DocumentFlowStatusGUID == ScanDocumentTransferFlowStatus.Pending || x.DocumentFlowStatusGUID == ScanDocumentTransferFlowStatus.Confirmed)
                && x.dataScannDocumentTransfer.RequesterTypeGUID == DASDocumentCustodianType.Staff).ToList();

            if (myModels.Count > 0)
            {

                foreach (var item in myModels)
                {
                    SendConfirmationReceivingFileEmail((Guid)item.dataScannDocumentTransfer.RequesterGUID, item.dataScannDocumentTransfer.dataFileRequest.FileGUID,
                              item.dataScannDocumentTransfer.ScannDocumentTransferGUID, 2);
                }

                var myModel = myModels.FirstOrDefault();


                return Json(DbDAS.SingleUpdateMessage(DataTableNames.ReferralLanguagesDataTable, DbDAS.PrimaryKeyControl(myModel), DbDAS.RowVersionControls(Portal.SingleToList(myModel))));
            }

            try
            {

                return Json(DbDAS.ErrorMessage("Warrning:Reminder will just send  for pending confirmation records"));

            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage("no Error"));
            }
        }



        #endregion
        #region Transfer Files

        [HttpGet]
        public ActionResult GetTransferBulkUserFiles()
        {
            return PartialView("~/Areas/DAS/Views/FileTransfer/_TransferUserBulkFiles.cshtml", new TransferFileModel { FileGUID = null });
        }

        [Route("DAS/ScanDocument/TransferBulkUserFiles/")]
        [HttpPost]

        public ActionResult TransferBulkUserFiles(List<Guid> models, TransferFileModel test)
        {
            int success = 0;
            if (models == null || models.Count() == 0) { return RedirectToAction("ConfirmRequestFiles", new { result = "NoFilesToTransfer" }); };
            DateTime ExecutionTime = DateTime.Now;
            List<Guid> scannDocumentTransferGUIDs = new List<Guid>();
            var unit = DbDAS.codeDASDestinationUnitFocalPoint.Where(x => x.UserGUID == test.RequesterGUID && x.Active==true).FirstOrDefault();

            var mySite = DbAHD.codeTablesValuesLanguages.Where(x => x.ValueGUID == unit.codeDASDestinationUnit.SiteOwnerGUID && x.LanguageID == LAN).FirstOrDefault();


            var transferLocation = DbDAS.codeDASTransferLocation.Where(x => x.TransferLocationGUID == test.TransferLocationGUID).FirstOrDefault();
            var mymodels = models.Select(x => x).Distinct();
            //check x
            foreach (var item in mymodels)
            {
                var myFile = DbDAS.dataFile.Where(x => x.FileGUID == item).FirstOrDefault();
                var lastCustName = myFile.LastCustodianTypeName;
                var lastCustNameGUID = myFile.LastCustodianTypeNameGUID;
                var LastTransfer = myFile.LastTransferFromName;

                dataFileRequest fileRequest = new dataFileRequest();
                if (myFile.LastRequesterNameGUID == null || (myFile.LastRequesterNameGUID != null && test.RequesterGUID != myFile.LastRequesterNameGUID))
                {
                    var priRequst = DbDAS.dataFileRequest.Where(x => x.FileGUID == item).ToList();
                    priRequst.ForEach(x => x.IsLastRequest = false);
                    string custName = "";
                    string typeName = "";
                    if (myFile.LastCustodianTypeNameGUID == null)
                    {
                        custName = "Filing";
                        typeName = "Unit";
                    }
                    else
                    {
                        var myStaff = DbDAS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN
                                && x.UserGUID == test.RequesterGUID).FirstOrDefault();
                        custName = myStaff.FirstName + " " + myStaff.Surname;
                        typeName = "Staff";
                    }

                    myFile.LastRequesterNameGUID = null;
                    myFile.LastRequesterName = null;
                    DbDAS.Update(myFile, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);

                    fileRequest = new dataFileRequest
                    {

                        FileRequestGUID = Guid.NewGuid(),
                        FileGUID = item,
                        RequesterTypeGUID = test.RequesterTypeGUID,
                        RequesterGUID = test.RequesterGUID,
                        RequestDate = test.RequestDate != null ? test.RequestDate : ExecutionTime,
                        RequestDurationDate = test.RequestDurationDate,
                        RequestStatusGUID = FileRequestStatus.Delivered,
                        IsLastRequest = true,
                        OrderId = priRequst.Select(x => x.OrderId).Max() + 1 ?? 1,
                        Comments = test.Comments,
                        RequsterFromNameGUID = myFile.LastCustodianTypeNameGUID,
                        RequsterFromName = myFile.LastCustodianTypeName,
                        RequsterName = custName,

                    };
                    DbDAS.Create(fileRequest, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);



                }
                else
                {
                    fileRequest = DbDAS.dataFileRequest.Where(x => x.FileGUID == item && x.IsLastRequest == true).FirstOrDefault();
                    if (fileRequest != null)
                    {
                        fileRequest.RequestStatusGUID = FileRequestStatus.Delivered;
                        fileRequest.IsLastRequest = false;

                        DbDAS.Update(fileRequest, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);
                    }
                }
                if (item != null)
                {
                    string custType = "";
                    string custName = "";
                    var oldTransfer = DbDAS.dataScannDocumentTransferFlow.Where(x => x.dataScannDocumentTransfer.dataFileRequest.FileGUID == item).ToList();
                    oldTransfer.ForEach(x => x.IsLastAction = false);

                    dataScannDocumentTransfer transfer = new dataScannDocumentTransfer
                    {
                        ScannDocumentTransferGUID = Guid.NewGuid(),
                        FileRequestGUID = fileRequest.FileRequestGUID,
                        RequesterTypeGUID = fileRequest.RequesterTypeGUID,
                        RequesterGUID = fileRequest.RequesterGUID,
                        TransferLocationGUID = test.TransferLocationGUID,
                        TransferDate = ExecutionTime,
                        CreateByGUID = UserGUID,
                        CreateDate = ExecutionTime,

                        OrderId = oldTransfer.Select(x => x.OrderId).Max() != null ? oldTransfer.Select(x => x.OrderId).Max() + 1 : 1,

                    };

                    dataScannDocumentTransferFlow flow = new dataScannDocumentTransferFlow
                    {
                        ScannDocumentTransferFlowGUID = Guid.NewGuid(),
                        ScannDocumentTransferGUID = transfer.ScannDocumentTransferGUID,
                        DocumentFlowStatusGUID = ScanDocumentTransferFlowStatus.Pending,
                        IsLastAction = true,
                        CreateByGUID = UserGUID,
                        CreateDate = ExecutionTime

                    };
                    var priLocations = DbDAS.dataFileLocationMovement.Where(x => x.FileGUID == item && x.IsLastAction == true).ToList();
                    priLocations.ForEach(f => f.IsLastAction = false);
                    DbDAS.UpdateBulk(priLocations, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);
                    if (test.TransferLocationGUID != null)
                    {
                        dataFileLocationMovement movement = new dataFileLocationMovement
                        {
                            FileLocationMovementGUID = Guid.NewGuid(),
                            FileGUID = item,
                            TransferLocationGUID = test.TransferLocationGUID,
                            IsLastAction = true,
                            ActionDate = ExecutionTime,
                            CreatedByGUID = UserGUID
                        };
                        DbDAS.Create(movement, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);
                    }

                    scannDocumentTransferGUIDs.Add(transfer.ScannDocumentTransferGUID);
                    DbDAS.Create(flow, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);
                    DbDAS.UpdateBulk(oldTransfer, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);
                    var file = DbDAS.dataFile.Where(x => x.FileGUID == item).FirstOrDefault();
                    //if (fileRequest.RequesterTypeGUID == DASDocumentCustodianType.Staff)
                    //{
                    var myStaff = DbDAS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN
                            && x.UserGUID == fileRequest.RequesterGUID).FirstOrDefault();
                    file.LastCustodianTypeName = myStaff.FirstName + " " + myStaff.Surname;
                    file.LastCustodianType = "Staff";
                    var myTransfer = DbDAS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN
                                                 && x.UserGUID == UserGUID).FirstOrDefault();
                    if (transferLocation != null)
                    {
                        file.LastTransferLocationName = transferLocation.TransferLocationName;
                        file.LastTransferLocationGUID = test.TransferLocationGUID;
                    }
                    else
                    {
                        file.LastTransferLocationName = null;
                        file.LastTransferLocationGUID = null;
                    }
                    file.LastRequesterName = null;
                    file.LastRequesterNameGUID = null;
                    file.LastDestinationUnitGUID = unit.DestinationUnitGUID;
                    file.LastUnitName = unit.codeDASDestinationUnit.DestinationUnitName;


                    custType = "Staff";
                    custName = myStaff.FirstName + " " + myStaff.Surname;
                    file.LastCustodianTypeGUID = DASDocumentCustodianType.Staff;
                    file.LastCustodianTypeNameGUID = fileRequest.RequesterGUID;
                    file.LastFlowStatusName = "Pending";
                    file.LastFlowStatusGUID = ScanDocumentTransferFlowStatus.Pending;
                    file.RequestStatusGUID = FileRequestStatus.Delivered;
                    file.RequestStatusName = "Delivered";
                    file.IsRequested = false;

                    file.LastTransferFromName = myTransfer.FirstName + " " + myTransfer.Surname;
                    file.LastTransferFromNameGUID = UserGUID;
                    file.SiteCode = mySite.ValueDescription;
                    file.SiteGUID = mySite.ValueGUID;

                    DbDAS.Update(file, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);

                    transfer.CustodianName = custName;
                    transfer.CustodianType = custType;
                    transfer.TransferFromName = myFile.LastCustodianTypeName;
                    DbDAS.Create(transfer, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);

                    try
                    {
                        DbDAS.SaveChanges();
                        DbCMS.SaveChanges();


                        //return Json(DbDAS.SingleUpdateMessage(DataTableNames.RefugeeScannedDocumentDataTable, DbDAS.PrimaryKeyControl(file), DbDAS.RowVersionControls(Portal.SingleToList(file))));
                        //return Json(DbDAS.SingleCreateMessage(DbDAS.PrimaryKeyControl(transfer), DbDAS.RowVersionControls(transfer, transfer), null, "", null));
                    }
                    catch (Exception e)
                    {
                        return Json(DbDAS.ErrorMessage(e.Message));
                    }
                }

            }
            if (models.Count > 0)
            {
                //if (test.RequesterTypeGUID != null && test.RequesterTypeGUID == DASDocumentCustodianType.Staff)
                //{
                SendConfirmationTransferBulkFilesEmail(test, scannDocumentTransferGUIDs, 1);
                //}
                return RedirectToAction("ConfirmRequestFiles", new { result = "Cofirmed" });
            }
            else
                return RedirectToAction("ConfirmRequestFiles", new { result = "Pending" });

        }

        [HttpGet]
        public ActionResult TransferBulkPhysicalFiles()
        {
            return PartialView("~/Areas/DAS/Views/FileTransfer/_TransferBulkFiles.cshtml", new TransferFileModel { FileGUID = null });
        }

        [Route("DAS/ScanDocument/TransferBulkPhysicalFile/")]
        [HttpPost]

        public ActionResult TransferBulkPhysicalFile(List<Guid> models, TransferFileModel test)
        {
            int success = 0;
            if (models == null || models.Count() == 0) { return RedirectToAction("ConfirmRequestFiles", new { result = "NoFilesToTransfer" }); };
            DateTime ExecutionTime = DateTime.Now;
            List<Guid> scannDocumentTransferGUIDs = new List<Guid>();
            var unit = DbDAS.codeDASDestinationUnitFocalPoint.Where(x => x.UserGUID == test.RequesterGUID && x.Active==true).FirstOrDefault();
            var mySite = DbAHD.codeTablesValuesLanguages.Where(x => x.ValueGUID == unit.codeDASDestinationUnit.SiteOwnerGUID && x.LanguageID == LAN).FirstOrDefault();
            var transferLocation = DbDAS.codeDASTransferLocation.Where(x => x.TransferLocationGUID == test.TransferLocationGUID).FirstOrDefault();
            //Guid filingUnitGUID = Guid.Parse("bb915454-05fb-4e4d-b033-e01c7c790997");
            //var isSupervisor = DbDAS.codeDASDestinationUnitFocalPoint.Where(x => x.UserGUID == UserGUID && x.IsSupervisor==true).FirstOrDefault().DestinationUnitFocalPointGUID;
            var userFilingGUID = DbDAS.codeDASDestinationUnitFocalPoint.Where(x => //x.DestinationUnitGUID == filingUnitGUID             &&
            x.UserGUID == UserGUID).FirstOrDefault().DestinationUnitFocalPointGUID;
            var myFileGUIDs = models.Select(x => x).Distinct();

            //var myModels = DbDAS.dataFile.Where(x => myFileGUIDs.Contains(x.FileGUID) && (x.LastCustodianTypeNameGUID == UserGUID || (x.LastCustodianTypeNameGUID == null && userFilingGUID != null))).Select(x => x.FileGUID).ToList();
            var myModels = DbDAS.dataFile.Where(x => myFileGUIDs.Contains(x.FileGUID) && (x.LastCustodianTypeNameGUID == UserGUID || (x.LastCustodianTypeNameGUID == null && userFilingGUID != null))).Select(x => x.FileGUID).ToList();
            var allfiles = DbDAS.dataFile.Where(x => myFileGUIDs.Contains(x.FileGUID)).ToList();
            var allRequests = DbDAS.dataFileRequest.Where(x => myFileGUIDs.Contains((Guid)x.FileGUID)).ToList();
            var myStaff = DbDAS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN
                   && x.UserGUID == UserGUID).FirstOrDefault();
            var allusers = DbDAS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.Active).ToList();
            var allflows = DbDAS.dataScannDocumentTransferFlow.Where(x => myFileGUIDs.Contains((Guid)x.dataScannDocumentTransfer.dataFileRequest.FileGUID)).ToList();
            var allmovements = DbDAS.dataFileLocationMovement.Where(x => myFileGUIDs.Contains((Guid)x.FileGUID)).ToList();

            List<dataFileRequest> allfileRequests = new List<dataFileRequest>();
            List<dataScannDocumentTransfer> alltransfers = new List<dataScannDocumentTransfer>();
            List<dataScannDocumentTransferFlow> transferflow = new List<dataScannDocumentTransferFlow>();
            List<dataFileLocationMovement> movements = new List<dataFileLocationMovement>();
            foreach (var item in myModels.Distinct())
            {
                var myFile = allfiles.Where(x => x.FileGUID == item).FirstOrDefault();
                var lastCustName = myFile.LastCustodianTypeName;
                var lastCustNameGUID = myFile.LastCustodianTypeNameGUID;
                var LastTransfer = myFile.LastTransferFromName;

                dataFileRequest fileRequest = new dataFileRequest();
                if (myFile.LastRequesterNameGUID == null || (myFile.LastRequesterNameGUID != null && test.RequesterGUID != myFile.LastRequesterNameGUID))
                {
                    var priRequst = allRequests.Where(x => x.FileGUID == item).ToList();

                    priRequst.ForEach(x => x.IsLastRequest = false);
                    string custName = "";
                    string typeName = "";
                    if (myFile.LastCustodianTypeNameGUID == null)
                    {

                        custName = myStaff.FirstName + " " + myStaff.Surname;
                        typeName = "Staff";

                    }
                    else
                    {
                        myStaff = allusers.Where(x => x.LanguageID == LAN && x.Active
                               && x.UserGUID == test.RequesterGUID).FirstOrDefault();
                        custName = myStaff.FirstName + " " + myStaff.Surname;
                        typeName = "Staff";
                    }

                    myFile.LastRequesterNameGUID = null;
                    myFile.LastRequesterName = null;
                    myFile.LastTransferFromNameGUID = UserGUID;
                    myFile.LastTransferFromName = custName;
                    myFile.SiteCode = mySite.ValueDescription;
                    myFile.SiteGUID = mySite.ValueGUID;
                    DbDAS.Update(myFile, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);

                    fileRequest = new dataFileRequest
                    {

                        FileRequestGUID = Guid.NewGuid(),
                        FileGUID = item,
                        RequesterTypeGUID = test.RequesterTypeGUID,
                        RequesterGUID = test.RequesterGUID,
                        RequestDate = test.RequestDate != null ? test.RequestDate : ExecutionTime,
                        RequestDurationDate = test.RequestDurationDate,
                        RequestStatusGUID = FileRequestStatus.Delivered,
                        IsLastRequest = true,
                        OrderId = priRequst.Select(x => x.OrderId).Max() + 1 ?? 1,
                        Comments = test.Comments,
                        RequsterFromNameGUID = myFile.LastCustodianTypeNameGUID,
                        RequsterFromName = myFile.LastCustodianTypeName,
                        RequsterName = custName,

                    };
                    allfileRequests.Add(fileRequest);




                }
                else
                {
                    fileRequest = allRequests.Where(x => x.FileGUID == item && x.IsLastRequest == true).FirstOrDefault();
                    if (fileRequest != null)
                    {
                        fileRequest.RequestStatusGUID = FileRequestStatus.Delivered;
                        fileRequest.IsLastRequest = false;

                        DbDAS.Update(fileRequest, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);
                    }
                }
                if (item != null)
                {
                    string custType = "";
                    string custName = "";
                    var oldTransfer = allflows.Where(x => x.dataScannDocumentTransfer.dataFileRequest.FileGUID == item).ToList();
                    oldTransfer.ForEach(x => x.IsLastAction = false);

                    dataScannDocumentTransfer transfer = new dataScannDocumentTransfer
                    {
                        ScannDocumentTransferGUID = Guid.NewGuid(),
                        FileRequestGUID = fileRequest.FileRequestGUID,
                        RequesterTypeGUID = fileRequest.RequesterTypeGUID,
                        RequesterGUID = fileRequest.RequesterGUID,
                        TransferLocationGUID = test.TransferLocationGUID,
                        TransferDate = ExecutionTime,
                        CreateByGUID = UserGUID,
                        CreateDate = ExecutionTime,

                        OrderId = oldTransfer.Select(x => x.OrderId).Max() != null ? oldTransfer.Select(x => x.OrderId).Max() + 1 : 1,

                    };

                    dataScannDocumentTransferFlow flow = new dataScannDocumentTransferFlow
                    {
                        ScannDocumentTransferFlowGUID = Guid.NewGuid(),
                        ScannDocumentTransferGUID = transfer.ScannDocumentTransferGUID,
                        DocumentFlowStatusGUID = ScanDocumentTransferFlowStatus.Pending,
                        IsLastAction = true,
                        CreateByGUID = UserGUID,
                        CreateDate = ExecutionTime

                    };
                    var priLocations = allmovements.Where(x => x.FileGUID == item && x.IsLastAction == true).ToList();
                    priLocations.ForEach(f => f.IsLastAction = false);
                    DbDAS.UpdateBulk(priLocations, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);
                    if (test.TransferLocationGUID != null)
                    {
                        dataFileLocationMovement movement = new dataFileLocationMovement
                        {
                            FileLocationMovementGUID = Guid.NewGuid(),
                            FileGUID = item,
                            TransferLocationGUID = test.TransferLocationGUID,
                            IsLastAction = true,
                            ActionDate = ExecutionTime,
                            CreatedByGUID = UserGUID
                        };
                        movements.Add(movement);

                    }

                    scannDocumentTransferGUIDs.Add(transfer.ScannDocumentTransferGUID);
                    transferflow.Add(flow);

                    DbDAS.UpdateBulk(oldTransfer, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);
                    var file = DbDAS.dataFile.Where(x => x.FileGUID == item).FirstOrDefault();
                    //if (fileRequest.RequesterTypeGUID == DASDocumentCustodianType.Staff)
                    //{
                    myStaff = allusers.Where(x => x.LanguageID == LAN && x.UserGUID == fileRequest.RequesterGUID).FirstOrDefault();
                    file.LastCustodianTypeName = myStaff.FirstName + " " + myStaff.Surname;
                    file.LastCustodianType = "Staff";

                    if (transferLocation != null)
                    {
                        file.LastTransferLocationName = transferLocation.TransferLocationName;
                        file.LastTransferLocationGUID = test.TransferLocationGUID;
                    }
                    else
                    {
                        file.LastTransferLocationName = null;
                        file.LastTransferLocationGUID = null;
                    }
                    file.LastRequesterName = null;
                    file.LastRequesterNameGUID = null;
                    file.LastDestinationUnitGUID = unit.DestinationUnitGUID;
                    file.LastUnitName = unit.codeDASDestinationUnit.DestinationUnitName;


                    custType = "Staff";
                    custName = myStaff.FirstName + " " + myStaff.Surname;
                    file.LastCustodianTypeGUID = DASDocumentCustodianType.Staff;
                    file.LastCustodianTypeNameGUID = test.RequesterGUID;
                    file.LastFlowStatusName = "Pending";
                    file.LastFlowStatusGUID = ScanDocumentTransferFlowStatus.Pending;
                    file.RequestStatusGUID = FileRequestStatus.Delivered;
                    file.RequestStatusName = "Delivered";
                    file.IsRequested = false;



                    var myTransfer = allusers.Where(x => x.LanguageID == LAN && x.UserGUID == UserGUID).FirstOrDefault();
                    file.LastTransferFromName = myTransfer.FirstName + " " + myTransfer.Surname;
                    file.LastTransferFromNameGUID = UserGUID;
                    //file.LastTransferFromName = lastCustName;
                    //    file.LastTransferFromNameGUID = lastCustNameGUID;

                    DbDAS.Update(file, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);

                    transfer.CustodianName = custName;
                    transfer.CustodianType = custType;
                    transfer.TransferFromName = myFile.LastCustodianTypeName;
                    alltransfers.Add(transfer);



                }

            }

            DbDAS.CreateBulk(allfileRequests, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);
            DbDAS.CreateBulk(allfileRequests, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);
            DbDAS.CreateBulk(movements, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);
            DbDAS.CreateBulk(transferflow, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);
            DbDAS.CreateBulk(alltransfers, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);
            try
            {
                DbDAS.SaveChanges();
                DbCMS.SaveChanges();


                //return Json(DbDAS.SingleUpdateMessage(DataTableNames.RefugeeScannedDocumentDataTable, DbDAS.PrimaryKeyControl(file), DbDAS.RowVersionControls(Portal.SingleToList(file))));
                //return Json(DbDAS.SingleCreateMessage(DbDAS.PrimaryKeyControl(transfer), DbDAS.RowVersionControls(transfer, transfer), null, "", null));
            }
            catch (Exception e)
            {
                return RedirectToAction("ConfirmRequestFiles", new { result = "NoFilesToTransfer" });
            }
            if (models.Count > 0)
            {
                //if (test.RequesterTypeGUID != null && test.RequesterTypeGUID == DASDocumentCustodianType.Staff)
                //{
                SendConfirmationTransferBulkFilesEmail(test, scannDocumentTransferGUIDs, 1);
                //}
                return RedirectToAction("ConfirmRequestFiles", new { result = "Cofirmed" });
            }
            else
                return RedirectToAction("ConfirmRequestFiles", new { result = "NoFilesToTransfer" });

        }

        [HttpGet]
        public ActionResult TransferBulkPhysicalFilesFromUpload()
        {
            return PartialView("~/Areas/DAS/Views/FileTransfer/_TransferBulkFilesFromUpload.cshtml", new TransferFileModel { FileGUID = null });
        }


        [Route("DAS/ScanDocument/TransferBulkPhysicalFileFromUpload/")]
        [HttpPost]

        public ActionResult TransferBulkPhysicalFileFromUpload(List<Guid> models, TransferFileModel test)
        {
            int success = 0;
            if (models == null || models.Count == 0) { return RedirectToAction("ConfirmRequestFiles", new { result = "NoFilesToTransfer" }); }
            DateTime ExecutionTime = DateTime.Now;
            List<Guid> scannDocumentTransferGUIDs = new List<Guid>();
            // Guid filingUnitGUID = Guid.Parse("bb915454-05fb-4e4d-b033-e01c7c790997");
            var unit = DbDAS.codeDASDestinationUnitFocalPoint.Where(x => x.UserGUID == test.RequesterGUID && x.Active==true).FirstOrDefault();

            var mySite = DbAHD.codeTablesValuesLanguages.Where(x => x.ValueGUID == unit.codeDASDestinationUnit.SiteOwnerGUID && x.LanguageID == LAN).FirstOrDefault();
            var transferLocation = DbDAS.codeDASTransferLocation.Where(x => x.TransferLocationGUID == test.TransferLocationGUID).FirstOrDefault();
            var userFilingGUID = DbDAS.codeDASDestinationUnitFocalPoint.Where(x => x.UserGUID == UserGUID && x.Active==true).FirstOrDefault().DestinationUnitFocalPointGUID;
            var modelsGUIDs = models.Select(x => x).Distinct();
            var myFileGUIDs = DbDAS.dataUploadUserFile.Where(x => modelsGUIDs.Contains(x.UploadUserFileGUID)).Select(a => a.FileGUID).ToList();
            var myModels = DbDAS.dataFile.Where(x => myFileGUIDs.Contains(x.FileGUID) && (x.LastCustodianTypeNameGUID == UserGUID || (x.LastCustodianTypeNameGUID == null && userFilingGUID != null))).Select(x => x.FileGUID).ToList();
            foreach (var myfileGUID in myModels)
            {
                //var userFile = DbDAS.dataUploadUserFile.Where(x => x.UploadUserFileGUID == item).FirstOrDefault();
                var myFile = DbDAS.dataFile.Where(x => x.FileGUID == myfileGUID).FirstOrDefault();
                dataFileRequest fileRequest = new dataFileRequest();
                if (myFile.LastRequesterNameGUID == null || (myFile.LastRequesterNameGUID != null && test.RequesterGUID != myFile.LastRequesterNameGUID))
                {
                    var priRequst = DbDAS.dataFileRequest.Where(x => x.FileGUID == myFile.FileGUID).ToList();

                    priRequst.ForEach(x => x.IsLastRequest = false);
                    string custName = "";
                    string typeName = "";
                    if (myFile.LastCustodianTypeNameGUID == null)
                    {
                        var myStaff = DbDAS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN
                              && x.UserGUID == UserGUID).FirstOrDefault();
                        custName = myStaff.FirstName + " " + myStaff.Surname;
                        typeName = "Staff";

                    }
                    else
                    {
                        var myStaff = DbDAS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN
                                && x.UserGUID == test.RequesterGUID).FirstOrDefault();
                        custName = myStaff.FirstName + " " + myStaff.Surname;
                        typeName = "Staff";
                    }
                    fileRequest = new dataFileRequest
                    {
                        FileRequestGUID = Guid.NewGuid(),
                        FileGUID = myFile.FileGUID,
                        RequesterTypeGUID = test.RequesterTypeGUID,
                        RequesterGUID = test.RequesterGUID,
                        RequestDate = test.RequestDate != null ? test.RequestDate : ExecutionTime,
                        RequestDurationDate = test.RequestDurationDate,
                        RequestStatusGUID = FileRequestStatus.Delivered,
                        IsLastRequest = true,
                        OrderId = priRequst.Select(x => x.OrderId).Max() + 1 ?? 1,
                        Comments = test.Comments,
                        RequsterFromNameGUID = myFile.LastCustodianTypeNameGUID,
                        RequsterFromName = myFile.LastCustodianTypeName,
                        RequsterName = custName,

                    };
                    DbDAS.Create(fileRequest, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);
                }
                else
                {
                    fileRequest = DbDAS.dataFileRequest.Where(x => x.FileGUID == myFile.FileGUID && x.IsLastRequest == true).FirstOrDefault();
                    fileRequest.RequestStatusGUID = FileRequestStatus.Delivered;
                    DbDAS.Update(fileRequest, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);
                }
                if (myFile.FileGUID != null)
                {
                    string custType = "";
                    string custName = "";
                    var oldTransfer = DbDAS.dataScannDocumentTransferFlow.Where(x => x.dataScannDocumentTransfer.dataFileRequest.FileGUID == myFile.FileGUID).ToList();
                    oldTransfer.ForEach(x => x.IsLastAction = false);
                    dataScannDocumentTransfer transfer = new dataScannDocumentTransfer
                    {
                        ScannDocumentTransferGUID = Guid.NewGuid(),
                        FileRequestGUID = fileRequest.FileRequestGUID,
                        RequesterTypeGUID = fileRequest.RequesterTypeGUID,
                        RequesterGUID = fileRequest.RequesterGUID,
                        TransferDate = ExecutionTime,
                        CreateByGUID = UserGUID,
                        CreateDate = ExecutionTime,

                        OrderId = oldTransfer.Select(x => x.OrderId).Max() != null ? oldTransfer.Select(x => x.OrderId).Max() + 1 : 1,

                    };

                    dataScannDocumentTransferFlow flow = new dataScannDocumentTransferFlow
                    {
                        ScannDocumentTransferFlowGUID = Guid.NewGuid(),
                        ScannDocumentTransferGUID = transfer.ScannDocumentTransferGUID,
                        DocumentFlowStatusGUID = ScanDocumentTransferFlowStatus.Pending,
                        IsLastAction = true,
                        CreateByGUID = UserGUID,
                        CreateDate = ExecutionTime

                    };


                    var priLocations = DbDAS.dataFileLocationMovement.Where(x => x.FileGUID == myfileGUID && x.IsLastAction == true).ToList();
                    priLocations.ForEach(f => f.IsLastAction = false);
                    DbDAS.UpdateBulk(priLocations, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);

                    if (test.TransferLocationGUID != null)
                    {
                        dataFileLocationMovement movement = new dataFileLocationMovement
                        {
                            FileLocationMovementGUID = Guid.NewGuid(),
                            FileGUID = myfileGUID,
                            TransferLocationGUID = test.TransferLocationGUID,
                            IsLastAction = true,
                            ActionDate = ExecutionTime,
                            CreatedByGUID = UserGUID
                        };
                        DbDAS.Create(movement, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);
                    }

                    scannDocumentTransferGUIDs.Add(transfer.ScannDocumentTransferGUID);
                    DbDAS.Create(flow, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);
                    DbDAS.UpdateBulk(oldTransfer, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);
                    var file = DbDAS.dataFile.Where(x => x.FileGUID == myFile.FileGUID).FirstOrDefault();
                    //if (fileRequest.RequesterTypeGUID == DASDocumentCustodianType.Staff)
                    //{
                    var myStaff = DbDAS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.UserGUID == fileRequest.RequesterGUID).FirstOrDefault();
                    file.LastCustodianTypeName = myStaff.FirstName + " " + myStaff.Surname;
                    file.LastCustodianType = "Staff";

                    var lastTransfer = DbDAS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.UserGUID == UserGUID).FirstOrDefault();

                    file.LastTransferLocationGUID = test.TransferLocationGUID;
                    file.LastTransferLocationName = transferLocation != null ? transferLocation.TransferLocationName : null;
                    custType = "Staff";
                    custName = myStaff.FirstName + " " + myStaff.Surname;
                    file.LastCustodianTypeGUID = fileRequest.RequesterTypeGUID;
                    file.LastCustodianTypeNameGUID = fileRequest.RequesterGUID;
                    file.LastFlowStatusName = "Pending";
                    file.LastFlowStatusGUID = ScanDocumentTransferFlowStatus.Pending;
                    file.RequestStatusGUID = FileRequestStatus.Delivered;
                    file.RequestStatusName = "Delivered";
                    file.IsRequested = false;
                    file.LastDestinationUnitGUID = unit.DestinationUnitGUID;
                    file.LastUnitName = unit.codeDASDestinationUnit.DestinationUnitName;

                    file.SiteCode = mySite.ValueDescription;
                    file.SiteGUID = mySite.ValueGUID;

                    file.LastTransferFromName = lastTransfer.FirstName + " " + lastTransfer.Surname;
                    file.LastTransferFromNameGUID = lastTransfer.UserGUID;

                    DbDAS.Update(file, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);
                    //}
                    //else if (fileRequest.RequesterTypeGUID == DASDocumentCustodianType.UNIT)
                    //{
                    //    var unit = DbDAS.codeDASDestinationUnit.Where(x => x.DestinationUnitGUID == fileRequest.RequesterGUID).FirstOrDefault();
                    //    file.LastCustodianTypeName = unit.DestinationUnitName;
                    //    file.LastCustodianType = "Unit";
                    //    file.LastTransferLocationGUID = test.TransferLocationGUID;
                    //    file.LastTransferLocationName = transferLocation.TransferLocationName;
                    //    file.LastFlowStatusName = "Pending";
                    //    custType = "Unit";
                    //    custName = unit.DestinationUnitName;
                    //    file.LastCustodianTypeGUID = fileRequest.RequesterTypeGUID;
                    //    file.LastCustodianTypeNameGUID = fileRequest.RequesterGUID;
                    //    file.LastFlowStatusGUID = ScanDocumentTransferFlowStatus.Pending;
                    //    file.RequestStatusGUID = FileRequestStatus.Delivered;
                    //    file.RequestStatusName = "Delivered";
                    //    file.IsRequested = false;
                    //    file.LastTransferFromName = myFile.LastCustodianTypeName;
                    //    file.LastTransferFromNameGUID = myFile.LastCustodianTypeNameGUID;
                    //    DbDAS.Update(file, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);

                    //}
                    transfer.CustodianName = custName;
                    transfer.CustodianType = custType;
                    transfer.TransferFromName = myFile.LastCustodianTypeName;



                    DbDAS.Create(transfer, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);

                    try
                    {
                        DbDAS.SaveChanges();
                        DbCMS.SaveChanges();


                        //return Json(DbDAS.SingleUpdateMessage(DataTableNames.RefugeeScannedDocumentDataTable, DbDAS.PrimaryKeyControl(file), DbDAS.RowVersionControls(Portal.SingleToList(file))));
                        //return Json(DbDAS.SingleCreateMessage(DbDAS.PrimaryKeyControl(transfer), DbDAS.RowVersionControls(transfer, transfer), null, "", null));
                    }
                    catch (Exception e)
                    {
                        return Json(DbDAS.ErrorMessage(e.Message));
                    }
                }

            }
            if (myModels.Count > 0)
            {

                SendConfirmationTransferBulkFilesEmail(test, scannDocumentTransferGUIDs, 1);

                return RedirectToAction("ConfirmRequestFiles", new { result = "Cofirmed" });
            }
            else
                return RedirectToAction("ConfirmRequestFiles", new { result = "NoFilesToTransfer" });

        }
        #endregion
        #region Request physial files
        [HttpGet]
        public ActionResult GetRequestBulkPhysicalFiles()
        {
            return PartialView("~/Areas/DAS/Views/FileRequest/_BulkFileRequest.cshtml", new FileRequestModel { FileGUID = null });
        }
        [HttpGet]
        public ActionResult GetRequestBulkPhysicalFilesFromUpload()
        {
            return PartialView("~/Areas/DAS/Views/FileRequest/_BulkFileRequestFromUpload.cshtml", new FileRequestModel { FileGUID = null });
        }
        [HttpGet]
        public ActionResult RequestBulkPhysicalFiles()
        {
            return PartialView("~/Areas/DAS/Views/FileRequest/_BulkFileRequest.cshtml", new FileRequestModel { FileGUID = null });
        }
        [Route("DAS/ScanDocument/RequestBulkPhysicalFilesFromUpload/")]

        [HttpPost]
        public ActionResult RefugeeScannedDocumentRequestBulkPhysicalFilesFromUpload(List<Guid> models, FileRequestModel test)
        {
            if (!CMS.HasAction(Permissions.RefugeeScannedDocument.Create, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            if (models.Count > 0)
            {

                var guids = DbDAS.dataUploadUserFile.Where(x => models.Contains(x.UploadUserFileGUID)).Select(x => x.FileGUID).ToList();
                var fileRequstGUIDs = DbDAS.dataFileRequest.Where(x => guids.Contains((Guid)x.FileGUID)
                && (x.dataFile.IsRequested == true)
                  ).Select(x => x.FileGUID).ToList();
                var myModels = DbDAS.dataFile.Where(x =>
                    guids.Contains(x.FileGUID)
                    && (x.LastCustodianTypeNameGUID != UserGUID || x.LastTransferFromNameGUID == null)
                    && !fileRequstGUIDs.Contains(x.FileGUID)).ToList();
                string custName = "";
                string typeName = "";

                var myStaff = DbDAS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN
                && x.UserGUID == UserGUID).FirstOrDefault();
                custName = myStaff.FirstName + " " + myStaff.Surname;
                typeName = "Staff";



                var myFiles = DbDAS.dataFile.Where(x => guids.Contains(x.FileGUID)).ToList();
                var myFileGUIDs = myModels.Select(x => x.FileGUID).ToList();
                DateTime ExecutionTime = DateTime.Now;
                if (myModels.Count > 0)
                {
                    List<dataFileRequest> ToAddrequests = new List<dataFileRequest>();
                    #region Retertive items
                    foreach (var myModel in myModels)
                    {
                        var priRequst = DbDAS.dataFileRequest.Where(x => x.FileGUID == myModel.FileGUID).ToList();

                        priRequst.ForEach(x => x.IsLastRequest = false);
                        var myCurrentFile = DbDAS.dataFile.Where(x => x.FileGUID == myModel.FileGUID).FirstOrDefault();
                        myCurrentFile.IsRequested = true;
                        myCurrentFile.RequestStatusGUID = FileRequestStatus.Requested;
                        myCurrentFile.RequestStatusName = "Requested";
                        myCurrentFile.LastRequesterName = custName;
                        myCurrentFile.LastRequesterNameGUID = UserGUID;
                        DbDAS.Update(myCurrentFile, Permissions.RefugeeScannedDocument.UpdateGuid, ExecutionTime, DbCMS);


                        dataFileRequest myRequest = new dataFileRequest
                        {
                            FileRequestGUID = Guid.NewGuid(),
                            FileGUID = myModel.FileGUID,

                            RequesterTypeGUID = DASDocumentCustodianType.Staff,
                            RequesterGUID = UserGUID,
                            RequestDate = test.RequestDate,
                            RequestDurationDate = test.RequestDurationDate,
                            RequestStatusGUID = FileRequestStatus.Requested,
                            IsLastRequest = true,
                            OrderId = priRequst.Select(x => x.OrderId).Max() + 1 ?? 1,
                            Comments = test.Comments,
                            RequsterFromNameGUID = myFiles.Where(x => x.FileGUID == myModel.FileGUID).Select(x => x.LastRequesterNameGUID).FirstOrDefault(),
                            RequsterFromName = myFiles.Where(x => x.FileGUID == myModel.FileGUID).Select(x => x.LastCustodianTypeName).FirstOrDefault(),
                            RequsterName = custName,


                        };
                        ToAddrequests.Add(myRequest);
                        DbDAS.Create(myRequest, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);

                        try
                        {
                            DbDAS.SaveChanges();
                            DbCMS.SaveChanges();






                        }
                        catch (Exception e)
                        {

                            throw;
                        }

                    }

                    #endregion
                    SendConfirmationRequestingFileEmail(ToAddrequests);
                    return RedirectToAction("ConfirmRequestFiles", new { result = "Cofirmed" });
                }

            }
            return RedirectToAction("ConfirmRequestFiles", new { result = "Pending" });

        }

        [Route("DAS/ScanDocument/RequestBulkPhysicalFiles/")]

        [HttpPost]
        public ActionResult RefugeeScannedDocumentRequestBulkPhysicalFiles(List<Guid> models, FileRequestModel test)
        {
            if (!CMS.HasAction(Permissions.RefugeeScannedDocument.Create, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            if (models.Count > 0)
            {
                var guids = models;

                var fileRequstGUIDs = DbDAS.dataFileRequest.Where(x => guids.Contains((Guid)x.FileGUID)
                && (x.dataFile.IsRequested == true)
                  ).Select(x => x.FileGUID).ToList();
                var myModels = DbDAS.dataFile.Where(x =>
                    guids.Contains(x.FileGUID)
                    && (x.LastCustodianTypeNameGUID != UserGUID || x.LastTransferFromNameGUID == null)
                    && !fileRequstGUIDs.Contains(x.FileGUID)).ToList();
                string custName = "";
                string typeName = "";

                var myStaff = DbDAS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN
                && x.UserGUID == UserGUID).FirstOrDefault();
                custName = myStaff.FirstName + " " + myStaff.Surname;
                typeName = "Staff";



                var myFiles = DbDAS.dataFile.Where(x => guids.Contains(x.FileGUID)).ToList();
                var myFileGUIDs = myModels.Select(x => x.FileGUID).ToList();
                DateTime ExecutionTime = DateTime.Now;
                if (myModels.Count > 0)
                {
                    List<dataFileRequest> ToAddrequests = new List<dataFileRequest>();
                    #region Retertive items
                    foreach (var myModel in myModels)
                    {
                        var priRequst = DbDAS.dataFileRequest.Where(x => x.FileGUID == myModel.FileGUID).ToList();

                        priRequst.ForEach(x => x.IsLastRequest = false);
                        var myCurrentFile = DbDAS.dataFile.Where(x => x.FileGUID == myModel.FileGUID).FirstOrDefault();
                        myCurrentFile.IsRequested = true;
                        myCurrentFile.RequestStatusGUID = FileRequestStatus.Requested;
                        myCurrentFile.RequestStatusName = "Requested";
                        myCurrentFile.LastRequesterName = custName;
                        myCurrentFile.LastRequesterNameGUID = UserGUID;
                        DbDAS.Update(myCurrentFile, Permissions.RefugeeScannedDocument.UpdateGuid, ExecutionTime, DbCMS);


                        dataFileRequest myRequest = new dataFileRequest
                        {
                            FileRequestGUID = Guid.NewGuid(),
                            FileGUID = myModel.FileGUID,

                            RequesterTypeGUID = DASDocumentCustodianType.Staff,
                            RequesterGUID = UserGUID,
                            RequestDate = test.RequestDate,
                            RequestDurationDate = test.RequestDurationDate,
                            RequestStatusGUID = FileRequestStatus.Requested,
                            IsLastRequest = true,
                            OrderId = priRequst.Select(x => x.OrderId).Max() + 1 ?? 1,
                            Comments = test.Comments,
                            RequsterFromNameGUID = myFiles.Where(x => x.FileGUID == myModel.FileGUID).Select(x => x.LastRequesterNameGUID).FirstOrDefault(),
                            RequsterFromName = myFiles.Where(x => x.FileGUID == myModel.FileGUID).Select(x => x.LastCustodianTypeName).FirstOrDefault(),
                            RequsterName = custName,


                        };
                        ToAddrequests.Add(myRequest);
                        DbDAS.Create(myRequest, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);

                        try
                        {
                            DbDAS.SaveChanges();
                            DbCMS.SaveChanges();






                        }
                        catch (Exception e)
                        {

                            throw;
                        }

                    }

                    #endregion
                    SendConfirmationRequestingFileEmail(ToAddrequests);
                    return RedirectToAction("ConfirmRequestFiles", new { result = "Cofirmed" });
                }

            }
            return RedirectToAction("ConfirmRequestFiles", new { result = "Pending" });

        }

        public ActionResult ConfirmRequestFiles(string result)
        {
            return View("~/Areas/DAS/Views/FileConfirmation/ConfirmRequestFiles.cshtml", new FileRequestConfirmationModel { ConfirmationStatus = result });

        }
        #endregion

        #region Upload Cases
        #region Upload 
        public ActionResult UploadDocumentIndex()
        {
            if (!CMS.HasAction(Permissions.RefugeeScannedDocument.Access, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            Guid filingUnitGUID = Guid.Parse("bb915454-05fb-4e4d-b033-e01c7c790997");
            var unit = DbDAS.codeDASDestinationUnitFocalPoint.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
            if (unit.DestinationUnitGUID == filingUnitGUID)
            {
                ViewBag.IsFiling = 1;
            }
            else
                ViewBag.IsFiling = 0;
            return View("~/Areas/DAS/Views/RefugeeScannedDocument/UploadDocumentIndex.cshtml");
        }
        [HttpGet]
        public ActionResult UploadFiles()
        {
            return PartialView("~/Areas/DAS/Views/FileManager/_BulkFileUploadUpload.cshtml",
                new dataFile());
        }
        [HttpPost]
        public FineUploaderResult UploadFiles(FineUpload upload)
        {

            return new FineUploaderResult(true, new { path = Upload(upload), success = true });
        }

        public ActionResult Upload(FineUpload upload)
        {
            var _stearm = upload.InputStream;
            DateTime ExecutionTime = DateTime.Now;
            string _ext = Path.GetExtension(upload.FileName).Split('.')[1];
            dataFile file = new dataFile();

            file.FileGUID = Guid.NewGuid();
            string FilePath = Server.MapPath("~/Uploads/DAS/temp/" + file.FileGUID + ".xlsx");
            //Server.MapPath("~/Areas/WMS/temp/Standard_Operating_Procedure_for_issuing_ICT_equipment_to_staff" + DateTime.Now.ToBinary() + ".pdf");

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
                    List<dataUploadUserFile> files = new List<dataUploadUserFile>();
                    bool ok = Validate(workSheet);
                    package.Save();
                    if (ok)
                    {
                        int totalRows = workSheet.Dimension.End.Row;
                        List<dataUploadUserFile> inputBalances = new List<dataUploadUserFile>();
                        var allfiles = DbDAS.dataFile.AsQueryable();
                        var toRemove = DbDAS.dataUploadUserFile.Where(a => a.UserGUID == UserGUID).ToList();
                        DbDAS.dataUploadUserFile.RemoveRange(toRemove);
                        try
                        {
                            DbDAS.SaveChanges();
                            DbCMS.SaveChanges();
                        }
                        catch (Exception)
                        {

                            throw;
                        }

                        for (int i = 2; i <= totalRows; i++)
                        {
                            dataUploadUserFile ToAddInput = new dataUploadUserFile();
                            var fileNumber = workSheet.Cells["A" + i].Value;

                            if (fileNumber == null)
                                break;
                            var currentFile = allfiles.Where(a => a.FileNumber == fileNumber).FirstOrDefault();
                            var _checkFile = files.Where(a => a.FileGUID == currentFile.FileGUID).FirstOrDefault();
                            if (_checkFile != null)
                                continue;
                            dataUploadUserFile myFile = new dataUploadUserFile
                            {
                                UploadUserFileGUID = Guid.NewGuid(),
                                FileGUID = currentFile.FileGUID,
                                UserGUID = UserGUID,
                                Active = true

                            };
                            files.Add(myFile);

                        }
                        DbDAS.CreateBulk(files, Permissions.RefugeeScannedDocument.UpdateGuid, DateTime.Now, DbCMS);

                        try
                        {
                            DbDAS.SaveChanges();
                            DbCMS.SaveChanges();

                        }
                        catch (Exception ex)
                        {
                            var error = DbDAS.ErrorMessage(ex.Message);
                        }
                    }
                }
            }
            return Json("~/Uploads/DAS/temp/" + file.FileGUID + ".xlsx", JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// Validate th Excel 
        /// </summary>
        /// <param name="workSheet"></param>
        /// <returns></returns>
        private bool Validate(ExcelWorksheet workSheet)
        {

            int totalRows = workSheet.Dimension.End.Row;
            bool valid = true;

            var files = DbDAS.dataFile.AsQueryable();
            for (int i = 2; i < totalRows; i++)
            {
                var myFileNumber = workSheet.Cells["A" + i].Value;
                if (myFileNumber == null)
                    break;





                var _checkmyFileNumber = files.Where(x => x.FileNumber.ToString().ToLower() == myFileNumber.ToString().ToLower()).FirstOrDefault();


                if ((_checkmyFileNumber == null))
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
        #endregion
        #region Upload Cases Check
        [Route("DAS/RefugeeUploadScannedDocumentDataTable/")]
        public JsonResult RefugeeUploadScannedDocumentDataTable(DataTableRecievedOptions options)
        {

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<FileDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {

                Predicate = SearchHelper.CreateSearchPredicate<FileDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.RefugeeScannedDocument.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList();
            //Fix
            //Guid filingUnitGUID = Guid.Parse("bb915454-05fb-4e4d-b033-e01c7c790997");
            var filingUnitUserGUID = DbDAS.codeDASDestinationUnitFocalPoint.Where(f => f.UserGUID == UserGUID
            // && f.DestinationUnitGUID == filingUnitGUID
            )
                .Select(f => f.UserGUID).FirstOrDefault();

            var UserUnit = DbDAS.codeDASDestinationUnitFocalPoint.Where(f => f.UserGUID == UserGUID);
            var UserUnitGUID = UserUnit.Select(f => f.DestinationUnitGUID).FirstOrDefault();
            //var filingUnitUserGUID = UserUnit.Where(f =>  f.DestinationUnitGUID == filingUnitGUID)
            //  .Select(f => f.UserGUID).FirstOrDefault();

            var All = (

                from a in DbDAS.dataFile.Where(x => x.Active).AsQueryable().AsExpandable()
                join b in DbDAS.dataUploadUserFile.Where(x => x.UserGUID == UserGUID) on a.FileGUID equals b.FileGUID

                select new FileDataTableModel
                {
                    UploadUserFileGUID = b.UploadUserFileGUID.ToString(),
                    FileGUID = a.FileGUID.ToString(),
                    ProcessStatusName = a.ProcessStatusName,
                    CaseSize = a.CaseSize,
                    RefugeeStatus = a.RefugeeStatus,
                    OrigionCountry = a.OrigionCountry,
                    Active = a.Active,
                    SiteCode = a.SiteCode,
                    SiteGUID = a.SiteGUID.ToString(),
                    SiteOwner = a.SiteOwnerName,
                    FileNumber = a.FileNumber,
                    LastTransferFromNameGUID = a.LastTransferFromNameGUID.ToString(),
                    LastTransferFromName = a.LastTransferFromName,
                    LastCustodianTypeGUID = a.LastCustodianTypeGUID.ToString(),
                    FileMergeStatusGUID = a.FileMergeStatusGUID.ToString(),
                    FileMergeStatus = a.FileMergeStatus,
                    //LastCustodianType = a.LastCustodianType,
                    CurrentOwner = a.LastCustodianTypeName == null ? "Filing Unit" : a.LastCustodianTypeName,
                    LastCustodianTypeNameGUID = a.LastCustodianTypeNameGUID.ToString(),
                    //UserTransferStatus = a.LastCustodianTypeGUID == DASDocumentCustodianType.UNIT? a.LastFlowStatusGUID == ScanDocumentTransferFlowStatus.Pending ?a.LastCustodianTypeGUID== FileLocationMovement ?"fun":"as": "Other":"Others",
                    UserTransferStatus = (a.LastFlowStatusGUID == null && UserGUID == filingUnitUserGUID) ||
                    (a.LastFlowStatusGUID == ScanDocumentTransferFlowStatus.Confirmed && a.LastCustodianTypeNameGUID == UserGUID)
                    ? "ToTransfer" : ((a.LastFlowStatusGUID == ScanDocumentTransferFlowStatus.Pending && (a.LastCustodianTypeNameGUID == UserGUID || (a.LastCustodianTypeGUID == DASDocumentCustodianType.UNIT && a.LastCustodianTypeNameGUID == UserUnitGUID))) ? "ToConfirm" : ""),




                    LastFlowStatusName = a.LastFlowStatusName,
                    //LastCustodianTypeGUID = a.LastCustodianTypeGUID.ToString(),
                    LatestAppointmentDate = a.LatestAppointmentDate,
                    TotalDaysReminigForAppointment = a.TotalDaysReminigForAppointment,
                    IsRequested = a.IsRequested,
                    LastRequesterName = a.LastRequesterName,
                    LastRequesterNameGUID = a.LastRequesterNameGUID.ToString(),
                    RequestStatusName = a.RequestStatusName,

                    LastFlowStatusGUID = a.LastFlowStatusGUID.ToString(),
                    dataUploadUserFileRowVersion = b.dataUploadUserFileRowVersion,

                }).Where(Predicate);

            All = All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<FileDataTableModel> Result = Mapper.Map<List<FileDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());


            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Transfer Location 
        public ActionResult FileLocationMovementDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/DAS/Views/FileLocationMovement/_FileLocationMovementDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<FileLocationMovementDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<FileLocationMovementDataTableModel>(DataTable.Filters);
            }

            var Result = (

                from a in DbDAS.dataFileLocationMovement.Where(x => x.FileGUID == PK).AsExpandable()
                join b in DbDAS.codeDASTransferLocation.Where(x => x.Active) on a.TransferLocationGUID equals b.TransferLocationGUID into LJ1
                from R1 in LJ1.DefaultIfEmpty()


                join c in DbDAS.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.CreatedByGUID equals c.UserGUID into LJ2
                from R2 in LJ2.DefaultIfEmpty()


                select new FileLocationMovementDataTableModel
                {
                    FileLocationMovementGUID = a.FileLocationMovementGUID.ToString(),
                    IsLastAction = a.IsLastAction == true ? "Yes" : "No",
                    ActionDate = a.ActionDate,
                    TransferLocationName = R1.TransferLocationName,
                    CreateBy = R2.FirstName + " " + R2.Surname,
                    CreatedByGUID = a.CreatedByGUID.ToString(),
                    dataFileLocationMovementRowVersion = a.dataFileLocationMovementRowVersion,
                    Active = a.Active

                }).Where(Predicate);

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }


        public ActionResult FileLocationMovementCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.DASConfiguration.Create, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            FileLocationMovementModel model = new FileLocationMovementModel { FileGUID = FK, };

            return PartialView("~/Areas/DAS/Views/FileLocationMovement/_FileLocationMovementUpdateModal.cshtml",
               model);
        }

        public ActionResult FileLocationMovementUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.DASConfiguration.Access, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            FileLocationMovementModel model = Mapper.Map(DbDAS.dataFileLocationMovement.Where(a => a.FileGUID == PK).FirstOrDefault(), new FileLocationMovementModel());
            //FileLocationMovementModel model = DbDAS.dataFileLocationMovement.Where(a => a.FileGUID == PK).
            //    Select(a => new FileLocationMovementModel
            //    {
            //        FileLocationMovementGUID = a.FileLocationMovementGUID,
            //        FileGUID = a.FileGUID,
            //        TransferLocationGUID = a.TransferLocationGUID,
            //        IsLastAction = a.IsLastAction,
            //        ActionDate = a.ActionDate,
            //        Comments = a.Comments,
            //        CreatedByGUID = a.CreatedByGUID,
            //        Active = a.Active


            //    }
            //    ).FirstOrDefault();
            return PartialView("~/Areas/DAS/Views/FileLocationMovement/_FileLocationMovementUpdateModal.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult FileLocationMovementCreate(FileLocationMovementModel model)
        {
            if (!CMS.HasAction(Permissions.DASConfiguration.Create, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            DateTime ExecutionTime = DateTime.Now;
            if (!ModelState.IsValid) return PartialView("~/Areas/DAS/Views/FileLocationMovement/_FileLocationMovementUpdateModal.cshtml", model);
            var pri = DbDAS.dataFileLocationMovement.Where(x => x.FileGUID == model.FileGUID && x.IsLastAction == true).ToList();
            pri.ForEach(x => x.IsLastAction = false);
            DbDAS.UpdateBulk(pri, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);
            dataFileLocationMovement _model = new dataFileLocationMovement
            {

                FileLocationMovementGUID = Guid.NewGuid(),
                FileGUID = model.FileGUID,
                TransferLocationGUID = model.TransferLocationGUID,
                IsLastAction = true,

                ActionDate = ExecutionTime,
                Comments = model.Comments,
                CreatedByGUID = UserGUID
            };

            var file = DbDAS.dataFile.Where(x => x.FileGUID == model.FileGUID).FirstOrDefault();
            file.LastTransferLocationGUID = model.TransferLocationGUID;
            file.LastTransferLocationName = DbDAS.codeDASTransferLocation.Where(x => x.TransferLocationGUID == model.TransferLocationGUID).FirstOrDefault().TransferLocationName;

            DbDAS.Create(_model, Permissions.DASConfiguration.CreateGuid, ExecutionTime, DbCMS);
            DbDAS.Update(file, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);



            try
            {
                DbDAS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbDAS.SingleUpdateMessage(DataTableNames.FileLocationMovementDataTable, DbDAS.PrimaryKeyControl(_model), DbDAS.RowVersionControls(Portal.SingleToList(_model))));
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult FileLocationMovementUpdate(FileLocationMovementModel model)
        {
            if (!CMS.HasAction(Permissions.DASConfiguration.Update, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid) return PartialView("~/Areas/DAS/Views/FileLocationMovement/_FileLocationMovementUpdateModal.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            dataFileLocationMovement _model = Mapper.Map(model, new dataFileLocationMovement());
            DbDAS.Update(_model, Permissions.DASConfiguration.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbDAS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbDAS.SingleUpdateMessage(DataTableNames.FileLocationMovementDataTable,
                    DbDAS.PrimaryKeyControl(model),
                    DbDAS.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyFileLocationMovement(model.FileLocationMovementGUID);
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult FileLocationMovementDelete(dataFileLocationMovement model)
        {
            if (!CMS.HasAction(Permissions.DASConfiguration.Delete, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataFileLocationMovement> DeletedLanguages = DeleteFileLocationMovement(new List<dataFileLocationMovement> { model });

            try
            {
                DbDAS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbDAS.SingleDeleteMessage(DeletedLanguages, DataTableNames.FileLocationMovementDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyFileLocationMovement(model.FileLocationMovementGUID);
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult FileLocationMovementRestore(FileLocationMovementModel model)
        {
            if (!CMS.HasAction(Permissions.DASConfiguration.Restore, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }


            List<FileLocationMovementModel> RestoredLanguages = RestoreFileLocationMovement(Portal.SingleToList(model));

            try
            {
                DbDAS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbDAS.SingleRestoreMessage(RestoredLanguages, DataTableNames.FileLocationMovementDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyFileLocationMovement(model.FileLocationMovementGUID);
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult FileLocationMovementsDataTableDelete(List<dataFileLocationMovement> models)
        {
            if (!CMS.HasAction(Permissions.DASConfiguration.Delete, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataFileLocationMovement> DeletedLanguages = DeleteFileLocationMovement(models);

            try
            {
                DbDAS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbDAS.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.FileLocationMovementDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult FileLocationMovementsDataTableRestore(List<FileLocationMovementModel> models)
        {
            if (!CMS.HasAction(Permissions.DASConfiguration.Restore, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<FileLocationMovementModel> RestoredLanguages = RestoreFileLocationMovement(models);

            try
            {
                DbDAS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbDAS.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.FileLocationMovementDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbDAS.ErrorMessage(ex.Message));
            }
        }

        private List<dataFileLocationMovement> DeleteFileLocationMovement(List<dataFileLocationMovement> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<dataFileLocationMovement> DeletedFileLocationMovement = new List<dataFileLocationMovement>();

            string query = DbDAS.QueryBuilder(models, Permissions.DASConfiguration.DeleteGuid, SubmitTypes.Delete, "");

            var languages = DbDAS.Database.SqlQuery<dataFileLocationMovement>(query).ToList();

            foreach (var language in languages)
            {
                DeletedFileLocationMovement.Add(DbDAS.Delete(language, ExecutionTime, Permissions.DASConfiguration.DeleteGuid, DbCMS));
            }

            return DeletedFileLocationMovement;
        }

        private List<FileLocationMovementModel> RestoreFileLocationMovement(List<FileLocationMovementModel> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<FileLocationMovementModel> RestoredLanguages = new List<FileLocationMovementModel>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbDAS.QueryBuilder(models, Permissions.DASConfiguration.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Languages = DbDAS.Database.SqlQuery<FileLocationMovementModel>(query).ToList();


            return RestoredLanguages;
        }

        private JsonResult ConcrrencyFileLocationMovement(Guid PK)
        {
            FileLocationMovementModel dbModel = new FileLocationMovementModel();

            var Language = DbDAS.dataFileLocationMovement.Where(l => l.FileGUID == PK).FirstOrDefault();
            var dbLanguage = DbDAS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);



            return Json(JsonMessages.ConcurrencyError(DbDAS, dbModel, "LanguagesContainer"));
        }


        #endregion
        #region Old FTS

        public ActionResult DASOLDFTSFileMovementsDataHistoryDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/DAS/Views/OldFTS/_OldFTSFileMovementDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);
            var currentFileNumber = DbDAS.dataFile.Where(a => a.FileGUID == PK).Select(a => a.FileNumber).FirstOrDefault();
            Expression<Func<OldFTSFileMovementDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<OldFTSFileMovementDataTableModel>(DataTable.Filters);
            }

            var Result = (

                from a in DbDAS.dataOldFTSFileMovement.Where(x => x.ProcessingGroupNumber == currentFileNumber).AsExpandable()

                select new OldFTSFileMovementDataTableModel
                {
                    Id = a.Id.ToString(),
                    ProcessingGroupNumber = a.ProcessingGroupNumber,
                    Returned = a.Returned,
                    StaffID = a.StaffID,
                    TransDate = a.TransDate,
                    ReturnDate = a.ReturnDate,
                    DueDate = a.DueDate,
                    FileFrom = a.FileFrom,
                    LoanPeriod = a.LoanPeriod,
                    TransCreator = a.TransCreator,
                    Active = true


                }).Where(Predicate);

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result.OrderByDescending(f => f.TransDate)), JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Bulk Search File
        [Route("DAS/ScanDocument/SearchBulkFile/")]
        public ActionResult SearchBulkFile()
        {
            // DbDAS.dataScannDocumentMetaData.Where(x => x.ScannDocumentGUID == FK).ToList();
            return PartialView("~/Areas/DAS/Views/RefugeeScannedDocument/_SearchBulkFile.cshtml", new FileModel());

        }
        [HttpPost]
        public ActionResult SearchBulkFileCreate(FileModel model)
        {
            if (model == null)
            {
                return Json(new { success = 0 }, JsonRequestBehavior.AllowGet);
            }

            var res = model.FileNumbers;
            string[] splitInput = System.Text.RegularExpressions.Regex.Split(res, "\r\n");

            var toRemove = DbDAS.dataUploadUserFile.Where(a => a.UserGUID == UserGUID).ToList();
            DbDAS.dataUploadUserFile.RemoveRange(toRemove);
            dataFile myFile = new dataFile();
            List<dataUploadUserFile> filesToAdd = new List<dataUploadUserFile>();
            for (int i = 0; i < splitInput.Length; i++)
            {
                string current = splitInput[i];
                if (splitInput[i] == null)
                    continue;
                myFile = DbDAS.dataFile.Where(x => x.FileNumber == current).FirstOrDefault();
                if (myFile == null)
                    continue;
                DateTime ExecutionTime = DateTime.Now;
                dataUploadUserFile newFile = new dataUploadUserFile
                {
                    UploadUserFileGUID = Guid.NewGuid(),
                    FileGUID = myFile.FileGUID,
                    UserGUID = UserGUID,
                    Active = true

                };
                filesToAdd.Add(newFile);

            }
            DbDAS.CreateBulk(filesToAdd, Permissions.RefugeeScannedDocument.UpdateGuid, DateTime.Now, DbCMS);
            try
            {

                DbDAS.SaveChanges();
                DbCMS.SaveChanges();

                return Json(DbDAS.SingleUpdateMessage(DataTableNames.RefugeeUploadScannedDocumentDataTable, DbDAS.PrimaryKeyControl(myFile), DbDAS.RowVersionControls(Portal.SingleToList(myFile))));

            }
            catch (Exception e)
            {
                return Json(DbDAS.ErrorMessage(e.Message));
            }

        }
        #endregion
        #region Confirm Receiving File(s)
        [HttpGet]
        public ActionResult GetConfirmReceivingBulkFiles()
        {
            return PartialView("~/Areas/DAS/Views/FileConfirmation/_ConfirmReceivingBulkFiles.cshtml", new TransferFileModel { FileGUID = null });
        }

        //ffx



        public ActionResult ConfirmUserBulkFilesRecipt(Guid id)
        {
            var files = DbDAS.dataFile.Where(x => x.LastCustodianTypeNameGUID == id
                && x.LastFlowStatusGUID == ScanDocumentTransferFlowStatus.Pending).ToList();
            if (files.Count <= 0)
            {
                throw new HttpException(401, "Unauthorized access");
            }
            //if(DangerPay.FlowStatusGUID == NationalStaffDangerPaConfirmationStatus.Confirmed)
            //{
            //    return View("~/Areas/DAS/Views/NationalStaffDangerPayDetail/DangerPayConfirmed.cshtml");
            //}
            List<ConfirmReceivingFileModel> models = new List<ConfirmReceivingFileModel>();
            foreach (var item in files)
            {
                ConfirmReceivingFileModel myModel = new ConfirmReceivingFileModel
                {
                    FileGUID = item.FileGUID,
                    FileNumber = item.FileNumber,
                    LastCustodianTypeNameGUID = item.LastCustodianTypeNameGUID,
                    Validation = true
                };
                models.Add(myModel);
            }
            return View("~/Areas/DAS/Views/FileTransfer/ConfirmReceivingBulkFiles.cshtml", models);

        }

        [Route("DAS/ScanDocument/ConfirmReceivingBulkFiles/")]
        [HttpPost]

        public ActionResult ConfirmReceivingBulkFiles(List<Guid> models, TransferFileModel test)
        {
            int success = 0;
            if (models == null || models.Count == 0) { return RedirectToAction("ConfirmRequestFiles", new { result = "NoFilesToTransfer" }); }
            DateTime ExecutionTime = DateTime.Now;
            List<Guid> scannDocumentTransferGUIDs = new List<Guid>();
            var myFilesToConfirm = DbDAS.dataFile.Where(x => models.Contains(x.FileGUID) && x.LastFlowStatusGUID == ScanDocumentTransferFlowStatus.Pending && x.LastCustodianTypeNameGUID == UserGUID).Select(x => x.FileGUID).ToList();
            foreach (var item in myFilesToConfirm)
            {
                dataFile myModel = DbDAS.dataFile.Where(x => x.FileGUID == item).FirstOrDefault();
                dataScannDocumentTransferFlow flow = DbDAS.dataScannDocumentTransferFlow.Where(x => x.dataScannDocumentTransfer.dataFileRequest.FileGUID == item
                && x.IsLastAction == true).FirstOrDefault();
                flow.IsLastAction = false;
                DbDAS.Update(flow, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);

                dataScannDocumentTransferFlow newFlow = new dataScannDocumentTransferFlow
                {
                    ScannDocumentTransferFlowGUID = Guid.NewGuid(),
                    ScannDocumentTransferGUID = flow.ScannDocumentTransferGUID,
                    DocumentFlowStatusGUID = ScanDocumentTransferFlowStatus.Confirmed,
                    IsLastAction = true,
                    CreateByGUID = UserGUID,
                    CreateDate = ExecutionTime

                };
                DbDAS.Create(newFlow, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);


                myModel.LastFlowStatusName = "Confirmed";
                myModel.LastFlowStatusGUID = ScanDocumentTransferFlowStatus.Confirmed;
                DbDAS.Update(myModel, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);

            }
            if (myFilesToConfirm.Count > 0)
            {
                try
                {

                    DbDAS.SaveChanges();
                    DbCMS.SaveChanges();

                    return RedirectToAction("ConfirmRequestFiles", new { result = "Cofirmed" });

                }
                catch (Exception e)
                {
                    return Json(DbDAS.ErrorMessage(e.Message));
                }



            }
            else
                return RedirectToAction("ConfirmRequestFiles", new { result = "Pending" });

        }
        #endregion

        #region Cancel tranfer files 
        public ActionResult GetCancelTransferBulkFiles()
        {
            return PartialView("~/Areas/DAS/Views/CancelTranferFile/_CancelTransferBulkFiles.cshtml", new TransferFileModel { FileGUID = null });
        }
        [Route("DAS/ScanDocument/CancelTransferBulkPhysicalFileFromUpload/")]
        [HttpPost]

        public ActionResult CancelTransferBulkPhysicalFileFromUpload(List<Guid> models, TransferFileModel test)
        {
            int success = 0;
            if (models == null || models.Count == 0) { return RedirectToAction("ConfirmRequestFiles", new { result = "NoFilesToTransfer" }); }
            DateTime ExecutionTime = DateTime.Now;
            List<Guid> scannDocumentTransferGUIDs = new List<Guid>();
            //  Guid filingUnitGUID = Guid.Parse("bb915454-05fb-4e4d-b033-e01c7c790997");

            var transferLocation = DbDAS.codeDASTransferLocation.Where(x => x.TransferLocationGUID == test.TransferLocationGUID).FirstOrDefault();
            var userFilingGUID = DbDAS.codeDASDestinationUnitFocalPoint.Where(x =>
            //x.DestinationUnitGUID == filingUnitGUID &&_
            x.UserGUID == UserGUID).FirstOrDefault().DestinationUnitFocalPointGUID;
            var myFileGUID = models.Select(x => x).Distinct();
            var myFileGUIDs = DbDAS.dataUploadUserFile.Where(x => myFileGUID.Contains(x.UploadUserFileGUID)).Select(a => a.FileGUID).ToList();
            var myModels = DbDAS.dataFile.Where(x => myFileGUIDs.Contains(x.FileGUID) && (x.LastTransferFromNameGUID == UserGUID) && x.LastFlowStatusGUID == ScanDocumentTransferFlowStatus.Pending).Select(x => x.FileGUID).ToList();
            var allFiles = DbDAS.dataFile.Where(x => myFileGUIDs.Contains(x.FileGUID)).ToList();
            var allflows = DbDAS.dataScannDocumentTransferFlow.Where(x => myFileGUIDs.Contains((Guid)x.dataScannDocumentTransfer.dataFileRequest.FileGUID)).ToList();
            var unit = DbDAS.codeDASDestinationUnitFocalPoint.Where(x => x.UserGUID == UserGUID).FirstOrDefault();

            var mySite = DbAHD.codeTablesValuesLanguages.Where(x => x.ValueGUID == unit.codeDASDestinationUnit.SiteOwnerGUID && x.LanguageID == LAN).FirstOrDefault();
            var myStaff = DbDAS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.UserGUID == UserGUID).FirstOrDefault();
            var allpriLocations = DbDAS.dataFileLocationMovement.Where(x => myFileGUIDs.Contains((Guid)x.FileGUID) && x.IsLastAction == true).ToList();
            List<dataFileRequest> allRequests = new List<dataFileRequest>();
            List<dataFileLocationMovement> movements = new List<dataFileLocationMovement>();
            List<dataScannDocumentTransferFlow> flows = new List<dataScannDocumentTransferFlow>();
            List<dataScannDocumentTransfer> transfers = new List<dataScannDocumentTransfer>();

            foreach (var myfileGUID in myModels)
            {
                //var userFile = DbDAS.dataUploadUserFile.Where(x => x.UploadUserFileGUID == item).FirstOrDefault();
                var myFile = allFiles.Where(x => x.FileGUID == myfileGUID).FirstOrDefault();
                dataFileRequest fileRequest = new dataFileRequest();

                if (myFile.FileGUID != null)
                {

                    string custType = "";
                    string custName = "";
                    var oldTransfer = allflows.Where(x => x.dataScannDocumentTransfer.dataFileRequest.FileGUID == myFile.FileGUID).ToList();

                    oldTransfer.ForEach(x => x.IsLastAction = false);

                    fileRequest = new dataFileRequest
                    {

                        FileRequestGUID = Guid.NewGuid(),
                        FileGUID = myFile.FileGUID,
                        RequesterTypeGUID = DASDocumentCustodianType.Staff,
                        RequesterGUID = UserGUID,
                        RequestDate = test.RequestDate != null ? test.RequestDate : ExecutionTime,
                        RequestDurationDate = test.RequestDurationDate,
                        RequestStatusGUID = FileRequestStatus.Delivered,
                        IsLastRequest = true,
                        OrderId = oldTransfer.Select(x => x.OrderId).Max() + 1 ?? 1,
                        Comments = test.Comments,
                        RequsterFromNameGUID = myFile.LastCustodianTypeNameGUID,
                        RequsterFromName = myFile.LastCustodianTypeName,
                        RequsterName = custName,

                    };
                    allRequests.Add(fileRequest);



                    dataScannDocumentTransfer transfer = new dataScannDocumentTransfer
                    {
                        ScannDocumentTransferGUID = Guid.NewGuid(),
                        FileRequestGUID = fileRequest.FileRequestGUID,
                        RequesterTypeGUID = fileRequest.RequesterTypeGUID,
                        RequesterGUID = fileRequest.RequesterGUID,
                        TransferDate = ExecutionTime,
                        CreateByGUID = UserGUID,
                        CreateDate = ExecutionTime,

                        OrderId = oldTransfer.Select(x => x.OrderId).Max() != null ? oldTransfer.Select(x => x.OrderId).Max() + 1 : 1,

                    };

                    dataScannDocumentTransferFlow flow = new dataScannDocumentTransferFlow
                    {
                        ScannDocumentTransferFlowGUID = Guid.NewGuid(),
                        ScannDocumentTransferGUID = transfer.ScannDocumentTransferGUID,
                        DocumentFlowStatusGUID = ScanDocumentTransferFlowStatus.Pending,
                        IsLastAction = true,
                        CreateByGUID = UserGUID,
                        CreateDate = ExecutionTime

                    };


                    var priLocations = allpriLocations.Where(x => x.FileGUID == myfileGUID && x.IsLastAction == true).ToList();
                    priLocations.ForEach(f => f.IsLastAction = false);
                    DbDAS.UpdateBulk(priLocations, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);

                    if (test.TransferLocationGUID != null)
                    {
                        dataFileLocationMovement movement = new dataFileLocationMovement
                        {
                            FileLocationMovementGUID = Guid.NewGuid(),
                            FileGUID = myfileGUID,
                            TransferLocationGUID = test.TransferLocationGUID,
                            IsLastAction = true,
                            ActionDate = ExecutionTime,
                            CreatedByGUID = UserGUID
                        };
                        movements.Add(movement);

                    }
                    flows.Add(flow);
                    scannDocumentTransferGUIDs.Add(transfer.ScannDocumentTransferGUID);

                    DbDAS.UpdateBulk(oldTransfer, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);
                    var file = allFiles.Where(x => x.FileGUID == myFile.FileGUID).FirstOrDefault();
                    //if (fileRequest.RequesterTypeGUID == DASDocumentCustodianType.Staff)
                    //{

                    file.LastCustodianTypeName = myStaff.FirstName + " " + myStaff.Surname;
                    file.LastCustodianType = "Staff";

                    file.LastTransferLocationGUID = test.TransferLocationGUID;
                    file.LastTransferLocationName = transferLocation != null ? transferLocation.TransferLocationName : null;



                    custType = "Staff";
                    custName = myStaff.FirstName + " " + myStaff.Surname;
                    file.LastCustodianTypeGUID = fileRequest.RequesterTypeGUID;
                    file.LastCustodianTypeNameGUID = fileRequest.RequesterGUID;
                    file.LastFlowStatusName = "Pending";
                    file.LastFlowStatusGUID = ScanDocumentTransferFlowStatus.Pending;
                    file.RequestStatusGUID = FileRequestStatus.Delivered;
                    file.RequestStatusName = "Delivered";
                    file.IsRequested = false;
                    file.LastTransferFromName = myFile.LastCustodianTypeName;
                    file.LastTransferFromNameGUID = myFile.LastCustodianTypeNameGUID;
                    file.SiteCode = mySite.ValueDescription;
                    file.SiteGUID = mySite.ValueGUID;


                    DbDAS.Update(file, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);

                    transfer.CustodianName = custName;
                    transfer.CustodianType = custType;
                    transfer.TransferFromName = myFile.LastCustodianTypeName;

                    transfers.Add(transfer);




                }

            }
            DbDAS.CreateBulk(allRequests, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);
            DbDAS.CreateBulk(movements, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);
            DbDAS.CreateBulk(flows, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);
            DbDAS.CreateBulk(transfers, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);
            try
            {
                DbDAS.SaveChanges();
                DbCMS.SaveChanges();



            }
            catch (Exception e)
            {
                return Json(DbDAS.ErrorMessage(e.Message));
            }
            if (myModels.Count > 0)
            {

                // SendConfirmationTransferBulkFilesEmail(test, scannDocumentTransferGUIDs, 1);

                return RedirectToAction("ConfirmRequestFiles", new { result = "Cofirmed" });
            }
            else
                return RedirectToAction("ConfirmRequestFiles", new { result = "NoFilesToTransfer" });

        }
        #endregion
        #region Change file location 
        [HttpGet]
        public ActionResult GetChangeFileLocation()
        {
            return PartialView("~/Areas/DAS/Views/FileTransfer/_ChangeFilesLocation.cshtml", new TransferFileModel { FileGUID = null });
        }
        [Route("DAS/ScanDocument/ChangeBulkFileLocation/")]
        [HttpPost]

        public ActionResult ChangeBulkFileLocation(List<Guid> models, TransferFileModel test)
        {
            int success = 0;
            if (models == null || models.Count == 0) { return RedirectToAction("ConfirmRequestFiles", new { result = "NoFilesToTransfer" }); }
            DateTime ExecutionTime = DateTime.Now;
            List<Guid> scannDocumentTransferGUIDs = new List<Guid>();
            //Guid filingUnitGUID = Guid.Parse("bb915454-05fb-4e4d-b033-e01c7c790997");
            var unit = DbDAS.codeDASDestinationUnitFocalPoint.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
            var transferLocation = DbDAS.codeDASTransferLocation.Where(x => x.TransferLocationGUID == test.TransferLocationGUID).FirstOrDefault();

            var myModels = DbDAS.dataFile.Where(x => models.Contains(x.FileGUID) && (x.LastCustodianTypeNameGUID == UserGUID)).Select(x => x.FileGUID).ToList();
            foreach (var myfileGUID in myModels)
            {
                var pri = DbDAS.dataFileLocationMovement.Where(x => x.FileGUID == myfileGUID && x.IsLastAction == true).ToList();
                pri.ForEach(x => x.IsLastAction = false);
                DbDAS.UpdateBulk(pri, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);
                dataFileLocationMovement _model = new dataFileLocationMovement
                {

                    FileLocationMovementGUID = Guid.NewGuid(),
                    FileGUID = myfileGUID,
                    TransferLocationGUID = test.TransferLocationGUID,
                    IsLastAction = true,

                    ActionDate = ExecutionTime,
                    Comments = test.Comments,
                    CreatedByGUID = UserGUID
                };


                var file = DbDAS.dataFile.Where(a => a.FileGUID == myfileGUID).FirstOrDefault();

                file.LastTransferLocationGUID = test.TransferLocationGUID;
                file.LastTransferLocationName = transferLocation.TransferLocationName;


                DbDAS.Create(_model, Permissions.DASConfiguration.CreateGuid, ExecutionTime, DbCMS);
                DbDAS.Update(file, Permissions.RefugeeScannedDocument.CreateGuid, ExecutionTime, DbCMS);


                try
                {
                    DbDAS.SaveChanges();
                    DbCMS.SaveChanges();

                }
                catch (Exception ex)
                {
                    return Json(DbDAS.ErrorMessage(ex.Message));
                }
            }
            if (myModels.Count > 0)
            {

                //  return Json(DbDAS.SingleUpdateMessage(DataTableNames.DASUserFilesDataTable, DbDAS.PrimaryKeyControl(transferLocation), DbDAS.RowVersionControls(Portal.SingleToList(transferLocation))));

                return RedirectToAction("ConfirmRequestFiles", new { result = "Cofirmed" });
            }
            else
                return RedirectToAction("ConfirmRequestFiles", new { result = "NoFilesToTransfer" });

        }
        #endregion
    }

}