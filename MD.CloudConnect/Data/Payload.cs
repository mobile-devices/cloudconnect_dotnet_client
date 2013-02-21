using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MD.CloudConnect
{
    [Serializable]
    public class Payload : ITracking, IMessage
    {
        /* Common Fields */
        public string Asset { get; set; }
        public string Id { get; set; }
        public string Id_str { get; set; }

        /* Tracking Fields */
        public DateTime Recorded_at { get; set; }
        public DateTime Received_at { get; set; }
        public double[] loc { get; set; }
        public Dictionary<string, Field> fields { get; set; }

        /* Message Fields */
        public string b64_payload { get; set; }
        private string b64_payload_decoded = null;
        public string Channel { get; set; }

        /* Presence Fields */
        public DateTime time { get; set; }
        public string type { get; set; }
        public string reason { get; set; }

        internal bool shouldBeIgnore = false;

        /* Tracking Implementation */
        double ITracking.Longitude
        {
            get
            {
                if (loc != null && loc.Length >= 2)
                    return loc[0];
                else
                    return 0.0;
            }
        }

        double ITracking.Latitude
        {
            get
            {
                if (loc != null && loc.Length >= 2)
                    return loc[1];
                else
                    return 0.0;
            }
        }

        bool ITracking.IsValid
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

        double ITracking.Speed
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

        double ITracking.SpeedKmPerHour
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

        float ITracking.Direction
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

        double ITracking.FullOdometer
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

        string ITracking.DriverId
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

        bool ITracking.Ignition
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
        bool ITracking.Alarm
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
        byte ITracking.Inputs
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

        bool ITracking.GetFieldAsBool(string fieldName)
        {
            if (fields.ContainsKey(fieldName))
            {
                return fields[fieldName].GetValueAsBool();
            }
            else throw new KeyNotFoundException(String.Format("The key {0} is not present", fieldName));
        }

        string ITracking.GetFieldAsString(string fieldName)
        {
            if (fields.ContainsKey(fieldName))
            {
                return fields[fieldName].GetValueAsString();
            }
            else throw new KeyNotFoundException(String.Format("The key {0} is not present", fieldName));
        }

        int ITracking.GetFieldAsInt(string fieldName)
        {
            if (fields.ContainsKey(fieldName))
            {
                return fields[fieldName].GetValueAsInt();
            }
            else throw new KeyNotFoundException(String.Format("The key {0} is not present", fieldName));
        }

        bool ITracking.ContainsField(string fieldName)
        {
            return fields.ContainsKey(fieldName);
        }

        string[] ITracking.GetFieldsPresent()
        {
            return fields.Keys.ToArray();
        }

        string IMessage.Message
        {
            get
            {
                if (String.IsNullOrEmpty(b64_payload_decoded) && !String.IsNullOrEmpty(b64_payload))
                {
                    if (!String.IsNullOrEmpty(b64_payload))
                    {
                        byte[] values = Convert.FromBase64String(b64_payload);
                        b64_payload_decoded = String.Empty;
                        for (int i = 0; i < values.Length; i++)
                        {
                            b64_payload_decoded += (char)values[i];
                        }
                    }
                    else b64_payload_decoded = String.Empty;
                }
                return b64_payload_decoded;
            }
        }
    }
}
