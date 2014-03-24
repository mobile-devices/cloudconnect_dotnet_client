using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebDemo.Models;
using WebDemo.Models.Repository;

namespace WebDemo.Controllers
{
    public class CollectionController : Controller
    {
        //
        // GET: /Collection/

        public ActionResult Index(string asset = "", int year = 2013, int month = 2, int day = 28)
        {
            List<CollectionModel> result = new List<CollectionModel>();
            ViewBag.Imei = "";
            ViewBag.Date = DateTime.MinValue;

            if (!String.IsNullOrEmpty(asset))
            {
                DateTime date = new DateTime(year, month, day);
                DeviceModel device = RepositoryFactory.Instance.DeviceDb.GetDevice(asset);
                if (device != null)
                {
                    ViewBag.Imei = device.Imei;
                    ViewBag.Date = date;
                    result = RepositoryFactory.Instance.DataCollectionDb.GetData(device, date);
                }
            }

            return View(result);
        }

    }
}
