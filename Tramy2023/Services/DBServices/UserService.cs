using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Tramy.Backend.Helpers;
using Tramy.Common.Chats;
using Tramy.Common.CrossModels;
using Tramy.Common.Hike;
using Tramy.Common.Locations;
using Tramy.Common.UserEvent;
using Tramy.Common.Users;

namespace Tramy.Backend.Data.DBServices
{
    /// <summary>
    /// User's service for MongoDB
    /// </summary>
    public class UserService : BaseDbService<User>
    {
        private readonly IMongoCollection<UserEvent> _userEventCollection;
        private readonly IMongoCollection<Hike> _hikeCollection;
        private readonly IMongoCollection<UserHikeNeed> _userHikeNeedCollection;
        private readonly IMongoCollection<Bunch> _bunchCollection;
      

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration">Configuration must be link from Startup</param>
        /// <param name="logger">Logger must be link from Startup</param>
        public UserService(ServiceConfiguration configuration, ILogger<BaseDbService<User>> logger, NotificationService notificationService, BaseLogService logService) : base(configuration, logger, logService)
        {
            _hikeCollection = MainCollection.Database.GetCollection<Hike>(nameof(Hike));
            _userEventCollection = MainCollection.Database.GetCollection<UserEvent>(nameof(UserEvent));
            _userHikeNeedCollection = MainDatabase.GetCollection<UserHikeNeed>(nameof(Common.Users.UserHikeNeed));
            _bunchCollection = MainCollection.Database.GetCollection<Bunch>(nameof(Bunch));
            _notService = notificationService;
        }

        private readonly Random _rnd = new Random();
        private readonly NotificationService _notService;

        /// <summary>
        /// Set user coordinates
        /// </summary>
        public virtual async Task SetMyLocation(Guid id, Guid? locationId, Guid? locationPartId, int x, int y)
        {
            try
            {
                var myUser = await MainCollection.Find(u => u.Id == id).FirstOrDefaultAsync();
                if (myUser != null)
                {
                    myUser.CurrentLocation = locationId;
                    myUser.CurrentLocationPart = locationPartId;
                    myUser.CurrentX = x;
                    myUser.CurrentY = y;
                    await FullUpdate(myUser);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(User)}: Cant set location");
            }
        }

        /// <summary>
        /// Set user chat hub id
        /// </summary>
        public virtual async Task SetChatHubId(Guid id, string chatHubId)
        {
            try
            {
                var myUser = await MainCollection.Find(u => u.Id == id).FirstOrDefaultAsync();
                if (myUser != null)
                {
                    myUser.CurrentChatHubId = chatHubId;
                    await FullUpdate(myUser);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(User)}: Cant set location");
            }
        }

        /// <summary>
        /// Change text status of the user
        /// </summary>
        /// <returns></returns>
        public virtual async Task ChangeTextStatus(Guid id, string status)
        {
            try
            {
                var user = await MainCollection.Find(u => u.Id == id).FirstOrDefaultAsync();
                if (!string.IsNullOrEmpty(status.Trim()))
                {
                    user.TextStatus = status;
                    await FullUpdate(user);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(User)}: Can't set TextStatus");
            }
        }

        /// <summary>
        /// Set user noti hub id
        /// </summary>
        public virtual async Task SetNotiHubId(Guid id, string notiHubId)
        {
            try
            {
                var myUser = await MainCollection.Find(u => u.Id == id).FirstOrDefaultAsync();
                if (myUser != null)
                {
                    myUser.CurrentNotiHubId = notiHubId;
                    await FullUpdate(myUser);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(User)}: Cant set location");
            }
        }

