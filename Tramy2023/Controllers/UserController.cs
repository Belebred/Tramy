using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using Tramy.Backend.Data.DBServices;
using Tramy.Backend.Hubs;
using Tramy.Common.Chats;
using Tramy.Common.CrossModels;
using Tramy.Common.Hike;
using Tramy.Common.UserEvent;
using Tramy.Common.Users;

namespace Tramy.Backend.Controllers
{
    /// <summary>
    /// Controller for manage Users
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        /// <summary>
        /// MongoDB service
        /// </summary>
        private readonly UserService _service;

        private readonly SystemEventService _systemEventService;

        private readonly NotiHub _notiHub;

        private readonly HttpClient _httpClient;

        /// <summary>
        /// Logger of controller
        /// </summary>
        private readonly ILogger<UserService> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="service">MongoDB service from Startup</param>
        /// <param name="systemEventService">MongoDB service of system events from Startup</param>
        /// <param name="logger">Logger from Startup</param>
        public UserController(UserService service, SystemEventService systemEventService, HttpClient httpClient, NotiHub notiHub, ILogger<UserService> logger)
        {
            //save services and logger
            _service = service;
            _systemEventService = systemEventService;
            _notiHub = notiHub;
            _httpClient = httpClient;
            _logger = logger;
        }

        /// <summary>
        /// Set user coordinates
        /// </summary>
        /// <returns>ok</returns>
        [HttpPost("SetMyLocation/{id}/{locationId}/{locationPartId}/{x}/{y}")]
        [Authorize]
        [SwaggerOperation(OperationId = "SetUserLocation")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]


        public async Task<IActionResult> SetMyLocation( Guid id,  Guid? locationId,  Guid? locationPartId,  int x,  int y)
        {
            await _service.SetMyLocation(id, locationId, locationPartId, x, y);
            return Ok();
        }

        /// <summary>
        /// Set user GPS coordinates
        /// </summary>
        /// <returns>ok</returns>
        [HttpPost("SetMyGPSLocation/{id}/{lat}/{lon}")]
        [Authorize]
        [SwaggerOperation(OperationId = "SetGPSLocation")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]


        public async Task<IActionResult> SetMyGPSLocation( Guid id,  double lat,  double lon)
        {
            await _service.SetMyGPSLocation(id, lat, lon);
            return Ok();
        }


        /// <summary>
        /// Set user chat hub id
        /// </summary>
        /// <returns>ok</returns>
        [HttpPost("SetChatHubId/{id}/{chatHubId}")]
        [Authorize]
        [SwaggerOperation(OperationId = "SetChatHubId")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]


        public async Task<IActionResult> SetChatHubId( Guid id,  string chatHubId)
        {
            await _service.SetChatHubId(id, chatHubId);
            return Ok();
        }

        /// <summary>
        /// Set user noti hub id
        /// </summary>
        /// <returns>ok</returns>
        [HttpPost("SetNotiHubId/{id}/{notiHubId}")]
        [Authorize]
        [SwaggerOperation(OperationId = "SetNotiHubId")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]


        public async Task<IActionResult> SetNotiHubId( Guid id,  string notiHubId)
        {
            await _service.SetNotiHubId(id, notiHubId);
            return Ok();
        }

        /// <summary>
        /// Set user noti hub id
        /// </summary>
        /// <returns>ok</returns>
        //[HttpGet("GetSmsCode/{phone}")]
        //[Authorize]
        //[SwaggerOperation(OperationId = "GetSmsCode")]
        //[ProducesResponseType((int)HttpStatusCode.OK)]
        //[ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        //[ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        //[ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]

        //public async Task<IActionResult> GetSmsCode( string phone)
        //{
        //    var code = await _service.GetSmsCodeAndRegisterIfNeed(phone);
        //    if (code != null)
        //    {
        //        var smsSender = new SmsSender(_httpClient);
        //        await smsSender.SendSmsWithCode(phone, code);
        //    }
        //    return Ok();
        //}

