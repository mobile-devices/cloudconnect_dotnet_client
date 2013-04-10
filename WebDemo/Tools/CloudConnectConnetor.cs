using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebDemo.Models.Repository;
using WebDemo.Models;
using MD.CloudConnect.Data;

namespace WebDemo.Tools
{
    public class CloudConnectConnetor : MD.CloudConnect.IDataCache
    {
        public string[] Fields { get; set; }

        protected static readonly CloudConnectConnetor _instance = new CloudConnectConnetor();
        public static CloudConnectConnetor Instance
        {
            get
            {
                lock (_instance)
                {
                    return _instance;
                }
            }
        }

        static CloudConnectConnetor()
        {

        }

        public void InitializeFields()
        {
            Fields = ExtendedFieldDefinition.Fields.Where(x => !x.Value.IgnoreInHistory).Select(x => x.Key).ToArray();
        }


        public DateTime getHistoryFor(string asset, MD.CloudConnect.ITracking data)
        {
            DeviceModel device = RepositoryFactory.Instance.DeviceDb.GetDevice(asset);

            if (device != null)
            {
                foreach (KeyValuePair<string, Field> item in ((TrackingData)data).fields)
                {
                    if (device.LastFields != null && device.LastFields.ContainsKey(item.Key))
                        ((TrackingData)data).fields[item.Key].b64_value = device.LastFields[item.Key].B64Value;
                }
                ((TrackingData)data).location = new double[] { device.LastLongitude, device.LastLatitude };
                return device.LastReport;
            }
            else return DateTime.MinValue;
        }
    }
}