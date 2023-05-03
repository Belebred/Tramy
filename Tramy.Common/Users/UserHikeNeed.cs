using System;
using System.Collections.Generic;
using System.Text;
using Tramy.Common.Common;
using Tramy.Common.Hike;

namespace Tramy.Common.Users
{
    /// <summary>
    /// Class of User need for Hike
    /// </summary>
    public class UserHikeNeed: BaseMongoItem
    {
        /// <summary>
        /// User's id
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Type of Hikem can be many
        /// </summary>
        public HikeType[] HikeType { get; set; }

        /// <summary>
        /// Start of the Hike
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// End of the Hike
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Max date shift, default = 3
        /// </summary>
        public int MaxShift { get; set; } = 3;

        /// <summary>
        /// Is need completed
        /// </summary>
        public bool Completed { get; set; }
    }
}
