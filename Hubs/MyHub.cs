using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace MyBot.Hubs
{
    public class MyHub : Hub
    {
        public void SendMessageTo(string message, string user)
        {
            Clients.All.Notify(message, user);
        }
    }
}