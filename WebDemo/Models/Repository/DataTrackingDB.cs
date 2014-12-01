using MD.CloudConnect.CouchBaseProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebDemo.Tools;

namespace WebDemo.Models.Repository
{
    public class DataTrackingDB : RepositoryBase
    {
        private const string TRACKING_DB_NAME = "TRACKING";

        public List<Track> GetData(Device device, DateTime date)
        {
            int datekey = date.GenerateKey();
            List<Track> cache = new List<Track>();
            List<Track> result = new List<Track>();
            int limit = 1000;
            string docid = "";
            do
            {
                cache = CouchbaseManager.Instance.TrackRepository.GetData(device.Imei, datekey, docid, true);

                if (cache.Count > 0)
                {
                    if (!String.IsNullOrEmpty(docid))
                        cache.RemoveAt(0);
                    docid = cache.Last().Id;
                    result.AddRange(cache);
                }

            } while (cache.Count >= limit - 1) ;

            return result;
        }
    }
}