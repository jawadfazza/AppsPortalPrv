using AppsPortal.Controllers;
using AppsPortal.Data;
using AppsPortal.Extensions;
using AppsPortal.Library;
using ExpressiveAnnotations.MvcUnobtrusive.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;


namespace AppsPortal
{


    public class MvcApplication : System.Web.HttpApplication
    {

        protected void Application_Start()
        {
            //AreaRegistration.RegisterAllAreas();
            System.Web.Http.GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            //BundleConfig.RegisterBundles(BundleTable.Bundles);
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new MyViewEngine());

            AutoMapper.Mapper.Initialize(cfg =>
            {
                cfg.CreateMissingTypeMaps = true;
                cfg.AllowNullCollections = false;
                cfg.AddProfile<CMSMappingProfiles>();
            });

            // Register the default hubs route: ~/signalr/hubs
            //RouteTable.Routes.MapHubs();

            GlobalFilters.Filters.Add(new CheckSessionOutAttribute());

            ModelBinders.Binders.Add(typeof(DateTime), new DateTimeModelBinder());
            ModelBinders.Binders.Add(typeof(DateTime?), new DateTimeModelBinder());

            ModelBinders.Binders.Add(typeof(string), new StringModelBinder());

            ModelValidatorProviders.Providers.Remove(ModelValidatorProviders.Providers.FirstOrDefault(x => x is DataAnnotationsModelValidatorProvider));
            ModelValidatorProviders.Providers.Add(new ExpressiveAnnotationsModelValidatorProvider());

            //HttpContext.Current.Session.RemoveAll();

            HttpContext.Current.Application["UsersTokens"] = new Dictionary<String, String>();
            HttpContext.Current.Application["OnlineUsers"] = new List<Guid>();

            new CMS().BuildPublicMenus();

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            try
            {

                string cultureName = null;

                // Attempt to read the culture cookie from Request
                HttpCookie cultureCookie = Request.Cookies["_culture"];
                if (cultureCookie != null)
                    cultureName = cultureCookie.Value;
                else
                    cultureName = Request.UserLanguages != null && Request.UserLanguages.Length > 0 ? Request.UserLanguages[0] : null; // obtain it from HTTP header AcceptLanguages

                // Modify current thread's cultures            
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cultureName);
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(cultureName);
            }
            catch (Exception ex) { }
        }

        void Application_AcquireRequestState(object sender, EventArgs e)
        {
            try
            {
                if (HttpContext.Current.Session[SessionKeys.UserProfileGUID] == null)
                {
                    List<string> noAct = new List<string> { "/LOGIN", "/ACCOUNT/INDEX", "/ACCOUNT/LOGIN", "/Media/Users" };
                    List<string> wait = new List<string> { "/AMS"};
                    var url = Request.Url;
                    if (!noAct.Contains(url.AbsolutePath.ToUpper()) && !url.ToString().EndsWith(".js") && !url.ToString().EndsWith(".css") && !url.ToString().EndsWith(".jpg"))
                    {
                        HttpContext context = HttpContext.Current;
                        context.Session["RedirectURL"] = url;
                    }

                }
              
            }
            catch
            {
                List<string> noAct = new List<string> { "/LOGIN", "/ACCOUNT/INDEX", "/ACCOUNT/LOGIN", "/Media/Users" };
                var url = Request.Url;
                if (!noAct.Contains(url.AbsolutePath.ToUpper()) && !url.ToString().EndsWith(".js") && !url.ToString().EndsWith(".css") && !url.ToString().EndsWith(".jpg"))
                {
                    HttpContext context = HttpContext.Current;
                    try { context.Session["RedirectURL"] = url; } catch { }
                }
            }
        }

        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
        public class CheckSessionOutAttribute : System.Web.Mvc.ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                List<string> controllers = new List<string> { "account", "registration", "vote", "changelanguage", "partnerscapacityassessment", "powerbi" };
                string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToLower();
                if (!controllers.Contains(controllerName))
                {

                    HttpSessionStateBase session = filterContext.HttpContext.Session;
                    var user = session[SessionKeys.UserGUID]; //Key 2 should be User or UserName
                    if (((user == null) && (!session.IsNewSession)) || (session.IsNewSession))
                    {
                        //send them off to the login page
                        var url = new UrlHelper(filterContext.RequestContext);
                        var loginUrl = url.Content("~/Account/Index");
                        filterContext.HttpContext.Response.Redirect(loginUrl, true);
                    }
                }
            }
        }


        //protected void Application_Error(object sender, EventArgs e)
        //{
        //    Exception exception = Server.GetLastError();
        //    Response.Clear();
        //    HttpException httpException = exception as HttpException;
        //    if (httpException != null)
        //    {
        //        HttpStatusCode statusCode = (HttpStatusCode)httpException.GetHttpCode();
        //        Server.ClearError();
        //        if (exception.Message == "SessionTimeOut")
        //        {
        //            IController loginController = new AccountController();
        //            var routeData = new RouteData();
        //            routeData.Values["controller"] = "Account";
        //            routeData.Values["action"] = "SessionTimeout";
        //            RequestContext rq = new RequestContext(new HttpContextWrapper(Context), routeData);
        //            loginController.Execute(rq);
        //        }
        //        else
        //        {
        //            IController errorController = new ErrorsController();
        //            var routeData = new RouteData();
        //            routeData.Values["controller"] = "Errors";
        //            routeData.Values["action"] = "Error";
        //            routeData.Values["statusCode"] = statusCode;
        //            routeData.Values["BackURL"] = exception.Message; //Hey, this is not the message this is the backURL;
        //            RequestContext rq = new RequestContext(new HttpContextWrapper(Context), routeData);
        //            errorController.Execute(rq);
        //        }

        //        Response.End();
        //    }
        //    else
        //    {

        //        IController errorController = new ErrorsController();
        //        var routeData = new RouteData();
        //        routeData.Values["controller"] = "Errors";
        //        routeData.Values["action"] = "Error";
        //        routeData.Values["statusCode"] = System.Net.HttpStatusCode.InternalServerError;
        //        routeData.Values["BackURL"] = exception.HelpLink; //Hey, this is not the message this is the backURL;
        //        RequestContext rq = new RequestContext(new HttpContextWrapper(Context), routeData);
        //        errorController.Execute(rq);
        //    }
        //}

        protected void Application_End(object sender, EventArgs e)
        {
            //  Code that runs on application shutdown
        }

        protected void Session_End(object sender, EventArgs e)
        {
        }
    }
}
