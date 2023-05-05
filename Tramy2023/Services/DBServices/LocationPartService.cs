using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tramy.Common.Locations;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Tramy.Backend.Helpers;

namespace Tramy.Backend.Data.DBServices
{
    /// <summary>
    /// Location part service for MongoDB
    /// </summary>
    public class LocationPartService : BaseDbService<LocationPart>
    {
        /// <summary>
        /// Location service
        /// </summary>
        private LocationService _locationService;

        /// <summary>
        /// Collections of polygons
        /// </summary>
        private IMongoCollection<Polygon> _polygonCollection;

        public LocationPartService(ServiceConfiguration configuration, ILogger<BaseDbService<LocationPart>> logger, LocationService locationService, BaseLogService logService) : base(configuration, logger, logService)
        {
            _locationService = locationService;
            _polygonCollection = MainDatabase.GetCollection<Polygon>(nameof(Polygon));
        }

        /// <summary>
        /// Creates new location part in location
        /// </summary>
        /// <param name="locationId">Location Id</param>
        /// <returns>Location part and error list</returns>
        public async Task<(LocationPart, IEnumerable<string>)> CreateNewLocationPart(Guid locationId)
        {
            var locationPart = new LocationPart()
            {
                Name = "New location part"
            };

            //check location
            var location = await _locationService.GetById(locationId);
            if (location == null)
            {
                Logger.LogError($"Cant find location by id {locationId}");
                return (null, new List<string>() { $"Cant find location by id {locationId}" });
            }
            //try insert
            var errors = await Insert(locationPart);
            if (errors.Count() > 0)
                return (null, errors);
            //add to location            
            location.Parts.Add(locationPart.Id);
            await _locationService.FullUpdate(location);

            return (locationPart, new List<string>());
        }

        /// <summary>
        /// Get all locations part of location
        /// </summary>
        /// <param name="locationId">Location Id</param>
        /// <returns>List of locations part</returns>
        public async Task<IEnumerable<LocationPart>> GetByLocation(Guid locationId, int skip=0, int limit=10)
        {
            var result = new List<LocationPart>();
            var location = await _locationService.GetById(locationId);
            if (location == null)
            {
                Logger.LogError($"Cant find location by id {locationId}");
                return result.Skip(skip).Take(limit).ToList();
            }
            result =  await MainCollection.Find(lp => location.Parts.Contains(lp.Id)).ToListAsync();
            return result.Skip(skip).Take(limit).ToList();
        }

        /// <summary>
        /// Insert new polygon
        /// </summary>
        /// <param name="entity">Entity to insert</param>
        public virtual async Task<IEnumerable<string>> InsertPolygon(Polygon entity)
        {
            try
            {
                //validate
                var errors = await ValidatePolygon(entity);
                if (errors.Count > 0)
                    return errors;
                //and insert entity
                await _polygonCollection.InsertOneAsync(entity);
                Logger.LogDebug($"{typeof(Polygon).Name}: Item with id: {entity.Id} inserted");
                return new List<string>();
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{typeof(Polygon).Name}: Cant insert item with id {entity.Id}");
                return new List<string>() { "Cant insert entity" };
            }
        }


        /// <summary>
        /// Replace polygons 
        /// </summary>
        /// <param name="id">Location part Id</param>
        /// <param name="polygons">Polygons to replace</param>
        public virtual async Task<IEnumerable<string>> ReplacePolygons(Guid id, List<Polygon> polygons)
        {
            try
            {
                var part = MainCollection.Find(f => f.Id == id).FirstOrDefault();
                if (part != null)
                {
                    await _polygonCollection.DeleteManyAsync(Builders<Polygon>.Filter.In(p => p.Id, part.Polygons));
                    part.Polygons = polygons.Select(p => p.Id).ToList();
                    await FullUpdate(part);
                    await _polygonCollection.InsertManyAsync(polygons);

                }
             
                return new List<string>();
            }
            catch (Exception e)
            {
                 return new List<string>() { "Cant insert entity" };
            }
        }

        /// <summary>
        /// Update polygon
        /// </summary>
        /// <param name="entity">Entity to update</param>
        public virtual async Task<IEnumerable<string>> UpdatePolygon(Polygon entity)
        {
            try
            {
                //validate
                var errors = await ValidatePolygon(entity);
                if (errors.Count > 0)
                    return errors;
                //and insert entity
                await _polygonCollection.ReplaceOneAsync(p=>p.Id == entity.Id, entity);
                Logger.LogDebug($"{typeof(Polygon).Name}: Item with id: {entity.Id} updated");
                return new List<string>();
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{typeof(Polygon).Name}: Cant update item with id {entity.Id}");
                return new List<string>() { "Cant update entity" };
            }
        }

        /// <summary>
        /// Delete Polygon by id
        /// </summary>
        /// <param name="id">Entity to delete</param>
        public virtual async Task<IEnumerable<string>> DeletePolygonById(Guid id)
        {
            try
            {
                var entity = _polygonCollection.Find(f => f.Id == id).FirstOrDefault();
                //if null 
                if (entity == null)
                    return new List<string>() { "Entity not found" };
                //delete
                await _polygonCollection.DeleteOneAsync(Builders<Polygon>.Filter.Eq(e => e.Id, id));
                Logger.LogDebug($"{typeof(Polygon).Name}: Item with id: {id} deleted");
                return new List<string>();
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{typeof(Polygon).Name}: Cant delete item with id {id}");
                return new List<string>();
            }
        }

        /// <summary>
        /// Get polygons by array ids
        /// </summary>
        /// <param name="ids">Array of ids</param>
        public virtual async Task<IEnumerable<Polygon>> GetPolygons(IEnumerable<Guid> ids, int skip=0, int limit=10)
        {
            try
            {
                return await _polygonCollection.Find(f => ids.Contains(f.Id)).Skip(skip).Limit(limit).ToListAsync();
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{typeof(Polygon).Name}: Cant get items by ids {string.Join(',',ids)}");
                return null;
            }
        }

        /// <summary>
        /// Validate polygon
        /// </summary>
        /// <param name="entity">Polygon to validate</param>
        /// <returns>Returns list of error or null if errors is not exists</returns>
        protected virtual async Task<List<string>> ValidatePolygon(Polygon entity)
        {
            return new List<string>();
        }
    }

}
