using System.Web.Mvc;

namespace AppsPortal.Areas.ISS
{
    public class ISSAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "ISS";
            }
        }
        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                name: "ISSTwoParameters",
                url: "ISS/{controller}/{action}/{PK}/{FK}");

            context.MapRoute(
                name: "ISS_default",
                url: "ISS/{controller}/{action}/{PK}",
                defaults: new { controller = "Home", action = "Index", PK = UrlParameter.Optional },
                namespaces: new[] { "AppsPortal.Areas.ISS.Controllers" }
            );
        }
    }
}