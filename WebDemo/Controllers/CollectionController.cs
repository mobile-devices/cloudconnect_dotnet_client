using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebDemo.Models;
using WebDemo.Models.Repository;
using MongoDB.Driver.Builders;
using WebDemo.Tools;
using MongoDB.Driver.Linq;
using System.Diagnostics;
using MongoDB.Bson;
using WebDemo.AbstractModel;

namespace WebDemo.Controllers
{
    public class CollectionController : Controller
    {
        //
        // GET: /Collection/


        //internal static CloudConnect.MongoProvider.NotificationProvider _provider = new CloudConnect.MongoProvider.NotificationProvider(System.Configuration.ConfigurationManager.AppSettings["MongoUri"], System.Configuration.ConfigurationManager.AppSettings["MongoDbName"]);

        public ContentResult Test()
        {
            CloudConnect.CouchBaseProvider.NotificationProvider np = new CloudConnect.CouchBaseProvider.NotificationProvider();

            //IEnumerable<MD.CloudConnect.INotificationData> data = np.GetAllByDropped("[false]", "[false]", 1000);
            //int test = data.Count();

            //foreach (MD.CloudConnect.INotificationData d in data)
            //{
            //    Console.WriteLine(d.ToString());
            //}

            //CloudConnect.CouchBaseProvider.Device device = new CloudConnect.CouchBaseProvider.Device()
            //{
            //    CreatedAt = DateTime.UtcNow,
            //    Imei = "1234"
            //};

            //CloudConnect.CouchBaseProvider.DeviceRepository dRepo = new CloudConnect.CouchBaseProvider.DeviceRepository();
            //dRepo.SaveDevice(device);

            //CloudConnect.MongoProvider.NotificationProvider provider = new CloudConnect.MongoProvider.NotificationProvider(System.Configuration.ConfigurationManager.AppSettings["MongoUri"], System.Configuration.ConfigurationManager.AppSettings["MongoDbName"]);
            //int idx = 0;
            ////do
            ////{
            //    IEnumerable<MD.CloudConnect.INotificationData> data = provider.RequestNotificationCache("GEN_NOTIF", DateTime.UtcNow);

            //    foreach (MD.CloudConnect.INotificationData d in data)
            //    {
            //        np.PushNotificationCache("GEN_NOTIF", d.Data, DateTime.UtcNow);
            //        idx++;
            //    }
           // } while (idx < 50000);
            List<MD.CloudConnect.MDData> data = null;
            List<MD.CloudConnect.MDData> dropdata = null;
            string hash_key;

            Stopwatch watch = new Stopwatch();
            watch.Start();
            hash_key = MD.CloudConnect.Notification.Instance.AsyncDecode(out data, out dropdata);

            if (data.Count > 0)
            {
                Tools.Log.Instance.General.Debug(String.Format("{0} decoded values on {1} raw notifications / dropData : {2}", data.Count, np.SizeOfCache(), dropdata.Count));
                Tools.Log.Instance.General.Debug(String.Format("\tAsyncDecode:{0} ms", watch.ElapsedMilliseconds));
                watch.Restart();
                Analyze(data);
                Tools.Log.Instance.General.Debug(String.Format("\tAnalyze:{0} ms", watch.ElapsedMilliseconds));
                watch.Restart();
                SaveDroppedData(dropdata);
                Tools.Log.Instance.General.Debug(String.Format("\tSaveDroppedData:{0} ms", watch.ElapsedMilliseconds));
                watch.Restart();
                MD.CloudConnect.Notification.Instance.AckDecodedData(hash_key);
                Tools.Log.Instance.General.Debug(String.Format("\tAckDecodedData:{0} ms", watch.ElapsedMilliseconds));
            }

            return Content("Ok");
        }


