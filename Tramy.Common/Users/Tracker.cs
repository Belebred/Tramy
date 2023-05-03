using System;
using System.Collections.Generic;
using System.Text;
using Tramy.Common.Common;

namespace Tramy.Common.Users
{
    /// <summary>
    /// Tracker class of Tramy
    /// </summary>
    public class Tracker: BaseMongoItem
    {
        /// <summary>
        /// User
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Date and time of start
        /// </summary>
        public DateTime Start { get; set; } = new DateTime(DateTime.Now.Ticks);

        /// <summary>
        /// Date and time of ending
        /// </summary>
        public DateTime End { get; set; }

        /// <summary>
        /// Points of the route
        /// </summary>
        public Dictionary<DateTime, double[]> RoutePoints { get; set; }
    }
}
