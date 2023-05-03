using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Tramy.Backend.Data.DBServices;
using Tramy.Common.Organizations;

namespace Tramy.Backend.Controllers
{
    /// <summary>
    /// Controller for manage Buildings
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class BuildingController : Controller
    {
        /// <summary>
        /// MongoDB service
        /// </summary>
        private readonly BuildingService _service;
        private readonly FloorService _floorService;

        private readonly SystemEventService _systemEventService;

        /// <summary>
        /// Logger of controller
        /// </summary>
        private readonly ILogger<BuildingService> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="service">MongoDB service from Startup</param>
        /// <param name="systemEventService">MongoDB service of system events from Startup</param>
        /// <param name="logger">Logger from Startup</param>
        public BuildingController(BuildingService service, FloorService floorService, SystemEventService systemEventService, ILogger<BuildingService> logger)
        {
            //save services and logger
            _service = service;
            _floorService = floorService;
            _systemEventService = systemEventService;
            _logger = logger;
        }

        /// <summary>
        /// Insert new building
        /// </summary>
        /// <param name="entity">Building to insert</param>
        [HttpPost]
        [Authorize]
        [SwaggerOperation(OperationId = "InsertBuilding")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]

        public async Task<IActionResult> Post(Building entity)
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
        /// Get Building by Id
        /// </summary>
        /// <returns>Building by id</returns>
        [HttpGet("{id}")]
        [Authorize]
        [SwaggerOperation(OperationId = "GetBuildingById")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]

        public async Task<Building> Get( Guid id)
        {
            return await _service.GetById(id);
        }

        /// <summary>
        /// Get all buildings
        /// </summary>
        /// <returns>All Buildings</returns>
        [HttpGet("{limit}/{skip}")]
        [Authorize]
        [SwaggerOperation(OperationId = "GetAllBuildings")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]

        public async Task<IEnumerable<Building>> Get(int limit, int skip)
        {
            return await _service.Get(limit, skip);
        }

        /// <summary>
        /// Update Building
        /// </summary>
        /// <param name="entity">Building to update</param>
        [HttpPut]
        [Authorize]
        [SwaggerOperation(OperationId = "UpdateBuilding")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]

        public async Task<IActionResult> Put(Building entity)
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
        /// Delete Building
        /// </summary>
        /// <param name="id">Building to delete</param>
        [HttpDelete("{id}")]
        [Authorize]
        [SwaggerOperation(OperationId = "DeleteBuildingById")]
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
            _systemEventService.LogDeleteEntity(id, typeof(Building), Request.HttpContext.Connection.RemoteIpAddress?.ToString());
            return Ok();
        }

        /// <summary>
        /// Insert Floors into Building
        /// </summary>
        /// <param name="id">Building id</param>
        /// <param name="floor_id">Floor id</param>
        [HttpPut("{id}")]
        [Authorize]
        [SwaggerOperation(OperationId = "AddFloorsToBuilding")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]

        //[Authorize}
        public async Task<IActionResult> AddFloors( Guid id, IEnumerable<Guid> floor_id)
        {
            var building = _service.GetById(id);
            // check if Building exists
            if (building == null) return BadRequest("No building with such id");

            building.Result.Floors = floor_id;
            await _service.FullUpdate(building.Result);
            return Ok();
        }

    }
}
