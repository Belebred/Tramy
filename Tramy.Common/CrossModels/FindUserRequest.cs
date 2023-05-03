using System;
using System.Collections.Generic;
using System.Text;

namespace Tramy.Common.CrossModels
{
    /// <summary>
    /// Class to find user's request
    /// </summary>
    public class FindUserRequest
    {
        /// <summary>
        /// User's id
        /// </summary>
        public Guid MyId { get; set; }

        /// <summary>
        /// Search string
        /// </summary>
        public string Search { get; set; }
    }
}
