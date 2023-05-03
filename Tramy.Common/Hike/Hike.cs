using System;
using System.Collections.Generic;
using System.Text;
using Tramy.Common.Common;

namespace Tramy.Common.Hike
{
    /// <summary>
    /// Hike definition
    /// </summary>
    public class Hike: BaseMongoItem
    {
        /// <summary>
        /// Creator of hike
        /// </summary>
        public Guid Creator { get; set; }

        /// <summary>
        /// Leader of the team
        /// </summary>
        public Guid TeamLeader { get; set; }

        /// <summary>
        /// Group
        /// </summary>
        public List<Guid> Group { get; set; }

        /// <summary>
        /// Request from user
        /// </summary>
        public List<Guid> UserRequests { get; set; }

        /// <summary>
        /// Invite to user
        /// </summary>
        public List<Guid> UserInvites { get; set; }

        /// <summary>
        /// Type of the hike
        /// </summary>
        public HikeType HikeType { get; set; }
        
        /// <summary>
        /// Start of Hike
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// End of the Hike
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Is Hike completed
        /// </summary>
        public bool Completed { get; set; }

        /// <summary>
        /// Toute of Hike
        /// </summary>
        public List<Guid> Route { get; set; }

        /// <summary>
        /// Rating of Hike
        /// </summary>
        public decimal Rate { get; set; }

        /// <summary>
        /// Desired Number of people in Hike
        /// </summary>
        public int DesiredNumber { get; set; }

        /// <summary>
        /// Is Hike open
        /// </summary>
        public bool Open { get; set; }

        /// <summary>
        /// Country where Hike goes on
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// State of Country
        /// </summary>
        public string State { get; set; }
    }
}
