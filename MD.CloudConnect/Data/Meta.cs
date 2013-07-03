using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace MD.CloudConnect
{
    [Serializable]
    public class Meta
    {
        [JsonProperty("account")]
        public string Account { get; set; }
        [JsonProperty("event")]
        public string Event { get; set; }
    }
}
