using System;
using System.ComponentModel.DataAnnotations;
using Tramy.Common.Common;

namespace Tramy.Common.Devices
{
    /// <summary>
    /// Device definition
    /// </summary>
    public class Device:BaseMongoItem
    {
        /// <summary>
        /// MAC address of device
        /// </summary>
        [Required]
        public string MAC { get; set; }

        /// <summary>
        /// Link to device series
        /// </summary>
        [Required]
        public Guid Series { get; set; }

        /// <summary>
        /// Date of last service
        /// </summary>
        public DateTime? LastService { get; set; }

        /// <summary>
        /// Date of start exploitation
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Date of end exploitation
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Status of device
        /// </summary>
        [Required]
        public DeviceStatus Status { get; set; }
    }
}
