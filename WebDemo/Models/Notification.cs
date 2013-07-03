using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace WebDemo.Models
{
    public class Notification
    {
        public ObjectId Id { get; set; }
        public DateTime Created_at { get; set; }
        public string Name { get; set; }
        public string url { get; set; }
        public string content { get; set; }

        public bool SendData()
        {
            try
            {
                if (!String.IsNullOrEmpty(content))
                {
                    WebRequest request = HttpWebRequest.Create(url);
                    request.Method = "POST";
                    request.ContentType = "application/json";
                    StreamWriter sw = new StreamWriter(request.GetRequestStream());
                    sw.Write(content);
                    sw.Close();
                    WebResponse rep = request.GetResponse();
                    StreamReader loResponseStream = new StreamReader(rep.GetResponseStream());
                    string Response = loResponseStream.ReadToEnd();
                    Console.WriteLine(Response);
                    loResponseStream.Close();
                    rep.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                Tools.Log.Instance.Notification.Error("Forward Notification (webdemo)[" + url + "] : " + ex.Message);
                return false;
            }
        }

        public void CreateContent(string[] assets, string data, string account)
        {
            List<MD.CloudConnect.MDData> decodedData = JsonConvert.DeserializeObject<List<MD.CloudConnect.MDData>>(data).Where(x => x.Meta.Account == account).ToList();
            List<MD.CloudConnect.MDData> selectedData = new List<MD.CloudConnect.MDData>();
            if (assets != null && assets.Length > 0)
            {
                List<MD.CloudConnect.MDData> selectedTrackingData = (from d in decodedData.Where(x => x.Meta.Event == "track")
                                                                     join a in assets on d.Tracking.Asset equals a
                                                                     select d).ToList();
                List<MD.CloudConnect.MDData> selectedMessageData = (from d in decodedData.Where(x => x.Meta.Event == "message")
                                                                    join a in assets on d.Message.Asset equals a
                                                                    select d).ToList();

                selectedData.AddRange(selectedTrackingData);
                selectedData.AddRange(selectedMessageData);
            }
            else
            {
                selectedData = decodedData;
            }
            if (selectedData.Count > 0)
                this.content = JsonConvert.SerializeObject(selectedData, Formatting.None);
        }
    }
}