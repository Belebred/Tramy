using System;
using System.ComponentModel.DataAnnotations;
using Tramy.Common.Common;

namespace Tramy.Common.Devices
{
    /// <summary>
    /// Definition of series of devices
    /// </summary>
    public class DeviceSeries:BaseMongoItem
    {
        /// <summary>
        /// Name of series
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Unique number of series
        /// </summary>
        [Required]
        public string Number { get; set; }

        /// <summary>
        /// Issuer name
        /// </summary>
        [Required]
        public string Issuer { get; set; }

        /// <summary>
        /// Date of issue
        /// </summary>
        [Required]
        public DateTime? IssueDate { get; set; }


        //[Required]
        //public TimeSpan ServicePeriod { get; set; }

        /// <summary>
        ///  Date when the last service was carried out
        /// </summary>
        public DateTime? LastService { get; set; }

        /// <summary>
        /// Comments
        /// </summary>
        public string Comments { get; set; }

        /// <summary>
        /// Distance from the device to the point at which the power of the P0 signal of the device was measured in meters
        /// </summary>
        [Required]
        public decimal D0 { get; set; }

        /// <summary>
        /// Device signal strength measured at unit distance in dBW 
        /// </summary>
        [Required]
        public decimal P0 { get; set; }
    }
}
