using System;
using System.Collections.Generic;
using System.Text;

namespace Tramy.Common.Locations
{
    /// <summary>
    /// Locations after search
    /// </summary>
    public class NearLocation
    {
        /// <summary>
        /// Id of location
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Name of location
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Geo point
        /// </summary>
        public double[] GeoPoint { get; set; } = new double[2] { 0, 0 };

        /// <summary>
        /// Location's address
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Distance to me
        /// </summary>
        public double Distance { get; set; }
    }
}
