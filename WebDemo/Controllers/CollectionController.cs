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

namespace WebDemo.Controllers
{
    public class CollectionController : Controller
    {
        //
        // GET: /Collection/

        public ActionResult Index(string asset = "", int year = 2013, int month = 2, int day = 28)
        {
            List<CollectionModel> result = new List<CollectionModel>();
            ViewBag.Imei = "";
            ViewBag.Date = DateTime.MinValue;

            if (!String.IsNullOrEmpty(asset))
            {
                DateTime date = new DateTime(year, month, day);
                DeviceModel device = RepositoryFactory.Instance.DeviceDb.GetDevice(asset);
                if (device != null)
                {
                    ViewBag.Imei = device.Imei;
                    ViewBag.Date = date;
                    result = RepositoryFactory.Instance.DataCollectionDb.GetData(device, date);
                }
            }

            return View(result);
        }

        internal static CloudConnect.MongoProvider.NotificationProvider _provider = new CloudConnect.MongoProvider.NotificationProvider(System.Configuration.ConfigurationManager.AppSettings["MongoUri"], System.Configuration.ConfigurationManager.AppSettings["MongoDbName"]);

        public ContentResult Test()
        {
           string json =  "{\"meta\":{\"account\":\"unstable\",\"event\":\"poke\"},\"payload\":{\"id\":597363964166275184,\"id_str\":\"597363964166275184\",\"asset\":\"351732052448241\",\"sender\":\"Vice Manager\",\"namespace\":\"ToTo\",\"received_at\":\"2014-07-07T09:51:07Z\",\"b64_payload\":\"b2hoaCB5ZWFoaGg=\"}}]";
           
            
            //PokeHub.BroadCastPoke("{ test: 'tata', asset: 'toto' }");
             
           
            //CloudConnect.MongoProvider.NotificationProvider provider = new CloudConnect.MongoProvider.NotificationProvider(System.Configuration.ConfigurationManager.AppSettings["MongoUri"], System.Configuration.ConfigurationManager.AppSettings["MongoDbName"]);
            //int idx = 0;
            //do
            //{
            //    IEnumerable<MD.CloudConnect.INotificationData> data = provider.RequestNotificationCache("GEN_NOTIF", DateTime.UtcNow);
               
            //    foreach (MD.CloudConnect.INotificationData d in data)
            //    {
            //        np.PushNotificationCache("GEN_NOTIF", d.Data, DateTime.UtcNow);
            //        idx++;
            //    }
            //} while (idx < 50000);

          

            return Content("Ok");
        }

        private static void SaveDroppedData(List<MD.CloudConnect.MDData> dropdata)
        {
            if (dropdata != null && dropdata.Count > 0)
            {
                List<TrackingModel> saveTracks = new List<TrackingModel>();

                foreach (MD.CloudConnect.MDData dData in dropdata)
                {
                    if (dData.Meta.Event == "track")
                    {
                        string imei = dData.Tracking.Asset;

                        TrackingModel track = new TrackingModel();
                        track.Data = buildTrackingData((MD.CloudConnect.Data.TrackingData)dData.Tracking);
                        track.Dropped = true;
                        track.CreatedAt = DateTime.UtcNow;
                        track.RecordedDateKey = dData.Tracking.Recorded_at.GenerateKey();
                        track.Asset = imei;
                        saveTracks.Add(track);

                    }
                }

                if (saveTracks.Count > 0)
                    RepositoryFactory.Instance.DataTrackingDB.Save(saveTracks);
            }
        }

        private static void Analyze(List<MD.CloudConnect.MDData> decodedData)
        {
            List<DeviceModel> deviceInDb = RepositoryFactory.Instance.DeviceDb.GetDevices();
            List<TrackingModel> saveTracks = new List<TrackingModel>();
            List<DeviceModel> saveDevices = new List<DeviceModel>();
            List<CollectionModel> saveCollections = new List<CollectionModel>();

            List<AccountModel> accounts = RepositoryFactory.Instance.AccountDb.GetAccounts();
            AccountModel currentAccount = null;
            foreach (MD.CloudConnect.MDData dData in decodedData)
            {
                //if the current data is for the same previous account, we don't need to reload the account
                if (currentAccount == null || currentAccount.Name != dData.Meta.Account)
                    currentAccount = accounts.Where(x => x.Name == dData.Meta.Account).FirstOrDefault();

                if (currentAccount == null)
                {
                    currentAccount = new AccountModel() { Name = dData.Meta.Account };
                    RepositoryFactory.Instance.AccountDb.Save(currentAccount);
                    accounts.Add(currentAccount);
                }

                if (dData.Meta.Event == "track")
                {
                    DecodeTracking(dData.Tracking, currentAccount, saveTracks, saveDevices, deviceInDb);
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
                RepositoryFactory.Instance.DataTrackingDB.Save(saveTracks);
            if (saveDevices.Count > 0)
                RepositoryFactory.Instance.DeviceDb.Save(saveDevices);

        }

        private static void DecodeTracking(MD.CloudConnect.ITracking t, AccountModel account, List<TrackingModel> saveTracks, List<DeviceModel> saveDevices, List<DeviceModel> allDevices)
        {
            string imei = t.Asset;

            DeviceModel device = saveDevices.Where(x => x.Imei == imei).FirstOrDefault();
            if (device == null)
                device = allDevices.Where(x => x.Imei == imei).FirstOrDefault();

            if (device == null)
            {
                device = new DeviceModel() { Imei = imei };
                RepositoryFactory.Instance.DeviceDb.Save(device);
                device = RepositoryFactory.Instance.DeviceDb.GetDevice(imei);
            }

            device.LastReport = t.Recorded_at;
            if (t.IsValid)
            {
                device.LastValidLocation = t.Recorded_at;
                device.LastLongitude = t.Longitude;
                device.LastLatitude = t.Latitude;
            }

            TrackingModel track = new TrackingModel();
            track.DeviceID = device.Id;
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
