using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.ViewModels;
using AutoMapper;
using LinqKit;
using OVS_DAL.Model;
using RES_Repo.Globalization;
using AppsPortal.OVS.ViewModels;
using System.Threading.Tasks;
using iTextSharp.text.pdf.qrcode;

namespace AppsPortal.Areas.OVS.Controllers
{
    public class StaffController : OVSBaseController
    {

        #region Election Staff
        public ActionResult ElectionStaffsDataTable(DataTableRecievedOptions options, Guid PK)
        {
            if (options.columns == null)
                return PartialView("~/Areas/OVS/Views/Staff/_StaffsDataTable.cshtml",
                    new MasterRecordStatus { ParentGUID = PK, IsParentActive = true });

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<dataElectionStaff, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<dataElectionStaff>(DataTable.Filters);
            }

            var All = DbOVS.dataElectionStaff.AsNoTracking().AsExpandable().Where(x => x.ElectionGUID == PK)
                .Where(Predicate)
                .Select(a => new ElectionStaffModel
                {
                    ElectionStaffGUID = a.ElectionStaffGUID,
                    FullName = a.dataStaff.FullName,
                    EmailAddress = a.dataStaff.EmailAddress,
                    dataElectionStaffRowVersion = a.dataElectionStaffRowVersion
                });
            if (!String.IsNullOrEmpty(DataTable.Order.OrderBy))
                All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<ElectionStaffModel> Result = Mapper.Map<List<ElectionStaffModel>>(All).ToList();

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ElectionStaffCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Create, Apps.OVS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            return PartialView("~/Areas/OVS/Views/Staff/_StaffUpdateModal.cshtml",
                new dataStaffModel() { ElectionGUID=FK} );
        }

        public ActionResult ElectionStaffUpdate(Guid PK)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Access, Apps.OVS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var staffElec = DbOVS.dataElectionStaff.Where(x => x.ElectionStaffGUID == PK).FirstOrDefault();
            return PartialView("~/Areas/OVS/Views/Staff/_StaffUpdateModal.cshtml",
                DbOVS.dataStaff.Where(x=>x.StaffGUID== staffElec.StaffGUID).Select(x=>new dataStaffModel
                {
                    StaffGUID = x.StaffGUID,
                    ElectionGUID = staffElec.ElectionGUID,
                    ElectionStaffGUID = staffElec.ElectionStaffGUID,
                    EmailAddress = x.EmailAddress,
                    FullName = x.FullName,
                    dataStaffRowVersion = x.dataStaffRowVersion,
                    Active = x.Active,
                }).FirstOrDefault());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ElectionStaffCreate(dataStaffModel model)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Create, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            if (!ModelState.IsValid || ActiveElectionStaff(model))
                return PartialView("~/Areas/OVS/Views/Staff/_StaffUpdateModal.cshtml", model);

            Guid ActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            DateTime ExecutionTime = DateTime.Now;

