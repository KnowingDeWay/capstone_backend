using Ext_Dynamics_API.Canvas;
using Ext_Dynamics_API.Configuration.Models;
using Ext_Dynamics_API.DataAccess;
using Ext_Dynamics_API.Models;
using Ext_Dynamics_API.RequestModels;
using Ext_Dynamics_API.ResponseModels;
using Ext_Dynamics_API.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace Ext_Dynamics_API.Controllers
{
    [Route("api/CanvasAccessTokenController")]
    [ApiController]
    public class CanvasAccessTokenController : ControllerBase
    {
        private readonly ExtensibleDbContext _dbCtx;
        private readonly TokenManager _tokenManager;
        private readonly SystemConfig _config;
        private readonly CanvasTokenManager _canvasTokenManager;

        public CanvasAccessTokenController(ExtensibleDbContext dbCtx)
        {
            _dbCtx = dbCtx;
            _config = SystemConfig.LoadConfig();
            _tokenManager = new TokenManager(_dbCtx, _config);
            _canvasTokenManager = new CanvasTokenManager(_dbCtx);
        }

        /// <summary>
        /// Gets all Canvas Access Tokens pertaining to a user
        /// </summary>
        /// <returns>ListResponse&lt;CanvasPersonalAccessToken>: Returns the list of Canvas PATs with a feedback message</returns>
        [HttpGet]
        [Authorize]
        [Route("GetUserAccessTokens")]
        public IActionResult GetUserAccessTokens()
        {
            var encodedToken = _tokenManager.ReadAndValidateToken(Request.Headers[_config.authHeader]);
            var handler = new JwtSecurityTokenHandler();
            JwtSecurityToken decodedToken;

            var objResponse = new ListResponse<CanvasPersonalAccessToken>();
            try
            {
                decodedToken = handler.ReadJwtToken(encodedToken);
            }
            catch (ArgumentException)
            {
                objResponse.ResponseMessage = "User Token Is Not Valid";
                return new UnauthorizedObjectResult(objResponse);
            }

            var userId = _tokenManager.GetUserIdFromToken(decodedToken);

            if (userId == -1)
            {
                objResponse.ResponseMessage = "Failed to find User";
                return new NotFoundObjectResult(objResponse);
            }

            var list = _dbCtx.PersonalAccessTokens.Where(x => x.AppUserId == userId).ToList();

            objResponse.ResponseMessage = "";
            objResponse.ListContent = new List<CanvasPersonalAccessToken>();

            objResponse.ListContent.AddRange(list);

            return new OkObjectResult(objResponse);

        }

        /// <summary>
        /// Gets information on a specific Canvas PAT based on an id
        /// </summary>
        /// <param name="patId">The id of the PAT</param>
        /// <returns>
        /// ObjectResponse&lt;CanvasPersonalAccessToken>: The Canvas PAT along with feedback detailing the result of the
        /// attempted operation
        /// </returns>
        [HttpGet]
        [Authorize]
        [Route("GetUserAccessTokenById/{id}")]
        public IActionResult GetUserAccessTokenById([FromRoute] int patId)
        {
            var encodedToken = _tokenManager.ReadAndValidateToken(Request.Headers[_config.authHeader]);
            var handler = new JwtSecurityTokenHandler();
            JwtSecurityToken decodedToken;

            var objResponse = new ObjectResponse<CanvasPersonalAccessToken>();
            try
            {
                decodedToken = handler.ReadJwtToken(encodedToken);
            }
            catch (ArgumentException)
            {
                objResponse.Message = "User Token Is Not Valid";
                return new UnauthorizedObjectResult(objResponse);
            }

            var userId = _tokenManager.GetUserIdFromToken(decodedToken);

            if (userId == -1)
            {
                objResponse.Message = "Failed to find User";
                return new NotFoundObjectResult(objResponse);
            }

            var pat = _dbCtx.PersonalAccessTokens.Where(x => x.Id == patId).FirstOrDefault();

            if(pat == null)
            {
                objResponse.Message = "Failed to find token";
                return new NotFoundObjectResult(objResponse);
            }

            if(pat.AppUserId != userId)
            {
                objResponse.Message = "You cannot view this token";
                return new NotFoundObjectResult(objResponse);
            }

            objResponse.Message = "Successfully retreived personal access token";
            objResponse.Value = pat;

            return new OkObjectResult(objResponse);
        }

        /// <summary>
        /// Adds a new Canvas PAT to the system
        /// </summary>
        /// <param name="canvasToken">Information detaling the new PAT</param>
        /// <returns>String: The result of the attempted operation</returns>
        [HttpPost]
        [Authorize]
        [Route("AddCanvasToken")]
        public IActionResult AddCanvasToken([FromBody] CanvasToken canvasToken)
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
                return new UnauthorizedObjectResult("User Token Is Not Valid");
            }

            var userId = _tokenManager.GetUserIdFromToken(decodedToken);

            if(userId == -1)
            {
                return new NotFoundObjectResult("Failed to find User");
            }

            var pat = new CanvasPersonalAccessToken()
            {
                TokenName = canvasToken.TokenName,
                AccessToken = canvasToken.ApiKey,
                AppUserId = userId,
                TokenActive = false
            };

            _dbCtx.PersonalAccessTokens.Add(pat);

            try
            {
                _dbCtx.SaveChanges();
                return new OkObjectResult($"Canvas token: {pat.TokenName} was successfully added!");
            }
            catch (Exception)
            {
                // Rollback token addition
                _dbCtx.PersonalAccessTokens.Remove(pat);
                return new NotFoundObjectResult("Failed to Add Canvas Token");
            }
        }

        /// <summary>
        /// Updates information pertaining to a specific Canvas PAT
        /// </summary>
        /// <param name="canvasToken">The updated details of the Canvas PAT</param>
        /// <returns>String: The result of the attempted operation</returns>
        [HttpPut]
        [Authorize]
        [Route("EditAccessToken")]
        public IActionResult EditAccessToken([FromBody] CanvasPersonalAccessToken canvasToken)
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
                return new UnauthorizedObjectResult("User Token Is Not Valid");
            }

            var userId = _tokenManager.GetUserIdFromToken(decodedToken);

            if(userId == canvasToken.AppUserId)
            {
                if(_canvasTokenManager.UpdateCanvasPat(canvasToken))
                {
                    return new OkObjectResult($"Successfully Updated token: {canvasToken.TokenName}");
                }
                else
                {
                    return new BadRequestObjectResult("An error occured while trying to save the token");
                }
            }
            else
            {
                return new UnauthorizedObjectResult("You do not have permission to edit this token");
            }
        }

        /// <summary>
        /// Deletes a specific Canvas PAT for a user
        /// </summary>
        /// <param name="patId">The id of the PAT to delete</param>
        /// <returns>String: The result of the attempted operation</returns>
        [HttpDelete]
        [Authorize]
        [Route("DeleteAccessToken/{patId}")]
        public IActionResult DeleteAccessToken([FromRoute] int patId)
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
                return new UnauthorizedObjectResult("User Token Is Not Valid");
            }

            var userId = _tokenManager.GetUserIdFromToken(decodedToken);

            var canvasToken = _dbCtx.PersonalAccessTokens.Where(x => x.Id == patId).FirstOrDefault();

            if(canvasToken == null)
            {
                return new NotFoundObjectResult($"Could not find token with id: {patId}");
            }

            if (userId == canvasToken.AppUserId)
            {
                var tokenToDelete = _dbCtx.PersonalAccessTokens.Where(x => x.Id == canvasToken.Id).FirstOrDefault();

                if(tokenToDelete != null)
                {
                    _dbCtx.PersonalAccessTokens.Remove(tokenToDelete);
                    _dbCtx.SaveChanges();
                    return new OkObjectResult($"Successfully deleted token with name: {canvasToken.TokenName}");
                }
                else
                {
                    return new BadRequestObjectResult("Cannot find the token to delete");
                }
            }
            else
            {
                return new UnauthorizedObjectResult("You do not have permission to edit this token");
            }
        }

        /// <summary>
        /// Activates a specific Canvas PAT for a user; this PAT will be used for all interactions between the system and Canvas for
        /// this particular user
        /// </summary>
        /// <param name="patId">The id of the PAT to activate</param>
        /// <returns>String: The result of the attempted operation</returns>
        [HttpPost]
        [Authorize]
        [Route("ActivateAccessToken/{patId}")]
        public IActionResult ActivateAccessToken([FromRoute] int patId)
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
                return new UnauthorizedObjectResult("User Token Is Not Valid");
            }

            var userId = _tokenManager.GetUserIdFromToken(decodedToken);

            var canvasToken = _dbCtx.PersonalAccessTokens.Where(x => x.Id == patId).FirstOrDefault();

            if (canvasToken == null)
            {
                return new NotFoundObjectResult($"Could not find token with id: {patId}");
            }

            if (userId == canvasToken.AppUserId)
            {
                if(canvasToken.TokenActive)
                {
                    return new BadRequestObjectResult("Token Is Already Activated");
                }
                if (_canvasTokenManager.ActivateToken(patId, userId)) 
                {
                    return new OkObjectResult("Token has Been Activated");
                }
                else
                {
                    return new BadRequestObjectResult("An error occured while trying to activate the token");
                }
            }
            else
            {
                return new UnauthorizedObjectResult("You do not have permission to edit this token");
            }
        }
    }
}
