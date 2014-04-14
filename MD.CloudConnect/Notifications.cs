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
    public class Notification
    {
        private ICacheProvider _cacheProvider = null;
        private string[] _fieldsUse = null;
        private IDataCache _dataCache = null;
        private bool _autoFilter = true;

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

            currentHistory = _cacheProvider.findCache(asset);
            if (currentHistory != null)
            {
                UpdateCache(currentHistory, field, data);
                _cacheProvider.UpdateCache(asset,currentHistory);
            }
        }

        /// <summary>
        /// Prepare notification class with advanced features
        /// </summary>
        /// <param name="fieldsName">List of field name that you want in the data cache (use to recreate data)</param>
        /// <param name="dataCache">Class call one time to load a previous data save in your database</param>
        /// <param name="autoFilter">Activate auto filter function. Remove bad data like data with date in the future or not order</param>
        /// <param name="fixMoving">Activate function to detect when a unit move or not.This information is send by the unit but can be not correct in some case</param>
        public void Initialize(string[] fieldsName, IDataCache dataCache, bool autoFilter = true, bool fixMoving = false, ICacheProvider cacheProvider = null)
        {
            if (fieldsName != null && fieldsName.Length > 0)
            {
                _fieldsUse = fieldsName;
                _dataCache = dataCache;
            }

            if (cacheProvider == null)
                _cacheProvider = new InMemory();
            _autoFilter = autoFilter;
            _fixMoving = fixMoving;
        }

        public List<MDData> Decode(string jsonData)
        {
            List<MDData> datas = JsonConvert.DeserializeObject<List<MDData>>(jsonData);
            if (datas != null && datas.Count > 0)
            {
                datas = datas.OrderBy(x => x.DateOfData).ToList();

                if (_dataCache != null && _fieldsUse != null && _fieldsUse.Length > 0)
                {
                    TrackingData history = null;

                    foreach (MD.CloudConnect.MDData data in datas)
                    {
                        if (data.Meta.Event == "track")
                        {
                            ITracking track = (ITracking)data.Tracking;
                            history = _cacheProvider.findCache(track.Asset);
                            if (history == null)
                            {
                                history = GeneratePayload(track.Asset, _fieldsUse);
                                history.Recorded_at = _dataCache.getHistoryFor(track.Asset, (ITracking)history);
                                _cacheProvider.AddCache(track.Asset, history);
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

                            _cacheProvider.UpdateCache(track.Asset, history);
                        }
                    }
                }
                return datas.Where(x => !x.ShouldBeIgnore).ToList();
            }
            else return new List<MDData>();
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

            if (((TrackingData)data).location == null)
            {
                ((TrackingData)data).location = (double[])history.location.Clone();
            }
            else
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