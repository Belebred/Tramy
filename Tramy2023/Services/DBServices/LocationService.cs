using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Tramy.Common.Locations;
using Tramy.Backend.Helpers;

namespace Tramy.Backend.Data.DBServices
{
    /// <summary>
    /// Location's service for MongoDB
    /// </summary>
    public class LocationService : BaseDbService<Location>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration">Configuration must be link from Startup</param>
        /// <param name="logger">Logger must be link from Startup</param>
        public LocationService(ServiceConfiguration configuration, ILogger<BaseDbService<Location>> logger, BaseLogService logService) : base(configuration, logger, logService)
        {
           
        }

        /// <summary>
        /// Get near locations
        /// </summary>
        /// <param name="lat">Latitude</param>
        /// <param name="lon">Longitude</param>
        /// <param name="nearDistance">Distance to me</param>
        /// <returns></returns>
        public async Task<IEnumerable<NearLocation>> GetNear(double lat, double lon, double nearDistance, int skip, int limit)
        {
            var result = await MainCollection.Aggregate<Location>().AppendStage<NearLocation>(new BsonDocument("$geoNear",
                new BsonDocument
                {
                    {
                        "near",
                        new BsonDocument
                        {
                            {"type", "Point"},
                            {
                                "coordinates",
                                new BsonArray
                                {
                                    lat,
                                    lon
                                }
                            }
                        }
                    },
                    {"distanceField", "Distance"},
                    {"maxDistance", nearDistance}
                })).AppendStage<NearLocation>(new BsonDocument("$project",
                new BsonDocument
                {
                    {"_id", 1},
                    {"Name", 1},
                    {"GeoPoint", 1},
                    {"Distance", 1},
                    {"Address", 1}
                })).ToListAsync();

            return result.Skip(skip).Take(limit).ToList();
        }
    }
}
