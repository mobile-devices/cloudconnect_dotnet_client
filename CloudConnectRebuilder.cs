using MD.CloudConnect.CacheProvider;
using MD.CloudConnect.Data;
using MD.CloudConnect.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MD.CloudConnect
{
    public class CloudConnectRebuilder
    {
        private string[] _fieldsUse = null;
        private IDataCache _dataCache = null;
        private ITrackingCacheProvider _trackingCacheProvider = null;
        private bool _autoFilter = true;

        #region singleton
        protected static readonly CloudConnectRebuilder _instance = new CloudConnectRebuilder();
        public static CloudConnectRebuilder Instance
        {
            get
            {
                lock (_instance)
                {
                    return _instance;
                }
            }
        }

        static CloudConnectRebuilder()
        {

        }
        #endregion

        public void Initialize(string[] fieldsName, IDataCache dataCache)
        {
            if (fieldsName != null && fieldsName.Length > 0)
            {
                _fieldsUse = fieldsName;
                _dataCache = dataCache;
            }

            _trackingCacheProvider = new InMemory();
        }

        public List<MDData> DecodeNotificationData(IEnumerable<INotificationData> data)
        {
            Dictionary<string, ITracking> cache = new Dictionary<string, ITracking>();
            System.Collections.Generic.SortedDictionary<string, MDData> cacheTrack = new SortedDictionary<string, MDData>();
            List<MDData> result = new List<MDData>();
            int anomaly = 0;

            foreach (INotificationData notification in data)
            {
                List<MDData> tmp = JsonConvert.DeserializeObject<List<MDData>>(notification.Data);
                foreach (MDData d in tmp)
                {
                    d.NotificationID = notification.Id;
                    if (d.Meta.Event == "track")
                    {
                        if (d.Tracking.ConnectionId.HasValue)
                        {
                            string key = String.Format("{0}-{1}-{2:0000}", d.Tracking.Asset, d.Tracking.ConnectionId, d.Tracking.Index);
                            if (!cacheTrack.ContainsKey(key))
                                cacheTrack.Add(key, d);
                            else
                                anomaly++;
                        }
                        else
                        {
                            Console.WriteLine("test");
                        }
                    }
                }
            }

            ITracking assetHistory = null;
            foreach (KeyValuePair<string, MDData> item in cacheTrack)
            {
                if (!cache.ContainsKey(item.Value.Tracking.Asset))
                {
                    assetHistory = _trackingCacheProvider.FindTrackingCache(item.Value.Tracking.Asset);

                    if (assetHistory == null)
                    {
                        assetHistory = GeneratePayload(item.Value.Tracking.Asset, _fieldsUse);
                    }
                    _dataCache.getHistoryFor(item.Value.Tracking.Asset, assetHistory);
                    cache.Add(item.Value.Tracking.Asset, assetHistory);
                }
                else
                    assetHistory = cache[item.Value.Tracking.Asset];

                if (canBeRebuild(assetHistory, item.Value.Tracking))
                {
                    UpdateStatusNotificationData(1, item.Value.NotificationID, data);
                    result.Add(item.Value);
                    cache[item.Value.Tracking.Asset] = item.Value.Tracking;
                }
                else
                {
                    Console.WriteLine("test");
                    //waiting more data
                }
            }

            RebuildTrackingData(result);

            return result;
        }

        private void UpdateStatusNotificationData(int status, string id, IEnumerable<INotificationData> data)
        {
            INotificationData d = data.Where(x => x.Id == id).First();
            d.Status = status;
        }

        private void RebuildTrackingData(List<MDData> alldata)
        {
            if (_fieldsUse != null && _fieldsUse.Length > 0)
            {
                TrackingData history = null;

                foreach (MD.CloudConnect.MDData data in alldata)
                {
                    if (data.Meta.Event == "track")
                    {
                        ITracking track = (ITracking)data.Tracking;
                        history = _trackingCacheProvider.FindTrackingCache(track.Asset);

                        if (history == null)
                        {
                            history = GeneratePayload(track.Asset, _fieldsUse);
                            if (_dataCache != null)
                                history.Recorded_at = _dataCache.getHistoryFor(track.Asset, (ITracking)history);
                            _trackingCacheProvider.AddTrackingCache(track.Asset, history);
                        }

                        if (track.Recorded_at.Ticks > history.Recorded_at.Ticks && track.Recorded_at.Ticks <= track.Received_at.Ticks)
                        {
                            FillTrackingDataUserChoice(track, history);
                            history.Recorded_at = track.Recorded_at;
                        }
                        else if (_autoFilter)
                        {
                            data.Tracking.Status = 1;
                        }
                        else
                            FillTrackingDataUserChoice(track, history, false);

                        _trackingCacheProvider.UpdateTrackingCache(track.Asset, history);
                    }
                }
            }
        }

        private bool canBeRebuild(ITracking previous, ITracking current)
        {
            bool result = false;
            if (current.ConnectionId.HasValue && previous.ConnectionId.HasValue)
            {
                if (current.ConnectionId.Value == previous.ConnectionId.Value)
                {
                    result = current.Index.Value == (previous.Index + previous.Fields.Count + 1);
                }
                else
                {
                    result = current.Index.Value == 1;
                }
            }
            else return true;

            if (!result)
                Console.WriteLine("test");
            return result;
        }

        private void FillTrackingDataUserChoice(ITracking data, TrackingData history, bool updateCache = true)
        {
            foreach (string field in _fieldsUse)
            {

                if (data.ContainsField(field))
                {
                    if (updateCache)
                    {
                        UpdateCache(history, field, data);
                    }
                }
                else
                {
                    if (history.Fields.ContainsKey(field))
                        ((TrackingData)data).Fields.Add(field, history.Fields[field]);
                    else
                        ((TrackingData)data).Fields.Add(field, new Field());
                }
            }

            if (((TrackingData)data).location != null)
            {
                history.location[0] = data.Longitude;
                history.location[1] = data.Latitude;
            }
        }

        private void UpdateCache(TrackingData history, string field, ITracking data)
        {
            if (!history.Fields.ContainsKey(field))
                history.Fields.Add(field, ((TrackingData)data).Fields[field]);
            else
                history.Fields[field] = ((TrackingData)data).Fields[field];
        }

        private TrackingData GeneratePayload(string asset, string[] fields)
        {
            TrackingData result = new TrackingData();

            result.Asset = asset;
            result.Fields = new Dictionary<string, Field>();
            foreach (string field in fields)
            {
                result.Fields.Add(field, new Field());
            }

            result.location = new double[2];

            return result;
        }
    }
}
