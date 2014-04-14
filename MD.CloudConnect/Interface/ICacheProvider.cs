using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MD.CloudConnect.Data;

namespace MD.CloudConnect.Interface
{
    public interface ICacheProvider
    {
        TrackingData findCache(string asset);
        void AddCache(string asset, TrackingData data);
        void UpdateCache(string asset, TrackingData data);
    }
}
