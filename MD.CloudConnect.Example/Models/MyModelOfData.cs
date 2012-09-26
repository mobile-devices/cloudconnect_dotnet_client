using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MD.CloudConnect.Example.Models
{
    public class MyModelOfData
    {
        public string Asset { get; set; }
        public DateTime Date { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }

        public bool IsValid { get; set; }
        public bool Ignition { get; set; }
        public double Speed { get; set; }
        public double Direction { get; set; }

        public int TotalOdometer { get; set; }
        public int DrivingJourney { get; set; }
        public int IdleJourney { get; set; }
        public int JourneyTime { get; set; }
    }
}