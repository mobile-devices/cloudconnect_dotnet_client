using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudConnect.CouchBaseProvider
{
    public class RepositoryFactory : WebDemo.AbstractModel.IRepositoryFactory
    {
        public WebDemo.AbstractModel.IRepository<WebDemo.AbstractModel.Device> DeviceRepository
        {
            get { return new DeviceRepository() as WebDemo.AbstractModel.IRepository<WebDemo.AbstractModel.Device>; }
        }

        public WebDemo.AbstractModel.ITrackingRepository TrackingRepository
        {
            get { return new TrackingRepository() as WebDemo.AbstractModel.ITrackingRepository; }
        }
    }
}
