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
        private Dictionary<string, Dictionary<DateTime, string>> _notificationCache = new Dictionary<string, Dictionary<DateTime, string>>();

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
            if (!_notificationCache.ContainsKey(key))
                _notificationCache.Add(key, new Dictionary<DateTime, string>());
            _notificationCache[key].Add(recorded_date, jsonData);
        }

        public IDictionary<DateTime, string> RequestNotificationCache(string key, DateTime max_date)
        {
            Dictionary<DateTime, string> result = new Dictionary<DateTime, string>();
            if (_notificationCache.ContainsKey(key))
            {
                foreach (KeyValuePair<DateTime, string> item in _notificationCache[key])
                {
                    if (item.Key <= max_date)
                        result.Add(item.Key, item.Value);
                }
            }
            return result;
        }

        public void DropNotificationCache(string key, DateTime max_date)
        {
            if (_notificationCache.ContainsKey(key))
            {
                DateTime[] dates = _notificationCache[key].Keys.Where(k => k <= max_date).ToArray();
                foreach (DateTime d in dates)
                    _notificationCache[key].Remove(d);
            }
        }
    }
}
