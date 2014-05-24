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

namespace WebDemo.httphandler
{
    /// <summary>
    /// Summary description for Notifications
    /// </summary>
    public class Notifications : IHttpHandler
    {
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
                    int hash_key = MD.CloudConnect.Notification.Instance.AsyncDecode(out data);
                    Analyze(data);
                    MD.CloudConnect.Notification.Instance.AckDecodedData(hash_key);
                }
                catch (Exception ex)
                {
                    Tools.Log.Instance.Notification.Error("Decode Error in notification task : " + ex.Message);
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
                lock (Tools.Log.Instance.Notification)
                {
                    MD.CloudConnect.Notification.Instance.PrepareDataToDecode(data);
                    Tools.Log.Instance.Notification.Info(data);
                }
                if (HttpRuntime.Cache["Notification"] == null)
                {
                    HttpRuntime.Cache.Insert("Notification", true, null, DateTime.Now.Add(new TimeSpan(0, 0, 15)), TimeSpan.Zero, CacheItemPriority.Normal, NotificationTask);
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
        }

        public bool IsReusable
        {
            get
            {
                return true;
            }
        }

        private static void Analyze(List<MD.CloudConnect.MDData> decodedData)
        {
            //List<MD.CloudConnect.MDData> decodedData = MD.CloudConnect.Notification.Instance.Decode(data);

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
                    DecodeTracking(dData.Tracking, currentAccount, saveTracks, saveDevices);
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
            if (saveCollections.Count > 0)
                RepositoryFactory.Instance.DataCollectionDb.Save(saveCollections);

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

            //List<TrackingModel> tracks = new List<TrackingModel>();
            //foreach (MD.CloudConnect.Data.TrackingData data in c.Tracks)
            //{
            //    tracks.Add(new TrackingModel()
            //    {
            //        Data = data,
            //        DeviceID = device.Id,
            //        RecordedDateKey = data.Recorded_at.GenerateKey()
            //    });
            //}
            //collection.Tracks = ((List<ITracking>tracks).ToArray();

            saveCollections.Add(collection);
        }

        private static void DecodeTracking(MD.CloudConnect.ITracking t, AccountModel account, List<TrackingModel> saveTracks, List<DeviceModel> saveDevices)
        {
            string imei = t.Asset;

            DeviceModel device = RepositoryFactory.Instance.DeviceDb.GetDevice(imei);

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
            track.Data = (MD.CloudConnect.Data.TrackingData)t;
            track.RecordedDateKey = t.Recorded_at.GenerateKey();
            saveTracks.Add(track);

            device.UpdateField(track);

            if (saveDevices.Where(x => x.Imei == device.Imei).Count() == 0)
                saveDevices.Add(device);
        }
    }
}