using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace MD.CloudConnect
{
    public interface ICommonData
    {
        string Asset { get; }
        UInt64 Id { get; }
        string Id_str { get; }
    }
}
