using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Tramy.Backend.Helpers;
using Tramy.Common.Users;

namespace Tramy.Backend.Data.DBServices
{
    /// <summary>
    /// Role's service for MongoDB
    /// </summary>
    public class RoleService:BaseDbService<Role>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration">Configuration must be link from Startup</param>
        /// <param name="logger">Logger must be link from Startup</param>
        public RoleService(ServiceConfiguration configuration, ILogger<BaseDbService<Role>> logger, BaseLogService logService) : base(configuration, logger, logService)
        {
        }

        /// <summary>
        /// Validate Role for duplication name
        /// </summary>
        /// <param name="entity">New role</param>
        /// <returns>Errors if exists</returns>
        protected override async Task<List<string>> Validate(Role entity)
        {
            return await MainCollection.Find(r => r.Name == entity.Name && r.Id != entity.Id).AnyAsync() ? new List<string>() {"Роль с таким именем уже существует"} : new List<string>();
        }
    }
}
