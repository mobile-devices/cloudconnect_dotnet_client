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
        string Id { get; }
        string Id_str { get; }
    }
}
