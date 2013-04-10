using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace WebDemo.Models.Repository
{
    public class RepositoryBase
    {
        private List<string> _keys = new List<string>();

        private bool _isInitialize = false;
        public bool IsInitialize { get { return _isInitialize; } }

        public bool IsActive { get; set; }

        public void Recycle()
        {
            EmptyCache();
        }

        public object LoadDataCache(string request)
        {
            if (IsActive)
            {
                return HttpRuntime.Cache[request];
            }
            return null;
        }

        public virtual void RemovedCallback(String k, Object v, CacheItemRemovedReason r)
        {

        }

        public void SaveDataCache(string request, int expiredSecond, int expiredMinute, int expiredHour, object objet, string dependancyKey = ""
           , bool lowPriority = false, bool autoReload = false)
        {
            if (IsActive && HttpRuntime.Cache.EffectivePercentagePhysicalMemoryLimit > 5)
            {
                if (objet != null)
                {
                    if (HttpRuntime.Cache[request] != null)
                        HttpRuntime.Cache[request] = objet;
                    else
                    {
                        CacheDependency dependance = null;
                        if (!String.IsNullOrEmpty(dependancyKey))
                        {
                            dependance = new CacheDependency(null, new string[] { dependancyKey });
                        }

                        CacheItemPriority priorite = (lowPriority ? CacheItemPriority.Low : CacheItemPriority.Normal);

                        if (autoReload)
                            HttpRuntime.Cache.Insert(request, objet, dependance, DateTime.Now.Add(new TimeSpan(expiredHour, expiredMinute, expiredSecond)), TimeSpan.Zero, priorite, RemovedCallback);
                        else
                            HttpRuntime.Cache.Insert(request, objet, dependance, DateTime.Now.Add(new TimeSpan(expiredHour, expiredMinute, expiredSecond)), TimeSpan.Zero, priorite, null);

                        if (!_keys.Contains(request))
                            _keys.Add(request);
                    }
                }
            }
        }

        public void EmptyCache()
        {
            foreach (string k in _keys)
            {
                HttpRuntime.Cache.Remove(k);
            }
            _keys.Clear();
        }
    }
}