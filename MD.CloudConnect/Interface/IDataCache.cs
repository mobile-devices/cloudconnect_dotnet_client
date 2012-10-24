using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MD.CloudConnect
{
    public interface IDataCache
    {
        DateTime getHistoryFor(string asset, ITracking data);
    }
}
