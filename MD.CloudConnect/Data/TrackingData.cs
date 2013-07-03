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
        public string Id { get; set; }
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
        [JsonIgnore]
        public double[] location;
        public double[] loc { get { return location; } set { location = value; } }
        public Dictionary<string, Field> fields { get; set; }
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
                if (fields.ContainsKey(FieldDefinition.GPRMC_VALID.Key))
                    return fields[FieldDefinition.GPRMC_VALID.Key].GetValueAsString() == "A" ? true : false;
                else throw new KeyNotFoundException(String.Format("The key {0} is not present", FieldDefinition.GPRMC_VALID));
            }
            set
            {
                if (fields.ContainsKey(FieldDefinition.GPRMC_VALID.Key))
                    fields[FieldDefinition.GPRMC_VALID.Key].SetValueAsString(value ? "A" : "V");
                else throw new KeyNotFoundException(String.Format("The key {0} is not present", FieldDefinition.GPRMC_VALID));
            }
        }
        [JsonIgnore]
        public bool IsMoving
        {
            get
            {
                if (fields.ContainsKey(FieldDefinition.MVT_STATE.Key))
                    return fields[FieldDefinition.MVT_STATE.Key].GetValueAsBool();
                else throw new KeyNotFoundException(String.Format("The key {0} is not present", FieldDefinition.MVT_STATE));
            }
            set
            {
                if (fields.ContainsKey(FieldDefinition.MVT_STATE.Key))
                    fields[FieldDefinition.MVT_STATE.Key].SetValueAsBool(value);
                else throw new KeyNotFoundException(String.Format("The key {0} is not present", FieldDefinition.MVT_STATE));
            }
        }

        [JsonIgnore]
        public double Speed
        {
            get
            {
                if (fields.ContainsKey(FieldDefinition.GPS_SPEED.Key))
                    return fields[FieldDefinition.GPS_SPEED.Key].GetValueAsInt() / 1000.0;
                else throw new KeyNotFoundException(String.Format("The key {0} is not present", FieldDefinition.GPS_SPEED));
            }
            set
            {
                if (fields.ContainsKey(FieldDefinition.GPS_SPEED.Key))
                    fields[FieldDefinition.GPS_SPEED.Key].SetValueAsInt((int)(value * 1000));
                else throw new KeyNotFoundException(String.Format("The key {0} is not present", FieldDefinition.GPS_SPEED));
            }
        }
        [JsonIgnore]
        public double SpeedKmPerHour
        {
            get
            {
                if (fields.ContainsKey(FieldDefinition.GPS_SPEED.Key))
                    return fields[FieldDefinition.GPS_SPEED.Key].GetValueAsInt() * 1.852 / 1000.0;
                else throw new KeyNotFoundException(String.Format("The key {0} is not present", FieldDefinition.GPS_SPEED));
            }
            set
            {
                if (fields.ContainsKey(FieldDefinition.GPS_SPEED.Key))
                    fields[FieldDefinition.GPS_SPEED.Key].SetValueAsInt((int)(value * 1000 / 1.852));
                else throw new KeyNotFoundException(String.Format("The key {0} is not present", FieldDefinition.GPS_SPEED));
            }
        }
        [JsonIgnore]
        public float Direction
        {
            get
            {
                if (fields.ContainsKey(FieldDefinition.GPS_DIR.Key))
                    return fields[FieldDefinition.GPS_DIR.Key].GetValueAsInt() / 100.0f;
                else throw new KeyNotFoundException(String.Format("The key {0} is not present", FieldDefinition.GPS_DIR));
            }
            set
            {
                if (fields.ContainsKey(FieldDefinition.GPS_DIR.Key))
                    fields[FieldDefinition.GPS_DIR.Key].SetValueAsInt((int)(value * 100.0f));
                else throw new KeyNotFoundException(String.Format("The key {0} is not present", FieldDefinition.GPS_DIR));
            }
        }
        [JsonIgnore]
        public double FullOdometer
        {
            get
            {
                if (fields.ContainsKey(FieldDefinition.ODO_FULL.Key))
                    return fields[FieldDefinition.ODO_FULL.Key].GetValueAsInt();
                else throw new KeyNotFoundException(String.Format("The key {0} is not present", FieldDefinition.ODO_FULL));
            }
            set
            {
                if (fields.ContainsKey(FieldDefinition.ODO_FULL.Key))
                    fields[FieldDefinition.ODO_FULL.Key].SetValueAsInt((int)value);
                else throw new KeyNotFoundException(String.Format("The key {0} is not present", FieldDefinition.ODO_FULL));
            }
        }
        [JsonIgnore]
        public string DriverId
        {
            get
            {
                if (fields.ContainsKey(FieldDefinition.DRIVER_ID.Key))
                    return fields[FieldDefinition.DRIVER_ID.Key].GetValueAsString();
                else throw new KeyNotFoundException(String.Format("The key {0} is not present", FieldDefinition.DRIVER_ID));
            }
            set
            {
                if (fields.ContainsKey(FieldDefinition.DRIVER_ID.Key))
                    fields[FieldDefinition.DRIVER_ID.Key].SetValueAsString(value);
                else throw new KeyNotFoundException(String.Format("The key {0} is not present", FieldDefinition.DRIVER_ID));
            }
        }
        [JsonIgnore]
        public bool Ignition
        {
            get
            {
                if (fields.ContainsKey(FieldDefinition.DIO_IGNITION.Key))
                    return fields[FieldDefinition.DIO_IGNITION.Key].GetValueAsBool();
                else throw new KeyNotFoundException(String.Format("The key {0} is not present", FieldDefinition.DIO_IGNITION));
            }
            set
            {
                if (fields.ContainsKey(FieldDefinition.DIO_IGNITION.Key))
                    fields[FieldDefinition.DIO_IGNITION.Key].SetValueAsBool(value);
                else throw new KeyNotFoundException(String.Format("The key {0} is not present", FieldDefinition.DIO_IGNITION));
            }
        }
        [JsonIgnore]
        public bool Alarm
        {
            get
            {
                if (fields.ContainsKey(FieldDefinition.DIO_ALARM.Key))
                    return fields[FieldDefinition.DIO_ALARM.Key].GetValueAsBool();
                else throw new KeyNotFoundException(String.Format("The key {0} is not present", FieldDefinition.DIO_ALARM));
            }
            set
            {
                if (fields.ContainsKey(FieldDefinition.DIO_ALARM.Key))
                    fields[FieldDefinition.DIO_ALARM.Key].SetValueAsBool(value);
                else throw new KeyNotFoundException(String.Format("The key {0} is not present", FieldDefinition.DIO_ALARM));
            }
        }
        [JsonIgnore]
        public byte Inputs
        {
            get
            {
                if (fields.ContainsKey(FieldDefinition.DIO_IN_TOR.Key))
                    return (byte)fields[FieldDefinition.DIO_IN_TOR.Key].GetValueAsInt();
                else throw new KeyNotFoundException(String.Format("The key {0} is not present", FieldDefinition.DIO_IN_TOR));
            }
            set
            {
                if (fields.ContainsKey(FieldDefinition.DIO_IN_TOR.Key))
                    fields[FieldDefinition.DIO_IN_TOR.Key].SetValueAsInt(value);
                else throw new KeyNotFoundException(String.Format("The key {0} is not present", FieldDefinition.DIO_IN_TOR));
            }
        }

        public bool GetFieldAsBool(string fieldName)
        {
            if (fields.ContainsKey(fieldName))
            {
                return fields[fieldName].GetValueAsBool();
            }
            else throw new KeyNotFoundException(String.Format("The key {0} is not present", fieldName));
        }

        public string GetFieldAsString(string fieldName)
        {
            if (fields.ContainsKey(fieldName))
            {
                return fields[fieldName].GetValueAsString();
            }
            else throw new KeyNotFoundException(String.Format("The key {0} is not present", fieldName));
        }

        public int GetFieldAsInt(string fieldName)
        {
            if (fields.ContainsKey(fieldName))
            {
                return fields[fieldName].GetValueAsInt();
            }
            else throw new KeyNotFoundException(String.Format("The key {0} is not present", fieldName));
        }

        public bool ContainsField(string fieldName)
        {
            return fields.ContainsKey(fieldName);
        }

        public string[] GetFieldsPresent()
        {
            return fields.Keys.ToArray();
        }

    }
}
