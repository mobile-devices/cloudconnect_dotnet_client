using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebDemo.AbstractModel
{
    public interface IRepositoryFactory
    {
        IRepository<Device> DeviceRepository { get; }
        ITrackingRepository TrackingRepository { get; }
    }

}
