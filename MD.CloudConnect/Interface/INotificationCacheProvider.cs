using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MD.CloudConnect.Interface
{
    public interface INotificationCacheProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="jsonData"></param>
        void PushNotificationCache(string key, string jsonData, DateTime recorded_date);
         
        IEnumerable<INotificationData> RequestNotificationCache(string key, DateTime max_date);

        void DropNotificationCache(string key , DateTime max_date);
    }
}
