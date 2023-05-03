using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tramy.Common.CrossModels;
using Tramy.Common.Hike;
using Tramy.Common.Users;

namespace Tramy.Backend.Data.DBServices
{
    /// <summary>
    /// Service to manage Hike
    /// </summary>
    public class HikeService: BaseDbService<Hike>
    {
        /// <summary>
        /// Main MongoDB collection of base type
        /// </summary>
        private readonly IMongoCollection<Hike> _hikeCollection;
        private readonly IMongoCollection<User> _userCollection;
        private readonly IMongoCollection<UserHikeNeed> _userHikeNeedCollection;
        private readonly IMongoCollection<TravelKit> _travelKitCollection;
        private readonly IMongoCollection<TravelKitItem> _travelKitItemCollection;

        private readonly UserService _userService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration">Configuration must be link from Startup</param>
        /// <param name="logger">Logger must be link from Startup</param>
        public HikeService(UserService userService, IConfiguration configuration, ILogger<BaseDbService<Hike>> logger, BaseLogService logService) : base(configuration, logger, logService)
        {
            _hikeCollection = MainCollection.Database.GetCollection<Hike>(nameof(Hike));
            _userCollection = MainCollection.Database.GetCollection<User>(nameof(User));
            _userHikeNeedCollection = MainDatabase.GetCollection<UserHikeNeed>(nameof(UserHikeNeed));
            _travelKitCollection = MainDatabase.GetCollection<TravelKit>(nameof(TravelKit));
            _travelKitItemCollection = MainDatabase.GetCollection<TravelKitItem>(nameof(TravelKitItem));
            _userService = userService;
        }

        /// <summary>
        /// Send Invite to Hike to the User
        /// </summary>
        /// <param name="userId">Id of User</param>
        /// <param name="hikeId">Id of Hike</param>
        /// <returns></returns>
        public virtual async Task SendInvite(Guid userId, Guid hikeId)
        {
            try
            {
                var hike = await MainCollection.Find(u => u.Id == hikeId).FirstOrDefaultAsync();
                var user = await _userCollection.Find(u => u.Id == userId).FirstOrDefaultAsync();
                if (hike != null && user != null)
                {
                    if (!hike.UserInvites.Contains(userId))
                        hike.UserInvites.Add(userId);
                    if (!user.HikeInvites.Contains(hikeId))
                        user.HikeInvites.Add(hikeId);

                    await FullUpdate(hike);
                    await _userService.FullUpdate(user);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(Hike)}: Can't send ivite to Hike");
            }
        }
        
        /// <summary>
        /// Remove invite from Hike to User
        /// </summary>
        /// <param name="userId">Id of User</param>
        /// <param name="hikeId">Id of Hike</param>
        /// <returns></returns>
        public virtual async Task RemoveInvite(Guid userId, Guid hikeId)
        {
            try
            {
                var hike = await MainCollection.Find(u => u.Id == hikeId).FirstOrDefaultAsync();
                var user = await _userCollection.Find(u => u.Id == userId).FirstOrDefaultAsync();
                if (hike != null && user != null)
                {
                    if (hike.UserInvites.Contains(userId))
                        hike.UserInvites.Remove(userId);
                    if (user.HikeInvites.Contains(hikeId))
                        user.HikeInvites.Remove(hikeId);

                    await FullUpdate(hike);
                    await _userService.FullUpdate(user);
                }

            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(Hike)}: Can't remove ivite to Hike");
            }
        }

        /// <summary>
        /// Accept user request
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="hikeId">Hike id</param>
        /// <returns></returns>
        public virtual async Task AcceptRequest(Guid userId, Guid hikeId)
        {
            try
            {
                var hike = await MainCollection.Find(u => u.Id == hikeId).FirstOrDefaultAsync();
                var user = await _userCollection.Find(u => u.Id == userId).FirstOrDefaultAsync();
                if (hike != null && user != null)
                {
                    if (hike.UserRequests.Contains(userId))
                    {
                        hike.Group.Add(userId);
                        hike.UserRequests.Remove(userId);
                    }
                    if (user.HikeRequests.Contains(hikeId))
                    {
                        user.Hikes.Add(hikeId);
                        user.HikeRequests.Remove(hikeId);
                    }
                    await FullUpdate(hike);
                    await _userService.FullUpdate(user);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(Hike)}: Can't accept request to Hike");
            }
        }

