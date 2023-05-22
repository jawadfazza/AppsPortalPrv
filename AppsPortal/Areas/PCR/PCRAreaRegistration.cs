using System.Web.Mvc;

namespace AppsPortal.Areas.PCR
{
    public class PCRAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "PCR";
            }
        }
        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                name: "PCRTwoParameters",
                url: "PCR/{controller}/{action}/{PK}/{FK}");

            context.MapRoute(
                name: "PCR_default",
                url: "PCR/{controller}/{action}/{PK}",
                defaults: new { controller = "Home", action = "Index", PK = UrlParameter.Optional },
                namespaces: new[] { "AppsPortal.Areas.PCR.Controllers" }
            );
        }

     
    }
}