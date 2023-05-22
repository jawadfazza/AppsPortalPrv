using AHD_DAL.Model;
using AHD_DAL.ViewModels;
using AppsPortal.Areas.AHD.Service;
using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.ViewModels;
using AutoMapper;
using FineUploader;
using iTextSharp.text;
using iTextSharp.text.pdf;
using LinqKit;
using OfficeOpenXml;
using RES_Repo.Globalization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;


namespace AppsPortal.Areas.AHD.Controllers
{
    public class StaffDelegationController : AHDBaseController
    {
        // GET: AHD/StaffDelegation


        #region   Delegation Manager
        public ActionResult Index()
        {
            if (!CMS.HasAction(Permissions.InternationalStaffRestAndRecuperationLeave.Access, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }

            return View("~/Areas/AHD/Views/ShuttleDelegationManager/Index.cshtml");
        }

        [Route("AHD/DelegationManagerIndex/")]
        public ActionResult ShuttleDelegationIndex()
        {
            if (!CMS.HasAction(Permissions.InternationalStaffRestAndRecuperationLeave.Access, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }

            return View("~/Areas/AHD/Views/ShuttleDelegationManager/Index.cshtml");
        }
        [HttpPost]
        public ActionResult GetWorkingDay()
        {
            var _user = DbCMS.StaffCoreData.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
            Guid DutyStationConfigurationGUID = DbCMS.codeDutyStationsConfigurations.Where(x => x.DutyStationGUID == _user.DutyStationGUID).FirstOrDefault().DutyStationConfigurationGUID;

            var WorkingDay = (from a in DbCMS.codeWorkingDaysConfigurations.Where(x => x.Active && x.DutyStationConfigurationGUID == DutyStationConfigurationGUID)
                              join b in DbCMS.codeTablesValues on a.DayGUID equals b.ValueGUID
                              select new WorkDay { Day = b.SortID.Value }).ToList();
            return Json(new JsonReturn { WorkDays = WorkingDay });
        }


        [HttpPost]
        public ActionResult GetCalendarDataFromDatabase(DateTime start, DateTime end)
        {
            //Access is authorized by Access Action Department
            //List<string> AuthorizedListDepartment = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.AppointmentType.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).Distinct().ToList();
            ////Access is authorized by Access Action DutyStation
            //List<string> AuthorizedListDutyStation = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == Permissions.InternationalStaffAttendancePresence.AccessGuid && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).Distinct().ToList();

            var userProfiles = DbCMS.userProfiles.Where(x => x.UserProfileGUID == UserProfileGUID).FirstOrDefault();
            var Events = (from a in DbAHD.dataBlomShuttleDelegationDate
                          .Where(x => x.StartDate >= start && x.Active).ToList()

                          select new CalendarEvents
                          {
                              EventId = a.BlomShuttleDelegationDateGUID,

                              EventStartDate = a.StartDate,
                              EventEndDate = a.StartDate.Value.AddDays(1),
                              Title = "Shuttle" + "- " + a.StartDate.Value.Day + "-" + a.StartDate.Value.Year,
                              EventDescription = a.Comments,
                              AllDayEvent = false,

                              backgroundColor = "#0066CC",


                              borderColor = "#00c0ef",


                          }).ToList();
            return Json(new { CalendarEvents = Events }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ShuttleDelegationCreate()
        {
            if (!CMS.HasAction(Permissions.StaffShuttleDelegation.Create, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }

            BlomShuttleDelegationDateUpdateModel model = new BlomShuttleDelegationDateUpdateModel();

            return View("~/Areas/AHD/Views/ShuttleDelegationManager/ShuttleDelegationManagerForm.cshtml", model);

            //return PartialView("~/Areas/AHD/Views/AppointmentTypeCalendars/_AppointmentTypeCalendarModal.cshtml",
            //    new AppointmentTypeCalenderUpdateModel { OrganizationInstanceGUID = userProfiles.OrganizationInstanceGUID, DutyStationGUID = userProfiles.DutyStationGUID, EventEachDay = true });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ShuttleDelegationCreate(BlomShuttleDelegationDateUpdateModel model)
        {
            if (!CMS.HasAction(Permissions.StaffShuttleDelegation.Create, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            if (!ModelState.IsValid) return PartialView("~/Areas/AHD/Views/ItemModels/_ItemModelForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();

            dataBlomShuttleDelegationDate _myModel = Mapper.Map(model, new dataBlomShuttleDelegationDate());
            _myModel.BlomShuttleDelegationDateGUID = EntityPK;
            _myModel.CreateDate = ExecutionTime;
            _myModel.CreateByGUID = UserGUID;
            DbAHD.Create(_myModel, Permissions.StaffShuttleDelegation.CreateGuid, ExecutionTime, DbCMS);
            string folderName = "";
            var folderPath = Server.MapPath("~\\Areas\\AHD\\Templates\\BLOMDelegation\\StaffDelegations\\");
            folderName = @"~/Areas/AHD/Templates/BLOMDelegation/StaffDelegations/" + EntityPK.ToString();
            Directory.CreateDirectory(folderPath + EntityPK.ToString());



            List<PartialViewModel> Partials = new List<PartialViewModel>();
            //codeSitemapsLanguages Language = Mapper.Map(model, new codeSitemapsLanguages());
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.BlomShuttleDelegationTravelerDataTable, ControllerContext, "TravelersFormControls"));
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.BlomShuttleDelegationStaffRequestDataTable, ControllerContext, "StaffRequestsFormControls"));


            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.StaffShuttleDelegation.Create, Apps.AHD, new UrlHelper(Request.RequestContext).Action("Create", "StaffDelegation", new { Area = "AHD" })), Container = "StaffShuttleDelegationFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.StaffShuttleDelegation.Update, Apps.AHD), Container = "StaffShuttleDelegationFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.StaffShuttleDelegation.Delete, Apps.AHD), Container = "StaffShuttleDelegationFormControls" });

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.SingleCreateMessage(DbAHD.PrimaryKeyControl(_myModel), DbAHD.RowVersionControls(_myModel, _myModel), Partials, "", UIButtons));
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }





