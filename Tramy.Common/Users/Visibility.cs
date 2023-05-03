using System;
using System.Collections.Generic;
using System.Text;
using Tramy.Common.Common;

namespace Tramy.Common.Users
{
    /// <summary>
    /// User' visibility for others
    /// </summary>
    public class Visibility : BaseMongoItem
    {
        /// <summary>
        /// Active visibility or not
        /// </summary>
        public bool Active { get; set; } = true;

        /// <summary>
        /// Permission to change visibility
        /// </summary>
        public bool Perm { get; set; } = true;

        /// <summary>
        /// Visible statuses of user
        /// </summary>
        public VisibleStatus VisibleStatus { get; set; }
    }
}
