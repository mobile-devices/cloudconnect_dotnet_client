using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using CloudConnectReader.Tools;

namespace CloudConnectReader.Models.Repository
{
    public class DataTrackingDB : RepositoryBase
    {
        private const string TRACKING_DB_NAME = "TRACKING";

        public List<TrackingModel> GetData(DeviceModel device, DateTime date)
        {
            MongoCollection<TrackingModel> dataDb = Tools.MongoConnector.Instance.DataBaseReadOnly.GetCollection<TrackingModel>(TRACKING_DB_NAME);

            List<TrackingModel>  result = (from d in dataDb.AsQueryable<TrackingModel>()
                      where d.DeviceID == device.Id && d.RecordedDateKey == date.GenerateKey()
                      select d).ToList();

            return result;
        }

        public void Save(List<TrackingModel> data)
        {
            MongoCollection<TrackingModel> dataDb = Tools.MongoConnector.Instance.DataBase.GetCollection<TrackingModel>(TRACKING_DB_NAME);
            dataDb.InsertBatch<TrackingModel>(data);
        }
    }
}