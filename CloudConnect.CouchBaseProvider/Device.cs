using CouchbaseModelViews.Framework.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudConnect.CouchBaseProvider
{
    [CouchbaseDesignDoc("devices")]
    [CouchbaseAllView]
    public class Device : WebDemo.AbstractModel.Device, IModelBase
    {
        public string Id { get; set; }

        public override String Imei
        {
            get
            {
                return this.Id;
            }
            set
            {
                this.Id = value;
            }
        }

        [JsonProperty("last_report")]
        public override DateTime LastReport { get; set; }
        [JsonProperty("last_valid_location")]
        public override DateTime LastValidLocation { get; set; }
        [JsonProperty("last_longitude")]
        public override double LastLongitude { get; set; }
        [JsonProperty("last_latitude")]
        public override double LastLatitude { get; set; }
        [JsonProperty("created_at")]
        public override DateTime CreatedAt { get; set; }
        [JsonProperty("last_fields")]
        public override Dictionary<string, WebDemo.AbstractModel.Field> LastFields { get; set; }
        [JsonProperty("id_last_task")]
        public override int? IdLastTask { get; set; }

        public string Type
        {
            get { return "Device"; }
        }
    }
}
