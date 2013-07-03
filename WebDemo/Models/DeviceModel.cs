using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MD.CloudConnect.Data;

namespace WebDemo.Models
{
    public class DeviceModel
    {
        public ObjectId Id { get; set; }
        public String Imei { get; set; }

        public DateTime LastReport { get; set; }

        public DateTime LastValidLocation { get; set; }
        public double LastLongitude { get; set; }
        public double LastLatitude { get; set; }

        public Dictionary<string, FieldModel> LastFields { get; set; }

        public int? IdLastTask { get; set; }
        
        public List<string> GetOrderFieldName()
        {
            List<string> result = new List<string>();


            foreach (string key in ExtendedFieldDefinition.Fields.Keys)
            {
                if (this.LastFields.ContainsKey(key) && this.LastFields[key].B64Value != null)
                {
                    result.Add(key);
                }
            }

            return result;
        }

        public void UpdateField(TrackingModel track)
        {
            if (track.Data != null && track.Data.fields != null)
            {
                if (LastFields == null)
                    LastFields = new Dictionary<string, FieldModel>();
                foreach (KeyValuePair<string, Field> item in track.Data.fields)
                {
                    if (LastFields.ContainsKey(item.Key))
                        LastFields[item.Key].B64Value = item.Value.b64_value;
                    else
                        LastFields.Add(item.Key, new FieldModel() { B64Value = item.Value.b64_value, Key = item.Key });
                }
            }
        }
    }
}