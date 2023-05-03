using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Tramy.Common.Common;
using Tramy.Common.System;
using Tramy.Common.Users;

namespace Tramy.Backend.Data.DBServices
{
    /// <summary>
    /// System event's service for MongoDB
    /// </summary>
    public class SystemEventService:BaseDbService<Event>
    { 
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration">Configuration must be link from Startup</param>
        /// <param name="logger">Logger must be link from Startup</param>
        public SystemEventService(IConfiguration configuration, ILogger<BaseDbService<Event>> logger, BaseLogService logService) : base(configuration, logger, logService)
        {
        }

        /// <summary>
        /// Create and insert system event
        /// </summary>
        /// <param name="id">MongoDB entity id</param>
        /// <param name="eventType">MongoDB entity type</param>
        /// <param name="act">Type of act</param>
        /// <param name="ipAddress">Ip address if applicable</param>
        private async Task InsertDbEntitySystemEventBase(Guid id, Type eventType, string act, string ipAddress = null)
        {
            var systemEvent = new Event()
            {
                Comments = $"Act={act};Ip={ipAddress}",
                DbTypeName = eventType.Name,
                EntityId = id,
                EventType = EventType.Data,
                LogLevel = EventLogLevel.Info
            };
            await MainCollection.InsertOneAsync(systemEvent);
        }

        /// <summary>
        /// Create and insert system event
        /// </summary>
        /// <param name="entity">MongoDB entity</param>
        /// <param name="act">Type of act</param>
        /// <param name="ipAddress">Ip address if applicable</param>
        private async Task InsertDbEntitySystemEvent(BaseMongoItem entity, string act, string ipAddress = null)
        {
            //if entity is null logging about and return
            if (entity == null)
            {
                Logger.LogWarning($"Cant create entity system log. Act: {act}, Ip: {ipAddress}");
                return;
            }
            //else create system log
            await InsertDbEntitySystemEventBase(entity.Id, entity.GetType(), act, ipAddress);
        }

        /// <summary>
        /// Create system event if entity change
        /// </summary>
        /// <param name="entity">MongoDB entity</param>
        /// <param name="ipAddress">Ip address if applicable</param>
        public async void LogChangeEntity(BaseMongoItem entity, string ipAddress = null)
        {
            await InsertDbEntitySystemEvent(entity, "Change", ipAddress);
        }

        /// <summary>
        /// Create system event if entity insert
        /// </summary>
        /// <param name="entity">MongoDB entity</param>
        /// <param name="ipAddress">Ip address if applicable</param>
        public async void LogInsertEntity(BaseMongoItem entity, string ipAddress = null)
        {
            await InsertDbEntitySystemEvent(entity, "Insert", ipAddress);
        }

        /// <summary>
        /// Create system event if entity delete
        /// </summary>
        /// <param name="entity">MongoDB entity</param>
        /// <param name="ipAddress">Ip address if applicable</param>
        public async void LogDeleteEntity(BaseMongoItem entity, string ipAddress = null)
        {
            await InsertDbEntitySystemEvent(entity, "Delete", ipAddress);
        }

        /// <summary>
        /// Create system event if entity delete
        /// </summary>
        /// <param name="id">MongoDB entity id</param>
        /// <param name="eventType">MongoDB entity type</param>
        /// <param name="ipAddress">Ip address if applicable</param>
        public async void LogDeleteEntity(Guid? id, Type eventType, string ipAddress = null)
        {
            //if entity is null logging about and return
            if (id == null)
            {
                Logger.LogWarning($"Cant create entity system log. Act: Delete, Ip: {ipAddress}");
                return;
            }
            //else create system log
            await InsertDbEntitySystemEventBase(id.Value, eventType, "Delete", ipAddress);
        }

        /// <summary>
        /// Create system event if user login failed
        /// </summary>
        /// <param name="id">User's id if applicable</param>
        /// <param name="login">User's login (email)</param>
        /// <param name="password">User's password</param>
        /// <param name="ipAddress">Ip address if applicable</param>
        public async void LogUserLoginFailed(Guid? id, string login, string password, string ipAddress = null)
        {
            var systemEvent = new Event()
            {
                Comments = $"Act=Login;Ip={ipAddress};Login={login}",
                DbTypeName = nameof(User),
                EntityId = id,
                EventType = EventType.User,
                LogLevel = EventLogLevel.Warning
            };
            await MainCollection.InsertOneAsync(systemEvent);
        }

        /// <summary>
        /// Create system event if user login success
        /// </summary>
        /// <param name="id">User's id if applicable</param>
        /// <param name="ipAddress">Ip address if applicable</param>
        public async void LogUserLoginSuccess(Guid? id, string ipAddress = null)
        {
            var systemEvent = new Event()
            {
                Comments = $"Act=Login;Ip={ipAddress};",
                DbTypeName = nameof(User),
                EntityId = id,
                EventType = EventType.User,
                LogLevel = EventLogLevel.Info
            };
            await MainCollection.InsertOneAsync(systemEvent);
        }
    }
}
