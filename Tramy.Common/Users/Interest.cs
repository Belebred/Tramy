using System;
using System.Collections.Generic;
using System.Text;
using Tramy.Common.Common;

namespace Tramy.Common.Users
{
    /// <summary>
    /// Class for user's interest in Tramy
    /// </summary>
    public class Interest:BaseMongoItem
    {
        /// <summary>
        /// Name of the interest
        /// </summary>
        public string Name { get; set; }
    }
}
