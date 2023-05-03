using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;
using Tramy.Backend.Data.DBServices;
using Tramy.Common.Locations;
using System.Net;

namespace Tramy.Backend.Controllers
{
    /// <summary>
    /// Controller for manage Locations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class LocationController : Controller
    {
        /// <summary>
        /// MongoDB service
        /// </summary>
        private readonly LocationService _service;

        private readonly SystemEventService _systemEventService;

        /// <summary>
        /// Logger of controller
        /// </summary>
        private readonly ILogger<LocationService> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="service">MongoDB service from Startup</param>
        /// <param name="systemEventService">MongoDB service of system events from Startup</param>
        /// <param name="logger">Logger from Startup</param>
        public LocationController(LocationService service, SystemEventService systemEventService, ILogger<LocationService> logger)
        {
            //save services and logger
            _service = service;
            _systemEventService = systemEventService;
            _logger = logger;
        }

        /// <summary>
        /// Insert new location
        /// </summary>
        /// <param name="entity">Location to insert</param>
        [HttpPost]
        [Authorize]
        [SwaggerOperation(OperationId = "InsertLocation")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]

        public async Task<IActionResult> Post(Location entity)
        {
            //validate and try to insert
            var errors = await _service.Insert(entity);
            //if errors - send errors
            if (errors.Any()) return BadRequest(errors);

            //else - log about
            _systemEventService.LogInsertEntity(entity, Request.HttpContext.Connection.RemoteIpAddress?.ToString());
            return Ok();
        }

        /// <summary>
        /// Get Location by Id
        /// </summary>
        /// <returns>Location by id</returns>
        [HttpGet("{id}")]
        [Authorize]
        [SwaggerOperation(OperationId = "GetLocationById")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]


        public async Task<Location> Get( Guid id)
        {
            return await _service.GetById(id);
        }


        /// <summary>
        /// Get all locations
        /// </summary>
        /// <returns>All locations</returns>
        [HttpGet("{limit}/{skip}")]
        [Authorize]
        [SwaggerOperation(OperationId = "GetAllLocations")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]


        public async Task<IEnumerable<Location>> Get(int limit, int skip)
        {
            return await _service.Get(limit, skip);
        }

        /// <summary>
        /// Get all locations near coords
        /// </summary>
        /// <returns>All locations</returns>
        [HttpGet("GetNear/{lat}/{lon}/{dist}/{skip}/{limit}")]
        [Authorize]
        [SwaggerOperation(OperationId = "GetAllNearLocations")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]


        public async Task<IEnumerable<NearLocation>> GetNear(double lat,  double lon,  double dist, int skip, int limit)
        {
            return await _service.GetNear(lat,lon, dist, skip, limit);
        }

        /// <summary>
        /// Update Location
        /// </summary>
        /// <param name="entity">Location to update</param>
        [HttpPut]
        [Authorize]
        [SwaggerOperation(OperationId = "UpdateLocation")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]


        public async Task<IActionResult> Put(Location entity)
        {
            //validate and try to update
            var errors = await _service.FullUpdate(entity);
            //if errors - send errors
            if (errors.Any()) return BadRequest(errors);

            //else - log about
            _systemEventService.LogChangeEntity(entity, Request.HttpContext.Connection.RemoteIpAddress?.ToString());
            return Ok();
        }

        /// <summary>
        /// Delete location
        /// </summary>
        /// <param name="id">Location to delete</param>
        [HttpDelete("{id}")]
        [Authorize]
        [SwaggerOperation(OperationId = "DeleteLocationById")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]



        public async Task<IActionResult> Delete( Guid id)
        {
            //validate and try to delete
            var errors = await _service.DeleteById(id);
            //if errors - send errors
            if (errors.Any()) return BadRequest(errors);

            //else - log about
            _systemEventService.LogDeleteEntity(id, typeof(Location), Request.HttpContext.Connection.RemoteIpAddress?.ToString());
            return Ok();
        }
    }
}
