using System;
using System.Collections.Generic;
using System.Text;
using Tramy.Common.Common;

namespace Tramy.Common.Hike
{
    /// <summary>
    /// Travel Kit for the Hike
    /// </summary>
    public class TravelKit: BaseMongoItem
    {
        /// <summary>
        /// Name of Travel Kit
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Person who is responsible for all kit
        /// </summary>
        public Guid Person { get; set; }

        /// <summary>
        /// Id of the Hike
        /// </summary>
        public Guid HikeId { get; set; }

        /// <summary>
        /// Items in a TravelKit
        /// </summary>
        public List<Guid> Items { get; set; }
    }
}
