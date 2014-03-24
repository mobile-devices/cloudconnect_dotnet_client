using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MD.CloudConnect.Data
{
    public class CollectionData : ICollection
    {
        public DateTime? Start_at { get; set; }

        public DateTime? Stop_at { get; set; }

        public string Name { get; set; }

        public string Asset { get; set; }

        public string Id { get; set; }

        public string Id_str { get; set; }

        public List<MD.CloudConnect.Data.TrackingData> Tracks { get; set; }

        public List<MD.CloudConnect.Data.MessageData> Messages { get; set; }
    }
}
