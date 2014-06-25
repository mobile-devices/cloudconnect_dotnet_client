using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudConnect.CouchBaseProvider
{
    public class Field
    {
        public string Key { get; set; }
        public string B64Value { get; set; }

        public int IntegerValue { get; set; }
        public bool BooleanValue { get; set; }
        public string StringValue { get; set; }
    }
}
