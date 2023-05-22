using System.Web.Mvc;

namespace AppsPortal.Areas.TTT
{
    public class TTTAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "TTT";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                name: "TTTTwoParameters",
                url: "TTT/{controller}/{action}/{PK}/{FK}");

            context.MapRoute(
                name: "TTT_default",
                url: "TTT/{controller}/{action}/{PK}",
                defaults: new { controller = "Home", action = "Index", PK = UrlParameter.Optional },
                namespaces: new[] { "AppsPortal.Areas.TTT.Controllers" }
            );
        }
    }
}