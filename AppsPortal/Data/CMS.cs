using AppsPortal.Extensions;
using RES_Repo.Globalization;
using AppsPortal.Library;
using AppsPortal.Models;
using AppsPortal.ViewModels;
using RES_Repo.Globalization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using RES_Repo.Globalization;



namespace AppsPortal.Data
{
    public class CMS
    {
        private CMSEntities DbCMS;
        private string EmailAddress = HttpContext.Current.Session != null ? HttpContext.Current.Session[SessionKeys.EmailAddress] != null ? HttpContext.Current.Session[SessionKeys.EmailAddress].ToString() : "" : "";
        private Guid UserGUID = HttpContext.Current.Session != null ? HttpContext.Current.Session[SessionKeys.UserGUID] != null ? Guid.Parse(HttpContext.Current.Session[SessionKeys.UserGUID].ToString()) : Guid.Empty : Guid.Empty;
        private Guid UserProfileGUID = HttpContext.Current.Session != null ? HttpContext.Current.Session[SessionKeys.UserProfileGUID] != null ? Guid.Parse(HttpContext.Current.Session[SessionKeys.UserProfileGUID].ToString()) : Guid.Empty : Guid.Empty;
        string LAN = Languages.CurrentLanguage().ToUpper();

        public CMS(CMSEntities DbCMS)
        {
            this.DbCMS = DbCMS;
            //try { Guid UserGUID = HttpContext.Current.Session[SessionKeys.UserGUID] != null ? Guid.Parse(HttpContext.Current.Session[SessionKeys.UserGUID].ToString()) : Guid.Empty; } catch { }
            //try { UserProfileGUID = HttpContext.Current.Session[SessionKeys.UserProfileGUID] != null ? Guid.Parse(HttpContext.Current.Session[SessionKeys.UserProfileGUID].ToString()) : Guid.Empty; } catch { }
            //try { LAN = Languages.CurrentLanguage().ToUpper(); } catch { }
        }

        public CMS()
        {
            this.DbCMS = new CMSEntities();
            //try { Guid UserGUID = HttpContext.Current.Session[SessionKeys.UserGUID] != null ? Guid.Parse(HttpContext.Current.Session[SessionKeys.UserGUID].ToString()) : Guid.Empty; } catch { }
            //try { UserProfileGUID = HttpContext.Current.Session[SessionKeys.UserProfileGUID] != null ? Guid.Parse(HttpContext.Current.Session[SessionKeys.UserProfileGUID].ToString()) : Guid.Empty; } catch { }
            //try { LAN = Languages.CurrentLanguage().ToUpper(); } catch { }
        }

        #region User Data Region
        public UserModel GetUser(Guid UserProfileGUID)
        {
            UserModel user = (from a in DbCMS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.Active)
                              join b in DbCMS.userPersonalDetails.Where(x => x.Active) on a.UserGUID equals b.UserGUID
                              join c in DbCMS.codeTablesValuesLanguages.Where(x => x.LanguageID == LAN && x.Active) on b.GenderGUID equals c.ValueGUID
                              join d in DbCMS.userServiceHistory.Where(x => x.Active) on a.UserGUID equals d.UserGUID

                              join e in DbCMS.userProfiles.Where(x => x.Active && x.UserProfileGUID == UserProfileGUID) on d.ServiceHistoryGUID equals e.ServiceHistoryGUID
                              join f in DbCMS.codeOrganizationsInstancesLanguages.Where(x => x.LanguageID == LAN && x.Active) on e.OrganizationInstanceGUID equals f.OrganizationInstanceGUID
                              //join g in DbCMS.codeJobTitlesLanguages.Where(x => x.LanguageID == LAN && x.Active) on e.JobTitleGUID equals g.JobTitleGUID 
                              join h in DbCMS.codeOrganizationsLanguages.Where(x => x.LanguageID == LAN && x.Active) on d.OrganizationGUID equals h.OrganizationGUID

                              orderby e.FromDate descending
                              select new
                              UserModel
                              {
                                  UserGUID = a.UserGUID,
                                  Gender = c.ValueDescription,
                                  FirstName = a.FirstName,
                                  Surname = a.Surname,
                                  JobTitle = /*g.JobTitleDescription,*/"",
                                  EmailAddress = d.EmailAddress,
                                  Organization = h.OrganizationDescription
                              }).FirstOrDefault();

            //UserModel user = (from u in DbCMS.userPersonalDetailsLanguage)

            user.ProfilePhoto = new Portal().ProfilePhoto(user.UserGUID);
            return user;
        }

        public Guid GetUserGuidByProfileGuid(Guid UserProfileGUID)
        {
            return DbCMS.userProfiles.Where(x => x.UserProfileGUID == UserProfileGUID).Select(x => x.userServiceHistory.UserGUID).FirstOrDefault();
        }

        public string GetCurrentUserEmail(Guid UserGUID)
        {
            var emailAddress = (from a in DbCMS.userServiceHistory.Where(x => x.Active && x.UserGUID == UserGUID)
                                join b in DbCMS.userProfiles.Where(x => x.Active) on a.ServiceHistoryGUID equals b.ServiceHistoryGUID
                                orderby b.FromDate descending
                                select a.EmailAddress).FirstOrDefault();
            return emailAddress;
        }

        public string GetFullName(Guid UserGUID, string Language)
        {
            string fullname = "";
            string LAN = Language.Substring(0, 2);

            var FullName = DbCMS.userPersonalDetailsLanguage
                .Where(u => u.userPersonalDetails.UserGUID == UserGUID && u.LanguageID == LAN && u.Active)
                .Select(u => new { u.FirstName, u.Surname }).FirstOrDefault();

            if (FullName == null)
            {
                fullname = "Your Name is not set in " + Language + " Language";
                return fullname;
            }
            return FullName.FirstName + " " + FullName.Surname;
        }

        public Guid GetCurrentUserServiceJobGuidFromSession()
        {
            return Guid.Parse(HttpContext.Current.Session[SessionKeys.UserProfileGUID].ToString());
        }

