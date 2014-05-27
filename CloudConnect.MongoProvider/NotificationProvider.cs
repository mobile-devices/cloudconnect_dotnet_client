using MD.CloudConnect.Interface;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver.Linq;
using MongoDB.Driver.Builders;
using MD.CloudConnect;

namespace CloudConnect.MongoProvider
{
    public class NotificationProvider : INotificationCacheProvider
    {
        private MongoDatabase _dataBase;
        private const string NOTIFICATION_DB_NAME = "NOTIFICATION";


        public NotificationProvider(string connection, string dataBaseName)
        {
            var client = new MongoClient(connection);
            var server = client.GetServer();
            _dataBase = server.GetDatabase(dataBaseName);
        }

        public void PushNotificationCache(string key, string jsonData, DateTime recorded_date)
        {
            MongoCollection<NotificationData> dataDb = _dataBase.GetCollection<NotificationData>(NOTIFICATION_DB_NAME);
            dataDb.Insert<NotificationData>(new NotificationData()
            {
                Content = jsonData,
                Received_at = recorded_date.Ticks,
                Key = key,
                Dropped = false
            });
        }

        public IEnumerable<INotificationData> RequestNotificationCache(string key, DateTime max_date)
        {
            MongoCollection<NotificationData> dataDb = _dataBase.GetCollection<NotificationData>(NOTIFICATION_DB_NAME);
            IEnumerable<INotificationData> result = dataDb.AsQueryable<NotificationData>().Where(x => x.Key == key && x.Received_at <= max_date.Ticks && x.Dropped == false).OrderBy(x => x.Id).Take(5000);
            return result;
        }

        public void DropNotificationCache(string key, DateTime max_date)
        {
            MongoCollection<NotificationData> dataDb = _dataBase.GetCollection<NotificationData>(NOTIFICATION_DB_NAME);
            List<NotificationData> data = dataDb.AsQueryable<NotificationData>().Where(x => x.Key == key && x.Received_at <= max_date.Ticks && x.Dropped == false).OrderBy(x => x.Id).Take(5000).ToList();

            foreach(NotificationData d in data)
            {
                d.Dropped = true;
                dataDb.Save(d);
            }
            // var query = Query.And(Query.LTE("Created_at", max_date), Query.EQ("Dropped", false));
            // var update = Update.Set("Dropped", true);
            // dataDb.Update(query,  update);
        }

        public int SizeOfCache()
        {
            MongoCollection<NotificationData> dataDb = _dataBase.GetCollection<NotificationData>(NOTIFICATION_DB_NAME);
            return dataDb.AsQueryable<NotificationData>().Where(x => x.Dropped == false).Count();
        }
    }
}
