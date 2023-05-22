using System.Web.Mvc;

namespace AppsPortal.Areas.IDC
{
    public class IDCAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "IDC";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                name: "IDCTwoParameters",
                url: "IDC/{controller}/{action}/{PK}/{FK}");

            context.MapRoute(
                name: "IDC_default",
                url: "IDC/{controller}/{action}/{PK}",
                defaults: new { controller = "Home", action = "Index", PK = UrlParameter.Optional },
                namespaces: new[] { "AppsPortal.Areas.IDC.Controllers" }
            );
        }

    }
}