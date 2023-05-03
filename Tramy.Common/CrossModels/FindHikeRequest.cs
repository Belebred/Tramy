using System;
using System.Collections.Generic;
using System.Text;
using Tramy.Common.Hike;

namespace Tramy.Common.CrossModels
{
    /// <summary>
    /// Class to find Hikes
    /// </summary>
    public class FindHikeRequest
    {
        /// <summary>
        /// Date when Hike happens
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// Type of the Hike
        /// </summary>
        public HikeType? Type { get; set; }

        /// <summary>
        /// Country where Hike happens
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// State of country
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// New people in the Hike
        /// </summary>
        public bool IsPeopleNeed { get; set; }
    }
}
