using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudConnect.CouchBaseProvider
{
    public interface IModelBase
    {
        string Id { get; set; }
        string Type { get; }
    }
}
