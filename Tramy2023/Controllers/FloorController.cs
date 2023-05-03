using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;
using Tramy.Backend.Data.DBServices;
using Tramy.Common.Organizations;
using Microsoft.AspNetCore.Authorization;
using System.Net;

namespace Tramy.Backend.Controllers
{
    /// <summary>
    /// Controller for manage Floors
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class FloorController : Controller
    {
        /// <summary>
        /// MongoDB service
        /// </summary>
        private readonly FloorService _service;
        private readonly LocationService _locationService;

        private readonly SystemEventService _systemEventService;

        /// <summary>
        /// Logger of controller
        /// </summary>
        private readonly ILogger<FloorService> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="service">MongoDB service from Startup</param>
        /// <param name="locationService">MongoDB service for locations</param>
        /// <param name="systemEventService">MongoDB service of system events from Startup</param>
        /// <param name="logger">Logger from Startup</param>
        public FloorController(FloorService service, LocationService locationService, SystemEventService systemEventService, ILogger<FloorService> logger)
        {
            //save services and logger
            _service = service;
            _locationService = locationService;
            _systemEventService = systemEventService;
            _logger = logger;
        }

        /// <summary>
        /// Insert new floor
        /// </summary>
        /// <param name="entity">Floor to insert</param>
        [HttpPost]
        [Authorize]
        [SwaggerOperation(OperationId = "InsertFloor")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]

        public async Task<IActionResult> Post(Floor entity)
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
        /// Update new floor
        /// </summary>
        /// <param name="entity">Floor to update</param>
        [HttpPut]
        [Authorize]
        [SwaggerOperation(OperationId = "UpdateFloor")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]

        public async Task<IActionResult> Put(Floor entity)
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
        /// Delete new floor
        /// </summary>
        /// <param name="id">Floor to delete</param>
        [HttpDelete("{id}")]
        [Authorize]
        [SwaggerOperation(OperationId = "DeleteFloorById")]
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
            _systemEventService.LogDeleteEntity(id, typeof(Floor), Request.HttpContext.Connection.RemoteIpAddress?.ToString());
            return Ok();
        }


        /// <summary>
        /// Get all floors
        /// </summary>
        /// <returns>All floors</returns>
        [HttpGet("{limit}/{skip}")]
        [Authorize]
        [SwaggerOperation(OperationId = "GetAllFloors")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]

        public async Task<IEnumerable<Floor>> Get(int limit, int skip)
        {
            return await _service.Get(limit, skip);
        }

        /// <summary>
        /// Get floor by Id
        /// </summary>
        /// <returns>Floor by id</returns>
        [HttpGet("{id}")]
        [Authorize]
        [SwaggerOperation(OperationId = "GetFloorById")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]


        public async Task<Floor> Get( Guid id)
        {
            return await _service.GetById(id);
        }

        /// <summary>
        /// Insert organizations in the floor
        /// </summary>
        /// <param name="id">Floor id to insert into</param>
        /// <param name="loc_id">Location's id being inserted</param>
        [HttpPost("{id}")]
        [Authorize]
        [SwaggerOperation(OperationId = "InsertOrganizationInFloor")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]


        public async Task<IActionResult> AddOrg( Guid id, IEnumerable<Guid> loc_id)
        {
            var floor = _service.GetById(id);
            if (floor == null) return BadRequest("No floor with such id");

            floor.Result.Locations = loc_id;
            await _service.FullUpdate(floor.Result);
            return Ok();
        }
    }
}