        public ActionResult ShuttleDelegationUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.StaffShuttleDelegation.Update, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            BlomShuttleDelegationDateUpdateModel model = new BlomShuttleDelegationDateUpdateModel();
            var Event = DbAHD.dataBlomShuttleDelegationDate.Find(PK);
            Mapper.Map(Event, model);

            return View("~/Areas/AHD/Views/ShuttleDelegationManager/ShuttleDelegationManagerForm.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ShuttleDelegationUpdate(dataBlomShuttleDelegationDate model)
        {
            if (!CMS.HasAction(Permissions.StaffShuttleDelegation.Update, Apps.AHD))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            if (!ModelState.IsValid) return View("~/Areas/AHD/Views/ShuttleDelegationManager/ShuttleDelegationManagerForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(Guid.NewGuid(), DataTableNames.BlomShuttleDelegationDateDataTable, ControllerContext, "InternationalTempStaffRAndRDatesDataTable"));
            List<UIButtons> UIButtons = new List<UIButtons>();

            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.StaffShuttleDelegation.Update, Apps.AHD), Container = "ItemModelDetailFormControls" });


            DbAHD.Update(model, Permissions.StaffShuttleDelegation.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.SingleCreateMessage(DbAHD.PrimaryKeyControl(model), DbAHD.RowVersionControls(model, model), Partials, "", UIButtons));
            }
            catch (DbUpdateConcurrencyException)
            {
                return Json(DbAHD.ErrorMessage("error"));
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }
        #endregion


        #region Traverlers