        /// <summary>
        /// Set user GPS coordinates
        /// </summary>
        public virtual async Task SetMyGPSLocation(Guid id, double lat, double lon)
        {
            try
            {
                var myUser = await MainCollection.Find(u => u.Id == id).FirstOrDefaultAsync();
                if (myUser != null)
                {
                    myUser.GeoPoint[0] = lat;
                    myUser.GeoPoint[1] = lon;
                    await FullUpdate(myUser);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(User)}: Cant set location");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public virtual async Task ChangeVisibleStatus(Guid id, string status)
        {
            try
            {
                var user = await MainCollection.Find(u => u.Id == id).FirstOrDefaultAsync();
                if (!string.IsNullOrEmpty(status.Trim()) && user.Visibility.Perm)
                {
                    user.Visibility.VisibleStatus = (VisibleStatus)Enum.Parse(typeof(VisibleStatus), status);
                    await FullUpdate(user);
                    foreach (var par in user.Parents)
                    {
                        var not = new Common.Notification.UserNotification()
                        {
                            Id = Guid.NewGuid(),
                            UserToId = par,
                            IsRead = false,
                            IsSend = true,
                            CreationDate = DateTime.Today,
                            Message = $"{user.FirstName} {user.LastName} change visible status"
                        };
                        await _notService.Insert(not);
                        MainCollection.Find(u => u.Id == par).FirstOrDefault().Notifications.Add(not.Id);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(User)}: Cant set location");
            }
        }

        /// <summary>
        /// Change visible permission for children
        /// </summary>
        /// <param name="parId">Parent Id</param>
        /// <param name="chId">Child id</param>
        /// <returns></returns>
        public virtual async Task ChangeVisiblePerm(Guid parId, Guid chId)
        {
            try
            {
                var parent = await MainCollection.Find(u => u.Id == parId).FirstOrDefaultAsync();
                var child = await MainCollection.Find(u => u.Id == chId).FirstOrDefaultAsync();

                if (parent.Children.Contains(chId) && child.Parents.Contains(parId))
                {
                    child.Visibility.Perm = !child.Visibility.Perm;
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(User)} can't change permission");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parId"></param>
        /// <param name="chId"></param>
        /// <returns></returns>
        public virtual async Task ChangeStatusPerm(Guid parId, Guid chId)
        {
            try
            {
                var parent = await MainCollection.Find(u => u.Id == parId).FirstOrDefaultAsync();
                var child = await MainCollection.Find(u => u.Id == chId).FirstOrDefaultAsync();

                if (parent.Children.Contains(chId) && child.Parents.Contains(parId))
                {
                    child.UserStatusPerm = !child.UserStatusPerm;
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(User)} can't change permission");
            }
        }

        /// <summary>
        /// Cnahge User Status
        /// </summary>
        /// <param name="id">User Id</param>
        /// <param name="status">User status</param>
        /// <returns></returns>
        public virtual async Task ChangeUserStatus(Guid id, string status)
        {
            try
            {
                var user = await MainCollection.Find(u => u.Id == id).FirstOrDefaultAsync();
                if (user != null && user.UserStatusPerm)
                {
                    user.UserStatus = (UserStatus)Enum.Parse(typeof(UserStatus), status);
                    foreach (var par in user.Parents)
                    {
                        var not = new Common.Notification.UserNotification()
                        {
                            Id = Guid.NewGuid(),
                            UserToId = par,
                            IsRead = false,
                            IsSend = true,
                            CreationDate = DateTime.Today,
                            Message = $"{user.FirstName} {user.LastName} change inform status"
                        };
                        await _notService.Insert(not);
                        MainCollection.Find(u => u.Id == par).FirstOrDefault().Notifications.Add(not.Id);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(User)}: Can't change UserStatus");
            }
        }

        /// <summary>
        /// Create MD5 hash of string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string CreateMd5(string input)
        {
            // Use input string to calculate MD5 hash
            using System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            var inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            var hashBytes = md5.ComputeHash(inputBytes);

            // Convert the byte array to hexadecimal string
            var sb = new StringBuilder();
            foreach (var t in hashBytes)
            {
                sb.Append(t.ToString("X2"));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Get User by email or null if not exists
        /// </summary>
        /// <param name="email">Email of User</param>
        public virtual async Task<User?> GetByEmail(string email)
        {
            try
            {
                return await MainCollection.Find(f => f.Email == email).FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(User)}: Cant get item by email {email}");
                return null;
            }
        }

        /// <summary>
        /// Get User by phone or null if not exists
        /// </summary>
        /// <param name="phone">Phone of User</param>
        public virtual async Task<User> GetByPhone(string phone)
        {
            try
            {
                return await MainCollection.Find(f => f.Phone == phone).FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(User)}: Cant get item by phone {phone}");
                return null;
            }
        }

        private string GetSmsCode()
        {
            var code = _rnd.Next(1000000).ToString();
            while (code.Length < 6)
            {
                code = "0" + code;
            }

            return code;
        }

        /// <summary>
        /// Get User by phone or null if not exists
        /// </summary>
        /// <param name="phone">Phone of User</param>
        public virtual async Task<string> GetSmsCodeAndRegisterIfNeed(string phone)
        {
            try
            {
                var user = await MainCollection.Find(f => f.Phone == phone).FirstOrDefaultAsync();
                var code = GetSmsCode();
                user ??= new User()
                {
                    Phone = phone,
                };
                user.PhoneLoginCode = code;
                await FullUpdate(user, true);
                return code;
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(User)}: Cant get item by phone {phone}");
                return null;
            }
        }

        /// <summary>
        /// Method to find hikes by user request
        /// </summary>
        /// <param name="request">User request</param>
        /// <returns></returns>
        public virtual async Task<List<Guid>> FindHikes(FindHikeRequest request)
        {
            try
            {
                var hikes = await _hikeCollection.Find(h =>
                h.Country.ToLower().Contains(request.Country) &&
                h.State.ToLower().Contains(request.State) &&
                h.Open).ToListAsync();
                if (request.IsPeopleNeed) hikes = hikes.Where(h => (h.Group.Count < h.DesiredNumber)).ToList();
                if (request.Type.HasValue) hikes = hikes.Where(h => h.HikeType.Equals(request.Type.Value)).ToList();
                if (request.Date.HasValue) hikes = hikes.Where(h => (h.StartDate - request.Date.Value).Days < 3).ToList();
                return hikes.Select(h => h.Id).ToList();
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(Hike)}: Cant find hikes");
                return new List<Guid>();
            }
        }


        /// <summary>
        /// Find users by substring
        /// </summary>
        /// <param name="id"></param>
        /// <param name="search"></param>
        public virtual async Task<List<FindUserResult>> FindUsers(Guid id, string search)
        {
            try
            {
                if (string.IsNullOrEmpty(search))
                    return new List<FindUserResult>();

                search = search.ToLower().Trim();
                var users = await MainCollection.Find(f => f.Email.ToLower().Contains(search) || f.FirstName.ToLower().Contains(search) || f.LastName.ToLower().Contains(search)).Limit(10).ToListAsync();
                var result = new List<FindUserResult>();

                var myUser = await MainCollection.Find(u => u.Id == id).FirstOrDefaultAsync();

                if (myUser != null)
                {
                    result.AddRange(users.Where(u => u.Id != myUser.Id).Select(user => new FindUserResult(user, myUser)));
                }

                return result;
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(User)}: Cant find users");
                return new List<FindUserResult>();
            }
        }

        /// <summary>
        /// Get userevents by user's invites
        /// </summary>
        /// <param name="id"></param>
        /// <param name="limit"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        public virtual async Task<List<UserEvent>> GetInvitingEvents(Guid id, int limit, int skip)
        {
            try
            {
                var user = await MainCollection.Find(u => u.Id == id).FirstOrDefaultAsync();
                if (user == null) return null;
                var events = user.UserEventInvites.Select(ue => _userEventCollection.Find(f => f.Id == ue).FirstOrDefault()).ToList();
                return events.Skip(skip).Take(limit).ToList();
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(User)}: Can't get invites for");
                return null;
            }
        }

        /// <summary>
        /// Get userevents by user's requests
        /// </summary>
        /// <param name="id"></param>
        /// <param name="limit"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        public virtual async Task<List<UserEvent>> GetRequestedEvents(Guid id, int limit, int skip)
        {
            try
            {
                var user = await MainCollection.Find(u => u.Id == id).FirstOrDefaultAsync();
                if (user == null) return null;
                var events = user.UserEventRequests.Select(ue => _userEventCollection.Find(f => f.Id == ue).FirstOrDefault()).ToList();
                return events.Skip(skip).Take(limit).ToList();
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(User)}: Can't get requests for");
                return null;
            }
        }

        /// <summary>
        /// Get bunches by user's invites
        /// </summary>
        /// <param name="id"></param>
        /// <param name="limit"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        public virtual async Task<List<Bunch>> GetInvitingBunches(Guid id, int limit, int skip)
        {
            try
            {
                var user = await MainCollection.Find(u => u.Id == id).FirstOrDefaultAsync();
                if (user == null) return null;
                var bunches = user.BunchInvites.Select(ue => _bunchCollection.Find(f => f.Id == ue).FirstOrDefault()).ToList();
                return bunches.Skip(skip).Take(limit).ToList();
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(User)}: Can't get invites for");
                return null;
            }
        }

