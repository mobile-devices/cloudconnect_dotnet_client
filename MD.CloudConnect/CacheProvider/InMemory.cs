using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MD.CloudConnect.Interface;
using MD.CloudConnect.Data;

namespace MD.CloudConnect.CacheProvider
{
    public class InMemory : ICacheProvider
    {
        private Dictionary<string, TrackingData> _fieldsCache = new Dictionary<string, TrackingData>();

        public Data.TrackingData findCache(string asset)
        {
            TrackingData data = null;
            if (_fieldsCache.ContainsKey(asset))
                data = _fieldsCache[asset];
            return data;
        }

        public void AddCache(string asset, Data.TrackingData data)
        {
            _fieldsCache.Add(asset, data);
        }

        public void UpdateCache(string asset, Data.TrackingData data)
        {
            if (_fieldsCache.ContainsKey(asset))
                _fieldsCache[asset] = data;
        }
    }
}
