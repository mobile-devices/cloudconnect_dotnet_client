using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MD.CloudConnect.Data;

namespace WebDemo.Models
{
    public enum ExtendedFieldType
    {
        String,
        Integer,
        Boolean,
        Speed,
        Int100,
        Int1000,
        MilliSecond,
        Second
    }

    public struct ExtendedFieldDetail
    {
        public string Key;
        public string DisplayName;
        public int Id;

        public string FieldDependency;
        public bool IgnoreInHistory;

        public ExtendedFieldType Type;

        public string DisplayValue(Field field)
        {
            switch (Type)
            {
                case ExtendedFieldType.Boolean: return field.GetValueAsBool().ToString();
                case ExtendedFieldType.Integer: return field.GetValueAsInt().ToString();
                case ExtendedFieldType.String: return field.GetValueAsString().ToString();
                case ExtendedFieldType.Speed: return (Math.Round(field.GetValueAsInt() * 1.852 / 1000.0, 2)).ToString();
                case ExtendedFieldType.Int100: return (Math.Round(field.GetValueAsInt() / 100.00, 2)).ToString();
                case ExtendedFieldType.Int1000: return (Math.Round(field.GetValueAsInt() / 1000.00, 3)).ToString();
                case ExtendedFieldType.MilliSecond: return new TimeSpan(0, 0, 0, 0, field.GetValueAsInt()).ToString();
                case ExtendedFieldType.Second: return new TimeSpan(0, 0, field.GetValueAsInt()).ToString("c");
                default: return field.b64_value;
            }
        }
    }

