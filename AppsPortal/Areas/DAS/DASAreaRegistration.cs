using System.Web.Mvc;

namespace AppsPortal.Areas.DAS
{
    public class DASAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "DAS";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                name: "DASTwoParameters",
                url: "DAS/{controller}/{action}/{PK}/{FK}");

            context.MapRoute(
                name: "DAS_default",
                url: "DAS/{controller}/{action}/{PK}",
                defaults: new { controller = "Home", action = "Index", PK = UrlParameter.Optional },
                namespaces: new[] { "AppsPortal.Areas.DAS.Controllers" }
            );
        }
    }
}