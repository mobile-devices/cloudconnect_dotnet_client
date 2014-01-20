using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Text;
using MongoDB.Driver;
using System.Web.Caching;

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
            WebDemo.Models.Repository.RepositoryFactory.Instance.Initialize();
            // LoadNotification();

            Tools.CloudConnectConnetor.Instance.InitializeFields();
            MD.CloudConnect.Notification.Instance.Initialize(Tools.CloudConnectConnetor.Instance.Fields, Tools.CloudConnectConnetor.Instance, true, true);
        }

        protected void Application_End()
        {
            // SaveNotification();
        }

        private void LoadNotification()
        {
            //try
            //{
            //    MongoCollection<WebDemo.Models.Notification> notificationDb = Tools.MongoConnector.Instance.DataBaseReadOnly.GetCollection<WebDemo.Models.Notification>("NOTIFICATION");
            //    List<WebDemo.Models.Notification> result = notificationDb.FindAll().ToList();
            //    if (result.Count > 0)
            //    {
            //        notificationDb.RemoveAll();

            //        lock (WebDemo.httphandler.Notifications._notificationQ)
            //        {
            //            WebDemo.httphandler.Notifications._notificationQ.AddRange(result);
            //        }
            //        if (HttpRuntime.Cache["Notification"] == null)
            //        {
            //            HttpRuntime.Cache.Insert("Notification", true, null, DateTime.Now.Add(new TimeSpan(0, 0, 10)), TimeSpan.Zero, CacheItemPriority.Normal, WebDemo.httphandler.Notifications.NotificationTask);
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Tools.Log.Instance.General.Error("Application_Start : " + ex.Message);
            //}
        }

        private void SaveNotification()
        {
            //try
            //{
            //    MongoCollection<WebDemo.Models.Notification> dataDb = Tools.MongoConnector.Instance.DataBase.GetCollection<WebDemo.Models.Notification>("NOTIFICATION");

            //    lock (WebDemo.httphandler.Notifications._notificationQ)
            //    {
            //        if (WebDemo.httphandler.Notifications._notificationQ.Count > 0)
            //            dataDb.InsertBatch<WebDemo.Models.Notification>(WebDemo.httphandler.Notifications._notificationQ);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Tools.Log.Instance.General.Error("Application_Stop : " + ex.Message);
            //}
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