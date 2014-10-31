using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MD.CloudConnect.Extension
{
    public class CarDiagnostic
    {
        public static FieldDetail MDI_OBD_SPEED = new FieldDetail() { Key = "MDI_OBD_SPEED", Id = 235, Type = FieldType.Integer };
        public static FieldDetail MDI_OBD_RPM = new FieldDetail() { Key = "MDI_OBD_RPM", Id = 236, Type = FieldType.Integer };
        public static FieldDetail MDI_OBD_FUEL = new FieldDetail() { Key = "MDI_OBD_FUEL", Id = 237, Type = FieldType.Integer };
        public static FieldDetail MDI_OBD_VIN = new FieldDetail() { Key = "MDI_OBD_VIN", Id = 238, Type = FieldType.String };
        public static FieldDetail MDI_OBD_MILEAGE = new FieldDetail() { Key = "MDI_OBD_MILEAGE", Id = 239, Type = FieldType.Integer };

        /// <summary>
        /// List of available Field for this module
        /// </summary>
        public static string[] GetAllFieldsDetail()
        {
            return new string[]
            {
                MDI_OBD_SPEED.Key,
                MDI_OBD_RPM.Key,
                MDI_OBD_FUEL.Key,
                MDI_OBD_VIN.Key,
                MDI_OBD_MILEAGE.Key
            };
        }

        /// <summary>
        /// Gives speed directly computed from the OBD stack (km/h)
        /// </summary>
        public static int? GetObdSpeed(ITracking trackingData)
        {
            if (trackingData == null)
                throw new NullReferenceException("ITracking is not initialize");
            if (!trackingData.ContainsField(CarDiagnostic.MDI_OBD_SPEED.Key))
                return null;
            return trackingData.GetFieldAsInt(CarDiagnostic.MDI_OBD_SPEED.Key);
        }
        /// <summary>
        /// Gives engine round per minute directly retrieved from the OBD stack (rpm)
        /// </summary>
        public static int? GetObdRpm(ITracking trackingData)
        {
            if (trackingData == null)
                throw new NullReferenceException("ITracking is not initialize");
            if (!trackingData.ContainsField(CarDiagnostic.MDI_OBD_RPM.Key))
                return null;
            return trackingData.GetFieldAsInt(CarDiagnostic.MDI_OBD_RPM.Key);
        }
        /// <summary>
        /// Gives total fuel consumption directly computed from the OBD stack (fuel consumption is estimated) (Liter)
        /// </summary>
        public static int? GetObdFuel(ITracking trackingData)
        {
            if (trackingData == null)
                throw new NullReferenceException("ITracking is not initialize");

            if (!trackingData.ContainsField(CarDiagnostic.MDI_OBD_FUEL.Key))
                return null;
            return trackingData.GetFieldAsInt(CarDiagnostic.MDI_OBD_FUEL.Key);
        }
        /// <summary>
        ///Returns the vehicle number identification retrieved from the OBD stack
        /// </summary>
        public static string GetObdVin(ITracking trackingData)
        {
            if (trackingData == null)
                throw new NullReferenceException("ITracking is not initialize");
            if (!trackingData.ContainsField(CarDiagnostic.MDI_OBD_VIN.Key))
                return null;
            return trackingData.GetFieldAsString(CarDiagnostic.MDI_OBD_VIN.Key);
        }
        /// <summary>
        /// Returns the total mileage driven since the device was plugged in. Computed from the OBD stack (Km)
        /// </summary>
        public static int? GetObdMileage(ITracking trackingData)
        {
            if (trackingData == null)
                throw new NullReferenceException("ITracking is not initialize");
            if (!trackingData.ContainsField(CarDiagnostic.MDI_OBD_MILEAGE.Key))
                return null;
            return trackingData.GetFieldAsInt(CarDiagnostic.MDI_OBD_MILEAGE.Key);
        }
    }
}
