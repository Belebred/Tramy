
using System;
using System.Collections.Generic;
using Tramy.Common.Common;

namespace Tramy.Common.Locations
{
    /// <summary>
    /// Location
    /// </summary>
    public class Location : BaseMongoItem
    {
        /// <summary>
        /// Name of location
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Location's description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Geo point
        /// </summary>
        public double[] GeoPoint { get; set; } = new double[2] {0, 0};

        /// <summary>
        /// Location's address
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Parts in location. IEnumerable of links to LocationPart
        /// </summary>
        public List<Guid> Parts { get; set; } = new List<Guid>();

    }
}
