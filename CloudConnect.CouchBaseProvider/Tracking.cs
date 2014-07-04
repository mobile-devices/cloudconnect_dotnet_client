using CouchbaseModelViews.Framework.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebDemo.AbstractModel;

namespace CloudConnect.CouchBaseProvider
{
    [CouchbaseDesignDoc("tracks")]
    public class Track : WebDemo.AbstractModel.Track, IModelBase
    {
        public string Id { get; set; }
        public string Type
        {
            get { return "Tracking"; }
        }
        [CouchbaseViewKey("all_by_asset_and_datekey", "record_date_key", 1)]
        [JsonProperty("record_date_key")]
        public override int RecordedDateKey { get; set; }
        [JsonProperty("record_date")]
        public override DateTime RecordedDate { get; set; }
        [JsonProperty("create_at")]
        public override DateTime CreatedAt { get; set; }
        [CouchbaseViewKey("all_by_asset_and_datekey", "asset", 0)]
        [JsonProperty("asset")]
        public override string Asset { get; set; }
        [JsonProperty("data")]
        public override WebDemoTrackingData Data { get; set; }
        [JsonProperty("dropped")]
        public override bool Dropped { get; set; }
    }
}
