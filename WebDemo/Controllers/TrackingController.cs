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
using Newtonsoft.Json;
using MD.CloudConnect.CouchBaseProvider;

namespace WebDemo.Controllers
{
    public class TrackingController : Controller
    {
        //
        // GET: /Tracking/

        public ActionResult Index(string asset = "", int year = 2013, int month = 2, int day = 28, string raw = "off", string debug = "off", string server_date = "off")
        {
            IEnumerable<Track> tracks = new List<Track>();
            ViewBag.Imei = "";
            ViewBag.Fields = new List<string>();
            ViewBag.Date = DateTime.MinValue;
            ViewBag.DebugOn = false;
            ViewBag.ServerDate = false;
            if (server_date == "on")
                ViewBag.ServerDate = true;
            if (debug == "on")
                ViewBag.DebugOn = true;
            if (!String.IsNullOrEmpty(asset))
            {
                Dictionary<string, string> fieldPresent = new Dictionary<string, string>();
                DateTime date = new DateTime(year, month, day);
                Device device = RepositoryFactory.Instance.DeviceDb.GetDevice(asset);
                if (device != null)
                {
                    ViewBag.Imei = device.Imei;
                    tracks = RepositoryFactory.Instance.DataTrackingDB.GetData(device, date).OrderByDescending(x => x.Recorded_at);
                    if (debug == "off")
                        tracks = tracks.Where(x => x.Status == Track.OK_REBUILDED || x.Status == Track.DATA_FEED_REBUILDED);

                    foreach (Track t in tracks)
                    {
                        if (raw == "on")
                            t.Fields = t.Fields.Where(items => items.Value.RecordedAt == t.Recorded_at).ToDictionary(pair => pair.Key, pair => pair.Value);

                        foreach (Field f in t.Fields.Values)
                        {

                            if (!fieldPresent.ContainsKey(f.Key))
                                fieldPresent.Add(f.Key, f.Key);
                        }
                    }
                    ViewBag.DateKey = date.GenerateKey();
                    ViewBag.Date = date;
                    ViewBag.Fields = FieldHelper.GetFields(fieldPresent.Keys.ToList());
                }
            }

            return View(tracks.ToList());
        }

        private int CanBeUpdate(Device d, Track t)
        {
            if (String.IsNullOrEmpty(t.ConnectionId))
                return 4;
            if (d.LastConnectionId == null)
                return 1;

            if (d.LastConnectionId == t.ConnectionId)
            {
                if (d.NextWaitingIndex == t.Index)
                    return 1;
                else if (d.LastIndex == t.Index)
                    return 3;
            }
            else
            {
                //hack connection id change but index still increase
                if (d.NextWaitingIndex == t.Index || t.Index == 1)
                    return 1;
                if (d.LastRecordedAt > t.Recorded_at && (d.LastRecordedAt.Ticks - t.Recorded_at.Ticks) > (TimeSpan.TicksPerMinute * 15))
                    return 5;
            }

            // timeout
            if ((DateTime.UtcNow.Ticks - t.Created_at.Ticks) > (TimeSpan.TicksPerMinute * 5))
            {
                //try to rebuild if recorded at is correct
                if (t.Recorded_at > d.LastRecordedAt)
                    return 1;
                else
                    return 2;
            }
            // wait more data
            return 0;
        }

        private void mergeField(Device d, Track t)
        {
            foreach (KeyValuePair<string, Field> item in t.Fields)
            {
                if (d.Fields.ContainsKey(item.Key))
                    d.Fields[item.Key] = item.Value;
                else d.Fields.Add(item.Key, item.Value);
            }

            foreach (KeyValuePair<string, Field> item in d.Fields)
            {
                if (FieldHelper.MustBeRebuild(item.Value.Key) && !t.Fields.ContainsKey(item.Key))
                {
                    t.Fields[item.Key] = item.Value;
                }
            }
        }

