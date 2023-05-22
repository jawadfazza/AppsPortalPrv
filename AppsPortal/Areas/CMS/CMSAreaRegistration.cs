using System.Web.Mvc;

namespace AppsPortal.Areas.CMS
{
    public class CMSAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "CMS";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                name: "CMSTwoParameters",
                url: "CMS/{controller}/{action}/{PK}/{FK}");

            context.MapRoute(
                name: "CMS_default",
                url: "CMS/{controller}/{action}/{PK}",
                defaults: new { controller = "Home", action = "Index", PK = UrlParameter.Optional },
                namespaces: new[] { "AppsPortal.Areas.CMS.Controllers" }
            );
        }
    }
}