using System;
using System.Collections.Generic;
using System.Text;
using Tramy.Common.Common;

namespace Tramy.Common.Chats
{
    /// <summary>
    /// User's chat
    /// </summary>
    public class Chat:BaseMongoItem
    {
        /// <summary>
        /// Chat's name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// List of users
        /// </summary>
        public List<Guid> Users { get; set; } = new List<Guid>();
    }
}
