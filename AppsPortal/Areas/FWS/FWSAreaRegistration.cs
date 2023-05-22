using System.Web.Mvc;

namespace AppsPortal.Areas.FWS
{
    public class FWSAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "FWS";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
               name: "FWSTwoParameters",
               url: "FWS/{controller}/{action}/{PK}/{FK}");

            context.MapRoute(
                name: "FWS_default",
                url: "FWS/{controller}/{action}/{PK}",
                defaults: new { controller = "Home", action = "Index", PK = UrlParameter.Optional },
                namespaces: new[] { "AppsPortal.Areas.FWS.Controllers" }
            );
        }
    }
}