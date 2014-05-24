using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using MD.CloudConnect.Data;
using MD.CloudConnect.Interface;
using MD.CloudConnect.CacheProvider;

namespace MD.CloudConnect
{
    internal class DecodedNotificationData
    {
        public List<MDData> Data { get; set; }
        public DateTime DateOfGroup { get; set; }

        private UInt64 _maxId = 0;
        public UInt64 MaxID
        {
            get
            {
                return _maxId;
            }
        }

        //private DateTime _maxRecordedAt = DateTime.MinValue;
        //public DateTime MaxRecorededAt
        //{
        //    get
        //    {
        //        return _maxRecordedAt;
        //    }
        //}

        public DecodedNotificationData(DateTime date, string json)
        {
            Data = JsonConvert.DeserializeObject<List<MDData>>(json);
            DateOfGroup = date;
            foreach (MDData data in Data)
            {
                if (_maxId < data.IdOfData)
                    _maxId = data.IdOfData;
                //if (_maxRecordedAt < data.DateOfData)
                //    _maxRecordedAt = data.DateOfData;
            }
        }
    }

    public class Notification
    {
        private ITrackingCacheProvider _trackingCacheProvider = null;
        private INotificationCacheProvider _notificationCacheProvider = null;
        private string[] _fieldsUse = null;
        private IDataCache _dataCache = null;
        private bool _autoFilter = true;

        private int _maxBufferTime;
        private bool _splitCacheByAsset;
        private Dictionary<int, List<DateTime>> _currentDataInPipe = new Dictionary<int, List<DateTime>>();

        private bool _fixMoving = false;
        //Decimal degrees (11.1 meters)
        private const double MinDistanceToDetectMovement = 0.0001;

        #region singleton
        protected static readonly Notification _instance = new Notification();
        public static Notification Instance
        {
            get
            {
                lock (_instance)
                {
                    return _instance;
                }
            }
        }

        static Notification()
        {

        }
        #endregion

        /// <summary>
        /// You can update data cache manualy with this function. It will be apply after internal process
        /// Your modification will not be overwrite.
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="data"></param>
        public void UpdateManualyDataCache(string asset, string field, ITracking data)
        {
            TrackingData currentHistory = null;

            currentHistory = _trackingCacheProvider.FindTrackingCache(asset);
            if (currentHistory != null)
            {
                UpdateCache(currentHistory, field, data);
                _trackingCacheProvider.UpdateTrackingCache(asset, currentHistory);
            }
        }

        /// <summary>
        /// Prepare notification class with advanced features
        /// </summary>
        /// <param name="fieldsName">List of field name that you want in the data cache (use to recreate data)</param>
        /// <param name="dataCache">Class call one time to load a previous data save in your database</param>
        /// <param name="autoFilter">Activate auto filter function. Remove bad data like data with date in the future or not order</param>
        /// <param name="fixMoving">Activate function to detect when a unit move or not.This information is send by the unit but can be not correct in some case</param>
        public void Initialize(string[] fieldsName, IDataCache dataCache, bool autoFilter = true, bool fixMoving = false)
        {
            _splitCacheByAsset = false;
            _maxBufferTime = 5;
            if (fieldsName != null && fieldsName.Length > 0)
            {
                _fieldsUse = fieldsName;
                _dataCache = dataCache;
            }

            _trackingCacheProvider = new InMemory();
            _notificationCacheProvider = new InMemory();

            _autoFilter = autoFilter;
            _fixMoving = fixMoving;
        }

        public void Initialize(string[] fieldsName, bool autoFilter = true, bool fixMoving = false
            , ITrackingCacheProvider trackingCacheProvider = null, INotificationCacheProvider notificationCacheProvider = null
            , int maxBufferTime = 5, bool splitCacheByAsset = false)
        {
            _splitCacheByAsset = splitCacheByAsset;
            _maxBufferTime = maxBufferTime;
            if (fieldsName != null && fieldsName.Length > 0)
            {
                _fieldsUse = fieldsName;
                _dataCache = null;
            }

            if (trackingCacheProvider == null)
                _trackingCacheProvider = new InMemory();
            if (notificationCacheProvider == null)
                _notificationCacheProvider = new InMemory();

            _autoFilter = autoFilter;
            _fixMoving = fixMoving;
        }


        public void PrepareDataToDecode(string jsonData)
        {
            if (_splitCacheByAsset)
            {
                List<MDData> datas = JsonConvert.DeserializeObject<List<MDData>>(jsonData);
                if (datas != null && datas.Count > 0)
                {
                    //TODO
                }
            }
            else
                _notificationCacheProvider.PushNotificationCache("GEN_NOTIF", jsonData, DateTime.UtcNow);
        }