        private bool UpdateFieldHistory(Device d, Track t)
        {
            //already decoded but download in the view because all nodes are yet all updated
            if (t.Status > 0)
                return false;
            t.Updated_at = DateTime.UtcNow;
            int mergeStatus = CanBeUpdate(d, t);
            if (mergeStatus == 1)
            {
                d.NextWaitingIndex = t.Index.Value + (uint)(t.Fields.Count + 1);
                if (t.Latitude != 0.0 && t.Longitude != 0.0)
                {
                    d.LastLatitude = t.Latitude;
                    d.LastLongitude = t.Longitude;
                }
                d.LastCloudId = t.CloudId;
                d.LastReceivedAt = t.Received_at;
                d.LastRecordedAt = t.Recorded_at;
                d.LastConnectionId = t.ConnectionId;
                d.LastIndex = t.Index.Value;
                d.UpdatedAt = DateTime.UtcNow;
                d.LastTrackId = t.Id;

                mergeField(d, t);
                t.Status = 1;
            }
            else if (mergeStatus == 0)
            {
                return true;
            }
            else if (mergeStatus == 2)
            {
                //specific case - timeout
                // Add log here
                //same connection id and index
                t.Status = 3;
            }
            else if (mergeStatus == 3)
            {
                // Add log here
                t.Status = 2;
            }
            else if (mergeStatus == 4)
            {
                foreach (KeyValuePair<string, Field> item in t.Fields)
                {
                    if (d.Fields.ContainsKey(item.Key))
                        d.Fields[item.Key] = item.Value;
                    else d.Fields.Add(item.Key, item.Value);
                }
                t.Status = 4;
            }
            else if (mergeStatus == 5)
            {
                // recorded at was in the past compare to the data already decoded for this device
                t.Status = 5;
            }
            return false;
        }

        public ContentResult RebuildedData(string asset, int year, int month, int day)
        {
            List<string> result = new List<string>();
            asset = asset.Trim();
            if (!String.IsNullOrEmpty(asset))
            {
                DateTime date = new DateTime(year, month, day);
                int datekey = date.GenerateKey();
                Device device = RepositoryFactory.Instance.DeviceDb.GetDevice(asset);
                if (device != null)
                {
                    List<Track> tracks = RepositoryFactory.Instance.DataTrackingDB.GetData(device, date).OrderBy(x => x.Recorded_at).ToList();

                    result.Add(String.Format("[BEFORE Track]{0} tracks - waiting data", tracks.Where(x => x.Status == 0).Count()));
                    result.Add(String.Format("[BEFORE Track]{0} tracks - ok", tracks.Where(x => x.Status == 1).Count()));
                    result.Add(String.Format("[BEFORE Track]{0} tracks - timeout", tracks.Where(x => x.Status == 2).Count()));
                    result.Add(String.Format("[BEFORE Track]{0} tracks - bad time", tracks.Where(x => x.Status == 5).Count()));
                    result.Add(String.Format("[BEFORE Track]{0} tracks - others", tracks.Where(x => x.Status != 2 && x.Status != 1 && x.Status != 5 && x.Status != 0).Count()));

                    Track first = tracks.Where(x => x.Status == 1).First();
                    device.NextWaitingIndex = first.Index.Value + (uint)(first.Fields.Count + 1);
                    if (first.Latitude != 0.0 && first.Longitude != 0.0)
                    {
                        device.LastLatitude = first.Latitude;
                        device.LastLongitude = first.Longitude;
                    }
                    device.LastCloudId = first.CloudId;
                    device.LastReceivedAt = first.Received_at;
                    device.LastRecordedAt = first.Recorded_at;
                    device.LastConnectionId = first.ConnectionId;
                    device.LastIndex = first.Index.Value;
                    device.UpdatedAt = DateTime.UtcNow;
                    device.LastTrackId = first.Id;
                    device.Fields = first.Fields;
                    //clean tracks & rebuild
                    foreach (Track t in tracks)
                    {
                        if (t.Recorded_at > device.LastRecordedAt)
                        {
                            t.Fields = t.Fields.Where(items => items.Value.RecordedAt == t.Recorded_at).ToDictionary(pair => pair.Key, pair => pair.Value);
                            t.Status = 0;
                            UpdateFieldHistory(device, t);
                        }
                    }
                    result.Add(String.Format("[AFTER Track]{0} tracks - waiting data", tracks.Where(x => x.Status == 0).Count()));
                    result.Add(String.Format("[AFTER Track]{0} tracks - ok", tracks.Where(x => x.Status == 1).Count()));
                    result.Add(String.Format("[AFTER Track]{0} tracks - timeout", tracks.Where(x => x.Status == 2).Count()));
                    result.Add(String.Format("[AFTER Track]{0} tracks - bad time", tracks.Where(x => x.Status == 5).Count()));
                    result.Add(String.Format("[AFTER Track]{0} tracks - others", tracks.Where(x => x.Status != 2 && x.Status != 1 && x.Status != 5 && x.Status != 0).Count()));
                    CouchbaseManager.Instance.TrackRepository.BulkUpsert(tracks);
                }
            }
            return new ContentResult()
            {
                Content = JsonConvert.SerializeObject(result, Formatting.None),
                ContentType = "application/json"
            };
        }

