using System.Web.Mvc;

namespace AppsPortal.Areas.AMS
{
    public class AMSAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "AMS";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                name: "AMSTwoParameters",
                url: "AMS/{controller}/{action}/{PK}/{FK}");

            context.MapRoute(
                name: "AMS_default",
                url: "AMS/{controller}/{action}/{PK}",
                defaults: new { controller = "Home", action = "Index", PK = UrlParameter.Optional },
                namespaces: new[] { "AppsPortal.Areas.AMS.Controllers" }
            );
        }
    }
}