using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace MD.CloudConnect.Data
{
    public class CollectionData : ICollection
    {
        public DateTime? Start_at { get; set; }

        public DateTime? Stop_at { get; set; }

        public string Name { get; set; }

        public string Asset { get; set; }

        public UInt64 Id { get; set; }

        public string Id_str { get; set; }

        [JsonProperty("tracks")]
        Newtonsoft.Json.Linq.JArray RawTracks { get; set; }
        private List<TrackingData> _tracks = null;
        [JsonIgnore]
        public IEnumerable<ITracking> Tracks
        {
            get
            {
                if (RawTracks != null && _tracks == null)
                {
                    _tracks = JsonConvert.DeserializeObject<List<TrackingData>>(RawTracks.ToString());
                }
                return _tracks;
            }
        }

        [JsonProperty("messages")]
        public Newtonsoft.Json.Linq.JArray RawMessages { get; set; }

        private List<MessageData> _messages;
        [JsonIgnore]
        public IEnumerable<IMessage> Messages
        {
            get
            {
                if (RawMessages != null && _messages == null)
                {
                    _messages = JsonConvert.DeserializeObject<List<MessageData>>(RawMessages.ToString());
                }
                return _messages;
            }
        }
    }
}
