using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace WebDemo.Controllers
{
    public class LognotificationController : Controller
    {
        //
        // GET: /Lognotification/

        [Authorize(Roles = "Admin")]
        public FileResult Index(string date)
        {
            MemoryStream memStream = new MemoryStream();
            StreamWriter writer = new StreamWriter(memStream);


            writer.Flush();
            memStream.Seek(0, SeekOrigin.Begin);
            FileStreamResult file = new FileStreamResult(memStream, "text/csv");
            file.FileDownloadName = String.Format("export_log_notification_{0}.csv", date);
            return file;
        }

    }
}
