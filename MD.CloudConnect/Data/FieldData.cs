using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MD.CloudConnect.Data
{
    public class FieldData
    {
        public string Field { get; set; }
        public string Name { get; set; }
        public int Size { get; set; }
        public string FieldType { get; set; }
        public int Ack { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
    }
}
