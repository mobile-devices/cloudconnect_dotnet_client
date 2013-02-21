using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MD.CloudConnect
{
    public interface ITracking : ICommonData
    {
        /// <summary>
        /// Date and time when the field recoreded in the unit
        /// </summary>
        DateTime Recorded_at { get; }
        /// <summary>
        /// Date and time when the field received on the Cloud servers
        /// </summary>
        DateTime Received_at { get; }

        double Longitude { get; }
        double Latitude { get; }
        /// <summary>
        /// To know if the data have a valid gps position
        /// </summary>
        bool IsValid { get; set; }

        bool IsMoving { get; set; }

        /// <summary>
        /// Speed in Knots
        /// </summary>
        double Speed { get; set; }
        /// <summary>
        /// Speed convert in Km/h
        /// </summary>
        double SpeedKmPerHour { get; set; }
        float Direction { get; set; }
        double FullOdometer { get; set; }
        /// <summary>
        /// ID send by the unit when a Key tag is plug
        /// </summary>
        string DriverId { get; set; }
        /// <summary>
        /// To know if engine is On or Off (on C4Evo, C4Max this information come from white cable on red wrapper)
        /// </summary>
        bool Ignition { get; set; }
        /// <summary>
        /// Input alarm status (orange cable on blue wrapper)
        /// </summary>
        bool Alarm { get; set; }
        /// <summary>
        /// Input status (1 for input 1, 2 for input 2 and 4 for input 3) it's a mask bit so you can have
        /// 5 to say input 1 and 3 are active
        /// </summary>
        byte Inputs { get; set; }

        bool GetFieldAsBool(string fieldName);
        string GetFieldAsString(string fieldName);
        int GetFieldAsInt(string fieldName);

        /// <summary>
        /// Check if a field is present in Json data
        /// </summary>
        bool ContainsField(string fieldName);
        /// <summary>
        /// Return all fields present in the Json data
        /// </summary>
        string[] GetFieldsPresent();
    }
}
