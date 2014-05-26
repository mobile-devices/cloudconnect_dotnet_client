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

        private DateTime _dateOfData = DateTime.MinValue;
        private UInt64 _idOfData = 0;
        [JsonIgnore]
        public UInt64 IdOfData
        {
            get
            {
                if (Meta != null)
                {
                    if (_idOfData == 0)
                    {
                        // TODO : find a way to optimize this
                        if (Meta.Event == "track")
                        {
                            if (this.Tracking != null &&  !String.IsNullOrEmpty(this.Tracking.Id_str))
                                 _idOfData =  this.Tracking.Id;
                        }
                        else if (Meta.Event == "message")
                        {
                            if (this.Message != null &&  !String.IsNullOrEmpty(this.Message.Id_str))
                                 _idOfData =  this.Message.Id;
                        }
                        else if (Meta.Event == "presence")
                        {
                            if (this.Presence != null &&  !String.IsNullOrEmpty(this.Presence.Id_str))
                                 _idOfData =  this.Presence.Id;
                        }
                        else if (Meta.Event == "collection")
                        {
                            if (this.Collection != null && !String.IsNullOrEmpty(this.Collection.Id_str))
                                _idOfData =  this.Collection.Id;
                        }
                    }
                }
                return _idOfData;
            }
        }

        [JsonIgnore]
        public DateTime DateOfData
        {
            get
            {
                if (Meta != null)
                {
                    if (_dateOfData == DateTime.MinValue)
                    {
                        // TODO : find a way to optimize this
                        if (Meta.Event == "track")
                        {
                            if (this.Tracking != null)
                                return this.Tracking.Recorded_at;
                            else return DateTime.UtcNow;
                        }
                        else if (Meta.Event == "message")
                        {
                            if (this.Message != null && this.Message.Recorded_at.HasValue)
                                return this.Message.Recorded_at.Value;
                            else return DateTime.UtcNow;
                        }
                        else if (Meta.Event == "presence")
                        {
                            if (this.Presence != null && this.Presence.Time.HasValue)
                                return this.Presence.Time.Value;
                            else return DateTime.UtcNow;
                        }
                        else if (Meta.Event == "collection")
                        {
                            if (this.Collection != null && this.Collection.Start_at.HasValue)
                                return this.Collection.Start_at.Value;
                            else return DateTime.UtcNow;
                        }
                    }
                }
                return _dateOfData;
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

        private PresenceData _presence = null;
        [JsonIgnore]
        public IPresence Presence
        {
            get
            {
                if (Meta != null && Meta.Event == "presence" && Payload != null)
                {
                    if (_presence == null)
                        _presence = JsonConvert.DeserializeObject<PresenceData>(Payload.ToString());
                }
                return _presence;
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
