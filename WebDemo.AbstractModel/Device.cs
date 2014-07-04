using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebDemo.AbstractModel
{
    public abstract class Device
    {
        public virtual String Imei { get; set; }
        public virtual DateTime LastReport { get; set; }
        public virtual DateTime LastValidLocation { get; set; }
        public virtual double LastLongitude { get; set; }
        public virtual double LastLatitude { get; set; }
        public virtual DateTime CreatedAt { get; set; }
        public virtual Dictionary<string, Field> LastFields { get; set; }
        public virtual int? IdLastTask { get; set; }

        public List<string> GetOrderFieldName()
        {
            List<string> result = new List<string>();


            foreach (string key in ExtendedFieldDefinition.Fields.Keys)
            {
                if (this.LastFields != null && this.LastFields.ContainsKey(key) && this.LastFields[key].B64Value != null)
                {
                    result.Add(key);
                }
            }

            return result;
        }

        public void UpdateField(Track track)
        {
            //if (track.Data != null && track.Data.Fields != null)
            if (track.Data != null && track.Data.Fields != null)
            {
                if (LastFields == null)
                    LastFields = new Dictionary<string, Field>();
                foreach (KeyValuePair<string, MD.CloudConnect.Data.Field> item in track.Data.Fields)
                //foreach (KeyValuePair<string, Field> item in track.Data.Fields)
                {
                    if (LastFields.ContainsKey(item.Key))
                        LastFields[item.Key].B64Value = item.Value.b64_value;
                    else
                        LastFields.Add(item.Key, new Field() { B64Value = item.Value.b64_value, Key = item.Key });
                }
            }
        }
    }
}
