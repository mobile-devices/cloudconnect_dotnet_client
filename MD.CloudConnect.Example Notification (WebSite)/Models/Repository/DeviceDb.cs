using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace WebDemo.Models.Repository
{
    public class DeviceDb : RepositoryBase
    {
        private const string DEVICE_KEY = "DEVICES_DB";
        private const string DEVICE_DB_NAME = "DEVICES";

        public DeviceModel GetDevice(string imei)
        {
            MongoCollection<DeviceModel> devicesDB = Tools.MongoConnector.Instance.DataBaseReadOnly.GetCollection<DeviceModel>(DEVICE_DB_NAME);

            DeviceModel result = (from d in devicesDB.AsQueryable<DeviceModel>()
                                  where d.Imei == imei
                                  select d).FirstOrDefault();

            return result;
        }

        public void Save(DeviceModel device)
        {
            MongoCollection<DeviceModel> devicesDB = Tools.MongoConnector.Instance.DataBase.GetCollection<DeviceModel>(DEVICE_DB_NAME);
            devicesDB.Save(device);
        }


        public void Save(List<DeviceModel> device)
        {
            MongoCollection<DeviceModel> devicesDB = Tools.MongoConnector.Instance.DataBase.GetCollection<DeviceModel>(DEVICE_DB_NAME);

            foreach (DeviceModel d in device)
                devicesDB.Save(d);
        }
    }
}