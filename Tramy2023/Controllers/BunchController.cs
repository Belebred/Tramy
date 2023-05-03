using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;
using Tramy.Backend.Data.DBServices;
using Tramy.Common.Chats;
using Tramy.Common.Users;
using System.Net;
using Microsoft.AspNetCore.Authorization;

namespace Tramy.Backend.Controllers
{
    /// <summary>
    /// Controller to manage bunches
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class BunchController : ControllerBase
    {
        private readonly BunchService _service;

        private readonly ChatService _chatService;


        private ILogger<BunchService> _logger;

        private readonly SystemEventService _systemEventService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="service">MongoDB service from Startup</param>
        /// <param name="chatService">MongoDB chat service</param>
        /// <param name="systemEventService">MongoDB service of system events from Startup</param>
        /// <param name="logger">Logger from Startup</param>

        public BunchController(BunchService service, ChatService chatService, SystemEventService systemEventService, ILogger<BunchService> logger)

        {
            _service = service;
            _logger = logger;
            _systemEventService = systemEventService;

            _chatService = chatService;

        }

       
        /// <summary>
        /// Insert new Bunch
        /// </summary>
        /// <param name="entity">Bunch to insert</param>
        [HttpPost]
        [Authorize]
        [SwaggerOperation(OperationId = "InsertBunch")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Post(Bunch entity)
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
        /// Get Bunch by Id
        /// </summary>
        /// <returns>Bunch by id</returns>
        [HttpGet("{id}")]
        [Authorize]
        [SwaggerOperation(OperationId = "GetBunchById")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]


        public async Task<Bunch> Get( Guid id)
        {
            return await _service.GetById(id);
        }

        /// <summary>
        /// Get all bunches
        /// </summary>
        /// <returns>All Bunches</returns>
        [HttpGet("{limit}/{skip}")]
        [Authorize]
        [SwaggerOperation(OperationId = "GetAllBunches")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]

        public async Task<IEnumerable<Bunch>> Get(int limit, int skip)
        {
            return await _service.Get(limit, skip);
        }

        /// <summary>
        /// Update Bunch
        /// </summary>
        /// <param name="entity">Bunch to update</param>
        [HttpPut]
        [Authorize]
        [SwaggerOperation(OperationId = "UpdateBunch")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]

        public async Task<IActionResult> Put(Bunch entity)
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
        /// Find bunch by its name
        /// </summary>
        /// <param name="name">Search string</param>
        /// <returns></returns>
        [HttpGet("FindByName/{name}")]
        [Authorize]
        [SwaggerOperation(OperationId = "GetBunchByName")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]


        public async Task<IEnumerable<Bunch>> FindBunchByName(string name)
        {
            return await _service.FindBunchByName(name);
        }

        

        /// <summary>
        /// Delete Bunch
        /// </summary>
        /// <param name="id">Bunch to delete</param>
        [HttpDelete("{id}")]
        [Authorize]
        [SwaggerOperation(OperationId = "DeleteBunchById")]
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
            _systemEventService.LogDeleteEntity(id, typeof(Bunch), Request.HttpContext.Connection.RemoteIpAddress?.ToString());
            return Ok();
        }

        
        /// <summary>
        /// Send from the bunch to the user
        /// </summary>
        /// <param name="bunchId">Id of the bunch</param>
        /// <param name="userId">Id of the user</param>
        /// <returns>Ok</returns>
        [HttpPost("SetInvite/{bunchId}/{userId}")]
        [Authorize]
        [SwaggerOperation(OperationId = "InviteUserToBunch")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]


        public async Task<IActionResult> SetInvite(Guid bunchId, Guid userId)
        {
            await _service.InviteToBunch(bunchId, userId);
            return Ok();
        }

        /// <summary>
        /// Accept invite from the bunch
        /// </summary>
        /// <param name="userId">Id of the user</param>
        /// <param name="bunchId">Id of the bunch</param>
        /// <returns></returns>
        [HttpPost("AcceptInviteToBunch/{userId}/{bunchId}")]
        [Authorize]
        [SwaggerOperation(OperationId = "AcceptInviteToBunch")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AcceptBunchInvite(Guid userId, Guid bunchId)
        {
            await _service.ApproveBunchInvite(bunchId, userId);
            return Ok();
        }

        /// <summary>
        /// Decline invite rom the bunch
        /// </summary>
        /// <param name="userId">Id of the user</param>
        /// <param name="bunchId">Id of the bunch</param>
        /// <returns></returns>
        [HttpPost("DeclineInviteToBunch/{userId}/{bunchId}")]
        [Authorize]
        [SwaggerOperation(OperationId = "DeclineInviteToBunch")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> DeclineBunchInvite(Guid userId, Guid bunchId)
        {
            await _service.DisapproveBunchInvite(bunchId, userId);
            return Ok();
        }

        /// <summary>
        /// Send request from user to the bunch
        /// </summary>
        /// <param name="userId">Id of the user</param>
        /// <param name="bunchId">Id of the bunch</param>
        /// <returns>Ok</returns>
        [HttpPost("SetBunchRequest/{userId}/{bunchId}")]
        [Authorize]
        [SwaggerOperation(OperationId = "SetRequestToBunch")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]

        public async Task<IActionResult> SetBunchRequest(Guid userId, Guid bunchId)
        {
            await _service.SetBunchRequest(bunchId, userId);
            return Ok();
        }

        /// <summary>
        /// Accept the user's request
        /// </summary>
        /// <param name="bunchId">Id of the bunch</param>
        /// <param name="userId">Id of the user</param>]
        /// <returns></returns>
        [HttpPost("AcceptRequest/{bunchId}/{userId}")]
        [Authorize]
        [SwaggerOperation(OperationId = "AcceptRequestFromUser")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]


        public async Task<IActionResult> AcceptUserRequset(Guid bunchId, Guid userId)
        {
            await _service.ApproveRequest(bunchId, userId);
            return Ok();
        }

        /// <summary>
        /// Decline the user's request
        /// </summary>
        /// <param name="bunchId">Id of the bunch</param>
        /// <param name="userId">Id of the user</param>]
        /// <returns></returns>
        [HttpPost("DeclineRequest/{bunchId}/{userId}")]
        [Authorize]
        [SwaggerOperation(OperationId = "DeclineRequestFromUser")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]


        public async Task<IActionResult> DeclineUserRequset(Guid bunchId, Guid userId)
        {
            await _service.DisapproveRequest(bunchId, userId);
            return Ok();
        }


        /// <summary>
        /// Get all bunches by user
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns></returns>
        [HttpGet("GetBunches/{id}/{skip}/{limit}")]
        [Authorize]
        [SwaggerOperation(OperationId = "GetAllBunchesByUser")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]

        public async Task<IEnumerable<Bunch>> GetBunches(Guid id, int skip, int limit)
        {
            return await _service.GetBunches(id, skip, limit);
        }

        /// <summary>
        /// Leave Bunch
        /// </summary>
        /// <param name="userId">Id of the user</param>
        /// <param name="bunchId">Id of the bunch</param>
        /// <returns></returns>
        [HttpPost("LeaveBunch/{userId}/{bunchId}")]
        [Authorize]
        [SwaggerOperation(OperationId = "LeaveBunch")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> LeaveBunch(Guid userId, Guid bunchId)
        {
            await _service.LeaveBunch(userId, bunchId);
            return Ok();
        }

        /// <summary>
        /// Get all users from the bunch
        /// </summary>
        /// <param name="id">Bunch id</param>
        /// <returns>List of users</returns>
        [HttpGet("GetUsers/{id}/{skip}/{limit}")]
        [Authorize]
        [SwaggerOperation(OperationId = "GetAllUsersInBunch")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]


        public async Task<IEnumerable<User>> GetUsers(Guid id, int skip, int limit)
        {
            return await _service.GetUsers(id, skip, limit);
        }

        /// <summary>
        /// Find bunches by interests
        /// </summary>
        /// <param name="city">City</param>
        /// <param name="interests">List of interests</param>
        [HttpPost("FindByInterests/{city}")]
        [Authorize]
        [SwaggerOperation(OperationId = "GetBunchByInterests")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        public async Task<IEnumerable<Bunch>> GetBunchesByInterests(string city, List<string> interests)
        {
            return await _service.FindBunchByInterests(interests, city);
        }

    }
}
