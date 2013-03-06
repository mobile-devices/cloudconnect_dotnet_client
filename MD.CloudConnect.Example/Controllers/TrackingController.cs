using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CloudConnectReader.Models;
using CloudConnectReader.Models.Repository;
using CloudConnectReader.Tools;
using System.IO;
using System.Web.Script.Serialization;

namespace CloudConnectReader.Controllers
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
            var serializer = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue, RecursionLimit = 100 };

            return new ContentResult()
            {
                Content = serializer.Serialize(ExtendedFieldDefinition.Fields.Select(x => x.Key).ToList()),
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
                    List<TrackingModel> tracks = RepositoryFactory.Instance.DataTrackingDB.GetData(device, date).ToList();
                    List<string> keys = device.GetOrderFieldName();

                    JsonTrackingModel jtrack = null;
                    for (int i = 0; i < tracks.Count; i++)
                    {
                        TrackingModel t = tracks[i];
                        jtrack = new JsonTrackingModel()
                        {
                            Id = i,
                            Latitude = t.Data.Latitude,
                            Longitude = t.Data.Longitude,
                            Recorded_at = t.Data.Recorded_at.ToString("HH:mm:ss"),
                            Fields = new List<JsonFieldModel>()
                        };

                        foreach (string key in keys)
                        {
                            jtrack.Fields.Add(new JsonFieldModel()
                            {
                                Key = key,
                                DisplayName = t.GetDisplayNameField(key),
                                Value = t.GetDisplayDataFor(key, (i < tracks.Count - 1 ? tracks[i + 1].Data.fields : null))
                            });
                        }

                        result.Add(jtrack);
                    }

                }
            }
            var serializer = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue, RecursionLimit = 100 };

            return new ContentResult()
            {
                Content = serializer.Serialize(result),
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
