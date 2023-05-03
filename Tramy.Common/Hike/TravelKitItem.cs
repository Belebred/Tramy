using System;
using Tramy.Common.Common;

namespace Tramy.Common.Hike
{
    /// <summary>
    /// Item in TravelKit
    /// </summary>
    public class TravelKitItem: BaseMongoItem
    {
        /// <summary>
        /// Name of Item
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Count of items
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Person responsile for item
        /// </summary>
        public Guid Performer { get; set; }

        /// <summary>
        /// Is item packed
        /// </summary>
        public bool Complete { get; set; }

        /// <summary>
        /// Is item individual
        /// </summary>
        public bool Individual { get; set; }
    }
}