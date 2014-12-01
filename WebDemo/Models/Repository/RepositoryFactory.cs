using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebDemo.Models.Repository
{
    public struct Factory<KeyType, RepoBase>
    {
        public void Register<SpecificProduct>(KeyType key)
            where SpecificProduct : RepoBase, new()
        {
            if (m_mapProducts == null)
            {
                m_mapProducts = new SortedList<KeyType, RepoBase>();
            }
            CreateFunctor createFunctor = Creator<SpecificProduct>;
            m_mapProducts.Add(key, createFunctor());
        }

        //Create a registered object 
        public RepoBase Create(KeyType key)
        {
            return m_mapProducts[key];
        }

        private RepoBase Creator<SpecificProduct>()
            where SpecificProduct : RepoBase, new()
        {
            return new SpecificProduct();
        }

        public bool IsRegistered(KeyType key)
        {
            return m_mapProducts.ContainsKey(key);
        }

        public List<KeyType> Keys
        {
            get
            {
                return m_mapProducts.Keys.ToList();
            }
        }

        private delegate RepoBase CreateFunctor();

        private SortedList<KeyType, RepoBase> m_mapProducts;
    }


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

        private Factory<string, RepositoryBase> _factory = new Factory<string, RepositoryBase>();

        public void Initialize()
        {
            _factory.Register<DeviceDb>("DeviceDb");
            _factory.Register<DataTrackingDB>("DataTrackingDB");
        }

        public RepositoryBase RepoFactory(string name)
        {
            RepositoryBase repo = _factory.Create(name);
            return repo;
        }

        public DeviceDb DeviceDb
        {
            get { return this.RepoFactory("DeviceDb") as DeviceDb; }
        }

        public DataTrackingDB DataTrackingDB
        {
            get { return this.RepoFactory("DataTrackingDB") as DataTrackingDB; }
        }

    }
}