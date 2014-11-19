using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MD.CloudConnect
{
    public interface INotificationData
    {
        // ID (unique)
        string Id { get; set; }
        long Received_at { get; set; }
        // Key to dispatch data
        string Key { get; set; }
        string Data { get; set; }

        /// <summary>
        /// 0 : not yet decoded
        /// 1 : decoded
        /// 2 : ignored
        /// 3 : partial decoded (some data use, some data dropped)
        /// </summary>
        int Status { get; set; }
    }
}
