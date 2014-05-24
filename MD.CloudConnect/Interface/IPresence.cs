using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MD.CloudConnect.Interface
{
    public interface IPresence : ICommonData
    {
        /// <summary>
        /// Date and time when the field recoreded in the unit
        /// </summary>
        DateTime? Time { get; }

        string Reason { get; }
        string Type { get; }
    }
}
