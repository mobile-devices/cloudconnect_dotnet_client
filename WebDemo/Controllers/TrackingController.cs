using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebDemo.Models;
using WebDemo.Models.Repository;
using WebDemo.Tools;
using System.IO;
using System.Web.Script.Serialization;
using MongoDB.Driver.Builders;
using Newtonsoft.Json;

namespace WebDemo.Controllers
{
    public class TrackingController : Controller
    {
        //
        // GET: /Tracking/

        public ActionResult Index(string asset = "", int year = 2013, int month = 2, int day = 28)
        {
            List<TrackingModel> tracks = new List<TrackingModel>();
            ViewBag.Imei = "";
            ViewBag.Fields = new Dictionary<string, MD.CloudConnect.Data.Field>();
            ViewBag.Date = DateTime.MinValue;

            if (!String.IsNullOrEmpty(asset))
            {
                DateTime date = new DateTime(year, month, day);
                DeviceModel device = RepositoryFactory.Instance.DeviceDb.GetDevice(asset);
                if (device != null)
                {
                    ViewBag.Imei = device.Imei;
                    tracks = RepositoryFactory.Instance.DataTrackingDB.GetData(device, date).OrderByDescending(x => x.Data.Recorded_at).ToList();
                    ViewBag.DateKey = date.GenerateKey();
                    ViewBag.Date = date;
                    ViewBag.Fields = device.GetOrderFieldName();
                }
            }

            return View(tracks);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Drop(string asset = "", int year = 2013, int month = 2, int day = 28)
        {
            DateTime date = new DateTime(year, month, day);

            if (!String.IsNullOrEmpty(asset))
            {
                MongoDB.Driver.MongoCollection<WebDemo.Models.TrackingModel> dataDb = Tools.MongoConnector.Instance.DataBase.GetCollection<WebDemo.Models.TrackingModel>("TRACKING");

                var resultat = dataDb.Remove(Query.And(Query.EQ("Data.Asset", asset)
                         , Query.EQ("RecordedDateKey", date.GenerateKey())));

            }
            return RedirectToAction("index", new { asset = asset, year = year, month = month, Day = day });
        }

        public FileResult CsvExport(string asset = "", int dateKey = 20130228)
        {
            MemoryStream memStream = new MemoryStream();
            StreamWriter writer = new StreamWriter(memStream);

            if (!String.IsNullOrEmpty(asset))
            {
                DeviceModel device = RepositoryFactory.Instance.DeviceDb.GetDevice(asset);
                if (device != null)
                {
                    List<string> keys = device.GetOrderFieldName();
                    WriteCsvCell(writer, "Recorded_at");
                    WriteCsvCell(writer, "Longitude");
                    WriteCsvCell(writer, "Latitude");
                    foreach (string key in keys)
                    {
                        WriteCsvCell(writer, key);
                    }
                    DateTime date = DateTime.ParseExact(dateKey.ToString(), "yyyyMMdd", null);
                    List<TrackingModel> tracks = RepositoryFactory.Instance.DataTrackingDB.GetData(device, date).OrderByDescending(x => x.Data.Recorded_at).ToList();
                    writer.WriteLine();
                    for (int i = 0; i < tracks.Count; i++)
                    {
                        TrackingModel t = tracks[i];
                        WriteCsvCell(writer, t.Data.Recorded_at.ToString("HH:mm:ss"));
                        WriteCsvCell(writer, t.Data.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture));
                        WriteCsvCell(writer, t.Data.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture));
                        foreach (string key in keys)
                        {
                            WriteCsvCell(writer, t.GetDisplayDataFor(key, (i < tracks.Count - 1 ? tracks[i + 1].Data.fields : null)));
                        }
                        writer.WriteLine();
                    }
                }
            }
            writer.Flush();
            memStream.Seek(0, SeekOrigin.Begin);
            FileStreamResult file = new FileStreamResult(memStream, "text/csv");
            file.FileDownloadName = String.Format("export_tracking_{0}_{1}.csv", asset, dateKey);
            return file;
        }

        public ContentResult AuthorizeFields()
        {
            var result = new
            {
                authorizedKeys = ExtendedFieldDefinition.Fields.Select(x => x.Key).ToList(),
                defaultKeyMap = new string[] { 
                    "GPRMC_VALID",
                    "GPS_SPEED",
                    "DIO_IGNITION",
                    "ODO_FULL",
                    "MDI_OBD_SPEED",
                    "MDI_OBD_RPM",
                    "MDI_OBD_FUEL",
                    "MDI_OBD_MILEAGE"
                },
                defaultKeyTable = new string[] 
                { 
                    "GPRMC_VALID",
                    "GPS_SPEED",
                    "GPS_DIR",
                    "DIO_IGNITION",
                    "ODO_FULL",
                    "BEHAVE_ID",
                    "MDI_OBD_SPEED",
                    "MDI_OBD_RPM",
                    "MDI_OBD_FUEL",
                    "MDI_OBD_VIN",
                    "MDI_OBD_MILEAGE"
                }
            };
            return new ContentResult()
            {
                Content = JsonConvert.SerializeObject(result, Formatting.None),
                ContentType = "application/json"
            };
        }

