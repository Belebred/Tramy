using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;
using Tramy.Backend.Data.DBServices;
using Tramy.Common.UserEvent;
using Microsoft.AspNetCore.Authorization;
using System.Net;

namespace Tramy.Backend.Controllers
{
    /// <summary>
    /// Controller for userevent
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UserEventController : Controller
    {
        /// <summary>
        /// MongoDB service
        /// </summary>
        private readonly UserEventService _service;
        private readonly UserService _userService;

        private readonly SystemEventService _systemEventService;

        /// <summary>
        /// Logger of controller
        /// </summary>
        private readonly ILogger<UserEventService> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="service">MongoDB service from Startup</param>
        /// <param name="userService">MongoDB service to manage users</param>
        /// <param name="systemEventService">MongoDB service of system events from Startup</param>
        /// <param name="logger">Logger from Startup</param>
        public UserEventController(UserEventService service, UserService userService, SystemEventService systemEventService, ILogger<UserEventService> logger)
        {
            //save services and logger
            _service = service;
            _systemEventService = systemEventService;
            _logger = logger;
            _userService = userService;
        }

        /// <summary>
        /// Insert new UserEvent
        /// </summary>
        /// <param name="entity">UserEvent</param>
        [HttpPost]
        [Authorize]
        [SwaggerOperation(OperationId = "InsertUserEvent")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        //{Authorize]
        public async Task<IActionResult> Post(UserEvent entity)
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
        /// Get UserEvent by Id
        /// </summary>
        /// <returns>UserEvent by id</returns>
        [HttpGet("{id}")]
        [Authorize]
        [SwaggerOperation(OperationId = "GetUserEventById")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]


        public async Task<UserEvent> Get( Guid id)
        {
            return await _service.GetById(id);
        }

        /// <summary>
        /// Get UserEvent by its name
        /// </summary>
        /// <param name="name"></param>
        /// <returns>UserEvent</returns>
        [HttpGet("EventName/{name}")]
        //[Authorize]
        [SwaggerOperation(OperationId = "GetUserEventByName")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        public async Task<IEnumerable<UserEvent>> GetByName( string name)
        {
            return await _service.GetByName(name);
        }

        /// <summary>
        /// Get all UserEvents
        /// </summary>
        /// <returns>All UserEvents</returns>
        [HttpGet("{limit}/{skip}")]
        [Authorize]
        [SwaggerOperation(OperationId = "GetAllUserEvent")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]

        public async Task<IEnumerable<UserEvent>> Get(int limit, int skip)
        {
            return await _service.Get(limit, skip);
        }

        /// <summary>
        /// Update UserEvent
        /// </summary>
        /// <param name="entity">UserEvent to update</param>
        [HttpPut]
        [Authorize]
        [SwaggerOperation(OperationId = "UpdateUserEvent")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]

        public async Task<IActionResult> Put(UserEvent entity)
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
        /// Add User to the UserEvent
        /// </summary>
        /// <param name="userEventId">Id of UserEvent</param>
        /// <param name="userId">Id of users</param>
        [HttpPost("{userEventId}/{userId}")]
        [Authorize]
        [SwaggerOperation(OperationId = "AddUserToUserEvent")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]

        public async Task<IActionResult> AddPersonToUserEvent(Guid userEventId, Guid userId)
        {
            var userEvent = await _service.GetById(userEventId);
            var user = await _userService.GetById(userId);

            // check if user and event exist
            if (userEvent == null) return BadRequest("No Event with this id");
            if (user == null) return BadRequest("No user with this id");
            userEvent.Guests.Add(userId);
            return Ok();
        }


        /// <summary>
        /// Delete UserEvent
        /// </summary>
        /// <param name="id">UserEvent to delete</param>
        [HttpDelete("{id}")]
        [Authorize]
        [SwaggerOperation(OperationId = "DeleteUserEventById")]
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
            _systemEventService.LogDeleteEntity(id, typeof(UserEvent), Request.HttpContext.Connection.RemoteIpAddress?.ToString());
            return Ok();
        }

        /// <summary>
        /// Send request from user to the UserEvent
        /// </summary>
        /// <param name="userId">Id of the user</param>
        /// <param name="eventId">Id of the UserEvent</param>
        /// <returns>Ok</returns>
        [HttpPost("SetEventRequest/{userId}/{eventId}")]
        [Authorize]
        [SwaggerOperation(OperationId = "SetRequestToUserEvent")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]

        public async Task<IActionResult> SetEventRequest(Guid userId, Guid eventId)
        {
            await _service.SetUserEventRequest(eventId, userId);
            return Ok();
        }

        /// <summary>
        /// Accept the user's request
        /// </summary>
        /// <param name="eventId">Id of the bunch</param>
        /// <param name="userId">Id of the user</param>
        /// <returns></returns>
        [HttpPost("AcceptRequest/{eventId}/{userId}")]
        [Authorize]
        [SwaggerOperation(OperationId = "AcceptRequestToUserEvent")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]

        public async Task<IActionResult> AcceptUserRequset( Guid eventId,  Guid userId)
        {
            await _service.ApproveRequest(eventId, userId);
            return Ok();
        }

        /// <summary>
        /// Decline the user's request
        /// </summary>
        /// <param name="eventId">Id of the bunch</param>
        /// <param name="userId">Id of the user</param>
        /// <returns></returns>
        [HttpPost("DeclineRequest/{eventId}/{userId}")]
        [Authorize]
        [SwaggerOperation(OperationId = "AcceptRequestToUserEvent")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]

        public async Task<IActionResult> DeclineUserRequset(Guid eventId, Guid userId)
        {
            await _service.DisapproveRequest(eventId, userId);
            return Ok();
        }

        /// <summary>
        /// Get asll UserEvents by the user
        /// </summary>
        /// <param name="id">Id of the user</param>
        /// <returns></returns>
        [HttpGet("GetEvents/{id}/{skip}/{limit}")]
        [Authorize]
        [SwaggerOperation(OperationId = "GetAllUserEventsByUser")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]

        public async Task<IEnumerable<UserEvent>> GetUserEvents(Guid id, int skip, int limit)
        {
            return await _service.GetUserEvents(id, skip, limit);
        }

        /// <summary>
        /// Leave Userevent
        /// </summary>
        /// <param name="userId">Id of the user</param>
        /// <param name="eventId">Id of the event</param>
        /// <returns></returns>
        [HttpPost("LeaveEvent/{userId}/{eventId}")]
        [Authorize]
        [SwaggerOperation(OperationId = "LeaveUserEvent")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> LeaveEvent(Guid userId, Guid eventId)
        {
            await _service.LeaveUserEvent(userId, eventId);
            return Ok();
        }

        /// <summary>
        /// Send from the event to the user
        /// </summary>
        /// <param name="eventId">Id of the event</param>
        /// <param name="userId">Id of the user</param>
        /// <returns>Ok</returns>
        [HttpPost("SetInvite/{eventId}/{userId}")]
        [Authorize]
        [SwaggerOperation(OperationId = "InviteUserToUserEvent")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]

        public async Task<IActionResult> SetInvite(Guid eventId, Guid userId)
        {
            await _service.InviteToEvent(eventId, userId);
            return Ok();
        }

        /// <summary>
        /// Accept invite rom the bunch
        /// </summary>
        /// <param name="userId">Id of the user</param>
        /// <param name="eventId">Id of the event</param>
        /// <returns></returns>
        [HttpPost("AcceptInviteToEvent/{userId}/{eventId}")]
        [Authorize]
        [SwaggerOperation(OperationId = "AcceptInvite")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AcceptUserEventInvite(Guid userId, Guid eventId)
        {
            await _service.ApproveUserEventInvite(eventId, userId);
            return Ok();
        }

        /// <summary>
        /// Decline invite rom the bunch
        /// </summary>
        /// <param name="userId">Id of the user</param>
        /// <param name="eventId">Id of the event</param>
        /// <returns></returns>
        [HttpPost("DeclineInviteToEvent/{userId}/{eventId}")]
        [Authorize]
        [SwaggerOperation(OperationId = "DeclineInvite")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> DeclineUserEventInvite(Guid userId, Guid eventId)
        {
            await _service.DisapproveUserEventInvite(eventId, userId);
            return Ok();
        }

        /// <summary>
        /// Get list of Users from UserEvent requests
        /// </summary>
        /// <param name="userEventId">User Id</param>
        /// <param name="limit">How many records</param>
        /// <param name="skip">Skip from the beginning</param>
        /// <returns>List of requested Users</returns>
        [HttpGet("UserRequests/{userEventId}/{skip}/{limit}")]
        [Authorize]
        [SwaggerOperation(OperationId = "GetUserRequestsFromUserEvent")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        public async Task<List<Common.Users.User>> GetUserRequests(Guid userEventId, int limit = 10, int skip = 0)
        {
            return await _service.GetIncomingUsers(userEventId, limit, skip);
        }

        /// <summary>
        /// Get list of Users from UserEvent invites
        /// </summary>
        /// <param name="userEventId">User Id</param>
        /// <param name="limit">How many records</param>
        /// <param name="skip">Skip from the beginning</param>
        /// <returns>List of invited Users</returns>
        [HttpGet("UserInvites/{userEventId}/{skip}/{limit}")]
        [Authorize]
        [SwaggerOperation(OperationId = "GetUserInvitesFromUserEvent")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        public async Task<List<Common.Users.User>> GetUserInvites(Guid userEventId, int limit = 10, int skip = 0)
        {
            return await _service.GetInvitedUsers(userEventId, limit, skip);
        }
    }

}
