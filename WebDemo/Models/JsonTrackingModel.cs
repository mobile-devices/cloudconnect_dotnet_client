using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebDemo.Models
{
    public class JsonFieldModel
    {
        public string Key { get; set; }
        public string DisplayName { get; set; }
        public string Value { get; set; }
    }

    public class JsonTrackingModel
    {
        public int Id { get; set; }
        public string DocId { get; set; }
        public string Recorded_at { get; set; }
        public string Received_at { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }

        public List<JsonFieldModel> Fields { get; set; }
    }
}