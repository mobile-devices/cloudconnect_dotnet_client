using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MD.CloudConnect
{
    [Serializable]
    public class MDData
    {
        public Meta Meta { get; set; }
        public Payload Payload { get; set; }

        public ITracking Tracking
        {
            get
            {
                if (Meta != null && Meta.Event == "track" && Payload != null)
                    return (ITracking)Payload;
                else
                    return null;
            }
        }

        public IMessage Message
        {
            get
            {
                if (Meta != null && Meta.Event == "message" && Payload != null)
                    return (IMessage)Payload;
                else
                    return null;
            }

        }
    }
}
