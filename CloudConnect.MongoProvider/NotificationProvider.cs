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

        public void PushNotificationCache(string key, string data, DateTime recorded_date)
        {
            MongoCollection<NotificationData> dataDb = _dataBase.GetCollection<NotificationData>(NOTIFICATION_DB_NAME);
            dataDb.Insert<NotificationData>(new NotificationData()
            {
                Data = data,
                Received_at = recorded_date.Ticks,
                Key = key,
                CreatedAt = DateTime.UtcNow,
                Dropped = false
            });
        }

        public IEnumerable<INotificationData> RequestNotificationCache(string key, DateTime max_date)
        {
            MongoCollection<NotificationData> dataDb = _dataBase.GetCollection<NotificationData>(NOTIFICATION_DB_NAME);
            IEnumerable<INotificationData> result = dataDb.AsQueryable<NotificationData>().Where(x => x.Key == key && x.Received_at <= max_date.Ticks && x.Dropped == false).OrderBy(x => x.Received_at).Take(10000);
                //.OrderBy(x => x.Id).Take(10);
      
            return result;
        }

        public void DropNotificationCache(string key, DateTime max_date)
        {
            MongoCollection<NotificationData> dataDb = _dataBase.GetCollection<NotificationData>(NOTIFICATION_DB_NAME);
            List<NotificationData> data = dataDb.AsQueryable<NotificationData>().Where(x => x.Key == key && x.Received_at <= max_date.Ticks && x.Dropped == false).OrderBy(x => x.Received_at).Take(10000).ToList();
               //.OrderBy(x => x.Id).Take(10).ToList();
           
            foreach (NotificationData d in data)
            {
                d.Dropped = true;
                dataDb.Save(d);
            }

            //var query = Query.And(Query.EQ("Key", key), Query.LTE("Created_at", max_date), Query.EQ("Dropped", false));
            //var update = Update.Set("Dropped", true);

            //var options = new MongoUpdateOptions { Flags = UpdateFlags.Multi };
            //dataDb.Update(query, update, options);
        }

        public List<string> GetAssetsForGroup(string groupName)
        {
            return new List<string>();
        }

        public void SetAssetsForGroup(string groupName, List<string> assets)
        {

        }

        public int SizeOfCache()
        {
            MongoCollection<NotificationData> dataDb = _dataBase.GetCollection<NotificationData>(NOTIFICATION_DB_NAME);
            return dataDb.AsQueryable<NotificationData>().Where(x => x.Dropped == false).Count();
        }
    }
}
