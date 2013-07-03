using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;

namespace WebDemo.Models
{
    public class TrackingModel
    {
        public ObjectId Id { get; set; }
        public ObjectId DeviceID { get; set; }

        public int RecordedDateKey { get; set; }

        public MD.CloudConnect.Data.TrackingData Data { get; set; }

        public string GetDisplayNameField(string key)
        {
            if (WebDemo.Models.ExtendedFieldDefinition.Fields.ContainsKey(key))
            {
                if (!String.IsNullOrEmpty(WebDemo.Models.ExtendedFieldDefinition.Fields[key].DisplayName))
                    return WebDemo.Models.ExtendedFieldDefinition.Fields[key].DisplayName;
                else return key;
            }
            else return "";
        }

        public string GetDisplayDataFor(string key)
        {
            if (Data.fields.ContainsKey(key))
                return WebDemo.Models.ExtendedFieldDefinition.Fields[key].DisplayValue(Data.fields[key]);
            else return " - ";
        }

        public string GetDisplayDataFor(string key, Dictionary<string, MD.CloudConnect.Data.Field> dependencyValues)
        {
            if (Data.fields.ContainsKey(key))
            {
                if (!String.IsNullOrEmpty(WebDemo.Models.ExtendedFieldDefinition.Fields[key].FieldDependency))
                {
                    if (dependencyValues == null || !dependencyValues.ContainsKey(WebDemo.Models.ExtendedFieldDefinition.Fields[key].FieldDependency))
                        return " - ";
                    string b64valueDependency = dependencyValues[WebDemo.Models.ExtendedFieldDefinition.Fields[key].FieldDependency].b64_value;

                    if (Data.fields[WebDemo.Models.ExtendedFieldDefinition.Fields[key].FieldDependency].b64_value != b64valueDependency)
                        return WebDemo.Models.ExtendedFieldDefinition.Fields[key].DisplayValue(Data.fields[key]);
                    else return " - ";
                }
                else
                    return WebDemo.Models.ExtendedFieldDefinition.Fields[key].DisplayValue(Data.fields[key]);
            }
            else return " - ";
        }
    }
}