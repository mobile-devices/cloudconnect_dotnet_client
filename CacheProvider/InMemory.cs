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
        private List<InMemoryNotificationData> _notificationCache = new List<InMemoryNotificationData>();
        private Dictionary<string, List<string>> _groupCache = new Dictionary<string, List<string>>();

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

        public void PushNotificationCache(string key, string data, DateTime recorded_date)
        {
            _notificationCache.Add(new InMemoryNotificationData()
            {
                Data = data,
                Received_at = recorded_date.Ticks,
                Key = key
            });
        }

        public List<string> GetAssetsForGroup(string groupName)
        {
            if(_groupCache.ContainsKey(groupName))
                return _groupCache[groupName];
            return new List<string>();
        }

        public void SetAssetsForGroup(string groupName, List<string> assets)
        {
            if (_groupCache.ContainsKey(groupName))
                _groupCache[groupName] = assets;
            else
                _groupCache.Add(groupName, assets);
        }

        public IEnumerable<INotificationData> RequestNotificationCache(string key, DateTime max_date)
        {
            List<INotificationData> result = new List<INotificationData>();
            if (_notificationCache.Count > 0)
            {
                result = (List<INotificationData>)_notificationCache.Where(x => x.Key == key && x.Received_at <= max_date.Ticks);
            }
            return result;
        }

        public void DropNotificationCache(string key, DateTime max_date)
        {
            if (_notificationCache.Count > 0)
            {
                _notificationCache.RemoveAll(x => x.Received_at <= max_date.Ticks && x.Key == key);
            }
        }
    }
}
