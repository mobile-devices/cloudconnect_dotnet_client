using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Globalization;
using WebDemo.Models;
using Newtonsoft.Json;

namespace WebDemo.Controllers
{
    public class ObdStackController : Controller
    {
        //
        // GET: /ObdStack/

        public ActionResult Index()
        {
            if (HttpRuntime.Cache["OBDSTACK_LAST_RESULT_CONTENT"] != null)
                ViewBag.LastResultContent = HttpRuntime.Cache["OBDSTACK_LAST_RESULT_CONTENT"].ToString();
            else
                ViewBag.LastResultContent = "No Result";
            if (HttpRuntime.Cache["OBDSTACK_LAST_CONTENT"] != null)
                ViewBag.LastContent = HttpRuntime.Cache["OBDSTACK_LAST_CONTENT"].ToString();
            else
                ViewBag.LastContent = "No content";
            if (HttpRuntime.Cache["OBDSTACK_LAST_ERROR"] != null)
                ViewBag.LastError = HttpRuntime.Cache["OBDSTACK_LAST_ERROR"].ToString();
            else
                ViewBag.LastError = "No error";
            return View();
        }

        public JsonResult Decode()
        {
            List<TrackingFeedResult> result = new List<TrackingFeedResult>();
            string data = "";
            using (StreamReader stream = new StreamReader(HttpContext.Request.InputStream, HttpContext.Request.ContentEncoding))
            {
                data = stream.ReadToEnd();
            }
            HttpRuntime.Cache["OBDSTACK_LAST_CONTENT"] = data;
            //we use the fonction provide by MD.CloudConnect Library but if you use another langage you can simply decode data
            //with something like JSON.parse(data)
            List<MD.CloudConnect.MDData> decodedData = MD.CloudConnect.Notification.Instance.Decode(data);

            if (decodedData.Count > 0)
            {
                //for this demo, we just use the first data
                MD.CloudConnect.MDData newdata = decodedData[0];

                if (newdata.Meta.Event == "message" && newdata.Message.Channel == "com.mdi.applications.obdstacks")
                {
                    //The message is encode in Base64 value "b64_payload", you must decode the payload before deserializeObject
                    ObdStackMessage obdMessage = JsonConvert.DeserializeObject<ObdStackMessage>(newdata.Message.Message);
                    if (obdMessage.Results.Count > 0)
                    {
                        //for this demo, we just use the first data
                        if (obdMessage.Results[0].ActionListName == "getRPM" && obdMessage.Results[0].ResultList.Count > 0)
                        {
                            ObsStackResultCommand resultCommand = obdMessage.Results[0].ResultList[0];

                            if (resultCommand.CommandName == "OBD_RPM")
                            {
                                TrackingFeedResult tmp = new TrackingFeedResult()
                                {
                                    asset = newdata.Message.Asset,
                                    date = newdata.Message.Recorded_at.Value.ToString("yyyy-MM-dd'T'HH:mm:ss.fffK", DateTimeFormatInfo.InvariantInfo),
                                    feed_type = "track",
                                    content = new Dictionary<string, string>(),
                                    rule_id = newdata.MetaData["rule_id"]
                                };

                                // http://en.wikipedia.org/wiki/OBD-II_PIDs
                                //PID OC = ((A*256)+B)/4
                                tmp.content.Add("MDI_DASHBOARD_FUEL", (((resultCommand.Data[0] * 256) + resultCommand.Data[1]) / 4).ToString());
                                result.Add(tmp);
                            }
                        }
                    }
                }
            }
            HttpRuntime.Cache["OBDSTACK_LAST_RESULT_CONTENT"] = JsonConvert.SerializeObject(result.ToArray(), Formatting.None);
            HttpRuntime.Cache["OBDSTACK_LAST_ERROR"] = "";
            return Json(result.ToArray(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult Error()
        {
            string data = "";
            using (StreamReader stream = new StreamReader(HttpContext.Request.InputStream, HttpContext.Request.ContentEncoding))
            {
                data = stream.ReadToEnd();
            }

            HttpRuntime.Cache["OBDSTACK_LAST_ERROR"] = data;
            return Json("{}", JsonRequestBehavior.AllowGet);
        }
    }
}
