using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebDemo.Models;
using WebDemo.Models.Repository;
using MD.CloudConnect.Data;

namespace WebDemo.Controllers
{
    public class messagingController : Controller
    {
        //
        // GET: /messaging/

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult SendMessage(string asset, string environment, string account, string token, string message)
        {
            string res = "OK";
            Session["MsgDate"] = null;

            MD.CloudConnect.CloudConnect cc = new MD.CloudConnect.CloudConnect(environment, account, token);

            if (cc.Message.PostMessage(asset, "com.mdi.applications.message", message) > 0)
            {
                res = "BAD";
            }

            List<MessageData> messages = cc.Message.Get("asset:" + asset);
            foreach (MessageData msg in messages)
            {
                if (msg.Channel == "com.mdi.applications.message" && msg.Asset == asset && msg.Sender != asset)
                {
                    Session["MsgDate"] = msg.Received_at;
                    break;
                }
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckMessage(string asset, string environment, string account, string token, string message)
        {
            string res = "";
            if (Session["MsgDate"] != null)
            {
                MD.CloudConnect.CloudConnect cc = new MD.CloudConnect.CloudConnect(environment, account, token);
                DateTime date = (DateTime)Session["MsgDate"];
                List<MessageData> messages = cc.Message.Get("asset:" + asset);
                foreach (MessageData msg in messages)
                {
                    if (msg.Channel == "com.mdi.applications.message" && msg.Asset == asset && msg.Sender == asset
                        && msg.Received_at.HasValue && msg.Received_at.Value.Ticks > date.Ticks)
                    {
                        res = msg.Message;
                    }
                }
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SendTasks(JsonTasksModel data)
        {
            string res = "OK";
            DeviceModel device = RepositoryFactory.Instance.DeviceDb.GetDevice(data.Asset);

            if (device != null)
            {
                if (!device.IdLastTask.HasValue)
                    device.IdLastTask = 0;
                device.IdLastTask++;
                data.ID = device.IdLastTask.Value;

                MD.CloudConnect.CloudConnect cc = new MD.CloudConnect.CloudConnect(data.Environment, data.Account, data.Token);

                if (cc.Message.PostMessage(data.Asset, "com.mdi.task_manager", data.getMessage()) > 0)
                {
                    res = "BAD";
                }

                RepositoryFactory.Instance.DeviceDb.Save(device);
            }
            else
                res = "Device does not exists";
            return Json(res);
        }

        //
        // GET: /messaging/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /messaging/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /messaging/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /messaging/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /messaging/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /messaging/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /messaging/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
