using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tramy.Common.Chats;

using Tramy.Common.Users;


namespace Tramy.Backend.Data.DBServices
{
    /// <summary>
    /// Service to manage bunches
    /// </summary>
    public class BunchService:BaseDbService<Bunch>
    {
        /// <summary>
        /// Collection of bunches
        /// </summary>
        private readonly IMongoCollection<Bunch> _bunchCollection;

        private readonly IMongoCollection<User> _userCollection;

        private UserService _userService;

        
        /// <summary>
        /// Bunch constructor
        /// </summary>
        /// <param name="configuration">Configuration must be linked from Startup</param>
        /// <param name="logger">Logger must be linked from Startup</param>
        public BunchService(UserService userService, IConfiguration configuration, ILogger<BaseDbService<Bunch>> logger, BaseLogService logService) : base(configuration, logger, logService)
        {
            _bunchCollection = MainCollection.Database.GetCollection<Bunch>(nameof(Bunch));
            _userCollection = MainCollection.Database.GetCollection<User>(nameof(User));
            _userService = userService;
        }

        /// <summary>
        /// Leave Bunch
        /// </summary>
        /// <param name="userId">Id of the user</param>
        /// <param name="bunchId">Id of the bunch</param>
        /// <returns></returns>
        public virtual async Task LeaveBunch(Guid userId, Guid bunchId)
        {
            try
            {
                var bunch = await MainCollection.Find(ue => ue.Id == bunchId).FirstOrDefaultAsync();
                var user = await _userCollection.Find(u => u.Id == userId).FirstOrDefaultAsync();

                if (bunch != null && user != null)
                {
                    if (user.Bunches.Contains(bunchId)) user.Bunches.Remove(bunchId);
                    if (bunch.Users.Contains(userId)) bunch.Users.Remove(userId);

                    await _userService.FullUpdate(user);
                    await FullUpdate(bunch);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(Bunch)}: Cant leave bunch");
            }
        }

        /// <summary>
        /// Get all bunches by user
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns></returns>
        public virtual async Task<List<Bunch>> GetBunches(Guid id, int skip, int limit)
        {
            try
            {
                var user = await _userCollection.Find(u => u.Id == id).FirstOrDefaultAsync();
                var result = user.Bunches.Select(b => GetById(b).Result).Skip(skip).Take(limit).ToList();
                return result;
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(Bunch)}: Cant get bunches");
                return new List<Bunch>();
            }
        }

        /// <summary>
        /// Request from the user to the bunch
        /// </summary>
        /// <param name="bunchId">Id of the bunch</param>
        /// <param name="userId">Id of the user</param>
        /// <returns></returns>
        public virtual async Task SetBunchRequest(Guid bunchId, Guid userId)
        {
            try
            {
                var bunch = await MainCollection.Find(u => u.Id == bunchId).FirstOrDefaultAsync();
                var user = await _userCollection.Find(u => u.Id == userId).FirstOrDefaultAsync();

                if (bunch != null && user != null)
                {
                    if (!user.BunchRequests.Contains(bunchId)) user.BunchRequests.Add(bunchId);
                    if (!bunch.UserRequests.Contains(userId)) bunch.UserRequests.Add(userId);

                    await _userService.FullUpdate(user);
                    await FullUpdate(bunch);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(Bunch)}: Cant set request");
            }
        }

        /// <summary>
        /// Approve invite from the bunch
        /// </summary>
        /// <param name="bunchId">Id of the bunch</param>
        /// <param name="userId">Id of the user</param>
        /// <returns></returns>
        public virtual async Task ApproveBunchInvite(Guid bunchId, Guid userId)
        {
            try
            {
                var user = await _userCollection.Find(u => u.Id == userId).FirstOrDefaultAsync();
                var bunch = await MainCollection.Find(b => b.Id == bunchId).FirstOrDefaultAsync();
                if (user != null && bunch != null)
                {
                    if (!user.Bunches.Contains(bunchId)) user.Bunches.Add(bunchId);
                    if (!bunch.Users.Contains(userId)) bunch.Users.Add(userId);

                    if (user.BunchInvites.Contains(bunchId)) user.BunchInvites.Remove(bunchId);
                    if (bunch.InvitedUsers.Contains(userId)) bunch.InvitedUsers.Remove(userId);

                    await _userService.FullUpdate(user);
                    await FullUpdate(bunch);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(Bunch)}: Cant approve invite");
            }
        }

        /// <summary>
        /// Dissaprove invite from the bunch
        /// </summary>
        /// <param name="bunchId">Id of the bunch</param>
        /// <param name="userId">Id of the user</param>
        /// <returns></returns>
        public virtual async Task DisapproveBunchInvite(Guid bunchId, Guid userId)
        {
            try
            {
                var user = await _userCollection.Find(u => u.Id == userId).FirstOrDefaultAsync();
                var bunch = await MainCollection.Find(b => b.Id == bunchId).FirstOrDefaultAsync();
                if (user != null && bunch != null)
                {
                    if (user.BunchInvites.Contains(bunchId)) user.BunchInvites.Remove(bunchId);
                    if (bunch.InvitedUsers.Contains(userId)) bunch.InvitedUsers.Remove(userId);

                    await _userService.FullUpdate(user);
                    await FullUpdate(bunch);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(Bunch)}: Cant disapprove invite");
            }
        }

