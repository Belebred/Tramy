using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tramy.Common.Organizations;

namespace Tramy.Backend.Data.DBServices
{
    /// <summary>
    /// Floor's service for MongoDB
    /// </summary>
    public class FloorService : BaseDbService<Floor>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration">Configuration must be link from Startup</param>
        /// <param name="logger">Logger must be link from Startup</param>
        public FloorService(IConfiguration configuration, ILogger<BaseDbService<Floor>> logger, BaseLogService logService) : base(configuration, logger, logService)
        {

        }
    }
}
