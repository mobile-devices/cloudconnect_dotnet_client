using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
using WebDemo.Models;
using WebDemo.Models.Repository;
using WebDemo.Tools;

namespace WebDemo.httphandler
{
    /// <summary>
    /// Summary description for Notifications
    /// </summary>
    public class Notifications : IHttpHandler
    {
        private static readonly object _verrou = new object();

        public void ProcessRequest(HttpContext context)
        {
            string data = "";

            if (context.Request.HttpMethod == "POST")
            {
                if (System.Threading.Monitor.TryEnter(_verrou, 10000))
                {
                    try
                    {
                        using (StreamReader fluxDonnees = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
                        {
                            data = fluxDonnees.ReadToEnd();
                            Tools.Log.Instance.Notification.Info(data);
                        }

                        Decode(data);
                    }
                    catch (Exception ex)
                    {
                        Tools.Log.Instance.Notification.Error("[" + ex.Message + "]" + data);
                        throw ex;
                    }
                    finally
                    {
                        System.Threading.Monitor.Exit(_verrou);
                    }
                }
            }
            else
            {
                data = context.Request.Params["Data"];
                if (!String.IsNullOrEmpty(data))
                {
                    Decode(data);
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

        private void Decode(string data)
        {
            List<MD.CloudConnect.MDData> decodedData = MD.CloudConnect.Notification.Instance.Decode(data);

            List<TrackingModel> saveTracks = new List<TrackingModel>();
            List<DeviceModel> saveDevices = new List<DeviceModel>();

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
            }
            if (saveTracks.Count > 0)
                RepositoryFactory.Instance.DataTrackingDB.Save(saveTracks);
            if (saveDevices.Count > 0)
                RepositoryFactory.Instance.DeviceDb.Save(saveDevices);
        }

        private void DecodeTracking(MD.CloudConnect.ITracking t, AccountModel account, List<TrackingModel> saveTracks, List<DeviceModel> saveDevices)
        {
            string imei = t.Asset;

            DeviceModel device = RepositoryFactory.Instance.DeviceDb.GetDevice(imei);

            if (device == null)
            {
                device = new DeviceModel() { Imei = imei };
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