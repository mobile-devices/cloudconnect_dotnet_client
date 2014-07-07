using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
using WebDemo.Models;
using WebDemo.Models.Repository;
using WebDemo.Tools;
using System.Web.Caching;
using System.Threading;
using System.Diagnostics;
using MD.CloudConnect.Data;

namespace WebDemo.httphandler
{
    /// <summary>
    /// Summary description for Notifications
    /// </summary>
    public class Notifications : IHttpHandler
    {
        internal static CloudConnect.MongoProvider.NotificationProvider _provider = new CloudConnect.MongoProvider.NotificationProvider(System.Configuration.ConfigurationManager.AppSettings["MongoUri"], System.Configuration.ConfigurationManager.AppSettings["MongoDbName"]);

        public static void NotificationTask(String k, Object v, CacheItemRemovedReason r)
        {
            if (k == "Notification"
                && (r == CacheItemRemovedReason.Expired
                || r == CacheItemRemovedReason.Underused
                || r == CacheItemRemovedReason.DependencyChanged))
            {
                try
                {

                    List<MD.CloudConnect.MDData> data = null;
                    List<MD.CloudConnect.MDData> dropdata = null;
                    string hash_key;
                    lock (MD.CloudConnect.Notification.Instance)
                    {
                        Stopwatch watch = new Stopwatch();
                        watch.Start();
                        hash_key = MD.CloudConnect.Notification.Instance.AsyncDecode(out data, out dropdata);

                        if (data.Count > 0)
                        {
                            Tools.Log.Instance.General.Debug(String.Format("{0} decoded values on {1} raw notifications / dropData : {2}", data.Count, _provider.SizeOfCache(), dropdata.Count));
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

                    }
                }
                catch (Exception ex)
                {
                    Tools.Log.Instance.General.Error("Decode Error in notification task : " + ex.Message);

                }
            }
        }


        public void ProcessRequest(HttpContext context)
        {
            string data = "";
            if (context.Request.HttpMethod == "POST")
            {
                using (StreamReader stream = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
                {
                    data = stream.ReadToEnd();
                }
                if (data.LongCount() < 6777216)
                {
                    lock (Tools.Log.Instance.Notification)
                    {
                        MD.CloudConnect.Notification.Instance.PrepareDataToDecode(data);
                        Tools.Log.Instance.Notification.Info(data);
                    }
                }
            }
            else
            {
                data = context.Request.Params["Data"];
                if (!String.IsNullOrEmpty(data))
                {
                    Analyze(MD.CloudConnect.Notification.Instance.Decode(data));
                }
                else
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01//EN\" \"http://www.w3.org/TR/html4/strict.dtd\">");
                    sb.AppendLine("<html>");
                    sb.AppendLine("<head></head>");
                    sb.AppendLine("<body>");
                    sb.AppendLine("<form>");
                    sb.AppendLine("<TEXTAREA name=\"Data\" style=\"width: 100%; height: 600px\"></TEXTAREA>");
                    sb.AppendLine("<br />");
                    sb.AppendLine("<input type=\"submit\" value=\"Notification (exceptions non catched) And new data in the database\"/>");
                    sb.AppendLine("</form>");
                    sb.AppendLine("</body>");
                    sb.AppendLine("</html>");

                    context.Response.Write(sb.ToString());
                }
            }
            if (HttpRuntime.Cache["Notification"] == null)
            {
                HttpRuntime.Cache.Insert("Notification", true, null, DateTime.Now.Add(new TimeSpan(0, 0, 10)), TimeSpan.Zero, CacheItemPriority.Normal, NotificationTask);
            }

        }

        public bool IsReusable
        {
            get
            {
                return true;
            }
        }

        private static void DecodeCollection(MD.CloudConnect.ICollection c, AccountModel account, List<CollectionModel> saveCollections)
        {
            string imei = c.Asset;
            DeviceModel device = RepositoryFactory.Instance.DeviceDb.GetDevice(imei);

            if (device == null)
            {
                device = new DeviceModel() { Imei = imei };
                RepositoryFactory.Instance.DeviceDb.Save(device);
                device = RepositoryFactory.Instance.DeviceDb.GetDevice(imei);
            }

            CollectionModel collection = new CollectionModel()
            {
                DeviceID = device.Id,
                Name = c.Name,
                Id_str = c.Id_str,
                StartDateKey = c.Start_at.Value.GenerateKey(),
                Start_at = c.Start_at,
                Stop_at = c.Stop_at,
                Tracks = ((IEnumerable<MD.CloudConnect.Data.TrackingData>)c.Tracks).ToArray(),
                Messages = ((IEnumerable<MD.CloudConnect.Data.MessageData>)c.Messages).ToArray()
            };

            saveCollections.Add(collection);
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
                if (dData.Meta.Event == "poke")
                {
                    try
                    {
                        PokeData p = dData.Poke;
                        string msg = MD.CloudConnect.Tools.Base64Decoder.GetValueAsString(p.b64_payload);
                        PokeHub.BroadCastPoke("{ asset: '" + p.Asset + "', msg: '" + msg + "' }" );
                    }
                    catch { }

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