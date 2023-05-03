using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Tramy.Common.Chats;

namespace Tramy.Backend.Data.DBServices
{
    /// <summary>
    /// Chat service
    /// </summary>
    public class ChatService:BaseDbService<Chat>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public ChatService(IConfiguration configuration, ILogger<BaseDbService<Chat>> logger, BaseLogService logService) : base(configuration, logger, logService)
        {
        }
    }
}
