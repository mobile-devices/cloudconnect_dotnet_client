using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MD.CloudConnect
{
    public class Easyconnect
    {
        public static FieldDetail MDI_JOURNEY_TIME = new FieldDetail() { Key = "MDI_JOURNEY_TIME", Type = FieldType.Integer };
        public static FieldDetail MDI_IDLE_JOURNEY = new FieldDetail() { Key = "MDI_IDLE_JOURNEY", Type = FieldType.Integer };
        public static FieldDetail MDI_DRIVING_JOURNEY = new FieldDetail() { Key = "MDI_DRIVING_JOURNEY", Type = FieldType.Integer };
        public static FieldDetail MDI_OVERSPEED_COUNTER = new FieldDetail() { Key = "MDI_OVERSPEED_COUNTER", Type = FieldType.Integer };
        public static FieldDetail MDI_TOW_AWAY = new FieldDetail() { Key = "MDI_TOW_AWAY", Type = FieldType.Boolean };
        public static FieldDetail MDI_ODO_JOURNEY = new FieldDetail() { Key = "MDI_ODO_JOURNEY", Type = FieldType.Integer };
        public static FieldDetail MDI_OVERSPEED = new FieldDetail() { Key = "MDI_OVERSPEED", Type = FieldType.Boolean };
        public static FieldDetail MDI_MAX_SPEED_JOURNEY = new FieldDetail() { Key = "MDI_MAX_SPEED_JOURNEY", Type = FieldType.Integer };
        public static FieldDetail MDI_JOURNEY_STATE = new FieldDetail() { Key = "MDI_JOURNEY_STATE", Type = FieldType.Boolean };

        public static FieldDetail[] GetAllFieldsDetail()
        {
            return new FieldDetail[]
            {
                MDI_JOURNEY_TIME,
                MDI_IDLE_JOURNEY,
                MDI_DRIVING_JOURNEY,
                MDI_OVERSPEED_COUNTER,
                MDI_TOW_AWAY,
                MDI_ODO_JOURNEY,
                MDI_OVERSPEED,
                MDI_MAX_SPEED_JOURNEY,
                MDI_JOURNEY_STATE
            };
        }


        /// <summary>
        /// Time since the beginning of the Journey in milliseconds
        /// </summary>
        public static int? GetJourneyTime(ITracking trackingData)
        {
            if (trackingData == null)
                throw new NullReferenceException("ITracking is not initialize");
            if (!trackingData.ContainsField(Easyconnect.MDI_JOURNEY_TIME.Key))
                return null;
            return trackingData.GetFieldAsInt(Easyconnect.MDI_JOURNEY_TIME.Key);
        }
        /// <summary>
        /// Time since the beginning of the Journey while vehicle is stopped in milliseconds
        /// (No movement detected by accelerometers during acceleroMvtDetector.no_movement_threshold)
        /// </summary>
        public static int? GetIdleJourney(ITracking trackingData)
        {
            if (trackingData == null)
                throw new NullReferenceException("ITracking is not initialize");
            if (!trackingData.ContainsField(Easyconnect.MDI_IDLE_JOURNEY.Key))
                return null;
            return trackingData.GetFieldAsInt(Easyconnect.MDI_IDLE_JOURNEY.Key);
        }
        /// <summary>
        /// Journey driving time in milliseconds
        /// </summary>
        public static int? GetDrivingJourney(ITracking trackingData)
        {
            if (trackingData == null)
                throw new NullReferenceException("ITracking is not initialize");
            if (!trackingData.ContainsField(Easyconnect.MDI_DRIVING_JOURNEY.Key))
                return null;
            return trackingData.GetFieldAsInt(Easyconnect.MDI_DRIVING_JOURNEY.Key);
        }
        /// <summary>
        /// Number of overspeeds since the beginning of the Journey
        /// </summary>
        public static int? GetOverspeedCounter(ITracking trackingData)
        {
            if (trackingData == null)
                throw new NullReferenceException("ITracking is not initialize");
            if (!trackingData.ContainsField(Easyconnect.MDI_OVERSPEED_COUNTER.Key))
                return null;
            return trackingData.GetFieldAsInt(Easyconnect.MDI_OVERSPEED_COUNTER.Key);
        }
        /// <summary>
        /// Number of overspeeds since the beginning of the Journey
        /// </summary>
        public static bool? GetTowAway(ITracking trackingData)
        {
            if (trackingData == null)
                throw new NullReferenceException("ITracking is not initialize");
            if (!trackingData.ContainsField(Easyconnect.MDI_TOW_AWAY.Key))
                return null;
            return trackingData.GetFieldAsInt(Easyconnect.MDI_TOW_AWAY.Key) == 0 ? false : true;
        }
        /// <summary>
        /// Journey odometer in kilometers
        /// </summary>
        public static int? GetOdoJourney(ITracking trackingData)
        {
            if (trackingData == null)
                throw new NullReferenceException("ITracking is not initialize");
            if (!trackingData.ContainsField(Easyconnect.MDI_ODO_JOURNEY.Key))
                return null;
            return trackingData.GetFieldAsInt(Easyconnect.MDI_ODO_JOURNEY.Key);
        }
        /// <summary>
        /// Overspeed state – When GPS speed is higher than  overspeed.speedThreshold  
        /// (in kilometers) during overspeed.timeThreshold (in milliseconds).
        /// true if overspeed, false if not
        /// </summary>
        public static bool? GetOverspeed(ITracking trackingData)
        {
            if (trackingData == null)
                throw new NullReferenceException("ITracking is not initialize");
            if (!trackingData.ContainsField(Easyconnect.MDI_OVERSPEED.Key))
                return null;
            return trackingData.GetFieldAsInt(Easyconnect.MDI_OVERSPEED.Key) == 0 ? false : true;
        }
        /// <summary>
        /// Max speed detected since the beginning of the Journey
        /// in milliknots
        /// </summary>
        public static int? GetMaxSpeedJourney(ITracking trackingData)
        {
            if (trackingData == null)
                throw new NullReferenceException("ITracking is not initialize");
            if (!trackingData.ContainsField(Easyconnect.MDI_MAX_SPEED_JOURNEY.Key))
                return null;
            return trackingData.GetFieldAsInt(Easyconnect.MDI_MAX_SPEED_JOURNEY.Key);
        }
        /// <summary>
        /// Journey state  - on C4Evo, when Ignition is ON – On C4Dongle, when RPM is not available (No RPM or C4Dongle offline)
        /// true if Journey on, false if not
        /// </summary>
        public static bool? GetJourneyState(ITracking trackingData)
        {
            if (trackingData == null)
                throw new NullReferenceException("ITracking is not initialize");
            if (!trackingData.ContainsField(Easyconnect.MDI_JOURNEY_STATE.Key))
                return null;
            return trackingData.GetFieldAsInt(Easyconnect.MDI_JOURNEY_STATE.Key) == 0 ? false : true;
        }
    }
}
