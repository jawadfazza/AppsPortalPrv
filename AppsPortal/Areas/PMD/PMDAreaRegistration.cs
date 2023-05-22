using System.Web.Mvc;

namespace AppsPortal.Areas.PMD
{
    public class PMDAreaRegistration : AreaRegistration 
    {
        public override string AreaName
        {
            get
            {
                return "PMD";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                name: "PMDTwoParameters",
                url: "PMD/{controller}/{action}/{PK}/{FK}");

            context.MapRoute(
                name: "PMD_default",
                url: "PMD/{controller}/{action}/{PK}",
                defaults: new { controller = "Home", action = "Index", PK = UrlParameter.Optional },
                namespaces: new[] { "AppsPortal.Areas.PMD.Controllers" }
            );
        }
    }
}