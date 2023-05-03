using System;
using System.Collections.Generic;
using System.Text;

namespace Tramy.Common.Navigation
{
    /// <summary>
    /// Status of measured point
    /// </summary>
    public enum MeasuredPointStatus
    {
        /// <summary>
        /// Point has been measured
        /// </summary>
        Measured, 
        
        /// <summary>
        /// Point hasn't been measured
        /// </summary>
        NotMeasured, 
        
        /// <summary>
        /// There is an error in measuring
        /// </summary>
        Error
    }
}
