using System.Web.Mvc;

namespace AppsPortal.Areas.IMS
{
    public class IMSAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "IMS";
            }
        }
        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                name: "IMSTwoParameters",
                url: "IMS/{controller}/{action}/{PK}/{FK}");

            context.MapRoute(
                name: "IMS_default",
                url: "IMS/{controller}/{action}/{PK}",
                defaults: new { controller = "Home", action = "Index", PK = UrlParameter.Optional },
                namespaces: new[] { "AppsPortal.Areas.IMS.Controllers" }
            );
        }
       
    }
}