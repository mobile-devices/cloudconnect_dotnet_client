using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebDemo.Models
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

   // public class TrackingModel
    //{
       // public string Id { get; set; }
       // public string DeviceID { get; set; }
       // public int RecordedDateKey { get; set; }

       // public DateTime RecordedDate { get; set; }
       // public DateTime CreatedAt { get; set; }

       // public string Asset { get; set; }
       // public WebDemoTrackingData Data { get; set; }
       //// public MD.CloudConnect.Data.TrackingData Data { get; set; }
       // public bool Dropped { get; set; }
       // public string GetDisplayNameField(string key)
       // {
       //     if (WebDemo.Models.ExtendedFieldDefinition.Fields.ContainsKey(key))
       //     {
       //         if (!String.IsNullOrEmpty(WebDemo.Models.ExtendedFieldDefinition.Fields[key].DisplayName))
       //             return WebDemo.Models.ExtendedFieldDefinition.Fields[key].DisplayName;
       //         else return key;
       //     }
       //     else return "";
       // }

       // public string GetDisplayDataFor(string key)
       // {
       //     if (Data.Fields.ContainsKey(key))
       //         return WebDemo.Models.ExtendedFieldDefinition.Fields[key].DisplayValue(Data.Fields[key]);
       //     else return " - ";
       // }

       // public string GetDisplayDataFor(string key, Dictionary<string, MD.CloudConnect.Data.Field> dependencyValues)
       // {
       //     if (Data.Fields.ContainsKey(key))
       //     {
       //         if (!String.IsNullOrEmpty(WebDemo.Models.ExtendedFieldDefinition.Fields[key].FieldDependency))
       //         {
       //             if (dependencyValues == null || !dependencyValues.ContainsKey(WebDemo.Models.ExtendedFieldDefinition.Fields[key].FieldDependency))
       //                 return " - ";
       //             string b64valueDependency = dependencyValues[WebDemo.Models.ExtendedFieldDefinition.Fields[key].FieldDependency].b64_value;

       //             if (Data.Fields[WebDemo.Models.ExtendedFieldDefinition.Fields[key].FieldDependency].b64_value != b64valueDependency)
       //                 return WebDemo.Models.ExtendedFieldDefinition.Fields[key].DisplayValue(Data.Fields[key]);
       //             else return " - ";
       //         }
       //         else
       //             return WebDemo.Models.ExtendedFieldDefinition.Fields[key].DisplayValue(Data.Fields[key]);
       //     }
       //     else return " - ";
       // }
    //}
}