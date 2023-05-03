using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Tramy.Common.Locations;
using Tramy.Common.Navigation;

namespace Tramy.Backend.Data.DBServices
{
    /// <summary>
    /// Service to work with measured points
    /// </summary>
    public class MeasuredPointService : BaseDbService<MeasuredPoint>
    {
        public MeasuredPointService(IConfiguration configuration, ILogger<BaseDbService<MeasuredPoint>> logger, BaseLogService logService) : base(configuration, logger, logService)
        {
        }

        /// <summary>
        /// Get all measured points part of location
        /// </summary>
        /// <param name="locationId">Location Id</param>
        /// <returns>List of locations part</returns>
        public async Task<IEnumerable<MeasuredPoint>> GetByLocationPart(Guid locationId, int skip=0, int limit=10)
        {
            return await MainCollection.Find(p => p.LocationPartId == locationId).Skip(skip).Limit(limit).ToListAsync();
        }

        /// <summary>
        /// remove all measured points from location part
        /// </summary>
        /// <param name="locationId">Location part's id</param>
        /// <returns></returns>
        public async Task RemoveAllLocationPartPoints(Guid locationId)
        {
            await MainCollection.DeleteManyAsync(Builders<MeasuredPoint>.Filter.Eq(p => p.LocationPartId, locationId));
        }
        /// <summary>
        /// Insert location part's measured points
        /// </summary>
        /// <param name="points">Points</param>
        /// <returns></returns>
        public async Task InsertLocationPartPoints(List<MeasuredPoint> points)
        {
            await MainCollection.InsertManyAsync(points);
        }
    }
}
