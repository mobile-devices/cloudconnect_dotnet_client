using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MD.CloudConnect.Example.Tools
{
    public class MyDataCacheRepository : MD.CloudConnect.IDataCache
    {
         protected static readonly MyDataCacheRepository _instance = new MyDataCacheRepository();
        public static MyDataCacheRepository Instance
        {
            get
            {
                lock (_instance)
                {
                    return _instance;
                }
            }
        }

        static MyDataCacheRepository()
        {

        }

        public DateTime getHistoryFor(string asset, ITracking data)
        {
            return DateTime.MinValue;
        }
    }
}