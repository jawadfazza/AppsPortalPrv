using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.Library.MimeDetective;
using AppsPortal.SHM.ViewModels;
using AppsPortal.ViewModels;
using AutoMapper;
using DocumentFormat.OpenXml.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Math;
using FineUploader;
using LinqKit;
using OfficeOpenXml;
using SHM_DAL.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.SHM.Controllers
{
    public class ShuttleRequestController : SHMBaseController
    {
        #region Shuttle Request

        public ActionResult Index()
        {
            return View();
        }

        [Route("SHM/ShuttleRequests/")]
        public ActionResult ShuttleRequestsIndex()
        {
            return View("~/Areas/SHM/Views/ShuttleRequests/Index.cshtml");
        }

        //[Route("SHM/ShuttleRequestsDataTable/")]
        public JsonResult ShuttleRequestsDataTable(DataTableRecievedOptions options)
        {
            var app = DbSHM.dataShuttleRequest.ToList();
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<ShuttleRequestsDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<ShuttleRequestsDataTableModel>(DataTable.Filters);
            }

            var All = (from a in DbSHM.dataShuttleRequest.AsExpandable().Where(x => x.UserGUID == UserGUID)
                       join b in DbSHM.codeLocationsLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.StartLocationGUID equals b.LocationGUID into LJ1
                       from R1 in LJ1.DefaultIfEmpty()
                       join c in DbSHM.codeLocationsLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.EndLocationGUID equals c.LocationGUID into LJ2
                       from R2 in LJ2.DefaultIfEmpty()
                       join d in DbSHM.codeShuttleTravelPurposeLanguage.Where(x => x.LanguageID == LAN && x.Active) on a.ShuttleTravelPurposeGUID equals d.ShuttleTravelPurposeGUID into LJ3
                       from R3 in LJ3.DefaultIfEmpty()
                       join e in DbSHM.codeReferralStatusLanguage.Where(x => x.LanguageID == LAN && x.Active) on a.ReferralStatusGUID equals e.ReferralStatusGUID into LJ4
                       from R4 in LJ4.DefaultIfEmpty()
                       join f in DbSHM.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.Active) on a.UserGUID equals f.UserGUID
                       orderby a.DepartureDate
                       select new ShuttleRequestsDataTableModel
                       {
                           DepartureDate = a.DepartureDate,
                           ShuttleTravelPurposeGUID = a.ShuttleTravelPurposeGUID,
                           ShuttleTravelPurposeDescription = R3.ShuttleTravelPurposeDescription,
                           DeparturePointGUID = a.DeparturePointGUID,
                           DropOffPointGUID = a.DropOffPointGUID,
                           EndLocationGUID = a.EndLocationGUID,
                           StartLocationGUID = a.StartLocationGUID,
                           EndLocation = R2.LocationDescription,
                           StartLocation = R1.LocationDescription,
                           UserGUID = a.UserGUID,
                           ReferralStatusGUID = a.ReferralStatusGUID,
                           ReferralStatusDescription = R4.Description,
                           dataShuttleRequestRowVersion = a.dataShuttleRequestRowVersion,
                           ShuttleRequestGUID = a.ShuttleRequestGUID,
                           StaffName = f.FirstName + " " + f.Surname,
                           Active = a.Active,
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<ShuttleRequestsDataTableModel> Result = Mapper.Map<List<ShuttleRequestsDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("SHM/ShuttleRequests/Create/")]
        public ActionResult ShuttleRequestCreate()
        {
            var staffcore = DbSHM.StaffCoreData.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
            return View("~/Areas/SHM/Views/ShuttleRequests/ShuttleRequest.cshtml", new ShuttleRequestUpdateModel()
            {
                OrganizationInstanceGUID= staffcore.OrganizationInstanceGUID,
                DutyStationGUID=staffcore.DutyStationGUID
            });
        }

        [Route("SHM/ShuttleRequests/Update/{PK}")]
        public ActionResult ShuttleRequestUpdate(Guid PK)
        {
            //if (!CMS.HasAction(Permissions.ShuttleRequest.Access, Apps.SHM))
            //{
            //    throw new HttpException(401, "Unauthorized access");
            //}
            var model = (from a in DbSHM.dataShuttleRequest.WherePK(PK)
                         join b in DbSHM.codeLocations on a.StartLocationGUID equals b.LocationGUID
                         join c in DbSHM.codeLocations on a.EndLocationGUID equals c.LocationGUID
                         join e in DbSHM.codeLocations on a.StartLocationReturnGUID equals e.LocationGUID into LJ1
                         from R1 in LJ1.DefaultIfEmpty()
                         join f in DbSHM.codeLocations on a.EndLocationReturnGUID equals f.LocationGUID into LJ2
                         from R2 in LJ2.DefaultIfEmpty()
                         join d in DbSHM.codeShuttleTravelPurpose on a.ShuttleTravelPurposeGUID equals d.ShuttleTravelPurposeGUID
                         select new ShuttleRequestUpdateModel
                         {
                             DepartureDate = a.DepartureDate,
                             ReturnDateTime=a.ReturnDateTime,
                             dataShuttleRequestRowVersion = a.dataShuttleRequestRowVersion,
                            /////////////////////////////////////
                             DeparturePointGUID = a.DeparturePointGUID,
                             DropOffPointGUID = a.DropOffPointGUID,
                             EndLocationGUID = a.EndLocationGUID,
                             StartLocationGUID = a.StartLocationGUID,
                             CountryGUID = b.CountryGUID,
                             CountryGUID1 = c.CountryGUID,
                             ////////////////////////////////////
                             DeparturePointReturnGUID = a.DeparturePointReturnGUID ,
                             DropOffPointReturnGUID = a.DropOffPointReturnGUID,
                             EndLocationReturnGUID = a.EndLocationReturnGUID,
                             StartLocationReturnGUID = a.StartLocationReturnGUID,
                             CountryReturnGUID = R1.CountryGUID,
                             CountryReturnGUID1 = R2.CountryGUID,
                             ///////////////////////////////////
                             ReferralStatusGUID = a.ReferralStatusGUID,
                             ShuttleTravelPurposeGUID = a.ShuttleTravelPurposeGUID,
                             UserGUID = a.UserGUID,
                             ShuttleRequestGUID = a.ShuttleRequestGUID,
                             Active = a.Active,
                             AdminComment = a.AdminComment,
                             StaffComment = a.StaffComment,
                             
                             WithReturnDate=d.WithReturnDate.Value ,
                             DutyStationGUID=a.DutyStationGUID,
                             OrganizationInstanceGUID=a.OrganizationInstanceGUID
                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("ShuttleRequest", "ShuttleRequests", new { Area = "SHM" }));

            return View("~/Areas/SHM/Views/ShuttleRequests/ShuttleRequest.cshtml", model);
        }

        [Route("SHM/ShuttleRequests/Upload/{PK}")]
        public ActionResult Upload(Guid PK)
        {
            var model = (from a in DbSHM.dataShuttleRequest.WherePK(PK)
                         select new ShuttleRequestUpdateModel
                         {
                             ShuttleRequestGUID = a.ShuttleRequestGUID,
                             Active = a.Active
                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("ShuttleRequest", "ShuttleRequests", new { Area = "SHM" }));

            return PartialView("~/Areas/SHM/Views/ShuttleRequests/_ShuttleRequestUploadFile.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]///SHM/ShuttleRequests/UploadFiles
        public FineUploaderResult UploadFiles(FineUpload upload, Guid PK)
        {
            string error = "";
            if (FileTypeValidator.IsPDF(upload.InputStream))
            {
                string FilePath = ConfigurationManager.AppSettings["DataFolder"] + "\\Uploads\\SHM\\ShuttleRequest\\" + PK + ".pdf";
                try
                {
                    upload.SaveAs(FilePath, true);
                    var req = DbSHM.dataShuttleRequest.Where(x => x.ShuttleRequestGUID == PK).FirstOrDefault();
                    req.FileAttached = true;
                    DbSHM.SaveChanges();
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                }
            }
            return new FineUploaderResult((error == "" ? true : false), new { Error = error, path = "", success = (error == "" ? true : false) });
        }

       
        public ActionResult ShuttleRequestCreate(ShuttleRequestUpdateModel model)
        {
            if (!ModelState.IsValid || ActiveShuttleRequest(model)) return PartialView("~/Areas/SHM/Views/ShuttleRequests/_ShuttleRequestForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            dataShuttleRequest ShuttleRequest = Mapper.Map(model, new dataShuttleRequest());
            ShuttleRequest.UserGUID = UserGUID;
            ShuttleRequest.ReferralStatusGUID = ShuttleRequestStaus.Pending;
            ShuttleRequest.ShuttleRequestGUID = EntityPK;
            ShuttleRequest.FileAttached = false;
            DbSHM.CreateNoAudit(ShuttleRequest);
            DbSHM.CreateNoAudit(new dataShuttleRequestStaff() {
                ShuttleRequestStaffGUID=Guid.NewGuid(),
                ShuttleRequestGUID=ShuttleRequest.ShuttleRequestGUID,
                UserGUID=ShuttleRequest.UserGUID,
                ReferralStatusGUID= ShuttleRequestStaus.Pending,
            Active =true
            });


            //In case we have a multi-route from point to point locations
            var route = (from a in DbSHM.dataShuttleRequestRoute.Where(x => x.StartLocationGUID == ShuttleRequest.StartLocationGUID.Value && x.EndLocationGUID == ShuttleRequest.EndLocationGUID.Value)
                         join b in DbSHM.dataShuttleRequestRouteStep on a.ShuttleRequestRouteGUID equals b.ShuttleRequestRouteGUID
                         select new
                         {
                             b.ShuttleRequestRouteStepGUID,
                             b.StartLocationGUID,
                             b.EndLocationGUID
                         }).ToList();
            if (route.Count!=0)
            {
                foreach (var row in route)
                {
                    dataShuttleRequestBoxState boxState = new dataShuttleRequestBoxState();
                    boxState.IsBoxStateDroped = false;
                    boxState.IsDeparture = true;
                    boxState.ShuttleRequestBoxStateGUID = Guid.NewGuid();
                    boxState.ShuttleRequestGUID = EntityPK;
                    boxState.StartLocationGUID = row.StartLocationGUID;
                    boxState.EndLocationGUID = row.EndLocationGUID;
                    boxState.ShuttleRequestRouteStepGUID = row.ShuttleRequestRouteStepGUID;
                    boxState.Active = true;
                    DbSHM.CreateNoAudit(boxState);
                }
            }
            else
            {
                dataShuttleRequestBoxState boxState = new dataShuttleRequestBoxState();
                boxState.IsBoxStateDroped = false; 
                boxState.IsDeparture = true;
                boxState.ShuttleRequestBoxStateGUID = Guid.NewGuid();
                boxState.ShuttleRequestGUID = EntityPK;
                boxState.StartLocationGUID = model.StartLocationGUID;
                boxState.EndLocationGUID = model.EndLocationGUID;
                boxState.ShuttleRequestRouteStepGUID = null;
                boxState.Active = true;
                DbSHM.CreateNoAudit(boxState);
            }
            


            //In case we have a multi-route from point to point locations
            var routeReturn = (from a in DbSHM.dataShuttleRequestRoute.Where(x => x.StartLocationGUID == ShuttleRequest.StartLocationReturnGUID.Value && x.EndLocationGUID == ShuttleRequest.EndLocationReturnGUID.Value)
                         join b in DbSHM.dataShuttleRequestRouteStep on a.ShuttleRequestRouteGUID equals b.ShuttleRequestRouteGUID
                         select new
                         {
                             b.ShuttleRequestRouteStepGUID,
                             b.StartLocationGUID,
                             b.EndLocationGUID
                         }).ToList();
            if (routeReturn.Count != 0)
            {
                foreach (var row in routeReturn)
                {
                    dataShuttleRequestBoxState boxState = new dataShuttleRequestBoxState();
                    boxState.IsBoxStateDroped = false;
                    boxState.IsDeparture = false;
                    boxState.ShuttleRequestBoxStateGUID = Guid.NewGuid();
                    boxState.ShuttleRequestGUID = EntityPK;
                    boxState.StartLocationGUID = row.StartLocationGUID;
                    boxState.EndLocationGUID = row.EndLocationGUID;
                    boxState.ShuttleRequestRouteStepGUID = row.ShuttleRequestRouteStepGUID;
                    boxState.Active = true;
                    DbSHM.CreateNoAudit(boxState);
                }
            }
            else
            {
                if (model.StartLocationReturnGUID != null && model.EndLocationReturnGUID != null)
                {
                    dataShuttleRequestBoxState boxState = new dataShuttleRequestBoxState();
                    boxState.IsBoxStateDroped = false;
                    boxState.IsDeparture = false;
                    boxState.ShuttleRequestBoxStateGUID = Guid.NewGuid();
                    boxState.ShuttleRequestGUID = EntityPK;
                    boxState.StartLocationGUID = model.StartLocationReturnGUID;
                    boxState.EndLocationGUID = model.EndLocationReturnGUID;
                    boxState.ShuttleRequestRouteStepGUID = null;
                    boxState.Active = true;
                    DbSHM.CreateNoAudit(boxState);
                }
            }

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButtonNoPermission(new UrlHelper(Request.RequestContext).Action("Create", "ShuttleRequests", new { Area = "SHM" })), Container = "ShuttleRequestFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButtonNoPermission(), Container = "ShuttleRequestFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButtonNoPermission(), Container = "ShuttleRequestFormControls" });

            try
            {
                DbSHM.SaveChanges();
                DbCMS.SaveChanges();
                new Email().SendShuttleRequestUserReceiveNotificationStatusPinding(UserGUID);
                //new Email().SendShuttleRequestFocallPointReceiveNotificationStatusPinding(EntityPK, UserGUID);

                               return Json(DbSHM.SingleCreateMessage(DbSHM.PrimaryKeyControl(ShuttleRequest), DbSHM.RowVersionControls(Portal.SingleToList(ShuttleRequest)), null, "window.location='/SHM/ShuttleRequests/Update/" + EntityPK + "'", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbSHM.ErrorMessage(ex.Message));
            }
        }
        

        public ActionResult ShuttleRequestUpdate(ShuttleRequestUpdateModel model)
        {
            if (!ModelState.IsValid || ActiveShuttleRequest(model)) return PartialView("~/Areas/SHM/Views/ShuttleRequests/_ShuttleRequestForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            dataShuttleRequest ShuttleRequest = Mapper.Map(model, new dataShuttleRequest());
            ShuttleRequest.UserGUID = UserGUID;
           if(ShuttleRequest.ReferralStatusGUID == ShuttleRequestStaus.Completed)
            {
                ShuttleRequest.ReferralStatusGUID = ShuttleRequestStaus.Progressing;
            }
            DbSHM.UpdateNoAudit(ShuttleRequest);

            //In case we have a multi-route from point to point locations
            var boxDropedFlase = DbSHM.dataShuttleRequestBoxState.Where(x => x.ShuttleRequestGUID == ShuttleRequest.ShuttleRequestGUID && !x.IsBoxStateDroped).ToList();
            var boxDropedTrue = DbSHM.dataShuttleRequestBoxState.Where(x => x.ShuttleRequestGUID == ShuttleRequest.ShuttleRequestGUID && x.IsBoxStateDroped).ToList();
            DbSHM.dataShuttleRequestBoxState.RemoveRange(boxDropedFlase);

            var route = (from a in DbSHM.dataShuttleRequestRoute.Where(x => x.StartLocationGUID == ShuttleRequest.StartLocationGUID.Value && x.EndLocationGUID == ShuttleRequest.EndLocationGUID.Value)
                         join b in DbSHM.dataShuttleRequestRouteStep on a.ShuttleRequestRouteGUID equals b.ShuttleRequestRouteGUID
                         select new
                         {
                             b.ShuttleRequestRouteStepGUID,
                             b.StartLocationGUID,
                             b.EndLocationGUID
                         }).ToList();
            if (route.Count != 0)
            {
                foreach (var row in route)
                {
                    if(boxDropedTrue.Where(x=>x.StartLocationGUID==model.StartLocationGUID && x.EndLocationGUID== model.EndLocationGUID).FirstOrDefault() == null)
                    {
                        dataShuttleRequestBoxState boxState = new dataShuttleRequestBoxState();
                        boxState.IsBoxStateDroped = false;
                        boxState.IsDeparture = true;
                        boxState.ShuttleRequestBoxStateGUID = Guid.NewGuid();
                        boxState.ShuttleRequestGUID = model.ShuttleRequestGUID;
                        boxState.StartLocationGUID = row.StartLocationGUID;
                        boxState.EndLocationGUID = row.EndLocationGUID;
                        boxState.ShuttleRequestRouteStepGUID = row.ShuttleRequestRouteStepGUID;
                        boxState.Active = true;
                        DbSHM.CreateNoAudit(boxState);
                    }
                   
                }
            }
            else
            {
                if (boxDropedTrue.Where(x => x.StartLocationGUID == model.StartLocationGUID && x.EndLocationGUID == model.EndLocationGUID).FirstOrDefault() == null)
                {
                    dataShuttleRequestBoxState boxState = new dataShuttleRequestBoxState();
                    boxState.IsBoxStateDroped = false;
                    boxState.IsDeparture = true;
                    boxState.ShuttleRequestBoxStateGUID = Guid.NewGuid();
                    boxState.ShuttleRequestGUID = model.ShuttleRequestGUID;
                    boxState.StartLocationGUID = model.StartLocationGUID;
                    boxState.EndLocationGUID = model.EndLocationGUID;
                    boxState.ShuttleRequestRouteStepGUID = null;
                    boxState.Active = true;
                    DbSHM.CreateNoAudit(boxState);
                }
            }



            //In case we have a multi-route from point to point locations
            var routeReturn = (from a in DbSHM.dataShuttleRequestRoute.Where(x => x.StartLocationGUID == ShuttleRequest.StartLocationReturnGUID.Value && x.EndLocationGUID == ShuttleRequest.EndLocationReturnGUID.Value)
                               join b in DbSHM.dataShuttleRequestRouteStep on a.ShuttleRequestRouteGUID equals b.ShuttleRequestRouteGUID
                               select new
                               {
                                   b.ShuttleRequestRouteStepGUID,
                                   b.StartLocationGUID,
                                   b.EndLocationGUID
                               }).ToList();
            if (routeReturn.Count != 0)
            {
                foreach (var row in routeReturn)
                {
                    if (boxDropedTrue.Where(x => x.StartLocationGUID == model.StartLocationReturnGUID && x.EndLocationGUID == model.EndLocationReturnGUID).FirstOrDefault() == null)
                    {
                        dataShuttleRequestBoxState boxState = new dataShuttleRequestBoxState();
                        boxState.IsBoxStateDroped = false;
                        boxState.IsDeparture = false;
                        boxState.ShuttleRequestBoxStateGUID = Guid.NewGuid();
                        boxState.ShuttleRequestGUID = model.ShuttleRequestGUID;
                        boxState.StartLocationGUID = row.StartLocationGUID;
                        boxState.EndLocationGUID = row.EndLocationGUID;
                        boxState.ShuttleRequestRouteStepGUID = row.ShuttleRequestRouteStepGUID;
                        boxState.Active = true;
                        DbSHM.CreateNoAudit(boxState);
                    }
                }
            }
            else
            {
                if (model.StartLocationReturnGUID != null && model.EndLocationReturnGUID != null)
                {
                    if (boxDropedTrue.Where(x => x.StartLocationGUID == model.StartLocationReturnGUID && x.EndLocationGUID == model.EndLocationReturnGUID).FirstOrDefault() == null)
                    {
                        dataShuttleRequestBoxState boxState = new dataShuttleRequestBoxState();
                        boxState.IsBoxStateDroped = false;
                        boxState.IsDeparture = false;
                        boxState.ShuttleRequestBoxStateGUID = Guid.NewGuid();
                        boxState.ShuttleRequestGUID = model.ShuttleRequestGUID;
                        boxState.StartLocationGUID = model.StartLocationReturnGUID;
                        boxState.EndLocationGUID = model.EndLocationReturnGUID;
                        boxState.ShuttleRequestRouteStepGUID = null;
                        boxState.Active = true;
                        DbSHM.CreateNoAudit(boxState);

                    }
                }
            }


            try
            {
                DbSHM.SaveChanges();
                DbCMS.SaveChanges();

                return Json(DbSHM.SingleUpdateMessage(DataTableNames.ShuttleRequestsDataTable, DbSHM.PrimaryKeyControl(ShuttleRequest), DbSHM.RowVersionControls(DbSHM.RowVersionControls(Portal.SingleToList(ShuttleRequest)))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyShuttleRequest(model.ShuttleRequestGUID);
            }
            catch (Exception ex)
            {
                return Json(DbSHM.ErrorMessage(ex.Message));
            }
        }



        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ShuttleRequestDelete(dataShuttleRequest model)
        {
            List<dataShuttleRequest> DeletedShuttleRequest = DeleteShuttleRequests(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButtonNoPermission(), Container = "ShuttleRequestFormControls" });

            try
            {
                int CommitedRows = DbSHM.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbSHM.SingleDeleteMessage(CommitedRows, DeletedShuttleRequest.FirstOrDefault(), "LanguagesContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyShuttleRequest(model.ShuttleRequestGUID);
            }
            catch (Exception ex)
            {
                return Json(DbSHM.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ShuttleRequestRestore(dataShuttleRequest model)
        {
            if (ActiveShuttleRequest(model))
            {
                return Json(DbSHM.RecordExists());
            }

            List<dataShuttleRequest> RestoredShuttleRequests = RestoreShuttleRequests(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButtonNoPermission(new UrlHelper(Request.RequestContext).Action("ShuttleRequests/Create", "ShuttleRequest", new { Area = "SHM" })), Container = "ShuttleRequestFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButtonNoPermission(), Container = "ShuttleRequestFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButtonNoPermission(), Container = "ShuttleRequestFormControls" });

            try
            {
                int CommitedRows = DbSHM.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbSHM.SingleRestoreMessage(CommitedRows, RestoredShuttleRequests, DbSHM.PrimaryKeyControl(RestoredShuttleRequests.FirstOrDefault()), null, null, UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyShuttleRequest(model.ShuttleRequestGUID);
            }
            catch (Exception ex)
            {
                return Json(DbSHM.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ShuttleRequestsDataTableDelete(List<dataShuttleRequest> models)
        {
            List<dataShuttleRequest> DeletedShuttleRequests = DeleteShuttleRequests(models);

            try
            {
                DbSHM.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbSHM.PartialDeleteMessage(DeletedShuttleRequests, models, DataTableNames.ShuttleRequestsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbSHM.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ShuttleRequestsDataTableRestore(List<dataShuttleRequest> models)
        {
            List<dataShuttleRequest> RestoredShuttleRequests = RestoreShuttleRequests(models);

            try
            {
                DbSHM.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbSHM.PartialRestoreMessage(RestoredShuttleRequests, models, DataTableNames.ShuttleRequestsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbSHM.ErrorMessage(ex.Message));
            }
        }

        private List<dataShuttleRequest> DeleteShuttleRequests(List<dataShuttleRequest> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<dataShuttleRequest> DeletedShuttleRequests = new List<dataShuttleRequest>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbSHM.QueryBuilder(models, Permissions.ShuttleRequest.DeleteGuid, SubmitTypes.Delete, baseQuery);

            var Records = DbSHM.Database.SqlQuery<dataShuttleRequest>(query).ToList();
            foreach (var record in Records)
            {
                DeletedShuttleRequests.Add(DbSHM.DeleteNoAudit(record, ExecutionTime));
            }
            var ShuttleRequestStaffs = DeletedShuttleRequests.SelectMany(a => a.dataShuttleRequestStaff).Where(l => l.Active).ToList();
            foreach (var ShuttleRequestStaff in ShuttleRequestStaffs)
            {
                DbSHM.Delete(ShuttleRequestStaff, ExecutionTime, Permissions.ShuttleRequest.DeleteGuid, DbCMS);
            }

            return DeletedShuttleRequests;
        }

        private List<dataShuttleRequest> RestoreShuttleRequests(List<dataShuttleRequest> models)
        {

            DateTime RestoringTime = DateTime.Now;

            List<dataShuttleRequest> RestoredShuttleRequests = new List<dataShuttleRequest>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbSHM.QueryBuilder(models, Permissions.ShuttleRequest.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Records = DbSHM.Database.SqlQuery<dataShuttleRequest>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveShuttleRequest(record))
                {
                    RestoredShuttleRequests.Add(DbSHM.Restore(record, Permissions.ShuttleRequest.DeleteGuid, Permissions.ShuttleRequest.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            var ShuttleShuttleRequests = RestoredShuttleRequests.SelectMany(x => x.dataShuttleRequestStaff.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var ShuttleShuttleRequest in ShuttleShuttleRequests)
            {
                DbSHM.Restore(ShuttleShuttleRequest, Permissions.ShuttleRequest.DeleteGuid, Permissions.ShuttleRequest.RestoreGuid, RestoringTime, DbCMS);
            }

            return RestoredShuttleRequests;
        }

        private JsonResult ConcurrencyShuttleRequest(Guid PK)
        {
            ShuttleRequestUpdateModel dbModel = new ShuttleRequestUpdateModel();

            var ShuttleRequest = DbSHM.dataShuttleRequest.Where(x => x.ShuttleRequestGUID == PK).FirstOrDefault();
            var dbShuttleRequest = DbSHM.Entry(ShuttleRequest).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbShuttleRequest, dbModel);

            if (ShuttleRequest.dataShuttleRequestRowVersion.SequenceEqual(dbModel.dataShuttleRequestRowVersion))
            {
                return Json(DbSHM.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbSHM, dbModel, "LanguagesContainer"));
        }

        private bool ActiveShuttleRequest(Object model)
        {
            dataShuttleRequest ShuttleRequest = Mapper.Map(model, new dataShuttleRequest());
            int ShuttleRequestValid = DbSHM.dataShuttleRequest.Where(x =>
                                                x.DepartureDate == ShuttleRequest.DepartureDate &&
                                                x.StartLocationGUID == ShuttleRequest.StartLocationGUID &&
                                                x.EndLocationGUID == ShuttleRequest.EndLocationGUID &&
                                                x.ShuttleTravelPurposeGUID == ShuttleRequest.ShuttleTravelPurposeGUID &&
                                                x.ShuttleRequestGUID != ShuttleRequest.ShuttleRequestGUID &&
                                                x.Active).Count();
            if (ShuttleRequestValid > 0)
            {
                ModelState.AddModelError("DepartureDate", "Shuttle Request is already exists");
            }
            if (ShuttleRequest.DepartureDate < DateTime.Now && (ShuttleRequest.StartLocationGUID==null || ShuttleRequest.EndLocationGUID==null))
            {
                ShuttleRequestValid++;
                ModelState.AddModelError("DepartureDate", "Shuttle date is invalid");
            }
            return (ShuttleRequestValid > 0);
        }

        //public FineUploaderResult UploadFiles(FineUpload upload, dataPartnerReportCompiled PartnerReportCompiled)
        //{
        //    ShuttleRequestUpdateModel model
        //   return new FineUploaderResult(true, new { path = Upload(upload, PartnerReportCompiled.PartnerReportCompiledGUID), success = true });
        //}


        [HttpPost]
        public ActionResult CheckWithReturnDate(Guid PK)
        {
            string Message = "0";
            var TravelPurpose = DbSHM.codeShuttleTravelPurpose.Where(x => x.ShuttleTravelPurposeGUID == PK).FirstOrDefault();
            if (TravelPurpose.WithReturnDate.Value)
            {
                Message = "1";
            }
            return Json(new { Message});
        }
        #endregion

        #region Shuttlet Request Staffs

        // [Route("SHM/ShuttleRequestStaffsDataTable/{PK}")]
        public ActionResult ShuttleRequestStaffsDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/SHM/Views/ShuttleRequests/_Index.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<ShuttleRequestStaffDataTable, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<ShuttleRequestStaffDataTable>(DataTable.Filters);
            }

            var Result = (from a in DbSHM.dataShuttleRequestStaff.AsNoTracking().AsExpandable().Where(x => x.ShuttleRequestGUID == PK)
                          join b in DbSHM.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.UserGUID equals b.UserGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()
                          join c in DbSHM.codeReferralStatusLanguage.Where(x => x.LanguageID == LAN && x.Active) on a.ReferralStatusGUID equals c.ReferralStatusGUID into LJ2
                          from R2 in LJ2.DefaultIfEmpty()
                          select new ShuttleRequestStaffDataTable
                          {
                              ShuttleRequestStaffGUID = a.ShuttleRequestStaffGUID,
                              StaffName = R1.FirstName + " " + R1.Surname,
                              Active = a.Active,
                              dataShuttleRequestStaffRowVersion = a.dataShuttleRequestStaffRowVersion,
                              ReferralStatusGUID=a.ReferralStatusGUID,
                              ReferralStatusDescription=R2.Description
                          }).Where(Predicate);

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ShuttleRequestStaffCreate(Guid FK)
        {
            return PartialView("~/Areas/SHM/Views/ShuttleRequests/_ShuttleRequestStaffForm.cshtml",
                new dataShuttleRequestStaff { ShuttleRequestGUID = FK });
        }

        public ActionResult ShuttleRequestStaffUpdate(Guid PK)
        {
            return PartialView("~/Areas/SHM/Views/ShuttleRequests/_ShuttleRequestStaffForm.cshtml", DbSHM.dataShuttleRequestStaff.Find(PK));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ShuttleRequestStaffCreate(dataShuttleRequestStaff model)
        {
            if (!ModelState.IsValid || ActiveShuttleRequestStaff(model)) return PartialView("~/Areas/SHM/Views/ShuttleRequests/_ShuttleRequestStaffForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;
            model.ReferralStatusGUID = ShuttleRequestStaus.Pending;
            DbSHM.CreateNoAudit(model);

            try
            {
                DbSHM.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbSHM.SingleUpdateMessage(DataTableNames.ShuttleRequestStaffsDataTable, DbSHM.PrimaryKeyControl(model), DbSHM.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbSHM.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ShuttleRequestStaffUpdate(dataShuttleRequestStaff model)
        {
            if (!ModelState.IsValid) return PartialView("~/Areas/SHM/Views/ShuttleRequests/_ShuttleRequestStaffForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            DbSHM.UpdateNoAudit(model);

            try
            {
                DbSHM.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbSHM.SingleUpdateMessage(DataTableNames.ShuttleRequestStaffsDataTable,
                    DbSHM.PrimaryKeyControl(model),
                    DbSHM.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyShuttleRequestStaff(model.ShuttleRequestStaffGUID);
            }
            catch (Exception ex)
            {
                return Json(DbSHM.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ShuttleRequestStaffDelete(dataShuttleRequestStaff model)
        {
            List<dataShuttleRequestStaff> Deleteds = DeleteShuttleRequestStaffs(new List<dataShuttleRequestStaff> { model });

            try
            {
                DbSHM.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbSHM.SingleDeleteMessage(Deleteds, DataTableNames.ShuttleRequestStaffsDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyShuttleRequestStaff(model.ShuttleRequestStaffGUID);
            }
            catch (Exception ex)
            {
                return Json(DbSHM.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ShuttleRequestStaffRestore(dataShuttleRequestStaff model)
        {
            if (ActiveShuttleRequestStaff(model))
            {
                return Json(DbSHM.RecordExists());
            }

            List<dataShuttleRequestStaff> Restoreds = RestoreShuttleRequestStaffs(Portal.SingleToList(model));

            try
            {
                DbSHM.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbSHM.SingleRestoreMessage(Restoreds, DataTableNames.ShuttleRequestStaffsDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyShuttleRequestStaff(model.ShuttleRequestStaffGUID);
            }
            catch (Exception ex)
            {
                return Json(DbSHM.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ShuttleRequestStaffsDataTableDelete(List<dataShuttleRequestStaff> models)
        {
            List<dataShuttleRequestStaff> Deleteds = DeleteShuttleRequestStaffs(models);

            try
            {
                DbSHM.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbSHM.PartialDeleteMessage(Deleteds, models, DataTableNames.ShuttleRequestStaffsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbSHM.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ShuttleRequestStaffsDataTableRestore(List<dataShuttleRequestStaff> models)
        {
            List<dataShuttleRequestStaff> Restoreds = RestoreShuttleRequestStaffs(models);

            try
            {
                DbSHM.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbSHM.PartialRestoreMessage(Restoreds, models, DataTableNames.ShuttleRequestStaffsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbSHM.ErrorMessage(ex.Message));
            }
        }

        private List<dataShuttleRequestStaff> DeleteShuttleRequestStaffs(List<dataShuttleRequestStaff> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<dataShuttleRequestStaff> DeletedShuttleRequestStaffs = new List<dataShuttleRequestStaff>();

            string query = DbSHM.QueryBuilder(models, Permissions.ShuttleRequest.DeleteGuid, SubmitTypes.Delete, "");

            var staffs = DbSHM.Database.SqlQuery<dataShuttleRequestStaff>(query).ToList();

            foreach (var staff in staffs)
            {
                DeletedShuttleRequestStaffs.Add(DbSHM.DeleteNoAudit(staff,ExecutionTime));
            }

            return DeletedShuttleRequestStaffs;
        }

        private List<dataShuttleRequestStaff> RestoreShuttleRequestStaffs(List<dataShuttleRequestStaff> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<dataShuttleRequestStaff> Restoreds = new List<dataShuttleRequestStaff>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbSHM.QueryBuilder(models, Permissions.ShuttleRequest.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var staffs = DbSHM.Database.SqlQuery<dataShuttleRequestStaff>(query).ToList();
            foreach (var staff in staffs)
            {
                if (!ActiveShuttleRequestStaff(staff))
                {
                    Restoreds.Add(DbSHM.Restore(staff, Permissions.ShuttleRequest.DeleteGuid, Permissions.ShuttleRequest.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return Restoreds;
        }

        private JsonResult ConcrrencyShuttleRequestStaff(Guid PK)
        {
            dataShuttleRequestStaff dbModel = new dataShuttleRequestStaff();

            var ShuttleRequestStaff = DbSHM.dataShuttleRequestStaff.Where(l => l.ShuttleRequestStaffGUID == PK).FirstOrDefault();
            var dbShuttleRequestStaff = DbSHM.Entry(ShuttleRequestStaff).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbShuttleRequestStaff, dbModel);

            if (ShuttleRequestStaff.dataShuttleRequestStaffRowVersion.SequenceEqual(dbModel.dataShuttleRequestStaffRowVersion))
            {
                return Json(DbSHM.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbSHM, dbModel, "TabContainer"));
        }

        private bool ActiveShuttleRequestStaff(dataShuttleRequestStaff model)
        {
            int ShuttleRequestStaffID = DbSHM.dataShuttleRequestStaff
                                  .Where(x => x.ShuttleRequestGUID == model.ShuttleRequestGUID &&
                                              x.UserGUID == model.UserGUID &&
                                              x.Active).Count();
            if (ShuttleRequestStaffID > 0)
            {
                ModelState.AddModelError("UserGUID", "Staff already exists"); //From resource ?????? Amer  
            }

            return (ShuttleRequestStaffID > 0);
        }

        #endregion
    }
    public class ShuttleRequestStaus
    {
        public readonly static Guid Pending = Guid.Parse("a73e5015-896a-42cd-8845-d5fce5b80e15");
        public readonly static Guid Canceled = Guid.Parse("4adbdc93-b118-4d23-acf8-cae98cfc959b");
        public readonly static Guid Completed = Guid.Parse("9ea61a3f-ea90-4f4b-b573-0fe3660dcc72");
        public readonly static Guid Progressing = Guid.Parse("e58a961c-d300-4a5a-8df4-f75b5e873cd4");
    }
}