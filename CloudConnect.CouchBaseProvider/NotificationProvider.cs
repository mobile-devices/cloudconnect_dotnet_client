﻿using MD.CloudConnect.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Enyim.Caching.Memcached;
using Couchbase.Extensions;
using Couchbase;
using MD.CloudConnect;

namespace CloudConnect.CouchBaseProvider
{
    public class NotificationProvider : RepositoryBase<Notification> , INotificationCacheProvider
    {
        public void PushNotificationCache(string key, string data, DateTime recorded_date)
        {
            Notification notif = new Notification()
            {
                Created_at = DateTime.Now,
                Data = data,
                Dropped = false,
                Key = key,
                Received_at = recorded_date.Ticks
            };
            this.CreateWithExpireTime(notif, recorded_date.AddDays(7));
        }

        public int SizeOfCache()
        {
            IEnumerable<Notification> result = GetAllByDropped("[ 'false' ]", "[ 'false' ]");
            return result.Count();
        }

        public IEnumerable<Notification> GetAllByDropped(string startKey = null, string endKey = null, int limit = 0, bool allowStale = false)
        {
            var view = GetView("by_dropped_and_received_and_key");
            if (limit > 0) view.Limit(limit);
            if (!allowStale) view.Stale(StaleMode.False);
            if (!string.IsNullOrEmpty(startKey)) view.StartKey(startKey);
            if (!string.IsNullOrEmpty(endKey)) view.StartKey(endKey);
            return view;
        }

        public IEnumerable<MD.CloudConnect.INotificationData> RequestNotificationCache(string key, DateTime max_date)
        {
            IEnumerable<Notification> result = GetAllByDropped("[ 'false' ]", "[ 'false' ]", 10000);
            return result.Where(x => x.Key == key && x.Received_at <= max_date.Ticks && x.Dropped == false).OrderBy(x => x.Received_at);
        }

        public void DropNotificationCache(string key, DateTime max_date)
        {
            IEnumerable<Notification> result = GetAllByDropped("[ 'false' ]", "[ 'false' ]", 10000);
            result = result.Where(x => x.Key == key && x.Received_at <= max_date.Ticks && x.Dropped == false).OrderBy(x => x.Received_at);

            foreach (Notification d in result)
            {
                d.Dropped = true;
                this.Update(d);
            }
        }

        public List<string> GetAssetsForGroup(string groupName)
        {
            throw new NotImplementedException();
        }

        public void SetAssetsForGroup(string groupName, List<string> assets)
        {
            throw new NotImplementedException();
        }
    }
}
