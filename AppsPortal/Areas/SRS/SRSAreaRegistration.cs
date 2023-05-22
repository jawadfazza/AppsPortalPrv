using System.Web.Mvc;

namespace AppsPortal.Areas.SRS
{
    public class SRSAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "SRS";
            }
        }


        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                name: "SRSTwoParameters",
                url: "SRS/{controller}/{action}/{PK}/{FK}");

            context.MapRoute(
                name: "SRS_default",
                url: "SRS/{controller}/{action}/{PK}",
                defaults: new { controller = "Home", action = "Index", PK = UrlParameter.Optional },
                namespaces: new[] { "AppsPortal.Areas.SRS.Controllers" }
            );
        }
    }
}