        public int AsyncDecode(out List<MDData> data, List<string> assets = null)
        {
            List<DateTime> dates = new List<DateTime>();
            data = new List<MDData>();
            int hash = 0;
            if (_splitCacheByAsset && assets != null)
            {
                data = new List<MDData>();
                foreach (string asset in assets)
                {

                }
            }
            else
            {
                // We have the current buffer of X second + 1 buffer . If the second buffer is full (X sec) we can decode the first one
                DateTime maxTimeLimit = DateTime.UtcNow.AddSeconds(-(_maxBufferTime * 2));
                DateTime timeLimit = DateTime.UtcNow.AddSeconds(-_maxBufferTime);

                IDictionary<DateTime, string> jsonData = _notificationCacheProvider.RequestNotificationCache("GEN_NOTIF", maxTimeLimit);
                List<DecodedNotificationData> decodedData = new List<DecodedNotificationData>();

                if (jsonData.Keys.LastOrDefault() > timeLimit)
                {
                    //step 1 : new Class with 2 important things = DateTime notification and MaxID in Json
                    foreach (KeyValuePair<DateTime, string> d in jsonData)
                    {
                        decodedData.Add(new DecodedNotificationData(d.Key, d.Value));
                    }

                    //step 2 : order by ID (because IDs are always generate by the cloud in a correct order so it's a good way to re-order received data)
                    decodedData.OrderBy(d => d.MaxID);

                    //step 3 : consider K-sort constraint, we can only decode data from the current buffer limit by the Max time size
                    foreach(DecodedNotificationData newData in decodedData)
                    {
                        if (newData.DateOfGroup <= timeLimit)
                            data.AddRange(newData.Data);
                    }

                    hash = data.GetHashCode();
                }
            }
            _currentDataInPipe.Add(hash, dates);

            return hash;
        }

        public void AckDecodedData(int hashId)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        public List<MDData> Decode(string jsonData)
        {
            List<MDData> data = JsonConvert.DeserializeObject<List<MDData>>(jsonData);
            if (data != null && data.Count > 0)
            {
                data = data.OrderBy(x => x.DateOfData).ToList();
                RebuildTrackingDate(data);
                return data.Where(x => !x.ShouldBeIgnore).ToList();
            }
            else return new List<MDData>();
        }

        private void RebuildTrackingDate(List<MDData> alldata)
        {
            if (_dataCache != null && _fieldsUse != null && _fieldsUse.Length > 0)
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
                            history.Recorded_at = _dataCache.getHistoryFor(track.Asset, (ITracking)history);
                            _trackingCacheProvider.AddTrackingCache(track.Asset, history);
                        }

                        if (track.Recorded_at.Ticks > history.Recorded_at.Ticks && track.Recorded_at.Ticks <= track.Received_at.Ticks)
                        {
                            FillTrackingDataUserChoice(track, history);
                            history.Recorded_at = track.Recorded_at;
                        }
                        else if (_autoFilter)
                            data.ShouldBeIgnore = true;
                        else
                            FillTrackingDataUserChoice(track, history, false);

                        _trackingCacheProvider.UpdateTrackingCache(track.Asset, history);
                    }
                }
            }
        }

        private void FillTrackingDataUserChoice(ITracking data, TrackingData history, bool updateCache = true)
        {
            bool fixMovementValue = false;

            foreach (string field in _fieldsUse)
            {
                /* Fix Movement before to update data cache */
                if (field == MD.CloudConnect.FieldDefinition.MVT_STATE.Key && _fixMoving)
                {
                    fixMovementValue = IsInMovement(data, history);

                    if (data.ContainsField(MD.CloudConnect.FieldDefinition.MVT_STATE.Key))
                        data.IsMoving = fixMovementValue;
                    else if (history.ContainsField(MD.CloudConnect.FieldDefinition.MVT_STATE.Key))
                        history.IsMoving = fixMovementValue;
                }
                /* End Fix Movement*/

                if (data.ContainsField(field))
                {
                    if (updateCache)
                    {
                        UpdateCache(history, field, data);
                    }
                }
                else
                {
                    if (history.fields.ContainsKey(field))
                        ((TrackingData)data).fields.Add(field, history.fields[field]);
                    else
                        ((TrackingData)data).fields.Add(field, new Field());
                }
            }

            if (((TrackingData)data).location != null)
            //{
            //    ((TrackingData)data).location = (double[])history.location.Clone();
            //}
            //else
            {
                history.location[0] = data.Longitude;
                history.location[1] = data.Latitude;
            }
        }

        private bool MinDistanceDetected(ITracking p1, ITracking p2)
        {
            if (p2.Longitude - p1.Longitude > MinDistanceToDetectMovement)
                return true;
            if (p2.Latitude - p1.Latitude > MinDistanceToDetectMovement)
                return true;

            if (Math.Sqrt(Math.Abs(Math.Pow(p2.Longitude - p1.Longitude, 2) + Math.Pow(p2.Latitude - p1.Latitude, 2))) >= MinDistanceToDetectMovement)
                return true;

            return false;
        }

        private bool IsInMovement(ITracking current, ITracking previous)
        {
            if (current.Longitude != 0.0 && previous.Longitude != 0.0)
            {
                return MinDistanceDetected(previous, current);
            }
            else
            {
                if (current.ContainsField(MD.CloudConnect.FieldDefinition.MVT_STATE.Key))
                    return current.IsMoving;
                else if (current.SpeedKmPerHour > 5.0)
                    return true;
                else
                    return false;
            }
        }

        private void UpdateCache(TrackingData history, string field, ITracking data)
        {
            if (!history.fields.ContainsKey(field))
                history.fields.Add(field, ((TrackingData)data).fields[field]);
            else
                history.fields[field] = ((TrackingData)data).fields[field];
        }

        private TrackingData GeneratePayload(string asset, string[] fields)
        {
            TrackingData result = new TrackingData();

            result.Asset = asset;
            result.fields = new Dictionary<string, Field>();
            foreach (string field in fields)
            {
                result.fields.Add(field, new Field());
            }

            result.location = new double[2];

            return result;
        }
    }
}