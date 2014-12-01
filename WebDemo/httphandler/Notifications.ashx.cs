using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
using WebDemo.Models;
using WebDemo.Models.Repository;
using WebDemo.Tools;
using System.Web.Caching;
using System.Threading;
using System.Diagnostics;
using MD.CloudConnect.Data;

namespace WebDemo.httphandler
{
    /// <summary>
    /// Summary description for Notifications
    /// </summary>
    public class Notifications : IHttpHandler
    {
     
        public void ProcessRequest(HttpContext context)
        {
            string data = "";
            if (context.Request.HttpMethod == "POST")
            {
                using (StreamReader stream = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
                {
                    data = stream.ReadToEnd();
                }
                if (data.LongCount() < 6777216)
                {
                    lock (Tools.Log.Instance.Notification)
                    {
                        Tools.Log.Instance.Notification.Info(data);
                        MD.CloudConnect.CouchBaseProvider.CouchbaseManager.Instance.NotificationRepository.PushNotificationCache("GLOBAL", data, DateTime.UtcNow);
                    }
                }
            }
        }

        public bool IsReusable
        {
            get
            {
                return true;
            }
        }
    }
}