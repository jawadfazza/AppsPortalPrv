using System.Web.Optimization;

namespace AppsPortal
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/bundles/styles")
                .Include("~/Assets/Plugin/UNHCR/app.css")

                .Include("~/Assets/Plugin/Pikaday/pikaday.css")
                .Include("~/Assets/Plugin/Pikaday/theme.css")
                .Include("~/Assets/Plugin/font-awesome/css/font-awesome.min.css")
                .Include("~/Assets/Plugin/Select2/select2.css")
                .Include("~/Assets/Plugin/DataTable/jquery.dataTables.min.css")
                .Include("~/Assets/Plugin/DataTable/Responsive/responsive.jqueryui.min.css")
                .Include("~/Assets/Plugin/DataTable/Buttons/buttons.datatable.css")
                .Include("~/Assets/Plugin/iCheck/blue.css")
                .Include("~/Assets/Plugin/WebUI-Popover/jquery.webui-popover.css")
                .Include("~/Assets/Style/Site.css"));

            //////MvcFoolproofValidation
            //bundles.Add(new ScriptBundle("~/bundles/MvcFoolproofValidation").Include(
            //    "~/Assets/Plugin/Mvcfoolproof/mvcfoolproof.unobtrusive.min.js",
            //    "~/Assets/Plugin/Mvcfoolproof/MvcFoolproofJQueryValidation.min.js",
            //    "~/Assets/Plugin/Mvcfoolproof/MvcFoolproofValidation.min.js"));



            bundles.Add(new ScriptBundle("~/bundles/Scripts")
                    .Include("~/Assets/Plugin/JQuery/jquery-3.2.1.js")
                    .Include("~/Assets/Plugin/JQuery/jquery-ui.min.js")
                    .Include("~/Assets/Plugin/JQuery/jquery-touch-punch.js")
                    .Include("~/Assets/Plugin/moment/moment.min.js")
                    .Include("~/Assets/Plugin/Modernizr/modernizer.js")
                    .Include("~/Assets/Plugin/Bootstrap/bootstrap.js")
                    .Include("~/Assets/Plugin/Pikaday/pikadayTime.js")
                    .Include("~/Assets/Plugin/Pikaday/pikaday.jquery.js")
                    .Include("~/Assets/Plugin/Select2/select2.full.min.js")
                    .Include("~/Assets/Plugin/DataTable/jquery.dataTables.js")
                    .Include("~/Assets/Plugin/DataTable/Responsive/dataTables.responsive.min.js")
                    .Include("~/Assets/Plugin/DataTable/Buttons/dataTables.buttons.min.js")
                    .Include("~/Assets/Plugin/iCheck/icheck.min.js")
                    .Include("~/Assets/Plugin/WebUI-Popover/jquery.webui-popover.js")
                    .Include("~/Assets/Plugin/JQuery/jquery.counterup.min.js")
                    .Include("~/Assets/Plugin/JQuery/jquery.validate.min.js")
                    .Include("~/Assets/Plugin/JQuery/jquery.validate.unobtrusive.min.js")
                    .Include("~/Assets/Plugin/JQuery/jquery.easing.min.js")
                    .Include("~/Assets/Plugin/JQuery/jquery.cookie.min.js")
                    .Include("~/Assets/Plugin/bxSlider/jquery.bxslider.min.js")
                    //.Include("~/Assets/Plugin/UNHCR/jquery.tweetHighlighted.min.js")//Should be removed and from apps.js
                    //.Include("~/Assets/Plugin/EasyTab/jquery.easytabs.min.js")
                    //.Include("~/Assets/Plugin/matchHeight/jquery.matchHeight.min.js")
                    .Include("~/Assets/Plugin/FitText/jquery.fittext.min.js")
                    .Include("~/Assets/Plugin/UNHCR/app.min.js")
                    .Include("~/Assets/Plugin/JSZip/jszip.min.js")                    
                    
                    .Include("~/Assets/Plugin/DataTable/Buttons/buttons.print.min.js")
                    .Include("~/Assets/Plugin/DataTable/Buttons/buttons.html5.min.js")
                    .Include("~/Assets/JScript/JScript.js")
                );
            BundleTable.EnableOptimizations = false;
        }
    }
}
