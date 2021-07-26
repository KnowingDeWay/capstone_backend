using Ext_Dynamics_API.Canvas;
using Ext_Dynamics_API.Configuration.Models;
using Ext_Dynamics_API.DataAccess;
using Ext_Dynamics_API.Models;
using Ext_Dynamics_API.RequestModels;
using Ext_Dynamics_API.ResponseModels;
using Ext_Dynamics_API.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

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

        [HttpGet]
        [Authorize]
        [Route("GetUserAccessTokens")]
        public ActionResult GetUserAccessTokens()
        {
            var encodedToken = _tokenManager.ReadToken(Request.Headers[_config.authHeader]);
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

            objResponse.ResponseMessage = "Successfully retreived personal access tokens";
            objResponse.ListContent = new List<CanvasPersonalAccessToken>();

            objResponse.ListContent.AddRange(list);

            return new OkObjectResult(objResponse);

        }

        [HttpGet]
        [Authorize]
        [Route("GetUserAccessToken/{id}")]
        public ActionResult GetUserAccessToken([FromRoute] int patId)
        {
            var encodedToken = _tokenManager.ReadToken(Request.Headers[_config.authHeader]);
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

            objResponse.Message = "Successfully retreived personal access tokens";
            objResponse.Value = pat;

            return new OkObjectResult(objResponse);
        }

        [HttpPost]
        [Authorize]
        [Route("AddCanvasToken")]
        public ActionResult AddCanvasToken([FromBody] CanvasToken canvasToken)
        {
            var encodedToken = _tokenManager.ReadToken(Request.Headers[_config.authHeader]);
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
                AccessToken = canvasToken.EncodedToken,
                AppUserId = userId
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
                return new NotFoundObjectResult("Failed to Delete User Profile");
            }
        }

        [HttpPut]
        [Authorize]
        [Route("EditAccessToken")]
        public ActionResult EditAccessToken([FromBody] CanvasPersonalAccessToken canvasToken)
        {
            var encodedToken = _tokenManager.ReadToken(Request.Headers[_config.authHeader]);
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

        [HttpDelete]
        [Authorize]
        [Route("DeleteAccessToken")]
        public ActionResult DeleteAccessToken([FromBody] CanvasPersonalAccessToken canvasToken)
        {
            var encodedToken = _tokenManager.ReadToken(Request.Headers[_config.authHeader]);
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

    }
}
