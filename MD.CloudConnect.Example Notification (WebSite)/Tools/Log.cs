using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net;
using log4net.Config;
using System.IO;

namespace WebDemo.Tools
{
    public class Log
    {
        private bool _isReady = false;
        public bool IsReady
        {
            get
            {
                return _isReady;
            }
        }

        private ILog _general;
        private ILog _notification;

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
            _isReady = true;

            string configFile = HttpContext.Current.Server.MapPath(@"~\log4net.config");
            XmlConfigurator.Configure(new FileInfo(configFile));

            _general = LogManager.GetLogger("General");
            _notification = LogManager.GetLogger("Notification");
        }
    }
}