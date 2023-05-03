using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tramy.Common.UserEvent;
using Tramy.Common.Users;

namespace Tramy.Backend.Data.DBServices
{
    public class UserEventService : BaseDbService<UserEvent>
    {
        /// <summary>
        /// Main MongoDB collection of UserEvents
        /// </summary>
        private readonly IMongoCollection<UserEvent> _userEventCollection;

        private readonly IMongoCollection<User> _userCollection;

        private UserService _userService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration">Configuration must be link from Startup</param>
        /// <param name="logger">Logger must be link from Startup</param>
        public UserEventService( IConfiguration configuration, ILogger<BaseDbService<UserEvent>> logger, UserService userService, BaseLogService logService) : base(configuration, logger, logService)
        {
            _userEventCollection = MainCollection.Database.GetCollection<UserEvent>(nameof(UserEvent));
            _userCollection = MainCollection.Database.GetCollection<User>(nameof(User));
            _userService = userService;
        }

        /// <summary>
        /// Get UserEvent by its name
        /// </summary>
        /// <param name="name">UserEvent's name</param>
        /// <returns></returns>
        public virtual async Task<List<UserEvent>> GetByName(string name)
        {
            name = name.Trim().ToLower();
            try
            {
                var result = await _userEventCollection.Find(f => f.Open).ToListAsync();
                result = result.Where(r => r.Name.ToLower().Contains(name)).ToList();
                return result;
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(UserEvent)}: Cant get item by id {name}");
                return null;
            }
        }

