using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;
using Tramy.Backend.Data.DBServices;
using Tramy.Common.Locations;
using Tramy.Common.Navigation;
using Microsoft.AspNetCore.Authorization;
using System.Net;

namespace Tramy.Backend.Controllers
{
    /// <summary>
    /// Controller for manage Locations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class LocationPartController : Controller
    {
        /// <summary>
        /// MongoDB service
        /// </summary>
        private readonly LocationPartService _service;

        private readonly DeviceService _deviceService;

        private readonly MeasuredPointService _pointService;

        private readonly SystemEventService _systemEventService;

        /// <summary>
        /// Logger of controller
        /// </summary>
        private readonly ILogger<LocationPartService> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="service">MongoDB service from Startup</param>
        /// <param name="deviceService">Devices service DB</param>
        /// <param name="systemEventService">MongoDB service of system events from Startup</param>
        /// <param name="logger">Logger from Startup</param>
        public LocationPartController(LocationPartService service, DeviceService deviceService, MeasuredPointService pointService, SystemEventService systemEventService, ILogger<LocationPartService> logger)
        {
            //save services and logger
            _service = service;
            _deviceService = deviceService;
            _pointService = pointService;
            _systemEventService = systemEventService;
            _logger = logger;
        }

        /// <summary>
        /// Insert new location
        /// </summary>
        /// <param name="locationId">LocationId to insert</param>
        [HttpPost("Create/{locationId}")]
        [Authorize]
        [SwaggerOperation(OperationId = "InsertLocationPart")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]

        public async Task<IActionResult> Create( Guid locationId)
        {
            //create new location part
            //validate and try to insert
            var (locationPart, errors) = await _service.CreateNewLocationPart(locationId);
            //if errors - send errors
            if (errors.Any()) return BadRequest(errors);

            //else - log about
            _systemEventService.LogInsertEntity(locationPart, Request.HttpContext.Connection.RemoteIpAddress?.ToString());
            return Ok(locationPart.Id);
        }


        /// <summary>
        /// Update measures to point
        /// </summary>
        /// <param name="pointId">Point id to update</param>
        /// <param name="measures"></param>
        [HttpPut("UpdatePoint/{pointId}")]
        [Authorize]
        [SwaggerOperation(OperationId = "UpdatePointById")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]

        public async Task<IActionResult> UpdatePoint( Guid pointId, List<DeviceMeasure> measures)
        {
            var point = await _pointService.GetById(pointId);
            if (point != null)
            {
                point.AverageMeasures = measures;
                point.Status = MeasuredPointStatus.Measured;
                await _pointService.FullUpdate(point);
            }
            return Ok();
        }

        /// <summary>
        /// Converts string base64 to image
        /// </summary>
        /// <param name="base64String"></param>
        /// <returns></returns>
        //[HttpPost("ConvertBase64/{base64String}")]
        //[Authorize]
        //[SwaggerOperation(OperationId = "ConvertBase64ToImage")]
        //[ProducesResponseType((int)HttpStatusCode.OK)]
        //[ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        //[ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        //[ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        //public Image Base64ToImage(string base64String)
        //{
        //    byte[] imageBytes = Convert.FromBase64String(base64String.Substring(base64String.LastIndexOf(',') + 1));
        //    MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);

        //    ms.Write(imageBytes, 0, imageBytes.Length);
        //    return System.Drawing.Image.FromStream(ms, true);
        //}

        /// <summary>
        /// Change map
        /// </summary>
        /// <param name="id">Part to change map</param>
        //[HttpPost("ChangeMap/{id}")]
        //[Authorize]
        //[SwaggerOperation(OperationId = "ChangeMapInLocationPart")]
        //[ProducesResponseType((int)HttpStatusCode.OK)]
        //[ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        //[ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        //[ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        //[ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]


        //public async Task<IActionResult> ChangeMap( Guid id)
        //{

        //    var locationPart = await _service.GetById(id);
        //    if (locationPart == null)
        //        return BadRequest("Entity not found");


        //    var map = "";

        //    using (var reader = new StreamReader(Request.Body))
        //    {
        //        map = await reader.ReadToEndAsync();
        //    }

        //    if (string.IsNullOrEmpty(map))
        //        return BadRequest(new List<string>() { "Map is empty" });

        //    try
        //    {
        //        var image = Base64ToImage(map);
        //        if (locationPart.SizeX > 0 && locationPart.SizeY > 0)
        //        {
        //            locationPart.Kx = image.Width / locationPart.SizeX;
        //            locationPart.Ky = image.Height / locationPart.SizeY;
        //        }
        //    }
        //    catch
        //    {
        //        return BadRequest(new List<string>() { "Image has bad format. Allows formats: png, jpeg, jpg, bmp" });
        //    }


        //    locationPart.Map = map;
        //    var errors = await _service.FullUpdate(locationPart);
        //    //if errors - send errors
        //    if (errors.Any()) return BadRequest(errors);

        //    //else - log about
        //    _systemEventService.LogChangeEntity(locationPart,
        //        Request.HttpContext.Connection.RemoteIpAddress?.ToString());
        //    return Ok(locationPart.Id);
        //}

        /// <summary>
        /// Insert polygon
        /// </summary>
        /// <param name="polygon">Polygon to insert</param>
        [HttpPost("InsertPolygon")]
        [Authorize]
        [SwaggerOperation(OperationId = "InsertPolygon")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]

        public async Task<IActionResult> InsertPolygon(Polygon polygon)
        {
            //validate and try to insert
            var errors = await _service.InsertPolygon(polygon);
            //if errors - send errors
            if (errors.Any()) return BadRequest(errors);

            //else - log about
            _systemEventService.LogChangeEntity(polygon, Request.HttpContext.Connection.RemoteIpAddress?.ToString());
            return Ok();
        }


        /// <summary>
        /// Update measured points
        /// </summary>
        /// <param name="id">Location part id</param>
        /// <param name="points">Measured points</param>
        [HttpPut("UpdatePoints/{id}")]
        [Authorize]
        [SwaggerOperation(OperationId = "UpdatePointsInLocationPart")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]


        public async Task<IActionResult> UpdatePoints( Guid id, List<MeasuredPoint> points)
        {
            await _pointService.RemoveAllLocationPartPoints(id);
            await _pointService.InsertLocationPartPoints(points);
            return Ok();
        }

        /// <summary>
        /// Get location part measured points
        /// </summary>
        /// <param name="id">Location part id</param>
        [HttpGet("GetPoints/{id}/{skip}/{limit}")]
        [Authorize]
        [SwaggerOperation(OperationId = "GetAllPointsFromLocationPart")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]

        public async Task<IEnumerable<MeasuredPoint>> GetPoints( Guid id, int skip, int limit)
        {
            return await _pointService.GetByLocationPart(id, skip, limit);
        }

        /// <summary>
        /// Update polygon
        /// </summary>
        /// <param name="polygon">Polygon to insert</param>
        [HttpPut("UpdatePolygon")]
        [Authorize]
        [SwaggerOperation(OperationId = "UpdatePolygon")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]

        public async Task<IActionResult> UpdatePolygon(Polygon polygon)
        {
            //validate and try to update
            var errors = await _service.UpdatePolygon(polygon);
            //if errors - send errors
            if (errors.Any()) return BadRequest(errors);

            //else - log about
            _systemEventService.LogChangeEntity(polygon, Request.HttpContext.Connection.RemoteIpAddress?.ToString());
            return Ok();
        }

        /// <summary>
        /// Replace polygons after union
        /// </summary>
        /// <param name="locationPartId">Location part Id</param>
        /// <param name="polygons">Polygons to replace</param>
        [HttpPut("ReplacePolygons/{locationPartId}")]
        [Authorize]
        [SwaggerOperation(OperationId = "ReplacePolygons")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]

        public async Task<IActionResult> ReplacePolygons( Guid locationPartId, List<Polygon> polygons)
        {
             await _service.ReplacePolygons(locationPartId, polygons);
             return Ok();
        }

        /// <summary>
        /// Get polygons
        /// </summary>
        /// <param name="ids">Array of ids</param>
        [HttpGet("GetPolygons/{skip}/{limit}")]
        [Authorize]
        [SwaggerOperation(OperationId = "GetAllPolygons")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]

        public async Task<IEnumerable<Polygon>> GetPolygons([FromQuery]IEnumerable<Guid> ids, int skip, int limit)
        {
            return await _service.GetPolygons(ids, skip, limit);
        }

        /// <summary>
        /// Delete polygon
        /// </summary>
        /// <param name="id">Polugon to delete</param>
        [HttpDelete("DeletePolygon/{id}")]
        [Authorize]
        [SwaggerOperation(OperationId = "DeletePolygonById")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeletePolygon( Guid id)
        {
            //validate and try to delete
            var errors = await _service.DeletePolygonById(id);
            //if errors - send errors
            if (errors.Any()) return BadRequest(errors);

            //else - log about
            _systemEventService.LogDeleteEntity(id, typeof(Polygon), Request.HttpContext.Connection.RemoteIpAddress?.ToString());
            return Ok();
        }

        /// <summary>
        /// Get Location part by Id
        /// </summary>
        /// <returns>Location part by id</returns>
        [HttpGet("{id}")]
        [Authorize]
        [SwaggerOperation(OperationId = "GetPolygonById")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]


        public async Task<LocationPart> Get( Guid id)
        {
            return await _service.GetById(id);
        }

        /// <summary>
        /// Get Map Location part by Id
        /// </summary>
        /// <returns>Location part by id</returns>
        [HttpGet("GetMap/{id}")]
        [Authorize]
        [SwaggerOperation(OperationId = "GetMapByLocationPartId")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]

        public async Task<string> GetMap( Guid id)
        {
            var lp = await _service.GetById(id);
            return lp?.Map;
        }

        /// <summary>
        /// Get Location part by Location id
        /// </summary>
        /// <param name="id">Location id</param>
        /// <returns>Location part by id</returns>
        [HttpGet("GetByLocation/{id}/{skip}/{limit}")]
        [Authorize]
        [SwaggerOperation(OperationId = "GetAllLocationPartByLocationId")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]

        public async Task<IEnumerable<LocationPart>> GetByLocation( Guid id, int skip, int limit)
        {
            return await _service.GetByLocation(id, skip, limit);
        }

        /// <summary>
        /// Get all locations parts of location by mobile
        /// </summary>
        /// <param name="locationId">LocationId to get parts</param>
        /// <returns>All locations parts</returns>
        [HttpGet("GetByLocationToMobile/{locationId}/{skip}/{limit}")]
        [Authorize]
        [SwaggerOperation(OperationId = "GetAllMobileLocationPartByLocationId")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]

        public async Task<IEnumerable<MobileLocationPart>> GetByLocationToMobile( Guid locationId, int skip, int limit)
        {
            var lParts =  await _service.GetByLocation(locationId);
            var result = new List<MobileLocationPart>();
            foreach (var locationPart in lParts)
            {
                var devices = await _deviceService.GetDeviceInLocationsById(locationPart.Id);
                var points = await _pointService.GetByLocationPart(locationPart.Id);
                var polygons = await _service.GetPolygons(locationPart.Polygons);
                var mlp = MobileLocationPart.FromLocationPart(locationPart, devices, points, polygons);
                result.Add(mlp);
            }
            return result.Skip(skip).Take(limit).ToList();
        }

        /// <summary>
        /// Update Location part
        /// </summary>
        /// <param name="entity">Location part to update</param>
        [HttpPut]
        [Authorize]
        [SwaggerOperation(OperationId = "UpdateLocationPart")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]

        public async Task<IActionResult> Put(LocationPart entity)
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
        /// Delete location part
        /// </summary>
        /// <param name="id">Location to delete</param>
        [HttpDelete("{id}")]
        [Authorize]
        [SwaggerOperation(OperationId = "DeleteLocationPartById")]
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
            _systemEventService.LogDeleteEntity(id, typeof(LocationPart), Request.HttpContext.Connection.RemoteIpAddress?.ToString());
            return Ok();
        }


        /// <summary>
        /// Insert point in location part
        /// </summary>
        /// <param name="point">Point</param>
        /// <returns></returns>
        [HttpPost("InsertPointInLocation")]
        [Authorize]
        [SwaggerOperation(OperationId = "InsertPointInLocationPart")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]

        public async Task<IActionResult> InsertPointInLocation(MeasuredPoint point)
        {
            await _pointService.Insert(point);
            return Ok();
        }

       
        /// <summary>
        /// Delete point in location
        /// </summary>
        /// <param name="id">Point in location id</param>
        /// <returns></returns>
        [HttpDelete("DeletePointInLocation/{id}")]
        [Authorize]
        [SwaggerOperation(OperationId = "DeletePointInLocationPartById")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]

        public async Task<IActionResult> DeletePointInLocation( Guid id)
        {
            await _pointService.DeleteById(id);
            return Ok();
        }


    }
}