        public FileResult CsvExport(string asset = "", int dateKey = 20130228)
        {
            MemoryStream memStream = new MemoryStream();
            StreamWriter writer = new StreamWriter(memStream);

            if (!String.IsNullOrEmpty(asset))
            {
                Device device = RepositoryFactory.Instance.DeviceDb.GetDevice(asset);
                if (device != null)
                {
                    List<string> keys = FieldHelper.GetFields(device.Fields.Values.Select(x => x.Key).ToList()); //new List<string>(device.Fields.Values.Select(x => x.Key).ToList());
                    WriteCsvCell(writer, "Date");
                    WriteCsvCell(writer, "Recorded_at");
                    WriteCsvCell(writer, "Longitude");
                    WriteCsvCell(writer, "Latitude");
                    foreach (string key in keys)
                    {
                        WriteCsvCell(writer, key);
                    }
                    DateTime date = DateTime.ParseExact(dateKey.ToString(), "yyyyMMdd", null);
                    List<Track> tracks = RepositoryFactory.Instance.DataTrackingDB.GetData(device, date).OrderByDescending(x => x.Recorded_at).Where(x => x.Status == 1).ToList();
                    writer.WriteLine();
                    for (int i = 0; i < tracks.Count; i++)
                    {
                        Track t = tracks[i];
                        WriteCsvCell(writer, t.Recorded_at.ToString("yyyy/MM/dd"));
                        WriteCsvCell(writer, t.Recorded_at.ToString("HH:mm:ss"));
                        WriteCsvCell(writer, t.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture));
                        WriteCsvCell(writer, t.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture));
                        foreach (string key in keys)
                        {
                            WriteCsvCell(writer, FieldHelper.GetDisplayDataFor(key, t.Fields, (i < tracks.Count - 1 ? tracks[i + 1].Fields : null)));
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
                authorizedKeys = FieldHelper.GetFields(),
                defaultKeyMap = new string[] { 
                    "GPRMC_VALID",
                    "GPS_SPEED",
                    "DIO_IGNITION",
                    "ODO_FULL",
                    "MDI_OBD_SPEED",    
                    "MDI_OBD_RPM",
                    "MDI_OBD_FUEL",
                    "MDI_OBD_MILEAGE",
                    "MDI_RECORD_REASON"
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
                    "MDI_OBD_MILEAGE",
                    "MDI_RECORD_REASON"
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
                Device device = RepositoryFactory.Instance.DeviceDb.GetDevice(asset);
                if (device != null)
                {
                    List<Track> tracks = RepositoryFactory.Instance.DataTrackingDB.GetData(device, date).OrderBy(x => x.Recorded_at).Where(x => x.Status == 1).ToList();

                    Dictionary<string, Field> previousFields = null;
                    foreach (Track t in tracks)
                    {
                        if (previousFields == null)
                        {
                            previousFields = new Dictionary<string, Field>();
                            foreach (KeyValuePair<string, Field> item in t.Fields)
                            {
                                if (item.Value.B64Value != null)
                                    previousFields.Add(item.Key, item.Value);
                            }
                            t.Fields = previousFields;
                        }
                        else
                        {
                            Dictionary<string, Field> tmp = new Dictionary<string, Field>();
                            //remove not usefull fields to simulate the cloud alogrithm
                            foreach (KeyValuePair<string, Field> item in t.Fields)
                            {
                                if ((previousFields.ContainsKey(item.Key) && previousFields[item.Key].B64Value != item.Value.B64Value)
                                    && t.Fields[item.Key].B64Value != null)
                                {
                                    tmp.Add(item.Key, item.Value);
                                    previousFields[item.Key].B64Value = item.Value.B64Value;
                                }
                            }
                            t.Fields = tmp;
                        }
                    }

                    result.TotalItems = tracks.Count;
                    result.TotalPages = tracks.Count / maxItemPerPage;

                    result.Page = page;
                    List<Track> tracksCurrentPage = tracks.Skip(page * maxItemPerPage).Take(maxItemPerPage).ToList();
                    result.NumberOfItems = tracksCurrentPage.Count;

                    List<MD.CloudConnect.MDData> data = new List<MD.CloudConnect.MDData>();

                    foreach (Track t in tracksCurrentPage)
                    {
                        MD.CloudConnect.MDData current = new MD.CloudConnect.MDData()
                        {
                            Meta = new MD.CloudConnect.Meta()
                            {
                                Account = "webdemo",
                                Event = "track",
                            },
                            Payload = Newtonsoft.Json.Linq.JObject.Parse(JsonConvert.SerializeObject(t, Formatting.None))
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

        public ContentResult LoadData(string asset, int year, int month, int day, int limit = 100, string nextDocID = "")
        {
            asset = asset.Trim();
            List<JsonTrackingModel> result = new List<JsonTrackingModel>();
            if (!String.IsNullOrEmpty(asset))
            {
                DateTime date = new DateTime(year, month, day);
                int datekey = date.GenerateKey();
                Device device = RepositoryFactory.Instance.DeviceDb.GetDevice(asset);
                if (device != null)
                {
                    ViewBag.Imei = device.Imei;
                    List<Track> tracks = RepositoryFactory.Instance.DataTrackingDB.GetData(device, date).OrderByDescending(x => x.Recorded_at).Where(x => x.Status == 1).ToList();
                    List<string> keys = FieldHelper.GetFields(device.Fields.Values.Select(x => x.Key).ToList());
                    JsonTrackingModel jtrack = null;
                    tracks.Reverse();
                    for (int i = 0; i < tracks.Count; i++)
                    {
                        Track t = tracks[i];
                        jtrack = new JsonTrackingModel()
                        {
                            Id = i,
                            DocId = t.Id,
                            Latitude = t.Latitude,
                            Longitude = t.Longitude,
                            Recorded_at = t.Recorded_at.ToString("HH:mm:ss.fff"),
                            Received_at = t.Received_at.ToString("dd/MM/yyyy HH:mm:ss"),
                            Fields = new List<JsonFieldModel>()
                        };

                        foreach (string key in keys)
                        {
                            jtrack.Fields.Add(new JsonFieldModel()
                            {
                                Key = key,
                                DisplayName = FieldHelper.GetDisplayNameField(key),
                                Value = FieldHelper.GetDisplayDataFor(key, t.Fields, (i > 0 ? tracks[i - 1].Fields : null))
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
