using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using WebDemo.Tools;

namespace WebDemo.Models.Repository
{
    public class DataCollectionDb : RepositoryBase
    {
        private const string COLLECTION_DB_NAME = "COLLECTION";

        public List<CollectionModel> GetData(DeviceModel device, DateTime date)
        {
            MongoCollection<CollectionModel> dataDb = Tools.MongoConnector.Instance.DataBaseReadOnly.GetCollection<CollectionModel>(COLLECTION_DB_NAME);

            List<CollectionModel> result = (from d in dataDb.AsQueryable<CollectionModel>()
                                          where d.DeviceID == device.Id && d.StartDateKey == date.GenerateKey()
                                          select d).ToList();

            return result;
        }

        public void Save(List<CollectionModel> data)
        {
            MongoCollection<CollectionModel> dataDb = Tools.MongoConnector.Instance.DataBase.GetCollection<CollectionModel>(COLLECTION_DB_NAME);
            dataDb.InsertBatch<CollectionModel>(data);
        }
    }
}