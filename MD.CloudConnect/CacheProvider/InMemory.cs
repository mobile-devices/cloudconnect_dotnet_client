using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MD.CloudConnect.Interface;
using MD.CloudConnect.Data;

namespace MD.CloudConnect.CacheProvider
{
    public class InMemory : ITrackingCacheProvider, INotificationCacheProvider
    {
        private Dictionary<string, TrackingData> _fieldsCache = new Dictionary<string, TrackingData>();

        public TrackingData FindTrackingCache(string asset)
        {
            TrackingData data = null;
            if (_fieldsCache.ContainsKey(asset))
                data = _fieldsCache[asset];
            return data;
        }

        public void AddTrackingCache(string asset, TrackingData data)
        {
            _fieldsCache.Add(asset, data);
        }

        public void UpdateTrackingCache(string asset, TrackingData data)
        {
            if (_fieldsCache.ContainsKey(asset))
                _fieldsCache[asset] = data;
        }

        public void PushNotificationCache(string key, string jsonData, DateTime recorded_date)
        {
         
        }

        public IDictionary<DateTime, string> RequestNotificationCache(string key, DateTime max_date)
        {
            return new Dictionary<DateTime, string>();
        }

        public void DropNotificationCache(string key , DateTime max_date)
        {

        }
    }
}
