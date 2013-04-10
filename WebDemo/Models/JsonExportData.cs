using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebDemo.Models
{
    public class JsonExportData
    {
        public int Page { get; set; }
        public int NumberOfItems { get; set; }

        public int TotalPages { get; set; }
        public int TotalItems { get; set; }

        public MD.CloudConnect.MDData[] Content { get; set; }
    }
}