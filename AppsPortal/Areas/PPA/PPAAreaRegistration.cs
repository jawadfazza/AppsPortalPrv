using System.Web.Mvc;

namespace AppsPortal.Areas.PPA
{
    public class PPAAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "PPA";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                name: "PPATwoParameters",
                url: "PPA/{controller}/{action}/{PK}/{FK}");

            context.MapRoute(
                name: "PPA_default",
                url: "PPA/{controller}/{action}/{PK}",
                defaults: new { controller = "Home", action = "Index", PK = UrlParameter.Optional },
                namespaces: new[] { "AppsPortal.Areas.PPA.Controllers" }
            );
        }

    }
}