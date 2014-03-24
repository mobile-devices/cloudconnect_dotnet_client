using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebDemo.Models
{
    public abstract class FeedResult
    {
        public virtual string feed_type { get; set; }
        public string date { get; set; }
        public string asset { get; set; }
        public string rule_id { get; set; }
        public double[] location { get; set; }
    }

    public class TrackingFeedResult : FeedResult
    {
        public Dictionary<string, string> content { get; set; }

        public override string feed_type
        {
            get
            {
                return "track";
            }
        }
    }

    public class MessageFeedResult : FeedResult
    {
        public string content { get; set; }

        public override string feed_type
        {
            get
            {
                return "message";
            }
        }
    }
}