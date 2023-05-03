using System;
using System.Collections.Generic;
using System.Text;
using Tramy.Common.Common;

namespace Tramy.Common.Logs
{
    /// <summary>
    /// System logs
    /// </summary>
    public class Log:BaseMongoItem
    {
        /// <summary>
        /// Date and time of log
        /// </summary>
        public DateTime DateTime { get; set; }

        /// <summary>
        /// User id log
        /// </summary>
        public Guid? UserId { get; set; }

        /// <summary>
        /// Comment if need
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Level
        /// </summary>
        public LogLevel Level { get; set; }
    }
}
