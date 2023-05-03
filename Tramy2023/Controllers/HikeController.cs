using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Tramy.Backend.Data.DBServices;
using Tramy.Common.Hike;
using Tramy.Common.Users;

namespace Tramy.Backend.Controllers
{
    /// <summary>
    /// Controller to manage Hike
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class HikeController : Controller
    {
        /// <summary>
        /// MongoDB service
        /// </summary>
        private readonly HikeService _service;

        private readonly SystemEventService _systemEventService;

        /// <summary>
        /// Logger of controller
        /// </summary>
        private readonly ILogger<HikeService> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="service">MongoDB service from Startup</param>
        /// <param name="systemEventService">MongoDB service of system events from Startup</param>
        /// <param name="logger">Logger from Startup</param>
        public HikeController(HikeService service, SystemEventService systemEventService, ILogger<HikeService> logger)
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
        [SwaggerOperation(OperationId = "InsertHike")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]

        public async Task<IActionResult> Post(Hike entity)
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
        /// Update new hike
        /// </summary>
        /// <param name="entity">Hike to update</param>
        [HttpPut]
        [Authorize]
        [SwaggerOperation(OperationId = "UpdateHike")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]

        public async Task<IActionResult> Put(Hike entity)
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
        /// Delete new hike
        /// </summary>
        /// <param name="id">Hike to delete</param>
        [HttpDelete("{id}")]
        [Authorize]
        [SwaggerOperation(OperationId = "DeleteHikeById")]
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
            _systemEventService.LogDeleteEntity(id, typeof(Hike), Request.HttpContext.Connection.RemoteIpAddress?.ToString());
            return Ok();
        }


        /// <summary>
        /// Get all Hikes
        /// </summary>
        /// <returns>All Hikes</returns>
        [HttpGet("{limit}/{skip}")]
        [Authorize]
        [SwaggerOperation(OperationId = "GetAllHikes")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]

        public async Task<IEnumerable<Hike>> Get(int limit, int skip)
        {
            return await _service.Get(limit, skip);
        }

