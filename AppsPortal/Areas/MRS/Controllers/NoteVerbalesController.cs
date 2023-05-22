using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.ViewModels;
using AutoMapper;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OfficeOpenXml;
using System.IO;
using MRS_DAL.Model;
using FineUploader;
using System.Configuration;
using AppsPortal.Library.MimeDetective;

namespace AppsPortal.Areas.MRS.Controllers
{
    public class NoteVerbalesController : MRSBaseController
    {
        #region Note Verbales
        public ActionResult Index()
        {
            return View();
        }

        [Route("MRS/NoteVerbales/")]
        public ActionResult NoteVerbalesIndex()
        {
            if (!CMS.HasAction(Permissions.NoteVerbale.Access, Apps.MRS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/MRS/Views/NoteVerbales/Index.cshtml");
        }
       

        [Route("MRS/NoteVerbalesDataTable/")]
        public JsonResult NoteVerbalesDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<NoteVerbalesDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<NoteVerbalesDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.NoteVerbale.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).Distinct().ToList();

            var All = (from a in DbMRS.dataNoteVerbale.AsExpandable()//.Where(x => AuthorizedList.Contains(x.DutyStationGUID.ToString()))
                       join b in DbMRS.codeLocationsLanguages.Where(x=>x.LanguageID==LAN && x.Active) on a.LocationGUID equals b.LocationGUID into LJ1
                       from R1 in LJ1.DefaultIfEmpty()
                       join c in DbMRS.codeReferralStatusLanguage on a.ReferralStatusGUID equals c.ReferralStatusGUID into LJ2
                       from R2 in LJ2.DefaultIfEmpty()
                       join d in DbMRS.codeDutyStationsLanguages.Where(x=>x.Active && x.LanguageID==LAN) on a.DutyStationGUID equals d.DutyStationGUID into LJ3
                       from R3 in LJ3.DefaultIfEmpty()
                       join e in DbMRS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.OrganizationMissionTypeGUID equals e.ValueGUID into LJ4
                       from R4 in LJ4.DefaultIfEmpty()
                       join f in DbMRS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.MissionCategoryGUID equals f.ValueGUID into LJ5
                       from R5 in LJ5.DefaultIfEmpty()
                       join j in DbMRS.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.TeamLeaderGUID equals j.UserGUID into LJ6
                       from R6 in LJ6.DefaultIfEmpty()
                       select new NoteVerbalesDataTableModel
                       {
                           NoteVerbaleGUID = a.NoteVerbaleGUID,
                           LocationGUID=a.LocationGUID,
                           LocationDescription=R1.LocationDescription,
                           NoteVerbaleDate=a.NoteVerbaleDate,
                           Reference=a.Reference!=""? a.Reference: "No Reference",
                           ReferralStatusDescription=R2.Description,
                           ReferralStatusGUID=R2.ReferralStatusGUID.ToString(),
                           VisitDate=a.VisitDate,
                           DutyStationGUID=a.DutyStationGUID.ToString(),
                           DutyStationDescription=R3.DutyStationDescription,
                           MissionAccomplished=a.MissionAccomplished,
                           MissionReport=a.MissionReport,
                           MissionCategoryGUID=a.MissionCategoryGUID.ToString(),
                           MissionCategoryDescription=R5.ValueDescription,
                           OrganizationMissionTypeGUID=a.OrganizationMissionTypeGUID.ToString(),
                           OrganizationMissionTypeDescription=R4.ValueDescription,
                           OrganizationInstanceGUID=a.OrganizationInstanceGUID,
                           RescheduledDate=a.RescheduledDate,
                           ResponseDateMFA=a.ResponseDateMFA,
                           TeamLeaderGUID=a.TeamLeaderGUID.ToString(),
                           TeamLeaderName=R6.FirstName +" "+ R6.Surname,
                           dataNoteVerbaleRowVersion = a.dataNoteVerbaleRowVersion,
                           Active = a.Active,
                           Map = false
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<NoteVerbalesDataTableModel> Result = Mapper.Map<List<NoteVerbalesDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("MRS/NoteVerbales/Create/")]
        public ActionResult NoteVerbaleCreate()
        {
            if (!CMS.HasAction(Permissions.NoteVerbale.Create, Apps.MRS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var userProfile = DbCMS.userProfiles.Where(x => x.UserProfileGUID == UserProfileGUID).FirstOrDefault();
            if (userProfile != null)
            {
                return View("~/Areas/MRS/Views/NoteVerbales/NoteVerbale.cshtml", new NoteVerbaleUpdateModel()
                { OrganizationInstanceGUID=userProfile.OrganizationInstanceGUID,
                  DutyStationGUID =userProfile.DutyStationGUID
                });
            }                                        
            else { return View("~/Areas/MRS/Views/NoteVerbales/NoteVerbale.cshtml", new NoteVerbaleUpdateModel()); }
        }

        [Route("MRS/NoteVerbales/Update/{PK}")]
        public ActionResult NoteVerbaleUpdate(string PK)
        {
            Guid PK1 =new Guid( PK.Split(';')[0]);
            bool Map = bool.Parse(PK.Split(';')[1]);
            if (!CMS.HasAction(Permissions.NoteVerbale.Access, Apps.MRS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var model = (from a in DbMRS.dataNoteVerbale.WherePK(PK1)
                         select new NoteVerbaleUpdateModel
                         {
                             NoteVerbaleGUID = a.NoteVerbaleGUID,
                             LocationGUID = a.LocationGUID,
                             VisitDate = a.VisitDate,
                             Reference = a.Reference,
                             NoteVerbaleDate = a.NoteVerbaleDate,
                             VisitPurpose_EN = a.VisitPurpose_EN,
                             VisitPurpose_AR = a.VisitPurpose_AR,
                             ReferralStatusGUID = a.ReferralStatusGUID,
                             DutyStationGUID=a.DutyStationGUID,
                             OrganizationInstanceGUID=a.OrganizationInstanceGUID,
                             OrganizationMissionTypeGUID=a.OrganizationMissionTypeGUID,
                             MissionAccomplished=a.MissionAccomplished,
                             MissionReport=a.MissionReport,
                             MissionCategoryGUID=a.MissionCategoryGUID,
                             TeamLeaderGUID=a.TeamLeaderGUID,
                             RescheduledDate=a.RescheduledDate,
                             ResponseDateMFA=a.ResponseDateMFA,
                             Active = a.Active,
                             dataNoteVerbaleRowVersion = a.dataNoteVerbaleRowVersion,
                             Map=Map
                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("NoteVerbale", "NoteVerbales", new { Area = "MRS" }));

            return View("~/Areas/MRS/Views/NoteVerbales/NoteVerbale.cshtml", model);
        }



        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult NoteVerbaleCreate(NoteVerbaleUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.NoteVerbale.Create, Apps.MRS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveNoteVerbale(model)) return PartialView("~/Areas/MRS/Views/NoteVerbales/_NoteVerbaleForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            dataNoteVerbale NoteVerbale = Mapper.Map(model, new dataNoteVerbale());
            NoteVerbale.NoteVerbaleGUID = EntityPK;
            DbMRS.Create(NoteVerbale, Permissions.NoteVerbale.CreateGuid, ExecutionTime, DbCMS);

            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.NoteVerbaleStaffsDataTable , "NoteVerbaleStaffs", "TabContainer"));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.NoteVerbale.Create, Apps.MRS, new UrlHelper(Request.RequestContext).Action("Create", "NoteVerbales", new { Area = "MRS" })), Container = "NoteVerbaleFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.NoteVerbale.Update, Apps.MRS), Container = "NoteVerbaleFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.NoteVerbale.Delete, Apps.MRS), Container = "NoteVerbaleFormControls" });
            try
            {
                DbMRS.SaveChanges();
                DbCMS.SaveChanges();

                return Json(DbMRS.SingleCreateMessage(DbMRS.PrimaryKeyControl(NoteVerbale),
                    DbMRS.RowVersionControls(new List<dataNoteVerbale>() { NoteVerbale }), Partials, "", UIButtons));

            }
            catch (Exception ex)
            {
                return Json(DbMRS.ErrorMessage(ex.Message));
            }
        }

     
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult NoteVerbaleUpdate(NoteVerbaleUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.NoteVerbale.Update, Apps.MRS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid || ActiveNoteVerbale(model)) return PartialView("~/Areas/MRS/Views/NoteVerbales/_NoteVerbaleForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            dataNoteVerbale NoteVerbale = Mapper.Map(model, new dataNoteVerbale());
            DbMRS.Update(NoteVerbale, Permissions.NoteVerbale.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbMRS.SaveChanges();
                DbCMS.SaveChanges();

                return Json(DbMRS.SingleUpdateMessage(null, null, DbMRS.RowVersionControls(new List<dataNoteVerbale>() { NoteVerbale })));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyNoteVerbale(model.NoteVerbaleGUID);
            }
            catch (Exception ex)
            {
                return Json(DbMRS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult NoteVerbaleDelete(dataNoteVerbale model)
        {
            if (!CMS.HasAction(Permissions.NoteVerbale.Delete, Apps.MRS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataNoteVerbale> DeletedNoteVerbale = DeleteNoteVerbales(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.RestoreButton(Permissions.NoteVerbale.Restore, Apps.MRS), Container = "NoteVerbaleFormControls" });

            try
            {
                int CommitedRows = DbMRS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbMRS.SingleDeleteMessage(CommitedRows, DeletedNoteVerbale.FirstOrDefault(), "TabContainer,TabContainer1", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyNoteVerbale(model.NoteVerbaleGUID);
            }
            catch (Exception ex)
            {
                return Json(DbMRS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult NoteVerbaleRestore(dataNoteVerbale model)
        {
            if (!CMS.HasAction(Permissions.NoteVerbale.Restore, Apps.MRS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (ActiveNoteVerbale(model))
            {
                return Json(DbMRS.RecordExists());
            }

            List<dataNoteVerbale> RestoredNoteVerbales = RestoreNoteVerbales(Portal.SingleToList(model));

            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.NoteVerbale.Create, Apps.MRS, new UrlHelper(Request.RequestContext).Action("NoteVerbaleCreate", "NoteVerbales", new { Area = "MRS" })), Container = "NoteVerbaleFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.NoteVerbale.Update, Apps.MRS), Container = "NoteVerbaleFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.NoteVerbale.Delete, Apps.MRS), Container = "NoteVerbaleFormControls" });

            try
            {
                int CommitedRows = DbMRS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbMRS.SingleRestoreMessage(CommitedRows, RestoredNoteVerbales, DbMRS.PrimaryKeyControl(RestoredNoteVerbales.FirstOrDefault()),
                    Url.Action(DataTableNames.NoteVerbaleStaffsDataTable, "NoteVerbaleStaffs") , "TabContainer", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyNoteVerbale(model.NoteVerbaleGUID);
            }
            catch (Exception ex)
            {
                return Json(DbMRS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult NoteVerbalesDataTableDelete(List<dataNoteVerbale> models)
        {
            if (!CMS.HasAction(Permissions.NoteVerbale.Delete, Apps.MRS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataNoteVerbale> DeletedNoteVerbales = DeleteNoteVerbales(models);

            try
            {
                DbMRS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbMRS.PartialDeleteMessage(DeletedNoteVerbales, models, DataTableNames.NoteVerbalesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbMRS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult NoteVerbalesDataTableRestore(List<dataNoteVerbale> models)
        {
            if (!CMS.HasAction(Permissions.NoteVerbale.Restore, Apps.MRS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            List<dataNoteVerbale> RestoredNoteVerbales = RestoreNoteVerbales(models);

            try
            {
                DbMRS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbMRS.PartialRestoreMessage(RestoredNoteVerbales, models, DataTableNames.NoteVerbalesDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbMRS.ErrorMessage(ex.Message));
            }
        }

        private List<dataNoteVerbale> DeleteNoteVerbales(List<dataNoteVerbale> models)
        {
            DateTime ExecutionTime = DateTime.Now;
            List<dataNoteVerbale> DeletedNoteVerbales = new List<dataNoteVerbale>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";// "SELECT NoteVerbaleGUID,CONVERT(varchar(50), OrganizationInstanceGUID) as C2 ,dataNoteVerbaleRowVersion FROM ams.dataNoteVerbale where NoteVerbaleGUID in (" + string.Join(",", models.Select(x => "'" + x.NoteVerbaleGUID + "'").ToArray()) + ")";


            string query = DbMRS.QueryBuilder(models, Permissions.NoteVerbale.DeleteGuid, SubmitTypes.Delete, baseQuery);

            var Records = DbMRS.Database.SqlQuery<dataNoteVerbale>(query).ToList();
            foreach (var record in Records)
            {
                DeletedNoteVerbales.Add(DbMRS.Delete(record, ExecutionTime, Permissions.NoteVerbale.DeleteGuid, DbCMS));
            }

            var NoteVerbaleStaffs = DeletedNoteVerbales.SelectMany(a => a.dataNoteVerbaleStaff).Where(l => l.Active).ToList();
            foreach (var NoteVerbaleStaff in NoteVerbaleStaffs)
            {
                DbMRS.Delete(NoteVerbaleStaff, ExecutionTime, Permissions.NoteVerbale.DeleteGuid, DbCMS);
            }
            var NoteVerbaleVehicals = DeletedNoteVerbales.SelectMany(a => a.dataNoteVerbaleVehicle).Where(l => l.Active).ToList();
            foreach (var NoteVerbaleVehical in NoteVerbaleVehicals)
            {
                DbMRS.Delete(NoteVerbaleVehical, ExecutionTime, Permissions.NoteVerbale.DeleteGuid, DbCMS);
            }
            var NoteVerbaleOrganizations = DeletedNoteVerbales.SelectMany(a => a.dataNoteVerbaleOrganization).Where(l => l.Active).ToList();
            foreach (var NoteVerbaleOrganization in NoteVerbaleOrganizations)
            {
                DbMRS.Delete(NoteVerbaleOrganization, ExecutionTime, Permissions.NoteVerbale.DeleteGuid, DbCMS);
            }
            return DeletedNoteVerbales;
        }

        private List<dataNoteVerbale> RestoreNoteVerbales(List<dataNoteVerbale> models)
        {

            DateTime RestoringTime = DateTime.Now;

            List<dataNoteVerbale> RestoredNoteVerbales = new List<dataNoteVerbale>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = ""; //"SELECT NoteVerbaleGUID,CONVERT(varchar(50), OrganizationInstanceGUID) as C2 ,dataNoteVerbaleRowVersion FROM ams.dataNoteVerbale where NoteVerbaleGUID in (" + string.Join(",", models.Select(x => "'" + x.NoteVerbaleGUID + "'").ToArray()) + ")";

            string query = DbMRS.QueryBuilder(models, Permissions.NoteVerbale.DeleteGuid, SubmitTypes.Restore, baseQuery);

            var Records = DbMRS.Database.SqlQuery<dataNoteVerbale>(query).ToList();
            foreach (var record in Records)
            {
                if (!ActiveNoteVerbale(record))
                {
                    RestoredNoteVerbales.Add(DbMRS.Restore(record, Permissions.NoteVerbale.DeleteGuid, Permissions.NoteVerbale.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            var NoteVerbaleStaffs = RestoredNoteVerbales.SelectMany(x => x.dataNoteVerbaleStaff.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var NoteVerbaleStaff in NoteVerbaleStaffs)
            {
                DbMRS.Restore(NoteVerbaleStaff, Permissions.NoteVerbale.DeleteGuid, Permissions.NoteVerbale.RestoreGuid, RestoringTime, DbCMS);
            }
            var NoteVerbaleVehicals = RestoredNoteVerbales.SelectMany(x => x.dataNoteVerbaleVehicle.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var NoteVerbaleVehical in NoteVerbaleVehicals)
            {
                DbMRS.Restore(NoteVerbaleVehical, Permissions.NoteVerbale.DeleteGuid, Permissions.NoteVerbale.RestoreGuid, RestoringTime, DbCMS);
            }
            var NoteVerbaleOrganizations = RestoredNoteVerbales.SelectMany(x => x.dataNoteVerbaleOrganization.Where(xl => xl.DeletedOn == x.DeletedOn)).ToList();
            foreach (var NoteVerbaleOrganization in NoteVerbaleOrganizations)
            {
                DbMRS.Restore(NoteVerbaleOrganization, Permissions.NoteVerbale.DeleteGuid, Permissions.NoteVerbale.RestoreGuid, RestoringTime, DbCMS);
            }

            return RestoredNoteVerbales;
        }

        private JsonResult ConcurrencyNoteVerbale(Guid PK)
        {
            NoteVerbaleUpdateModel dbModel = new NoteVerbaleUpdateModel();

            var NoteVerbale = DbMRS.dataNoteVerbale.Where(x => x.NoteVerbaleGUID == PK).FirstOrDefault();
            var dbNoteVerbale = DbMRS.Entry(NoteVerbale).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbNoteVerbale, dbModel);

            var Language = DbMRS.dataNoteVerbale.Where(x => x.NoteVerbaleGUID == PK).Where(x => x.Active == true).FirstOrDefault();
            var dbLanguage = DbMRS.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (NoteVerbale.dataNoteVerbaleRowVersion.SequenceEqual(dbModel.dataNoteVerbaleRowVersion) && Language.dataNoteVerbaleRowVersion.SequenceEqual(dbModel.dataNoteVerbaleRowVersion))
            {
                return Json(DbMRS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbMRS, dbModel, "TabContainer"));
        }

        private bool ActiveNoteVerbale(Object model)
        {
            dataNoteVerbale NoteVerbale = Mapper.Map(model, new dataNoteVerbale());
            int NoteVerbaleDescription = 0;
            if (NoteVerbaleDescription > 0)
            {
                ModelState.AddModelError("NoteVerbaleDescription", "NoteVerbale is already exists");
            }
            return (NoteVerbaleDescription > 0);
        }
        [HttpGet]
        public ActionResult NoteVerbaleCalendar()
        {
            return View();
        }

        //[HttpPost]
        //public ActionResult GetNoteVerbalesCalendarData(DateTime start, DateTime end)
        //{
        //    //Access is authorized by Access Action
        //    //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.Appointment.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).Distinct().ToList();

        //    var NoteVerbales = (from a in DbMRS.dataNoteVerbale.Where(x => x.DepartureDateTime >= start && x.DepartureDateTime <= end && x.Active)
        //                    join b in DbMRS.dataNoteVerbaleRoute.Where(x => x.Active) on a.NoteVerbaleGUID equals b.NoteVerbaleGUID
        //                    join c in DbMRS.codeLocationsLanguages.Where(x => x.Active && x.LanguageID == LAN) on b.StartLocationGUID equals c.LocationGUID into LJ1
        //                    from R1 in LJ1.DefaultIfEmpty()
        //                    join d in DbMRS.codeLocationsLanguages.Where(x => x.Active && x.LanguageID == LAN) on b.EndLocationGUID equals d.LocationGUID into LJ2
        //                    from R2 in LJ2.DefaultIfEmpty()
        //                    select new CalendarEvents
        //                    {
        //                        EventId = a.NoteVerbaleGUID,
        //                        EventStartDate = a.DepartureDateTime,
        //                        EventEndDate = a.DepartureDateTime,
        //                        Title = R1.LocationDescription + " - " + R2.LocationDescription,
        //                        AllDayEvent = false,
        //                        ReferralStatus = a.ReferralStatusGUID.ToString()
        //                    }).ToList();
        //    return Json(new { CalendarEvents = NoteVerbales });
        //}


        [Route("MRS/NoteVerbales/Upload/{PK}")]
        public ActionResult Upload(Guid PK)
        {
            var model = (from a in DbMRS.dataNoteVerbale.WherePK(PK)
                         select new NoteVerbaleUpdateModel
                         {
                             NoteVerbaleGUID = a.NoteVerbaleGUID,
                             Active = a.Active
                         }).FirstOrDefault();

            if (model == null) throw new HttpException((int)HttpStatusCode.NotFound, Url.Action("NoteVerbale", "NoteVerbales", new { Area = "MRS" }));

            return PartialView("~/Areas/MRS/Views/NoteVerbales/_NoteVerbaleUploadFile.cshtml", model);
        }

        [HttpPost]
        public FineUploaderResult UploadFiles(FineUpload upload, Guid PK)
        {
            string error = "";
            if (FileTypeValidator.IsPDF(upload.InputStream))
            {
                string FilePath = ConfigurationManager.AppSettings["DataFolder"] + "\\Uploads\\MRS\\NoteVerbale\\" + PK + ".pdf";
                try
                {
                    upload.SaveAs(FilePath);
                    var req = DbMRS.dataNoteVerbale.Where(x => x.NoteVerbaleGUID == PK).FirstOrDefault();
                    DbMRS.SaveChanges();
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                }
            }
            return new FineUploaderResult((error == "" ? true : false), new { Error = error, path = "", success = (error == "" ? true : false) });
        }


      

        [Route("MRS/NoteVerbalesMapIndex/{FK}")]
        public ActionResult NoteVerbalesIndex(string FK)
        {
            if (!CMS.HasAction(Permissions.NoteVerbale.Access, Apps.MRS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Areas/MRS/Views/NoteVerbales/_IndexMap.cshtml", new MasterRecordStatus() { ParentGUID = new Guid(FK) });
        }
        public ActionResult NoteVerbalesDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/MRS/Views/NoteVerbales/_IndexMap.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<NoteVerbalesDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<NoteVerbalesDataTableModel>(DataTable.Filters);
            }
            //Access is authorized by Access Action
            //List<string> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.NoteVerbale.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).Distinct().ToList();

            var All = (from a in DbMRS.dataNoteVerbale.Where(x=> x.LocationGUID == PK && x.Active)//.Where(x => AuthorizedList.Contains(x.DutyStationGUID.ToString()))
                       join b in DbMRS.codeLocationsLanguages.Where(x => x.LanguageID == LAN && x.Active ) on a.LocationGUID equals b.LocationGUID into LJ1
                       from R1 in LJ1.DefaultIfEmpty()
                       join c in DbMRS.codeReferralStatusLanguage on a.ReferralStatusGUID equals c.ReferralStatusGUID into LJ2
                       from R2 in LJ2.DefaultIfEmpty()
                       join d in DbMRS.codeDutyStationsLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.DutyStationGUID equals d.DutyStationGUID into LJ3
                       from R3 in LJ3.DefaultIfEmpty()
                       join e in DbMRS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.OrganizationMissionTypeGUID equals e.ValueGUID into LJ4
                       from R4 in LJ4.DefaultIfEmpty()
                       join f in DbMRS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.MissionCategoryGUID equals f.ValueGUID into LJ5
                       from R5 in LJ5.DefaultIfEmpty()
                       join j in DbMRS.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == LAN) on a.TeamLeaderGUID equals j.UserGUID into LJ6
                       from R6 in LJ6.DefaultIfEmpty()
                       select new NoteVerbalesDataTableModel
                       {
                           NoteVerbaleGUID = a.NoteVerbaleGUID,
                           LocationGUID = a.LocationGUID,
                           LocationDescription = R1.LocationDescription,
                           NoteVerbaleDate = a.NoteVerbaleDate,
                           Reference = a.Reference != "" ? a.Reference : "No Reference",
                           ReferralStatusDescription = R2.Description,
                           ReferralStatusGUID = R2.ReferralStatusGUID.ToString(),
                           VisitDate = a.VisitDate,
                           DutyStationGUID = a.DutyStationGUID.ToString(),
                           DutyStationDescription = R3.DutyStationDescription,
                           MissionAccomplished = a.MissionAccomplished,
                           MissionReport = a.MissionReport,
                           MissionCategoryGUID = a.MissionCategoryGUID.ToString(),
                           MissionCategoryDescription = R5.ValueDescription,
                           OrganizationMissionTypeGUID = a.OrganizationMissionTypeGUID.ToString(),
                           OrganizationMissionTypeDescription = R4.ValueDescription,
                           OrganizationInstanceGUID = a.OrganizationInstanceGUID,
                           RescheduledDate = a.RescheduledDate,
                           ResponseDateMFA = a.ResponseDateMFA,
                           TeamLeaderGUID = a.TeamLeaderGUID.ToString(),
                           TeamLeaderName = R6.FirstName + " " + R6.Surname,
                           dataNoteVerbaleRowVersion = a.dataNoteVerbaleRowVersion,
                           Active = a.Active,
                          Map=true
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<NoteVerbalesDataTableModel> Result = Mapper.Map<List<NoteVerbalesDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}