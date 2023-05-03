using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Tramy.Common.Devices;

namespace Tramy.Backend.Data.DBServices
{
    /// <summary>
    /// Device's service for MongoDB
    /// </summary>
    public class DeviceService:BaseDbService<Device>
    {
        /// <summary>
        /// Main MongoDB collection of device series type
        /// </summary>
        private readonly IMongoCollection<DeviceSeries> _seriesCollection;
        /// <summary>
        /// Main MongoDB collection of base type
        /// </summary>
        private readonly IMongoCollection<DeviceInLocation> _deviceInLocationCollection;
        /// <summary>
        /// Main MongoDB collection of device
        /// </summary>
        private readonly IMongoCollection<Device> _deviceCollection;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration">Configuration must be link from Startup</param>
        /// <param name="logger">Logger must be link from Startup</param>
        public DeviceService(IConfiguration configuration, ILogger<BaseDbService<Device>> logger, BaseLogService logService) : base(configuration, logger, logService)
        {
            _seriesCollection = MainCollection.Database.GetCollection<DeviceSeries>(nameof(DeviceSeries));
            _deviceInLocationCollection = MainCollection.Database.GetCollection<DeviceInLocation>(nameof(DeviceInLocation));
            _deviceCollection = MainCollection.Database.GetCollection<Device>(nameof(Device));
        }

        /// <summary>
        /// Get all series
        /// </summary>
        public virtual async Task<IEnumerable<DeviceSeries>> GetSeries(int skip, int limit)
        {
            try
            {
                return await _seriesCollection.Find(f => true).Skip(skip).Limit(limit).ToListAsync();

            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(DeviceSeries)}: Cant get all items");
                return new List<DeviceSeries>();
            }
        }