        public ContentResult ExportData(string asset, int year, int month, int day, int page = 0, int maxItemPerPage = 20)
        {
            JsonExportData result = new JsonExportData();

            if (!String.IsNullOrEmpty(asset))
            {
                DateTime date = new DateTime(year, month, day);
                DeviceModel device = RepositoryFactory.Instance.DeviceDb.GetDevice(asset);
                if (device != null)
                {
                    List<TrackingModel> tracks = RepositoryFactory.Instance.DataTrackingDB.GetData(device, date).OrderBy(x => x.Data.Recorded_at).ToList();

                    Dictionary<string, MD.CloudConnect.Data.Field> previousFields = null;
                    foreach (TrackingModel t in tracks)
                    {
                        if (previousFields == null)
                        {
                            previousFields = new Dictionary<string, MD.CloudConnect.Data.Field>();
                            foreach (KeyValuePair<string, MD.CloudConnect.Data.Field> item in t.Data.fields)
                            {
                                if (item.Value.b64_value != null)
                                    previousFields.Add(item.Key, item.Value);
                            }
                            t.Data.fields = previousFields;
                        }
                        else
                        {
                            Dictionary<string, MD.CloudConnect.Data.Field> tmp = new Dictionary<string, MD.CloudConnect.Data.Field>();
                            //remove not usefull fields to simulate the cloud alogrithm
                            foreach (KeyValuePair<string, MD.CloudConnect.Data.Field> item in t.Data.fields)
                            {
                                if ((previousFields.ContainsKey(item.Key) && previousFields[item.Key].b64_value != item.Value.b64_value)
                                    && t.Data.fields[item.Key].b64_value != null)
                                {
                                    tmp.Add(item.Key, item.Value);
                                    previousFields[item.Key].b64_value = item.Value.b64_value;
                                }
                            }
                            t.Data.fields = tmp;
                        }
                    }

                    result.TotalItems = tracks.Count;
                    result.TotalPages = tracks.Count / maxItemPerPage;

                    result.Page = page;
                    List<TrackingModel> tracksCurrentPage = tracks.Skip(page * maxItemPerPage).Take(maxItemPerPage).ToList();
                    result.NumberOfItems = tracksCurrentPage.Count;

                    List<MD.CloudConnect.MDData> data = new List<MD.CloudConnect.MDData>();

                    foreach (TrackingModel t in tracksCurrentPage)
                    {
                        MD.CloudConnect.MDData current = new MD.CloudConnect.MDData()
                        {
                            Meta = new MD.CloudConnect.Meta()
                            {
                                Account = "webdemo",
                                Event = "track",
                            },
                            Payload = Newtonsoft.Json.Linq.JObject.Parse(JsonConvert.SerializeObject(t.Data, Formatting.None))
                        };

                        data.Add(current);
                    }
                    result.Content = data.ToArray();
                }
            }
            return new ContentResult()
            {
                Content = JsonConvert.SerializeObject(result, Formatting.None),
                ContentType = "application/json"
            };
        }

        public ContentResult LoadData(string asset, int year, int month, int day)
        {
            List<JsonTrackingModel> result = new List<JsonTrackingModel>();
            if (!String.IsNullOrEmpty(asset))
            {
                DateTime date = new DateTime(year, month, day);
                DeviceModel device = RepositoryFactory.Instance.DeviceDb.GetDevice(asset);
                if (device != null)
                {
                    ViewBag.Imei = device.Imei;
                    List<TrackingModel> tracks = RepositoryFactory.Instance.DataTrackingDB.GetData(device, date).OrderBy(x => x.Data.Recorded_at).ToList();
                    List<string> keys = device.GetOrderFieldName();

                    JsonTrackingModel jtrack = null;

                    //for (int i = 0; i < tracks.Count; i++)
                    //{
                    //    TrackingModel t = tracks[i];
                    //    WriteCsvCell(writer, t.Data.Recorded_at.ToString("HH:mm:ss"));
                    //    WriteCsvCell(writer, t.Data.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    //    WriteCsvCell(writer, t.Data.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    //    foreach (string key in keys)
                    //    {
                    //        WriteCsvCell(writer, t.GetDisplayDataFor(key, (i < tracks.Count - 1 ? tracks[i + 1].Data.fields : null)));
                    //    }
                    //    writer.WriteLine();
                    //}

                    for (int i = 0; i < tracks.Count; i++)
                    {
                        TrackingModel t = tracks[i];
                        jtrack = new JsonTrackingModel()
                        {
                            Id = i,
                            Latitude = t.Data.Latitude,
                            Longitude = t.Data.Longitude,
                            Recorded_at = t.Data.Recorded_at.ToString("HH:mm:ss.fff"),
                            Received_at = t.Data.Received_at.ToString("dd/MM/yyyy HH:mm:ss"),
                            Fields = new List<JsonFieldModel>()
                        };

                        foreach (string key in keys)
                        {
                            jtrack.Fields.Add(new JsonFieldModel()
                            {
                                Key = key,
                                DisplayName = t.GetDisplayNameField(key),
                                Value = t.GetDisplayDataFor(key, (i > 0 ? tracks[i - 1].Data.fields : null))
                            });
                        }

                        result.Add(jtrack);
                    }

                }
            }

            return new ContentResult()
            {
                Content = JsonConvert.SerializeObject(result, Formatting.None),
                ContentType = "application/json"
            };
        }

        private void WriteCsvCell(StreamWriter writer, string content)
        {
            writer.Write(content);
            if (HttpContext.Request.UserLanguages != null && HttpContext.Request.UserLanguages[0].ToLower().Contains("en"))
                writer.Write(",");
            else
                writer.Write(";");
        }
    }
}
