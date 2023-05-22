using AppsPortal.ViewModels;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Web;
using TBS_DAL.Model;

namespace AppsPortal.Areas.TBS.ADHelpers
{
    public class ADHelper
    {
        private string country = null;
        private string dutystation = null;
        private string adPath = null;

        public ADHelper(Guid DutyStationGUID, TBSEntities DbTBS)
        {
            var DutyStationDescription = (from a in DbTBS.codeDutyStationsLanguages.Where(x => x.DutyStationGUID == DutyStationGUID && x.Active && x.LanguageID == "EN")
                                          select a.DutyStationDescription).FirstOrDefault();
            switch (DutyStationDescription)
            {
                case "Damascus":
                    this.country = "SYR";
                    this.dutystation = "DA";
                    break;
                case "Aleppo":
                    this.country = "SYR";
                    this.dutystation = "AL";
                    break;
                case "Sweida":
                    this.country = "SYR";
                    this.dutystation = "SW";
                    break;
                case "Tartous":
                    this.country = "SYR";
                    this.dutystation = "TA";
                    break;
                case "Homs":
                    this.country = "SYR";
                    this.dutystation = "HO";
                    break;
            }
            //string path = "LDAP://10.240.224.203:389/OU=Users,OU=Users And Groups,OU=" + dutystation + ",OU=" + country + ",OU=Country,OU=Organisation,DC=UNHCR,DC=LOCAL";
            //this.adPath = "LDAP://10.240.224.203:389/OU=Organisation,DC=UNHCR,DC=LOCAL";
            //this.adPath = "LDAP://unhcr.local/OU=Organisation,DC=UNHCR,DC=LOCAL";
            //this.adPath = "LDAP://10.240.224.203:389/OU=Users,OU=Users And Groups,OU=" + dutystation + ",OU=" + country + ",OU=Country,OU=Organisation,DC=UNHCR,DC=LOCAL";
        }

        public string GetUserEmailByNameFromAD(string fullName)
        {

            fullName = fullName.Replace(" _P", "");
            DirectoryEntry ldapConnection = new DirectoryEntry("LDAP://10.240.224.203:389/OU=Organisation,DC=UNHCR,DC=LOCAL", "a-syrdascan", "Unhcr@1234", AuthenticationTypes.Secure);
            DirectorySearcher search = new DirectorySearcher(ldapConnection);
            search.Filter = "(displayname=" + fullName + ")";
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
                return "";
            }
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

            return adu.mail;
        }

        public string GetUserEmailByExtensionNumber(string extension)
        {
            //DirectoryEntry ldapConnection = new DirectoryEntry(this.adPath, "a-syrdascan", "Unhcr@1234", AuthenticationTypes.Secure);
            //DirectorySearcher searcher = new DirectorySearcher(ldapConnection);
            //searcher.SearchScope = SearchScope.OneLevel;
            //searcher.Filter = "(displayname=*" + extension + ")";
            //searcher.PropertiesToLoad.Add("mail");
            //SearchResult result = searcher.FindOne();
            //string emailAddress = result.Properties["mail"][0].ToString();
            //return emailAddress;
            return "";
        }
    }
}