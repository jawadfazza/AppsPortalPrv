using System.Web.Mvc;

namespace AppsPortal.Areas.GTP
{
    public class GTPAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "GTP";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                name: "GTPTwoParameters",
                url: "GTP/{controller}/{action}/{PK}/{FK}");

            context.MapRoute(
                name: "GTP_default",
                url: "GTP/{controller}/{action}/{PK}",
                defaults: new { controller = "Home", action = "Index", PK = UrlParameter.Optional },
                namespaces: new[] { "AppsPortal.Areas.GTP.Controllers" }
            );
        }
    }
}