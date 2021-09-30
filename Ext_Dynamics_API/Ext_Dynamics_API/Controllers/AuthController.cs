using Ext_Dynamics_API.DataAccess;
using Ext_Dynamics_API.RequestModels;
using Ext_Dynamics_API.ResponseModels;
using Ext_Dynamics_API.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using Scrypt;
using Ext_Dynamics_API.Configuration.Models;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Ext_Dynamics_API.Controllers
{
    [Route("api/TokenAuthController")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ExtensibleDbContext _dbCtx;
        private readonly TokenManager _tokenManager;
        private readonly ScryptEncoder _scryptHasher; // Using the SCRYPT hashing algorithm 
        private readonly SystemConfig _config;

        public AuthController(ExtensibleDbContext dbCtx)
        {
            _dbCtx = dbCtx;
            _config = SystemConfig.LoadConfig();
            _tokenManager = new TokenManager(_dbCtx, _config);
            _scryptHasher = new ScryptEncoder();
        }

        /// <summary>
        /// Authenticates a user and issues a JWT token in encoded format to the user
        /// </summary>
        /// <param name="credentials">The login credentials</param>
        /// <returns>AuthResponse: Details the result of the authentication attempt and issues a JWT token if successful</returns>
        [HttpPost]
        [Route("LoginUser")]
        public IActionResult LoginUser([FromBody] LoginCredentials credentials)
        {
            var user = _dbCtx.UserAccounts.Where(x => x.AppUserName.Equals(credentials.Username)).FirstOrDefault();
            var response = new AuthResponse();
            if(user == null)
            {
                response.ResponseMessage = "Username is Incorrect or non-existent";
                return new UnauthorizedObjectResult(response);
            }
            if(!_scryptHasher.Compare(credentials.Password, user.UserPassword))
            {
                response.ResponseMessage = "Password is Incorrect";
                return new UnauthorizedObjectResult(response);
            }
            var token = _tokenManager.IssueToken(user);
            if(token.Equals(""))
            {
                response.ResponseMessage = "Failed to generate Authentication token";
                return new NotFoundObjectResult(response);
            }
            response.ResponseMessage = "Successful Login";
            response.ResponseToken = token;
            return new OkObjectResult(response);

        }

        /// <summary>
        /// Logs out the user, deleting and invalidating all user tokens related to that user
        /// </summary>
        /// <returns>AuthResponse: Details the result of the logout attempt</returns>
        [Authorize]
        [HttpPost]
        [Route("LogoutUser")]
        public IActionResult LogoutUser()
        {
            var encodedToken = _tokenManager.ReadAndValidateToken(Request.Headers[_config.authHeader]);
            var handler = new JwtSecurityTokenHandler();
            JwtSecurityToken decodedToken;

            try
            {
                decodedToken = handler.ReadJwtToken(encodedToken);
            }
            catch (ArgumentException)
            {
                return new UnauthorizedObjectResult(new AuthResponse() { 
                    ResponseMessage = "User Token Is Not Valid"
                });
            }

            if (_tokenManager.IsTokenValid(encodedToken))
            {
                var userId = _tokenManager.GetUserIdFromToken(decodedToken);
                if(userId != -1)
                {
                    _tokenManager.DeleteUserTokens(userId);
                    return new OkObjectResult(new AuthResponse() { 
                        ResponseMessage = "Successful Logout"
                    });
                }
                return new NotFoundObjectResult(new AuthResponse() { 
                    ResponseMessage = "Failed to Find user"
                });
            }
            return new BadRequestObjectResult(new AuthResponse() { 
                ResponseMessage = "The Supplied Token is Invalid"
            });
        }

        /// <summary>
        /// Performs a Validation check on a supplied JWT in encoded format
        /// </summary>
        /// <returns>Boolean: Whether or not the token is valid</returns>
        [HttpGet]
        [Route("ValidateToken")]
        public IActionResult ValidateToken()
        {
            var encodedToken = _tokenManager.ReadAndValidateToken(Request.Headers[_config.authHeader]);
            return new OkObjectResult(!string.IsNullOrWhiteSpace(encodedToken));
        }
    }
}
