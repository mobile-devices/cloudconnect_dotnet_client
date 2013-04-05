using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MD.CloudConnect.Data;

namespace MD.CloudConnect.Api
{
    public class Message : AbstractApi<MessageData>
    {
        public override string Function
        {
            get { return "messages"; }
        }

        /// <summary>
        /// Encode the message in base64 and send it throught the cloud
        /// </summary>
        /// <param name="asset">An asset identifier to send the message to</param>
        /// <param name="channel">Channel identifier</param>
        /// <param name="message">Content message (will be automaticly encoded in base64, no need to encode yourself)</param>
        /// <param name="timeout">if -1 never timeout else the delay in minutes before the message is considered invalid (will not be sent)</param>
        /// <returns>
        /// 0  : message send
        /// -1 : channel does not exist
        /// </returns>
        public int PostMessage(string asset, string channel, string message, int timeout = -1)
        {
            ChannelData chan = this.Cloud.Channel.Get(channel);

            if (chan == null)
                return 1;
            else
            {
                string encodedMsg = MD.CloudConnect.Tools.Base64Encoder.GetEncodedValue(message);
                string jsonParam = "";
   
                jsonParam += "{ \"asset\": \"" + asset + "\"";
                jsonParam += ", \"channel\": \"" + channel + "\"";
                jsonParam += ", \"b64_payload\": \"" + encodedMsg + "\"";

                if (timeout > 0)
                    jsonParam += ", \"timeout\": \"" + timeout.ToString() + "\"";

                jsonParam += "}";

                this.Post(jsonParam);
            }
            return 0;
        }
    }
}
