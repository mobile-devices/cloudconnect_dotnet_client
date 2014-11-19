using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace MD.CloudConnect.Data
{
    public class TrackingData : ITracking
    {
        /* Common Fields */
        [JsonProperty("asset")]
        public string Asset { get; set; }
        [JsonProperty("id")]
        public UInt64 Id { get; set; }
        [JsonProperty("id_str")]
        public string Id_str { get; set; }

        /* Tracking Fields */
        private DateTime _recorded_at = DateTime.MinValue;
        [JsonProperty("recorded_at")]
        public DateTime Recorded_at
        {
            get
            {
                if (Recorded_at_ms.HasValue)
                    return Recorded_at_ms.Value;
                else return _recorded_at;
            }
            set
            {
                _recorded_at = value;
            }
        }

        [JsonProperty("received_at")]
        public DateTime Received_at { get; set; }

        [JsonProperty("recorded_at_ms")]
        public DateTime? Recorded_at_ms { get; set; }
        /* Difference between Api request where we need "Location" and  notification process where we need "loc" */
        public double[] location;
        public double[] loc { get { return location; } set { location = value; } }

        public Dictionary<string, MD.CloudConnect.Data.Field> Fields { get; set; }

        [JsonProperty("connection_id")]
        public UInt64? ConnectionId { get; set; }

        [JsonProperty("index")]
        public UInt32? Index { get; set; }

        [JsonIgnore]
        public int Status { get; set; }

        [JsonIgnore]
        public double Longitude
        {
            get
            {
                if (location != null && location.Length >= 2)
                    return location[0];
                else
                    return 0.0;
            }
            set
            {
                if (location != null && location.Length >= 2)
                    location[0] = value;
            }
        }

        [JsonIgnore]
        public double Latitude
        {
            get
            {
                if (location != null && location.Length >= 2)
                    return location[1];
                else
                    return 0.0;
            }
            set
            {
                if (location != null && location.Length >= 2)
                    location[1] = value;
            }
        }

        [JsonIgnore]
        public bool IsValid
        {
            get
            {
                if (location != null && location.Length >= 2)
                    return true;
                else
                    return false;
            }
            set
            {
                //DEPRECATED   
            }
        }

        [JsonIgnore]
        public bool IsMoving
        {
            get
            {
                if (Fields.ContainsKey(FieldDefinition.MVT_STATE.Key))
                    return Fields[FieldDefinition.MVT_STATE.Key].GetValueAsBool();
                else throw new KeyNotFoundException(String.Format("The key {0} is not present", FieldDefinition.MVT_STATE));
            }
            set
            {
                if (Fields.ContainsKey(FieldDefinition.MVT_STATE.Key))
                    Fields[FieldDefinition.MVT_STATE.Key].SetValueAsBool(value);
                else throw new KeyNotFoundException(String.Format("The key {0} is not present", FieldDefinition.MVT_STATE));
            }
        }

        [JsonIgnore]
        public double Speed
        {
            get
            {
                if (Fields.ContainsKey(FieldDefinition.GPS_SPEED.Key))
                    return Fields[FieldDefinition.GPS_SPEED.Key].GetValueAsInt() / 1000.0;
                else throw new KeyNotFoundException(String.Format("The key {0} is not present", FieldDefinition.GPS_SPEED));
            }
            set
            {
                if (Fields.ContainsKey(FieldDefinition.GPS_SPEED.Key))
                    Fields[FieldDefinition.GPS_SPEED.Key].SetValueAsInt((int)(value * 1000));
                else throw new KeyNotFoundException(String.Format("The key {0} is not present", FieldDefinition.GPS_SPEED));
            }
        }
        [JsonIgnore]
        public double SpeedKmPerHour
        {
            get
            {
                if (Fields.ContainsKey(FieldDefinition.GPS_SPEED.Key))
                    return Fields[FieldDefinition.GPS_SPEED.Key].GetValueAsInt() * 1.852 / 1000.0;
                else throw new KeyNotFoundException(String.Format("The key {0} is not present", FieldDefinition.GPS_SPEED));
            }
            set
            {
                if (Fields.ContainsKey(FieldDefinition.GPS_SPEED.Key))
                    Fields[FieldDefinition.GPS_SPEED.Key].SetValueAsInt((int)(value * 1000 / 1.852));
                else throw new KeyNotFoundException(String.Format("The key {0} is not present", FieldDefinition.GPS_SPEED));
            }
        }
        [JsonIgnore]
        public float Direction
        {
            get
            {
                if (Fields.ContainsKey(FieldDefinition.GPS_DIR.Key))
                    return Fields[FieldDefinition.GPS_DIR.Key].GetValueAsInt() / 100.0f;
                else throw new KeyNotFoundException(String.Format("The key {0} is not present", FieldDefinition.GPS_DIR));
            }
            set
            {
                if (Fields.ContainsKey(FieldDefinition.GPS_DIR.Key))
                    Fields[FieldDefinition.GPS_DIR.Key].SetValueAsInt((int)(value * 100.0f));
                else throw new KeyNotFoundException(String.Format("The key {0} is not present", FieldDefinition.GPS_DIR));
            }
        }
        [JsonIgnore]
        public double FullOdometer
        {
            get
            {
                if (Fields.ContainsKey(FieldDefinition.ODO_FULL.Key))
                    return Fields[FieldDefinition.ODO_FULL.Key].GetValueAsInt();
                else throw new KeyNotFoundException(String.Format("The key {0} is not present", FieldDefinition.ODO_FULL));
            }
            set
            {
                if (Fields.ContainsKey(FieldDefinition.ODO_FULL.Key))
                    Fields[FieldDefinition.ODO_FULL.Key].SetValueAsInt((int)value);
                else throw new KeyNotFoundException(String.Format("The key {0} is not present", FieldDefinition.ODO_FULL));
            }
        }
        [JsonIgnore]
        public string DriverId
        {
            get
            {
                if (Fields.ContainsKey(FieldDefinition.DRIVER_ID.Key))
                    return Fields[FieldDefinition.DRIVER_ID.Key].GetValueAsString();
                else throw new KeyNotFoundException(String.Format("The key {0} is not present", FieldDefinition.DRIVER_ID));
            }
            set
            {
                if (Fields.ContainsKey(FieldDefinition.DRIVER_ID.Key))
                    Fields[FieldDefinition.DRIVER_ID.Key].SetValueAsString(value);
                else throw new KeyNotFoundException(String.Format("The key {0} is not present", FieldDefinition.DRIVER_ID));
            }
        }
        [JsonIgnore]
        public bool Ignition
        {
            get
            {
                if (Fields.ContainsKey(FieldDefinition.DIO_IGNITION.Key))
                    return Fields[FieldDefinition.DIO_IGNITION.Key].GetValueAsBool();
                else throw new KeyNotFoundException(String.Format("The key {0} is not present", FieldDefinition.DIO_IGNITION));
            }
            set
            {
                if (Fields.ContainsKey(FieldDefinition.DIO_IGNITION.Key))
                    Fields[FieldDefinition.DIO_IGNITION.Key].SetValueAsBool(value);
                else throw new KeyNotFoundException(String.Format("The key {0} is not present", FieldDefinition.DIO_IGNITION));
            }
        }
        [JsonIgnore]
        public bool Alarm
        {
            get
            {
                if (Fields.ContainsKey(FieldDefinition.DIO_ALARM.Key))
                    return Fields[FieldDefinition.DIO_ALARM.Key].GetValueAsBool();
                else throw new KeyNotFoundException(String.Format("The key {0} is not present", FieldDefinition.DIO_ALARM));
            }
            set
            {
                if (Fields.ContainsKey(FieldDefinition.DIO_ALARM.Key))
                    Fields[FieldDefinition.DIO_ALARM.Key].SetValueAsBool(value);
                else throw new KeyNotFoundException(String.Format("The key {0} is not present", FieldDefinition.DIO_ALARM));
            }
        }
        [JsonIgnore]
        public byte Inputs
        {
            get
            {
                if (Fields.ContainsKey(FieldDefinition.DIO_IN_TOR.Key))
                    return (byte)Fields[FieldDefinition.DIO_IN_TOR.Key].GetValueAsInt();
                else throw new KeyNotFoundException(String.Format("The key {0} is not present", FieldDefinition.DIO_IN_TOR));
            }
            set
            {
                if (Fields.ContainsKey(FieldDefinition.DIO_IN_TOR.Key))
                    Fields[FieldDefinition.DIO_IN_TOR.Key].SetValueAsInt(value);
                else throw new KeyNotFoundException(String.Format("The key {0} is not present", FieldDefinition.DIO_IN_TOR));
            }
        }

        public bool GetFieldAsBool(string fieldName)
        {
            if (Fields.ContainsKey(fieldName))
            {
                return Fields[fieldName].GetValueAsBool();
            }
            else throw new KeyNotFoundException(String.Format("The key {0} is not present", fieldName));
        }

        public string GetFieldAsString(string fieldName)
        {
            if (Fields.ContainsKey(fieldName))
            {
                return Fields[fieldName].GetValueAsString();
            }
            else throw new KeyNotFoundException(String.Format("The key {0} is not present", fieldName));
        }

        public int GetFieldAsInt(string fieldName)
        {
            if (Fields.ContainsKey(fieldName))
            {
                return Fields[fieldName].GetValueAsInt();
            }
            else throw new KeyNotFoundException(String.Format("The key {0} is not present", fieldName));
        }

        public bool ContainsField(string fieldName)
        {
            return Fields.ContainsKey(fieldName);
        }

        public string[] GetFieldsPresent()
        {
            return Fields.Keys.ToArray();
        }

    }
}
