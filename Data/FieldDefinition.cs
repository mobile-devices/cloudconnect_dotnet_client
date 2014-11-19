using System;
using System.Collections.Generic;
using System.Linq;
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

    public enum Soure
    {
        Asset,
        Cloud
    }

    public struct FieldDetail
    {
        /// <summary>
        /// String identifier (fieldname)
        /// </summary>
        public string Key;
        /// <summary>
        /// base type
        /// </summary>
        public FieldType Type;
        /// <summary>
        /// Field created by the device or by the cloud
        /// </summary>
        public Soure Source;
        /// <summary>
        /// 
        /// </summary>
        public bool IsEvent;
        /// <summary>
        /// 
        /// </summary>
        public string FieldNameDependency;
    }


    public class FieldDefinition
    {
        public static FieldDetail GPRMC_VALID = new FieldDetail() { Key = "GPRMC_VALID", Type = FieldType.Boolean };
        public static FieldDetail GPS_SPEED = new FieldDetail() { Key = "GPS_SPEED", Type = FieldType.Integer };
        public static FieldDetail GPS_DIR = new FieldDetail() { Key = "GPS_DIR", Type = FieldType.Integer };
        public static FieldDetail DIO_IGNITION = new FieldDetail() { Key = "DIO_IGNITION", Type = FieldType.Boolean };
        public static FieldDetail BATT = new FieldDetail() { Key = "BATT", Type = FieldType.Integer };
        public static FieldDetail GPRS_HEADER = new FieldDetail() { Key = "GPRS_HEADER", Type = FieldType.Integer };
        public static FieldDetail RSSI = new FieldDetail() { Key = "RSSI", Type = FieldType.Integer };
        public static FieldDetail ODO_FULL = new FieldDetail() { Key = "ODO_FULL", Type = FieldType.Integer };
        public static FieldDetail DIO_ALARM = new FieldDetail() { Key = "DIO_ALARM", Type = FieldType.Boolean };
        public static FieldDetail DRIVER_ID = new FieldDetail() { Key = "DRIVER_ID", Type = FieldType.String };
        public static FieldDetail DIO_IN_TOR = new FieldDetail() { Key = "DIO_IN_TOR", Type = FieldType.Integer };
        public static FieldDetail TACHOGRAPH_FIRST_DRIVER_STATE = new FieldDetail() { Key = "TACHOGRAPH_FIRST_DRIVER_STATE", Type = FieldType.String };
        public static FieldDetail TACHOGRAPH_FIRST_DRIVER_DRIVING_STATE = new FieldDetail() { Key = "TACHOGRAPH_FIRST_DRIVER_DRIVING_STATE", Type = FieldType.Integer };
        public static FieldDetail TACHOGRAPH_DAILYMETER = new FieldDetail() { Key = "TACHOGRAPH_DAILYMETER", Type = FieldType.Integer };
        public static FieldDetail TACHOGRAPH_ODOMETER = new FieldDetail() { Key = "TACHOGRAPH_ODOMETER", Type = FieldType.Integer };
        public static FieldDetail ODO_PARTIAL_KM = new FieldDetail() { Key = "ODO_PARTIAL_KM", Type = FieldType.String };
        public static FieldDetail TACHOGRAPH_DRIVING_TIME = new FieldDetail() { Key = "TACHOGRAPH_DRIVING_TIME", Type = FieldType.Integer };
        public static FieldDetail MVT_STATE = new FieldDetail() { Key = "MVT_STATE", Type = FieldType.Boolean };
        public static FieldDetail MDI_EXT_BATT_LOW = new FieldDetail() { Key = "MDI_EXT_BATT_LOW", Type = FieldType.Boolean };
        public static FieldDetail MDI_EXT_BATT_VOLTAGE = new FieldDetail() { Key = "MDI_EXT_BATT_VOLTAGE", Type = FieldType.Integer };
    }
}
