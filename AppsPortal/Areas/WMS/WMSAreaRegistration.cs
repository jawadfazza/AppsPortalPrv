using System.Web.Mvc;

namespace AppsPortal.Areas.WMS
{
    public class WMSAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get { return "WMS"; }
        }



        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                name: "WMSTwoParameters",
                url: "WMS/{controller}/{action}/{PK}/{FK}");

            context.MapRoute(
                name: "WMS_default",
                url: "WMS/{controller}/{action}/{PK}",
                defaults: new {controller = "Home", action = "Index", PK = UrlParameter.Optional},
                namespaces: new[] {"AppsPortal.Areas.WMS.Controllers"}
            );
        }
    }
}