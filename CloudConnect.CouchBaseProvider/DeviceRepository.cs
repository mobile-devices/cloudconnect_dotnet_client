using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudConnect.CouchBaseProvider
{
    public class DeviceRepository : RepositoryBase<Device>
    {
        public void SaveDevice(Device d)
        {
            this.Create(d);
        }

        protected override string BuildKey(Device model)
        {
            return model.Imei.InflectTo().Underscored;
        }
    }
}
