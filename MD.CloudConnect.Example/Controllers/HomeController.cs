using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MD.CloudConnect.Data;
using MD.CloudConnect.Example.Models;

namespace MD.CloudConnect.Example.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult getAssets(string account, string environment, string token)
        {
            CloudConnect cc = new CloudConnect(environment, account, token);
            List<AssetData> datas = cc.Asset.Get();

            return Json(datas, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getLastracks(string account, string environment, string token, string asset)
        {
            CloudConnect cc = new CloudConnect(environment, account, token);
            List<TrackingData> datas = cc.Track.Get("asset:" + asset, per_page: "100");

            List<MyJsonTrackingData> result = (from d in datas
                                               select new MyJsonTrackingData()
                                                   {
                                                       Received_at = d.Received_at.ToString("yyyy/MM/dd HH:mm:ss"),
                                                       Recorded_at = d.Recorded_at.ToString("yyyy/MM/dd HH:mm:ss"),
                                                       IsValid = d.ContainsField(MD.CloudConnect.FieldDefinition.GPRMC_VALID.Key) ? d.IsValid : false,
                                                       Speed = d.ContainsField(MD.CloudConnect.FieldDefinition.GPS_SPEED.Key) ? Math.Round(d.SpeedKmPerHour, 2) : 0.0,                                                       
                                                       Latitude = d.Latitude,
                                                       Longitude = d.Longitude
                                                   }).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
