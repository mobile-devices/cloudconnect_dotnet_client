using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MD.CloudConnect.Example.Models
{
    public class MyJsonTrackingData
    {
        public string Recorded_at { get; set; }
        public string Received_at { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public bool IsValid { get; set; }
        public double Speed { get; set; }
    }

}