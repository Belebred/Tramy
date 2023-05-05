using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tramy.Backend.Helpers;
using Tramy.Common.Users;

namespace Tramy.Backend.Data.DBServices
{
    public class TrackerService : BaseDbService<Tracker>
    {
        private readonly IMongoCollection<Tracker> _trackerCollcetion;
        private readonly IMongoCollection<User> _userCollection;
        private UserService _userService;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration">Configuration must be link from Startup</param>
        /// <param name="logger">Logger must be link from Startup</param>
        public TrackerService(ServiceConfiguration configuration, ILogger<BaseDbService<Tracker>> logger, UserService userService, BaseLogService logService) : base(configuration, logger, logService)
        {
            _trackerCollcetion = MainCollection.Database.GetCollection<Tracker>(nameof(Tracker));
            _userCollection = MainCollection.Database.GetCollection<User>(nameof(User));
            _userService = userService;
        }

        /// <summary>
        /// Get all trackers of the user
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns></returns>
        public virtual async Task<List<Tracker>> GetAllTrackers(Guid id, int skip, int limit)
        {
            try
            {
                var user = await _userCollection.Find(u => u.Id == id).FirstOrDefaultAsync();

                if (user == null) return null;
                return user.Trackers.Select(t => GetById(t).Result).Skip(skip).Take(limit).ToList();

            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(User)}: Can't get trackers");
                return null;
            }
        }

        public virtual async Task AddPointToRoute (Guid trackId, DateTime date, double[] coords)
        {
            try
            {
                var track = await _trackerCollcetion.Find(t => t.Id == trackId).FirstOrDefaultAsync();

                if (track != null && coords != null)
                {
                    track.RoutePoints.Add(date, coords);
                }
                await FullUpdate(track);
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"{nameof(Tracker)}: Can't add RoutePoint");
            }
        }
        }
    }

