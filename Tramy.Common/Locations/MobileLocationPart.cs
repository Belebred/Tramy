using System;
using System.Collections.Generic;
using System.Text;
using Tramy.Common.Devices;
using Tramy.Common.Navigation;

namespace Tramy.Common.Locations
{
    /// <summary>
    /// Location part to mobile
    /// </summary>
    public class MobileLocationPart
    {
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Has entry from street
        /// </summary>
        public bool HasEntry { get; set; }

        /// <summary>
        /// Is Main
        /// </summary>
        public bool IsMain { get; set; }


        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// List of devices
        /// </summary>
        public IEnumerable<DeviceInLocation> Devices { get; set; }


        /// <summary>
        /// List of devices
        /// </summary>
        public IEnumerable<Polygon> Polygons { get; set; }

        /// <summary>
        /// Size Y in meters
        /// </summary>
        public decimal SizeY { get; set; }

        /// <summary>
        /// Size X in meters
        /// </summary>
        public decimal SizeX { get; set; }

        /// <summary>
        /// Scale x
        /// </summary>
        public decimal Kx { get; set; }

        /// <summary>
        /// Scale y
        /// </summary>
        public decimal Ky { get; set; }


        /// <summary>
        /// Measured points in location part. IEnumerable of links to Measured Point
        /// </summary>
        public IEnumerable<MeasuredPoint> MeasuredPoints { get; set; } = new List<MeasuredPoint>();

        /// <summary>
        /// Create mobile part from server part
        /// </summary>
        /// <param name="part">Location part</param>
        /// <param name="devices">List of devices</param>
        /// <param name="points">List of measured points</param>
        /// <param name="polygons">List of polygons</param>
        /// <returns></returns>
        public static MobileLocationPart FromLocationPart(LocationPart part, IEnumerable<DeviceInLocation> devices, IEnumerable<MeasuredPoint> points, IEnumerable<Polygon> polygons)
        {
            return new MobileLocationPart()
            {
                IsMain = part.IsMain,
                Id = part.Id,
                HasEntry = part.HasEntry,
                Name = part.Name,
                SizeX = part.SizeX,
                SizeY = part.SizeY,
                Devices = devices,
                Kx = part.Kx,
                Ky = part.Ky,
                MeasuredPoints = points,
                Polygons = polygons
            };
        }
    }
}
