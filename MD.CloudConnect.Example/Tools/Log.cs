using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net;
using log4net.Config;
using System.IO;

namespace CloudConnectReader.Tools
{
    public class Log
    {
        private ILog _handler;
        private ILog _general;
        private ILog _notification;

        public ILog Handler
        {
            get
            {
                return _handler;
            }
        }

        public ILog General
        {
            get
            {
                return _general;
            }
        }

        public ILog Notification
        {
            get
            {
                return _notification;
            }
        }

        #region singleton
        protected static readonly Log _instance = new Log();
        public static Log Instance
        {
            get
            {
                lock (_instance)
                {
                    return _instance;
                }
            }
        }

        static Log()
        {

        }
        #endregion

        public void Initialize()
        {
            string configFile = HttpContext.Current.Server.MapPath(@"~\log4net.config");
            XmlConfigurator.Configure(new FileInfo(configFile));

            _general = LogManager.GetLogger("General");
            _handler = LogManager.GetLogger("Handler");
            _notification = LogManager.GetLogger("Notification");
        }
    }
}