using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;
using Tramy.Backend.Data.DBServices;
using Tramy.Common.Devices;
using Tramy.Common.Locations;
using Microsoft.AspNetCore.Authorization;

namespace Tramy.Backend.Controllers
{
    /// <summary>
    /// Controller for manage Devices
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DeviceController : Controller
    {
        /// <summary>
        /// MongoDB service
        /// </summary>
        private readonly DeviceService _service;

        private readonly SystemEventService _systemEventService;

        /// <summary>
        /// Logger of controller
        /// </summary>
        private readonly ILogger<DeviceService> _logger;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="service">MongoDB service from Startup</param>
        /// <param name="systemEventService">MongoDB service of system events from Startup</param>
        /// <param name="logger">Logger from Startup</param>
        public DeviceController(DeviceService service, SystemEventService systemEventService, ILogger<DeviceService> logger)
        {
            //save services and logger
            _service = service;
            _systemEventService = systemEventService;
            _logger = logger;
        }

        /// <summary>
        /// Insert new device
        /// </summary>
        /// <param name="entity">Device to insert</param>
        [HttpPost]
        [Authorize]
        [SwaggerOperation(OperationId = "InsertDevice")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Post(Device entity)
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
        /// Update new device
        /// </summary>
        /// <param name="entity">Device to update</param>
        [HttpPut]
        [Authorize]
        [SwaggerOperation(OperationId = "UpdateDevice")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Put(Device entity)
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
        ///  Change status of device to "In use"
        /// </summary>
        /// <param name="id">Device id</param>
        [HttpPost("SetDeviceStatusInUse/{id}")]
        [Authorize]
        [SwaggerOperation(OperationId = "SetDeviceStatusInUse")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]

        public async Task<IActionResult> DeviceStatusInUse(Guid id)
        {
            var device = _service.GetById(id);

            //check if device exists
            if (device == null)
                return BadRequest(new []{"Device with such Id not exist"});


            device.Result.Status = DeviceStatus.InUse;
            device.Result.LastService = DateTime.Now;
            await _service.FullUpdate(device.Result);
            return Ok();
        }


        /// <summary>
        /// Delete new device by id
        /// </summary>
        /// <param name="id">Device to delete</param>
        [Authorize]
        [HttpDelete("{id}")]
        [SwaggerOperation(OperationId = "DeleteDeviceById")]
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
            _systemEventService.LogDeleteEntity(id, typeof(Device), Request.HttpContext.Connection.RemoteIpAddress?.ToString());
            return Ok();
        }

        /// <summary>
        /// Get all devices
        /// </summary>
        /// <returns>All devices</returns>
        [Authorize]
        [HttpGet("{limit}/{skip}")]
        [SwaggerOperation(OperationId = "GetAllDevices")]
        [ProducesResponseType((typeof(IEnumerable<Device>)), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        public async Task<IEnumerable<Device>> Get(int limit, int skip)
        {
            return await _service.Get(limit, skip);
        }

        /// <summary>
        /// Get all devices series
        /// </summary>
        /// <returns>All device series</returns>
        [Authorize]
        [HttpGet("Series/{skip}/{limit}")]
        [SwaggerOperation(OperationId = "GetAllDeviceSeries")]
        [ProducesResponseType((typeof(IEnumerable<DeviceSeries>)), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        public async Task<IEnumerable<DeviceSeries>> Series(int skip, int limit)
        {
            return await _service.GetSeries(skip, limit);
        }

        /// <summary>
        /// Get List of devices by MAC
        /// </summary>
        /// <returns>List of first five devices with such MAC</returns>
        [Authorize]
        [HttpGet("FindMac/{mac}/{skip}/{limit}")]
        [SwaggerOperation(OperationId = "FindMac")]
        [ProducesResponseType((typeof(IEnumerable<string>)), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        public async Task<IEnumerable<string>> FindMac(string mac,int skip, int? limit = 5 )
        {
            return await _service.FindMacByPart(mac, limit??5, skip);
        }

        /// <summary>
        /// Update device series
        /// </summary>
        /// <param name="deviceSeries">Device series to update</param>
        [Authorize]
        [HttpPut("Series")]
        [SwaggerOperation(OperationId = "UpdateDeviceSeries")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> SeriesUpdate(DeviceSeries deviceSeries)
        {
            //validate and try to update
            var errors = await _service.FullUpdateSeries(deviceSeries);
            //if errors - send errors
            if (errors.Any()) return BadRequest(errors);

            //else - log about
            _systemEventService.LogChangeEntity(deviceSeries, Request.HttpContext.Connection.RemoteIpAddress?.ToString());
            return Ok();
        }


        /// <summary>
        /// Insert new device series
        /// </summary>
        /// <param name="entity">Device series to insert</param>
        [Authorize]
        [HttpPost("Series")]
        [SwaggerOperation(OperationId = "AddDeviceSeries")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> SeriesInsert(DeviceSeries entity)
        {
            //validate and try to insert
            var errors = await _service.InsertSeries(entity);
            //if errors - send errors
            if (errors.Any()) return BadRequest(errors);

            //else - log about
            _systemEventService.LogInsertEntity(entity, Request.HttpContext.Connection.RemoteIpAddress?.ToString());
            return Ok();
        }

        /// <summary>
        /// Delete  device series by id
        /// </summary>
        /// <param name="id">Device series to delete</param>
        [Authorize]
        [HttpDelete("Series/{id}")]
        [SwaggerOperation(OperationId = "DeleteDeviceSeriesById")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]

        public async Task<IActionResult> Series( Guid id)
        {
            //validate and try to delete
            var errors = await _service.DeleteSeriesById(id);
            //if errors - send errors
            if (errors.Any()) return BadRequest(errors);

            //else - log about
            _systemEventService.LogDeleteEntity(id, typeof(Device), Request.HttpContext.Connection.RemoteIpAddress?.ToString());
            return Ok();
        }


        /// <summary>
        /// Get devices in location part
        /// </summary>
        /// <param name="locationId">Location part's Id</param>
        /// <returns>All devices</returns>
        [Authorize]
        [HttpGet("DevicesInLocation/{locationId}/{skip}/{limit}")]
        [SwaggerOperation(OperationId = "GetAllDeviceInLocation")]
        [ProducesResponseType(typeof(IEnumerable<DeviceInLocation>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        public async Task<IEnumerable<DeviceInLocation>> GetDevicesInLocation( Guid locationId, int skip, int limit)
        {
            return await _service.Get(locationId, skip, limit);
        }



        /// <summary>
        /// Insert device in location
        /// </summary>
        /// <param name="device">Device</param>
        /// <returns>All devices</returns>
        [Authorize]
        [HttpPost("DeviceInLocation")]
        [SwaggerOperation(OperationId = "InsertDeviceInLocation")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> InsertDeviceInLocation(DeviceInLocation device)
        {
            var errors = await _service.InsertLocationDevice(device);
            //if errors - send errors
            if (errors.Any()) return BadRequest(errors);
            return Ok();
        }

        /// <summary>
        /// Update device in location
        /// </summary>
        /// <param name="entity">Entity to update</param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("DeviceInLocation")]
        [SwaggerOperation(OperationId = "UpdateDeviceInLocation")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateDeviceInLocation(DeviceInLocation entity)
        {
            //validate and try to update
            var errors = await _service.FullUpdateDeviceInLocation(entity);
            //if errors - send errors
            if (errors.Any()) return BadRequest(errors);

            //else - log about
            _systemEventService.LogChangeEntity(entity, Request.HttpContext.Connection.RemoteIpAddress?.ToString());
            return Ok();
        }

        /// <summary>
        /// Delete device in location
        /// </summary>
        /// <param name="id">Device in location id</param>
        /// <returns>All devices</returns>
        [HttpDelete("DeviceInLocation/{id}")]
        [SwaggerOperation(OperationId = "DeleteDeviceInLocationById")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]

        public async Task<IActionResult> DeleteDeviceInLocation(Guid id)
        {
            var errors = await _service.DeleteDeviceInLocationById(id);
            if (errors.Any()) return BadRequest(errors);
            return Ok();
        }
    }
}
