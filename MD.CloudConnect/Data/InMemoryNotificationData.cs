using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MD.CloudConnect.Data
{
    internal class InMemoryNotificationData : INotificationData
    {
        public long Received_at { get; set; }

        public string Key { get; set; }

        public string Content { get; set; }

        public string Group { get; set; }
    }
}