            dataStaff currStaff =
                DbOVS.dataStaff.FirstOrDefault(x => x.EmailAddress == model.EmailAddress);
            if (currStaff != null)
            {
                model.StaffGUID = currStaff.StaffGUID;
            }
            if (model.StaffGUID == null || model.StaffGUID == Guid.Empty)
            {
                currStaff = new dataStaff();
                Mapper.Map(model, currStaff);
                DbOVS.Create(model, Permissions.ElectionsManagement.CreateGuid, ExecutionTime, DbCMS);
                DbOVS.SaveChanges();
                //dataStaff newstaff = DbOVS.dataStaff.FirstOrDefault(x => x.EmailAddress == model.EmailAddress);
                //if (newstaff != null) model.StaffGUID = newstaff.StaffGUID;
            }
            dataElectionStaff electionStaff = new dataElectionStaff();
            electionStaff.AccessKey = Guid.NewGuid().ToString();
            electionStaff.StaffGUID = model.StaffGUID;
            electionStaff.Active = true;
            electionStaff.ElectionGUID = model.ElectionGUID.Value;
            electionStaff.ElectionStaffGUID = Guid.NewGuid();
            DbOVS.Create(electionStaff, Permissions.ElectionsManagement.CreateGuid, ExecutionTime, DbCMS);
            try
            {
                DbOVS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbOVS.SingleUpdateMessage(DataTableNames.ElectionStaffsDataTable,
                    DbOVS.PrimaryKeyControl(electionStaff), DbOVS.RowVersionControls(Portal.SingleToList(electionStaff))));
            }
            catch (Exception ex)
            {
                return Json(DbOVS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ElectionStaffUpdate(dataStaffModel model)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Update, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            if (!ModelState.IsValid)
                return PartialView("~/Areas/OVS/Views/Staff/_StaffUpdateModal.cshtml", model);

            Guid ActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            DateTime ExecutionTime = DateTime.Now;
            dataStaff staff =  new dataStaff();
            Mapper.Map(model, staff);
            DbOVS.Update(staff, ActionGUID, ExecutionTime,DbCMS);

            try
            {
                DbOVS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbOVS.SingleUpdateMessage(DataTableNames.ElectionStaffsDataTable,
                    DbOVS.PrimaryKeyControl(staff),
                    DbOVS.RowVersionControls(Portal.SingleToList(staff))));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyElectionStaff(model.ElectionStaffGUID.Value);
            }
            catch (Exception ex)
            {
                return Json(DbOVS.ErrorMessage(ex.Message));
            }
        }

        public ActionResult ElectionStaffDelete(dataElectionStaff model)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Delete, Apps.OVS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            List<dataElectionStaff> DeletedStaffs =
                DeleteElectionStaffs(new List<dataElectionStaff> { model });

            try
            {
                DbOVS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbOVS.SingleDeleteMessage(DeletedStaffs, DataTableNames.ElectionStaffsDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyElectionStaff(model.ElectionStaffGUID);
            }
            catch (Exception ex)
            {
                return Json(DbOVS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ElectionStaffRestore(dataElectionStaff model)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Restore, Apps.OVS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            if (ActiveElectionStaff(new dataStaffModel() { ElectionGUID=model.ElectionGUID,ElectionStaffGUID=model.ElectionStaffGUID}))
            {
                return Json(DbOVS.RecordExists());
            }

            List<dataElectionStaff> RestoredStaffs = RestoreElectionStaff(Portal.SingleToList(model));

            try
            {
                DbOVS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbOVS.SingleRestoreMessage(RestoredStaffs, DataTableNames.ElectionStaffsDataTable));
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcrrencyElectionStaff(model.ElectionStaffGUID);
            }
            catch (Exception ex)
            {
                return Json(DbOVS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ElectionStaffsDataTableDelete(List<dataElectionStaff> models)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Delete, Apps.OVS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            List<dataElectionStaff> DeletedStaffs = DeleteElectionStaffs(models);

            try
            {
                DbOVS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbOVS.PartialDeleteMessage(DeletedStaffs, models,
                    DataTableNames.ElectionStaffsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbOVS.ErrorMessage(ex.Message));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ElectionStaffsDataTableRestore(List<dataElectionStaff> models)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Restore, Apps.OVS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            List<dataElectionStaff> RestoredStaffs = RestoreElectionStaff(models);

            try
            {
                DbOVS.SaveChanges();
                DbCMS.SaveChanges();
                return Json(DbOVS.PartialRestoreMessage(RestoredStaffs, models,
                    DataTableNames.ElectionStaffsDataTable));
            }
            catch (Exception ex)
            {
                return Json(DbOVS.ErrorMessage(ex.Message));
            }
        }

        private List<dataElectionStaff> DeleteElectionStaffs(List<dataElectionStaff> models)
        {
            DateTime ExecutionTime = DateTime.Now;

            List<dataElectionStaff> DeletedElectionStaffs = new List<dataElectionStaff>();

            string query = DbOVS.QueryBuilder(models, Permissions.ElectionsManagement.DeleteGuid, SubmitTypes.Delete, "");

            var staffs = DbOVS.Database.SqlQuery<dataElectionStaff>(query).ToList();

            foreach (var staff in staffs)
            {
                DeletedElectionStaffs.Add(DbOVS.Delete(staff, ExecutionTime,
                    Permissions.ElectionsManagement.DeleteGuid, DbCMS));
            }

            return DeletedElectionStaffs;
        }

        private List<dataElectionStaff> RestoreElectionStaff(List<dataElectionStaff> models)
        {
            DateTime RestoringTime = DateTime.Now;

            List<dataElectionStaff> RestoredStaffs = new List<dataElectionStaff>();

            //Select the table and all the factors from other tables into one query.
            string baseQuery = "";

            string query = DbOVS.QueryBuilder(models, Permissions.ElectionsManagement.DeleteGuid, SubmitTypes.Restore,
                baseQuery);

            var staffs = DbOVS.Database.SqlQuery<dataElectionStaff>(query).ToList();
            foreach (var staff in staffs)
            {
                if (!ActiveElectionStaff(new dataStaffModel() { ElectionGUID = staff.ElectionGUID, ElectionStaffGUID = staff.ElectionStaffGUID }))
                {
                    RestoredStaffs.Add(DbOVS.Restore(staff, Permissions.ElectionsManagement.DeleteGuid,
                        Permissions.ElectionsManagement.RestoreGuid, RestoringTime, DbCMS));
                }
            }

            return RestoredStaffs;
        }

        private JsonResult ConcrrencyElectionStaff(Guid PK)
        {
            dataElectionStaff dbModel = new dataElectionStaff();

            var staff = DbOVS.dataElectionCondition.Where(l => l.ElectionConditionGUID == PK).FirstOrDefault();
            var dbStaff = DbOVS.Entry(staff).GetDatabaseValues().ToObject();
            dbModel = Mapper.Map(dbStaff, dbModel);

            if (staff.dataElectionConditionRowVersion.SequenceEqual(dbModel.dataElectionStaffRowVersion))
            {
                return Json(DbOVS.PermissionError());
            }

            return Json(JsonMessages.ConcurrencyError(DbOVS, dbModel, "StaffsContainer"));
        }

        private bool ActiveElectionStaff(dataStaffModel model)
        {
            int electionStaff = DbOVS.dataElectionStaff
                .Where(x => x.dataStaff.EmailAddress == model.EmailAddress &&
                            x.ElectionGUID == model.ElectionGUID &&

                            x.Active).Count();
            if (electionStaff > 0)
            {
                ModelState.AddModelError("Staff", "this staff is already exists");
            }

            return (electionStaff > 0);
        }



        public ActionResult ElectionStaffImport(Guid FK)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Create, Apps.OVS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            return PartialView("~/Areas/OVS/Views/Staff/_StaffImportModal.cshtml",
                new ElectionStaffModel { ElectionGUID = FK, FileImportStaffWarningMessage = resxMessages.FileImportStaffWarning });
        }
        [HttpPost]
        public ActionResult ElectionStaffImportCheck(ElectionStaffModel model)
        {
            if (HttpContext != null && HttpContext.Request.Files["file"].ContentLength > 0)
            {
                DataTable ds = importFile.ImportDataSet();
                List<ElectionStaffImportModel> staffs = new List<ElectionStaffImportModel>();
                for (int i = 0; i < ds.Rows.Count; i++)
                {
                    ElectionStaffImportModel staff = new ElectionStaffImportModel();
                    staff.FullName = ds.Rows[i][0].ToString();
                    staff.EmailAddress = ds.Rows[i][1].ToString();
                    if (string.IsNullOrEmpty(staff.FullName))
                        break;
                    staff.Status =
                    (DbOVS.dataStaff.Count(a => a.EmailAddress.ToLower().Equals(staff.EmailAddress.ToLower())) >
                     0);
                    staffs.Add(staff);
                }

                return Json(new { data = staffs }, JsonRequestBehavior.AllowGet);
            }
            return Json(DbOVS.ErrorMessage("Please Chose file"));

        }

        private static IDictionary<Guid, int> processCalculationTask = new Dictionary<Guid, int>();
        [HttpPost]
        public ActionResult ElectionStaffImport(List<ElectionStaffImportModel> model, Guid ElectionGUID)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Create, Apps.OVS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            var processCalculationTaskId = Guid.NewGuid();
            processCalculationTask.Add(processCalculationTaskId, 0);
            if (ElectionGUID == null || ElectionGUID == Guid.Empty)
            {
                throw new HttpException(401, "No Election");
            }
            else
            {
                var elec = DbOVS.dataElection.Where(x => x.ElectionGUID == ElectionGUID).FirstOrDefault();
                //there is nothing new to process
                if (model.Count == 0)
                {
                    return Json(new { NothingToProcess = true }, JsonRequestBehavior.AllowGet);
                }
                Task.Factory.StartNew(() =>
                {
                    DbOVS = new OVSEntities();
                    DbCMS = new Models.CMSEntities();

                    List<dataStaff> nodAddedStaffs = new List<dataStaff>();
                    List<dataStaff> staffsToBeAdded = new List<dataStaff>();
                    List<dataStaff> staffsToBeUpdated = new List<dataStaff>();
                    List<dataElectionStaff> electionStaffsToBeAdded = new List<dataElectionStaff>();
                    DateTime executionTime = DateTime.Now;
                    int index = 0;
                    foreach (var item in model)
                    {
                        processCalculationTask[processCalculationTaskId] = index++;
                        dataStaff staff =
                                DbOVS.dataStaff.FirstOrDefault(a => a.EmailAddress.ToLower().Equals(item.EmailAddress.ToLower()));
                        if (staff == null)
                        {
                            staff = new dataStaff();
                            staff.StaffGUID = Guid.NewGuid();
                            staff.EmailAddress = item.EmailAddress;
                            staff.FullName = item.FullName;
                            //staff.DutyStationGUID = elec.DutyStationGUID;
                            //staff.OrganizationInstanceGUID = elec.OrganizationInstanceGUID;
                            staff.Active = true;
                            staffsToBeAdded.Add(staff);
                        }
                        else
                        {
                            staff.Active = true;
                            staffsToBeUpdated.Add(staff);

                        }
                        var electionStaff = DbOVS.dataElectionStaff.Where(x => x.StaffGUID == staff.StaffGUID && x.ElectionGUID == ElectionGUID).FirstOrDefault();
                        if (electionStaff == null)
                        {
                            electionStaff = new dataElectionStaff();
                            electionStaff.ElectionStaffGUID = Guid.NewGuid();
                            electionStaff.ElectionGUID = ElectionGUID;
                            electionStaff.StaffGUID = staff.StaffGUID;
                            electionStaff.AccessKey = Guid.NewGuid().ToString();
                            electionStaff.Active = true;
                            electionStaffsToBeAdded.Add(electionStaff);
                        }
                        else
                        {
                            electionStaff.Active = true;
                        }

                    }


                    try
                    {

                        DbOVS.dataStaff.AddRange(staffsToBeAdded);
                        DbOVS.dataElectionStaff.AddRange(electionStaffsToBeAdded);
                        //DbOVS.UpdateBulk(staffsToBeUpdated, Permissions.ElectionsManagement.CreateGuid, executionTime,DbCMS);
                        //DbOVS.CreateBulk(staffsToBeAdded, Permissions.ElectionsManagement.CreateGuid, executionTime, DbCMS);
                        //DbOVS.CreateBulk(electionStaffsToBeAdded, Permissions.ElectionsManagement.CreateGuid,executionTime, DbCMS);
                        DbOVS.SaveChanges();
                        DbCMS.SaveChanges();
                        processCalculationTask[processCalculationTaskId] = index++;
                    }
                    catch (Exception ex)
                    {
                    }
                });

                return Json(new { processCalculationTaskId = processCalculationTaskId, TotalRecords = model.Count });
            }


        }

        public ActionResult ProcessCalculationProgres(Guid id)
        {
            return Json(processCalculationTask.Keys.Where(x => x.ToString() == id.ToString()).Contains(id) ? processCalculationTask[id] : -100);
        }

        #region Mail
        public ActionResult ElectionStaffMailBrodcastCreate(Guid FK)
        {
            if (!CMS.HasAction(Permissions.ElectionsManagement.Create, Apps.OVS))
            {
                throw new HttpException(401, "Unauthorized access");
            }

            dataElection model = DbOVS.dataElection.WherePK(FK).FirstOrDefault();
            string URL = AppSettingsKeys.Domain + "OVS/Vote/VoteChecker/?accessKey=";
            string Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.VoteInvitaion + "</a>";
            string Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";
            ViewBag.MailMessage = resxEmails.VoteInvitationFormat.Replace("$InvitationDate", model.StartDate.ToString())
                 .Replace("$VoteName", model.dataElectionLanguage.FirstOrDefault(x => x.Active && x.LanguageID == LAN).Title)
                 .Replace("$CloseDate", model.CloseDate.ToString())
                 .Replace("$VoteLink", Anchor)
                 .Replace("$Link", Link);
            return PartialView("~/Areas/OVS/Views/Staff/_StaffMailModal.cshtml",
                model);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ElectionStaffMailBrodcastCreate(dataElection model)
        {
            string _message = "";
            var election = (from a in DbOVS.dataElection.WherePK(model.ElectionGUID)
                            join b in DbOVS.dataElectionLanguage.Where(x =>
                                    (x.Active == true ? x.Active : x.DeletedOn == x.dataElection.DeletedOn) &&
                                    x.LanguageID == LAN) on
                                a.ElectionGUID equals b.ElectionGUID into LJ1
                            from R1 in LJ1.DefaultIfEmpty()
                            select new ElectionUpdateModel
                            {
                                ElectionGUID = a.ElectionGUID,
                                Active = a.Active,
                                Details = R1.Details,
                                Title = R1.Title,
                                StartDate = a.StartDate,
                                CloseDate = a.CloseDate,
                            }).FirstOrDefault();
            List<dataElectionStaff> staffs =
                DbOVS.dataElectionStaff.Where(x => x.ElectionGUID == election.ElectionGUID && x.Active).ToList();
            if (staffs != null && staffs.Count > 0)
            {
                if (!string.IsNullOrEmpty(model.CustomMessage)) _message = model.CustomMessage;
                foreach (var item in staffs)
                {
                    string URL = AppSettingsKeys.Domain + "/OVS/Vote/VoteChecker/?accessKey=" + item.AccessKey;
                    string Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.VoteInvitaion + "</a>";
                    string Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";
                    if (string.IsNullOrEmpty(model.CustomMessage))
                        _message = resxEmails.VoteInvitationMessage.Replace("$FullName", item.dataStaff.FullName)
                            .Replace("$InvitationDate", election.StartDate.ToString())
                            .Replace("$CloseDate", election.CloseDate.ToString())
                            .Replace("$VoteName", election.Title)
                            .Replace("$VoteLink", Anchor)
                            .Replace("$Link", Link);
                    mail.Send(item.dataStaff.EmailAddress, resxEmails.VoteInvitationSubject, _message,"Online Voting");
                }

                return Json(DbOVS.SuccessMessage("Vote invitation are sent successfully"));
            }
            else
            {
                return Json(DbOVS.ErrorMessage("No staff in the list "));
            }

        }

        public ActionResult ElectionStaffSingleMessgeCreate(dataElectionStaff model)
        {
            dataElectionStaff staff =
                 DbOVS.dataElectionStaff.Where(x => x.ElectionStaffGUID == model.ElectionStaffGUID && x.Active).FirstOrDefault();
            if (staff != null)
            {
                string URL = AppSettingsKeys.Domain + "/OVS/Vote/VoteChecker/?accessKey=" + staff.AccessKey;
                string Anchor = "<a href='" + URL + "' target='_blank'>" + resxUIControls.VoteInvitaion + "</a>";
                string Link = "<a href='" + URL + "' target='_blank'>" + URL + "</a>";

                string _message = resxEmails.VoteInvitationMessage.Replace("$FullName", staff.dataStaff.FullName)
                      .Replace("$InvitationDate", staff.dataElection.StartDate.ToString())
                      .Replace("$CloseDate", staff.dataElection.CloseDate.ToString())
                      .Replace("$VoteName", staff.dataElection.dataElectionLanguage.FirstOrDefault(x => x.LanguageID == LAN).Title)
                      .Replace("$VoteLink", Anchor)
                      .Replace("$Link", Link);
                mail.Send(staff.dataStaff.EmailAddress, resxEmails.VoteInvitationSubject, _message);


                return Json(DbOVS.SuccessMessage("Vote invitation are sent successfully"));
            }
            else
            {
                return Json(DbOVS.ErrorMessage("No staff in the list "));
            }

        }
        #endregion
        #endregion
    }
}