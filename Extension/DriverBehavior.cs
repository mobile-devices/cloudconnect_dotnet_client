using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MD.CloudConnect.Extension
{
    public class DriverBehavior
    {
        public static FieldDetail BEHAVE_ID = new FieldDetail() { Key = "BEHAVE_ID", Type = FieldType.Integer };
        public static FieldDetail BEHAVE_LONG = new FieldDetail() { Key = "BEHAVE_LONG", Type = FieldType.Integer };
        public static FieldDetail BEHAVE_LAT = new FieldDetail() { Key = "BEHAVE_LAT", Type = FieldType.Integer };
        public static FieldDetail BEHAVE_DAY_OF_YEAR = new FieldDetail() { Key = "BEHAVE_DAY_OF_YEAR", Type = FieldType.Integer };
        public static FieldDetail BEHAVE_TIME_OF_DAY = new FieldDetail() { Key = "BEHAVE_TIME_OF_DAY",  Type = FieldType.Integer };
        public static FieldDetail BEHAVE_GPS_SPEED_BEGIN = new FieldDetail() { Key = "BEHAVE_GPS_SPEED_BEGIN", Type = FieldType.Integer };
        public static FieldDetail BEHAVE_GPS_SPEED_PEAK = new FieldDetail() { Key = "BEHAVE_GPS_SPEED_PEAK",  Type = FieldType.Integer };
        public static FieldDetail BEHAVE_GPS_SPEED_END = new FieldDetail() { Key = "BEHAVE_GPS_SPEED_END", Type = FieldType.Integer };
        public static FieldDetail BEHAVE_GPS_HEADING_BEGIN = new FieldDetail() { Key = "BEHAVE_GPS_HEADING_BEGIN", Type = FieldType.Integer };
        public static FieldDetail BEHAVE_GPS_HEADING_PEAK = new FieldDetail() { Key = "BEHAVE_GPS_HEADING_PEAK",  Type = FieldType.Integer };
        public static FieldDetail BEHAVE_GPS_HEADING_END = new FieldDetail() { Key = "BEHAVE_GPS_HEADING_END",  Type = FieldType.Integer };
        public static FieldDetail BEHAVE_ACC_X_BEGIN = new FieldDetail() { Key = "BEHAVE_ACC_X_BEGIN", Type = FieldType.Integer };
        public static FieldDetail BEHAVE_ACC_X_PEAK = new FieldDetail() { Key = "BEHAVE_ACC_X_PEAK", Type = FieldType.Integer };
        public static FieldDetail BEHAVE_ACC_X_END = new FieldDetail() { Key = "BEHAVE_ACC_X_END", Type = FieldType.Integer };
        public static FieldDetail BEHAVE_ACC_Y_BEGIN = new FieldDetail() { Key = "BEHAVE_ACC_Y_BEGIN",  Type = FieldType.Integer };
        public static FieldDetail BEHAVE_ACC_Y_PEAK = new FieldDetail() { Key = "BEHAVE_ACC_Y_PEAK", Type = FieldType.Integer };
        public static FieldDetail BEHAVE_ACC_Y_END = new FieldDetail() { Key = "BEHAVE_ACC_Y_END", Type = FieldType.Integer };
        public static FieldDetail BEHAVE_ACC_Z_BEGIN = new FieldDetail() { Key = "BEHAVE_ACC_Z_BEGIN", Type = FieldType.Integer };
        public static FieldDetail BEHAVE_ACC_Z_PEAK = new FieldDetail() { Key = "BEHAVE_ACC_Z_PEAK", Type = FieldType.Integer };
        public static FieldDetail BEHAVE_ACC_Z_END = new FieldDetail() { Key = "BEHAVE_ACC_Z_PEAK", Type = FieldType.Integer };
        public static FieldDetail BEHAVE_ELAPSED = new FieldDetail() { Key = "BEHAVE_ELAPSED",  Type = FieldType.Integer };
        public static FieldDetail BEHAVE_UNIQUE_ID = new FieldDetail() { Key = "BEHAVE_UNIQUE_ID", Type = FieldType.Integer };

        /// <summary>
        /// List of available Field for this module
        /// </summary>
        public static string[] GetAllFieldsDetail()
        {
            return new string[]
            {
                BEHAVE_ID.Key,
                BEHAVE_LONG.Key,
                BEHAVE_LAT.Key,
                BEHAVE_DAY_OF_YEAR.Key,
                BEHAVE_TIME_OF_DAY.Key,
                BEHAVE_GPS_SPEED_BEGIN.Key,
                BEHAVE_GPS_SPEED_PEAK.Key,
                BEHAVE_GPS_SPEED_END.Key,
                BEHAVE_GPS_HEADING_BEGIN.Key,
                BEHAVE_GPS_HEADING_PEAK.Key,
                BEHAVE_GPS_HEADING_END.Key,
                BEHAVE_ACC_X_BEGIN.Key,
                BEHAVE_ACC_X_PEAK.Key,
                BEHAVE_ACC_X_END.Key,
                BEHAVE_ACC_Y_BEGIN.Key,
                BEHAVE_ACC_Y_PEAK.Key,
                BEHAVE_ACC_Y_END.Key,
                BEHAVE_ACC_Z_BEGIN.Key,
                BEHAVE_ACC_Z_PEAK.Key,
                BEHAVE_ACC_Z_END.Key,
                BEHAVE_ELAPSED.Key,
                BEHAVE_UNIQUE_ID.Key
            };
        }

        /// <summary>
        /// Pattern id
        /// 
        /// Example : 
        /// 
        /// Id : 10 => Harsh braking
        /// Id : 11 => Harsh acceleration
        /// Id : 12 => Left turn
        /// Id : 13 => Right turn
        /// </summary>
        public static int? GetId(ITracking trackingData)
        {
            if (trackingData == null)
                throw new NullReferenceException("ITracking is not initialize");
            if (!trackingData.ContainsField(DriverBehavior.BEHAVE_ID.Key))
                return null;
            return trackingData.GetFieldAsInt(DriverBehavior.BEHAVE_ID.Key);
        }
        /// <summary>
        /// Longitude in degrees
        /// </summary>
        public static double? GetLongitude(ITracking trackingData)
        {
            if (trackingData == null)
                throw new NullReferenceException("ITracking is not initialize");
            if (!trackingData.ContainsField(DriverBehavior.BEHAVE_LONG.Key))
                return null;
            return trackingData.GetFieldAsInt(DriverBehavior.BEHAVE_LONG.Key) / 100000.0;
        }
        /// <summary>
        /// Latitude in degrees
        /// </summary>
        public static double? GetLatitude(ITracking trackingData)
        {
            if (trackingData == null)
                throw new NullReferenceException("ITracking is not initialize");
            if (!trackingData.ContainsField(DriverBehavior.BEHAVE_LAT.Key))
                return null;
            return trackingData.GetFieldAsInt(DriverBehavior.BEHAVE_LAT.Key) / 100000.0;
        }
        /// <summary>
        /// Day when the event occurred	
        /// </summary>
        /// <returns>25/10/2012 will be 121025</returns>
        public static int? GetDay(ITracking trackingData)
        {
            if (trackingData == null)
                throw new NullReferenceException("ITracking is not initialize");
            if (!trackingData.ContainsField(DriverBehavior.BEHAVE_DAY_OF_YEAR.Key))
                return null;
            return trackingData.GetFieldAsInt(DriverBehavior.BEHAVE_DAY_OF_YEAR.Key);
        }
        /// <summary>
        /// Time when the event occurred	
        /// </summary>
        /// <returns>14h35 12s will be 143512</returns>
        public static int? GetTime(ITracking trackingData)
        {
            if (trackingData == null)
                throw new NullReferenceException("ITracking is not initialize");
            if (!trackingData.ContainsField(DriverBehavior.BEHAVE_TIME_OF_DAY.Key))
                return null;
            return trackingData.GetFieldAsInt(DriverBehavior.BEHAVE_TIME_OF_DAY.Key);
        }
        /// <summary>
        /// Speed over ground		
        /// </summary>
        /// <returns>Knots * 10^3</returns>
        public static int? GetSpeedBegin(ITracking trackingData)
        {
            if (trackingData == null)
                throw new NullReferenceException("ITracking is not initialize");
            if (!trackingData.ContainsField(DriverBehavior.BEHAVE_GPS_SPEED_BEGIN.Key))
                return null;
            return trackingData.GetFieldAsInt(DriverBehavior.BEHAVE_GPS_SPEED_BEGIN.Key);
        }
        /// <summary>
        /// Speed over ground		
        /// </summary>
        /// <returns>Knots * 10^3</returns>
        public static int? GetSpeedPeak(ITracking trackingData)
        {
            if (trackingData == null)
                throw new NullReferenceException("ITracking is not initialize");
            if (!trackingData.ContainsField(DriverBehavior.BEHAVE_GPS_SPEED_PEAK.Key))
                return null;
            return trackingData.GetFieldAsInt(DriverBehavior.BEHAVE_GPS_SPEED_PEAK.Key);
        }
        /// <summary>
        /// Speed over ground		
        /// </summary>
        /// <returns>Knots * 10^3</returns>
        public static int? GetSpeedEnd(ITracking trackingData)
        {
            if (trackingData == null)
                throw new NullReferenceException("ITracking is not initialize");
            if (!trackingData.ContainsField(DriverBehavior.BEHAVE_GPS_SPEED_END.Key))
                return null;
            return trackingData.GetFieldAsInt(DriverBehavior.BEHAVE_GPS_SPEED_END.Key);
        }
        /// <summary>
        /// Get course of degrees
        /// </summary>
        public static double? GetHeadingBegin(ITracking trackingData)
        {
            if (trackingData == null)
                throw new NullReferenceException("ITracking is not initialize");
            if (!trackingData.ContainsField(DriverBehavior.BEHAVE_GPS_HEADING_BEGIN.Key))
                return null;
            return trackingData.GetFieldAsInt(DriverBehavior.BEHAVE_GPS_HEADING_BEGIN.Key) / 1000.0;
        }
        /// <summary>
        /// Get course of degrees
        /// </summary>
        public static double? GetHeadingPeak(ITracking trackingData)
        {
            if (trackingData == null)
                throw new NullReferenceException("ITracking is not initialize");
            if (!trackingData.ContainsField(DriverBehavior.BEHAVE_GPS_HEADING_PEAK.Key))
                return null;
            return trackingData.GetFieldAsInt(DriverBehavior.BEHAVE_GPS_HEADING_PEAK.Key) / 1000.0;
        }
        /// <summary>
        /// Get course of degrees
        /// </summary>
        public static double? GetHeadingEnd(ITracking trackingData)
        {
            if (trackingData == null)
                throw new NullReferenceException("ITracking is not initialize");
            if (!trackingData.ContainsField(DriverBehavior.BEHAVE_GPS_HEADING_END.Key))
                return null;
            return trackingData.GetFieldAsInt(DriverBehavior.BEHAVE_GPS_HEADING_END.Key) / 1000.0;
        }
        /// <summary>
        /// X-axis value at the begin of pattern detection
        /// </summary>
        /// <returns>mG</returns>
        public static int? GetAccXBegin(ITracking trackingData)
        {
            if (trackingData == null)
                throw new NullReferenceException("ITracking is not initialize");
            if (!trackingData.ContainsField(DriverBehavior.BEHAVE_ACC_X_BEGIN.Key))
                return null;
            return trackingData.GetFieldAsInt(DriverBehavior.BEHAVE_ACC_X_BEGIN.Key);
        }
        /// <summary>
        /// X-axis value at the begin of pattern detection
        /// </summary>
        /// <returns>mG</returns>
        public static int? GetAccXPeak(ITracking trackingData)
        {
            if (trackingData == null)
                throw new NullReferenceException("ITracking is not initialize");
            if (!trackingData.ContainsField(DriverBehavior.BEHAVE_ACC_X_PEAK.Key))
                return null;
            return trackingData.GetFieldAsInt(DriverBehavior.BEHAVE_ACC_X_PEAK.Key);
        }
        /// <summary>
        /// X-axis value at the begin of pattern detection
        /// </summary>
        /// <returns>mG</returns>
        public static int? GetAccXEnd(ITracking trackingData)
        {
            if (trackingData == null)
                throw new NullReferenceException("ITracking is not initialize");
            if (!trackingData.ContainsField(DriverBehavior.BEHAVE_ACC_X_END.Key))
                return null;
            return trackingData.GetFieldAsInt(DriverBehavior.BEHAVE_ACC_X_END.Key);
        }
        /// <summary>
        /// Y-axis value at the begin of pattern detection
        /// </summary>
        /// <returns>mG</returns>
        public static int? GetAccYBegin(ITracking trackingData)
        {
            if (trackingData == null)
                throw new NullReferenceException("ITracking is not initialize");
            if (!trackingData.ContainsField(DriverBehavior.BEHAVE_ACC_Y_BEGIN.Key))
                return null;
            return trackingData.GetFieldAsInt(DriverBehavior.BEHAVE_ACC_Y_BEGIN.Key);
        }
        /// <summary>
        /// Y-axis value at the begin of pattern detection
        /// </summary>
        /// <returns>mG</returns>
        public static int? GetAccYPeak(ITracking trackingData)
        {
            if (trackingData == null)
                throw new NullReferenceException("ITracking is not initialize");
            if (!trackingData.ContainsField(DriverBehavior.BEHAVE_ACC_Y_PEAK.Key))
                return null;
            return trackingData.GetFieldAsInt(DriverBehavior.BEHAVE_ACC_Y_PEAK.Key);
        }
        /// <summary>
        /// Y-axis value at the begin of pattern detection
        /// </summary>
        /// <returns>mG</returns>
        public static int? GetAccYEnd(ITracking trackingData)
        {
            if (trackingData == null)
                throw new NullReferenceException("ITracking is not initialize");
            if (!trackingData.ContainsField(DriverBehavior.BEHAVE_ACC_Y_END.Key))
                return null;
            return trackingData.GetFieldAsInt(DriverBehavior.BEHAVE_ACC_Y_END.Key);
        }
        /// <summary>
        /// Z-axis value at the begin of pattern detection
        /// </summary>
        /// <returns>mG</returns>
        public static int? GetAccZBegin(ITracking trackingData)
        {
            if (trackingData == null)
                throw new NullReferenceException("ITracking is not initialize");
            if (!trackingData.ContainsField(DriverBehavior.BEHAVE_ACC_Z_BEGIN.Key))
                return null;
            return trackingData.GetFieldAsInt(DriverBehavior.BEHAVE_ACC_Z_BEGIN.Key);
        }
        /// <summary>
        /// Z-axis value at the begin of pattern detection
        /// </summary>
        /// <returns>mG</returns>
        public static int? GetAccZPeak(ITracking trackingData)
        {
            if (trackingData == null)
                throw new NullReferenceException("ITracking is not initialize");
            if (!trackingData.ContainsField(DriverBehavior.BEHAVE_ACC_Z_PEAK.Key))
                return null;
            return trackingData.GetFieldAsInt(DriverBehavior.BEHAVE_ACC_Z_PEAK.Key);
        }
        /// <summary>
        /// Z-axis value at the begin of pattern detection
        /// </summary>
        /// <returns>mG</returns>
        public static int? GetAccZEnd(ITracking trackingData)
        {
            if (trackingData == null)
                throw new NullReferenceException("ITracking is not initialize");
            if (!trackingData.ContainsField(DriverBehavior.BEHAVE_ACC_Z_END.Key))
                return null;
            return trackingData.GetFieldAsInt(DriverBehavior.BEHAVE_ACC_Z_END.Key);
        }
        /// <summary>
        /// Pattern duration
        /// </summary>
        /// <returns>millisecond</returns>
        public static int? GetElapsed(ITracking trackingData)
        {
            if (trackingData == null)
                throw new NullReferenceException("ITracking is not initialize");
            if (!trackingData.ContainsField(DriverBehavior.BEHAVE_ELAPSED.Key))
                return null;
            return trackingData.GetFieldAsInt(DriverBehavior.BEHAVE_ELAPSED.Key);
        }
        /// <summary>
        /// unique event id per session (reset at reboot)
        /// </summary>
        public static int? GetUniqueID(ITracking trackingData)
        {
            if (trackingData == null)
                throw new NullReferenceException("ITracking is not initialize");
            if (!trackingData.ContainsField(DriverBehavior.BEHAVE_UNIQUE_ID.Key))
                return null;
            return trackingData.GetFieldAsInt(DriverBehavior.BEHAVE_UNIQUE_ID.Key);
        }
    }
}
