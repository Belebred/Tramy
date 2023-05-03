using System;
using System.ComponentModel.DataAnnotations;


namespace Tramy.Common.Organizations
{
    /// <summary>
    /// Definition of device place 
    /// </summary>
    public class DevicePlace
    {
        /// <summary>
        /// X coordinates of device
        /// </summary>
        [Required]
        public decimal X { get; set; }

        /// <summary>
        /// Y coordinates of device
        /// </summary>
        [Required]
        public decimal Y { get; set; }

        /// <summary>
        /// Link to device
        /// </summary>
        [Required]

        public Guid Device { get; set; }
    }
}
