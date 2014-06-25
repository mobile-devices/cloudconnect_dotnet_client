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
    [CouchbaseAllView]
    public class NotificationData : ModelBase, INotificationData
    {
        [JsonProperty("created_at")]
        public DateTime Created_at { get; set; }
        [JsonProperty("received_at")]
        public long Received_at { get; set; }

        [CouchbaseViewKey("all_by_key_and_dropped", "key")]
        [JsonProperty("key")]
        public string Key { get; set; }
        [JsonProperty("data")]
        public string Data { get; set; }

        [CouchbaseViewKey("by_dropped", "dropped")]
        [JsonProperty("dropped")]
        public bool Dropped { get; set; }

        public override string Type
        {
            get { return "notification"; }
        }
    }
}
