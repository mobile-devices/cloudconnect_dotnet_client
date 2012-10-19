using System;
using System.Web;
using System.IO;
using System.Collections.Generic;

namespace MD.CloudConnect.Example.HttpHandler
{
    public class Notification : IHttpHandler
    {
        /// <summary>
        /// You will need to configure this handler in the web.config file of your 
        /// web and register it with IIS before being able to use it. For more information
        /// see the following link: http://go.microsoft.com/?linkid=8101007
        /// </summary>
        #region IHttpHandler Members

        public bool IsReusable
        {
            // Return false in case your Managed Handler cannot be reused for another request.
            // Usually this would be false in case you have some state information preserved per request.
            get { return true; }
        }

        private static readonly object _lockHandler = new object();

        public void ProcessRequest(HttpContext context)
        {
            string data = String.Empty;

            if (context.Request.HttpMethod == "POST")
            {
                //to not have concurrent access
                if (System.Threading.Monitor.TryEnter(_lockHandler, 15000))
                {
                    try
                    {
                        using (StreamReader stream = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
                        {
                            data = stream.ReadToEnd();
                        }

                        List<MD.CloudConnect.MDData> decodedData = MD.CloudConnect.Notification.Instance.Decode(data);
                        foreach (MD.CloudConnect.MDData mdData in decodedData)
                        {
                            if (mdData.Meta.Event == "track")
                            {
                                ITracking tacking = mdData.Tracking;

                                /* use tracking.Longitude, tracking.Speed ... */
                            }
                        }
                    }
                    catch /* (Exception error) */
                    {
                        // Log error here for example with Log.net library
                    }
                    finally
                    {
                        System.Threading.Monitor.Exit(_lockHandler);
                    }
                }
            }
        }

        #endregion
    }
}
