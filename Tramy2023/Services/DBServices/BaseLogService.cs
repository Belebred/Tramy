using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Tramy.Common.Common;
using Tramy.Common.Locations;
using Tramy.Common.Logs;

namespace Tramy.Backend.Data.DBServices
{
    /// <summary>
    /// Base class for all logs MongoDB items. All inherited must be added as singleton in Startup
    /// </summary>
    public class BaseLogService
    {
        /// <summary>
        /// Logger of service
        /// </summary>
        protected readonly ILogger<BaseLogService> Logger;

        /// <summary>
        /// Main MongoDB Database
        /// </summary>
        protected readonly IMongoDatabase MainDatabase;

 
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration">Configuration must be link from Startup</param>
        /// <param name="logger">Logger must be link from Startup</param>
        public BaseLogService(IConfiguration configuration, ILogger<BaseLogService> logger)
        {
            //save logger
            Logger = logger;
            //create MongoDB connection
            try
            {
                var mongoClient = new MongoClient(configuration["MongoConnection"]);
                MainDatabase = mongoClient.GetDatabase(configuration["MongoLogsDB"]);
                Logger.LogDebug($"Logs: Created connection");
            }
            catch (Exception e)
            {
                Logger.LogCritical(e,$"Logs: Cant create MongoDB connection");
            }
        }

        


        /// <summary>
        /// Get all Items
        /// </summary>
        public virtual async Task<IEnumerable<T>> Get<T>(int? limit = 10, int? skip = 0) where T:Log
        {
            try
            {
                var collection = MainDatabase.GetCollection<T>(typeof(T).Name);
                return await collection.Find(f => true).Skip(skip).Limit(limit).ToListAsync();

            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{typeof(T).Name}: Cant get all items");
                return new List<T>();
            }
        }

        /// <summary>
        /// Get by Linq filter
        /// </summary>
        public virtual async Task<IEnumerable<T>> Get<T>(Expression<Func<T,bool>> filter) where T :Log
        {
            try
            {
                var collection = MainDatabase.GetCollection<T>(typeof(T).Name);
                return await collection.Find(filter).ToListAsync();

            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{typeof(T).Name}: Cant get items by filter");
                return new List<T>();
            }
        }

        /// <summary>
        /// Get Entity by id or null if not exists
        /// </summary>
        /// <param name="id">Id of Entity</param>
        public virtual async Task<T> GetById<T>(Guid id) where T : Log
        {
            try
            {
                var collection = MainDatabase.GetCollection<T>(typeof(T).Name);
                return await collection.Find(f => f.Id == id).FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                Logger.LogError(e,$"{typeof(T).Name}: Cant get item by id {id}");
                return null;
            }
        }

       



        /// <summary>
        /// Insert new Entity
        /// </summary>
        /// <param name="entity">Entity to insert</param>
        public virtual async Task<IEnumerable<string>> Insert<T>(T entity) where T : Log
        {
            try
            {
                var collection = MainDatabase.GetCollection<T>(typeof(T).Name);
                //create new Id
                entity.Id = Guid.NewGuid();
                //validate
                var errors = await Validate(entity);
                if (errors.Count > 0)
                    return errors;
                //and insert entity
                await collection.InsertOneAsync(entity);
                Logger.LogDebug($"{typeof(T).Name}: Item with id: {entity.Id} inserted");
                return new List<string>();
            }
            catch (Exception e)
            {
                Logger.LogError(e,$"{typeof(T).Name}: Cant insert item with id {entity.Id}");
                return new List<string>(){"Cant insert entity"};
            }
        }

        /// <summary>
        /// Delete Entity
        /// </summary>
        /// <param name="entity">Entity to delete</param>
        public virtual async Task<IEnumerable<string>> Delete<T>(T entity) where T : Log
        {
            //if null 
            if (entity == null)
                return new List<string>() { "Entity not found" };
            //validate
            var errors = await ValidateBeforeDelete(entity);
            if (errors.Count > 0)
                return errors;
            //delete
            return await DeleteById<T>(entity.Id);
        }

        /// <summary>
        /// Delete Entity by id
        /// </summary>
        /// <param name="id">Entity to delete</param>
        public virtual async Task<IEnumerable<string>> DeleteById<T>(Guid id) where T : Log
        {
            try
            {
                var collection = MainDatabase.GetCollection<T>(typeof(T).Name);
                var entity = collection.Find(f => f.Id == id).FirstOrDefault();
                //if null 
                if (entity == null)
                    return new List<string>() {"Entity not found"};
                //validate
                var errors = await ValidateBeforeDelete(entity);
                if (errors.Count > 0)
                    return errors;
                //delete
                await collection.DeleteOneAsync(Builders<T>.Filter.Eq(e => e.Id, id));
                Logger.LogDebug($"{typeof(T).Name}: Item with id: {id} deleted");
                return new List<string>(); 
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{typeof(T).Name}: Cant delete item with id {id}");
                return new List<string>(); 
            }
        }

        /// <summary>
        /// Update all fields of Entity
        /// </summary>
        /// <param name="entity">Entity to full update</param>
        /// <param name="isUpsert">If true, create entity if not exists</param>
        public virtual async Task<IEnumerable<string>> FullUpdate<T>(T entity, bool isUpsert = false) where T : Log
        {
            try
            {
                var collection = MainDatabase.GetCollection<T>(typeof(T).Name);
                //validate
                var errors = await Validate(entity);
                if (errors.Count > 0)
                    return errors;
                //and update if good
                await collection.ReplaceOneAsync(Builders<T>.Filter.Eq(e => e.Id, entity.Id), entity, new ReplaceOptions(){IsUpsert = isUpsert });
                Logger.LogDebug($"{typeof(T).Name}: Item with id: {entity.Id} updated");
                return new List<string>();
            }
            catch (Exception e)
            {
                Logger.LogError(e,$"{typeof(T).Name}: Cant update item with id {entity.Id}");
                return new List<string>() { "Cant update entity" };
            }
        }


        /// <summary>
        /// Validate entity
        /// </summary>
        /// <param name="entity">Entity to validate</param>
        /// <returns>Returns list of error or null if errors is not exists</returns>
        protected virtual async Task<List<string>> Validate<T>(T entity) where T : Log
        {
            return new List<string>();
        }

        /// <summary>
        /// Validate entity before delete
        /// </summary>
        /// <param name="entity">Entity to validate</param>
        /// <returns>Returns list of error or null if errors is not exists</returns>
        protected virtual async Task<List<string>> ValidateBeforeDelete<T>(T entity) where T : Log
        {
            return new List<string>();
        }
    }
}
