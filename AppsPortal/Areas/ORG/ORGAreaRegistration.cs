using System.Web.Mvc;

namespace AppsPortal.Areas.ORG
{
    public class ORGAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "ORG";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                name: "ORGTwoParameters",
                url: "ORG/{controller}/{action}/{PK}/{FK}");

            context.MapRoute(
                name: "ORG_default",
                url: "ORG/{controller}/{action}/{PK}",
                defaults: new { controller = "Home", action = "Index", PK = UrlParameter.Optional },
                namespaces: new[] { "AppsPortal.Areas.ORG.Controllers" }
            );
        }



    }
}