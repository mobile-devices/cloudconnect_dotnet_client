using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace WebDemo.Models.Repository
{
    public class NotificationRepository : RepositoryBase
    {

        private const string NOTIFICATION_DB_NAME = "NOTIFICATION";

        public List<Notification> GetLastData(int limit = 1000)
        {
            MongoCollection<Notification> dataDb = Tools.MongoConnector.Instance.DataBaseReadOnly.GetCollection<Notification>(NOTIFICATION_DB_NAME);
            List<Notification> result = dataDb.AsQueryable<Notification>().Take(limit).ToList();
            return result;
        }

        public void Save(List<Notification> data)
        {
            MongoCollection<Notification> dataDb = Tools.MongoConnector.Instance.DataBase.GetCollection<Notification>(NOTIFICATION_DB_NAME);
            dataDb.InsertBatch<Notification>(data);
        }

        public void Save(Notification data)
        {
            MongoCollection<Notification> dataDb = Tools.MongoConnector.Instance.DataBase.GetCollection<Notification>(NOTIFICATION_DB_NAME);
            dataDb.Insert<Notification>(data);
        }
    }
}