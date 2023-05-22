using RES_Repo.Globalization;
using System.Net;
using System.Web.Mvc;
using System.Web.Routing;

namespace AppsPortal.Library
{
    public class ErrorsController : Controller
    {
        public ActionResult Error(HttpStatusCode statusCode, string BackURL)
        {
            //exceptionMessage ?? do we need it ? maybe not !!
            string title = resxHttpErrors.ResourceManager.GetString(statusCode.ToString() + "Title");
            string errorHeader = resxHttpErrors.ResourceManager.GetString(statusCode.ToString() + "Header");
            string errorDescription = resxHttpErrors.ResourceManager.GetString(statusCode.ToString() + "Description");

            ViewBag.Title = title == null ? resxHttpErrors.GenericTitle : title;
            ViewBag.ErrorHeader = errorHeader == null ? resxHttpErrors.GenericHeader : errorHeader;
            ViewBag.ErrorDescription = errorDescription == null ? resxHttpErrors.GenericDescription : errorDescription;
            ViewBag.RedirectURL = BackURL;
            if (Request.IsAjaxRequest())
            {
                return JavaScript("window.location = '/Errors/Error?BackURL = " + BackURL + "&statusCode=" + statusCode + "'");
            }
            else
            {
                return View("~/Views/Shared/Errors/Error.cshtml");
            }
            
        }
    }
}