using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppsPortal.Data
{
    public class FactorsCollector
    {
        private CMSEntities DbCMS;
        private Guid UserGUID = HttpContext.Current.Session[SessionKeys.UserGUID] != null ? Guid.Parse(HttpContext.Current.Session[SessionKeys.UserGUID].ToString()) : Guid.Empty;
        private Guid UserProfileGUID = HttpContext.Current.Session[SessionKeys.UserProfileGUID] != null ? Guid.Parse(HttpContext.Current.Session[SessionKeys.UserProfileGUID].ToString()) : Guid.Empty;
        string LAN = Languages.CurrentLanguage().ToUpper();

        public FactorsCollector(CMSEntities DbCMS)
        {
            this.DbCMS = DbCMS;
        }

        public FactorsCollector()
        {
            this.DbCMS = new CMSEntities();
        }




        public string LookupValue(Guid PK)
        {
            if (PK != Guid.Empty)
            {
                Guid FactorGuid = DbCMS.codeTablesValues.Where(v => v.ValueGUID == PK).FirstOrDefault().TableGUID;
                return FactorGuid.ToString();
            }
            else
            {
                return Guid.Empty.ToString();
            }
        }

        public string LookupValueLanguage(Guid PK)
        {
            if (PK != Guid.Empty)
            {
                Guid FactorGuid = DbCMS.codeTablesValuesLanguages.Where(v => v.TableValueLanguageGUID == PK).FirstOrDefault().codeTablesValues.TableGUID;
                return FactorGuid.ToString();
            }
            else
            {
                return Guid.Empty.ToString();
            }
        }



        public string Office(Guid OfficeGUID)
        {
            if (OfficeGUID != Guid.Empty)
            {
                return DbCMS.codeOffices.Where(s => s.OfficeGUID == OfficeGUID).FirstOrDefault().OrganizationGUID.ToString();
            }
            else
            {
                return Guid.Empty.ToString();
            }
        }

        public string DutyStation(Guid CaseGUID)
        {
            if (CaseGUID != Guid.Empty)
            {
                return null;
                //return DbCMS.codeApplications.Where(s => s.OfficeGUID == OfficeGUID).FirstOrDefault().OrganizationGUID.ToString();
            }
            else
            {
                return Guid.Empty.ToString();
            }
        }
    }
}