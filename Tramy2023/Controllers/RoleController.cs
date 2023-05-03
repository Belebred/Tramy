using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using Tramy.Backend.Data.DBServices;
using Tramy.Common.Users;

namespace Tramy.Backend.Controllers
{
    /// <summary>
    /// Controller for manage Roles
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController:Controller
    {
        /// <summary>
        /// MongoDB service
        /// </summary>
        private readonly RoleService _service;

        private readonly SystemEventService _systemEventService;

        /// <summary>
        /// Logger of controller
        /// </summary>
        private readonly ILogger<RoleService> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="service">MongoDB service from Startup</param>
        /// <param name="systemEventService">MongoDB service of system events from Startup</param>
        /// <param name="logger">Logger from Startup</param>
        public RoleController(RoleService service, SystemEventService systemEventService, ILogger<RoleService> logger)
        {
            //save services and logger
            _service = service;
            _systemEventService = systemEventService;
            _logger = logger;
        }

        /// <summary>
        /// Insert new role
        /// </summary>
        /// <param name="entity">Role to insert</param>
        [HttpPost]
        [Authorize]
        [SwaggerOperation(OperationId = "InsertRole")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]

        public async Task<IActionResult> Post(Role entity)
        {
            //validate and try to insert
            var errors = await _service.Insert(entity);
            //if errors - send errors
            if (errors.Any()) return BadRequest(errors);

            //else - log about
            _systemEventService.LogInsertEntity(entity,Request.HttpContext.Connection.RemoteIpAddress?.ToString());
            return Ok();
        }

        /// <summary>
        /// Update new role
        /// </summary>
        /// <param name="entity">Role to update</param>
        [HttpPut]
        [Authorize]
        [SwaggerOperation(OperationId = "UpdateRole")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]

        public async Task<IActionResult> Put(Role entity)
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
        /// Delete new role
        /// </summary>
        /// <param name="id">Role to delete</param>
        [HttpDelete("{id}")]
        [Authorize]
        [SwaggerOperation(OperationId = "DeleteRoleById")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]


        public async Task<IActionResult> Delete(Guid id)
        {
            //validate and try to delete
            var errors = await _service.DeleteById(id);
            //if errors - send errors
            if (errors.Any()) return BadRequest(errors);

            //else - log about
            _systemEventService.LogDeleteEntity(id, typeof(Role), Request.HttpContext.Connection.RemoteIpAddress?.ToString());
            return Ok();
        }


        /// <summary>
        /// Get all roles
        /// </summary>
        /// <returns>All roles</returns>
        [HttpGet("{limit}/{skip}")]
        [Authorize]
        [SwaggerOperation(OperationId = "GetAllRoles")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]

        public async Task<IEnumerable<Role>> Get(int limit, int skip)
        {
            return await _service.Get(limit, skip);
        }

        /// <summary>
        /// Get role by Id
        /// </summary>
        /// <returns>All roles</returns>
        [HttpGet("{id}")]
        [Authorize]
        [SwaggerOperation(OperationId = "GetRoleById")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]


        public async Task<Role> Get( Guid id)
        {
            return await _service.GetById(id);
        }
    }
}