        /// <summary>
        /// Get Hike by Id
        /// </summary>
        /// <returns>All roles</returns>
        [HttpGet("{id}")]
        [Authorize]
        [SwaggerOperation(OperationId = "GetHikeById")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
        public async Task<Hike> Get(Guid id)
        {
            return await _service.GetById(id);
        }

        /// <summary>
        /// Find Users for Hike by userHikeNeed
        /// </summary>
        /// <param name="hikeId">Id of the Hike</param>
        /// <returns>List of UserId</returns>
        [HttpPost("FindUsers/{hikeId}")]
        [Authorize]
        [SwaggerOperation(OperationId = "FindUsersForHike")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
        public async Task<List<Guid>> FindUsersForHike(Guid hikeId)
        {
            return await _service.FindUsers(hikeId);
        }

        /// <summary>
        /// Send invite to User
        /// </summary>
        /// <param name="userId">Id of user</param>
        /// <param name="hikeId">id of Hike</param>
        /// <returns></returns>
        [HttpPost("SendInvite/{userId}/{hikeId}")]
        [Authorize]
        [SwaggerOperation(OperationId = "SendInviteToUserFromHike")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> InviteUserToHike(Guid userId, Guid hikeId)
        {
            await _service.SendInvite(userId, hikeId);
            return Ok();
        }

        /// <summary>
        /// Send request to Hike
        /// </summary>
        /// <param name="userId">Id of user</param>
        /// <param name="hikeId">Id of hike</param>
        /// <returns></returns>
        [HttpPost("SendHikeRequest/{userId}/{hikeId}")]
        [Authorize]
        [SwaggerOperation(OperationId = "SendHikeRequest")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> SendHikeRequest(Guid userId, Guid hikeId)
        {
            await _service.SetHikeRequest(userId, hikeId);
            return Ok();
        }

        /// <summary>
        /// Decline request rom the user
        /// </summary>
        /// <param name="userId">Id of user</param>
        /// <param name="hikeId">Id of hike</param>
        /// <returns></returns>
        [HttpPost("DeclineRequest/{userId}/{hikeId}")]
        [Authorize]
        [SwaggerOperation(OperationId = "DeclineUserRequestForHike")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeclineUserRequest(Guid userId, Guid hikeId)
        {
            await _service.DeclineRequest(userId, hikeId);
            return Ok();
        }

        /// <summary>
        /// Accept request to hike from user
        /// </summary>
        /// <param name="userId">Id of user</param>
        /// <param name="hikeId">Id of hike</param>
        /// <returns></returns>
        [HttpPost("AcceptRequest/{userId}/{hikeId}")]
        [Authorize]
        [SwaggerOperation(OperationId = "AcceptRequestToHike")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
        public virtual async Task<IActionResult> AcceptUserRequest(Guid userId, Guid hikeId)
        {
            await _service.AcceptRequest(userId, hikeId);
            return Ok();
        }

        

        /// <summary>
        /// Decline Invite from hike
        /// </summary>
        /// <param name="userId">Id of user</param>
        /// <param name="hikeId">Id of hike</param>
        /// <returns></returns>
        [HttpPost("DeclineHikeInvite/{userId}/{hikeId}")]
        [Authorize]
        [SwaggerOperation(OperationId = "DeclineHikeInvite")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> DeclineHikeInvite(Guid userId, Guid hikeId)
        {
            await _service.DeclineInvite(userId, hikeId);
            return Ok();
        }

        /// <summary>
        /// Accept invite from hike
        /// </summary>
        /// <param name="userId">Id of user</param>
        /// <param name="hikeId">Id of hike</param>
        /// <returns></returns>
        [HttpPost("AcceptHikeInvite/{userId}/{hikeId}")]
        [Authorize]
        [SwaggerOperation(OperationId = "AcceptHikeInvite")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AcceptHikeInvite(Guid userId, Guid hikeId)
        {
            await _service.AcceptInvite(userId, hikeId);
            return Ok();
        }

        /// <summary>
        /// Insert new TravelKit
        /// </summary>
        /// <param name="entity">TravelKit to insert</param>
        [HttpPost("TravelKit")]
        [Authorize]
        [SwaggerOperation(OperationId = "InsertTravelKit")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]

        public async Task<IActionResult> InsertTravelKit(TravelKit entity)
        {
            //validate and try to insert
            var errors = await _service.InsertTravelKit(entity);
            //if errors - send errors
            if (errors.Any()) return BadRequest(errors);

            //else - log about
            _systemEventService.LogInsertEntity(entity, Request.HttpContext.Connection.RemoteIpAddress?.ToString());
            return Ok();
        }

        /// <summary>
        /// Update new TravelKit
        /// </summary>
        /// <param name="entity">TravelKit to update</param>
        [HttpPut("UpdateTravelKit")]
        [Authorize]
        [SwaggerOperation(OperationId = "UpdateTravelKit")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]

        public async Task<IActionResult> UpdateTravelKit(TravelKit entity)
        {
            //validate and try to update
            var errors = await _service.FullUpdateTravelKit(entity);
            //if errors - send errors
            if (errors.Any()) return BadRequest(errors);

            //else - log about
            _systemEventService.LogChangeEntity(entity, Request.HttpContext.Connection.RemoteIpAddress?.ToString());
            return Ok();
        }

        /// <summary>
        /// Delete new TravelKit
        /// </summary>
        /// <param name="id">TravelKit to delete</param>
        [HttpDelete("TravelKit/{id}")]
        [Authorize]
        [SwaggerOperation(OperationId = "DeleteTravelKitById")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]


        public async Task<IActionResult> DeleteTravelKit(Guid id)
        {
            //validate and try to delete
            var errors = await _service.DeleteTravelKitById(id);
            //if errors - send errors
            if (errors.Any()) return BadRequest(errors);

            //else - log about
            _systemEventService.LogDeleteEntity(id, typeof(TravelKit), Request.HttpContext.Connection.RemoteIpAddress?.ToString());
            return Ok();
        }


        /// <summary>
        /// Get all TravelKits
        /// </summary>
        /// <returns>All TravelKits</returns>
        [HttpGet("TravelKit/{limit}/{skip}")]
        [Authorize]
        [SwaggerOperation(OperationId = "GetAllTravelKits")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]

        public async Task<IEnumerable<TravelKit>> GetAllTravelKIts(int limit, int skip)
        {
            return await _service.GetAllTravelKit(limit, skip);
        }

        /// <summary>
        /// Get TravelKit by Id
        /// </summary>
        /// <returns>Travel Kit</returns>
        [HttpGet("TravelKit/{id}")]
        [Authorize]
        [SwaggerOperation(OperationId = "GetTravelKitById")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
        public async Task<TravelKit> GetTravelKitById(Guid id)
        {
            return await _service.GetTravelKitById(id);
        }

        /// <summary>
        /// Insert new TravelKitItem
        /// </summary>
        /// <param name="entity">TravelKitItem to insert</param>
        [HttpPost("TravelKitItem")]
        [Authorize]
        [SwaggerOperation(OperationId = "InsertTravelKitItem")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]

        public async Task<IActionResult> InsertTravelKitItem(TravelKitItem entity)
        {
            //validate and try to insert
            var errors = await _service.InsertTravelKitItem(entity);
            //if errors - send errors
            if (errors.Any()) return BadRequest(errors);

            //else - log about
            _systemEventService.LogInsertEntity(entity, Request.HttpContext.Connection.RemoteIpAddress?.ToString());
            return Ok();
        }

        /// <summary>
        /// Update new TravelKitItem
        /// </summary>
        /// <param name="entity">TravelKitItem to update</param>
        [HttpPut("TravelKitItem")]
        [Authorize]
        [SwaggerOperation(OperationId = "UpdateTravelKitItem")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]

        public async Task<IActionResult> UpdateTravelKitItem(TravelKitItem entity)
        {
            //validate and try to update
            var errors = await _service.FullUpdateTravelKitItem(entity);
            //if errors - send errors
            if (errors.Any()) return BadRequest(errors);

            //else - log about
            _systemEventService.LogChangeEntity(entity, Request.HttpContext.Connection.RemoteIpAddress?.ToString());
            return Ok();
        }

        /// <summary>
        /// Delete new TravelKitItem
        /// </summary>
        /// <param name="id">TravelKitItem to delete</param>
        [HttpDelete("TravelKitItem/{id}")]
        [Authorize]
        [SwaggerOperation(OperationId = "DeleteTravelKitItemById")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]


        public async Task<IActionResult> DeleteTravelKitItem(Guid id)
        {
            //validate and try to delete
            var errors = await _service.DeleteTravelKitItemById(id);
            //if errors - send errors
            if (errors.Any()) return BadRequest(errors);

            //else - log about
            _systemEventService.LogDeleteEntity(id, typeof(TravelKitItem), Request.HttpContext.Connection.RemoteIpAddress?.ToString());
            return Ok();
        }


        /// <summary>
        /// Get all Travel Kit Items
        /// </summary>
        /// <returns>All Travel Kit Items</returns>
        [HttpGet("TravelKitItem/{limit}/{skip}")]
        [Authorize]
        [SwaggerOperation(OperationId = "GetAllTravelKitItems")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]

        public async Task<IEnumerable<TravelKitItem>> GetAllTravelKitItems(int limit, int skip)
        {
            return await _service.GetAllTravelKitItems(limit, skip);
        }

        /// <summary>
        /// Get TravelKitItem by Id
        /// </summary>
        /// <returns>All TravelKitItems</returns>
        [HttpGet("TravelKitItem/{id}")]
        [Authorize]
        [SwaggerOperation(OperationId = "GetTravelKitItemById")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
        public async Task<TravelKitItem> GetTravelKitItemById(Guid id)
        {
            return await _service.GetTravelKitItemById(id);
        }

        /// <summary>
        /// Add item to the kit
        /// </summary>
        /// <param name="itemId">Id of the item</param>
        /// <param name="kitId">Id of the kit</param>
        /// <returns>Ok</returns>
        [HttpPut("AddItemToKit/{kitId}/{itemId}")]
        [Authorize]
        [SwaggerOperation(OperationId = "AddItemToKit")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> SetItemToTravelKit(Guid itemId, Guid kitId)
        {
            await _service.AddItemToKit(itemId, kitId);
            return Ok();
        }
    }
}
