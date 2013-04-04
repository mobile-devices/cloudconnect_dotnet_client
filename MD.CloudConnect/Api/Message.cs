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

        public void PostMessage(string asset, string channel, string message)
        {
            
        }
    }
}
