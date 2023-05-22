using System.Web.Mvc;

namespace AppsPortal.Areas.AHD
{
    public class AHDAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "AHD";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                name: "AHDTwoParameters",
                url: "AHD/{controller}/{action}/{PK}/{FK}");

            context.MapRoute(
                name: "AHD_default",
                url: "AHD/{controller}/{action}/{PK}",
                defaults: new { controller = "Home", action = "Index", PK = UrlParameter.Optional },
                namespaces: new[] { "AppsPortal.Areas.AHD.Controllers" }
            );
        }


    }
}