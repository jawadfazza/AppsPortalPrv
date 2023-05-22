using System.Web.Mvc;

namespace AppsPortal.Areas.MRS
{
    public class MRSAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "MRS";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                name: "MRSTwoParameters",
                url: "MRS/{controller}/{action}/{PK}/{FK}");

            context.MapRoute(
                name: "MRS_default",
                url: "MRS/{controller}/{action}/{PK}",
                defaults: new { controller = "Home", action = "Index", PK = UrlParameter.Optional },
                namespaces: new[] { "AppsPortal.Areas.MRS.Controllers" }
            );
        }

    }
}