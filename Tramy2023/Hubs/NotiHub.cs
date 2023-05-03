using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Tramy.Backend.Data.DBServices;

namespace Tramy.Backend.Hubs
{
    /// <summary>
    /// Notification service
    /// </summary>
    public class NotiHub : Hub
    {

        /// <summary>
        /// 
        /// </summary>
        public NotiHub()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="notificationService"></param>
        /// <param name="userService"></param>
        public NotiHub(NotificationService notificationService, UserService userService)
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
