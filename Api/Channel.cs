using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MD.CloudConnect.Data;
using System.Web;
using System.Net;

namespace MD.CloudConnect.Api
{
    public class Channel : AbstractApi<ChannelData>
    {
        public override string Function
        {
            get { return "channels"; }
        }

        public ChannelData Get(string name)
        {
            try
            {
                return this.GetOne(name);
            }
            catch (WebException error)
            {
                if (error.Response.Headers.AllKeys.Contains("Status") && error.Response.Headers["status"].Contains("404"))
                    return null;
                else throw error;
            }
        }
    }
}
