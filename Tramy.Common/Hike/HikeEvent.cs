using System;
using System.Collections.Generic;
using System.Text;
using Tramy.Common.Common;

namespace Tramy.Common.Hike
{
    /// <summary>
    /// Event happened during Hike
    /// </summary>
    public class HikeEvent: BaseMongoItem
    {
        /// <summary>
        /// Name of the Event
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Id of the Hike
        /// </summary>
        public Guid HikeId { get; set; }

        /// <summary>
        /// GeoCoords of Event
        /// </summary>
        public double?[] GeoPoint { get; set; } = new double?[2] { 0, 0 };
    }
}