        public ActionResult BlomShuttleDelegationTravelerDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/AHD/Views/ShuttleDelegationManager/_TravelerDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<ShuttleDelegationTravelerDataTable, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<ShuttleDelegationTravelerDataTable>(DataTable.Filters);
            }

            var Result = (from a in DbAHD.dataBlomShuttleDelegationTraveler.AsExpandable().Where(x => x.BlomShuttleDelegationDateGUID == PK && x.Active == true)
                          join b in DbAHD.codeTablesValuesLanguages.AsExpandable().Where(x => x.Active && x.LanguageID == LAN) on a.DelgationStaffTypeGUID equals b.ValueGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()
                          select new ShuttleDelegationTravelerDataTable
                          {
                              BlomShuttleDelegationTravelerGUID = a.BlomShuttleDelegationTravelerGUID.ToString(),
                              BlomShuttleDelegationDateGUID = a.BlomShuttleDelegationDateGUID.ToString(),
                              StaffGUID = a.StaffGUID.ToString(),
                              TravelerName = a.TravelerName,
                              NationalIDNumber = a.NationalIDNumber,
                              //SyrianIDNumber = a.SyrianIDNumber,
                              DelgationStaffTypeGUID = a.DelgationStaffTypeGUID.ToString(),

                              Active = a.Active,

                              dataBlomShuttleDelegationTravelerRowVersion = a.dataBlomShuttleDelegationTravelerRowVersion
                          }).Where(Predicate);

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }



        public ActionResult BlomShuttleDelegationTravelerCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.StaffShuttleDelegation.Update, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            return PartialView("~/Areas/AHD/Views/ShuttleDelegationManager/_TravelerUpdateModal.cshtml",
                new ShuttleDelegationTravelerUpdateModel { BlomShuttleDelegationDateGUID = FK });
        }



        public ActionResult BlomShuttleDelegationTravelerUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.StaffShuttleDelegation.Update, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            ShuttleDelegationTravelerUpdateModel model = DbAHD.dataBlomShuttleDelegationTraveler.Where(x => x.BlomShuttleDelegationTravelerGUID == PK).Select(f => new ShuttleDelegationTravelerUpdateModel
            {

                BlomShuttleDelegationTravelerGUID = (Guid)f.BlomShuttleDelegationTravelerGUID,
                BlomShuttleDelegationDateGUID = f.BlomShuttleDelegationDateGUID,
                StaffGUID = f.StaffGUID,
                TravelerName = f.TravelerName,
                NationalIDNumber = f.NationalIDNumber,
                //SyrianIDNumber = f.SyrianIDNumber,
                DelgationStaffTypeGUID = f.DelgationStaffTypeGUID,
                Comments = f.Comments,


                Active = f.Active

            }).FirstOrDefault();
            return PartialView("~/Areas/AHD/Views/ShuttleDelegationManager/_TravelerUpdateModal.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult BlomShuttleDelegationTravelerCreate(ShuttleDelegationTravelerUpdateModel mymodel)
        {
            if (!CMS.HasAction(Permissions.StaffShuttleDelegation.Create, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            if (!ModelState.IsValid || (mymodel.StaffGUID == null || mymodel.DelgationStaffTypeGUID == null)) return PartialView("~/Areas/AHD/Views/ShuttleDelegationManager/_TravelerUpdateModal.cshtml", mymodel);

            DateTime ExecutionTime = DateTime.Now;
            dataBlomShuttleDelegationTraveler model = Mapper.Map(mymodel, new dataBlomShuttleDelegationTraveler());
            string ar = "AR";
            var _staff = DbAHD.userPersonalDetailsLanguage.Where(x => x.UserGUID == mymodel.StaffGUID && x.Active && x.LanguageID == ar).FirstOrDefault();
            model.TravelerName = _staff.FirstName + " " + _staff.Surname;


            model.Comments = mymodel.Comments;


            DbAHD.Create(model, Permissions.StaffShuttleDelegation.CreateGuid, ExecutionTime, DbCMS);

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.SingleUpdateMessage(DataTableNames.BlomShuttleDelegationTravelerDataTable, DbAHD.PrimaryKeyControl(model), DbAHD.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult BlomShuttleDelegationTravelerUpdate(ShuttleDelegationTravelerUpdateModel mymodel)
        {
            if (!CMS.HasAction(Permissions.StaffShuttleDelegation.Update, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            if (!ModelState.IsValid || (mymodel.StaffGUID == null || mymodel.DelgationStaffTypeGUID == null)) return PartialView("~/Areas/AHD/Views/ShuttleDelegationManager/_TravelerUpdateModal.cshtml", mymodel);
            var model = DbAHD.dataBlomShuttleDelegationTraveler.Where(x => x.BlomShuttleDelegationTravelerGUID == mymodel.BlomShuttleDelegationTravelerGUID).FirstOrDefault();

            DateTime ExecutionTime = DateTime.Now;
            model.DelgationStaffTypeGUID = mymodel.DelgationStaffTypeGUID;
            model.StaffGUID = mymodel.StaffGUID;
            model.NationalIDNumber = mymodel.NationalIDNumber;
            //model.SyrianIDNumber = mymodel.SyrianIDNumber;
            model.Comments = mymodel.Comments;
            string ar = "AR";
            var _staff = DbAHD.userPersonalDetailsLanguage.Where(x => x.UserGUID == mymodel.StaffGUID && x.Active && x.LanguageID == ar).FirstOrDefault();
            model.TravelerName = _staff.FirstName + " " + _staff.Surname;
            DbAHD.Update(model, Permissions.StaffShuttleDelegation.UpdateGuid, ExecutionTime, DbCMS);

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.SingleUpdateMessage(DataTableNames.BlomShuttleDelegationTravelerDataTable,
                    DbAHD.PrimaryKeyControl(model),
                    DbAHD.RowVersionControls(Portal.SingleToList(model))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyBlomShuttleDelegationTraveler(model.BlomShuttleDelegationTravelerGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult BlomShuttleDelegationTravelerDelete(dataBlomShuttleDelegationTraveler model)
        {
            if (!CMS.HasAction(Permissions.StaffShuttleDelegation.Update, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            List<dataBlomShuttleDelegationTraveler> DeletedLanguages = DeleteBlomShuttleDelegationTraveler(new List<dataBlomShuttleDelegationTraveler> { model });

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.SingleDeleteMessage(DeletedLanguages, DataTableNames.BlomShuttleDelegationTravelerDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyBlomShuttleDelegationTraveler(model.BlomShuttleDelegationTravelerGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult BlomShuttleDelegationTravelerRestore(dataBlomShuttleDelegationTraveler model)
        {
            if (!CMS.HasAction(Permissions.StaffShuttleDelegation.Update, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            if (ActiveBlomShuttleDelegationTraveler(model))
            {
                return Json(DbAHD.RecordExists());
            }

            List<dataBlomShuttleDelegationTraveler> RestoredLanguages = RestoreBlomShuttleDelegationTraveler(Portal.SingleToList(model));

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.SingleRestoreMessage(RestoredLanguages, DataTableNames.BlomShuttleDelegationTravelerDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyBlomShuttleDelegationTraveler(model.BlomShuttleDelegationTravelerGUID);
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult BlomShuttleDelegationTravelerDataTableDelete(List<dataBlomShuttleDelegationTraveler> models)
        {
            if (!CMS.HasAction(Permissions.StaffShuttleDelegation.Update, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            List<dataBlomShuttleDelegationTraveler> DeletedLanguages = DeleteBlomShuttleDelegationTraveler(models);

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.PartialDeleteMessage(DeletedLanguages, models, DataTableNames.BlomShuttleDelegationTravelerDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult BlomShuttleDelegationTravelerDataTableModelRestore(List<dataBlomShuttleDelegationTraveler> models)
        {
            if (!CMS.HasAction(Permissions.StaffShuttleDelegation.Update, Apps.AHD))
            {
                return Json(DbAHD.PermissionError());
            }
            List<dataBlomShuttleDelegationTraveler> RestoredLanguages = RestoreBlomShuttleDelegationTraveler(models);

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbAHD.PartialRestoreMessage(RestoredLanguages, models, DataTableNames.BlomShuttleDelegationTravelerDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbAHD.ErrorMessage(ex.Message));
            }
        }

        private List<dataBlomShuttleDelegationTraveler> DeleteBlomShuttleDelegationTraveler(List<dataBlomShuttleDelegationTraveler> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<dataBlomShuttleDelegationTraveler> DeletedStaffBankAccount = new List<dataBlomShuttleDelegationTraveler>();

            string query = DbAHD.QueryBuilder(models, Permissions.StaffShuttleDelegation.UpdateGuid, SubmitTypes.Delete, "");

            var languages = DbAHD.Database.SqlQuery<dataBlomShuttleDelegationTraveler>(query).ToList();

            foreach (var language in languages)
            {
                DeletedStaffBankAccount.Add(DbAHD.Delete(language, ExecutionTime, Permissions.StaffShuttleDelegation.UpdateGuid, DbCMS));
            }

            return DeletedStaffBankAccount;
        }

        private List<dataBlomShuttleDelegationTraveler> RestoreBlomShuttleDelegationTraveler(List<dataBlomShuttleDelegationTraveler> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<dataBlomShuttleDelegationTraveler> RestoredLanguages = new List<dataBlomShuttleDelegationTraveler>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbAHD.QueryBuilder(models, Permissions.StaffShuttleDelegation.UpdateGuid, SubmitTypes.Restore, baseQuery);

            var Languages = DbAHD.Database.SqlQuery<dataBlomShuttleDelegationTraveler>(query).ToList();
            foreach (var language in Languages)
            {
                if (!ActiveBlomShuttleDelegationTraveler(language))
                {
                    RestoredLanguages.Add(DbAHD.Restore(language, Permissions.StaffShuttleDelegation.UpdateGuid, Permissions.StaffShuttleDelegation.UpdateGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredLanguages;
        }

        private JsonResult ConcrrencyBlomShuttleDelegationTraveler(Guid PK)
        {
            dataBlomShuttleDelegationTraveler dbModel = new dataBlomShuttleDelegationTraveler();

            var Language = DbAHD.dataBlomShuttleDelegationTraveler.Where(l => l.BlomShuttleDelegationTravelerGUID == PK).FirstOrDefault();
            var dbLanguage = DbAHD.Entry(Language).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbLanguage, dbModel);

            if (Language.dataBlomShuttleDelegationTravelerRowVersion.SequenceEqual(dbModel.dataBlomShuttleDelegationTravelerRowVersion))
            {
                return Json(DbAHD.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbAHD, dbModel, "LanguagesContainer"));
        }

        private bool ActiveBlomShuttleDelegationTraveler(dataBlomShuttleDelegationTraveler model)
        {
            int LanguageID = DbAHD.dataBlomShuttleDelegationTraveler
                                  .Where(x =>
                                              x.StaffGUID == model.StaffGUID &&
                                              x.BlomShuttleDelegationDateGUID == model.BlomShuttleDelegationDateGUID &&
                                              x.DelgationStaffTypeGUID == model.DelgationStaffTypeGUID &&

                                              x.Active).Count();
            if (LanguageID > 0)
            {
                //Fix
                ModelState.AddModelError("LanguageID", "Already Exist");
            }

            return (LanguageID > 0);
        }


        #endregion

        #region Staff Requst

        public ActionResult BlomShuttleDelegationStaffRequestDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null) return PartialView("~/Areas/AHD/Views/ShuttleDelegationManager/_StaffRequestDataTable.cshtml", new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<DelegationStaffRequestDataTable, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<DelegationStaffRequestDataTable>(DataTable.Filters);
            }

            var Result = (from a in DbAHD.dataBlomShuttleDelegationStaffRequest.AsExpandable().Where(x => x.BlomShuttleDelegationDateGUID == PK && x.Active == true)

                          select new DelegationStaffRequestDataTable
                          {
                              BlomShuttleDelegationStaffRequestGUID = a.BlomShuttleDelegationStaffRequestGUID.ToString(),
                              BlomShuttleDelegationDateGUID = a.BlomShuttleDelegationDateGUID.ToString(),

                              StaffGUID = a.StaffGUID.ToString(),
                              StaffName = a.StaffName,
                              AmountUSD = a.AmountUSD.ToString(),
                              AmountUSDWritten = a.AmountUSDWritten,
                              NationalIDNumber = a.NationalIDNumber,
                              SyrianIDNumber = a.SyrianIDNumber,
                              AccountNumber = a.AccountNumber,
                              BranchName = a.BranchName,
                              CreateDate = a.CreateDate,
                              Active = a.Active,
                              dataBlomShuttleDelegationStaffRequestRowVersion = a.dataBlomShuttleDelegationStaffRequestRowVersion
                          }).Where(Predicate);

            Result = SearchHelper.OrderByDynamic(Result, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            return Json(Portal.DataTable(Result.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Staff Request Wizard
        [Route("AHD/StaffDelegation/StaffCalendarIndex/")]
        public ActionResult StaffCalendarIndex()
        {

            return View("~/Areas/AHD/Views/ShuttleDelegationManager/StaffCalendar.cshtml");
        }
        public ActionResult BlomShuttleDelegationStaffRequestCreate(Guid FK)
        {

            DelegationStaffRequestUpdateModel model = new DelegationStaffRequestUpdateModel();
            DateTime ExecutionTime = DateTime.Now;

            var _staffRequest = DbAHD.dataBlomShuttleDelegationStaffRequest.Where(x => x.BlomShuttleDelegationDateGUID == FK && x.StaffGUID == UserGUID).FirstOrDefault();
            if (_staffRequest != null && _staffRequest.dataBlomShuttleDelegationDate.StartDate < ExecutionTime)
            {
                return Json(DbAHD.PermissionError());

            }
            if (_staffRequest != null)
            {

                return View("~/Areas/AHD/Views/ShuttleDelegationManager/EditStaffShuttleRequest.cshtml", _staffRequest);
            }
            model.BlomShuttleDelegationDateGUID = FK;
            model.StaffGUID = UserGUID;
            model.BlomShuttleDelegationStaffRequestGUID = Guid.NewGuid();
            var _staffcore = DbAHD.StaffCoreData.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
            model.NationalIDNumber = _staffcore.SyrianNationalIDNumber;

            return View("~/Areas/AHD/Views/ShuttleDelegationManager/StaffRequestForm.cshtml", model);

            //return PartialView("~/Areas/AHD/Views/AppointmentTypeCalendars/_AppointmentTypeCalendarModal.cshtml",
            //    new AppointmentTypeCalenderUpdateModel { OrganizationInstanceGUID = userProfiles.OrganizationInstanceGUID, DutyStationGUID = userProfiles.DutyStationGUID, EventEachDay = true });
        }

        [HttpPost]
        public ActionResult BlomShuttleDelegationStaffRequestCreate(Guid? blomShuttleDelegationStaffRequestGUID, Guid? blomShuttleDelegationDateGUID, Guid? _traveler,
                                                             Guid? staffGUID, int _AmountUSD,
                                                             //string _amountUSDWritten, 
                                                             string _NationalIDNumber,
                                                             //string _SyrianIDNumber,
                                                             string _AccountNumber,
                                                             Guid? _BankBranchGUID,
                                                             string _Comments)
        {

            var _check = DbAHD.dataBlomShuttleDelegationStaffRequest.Where(x => x.BlomShuttleDelegationDateGUID == blomShuttleDelegationDateGUID && x.StaffGUID == staffGUID).FirstOrDefault();
            if (_check != null) return Json(new { success = -33 }, JsonRequestBehavior.AllowGet);




            //if (model.WarehouseItemDescription == null || ActiveItem(model)) return PartialView("~/Areas/AHD/Views/Items/_ItemForm.cshtml", model);

            DateTime ExecutionTime = DateTime.Now;

            Guid EntityPK = Guid.NewGuid();
            var staffCore = DbAHD.StaffCoreData.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
            string ar = "AR";
            var staffPers = DbAHD.userPersonalDetailsLanguage.Where(x => x.UserGUID == UserGUID && x.LanguageID == ar).FirstOrDefault();
            var codeTablesValue = DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN).AsQueryable();



            dataBlomShuttleDelegationStaffRequest myModel = new dataBlomShuttleDelegationStaffRequest
            {
                BlomShuttleDelegationStaffRequestGUID = (Guid)blomShuttleDelegationStaffRequestGUID,
                BlomShuttleDelegationDateGUID = blomShuttleDelegationDateGUID,
                BlomShuttleDelegationTravelerGUID = _traveler,
                StaffGUID = staffGUID,
                StaffName = staffPers.FirstName + " " + staffPers.Surname,
                AmountUSD = _AmountUSD,
                //AmountUSDWritten=_amountUSDWritten,
                CreateDate = ExecutionTime,
                NationalIDNumber = _NationalIDNumber,
                //SyrianIDNumber = _SyrianIDNumber,
                AccountNumber = _AccountNumber,
                BranchName = codeTablesValue.Where(x => x.ValueGUID == _BankBranchGUID).FirstOrDefault().ValueDescription,
                BankBranchGUID = _BankBranchGUID,
                Comments = _Comments

            };




            DbAHD.CreateNoAudit(myModel);


            //DbAHD.Create(Language, Permissions.InternationalStaffRestAndRecuperationLeave.CreateGuid, ExecutionTime, DbCMS);

            List<PartialViewModel> Partials = new List<PartialViewModel>();
            Partials.Add(Portal.PartialView(EntityPK, DataTableNames.InternationalTempStaffRAndRDatesDataTable, ControllerContext, "InternationalTempStaffRAndRDatesDataTable"));


            List<UIButtons> UIButtons = new List<UIButtons>();
            UIButtons.Add(new UIButtons { Button = HtmlExtension.CreateNewButton(Permissions.InternationalStaffRestAndRecuperationLeave.Create, Apps.AHD, new UrlHelper(Request.RequestContext).Action("Create", "StaffRAndRLeave", new { Area = "AHD" })), Container = "ItemModelDetailFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.UpdateButton(Permissions.InternationalStaffRestAndRecuperationLeave.Update, Apps.AHD), Container = "ItemModelDetailFormControls" });
            UIButtons.Add(new UIButtons { Button = HtmlExtension.DeleteButton(Permissions.InternationalStaffRestAndRecuperationLeave.Delete, Apps.AHD), Container = "ItemModelDetailFormControls" });

            try
            {
                DbAHD.SaveChanges();
                DbCMS.SaveChanges();
                // SendHRApprovalReviewMail((Guid)RestAndRecuperationLeaveGUID);

                return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);
                //return Json(DbAHD.SingleCreateMessage(DbAHD.PrimaryKeyControl(myModel), DbAHD.RowVersionControls(myModel, myModel), Partials, "", UIButtons));
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
        }


        #endregion
        #region Upload Delegation Report

        //[Route("AHD/ModelMovements/UploadDamgedReport/{PK}")]
        public ActionResult GetUploadDelegationdReport(Guid PK)
        {

            UploadDelegationRequestVM model = new UploadDelegationRequestVM();

            model.BlomShuttleDelegationStaffRequestGUID = PK;
            model.UserGUID = UserGUID;

            return PartialView("~/Areas/AHD/Views/ShuttleDelegationManager/_UploadDelegationReport.cshtml", model);
        }
        [HttpPost]
        public FineUploaderResult UploadDelegationdReport(FineUpload upload, UploadDelegationRequestVM model)
        {

            return new FineUploaderResult(true, new { path = UploadStaffDelegationReport(upload, model), success = true });
        }

        public string UploadStaffDelegationReport(FineUpload upload, UploadDelegationRequestVM model)
        {

            if (upload != null)
            {
                var detail = DbAHD.dataBlomShuttleDelegationStaffRequest.Where(x => x.BlomShuttleDelegationStaffRequestGUID == model.BlomShuttleDelegationStaffRequestGUID).FirstOrDefault();


                var filePath = Server.MapPath("~\\Areas\\AHD\\Templates\\BLOMDelegation\\StaffDelegations\\");

                string extension = Path.GetExtension(upload.FileName);
                string fullFileName = filePath + "\\" + detail.BlomShuttleDelegationDateGUID + "\\" + model.BlomShuttleDelegationStaffRequestGUID + extension;
                upload.SaveAs(fullFileName);
                try
                {
                    DbAHD.SaveChanges();
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

        #region Number convert 
        public string wordify(decimal v)
        {
            if (v == 0) return "zero";
            var units = " one two three four five six seven eight nine".Split();
            var teens = " eleven twelve thir# four# fif# six# seven# eigh# nine#".Replace("#", "teen").Split();
            var tens = " ten twenty thirty forty fifty sixty seventy eighty ninety".Split();
            var thou = " thousand m# b# tr# quadr# quint# sext# sept# oct#".Replace("#", "illion").Split();
            var g = (v < 0) ? "minus " : "";
            var w = "";
            var p = 0;
            v = Math.Abs(v);
            while (v > 0)
            {
                int b = (int)(v % 1000);
                if (b > 0)
                {
                    var h = (b / 100);
                    var t = (b - h * 100) / 10;
                    var u = (b - h * 100 - t * 10);
                    var s = ((h > 0) ? units[h] + " hundred" + ((t > 0 | u > 0) ? " and " : "") : "")
                          + ((t > 0) ? (t == 1 && u > 0) ? teens[u] : tens[t] + ((u > 0) ? "-" : "") : "")
                          + ((t != 1) ? units[u] : "");
                    s = (((v > 1000) && (h == 0) && (p == 0)) ? " and " : (v > 1000) ? ", " : "") + s;
                    w = s + " " + thou[p] + w;
                }
                v = v / 1000;
                p++;
            }
            return g + w;
        }
        #endregion

        #region Print 




        private static string[] GetFileNames(string path, string filter)
        {
            string[] files = Directory.GetFiles(path, filter);
            for (int i = 0; i < files.Length; i++)
                files[i] = Path.GetFileName(files[i]);
            return files;
        }


        public void Print(Guid id)
        {
            //string[] pdfFiles = GetFileNames("~\\Areas\\AHD\\Templates\\BLOMDelegation\\StaffDelegations\\"+id.ToString(), "*.pdf");
            var filePath = Server.MapPath("~\\Areas\\AHD\\Templates\\BLOMDelegation\\StaffDelegations\\");


            string pathname = filePath + "\\" + id.ToString();

            List<string> sourceDir = Directory.GetFiles(pathname).Select(d => Path.GetFileName(d)).ToList();

            foreach (var item in sourceDir)
            {
                string myPath = pathname + item;
                Console.WriteLine(myPath); // full path
                Console.WriteLine(System.IO.Path.GetFileName(myPath)); // file name
            }
            //foreach (var path in pdfFiles)
            //{
            //    Console.WriteLine(path); // full path
            //    Console.WriteLine(System.IO.Path.GetFileName(path)); // file name
            //}


        }


        #endregion

        #region Download Multi Files

        public List<FileInfoVM> GetFile(Guid id)
        {
            var filePath = Server.MapPath("~\\Areas\\AHD\\Templates\\BLOMDelegation\\StaffDelegations\\");
            string pathname = filePath + "\\" + id.ToString();

            List<FileInfoVM> listFiles = new List<FileInfoVM>();
            string fileSavePath = System.Web.Hosting.HostingEnvironment.MapPath("~\\Areas\\AHD\\Templates\\BLOMDelegation\\StaffDelegations\\" + "\\" + id.ToString());
            DirectoryInfo dirInfo = new DirectoryInfo(fileSavePath);
            int i = 0;
            foreach (var item in dirInfo.GetFiles())
            {
                listFiles.Add(new FileInfoVM()
                {
                    FileId = i + 1,
                    FileName = item.Name,
                    FilePath = dirInfo.FullName + @"\\" + item.Name
                });
                i = i + 1;
            }
            return listFiles;
        }
        public ActionResult Download(Guid id)
        {
            //FileDownloads obj = new FileDownloads();
            //////int CurrentFileID = Convert.ToInt32(FileID);  
            var filesCol = GetFile(id).ToList();
            using (var memoryStream = new MemoryStream())
            {
                using (var ziparchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    for (int i = 0; i < filesCol.Count; i++)
                    {
                        ziparchive.CreateEntryFromFile(filesCol[i].FilePath, filesCol[i].FileName);
                    }
                }
                return File(memoryStream.ToArray(), "application/zip", "Attachments.zip");
            }
        }

        #endregion
    }
}