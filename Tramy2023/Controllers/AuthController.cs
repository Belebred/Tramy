using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;
using Tramy.Backend.Auth;
using Tramy.Backend.Data.DBServices;
using Tramy.Common.Users;
using Tramy.Common.Organizations;
using Tramy.Common.CrossModels;
using System.Net;

namespace Tramy.Backend.Controllers
{
    /// <summary>
    /// JWT Authentication controller
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController:Controller
    {
        /// <summary>
        /// MongoDB service
        /// </summary>
        private readonly UserService _service;

        private readonly OrganizationService _orgService;

        private readonly SystemEventService _systemEventService;

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
        /// <param name="orgService">MongoDB service for organizations</param>
        public AuthController(UserService service, SystemEventService systemEventService, ILogger<UserService> logger, OrganizationService orgService)
        {
            //save services and logger
            _service = service;
            _orgService = orgService;
            _systemEventService = systemEventService;
            _logger = logger;
        }

        /// <summary>
        /// Check login and password and get token
        /// </summary>
        /// <param name="model">Login model</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("Token")]
        [SwaggerOperation(OperationId = "PostToken")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Token(LoginModel model)
        {
            if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
                return BadRequest("Username or password can not be empty");

            //get identity
            var identity = GetIdentity(model.Username, model.Password);
            User user;

            //if null
            if (identity == null)
            {
                //log fail and return
                user = await _service.GetByEmail(model.Username);
                _systemEventService.LogUserLoginFailed(user?.Id,model.Username,model.Password, Request.HttpContext.Connection.RemoteIpAddress?.ToString());
                return BadRequest("Invalid username or password.");
            }

            var now = DateTime.UtcNow;
            // create JWT-token
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.Issuer,
                    audience: AuthOptions.Audience,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.Lifetime)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            user = await _service.GetByEmail(model.Username);
            var response = new LoginResultModel()
            {
                AccessToken = encodedJwt,
                Id = user.Id.ToString(),
                Email = user.Email,
                IsAdmin = user.AccountType == AccountType.Administration,
                FirstName = user.FirstName,
                LastName = user.LastName
            };

            _systemEventService.LogUserLoginSuccess(user?.Id, Request.HttpContext.Connection.RemoteIpAddress?.ToString());
            return Ok(response);
        }

        /// <summary>
        /// Check phone and code and get token
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("TokenByPhone")]
        [SwaggerOperation(OperationId = "PostTokenByPhone")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> TokenByPhone(LoginModel model)
        {
            if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
                return BadRequest("Phone or code can not be empty");

            //get identity
            var identity = GetIdentityByPhone(model.Username, model.Password.Replace(" ",string.Empty));
            User user;

            //if null
            if (identity == null)
            {
                //log fail and return
                user = await _service.GetByPhone(model.Username);
                _systemEventService.LogUserLoginFailed(user?.Id, model.Username, model.Password, Request.HttpContext.Connection.RemoteIpAddress?.ToString());
                return BadRequest("Invalid Phone or code.");
            }

            var now = DateTime.UtcNow;
            // create JWT-token
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.Issuer,
                    audience: AuthOptions.Audience,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.Lifetime)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            user = await _service.GetByPhone(model.Username);
            var response = new LoginResultModel()
            {
                AccessToken = encodedJwt,
                Id = user.Id.ToString(),
                Email = user.Email,
                IsAdmin = user.AccountType == AccountType.Administration
            };

            _systemEventService.LogUserLoginSuccess(user?.Id, Request.HttpContext.Connection.RemoteIpAddress?.ToString());
            return Ok(response);
        }

        /// <summary>
        /// Create new user in Tramy
        /// </summary>
        /// <param name="email">User's email</param>
        /// <param name="firstName">User's first name</param>
        /// <param name="lastName">User's last name</param>
        /// <param name="password">User's password</param>
        /// <param name="gender">User's gender</param>
        /// <param name="accountType">User's account type. By default is AccountType.User</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("Register")]
        [SwaggerOperation(OperationId = "RegisterUser")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]

        public async Task<IActionResult> Register(string email, string firstName, string lastName, string password, Gender gender, AccountType accountType = AccountType.User)
        {
            //check if email exists
            var user = await _service.GetByEmail(email);
            //if exists - bad
            if (user != null)
                return BadRequest("Invalid username");
            //else create and insert user
            user = new User()
            {
                AccountType = accountType,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                Gender = gender,
                PasswordHash = UserService.CreateMd5(password),
                RegistrationDate = DateTime.Now,
                //статус перетащить в организацию
            };
            await _service.Insert(user);
            //TODO send Email about activation account
            return Ok();
        }

        /// <summary>
        /// Create new organization in Tramy
        /// </summary>
        /// <param name="email">Organization's email</param>
        /// <param name="name">Organization's name</param>
        /// <param name="fullName">Organization's full name</param>
        /// <param name="address">Organization's address</param>
        /// <param name="phone">Organization's phone number</param>
        /// <param name="organizationType">Organization's type</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("RegisterOrg")]
        [SwaggerOperation(OperationId = "RegisterOrganization")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.BadRequest)]

        public async Task<IActionResult> RegisterOrganization(string email, string name, string fullName, string address, string phone, OrganizationType organizationType)
        {
            //check if email exists
            var organization = await _orgService.GetByEmail(email);
            //if exists - bad
            if (organization != null)
                return BadRequest("Invalid email");

            //check if name exists
            organization = await _orgService.GetByName(name);
            if (organization != null)
                return BadRequest("Invalid name");

            //check if fullname exists
            organization = await _orgService.GetByFullname(fullName);
            if (organization != null)
                return BadRequest("Invalid FullName");

            //else create and insert organization
            organization = new Organization()
            {
                FullName = fullName,
                Name = name,
                Address = address,
                Email = email,
                Phone = phone,
                OrganizationType = organizationType,
                Status = OrganizationStatus.Active, //TODO change status to not active before activation by email or phone
            };

            //Registration new User type = organization
            await Register(email, name, fullName, "password", Gender.Other, AccountType.Organization);

            await _orgService.Insert(organization);
            //TODO send Email about activation account
            return Ok();
        }

        /// <summary>
        /// Get Identity of User if login and password correct
        /// </summary>
        /// <param name="username">User's email</param>
        /// <param name="password">User's password</param>
        /// <returns>Identity or null</returns>
        private ClaimsIdentity GetIdentity(string username, string password)
        {
            try
            {
                var user = _service.Auth(username, password);
                if (user == null) return null;
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                    new Claim("Id", user.Id.ToString())
                };
                var claimsIdentity =
                    new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                        ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }
            catch (Exception e)
            {
                return null;
            }
         
        }

        private ClaimsIdentity GetIdentityByPhone(string phone, string code)
        {
            try
            {
                var user = _service.AuthByPhone(phone, code);
                if (user == null) return null;
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.Phone)
                };
                var claimsIdentity =
                    new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                        ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }
            catch (Exception e)
            {
                return null;
            }

        }
    }
}

