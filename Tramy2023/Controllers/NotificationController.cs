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
    /// Notification controller
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController:Controller
    {
        private readonly NotificationService _service;
        /// <summary>
        /// Logger of controller
        /// </summary>
        private readonly ILogger<NotificationService> _logger;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="service">MongoDB service from Startup</param>
        /// <param name="logger">Logger from Startup</param>
        public NotificationController(NotificationService service, ILogger<NotificationService> logger)
        {
            //save services and logger
            _service = service;
            _logger = logger;
        }
    }
}
