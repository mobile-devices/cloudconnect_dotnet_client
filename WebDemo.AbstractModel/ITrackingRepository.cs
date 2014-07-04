using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebDemo.AbstractModel
{
    public interface ITrackingRepository : IRepository<Track>
    {
        IEnumerable<Track> GetData(Device device, DateTime date);
    }
}
