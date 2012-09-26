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
        public static FieldDetail ODO_FULL = new FieldDetail() { Key = "ODO_FULL", Id = 24, Type = FieldType.Integer };
        public static FieldDetail DIO_ALARM = new FieldDetail() { Key = "DIO_ALARM", Id = 26, Type = FieldType.Boolean };
        public static FieldDetail DRIVER_ID = new FieldDetail() { Key = "DRIVER_ID", Id = 27, Type = FieldType.String };
        public static FieldDetail DIO_IN_TOR = new FieldDetail() { Key = "DIO_IN_TOR", Id = 38, Type = FieldType.Integer };
    }
}
