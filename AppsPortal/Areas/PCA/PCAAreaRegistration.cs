using System.Web.Mvc;

namespace AppsPortal.Areas.PCA
{
    public class PCAAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "PCA";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                name: "PCATwoParameters",
                url: "PCA/{controller}/{action}/{PK}/{FK}");

            context.MapRoute(
                name: "PCA_default",
                url: "PCA/{controller}/{action}/{PK}",
                defaults: new { controller = "Home", action = "Index", PK = UrlParameter.Optional },
                namespaces: new[] { "AppsPortal.Areas.PCA.Controllers" }
            );
        }

    }
}