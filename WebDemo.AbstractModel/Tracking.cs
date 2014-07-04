using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebDemo.AbstractModel
{
    public class WebDemoTrackingData
    {
        public DateTime Recorded_at { get; set; }
        public DateTime Received_at { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string Asset { get; set; }
        public ulong Id { get; set; }
        public string Id_str { get; set; }

        public Dictionary<string, MD.CloudConnect.Data.Field> Fields { get; set; }
    }

    public abstract class Track
    {
        public virtual int RecordedDateKey { get; set; }
        public virtual DateTime RecordedDate { get; set; }
        public virtual DateTime CreatedAt { get; set; }
        public virtual string Asset { get; set; }
        public virtual WebDemoTrackingData Data { get; set; }
        public virtual bool Dropped { get; set; }

        public string GetDisplayNameField(string key)
        {
            if (ExtendedFieldDefinition.Fields.ContainsKey(key))
            {
                if (!String.IsNullOrEmpty(ExtendedFieldDefinition.Fields[key].DisplayName))
                    return ExtendedFieldDefinition.Fields[key].DisplayName;
                else return key;
            }
            else return "";
        }

        public string GetDisplayDataFor(string key)
        {
            if (Data.Fields.ContainsKey(key))
                return ExtendedFieldDefinition.Fields[key].DisplayValue(Data.Fields[key]);
            else return " - ";
        }

        public string GetDisplayDataFor(string key, Dictionary<string, MD.CloudConnect.Data.Field> dependencyValues)
        {
            if (Data.Fields.ContainsKey(key))
            {
                if (!String.IsNullOrEmpty(ExtendedFieldDefinition.Fields[key].FieldDependency))
                {
                    if (dependencyValues == null || !dependencyValues.ContainsKey(ExtendedFieldDefinition.Fields[key].FieldDependency))
                        return " - ";
                    string b64valueDependency = dependencyValues[ExtendedFieldDefinition.Fields[key].FieldDependency].b64_value;

                    if (Data.Fields[ExtendedFieldDefinition.Fields[key].FieldDependency].b64_value != b64valueDependency)
                        return ExtendedFieldDefinition.Fields[key].DisplayValue(Data.Fields[key]);
                    else return " - ";
                }
                else
                    return ExtendedFieldDefinition.Fields[key].DisplayValue(Data.Fields[key]);
            }
            else return " - ";
        }
    }
}
