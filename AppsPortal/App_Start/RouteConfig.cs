using System.Web.Mvc;
using System.Web.Routing;

namespace AppsPortal
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapMvcAttributeRoutes();
            AreaRegistration.RegisterAllAreas();

            routes.MapRoute(
                name: "TwoParameters",
                url: "{controller}/{action}/{PK}/{FK}"

            );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{PK}",
                defaults: new { controller = "Account", action = "Index", PK = UrlParameter.Optional },
                namespaces: new[] { "AppsPortal.Controllers" }
            );
        }
    }
}


