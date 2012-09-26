using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MD.CloudConnect
{
    public class EasyFleet
    {
        public static FieldDetail MDI_OBD_PID = new FieldDetail() { Key = "MDI_OBD_PID", Id = 212, Type = FieldType.String };
        public static FieldDetail MDI_JOURNEY_TIME = new FieldDetail() { Key = "MDI_JOURNEY_TIME", Id = 240, Type = FieldType.Integer };
        public static FieldDetail MDI_IDLE_JOURNEY = new FieldDetail() { Key = "MDI_IDLE_JOURNEY", Id = 241, Type = FieldType.Integer };
        public static FieldDetail MDI_DRIVING_JOURNEY = new FieldDetail() { Key = "MDI_DRIVING_JOURNEY", Id = 242, Type = FieldType.Integer };
        public static FieldDetail MDI_HASHBRAKING = new FieldDetail() { Key = "MDI_HASHBRAKING", Id = 243, Type = FieldType.Boolean };
        public static FieldDetail MDI_OVERSPEED_COUNTER = new FieldDetail() { Key = "MDI_OVERSPEED_COUNTER", Id = 244, Type = FieldType.Integer };
        public static FieldDetail MDI_TOW_AWAY = new FieldDetail() { Key = "MDI_TOW_AWAY", Id = 245, Type = FieldType.Integer };
        public static FieldDetail MDI_ODO_JOURNEY = new FieldDetail() { Key = "MDI_ODO_JOURNEY", Id = 246, Type = FieldType.Integer };
        public static FieldDetail MDI_OVERSPEED = new FieldDetail() { Key = "MDI_OVERSPEED", Id = 247, Type = FieldType.Boolean };
        public static FieldDetail MDI_MAX_SPEED_JOURNEY = new FieldDetail() { Key = "MDI_MAX_SPEED_JOURNEY", Id = 248, Type = FieldType.Integer };
        public static FieldDetail MDI_JOURNEY_STATE = new FieldDetail() { Key = "MDI_JOURNEY_STATE", Id = 249, Type = FieldType.Boolean };

        /*   212 MDI_OBD_PID OBD data (configurable PIDs) String
         *   240 MDI_JOURNEY_TIME Time since the beginning of the Journey in milliseconds
         *   241 MDI_IDLE_JOURNEY Time since the beginning of the Journey while vehicle is stopped 
         *       (No movement detected by accelerometers during acceleroMvtDetector.no_movement_threshold  (in seconds) in milliseconds
         *   242 MDI_DRIVING_JOURNEY Journey driving time in milliseconds
         *   243 MDI_HASHBRAKING (available in August) Hashbraking 1 if Hashbraking, 0 if not
         *   244 MDI_OVERSPEED_COUNTER Number of overspeeds since the beginning of the Journey Int
         *   245 MDI_TOW_AWAY Tow away detection – Tow away state is detected on when Ignition is Off and accelerometers detect a movement.
         *       1 if tow away detected, 0 if not
         *   246 MDI_ODO_JOURNEY Journey odometer in kilometers
         *   247 MDI_OVERSPEED Overspeed state – When GPS speed is higher than  overspeed.speedThreshold  (in kilometers) 
         *       during overspeed.timeThreshold (in milliseconds).1 if overspeeding, 0 if not
         *   248 MDI_MAX_SPEED_JOURNEY Max speed detected since the beginning of the Journey in milliknots
         *   249 MDI_JOURNEY_STATE Journey state  - on C4Evo, when Ignition is ON – On C4Dongle, 
         *       when RPM is not available (No RPM or C4Dongle offline) 1 if Journey on, 0 if n
         */

        public static FieldDetail[] GetAllFieldsDetailOfEasyFleet()
        {
            return new FieldDetail[]
            {
                MDI_OBD_PID,
                MDI_JOURNEY_TIME,
                MDI_IDLE_JOURNEY,
                MDI_DRIVING_JOURNEY,
                MDI_HASHBRAKING,
                MDI_OVERSPEED_COUNTER,
                MDI_TOW_AWAY,
                MDI_ODO_JOURNEY,
                MDI_OVERSPEED,
                MDI_MAX_SPEED_JOURNEY,
                MDI_JOURNEY_STATE
            };
        }

        /// <summary>
        /// OBD data (configurable PIDs)
        /// </summary>        
        public static string GetObdPid(ITracking trackingData)
        {
            if (trackingData == null)
                throw new NullReferenceException("ITracking is not initialize");
            return trackingData.GetFieldAsString(EasyFleet.MDI_OBD_PID.Key);
        }
        /// <summary>
        /// Time since the beginning of the Journey in milliseconds
        /// </summary>
        public static int GetJourneyTime(ITracking trackingData)
        {
            if (trackingData == null)
                throw new NullReferenceException("ITracking is not initialize");
            return trackingData.GetFieldAsInt(EasyFleet.MDI_JOURNEY_TIME.Key);
        }
        /// <summary>
        /// Time since the beginning of the Journey while vehicle is stopped in milliseconds
        /// (No movement detected by accelerometers during acceleroMvtDetector.no_movement_threshold)
        /// </summary>
        public static int GetIdleJourney(ITracking trackingData)
        {
            if (trackingData == null)
                throw new NullReferenceException("ITracking is not initialize");
            return trackingData.GetFieldAsInt(EasyFleet.MDI_IDLE_JOURNEY.Key);
        }
        /// <summary>
        /// Journey driving time in milliseconds
        /// </summary>
        public static int GetDrivingJourney(ITracking trackingData)
        {
            if (trackingData == null)
                throw new NullReferenceException("ITracking is not initialize");
            return trackingData.GetFieldAsInt(EasyFleet.MDI_DRIVING_JOURNEY.Key);
        }
        /// <summary>
        /// true if Hashbraking, false if not
        /// </summary>
        public static bool GetHashbraking(ITracking trackingData)
        {
            if (trackingData == null)
                throw new NullReferenceException("ITracking is not initialize");
            return trackingData.GetFieldAsInt(EasyFleet.MDI_HASHBRAKING.Key) == 0 ? false : true;
        }
        /// <summary>
        /// Number of overspeeds since the beginning of the Journey
        /// </summary>
        public static int GetOverspeedCounter(ITracking trackingData)
        {
            if (trackingData == null)
                throw new NullReferenceException("ITracking is not initialize");
            return trackingData.GetFieldAsInt(EasyFleet.MDI_OVERSPEED_COUNTER.Key);
        }
        /// <summary>
        /// Number of overspeeds since the beginning of the Journey
        /// </summary>
        public static int GetTowAway(ITracking trackingData)
        {
            if (trackingData == null)
                throw new NullReferenceException("ITracking is not initialize");
            return trackingData.GetFieldAsInt(EasyFleet.MDI_TOW_AWAY.Key);
        }
        /// <summary>
        /// Journey odometer in kilometers
        /// </summary>
        public static int GetOdoJourney(ITracking trackingData)
        {
            if (trackingData == null)
                throw new NullReferenceException("ITracking is not initialize");
            return trackingData.GetFieldAsInt(EasyFleet.MDI_ODO_JOURNEY.Key);
        }
        /// <summary>
        /// Overspeed state – When GPS speed is higher than  overspeed.speedThreshold  
        /// (in kilometers) during overspeed.timeThreshold (in milliseconds).
        /// true if overspeed, false if not
        /// </summary>
        public static bool GetOverspeed(ITracking trackingData)
        {
            if (trackingData == null)
                throw new NullReferenceException("ITracking is not initialize");
            return trackingData.GetFieldAsInt(EasyFleet.MDI_OVERSPEED.Key) == 0 ? false : true;
        }
        /// <summary>
        /// Max speed detected since the beginning of the Journey
        /// in milliknots
        /// </summary>
        public static int GetMaxSpeedJourney(ITracking trackingData)
        {
            if (trackingData == null)
                throw new NullReferenceException("ITracking is not initialize");
            return trackingData.GetFieldAsInt(EasyFleet.MDI_MAX_SPEED_JOURNEY.Key);
        }
        /// <summary>
        /// Journey state  - on C4Evo, when Ignition is ON – On C4Dongle, when RPM is not available (No RPM or C4Dongle offline)
        /// true if Journey on, false if not
        /// </summary>
        public static bool GetJourneyState(ITracking trackingData)
        {
            if (trackingData == null)
                throw new NullReferenceException("ITracking is not initialize");
            return trackingData.GetFieldAsInt(EasyFleet.MDI_JOURNEY_STATE.Key) == 0 ? false : true;
        }
    }
}
