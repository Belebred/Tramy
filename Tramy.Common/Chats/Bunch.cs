using System;
using System.Collections.Generic;
using System.Text;
using Tramy.Common.Common;
using Tramy.Common.Users;

namespace Tramy.Common.Chats
{
    /// <summary>
    /// Class for a bunch of people
    /// </summary>
    public class Bunch : BaseMongoItem
    {
        /// <summary>
        /// Name of the bunch
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Logo of the bunch
        /// </summary>
        public string Logo { get; set; }

        /// <summary>
        /// List of admins
        /// </summary>
        public List<Guid> Admins {get; set;}

        /// <summary>
        /// List of bunch interests
        /// </summary>
        public List<string> Interests { get; set; }

        /// <summary>
        /// List of chats
        /// </summary>
        public List<Guid> Chats { get; set; }

        /// <summary>
        /// City, where bunch exists
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Is bunch gathered or not
        /// </summary>
        public bool Gather { get; set; }


        /// <summary>
        /// List of users in the bunch
        /// </summary>
        public List<Guid> Users { get; set; } = new List<Guid>();
        /// <summary>
        /// List of users invited to the bunch
        /// </summary>
        public List<Guid> InvitedUsers { get; set; } = new List<Guid>();
        /// <summary>
        /// List of users request to the bunch
        /// </summary>
        public List<Guid> UserRequests { get; set; } = new List<Guid>();


        /// <summary>
        /// Open company or close
        /// </summary>
        public bool Open { get; set; }

    }
}
