using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Tramy.Backend.Helpers;
using Tramy.Common.Notification;

namespace Tramy.Backend.Data.DBServices
{
    /// <summary>
    /// Service to user notification
    /// </summary>
    public class NotificationService:BaseDbService<UserNotification>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public NotificationService(ServiceConfiguration configuration, ILogger<BaseDbService<UserNotification>> logger, BaseLogService logService) : base(configuration, logger, logService)
        {
        }
    }
}
