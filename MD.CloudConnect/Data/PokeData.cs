using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MD.CloudConnect.Data
{
    public class PokeData
    {
        public UInt64 Id { get; set; }
        public string Asset { get; set; }
        public string b64_payload { get; set; }
        public string Sender { get; set; }
        public string Namespace { get; set; }
    }
}
