using System.Web.Mvc;

namespace AppsPortal.Areas.PRG
{
    public class PRGAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "PRG";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                name: "PRGTwoParameters",
                url: "PRG/{controller}/{action}/{PK}/{FK}");

            context.MapRoute(
                name: "PRG_default",
                url: "PRG/{controller}/{action}/{PK}",
                defaults: new { controller = "Home", action = "Index", PK = UrlParameter.Optional },
                namespaces: new[] { "AppsPortal.Areas.PRG.Controllers" }
            );
        }
    }
}