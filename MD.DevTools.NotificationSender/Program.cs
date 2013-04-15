using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace MD.DevTools.NotificationSender
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start Notification Sender");
            Console.WriteLine("Be sure to have update the config file before to continue");
            Console.WriteLine("Press enter");
            Console.ReadLine();

            Data currentData = null;
            int page = 0;
            do
            {
                currentData = LoadData(NotificationSender.Properties.Settings.Default.Asset
                    , NotificationSender.Properties.Settings.Default.Date.Year, NotificationSender.Properties.Settings.Default.Date.Month, NotificationSender.Properties.Settings.Default.Date.Day
                    , page, NotificationSender.Properties.Settings.Default.ItemsPerPage);
                if (currentData.Content != null)
                {
                    Console.WriteLine("Page {0}/{1}", page, currentData.TotalPages);
                    SendData(JsonConvert.SerializeObject(currentData.Content, Formatting.None));
                    if (Properties.Settings.Default.PauseBetweenRequest)
                    {
                        Console.WriteLine("press enter to continue");
                        Console.ReadLine();
                    }
                }

                page++;
            } while (currentData.Page != currentData.TotalPages);

            Console.WriteLine("Press enter to quit");
            Console.ReadLine();
        }

        public static void SendData(string content)
        {
            try
            {
                WebRequest request = HttpWebRequest.Create(Properties.Settings.Default.Url);
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
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}", ex.Message);
            }
        }

        public static Data LoadData(string asset, int year, int month, int day, int page, int itemperpage)
        {
            Data result = null;

            try
            {
                WebRequest request = HttpWebRequest.Create(String.Format("http://webdemo.integration.cloudconnect.io/tracking/exportdata?asset={0}&year={1}&month={2}&day={3}&page={4}&maxitemperpage={5}"
                    , asset, year, month, day, page, itemperpage));
                request.Method = "GET";

                WebResponse rep = request.GetResponse();
                StreamReader loResponseStream = new StreamReader(rep.GetResponseStream());
                string jsonData = loResponseStream.ReadToEnd();
                result = JsonConvert.DeserializeObject<Data>(jsonData);

                loResponseStream.Close();
                rep.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR : " + ex.Message);
            }
            return result;
        }
    }
}
