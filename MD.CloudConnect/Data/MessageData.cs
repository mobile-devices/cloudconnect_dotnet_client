using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MD.CloudConnect.Data
{
    public class MessageData : IMessage
    {
        public string Id { get; set; }
        public string StrId { get; set; }
        public string ParentId { get; set; }
        public string ParentIdStr { get; set; }
        public int ThreadId { get; set; }
        public string ThreadIdStr { get; set; }
        public string Channel { get; set; }
        public string Type { get; set; }
        public string Sender { get; set; }
        public string Recipient { get; set; }
        public string Asset { get; set; }
        public string Length { get; set; }
        public string b64_payload { get; set; }
        public DateTime Received_at { get; set; }
        public DateTime Recorded_at { get; set; }
        public DateTime Created_at { get; set; }
        public string Url { get; set; }

        private string _payload = string.Empty;
        public string Payload
        {
            get
            {
                if (String.IsNullOrEmpty(_payload) && !String.IsNullOrEmpty(b64_payload))
                {
                    _payload = MD.CloudConnect.Tools.Base64Decoder.GetValueAsString(b64_payload);
                }
                return _payload;
            }
        }


        public string Message
        {
            get { return Payload; }
        }

        public string Id_str
        {
            get { return StrId; }
        }
    }
}
