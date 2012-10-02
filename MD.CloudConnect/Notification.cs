using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace MD.CloudConnect
{
    public class Notification
    {
        private Dictionary<string, Payload> _fieldsCache = new Dictionary<string, Payload>();
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
                _fieldsUse = fieldsName;
                _dataCache = dataCache;
            }
            _autoFilter = autoFilter;
        }

        public List<MDData> Decode(string jsonData)
        {
            List<MDData> datas = JsonConvert.DeserializeObject<List<MDData>>(jsonData);
            if (_dataCache != null && _fieldsUse != null && _fieldsUse.Length > 0)
            {
                Payload history = null;

                foreach (MD.CloudConnect.MDData data in datas)
                {
                    if (data.Meta.Event == "track")
                    {
                        ITracking track = (ITracking)data.Tracking;
                        if (!_fieldsCache.ContainsKey(track.Asset))
                        {
                            history = GeneratePayload(track.Asset, _fieldsUse);
                            history.Received_at = _dataCache.getHistoryFor(track.Asset, (ITracking)history);

                            _fieldsCache.Add(track.Asset, history);
                        }

                        if (track.Recorded_at.Ticks > history.Recorded_at.Ticks)
                            FillTrackingDataUserChoice(track, history);
                        else if (_autoFilter)
                        {
                            data.Payload.shouldBeIgnore = true;
                        }
                    }
                }
            }
            return datas.Where(x => !x.Payload.shouldBeIgnore).ToList();
        }

        private void FillTrackingDataUserChoice(ITracking data, Payload history)
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
                        ((Payload)data).fields.Add(field, history.fields[field]);
                    else
                        ((Payload)data).fields.Add(field, new Field());
                }
            }

        }

        private void UpdateCache(Payload history, string field, ITracking data)
        {
            if (!history.fields.ContainsKey(field))
                history.fields.Add(field, ((Payload)data).fields[field]);
            else
                history.fields[field] = ((Payload)data).fields[field];
        }

        private Payload GeneratePayload(string asset, string[] fields)
        {
            Payload result = new Payload();

            result.Asset = asset;
            foreach (string field in fields)
            {
                result.fields.Add(field, new Field());
            }
            return result;
        }
    }
}
