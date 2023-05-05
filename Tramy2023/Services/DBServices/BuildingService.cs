using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using Tramy.Backend.Helpers;
using Tramy.Common.Organizations;

namespace Tramy.Backend.Data.DBServices
{
    /// <summary>
    /// Location's service for MongoDB
    /// </summary>
    public class BuildingService : BaseDbService<Building>
    {
        /// <summary>
        /// Main MongoDB collection of base type
        /// </summary>
        private IMongoCollection<Building> _buildingCollection;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration">Configuration must be link from Startup</param>
        /// <param name="logger">Logger must be link from Startup</param>
        public BuildingService(ServiceConfiguration configuration, ILogger<BaseDbService<Building>> logger, BaseLogService logService) : base(configuration, logger, logService)
        {
            _buildingCollection = MainCollection.Database.GetCollection<Building>(nameof(Building));
        }


    }
}