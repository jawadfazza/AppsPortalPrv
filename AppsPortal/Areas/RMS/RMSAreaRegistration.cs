using System.Web.Mvc;

namespace AppsPortal.Areas.RMS
{
    public class RMSAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "RMS";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                name: "RMSTwoParameters",
                url: "RMS/{controller}/{action}/{PK}/{FK}");

            context.MapRoute(
                name: "RMS_default",
                url: "RMS/{controller}/{action}/{PK}",
                defaults: new { controller = "Home", action = "Index", PK = UrlParameter.Optional },
                namespaces: new[] { "AppsPortal.Areas.RMS.Controllers" }
            );
        }

    }
}