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

            Data test = LoadData("357322041761211", 2013, 04, 09, 0, 30);

            if (test != null)
            {
                Console.WriteLine("{0} items", test.TotalItems);
            }

            Console.WriteLine("Press enter to quit");
            Console.ReadLine();
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
                Console.WriteLine(jsonData);

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
