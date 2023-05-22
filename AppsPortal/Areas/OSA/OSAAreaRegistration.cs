using System.Web.Mvc;

namespace AppsPortal.Areas.OSA
{
    public class OSAAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "OSA";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                name: "OSATwoParameters",
                url: "OSA/{controller}/{action}/{PK}/{FK}");

            context.MapRoute(
                name: "OSA_default",
                url: "OSA/{controller}/{action}/{PK}",
                defaults: new { controller = "Home", action = "Index", PK = UrlParameter.Optional },
                namespaces: new[] { "AppsPortal.Areas.OSA.Controllers" }
            );
        }

    }
}