        /// <summary>
        /// Change UserStatus
        /// </summary>
        /// <param name="status">Status of user</param>
        /// <returns></returns>
        [HttpPost("ChangeStatus/{status}")]
        [Authorize]
        [SwaggerOperation(OperationId = "UpdateUserStatus")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ChangeUserStatus ([FromRoute] string status)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                await _service.ChangeUserStatus(new Guid(userId), status);
                return Ok();
            }
            else
            {
                return BadRequest("No user with such id");
            }
        }


        /// <summary>
        /// Changes userStatus permission of the child
        /// </summary>
        /// <param name="childId">Child id</param>
        /// <returns></returns>
        [HttpPost("ChangeUserStatusPerm/{childId}")]
        [Authorize]
        [SwaggerOperation(OperationId = "UpdateUserStatusPermission")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ChangeUserStatusPermission([FromRoute] Guid childId)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                await _service.ChangeStatusPerm(new Guid(userId), childId);
                return Ok();
            }
            else
            {
                return BadRequest("No user with such id");
            }
        }

        /// <summary>
        /// Changes visible status of user
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpPost("ChangeVisibleStatus/{status}")]
        [Authorize]
        [SwaggerOperation(OperationId = "UpdateVisibleStatus")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ChangeVisibleStatus ([FromRoute]string status)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                await _service.ChangeVisibleStatus(new Guid(userId), status);
                return Ok();
            }
            else
            {
                return BadRequest("No user with such id");
            }
        }

        /// <summary>
        /// Changes visible permission of the child
        /// </summary>
        /// <param name="childId">Child id</param>
        /// <returns></returns>
        [HttpPost("ChangeVisiblePerm/{childId}")]
        [Authorize]
        [SwaggerOperation(OperationId = "UpdateVisibleStatusPermission")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ChangeVisiblePermission([FromRoute]Guid childId)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                await _service.ChangeVisiblePerm(new Guid(userId), childId);
                return Ok();
            }
            else
            {
                return BadRequest("No user with such id");
            }
        }

        /// <summary>
        /// Changes text status of the user
        /// </summary>
        /// <param name="status">Status string</param>
        /// <returns></returns>
        [HttpPost("ChangeTextStatus/{status}")]
        [Authorize]
        [SwaggerOperation(OperationId = "UpdateTextStatus")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ChangeTextStatus(string status)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                await _service.ChangeTextStatus(new Guid(userId), status);
                return Ok();
            }
            else
            {
                return BadRequest("No user with such id");
            }
        }

        /// <summary>
        /// Insert new user
        /// </summary>
        /// <param name="entity">User to insert</param>
        [HttpPost]
        [Authorize]
        [SwaggerOperation(OperationId = "InsertUser")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]

        public async Task<IActionResult> Post(User entity)
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
        /// Takes name of property and set value for it
        /// </summary>
        /// <param name="propName">Name of property</param>
        /// <param name="propValue">Value of property</param>
        [HttpPost("UpdateProp/{propName}/{propValue}")]
        [Authorize]
        [SwaggerOperation(OperationId = "UpdateProperty")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]

        public async Task<IActionResult> UpdateProperty(string propName, string propValue)
        {
            var userId = new Guid(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var user = await _service.GetById(userId);
            try
            {
               user.GetType().GetProperty(propName).SetValue(user, propValue);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"{nameof(User)}: Cant set this value to property");
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Update new user
        /// </summary>
        /// <param name="entity">User to update</param>
        [HttpPut]
        [Authorize]
        [SwaggerOperation(OperationId = "UpdateUser")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]

        public async Task<IActionResult> Put(User entity)
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
        /// Delete new user
        /// </summary>
        /// <param name="id">User to delete</param>
        [HttpDelete("{id}")]
        [Authorize]
        [SwaggerOperation(OperationId = "DeleteUserById")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Delete( Guid id)
        {
            var user = await _service.GetById(id);
            if (user == null)
                return NotFound();
            if (user.Email == "admin@admin.ru")
                return Forbid();

            //validate and try to delete
            var errors = await _service.DeleteById(id);
            //if errors - send errors
            if (errors.Any()) return BadRequest(errors);

            //else - log about
            _systemEventService.LogDeleteEntity(id, typeof(User), Request.HttpContext.Connection.RemoteIpAddress?.ToString());
            return Ok();
        }

        /// <summary>
        /// Get user by Id
        /// </summary>
        /// <returns>User entity</returns>
        [HttpGet("{id}")]
        [Authorize]
        [SwaggerOperation(OperationId = "GetUserById")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]


        public async Task<User> Get( Guid id)
        {
            return await _service.GetById(id);
        }

        //TODO REMOVE
        /// <summary>
        /// Get all logins users !!!NOTSECURE
        /// </summary>
        /// <returns>User entity</returns>
        [HttpGet("GetAllLogins")]
        [AllowAnonymous]
        [SwaggerOperation(OperationId = "GetAllLogins")]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.OK)]
        public async Task<IEnumerable<object>> GetAllLogins()
        {
            return (await _service.Get(u=>true)).Select(u=>new {u.Email, u.PasswordHash});
        }

        //TODO REMOVE
        /// <summary>
        /// Restore admin !!!NOTSECURE
        /// </summary>
        /// <returns>User entity</returns>
        [HttpGet("RestoreAdmin")]
        [AllowAnonymous]
        [SwaggerOperation(OperationId = "RestoreAdmin")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> RestoreAdmin()
        {
            //check administrator account
            var user = await _service.GetByEmail("admin@admin.ru");
            if (user != null) return Ok();
            //insert if not exists
            user = new User()
            {
                AccountType = AccountType.Administration,
                Email = "admin@admin.ru",
                FirstName = "Admin",
                LastName = "Admin",
                RegStatus = RegStatus.Active,
                Gender = Gender.Man,
                PasswordHash = UserService.CreateMd5("A12345!")
            };
            await _service.Insert(user);
            return Ok();
        }

       

        /// <summary>
        /// Find hikes by request
        /// </summary>
        /// <param name="request">Request to find</param>
        /// <returns></returns>
        [HttpPost("FindHikes")]
        [Authorize]
        [SwaggerOperation(OperationId = "FindHikes")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]

        public async Task<List<Guid>> FindHikes(FindHikeRequest request)
        {
            return await _service.FindHikes(request);
        }

        /// <summary>
        /// Get user by Id
        /// </summary>
        /// <returns>All roles</returns>
        [HttpPost("FindUsers")]
        [Authorize]
        [SwaggerOperation(OperationId = "FindUsers")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]

        public async Task<List<FindUserResult>> FindUsers(FindUserRequest request)
        {
            return await _service.FindUsers(request.MyId, request.Search);
        }


        /// <summary>
        /// Get user's friends by Id
        /// </summary>
        /// <returns>All roles</returns>
        [HttpGet("GetFriends/{id}/{skip}/{limit}")]
        [Authorize]
        [SwaggerOperation(OperationId = "GetFriendsById")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]


        public async Task<List<FindUserResult>> GetFriends( Guid id, int skip, int limit)
        {
            return await _service.GetFriends(id, skip, limit);
        }

        /// <summary>
        /// Get user's incoming friends by Id
        /// </summary>
        /// <returns>All roles</returns>
        [HttpGet("GetIncomingFriends/{id}/{skip}/{limit}")]
        [Authorize]
        [SwaggerOperation(OperationId = "GetIncomingFriendsByUser")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]


        public async Task<List<FindUserResult>> GetIncomingFriends(Guid id, int skip, int limit)
        {
            return await _service.GetIncomingFriends(id, skip, limit);
        }


        /// <summary>
        /// Get user's upcoming friends by Id
        /// </summary>
        /// <returns>All upcoming friends</returns>
        [HttpGet("GetUpcomingFriends/{id}/{skip}/{limit}")]
        [Authorize]
        [SwaggerOperation(OperationId = "GetUpcomingFriendsByUser")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]


        public async Task<List<FindUserResult>> GetUpcomingFriends(Guid id, int skip, int limit)
        {
            return await _service.GetUpcomingFriends(id, skip, limit);
        }

        /// <summary>
        /// Get Events from user's invites
        /// </summary>
        /// <returns>All events from invites</returns>
        [HttpGet("GetInvitingEvents/{id}/{skip}/{limit}")]
        [Authorize]
        [SwaggerOperation(OperationId = "GetInvitingEventsByUser")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]


        public async Task<List<UserEvent>> GetInvitingEvents(Guid id, int skip, int limit)
        {
            return await _service.GetInvitingEvents(id, skip, limit);
        }

        /// <summary>
        /// Get Events from user's requests
        /// </summary>
        /// <returns>All events from requests</returns>
        [HttpGet("GetRequestedEvents/{id}/{skip}/{limit}")]
        [Authorize]
        [SwaggerOperation(OperationId = "GetRequestedEventsByUser")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]


        public async Task<List<UserEvent>> GetRequestedEvents(Guid id, int skip, int limit)
        {
            return await _service.GetInvitingEvents(id, skip, limit);
        }

        /// <summary>
        /// Get Bunches from user's invites
        /// </summary>
        /// <returns>All Bunches from invites</returns>
        [HttpGet("GetInvitingBunches/{id}/{skip}/{limit}")]
        [Authorize]
        [SwaggerOperation(OperationId = "GetInvitingBunchesByUser")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]


        public async Task<List<Bunch>> GetInvitingBunches(Guid id, int skip, int limit)
        {
            return await _service.GetInvitingBunches(id, skip, limit);
        }

        /// <summary>
        /// Get Bunches from user's requests
        /// </summary>
        /// <returns>All bunches from requests</returns>
        [HttpGet("GetRequestedBunches/{id}/{skip}/{limit}")]
        [Authorize]
        [SwaggerOperation(OperationId = "GetRequestedBunchesByUser")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]


        public async Task<List<Bunch>> GetRequestedBunches(Guid id, int skip, int limit)
        {
            return await _service.GetRequestedBunches(id, skip, limit);
        }

        /// <summary>
        /// Get all friends near coords
        /// </summary>
        /// <returns>All locations</returns>
        [HttpGet("GetNear/{lat}/{lon}/{dist}/{skip}/{limit}")]
        [Authorize]
        [SwaggerOperation(OperationId = "GetAllNearFriends")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]

        public async Task<IEnumerable<NearFriend>> GetNear( double lat,  double lon,  double dist, int skip, int limit)
        {
            return await _service.GetNearFriends(lat, lon, dist, skip, limit);
        }

        /// <summary>
        /// Get all users in location part
        /// </summary>
        /// <returns>All locations</returns>
        [HttpGet("GetLocationPartUsers/{id}/{skip}/{limit}")]
        [Authorize]
        [SwaggerOperation(OperationId = "GetAllLocationUsers")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]


        public async Task<IEnumerable<LocationUser>> GetLocationPartUsers( Guid id, int skip, int limit)
        {
            return await _service.GetUsersInLocationPart(id, skip, limit);
        }

        /// <summary>
        /// Set friends
        /// </summary>
        /// <returns>ok</returns>
        [HttpPost("SetFriends/{first}/{second}")]
        [Authorize]
        [SwaggerOperation(OperationId = "SetFriends")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]

        public async Task<IActionResult> SetFriends( Guid first,  Guid second)
        {
            await _service.SetFriends(first, second);
            return Ok();
        }

        /// <summary>
        /// Remove friends
        /// </summary>
        /// <returns>ok</returns>
        [HttpPost("RemoveFriend/{first}/{second}")]
        [Authorize]
        [SwaggerOperation(OperationId = "RemoveFriend")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]

        public async Task<IActionResult> RemoveFriend( Guid first,  Guid second)
        {
            await _service.RemoveFriend(first, second);
            return Ok();
        }

       

        
        /// <summary>
        /// Set friend request
        /// </summary>
        /// <returns>ok</returns>
        [HttpPost("SetFriendRequest/{first}/{second}")]
        [Authorize]
        [SwaggerOperation(OperationId = "SetFriendsRequest")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]

        public async Task<IActionResult> SetFriendRequest( Guid first,  Guid second)
        {
            await _service.SetFriendRequest(first, second);
            return Ok();
        }

        /// <summary>
        /// Answer friend request
        /// </summary>
        /// <returns>ok</returns>
        [HttpPost("AnswerFriendRequest/{first}/{second}/{decision}")]
        [Authorize]
        [SwaggerOperation(OperationId = "AnswerFriendsRequest")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]

        public async Task<IActionResult> AnswerFriendRequest( Guid first,  Guid second,  bool decision)
        {
            if (decision)
            {
                await _service.ApproveFriendRequest(first, second);
            }
            else
            {
                await _service.DisapproveFriendRequest(first, second);
            }
            return Ok();
        }

        

        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns>All users</returns>
        [HttpGet("{limit}/{skip}")]
        [Authorize]
        [SwaggerOperation(OperationId = "GetAllUsers")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]

        public async Task<IEnumerable<User>> Get(int limit, int skip)
        {
            return await _service.Get(limit, skip);
        }

        /// <summary>
        /// Create User hike need
        /// </summary>
        /// <param name="entity">User hike need</param>
        /// <returns></returns>
        [HttpPost("CreateUserHikeNeed")]
        [Authorize]
        [SwaggerOperation(OperationId = "InsertUserHikeNeed")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> InsertUserHikeNeed(UserHikeNeed entity)
        {
            //validate and try to insert
            var errors = await _service.InsertUserHikeNeed(entity);
            //if errors - send errors
            if (errors.Any()) return BadRequest(errors);

            //else - log about
            _systemEventService.LogInsertEntity(entity, Request.HttpContext.Connection.RemoteIpAddress?.ToString());
            return Ok();
        }

        /// <summary>
        /// Update UserHikeNeed
        /// </summary>
        /// <param name="entity">UserHikeNeed entity</param>
        /// <returns>Updated entity</returns>
        [HttpPut("UpdateUserHikeNeed")]
        [Authorize]
        [SwaggerOperation(OperationId = "UpdateUserHikeNeed")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateUserHikeNeed(UserHikeNeed entity)
        {
            //validate and try to update
            var errors = await _service.FullUpdateUserHikeNeed(entity);
            //if errors - send errors
            if (errors.Any()) return BadRequest(errors);

            //else - log about
            _systemEventService.LogChangeEntity(entity, Request.HttpContext.Connection.RemoteIpAddress?.ToString());
            return Ok();
        }

        /// <summary>
        /// Get all User hike needs
        /// </summary>
        /// <returns>All user hike needs</returns>
        [HttpGet("UserHikeNeed/{limit}/{skip}")]
        [Authorize]
        [SwaggerOperation(OperationId = "GetAllUsers")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]

        public async Task<IEnumerable<UserHikeNeed>> GetUserHikeNeeds(int limit, int skip)
        {
            return await _service.GetUserHikeNeeds(limit, skip) ;
        }

        /// <summary>
        /// Get User Hike Need by id
        /// </summary>
        /// <param name="id">User Hike Need id</param>
        /// <returns>User Hike Need entity</returns>
        [HttpGet("UserHikeNeed/{id}")]
        [Authorize]
        [SwaggerOperation(OperationId = "GetUserHikeNeedById")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
        public async Task<UserHikeNeed> GetUserHikeNeedById(Guid id)
        {
            return await _service.GetUserHikeNeedById(id);
        }

        /// <summary>
        /// Delete user hike need
        /// </summary>
        /// <param name="id">User hike need to delete</param>
        [HttpDelete("UserHikeNeed/{id}")]
        [Authorize]
        [SwaggerOperation(OperationId = "DeleteUserHikeNeedById")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteUserHikeNeed(Guid id)
        {
            //validate and try to delete
            var errors = await _service.DeleteUserHikeNeedById(id);
            //if errors - send errors
            if (errors.Any()) return BadRequest(errors);

            //else - log about
            _systemEventService.LogDeleteEntity(id, typeof(UserHikeNeed), Request.HttpContext.Connection.RemoteIpAddress?.ToString());
            return Ok();
        }
    }
}
