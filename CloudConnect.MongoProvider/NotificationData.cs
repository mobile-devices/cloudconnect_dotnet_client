using MD.CloudConnect;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudConnect.MongoProvider
{
    internal class NotificationData : INotificationData
    {
        public ObjectId Id { get; set; }
        public long Received_at { get; set; }
        public string Key { get; set; }
        public string Content { get; set; }
        public bool Dropped { get; set; }
    }
}
