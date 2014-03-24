using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MD.CloudConnect.Data;
using Newtonsoft.Json;

namespace MD.CloudConnect
{
    [Serializable]
    public class MDData
    {
        [JsonProperty("meta")]
        public Dictionary<string, string> MetaData { get; set; }

        private Meta _meta = null;
        public Meta Meta
        {
            get
            {
                if (_meta == null && this.MetaData != null)
                {
                    _meta = new Meta();
                    if (this.MetaData.ContainsKey("account"))
                        _meta.Account = (string)this.MetaData["account"];
                    if (this.MetaData.ContainsKey("event"))
                        _meta.Event = (string)this.MetaData["event"];
                }
                return _meta;
            }
            set { _meta = value; }

        }

        [JsonIgnore]
        public DateTime DateOfData
        {
            get
            {
                if (Meta != null)
                {
                    if (Meta.Event == "track")
                    {
                        if (this.Tracking != null && this.Tracking.Recorded_at != null)
                            return this.Tracking.Recorded_at;
                        else return DateTime.UtcNow;
                    }
                    else if (Meta.Event == "message")
                    {
                        if (this.Message != null && this.Message.Recorded_at != null)
                            return this.Message.Recorded_at.Value;
                        else return DateTime.UtcNow;
                    }
                }
                return DateTime.MinValue;
            }
        }

        [JsonProperty("payload")]
        public Newtonsoft.Json.Linq.JObject Payload { get; set; }

        private TrackingData _tracking = null;
        [JsonIgnore]
        public ITracking Tracking
        {
            get
            {
                if (Meta != null && Meta.Event == "track" && Payload != null)
                {
                    if (_tracking == null)
                        _tracking = JsonConvert.DeserializeObject<TrackingData>(Payload.ToString());
                }
                return _tracking;
            }
        }

        private CollectionData _collection = null;
        [JsonIgnore]
        public ICollection Collection
        {
            get
            {
                if (Meta != null && Meta.Event == "collection" && Payload != null)
                {
                    if (_collection == null)
                        _collection = JsonConvert.DeserializeObject<CollectionData>(Payload.ToString());
                }
                return _collection;
            }
        }


        private MessageData _message = null;
        [JsonIgnore]
        public IMessage Message
        {
            get
            {
                if (Meta != null && Meta.Event == "message" && Payload != null)
                {
                    if (_message == null)
                        _message = JsonConvert.DeserializeObject<MessageData>(Payload.ToString());
                }
                return _message;
            }
        }
        [JsonIgnore]
        public bool ShouldBeIgnore { get; set; }
    }
}
