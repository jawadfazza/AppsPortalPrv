using AppsPortal.Areas.PRG.Models;
using AppsPortal.Library;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.PRG.Data
{
    public class PRGDropDownList
    {
        private PRGEntities DbPRG;
        private Guid UserGUID = HttpContext.Current.Session[SessionKeys.UserGUID] != null ? Guid.Parse(HttpContext.Current.Session[SessionKeys.UserGUID].ToString()) : Guid.Empty;
        private Guid UserProfileGUID = HttpContext.Current.Session[SessionKeys.UserProfileGUID] != null ? Guid.Parse(HttpContext.Current.Session[SessionKeys.UserProfileGUID].ToString()) : Guid.Empty;
        string LAN = Extensions.Languages.CurrentLanguage().ToUpper();

        public PRGDropDownList(PRGEntities DbPRG)
        {
            this.DbPRG = DbPRG;
        }

        public PRGDropDownList()
        {
            this.DbPRG = new PRGEntities();
        }
        public SelectList ProcessStatus()
        {
            string LANProGres = "EN";
            if (LAN.ToUpper() != "AR")
            {
                LANProGres = LAN;
            }
            var Result  = (from a in DbPRG.codeProcessStatusText.Where(x => x.ProcessStatusLanguageCode.Contains(LANProGres) && x.ProcessStatusCode!="-")
                   select new
                   {
                       Text = a.ProcessStatusText,
                       Value = a.ProcessStatusCode
                   }).ToList();
            return new SelectList(Result, "Value", "Text");
        }
    }
}