using MD.CloudConnect.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MD.CloudConnect.Data
{
    public class PresenceData : IPresence
    {
        public DateTime? Time { get; set; }
        public string Reason { get; set; }
        public string Type { get; set; }
        public string Asset { get; set; }
        public ulong Id { get; set; }
        public string Id_str { get; set; }
    }
}
