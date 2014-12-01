using MD.CloudConnect.CouchBaseProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebDemo.Models.Repository
{
    public class DeviceDb : RepositoryBase
    {
        private const string DEVICE_KEY = "DEVICES_DB";
        private const string DEVICE_DB_NAME = "DEVICES";

        public Device GetDevice(string imei)
        {

            Device result = CouchbaseManager.Instance.DeviceRepository.Get(imei);
            return result;
        }

        public List<Device> GetDevices()
        {

            List<Device> result = CouchbaseManager.Instance.DeviceRepository.GetAll();            
            return result;
        }
    }
}