using System.Web.Mvc;

namespace AppsPortal.Areas.SHM
{
    public class SHMAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "SHM";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                name: "SHMTwoParameters",
                url: "SHM/{controller}/{action}/{PK}/{FK}");

            context.MapRoute(
                name: "SHM_default",
                url: "SHM/{controller}/{action}/{PK}",
                defaults: new { controller = "Home", action = "Index", PK = UrlParameter.Optional },
                namespaces: new[] { "AppsPortal.Areas.SHM.Controllers" }
            );
        }
    }
}