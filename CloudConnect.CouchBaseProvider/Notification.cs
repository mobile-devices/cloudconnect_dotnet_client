using CouchbaseModelViews.Framework.Attributes;
using MD.CloudConnect;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudConnect.CouchBaseProvider
{
    [CouchbaseDesignDoc("notifications")]
    public class Notification : IModelBase, INotificationData
    {
        [JsonProperty("created_at")]
        public DateTime Created_at { get; set; }
        [CouchbaseViewKey("by_dropped_and_received_and_key", "received_at", 1)]
        [JsonProperty("received_at")]
        public long Received_at { get; set; }
        [CouchbaseViewKey("by_dropped_and_received_and_key", "key", 2)]
        [JsonProperty("key")]
        public string Key { get; set; }
        [JsonProperty("data")]
        public string Data { get; set; }

        [CouchbaseViewKey("by_dropped_and_received_and_key", "dropped", 0)]
        [JsonProperty("dropped")]
        public string Converted_Dropped
        {
            get;
            set;
        }

        [JsonIgnore]
        public bool Dropped
        {
            get
            {
                return Converted_Dropped == "true";
            }
            set
            {
                Converted_Dropped = (value ? "true" : "false");
            }
        }



        public string Id { get; set; } 

        public string Type
        {
            get { return "notification"; }
        }
    }
}