        /// <summary>
        /// Decline user request 
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="hikeId">Hike Id</param>
        /// <returns></returns>
        public virtual async Task DeclineRequest(Guid userId, Guid hikeId)
        {
            try
            {
                var hike = await MainCollection.Find(u => u.Id == hikeId).FirstOrDefaultAsync();
                var user = await _userCollection.Find(u => u.Id == userId).FirstOrDefaultAsync();
                if (hike != null && user != null)
                {
                    if (hike.UserRequests.Contains(userId))
                        hike.UserRequests.Remove(userId);
                    if (user.HikeRequests.Contains(hikeId))
                        user.HikeRequests.Remove(hikeId);

                    await FullUpdate(hike);
                    await _userService.FullUpdate(user);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(Hike)}: Can't accept request to Hike");
            }
        }

        /// <summary>
        /// Find users by UserHikeNeed for Hike
        /// </summary>
        /// <param name="hikeId">Id of Hike</param>
        /// <returns></returns>
        public virtual async Task<List<Guid>> FindUsers(Guid hikeId)
        {
            try
            {
                var hike = await MainCollection.Find(u => u.Id == hikeId).FirstOrDefaultAsync();
                var needs = await _userHikeNeedCollection.Find(u => !u.Completed && u.HikeType.Contains(hike.HikeType)).ToListAsync();
                needs = needs.Where(u => (u.StartDate.Value - hike.StartDate).Days < u.MaxShift).ToList();
                needs = needs.Where(u => (u.EndDate.Value - hike.EndDate).Days < u.MaxShift).ToList();
                return needs.Select(n => n.UserId).ToList();
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(Hike)}: Can't find Hikes for this need");
                return new List<Guid>();
            }
        }
        
        /// <summary>
        /// Insert TravelKit
        /// </summary>
        /// <param name="entity">TravelKit entity</param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<string>> InsertTravelKit(TravelKit entity)
        {
            try
            {
                //create new Id
                entity.Id = new Guid();
                //validate
                var errors = await ValidateTravelKit(entity);
                if (errors.Count > 0)
                    return errors;
                //and insert entity
                await _travelKitCollection.InsertOneAsync(entity);
                Logger.LogDebug($"{nameof(TravelKit)}: Item with id: {entity.Id} inserted");
                return new List<string>();
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(TravelKit)}: Cant insert item with id {entity.Id}");
                return new List<string>() { "Cant insert entity" };
            }
        }
        
        /// <summary>
        /// Get all TravelKits
        /// </summary>
        /// <returns></returns>
        public virtual async Task<IEnumerable<TravelKit>> GetAllTravelKit(int limit = 10, int skip = 0)
        {
            try
            {
                return await _travelKitCollection.Find(t => true).Skip(skip).Limit(limit).ToListAsync();
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(TravelKit)}: Cant get all items");
                return new List<TravelKit>();
            }
        }

        /// <summary>
        /// Get one TravelKit by id
        /// </summary>
        /// <param name="trvelKitId">Id of TravelKit</param>
        /// <returns></returns>
        public virtual async Task<TravelKit> GetTravelKitById(Guid trvelKitId)
        {
            try
            {
                return await _travelKitCollection.Find(t => t.Id == trvelKitId).FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(TravelKit)}: Cant get item");
                return null;
            }
        }

        /// <summary>
        /// Update TravelKit
        /// </summary>
        /// <param name="entity">TravelKit entity</param>
        /// <param name="isUpsert"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<string>> FullUpdateTravelKit(TravelKit entity, bool isUpsert = false)
        {
            try
            {
                //validate
                var errors = await ValidateTravelKit(entity);
                if (errors.Count > 0)
                    return errors;
                //and update if good
                await _travelKitCollection.ReplaceOneAsync(Builders<TravelKit>.Filter.Eq(e => e.Id, entity.Id), entity, new ReplaceOptions() { IsUpsert = isUpsert });
                Logger.LogDebug($"{nameof(TravelKit)}: Item with id: {entity.Id} updated");
                return new List<string>();
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(TravelKit)}: Cant update item with id {entity.Id}");
                return new List<string>() { "Cant update entity" };
            }
        }

        /// <summary>
        /// Delete TravelKit
        /// </summary>
        /// <param name="id">TravelKit id</param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<string>> DeleteTravelKitById(Guid id)
        {
            try
            {
                var entity = _travelKitCollection.Find(f => f.Id == id).FirstOrDefault();
                //if null 
                if (entity == null)
                    return new List<string>() { "Entity not found" };
                //validate
                var errors = await ValidateTravelKitBeforeDelete(entity);
                if (errors.Count > 0)
                    return errors;
                //delete
                await _travelKitCollection.DeleteOneAsync(Builders<TravelKit>.Filter.Eq(e => e.Id, id));
                Logger.LogDebug($"{nameof(TravelKit)}: Item with id: {id} deleted");
                return new List<string>();
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(TravelKit)}: Cant delete item with id {id}");
                return new List<string>();
            }
        }

        /// <summary>
        /// Insert TravelKitItem
        /// </summary>
        /// <param name="entity">TravelKitItem entity</param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<string>> InsertTravelKitItem(TravelKitItem entity)
        {
            try
            {
                //create new Id
                entity.Id = new Guid();
                //validate
                var errors = await ValidateTravelKitItem(entity);
                if (errors.Count > 0)
                    return errors;
                //and insert entity
                await _travelKitItemCollection.InsertOneAsync(entity);
                Logger.LogDebug($"{nameof(TravelKitItem)}: Item with id: {entity.Id} inserted");
                return new List<string>();
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(TravelKitItem)}: Cant insert item with id {entity.Id}");
                return new List<string>() { "Cant insert entity" };
            }
        }

       

        /// <summary>
        /// Get all TravelKitItems
        /// </summary>
        /// <returns></returns>
        public virtual async Task<IEnumerable<TravelKitItem>> GetAllTravelKitItems(int limit = 10, int skip = 0)
        {
            try
            {
                return await _travelKitItemCollection.Find(t => true).Skip(skip).Limit(limit).ToListAsync();
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(TravelKitItem)}: Cant get all items");
                return new List<TravelKitItem>();
            }
        }

        /// <summary>
        /// Get one TravelKitItem by id
        /// </summary>
        /// <param name="trvelKitId">Id of TravelKitItem</param>
        /// <returns></returns>
        public virtual async Task<TravelKitItem> GetTravelKitItemById(Guid trvelKitId)
        {
            try
            {
                return await _travelKitItemCollection.Find(t => t.Id == trvelKitId).FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(TravelKitItem)}: Cant get item");
                return null;
            }
        }

        /// <summary>
        /// Update TravelKit
        /// </summary>
        /// <param name="entity">TravelKit entity</param>
        /// <param name="isUpsert"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<string>> FullUpdateTravelKitItem(TravelKitItem entity, bool isUpsert = false)
        {
            try
            {
                //validate
                var errors = await ValidateTravelKitItem(entity);
                if (errors.Count > 0)
                    return errors;
                //and update if good
                await _travelKitItemCollection.ReplaceOneAsync(Builders<TravelKitItem>.Filter.Eq(e => e.Id, entity.Id), entity, new ReplaceOptions() { IsUpsert = isUpsert });
                Logger.LogDebug($"{nameof(TravelKitItem)}: Item with id: {entity.Id} updated");
                return new List<string>();
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(TravelKitItem)}: Cant update item with id {entity.Id}");
                return new List<string>() { "Cant update entity" };
            }
        }

        /// <summary>
        /// Delete TravelKit
        /// </summary>
        /// <param name="id">TravelKit id</param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<string>> DeleteTravelKitItemById(Guid id)
        {
            try
            {
                var entity = _travelKitItemCollection.Find(f => f.Id == id).FirstOrDefault();
                //if null 
                if (entity == null)
                    return new List<string>() { "Entity not found" };
                //validate
                var errors = await ValidateTravelKitItemBeforeDelete(entity);
                if (errors.Count > 0)
                    return errors;
                //delete
                await _travelKitItemCollection.DeleteOneAsync(Builders<TravelKitItem>.Filter.Eq(e => e.Id, id));
                Logger.LogDebug($"{nameof(TravelKitItem)}: Item with id: {id} deleted");
                return new List<string>();
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(TravelKitItem)}: Cant delete item with id {id}");
                return new List<string>();
            }
        }

        /// <summary>
        /// Add item to the kit
        /// </summary>
        /// <param name="itemId">Item id</param>
        /// <param name="kitId">Kit id</param>
        /// <returns></returns>
        public virtual async Task AddItemToKit(Guid itemId, Guid kitId)
        {
            try
            {
                var kit = await _travelKitCollection.Find(k => k.Id == kitId).FirstOrDefaultAsync();
                var item = await _travelKitItemCollection.Find(i => i.Id == itemId).FirstOrDefaultAsync();

                if (kit != null && item != null) kit.Items.Add(itemId);
                await FullUpdateTravelKit(kit);
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(TravelKit)}, {nameof(TravelKitItem)}: Can't add item to kit");
            }
        }

        /// <summary>
        /// Remove user request to Hike
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="hikeId"></param>
        public virtual async Task RemoveHikeRequset(Guid userId, Guid hikeId)
        {
            try
            {
                var user = await _userCollection.Find(u => u.Id == userId).FirstOrDefaultAsync();
                var hike = await _hikeCollection.Find(u => u.Id == hikeId).FirstOrDefaultAsync();
                if (user != null && hike != null)
                {
                    if (user.HikeRequests.Contains(hikeId))
                        user.HikeRequests.Remove(hikeId);
                    if (hike.UserRequests.Contains(userId))
                        hike.UserRequests.Remove(userId);

                    await _userService.FullUpdate(user);
                    await FullUpdate(hike);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(User)}: Cant remove Hike Request");
            }
        }


        /// <summary>
        /// Set user request to Hike
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="hikeId">Hike Id</param>
        public virtual async Task SetHikeRequest(Guid userId, Guid hikeId)
        {
            try
            {
                var user = await _userCollection.Find(u => u.Id == userId).FirstOrDefaultAsync();
                var hike = await _hikeCollection.Find(u => u.Id == hikeId).FirstOrDefaultAsync();
                if (user != null && hike != null)
                {
                    if (!user.HikeRequests.Contains(hikeId))
                        user.HikeRequests.Add(hikeId);
                    if (!hike.UserRequests.Contains(userId))
                        hike.UserRequests.Add(userId);
                    await _userService.FullUpdate(user);
                    await FullUpdate(hike);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(User)}: Cant set request to Hike");
            }
        }

        /// <summary>
        /// Accept invite frm the Hike
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="hikeId">Hike id</param>
        /// <returns></returns>
        public virtual async Task AcceptInvite(Guid userId, Guid hikeId)
        {
            try
            {
                var user = await _userCollection.Find(u => u.Id == userId).FirstOrDefaultAsync();
                var hike = await _hikeCollection.Find(u => u.Id == hikeId).FirstOrDefaultAsync();
                if (user != null && hike != null)
                {
                    if (user.HikeInvites.Contains(hikeId))
                    {
                        user.HikeInvites.Remove(hikeId);
                        user.Hikes.Add(hikeId);
                    }
                    if (hike.UserInvites.Contains(userId))
                    {
                        hike.Group.Add(userId);
                        hike.UserInvites.Remove(userId);
                    }

                    await _userService.FullUpdate(user);
                    await FullUpdate(hike);

                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(User)}: Cant accept invite to Hike");
            }
        }

        /// <summary>
        /// Decline Invite from the Hike
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="hikeId">Hike Id</param>
        /// <returns></returns>
        public virtual async Task DeclineInvite(Guid userId, Guid hikeId)
        {
            try
            {
                var user = await _userCollection.Find(u => u.Id == userId).FirstOrDefaultAsync();
                var hike = await _hikeCollection.Find(u => u.Id == hikeId).FirstOrDefaultAsync();
                if (user != null && hike != null)
                {
                    if (user.HikeInvites.Contains(hikeId))
                    {
                        user.HikeInvites.Remove(hikeId);
                    }
                    if (hike.UserInvites.Contains(userId))
                    {
                        hike.UserInvites.Remove(userId);
                    }

                    await _userService.FullUpdate(user);
                    await FullUpdate(hike);

                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(User)}: Cant accept invite to Hike");
            }
        }


        private async Task<List<string>> ValidateTravelKitItemBeforeDelete(TravelKitItem entity)
        {
            return new List<string>();       
        }

        private async Task<List<string>> ValidateTravelKitBeforeDelete(TravelKit entity)
        {
            return new List<string>();
        }

        private async Task<List<string>> ValidateTravelKit(TravelKit entity)
        {
            return new List<string>();
        }

        private async Task<List<string>> ValidateTravelKitItem(TravelKitItem entity)
        {
            return new List<string>();
        }

        //Add TrvelKitItem to TravelKit
    }
}
