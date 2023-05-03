using System;
using System.Collections.Generic;
using System.Text;
using Tramy.Common.Common;

namespace Tramy.Common.Navigation
{
    /// <summary>
    /// Measured rssi in point
    /// </summary>
    public class MeasuredPoint:BaseMongoItem
    {
        /// <summary>
        /// Location Ids
        /// </summary>
        public Guid LocationPartId { get; set; }

        /// <summary>
        /// Status of point
        /// </summary>
        public MeasuredPointStatus Status { get; set; } = MeasuredPointStatus.NotMeasured;

        /// <summary>
        /// X coordinate
        /// </summary>
        public decimal X { get; set; }

        /// <summary>
        /// Y coordinate
        /// </summary>
        public decimal Y { get; set; }

        /// <summary>
        /// X coordinate
        /// </summary>
        public decimal RealX { get; set; }

        /// <summary>
        /// Y coordinate
        /// </summary>
        public decimal RealY { get; set; }

        /// <summary>
        /// North view direction measures
        /// </summary>
        public List<DeviceMeasure> NorthMeasures { get; set; } = new List<DeviceMeasure>();
        /// <summary>
        /// South view direction measures
        /// </summary>
        public List<DeviceMeasure> SouthMeasures { get; set; } = new List<DeviceMeasure>();
        /// <summary>
        /// West view direction measures
        /// </summary>
        public List<DeviceMeasure> WestMeasures { get; set; } = new List<DeviceMeasure>();
        /// <summary>
        /// East view direction measures
        /// </summary>
        public List<DeviceMeasure> EastMeasures { get; set; } = new List<DeviceMeasure>();
        /// <summary>
        /// Average view direction measures
        /// </summary>
        public List<DeviceMeasure> AverageMeasures { get; set; } = new List<DeviceMeasure>();

        /// <summary>
        /// Count of measures
        /// </summary>
        public int AverageMeasuresCount => AverageMeasures.Count;
    }
}
