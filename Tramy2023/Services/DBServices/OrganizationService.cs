using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Tramy.Backend.Helpers;
using Tramy.Common.Organizations;

namespace Tramy.Backend.Data.DBServices

    //Добавить работу со зданиями, этажами и стенами
{
    /// <summary>
    /// Organization's service for MongoDB
    /// </summary>
    public class OrganizationService:BaseDbService<Organization>
    {
        ///<summary>
        ///Main MongoDB collection of base type
        ///</summary>
        private readonly IMongoCollection<Organization> _organizationCollection;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration">Configuration must be link from Startup</param>
        /// <param name="logger">Logger must be link from Startup</param>
        public OrganizationService(ServiceConfiguration configuration, ILogger<BaseDbService<Organization>> logger, BaseLogService logService) : base(configuration, logger, logService)
        {
            _organizationCollection = MainCollection.Database.GetCollection<Organization>(nameof(Organization));
        }

        /// <summary>
        /// Get all Organizations
        /// </summary>
        public virtual async Task<IEnumerable<Organization>> GetSeries()
        {
            try
            {
                return await _organizationCollection.Find(f => true).ToListAsync();
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(Organization)}: Cant get all items");
                return new List<Organization>();
            }
        }

        /// <summary>
        /// Get Organiztion by id or null if not exists
        /// </summary>
        /// <param name="id">Id or Entity</param>
        public virtual async Task<Organization> GetOrganizationById(Guid id)
        {
            try
            {
                return await _organizationCollection.Find(f => f.Id == id).FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(Organization)}: Cant get item by id {id}");
                return null;
            }
        }

        /// <summary>
        /// Get Organization by email or null if not exists
        /// </summary>
        /// <param name="email">Email of Organization</param>
        public virtual async Task<Organization> GetByEmail(string email)
        {
            try
            {
                return await MainCollection.Find(f => f.Email == email).FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(Organization)}: Cant get item by email {email}");
                return null;
            }
        }

        /// <summary>
        /// Get organisation by name or null if not exists
        /// </summary>
        /// <param name="name">Name of Organization</param>
        public virtual async Task<Organization> GetByName(string name)
        {
            try
            {
                return await MainCollection.Find(f => f.Name == name).FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(Organization)}: Cant get item by name {name}");
                return null;
            }
        }

        /// <summary>
        /// Get organisation by name or null if not exists
        /// </summary>
        /// <param name="name">Name of Organization</param>
        public virtual async Task<Organization> GetByFullname(string name)
        {
            try
            {
                return await MainCollection.Find(f => f.FullName == name).FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(Organization)}: Cant get item by Fullname {name}");
                return null;
            }
        }

        /// <summary>
        /// Delete Organization
        /// </summary>
        /// <param name="entity">Entity to delete</param>
        public virtual async Task<IEnumerable<string>> DeleteOrganization(Organization entity)
        {
            //if null
            if (entity == null)
                return new List<string>() { "Entity not found" };
            //validate
            var errors = await ValidateOrganizationBeforeDelete(entity);
            if (errors.Count > 0)
                return errors;
            //delete
            return await DeleteById(entity.Id);
        }

       /// <summary>
       /// Delete Organization by id
       /// </summary>
       /// <param name="id">Entity to delete</param>
       public virtual async Task<IEnumerable<string>> DeleteOrganizationById(Guid id)
        {
            try
            {
                var entity = _organizationCollection.Find(f => f.Id == id).FirstOrDefault();
                //if null
                if (entity == null)
                    return new List<string>() { "Entity not found" };
                //validate
                var errors = await ValidateOrganizationBeforeDelete(entity);
                if (errors.Count > 0)
                    return errors;
                //delete
                await _organizationCollection.DeleteOneAsync(Builders<Organization>.Filter.Eq(e => e.Id, id));
                Logger.LogDebug($"{nameof(Organization)}: Item with {id} deleted");
                return new List<string>();
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(Organization)}: Cant be delete item with id {id}");
                return new List<string>();
            }
        }
        
        /// <summary>
        /// Update all fields of Organization
        /// </summary>
        /// <param name="entity">Entity to full update</param>
        /// <param name="isUpsert">Id true, create entity if not exists</param>
        public virtual async Task<IEnumerable<string>> FullUpdateOrganization(Organization entity, bool isUpsert = false)
        {
            try
            {
                //validate
                var errors = await ValidateOrganization(entity);
                if (errors.Count > 0)
                    return errors;
                //and update if good
                await _organizationCollection.ReplaceOneAsync(Builders<Organization>.Filter.Eq(e => e.Id, entity.Id), entity, new ReplaceOptions() { IsUpsert = isUpsert });
                Logger.LogDebug($"{nameof(Organization)}: Item with id: {entity.Id} updated");
                return new List<string>();
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(Organization)}: Cant update item with id {entity.Id}");
                return new List<string>() { "Cant update entity" };
            }
        }

        /// <summary>
        /// Validate Organization before 
        /// </summary>
        /// <param name="entity">Entity to validate</param>
        /// <returns>Returns list of error or null if errors is not exists</returns>
        private async Task<List<string>> ValidateOrganization(Organization entity)
        {
            return new List<string>();
        }

        /// <summary>
        /// Validate Organization before delete
        /// </summary>
        /// <param name="entity">Entity to validate</param>
        /// <returns>Returns list of error or null if errors is not exists</returns>
        protected virtual async Task<List<string>> ValidateOrganizationBeforeDelete(Organization entity)
        {
            return new List<string>();
        }
    }
}
