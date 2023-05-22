using System.IO;
using System.Web;
using System.Web.Mvc;

namespace AppsPortal.Extensions
{
    public class MyViewEngine : RazorViewEngine
    {
        public MyViewEngine()
        {
            var path = HttpContext.Current.Server.MapPath("~/Views/Shared");
            string[] fileEntries = Directory.GetDirectories(path);
            string[] locations = new string[fileEntries.Length + 1];

            locations[0] = "~/Views/{1}/{0}.cshtml";
            for (int i = 1; i <= fileEntries.Length; i++)
            {
                locations[i] = "~/Views/Shared/" + Path.GetFileName(fileEntries[i - 1]) + "/{0}.cshtml";
            }
            var viewEngine = new RazorViewEngine
            {
                ViewLocationFormats = locations,
                PartialViewLocationFormats = locations,
                MasterLocationFormats = locations,
                //AreaMasterLocationFormats = locations,
                //AreaViewLocationFormats = locations
            };

            ViewEngines.Engines.Add(viewEngine);
        }
    }
}