using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MD.CloudConnect.Data
{
    public class AssetData
    {
        public string Imei { get; set; }
        public string Name { get; set; }
        public string Serial { get; set; }
        public string Description { get; set; }

        public List<MessageData> LoadMessage()
        {
            return null;
        }

        public void SendMessage()
        {

        }

        public List<TrackingData> LoadLastTraking(int page, int per_page)
        {
            return null;
        }
    }
}
