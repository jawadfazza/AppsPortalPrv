using System.Web.Mvc;

namespace AppsPortal.Areas.COV
{
    public class COVAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "COV";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                name: "COVTwoParameters",
                url: "COV/{controller}/{action}/{PK}/{FK}");

            context.MapRoute(
                name: "COV_default",
                url: "COV/{controller}/{action}/{PK}",
                defaults: new { controller = "Home", action = "Index", PK = UrlParameter.Optional },
                namespaces: new[] { "AppsPortal.Areas.COV.Controllers" }
            );
        }

    }
}