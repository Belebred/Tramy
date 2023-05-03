using System;
using System.Collections.Generic;
using System.Text;

namespace Tramy.Common.Logs
{
    /// <summary>
    /// Logs of http request
    /// </summary>
    public class RequestLog:Log
    {
        /// <summary>
        /// Request method
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// Request endpoint
        /// </summary>
        public string Endpoint { get; set; }

        /// <summary>
        /// Request ip
        /// </summary>
        public string Ip { get; set; }
        /// <summary>
        /// Content type
        /// </summary>
        public string ContentType { get; set; }
    }
}
