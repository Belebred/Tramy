using System;
using System.Collections.Generic;
using System.Text;
using Tramy.Common.Common;

namespace Tramy.Common.Hike
{
    /// <summary>
    /// Comment to Hike
    /// </summary>
    public class HikeComment: BaseMongoItem
    {
        /// <summary>
        /// Id of Hike
        /// </summary>
        public Guid HikeId { get; set; }

        /// <summary>
        /// Comment's text
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Comment's author
        /// </summary>
        public Guid Author { get; set; }

        /// <summary>
        /// DateTime of Comment
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Comment's rating
        /// </summary>
        public decimal Rate { get; set; }

        /// <summary>
        /// HikeEvent if happens
        /// </summary>
        public Guid? HikeEventId { get; set; }
    }
}
