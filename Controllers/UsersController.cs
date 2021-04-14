using AutoMapper;
using BookStore.API.Contracts;
using BookStore.API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager; // sign-in/out
        private readonly UserManager<IdentityUser> _userManager; // user ops
        private readonly ILoggerService _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config; 

        public UsersController(SignInManager<IdentityUser> signInManager, 
            UserManager<IdentityUser> userManager,
            ILoggerService logger, IMapper mapper,
            IConfiguration config)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _mapper = mapper;
            _config = config;
        }

        /// <summary>
        /// User Registration Endpoint
        /// </summary>
        /// <param name="userDTO"></param>
        /// <returns></returns>
        [Route("register")]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] UserDTO userDTO)
        {
            var location = GetControllerActionNames();
            try
            {
                var username = userDTO.Username;
                _logger.LogInfo($"{location}: Registration attempt for {username}");
                var user = new IdentityUser()
                {
                    Email = username, UserName = username
                };
                var result = await _userManager.CreateAsync(user, userDTO.Password);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        _logger.LogError($"{location}: {error.Code} {error.Description}");
                    }
                    return InternalError($"{location}: {username} registration attempt failed");
                }
                return Ok(new { result.Succeeded });
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} {e.InnerException}");
            }
        }

        /// <summary>
        /// User Login Endpoint
        /// </summary>
        /// <param name="userDTO"></param>
        /// <returns></returns>
        [Route("login")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] UserDTO userDTO)
        {
            var location = GetControllerActionNames();
            try
            {
                var username = userDTO.Username;
                _logger.LogInfo($"{location}: Login attempt from User {username}");
                var result = await _signInManager.PasswordSignInAsync(username, userDTO.Password, false, false);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(username);
                    var tokenString = await GenerateJsonWebToken(user);
                    //var resultDTO = _mapper.Map<UserViewDTO>(user);
                    _logger.LogInfo($"{location}: {username} successfully authenticated");
                    //return Ok(new { user = resultDTO, token = tokenString} );
                    return Ok(new { token = tokenString }); // client should store & use this token
                }
                _logger.LogError($"{location}: {username} not authenticated"); // username or pwd incorrect
                return Unauthorized(userDTO); // 401
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} {e.InnerException}");
            }
        }

        // JSON Web Token (JWT) is a compact library for securely transmitting information between parties as a JSON object.
        // JWTs can be encrypted to also provide secrecy between parties, as signed tokens.
        // Strings that have encoded info about a user. Server authenticates i.e. validates that API request is from a legitimate source.
        // The JWT specification defines seven reserved claims that are recommended to allow interoperability with every application.
        // iss(issuer) : Issuer of the JWT
        // sub(subject) : Subject of the JWT (the user)
        // aud(audience) : Recipient for which the JWT is intended
        // exp(expiration time): Time after which the JWT expires; client will store that token until exp & then get a new one
        // nbf(not before time) : Time before which the JWT must not be accepted for processing
        // iat(issued at time): Time at which the JWT was issued; can be used to determine age of the JWT
        // jti(JWT ID): Unique identifier; can be used to prevent the JWT from being replayed (allows a token to be used only once)

        private async Task<string> GenerateJsonWebToken(IdentityUser user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"])); // from appsettings
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256); // hash
            var claims = new List<Claim> // claims/info to include in the JWT
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };
            var roles = await _userManager.GetRolesAsync(user); // get Role names
            claims.AddRange(roles.Select(r => new Claim(ClaimsIdentity.DefaultRoleClaimType, r))); // add roles to claims

            var token = new JwtSecurityToken(issuer: _config["Jwt:Issuer"], audience: _config["Jwt:Issuer"], 
                claims: claims, notBefore: null, expires: DateTime.Now.AddMinutes(5), signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GetControllerActionNames()
        {
            var controller = ControllerContext.ActionDescriptor.ControllerName;
            var action = ControllerContext.ActionDescriptor.ActionName;
            return $"{controller} - {action}";
        }

        private ObjectResult InternalError(string message)
        {
            _logger.LogError(message);
            return StatusCode(500, "Something went wrong. Please contact the Administrator");
        }


    }
}
