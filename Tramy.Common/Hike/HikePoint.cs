using System;
using System.Collections.Generic;
using Tramy.Common.Common;

namespace Tramy.Common.Hike
{
    /// <summary>
    /// Point in a route
    /// </summary>
    public class HikePoint: BaseMongoItem
    {
        /// <summary>
        /// Geo coords
        /// </summary>
        public double[] GeoPoint { get; set; } = new double[2] { 0, 0 };

        /// <summary>
        /// Type of Point
        /// </summary>
        public HikePointType HikePointType { get; set; }

        /// <summary>
        /// User's id
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Description of a point
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Photos of point
        /// </summary>
        public List<Guid> IdPhotos { get; set; }

        /// <summary>
        /// Is point auto-generated
        /// </summary>
        public bool Auto { get; set; }
    }
}