using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Areas.TBS
{
    public class TBSAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "TBS";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                name: "TBSTwoParameters",
                url: "TBS/{controller}/{action}/{PK}/{FK}");

            context.MapRoute(
                name: "TBS_default",
                url: "TBS/{controller}/{action}/{PK}",
                defaults: new { controller = "Home", action = "Index", PK = UrlParameter.Optional },
                namespaces: new[] { "AppsPortal.Areas.TBS.Controllers" }
            );
        }
    }
}