using System;
using System.Collections.Generic;
using System.Text;

namespace Tramy.Common.Notification
{
    /// <summary>
    /// Is friendship approve or disapprove notification
    /// </summary>
    public class FriendshipApproveNotification:UserNotification
    {
        /// <summary>
        /// Result
        /// </summary>
        public bool Approve { get; set; }
    }
}
