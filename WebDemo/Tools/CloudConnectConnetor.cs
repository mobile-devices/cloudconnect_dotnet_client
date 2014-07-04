using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebDemo.Models.Repository;
using WebDemo.Models;
using MD.CloudConnect.Data;
using WebDemo.AbstractModel;

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
            Fields = WebDemo.AbstractModel.ExtendedFieldDefinition.Fields.Where(x => !x.Value.IgnoreInHistory).Select(x => x.Key).ToArray();
        }


        public DateTime getHistoryFor(string asset, MD.CloudConnect.ITracking data)
        {
            Device device = RepositoryFactory.Instance.DeviceDb.Get(asset);

            if (device != null)
            {
                foreach (KeyValuePair<string, MD.CloudConnect.Data.Field> item in ((TrackingData)data).fields)
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