        /// <summary>
        /// Get all users in the bunch
        /// </summary>
        /// <param name="id">Bunch id</param>
        /// <returns></returns>
        public virtual async Task<List<User>> GetUsers (Guid id, int skip, int limit)
        {
            try
            {
                var bunch = await MainCollection.Find(b => b.Id == id).FirstOrDefaultAsync();
                var result = bunch.Users.Select(u => _userService.GetById(u).Result).Skip(skip).Take(limit).ToList();
                return result;
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(User)}: Cant get users");
                return new List<User>();            
            }
        }

        /// <summary>
        /// Get all user requests to the bunch
        /// </summary>
        /// <param name="id">Bunch id</param>
        /// <returns></returns>
        public virtual async Task<List<User>> GetRequests(Guid id)
        {
            try
            {
                var bunch = await MainCollection.Find(b => b.Id == id).FirstOrDefaultAsync();
                return bunch.UserRequests.Select(u => _userService.GetById(u).Result).ToList();
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(User)}: Cant get users");
                return new List<User>();
            }
        }

        /// <summary>
        /// Get all invites to users in the bunch
        /// </summary>
        /// <param name="id">Bunch id</param>
        /// <returns></returns>
        public virtual async Task<List<User>> GetInvites(Guid id)
        {
            try
            {
                var bunch = await MainCollection.Find(b => b.Id == id).FirstOrDefaultAsync();
                return bunch.InvitedUsers.Select(u => _userService.GetById(u).Result).ToList();
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(User)}: Cant get users");
                return new List<User>();
            }
        }

        /// <summary>
        /// Find bunch by its name
        /// </summary>
        /// <param name="name">Search string</param>
        /// <returns></returns>
        public virtual async Task<List<Bunch>> FindBunchByName (string name)
        {
            try
            {
                if (String.IsNullOrEmpty(name)) return new List<Bunch>();

                name = name.ToLower().Trim();
                return await MainCollection.Find(b => b.Name.ToLower().Contains(name) && b.Open == true).Limit(10).ToListAsync();
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(Bunch)}: Cant find bunches");
                return new List<Bunch>();
            }
        }

        /// <summary>
        /// Find bunches by the list of interests
        /// </summary>
        /// <param name="interests">List of the interests</param>
        /// <param name="city">City where bunch exists</param>
        /// <returns></returns>
        public virtual async Task<List<Bunch>> FindBunchByInterests (List<string> interests, string city)
        {
            try
            {
                if (interests.Count == 0) return new List<Bunch>();
                city = city.Trim().ToLower();
                var bunches = new List<Bunch>();
                
                interests.Select(i => i.ToLower().Trim()).ToList();
                foreach (var item in interests)
                {
                    bunches.AddRange(MainCollection.Find(
                        b => 
                    b.Interests.Where(i => i.Contains(item)).Any() 
                    && b.City.ToLower().Contains(city))
                        .Limit(10)
                        .ToList());
                    }
                return bunches.Distinct().ToList();
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(Bunch)}: Can't find bunches");
                return new List<Bunch>();
            }
        }

        /// <summary>
        /// Invite User to the Bunch
        /// </summary>
        /// <param name="bunchId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual async Task InviteToBunch(Guid bunchId, Guid userId)
        {
            try
            {
                var bunch = await MainCollection.Find(x => x.Id == bunchId).FirstOrDefaultAsync();
                var user = await _userCollection.Find(x => x.Id == userId).FirstOrDefaultAsync();

                if (!bunch.InvitedUsers.Contains(userId)) bunch.InvitedUsers.Add(userId);
                if (!user.BunchInvites.Contains(bunchId)) user.BunchInvites.Add(bunchId);

                await FullUpdate(bunch);
                await _userService.FullUpdate(user);
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(Bunch)}: Cant invite user");
            }
        }

        /// <summary>
        /// Approve user's request to the bunch
        /// </summary>
        /// <param name="bunchId">Id of the bunch</param>
        /// <param name="userId">Id of the user</param>
        /// <returns></returns>
        public virtual async Task ApproveRequest(Guid bunchId, Guid userId)
        {
            try
            {
                var user = await _userCollection.Find(u => u.Id == userId).FirstOrDefaultAsync();
                var bunch = await MainCollection.Find(b => b.Id == bunchId).FirstOrDefaultAsync();
                if (user != null && bunch != null)
                {
                    if (!user.Bunches.Contains(bunchId)) user.Bunches.Add(bunchId);
                    if (!bunch.Users.Contains(userId)) bunch.Users.Add(userId);

                    if (user.BunchRequests.Contains(bunchId)) user.BunchRequests.Remove(bunchId);
                    if (bunch.UserRequests.Contains(userId)) bunch.UserRequests.Remove(userId);

                    await FullUpdate(bunch);
                    await _userService.FullUpdate(user);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(User)}: Can't approve request");
            }
        }

        /// <summary>
        /// Dissaprove request from the user
        /// </summary>
        /// <param name="bunchId">Id of the bunch</param>
        /// <param name="userId">Id of the user</param>
        /// <returns></returns>
        public virtual async Task DisapproveRequest(Guid bunchId, Guid userId)
        {
            try
            {
                var user = await _userCollection.Find(u => u.Id == userId).FirstOrDefaultAsync();
                var bunch = await MainCollection.Find(b => b.Id == bunchId).FirstOrDefaultAsync();
                if (user != null && bunch != null)
                {
                    if (user.BunchRequests.Contains(bunchId)) user.BunchRequests.Remove(bunchId);
                    if (bunch.UserRequests.Contains(userId)) bunch.UserRequests.Remove(userId);

                    await FullUpdate(bunch);
                    await _userService.FullUpdate(user);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(User)}: Can't disapprove request");
            }
        }

        }
    }

