using Couchbase;
using Couchbase.Operations;
using Enyim.Caching.Memcached;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CloudConnect.CouchBaseProvider
{
    public abstract class RepositoryBase<T> where T : ModelBase
    {
        protected static CouchbaseClient _Client { get; set; }
        static RepositoryBase()
        {
            _Client = new CouchbaseClient();
        }

        protected virtual string BuildKey(T model)
        {
            if (string.IsNullOrEmpty(model.Id))
            {
                return Guid.NewGuid().ToString();
            }
            return model.Id.InflectTo().Underscored;
        }

        private readonly string _designDoc;
        public RepositoryBase()
        {
            _designDoc = typeof(T).Name.ToLower().InflectTo().Pluralized;
        }

        protected IView<T> GetView(string name, bool isProjection = false)
        {
            return _Client.GetView<T>(_designDoc, name, !isProjection);
        }

        protected IView<IViewRow> GetViewRaw(string name)
        {
            return _Client.GetView(_designDoc, name);
        }

        public virtual IEnumerable<T> GetAll(int limit = 0)
        {
            var view = _Client.GetView<T>(_designDoc, "all", true);
            if (limit > 0) view.Limit(limit);
            return view;
        }

        public virtual int Create(T value, PersistTo persistTo = PersistTo.Zero)
        {
            var result = _Client.ExecuteStore(StoreMode.Add, BuildKey(value), serializeAndIgnoreId(value), persistTo);
            if (result.Exception != null) throw result.Exception;
            return result.StatusCode.Value;
        }

        public virtual int CreateWithExpireTime(T value, DateTime expireAt, PersistTo persistTo = PersistTo.Zero)
        {
            var result = _Client.ExecuteStore(StoreMode.Add, BuildKey(value), serializeAndIgnoreId(value), expireAt, persistTo);
            if (result.Exception != null) throw result.Exception;
            return result.StatusCode.Value;
        }

        public virtual int Update(T value, PersistTo persistTo = PersistTo.Zero)
        {
            var result = _Client.ExecuteStore(StoreMode.Replace, value.Id, serializeAndIgnoreId(value), persistTo);
            if (result.Exception != null) throw result.Exception;
            return result.StatusCode.Value;
        }

        public virtual int Save(T value, PersistTo persistTo = PersistTo.Zero)
        {
            var key = string.IsNullOrEmpty(value.Id) ? BuildKey(value) : value.Id;
            var result = _Client.ExecuteStore(StoreMode.Set, key, serializeAndIgnoreId(value), persistTo);
            if (result.Exception != null) throw result.Exception;
            return result.StatusCode.Value;
        }

        public virtual T Get(string key)
        {
            var result = _Client.ExecuteGet<string>(key);
            if (result.Exception != null) throw result.Exception;

            if (result.Value == null)
            {
                return null;
            }

            var model = JsonConvert.DeserializeObject<T>(result.Value);
            model.Id = key; //Id is not serialized into the JSON document on store, so need to set it before returning
            return model;
        }

        public virtual int Delete(string key, PersistTo persistTo = PersistTo.Zero)
        {
            var result = _Client.ExecuteRemove(key, persistTo);
            if (result.Exception != null) throw result.Exception;
            return result.StatusCode.HasValue ? result.StatusCode.Value : 0;
        }



        private string serializeAndIgnoreId(T obj)
        {
            var json = JsonConvert.SerializeObject(obj,
                new JsonSerializerSettings()
                {
                    ContractResolver = new DocumentIdContractResolver(),
                });
            return json;
        }

        private class DocumentIdContractResolver : CamelCasePropertyNamesContractResolver
        {
            protected override List<MemberInfo> GetSerializableMembers(Type objectType)
            {
                return base.GetSerializableMembers(objectType).Where(o => o.Name != "Id").ToList();
            }
        }
    }
}
