using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace WebDemo.Models
{
    public class CollectionModel
    {
        public string Id { get; set; }
        public string DeviceID { get; set; }
        public string Name { get; set; }
        public string Id_str { get; set; }

        public int StartDateKey { get; set; }
        public DateTime? Start_at { get; set; }
        public DateTime? Stop_at { get; set; }

        public MD.CloudConnect.Data.TrackingData[] Tracks { get; set; }
        public MD.CloudConnect.Data.MessageData[] Messages { get; set; }
    }
}