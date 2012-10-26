using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using MD.CloudConnect.Data;

namespace MD.CloudConnect
{
    public class Notification
    {
        private Dictionary<string, TrackingData> _fieldsCache = new Dictionary<string, TrackingData>();
        private string[] _fieldsUse = null;
        private IDataCache _dataCache = null;
        private bool _autoFilter = true;

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

        public void Initialize(string[] fieldsName, IDataCache dataCache, bool autoFilter = true)
        {
            if (fieldsName != null && fieldsName.Length > 0)
            {
                _fieldsCache.Clear();
                _fieldsUse = fieldsName;
                _dataCache = dataCache;
            }
            _autoFilter = autoFilter;
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
                            if (!_fieldsCache.ContainsKey(track.Asset))
                            {
                                history = GeneratePayload(track.Asset, _fieldsUse);
                                history.Recorded_at = _dataCache.getHistoryFor(track.Asset, (ITracking)history);

                                _fieldsCache.Add(track.Asset, history);
                            }
                            else
                                history = _fieldsCache[track.Asset];

                            if (track.Recorded_at.Ticks > history.Recorded_at.Ticks)
                            {
                                FillTrackingDataUserChoice(track, history);
                                history.Recorded_at = track.Recorded_at;
                            }
                            else if (_autoFilter)
                            {
                                data.ShouldBeIgnore = true;
                            }
                        }
                    }
                }
                return datas.Where(x => !x.ShouldBeIgnore).ToList();
            }
            else return new List<MDData>();
        }

        private void FillTrackingDataUserChoice(ITracking data, TrackingData history)
        {
            foreach (string field in _fieldsUse)
            {
                if (data.ContainsField(field))
                {
                    UpdateCache(history, field, data);
                }
                else
                {
                    if (history.fields.ContainsKey(field))
                        ((TrackingData)data).fields.Add(field, history.fields[field]);
                    else
                        ((TrackingData)data).fields.Add(field, new Field());
                }
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
            return result;
        }
    }
}
