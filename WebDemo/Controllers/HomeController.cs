using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using WebDemo.Models;
using MongoDB.Driver.Builders;

namespace WebDemo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string id = "", int year = 0, int month = 0, int day = 0)
        {
            ViewBag.oldjs = (Request["oldjs"] == "1");
             
            if (year == 0 || month == 0 || day == 0)
                ViewBag.Date = DateTime.Now;
            else
                ViewBag.Date = new DateTime(year,month,day);

            ViewBag.Imei = id;
            return View();
        }

        public ActionResult Test()
        {
            return View();
        }
    }
}