    public class ExtendedFieldDefinition
    {
        public static Dictionary<string, ExtendedFieldDetail> Fields = new Dictionary<string, ExtendedFieldDetail>()
        {
            { "GPRMC_VALID",  new ExtendedFieldDetail() { Key = "GPRMC_VALID", Id = 3, Type = ExtendedFieldType.String, DisplayName = "Valid" }},
            { "GPS_SPEED",  new ExtendedFieldDetail() { Key = "GPS_SPEED", Id = 8, Type = ExtendedFieldType.Speed , DisplayName = "Speed (Km/h)" }},
            { "GPS_DIR",  new  ExtendedFieldDetail() { Key = "GPS_DIR", Id = 9, Type = ExtendedFieldType.Int100, DisplayName = "Dir" }},
            { "DIO_IGNITION",  new  ExtendedFieldDetail() { Key = "DIO_IGNITION", Id = 14, Type = ExtendedFieldType.Boolean, DisplayName="Ignition" }},
            { "ODO_FULL",new ExtendedFieldDetail() { Key = "ODO_FULL", Id = 24, Type = ExtendedFieldType.Integer ,DisplayName = "Km"}},
            { "DIO_ALARM", new ExtendedFieldDetail() { Key = "DIO_ALARM", Id = 26, Type = ExtendedFieldType.Boolean }},
            { "DRIVER_ID",new ExtendedFieldDetail() { Key = "DRIVER_ID", Id = 27, Type = ExtendedFieldType.String}},
            { "DIO_IN_TOR", new ExtendedFieldDetail() { Key = "DIO_IN_TOR", Id = 38, Type = ExtendedFieldType.Integer }},
            { "BATT",new ExtendedFieldDetail() { Key = "BATT", Id = 15, Type = ExtendedFieldType.Integer }},
            { "GPRS_HEADER", new ExtendedFieldDetail() { Key = "GPRS_HEADER", Id = 16, Type = ExtendedFieldType.Integer }},
            { "RSSI", new ExtendedFieldDetail() { Key = "RSSI", Id = 17, Type = ExtendedFieldType.Integer }},
            { "MVT_STATE", new ExtendedFieldDetail() { Key = "MVT_STATE", Id = 56, Type = ExtendedFieldType.Boolean }},

            { "BEHAVE_ID", new ExtendedFieldDetail() { Key = "BEHAVE_ID", Id = 100, Type = ExtendedFieldType.Integer, FieldDependency ="BEHAVE_UNIQUE_ID", DisplayName = "Behv. ID" }},
            { "BEHAVE_LONG", new ExtendedFieldDetail() { Key = "BEHAVE_LONG", Id = 101, Type = ExtendedFieldType.Integer , FieldDependency ="BEHAVE_UNIQUE_ID", DisplayName = "B. Long."}},
            { "BEHAVE_LAT", new ExtendedFieldDetail() { Key = "BEHAVE_LAT", Id = 102, Type = ExtendedFieldType.Integer , FieldDependency ="BEHAVE_UNIQUE_ID", DisplayName = "B. Lat."}},
            { "BEHAVE_DAY_OF_YEAR", new ExtendedFieldDetail() { Key = "BEHAVE_DAY_OF_YEAR", Id = 103, Type = ExtendedFieldType.Integer , FieldDependency ="BEHAVE_UNIQUE_ID", DisplayName = "B. Date"}},
            { "BEHAVE_TIME_OF_DAY", new ExtendedFieldDetail() { Key = "BEHAVE_TIME_OF_DAY", Id = 104, Type = ExtendedFieldType.Integer , FieldDependency ="BEHAVE_UNIQUE_ID", DisplayName = "B. Time"}},
            { "BEHAVE_GPS_SPEED_BEGIN", new ExtendedFieldDetail() { Key = "BEHAVE_GPS_SPEED_BEGIN", Id = 105, Type = ExtendedFieldType.Integer , FieldDependency ="BEHAVE_UNIQUE_ID", DisplayName = "B. Speed Begin" }},
            { "BEHAVE_GPS_SPEED_PEAK", new ExtendedFieldDetail() { Key = "BEHAVE_GPS_SPEED_PEAK", Id = 106, Type = ExtendedFieldType.Integer, FieldDependency ="BEHAVE_UNIQUE_ID", DisplayName = "B. Speed Peak" }},
            { "BEHAVE_GPS_SPEED_END", new ExtendedFieldDetail() { Key = "BEHAVE_GPS_SPEED_END", Id = 107, Type = ExtendedFieldType.Integer , FieldDependency ="BEHAVE_UNIQUE_ID", DisplayName = "B. Speed End"}},
            { "BEHAVE_GPS_HEADING_BEGIN", new ExtendedFieldDetail() { Key = "BEHAVE_GPS_HEADING_BEGIN", Id = 108, Type = ExtendedFieldType.Integer , FieldDependency ="BEHAVE_UNIQUE_ID", DisplayName = "B. Heading Begin"}},
            { "BEHAVE_GPS_HEADING_PEAK", new ExtendedFieldDetail() { Key = "BEHAVE_GPS_HEADING_PEAK", Id = 109, Type = ExtendedFieldType.Integer , FieldDependency ="BEHAVE_UNIQUE_ID", DisplayName = "B. Heading Peak"}},
            { "BEHAVE_GPS_HEADING_END", new ExtendedFieldDetail() { Key = "BEHAVE_GPS_HEADING_END", Id = 110, Type = ExtendedFieldType.Integer , FieldDependency ="BEHAVE_UNIQUE_ID", DisplayName = "B. Heading End"}},
            { "BEHAVE_ACC_X_BEGIN", new ExtendedFieldDetail() { Key = "BEHAVE_ACC_X_BEGIN", Id = 111, Type = ExtendedFieldType.Integer, FieldDependency ="BEHAVE_UNIQUE_ID", DisplayName = "B. AccX Begin" }},
            { "BEHAVE_ACC_X_PEAK", new ExtendedFieldDetail() { Key = "BEHAVE_ACC_X_PEAK", Id = 112, Type = ExtendedFieldType.Integer, FieldDependency ="BEHAVE_UNIQUE_ID", DisplayName = "B. AccX Peak" }},
            { "BEHAVE_ACC_X_END", new ExtendedFieldDetail() { Key = "BEHAVE_ACC_X_END", Id = 113, Type = ExtendedFieldType.Integer, FieldDependency ="BEHAVE_UNIQUE_ID" , DisplayName = "B. AccX End"}},
            { "BEHAVE_ACC_Y_BEGIN", new ExtendedFieldDetail() { Key = "BEHAVE_ACC_Y_BEGIN", Id = 114, Type = ExtendedFieldType.Integer , FieldDependency ="BEHAVE_UNIQUE_ID", DisplayName = "B. AccY Begin"}},
            { "BEHAVE_ACC_Y_PEAK", new ExtendedFieldDetail() { Key = "BEHAVE_ACC_Y_PEAK", Id = 115, Type = ExtendedFieldType.Integer , FieldDependency ="BEHAVE_UNIQUE_ID", DisplayName = "B. AccY Peak"}},
            { "BEHAVE_ACC_Y_END", new ExtendedFieldDetail() { Key = "BEHAVE_ACC_Y_END", Id = 116, Type = ExtendedFieldType.Integer, FieldDependency ="BEHAVE_UNIQUE_ID" , DisplayName = "B. AccY End"}},
            { "BEHAVE_ACC_Z_BEGIN", new ExtendedFieldDetail() { Key = "BEHAVE_ACC_Z_BEGIN", Id = 117, Type = ExtendedFieldType.Integer, FieldDependency ="BEHAVE_UNIQUE_ID" , DisplayName = "B. AccZ Begin"}},
            { "BEHAVE_ACC_Z_PEAK", new ExtendedFieldDetail() { Key = "BEHAVE_ACC_Z_PEAK", Id = 118, Type = ExtendedFieldType.Integer, FieldDependency ="BEHAVE_UNIQUE_ID", DisplayName = "B. AccZ Peak" }},
            { "BEHAVE_ACC_Z_END", new ExtendedFieldDetail() { Key = "BEHAVE_ACC_Z_END", Id = 119, Type = ExtendedFieldType.Integer, FieldDependency ="BEHAVE_UNIQUE_ID" , DisplayName = "B. AccZ Ends"}},
            { "BEHAVE_ELAPSED", new ExtendedFieldDetail() { Key = "BEHAVE_ELAPSED", Id = 120, Type = ExtendedFieldType.Integer , FieldDependency ="BEHAVE_UNIQUE_ID", DisplayName = "B. Elapsed"}},
            { "BEHAVE_UNIQUE_ID", new ExtendedFieldDetail() { Key = "BEHAVE_UNIQUE_ID", Id = 121, Type = ExtendedFieldType.Integer, DisplayName = "B. UniqueID", FieldDependency ="BEHAVE_UNIQUE_ID" }},

            { "MDI_EXT_BATT_LOW", new ExtendedFieldDetail() { Key = "MDI_EXT_BATT_LOW", Id = 150, Type = ExtendedFieldType.Boolean , DisplayName = "Ext. Batt. Low"}},
            { "MDI_EXT_BATT_VOLTAGE", new ExtendedFieldDetail() { Key = "MDI_EXT_BATT_VOLTAGE", Id = 151 , Type = ExtendedFieldType.Int1000 , DisplayName = "Ext. Batt. Voltage"}},

            { "MDI_OBD_SPEED", new ExtendedFieldDetail() { Key = "MDI_OBD_SPEED", Id = 235, Type = ExtendedFieldType.Integer, DisplayName = "Obd Speed (km/h)" }},
            { "MDI_OBD_RPM", new ExtendedFieldDetail() { Key = "MDI_OBD_RPM", Id = 236, Type = ExtendedFieldType.Integer, DisplayName = "Obd Rpm" }},
            { "MDI_OBD_FUEL", new ExtendedFieldDetail() { Key = "MDI_OBD_FUEL", Id = 237, Type = ExtendedFieldType.Integer , DisplayName = "Obd Fuel"}},
            { "MDI_OBD_VIN", new ExtendedFieldDetail() { Key = "MDI_OBD_VIN", Id = 238, Type = ExtendedFieldType.String , DisplayName = "Obd Vin"}},
            { "MDI_OBD_MILEAGE", new ExtendedFieldDetail() { Key = "MDI_OBD_MILEAGE", Id = 239, Type = ExtendedFieldType.Integer , DisplayName = "Obd Km"}},

            { "MDI_JOURNEY_TIME", new ExtendedFieldDetail() { Key = "MDI_JOURNEY_TIME", Id = 240, Type = ExtendedFieldType.Second , DisplayName = "Journey Time"}},
            { "MDI_IDLE_JOURNEY", new ExtendedFieldDetail() { Key = "MDI_IDLE_JOURNEY", Id = 241, Type = ExtendedFieldType.Second , DisplayName = "Idle journey"}},
            { "MDI_DRIVING_JOURNEY", new ExtendedFieldDetail() { Key = "MDI_DRIVING_JOURNEY", Id = 242, Type = ExtendedFieldType.Second , DisplayName = "Driving journey"}},
            { "MDI_OVERSPEED_COUNTER", new ExtendedFieldDetail() { Key = "MDI_OVERSPEED_COUNTER", Id = 244, Type = ExtendedFieldType.Integer , DisplayName = "Overspeed Counter"}},
            { "MDI_TOW_AWAY", new ExtendedFieldDetail() { Key = "MDI_TOW_AWAY", Id = 245, Type = ExtendedFieldType.Boolean , DisplayName = "Tow Away"}},
            { "MDI_ODO_JOURNEY", new ExtendedFieldDetail() { Key = "MDI_ODO_JOURNEY", Id = 246, Type = ExtendedFieldType.Integer , DisplayName = "Odo Journey"}},
            { "MDI_OVERSPEED", new ExtendedFieldDetail() { Key = "MDI_OVERSPEED", Id = 247, Type = ExtendedFieldType.Boolean , DisplayName = "Overspeed Status"}},
            { "MDI_MAX_SPEED_JOURNEY", new ExtendedFieldDetail() { Key = "MDI_MAX_SPEED_JOURNEY", Id = 248, Type = ExtendedFieldType.Integer , DisplayName = "Overspeed Max"}},
            { "MDI_JOURNEY_STATE", new ExtendedFieldDetail() { Key = "MDI_JOURNEY_STATE", Id = 249, Type = ExtendedFieldType.Integer , DisplayName = "Journey State"}},

            { "MDI_BOOT_REASON", new ExtendedFieldDetail() { Key = "MDI_BOOT_REASON", Id = 0, Type = ExtendedFieldType.String , DisplayName = "Boot Reason", IgnoreInHistory = true}},
            { "MDI_SHUTDOWN_REASON", new ExtendedFieldDetail() { Key = "MDI_SHUTDOWN_REASON", Id = 0, Type = ExtendedFieldType.String , DisplayName = "Shutd. Reason", IgnoreInHistory = true}}
          
        };
    }

    public class FieldModel
    {
        public string Key { get; set; }
        public string B64Value { get; set; }

        public int IntegerValue { get; set; }
        public bool BooleanValue { get; set; }
        public string StringValue { get; set; }
    }
}