using System;
using System.Collections.Generic;
using System.Text;

namespace Tramy.Common.Notification
{
    /// <summary>
    /// Notification to start friendship
    /// </summary>
    public class FriendshipStartNotification:UserNotification
    {
        /// <summary>
        /// User to start friendship
        /// </summary>
        public Guid UserFromId { get; set; }

        /// <summary>
        /// Is approved
        /// </summary>
        public bool? Approved { get; set; }

        /// <summary>
        /// Date of decision
        /// </summary>
        public DateTime? DecisionDate { get; set; }
    }
}