        public List<UserModel> GetOnlineUsers()
        {
            List<Guid> UsersGuids = (List<Guid>)HttpContext.Current.Application["OnlineUsers"];
            List<UserModel> Users = (from a in DbCMS.userAccounts.Where(x => UsersGuids.Contains(x.UserGUID) && x.Active)
                                     join b in DbCMS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.Active) on a.UserGUID equals b.UserGUID
                                     select new UserModel
                                     {
                                         UserGUID = a.UserGUID,
                                         FirstName = b.FirstName,
                                         Surname = b.Surname
                                     }).ToList();
            return Users;

        }
        //public Guid GetUserCurrentOrganization(Guid UserGUID)
        //{
        //    var OrganizationGUID = (from a in DbCMS.userProfiles
        //                            where a.userServiceHistory.UserGUID == UserGUID
        //                            && a.Active
        //                            orderby a.FromDate descending
        //                            select a.userServiceHistory.OrganizationGUID).FirstOrDefault();
        //    return OrganizationGUID;
        //}

        //public Guid GetUserCurrentOperation(Guid UserGUID)
        //{
        //    var OperationGUID = (from a in DbCMS.userProfiles
        //                         where a.userServiceHistory.UserGUID == UserGUID
        //                         && a.Active
        //                         orderby a.FromDate descending
        //                         select a.OperationGUID).FirstOrDefault();
        //    return OperationGUID;
        //}

        //public Guid GetUserCurrentOperationOrganization(Guid UserGUID)
        //{
        //    var OrganizationOperationGUID = (from a in DbCMS.userProfiles
        //                                     from o in DbCMS.codeOrganizationsOperations
        //                                     where a.OperationGUID == o.OperationGUID
        //                                     && a.userServiceHistory.OrganizationGUID == o.OrganizationGUID
        //                                     && a.userServiceHistory.UserGUID == UserGUID
        //                                     && a.Active
        //                                     && o.Active
        //                                     orderby a.FromDate descending
        //                                     select o.OrganizationOperationGUID).FirstOrDefault();
        //    return OrganizationOperationGUID;
        //}
        #endregion

        #region Other
        public Dictionary<string, string> ManagerTypesList()
        {
            var Result = (from a in DbCMS.codeTablesValues.Where(a => a.Active && a.codeTables.TableName == "ManagerTypes")
                          join b in DbCMS.codeTablesValuesLanguages.Where(l => l.LanguageID == LAN && l.Active) on a.ValueGUID equals b.ValueGUID
                          select new
                          {
                              ID = a.ValueGUID.ToString(),
                              Value = b.ValueDescription
                          }).ToDictionary(t => t.ID, t => t.Value);
            return Result;
        }

        public string GetManagerType(Guid? ManagerTypeGUID)
        {
            if (ManagerTypeGUID == null)
            {
                return "";
            }
            string ManagerType = (from a in DbCMS.codeTablesValues.Where(a => a.Active && a.codeTables.TableGUID == LookupTables.ManagerTypes)
                                  join b in DbCMS.codeTablesValuesLanguages.Where(l => l.LanguageID == LAN && l.Active && l.ValueGUID == ManagerTypeGUID) on a.ValueGUID equals b.ValueGUID
                                  select b.ValueDescription).FirstOrDefault();

            return ManagerType;
        }

        public string GetCodeTableName(Guid? TableGuid)
        {
            return TableGuid != null ? DbCMS.codeTables.Where(t => t.TableGUID == TableGuid && t.Active).FirstOrDefault().TableName : "";
        }

        Guid CodeTablesMenuGuid = Guid.Parse("180899B5-C15C-4C51-BD89-E4944495299B");
        Guid LookupTablesMenuGuid = Guid.Parse("92F513B9-846E-468F-9534-B0027D507735");
        Guid ApplicationsMenuGuid = Guid.Parse("2ACCAB0E-E445-4A69-A400-C4B684C2590D");

        public List<NavigationMenu> ApplicationMenus(Guid ApplicationGUID)
        {
            //Public Union Authorized
            List<NavigationMenu> Menus = (from a in DbCMS.codeMenus
                                          from b in DbCMS.codeMenusLanguages
                                          from c in DbCMS.codeMenus
                                          from d in DbCMS.codeMenusLanguages
                                          where a.MenuGUID == b.MenuGUID && c.MenuGUID == d.MenuGUID && a.MenuGUID == c.ParentGUID && c.ActionGUID == null && a.ApplicationGUID == ApplicationGUID && a.Active && c.Active && b.LanguageID == LAN && d.LanguageID == LAN
                                          && a.MenuGUID != CodeTablesMenuGuid && a.MenuGUID != LookupTablesMenuGuid
                                          select new
                                          {
                                              MenuGUID = a.MenuGUID,
                                              RootMenuDescription = b.MenuDescription,
                                              RootMenuUrl = a.ActionUrl,
                                              SubMenuDescriptoin = d.MenuDescription,
                                              SubMenuUrl = c.ActionUrl,
                                              RootMenuSortID = a.SortID,
                                              SubMenuSortID = c.SortID
                                          }).Union((from a in DbCMS.codeMenus
                                                    from b in DbCMS.codeMenusLanguages
                                                    from d in DbCMS.codeMenus
                                                    from e in DbCMS.codeMenusLanguages
                                                    from f in DbCMS.userPermissions
                                                    where
                                                    a.MenuGUID == b.MenuGUID &&
                                                    d.MenuGUID == e.MenuGUID &&
                                                    a.MenuGUID == d.ParentGUID &&
                                                    d.ActionGUID != null &&
                                                    d.ActionGUID == f.ActionGUID &&
                                                    f.UserProfileGUID == UserProfileGUID &&
                                                    f.Active &&
                                                    a.ApplicationGUID == ApplicationGUID &&
                                                    a.Active &&
                                                    d.Active &&
                                                    b.LanguageID == LAN &&
                                                    e.LanguageID == LAN
                                                    && a.MenuGUID != CodeTablesMenuGuid && a.MenuGUID != LookupTablesMenuGuid
                                                    select new { MenuGUID = a.MenuGUID, RootMenuDescription = b.MenuDescription, RootMenuUrl = a.ActionUrl, SubMenuDescriptoin = e.MenuDescription, SubMenuUrl = d.ActionUrl, RootMenuSortID = a.SortID, SubMenuSortID = d.SortID }))
                                                    .GroupBy(x => new
                                                    {
                                                        MenuID = x.MenuGUID,
                                                        RootMenuDescription = x.RootMenuDescription,
                                                        RootMenuUrl = x.RootMenuUrl,
                                                        RootMenuSortID = x.RootMenuSortID
                                                    })
                                                    .Select(x => new NavigationMenu
                                                    {
                                                        MenuID = x.Key.MenuID,
                                                        RootMenuDescription = x.Key.RootMenuDescription,
                                                        RootMenuUrl = x.Key.RootMenuUrl,
                                                        RootMenuSortID = x.Key.RootMenuSortID,
                                                        SubMenu = x.Select(sm => new NavigationSubMenu
                                                        {
                                                            //SubMenuID = sm.MenuGUID,
                                                            SubMenuDescription = sm.SubMenuDescriptoin,
                                                            SubMenuUrl = sm.SubMenuUrl,
                                                            SubMenuSortID = sm.SubMenuSortID
                                                        })
                                                            .OrderBy(so => so.SubMenuSortID).ToList()
                                                    }).OrderBy(s => s.RootMenuSortID).ToList();

            //92f513b9-846e-468f-9534-b0027d507735 Lookup Table Menu ID


            //Step #1: bring the Lookup Menu Action and Factors
            Guid AccessLookupActionGUID = Permissions.LookupValues.AccessGuid;
            Guid LookupMenuID = Guid.Parse("92f513b9-846e-468f-9534-b0027d507735");

            NavigationMenu LookupMenu = Menus.Where(m => m.MenuID == LookupMenuID).FirstOrDefault();

            List<NavigationSubMenu> NavigationSubMenu = new List<NavigationSubMenu>();
            if (LookupMenu != null)
            {
                foreach (var menu in LookupMenu.SubMenu)
                {
                    string TableID = menu.SubMenuUrl.Split('/').Last();
                    //string PK = typeof(LookupValues).GetField(TableName, BindingFlags.Public | BindingFlags.Static).GetValue(null).ToString();

                    if (new CMS().HasAction(Permissions.LookupValues.Access, Apps.CMS, TableID))
                    {
                        //Add
                        var SubMenuElement = Menus.Where(x => x.MenuID == LookupMenuID).FirstOrDefault().SubMenu.Where(x => x.SubMenuUrl == menu.SubMenuUrl).FirstOrDefault();
                        NavigationSubMenu.Add(SubMenuElement);

                    }
                }
                Menus.Where(x => x.MenuID == LookupMenuID).FirstOrDefault().SubMenu = NavigationSubMenu;

                if (Menus.Where(x => x.MenuID == LookupMenuID).FirstOrDefault().SubMenu.Count() == 0)
                {
                    Menus.Remove(Menus.Where(x => x.MenuID == LookupMenuID).FirstOrDefault());
                }
            }
            //Remove services menu if user is not UNHCR staff
            Guid ServicesMenuGUID = Guid.Parse("c126f762-16f8-44e1-9084-84c2ac679391");
            if (!System.Web.HttpContext.Current.Session[SessionKeys.EmailAddress].ToString().ToUpper().EndsWith("UNHCR.ORG"))
            {
                Menus.Remove(Menus.Where(x => x.MenuID == ServicesMenuGUID).FirstOrDefault());
            }
            return Menus;
        }
        public List<NavigationMenu> ApplicationMenusWithLan(Guid ApplicationGUID, string LAN)
        {
            //Public Union Authorized
            UserProfileGUID = HttpContext.Current.Session != null ? HttpContext.Current.Session[SessionKeys.UserProfileGUID] != null ? Guid.Parse(HttpContext.Current.Session[SessionKeys.UserProfileGUID].ToString()) : Guid.Empty : Guid.Empty;
            List<NavigationMenu> Menus = (from a in DbCMS.codeMenus
                                          from b in DbCMS.codeMenusLanguages
                                          from c in DbCMS.codeMenus
                                          from d in DbCMS.codeMenusLanguages
                                          where a.MenuGUID == b.MenuGUID && c.MenuGUID == d.MenuGUID && a.MenuGUID == c.ParentGUID && c.ActionGUID == null && a.ApplicationGUID == Apps.CMS && a.Active && c.Active && b.LanguageID == LAN && d.LanguageID == LAN
                                          && a.MenuGUID != CodeTablesMenuGuid && a.MenuGUID != LookupTablesMenuGuid
                                          select new
                                          {
                                              MenuGUID = a.MenuGUID,
                                              RootMenuDescription = b.MenuDescription,
                                              RootMenuUrl = a.ActionUrl,
                                              SubMenuDescriptoin = d.MenuDescription,
                                              SubMenuUrl = c.ActionUrl,
                                              RootMenuSortID = a.SortID,
                                              SubMenuSortID = c.SortID
                                          }).Union((from a in DbCMS.codeMenus
                                                    from b in DbCMS.codeMenusLanguages
                                                    from d in DbCMS.codeMenus
                                                    from e in DbCMS.codeMenusLanguages
                                                    from f in DbCMS.userPermissions
                                                    where
                                                    a.MenuGUID == b.MenuGUID &&
                                                    d.MenuGUID == e.MenuGUID &&
                                                    a.MenuGUID == d.ParentGUID &&
                                                    d.ActionGUID != null &&
                                                    d.ActionGUID == f.ActionGUID &&
                                                    f.UserProfileGUID == UserProfileGUID &&
                                                    f.Active &&
                                                    a.ApplicationGUID == ApplicationGUID &&
                                                    a.Active &&
                                                    d.Active &&
                                                    b.LanguageID == LAN &&
                                                    e.LanguageID == LAN
                                                    && a.MenuGUID != CodeTablesMenuGuid && a.MenuGUID != LookupTablesMenuGuid
                                                    select new
                                                    {
                                                        MenuGUID = a.MenuGUID,
                                                        RootMenuDescription = b.MenuDescription,
                                                        RootMenuUrl = a.ActionUrl,
                                                        SubMenuDescriptoin = e.MenuDescription,
                                                        SubMenuUrl = d.ActionUrl,
                                                        RootMenuSortID = a.SortID,
                                                        SubMenuSortID = d.SortID
                                                    })).GroupBy(x => new
                                                    {
                                                        MenuID = x.MenuGUID,
                                                        RootMenuDescription = x.RootMenuDescription,
                                                        RootMenuUrl = x.RootMenuUrl,
                                                        RootMenuSortID = x.RootMenuSortID
                                                    }).Select(x => new NavigationMenu
                                                    {
                                                        MenuID = x.Key.MenuID,
                                                        RootMenuDescription = x.Key.RootMenuDescription,
                                                        RootMenuUrl = x.Key.RootMenuUrl,
                                                        RootMenuSortID = x.Key.RootMenuSortID,
                                                        SubMenu = x.Select(sm => new NavigationSubMenu
                                                        {
                                                            //SubMenuID = sm.MenuGUID,
                                                            SubMenuDescription = sm.SubMenuDescriptoin,
                                                            SubMenuUrl = sm.SubMenuUrl,
                                                            SubMenuSortID = sm.SubMenuSortID
                                                        })
                                                            .OrderBy(so => so.SubMenuSortID).ToList()
                                                    }).OrderBy(s => s.RootMenuSortID).ToList();
            //92f513b9-846e-468f-9534-b0027d507735 Lookup Table Menu ID

            //Add the application Menu
            var appMenu = DbCMS.codeMenusLanguages.Where(x => x.Active && x.LanguageID == LAN && x.MenuGUID == ApplicationsMenuGuid).Select(x =>
                    new NavigationMenu
                    {
                        MenuID = x.MenuGUID,
                        RootMenuDescription = x.MenuDescription,
                        RootMenuUrl = x.codeMenus.ActionUrl,
                        RootMenuSortID = 0,

                    }).FirstOrDefault();

            appMenu.SubMenu = new CMS().GetHomePageApplications(LAN).Select(y =>
                new NavigationSubMenu
                {
                    SubMenuDescription = y.Description,
                    SubMenuUrl = "/" + y.URL,
                    SubMenuSortID = 0
                }).ToList();
            if (appMenu.SubMenu.Count() > 0)
            {
                Menus.Add(appMenu);
                Menus = Menus.OrderBy(s => s.RootMenuSortID).ToList();
            }


            //Step #1: bring the Lookup Menu Action and Factors
            Guid AccessLookupActionGUID = Permissions.LookupValues.AccessGuid;
            Guid LookupMenuID = Guid.Parse("92f513b9-846e-468f-9534-b0027d507735");

            NavigationMenu LookupMenu = Menus.Where(m => m.MenuID == LookupMenuID).FirstOrDefault();

            List<NavigationSubMenu> NavigationSubMenu = new List<NavigationSubMenu>();
            if (LookupMenu != null)
            {
                foreach (var menu in LookupMenu.SubMenu)
                {
                    string TableID = menu.SubMenuUrl.Split('/').Last();
                    //string PK = typeof(LookupValues).GetField(TableName, BindingFlags.Public | BindingFlags.Static).GetValue(null).ToString();

                    if (new CMS().HasAction(Permissions.LookupValues.Access, Apps.CMS, TableID))
                    {
                        //Add
                        var SubMenuElement = Menus.Where(x => x.MenuID == LookupMenuID).FirstOrDefault().SubMenu.Where(x => x.SubMenuUrl == menu.SubMenuUrl).FirstOrDefault();
                        NavigationSubMenu.Add(SubMenuElement);

                    }
                }
                Menus.Where(x => x.MenuID == LookupMenuID).FirstOrDefault().SubMenu = NavigationSubMenu;

                if (Menus.Where(x => x.MenuID == LookupMenuID).FirstOrDefault().SubMenu.Count() == 0)
                {
                    Menus.Remove(Menus.Where(x => x.MenuID == LookupMenuID).FirstOrDefault());
                }
            }
            //Remove services menu if user is not UNHCR staff
            Guid ServicesMenuGUID = Guid.Parse("c126f762-16f8-44e1-9084-84c2ac679391");
            if (!System.Web.HttpContext.Current.Session[SessionKeys.EmailAddress].ToString().ToUpper().EndsWith("UNHCR.ORG"))
            {
                Menus.Remove(Menus.Where(x => x.MenuID == ServicesMenuGUID).FirstOrDefault());
            }
            return Menus;
        }
        public List<NavigationMenu> PublicMenus(Guid ApplicationGUID, string LAN)
        {
            //Public Union Authorized
            List<NavigationMenu> Menus = (from a in DbCMS.codeMenus
                                          from b in DbCMS.codeMenusLanguages
                                          from c in DbCMS.codeMenus
                                          from d in DbCMS.codeMenusLanguages
                                          where a.MenuGUID == b.MenuGUID && c.MenuGUID == d.MenuGUID && a.MenuGUID == c.ParentGUID && c.ActionGUID == null && a.ApplicationGUID == ApplicationGUID && a.Active && c.Active && b.LanguageID == LAN && d.LanguageID == LAN
                                          && a.MenuGUID != CodeTablesMenuGuid && a.MenuGUID != LookupTablesMenuGuid
                                          select new
                                          {
                                              MenuGUID = a.MenuGUID,
                                              RootMenuDescription = b.MenuDescription,
                                              RootMenuUrl = a.ActionUrl,
                                              SubMenuDescriptoin = d.MenuDescription,
                                              SubMenuUrl = c.ActionUrl,
                                              RootMenuSortID = a.SortID,
                                              SubMenuSortID = c.SortID
                                          })
                                                    .GroupBy(x => new
                                                    {
                                                        MenuID = x.MenuGUID,
                                                        RootMenuDescription = x.RootMenuDescription,
                                                        RootMenuUrl = x.RootMenuUrl,
                                                        RootMenuSortID = x.RootMenuSortID
                                                    })
                                                    .Select(x => new NavigationMenu
                                                    {
                                                        MenuID = x.Key.MenuID,
                                                        RootMenuDescription = x.Key.RootMenuDescription,
                                                        RootMenuUrl = x.Key.RootMenuUrl,
                                                        RootMenuSortID = x.Key.RootMenuSortID,
                                                        SubMenu = x.Select(sm => new NavigationSubMenu
                                                        {
                                                            //SubMenuID = sm.MenuGUID,
                                                            SubMenuDescription = sm.SubMenuDescriptoin,
                                                            SubMenuUrl = sm.SubMenuUrl,
                                                            SubMenuSortID = sm.SubMenuSortID
                                                        })
                                                            .OrderBy(so => so.SubMenuSortID).ToList()
                                                    }).OrderBy(s => s.RootMenuSortID).ToList();
            //92f513b9-846e-468f-9534-b0027d507735 Lookup Table Menu ID

            //Step #1: bring the Lookup Menu Action and Factors
            Guid AccessLookupActionGUID = Permissions.LookupValues.AccessGuid;
            Guid LookupMenuID = Guid.Parse("92f513b9-846e-468f-9534-b0027d507735");

            NavigationMenu LookupMenu = Menus.Where(m => m.MenuID == LookupMenuID).FirstOrDefault();

            List<NavigationSubMenu> NavigationSubMenu = new List<NavigationSubMenu>();
            if (LookupMenu != null)
            {
                foreach (var menu in LookupMenu.SubMenu)
                {
                    string TableID = menu.SubMenuUrl.Split('/').Last();
                    //string PK = typeof(LookupValues).GetField(TableName, BindingFlags.Public | BindingFlags.Static).GetValue(null).ToString();

                    if (new CMS().HasAction(Permissions.LookupValues.Access, Apps.CMS, TableID))
                    {
                        //Add
                        var SubMenuElement = Menus.Where(x => x.MenuID == LookupMenuID).FirstOrDefault().SubMenu.Where(x => x.SubMenuUrl == menu.SubMenuUrl).FirstOrDefault();
                        NavigationSubMenu.Add(SubMenuElement);

                    }
                }
                Menus.Where(x => x.MenuID == LookupMenuID).FirstOrDefault().SubMenu = NavigationSubMenu;

                if (Menus.Where(x => x.MenuID == LookupMenuID).FirstOrDefault().SubMenu.Count() == 0)
                {
                    Menus.Remove(Menus.Where(x => x.MenuID == LookupMenuID).FirstOrDefault());
                }
            }
            return Menus;
        }


        public List<NavigationMenu> CodesMenus()
        {
            //Public Union Authorized
            List<NavigationMenu> Menus = (from a in DbCMS.codeMenus
                                          from b in DbCMS.codeMenusLanguages
                                          from c in DbCMS.codeMenus
                                          from d in DbCMS.codeMenusLanguages
                                          where a.MenuGUID == b.MenuGUID && c.MenuGUID == d.MenuGUID && a.MenuGUID == c.ParentGUID && c.ActionGUID == null && a.ApplicationGUID == Apps.CMS && a.Active && c.Active && b.LanguageID == LAN && d.LanguageID == LAN
                                          && (a.MenuGUID == CodeTablesMenuGuid || a.MenuGUID == LookupTablesMenuGuid)
                                          select new
                                          {
                                              MenuGUID = a.MenuGUID,
                                              RootMenuDescription = b.MenuDescription,
                                              RootMenuUrl = a.ActionUrl,
                                              SubMenuDescriptoin = d.MenuDescription,
                                              SubMenuUrl = c.ActionUrl,
                                              RootMenuSortID = a.SortID,
                                              SubMenuSortID = c.SortID
                                          }).Union((from a in DbCMS.codeMenus
                                                    from b in DbCMS.codeMenusLanguages
                                                    from d in DbCMS.codeMenus
                                                    from e in DbCMS.codeMenusLanguages
                                                    from f in DbCMS.userPermissions
                                                    where
                                                    a.MenuGUID == b.MenuGUID &&
                                                    d.MenuGUID == e.MenuGUID &&
                                                    a.MenuGUID == d.ParentGUID &&
                                                    d.ActionGUID != null &&
                                                    d.ActionGUID == f.ActionGUID &&
                                                    f.UserProfileGUID == UserProfileGUID &&
                                                    f.Active &&
                                                    a.ApplicationGUID == Apps.CMS &&
                                                    a.Active &&
                                                    d.Active &&
                                                    b.LanguageID == LAN &&
                                                    e.LanguageID == LAN
                                                    && (a.MenuGUID == CodeTablesMenuGuid || a.MenuGUID == LookupTablesMenuGuid)
                                                    select new { MenuGUID = a.MenuGUID, RootMenuDescription = b.MenuDescription, RootMenuUrl = a.ActionUrl, SubMenuDescriptoin = e.MenuDescription, SubMenuUrl = d.ActionUrl, RootMenuSortID = a.SortID, SubMenuSortID = d.SortID }))
                                                    .GroupBy(x => new
                                                    {
                                                        MenuID = x.MenuGUID,
                                                        RootMenuDescription = x.RootMenuDescription,
                                                        RootMenuUrl = x.RootMenuUrl,
                                                        RootMenuSortID = x.RootMenuSortID
                                                    })
                                                    .Select(x => new NavigationMenu
                                                    {
                                                        MenuID = x.Key.MenuID,
                                                        RootMenuDescription = x.Key.RootMenuDescription,
                                                        RootMenuUrl = x.Key.RootMenuUrl,
                                                        RootMenuSortID = x.Key.RootMenuSortID,
                                                        SubMenu = x.Select(sm => new NavigationSubMenu
                                                        {
                                                            //SubMenuID = sm.MenuGUID,
                                                            SubMenuDescription = sm.SubMenuDescriptoin,
                                                            SubMenuUrl = sm.SubMenuUrl,
                                                            SubMenuSortID = sm.SubMenuSortID
                                                        })
                                                            .OrderBy(so => so.SubMenuDescription).ToList()
                                                    }).OrderBy(s => s.RootMenuSortID).ToList();
            //92f513b9-846e-468f-9534-b0027d507735 Lookup Table Menu ID

            //Step #1: bring the Lookup Menu Action and Factors
            Guid AccessLookupActionGUID = Permissions.LookupValues.AccessGuid;
            Guid LookupMenuID = Guid.Parse("92f513b9-846e-468f-9534-b0027d507735");

            NavigationMenu LookupMenu = Menus.Where(m => m.MenuID == LookupMenuID).FirstOrDefault();

            List<NavigationSubMenu> NavigationSubMenu = new List<NavigationSubMenu>();
            if (LookupMenu != null)
            {
                foreach (var menu in LookupMenu.SubMenu)
                {
                    string TableID = menu.SubMenuUrl.Split('/').Last();
                    //string PK = typeof(LookupValues).GetField(TableName, BindingFlags.Public | BindingFlags.Static).GetValue(null).ToString();

                    if (new CMS().HasAction(Permissions.LookupValues.Access, Apps.CMS, TableID))
                    {
                        //Add
                        var SubMenuElement = Menus.Where(x => x.MenuID == LookupMenuID).FirstOrDefault().SubMenu.Where(x => x.SubMenuUrl == menu.SubMenuUrl).FirstOrDefault();
                        NavigationSubMenu.Add(SubMenuElement);

                    }
                }
                Menus.Where(x => x.MenuID == LookupMenuID).FirstOrDefault().SubMenu = NavigationSubMenu;

                if (Menus.Where(x => x.MenuID == LookupMenuID).FirstOrDefault().SubMenu.Count() == 0)
                {
                    Menus.Remove(Menus.Where(x => x.MenuID == LookupMenuID).FirstOrDefault());
                }
            }
            return Menus;
        }

        public string DataTableControl(string DataTableName, string FormController, string Area, string Type, string EditIndicator, Guid? PK, int OrderBy, Guid? FK = null, bool SelectAll = false)
        {
            List<DataTableRender> dt = DbCMS.sysDataTablesProperties
            .Where(a => a.DataTableID == DataTableName)
            .Select(a => new DataTableRender { name = a.Name, data = a.Data, orderable = a.Orderable, searchable = a.Searchable, targets = a.Target, responsivePriority = a.ResponsivePriority, className = a.CssClass, width = a.ColumnWidth, render = a.RenderHtml, defaultContent = "" }).ToList();

            if (EditIndicator != "" && EditIndicator != "NoCheckBox")
                dt.Add(new DataTableRender
                {
                    name = string.Empty,
                    data = string.Empty,
                    orderable = false,
                    searchable = false,
                    targets = -1,
                    responsivePriority = 2,
                    width = "10px",
                    render = string.Empty,
                    defaultContent = EditIndicator,
                });


            //if (EditIndicator == "" || EditIndicator == "NoCheckBox")
            //{
            //    //Remove the last item
            //    dt.RemoveAt(dt.Count - 1);
            //}


            var jsonSerialiser = new JavaScriptSerializer();
            string dirtyJSON = jsonSerialiser.Serialize(dt);
            string JS = "";
            JS = dirtyJSON
                .Replace("\\u003c", "<")
                    .Replace("\\u0027", "'")
                    .Replace("\\u003e", ">")
                    .Replace("\"function", "function")
                    .Replace("}\"", "}")
                    .Replace("\\", "")
                    .Replace("\"orderable\":0", "\"orderable\":false")
                    .Replace("\"orderable\":1", "\"orderable\":true")
                    .Replace("\"searchable\":0", "\"searchable\":false")
                    .Replace("\"searchable\":1", "\"searchable\":true") + ";";
            string FieldClass = Type == "Field" ? "field-datatable" : "";

            string selectAll = "";
            if (SelectAll)
            {
                selectAll = "Select All"; //From Resource
            }
            string _PK = "";
            string _FK = "";
            if (PK != null)
            {
                _PK = PK.ToString();
            }
            if (FK != null)
            {
                _FK = "/" + FK.ToString();
            }
            string _Area = "";
            if (!string.IsNullOrEmpty(Area))
            {
                _Area = "/" + Area;
            }

            string CheckboxControls = "<th class='dtProcessing'><label class='checkbox-label'><input name = '" + DataTableName + "SelectAll' id='" + DataTableName + "SelectAll' type='checkbox' class='chkHead' /><b>" + selectAll + "</b></label></th>";

            if (EditIndicator == DataTableEditMode.None || EditIndicator == DataTableEditMode.NoCheckBox)
            {
                CheckboxControls = "";
            }
            string DataTableBody = "<table id = '" + DataTableName + "' data-url = '" + _Area + "/" + FormController + "/" + DataTableName + "/" + _PK + _FK
                                 + "' class='table dataTable" + FieldClass + "'  cellspacing='0' style='width:100%;'>" +
                                   "<thead><tr>" + CheckboxControls;


            resxDbFields LanguageResource = new resxDbFields();

            //From 1 to skip the PK which is on the checkbox and to count -1 to skip the last column which is the indicator > or ...
            if (EditIndicator == "" || EditIndicator == DataTableEditMode.NoCheckBox)
            {
                for (int i = 0; i < dt.Count; i++)
                {
                    DataTableBody += "<th>" + LanguageResource.GetType().GetProperty(dt[i].name).GetValue(LanguageResource, null) + "</th>";
                }
            }
            else
            {
                for (int i = 1; i < dt.Count - 1; i++)
                {
                    DataTableBody += "<th>" + LanguageResource.GetType().GetProperty(dt[i].name).GetValue(LanguageResource, null) + "</th>";
                }
            }

            if (EditIndicator != DataTableEditMode.None && EditIndicator != DataTableEditMode.NoCheckBox)
            {
                DataTableBody += "<th style='width:10px;'></th>";
            }
            DataTableBody += "</tr></thead></table>";


            string InitVars = "var js" + DataTableName + " = { 'TableID': '" + DataTableName + "', 'Columns': Columns, 'OrderBy':'" + OrderBy.ToString() + "', 'Type':'" + Type + "' };";
            string DocumentReady = "$(document).ready(function() { InitFilter(); InitDataTable(js" + DataTableName + "); });";

            string FinalResult = "<script> var Columns = " + JS + InitVars + DocumentReady + " </script>" + DataTableBody;
            return FinalResult;
        }

        public List<Sitemap> GetPageSitemap(Guid PageID)
        {
            var Result = (from a in DbCMS.spSitemap(PageID, LAN)
                          select new Sitemap
                          {
                              URL = a.URL,
                              Description = a.Description
                          }).ToList();

            return Result;
        }

        public void UpdateUserPermissionToken(Guid UserProfileGUID, Guid ApplicationGUID)
        {
            Guid ActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");

            //getting each action index
            List<ActionCore> applicationActions = DbCMS.codeActions.Where(x => x.codeActionsCategories.ApplicationGUID == ApplicationGUID && x.ActionID > 0 )
                .Select(x => new ActionCore
                {
                    ActionGUID = x.ActionGUID,
                    ActionID = x.ActionID,
                }).ToList();

            //getting user actions
            List<Guid> userActions = (from a in DbCMS.userPermissions.Where(x => x.Active)
                                      join b in DbCMS.codeActions.Where(x => x.codeActionsCategories.ApplicationGUID == ApplicationGUID && x.Active) on a.ActionGUID equals b.ActionGUID
                                      where a.UserProfileGUID == UserProfileGUID
                                      select a.ActionGUID).ToList();

            foreach (var item in applicationActions)
            {
                item.Status = userActions.Where(x => x == item.ActionGUID).Count() > 0 ? '1' : '0';
                if (item.Status == '0')
                {

                }
            }

            //get user application token
            userApplicationToken UserApplicationToken = DbCMS.userApplicationToken.Where(s => s.ApplicationGUID == ApplicationGUID && s.UserProfileGUID == UserProfileGUID).FirstOrDefault();

            //update user application token
            char[] Token = null;

            if (UserApplicationToken != null)
            {
                if((UserApplicationToken.Token.Length-1) < applicationActions.Count())
                {
                    int AddToken = applicationActions.Count() - (UserApplicationToken.Token.Length-1);
                    for (int i=0;i<= AddToken;i++)
                    {
                        UserApplicationToken.Token += "0";
                    }
                    
                }
                Token = UserApplicationToken.Token.ToCharArray();
                foreach (var item in applicationActions)
                {
                    try
                    {
                        Token[item.ActionID] = item.Status;
                    }
                    catch
                    {
                      Console.WriteLine(   item.ActionID);
                    }
                }
                UserApplicationToken.Token = new string(Token);
                DbCMS.UpdateNoAudit(UserApplicationToken);
            }
            else
            {
                int actionsCount = applicationActions.Count;
                //int actionsCount = DbCMS.codeActions.AsNoTracking().Where(x => x.codeActionsCategories.ApplicationGUID == ApplicationGUID && x.ActionID > 0).Count();

                Token = ("#" + new string('0', actionsCount)).ToCharArray();
                try {
                foreach (var action in applicationActions)
                {
                    Token[action.ActionID] = action.Status;
                }
                }
                catch { }
                UserApplicationToken = new userApplicationToken();
                UserApplicationToken.ApplicationGUID = ApplicationGUID;
                UserApplicationToken.Token = new string(Token);
                UserApplicationToken.UserProfileGUID = UserProfileGUID;
                DbCMS.CreateNoAudit(UserApplicationToken);
            }
            Dictionary<String, String> UsersTokens = (Dictionary<String, String>)HttpContext.Current.Application["UsersTokens"];

            try
            {
                UsersTokens.Add(UserProfileGUID.ToString() + ApplicationGUID.ToString(), UserApplicationToken.Token);
            }
            catch //Key is exists, just overwrite the token value.
            {
                UsersTokens[UserProfileGUID.ToString() + ApplicationGUID.ToString()] = UserApplicationToken.Token;
            }
            //Push the Dictionary back
            HttpContext.Current.Application["UsersTokens"] = UsersTokens;

        }

        public void UpdateUserPermissionTokenOLD(List<Guid> ActionGUIDs, List<userPermissions> PermissionsToActivate, Guid TargetedApplicationGUID, Guid TargetedUserProfileGUID)
        {
            Guid ActionGUID = Guid.Parse("00000000-0000-0000-0000-000000000001");

            //getting each action index
            List<ActionCore> ActionCore = DbCMS.codeActions.Where(s => ActionGUIDs.Contains(s.ActionGUID))
                .Select(s => new ActionCore
                {
                    ActionGUID = s.ActionGUID,
                    ActionID = s.ActionID,
                }).ToList();

            foreach (var item in ActionCore)
            {
                item.Status = PermissionsToActivate.Where(x => x.ActionGUID == item.ActionGUID).Count() > 0 ? '1' : '0';
            }

            //get user application token
            userApplicationToken UserApplicationToken = DbCMS.userApplicationToken.Where(s => s.ApplicationGUID == TargetedApplicationGUID && s.UserProfileGUID == TargetedUserProfileGUID).FirstOrDefault();

            //update user application token
            char[] Token = null;

            if (UserApplicationToken != null)
            {
                Token = UserApplicationToken.Token.ToCharArray();
                foreach (var item in ActionCore)
                {
                    Token[item.ActionID] = item.Status;
                }
                UserApplicationToken.Token = new string(Token);
                DbCMS.UpdateNoAudit(UserApplicationToken);
            }
            else
            {
                int actionsCount = DbCMS.codeActions.AsNoTracking().Where(x => x.codeActionsCategories.ApplicationGUID == TargetedApplicationGUID && x.ActionID > 0).Count();

                Token = ("#" + new string('0', actionsCount)).ToCharArray();
                foreach (var action in ActionCore)
                {
                    Token[action.ActionID] = action.Status;
                }
                UserApplicationToken = new userApplicationToken();
                UserApplicationToken.ApplicationGUID = TargetedApplicationGUID;
                UserApplicationToken.Token = new string(Token);
                UserApplicationToken.UserProfileGUID = TargetedUserProfileGUID;
                DbCMS.CreateNoAudit(UserApplicationToken);
            }
            Dictionary<String, String> UsersTokens = (Dictionary<String, String>)HttpContext.Current.Application["UsersTokens"];

            try
            {
                UsersTokens.Add(UserProfileGUID.ToString() + TargetedApplicationGUID.ToString(), UserApplicationToken.Token);
            }
            catch //Key is exists, just overwrite the token value.
            {
                UsersTokens[UserProfileGUID.ToString() + TargetedApplicationGUID.ToString()] = UserApplicationToken.Token;
            }
            //Push the Dictionary back
            HttpContext.Current.Application["UsersTokens"] = UsersTokens;
        }

        public bool HasAction(int ActionID, Guid ApplicationGUID, string FactorsToken = null)
        {
            if (HttpContext.Current.Session[SessionKeys.UserProfileGUID] == null)
            {
                throw new HttpException("SessionTimeOut");
            }
            if (FactorsToken == null)
            {
                bool Has = GetUserToken(HttpContext.Current.Session[SessionKeys.UserProfileGUID].ToString() + ApplicationGUID)[ActionID] == '1';
                return Has;
            }
            else
            {
                int Found = (from a in DbCMS.userPermissions.Where(a => a.Active && a.UserProfileGUID == UserProfileGUID && a.FactorsToken == FactorsToken)
                             join b in DbCMS.codeActions.Where(a => a.Active && a.ActionID == ActionID) on a.ActionGUID equals b.ActionGUID
                             select a).Count();
                return Found > 0;
            }
        }


        public void SetUserToken(Guid UserProfileGUID, Guid ApplicationGUID)
        {
            string Token = "";
            var Tkn = DbCMS.userApplicationToken.Where(u => u.UserProfileGUID == UserProfileGUID && u.ApplicationGUID == ApplicationGUID).FirstOrDefault();

            if (Tkn == null)
            {
                int ActionsCount = (from a in DbCMS.codeActions.Where(a => a.Active)
                                    join b in DbCMS.codeActionsCategories on a.ActionCategoryGUID equals b.ActionCategoryGUID
                                    join c in DbCMS.codeApplications.Where(a => a.ApplicationGUID == ApplicationGUID) on b.ApplicationGUID equals c.ApplicationGUID
                                    select a).Count();
                Token = "#" + new String('0', ActionsCount);
            }
            else
            {
                Token = Tkn.Token;
            }

            Dictionary<String, String> UsersTokens = (Dictionary<String, String>)HttpContext.Current.Application["UsersTokens"];

            try
            {
                UsersTokens.Add(UserProfileGUID.ToString() + ApplicationGUID.ToString(), Token);
            }
            catch //Key is exists, just overwrite the token value.
            {
                UsersTokens[UserProfileGUID.ToString() + ApplicationGUID.ToString()] = Token;
            }
            //Push the Dictionary back
            HttpContext.Current.Application["UsersTokens"] = UsersTokens;
        }



        public string GetUserToken(string Key)
        {

            try
            {
                return ((Dictionary<String, String>)HttpContext.Current.Application["UsersTokens"])[Key];
            }
            catch
            {
                //if key not found --> 

                Guid UserProfileGUID = Guid.Parse(Key.Substring(0, 36));
                Guid ApplicationGUID = Guid.Parse(Key.Substring(36, 36));
                SetUserToken(UserProfileGUID, ApplicationGUID);
                return ((Dictionary<String, String>)HttpContext.Current.Application["UsersTokens"])[Key];
            }
        }


        #endregion

        public object[] PermissionsTreeRowMaterials(PermissionsTreeFilter Filter)
        {
            Guid EmptyGUID = Guid.Empty;

            var Applications = DbCMS.codeApplicationsLanguages.Where(l => l.LanguageID == LAN && l.ApplicationGUID == Filter.ApplicationGUID)
                    .Select(a => new PermissionTreeNode
                    {
                        ID = a.ApplicationGUID,
                        Text = a.ApplicationDescription,
                        ParentID = null
                    }).ToList();

            var Categories = (from a in DbCMS.codeActionsCategories.Where(x => x.Active).Where(x => (x.ApplicationGUID == Filter.ApplicationGUID) && (Filter.CategoryGUID == EmptyGUID || x.ActionCategoryGUID == Filter.CategoryGUID))
                              join b in DbCMS.codeActionsCategoriesLanguages.Where(x => x.LanguageID == LAN && x.Active) on a.ActionCategoryGUID equals b.ActionCategoryGUID
                              where a.codeActions.Where(x => x.Active && x.ForAuditPurpose == false).Count() > 0
                              orderby b.ActionCategoryDescription
                              select new PermissionTreeCategoryNode
                              {
                                  ID = a.ActionCategoryGUID,
                                  Text = b.ActionCategoryDescription,
                                  ParentID = a.ApplicationGUID,
                                  FactorsArray = a.codeActionsCategoriesFactors.OrderBy(o => o.FactorTreeLevel).Where(a => a.Active).Select(x => new FactorArray
                                  {
                                      FactorTypeID = x.FactorGUID,
                                      DependsOn = x.DependsOn,
                                      Purpose = x.IsValuePurpose
                                  }).ToList()
                              }).ToList();

            var Actions = (from a in DbCMS.codeActions.Where(a => a.Active && a.ForAuditPurpose == false)
                           join b in DbCMS.codeActionsVerbsLanguages.Where(l => l.LanguageID == LAN && l.Active) on a.ActionVerbGUID equals b.ActionVerbGUID
                           where (a.codeActionsCategories.ApplicationGUID == Filter.ApplicationGUID) &&
                                  (Filter.CategoryGUID == EmptyGUID || a.ActionCategoryGUID == Filter.CategoryGUID) &&
                                  (Filter.ActionGUID == EmptyGUID || a.ActionGUID == Filter.ActionGUID)
                           orderby a.ActionID
                           select new PermissionTreeNode
                           {
                               ID = a.ActionGUID,
                               Text = a.ActionID + " - " + b.ActionVerbDescription,
                               ParentID = a.ActionCategoryGUID
                           }).ToList();

            //List processing

            string strBureaus = null;
            if (Filter.UNHCRBureauGUID != EmptyGUID) strBureaus = string.Join(",", Filter.UNHCRBureauGUID);

            string strOperations = null;
            if (Filter.OperationGUIDs != null) strOperations = string.Join(",", Filter.OperationGUIDs);

            string strOrganizations = null;
            if (Filter.OrganizationGUIDs != null) strOrganizations = string.Join(",", Filter.OrganizationGUIDs);

            string strGovernorates = null;
            if (Filter.GovernorateGUIDs != null) strGovernorates = string.Join(",", Filter.GovernorateGUIDs);

            string strCountries = null;
            if (Filter.CountryGUIDs != null) strCountries = string.Join(",", Filter.CountryGUIDs);

            var Factors = (from FBB in DbCMS.spFactorsBlackBox(LAN, strBureaus, strOperations, strOrganizations, strGovernorates, strCountries, UserProfileGUID) select FBB).ToList();

            //Get User Permissions
            var UserPermissions = (from a in DbCMS.userPermissions.AsNoTracking()
                                   where a.UserProfileGUID == Filter.UserProfileGUID && a.Active
                                   select new
                                   {
                                       a.ActionGUID,
                                       a.FactorsToken,
                                   }).ToList();

            object[] R = new object[5];
            R[0] = Applications;
            R[1] = Categories;
            R[2] = Actions;
            R[3] = Factors;
            R[4] = UserPermissions;

            return R;
        }

        public JobModel GetJob(Guid UserProfileGUID)
        {
            var model = (from a in DbCMS.userProfiles.Where(x => x.Active && x.UserProfileGUID == UserProfileGUID)
                         join b in DbCMS.codeJobTitlesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.JobTitleGUID equals b.JobTitleGUID
                         join c in DbCMS.codeOrganizationsInstancesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.OrganizationInstanceGUID equals c.OrganizationInstanceGUID
                         join d in DbCMS.codeDutyStationsLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.DutyStationGUID equals d.DutyStationGUID into LJ1
                         from l1 in LJ1.DefaultIfEmpty()
                         join e in DbCMS.codeDepartmentsLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.DepartmentGUID equals e.DepartmentGUID into LJ2
                         from l2 in LJ2.DefaultIfEmpty()

                         select new JobModel
                         {
                             UserProfileGUID = a.UserProfileGUID,
                             FromDate = a.FromDate,
                             JobTitle = b.JobTitleDescription,
                             Grade = a.Grade,
                             PositionNumber = a.PositionNumber,
                             OrganizationInstance = c.OrganizationInstanceDescription,
                             DutyStation = l1.DutyStationDescription,
                             Department = l2.DepartmentDescription,
                             StepHistoryList = a.userStepsHistory,
                             ManagerHistoryList = a.userManagersHistory
                         }).FirstOrDefault();

            return model;
        }

        public List<JobModel> GetJobs(Guid ServiceHistoryGUID)
        {
            var model = (from a in DbCMS.userProfiles.Where(x => x.Active && x.ServiceHistoryGUID == ServiceHistoryGUID)
                         join b in DbCMS.codeJobTitlesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.JobTitleGUID equals b.JobTitleGUID
                         join c in DbCMS.codeOrganizationsInstancesLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.OrganizationInstanceGUID equals c.OrganizationInstanceGUID
                         join d in DbCMS.codeDutyStationsLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.DutyStationGUID equals d.DutyStationGUID into LJ1
                         from l1 in LJ1.DefaultIfEmpty()
                         join e in DbCMS.codeDepartmentsLanguages.Where(x => x.Active && x.LanguageID == LAN) on a.DepartmentGUID equals e.DepartmentGUID into LJ2
                         from l2 in LJ2.DefaultIfEmpty()
                         orderby a.FromDate descending
                         select new JobModel
                         {
                             UserProfileGUID = a.UserProfileGUID,
                             FromDate = a.FromDate,
                             JobTitle = b.JobTitleDescription,
                             Grade = a.Grade,
                             PositionNumber = a.PositionNumber,
                             OrganizationInstance = c.OrganizationInstanceDescription,
                             DutyStation = l1.DutyStationDescription,
                             Department = l2.DepartmentDescription,
                             StepHistoryList = a.userStepsHistory,
                             ManagerHistoryList = a.userManagersHistory
                         }).ToList();

            return model;
        }

        public List<Guid> GetAuthorizedList(Guid ActionGUID, Guid FactorGUID)
        {
            int FactorIndex = DbCMS.codeActions.AsNoTracking().Where(x => x.ActionGUID == ActionGUID).FirstOrDefault().codeActionsCategories.codeActionsCategoriesFactors.Where(x => x.FactorGUID == FactorGUID).FirstOrDefault().FactorTreeLevel;

            List<Guid> AuthorizedList = DbCMS.userPermissions.Where(x => x.Active && x.ActionGUID == ActionGUID && x.UserProfileGUID == UserProfileGUID).Select(x => x.FactorsToken).ToList().AsEnumerable().Select(x => Guid.Parse(x.Split(',')[FactorIndex])).ToList();

            return AuthorizedList;
        }

        public class HomePageApplication
        {
            public Guid ApplicationGUID { get; set; }
            public string URL { get; set; }
            public string Description { get; set; }
            public string StatusClass { get; set; }
            public string Status { get; set; }
            public string AppOwnerName { get; set; }
            public string ApplicationCategoryName { get; set; }

        }


        public List<HomePageApplication> GetHomePageApplications(string _LAN = null)
        {
            if (_LAN == null) { _LAN = LAN; }
            string ServerAccessibility = AppSettingsKeys.ServerAccessibility;

            Guid[] ExcludedAppsGuid = { Apps.MRS, Apps.SHM,Apps.OSA, Apps.ORG };
            Guid[] IncludedAppsGuid = { Apps.AHD };
            var Result = (from a in DbCMS.codeApplications.Where(x => x.Active && x.ServerAccessibility.Contains(ServerAccessibility) )
                          join b in DbCMS.codeApplicationsLanguages.Where(x => x.Active && x.LanguageID == _LAN) on a.ApplicationGUID equals b.ApplicationGUID
                          join c in DbCMS.codeActionsCategories.Where(x => x.Active) on a.ApplicationGUID equals c.ApplicationGUID
                          join d in DbCMS.codeActions.Where(x => x.Active) on c.ActionCategoryGUID equals d.ActionCategoryGUID
                          join e in DbCMS.userPermissions.Where(x => x.Active && x.UserProfileGUID == UserProfileGUID) on d.ActionGUID equals e.ActionGUID
                          join f in DbCMS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == _LAN) on a.ApplicationStatusGUID equals f.ValueGUID
                          select new HomePageApplication { ApplicationGUID = a.ApplicationGUID, Description = b.ApplicationDescription, StatusClass = a.ApplicationStatusGUID.ToString(), Status = f.ValueDescription, URL = a.ApplicationAcrynom }).Distinct().ToList();

          
            //add Admin Package 
            if ((Result.Where(x => ExcludedAppsGuid.Contains(x.ApplicationGUID)).Count() > 0) )
            {
                Result = Result.Where(x => !ExcludedAppsGuid.Contains(x.ApplicationGUID)).ToList();
                if (Result.Where(x => IncludedAppsGuid.Contains(x.ApplicationGUID)).Count() == 0)
                {
                    var ResultIncludeAccess = (from a in DbCMS.codeApplications.Where(x => x.Active && x.ServerAccessibility.Contains(ServerAccessibility) && IncludedAppsGuid.Contains(x.ApplicationGUID))
                                               join b in DbCMS.codeApplicationsLanguages.Where(x => x.Active && x.LanguageID == _LAN) on a.ApplicationGUID equals b.ApplicationGUID
                                               join f in DbCMS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == _LAN) on a.ApplicationStatusGUID equals f.ValueGUID
                                               select new HomePageApplication { ApplicationGUID = a.ApplicationGUID, Description = b.ApplicationDescription, StatusClass = a.ApplicationStatusGUID.ToString(), Status = f.ValueDescription, URL = a.ApplicationAcrynom }).Distinct().ToList();
                    Result.AddRange(ResultIncludeAccess);

                }
            }
            var ResultPublicAccess = (from a in DbCMS.codeApplications.Where(x => x.Active && x.ServerAccessibility.Contains(ServerAccessibility) && x.ApplicationGUID == Apps.OSA)
                                      join b in DbCMS.codeApplicationsLanguages.Where(x => x.Active && x.LanguageID == _LAN) on a.ApplicationGUID equals b.ApplicationGUID
                                      join f in DbCMS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == _LAN) on a.ApplicationStatusGUID equals f.ValueGUID
                                      select new HomePageApplication { ApplicationGUID = a.ApplicationGUID, Description = b.ApplicationDescription, StatusClass = a.ApplicationStatusGUID.ToString(), Status = f.ValueDescription, URL = a.ApplicationAcrynom }).Distinct().ToList();
            Result.AddRange(ResultPublicAccess);
            return Result;
        }

        public List<HomePageApplication> GetHomePageOtherApplications(string _LAN = null)
        {
            if (_LAN == null) { _LAN = LAN; }
            string ServerAccessibility = AppSettingsKeys.ServerAccessibility;

            Guid[] ExcludedAppsGuid = { Apps.MRS, Apps.SHM, Apps.OSA, Apps.ORG };
            Guid[] IncludedAppsGuid = { Apps.AHD };
            var _myApp = DbCMS.v_currentUserPermissions.Where(x => x.Active && x.UserProfileGUID == UserProfileGUID
            ).Select(x => x.ApplicationGUID).Distinct().ToList();

            Guid _orgGUID = Guid.Parse("B2CD1671-ECF4-4905-8FFA-F486CBA09D2A");
            var Result = (from a in DbCMS.codeApplications.Where(x => x.Active && x.ApplicationGUID != _orgGUID
                          && !_myApp.Contains(x.ApplicationGUID)
                          && !ExcludedAppsGuid.Contains(x.ApplicationGUID)
                          )
                          join b in DbCMS.codeApplicationsLanguages.Where(x => x.Active && x.LanguageID == _LAN) on a.ApplicationGUID equals b.ApplicationGUID
                          join c in DbCMS.codeActionsCategories.Where(x => x.Active) on a.ApplicationGUID equals c.ApplicationGUID
                          join d in DbCMS.codeActions.Where(x => x.Active) on c.ActionCategoryGUID equals d.ActionCategoryGUID
                          //join e in DbCMS.userPermissions.Where(x => x.Active && x.UserProfileGUID == UserProfileGUID) on d.ActionGUID equals e.ActionGUID
                          join f in DbCMS.codeTablesValuesLanguages.Where(x => x.Active && x.LanguageID == _LAN) on a.ApplicationStatusGUID equals f.ValueGUID
                          join h in DbCMS.userPersonalDetailsLanguage.Where(x => x.Active && x.LanguageID == _LAN) on a.AppOwnerGUID equals h.UserGUID
                          select new HomePageApplication
                          {
                              ApplicationGUID = a.ApplicationGUID,
                              Description = b.ApplicationDescription,
                              ApplicationCategoryName = a.codeApplicationCategory != null ? a.codeApplicationCategory.ApplicationCategoryName : "",
                              AppOwnerName = h.FirstName + " " + h.Surname,


                              StatusClass = a.ApplicationStatusGUID.ToString(),
                              Status = f.ValueDescription,
                              URL = a.ApplicationAcrynom
                          }).OrderBy(x => x.ApplicationCategoryName).Distinct().ToList();

            return Result;
        }

        public static List<string> PortalLanguages = new List<string>(new string[] { "EN", "FR", "AR" });

        public List<T> InitializeLanguagesList<T>(T model) where T : class
        {
            List<T> ListOfT = new List<T>();
            List<string> OrderedLanguages = PortalLanguages.OrderByDescending(x => x = LAN).ThenBy(x => x).ToList();
            foreach (var language in OrderedLanguages)
            {
                T t = (T)Activator.CreateInstance(typeof(T));
                t.GetType().GetProperty("LanguageID").SetValue(t, language);
                ListOfT.Add(t);
            }
            return ListOfT;
        }

        #region Menus Files Handling
        public void BuildUserMenus(Guid userGUID, string LAN)
        {
            List<NavigationMenu> Menus = ApplicationMenusWithLan(Guid.Parse(HttpContext.Current.Session[SessionKeys.CurrentApp].ToString()), LAN);

            StringBuilder sb = new StringBuilder();
            BuildMenuFile(Menus, HttpContext.Current.Server.MapPath("~/UserFiles/" + userGUID), userGUID.ToString() + "Menu.html");

        }


        public void BuildPublicMenus()
        {
            Guid ApplicationGUID = Guid.Empty;
            try
            {
                ApplicationGUID = Guid.Parse(HttpContext.Current.Session[SessionKeys.CurrentApp].ToString());
            }
            catch
            {
                ApplicationGUID = Apps.CMS;
            }

            foreach (var language in CMS.PortalLanguages)
            {
                List<NavigationMenu> Menus = PublicMenus(ApplicationGUID, language);
                BuildMenuFile(Menus, HttpContext.Current.Server.MapPath("~/UserFiles/" + Guid.Empty), Guid.Empty.ToString() + language + "Menu.html");
            }

        }


        private void BuildMenuFile(List<NavigationMenu> Menus, string Path, string FileName)
        {
            StringBuilder sb = new StringBuilder();
            List<string> NoPageExistsMenus = new List<string>();
            NoPageExistsMenus.Add("/manual/");//Development Team
            NoPageExistsMenus.Add("/portal/");//Applications Portal
            NoPageExistsMenus.Add("/team/");//Technical Information
            NoPageExistsMenus.Add("/technical/");//User Manual
            //NoPageExistsMenus.Add("/reportabug/");//Report a bug
            NoPageExistsMenus.Add("/requestenhancement/");//Enhancement Request
            NoPageExistsMenus.Add("/requestnewfeature/");//New Feature Request 
            NoPageExistsMenus.Add("/rateapplication/");//Rate Application

            foreach (var Menu in Menus)
            {
                sb.Append("<li>");
                sb.Append("<a href=\"" + Menu.RootMenuUrl + "\">" + Menu.RootMenuDescription + "</a>");
                sb.Append("<ul class=\"sub--menu\" style=\"height:auto;\">");
                foreach (var SubMenu in Menu.SubMenu)
                {
                    if (NoPageExistsMenus.Contains(SubMenu.SubMenuUrl))
                    {
                        //<a style="color:#ae030b;" class='Modal-Link' href='#' data-url='/SRS/HelpDesk/GlobalBugReportCreate/'><i style="color:#ae030b;" class="fa fa-bug  fa-1x" aria-hidden="true"></i> Report a bug </a>

                        sb.Append("<li><a class='NotExistPageLink' href=\"" + SubMenu.SubMenuUrl + "\">" + SubMenu.SubMenuDescription + "</a></li>");
                    }
                    else
                    {
                        if (SubMenu.SubMenuUrl.EndsWith("reportabug/"))
                        {
                            sb.Append("<li><a class='Modal-Link'  data-url='/SRS/HelpDesk/GlobalBugReportCreate/' href='#'>" + SubMenu.SubMenuDescription + "</a></li>");

                        }
                        else
                        {
                            sb.Append("<li><a  class='store-app-access' href=\"" + SubMenu.SubMenuUrl + "\">" + SubMenu.SubMenuDescription + "</a></li>");

                        }
                    }

                }
                sb.Append("</ul>");
                sb.Append("</li>");
            }

            if (!Directory.Exists(Path))
            {
                Directory.CreateDirectory(Path);
            }

            string filePath = System.IO.Path.Combine(Path, FileName);

            if (!File.Exists(Path))
            {
                using (
                    FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    using (StreamWriter w = new StreamWriter(fs, Encoding.UTF8))
                    {
                        w.WriteLine(sb);
                    }
                }
            }
        }
        #endregion

    }
}