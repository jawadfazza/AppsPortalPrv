using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.Services;
using AppsPortal.ViewModels;
using AutoMapper;
using DAS_DAL.Model;
using DAS_DAL.ViewModels;
using iTextSharp.text;
using RES_Repo.Globalization;
using iTextSharp.text.pdf;
using LinqKit;
using static AppsPortal.Library.DataTableNames;
using FineUploader;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using Image = iTextSharp.text.Image;

namespace AppsPortal.Areas.DAS.Controllers
{
    public class FileTrackingOverviewController : DASBaseController
    {
        // GET: DAS/FileTrackingOverview
        [Route("DAS/FileTrackingOverviewIndex/")]
        public ActionResult FileTrackingOverviewIndex()
        {
            if (!CMS.HasAction(Permissions.DASDashboard.Access, Apps.DAS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            FileTrackingOverviewModel model = new FileTrackingOverviewModel();
            model.TotalCasesHoldByAllStaff = DbDAS.dataFile.Where(x => x.LastCustodianTypeNameGUID != null).Select(x => x.FileGUID).Count();
            model.TotalCasesHoldByAllUnits = DbDAS.dataFile.Select(x => x.LastDestinationUnitGUID).Count();
            model.unitCasesDistModel = new List<UnitCasesDistModel>();
            model.staffCustodyMoedl = new List<StaffCustodyModel>();
            var units = DbDAS.codeDASDestinationUnit.ToList();
            var allfiles = DbDAS.dataFile.Where(x => x.LastCustodianTypeNameGUID != null).AsQueryable();
            var selectedunits = allfiles.Where(x => x.LastDestinationUnitGUID != null);
            var selectedUsers = allfiles.Where(x => x.LastCustodianTypeNameGUID != null);
            var usersGUIDs = allfiles.Select(x => x.LastCustodianTypeNameGUID).Distinct().ToList();
            var userNames = DbDAS.userPersonalDetailsLanguage.Where(x => usersGUIDs.Contains(x.UserGUID) && x.LanguageID == LAN).ToList();
            foreach (var item in usersGUIDs)
            {
                StaffCustodyModel staff = new StaffCustodyModel
                {
                    TotalCases = selectedUsers.Where(x => x.LastCustodianTypeNameGUID == item).Count(),
                    StaffName = userNames.Where(x => x.UserGUID == item).FirstOrDefault().FirstName + " " + userNames.Where(x => x.UserGUID == item).FirstOrDefault().Surname,
                    UserGUID = item
                };
                model.staffCustodyMoedl.Add(staff);

            }
            foreach (var item in units)
            {
                UnitCasesDistModel myUnit = new UnitCasesDistModel
                {
                    TotalCases = selectedunits.Where(x => x.LastDestinationUnitGUID == item.DestinationUnitGUID).Count(),
                    UnitName = item.DestinationUnitName,
                    DestinationUnitGUID = item.DestinationUnitGUID
                };
                model.unitCasesDistModel.Add(myUnit);


            }
            model.unitCasesDistModel = model.unitCasesDistModel.OrderByDescending(x => x.TotalCases).ToList();
            model.staffCustodyMoedl = model.staffCustodyMoedl.OrderByDescending(x => x.TotalCases).ToList();

            return View("~/Areas/DAS/Views/FileTrackingOverview/Index.cshtml", model);
        }
        public JsonResult InitDashboardData(List<Guid?> DestinationUnitGUIDs)
        {
            var files = DbDAS.dataFile.Where(x => x.LastCustodianTypeGUID != null).AsQueryable();
            var flows = DbDAS.dataScannDocumentTransferFlow.Where(x => x.IsLastAction == true && x.dataScannDocumentTransfer.TransferEndDate != null &&
                         (x.dataScannDocumentTransfer.dataFileRequest.dataFile.LastCustodianTypeGUID == DASDocumentCustodianType.Staff ||
                          x.dataScannDocumentTransfer.dataFileRequest.dataFile.LastCustodianTypeGUID == DASDocumentCustodianType.UNIT
                          )).AsQueryable();


            if (DestinationUnitGUIDs != null && DestinationUnitGUIDs.Count > 0)
            {
                files = files.Where(x => DestinationUnitGUIDs.Contains(x.LastCustodianTypeNameGUID));
                flows = flows.Where(x => DestinationUnitGUIDs.Contains(x.dataScannDocumentTransfer.dataFileRequest.dataFile.LastCustodianTypeNameGUID));

            }
            var requested = files.Where(x => x.LastRequesterNameGUID != null);
            CasesOverviewModel model = new CasesOverviewModel();
            model.TotalCasesDelay = flows.Count();
            model.TotalCasesPendingConfirmation = files.Where(x => x.LastFlowStatusGUID == ScanDocumentTransferFlowStatus.Pending).Count();
            model.TotalCasesConfirmed = files.Where(x => x.LastFlowStatusGUID == ScanDocumentTransferFlowStatus.Confirmed).Count();
            model.TotalCasesRequested = requested.Count();

            return Json(new { model = model }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult InitCaseFlowIndex(List<Guid?> DestinationUnitGUIDs)
        {
            int start = Convert.ToInt32(Request.Params["start"]);
            int length = Convert.ToInt32(Request.Params["length"]);
            string search = Request.Params["search[value]"];
            string orderColIndex = Request.Params["order[0][column]"];
            string orderCol = Request.Params["columns[" + orderColIndex + "][data]"];
            string orderDir = Request.Params["order[0][dir]"].ToUpper();
            string orderable = Request.Params["columns[1][orderable]"];
            string countryName = Request.Params["columns[4][search][value]"];
            string status = Request.Params["columns[2][search][value]"];



            int resultCount = 0;
            var files = DbDAS.dataFile.Where(x => x.LastCustodianTypeNameGUID != null).AsQueryable();
            if (DestinationUnitGUIDs != null && DestinationUnitGUIDs.Count > 0)
                files = files.Where(x => DestinationUnitGUIDs.Contains(x.LastCustodianTypeNameGUID) ||
                  DestinationUnitGUIDs.Contains(x.LastDestinationUnitGUID));
            if (search.Length > 1)
            {
                files =
                    files.Where(
                        a =>
                            a.FileNumber.ToString().ToLower().Contains(search.ToLower()) ||
                            a.LastCustodianTypeName.ToString().ToLower().Contains(search.ToLower()) ||
                            a.LastCustodianType.ToString().ToLower().Contains(search.ToLower())


                    );
            }

            resultCount = files.Count();

            var results = files.Select(
                a =>
                    new
                    {
                        FileGUID = a.FileGUID,
                        FileNumber = a.FileNumber,
                        LastCustodianTypeName = a.LastCustodianTypeName,
                        LastFlowStatusName = a.LastFlowStatusName,
                        LastTransferFromName = a.LastTransferFromName,



                    });

            #region orderColumn

            string order = orderCol + "_" + orderDir;

            switch (order)
            {
                case "FileNumber_ASC":
                    results = results.OrderBy(o => o.FileNumber);
                    break;
                case "FileNumber_DESC":
                    results = results.OrderByDescending(o => o.FileNumber);
                    break;
                case "LastCustodianTypeName_ASC":
                    results = results.OrderBy(o => o.LastCustodianTypeName);
                    break;
                case "LastCustodianTypeName_DESC":
                    results = results.OrderByDescending(o => o.LastCustodianTypeName);
                    break;




                default:
                    results = results.OrderByDescending(o => o.FileNumber);
                    break;
            }

            return Json(new
            {
                //sEcho = param.sEcho,
                iTotalRecords = resultCount,
                iTotalDisplayRecords = resultCount,
                aaData = results.Skip(start).Take(length).ToList()
            }, JsonRequestBehavior.AllowGet);



            #endregion
        }

    }
}