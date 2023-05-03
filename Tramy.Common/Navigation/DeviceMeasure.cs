using System;
using System.Collections.Generic;
using System.Text;

namespace Tramy.Common.Navigation
{
    /// <summary>
    /// Measure rssi of device
    /// </summary>
    public class DeviceMeasure
    {
        /// <summary>
        /// Mac of device
        /// </summary>
        public string Mac { get; set; }

        /// <summary>
        /// Measured rssi
        /// </summary>
        public int Rssi { get; set; }
    }
}