        private static void SaveDroppedData(List<MD.CloudConnect.MDData> dropdata)
        {
            if (dropdata != null && dropdata.Count > 0)
            {
                List<Track> saveTracks = new List<Track>();

                foreach (MD.CloudConnect.MDData dData in dropdata)
                {
                    if (dData.Meta.Event == "track")
                    {
                        string imei = dData.Tracking.Asset;

                        Track track = RepositoryFactory.Instance.DataTrackingDB.Build();
                        track.Data = buildTrackingData((MD.CloudConnect.Data.TrackingData)dData.Tracking);
                        track.Dropped = true;
                        track.CreatedAt = DateTime.UtcNow;
                        track.RecordedDateKey = dData.Tracking.Recorded_at.GenerateKey();
                        track.Asset = imei;
                        saveTracks.Add(track);

                    }
                }

                if (saveTracks.Count > 0)
                    RepositoryFactory.Instance.DataTrackingDB.Create(saveTracks);
            }
        }

        private static void Analyze(List<MD.CloudConnect.MDData> decodedData)
        {
            IList<Device> deviceInDb = RepositoryFactory.Instance.DeviceDb.GetAll();
            if (deviceInDb == null)
                deviceInDb = new List<Device>();
            IList<Track> saveTracks = new List<Track>();
            IList<Device> saveDevices = new List<Device>();
            //List<CollectionModel> saveCollections = new List<CollectionModel>();

            //List<AccountModel> accounts = RepositoryFactory.Instance.AccountDb.GetAccounts();
            //AccountModel currentAccount = null;
            foreach (MD.CloudConnect.MDData dData in decodedData)
            {
                //if the current data is for the same previous account, we don't need to reload the account
                //if (currentAccount == null || currentAccount.Name != dData.Meta.Account)
                //    currentAccount = accounts.Where(x => x.Name == dData.Meta.Account).FirstOrDefault();

                //if (currentAccount == null)
                //{
                //    currentAccount = new AccountModel() { Name = dData.Meta.Account };
                //    RepositoryFactory.Instance.AccountDb.Save(currentAccount);
                //    accounts.Add(currentAccount);
                //}

                if (dData.Meta.Event == "track")
                {
                    DecodeTracking(dData.Tracking, saveTracks, saveDevices, deviceInDb);
                }

                if (dData.Meta.Event == "message")
                {
                    //MD.CloudConnect.IMessage m = dData.Message;
                }

                if (dData.Meta.Event == "collection")
                {
                    //DecodeCollection(dData.Collection, currentAccount, saveCollections);
                }
            }
            if (saveTracks.Count > 0)
                RepositoryFactory.Instance.DataTrackingDB.Create(saveTracks);
            if (saveDevices.Count > 0)
                RepositoryFactory.Instance.DeviceDb.Update(saveDevices);

        }

        private static void DecodeTracking(MD.CloudConnect.ITracking t, IList<Track> saveTracks, IList<Device> saveDevices, IList<Device> allDevices)
        {
            string imei = t.Asset;

            Device device = saveDevices.Where(x => x.Imei == imei).FirstOrDefault();
            if (device == null)
                device = allDevices.Where(x => x.Imei == imei).FirstOrDefault();

            if (device == null)
            {
                device = RepositoryFactory.Instance.DeviceDb.Build();
                device.Imei = imei;
                RepositoryFactory.Instance.DeviceDb.Create(device);
                device = RepositoryFactory.Instance.DeviceDb.Get(imei);
            }

            device.LastReport = t.Recorded_at;
            if (t.IsValid)
            {
                device.LastValidLocation = t.Recorded_at;
                device.LastLongitude = t.Longitude;
                device.LastLatitude = t.Latitude;
            }

            Track track = RepositoryFactory.Instance.DataTrackingDB.Build();
            track.Data = buildTrackingData((MD.CloudConnect.Data.TrackingData)t);
            track.RecordedDateKey = t.Recorded_at.GenerateKey();
            track.CreatedAt = DateTime.UtcNow;
            track.Asset = imei;
            saveTracks.Add(track);

            device.UpdateField(track);

            if (saveDevices.Where(x => x.Imei == device.Imei).Count() == 0)
                saveDevices.Add(device);
        }


        private static WebDemoTrackingData buildTrackingData(MD.CloudConnect.Data.TrackingData t)
        {
            WebDemoTrackingData result = new WebDemoTrackingData()
            {
                Asset = t.Asset,
                Fields = t.fields,
                Id = t.Id,
                Id_str = t.Id_str,
                Latitude = t.Latitude,
                Longitude = t.Longitude,
                Received_at = t.Received_at,
                Recorded_at = t.Recorded_at
            };
            return result;
        }
    }
}
