using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebDemo.AbstractModel;

namespace WebDemo.Models.Repository
{
    public class RepositoryFactory
    {
        #region singleton
        protected static readonly RepositoryFactory _instance = new RepositoryFactory();
        public static RepositoryFactory Instance
        {
            get
            {
                lock (_instance)
                {
                    return _instance;
                }
            }
        }

        static RepositoryFactory()
        {

        }
        #endregion

        public WebDemo.AbstractModel.IRepositoryFactory Provider { get; set; }

        public IRepository<Device> DeviceDb
        {
            get { return Provider.DeviceRepository; }
        }

        public ITrackingRepository DataTrackingDB
        {
            get { return Provider.TrackingRepository; }
        }
    }
}