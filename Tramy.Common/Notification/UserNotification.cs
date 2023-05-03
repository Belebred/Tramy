using System;
using System.Collections.Generic;
using System.Text;
using Tramy.Common.Common;

namespace Tramy.Common.Notification
{
    /// <summary>
    /// User's notification
    /// </summary>
    public class UserNotification:BaseMongoItem
    {
        /// <summary>
        /// User to send notification
        /// </summary>
        public Guid UserToId { get; set; }

        /// <summary>
        /// User read notification
        /// </summary>
        public bool IsRead { get; set; }

        /// <summary>
        /// Send notification to user
        /// </summary>
        public bool IsSend { get; set; }

        /// <summary>
        /// Notification message
        /// </summary>
        public string Message { get; set; }
    }
}
