using AppsPortal.BaseControllers;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.Models;
using AppsPortal.ViewModels;
using AutoMapper;
using LinqKit;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Controllers
{
    public class UsersController : PortalBaseController
    {

        [Route("Users/")]
        public ActionResult UsersIndex()
        {
            if (!CMS.HasAction(Permissions.UserAccountsManagement.Access, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            return View("~/Views/Users/Index.cshtml");
        }

        [Route("Users/UsersDataTable/")]
        public JsonResult UsersDataTable(DataTableRecievedOptions options)
        {

            DataTableOptions DataTable = ConvertOptions.Fill(options);

            Expression<Func<UsersDataTableModel, bool>> Predicate = p => true;

            if (DataTable.Filters.FilterRules != null)
            {
                Predicate = SearchHelper.CreateSearchPredicate<UsersDataTableModel>(DataTable.Filters);
            }

            var All = (from a in DbCMS.userAccounts.AsExpandable()
                       join b in DbCMS.userPersonalDetails.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.userAccounts.DeletedOn)) on a.UserGUID equals b.UserGUID into LJ1
                       from R1 in LJ1.DefaultIfEmpty()
                       join c in DbCMS.userPersonalDetailsLanguage.Where(x => (x.Active == true ? x.Active : x.DeletedOn == x.userPersonalDetails.DeletedOn) && x.LanguageID == LAN && x.Active) on a.UserGUID equals c.UserGUID into LJ2
                       from R2 in LJ2.DefaultIfEmpty()
                       join d in DbCMS.userServiceHistory on a.UserGUID equals d.UserGUID into LJ3
                       from R3 in LJ3.DefaultIfEmpty()
                       join e in DbCMS.userProfiles on R3.ServiceHistoryGUID equals e.ServiceHistoryGUID into LJ4
                       from R4 in LJ4.DefaultIfEmpty()
                       join f in DbCMS.codeDutyStationsLanguages.Where(x => x.Active && x.LanguageID == LAN) on R4.DutyStationGUID equals f.DutyStationGUID into LJ5
                       from R5 in LJ5.DefaultIfEmpty()
                       join j in DbCMS.codeDepartmentsLanguages.Where(x => x.Active && x.LanguageID == LAN) on R4.DepartmentGUID equals j.DepartmentGUID into LJ6
                       from R6 in LJ6.DefaultIfEmpty()
                       join h in DbCMS.codeJobTitlesLanguages.Where(x => x.Active && x.LanguageID == LAN) on R4.JobTitleGUID equals h.JobTitleGUID into LJ7
                       from R7 in LJ7.DefaultIfEmpty()
                       join j in DbCMS.codeOrganizationsInstancesLanguages.Where(x => x.Active && x.LanguageID == LAN) on R4.OrganizationInstanceGUID equals j.OrganizationInstanceGUID into LJ8
                       from R8 in LJ8.DefaultIfEmpty()
                       where R4.FromDate == (from z in DbCMS.userProfiles where z.userServiceHistory.UserGUID == a.UserGUID select z.FromDate).Max()
                       select new UsersDataTableModel
                       {
                           UserGUID = a.UserGUID,
                           FullName =  R2.FirstName + " " + R2.Surname,
                           EmailAddress=R3.EmailAddress,
                           CountryGUID = R1.CountryGUID.Value,
                           FromDate = R4.FromDate,
                           ToDate = R4.ToDate,
                           OrganizationInstanceGUID = R4.OrganizationInstanceGUID.ToString(),
                           OrganizationInstanceDescription= R8.OrganizationInstanceDescription,
                           DutyStationGUID = R5.DutyStationGUID.ToString(),
                           DutyStationDescription = R5.DutyStationDescription,
                           DepartmentDescription = R6.DepartmentDescription,
                           JobTitleDescription = R7.JobTitleDescription,
                           Grade = R4.Grade,
                           AccountExpiredOn=a.AccountExpiredOn,
                           LastLogIn=a.LastLogIn,
                           Active = a.Active,
                           userAccountsRowVersion = a.userAccountsRowVersion
                       }).Where(x=> x.FullName != "").Where(Predicate);

            All = SearchHelper.OrderByDynamic(All, DataTable.Order.OrderBy, DataTable.Order.OrderDirection);

            List<UsersDataTableModel> Result = Mapper.Map<List<UsersDataTableModel>>(All.Skip(DataTable.Start).Take(DataTable.Length).ToList());

            return Json(Portal.DataTable(All.Count(), Result), JsonRequestBehavior.AllowGet);
        }

        [Route("Users/Create/")]
        public ActionResult UserCreate()
        {
            return View("~/Views/Users/User.cshtml", new UsersUpdateModel() { OfficeLandlineCountryCode = "+963" });
        }

        [HttpPost, ValidateAntiForgeryToken()]
        public ActionResult UserCreate(UsersUpdateModel model)
        {
            bool userFound = (from a in DbCMS.userServiceHistory where a.EmailAddress == model.EmailAddress && a.Active select a).Count() > 0;
            if (userFound)
            {
                ModelState.AddModelError("", "User is already exists");
                return PartialView("~/Views/Users/_UserForm.cshtml", model);
            }
            Guid NewUserGUID = Guid.NewGuid();
            userAccounts userAccounts = new userAccounts();
            userPersonalDetails userPersonalDetails = new userPersonalDetails();
            userPersonalDetailsLanguage userPersonalDetailsLanguage = new userPersonalDetailsLanguage();
            userServiceHistory userServiceHistory = new userServiceHistory();
            userProfiles userProfiles = new userProfiles();
            userPasswords userPasswords = new userPasswords();
            userContactDetails userContactDetails = new userContactDetails();

            userAccounts.UserGUID = NewUserGUID;
            userAccounts.IsFirstLogin = true;
            userAccounts.Active = true;
            userAccounts.RequestedOn = DateTime.Now;
            userAccounts.AccountExpiredOn = model.AccountExpiredOn;

            userAccounts.TimeZone = "Syria Standard Time";
            userAccounts.AccountStatusID = 10;
            userAccounts.SecurityQuestionGUID = Guid.Parse("EE813F52-8768-4E4A-8E07-42F43093859E"); //Who is your favorite artist?
            userAccounts.SecurityAnswer = "Automated security answer! Change it please";
            userAccounts.Active = true;


            userPersonalDetails.UserGUID = NewUserGUID;
            userPersonalDetails.PreferedLanguageID = "EN";

            userPersonalDetailsLanguage.PersonalDetailsLanguageGUID = Guid.NewGuid();
            userPersonalDetailsLanguage.UserGUID = NewUserGUID;
            userPersonalDetailsLanguage.LanguageID = "EN";
            userPersonalDetailsLanguage.FirstName = model.GivenName;
            userPersonalDetailsLanguage.Surname = model.SurName;
            userPersonalDetailsLanguage.Active = true;

            Guid NewServiceHistoryGUiD = Guid.NewGuid();
            userServiceHistory.ServiceHistoryGUID = NewServiceHistoryGUiD;
            userServiceHistory.UserGUID = NewUserGUID;
            Guid organizationGuid = (from a in DbCMS.codeOrganizationsInstances where a.OrganizationInstanceGUID == model.OrganizationInstanceGUID select a.OrganizationGUID).FirstOrDefault();
            userServiceHistory.OrganizationGUID = organizationGuid;
            userServiceHistory.EmailAddress = model.EmailAddress;
            userServiceHistory.Active = true;

            userProfiles.UserProfileGUID = Guid.NewGuid();
            userProfiles.ServiceHistoryGUID = NewServiceHistoryGUiD;
            userProfiles.OrganizationInstanceGUID = model.OrganizationInstanceGUID.Value;
            userProfiles.DutyStationGUID = model.DutyStationGUID;
            userProfiles.DepartmentGUID = model.DepartmentGUID;
            userProfiles.JobTitleGUID = model.JobTitleGUID;
            userProfiles.FromDate = new DateTime(1900, 1, 1);
            userProfiles.Active = true;

            userPasswords.PasswordGUID = Guid.NewGuid();
            userPasswords.UserGUID = NewUserGUID;
            userPasswords.ActivationDate = DateTime.Now;
            userPasswords.Active = true;
            userPasswords.Password = HashingHelper.EncryptPassword("P@ssw0rd123", Portal.GUIDToString(NewUserGUID));

            userContactDetails.ContactDetailsGUID = Guid.NewGuid();
            userContactDetails.UserGUID = NewUserGUID;
            userContactDetails.OfficeLandlineCountryCode = model.OfficeLandlineCountryCode;
            userContactDetails.OfficeLandlineAreaCode = model.OfficeLandlineAreaCode;
            userContactDetails.OfficeLandlineNumber = model.OfficeLandlineNumber;
            userContactDetails.Active = true;

            DbCMS.userAccounts.Add(userAccounts);
            DbCMS.userPersonalDetails.Add(userPersonalDetails);
            DbCMS.userPersonalDetailsLanguage.Add(userPersonalDetailsLanguage);
            DbCMS.userServiceHistory.Add(userServiceHistory);
            DbCMS.userProfiles.Add(userProfiles);
            DbCMS.userPasswords.Add(userPasswords);
            DbCMS.userContactDetails.Add(userContactDetails);

            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleCreateMessage());
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }

        }

        [Route("Users/Update/{PK}")]
        public ActionResult UserUpdate(Guid PK)
        {
            UsersUpdateModel model = (from y in DbCMS.userAccounts.Where(x => x.Active && x.UserGUID == PK)
                                      join a in DbCMS.userPersonalDetailsLanguage.Where(x => x.LanguageID == LAN && x.Active) on y.UserGUID equals a.UserGUID into LJ5
                                      from R5 in LJ5.DefaultIfEmpty()
                                      join b in DbCMS.userPersonalDetails.Where(x => x.Active) on R5.UserGUID equals b.UserGUID into LJ1
                                      from R1 in LJ1.DefaultIfEmpty()
                                      join c in DbCMS.userServiceHistory.Where(x => x.Active) on R5.UserGUID equals c.UserGUID into LJ2
                                      from R2 in LJ2.DefaultIfEmpty()
                                      join d in DbCMS.userProfiles.Where(x => x.Active && x.UserProfileGUID == UserProfileGUID) on R2.ServiceHistoryGUID equals d.ServiceHistoryGUID into LJ3
                                      from R3 in LJ3.DefaultIfEmpty()
                                      join e in DbCMS.userContactDetails.Where(x => x.Active == true) on R5.UserGUID equals e.UserGUID into LJ4
                                      from R4 in LJ4.DefaultIfEmpty()
                                      orderby R3.FromDate descending
                                      select new
                                      UsersUpdateModel
                                      {
                                          UserGUID = y.UserGUID,
                                          UserID = "",
                                          EmailAddress = R2.EmailAddress,
                                          GivenName = R5.FirstName,
                                          SurName = R5.Surname,
                                          ExtensionNumber = "",
                                          OrganizationInstanceGUID = R3.OrganizationInstanceGUID,
                                          DutyStationGUID = R3.DutyStationGUID.Value,
                                          DepartmentGUID = R3.DepartmentGUID.Value,
                                          JobTitleGUID = R3.JobTitleGUID.Value,
                                          HiddenPhoneNumber = "",
                                          OfficeLandlineCountryCode = R4.OfficeLandlineCountryCode,
                                          OfficeLandlineAreaCode = R4.OfficeLandlineAreaCode,
                                          OfficeLandlineNumber = R4.OfficeLandlineNumber,
                                          AccountExpiredOn=y.AccountExpiredOn,
                                          LastLogIn=y.LastLogIn,
                                          PasswordLastUpdate=y.PasswordLastUpdate,
                                          Active = R5.Active
                                      }).FirstOrDefault();
            model.OfficeLandlineCountryCode = "+963";
            model.OfficeLandlineAreaCode = "11";
            return View("~/Views/Users/User.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken()]
        public ActionResult UserUpdate(UsersUpdateModel model)
        {

            userProfiles userProfile = (from a in DbCMS.userAccounts.Where(x => x.UserGUID == model.UserGUID && x.Active)
                                        join b in DbCMS.userServiceHistory.Where(x => x.Active) on a.UserGUID equals b.UserGUID
                                        join e in DbCMS.userProfiles.Where(x => x.Active) on b.ServiceHistoryGUID equals e.ServiceHistoryGUID
                                        orderby e.FromDate descending
                                        select e
                        ).FirstOrDefault();
            userProfile.DutyStationGUID = model.DutyStationGUID;
            userProfile.DepartmentGUID = model.DepartmentGUID;
            userProfile.JobTitleGUID = model.JobTitleGUID;

            userAccounts account = DbCMS.userAccounts.Where(x => x.UserGUID == model.UserGUID && x.Active).FirstOrDefault();
            account.AccountExpiredOn = model.AccountExpiredOn;
            account.PasswordLastUpdate = model.PasswordLastUpdate;

            userServiceHistory serviceHistory = DbCMS.userServiceHistory.Where(x => x.UserGUID == model.UserGUID && x.Active).FirstOrDefault();
            serviceHistory.EmailAddress = model.EmailAddress;



            userContactDetails userContactDetail = (from a in DbCMS.userContactDetails where a.UserGUID == model.UserGUID && a.Active == true select a).FirstOrDefault();
            if (userContactDetail == null)
            {
                userContactDetail = new userContactDetails();
                Guid userGUID = model.UserGUID.Value;
                userContactDetail.ContactDetailsGUID = Guid.NewGuid();
                userContactDetail.UserGUID = userGUID;
                userContactDetail.OfficeLandlineCountryCode = model.OfficeLandlineCountryCode;
                userContactDetail.OfficeLandlineAreaCode = model.OfficeLandlineAreaCode;
                userContactDetail.OfficeLandlineNumber = model.OfficeLandlineNumber;
                userContactDetail.Active = true;
                DbCMS.userContactDetails.Add(userContactDetail);
            }
            else
            {
                userContactDetail.OfficeLandlineCountryCode = model.OfficeLandlineCountryCode;
                userContactDetail.OfficeLandlineAreaCode = model.OfficeLandlineAreaCode;
                userContactDetail.OfficeLandlineNumber = model.OfficeLandlineNumber;
            }


            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleCreateMessage());
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

        public JsonResult ActiveDirectoryLookup(string UserID)
        {

            DirectoryEntry ldapConnection = new DirectoryEntry("LDAP://unhcr.local/OU=Organisation,DC=UNHCR,DC=LOCAL", "a-syrdascan", "Unhcr@1234", AuthenticationTypes.Secure);
            DirectorySearcher search = new DirectorySearcher(ldapConnection);
            search.Filter = "(cn=" + UserID + ")";
            search.PropertiesToLoad.Add("sn");
            search.PropertiesToLoad.Add("samaccountname");
            search.PropertiesToLoad.Add("mail");
            search.PropertiesToLoad.Add("displayname");
            search.PropertiesToLoad.Add("telephoneNumber");
            search.PropertiesToLoad.Add("title");
            search.PropertiesToLoad.Add("department");
            search.PropertiesToLoad.Add("givenName");
            search.PropertiesToLoad.Add("co");
            search.PropertiesToLoad.Add("l");
            search.PropertiesToLoad.Add("company");
            search.PropertiesToLoad.Add("memberOf");

            SearchResult result = search.FindOne();
            //searcher.PropertiesToLoad.Add("sn");

            //var result = searcher.FindOne();

            if (result == null)
            {
                return Json(new { Error = "No Data" });
            }
            Guid OrganizationInstanceGUID = Guid.Empty;
            Guid UNHCRguid = Guid.Parse("F9BD9237-CA5B-4753-A0EB-B1E4C33B1490");

            ActiveDirectoryUser adu = new ActiveDirectoryUser();
            try { adu.sn = result.Properties["sn"][0].ToString(); } catch { }
            try { adu.samaccountname = result.Properties["samaccountname"][0].ToString(); } catch { }
            try { adu.mail = result.Properties["mail"][0].ToString(); } catch { }
            try { adu.displayname = result.Properties["displayname"][0].ToString(); } catch { }
            try { adu.telephoneNumber = result.Properties["telephoneNumber"][0].ToString(); } catch { }
            try { adu.title = result.Properties["title"][0].ToString(); } catch { }
            try { adu.department = result.Properties["department"][0].ToString(); } catch { }
            try { adu.givenName = result.Properties["givenName"][0].ToString(); } catch { }
            try { adu.co = result.Properties["co"][0].ToString(); } catch { }
            try { adu.l = result.Properties["l"][0].ToString(); } catch { }

            try
            {
                ResultPropertyValueCollection memberOfResult = result.Properties["memberof"];
                string abc = memberOfResult[memberOfResult.Count - 1].ToString(); //CN=SYRDA All Users,OU=Security Groups,OU=Global,OU=Organisation,DC=UNHCR,DC=LOCAL
                adu.container = abc.Substring(3, 5);

                try { adu.countryCode = adu.container.Substring(0, 3); } catch { }

                try { adu.areaCode = adu.container.Substring(3, 2); } catch { }

                try
                {
                    OrganizationInstanceGUID = (from a in DbCMS.codeCountries
                                                where a.CountryA3Code == adu.countryCode && a.Active
                                                join b in DbCMS.codeOperations on a.CountryGUID equals b.CountryGUID into LJ1
                                                from R1 in LJ1.DefaultIfEmpty()
                                                join c in DbCMS.codeOrganizationsInstances on R1.OperationGUID equals c.OperationGUID into LJ2
                                                from R2 in LJ2.DefaultIfEmpty()
                                                where R2.OrganizationGUID == UNHCRguid && R1.Active && R2.Active
                                                select R2.OrganizationInstanceGUID).FirstOrDefault();
                    adu.organizationInstanceGUID = OrganizationInstanceGUID.ToString();

                }
                catch { }

            }
            catch { }






            //country
            //operation
            //organization operation
            return Json(new
            {
                ActiveDirectoryUser = adu
            }, JsonRequestBehavior.AllowGet);

        }

        //AD groups lookup
        //


        public ActionResult SeedUsersOther()
        {
            string country = "SYR";
            string dutystation = "DA";
            string path = "LDAP://10.240.224.203:389/OU=Organisation,DC=UNHCR,DC=LOCAL";
            SeedUsersToDatabase(path);
            return null;
        }

        public ActionResult SeedUsersDamascus()
        {
            string country = "SYR";
            string dutystation = "DA";
            string path = "LDAP://10.240.224.203:389/OU=Users,OU=Users And Groups,OU=" + dutystation + ",OU=" + country + ",OU=Country,OU=Organisation,DC=UNHCR,DC=LOCAL";
            SeedUsersToDatabase(path);
            return null;
        }

        public ActionResult SeedUsersTartous()
        {
            string country = "SYR";
            string dutystation = "TA";
            string path = "LDAP://10.240.224.203:389/OU=Users,OU=Users And Groups,OU=" + dutystation + ",OU=" + country + ",OU=Country,OU=Organisation,DC=UNHCR,DC=LOCAL";
            SeedUsersToDatabase(path);
            return null;
        }

        public ActionResult SeedUsersHoms()
        {
            string country = "SYR";
            string dutystation = "HO";
            string path = "LDAP://10.240.224.203:389/OU=Users,OU=Users And Groups,OU=" + dutystation + ",OU=" + country + ",OU=Country,OU=Organisation,DC=UNHCR,DC=LOCAL";
            SeedUsersToDatabase(path);
            return null;
        }


        //public ActionResult SeedUsersAleppo()
        //{
        //    string country = "SYR";
        //    string dutystation = "AL";
        //    string path = "LDAP://10.240.224.203:389/OU=Users,OU=Users And Groups,OU=" + dutystation + ",OU=" + country + ",OU=Country,OU=Organisation,DC=UNHCR,DC=LOCAL";
        //    SeedUsersToDatabase(path);
        //    return null;
        //}

        //public ActionResult SeedUsersSweida()
        //{
        //    string country = "SYR";
        //    string dutystation = "SW";
        //    string path = "LDAP://10.240.224.203:389/OU=Users,OU=Users And Groups,OU=" + dutystation + ",OU=" + country + ",OU=Country,OU=Organisation,DC=UNHCR,DC=LOCAL";
        //    SeedUsersToDatabase(path);
        //    return null;
        //}

        //public ActionResult SeedUsersQamishli()
        //{
        //    string country = "SYR";
        //    string dutystation = "QA";
        //    string path = "LDAP://10.240.224.203:389/OU=Users,OU=Users And Groups,OU=" + dutystation + ",OU=" + country + ",OU=Country,OU=Organisation,DC=UNHCR,DC=LOCAL";
        //    SeedUsersToDatabase(path);
        //    return null;
        //}



        public void SeedUsersToDatabase(string path)
        {
            DirectoryEntry ldapConnection = new DirectoryEntry(path, "a-syrdascan", "Unhcr@1234", AuthenticationTypes.Secure);
            DirectorySearcher searcher = new DirectorySearcher(ldapConnection);
            searcher.SearchScope = SearchScope.OneLevel;
            searcher.PropertiesToLoad.Add("sn");
            searcher.PropertiesToLoad.Add("samaccountname");
            searcher.PropertiesToLoad.Add("mail");
            searcher.PropertiesToLoad.Add("displayname");
            searcher.PropertiesToLoad.Add("telephoneNumber");
            searcher.PropertiesToLoad.Add("title");
            searcher.PropertiesToLoad.Add("department");
            searcher.PropertiesToLoad.Add("givenName");
            searcher.PropertiesToLoad.Add("co");
            searcher.PropertiesToLoad.Add("l");
            searcher.PropertiesToLoad.Add("company");
            searcher.PropertiesToLoad.Add("memberOf");

            List<userAccounts> userAccountsList = new List<userAccounts>();
            List<userPersonalDetails> userPersonalDetailsList = new List<userPersonalDetails>();
            List<userPersonalDetailsLanguage> userPersonalDetailsLanguageList = new List<userPersonalDetailsLanguage>();
            List<userServiceHistory> userServiceHistoryList = new List<userServiceHistory>();
            List<userProfiles> userProfilesList = new List<userProfiles>();
            List<userPasswords> userPasswordsList = new List<userPasswords>();
            List<userContactDetails> userContactDetailsList = new List<userContactDetails>();
            List<StaffCoreData> staffCoreDataList = new List<StaffCoreData>();

            SearchResultCollection results = searcher.FindAll();

            foreach (SearchResult item in results)
            {

                Guid OrganizationInstanceGUID = Guid.Parse("E156C022-EC72-4A5A-BE09-163BD85C68EF");
                Guid UNHCRguid = Guid.Parse("F9BD9237-CA5B-4753-A0EB-B1E4C33B1490");
                Guid NewUserGUID = Guid.NewGuid();
                userAccounts userAccounts = new userAccounts();
                userPersonalDetails userPersonalDetails = new userPersonalDetails();
                userPersonalDetailsLanguage userPersonalDetailsLanguage = new userPersonalDetailsLanguage();
                userServiceHistory userServiceHistory = new userServiceHistory();
                userProfiles userProfiles = new userProfiles();
                userPasswords userPasswords = new userPasswords();
                userContactDetails userContactDetails = new userContactDetails();
                StaffCoreData staffCoreData = new StaffCoreData();


                ActiveDirectoryUser adu = new ActiveDirectoryUser();
                try { adu.sn = item.Properties["sn"][0].ToString(); } catch { }
                try { adu.samaccountname = item.Properties["samaccountname"][0].ToString(); } catch { }
                try
                {
                    adu.mail = item.Properties["mail"][0].ToString();
                    if (adu.mail.Length == 0 || adu.mail == null)
                    {
                        continue;
                    }
                }
                catch { continue; }
                bool userFound = (from a in DbCMS.userServiceHistory where a.EmailAddress.Trim().ToLower() == adu.mail.Trim().ToLower() select a).Count() > 0;
                if (userFound) { continue; }
                try { adu.displayname = item.Properties["displayname"][0].ToString(); } catch { }
                try { adu.telephoneNumber = item.Properties["telephoneNumber"][0].ToString(); } catch { }
                try { adu.title = item.Properties["title"][0].ToString(); } catch { }
                try { adu.department = item.Properties["department"][0].ToString(); } catch { }
                try { adu.givenName = item.Properties["givenName"][0].ToString(); } catch { }
                try { adu.co = item.Properties["co"][0].ToString(); } catch { }
                try { adu.l = item.Properties["l"][0].ToString(); } catch { }
                adu.organizationInstanceGUID = "E156C022-EC72-4A5A-BE09-163BD85C68EF";

                userAccounts.UserGUID = NewUserGUID;
                userAccounts.IsFirstLogin = true;
                userAccounts.Active = true;
                userAccounts.RequestedOn = DateTime.Now;

                userAccounts.TimeZone = "Syria Standard Time";
                userAccounts.AccountStatusID = 10;
                userAccounts.SecurityQuestionGUID = Guid.Parse("EE813F52-8768-4E4A-8E07-42F43093859E"); //Who is your favorite artist?
                userAccounts.SecurityAnswer = "Automated security answer! Change it please";
                userAccounts.Active = true;

                userPersonalDetails.UserGUID = NewUserGUID;
                userPersonalDetails.PreferedLanguageID = "EN";
                userPersonalDetails.Active = true;

                userPersonalDetailsLanguage.PersonalDetailsLanguageGUID = Guid.NewGuid();
                userPersonalDetailsLanguage.UserGUID = NewUserGUID;
                userPersonalDetailsLanguage.LanguageID = "EN";
                userPersonalDetailsLanguage.FirstName = item.Properties["givenName"][0].ToString();
                userPersonalDetailsLanguage.Surname = item.Properties["displayname"][0].ToString().Substring(item.Properties["givenName"][0].ToString().Length).ToString();
                userPersonalDetailsLanguage.Active = true;
                Guid NewServiceHistoryGUiD = Guid.NewGuid();
                userServiceHistory.ServiceHistoryGUID = NewServiceHistoryGUiD;
                userServiceHistory.UserGUID = NewUserGUID;
                //Guid organizationGuid = (from a in DbCMS.codeOrganizationsInstances where a.OrganizationInstanceGUID == OrganizationInstanceGUID select a.OrganizationGUID).FirstOrDefault();
                userServiceHistory.OrganizationGUID = UNHCRguid;
                userServiceHistory.EmailAddress = adu.mail;
                userServiceHistory.Active = true;

                userProfiles.UserProfileGUID = Guid.NewGuid();
                userProfiles.ServiceHistoryGUID = NewServiceHistoryGUiD;
                userProfiles.OrganizationInstanceGUID = Guid.Parse("E156C022-EC72-4A5A-BE09-163BD85C68EF");
                userProfiles.DutyStationGUID = Guid.Parse("FD4A8AE4-EB97-46B5-AED1-45B6ADD86D18"); //Other
                userProfiles.DepartmentGUID = Guid.Parse("90E12C9C-C113-49F5-ADF1-BD0140772E2B"); //Other
                userProfiles.JobTitleGUID = Guid.Parse("6FCE11E5-43F6-4A6D-B692-208D4F8B1740"); //Other
                userProfiles.FromDate = new DateTime(1900, 1, 1);
                userProfiles.Active = true;

                userPasswords.PasswordGUID = Guid.NewGuid();
                userPasswords.UserGUID = NewUserGUID;
                userPasswords.ActivationDate = DateTime.Now;
                userPasswords.Active = true;
                userPasswords.Password = HashingHelper.EncryptPassword("P@ssw0rd123", Portal.GUIDToString(NewUserGUID));


                staffCoreData.UserGUID = NewUserGUID;
                staffCoreData.EmailAddress = adu.mail;
                //staffCoreData.AssignmentType = Guid.Empty;
                staffCoreData.JobTitleGUID = Guid.Parse("6FCE11E5-43F6-4A6D-B692-208D4F8B1740");
                staffCoreData.DutyStationGUID = Guid.Parse("FD4A8AE4-EB97-46B5-AED1-45B6ADD86D18");
                staffCoreData.DepartmentGUID = Guid.Parse("90E12C9C-C113-49F5-ADF1-BD0140772E2B");
                staffCoreData.Active = true;
                //try
                //{
                //    string[] fullNumber = adu.telephoneNumber.Split(' ');
                //    userContactDetails.OfficeLandlineCountryCode = fullNumber[0];
                //    userContactDetails.OfficeLandlineAreaCode = fullNumber[1];
                //    userContactDetails.OfficeLandlineNumber = fullNumber[2];
                //    userContactDetails.Active = true;
                //    userContactDetails.ContactDetailsGUID = Guid.NewGuid();
                //    userContactDetails.UserGUID = NewUserGUID;
                //}
                //catch
                //{
                //    userContactDetails.UserGUID = Guid.Empty;
                //}
                userAccountsList.Add(userAccounts);
                staffCoreDataList.Add(staffCoreData);
                userPersonalDetailsList.Add(userPersonalDetails);
                userPersonalDetailsLanguageList.Add(userPersonalDetailsLanguage);
                userServiceHistoryList.Add(userServiceHistory);
                userProfilesList.Add(userProfiles);
                userPasswordsList.Add(userPasswords);
                //if (userContactDetails.UserGUID != Guid.Empty)
                //{
                //    DbCMS.userContactDetails.Add(userContactDetails);
                //}
            }



            try
            {
                DbCMS.userAccounts.AddRange(userAccountsList);
                DbCMS.userPersonalDetails.AddRange(userPersonalDetailsList);
                DbCMS.userPersonalDetailsLanguage.AddRange(userPersonalDetailsLanguageList);
                DbCMS.userServiceHistory.AddRange(userServiceHistoryList);
                DbCMS.userProfiles.AddRange(userProfilesList);
                DbCMS.userPasswords.AddRange(userPasswordsList);
                DbCMS.StaffCoreData.AddRange(staffCoreDataList);
                DbCMS.SaveChanges();
            }
            catch (Exception ex)
            {

            }

        }

        public ActionResult RemoveAllUserPermissions(Guid PK)
        {
            if (!CMS.HasAction(Permissions.UserAccountsManagement.Update, Apps.CMS))
            {
                throw new HttpException(401, "Unauthorized access");
            }
            userProfiles userProfile = (from a in DbCMS.userAccounts.Where(x => x.UserGUID == PK && x.Active)
                                        join b in DbCMS.userServiceHistory.Where(x => x.Active) on a.UserGUID equals b.UserGUID
                                        join e in DbCMS.userProfiles.Where(x => x.Active) on b.ServiceHistoryGUID equals e.ServiceHistoryGUID
                                        orderby e.FromDate descending
                                        select e
                       ).FirstOrDefault();
            var userPermissions = DbCMS.userPermissions.Where(x => x.UserProfileGUID== userProfile.UserProfileGUID).ToList();
            DbCMS.userPermissions.RemoveRange(userPermissions);
            try
            {
                DbCMS.SaveChanges();
                return Json(DbCMS.SingleCreateMessage());
            }
            catch (Exception ex)
            {
                return Json(DbCMS.ErrorMessage(ex.Message));
            }
        }

    }
}