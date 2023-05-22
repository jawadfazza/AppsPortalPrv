using AppsPortal.BaseControllers;
using AppsPortal.Data;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.Models;
using System;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Controllers
{
    public class ChangeLanguageController : PortalBaseController
    {

 
        // GET: ChangeLanguage
        public ActionResult SetLanguage(string culture)
        {

            // Validate input
            //culture = Languages.CurrentCulture();
            if (culture.ToUpper().StartsWith("AR"))
            {
                culture = "ar-IQ";
            }
            // Save culture in a cookie
            HttpCookie cookie = Request.Cookies["_culture"];
            if (cookie != null)
                cookie.Value = culture;   // update cookie value
            else
            {
                cookie = new HttpCookie("_culture");
                cookie.Secure = true;
                cookie.Value = culture;
                cookie.Expires = DateTime.Now.AddYears(1);
            }
            this.ControllerContext.HttpContext.Response.Cookies.Add(cookie);

            if (Session[SessionKeys.UserGUID] != null)
            {
                Session[SessionKeys.FullName] = CMS.GetFullName(Guid.Parse(Session[SessionKeys.UserGUID].ToString()), culture);
                CMS.BuildUserMenus(UserGUID, culture.Split('-')[0].ToUpper());
            }
            string url = this.Request.UrlReferrer.AbsolutePath;

            return Redirect(url);
        }
    }
}