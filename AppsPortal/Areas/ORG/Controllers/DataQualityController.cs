using AppsPortal.BaseControllers;
using AppsPortal.ViewModels;
using ORG_DAL.Model;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.ORG.Controllers
{
    public class DataQualityController : SASBaseController
    {

        public ActionResult Index()
        {
            List<StaffDataQualityModel> model = new List<StaffDataQualityModel>();


            List<LocalDBUserModel> localUsers = (from a in DbSAS.StaffCoreData
                              join b in DbSAS.codeDutyStationsLanguages.Where(x => x.LanguageID == "EN") on a.DutyStationGUID equals b.DutyStationGUID into LJ1
                              from RJ1 in LJ1.DefaultIfEmpty()
                              join bb in DbSAS.codeDutyStationsLanguages.Where(x => x.LanguageID == "AR") on a.DutyStationGUID equals bb.DutyStationGUID into LJb
                              from Rb in LJb.DefaultIfEmpty()

                              join c in DbSAS.codeDepartmentsLanguages.Where(x => x.LanguageID == "EN") on a.DepartmentGUID equals c.DepartmentGUID into LJ2
                              from RJ2 in LJ2.DefaultIfEmpty()

                              join cc in DbSAS.codeDepartmentsLanguages.Where(x => x.LanguageID == "AR") on a.DepartmentGUID equals cc.DepartmentGUID into LJc
                              from Rc in LJc.DefaultIfEmpty()

                              join d in DbSAS.codeJobTitlesLanguages.Where(x => x.LanguageID == "EN") on a.JobTitleGUID equals d.JobTitleGUID into LJ3
                              from RJ3 in LJ3.DefaultIfEmpty()
                              join dd in DbSAS.codeJobTitlesLanguages.Where(x => x.LanguageID == "AR") on a.JobTitleGUID equals dd.JobTitleGUID into LJd
                              from Rd in LJd.DefaultIfEmpty()


                              join e in DbSAS.userPersonalDetailsLanguage.Where(x => x.LanguageID == "EN") on a.UserGUID equals e.UserGUID into LJ4
                              from R4 in LJ4.DefaultIfEmpty()
                              join ee in DbSAS.userPersonalDetailsLanguage.Where(x => x.LanguageID == "AR") on a.UserGUID equals ee.UserGUID into LJe
                              from Re in LJe.DefaultIfEmpty()

                              //where a.Active && RJ1.Active && Rb.Active && RJ2.Active && Rc.Active && RJ3.Active && Rd.Active && R4.Active && Re.Active
                              orderby a.EmailAddress ascending
                              select new LocalDBUserModel
                              {
                                  UserGUID = a.UserGUID,
                                  EmailAddress = a.EmailAddress,

                                  DutyStationDescriptionEN = RJ1.DutyStationDescription,
                                  DutyStationDescriptionAR = Rb.DutyStationDescription,

                                  DepartmentDescriptionEN = RJ2.DepartmentDescription,
                                  DepartmentDescriptionAR = Rc.DepartmentDescription,

                                  JobTitleDescriptionEN = RJ3.JobTitleDescription,
                                  JobTitleDescriptionAR = Rd.JobTitleDescription,
                                  

                                  FirstNameEN = R4.FirstName,
                                  SurnameEN = R4.Surname,

                                  FirstNameAR = Re.FirstName,
                                  SurnameAR = Re.Surname
                              }).Distinct().ToList();

            foreach (var usr in localUsers)
            {
                ADUser aDUser = ActiveDirectoryLookup(usr.EmailAddress.Split('@')[0]);

                model.Add(new StaffDataQualityModel
                {
                    LocalUser = usr,
                    ADUser = aDUser
                });
            }

            return View("~/Areas/ORG/Views/DataQuality/Index.cshtml", model);
        }

       
        public ADUser ActiveDirectoryLookup(string UserID)
        {

            DirectoryEntry ldapConnection = new DirectoryEntry("LDAP://10.240.224.203:389/OU=Organisation,DC=UNHCR,DC=LOCAL", "a-syrdascan", "Unhcr@1234", AuthenticationTypes.Secure);
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
            if (result == null)
            {
                return null;
            }
            Guid OrganizationInstanceGUID = Guid.Empty;
            Guid UNHCRguid = Guid.Parse("F9BD9237-CA5B-4753-A0EB-B1E4C33B1490");

            ADUser aDUser = new ADUser();
            try { aDUser.sn = result.Properties["sn"][0].ToString(); } catch { }
            try { aDUser.samaccountname = result.Properties["samaccountname"][0].ToString(); } catch { }
            try { aDUser.mail = result.Properties["mail"][0].ToString(); } catch { }
            try { aDUser.displayname = result.Properties["displayname"][0].ToString(); } catch { }
            try { aDUser.telephoneNumber = result.Properties["telephoneNumber"][0].ToString(); } catch { }
            try { aDUser.title = result.Properties["title"][0].ToString(); } catch { }
            try { aDUser.department = result.Properties["department"][0].ToString(); } catch { }
            try { aDUser.givenName = result.Properties["givenName"][0].ToString(); } catch { }

            try { aDUser.firstName = result.Properties["givenName"][0].ToString(); } catch { }
            try { aDUser.surName = result.Properties["displayname"][0].ToString().Substring(result.Properties["givenName"][0].ToString().Length).ToString(); } catch { }

            try { aDUser.co = result.Properties["co"][0].ToString(); } catch { }
            try { aDUser.l = result.Properties["l"][0].ToString(); } catch { }

            return aDUser;
        }

    }
}