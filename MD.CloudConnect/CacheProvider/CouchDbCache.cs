using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MD.CloudConnect.Interface;

namespace MD.CloudConnect.CacheProvider
{
    public class CouchDbCache : ICacheProvider
    {
        public Data.TrackingData findCache(string asset)
        {
            throw new NotImplementedException();
        }

        public void AddCache(string asset, Data.TrackingData data)
        {
            throw new NotImplementedException();
        }

        public void UpdateCache(string asset, Data.TrackingData data)
        {
            throw new NotImplementedException();
        }
    }
}
