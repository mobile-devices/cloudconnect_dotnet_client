using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MD.CloudConnect.Data;
using System.Text;

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
        Second,
        DateDbehav,
        TimeDbehav,
        Sensor4hz
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
                case ExtendedFieldType.DateDbehav:
                    try
                    {
                        return DateTime.ParseExact(field.GetValueAsInt().ToString(), "yyMMdd", null).ToString("yyyy/MM/dd");
                    }
                    catch
                    {
                        return "-";
                    }
                case ExtendedFieldType.TimeDbehav:
                    try
                    {
                        string timeString = field.GetValueAsInt().ToString();
                        if (timeString.Length < 6) timeString = "0" + timeString;
                        return DateTime.ParseExact(timeString, "HHmmss", null).ToString("HH:mm:ss");
                    }
                    catch
                    {
                        return "-";
                    }
                case ExtendedFieldType.Sensor4hz:
                    try
                    {
                        byte[] ba = field.GetRawValue();
                        var hexString = BitConverter.ToString(ba);
                        hexString = hexString.Replace("-", "");
                        return hexString;
                    }
                    catch
                    {
                        return "-";
                    }
                default: return field.b64_value;
            }
        }
    }

    public class ExtendedFieldDefinition
    {
        public static Dictionary<string, ExtendedFieldDetail> Fields = new Dictionary<string, ExtendedFieldDetail>()
        {
            { "GPRMC_VALID",  new ExtendedFieldDetail() { Key = "GPRMC_VALID", Id = 3, Type = ExtendedFieldType.String, DisplayName = "Valid" }},
            { "GPS_SPEED",  new ExtendedFieldDetail() { Key = "GPS_SPEED", Id = 8, Type = ExtendedFieldType.Speed , DisplayName = "Gps Speed (Km/h)" }},
            { "GPS_DIR",  new  ExtendedFieldDetail() { Key = "GPS_DIR", Id = 9, Type = ExtendedFieldType.Int100, DisplayName = "Dir" }},
            { "DIO_IGNITION",  new  ExtendedFieldDetail() { Key = "DIO_IGNITION", Id = 14, Type = ExtendedFieldType.Boolean, DisplayName="Ignition" }},
            { "BATT",new ExtendedFieldDetail() { Key = "BATT", Id = 15, Type = ExtendedFieldType.Integer }},
            { "GPRS_HEADER", new ExtendedFieldDetail() { Key = "GPRS_HEADER", Id = 16, Type = ExtendedFieldType.Integer }},
            { "RSSI", new ExtendedFieldDetail() { Key = "RSSI", Id = 17, Type = ExtendedFieldType.Integer }},

            { "TACHOGRAPH_FIRST_DRIVER_STATE", new ExtendedFieldDetail() { Key = "TACHOGRAPH_FIRST_DRIVER_STATE", Id = 19, Type = ExtendedFieldType.String }},
            { "TACHOGRAPH_FIRST_DRIVER_DRIVING_STATE", new ExtendedFieldDetail() { Key = "TACHOGRAPH_FIRST_DRIVER_DRIVING_STATE", Id = 20, Type = ExtendedFieldType.String }},
            { "TACHOGRAPH_DAILYMETER", new ExtendedFieldDetail() { Key = "TACHOGRAPH_DAILYMETER", Id = 21, Type = ExtendedFieldType.Integer }},
            { "TACHOGRAPH_ODOMETER", new ExtendedFieldDetail() { Key = "TACHOGRAPH_ODOMETER", Id = 22, Type = ExtendedFieldType.Integer }},
            
            { "ODO_FULL",new ExtendedFieldDetail() { Key = "ODO_FULL", Id = 24, Type = ExtendedFieldType.Integer ,DisplayName = "Odo. Full"}},
            { "TACHOGRAPH_DRIVING_TIME", new ExtendedFieldDetail() { Key = "TACHOGRAPH_DRIVING_TIME", Id = 25, Type = ExtendedFieldType.Integer }},
            { "DIO_ALARM", new ExtendedFieldDetail() { Key = "DIO_ALARM", Id = 26, Type = ExtendedFieldType.Boolean }},
            { "DRIVER_ID",new ExtendedFieldDetail() { Key = "DRIVER_ID", Id = 27, Type = ExtendedFieldType.String}},

            { "TEMP_1",new ExtendedFieldDetail() { Key = "TEMP_1", Id = 28, Type = ExtendedFieldType.Integer}},
            { "TEMP_2",new ExtendedFieldDetail() { Key = "TEMP_2", Id = 29, Type = ExtendedFieldType.Integer}},
            { "TEMP_3",new ExtendedFieldDetail() { Key = "TEMP_3", Id = 30, Type = ExtendedFieldType.Integer}},
            { "TEMP_4",new ExtendedFieldDetail() { Key = "TEMP_4", Id = 31, Type = ExtendedFieldType.Integer}},
            { "TEMP_5",new ExtendedFieldDetail() { Key = "TEMP_5", Id = 32, Type = ExtendedFieldType.Integer}},
            { "TEMP_6",new ExtendedFieldDetail() { Key = "TEMP_6", Id = 33, Type = ExtendedFieldType.Integer}},
            { "TEMP_7",new ExtendedFieldDetail() { Key = "TEMP_7", Id = 34, Type = ExtendedFieldType.Integer}},
            { "TEMP_8",new ExtendedFieldDetail() { Key = "TEMP_8", Id = 35, Type = ExtendedFieldType.Integer}},

            { "DIO_IN_TOR", new ExtendedFieldDetail() { Key = "DIO_IN_TOR", Id = 38, Type = ExtendedFieldType.Integer }},
            { "GPS_HDOP", new ExtendedFieldDetail() { Key = "GPS_HDOP", Id = 39, Type = ExtendedFieldType.Integer }},
            { "GPS_VDOP", new ExtendedFieldDetail() { Key = "GPS_VDOP", Id = 40, Type = ExtendedFieldType.Integer }},
            { "GPS_PDOP", new ExtendedFieldDetail() { Key = "GPS_PDOP", Id = 41, Type = ExtendedFieldType.Integer }},

            { "BATT_TEMP", new ExtendedFieldDetail() { Key = "BATT_TEMP", Id = 42, Type = ExtendedFieldType.Int1000 }},
            { "CASE_TEMP", new ExtendedFieldDetail() { Key = "CASE_TEMP", Id = 43, Type = ExtendedFieldType.Int1000 }},

            { "OBD_CONNECTED_PROTOCOL", new ExtendedFieldDetail() { Key = "OBD_CONNECTED_PROTOCOL", Id = 45, Type = ExtendedFieldType.Integer }},

            { "BATT_VOLT", new ExtendedFieldDetail() { Key = "BATT_VOLT", Id = 51, Type = ExtendedFieldType.Integer, DisplayName = "Batt. volt(mV)" }},
            { "MDI_AREA_LIST", new ExtendedFieldDetail() { Key = "MDI_AREA_LIST", Id = 53, Type = ExtendedFieldType.String, DisplayName = "Area List" }},

            { "GPS_FIXED_SAT_NUM", new ExtendedFieldDetail() { Key = "GPS_FIXED_SAT_NUM", Id = 55, Type = ExtendedFieldType.Integer }},
            { "MVT_STATE", new ExtendedFieldDetail() { Key = "MVT_STATE", Id = 56, Type = ExtendedFieldType.Boolean }},

            { "BEHAVE_ID", new ExtendedFieldDetail() { Key = "BEHAVE_ID", Id = 100, Type = ExtendedFieldType.Integer, FieldDependency ="BEHAVE_UNIQUE_ID", DisplayName = "Behv. ID" }},
            { "BEHAVE_LONG", new ExtendedFieldDetail() { Key = "BEHAVE_LONG", Id = 101, Type = ExtendedFieldType.Integer , FieldDependency ="BEHAVE_UNIQUE_ID", DisplayName = "B. Long."}},
            { "BEHAVE_LAT", new ExtendedFieldDetail() { Key = "BEHAVE_LAT", Id = 102, Type = ExtendedFieldType.Integer , FieldDependency ="BEHAVE_UNIQUE_ID", DisplayName = "B. Lat."}},
            { "BEHAVE_DAY_OF_YEAR", new ExtendedFieldDetail() { Key = "BEHAVE_DAY_OF_YEAR", Id = 103, Type = ExtendedFieldType.DateDbehav , FieldDependency ="BEHAVE_UNIQUE_ID", DisplayName = "B. Date"}},
            { "BEHAVE_TIME_OF_DAY", new ExtendedFieldDetail() { Key = "BEHAVE_TIME_OF_DAY", Id = 104, Type = ExtendedFieldType.TimeDbehav , FieldDependency ="BEHAVE_UNIQUE_ID", DisplayName = "B. Time"}},
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

            { "MDI_CRASH_DETECTED", new ExtendedFieldDetail() { Key = "MDI_CRASH_DETECTED", Id = 122, Type = ExtendedFieldType.String , DisplayName = "Crash Detected", IgnoreInHistory = true}},
          
            { "MDI_EXT_BATT_LOW", new ExtendedFieldDetail() { Key = "MDI_EXT_BATT_LOW", Id = 150, Type = ExtendedFieldType.Boolean , DisplayName = "Ext. Batt. Low"}},
            { "MDI_EXT_BATT_VOLTAGE", new ExtendedFieldDetail() { Key = "MDI_EXT_BATT_VOLTAGE", Id = 151 , Type = ExtendedFieldType.Int1000 , DisplayName = "Ext. Batt. Voltage"}},

            { "MDI_DTC_MIL", new ExtendedFieldDetail() { Key = "MDI_DTC_MIL", Id = 154 , Type = ExtendedFieldType.Boolean , DisplayName = "Malfunction Indicator Lamp (MIL)"}},
            { "MDI_DTC_NUMBER", new ExtendedFieldDetail() { Key = "MDI_DTC_NUMBER", Id = 155 , Type = ExtendedFieldType.Integer , DisplayName = "Number of DTC"}},
            { "MDI_DTC_LIST", new ExtendedFieldDetail() { Key = "MDI_DTC_LIST", Id = 156 , Type = ExtendedFieldType.String , DisplayName = "List of DTC(s)"}},

            { "MDI_RPM_MAX", new ExtendedFieldDetail() { Key = "MDI_RPM_MAX", Id = 157 , Type = ExtendedFieldType.Integer , DisplayName = "Max. Rpm"}},
            { "MDI_RPM_MIN", new ExtendedFieldDetail() { Key = "MDI_RPM_MIN", Id = 158 , Type = ExtendedFieldType.Integer , DisplayName = "Min. Rpm"}},
            { "MDI_RPM_AVERAGE", new ExtendedFieldDetail() { Key = "MDI_RPM_AVERAGE", Id = 159 , Type = ExtendedFieldType.Integer , DisplayName = "Avg. Rpm"}},
            { "MDI_RPM_OVER", new ExtendedFieldDetail() { Key = "MDI_RPM_OVER", Id = 160 , Type = ExtendedFieldType.Boolean , DisplayName = "Over Rpm"}},

            { "MDI_RPM_AVERAGE_RANGE_1", new ExtendedFieldDetail() { Key = "MDI_RPM_AVERAGE_RANGE_1", Id = 161 , Type = ExtendedFieldType.Integer , DisplayName = "Rpm Average\nRange 1"}},
            { "MDI_RPM_AVERAGE_RANGE_2", new ExtendedFieldDetail() { Key = "MDI_RPM_AVERAGE_RANGE_2", Id = 162 , Type = ExtendedFieldType.Integer , DisplayName = "Rpm Average Range 2"}},
            { "MDI_RPM_AVERAGE_RANGE_3", new ExtendedFieldDetail() { Key = "MDI_RPM_AVERAGE_RANGE_3", Id = 163 , Type = ExtendedFieldType.Integer , DisplayName = "Rpm Average Range 3"}},
            { "MDI_RPM_AVERAGE_RANGE_4", new ExtendedFieldDetail() { Key = "MDI_RPM_AVERAGE_RANGE_4", Id = 164 , Type = ExtendedFieldType.Integer , DisplayName = "Rpm Average Range 4"}},

            { "MDI_SENSORS_RECORDER_DATA", new ExtendedFieldDetail() { Key = "MDI_SENSORS_RECORDER_DATA", Id = 165 , Type = ExtendedFieldType.Sensor4hz , DisplayName = "Sensors recorder data"}},
            { "MDI_SENSORS_RECORDER_CALIBRATION", new ExtendedFieldDetail() { Key = "MDI_SENSORS_RECORDER_CALIBRATION", Id = 166 , Type = ExtendedFieldType.Sensor4hz , DisplayName = "Sensors recorder calibration"}},

            { "MDI_OBD_PID_1", new ExtendedFieldDetail() { Key = "MDI_OBD_PID_1", Id = 215, Type = ExtendedFieldType.String, DisplayName = "OBD PID 1" }},
            { "MDI_OBD_PID_2", new ExtendedFieldDetail() { Key = "MDI_OBD_PID_2", Id = 216, Type = ExtendedFieldType.String, DisplayName = "OBD PID 2" }},
            { "MDI_OBD_PID_3", new ExtendedFieldDetail() { Key = "MDI_OBD_PID_3", Id = 217, Type = ExtendedFieldType.String, DisplayName = "OBD PID 3" }},
            { "MDI_OBD_PID_4", new ExtendedFieldDetail() { Key = "MDI_OBD_PID_4", Id = 218, Type = ExtendedFieldType.String, DisplayName = "OBD PID 4" }},
            { "MDI_OBD_PID_5", new ExtendedFieldDetail() { Key = "MDI_OBD_PID_5", Id = 219, Type = ExtendedFieldType.String, DisplayName = "OBD PID 5" }},

            { "MDI_SQUARELL_LAST_RECORDED_MESSAGE_PART_1", new ExtendedFieldDetail() { Key = "MDI_SQUARELL_LAST_RECORDED_MESSAGE_PART_1", Id = 220, Type = ExtendedFieldType.String, DisplayName = "SQUARELL 1" }},
            { "MDI_SQUARELL_LAST_RECORDED_MESSAGE_PART_2", new ExtendedFieldDetail() { Key = "MDI_SQUARELL_LAST_RECORDED_MESSAGE_PART_2", Id = 221, Type = ExtendedFieldType.String, DisplayName = "SQUARELL 2" }},
            { "MDI_SQUARELL_LAST_RECORDED_MESSAGE_PART_3", new ExtendedFieldDetail() { Key = "MDI_SQUARELL_LAST_RECORDED_MESSAGE_PART_3", Id = 222, Type = ExtendedFieldType.String, DisplayName = "SQUARELL 3" }},


            { "MDI_DASHBOARD_MILEAGE", new ExtendedFieldDetail() { Key = "MDI_DASHBOARD_MILEAGE", Id = 223, Type = ExtendedFieldType.Integer, DisplayName = "MDI_DASHBOARD_MILEAGE)" }},
            { "MDI_DASHBOARD_FUEL", new ExtendedFieldDetail() { Key = "MDI_DASHBOARD_FUEL", Id = 224, Type = ExtendedFieldType.Integer, DisplayName = "MDI_DASHBOARD_FUEL" }},
            { "MDI_DASHBOARD_FUEL_LEVEL", new ExtendedFieldDetail() { Key = "MDI_DASHBOARD_FUEL_LEVEL", Id = 225, Type = ExtendedFieldType.Integer , DisplayName = "MDI_DASHBOARD_FUEL_LEVEL"}},
           
            { "MDI_DIAG_1", new ExtendedFieldDetail() { Key = "MDI_DIAG_1", Id = 226, Type = ExtendedFieldType.String, DisplayName = "MDI_DIAG_1" }},
            { "MDI_DIAG_2", new ExtendedFieldDetail() { Key = "MDI_DIAG_2", Id = 227, Type = ExtendedFieldType.String, DisplayName = "MDI_DIAG_2" }},
            { "MDI_DIAG_3", new ExtendedFieldDetail() { Key = "MDI_DIAG_3", Id = 228, Type = ExtendedFieldType.String , DisplayName = "MDI_DIAG_3"}},
            

            { "MDI_OBD_SPEED", new ExtendedFieldDetail() { Key = "MDI_OBD_SPEED", Id = 235, Type = ExtendedFieldType.Integer, DisplayName = "Obd Speed (km/h)" }},
            { "MDI_OBD_RPM", new ExtendedFieldDetail() { Key = "MDI_OBD_RPM", Id = 236, Type = ExtendedFieldType.Integer, DisplayName = "Obd Rpm" }},
            { "MDI_OBD_FUEL", new ExtendedFieldDetail() { Key = "MDI_OBD_FUEL", Id = 237, Type = ExtendedFieldType.Integer , DisplayName = "Obd Fuel"}},
            { "MDI_OBD_VIN", new ExtendedFieldDetail() { Key = "MDI_OBD_VIN", Id = 238, Type = ExtendedFieldType.String , DisplayName = "Obd Vin"}},
            { "MDI_OBD_MILEAGE", new ExtendedFieldDetail() { Key = "MDI_OBD_MILEAGE", Id = 239, Type = ExtendedFieldType.Integer , DisplayName = "Obd Mileage"}},

            { "MDI_JOURNEY_TIME", new ExtendedFieldDetail() { Key = "MDI_JOURNEY_TIME", Id = 240, Type = ExtendedFieldType.Second , DisplayName = "Journey Time"}},
            { "MDI_IDLE_JOURNEY", new ExtendedFieldDetail() { Key = "MDI_IDLE_JOURNEY", Id = 241, Type = ExtendedFieldType.Second , DisplayName = "Idle journey"}},
            { "MDI_DRIVING_JOURNEY", new ExtendedFieldDetail() { Key = "MDI_DRIVING_JOURNEY", Id = 242, Type = ExtendedFieldType.Second , DisplayName = "Driving journey"}},
            { "MDI_MAX_SPEED_IN_LAST_OVERSPEED", new ExtendedFieldDetail() { Key = "MDI_MAX_SPEED_IN_LAST_OVERSPEED", Id = 243,FieldDependency = "MDI_OVERSPEED", Type = ExtendedFieldType.Integer , DisplayName = "Max speed in last overspeed"}},
            { "MDI_OVERSPEED_COUNTER", new ExtendedFieldDetail() { Key = "MDI_OVERSPEED_COUNTER", Id = 244, Type = ExtendedFieldType.Integer , DisplayName = "Overspeed Counter"}},
            { "MDI_TOW_AWAY", new ExtendedFieldDetail() { Key = "MDI_TOW_AWAY", Id = 245, Type = ExtendedFieldType.Boolean , DisplayName = "Tow Away"}},
            { "MDI_ODO_JOURNEY", new ExtendedFieldDetail() { Key = "MDI_ODO_JOURNEY", Id = 246, Type = ExtendedFieldType.Integer , DisplayName = "Odo Journey"}},
            { "MDI_OVERSPEED", new ExtendedFieldDetail() { Key = "MDI_OVERSPEED", Id = 247, Type = ExtendedFieldType.Boolean , DisplayName = "Overspeed Status"}},
            { "MDI_MAX_SPEED_JOURNEY", new ExtendedFieldDetail() { Key = "MDI_MAX_SPEED_JOURNEY", Id = 248, Type = ExtendedFieldType.Integer , DisplayName = "Overspeed Max"}},
            { "MDI_JOURNEY_STATE", new ExtendedFieldDetail() { Key = "MDI_JOURNEY_STATE", Id = 249, Type = ExtendedFieldType.Integer , DisplayName = "Journey State"}},
            { "MDI_RECORD_REASON", new ExtendedFieldDetail() { Key = "MDI_RECORD_REASON", Id = 250, Type = ExtendedFieldType.String , DisplayName = "Record Reason", IgnoreInHistory = true}},
    
            { "MDI_VEHICLE_STATE", new ExtendedFieldDetail() { Key = "MDI_VEHICLE_STATE", Id = 249, Type = ExtendedFieldType.String , DisplayName = "Vehicle State"}},

            { "MDI_BOOT_REASON", new ExtendedFieldDetail() { Key = "MDI_BOOT_REASON", Id = 0, Type = ExtendedFieldType.String , DisplayName = "Boot Reason", IgnoreInHistory = true}},
            { "MDI_SHUTDOWN_REASON", new ExtendedFieldDetail() { Key = "MDI_SHUTDOWN_REASON", Id = 0, Type = ExtendedFieldType.String , DisplayName = "Shutd. Reason", IgnoreInHistory = true}},
    
      
            { "MDI_PANIC_STATE", new ExtendedFieldDetail() { Key = "MDI_PANIC_STATE", Id = 0, Type = ExtendedFieldType.Boolean , DisplayName = "Panic state"}},
            { "MDI_PANIC_MESSAGE", new ExtendedFieldDetail() { Key = "MDI_PANIC_MESSAGE", Id = 0, Type = ExtendedFieldType.String , DisplayName = "Panic Message"}},
        
            { "ENH_DASHBOARD_MILEAGE", new ExtendedFieldDetail() { Key = "ENH_DASHBOARD_MILEAGE", Id = 10223, Type = ExtendedFieldType.Integer, DisplayName = "ENH_DASHBOARD_MILEAGE)" }},
            { "ENH_DASHBOARD_FUEL", new ExtendedFieldDetail() { Key = "ENH_DASHBOARD_FUEL", Id = 10224, Type = ExtendedFieldType.Integer, DisplayName = "ENH_DASHBOARD_FUEL" }},
            { "ENH_DASHBOARD_FUEL_LEVEL", new ExtendedFieldDetail() { Key = "ENH_DASHBOARD_FUEL_LEVEL", Id = 10225, Type = ExtendedFieldType.Integer , DisplayName = "ENH_DASHBOARD_FUEL_LEVEL"}}
           
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