using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebDemo.Models
{
    //{"results":[{"resultList":[{"data":[4,144],"commandName":"OBD_RPM"}],"result":0,"actionListName":"getRPM"}],"requestId":"null-2"}
    public class ObdStackMessage
    {
        public List<ObdStackCommand> Results { get; set; }
        public string RequestId { get; set; }
    }

    public class ObdStackCommand
    {
        public List<ObsStackResultCommand> ResultList { get; set; }
        public string Result { get; set; }
        public string ActionListName { get; set; }
    }

    public class ObsStackResultCommand
    {
        public int[] Data { get; set; }
        public string CommandName { get; set; }
    }
}