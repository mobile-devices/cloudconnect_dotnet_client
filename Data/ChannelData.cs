using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MD.CloudConnect.Data
{
    public class ChannelData
    {
        public string Name { get; set; }
        public int? MsgTtl { get; set; }
        public string ServerAck { get; set; }
        public int? UnitAck { get; set; }
    }
}
