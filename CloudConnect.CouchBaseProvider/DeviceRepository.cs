using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebDemo.AbstractModel;

namespace CloudConnect.CouchBaseProvider
{
    public class DeviceRepository : RepositoryBase<Device>, IRepository<WebDemo.AbstractModel.Device>
    {
        public void SaveDevice(Device d)
        {
            this.Create(d);
        }

        public void Create(WebDemo.AbstractModel.Device item)
        {
            base.Create((Device)item);
        }

        public void Delete(string id)
        {
            base.Delete(id);
        }

        public void Update(WebDemo.AbstractModel.Device item)
        {
            base.Update((Device)item);
        }

        public new IList<WebDemo.AbstractModel.Device> GetAll(int limit)
        {
            return base.GetAll(limit) as IList<WebDemo.AbstractModel.Device>;
        }

        public new WebDemo.AbstractModel.Device Get(string id)
        {
            return base.Get(id);
        }


        public void Update(IList<WebDemo.AbstractModel.Device> items)
        {
            foreach (Device device in items)
                base.Update(device);
        }

        public WebDemo.AbstractModel.Device Build()
        {
            return new Device();
        }


        public void Create(IList<WebDemo.AbstractModel.Device> items)
        {
            foreach (Device device in items)
                base.Create(device);
        }
    }
}
