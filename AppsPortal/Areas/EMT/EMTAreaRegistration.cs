using System.Web.Mvc;

namespace AppsPortal.Areas.EMT
{
    public class EMTAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "EMT";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                name: "EMTTwoParameters",
                url: "EMT/{controller}/{action}/{PK}/{FK}");

            context.MapRoute(
                name: "EMT_default",
                url: "EMT/{controller}/{action}/{PK}",
                defaults: new { controller = "Home", action = "Index", PK = UrlParameter.Optional },
                namespaces: new[] { "AppsPortal.Areas.EMT.Controllers" }
            );
        }

    }
}