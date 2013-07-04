using System;
using System.Collections.Generic;
using System.Text;

namespace MD.CloudConnect
{
    public enum FieldType
    {
        String,
        Integer,
        Boolean,
        Unknown
    }

    public struct FieldDetail
    {
        public string Key;
        public int Id;
        public FieldType Type;
    }


    public class FieldDefinition
    {
        public static FieldDetail GPRMC_VALID = new FieldDetail() { Key = "GPRMC_VALID", Id = 3, Type = FieldType.Boolean };
        public static FieldDetail GPS_SPEED = new FieldDetail() { Key = "GPS_SPEED", Id = 8, Type = FieldType.Integer };
        public static FieldDetail GPS_DIR = new FieldDetail() { Key = "GPS_DIR", Id = 9, Type = FieldType.Integer };

        public static FieldDetail DIO_IGNITION = new FieldDetail() { Key = "DIO_IGNITION", Id = 14, Type = FieldType.Boolean };

        public static FieldDetail BATT = new FieldDetail() { Key = "BATT", Id = 15, Type = FieldType.Integer };
        public static FieldDetail GPRS_HEADER = new FieldDetail() { Key = "GPRS_HEADER", Id = 16, Type = FieldType.Integer };
        public static FieldDetail RSSI = new FieldDetail() { Key = "RSSI", Id = 17, Type = FieldType.Integer };
        public static FieldDetail ODO_FULL = new FieldDetail() { Key = "ODO_FULL", Id = 24, Type = FieldType.Integer };
        public static FieldDetail DIO_ALARM = new FieldDetail() { Key = "DIO_ALARM", Id = 26, Type = FieldType.Boolean };
        public static FieldDetail DRIVER_ID = new FieldDetail() { Key = "DRIVER_ID", Id = 27, Type = FieldType.String };
        public static FieldDetail DIO_IN_TOR = new FieldDetail() { Key = "DIO_IN_TOR", Id = 38, Type = FieldType.Integer };

        public static FieldDetail TACHOGRAPH_FIRST_DRIVER_STATE = new FieldDetail() { Key = "TACHOGRAPH_FIRST_DRIVER_STATE", Id = 19, Type = FieldType.String };
        public static FieldDetail TACHOGRAPH_FIRST_DRIVER_DRIVING_STATE = new FieldDetail() { Key = "TACHOGRAPH_FIRST_DRIVER_DRIVING_STATE", Id = 20, Type = FieldType.Integer };
        public static FieldDetail TACHOGRAPH_DAILYMETER = new FieldDetail() { Key = "TACHOGRAPH_DAILYMETER", Id = 21, Type = FieldType.Integer };
        public static FieldDetail TACHOGRAPH_ODOMETER = new FieldDetail() { Key = "TACHOGRAPH_ODOMETER", Id = 22, Type = FieldType.Integer };
        public static FieldDetail ODO_PARTIAL_KM = new FieldDetail() { Key = "ODO_PARTIAL_KM", Id = 23, Type = FieldType.String };
        public static FieldDetail TACHOGRAPH_DRIVING_TIME = new FieldDetail() { Key = "TACHOGRAPH_DRIVING_TIME", Id = 17, Type = FieldType.Integer };

        public static FieldDetail MVT_STATE = new FieldDetail() { Key = "MVT_STATE", Id = 56, Type = FieldType.Boolean };

        public static FieldDetail MDI_EXT_BATT_LOW = new FieldDetail() { Key = "MDI_EXT_BATT_LOW", Id = 150, Type = FieldType.Boolean };
        public static FieldDetail MDI_EXT_BATT_VOLTAGE = new FieldDetail() { Key = "MDI_EXT_BATT_VOLTAGE", Id = 56, Type = FieldType.Integer };
    }
}
