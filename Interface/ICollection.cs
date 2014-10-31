using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MD.CloudConnect.Data;

namespace MD.CloudConnect
{
    public interface ICollection : ICommonData
    {
        DateTime? Start_at { get; }
        DateTime? Stop_at { get; }
        string Name { get; }
        IEnumerable<ITracking> Tracks { get; }
        IEnumerable<IMessage> Messages { get; }
    }
}
