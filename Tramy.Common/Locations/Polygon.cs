using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Tramy.Common.Common;

namespace Tramy.Common.Locations
{
    /// <summary>
    /// Locations polygon
    /// </summary>
    public class Polygon:BaseMongoItem
    {
        /// <summary>
        /// List of point
        /// </summary>
        public List<Point> Points = new List<Point>();
        /// <summary>
        /// Polygon's type
        /// </summary>
        public PolygonType PolygonType { get; set; } = PolygonType.None;
        /// <summary>
        /// Polygon's name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///Value of n for calculate RSSI
        /// </summary>
        public decimal N { get; set; }
        /// <summary>
        /// Type of material
        /// </summary>

        public string Material { get; set; }
        /// <summary>
        /// Thickness in meters
        /// </summary>

        public decimal Thickness { get; set; }
        /// <summary>
        /// Height in meters
        /// </summary>
        public decimal Height { get; set; }
        /// <summary>
        /// If door then true otherwise false
        /// </summary>
        public bool IsDoor { get; set; }
    }
}
