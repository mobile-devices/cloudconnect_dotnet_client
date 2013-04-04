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
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Test()
        {
            MongoDB.Driver.MongoCollection<WebDemo.Models.TrackingModel> dataDb = Tools.MongoConnector.Instance.DataBase.GetCollection<WebDemo.Models.TrackingModel>("TRACKING");

            List<TrackingModel> result = (from d in dataDb.AsQueryable<TrackingModel>()
                                          where d.Data.Asset == "351777047062540"
                                          && d.RecordedDateKey == 20130329
                                          && d.Data.Recorded_at < new DateTime(2013, 03, 29, 15, 06, 0, DateTimeKind.Utc)
                                          select d).ToList();
            if (result.Count > 0)
            {
                var resultat = dataDb.Remove(Query.And(Query.EQ("Data.Asset", "351777047062540")
                    , Query.EQ("RecordedDateKey", 20130329)
                    , Query.LT("Data.Recorded_at", MongoDB.Bson.BsonDateTime.Create(new DateTime(2013, 03, 29, 15, 06, 0, DateTimeKind.Utc)))));
                Console.WriteLine(resultat.Response);

                Console.WriteLine(result.Count);
            }


            return View();
        }
    }
}
