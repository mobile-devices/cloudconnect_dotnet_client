using MD.CloudConnect.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudConnect.MongoProvider
{
    public class MongoProvider : ITrackingCacheProvider, INotificationCacheProvider
    {
        public MongoProvider(string connection)
        {

        }

        public MD.CloudConnect.Data.TrackingData FindTrackingCache(string asset)
        {
            return null;
        }

        public void AddTrackingCache(string asset, MD.CloudConnect.Data.TrackingData data)
        {
           
        }

        public void UpdateTrackingCache(string asset, MD.CloudConnect.Data.TrackingData data)
        {
           
        }

        public void PushNotificationCache(string key, string jsonData, DateTime recorded_date)
        {
            
        }

        public IDictionary<DateTime, string> RequestNotificationCache(string key, DateTime max_date)
        {
            return new Dictionary<DateTime, string>();
        }

        public void DropNotificationCache(string key, DateTime max_date)
        {
           
        }
    }
}
