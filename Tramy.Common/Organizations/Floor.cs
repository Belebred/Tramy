using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Tramy.Common.Common;

namespace Tramy.Common.Organizations
{
    /// <summary>
    /// Building's floor definition
    /// </summary>
    public class Floor: BaseMongoItem
    {
        /// <summary>
        /// Name of floor
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Number of floor
        /// </summary>
        [Required]
        public int Number { get; set; }

        /// <summary>
        /// Svg image of floor
        /// </summary>
        [Required]
        public byte[] Map { get; set; }

        /// <summary>
        /// Max coordinate of X dimension. 1 unit = 1 meter
        /// </summary>
        [Required]
        public decimal SizeX { get; set; }

        /// <summary>
        /// Max coordinate of Y dimension. 1 unit = 1 meter
        /// </summary>
        [Required]
        public decimal SizeY { get; set; }

        /// <summary>
        /// Floor's locations
        /// </summary>
  
        public IEnumerable<Guid> Locations { get; set; } = new List<Guid>();


    }
}
