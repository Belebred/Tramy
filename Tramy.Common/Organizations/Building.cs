using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Tramy.Common.Common;

namespace Tramy.Common.Organizations
{
    /// <summary>
    /// Organization's building definition
    /// </summary>
    public class Building:BaseMongoItem
    {
        /// <summary>
        /// Name of building
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Address of building
        /// </summary>
        [Required]
        public string Address { get; set; }

        /// <summary>
        /// GPS longitude
        /// </summary>
        [Required]
        public decimal GeoLon { get; set; }

        /// <summary>
        /// GPS latitude
        /// </summary>
        [Required]
        public decimal GeoLat { get; set; }

        /// <summary>
        /// Building's floors
        /// </summary>

        public IEnumerable<Guid> Floors { get; set; } = new List<Guid>();
    }
}
