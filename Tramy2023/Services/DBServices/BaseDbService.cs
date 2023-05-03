using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Tramy.Common.Common;
using Tramy.Common.Locations;
using Tramy.Common.Logs;
using LogLevel = Tramy.Common.Logs.LogLevel;

namespace Tramy.Backend.Data.DBServices
{
    /// <summary>
    /// Base class for all MongoDB items. All inherited must be added as singleton in Startup
    /// </summary>
    public class BaseDbService<T> where T:BaseMongoItem
    {
        /// <summary>
        /// Logger of service
        /// </summary>
        protected readonly ILogger<BaseDbService<T>> Logger;

        /// <summary>
        /// Main MongoDB Database
        /// </summary>
        protected readonly IMongoDatabase MainDatabase;

        /// <summary>
        /// Main MongoDB collection of base type
        /// </summary>
        protected readonly IMongoCollection<T> MainCollection;

        /// <summary>
        /// Log events to db
        /// </summary>
        protected readonly BaseLogService LogService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration">Configuration must be link from Startup</param>
        /// <param name="logger">Logger must be link from Startup</param>
        public BaseDbService(IConfiguration configuration, ILogger<BaseDbService<T>> logger, BaseLogService logService)
        {
            //save logger
            Logger = logger;
            LogService = logService;
            //create MongoDB connection
            try
            {
                var mongoClient = new MongoClient(configuration["MongoConnection"]);
                MainDatabase = mongoClient.GetDatabase(configuration["MongoMainDB"]);
                MainCollection = MainDatabase.GetCollection<T>(typeof(T).Name);
                Logger.LogDebug($"{typeof(T).Name}: Created connection");
            }
            catch (Exception e)
            {
                Logger.LogCritical(e,$"{typeof(T).Name}: Cant create MongoDB connection");
            }
        }

        


        /// <summary>
        /// Get all Items
        /// </summary>
        public virtual async Task<IEnumerable<T>> Get(int? limit = 10, int? skip = 0)
        {
            try
            {
                return await MainCollection.Find(f => true).Skip(skip).Limit(limit).ToListAsync();

            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{typeof(T).Name}: Cant get all items");
                return new List<T>();
            }
        }

        /// <summary>
        /// Get User's id from token
        /// </summary>
        /// <param name="controller">Controller</param>
        /// <returns></returns>
        public virtual async Task<Guid> GetUserId(Controller controller)
        {
            var userId = new Guid(controller.User.FindFirst("Id")?.Value);
            return userId;
        }

        /// <summary>
        /// Get by Linq filter
        /// </summary>
        public virtual async Task<IEnumerable<T>> Get(Expression<Func<T,bool>> filter)
        {
            try
            {
                return await MainCollection.Find(filter).ToListAsync();

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
        public virtual async Task<T> GetById(Guid id)
        {
            try
            {
                return await MainCollection.Find(f => f.Id == id).FirstOrDefaultAsync();
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
        public virtual async Task<IEnumerable<string>> Insert(T entity)
        {
            try
            {
                //create new Id
                entity.Id = Guid.NewGuid();
                //validate
                var errors = await Validate(entity);
                if (errors.Count > 0)
                    return errors;
                //and insert entity
                await MainCollection.InsertOneAsync(entity);
                Logger.LogDebug($"{typeof(T).Name}: Item with id: {entity.Id} inserted");
                await LogService.Insert(new ItemLog()
                {
                    DateTime = DateTime.Now,
                    Level = LogLevel.Info,
                    Action = ItemAction.Create,
                    ItemId = entity.Id,
                    ItemType = typeof(T).Name,
                });
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
        public virtual async Task<IEnumerable<string>> Delete(T entity)
        {
            //if null 
            if (entity == null)
                return new List<string>() { "Entity not found" };
            //validate
            var errors = await ValidateBeforeDelete(entity);
            if (errors.Count > 0)
                return errors;
            //delete
            return await DeleteById(entity.Id);
        }

        /// <summary>
        /// Delete Entity by id
        /// </summary>
        /// <param name="id">Entity to delete</param>
        public virtual async Task<IEnumerable<string>> DeleteById(Guid id)
        {
            try
            {
                var entity = MainCollection.Find(f => f.Id == id).FirstOrDefault();
                //if null 
                if (entity == null)
                    return new List<string>() {"Entity not found"};
                //validate
                var errors = await ValidateBeforeDelete(entity);
                if (errors.Count > 0)
                    return errors;
                //delete
                await MainCollection.DeleteOneAsync(Builders<T>.Filter.Eq(e => e.Id, id));
                Logger.LogDebug($"{typeof(T).Name}: Item with id: {id} deleted");
                await LogService.Insert(new ItemLog()
                {
                    DateTime = DateTime.Now,
                    Level = LogLevel.Info,
                    Action = ItemAction.Delete,
                    ItemId = entity.Id,
                    ItemType = typeof(T).Name,
                });
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
        public virtual async Task<IEnumerable<string>> FullUpdate(T entity, bool isUpsert = false)
        {
            try
            { 
                //validate
                var errors = await Validate(entity);
                if (errors.Count > 0)
                    return errors;
                //and update if good
                await MainCollection.ReplaceOneAsync(Builders<T>.Filter.Eq(e => e.Id, entity.Id), entity, new ReplaceOptions(){IsUpsert = isUpsert });
                Logger.LogDebug($"{typeof(T).Name}: Item with id: {entity.Id} updated");
                await LogService.Insert(new ItemLog()
                {
                    DateTime = DateTime.Now,
                    Level = LogLevel.Info,
                    Action = ItemAction.Update,
                    ItemId = entity.Id,
                    ItemType = typeof(T).Name,
                });
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
        protected virtual async Task<List<string>> Validate(T entity)
        {
            return new List<string>();
        }

        /// <summary>
        /// Validate entity before delete
        /// </summary>
        /// <param name="entity">Entity to validate</param>
        /// <returns>Returns list of error or null if errors is not exists</returns>
        protected virtual async Task<List<string>> ValidateBeforeDelete(T entity)
        {
            return new List<string>();
        }
    }
}
