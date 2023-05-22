using System.Web.Mvc;

namespace AppsPortal.Areas.OVS
{
    public class OVSAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "OVS";
            }
        }


        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                name: "OVSTwoParameters",
                url: "OVS/{controller}/{action}/{PK}/{FK}");

            context.MapRoute(
                name: "OVS_default",
                url: "OVS/{controller}/{action}/{PK}",
                defaults: new { controller = "Home", action = "Index", PK = UrlParameter.Optional },
                namespaces: new[] { "AppsPortal.Areas.OVS.Controllers" }
            );

        
        }

    }
}