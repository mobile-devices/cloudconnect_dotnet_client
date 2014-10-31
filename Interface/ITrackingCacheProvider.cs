using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MD.CloudConnect.Data;

namespace MD.CloudConnect.Interface
{
    public interface ITrackingCacheProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        TrackingData FindTrackingCache(string asset);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="data"></param>
        void AddTrackingCache(string asset, TrackingData data);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="data"></param>
        void UpdateTrackingCache(string asset, TrackingData data);
    }
}
