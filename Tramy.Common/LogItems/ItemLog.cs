using System;
using System.Collections.Generic;
using System.Text;

namespace Tramy.Common.Logs
{
    /// <summary>
    /// Log of item actions
    /// </summary>
    public class ItemLog:Log
    {
        /// <summary>
        /// Type of item
        /// </summary>
        public string ItemType { get; set; }

        /// <summary>
        /// Item's id
        /// </summary>
        public Guid ItemId { get; set; }

        /// <summary>
        /// Action
        /// </summary>
        public ItemAction Action { get; set; }
    }
}
