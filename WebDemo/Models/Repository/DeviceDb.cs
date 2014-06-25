using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Driver.Builders;

namespace WebDemo.Models.Repository
{
    public class DeviceDb : RepositoryBase
    {
        private const string DEVICE_KEY = "DEVICES_DB";
        private const string DEVICE_DB_NAME = "DEVICES";


        private List<DeviceModel> RemoveDuplicateDevices(List<DeviceModel> devices)
        {
            List<string> imeis = new List<string>();
            List<DeviceModel> cleanList = new List<DeviceModel>();

            foreach(DeviceModel d in devices)
            {
                if (!imeis.Contains(d.Imei))
                {
                    cleanList.Add(d);
                    imeis.Add(d.Imei);
                }
                else
                {
                    MongoCollection<DeviceModel> devicesDB = Tools.MongoConnector.Instance.DataBase.GetCollection<DeviceModel>(DEVICE_DB_NAME);       
                    devicesDB.Remove(Query.EQ("_id", d.Id));
                }
            }
            return cleanList;
        }

        //private List<DeviceModel> AllDevices()
        //{
        //    MongoCollection<DeviceModel> devicesDB = Tools.MongoConnector.Instance.DataBaseReadOnly.GetCollection<DeviceModel>(DEVICE_DB_NAME);
        //    List<DeviceModel> result = (from d in devicesDB.AsQueryable<DeviceModel>() select d).OrderBy(x => x.Id).ToList();
        //    result = RemoveDuplicateDevices(result);
        //    this.SaveDataCache("DEVICES", 0, 0, 1, result);
        //    return result;
        //}

        private void UpdateCacheDevices(DeviceModel device)
        {
            List<DeviceModel> devices = this.LoadDataCache("DEVICES") as List<DeviceModel>;
            if (devices != null)
            {
                DeviceModel result = devices.Where(x => x.Imei == device.Imei).FirstOrDefault();
                if (result == null)
                    devices.Add(device);
                else
                {
                    result.LastFields = device.LastFields;
                    result.LastLatitude = device.LastLatitude;
                    result.LastLongitude = device.LastLongitude;
                    result.IdLastTask = device.IdLastTask;
                    result.LastReport = device.LastReport;
                    result.LastValidLocation = device.LastValidLocation;
                }
            }
        }

        public DeviceModel GetDevice(string imei)
        {
            //List<DeviceModel> devices = this.LoadDataCache("DEVICES") as List<DeviceModel>;
            //if (devices == null)
            //    devices = AllDevices();

            //DeviceModel result = devices.Where(x => x.Imei == imei).FirstOrDefault();
            MongoCollection<DeviceModel> devicesDB = Tools.MongoConnector.Instance.DataBase.GetCollection<DeviceModel>(DEVICE_DB_NAME);

            DeviceModel result = (from d in devicesDB.AsQueryable<DeviceModel>()
                                  where d.Imei == imei
                                  select d).OrderBy(x => x.CreatedAt).FirstOrDefault();

            return result;
        }

        public List<DeviceModel> GetDevices()
        {
            MongoCollection<DeviceModel> devicesDB = Tools.MongoConnector.Instance.DataBaseReadOnly.GetCollection<DeviceModel>(DEVICE_DB_NAME);
            List<DeviceModel> result = (from d in devicesDB.AsQueryable<DeviceModel>() select d).OrderBy(x => x.CreatedAt).ToList();
            return result;
        }

        public void Save(DeviceModel device)
        {
            MongoCollection<DeviceModel> devicesDB = Tools.MongoConnector.Instance.DataBase.GetCollection<DeviceModel>(DEVICE_DB_NAME);
            device.CreatedAt = DateTime.UtcNow;
            devicesDB.Save(device);
            //HttpRuntime.Cache.Remove("DEVICES");
            //UpdateCacheDevices(device);
        }


        public void Save(List<DeviceModel> device)
        {
            MongoCollection<DeviceModel> devicesDB = Tools.MongoConnector.Instance.DataBase.GetCollection<DeviceModel>(DEVICE_DB_NAME);
            List<string> test = new List<string>();

            foreach (DeviceModel d in device)
            {
                if (test.Contains(d.Imei))
                    throw new Exception("duplicate");
                test.Add(d.Imei);
                d.CreatedAt = DateTime.UtcNow;
                devicesDB.Save(d);

                //UpdateCacheDevices(d);
            }
//            HttpRuntime.Cache.Remove("DEVICES");
        }
    }
}