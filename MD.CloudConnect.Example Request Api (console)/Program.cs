using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MD.CloudConnect.Data;

namespace CloudconnectExample
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start the program");

            Console.WriteLine("Initialiaze CloudConnect Wrapper");
            MD.CloudConnect.CloudConnect cc = new MD.CloudConnect.CloudConnect("integration", Properties.Settings.Default.account, Properties.Settings.Default.token);

            List<AssetData> assets = cc.Asset.Get();

            foreach (AssetData asset in assets)
                Console.WriteLine("Imei : {0}", asset.Imei);

            if (assets.Count > 0)
            {
                List<TrackingData> tracks = cc.Track.Get("asset:" + assets[0].Imei, per_page: "100");

                foreach (TrackingData track in tracks)
                    Console.WriteLine("Long. : {0} Lat. : {1} Time. : {2}", track.Longitude, track.Latitude, track.Recorded_at);
            }

            List<FieldData> fields = cc.Field.Get();
            if (fields.Count > 0)
            {
                foreach (FieldData f in fields)
                    Console.WriteLine(f.Name);
            }

            List<MessageData> messages = cc.Message.Get();
            if (messages.Count > 0)
            {
                foreach (MessageData m in messages)
                    Console.WriteLine("Asset: {0}; msg : {1}", m.Asset, m.Payload);
            }


            if (assets.Count > 0)
            {
                if (cc.Message.PostMessage(assets[0].Imei, "com.mdi.services.adminProtocol"
                    , "{\"request\": \"getval\", \"data\":{ \"pwrManager\":{ \"ignitionManagement\" }}}") > 0)
                {
                    Console.WriteLine("Error happening with the POST message");
                }
            }

            Console.WriteLine("Press enter to exit");
            Console.ReadLine();
        }
    }
}
