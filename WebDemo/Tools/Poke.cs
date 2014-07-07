using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebDemo.Tools
{
    [HubName("pokeHub")]
    public class PokeHub : Hub
    {
        public void Send(string name, string message)
        {
            // Call the broadcastMessage method to update clients.
            Clients.All.broadcastMessage(name,message);
        }

        internal static void BroadCastPoke(string message)
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<PokeHub>();
            context.Clients.All.BroadCastPoke(message);
        }
    }

}