using System;
using Tramy.Common.Common;

namespace Tramy.Common.System
{
    /// <summary>
    /// System event for DB log
    /// </summary>
    public class Event:BaseMongoItem
    {
        /// <summary>
        /// Event's comments
        /// </summary>
        public string Comments { get; set; }

        /// <summary>
        /// Type of MondoDB entity. If applicable
        /// </summary>
        public string DbTypeName { get; set; }

        /// <summary>
        /// Id of MongoDB entity/ If applicable
        /// </summary>
        public Guid? EntityId { get; set; }

        /// <summary>
        /// Event's log level for filtering and notification
        /// </summary>
        public EventLogLevel LogLevel { get; set; }

        /// <summary>
        /// Event type for filtering and notification
        /// </summary>
        public EventType EventType { get; set; }
    }
}
