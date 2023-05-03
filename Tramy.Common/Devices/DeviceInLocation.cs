using System;
using System.Collections.Generic;
using System.Text;
using Tramy.Common.Common;

namespace Tramy.Common.Devices
{
    /// <summary>
    /// Device in location definition
    /// </summary>
    public class DeviceInLocation:BaseMongoItem
    {
        /// <summary>
        /// Device Id
        /// </summary>
        public string MAC { get; set; }

        /// <summary>
        /// Device Id
        /// </summary>
        public Guid LocationPartId { get; set; }

        /// <summary>
        /// X position in pixels
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Y position in pixels
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Koeff of x scale
        /// </summary>
        public decimal KX { get; set; }

        /// <summary>
        /// Koeff of y scale
        /// </summary>
        public decimal KY { get; set; }

        /// <summary>
        /// X position in meters
        /// </summary>
        public decimal RealX => KX == 0?0:X / KX;

        /// <summary>
        /// Y Position In meters
        /// </summary>
        public decimal RealY => KY == 0 ? 0 : Y / KY;

        /// <summary>
        /// Initial D
        /// </summary>
        public decimal D0 => 2;

        /// <summary>
        /// Initial P
        /// </summary>
        public decimal P0 => -70;

    }
}
