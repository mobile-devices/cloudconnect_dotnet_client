using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudConnect.CouchBaseProvider
{
    public class Device : ModelBase
    {
        [JsonProperty("imei")]
        public String Imei { get; set; }
        [JsonProperty("last_report")]
        public DateTime LastReport { get; set; }
        [JsonProperty("last_valid_location")]
        public DateTime LastValidLocation { get; set; }
        [JsonProperty("last_longitude")]
        public double LastLongitude { get; set; }
        [JsonProperty("last_latitude")]
        public double LastLatitude { get; set; }
        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }
        [JsonProperty("last_fields")]
        public Dictionary<string, Field> LastFields { get; set; }
        [JsonProperty("id_last_task")]
        public int? IdLastTask { get; set; }

        public override string Type
        {
            get { return "Device"; }
        }
    }
}
