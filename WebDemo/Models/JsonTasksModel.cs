using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebDemo.Models
{
    public class JsonTask
    {
        public int Id { get; set; }
        public string Info { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
    }

    public class JsonTasksModel
    {
        public int ID { get; set; }
        public string Asset { get; set; }
        public string Account { get; set; }
        public string Environment { get; set; }
        public string Token { get; set; }
        public string TaskName { get; set; }
        public string Info { get; set; }

        public JsonTask[] Tasks { get; set; }

        public string getMessage()
        {
            string result = "{\"MESSAGE\":{\"TYPE\":\"NEW\",\"VERSION_MSG\":\"1\",\"VERSION\":\"12\",\"PACKET\":\"1\",\"PACKET_TOTAL\":\"" + this.Tasks.Length + "\",";
            result += "\"ID\":\"" + this.ID.ToString() + "\",\"LABEL\":\"" + this.TaskName + "\",\"INFO\":\"" + this.Info + "\",";
            result += "\"STEPS\":[";

            for (int i = 0; i < this.Tasks.Length; i++)
            {
                JsonTask t = this.Tasks[i];
                result += "{\"ID\":\"" + t.Id.ToString() + "\",\"LABEL\":\"" + t.Info + "\",\"LATITUDE\":\"" + t.Latitude + "\", \"LONGITUDE\":\"" + t.Longitude + "\"";
                if (i < this.Tasks.Length)
                    result += "},";
                else
                    result += "}";

            }
            result += "]}}";

            return result;
        }
    }
}