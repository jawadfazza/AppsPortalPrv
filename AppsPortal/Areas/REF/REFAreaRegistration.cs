using System.Web.Mvc;

namespace AppsPortal.Areas.REF
{
    public class REFAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "REF";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                name: "REFTwoParameters",
                url: "REF/{controller}/{action}/{PK}/{FK}");

            context.MapRoute(
                name: "REF_default",
                url: "REF/{controller}/{action}/{PK}",
                defaults: new { controller = "Home", action = "Index", PK = UrlParameter.Optional },
                namespaces: new[] { "AppsPortal.Areas.REF.Controllers" }
            );
        }
    }
}