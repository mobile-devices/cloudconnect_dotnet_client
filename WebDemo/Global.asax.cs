using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Text;
using MongoDB.Driver;
using System.Web.Caching;
using CloudConnect.MongoProvider;
using System.Reflection;

namespace WebDemo
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Tracking",
                "Tracking/index/{asset}/{year}/{month}/{day}",
                new { controller = "Tracking", action = "Index", asset = UrlParameter.Optional, year = UrlParameter.Optional, month = UrlParameter.Optional, day = UrlParameter.Optional }
            );
            routes.MapRoute(
                "Collection",
                "Collection/index/{asset}/{year}/{month}/{day}",
                new { controller = "Collection", action = "Index", asset = UrlParameter.Optional, year = UrlParameter.Optional, month = UrlParameter.Optional, day = UrlParameter.Optional }
            );
            routes.MapRoute(
               "HomeIndex",
               "Home/index/{id}",
               new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                "Home",
                "Home/{id}/{year}/{month}/{day}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional, year = UrlParameter.Optional, month = UrlParameter.Optional, day = UrlParameter.Optional }
            );



            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );


        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            Tools.Log.Instance.Initialize();
            //WebDemo.Models.Repository.RepositoryFactory.Instance.Initialize();

            Tools.CloudConnectConnetor.Instance.InitializeFields();

            //NotificationProvider provider = new NotificationProvider(System.Configuration.ConfigurationManager.AppSettings["MongoUri"], System.Configuration.ConfigurationManager.AppSettings["MongoDbName"]);

            CloudConnect.CouchBaseProvider.CouchbaseManager.RegisterModelViews(new Assembly[] { Assembly.GetAssembly(typeof(CloudConnect.CouchBaseProvider.CouchbaseManager)) });
            CloudConnect.CouchBaseProvider.NotificationProvider provider = new CloudConnect.CouchBaseProvider.NotificationProvider();
            WebDemo.Models.Repository.RepositoryFactory.Instance.Provider = new CloudConnect.CouchBaseProvider.RepositoryFactory();
            MD.CloudConnect.Notification.Instance.Initialize(Tools.CloudConnectConnetor.Instance.Fields, true, true, true, null, provider, 10, false, Tools.CloudConnectConnetor.Instance);

            //            MD.CloudConnect.Notification.Instance.Initialize(Tools.CloudConnectConnetor.Instance.Fields, true, true, true, null, null, 5, false, Tools.CloudConnectConnetor.Instance);

        }


        //catch error and log (local file and send an email if there is one user with email alerts)
        protected void Application_Error(object sender, EventArgs e)
        {
            Exception error = Server.GetLastError();
            var code = (error is HttpException) ? (error as HttpException).GetHttpCode() : 500;

            //Don't log 404 errors
            if (code != 404)
            {
                StringBuilder log = new StringBuilder();
                log.AppendLine(System.Web.HttpContext.Current.Request.Url.PathAndQuery);
                log.AppendLine(String.Format("Exception : {0}", error.Message));
                if (error.InnerException != null)
                {
                    log.AppendLine(String.Format("-> Inner Exception : {0}", error.InnerException.Message));
                }
                log.AppendLine(error.StackTrace);
                if (Tools.Log.Instance != null && Tools.Log.Instance.IsReady)
                {
                    Tools.Log.Instance.General.Error(log.ToString());
                }
            }
        }
    }
}