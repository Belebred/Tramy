using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tramy.Backend.Data.DBServices;

namespace Tramy.Backend.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : Controller
    {

        private readonly ChatService _service;
        /// <summary>
        /// Logger of controller
        /// </summary>
        private readonly ILogger<ChatService> _logger;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="service">MongoDB service from Startup</param>
        /// <param name="logger">Logger from Startup</param>
        public ChatController(ChatService service,  ILogger<ChatService> logger)
        {
            //save services and logger
            _service = service;
            _logger = logger;
        }
    }
}
