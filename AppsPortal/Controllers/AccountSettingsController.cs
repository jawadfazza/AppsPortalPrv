using AppsPortal.BaseControllers;
using AppsPortal.Data;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.Models;
using AppsPortal.ViewModels;
using AutoMapper;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Controllers
{
    public class AccountSettingsController : PortalBaseController
    {
        // GET: AccountSettings
        public ActionResult Index()
        {
            return View();
        }

        #region Change Password
        public ActionResult ChangePasswordView()
        {
            DateTime dt = DbCMS.userPasswords.Where(x => x.Active && x.UserGUID == UserGUID)
                .OrderByDescending(up => up.ActivationDate)
                .FirstOrDefault().ActivationDate;

            ViewBag.PasswordLastModifiedOn = Portal.formatDate(dt) + ", Your password will be expired in " + (dt.AddMonths(4) - dt).TotalDays.ToString() + " day(s)";

            return PartialView("~/Views/AccountSettings/Password/_PasswordView.cshtml");
        }

        public ActionResult ChangePasswordUpdate()
        {
            return PartialView("~/Views/AccountSettings/Password/_PasswordUpdate.cshtml", new ChangePasswordModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ChangePasswordUpdate(ChangePasswordModel model)
        {
            if (!ModelState.IsValid) return PartialView("~/Views/Profile/HomeAddress/_PasswordUpdate", model);

            var dbPassword = DbCMS.userPasswords.Where(up => up.UserGUID == UserGUID && up.Active).OrderByDescending(up => up.ActivationDate).FirstOrDefault();

            string EnteredPassword = HashingHelper.EncryptPassword(model.CurrentPassword, Portal.GUIDToString(UserGUID));
            
            //This should be action less. each user change his password and it is already tracked (logged) on the userpasswords table. Ayas
            
            if (dbPassword.Password != EnteredPassword)
            {
                ModelState.AddModelError("Password", "Password is wrong <a href'#'>Forget?</a>");
                return View("Index");
            }
            DateTime ExecutionTime = DateTime.Now;

            model.Password = HashingHelper.EncryptPassword(model.Password, Portal.GUIDToString(UserGUID));
            userPasswords Password = Mapper.Map<ChangePasswordModel, userPasswords>(model);

            Password.UserGUID = UserGUID;
            Password.Active = true;
            DbCMS.Create(Password, Guid.Parse("00000000-0000-0000-0000-000000000001"), ExecutionTime);// userPasswords.Add(Password);
            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage("Password Changed Successfully"));
            }
            catch (Exception ex)
            {
                string s = ex.Message;
                return Json(JsonMessages.ErrorMessage(DbCMS, s));
            }
        }

        private bool LastFivePasswordList(string Password)
        {
            var Passwords = DbCMS.userPasswords.Where(x => x.UserGUID == UserGUID)
                .OrderByDescending(x => x.ActivationDate)
                .Select(x => x.Password).ToList().Take(5);

            return Passwords.Contains(Password);
        }

        #endregion

        #region Permisions
        public ActionResult PermissionsView()
        {
            return PartialView("~/Views/AccountSettings/Permissions/_PermissionsView.cshtml");
        }

        public ActionResult PermissionsUpdate()
        {
            return PartialView("~/Views/AccountSettings/Permissions/_PermissionsUpdate.cshtml");
        }

        public ActionResult GetMyPermissionsTree(Guid ApplicationGUID)
        {
            PermissionsTreeFilter Filter = new PermissionsTreeFilter
            {
                UserProfileGUID = UserProfileGUID,
                ApplicationGUID = ApplicationGUID
            };

            return new JsonResult { Data = CMS.PermissionsTreeRowMaterials(Filter), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        #endregion

        #region Application
        public ActionResult ApplicationsView()
        {
            return PartialView("~/Views/AccountSettings/Applications/_ApplicationsView.cshtml");
        }
        public ActionResult ApplicationsUpdate()
        {
            return PartialView("~/Views/AccountSettings/Applications/_ApplicationsUpdate.cshtml");
        }
        #endregion

        #region Notifications
        public ActionResult NotificationsView()
        {
            return PartialView("~/Views/AccountSettings/Notifications/_NotificationsView.cshtml");
        }
        public ActionResult NotificationsUpdate()
        {
            return PartialView("~/Views/AccountSettings/Notifications/_NotificationsUpdate.cshtml");
        }

        [Route("AccountSettings/NotificationsAccountSettingDataTable/")]
        public JsonResult NotificationsDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<ApplicationDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<ApplicationDataTableModel>(DataTable.Filters);
            }
            var Apps = new CMS().GetHomePageApplications().Select(x=>x.ApplicationGUID).ToList();

            var All = (from a in DbCMS.codeApplicationsLanguages.AsExpandable().Where(x => x.LanguageID == LAN && Apps.Contains(x.ApplicationGUID))

                       select new ApplicationDataTableModel
                       {
                           ApplicationGUID = a.ApplicationGUID,
                           ApplicationDescription = a.ApplicationDescription,
                           ApplicationAcrynom= a.codeApplications.ApplicationAcrynom,
                           Active=a.Active
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<ApplicationDataTableModel> Result = Mapper.Map<List<ApplicationDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult NotificationBlock(Guid PK)
        {
            var Apps = CMS.GetHomePageApplications().Select(x => x.ApplicationGUID).ToList();

            var result = (from a in DbCMS.codeNotifications.Where(x => x.ApplicationGUID==PK)
                          join b in DbCMS.codeNotificationLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.NotificationGUID equals b.NotificationGUID into LJ1
                          from R1 in LJ1.DefaultIfEmpty()
                          join c in DbCMS.userNotificationControls.Where(x => x.UserGUID == UserGUID) on a.NotificationGUID equals c.NotificationGUID into LJ2
                          from R2 in LJ2.DefaultIfEmpty()
                          select new UserNotificationViewModel
                          {
                              NotificationGUID = a.NotificationGUID,
                              ApplicationGUID=a.ApplicationGUID,
                              TitleTemplate = R1.TitleTemplete,
                              Block = R2.Block == null ? false : R2.Block.Value
                          }).ToList();
            return PartialView("~/Views/AccountSettings/Notifications/_NotificationUpdateModal.cshtml",
                result);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult NotificationBlock(List<Guid> FK,Guid ApplicationGUID)
        {
            ////change all default connection to specific application to false value 
            var BlockList =DbCMS.userNotificationControls.Where(x =>
            DbCMS.codeNotifications.Where(y => y.ApplicationGUID== ApplicationGUID). Select(z => z.NotificationGUID).ToList().Contains(x.NotificationGUID)
            && x.UserGUID == UserGUID).ToList();
            BlockList.ForEach(x => x.Block = false);

            if (FK != null)
            {
                //create or update User Notification controls
                foreach (var NotificationGUID in FK)
                {
                    var userNotification = DbCMS.userNotificationControls.Where(x => x.NotificationGUID == NotificationGUID && x.UserGUID == UserGUID).FirstOrDefault();
                    if (userNotification != null)
                    {
                        userNotification.Block = true;
                    }
                    else
                    {
                        userNotification = new userNotificationControls();
                        userNotification.UserNotificationControlGUID = Guid.NewGuid();
                        userNotification.NotificationGUID = NotificationGUID;
                        userNotification.UserGUID = UserGUID;
                        userNotification.Block = true;
                        DbCMS.userNotificationControls.Add(userNotification);
                    }
                }
            }

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(DataTableNames.ConnectionsAccountSettingDataTable,null,null));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        #endregion

        #region Connections
        public ActionResult ConnectionsView()
        {
            return PartialView("~/Views/AccountSettings/Connections/_ConnectionsView.cshtml");
        }
        public ActionResult ConnectionsUpdate()
        {
            return PartialView("~/Views/AccountSettings/Connections/_ConnectionsUpdate.cshtml");
        }

        [Route("AccountSettings/ConnectionsAccountSettingDataTable/")]
        public JsonResult ConnectionsDataTable(DataTableRecievedOptions options)
        {
            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<ConnectionDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<ConnectionDataTableModel>(DataTable.Filters);
            }
            var All = (from a in DbCMS.codeConnections.AsNoTracking().AsExpandable()
                       join b in DbCMS.codeApplicationsLanguages.Where(x => x.LanguageID == LAN) on a.ApplicationGUID equals b.ApplicationGUID into LJ1
                       from R1 in LJ1.DefaultIfEmpty() 
                       join c in DbCMS.userConnections.Where(x=>x.UserGUID==UserGUID) on a.ConnectionGUID equals c.ConnectionGUID into LJ2
                       from R2 in LJ2.DefaultIfEmpty()
                       select new ConnectionDataTableModel
                       {
                           ConnectionGUID = a.ConnectionGUID,
                           Active = a.Active,
                           codeConnectionsRowVersion = a.codeConnectionsRowVersion,
                           InstanceDescription = a.InstanceDescription,
                           DataSource = a.DataSource,
                           InitialCatalog = a.InitialCatalog,
                           ApplicationGUID= R1.ApplicationGUID.ToString(),
                           ApplicationDescription = R1.ApplicationDescription,
                           DefaultConnection = R2.DefaultConnection == null ? false: R2.DefaultConnection
                       }).Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<ConnectionDataTableModel> Result = Mapper.Map<List<ConnectionDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ConnectionDefault(Guid PK)
        {
            return PartialView("~/Views/AccountSettings/Connections/_ConnectionUpdateModal.cshtml",
                DbCMS.codeConnections.Where(x=>x.ConnectionGUID==PK).FirstOrDefault());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ConnectionDefault(codeConnections model)
        {
            //change all default connection to specific application to false value 
            DateTime ExecutionTime = DateTime.Now;
            DbCMS.userConnections.Where(x => 
            DbCMS.codeConnections.Where(y => y.ApplicationGUID == model.ApplicationGUID).
            Select(z => z.ConnectionGUID).ToList().Contains(x.ConnectionGUID)
            && x.UserGUID == UserGUID).ToList().ForEach(x => x.DefaultConnection = false);
            //create or update default connection
            var userConnection= DbCMS.userConnections.Where(x => x.ConnectionGUID == model.ConnectionGUID && x.UserGUID == UserGUID).FirstOrDefault();
            if (userConnection != null)
            {
                userConnection.DefaultConnection = true;
            }
            else
            {
                userConnection = new userConnections();
                userConnection.UserConnectionGUID = Guid.NewGuid();
                userConnection.ConnectionGUID = model.ConnectionGUID;
                userConnection.UserGUID = UserGUID;
                userConnection.DefaultConnection = true;
                DbCMS.userConnections.Add(userConnection);
            }

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleUpdateMessage(DataTableNames.ConnectionsAccountSettingDataTable,
                   DbCMS.PrimaryKeyControl(model),
                   null));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        #endregion

        #region Activities Log
        public ActionResult ActivitiesLogView()
        {
            return PartialView("~/Views/AccountSettings/ActivitiesLog/_ActivitiesLogView.cshtml");
        }
        public ActionResult ActivitiesLogUpdate()
        {
            return PartialView("~/Views/AccountSettings/ActivitiesLog/_ActivitiesLogUpdate.cshtml",new AuditReportFilterModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MyAuditReport(AuditReportFilterModel Filter)
        {
            //System.Threading.Thread.Sleep(1000);

            var Result = DbCMS.spAuditReport(
                LAN, 
                Filter.From, 
                Filter.To, 
                Filter.ActionCategoryGUID, 
                Filter.ActionGUID, 
                Filter.ActionVerbGUID, 
                UserGUID, //Hard coded
                Filter.ExecutedByUserProfileGUID, 
                Filter.FieldName, 
                Filter.BeforeChange, 
                Filter.AfterChange, 
                Filter.OrganizationGUID, 
                Filter.RankID, 
                Filter.OrderBy)
                           .GroupBy(x => new { x.ActionGUID, x.ExecutionTime, x.ActionDescription, x.jsFunction, x.ExecutedBy, x.JobTitleDescription, x.OrganizationInstanceDescription })
                           .Select(x => new AuditModel
                           {
                               ActionDescription = x.Key.ActionDescription,
                               ExecutionTime = x.Key.ExecutionTime,
                               ActionGUID = x.Key.ActionGUID,
                               ExecutedBy = x.Key.ExecutedBy,
                               JobTitleDescription = x.Key.JobTitleDescription,
                               OrganizationInstanceDescription = x.Key.OrganizationInstanceDescription,
                               jsFunction = x.Key.jsFunction,
                               UpdatedFields = x.Where(y => y.FieldName != null).Select(z => new AuditFieldsModel { FieldName = z.FieldName, AfterChange = z.AfterChange, BeforeChange = z.BeforeChange }).ToList()
                           });

            if (Filter.OrderBy == "ASC")
            {
                return Json(Result.OrderBy(x => x.ExecutionTime).ToList(), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(Result.OrderByDescending(x => x.ExecutionTime).ToList(), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
    }
}