using System;
using System.Collections.Generic;
using System.Text;
using Tramy.Common.Common;

namespace Tramy.Common.Chats
{
    /// <summary>
    /// Chat message
    /// </summary>
    public class ChatMessage:BaseMongoItem
    {
        /// <summary>
        /// User to send message
        /// </summary>
        public Guid UserFromId { get; set; }
        
        /// <summary>
        ///  Date and time of message
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Is message edited
        /// </summary>
        public bool IsEdited { get; set; }

        /// <summary>
        ///  User to read message
        /// </summary>
        public List<Guid> UsersDelivery { get; set; } = new List<Guid>();

        /// <summary>
        /// Message Id to comment
        /// </summary>
        public Guid? CommentToId { get; set; }
    }
}