        /// <summary>
        /// Update all fields of Entity
        /// </summary>
        /// <param name="entity">Entity to full update</param>
        /// <param name="isUpsert">If true, create entity if not exists</param>
        public virtual async Task<IEnumerable<string>> FullUpdate(DeviceInLocation entity, bool isUpsert = false)
        {
            try
            {
                //validate
                var errors = await ValidateDeviceInLocation(entity);
                if (errors.Count > 0)
                    return errors;
                //and update if good
                await _deviceInLocationCollection.ReplaceOneAsync(Builders<DeviceInLocation>.Filter.Eq(e => e.Id, entity.Id), entity, new ReplaceOptions() { IsUpsert = isUpsert });
                Logger.LogDebug($"{nameof(DeviceInLocation)}: Item with id: {entity.Id} updated");
                return new List<string>();
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(DeviceInLocation)}: Cant update item with id {entity.Id}");
                return new List<string>() { "Cant update entity" };
            }
        }

        /// <summary>
        /// Get Device list by MAC
        /// </summary>
        /// <param name="MAC"></param>
        /// <param name="limit"></param>
        /// <returns> List of Devices</returns>
        public virtual async Task<IEnumerable<string>> FindMacByPart(string MAC, int limit, int skip)
        {
            List<string> macs = new List<string>();
            try
            {
                var devcollection = await _deviceCollection.Find(f => f.MAC.StartsWith(MAC)).Skip(skip).Limit(limit).ToListAsync();
                foreach (var item in devcollection)
                {
                    macs.Add(item.MAC);
                }
                return macs;
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(Device)}: Can't get item by MAC: {MAC}");
                return null;
            }
        }

        /// <summary>
        /// Get DeviceSeries by id or null if not exists
        /// </summary>
        /// <param name="id">Id of Entity</param>
        public virtual async Task<DeviceSeries> GetSeriesById(Guid id)
        {
            try
            {
                return await _seriesCollection.Find(f => f.Id == id).FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(DeviceSeries)}: Cant get item by id {id}");
                return null;
            }
        }

        /// <summary>
        /// Insert new DeviceSeries
        /// </summary>
        /// <param name="entity">Entity to insert</param>
        public virtual async Task<IEnumerable<string>> InsertSeries(DeviceSeries entity)
        {
            try
            {
                //create new Id
                entity.Id = new Guid();
                //validate
                var errors = await ValidateSeries(entity);
                if (errors.Count > 0)
                    return errors;
                //and insert entity
                await _seriesCollection.InsertOneAsync(entity);
                Logger.LogDebug($"{nameof(DeviceSeries)}: Item with id: {entity.Id} inserted");
                return new List<string>();
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(DeviceSeries)}: Cant insert item with id {entity.Id}");
                return new List<string>() { "Cant insert entity" };
            }
        }

        /// <summary>
        /// Delete DeviceSeries
        /// </summary>
        /// <param name="entity">Entity to delete</param>
        public virtual async Task<IEnumerable<string>> DeleteSeries(DeviceSeries entity)
        {
            //if null 
            if (entity == null)
                return new List<string>() { "Entity not found" };
            //validate
            var errors = await ValidateSeriesBeforeDelete(entity);
            if (errors.Count > 0)
                return errors;
            //delete
            return await DeleteById(entity.Id);
        }

        /// <summary>
        /// Delete DeviceSeries by id
        /// </summary>
        /// <param name="id">Entity to delete</param>
        public virtual async Task<IEnumerable<string>> DeleteSeriesById(Guid id)
        {
            try
            {
                var entity = _seriesCollection.Find(f => f.Id == id).FirstOrDefault();
                //if null 
                if (entity == null)
                    return new List<string>() { "Entity not found" };
                //validate
                var errors = await ValidateSeriesBeforeDelete(entity);
                if (errors.Count > 0)
                    return errors;
                //delete
                await _seriesCollection.DeleteOneAsync(Builders<DeviceSeries>.Filter.Eq(e => e.Id, id));
                Logger.LogDebug($"{nameof(DeviceSeries)}: Item with id: {id} deleted");
                return new List<string>();
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(DeviceSeries)}: Cant delete item with id {id}");
                return new List<string>();
            }
        }

        /// <summary>
        /// Update all fields of DeviceSeries
        /// </summary>
        /// <param name="entity">Entity to full update</param>
        /// <param name="isUpsert">If true, create entity if not exists</param>
        public virtual async Task<IEnumerable<string>> FullUpdateSeries(DeviceSeries entity, bool isUpsert = false)
        {
            try
            {
                //validate
                var errors = await ValidateSeries(entity);
                if (errors.Count > 0)
                    return errors;
                //and update if good
                await _seriesCollection.ReplaceOneAsync(Builders<DeviceSeries>.Filter.Eq(e => e.Id, entity.Id), entity, new ReplaceOptions() { IsUpsert = isUpsert });
                Logger.LogDebug($"{nameof(DeviceSeries)}: Item with id: {entity.Id} updated");
                return new List<string>();
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(DeviceSeries)}: Cant update item with id {entity.Id}");
                return new List<string>() { "Cant update entity" };
            }
        }

        /// <summary>
        /// Get device in location by LocationPart id
        /// </summary>
        public virtual async Task<IEnumerable<DeviceInLocation>> Get(Guid id, int skip, int limit)
        {
            try
            {
                return await _deviceInLocationCollection.Find(d=>d.LocationPartId == id).Skip(skip).Limit(limit).ToListAsync();

            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(DeviceInLocation)}: Cant get items by location id");
                return new List<DeviceInLocation>();
            }
        }

        /// <summary>
        /// Insert new device in location
        /// </summary>
        /// <param name="entity">Device in location to insert</param>
        public virtual async Task<IEnumerable<string>> InsertLocationDevice(DeviceInLocation entity)
        {
            //TODO need to set location in device entity and change status to use.
            try
            {
                //create new Id
                entity.Id = Guid.NewGuid();
                //validate
                var errors = await ValidateDeviceInLocation(entity);
                if (errors.Count > 0)
                    return errors;
                //and insert entity
                await _deviceInLocationCollection.InsertOneAsync(entity);
                Logger.LogDebug($"{nameof(DeviceInLocation)}: Item with id: {entity.Id} inserted");
                return new List<string>();
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(DeviceInLocation)}: Cant insert item with id {entity.Id}");
                return new List<string>() { "Cant insert entity" };
            }
        }

        /// <summary>
        /// Update Device in location
        /// </summary>
        /// <param name="entity">Device in location entity</param>
        /// <param name="isUpsert">If true, create entity if not exists</param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<string>> FullUpdateDeviceInLocation(DeviceInLocation entity, bool isUpsert = false)
        {
            try
            {
                //validate
                var errors = await ValidateDeviceInLocation(entity);
                if (errors.Count > 0)
                    return errors;
                //and update if good
                await _deviceInLocationCollection.ReplaceOneAsync(Builders<DeviceInLocation>.Filter.Eq(e => e.Id, entity.Id), entity, new ReplaceOptions() { IsUpsert = isUpsert });
                Logger.LogDebug($"{nameof(DeviceInLocation)}: Item with id: {entity.Id} updated");
                return new List<string>();
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(DeviceInLocation)}: Cant update item with id {entity.Id}");
                return new List<string>() { "Cant update entity" };
            }
        }

        /// <summary>
        /// Delete device in location
        /// </summary>
        /// <param name="entity">Device in location to delete</param>
        public virtual async Task<IEnumerable<string>> DeleteDeviceInLocation(DeviceInLocation entity)
        {
            //if null 
            if (entity == null)
                return new List<string>() { "Entity not found" };
            return await DeleteDeviceInLocationById(entity.Id);
        }

        /// <summary>
        /// Delete Entity by id
        /// </summary>
        /// <param name="id">Entity to delete</param>
        public virtual async Task<IEnumerable<string>> DeleteDeviceInLocationById(Guid id)
        {
            //TODO need to unset location in device entity and change status to not-use.
            try
            {
                var entity = _deviceInLocationCollection.Find(f => f.Id == id).FirstOrDefault();
                //if null 
                if (entity == null)
                    return new List<string>() { "Entity not found" };
                //validate
                var errors = await ValidateDeviceInLocationBeforeDelete(entity);
                if (errors.Count > 0)
                    return errors;
                //delete
                await _deviceInLocationCollection.DeleteOneAsync(Builders<DeviceInLocation>.Filter.Eq(e => e.Id, id));
                Logger.LogDebug($"{nameof(DeviceInLocation)}: Item with id: {id} deleted");
                return new List<string>();
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(DeviceInLocation)}: Cant delete item with id {id}");
                return new List<string>();
            }
        }

        /// <summary>
        /// Returns devices in location part
        /// </summary>
        /// <param name="id">Location part id</param>
        /// <returns></returns>
        public async Task<List<DeviceInLocation>> GetDeviceInLocationsById(Guid id)
        {
            return await _deviceInLocationCollection.Find(Builders<DeviceInLocation>.Filter.Eq(e => e.LocationPartId, id)).ToListAsync();
        }

        /// <summary>
        /// Validate DeviceSeries before 
        /// </summary>
        /// <param name="entity">Entity to validate</param>
        /// <returns>Returns list of error or null if errors is not exists</returns>
        private async Task<List<string>> ValidateSeries(DeviceSeries entity)
        {
            return new List<string>();
        }

        /// <summary>
        /// Validate Levice in location before 
        /// </summary>
        /// <param name="entity">Entity to validate</param>
        /// <returns>Returns list of error or null if errors is not exists</returns>
        private async Task<List<string>> ValidateDeviceInLocation(DeviceInLocation entity)
        {
            return new List<string>();
        }

        /// <summary>
        /// Validate DeviceSeries before delete
        /// </summary>
        /// <param name="entity">Entity to validate</param>
        /// <returns>Returns list of error or null if errors is not exists</returns>
        protected virtual async Task<List<string>> ValidateSeriesBeforeDelete(DeviceSeries entity)
        {
            return new List<string>();
        }

        /// <summary>
        /// Validate Device in location before delete
        /// </summary>
        /// <param name="entity">Entity to validate</param>
        /// <returns>Returns list of error or null if errors is not exists</returns>
        protected virtual async Task<List<string>> ValidateDeviceInLocationBeforeDelete(DeviceInLocation entity)
        {
            return new List<string>();
        }
    }
}
