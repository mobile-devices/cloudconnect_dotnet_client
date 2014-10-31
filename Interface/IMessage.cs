using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MD.CloudConnect
{
    public interface IMessage : ICommonData
    {
        /// <summary>
        /// Date and time when the field recoreded in the unit
        /// </summary>
        DateTime? Recorded_at { get; }
        /// <summary>
        /// Date and time when the field received on the Cloud servers
        /// </summary>
        DateTime? Received_at { get; }

        string Channel { get; }
        string Message { get; }
    }
}
