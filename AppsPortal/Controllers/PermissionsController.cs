using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AppsPortal.BaseControllers;
using AppsPortal.ViewModels;
using AppsPortal.Models;
using AppsPortal.Data;
using AppsPortal.Library;
using AutoMapper;
using System.Web.UI.WebControls;

namespace AppsPortal.Controllers
{
    public class PermissionsController : PortalBaseController
    {
        public JsonResult GetUserDetails(string ID)
        {
            if (string.IsNullOrEmpty(ID)) return null;
            return new JsonResult { Data = CMS.GetUser(Guid.Parse(ID)), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult GetUserDetailsForProfileToProfile(string ID)
        {

            if (string.IsNullOrEmpty(ID)) return null;

            var User = CMS.GetUser(Guid.Parse(ID));
            var ListOfProfiles = DropDownList.UserProfiles(Guid.Parse(ID));


            var OldProfiles = ListOfProfiles.Skip(1);
            var CurrentProfile = ListOfProfiles.Take(1);

            return Json(new { User = User, OldProfiles = OldProfiles, CurrentProfile = CurrentProfile },JsonRequestBehavior.AllowGet);

        }

        private dataAuditPermissions UpdatePermissionsAudit(Guid PermissionGUID, Guid ByUser, int Type, DateTime Time)
        {
            return new dataAuditPermissions
            {
                AuditPermissionGUID = Guid.NewGuid(),
                PermissionGUID = PermissionGUID,
                ByUserProfileGUID = ByUser,
                TransactionDateTime = Time,
                TransactionType = Type
            };
        }

        #region Permission Management
        [Route("Permissions/Management/")]
        public ActionResult Permissions()
        {
            return View("~/Views/Permissions/Permissions.cshtml");
        }

        [HttpPost]
        public ActionResult PermissionsSubmit(List<JsTreeLI_Attr> SubmittedPermissionsList, Guid TargetedUserProfileGUID, Guid TargetedApplicationGUID)
        {
            var bbbb = Session[SessionKeys.UserGUID].ToString();
            var aaaaa = Session[SessionKeys.UserProfileGUID].ToString();


            Guid ActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            DateTime TransactionDateTime = DateTime.Now;
            Guid ByUserProfileGUID = CMS.GetCurrentUserServiceJobGuidFromSession();

            List<userPermissions> CheckedPermissionsList = Mapper.Map(SubmittedPermissionsList.Where(x => x.status == true).ToList(), new List<userPermissions>());
            List<userPermissions> UncheckedPermissionList = Mapper.Map(SubmittedPermissionsList.Where(x => x.status == false).ToList(), new List<userPermissions>());

            List<dataAuditPermissions> dataAuditPermissions = new List<dataAuditPermissions>();
            List<userPermissions> NewPermissions = new List<userPermissions>();
            Guid PK;

            List<userPermissions> UserPermissions = DbCMS.userPermissions.AsNoTracking().Where(u => u.UserProfileGUID == TargetedUserProfileGUID).ToList();//Hit #1 bring all the permissions of a user. even if we want to change one. alternative solution is to use Contains, but what about 10,000 GUIDs in IN Clause?
            List<Guid> List1 = new List<Guid>();
            List<String> List2 = new List<String>();

            //Step #1: Cross the permissions to be activated with the database (deleted permissions) to enable them back
            List1 = CheckedPermissionsList.Select(p => p.ActionGUID).ToList();
            List2 = CheckedPermissionsList.Select(f => f.FactorsToken).ToList();

            var ToReactivate = DbCMS.userPermissions.Where(p => List1.Contains(p.ActionGUID) && List2.Contains(p.FactorsToken) && p.UserProfileGUID == TargetedUserProfileGUID).ToList();
            foreach (var dp in ToReactivate)
            {
                dp.Active = true;
                dataAuditPermissions.Add(UpdatePermissionsAudit(dp.PermissionGUID, ByUserProfileGUID, TransactionTypes.Grant, TransactionDateTime));
            }

            //Step #2: Stop the stopped permission (status = false)
            List1 = UncheckedPermissionList.Select(x => x.ActionGUID).ToList();
            List2 = UncheckedPermissionList.Select(x => x.FactorsToken).ToList();

            var ToStop = DbCMS.userPermissions.Where(x => List1.Contains(x.ActionGUID) && List2.Contains(x.FactorsToken) && x.UserProfileGUID == TargetedUserProfileGUID).ToList(); //Hit #2

            foreach (var permission in ToStop)
            {
                permission.Active = false;
                dataAuditPermissions.Add(UpdatePermissionsAudit(permission.PermissionGUID, ByUserProfileGUID, TransactionTypes.Revoke, TransactionDateTime));
            }

            //Step #3: add new permissions which are not exists on the db at all
            var ToAdd = (from a in CheckedPermissionsList where !UserPermissions.Any(x => x.ActionGUID == a.ActionGUID && x.FactorsToken == a.FactorsToken && x.UserProfileGUID == TargetedUserProfileGUID) select a).ToList();
            foreach (var np in ToAdd)
            {
                PK = Guid.NewGuid();
                NewPermissions.Add(new userPermissions()
                {
                    PermissionGUID = PK,
                    UserProfileGUID = TargetedUserProfileGUID,
                    ActionGUID = np.ActionGUID,
                    FactorsToken = np.FactorsToken,
                    Active = true,
                });
                dataAuditPermissions.Add(UpdatePermissionsAudit(PK, ByUserProfileGUID, TransactionTypes.Grant, TransactionDateTime));
            }

            DbCMS.userPermissions.AddRange(NewPermissions);
            DbCMS.dataAuditPermissions.AddRange(dataAuditPermissions);

            //List<Guid> ActionGuidsList = SubmittedPermissionsList.Select(x => Guid.Parse(x.actionGUID)).Distinct().ToList();
            try
            {
                DbCMS.SaveChanges();
                CMS.UpdateUserPermissionToken(TargetedUserProfileGUID,TargetedApplicationGUID);
                int er = DbCMS.SaveChanges();

                CMS.BuildUserMenus(CMS.GetUserGuidByProfileGuid(TargetedUserProfileGUID), LAN);

                return Json(new { Result = er.ToString() });
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        public ActionResult GetRowMaterials(PermissionsTreeFilter Filter)
        {
            return new JsonResult { Data = CMS.PermissionsTreeRowMaterials(Filter), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        #endregion

        #region Profile To Profile
        [Route("Permissions/ProfileToProfile/")]

        public ActionResult ProfileToProfile()
        {
            return View("~/Views/Permissions/ProfileToProfile.cshtml");
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ProfileToProfileSubmit(Guid SourceProfileGUID, Guid DestinationProfileGUID)
        {
            Guid ActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
            DateTime TransactionDateTime = DateTime.Now;
            Guid ByUserProfileGUID = new CMS(DbCMS).GetCurrentUserServiceJobGuidFromSession();
            List<dataAuditPermissions> dataAuditPermissions = new List<dataAuditPermissions>();

            var SourcePermissions = DbCMS.userPermissions.Where(x => x.UserProfileGUID == SourceProfileGUID && x.Active).ToList();
            foreach (var p in SourcePermissions)
            {
                p.UserProfileGUID = DestinationProfileGUID;
                dataAuditPermissions.Add(UpdatePermissionsAudit(p.PermissionGUID, ByUserProfileGUID, TransactionTypes.Grant, TransactionDateTime));
            }
            DbCMS.dataAuditPermissions.AddRange(dataAuditPermissions);
            try
            {
                DbCMS.SaveChanges();

                CMS.BuildUserMenus(CMS.GetUserGuidByProfileGuid(DestinationProfileGUID), LAN);

                return Json(DbCMS.SuccessMessage("Permissions Transfered Successfully"));
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }
        #endregion

        #region User To User

        [Route("Permissions/UserToUser/")]
        public ActionResult UserToUser()
        {
            return View("~/Views/Permissions/UserToUser.cshtml");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult UserToUserSubmit(Guid ApplicationGUID, Guid SourceProfileGUID, Guid[] DestinationProfileGUIDs)
        {

            foreach (var DestinationProfileGUID in DestinationProfileGUIDs)
            {
                Guid ActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");
                DateTime TransactionDateTime = DateTime.Now;
                Guid ByUserProfileGUID = new CMS(DbCMS).GetCurrentUserServiceJobGuidFromSession();
                List<dataAuditPermissions> dataAuditPermissions = new List<dataAuditPermissions>();

                var SrcPermissions = (from a in DbCMS.userPermissions.AsNoTracking().Where(x => x.UserProfileGUID == SourceProfileGUID && x.Active )
                                      join b in DbCMS.codeActions on a.ActionGUID equals b.ActionGUID
                                      join c in DbCMS.codeActionsCategories.Where(x=>x.ApplicationGUID == ApplicationGUID ) on b.ActionCategoryGUID equals c.ActionCategoryGUID
                                      select a).ToList();
                var DstPermissions = (from a in DbCMS.userPermissions.AsNoTracking().Where(x => x.UserProfileGUID == DestinationProfileGUID && x.Active)
                                      join b in DbCMS.codeActions on a.ActionGUID equals b.ActionGUID
                                      join c in DbCMS.codeActionsCategories.Where(x => x.ApplicationGUID == ApplicationGUID) on b.ActionCategoryGUID equals c.ActionCategoryGUID
                                      select a).ToList();
                //Bring active and deleted

                // Step #1: Remove matching permissions
                var Matching = (from a in SrcPermissions
                                join b in DstPermissions.Where(x => x.Active) on new { a.ActionGUID, a.FactorsToken } equals new { b.ActionGUID, b.FactorsToken }
                                select a).ToList();
                SrcPermissions.RemoveAll(x => Matching.Select(y => y.ActionGUID).ToList().Contains(x.ActionGUID));
                DstPermissions.RemoveAll(x => Matching.Select(y => y.ActionGUID).ToList().Contains(x.ActionGUID));


                //Step #2: Active Disabled on Destination
                var Activate = (from a in DstPermissions.Where(x => x.Active == false)
                                join b in SrcPermissions on new { a.ActionGUID, a.FactorsToken } equals new { b.ActionGUID, b.FactorsToken }
                                select a).ToList();

                foreach (var p in Activate)
                {
                    p.Active = true;
                }
                SrcPermissions.RemoveAll(x => Activate.Select(y => y.ActionGUID).ToList().Contains(x.ActionGUID));


                //Step #3: Deactivate Enabled on Destination
                List<Guid> List1 = Activate.Select(x => x.PermissionGUID).ToList();
                var Deactivate = (from a in DstPermissions.Where(x => x.Active && !List1.Contains(x.PermissionGUID))
                                  select a).ToList();

                foreach (var p in Deactivate)
                {
                    p.Active = false;
                }

                //Step #4: Add Missing (Left Over)
                List<userPermissions> NewPermissions = new List<userPermissions>();
                foreach (var p in SrcPermissions)
                {
                    Guid PK = Guid.NewGuid();
                    NewPermissions.Add(new userPermissions
                    {
                        PermissionGUID = PK,
                        ActionGUID = p.ActionGUID,
                        FactorsToken = p.FactorsToken,
                        UserProfileGUID = DestinationProfileGUID,
                        Active = true
                    });
                    dataAuditPermissions.Add(UpdatePermissionsAudit(PK, ByUserProfileGUID, TransactionTypes.Grant, TransactionDateTime));
                }
                DbCMS.userPermissions.AddRange(NewPermissions);

              
                try
                {
                   
                    DbCMS.SaveChanges();
                    CMS.UpdateUserPermissionToken(DestinationProfileGUID, ApplicationGUID);
                    int er = DbCMS.SaveChanges();
                    CMS.BuildUserMenus(CMS.GetUserGuidByProfileGuid(DestinationProfileGUID), LAN);

                    
                }
                catch (Exception ex)
                {
                    return Json(DbCMS.ErrorMessage(ex.Message));
                }

            }
            return Json(DbCMS.SuccessMessage("Permissions Copied Successfully"));

        }
        #endregion


        public string ActionsClass()
        {
            string strClass = "public static class Permissions <br>{<br>";
            var Result = (from a in DbCMS.codeActions.Where(x => x.Active)
                          join b in DbCMS.codeActionsVerbsLanguages.Where(x => x.Active && x.LanguageID == "EN") on a.ActionVerbGUID equals b.ActionVerbGUID
                          join c in DbCMS.codeActionsCategoriesLanguages.Where(x => x.Active && x.LanguageID == "EN") on a.ActionCategoryGUID equals c.ActionCategoryGUID
                          join d in DbCMS.codeActionsEntitiesLanguages.Where(x=>x.Active && x.LanguageID  == "EN") on a.ActionEntityGUID equals d.ActionEntityGUID
                          orderby c.ActionCategoryDescription, a.ActionID, b.ActionVerbDescription
                          select new
                          {
                              c.ActionCategoryDescription,
                              b.ActionVerbDescription,
                              d.ActionEntityDescription,
                              a.ActionID,
                              a.ActionGUID,
                          }).GroupBy(x => x.ActionCategoryDescription).ToList();

            foreach (var cat in Result)
            {
                strClass += "public static class " + cat.Key.Replace(" Codes Management", "").Replace(" ", "") + "<br>{<br>";

                if(cat.Key == "Default Actions")
                {
                    foreach (var act in cat.OrderBy(x => x.ActionID))
                    {
                        strClass += "// " + act.ActionVerbDescription.Replace("Add", "Create") + " " + act.ActionEntityDescription + "<br>";

                        strClass += "public readonly static Guid " + act.ActionVerbDescription.Replace("Add", "Create") + act.ActionEntityDescription.Replace(" ","") + "Guid = Guid.Parse(\"" + act.ActionGUID.ToString() + "\");<br>";
                        strClass += "<br>";
                    }
                }
                else
                {
                    foreach (var act in cat.OrderBy(x => x.ActionID))
                    {
                        strClass += "// " + act.ActionVerbDescription.Replace("Add", "Create") + " " + act.ActionEntityDescription + "<br>";

                        if (act.ActionID > 0)
                        {
                            strClass += "public readonly static int " + act.ActionVerbDescription.Replace("Add", "Create") + " = " + act.ActionID.ToString() + ";<br>";
                        }
                        strClass += "public readonly static Guid " + act.ActionVerbDescription.Replace("Add", "Create") + "Guid = Guid.Parse(\"" + act.ActionGUID.ToString() + "\");<br>";
                        strClass += "<br>";
                    }
                }
                


                strClass += "}<br>";

            }


            return (strClass + "}");
        }
    }
}