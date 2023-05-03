using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Tramy.Backend.Data.DBServices;

namespace Tramy.Backend.Hubs
{
    /// <summary>
    /// 
    /// </summary>
    public class ChatHub:Hub
    {
        /// <summary>
        /// 
        /// </summary>
        public ChatHub()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chatService"></param>
        /// <param name="userService"></param>
        public ChatHub(ChatService chatService, UserService userService)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetConnectionId()
        {
            return Context.ConnectionId;
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