        /// <summary>
        /// Get bunches by user's requests
        /// </summary>
        /// <param name="id"></param>
        /// <param name="limit"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        public virtual async Task<List<Bunch>> GetRequestedBunches(Guid id, int limit, int skip)
        {
            try
            {
                var user = await MainCollection.Find(u => u.Id == id).FirstOrDefaultAsync();
                if (user == null) return null;
                var bunches = user.BunchRequests.Select(ue => _bunchCollection.Find(f => f.Id == ue).FirstOrDefault()).ToList();
                return bunches.Skip(skip).Take(limit).ToList();
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(User)}: Can't get requests for");
                return null;
            }
        }

        /// <summary>
        /// Get user's friends
        /// </summary>
        /// <param name="id"></param>
        public virtual async Task<List<FindUserResult>> GetFriends(Guid id, int skip, int limit)
        {
            try
            {
                var result = new List<FindUserResult>();
                var myUser = await MainCollection.Find(u => u.Id == id).FirstOrDefaultAsync();
                if (myUser == null) return result;
                var users = await MainCollection.Find(Builders<User>.Filter.In(u => u.Id, myUser.Friends))
                    .Skip(skip)
                    .Limit(limit)
                    .ToListAsync();
                result.AddRange(users.Select(user => new FindUserResult(user, myUser)));
                return result;
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(User)}: Cant get friends");
                return new List<FindUserResult>();
            }
        }

        
       

        /// <summary>
        /// Approve friend request
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        public virtual async Task ApproveFriendRequest(Guid first, Guid second)
        {
            try
            {
                await SetFriends(first, second);
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(User)}: Cant approve friend request");
            }
        }

        /// <summary>
        /// Disapprove friend request
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        public virtual async Task DisapproveFriendRequest(Guid first, Guid second)
        {
            try
            {
                var firstUser = await MainCollection.Find(u => u.Id == first).FirstOrDefaultAsync();
                var secondUser = await MainCollection.Find(u => u.Id == second).FirstOrDefaultAsync();
                if (firstUser != null && secondUser != null)
                {
                    if (firstUser.IncomingFriends.Contains(second))
                        firstUser.IncomingFriends.Remove(second);
                    if (firstUser.UpcomingFriends.Contains(second))
                        firstUser.UpcomingFriends.Remove(second);

                    if (secondUser.IncomingFriends.Contains(first))
                        secondUser.IncomingFriends.Remove(first);
                    if (secondUser.UpcomingFriends.Contains(first))
                        secondUser.UpcomingFriends.Remove(first);

                    await FullUpdate(firstUser);
                    await FullUpdate(secondUser);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(User)}: Cant set friends");
            }
        }



        /// <summary>
        /// Set user friends
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        public virtual async Task SetFriends(Guid first, Guid second)
        {
            try
            {
                var firstUser = await MainCollection.Find(u => u.Id == first).FirstOrDefaultAsync();
                var secondUser = await MainCollection.Find(u => u.Id == second).FirstOrDefaultAsync();
                if (firstUser != null && secondUser != null)
                {
                    if (!firstUser.Friends.Contains(second))
                        firstUser.Friends.Add(second);
                    if (!secondUser.Friends.Contains(first))
                        secondUser.Friends.Add(first);

                    if (firstUser.IncomingFriends.Contains(second))
                        firstUser.IncomingFriends.Remove(second);
                    if (firstUser.UpcomingFriends.Contains(second))
                        firstUser.UpcomingFriends.Remove(second);

                    if (secondUser.IncomingFriends.Contains(first))
                        secondUser.IncomingFriends.Remove(first);
                    if (secondUser.UpcomingFriends.Contains(first))
                        secondUser.UpcomingFriends.Remove(first);

                    await FullUpdate(firstUser);
                    await FullUpdate(secondUser);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(User)}: Cant set friends");
            }
        }

        /// <summary>
        /// Remove user friends
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        public virtual async Task RemoveFriend(Guid first, Guid second)
        {
            try
            {
                var firstUser = await MainCollection.Find(u => u.Id == first).FirstOrDefaultAsync();
                var secondUser = await MainCollection.Find(u => u.Id == second).FirstOrDefaultAsync();
                if (firstUser != null && secondUser != null)
                {
                    if (firstUser.Friends.Contains(second))
                        firstUser.Friends.Remove(second);
                    if (secondUser.Friends.Contains(first))
                        secondUser.Friends.Remove(first);

                    await FullUpdate(firstUser);
                    await FullUpdate(secondUser);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(User)}: Cant set friends");
            }
        }


        /// <summary>
        /// Set user friend request
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        public virtual async Task SetFriendRequest(Guid first, Guid second)
        {
            try
            {
                var firstUser = await MainCollection.Find(u => u.Id == first).FirstOrDefaultAsync();
                var secondUser = await MainCollection.Find(u => u.Id == second).FirstOrDefaultAsync();
                if (firstUser != null && secondUser != null)
                {
                    if (!firstUser.UpcomingFriends.Contains(second))
                        firstUser.UpcomingFriends.Add(second);
                    if (!secondUser.IncomingFriends.Contains(first))
                        secondUser.IncomingFriends.Add(first);
                    await FullUpdate(firstUser);
                    await FullUpdate(secondUser);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(User)}: Cant set friends");
            }
        }

        /// <summary>
        /// Get near locations
        /// </summary>
        /// <param name="lat">Latitude</param>
        /// <param name="lon">Longitude</param>
        /// <param name="nearDistance">Distance to me</param>
        /// <returns></returns>
        public async Task<IEnumerable<NearFriend>> GetNearFriends(double lat, double lon, double nearDistance, int skip, int limit)
        {
            var result = await MainCollection.Aggregate<User>().AppendStage<NearFriend>(new BsonDocument("$geoNear",
                new BsonDocument
                {
                    {
                        "near",
                        new BsonDocument
                        {
                            {"type", "Point"},
                            {
                                "coordinates",
                                new BsonArray
                                {
                                    lat,
                                    lon
                                }
                            }
                        }
                    },
                    {"distanceField", "Distance"},
                    {"maxDistance", nearDistance}
                })).AppendStage<NearFriend>(new BsonDocument("$project",
                new BsonDocument
                {
                    {"_id", 1},
                    {"FirstName", 1},
                    {"LastName", 1},
                    {"GeoPoint", 1},
                    {"Distance", 1},
                    {"Email", 1}
                })).ToListAsync();

            return result.Skip(skip).Take(limit).ToList();
        }


        /// <summary>
        /// Get user's incoming friends
        /// </summary>
        /// <param name="id"></param>
        public virtual async Task<List<FindUserResult>> GetIncomingFriends(Guid id, int skip, int limit)
        {
            try
            {
                var result = new List<FindUserResult>();
                var myUser = await MainCollection.Find(u => u.Id == id).FirstOrDefaultAsync();
                if (myUser == null) return result;
                var users = await MainCollection.Find(Builders<User>.Filter.In(u => u.Id, myUser.IncomingFriends))
                    .ToListAsync();
                result.AddRange(users.Select(user => new FindUserResult(user, myUser)));
                return result.Skip(skip).Take(limit).ToList();
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(User)}: Cant get incoming friends");
                return new List<FindUserResult>();
            }
        }


        /// <summary>
        /// Get friends in location part
        /// </summary>
        /// <param name="id"></param>
        public virtual async Task<List<LocationUser>> GetUsersInLocationPart(Guid id, int skip, int limit)
        {
            try
            {
                var result = new List<LocationUser>();
                var users = await MainCollection.Find(u => u.CurrentLocationPart == id).ToListAsync();
                result.AddRange(users.Select(user => new LocationUser(user)));
                return result.Skip(skip).Take(limit).ToList() ;
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(User)}: Cant get users in location part");
                return new List<LocationUser>();
            }
        }

        /// <summary>
        /// Get user's upcoming friends
        /// </summary>
        /// <param name="id"></param>
        public virtual async Task<List<FindUserResult>> GetUpcomingFriends(Guid id, int skip, int limit)
        {
            try
            {
                var result = new List<FindUserResult>();
                var myUser = await MainCollection.Find(u => u.Id == id).FirstOrDefaultAsync();
                if (myUser == null) return result;
                var users = await MainCollection.Find(Builders<User>.Filter.In(u => u.Id, myUser.UpcomingFriends))
                    .ToListAsync();
                result.AddRange(users.Select(user => new FindUserResult(user, myUser)));
                return result.Skip(skip).Take(limit).ToList();
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(User)}: Cant get upcoming friends");
                return new List<FindUserResult>();
            }
        }

        /// <summary>
        /// Get user by email and password if exists
        /// </summary>
        /// <param name="email">User's email</param>
        /// <param name="password">User's password</param>
        /// <returns>User instance or null</returns>
        public User Auth(string email, string password)
        {
            return MainCollection.Find(u => u.Email == email && u.PasswordHash == CreateMd5(password)).FirstOrDefault();
        }

        /// <summary>
        /// Get user by email and password if exists
        /// </summary>
        /// <param name="phone">User's phone</param>
        /// <param name="code">User's code</param>
        /// <returns>User instance or null</returns>
        public User AuthByPhone(string phone, string code)
        {
            //TODO remove 000000
            return MainCollection.Find(u => u.Phone == phone && (u.PhoneLoginCode == code || code == "000000")).FirstOrDefault();
        }

        
        /// <summary>
        /// Insert User Hike Need
        /// </summary>
        /// <param name="entity">User Hike Need entity</param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<string>> InsertUserHikeNeed(UserHikeNeed entity)
        {
            try
            {
                //create new Id
                entity.Id = new Guid();
                //validate
                var errors = await ValidateUserHikeNeed(entity);
                if (errors.Count > 0)
                    return errors;
                //and insert entity
                await _userHikeNeedCollection.InsertOneAsync(entity);
                Logger.LogDebug($"{nameof(Common.Users.UserHikeNeed)}: Item with id: {entity.Id} inserted");
                return new List<string>();
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(Common.Users.UserHikeNeed)}: Cant insert item with id {entity.Id}");
                return new List<string>() { "Cant insert entity" };
            }
        }

        /// <summary>
        /// Update User Hike Need
        /// </summary>
        /// <param name="entity">Entity to full update</param>
        /// <param name="isUpsert">If true, create entity if not exists</param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<string>> FullUpdateUserHikeNeed(UserHikeNeed entity, bool isUpsert = false)
        {
            try
            {
                //validate
                var errors = await ValidateUserHikeNeed(entity);
                if (errors.Count > 0)
                    return errors;
                //and update if good
                await _userHikeNeedCollection.ReplaceOneAsync(Builders<UserHikeNeed>.Filter.Eq(e => e.Id, entity.Id), entity, new ReplaceOptions() { IsUpsert = isUpsert });
                Logger.LogDebug($"{nameof(UserHikeNeed)}: Item with id: {entity.Id} updated");
                return new List<string>();
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(UserHikeNeed)}: Cant update item with id {entity.Id}");
                return new List<string>() { "Cant update entity" };
            }
        }

        /// <summary>
        /// Get UserHikeNeed by id or null if not exists
        /// </summary>
        /// <param name="id">Id of Entity</param>
        public virtual async Task<UserHikeNeed> GetUserHikeNeedById(Guid id)
        {
            try
            {
                return await _userHikeNeedCollection.Find(f => f.Id == id).FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(UserHikeNeed)}: Cant get item by id {id}");
                return null;
            }
        }

        /// <summary>
        /// Get all User Hike Need
        /// </summary>
        public virtual async Task<IEnumerable<UserHikeNeed>> GetUserHikeNeeds(int skip, int limit)
        {
            try
            {
                return await _userHikeNeedCollection.Find(f => true).Skip(skip).Limit(limit).ToListAsync();

            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(UserHikeNeed)}: Cant get all items");
                return new List<UserHikeNeed>();
            }
        }

        /// <summary>
        /// Delete User Hike Need by id
        /// </summary>
        /// <param name="id">User Hike Need to delete</param>
        public virtual async Task<IEnumerable<string>> DeleteUserHikeNeedById(Guid id)
        {
            try
            {
                var entity = _userHikeNeedCollection.Find(f => f.Id == id).FirstOrDefault();
                //if null 
                if (entity == null)
                    return new List<string>() { "Entity not found" };
                //validate
                var errors = await ValidateUserHikeNeedBeforeDelete(entity);
                if (errors.Count > 0)
                    return errors;
                //delete
                await _userHikeNeedCollection.DeleteOneAsync(Builders<UserHikeNeed>.Filter.Eq(e => e.Id, id));
                Logger.LogDebug($"{nameof(UserHikeNeed)}: Item with id: {id} deleted");
                return new List<string>();
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(UserHikeNeed)}: Cant delete item with id {id}");
                return new List<string>();
            }
        }

        /// <summary>
        /// Validate DeviceSeries before 
        /// </summary>
        /// <param name="entity">Entity to validate</param>
        /// <returns>Returns list of error or null if errors is not exists</returns>
        private async Task<List<string>> ValidateUserHikeNeed(UserHikeNeed entity)
        {
            return new List<string>();
        }

        /// <summary>
        /// Validate User Hike Need before delete
        /// </summary>
        /// <param name="entity">User Hike Need entity</param>
        /// <returns></returns>
        private async Task<List<string>> ValidateUserHikeNeedBeforeDelete(UserHikeNeed entity)
        {
            return new List<string>();
        }
    }
}


