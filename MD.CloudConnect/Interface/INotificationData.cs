using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MD.CloudConnect
{
    public interface INotificationData
    {
        long Received_at { get; set; }
        string Key { get; set; }
        string Content { get; set; }
        //string Group { get; set; }
    }
}