        /// <summary>
        /// Get List of user's request
        /// </summary>
        /// <param name="ueId">Id of UserEvent</param>
        /// <param name="limit">How mane records</param>
        /// <param name="skip">Skip from the begining</param>
        /// <returns></returns>
        public virtual async Task<List<User>> GetIncomingUsers (Guid ueId, int limit = 10, int skip = 0) 
        {
            UserEvent userEvent = (await _userEventCollection.FindAsync(ue => ue.Id == ueId)).FirstOrDefault();
            if(userEvent != null)
            {
                try
                {
                    List<User> result = new List<User>();
                    foreach(var user in userEvent.GuestRequests)
                    {
                        result.Add(await _userCollection.Find(u => u.Id == user).FirstOrDefaultAsync());
                    }
                    return result.Skip(skip).Take(limit).ToList();
                }
                catch (Exception e)
                {
                    Logger.LogError(e, $"{nameof(UserEvent)}: Can't get list of user requests");
                    return null;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets list of invited users
        /// </summary>
        /// <param name="ueId">UserEvent id</param>
        /// <param name="limit">How mane records</param>
        /// <param name="skip">Skip from the begining</param>
        /// <returns></returns>
        public virtual async Task<List<User>> GetInvitedUsers(Guid ueId, int limit, int skip)
        {
            UserEvent userEvent = (await _userEventCollection.FindAsync(ue => ue.Id == ueId)).FirstOrDefault();
            if (userEvent != null)
            {
                try
                {
                    List<User> result = new List<User>();
                    foreach (var user in userEvent.InvitedGuest)
                    {
                        result.Add(await _userCollection.Find(u => u.Id == user).FirstOrDefaultAsync());
                    }
                    return result.Skip(skip).Take(limit).ToList();
                }
                catch (Exception e)
                {
                    Logger.LogError(e, $"{nameof(UserEvent)}: Can't get list of invited users");
                    return null;
                }
            }
            return null;
        }

        /// <summary>
        /// Invite User to the event
        /// </summary>
        /// <param name="eventId">Id of the UserEvent</param>
        /// <param name="userId">Id of the User</param>
        /// <returns></returns>
        public virtual async Task InviteToEvent(Guid eventId, Guid userId)
        {
            try
            {
                var userEvent = await MainCollection.Find(ue => ue.Id == eventId).FirstOrDefaultAsync();
                var user = await _userCollection.Find(u => u.Id == userId).FirstOrDefaultAsync();
                if (userEvent != null && user != null)
                {
                    if (!userEvent.InvitedGuest.Contains(userId)) userEvent.InvitedGuest.Add(userId);
                    if (!user.UserEventInvites.Contains(eventId)) user.UserEventInvites.Add(eventId);

                    await FullUpdate(userEvent);
                    await _userService.FullUpdate(user);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(UserEvent)}: Cant invite user");
            }
        }

        /// <summary>
        /// Approve user's request to the UserEvent
        /// </summary>
        /// <param name="eventId">Id of the UserEvent</param>
        /// <param name="userId">Id of the user</param>
        /// <returns></returns>
        public virtual async Task ApproveRequest(Guid eventId, Guid userId)
        {
            try
            {
                var user = await _userCollection.Find(u => u.Id == userId).FirstOrDefaultAsync();
                var userEvent = await MainCollection.Find(b => b.Id == eventId).FirstOrDefaultAsync();
                if (user != null && userEvent != null)
                {
                    if (!user.UserEvents.Contains(eventId)) user.UserEvents.Add(eventId);
                    if (!userEvent.Guests.Contains(userId)) userEvent.Guests.Add(userId);

                    if (user.UserEventRequests.Contains(eventId)) user.UserEventRequests.Remove(eventId);
                    if (userEvent.GuestRequests.Contains(userId)) userEvent.GuestRequests.Remove(userId);

                    await FullUpdate(userEvent);
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
        /// <param name="eventId">Id of the UserEvent</param>
        /// <param name="userId">Id of the user</param>
        /// <returns></returns>
        public virtual async Task DisapproveRequest(Guid eventId, Guid userId)
        {
            try
            {
                var user = await _userCollection.Find(u => u.Id == userId).FirstOrDefaultAsync();
                var userEvent = await MainCollection.Find(b => b.Id == eventId).FirstOrDefaultAsync();
                if (user != null && userEvent != null)
                {
                    if (user.UserEventRequests.Contains(eventId)) user.UserEventRequests.Remove(eventId);
                    if (userEvent.GuestRequests.Contains(userId)) userEvent.GuestRequests.Remove(userId);

                    await FullUpdate(userEvent);
                    await _userService.FullUpdate(user);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(User)}: Can't disapprove request");
            }
        }

        /// <summary>
        /// Leave UserEvent
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="eventId">UserEvent id</param>
        /// <returns></returns>
        public virtual async Task LeaveUserEvent(Guid userId, Guid eventId)
        {
            try
            {
                var userEvent = await _userEventCollection.Find(ue => ue.Id == eventId).FirstOrDefaultAsync();
                var user = await _userCollection.Find(u => u.Id == userId).FirstOrDefaultAsync();

                if (userEvent != null && user != null)
                {
                    if (user.UserEvents.Contains(eventId)) user.UserEvents.Remove(eventId);
                    if (userEvent.Guests.Contains(userId)) userEvent.Guests.Remove(userId);

                    await _userService.FullUpdate(user);
                    await FullUpdate(userEvent);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(UserEvent)}: Cant leave event");
            }
        }

        /// <summary>
        /// Get all UserEvents of the user
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns>List of UserEvents</returns>
        public virtual async Task<List<UserEvent>> GetUserEvents(Guid id, int skip, int limit)
        {
            try
            {
                var user = await _userCollection.Find(u => u.Id == id).FirstOrDefaultAsync();
                return user.UserEvents.Select(b => GetById(b).Result).Skip(skip).Take(limit).ToList();
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(UserEvent)}: Cant get bunches");
                return new List<UserEvent>();
            }
        }

        /// <summary>
        /// Request from the user to the bunch
        /// </summary>
        /// <param name="eventId">Id of the bunch</param>
        /// <param name="userId">Id of the user</param>
        /// <returns></returns>
        public virtual async Task SetUserEventRequest(Guid eventId, Guid userId)
        {
            try
            {
                var userEvent = await MainCollection.Find(ue => ue.Id == eventId).FirstOrDefaultAsync();
                var user = await _userCollection.Find(u => u.Id == userId).FirstOrDefaultAsync();

                if (userEvent != null && user != null)
                {
                    if (!user.UserEventRequests.Contains(eventId)) user.UserEventRequests.Add(eventId);
                    if (!userEvent.InvitedGuest.Contains(userId)) userEvent.InvitedGuest.Add(userId);

                    await _userService.FullUpdate(user);
                    await FullUpdate(userEvent);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(UserEvent)}: Cant set request");
            }
        }

        /// <summary>
        /// Approve invite from the UserEvent
        /// </summary>
        /// <param name="eventId">Id of the UserEvent</param>
        /// <param name="userId">Id of the user</param>
        /// <returns></returns>
        public virtual async Task ApproveUserEventInvite(Guid eventId, Guid userId)
        {
            try
            {
                var user = await _userCollection.Find(u => u.Id == userId).FirstOrDefaultAsync();
                var userEvent = await _userEventCollection.Find(ue => ue.Id == eventId).FirstOrDefaultAsync();
                if (user != null && userEvent != null)
                {
                    if (!user.UserEvents.Contains(eventId)) user.UserEvents.Add(eventId);
                    if (!userEvent.Guests.Contains(userId)) userEvent.Guests.Add(userId);

                    if (user.UserEventInvites.Contains(eventId)) user.UserEventInvites.Remove(eventId);
                    if (userEvent.InvitedGuest.Contains(userId)) userEvent.InvitedGuest.Remove(userId);

                    await _userService.FullUpdate(user);
                    await FullUpdate(userEvent);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(UserEvent)}: Cant approve invite");
            }
        }

        /// <summary>
        /// Dissaprove invite from the UserEvent
        /// </summary>
        /// <param name="eventId">Id of the UserEvent</param>
        /// <param name="userId">Id of the user</param>
        /// <returns></returns>
        public virtual async Task DisapproveUserEventInvite(Guid eventId, Guid userId)
        {
            try
            {
                var user = await _userCollection.Find(u => u.Id == userId).FirstOrDefaultAsync();
                var userEvent = await _userEventCollection.Find(b => b.Id == eventId).FirstOrDefaultAsync();
                if (user != null && userEvent != null)
                {
                    if (user.UserEventInvites.Contains(eventId)) user.UserEventInvites.Remove(eventId);
                    if (userEvent.InvitedGuest.Contains(userId)) userEvent.InvitedGuest.Remove(userId);

                    await _userService.FullUpdate(user);
                    await FullUpdate(userEvent);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(UserEvent)}: Cant disapprove invite");
            }
        }

    }
}
